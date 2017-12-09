//tabs=4
// --------------------------------------------------------------------------------
// ASCOM Telescope driver for LX90 State Machine.
//
// GoF State pattern is used in protocols to prevent invalid state transitions
// due to operations being requested that are not valid for any given state.
//
#define TelescopeDriverState

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;
using System.Collections.Concurrent;

namespace ASCOM.LX90
{
   /////////////////////////////////////////////////////////////////////////
   // This region holds the driver State machine.
   #region Patched497StateMachine;

   /// <summary>
   /// All state requests and state changing operations come through here
   /// and we ensure that no interleaved serial coms can happen for starters.
   /// The state machine executes in here and therefore requires no locks AND
   /// prevents all manner of invalid state transitions or operations invalid
   /// on any given state.
   /// </summary>
   public class Patched497Queue
   {
      private static Patched497Queue instance = null;
      public static Patched497Queue Instance
      {
         get
         {
            if (instance == null)
            {
               instance = new Patched497Queue();
            }
            return instance;
         }
      }

      public ConcurrentQueue<Action> WorkQueue = new ConcurrentQueue<Action>();
      private Patched497Queue()
      {
         Task.Factory.StartNew(() =>
         {
            Action func;
            while (true)
            {
               if (WorkQueue.TryDequeue(out func))
               {
                  try
                  {
                     func.Invoke();
                  }
                  catch (Exception e)
                  {
                     Telescope.LogMessage("Exception in HBX work queue thread - \n", e.Message);
                  }
               }
               else
               {
                  System.Threading.Thread.Sleep(1);
               }
            }
         });
      }
   }

   ///////////////////////////////////////////////////////////////////
   /// <summary>
   /// All state transitions are found in this GoF state pattern.
   /// Current state is set atomically.
   /// State objects only implement the operations that are valid
   /// for them and these methods return the next state otherwise
   /// they return themselves (no state change) and protect against the
   /// state becoming corrupt.
   /// 
   /// Per GoF State pattern. This base class provides do nothing versions
   /// of all operations. Subclasses need only implement the state transitioning
   /// operations that are valid for the state they represent. Thus any request
   /// to do something that ought to be impossible for a given state is a no-op.
   /// </summary>
   public abstract class DriverStateBase
   {
      // Master as in all connect, disconnect start here.
      public static DriverStateBase MasterCurrentState = null;
      // Secondary axis for independent axis slewing and guiding.
      public static DriverStateBase SecondaryAxisCurrentState = null;

      // Property setting understands whether the state applies to a single axis
      // or dual axes.
      public static DriverStateBase CurrentState
      {
         set
         {
            // MasterCurrentState is NEVER null and not nullable.
            // We either tie the axes together or separate them
            // if we are moving them independently.
            if (value != null)
            {
               if (value.IsDualAxisState() || value.IsPrimaryAxisState())
               {
                  MasterCurrentState = value;
               }
               else // In the case value is a secondary axis state.
               {
                  SecondaryAxisCurrentState = value;
               }
            }
            else
            {
               throw new ASCOM.InvalidValueException("State for axes cannot be null.");
            }
         }
      }

      protected static ASCOM.Utilities.Serial serialPort = null;
      protected Util utilities = null;
      public DriverStateBase() 
      {
         utilities = new Util();
      }
      public virtual DriverStateBase Connect(int Port)
      {
         return this;
      }
      public virtual DriverStateBase Disconnect()
      {
         return this;
      }
      public virtual DriverStateBase Park()
      {
         return this;
      }
      public virtual DriverStateBase SlewComplete()
      {
         return this;
      }
      internal virtual DriverStateBase MoveAxisNorth(Rate rate)
      {
         return this;
      }
      internal virtual DriverStateBase MoveAxisSouth(Rate rate)
      {
         return this;
      }
      internal virtual DriverStateBase MoveAxisEast(Rate rate)
      {
         return this;
      }
      internal virtual DriverStateBase MoveAxisWest(Rate rate)
      {
         return this;
      }
      public virtual DriverStateBase MoveAxis(TelescopeAxes Axis, double rate)
      {
         return this;
      }
      internal virtual DriverStateBase InternalStopMoveAxis()   
      {
         return this;
      }
      public virtual DriverStateBase StopMoveAxis()
      {
         return this;
      }
      public virtual DriverStateBase StopMoveAxis(TelescopeAxes axis)
      {
         return this;
      }
      internal virtual DriverStateBase PulseGuideNorth(int durationMs)
      {
         return this;
      }
      internal virtual DriverStateBase PulseGuideSouth(int durationMs)
      {
         return this;
      }
      internal virtual DriverStateBase PulseGuideEast(int durationMs)
      {
         return this;
      }
      internal virtual DriverStateBase PulseGuideWest(int durationMs)
      {
         return this;
      }
      public virtual DriverStateBase PulseGuide(GuideDirections Direction, int Duration)
      {
         return this;
      }
      internal virtual DriverStateBase InternalPulseGuideComplete()
      {
         return this;
      }
      public virtual DriverStateBase AbortSlew()
      {
         return this;
      }
      public virtual DriverStateBase TrackSidereal()
      {
         return this;
      }
      public virtual DriverStateBase TrackLunar()
      {
         return this;
      }
      public virtual DriverStateBase SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         return this;
      }
      public virtual DriverStateBase SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         return this;
      }
      public virtual DriverStateBase SyncToAltAz(double Az, double Alt)
      {
         return this;
      }
      public virtual DriverStateBase SyncToCoordinates(double RA, double Dec)
      {
         return this;
      }
      public virtual DriverStateBase ResumeTracking()
      {
         return this;
      }
      public virtual bool IsParked() { return false; }
      internal virtual bool InternalIsSlewing() { return false; }
      public virtual bool IsSlewing() { return InternalIsSlewing(); }
      internal virtual bool InternalIsPulseGuiding() { return false; }
      public virtual bool IsPulseGuiding() { return InternalIsPulseGuiding(); }
      public virtual bool IsConnected() { return false; }
      internal virtual bool InternalIsHwTracking() { return false; }
      public virtual bool IsTracking() { return false; }
      public virtual bool IsTrackingSidereal() { return false; }
      public virtual bool IsTrackingLunar() { return false; }
      // What does the mount say about its slewing state?
      internal virtual bool InternalIsHwSlewing() { return false; }
      public virtual bool IsDualAxisState() { return !IsPrimaryAxisState() && !IsSecondaryAxisState(); }
      public virtual bool IsPrimaryAxisState() { return false; }
      public virtual bool IsSecondaryAxisState() { return false; }

      public virtual AlignmentModes AlignmentMode()
      {
         throw new ASCOM.NotConnectedException("Requested AlignmentMode of disconnected mount.");
      }

      public virtual double Altitude()
      {
         throw new ASCOM.NotConnectedException("Requested Altitude of disconnected mount.");
      }

      public virtual double Azimuth()
      {
         throw new ASCOM.NotConnectedException("Requested Azimuth of disconnected mount.");
      }

      public virtual double Declination()
      {
         throw new ASCOM.NotConnectedException("Requested Declination of disconnected mount.");
      }

      public virtual double RightAscension()
      {
         throw new ASCOM.NotConnectedException("Requested Right Ascension of disconnected mount.");
      }

      public virtual double DeclinationRate()
      {
         return 0.0; // Tracking offrset from sidereal is nothing.
      }

      public virtual double RightAscensionRate()
      {
         return 0.0; // Tracking offrset from sidereal is nothing.
      }

      public virtual double SiderealTime()
      {
         throw new ASCOM.NotConnectedException("Requested Sidereal Time of disconnected mount.");
      }

      public virtual double SiteLatitude()
      {
         throw new ASCOM.NotConnectedException("Requested Site Latitude of disconnected mount.");
      }

      public virtual void SetSiteLatitude(double newLat)
      {
         throw new ASCOM.NotConnectedException("Attempted to set Site Latitude of disconnected mount.");
      }

      public virtual double SiteLongitude()
      {
         throw new ASCOM.NotConnectedException("Requested Site Longitude of disconnected mount.");
      }

      public virtual void SetSiteLongitude(double newLon)
      {
         throw new ASCOM.NotConnectedException("Attempted to set Site Longitude of disconnected mount.");
      }

      public virtual DateTime UTCDate()
      {
         throw new ASCOM.NotConnectedException("Attempted to get UTC date of disconnected mount.");
      }
   }

   public abstract partial class ConnectedState : DriverStateBase { }
   public abstract partial class TrackingState : ConnectedState { }
   public abstract partial class PrimaryAxisTrackingState { }
   public partial class SiderealTrackingState : PrimaryAxisTrackingState { }
   public partial class LunarTrackingState : PrimaryAxisTrackingState { }
   public partial class SecondaryAxisTrackingState : TrackingState { }

   /// <summary>
   /// This simple factory class ensures we match what the mount says its selected tracking
   /// rate is.
   /// </summary>
   public class TrackingStateHelper
   {
      public static DriverStateBase TrackingStateForMount(ASCOM.Utilities.Serial serialPort)
      {
         serialPort.Transmit(":Q#");
         serialPort.Transmit(":GT#");
         string trackingRateStr = serialPort.ReceiveTerminated("#").Replace("#", "");
         double trackingRate = double.Parse(trackingRateStr);
         // From what I can see, some fuzziness needs to be applied.
         if (trackingRate > 58 && trackingRate < 62)
         {
            return new SiderealTrackingState();
         }
         else if (trackingRate <= 58)
         {
            return new LunarTrackingState();
         }
         else
         {
            // Throw up our hands because we don't have a clue
            throw new ASCOM.InvalidValueException("Unrecognized tracking rate: " + trackingRateStr + " Hz.");
         }
      }
   }

   /// <summary>
   /// Disconnected state is the initial state.
   /// Valid state transitions:
   ///    DisconnectedState->Connect(Port)->TrackingState on both axes.
   /// </summary>
   public class DisconnectedState : DriverStateBase
   {
      public override DriverStateBase Connect(int Port)
      {
         try
         {
            serialPort = new ASCOM.Utilities.Serial();
            serialPort.Port = Port;
            // To do. Cycle through the baud rates until we get a valid response from the
            // Mount.
            serialPort.Speed = SerialSpeed.ps9600;
            serialPort.Parity = SerialParity.None;
            serialPort.DataBits = 8;
            serialPort.Connected = true;
            MasterCurrentState = TrackingStateHelper.TrackingStateForMount(serialPort);
            SecondaryAxisCurrentState = new SecondaryAxisTrackingState();
            return MasterCurrentState;
         }
         catch(Exception e)
         {
            if (serialPort != null)
            {
               serialPort.Connected = false;
               serialPort.Dispose();
               serialPort = null;
            }
            throw e;
            //return this;
         }
      }
   }

   /// <summary>
   /// ParkedState is a special disconnected state. IsParked() will always return true.
   /// Valid state transitions:
   ///    ParkedState->Connect(Port)->TrackingState on both axes.
   /// </summary>
   public class ParkedState : DisconnectedState
   {
      public override bool IsParked() { return true; }
   }

   /// <summary>
   /// ConnectedState is the generalisation for all connected states. It provides all the methods
   /// to get state related to site, coordinates and so on from the mount.
   /// Valid state transitions:
   ///    ConnectedState->Park()->ParkedState.
   ///    ConnectedState->Disconnect()->DisconnectedState.
   /// </summary>
   public abstract partial class ConnectedState : DriverStateBase
   {
      public override DriverStateBase Disconnect()
      {
         if (serialPort != null)
         {
            serialPort.Connected = false;
            serialPort.Dispose();
            serialPort = null;
         }

         return new DisconnectedState();
      }

      public override DriverStateBase Park()
      {
         // Safe noop if we are not slewing.
         MasterCurrentState = MasterCurrentState.AbortSlew();
         if (SecondaryAxisCurrentState != null)
         {
            SecondaryAxisCurrentState.AbortSlew();
            SecondaryAxisCurrentState = null;
         }
         serialPort.Transmit(":hP#");
         // Disconnect.
         MasterCurrentState = MasterCurrentState.Disconnect();
         // Return the parked state.
         return new ParkedState();
      }

      public override bool IsConnected() { return true; }

      // Any connected state can tell you about mount position, date, time, etc.
      public override AlignmentModes AlignmentMode()
      {
         serialPort.Transmit(":GW#");
         string alignment = serialPort.ReceiveCounted(3);
         if (alignment.StartsWith("A"))
            return AlignmentModes.algAltAz;
         else if (alignment.StartsWith("P"))
            return AlignmentModes.algPolar;
         else
            throw new ASCOM.DriverException("Unsupported alignment returned by the mount: " + alignment);
      }

      public override double Altitude()
      {
         serialPort.Transmit(":GA#");
         string altitude = serialPort.ReceiveTerminated("#").Replace("#", "");
         // Can return sDD*MM# or sDD*MM'SS#
         altitude = altitude.Replace('*', ':').Replace('°', ':').Replace("ß", ":");
         altitude = altitude.Replace('\'', ':');
         if (altitude.Length == 6)
            altitude += ":00";
         return utilities.DMSToDegrees(altitude);
      }

      public override double Azimuth()
      {
         serialPort.Transmit(":GZ#");
         string azimuth = serialPort.ReceiveTerminated("#").Replace("#", "");
         // Can return DDD*MM# or DDD*MM'SS#
         azimuth = azimuth.Replace('*', ':').Replace('°', ':').Replace("ß", ":");
         azimuth = azimuth.Replace('\'', ':');
         if (azimuth.Length == 6)
            azimuth += ":00";
         return utilities.DMSToDegrees(azimuth);
      }

      public override double Declination()
      {
         serialPort.Transmit(":GD#");
         string decl = serialPort.ReceiveTerminated("#").Replace("#", "");
         // Can return sDD*MM# or sDD*MM'SS#
         decl = decl.Replace('*', ':').Replace('°', ':').Replace("ß", ":");
         decl = decl.Replace('\'', ':');
         if (decl.Length == 6)
            decl += ":00";
         return utilities.DMSToDegrees(decl);
      }

      public override double RightAscension()
      {
         serialPort.Transmit(":GR#");
         string ra = serialPort.ReceiveTerminated("#").Replace("#", "");
         if (ra.Contains("."))
         {
            uint t = uint.Parse(ra.Substring(6, 1)) * 60;
            ra = ra.Substring(0, 5) + ":" + t.ToString("D2");
         }
         return utilities.HMSToHours(ra);
      }

      // Let any connected state ask if the mount is REALLY slewing or not.
      // Only ever call from somewhere already in the Patched497Queue.
      internal override bool InternalIsHwSlewing()
      {
         serialPort.Transmit(":D#");
         string indicator = serialPort.ReceiveTerminated("#").Replace("#", "");
         bool hwSlewing = indicator.Length > 0;
         return hwSlewing;
      }

      // Let any connected state ask if the mount is REALLY slewing or not.
      // Only ever call from somewhere already in the Patched497Queue.
      internal override bool InternalIsHwTracking()
      {
         serialPort.Transmit(":GW#");
         string status = serialPort.ReceiveTerminated("#").Replace("#", "");
         return status.Contains("<T>");
      }

      private double lastGoodSiderealTime = 0.0;
      public override double SiderealTime()
      {
         try
         {
            double siderealTime;
            serialPort.Transmit(":GS#");
            string sidereal = serialPort.ReceiveTerminated("#").Replace("#", "");
            int hours = int.Parse(sidereal.Substring(0, 2));
            int min = int.Parse(sidereal.Substring(3, 2));
            int sec = int.Parse(sidereal.Substring(6, 2));
            siderealTime = hours + (min / 60.0) + (sec / 3600.0);
            lastGoodSiderealTime = siderealTime;
            return lastGoodSiderealTime;
         }
         catch(Exception)
         {
            return lastGoodSiderealTime;
         }
      }

      public override double SiteLatitude()
      {
         serialPort.Transmit(":Gt#");
         string lat = serialPort.ReceiveTerminated("#").Replace("#", "");
         // sDD*MM#
         lat = lat.Replace('*', ':').Replace('°', ':').Replace("ß", ":");
         if (lat.Length < 9)
            lat += ":00";
         return utilities.DMSToDegrees(lat);
      }

      public override void SetSiteLatitude(double newLat)
      {
         serialPort.Transmit(":St" + utilities.DegreesToDM(newLat, "*", "") + "#");
         byte[] result = serialPort.ReceiveCountedBinary(1);
         // Is it a flag or a char? Allow for either.
         if (result[0] != 1 && !result.ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount did not accept " + newLat.ToString() + "(" + utilities.DegreesToDM(newLat, "*", "") + ") as a valid latitude.");
         }
      }

      public override double SiteLongitude()
      {
         serialPort.Transmit(":Gg#");
         string lon = serialPort.ReceiveTerminated("#").Replace("#", "");
         // Returns sDDD*MM#
         lon = lon.Replace('*', ':').Replace('°', ':').Replace("ß", ":");
         lon += ":00";
         return utilities.DMSToDegrees(lon);
      }

      public override void SetSiteLongitude(double newLon)
      {
         serialPort.Transmit(":Sg" + utilities.DegreesToDM(newLon, "°", "") + "#");
         byte[] result = serialPort.ReceiveCountedBinary(1);
         // Is it a flag or a char? Allow for either.
         if (result[0] != 1 && !result.ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount did not accept " + newLon.ToString() + "(" + utilities.DegreesToDM(newLon, "*", "") + ") as a valid longitude.");
         }
      }

      public override DriverStateBase TrackLunar()
      {
         AbortSlew();
         return new LunarTrackingState();
      }

      public override DriverStateBase TrackSidereal()
      {
         AbortSlew();
         return new SiderealTrackingState();
      }

      public override DateTime UTCDate()
      {
         // Get the mount local time. Add the UTC offset to get UTC time.
         try
         {
            serialPort.Transmit(":GC#");
            string localDate = serialPort.ReceiveTerminated("#").Replace("#", "");
            serialPort.Transmit(":GL#");
            string localTime = serialPort.ReceiveTerminated("#").Replace("#", "");
            serialPort.Transmit(":GG#");
            string utcOffset = serialPort.ReceiveTerminated("#").Replace("#", "");
            DateTime dt = DateTime.Parse(localDate, new CultureInfo("en-US"));
            dt = dt.AddHours(double.Parse(localTime.Substring(0, 2)))
               .AddMinutes(double.Parse(localTime.Substring(3, 2)))
               .AddSeconds(double.Parse(localTime.Substring(6, 2)));
            DateTimeOffset offset = new DateTimeOffset(dt, new TimeSpan(int.Parse(utcOffset), 0, 0));
            return offset.DateTime.ToUniversalTime();
         }
         catch(Exception)
         {
            return DateTime.UtcNow;
         }
      }
   }

   /// <summary>
   /// Represents any state where we are moving the scope but not tracking. i.e. Slew, pulse guide, etc.
   /// Valid state transitions are:
   ///    ScopeMovingState->ResumeTracking()->TrackingState/SecondaryAxisTrackingState
   /// </summary>
   public abstract class ScopeMovingState : ConnectedState
   {
      protected DriverStateBase SavedAxisState = null; // So we can return to tracking/quiet state easily.
      internal Task<bool> HandleToCurrentScopeMoveTask = null; // So we can have a simple way of doing different slews.
      private CancellationTokenSource scopeMovementCancellationTokenSource = null; // So we can signal cancellation of slews to HandleToCurrentSlewTask.
      protected CancellationTokenSource ScopeMovementCancellationTokenSource
      {
         get
         {
            return scopeMovementCancellationTokenSource;
         }
         set
         {
            if (scopeMovementCancellationTokenSource != null)
            {
               scopeMovementCancellationTokenSource.Dispose();
               scopeMovementCancellationTokenSource = null;
            }
            if (value != null)
            {
               scopeMovementCancellationTokenSource = value;
            }
         }
      }
      public void Dispose()
      {
         if (HandleToCurrentScopeMoveTask != null)
         {
            HandleToCurrentScopeMoveTask.Dispose();
            HandleToCurrentScopeMoveTask = null;
         }
      }
      public override DriverStateBase ResumeTracking()
      {
         return SavedAxisState.ResumeTracking();
      }

      // This is kind of weird because axes can act independently but we report slewing as a global thing.
      public override bool IsSlewing() 
      {
         return MasterCurrentState.InternalIsSlewing() || SecondaryAxisCurrentState.InternalIsSlewing();
      }

      // This is kind of weird because axes can act independently but we report pulse guiding as a global thing.
      public override bool IsPulseGuiding()
      {
         return MasterCurrentState.InternalIsPulseGuiding() || SecondaryAxisCurrentState.InternalIsPulseGuiding();
      }
   }

   /// <summary>
   /// Moving state on the primary axis that saves away the MasterCurrentState as the 
   /// state to return to when ResumeTracking() is called.
   /// </summary>
   public abstract class PrimaryAxisMovingState : ScopeMovingState
   {
      public PrimaryAxisMovingState()
      {
         SavedAxisState = MasterCurrentState;
      }
      public override bool IsPrimaryAxisState() { return true; }
   }

   /// <summary>
   /// Moving state on the secondary axis that saves away the SeconaryAxisCurrentState as the 
   /// state to return to when ResumeTracking() is called.
   /// </summary>
   public abstract class SecondaryAxisMovingState : ScopeMovingState
   {
      public SecondaryAxisMovingState()
      {
         SavedAxisState = SecondaryAxisCurrentState;
      }
      public override bool IsSecondaryAxisState() { return true; }
   }

   /// <summary>
   /// Encapsulates SlewToAltAz(...) and SlewToCoordinates(...) type slewing where both
   /// axes act in unison. It takes over the two axes by setting itself as the state for both.
   ///    DualAxisSlewingState->AbortSlew()->Tracking and SecondaryAxisTrackingState.
   ///    DualAxisSlewingState->SlewComplete()->Tracking and SecondaryAxisTrackingState.
   /// </summary>
   public abstract class DualAxisSlewingState : ScopeMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      // Need to preserve the secondary axis state too.
      private DriverStateBase SecondaryAxisState;
      public DualAxisSlewingState()
      {
         SavedAxisState = MasterCurrentState;
         SecondaryAxisState = SecondaryAxisCurrentState;
         // We will occupy both axes... we're greedy like that.
         SecondaryAxisCurrentState = this;
      }
      public override DriverStateBase AbortSlew()
      {
         if (HandleToCurrentScopeMoveTask != null)
         {
            ScopeMovementCancellationTokenSource.Cancel();
         }
         // The abort slew will hopefully cause slews waited on to finish
         // Dual axis slew so kill slews in both axes.
         serialPort.Transmit(":Q#");

         return SlewComplete();
      }
      public override DriverStateBase SlewComplete()
      {
         SecondaryAxisCurrentState = SecondaryAxisState.ResumeTracking();
         return SavedAxisState.ResumeTracking();
      }
   }

   /// <summary>
   /// Some static utiltity functions to help us with the move axis serial commands and creating
   /// the correct state for the axis and rate given.
   /// </summary>
   public partial class MoveAxisHelper
   {
      // Convert the rate on the axis to a Meade serial protocol axis rate command.
      public static string RateToRateCommandString(TelescopeAxes Axis, Rate rate)
      {
         if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.Sidereal))
         {
            return ":RG#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.Siderealx2))
         {
            return ":RC#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.SlewThreeDegreePerSec))
         {
            return ":RM#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.SlewSixPointFiveDegreePerSec))
         {
            return ":RS#";
         }
         else if (rate.Minimum > ASCOM.LX90.AxisRates.Siderealx2 && rate.Minimum < ASCOM.LX90.AxisRates.SlewSixPointFiveDegreePerSec)
         {
            if (Axis == TelescopeAxes.axisPrimary)
            {
               return ":RA" + String.Format("{0:0.0####;-0.0####}", Telescope.customRateReverseLR ? -rate.Minimum : rate.Minimum) + "#";
            }
            else
            {
               return ":RE" + String.Format("{0:0.0####;-0.0####}", Telescope.customRateReverseUD ? -rate.Minimum : rate.Minimum) + "#";
            }
         }
         else
         {
            // Fallback rate...
            return ":RS#";
         }
      }

      // State the telescope slewing at the axis current set slew rate.
      // Slews westOrNorth if true, else eastOrSouth if false.
      public static string AxisToMovementDirectionCommandString(TelescopeAxes Axis, bool westOrNorth)
      {
         if (Axis == TelescopeAxes.axisPrimary) // RA/Az
         {
            return ":M" + (westOrNorth ? "w" : "e") + "#";
         }
         else
         {
            return ":M" + (westOrNorth ? "n" : "s") + "#";
         }
      }

   }

   /// <summary>
   /// Single axis state for slew on the primary axis. Returns to saved MasterCurrentState when done or aborted.
   /// Provides a specialised AbortSlew that only aborts the slew on the moving E/W axis.
   /// </summary>
   internal class MovePrimaryAxisSlewingState : PrimaryAxisMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      private bool movingW;
      internal override DriverStateBase MoveAxisEast(Rate rate)
      {
         movingW = false;
         return InternalMoveAxis(rate);
      }
      internal override DriverStateBase MoveAxisWest(Rate rate)
      {
         movingW = true;
         return InternalMoveAxis(rate);
      }
      private DriverStateBase InternalMoveAxis(Rate rate)
      {
         // First set desired slew rate.
         serialPort.Transmit(MoveAxisHelper.RateToRateCommandString(TelescopeAxes.axisPrimary, rate));
         // Then start the axis moving and leave it moving.
         serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisPrimary, movingW));
         return this;
      }
      public override DriverStateBase AbortSlew()
      {
         if (HandleToCurrentScopeMoveTask != null)
         {
            ScopeMovementCancellationTokenSource.Cancel();
         }
         // The abort slew will hopefully cause slews waited on to finish
         // Dual axis slew so kill slews in both axes.
         serialPort.Transmit(":Q" + (movingW ? "w" : "e") +"#");
         // current slew task should exit.
         return SavedAxisState.ResumeTracking();
      }

      public override DriverStateBase MoveAxis(TelescopeAxes Axis, double rate)
      {
         if (rate != 0.0)
         {
            if (Axis == TelescopeAxes.axisSecondary && !DriverStateBase.SecondaryAxisCurrentState.InternalIsSlewing())
               return MoveAxisHelper.CreateMoveAxisStateFor(Axis, rate);
            else
               return this;
         }
         throw new ASCOM.InvalidValueException("Can only MoveAxis(Rate != 0.0)");
      }

      /// <summary>
      /// As a master state. We can route this to the correct axis.
      /// </summary>
      public override DriverStateBase StopMoveAxis(TelescopeAxes Axis)
      {
         // Worst that can happen is we return this.
         if (Axis == TelescopeAxes.axisTertiary)
         {
            throw new ASCOM.InvalidValueException("Tertiary axis is not supported.");
         }
         // InternalStopMoveAxis so we can't infinitely recurse to death.
         return MoveAxisHelper.MoveAxisStateFor(Axis).InternalStopMoveAxis();
      }

      public override DriverStateBase StopMoveAxis()
      {
         return InternalStopMoveAxis();
      }
      internal override DriverStateBase InternalStopMoveAxis()
      {
         // Looks like AbortSlew is all that is required.
         return AbortSlew();
      }
   }

   /// <summary>
   /// Single axis state for slew on the secondary axis. Returns to saved secondary axis state when done or aborted.
   /// Provides a specialised AbortSlew that only aborts the slew on the moving N/S axis.
   /// </summary>
   internal class MoveSecondaryAxisSlewingState : SecondaryAxisMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      private bool movingN = true;
      internal override DriverStateBase MoveAxisNorth(Rate rate)
      {
         movingN = true;
         return InternalMoveAxis(rate);
      }
      internal override DriverStateBase MoveAxisSouth(Rate rate)
      {
         movingN = false;
         return InternalMoveAxis(rate);
      }
      private DriverStateBase InternalMoveAxis(Rate rate)
      {
         // First set desired slew rate.
         serialPort.Transmit(MoveAxisHelper.RateToRateCommandString(TelescopeAxes.axisSecondary, rate));
         // Then slew in the direction that will move the telescope in the direction requested (note: this may require Up/Dn reversal).
         serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisSecondary, movingN));
         return this;
      }
      public override DriverStateBase AbortSlew()
      {
         if (HandleToCurrentScopeMoveTask != null)
         {
            ScopeMovementCancellationTokenSource.Cancel();
         }
         // The abort slew will hopefully cause slews waited on to finish
         // Dual axis slew so kill slews in both axes.
         serialPort.Transmit(":Q" + (movingN ? "n" : "s") + "#");
         // current slew task should exit.
         return SavedAxisState.ResumeTracking();
      }
      public override DriverStateBase StopMoveAxis()
      {
         return InternalStopMoveAxis();
      }
      internal override DriverStateBase InternalStopMoveAxis()
      {
         // Looks like AbortSlew is all that is required.
         return AbortSlew();
      }
   }

   /// <summary>
   /// Sorts out what moving axis state to create from the axis and rate.
   /// i.e. Takes Axis and Rate from ASCOM and based on them returns MovePrimaryAxisSlewingState or
   /// MoveSecondaryAxisSlewingState.
   /// </summary>
   partial class MoveAxisHelper
   {
      public static DriverStateBase CreateMoveAxisStateFor(TelescopeAxes Axis, double degreesPerSecRate)
      {
         foreach (Rate rate in new AxisRates(Axis))
         {
            if (rate.Minimum == Math.Abs(degreesPerSecRate) && rate.Maximum == Math.Abs(degreesPerSecRate))
            {
               if (Axis == TelescopeAxes.axisPrimary)
               {
                  return degreesPerSecRate >= 0.0
                     ? DriverStateBase.MasterCurrentState.MoveAxisWest(rate)
                     : DriverStateBase.MasterCurrentState.MoveAxisEast(rate);
               }
               else if (Axis == TelescopeAxes.axisSecondary)
               {
                  return degreesPerSecRate >= 0.0
                     ? DriverStateBase.SecondaryAxisCurrentState.MoveAxisNorth(rate)
                     : DriverStateBase.SecondaryAxisCurrentState.MoveAxisSouth(rate);
               }
               else
               {
                  throw new ASCOM.InvalidValueException("Tertiary axis is not supported by the driver.");
               }
            }
         }

         throw new ASCOM.InvalidValueException("Requested rate " + degreesPerSecRate.ToString() + " on axis " + Axis.ToString() + " is not supported.");
      }

      public static DriverStateBase MoveAxisStateFor(TelescopeAxes Axis)
      {
         if (Axis == TelescopeAxes.axisPrimary)
         {
            return DriverStateBase.MasterCurrentState;
         }
         else if (Axis == TelescopeAxes.axisSecondary)
         {
            return DriverStateBase.SecondaryAxisCurrentState;
         }
         else
         {
            throw new ASCOM.InvalidValueException("Tertiary axis is not supported by the driver.");
         }
      }
   }

   /// <summary>
   /// PulseGuidingPrimaryAxisState. IsPulseGuiding() returns true.
   /// Valid state transitions are those of PrimaryAxisMovingState.
   /// 
   /// This state exists to ensure that we don't attempt a pulse guide on an axis until the prior pulse
   /// guide is likely to be complete on that axis. It also does range checking on the pulse guide
   /// time in ms.
   /// 
   /// @caveat We presume the time asked for is based upon the guide rate we provide.
   /// </summary>
   internal class PulseGuidingPrimaryAxisState : PrimaryAxisMovingState
   {
      /// <summary>
      /// Master state can redirect requests to secondary axis state.
      /// </summary>
      public override DriverStateBase PulseGuide(GuideDirections Direction, int Duration)
      {
         if (Direction == GuideDirections.guideNorth || Direction == GuideDirections.guideSouth)
         {
            if (SecondaryAxisCurrentState.InternalIsPulseGuiding())
               SecondaryAxisCurrentState.AbortSlew();
            if(Direction== GuideDirections.guideNorth)
               return SecondaryAxisCurrentState.PulseGuideNorth(Duration);
            else
               return SecondaryAxisCurrentState.PulseGuideSouth(Duration);
         }
         else
         {
            AbortSlew();
            if (Direction == GuideDirections.guideEast)
               return PulseGuideEast(Duration);
            else
               return PulseGuideWest(Duration);
         }
      }

      internal override bool InternalIsPulseGuiding() { return true; }
      private bool guideW;
      internal override DriverStateBase PulseGuideWest(int durationMs)
      {
         guideW = true;
         return InternalPulseGuide(durationMs);
      }
      internal override DriverStateBase PulseGuideEast(int durationMs)
      {
         guideW = false;
         return InternalPulseGuide(durationMs);
      }
      private DriverStateBase InternalPulseGuide(int durationMs)
      {
         ScopeMovementCancellationTokenSource = new CancellationTokenSource();

         // 4ms to 32ms. Given we have a range of 4 to 32,000. We restrict to 4 digits by the look
         // of the Meade spec giving us an absolute rante of 4 - 9999ms.
         int realDurationMs = durationMs < 4 ? 4 : (durationMs > 32000 ? 32000 : durationMs);

         // Apply the mismatched guide performance fudge factor.
         Decimal guideSlewCoefDec = new Decimal(Telescope.guideSlewCoef);
         if (!guideW && Telescope.guideSlewCoefDirection == GuideDirections.guideEast && guideSlewCoefDec != new Decimal(1.0))
            realDurationMs = (int)Math.Round(new Decimal(realDurationMs) * guideSlewCoefDec, MidpointRounding.AwayFromZero);
         else if (guideW && Telescope.guideSlewCoefDirection == GuideDirections.guideWest && guideSlewCoefDec != new Decimal(1.0))
            realDurationMs = (int)Math.Round(new Decimal(realDurationMs) * guideSlewCoefDec, MidpointRounding.AwayFromZero);

         if (Telescope.guidingIsRaReCommands())
         {
            double guideRate = Telescope.guideRate * LX90.AxisRates.Sidereal;
            if (Telescope.customRateReverseLR)
            {
               // Negate the rate, reverses L/R for :RA#.
               guideRate = -guideRate;
            }
            serialPort.Transmit(":RA" + String.Format("{0:0.0####;-0.0####}", guideRate) + "#");
            serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisPrimary, guideW));
         }
         else if(Telescope.guidingIsMoveCommands())
         {
            // Scale the time for the equivalent move at sidereal.
            realDurationMs = (int) Math.Round(new Decimal(realDurationMs) * new Decimal(Telescope.guideRate), MidpointRounding.AwayFromZero);
            serialPort.Transmit(":RG#");
            serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisPrimary, guideW));
         }
         else
         {
            serialPort.Transmit(":Mg" + (guideW ? "w" : "e") + realDurationMs.ToString() + "#");
         }

         // This is just a timer with checks for abort slew in effect.
         HandleToCurrentScopeMoveTask = Task.Factory.StartNew<bool>(() =>
         {
            try
            {
               DateTime pulseGuideCompleteTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, 0, 0, realDurationMs));
               while (DateTime.UtcNow < pulseGuideCompleteTime)
               {
                  ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                  Thread.Sleep(1);
               }
               Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                  MasterCurrentState = MasterCurrentState.InternalPulseGuideComplete());
               return true;
            }
            catch (System.OperationCanceledException)
            {
               // Means AbortSlew... It will return use to the SavedAxisState.
               return false;
            }
            catch (Exception e)
            {
               Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                  CurrentState = MasterCurrentState.AbortSlew());
               throw e;
            }
         }, ScopeMovementCancellationTokenSource.Token);

         return this;
      }
      internal override DriverStateBase InternalPulseGuideComplete()
      {
         if (!Telescope.guidingIsPulseGuideCommands())
         {
            if (HandleToCurrentScopeMoveTask != null)
            {
               ScopeMovementCancellationTokenSource.Cancel();
            }
            serialPort.Transmit(":Q" + (guideW ? "w" : "e") + "#");
         }
         return SavedAxisState.ResumeTracking();
      }
   }

   /// <summary>
   /// PulseGuidingSecondaryAxisState. IsPulseGuiding() returns true.
   /// Valid state transitions are those of SecondaryAxisMovingState.
   /// 
   /// This state exists to ensure that we don't attempt a pulse guide on an axis until the prior pulse
   /// guide is likely to be complete on that axis. It also does range checking on the pulse guide
   /// time in ms.
   /// 
   /// @caveat We presume the time asked for is based upon the guide rate we provide.
   /// </summary>
   internal class PulseGuidingSecondaryAxisState : SecondaryAxisMovingState
   {
      internal override bool InternalIsPulseGuiding() { return true; }
      private bool guideN;
      internal override DriverStateBase PulseGuideNorth(int durationMs)
      {
         guideN = true;
         return InternalPulseGuide(durationMs);
      }
      internal override DriverStateBase PulseGuideSouth(int durationMs)
      {
         guideN = false;
         return InternalPulseGuide(durationMs);
      }
      private DriverStateBase InternalPulseGuide(int durationMs)
      {
         ScopeMovementCancellationTokenSource = new CancellationTokenSource();

         int realDurationMs = durationMs < 4 ? 4 : durationMs > 32000 ? 32000 : durationMs;
         if (Telescope.guidingIsRaReCommands())
         {
            double guideRate = Telescope.guideRate * LX90.AxisRates.Sidereal;
            if (Telescope.customRateReverseUD)
            {
               // Negate the guide rate. Reverse U/D for :RE# commands
               guideRate = -guideRate;
            }
            serialPort.Transmit(":RE" + String.Format("{0:0.0####;-0.0####}", guideRate) + "#");
            serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisSecondary, guideN));
         }
         else if (Telescope.guidingIsMoveCommands())
         {
            // Scale the time for the equivalent move at sidereal.
            realDurationMs = (int) Math.Round(new Decimal(realDurationMs) * new Decimal(Telescope.guideRate), MidpointRounding.AwayFromZero);
            serialPort.Transmit(":RG#");
            serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisSecondary, guideN));
         }
         else
         {
            serialPort.Transmit(":Mg" + (guideN ? "n" : "s") + realDurationMs.ToString() + "#");
         }         

         // Task ensures we don't accept new pulse guides until this pulse guide has 
         // completed or aborted.
         HandleToCurrentScopeMoveTask = Task.Factory.StartNew<bool>(() =>
         {
            try
            {
               DateTime pulseGuideCompleteTime = DateTime.UtcNow.Add(new TimeSpan(0, 0, 0, 0, durationMs));
               while (DateTime.UtcNow < pulseGuideCompleteTime)
               {
                  ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                  Thread.Sleep(1);
               }
               Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                  SecondaryAxisCurrentState = SecondaryAxisCurrentState.InternalPulseGuideComplete());
               return true;
            }
            catch (System.OperationCanceledException)
            {
               // Means AbortSlew... It will return use to the SavedAxisState.
               return false;
            }
            catch (Exception e)
            {
               Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                  SecondaryAxisCurrentState = SecondaryAxisCurrentState.AbortSlew());
               throw e;
            }
         }, ScopeMovementCancellationTokenSource.Token);
         return this;
      }
      internal override DriverStateBase InternalPulseGuideComplete()
      {
         if (!Telescope.guidingIsPulseGuideCommands())
         {
            if (HandleToCurrentScopeMoveTask != null)
            {
               ScopeMovementCancellationTokenSource.Cancel();
            }
            serialPort.Transmit(":Q" + (guideN ? "n" : "s") + "#");
         }
         return SavedAxisState.ResumeTracking();
      }
   }

   /// <summary>
   /// Slewing to Alt/Az async. Synchronous versions are not supported.
   /// Valid state transitions are those of DualAxisSlewingState.
   /// 
   /// The mount is polled in a lazy worker thread so that we don't prematurely return
   /// false for Slewing(). i.e. When the HW returns not polling, we call SlewComplete().
   /// 
   /// Otherwise AbortSlew() will return us to a tracking state on both axes.
   /// </summary>
   internal class SlewingToAltAzState : DualAxisSlewingState
   {
      public override DriverStateBase SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         ScopeMovementCancellationTokenSource = new CancellationTokenSource();
         try
         {
            // First set target alt/az
            string Az = utilities.DegreesToDM(Azimuth, "*", "");
            serialPort.Transmit(":Sz" + Az + "#");
            byte[] valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] != '1')
            {
               throw new ASCOM.InvalidValueException("Mount rejected Azimuth " + Azimuth.ToString() + "(" + Az + ")");
            }

            // Valid because otherwise we throw.
            string Alt = utilities.DegreesToDM(Altitude, "*", "");
            serialPort.Transmit(":Sa" + Alt + "#");
            valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] != '1')
            {
               throw new ASCOM.InvalidValueException("Mount rejected Altitude " + Altitude.ToString() + "(" + Alt + ")");
            }

            // Again only here because we have not thrown.
            // Then slew to alt/az
            serialPort.Transmit(":MA#");
            valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] == '1')
            {
               throw new ASCOM.InvalidValueException("Mount failed to slew to Az/Alt " + Az + "/" + Alt + ".");
            }

            // This task is here to run async and check the HW slewing state without holding
            // up the works.
            HandleToCurrentScopeMoveTask = Task.Factory.StartNew< bool >(() =>
            {
               try
               {
                  bool isHwSlewing = true;
                  while (isHwSlewing)
                  {
                     Task<bool> hwSlewingTask = new Task<bool>(() => { return InternalIsHwSlewing(); });
                     Patched497Queue.Instance.WorkQueue.Enqueue(() => hwSlewingTask.RunSynchronously());
                     if (isHwSlewing = hwSlewingTask.Result)
                     {
                        ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Don't hammer the mount. We are doing a slew.
                        System.Threading.Thread.Sleep(100);
                     }
                  }

                  Patched497Queue.Instance.WorkQueue.Enqueue(() => 
                     // If still the expected state, then this returns all the
                     // axes states to what they were before we kicked off the slew.
                     CurrentState = MasterCurrentState.SlewComplete());
                  return true;
               }
               catch (System.OperationCanceledException)
               {
                  // Means Abort so no slew completion and states all returned to what they should be.
                  return false;
               }
               catch (Exception e)
               {
                  Patched497Queue.Instance.WorkQueue.Enqueue(() => 
                     CurrentState = MasterCurrentState.AbortSlew());
                  throw e;
               }
            }, ScopeMovementCancellationTokenSource.Token);
         }
         catch (Exception e)
         {
            Patched497Queue.Instance.WorkQueue.Enqueue(() => 
               CurrentState = MasterCurrentState.AbortSlew());
            throw e;
         }
         return this;
      }
   }

   /// <summary>
   /// Slewing to RA/Dec async. Synchronous versions are not supported.
   /// Valid state transitions are those of DualAxisSlewingState.
   /// 
   /// The mount is polled in a lazy worker thread so that we don't prematurely return
   /// false for Slewing(). i.e. When the HW returns not polling, we call SlewComplete().
   /// 
   /// Otherwise AbortSlew() will return us to a tracking state on both axes.
   /// </summary>
   internal class SlewingToCoordinatesState : DualAxisSlewingState
   {
      public override DriverStateBase SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         ScopeMovementCancellationTokenSource = new CancellationTokenSource();
         try
         {
            string RA = utilities.HoursToHMS(RightAscension);
            serialPort.Transmit(":Sr" + RA + "#");
            byte[] valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] != '1')
            {
               throw new ASCOM.InvalidValueException("Mount rejected RA " + RightAscension.ToString() + "(" + RA + ")");
            }

            // Valid because otherwise we throw.
            string Dec = utilities.DegreesToDMS(Declination, "*", ":");
            serialPort.Transmit(":Sd" + Dec + "#");
            valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] != '1')
            {
               throw new ASCOM.InvalidValueException("Mount rejected Declination " + Declination.ToString() + "(" + Dec + ")");
            }

            // Again only here because we have not thrown.
            // Then slew to RA/Dec
            serialPort.Transmit(":MS#");
            valid = serialPort.ReceiveCountedBinary(1);
            if (valid[0] != '0')
            {
               string mountMsg = serialPort.ReceiveTerminated("#").Replace("#", "");
               throw new ASCOM.InvalidValueException("Mount failed to slew to RA/Dec " + RA + "/" + Dec + ". \"" + mountMsg + "\"");
            }

            // This task is here to run async and check the HW slewing state without holding
            // up the works.
            HandleToCurrentScopeMoveTask = Task.Factory.StartNew<bool>(() =>
            {
               try
               {
                  bool isHwSlewing = true;
                  while(isHwSlewing)
                  {
                     Task<bool> hwSlewingTask = new Task<bool>(() => { return InternalIsHwSlewing(); });
                     Patched497Queue.Instance.WorkQueue.Enqueue(() => hwSlewingTask.RunSynchronously());
                     if (isHwSlewing = hwSlewingTask.Result)
                     {
                        ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Don't hammer the mount. We are doing a slew.
                        System.Threading.Thread.Sleep(100);
                     }
                  }

                  // HW no longer reports slewing so queue slew complete.
                  Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                     // If still the expected state, then this returns all the
                     // axes states to what they were before we kicked off the slew.
                     CurrentState = MasterCurrentState.SlewComplete());
                  return true;
               }
               catch (System.OperationCanceledException)
               {
                  // Means Abort so no slew completion and states all returned to what they should be.
                  return false;
               }
               catch (Exception e)
               {
                  Patched497Queue.Instance.WorkQueue.Enqueue(() =>
                     CurrentState = MasterCurrentState.AbortSlew());
                  throw e;
               }
            }, ScopeMovementCancellationTokenSource.Token);
         }
         catch (Exception e)
         {
            Patched497Queue.Instance.WorkQueue.Enqueue(() =>
               CurrentState = MasterCurrentState.AbortSlew());
            throw e;
         }
         return this;
      }
   }

   /// <summary>
   /// TrackingState is the connected state whenever NOT moving the scope at a rate other than
   /// sidereal or lunar.
   /// 
   /// Valid state transitions are:
   ///    TrackingState->MoveAxis(...)->Move...AxisSlewingState.
   ///    TrackingState->SlewToAltAz(...)->DualAxisSlewingState.
   ///    TrackingState->SlewToCoordinates(...)->DualAxisSlewingState.
   ///    TrackingState->PulseGuide(...)->PulseGuiding...AxisState.
   ///    And the state transitions of its base class ConnectedState.
   /// </summary>
   partial class TrackingState : ConnectedState
   {
      public TrackingState()
      {
         // Make doubly sure the mount reflects us...
         ResumeTracking();
      }

      // Yep. We are tracking. That is the state we represent.
      // So Everything is tracking if the secondary axis is tracking too.
      public override bool IsTracking() { return SecondaryAxisCurrentState.IsTracking(); }
      
      /// <summary>
      /// Work out which axis to create the Move...AxisSlewingState for.
      /// </summary>
      public override DriverStateBase MoveAxis(TelescopeAxes Axis, double Rate)
      {
         if (Rate != 0.0)
         {
            return MoveAxisHelper.CreateMoveAxisStateFor(Axis, Rate);
         }
         throw new ASCOM.InvalidValueException("Can only MoveAxis(Rate != 0.0)");
      }

      /// <summary>
      /// Because although this is not a Move...Axis state, the other
      /// axis might be and if this is the MasterCurrentState it needs to 
      /// pass on the request to the other axis.
      /// </summary>
      public override DriverStateBase StopMoveAxis(TelescopeAxes Axis)
      {
         // Worst that can happen is we return this.
         if (Axis == TelescopeAxes.axisTertiary)
         {
            throw new ASCOM.InvalidValueException("Tertiary axis is not supported.");
         }
         // InternalStopMoveAxis so we can't infinitely recurse to death.
         return MoveAxisHelper.MoveAxisStateFor(Axis).InternalStopMoveAxis();
      }

      /// <summary>
      /// If this is called, the axis it represents is tracking and therefore can
      /// start pulse guiding.
      /// 
      /// </summary>
      public override DriverStateBase PulseGuide(GuideDirections Direction, int Duration)
      {
         switch (Direction)
         {
            case GuideDirections.guideNorth:
               return SecondaryAxisCurrentState.PulseGuideNorth(Duration);
            case GuideDirections.guideSouth:
               return SecondaryAxisCurrentState.PulseGuideSouth(Duration);
            case GuideDirections.guideEast:
               return MasterCurrentState.PulseGuideEast(Duration);
            default:
               return MasterCurrentState.PulseGuideWest(Duration);
         }
      }

      public override DriverStateBase SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         if (MasterCurrentState.IsTracking())
            return (new SlewingToAltAzState()).SlewToAltAzAsync(Azimuth, Altitude);
         else
            return this;
      }

      public override DriverStateBase SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         if (MasterCurrentState.IsTracking())
            return (new SlewingToCoordinatesState()).SlewToCoordinatesAsync(RightAscension, Declination);
         else
            return this;
      }

      public override DriverStateBase SyncToAltAz(double Azimuth, double Altitude)
      {
         if (!MasterCurrentState.IsTracking())
            return this;
         // First set target alt/az
         string Az = utilities.DegreesToDM(Azimuth, "*", "");
         serialPort.Transmit(":Sz" + Az + "#");
         byte[] valid = serialPort.ReceiveCountedBinary(1);
         if (valid[0] != 1 && !valid[0].ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount rejected Azimuth " + Azimuth.ToString() + "(" + Az + ")");
         }

         // Valid because otherwise we throw.
         string Alt = utilities.DegreesToDM(Altitude, "*", "");
         serialPort.Transmit(":Sa" + Alt + "#");
         valid = serialPort.ReceiveCountedBinary(1);
         if (valid[0] != 1 && !valid[0].ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount rejected Altitude " + Altitude.ToString() + "(" + Alt + ")");
         }

         serialPort.Transmit(":CM#");
         serialPort.ReceiveTerminated("#");
         return this;
      }

      public override DriverStateBase SyncToCoordinates(double RightAscension, double Declination)
      {
         if (!MasterCurrentState.IsTracking())
            return this;
         string RA = utilities.DegreesToHMS(RightAscension);
         RA = RA.Substring(0, 5) + Math.Round(Decimal.Parse(RA.Substring(6, 2)) / new Decimal(60), 1).ToString();
         serialPort.Transmit(":Sr" + RA + "#");
         byte[] valid = serialPort.ReceiveCountedBinary(1);
         if (valid[0] != 1 && !valid[0].ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount rejected RA " + RightAscension.ToString() + "(" + RA + ")");
         }

         // Valid because otherwise we throw.
         string Dec = utilities.DegreesToDMS(Declination, "*", ":");
         serialPort.Transmit(":Sd" + Dec + "#");
         valid = serialPort.ReceiveCountedBinary(1);
         if (valid[0] != 1 && !valid[0].ToString().Equals("1"))
         {
            throw new ASCOM.InvalidValueException("Mount rejected Declination " + Declination.ToString() + "(" + Dec + ")");
         }

         serialPort.Transmit(":CM#");
         serialPort.ReceiveTerminated("#");
         return this;
      }
   }

   /// <summary>
   /// This represents the connected state of the secondary axis. Quiet. Not slewing, guiding, tracking.
   /// Valid state transitions are:
   ///    SecondaryAxisTrackingState->MoveAxis(...)->MoveSecondaryAxisSlewingState
   ///    SecondaryAxisTrackingState->PulseGuide(...)->PulseGuideSecondaryAxisState
   /// </summary>
   partial class SecondaryAxisTrackingState : TrackingState
   {
      public override bool IsSecondaryAxisState() { return true; }
      public override bool IsTracking() { return true; }

      // Tracking for us means don't move!
      public override DriverStateBase ResumeTracking()
      {
         serialPort.Transmit(":Qn#");
         serialPort.Transmit(":Qs#");
         return this;
      }

      internal override DriverStateBase PulseGuideNorth(int durationMs)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
         //   3. The other axis is doing a pulse guide - we don't care.
         if (!MasterCurrentState.InternalIsSlewing())
            return (new PulseGuidingSecondaryAxisState()).PulseGuideNorth(durationMs);
         else
            return this;
      }

      internal override DriverStateBase PulseGuideSouth(int durationMs)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
         //   3. The other axis is doing a pulse guide - we don't care.
         if (!MasterCurrentState.InternalIsSlewing())
            return (new PulseGuidingSecondaryAxisState()).PulseGuideSouth(durationMs);
         else
            return this;
      }

      internal override DriverStateBase MoveAxisNorth(Rate rate)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we don't care.
         //   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
         if (!MasterCurrentState.IsPulseGuiding())
            return (new MoveSecondaryAxisSlewingState()).MoveAxisNorth(rate);
         else
            return this;
      }

      internal override DriverStateBase MoveAxisSouth(Rate rate)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we don't care.
         //   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
         if (!MasterCurrentState.IsPulseGuiding())
            return (new MoveSecondaryAxisSlewingState()).MoveAxisSouth(rate);
         else
            return this;
      }
   }

   partial class PrimaryAxisTrackingState : TrackingState
   {
      internal override DriverStateBase PulseGuideEast(int durationMs)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
         //   3. The other axis is doing a pulse guide - we don't care.
         if (!SecondaryAxisCurrentState.InternalIsSlewing())
            return (new PulseGuidingPrimaryAxisState()).PulseGuideEast(durationMs);
         else
            return this;
      }

      internal override DriverStateBase PulseGuideWest(int durationMs)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
         //   3. The other axis is doing a pulse guide - we don't care.
         if (!SecondaryAxisCurrentState.InternalIsSlewing())
            return (new PulseGuidingPrimaryAxisState()).PulseGuideWest(durationMs);
         else
            return this;
      }

      internal override DriverStateBase MoveAxisEast(Rate rate)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we don't care.
         //   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
         if (!SecondaryAxisCurrentState.IsPulseGuiding())
            return (new MovePrimaryAxisSlewingState()).MoveAxisEast(rate);
         else
            return this;
      }

      internal override DriverStateBase MoveAxisWest(Rate rate)
      {
         // This axis is not slewing, that we know.
         // The other axis can't be doing a dual axis goto because this method is not
         // callable if that is the case.
         // So the only possibilities are:
         //   1. The other axis is tracking - we don't care.
         //   2. The other axis is doing a single axis slew - we don't care.
         //   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
         if (!SecondaryAxisCurrentState.IsPulseGuiding())
            return (new MovePrimaryAxisSlewingState()).MoveAxisWest(rate);
         else
            return this;
      }
   }

   /// <summary>
   /// Sidereal tracking state. Same as TrackingState but IsSiderealTracking() returns true and
   /// ResumeTracking() makes sure commands to track at sidereal get sent to the mount.
   /// </summary>
   partial class SiderealTrackingState : PrimaryAxisTrackingState
   {
      public override bool IsTrackingSidereal() { return true; }
      public override DriverStateBase ResumeTracking()
      {
         serialPort.Transmit(":TQ#");
         return this;
      }
   }

   /// <summary>
   /// Lunar tracking state. Same as TrackingState but IsLunarTracking() returns true and
   /// ResumeTracking() makes sure commands to track at lunar get sent to the mount.
   /// </summary>
   partial class LunarTrackingState : PrimaryAxisTrackingState
   {
      public override bool IsTrackingLunar() { return true; }
      public override DriverStateBase ResumeTracking()
      {
         serialPort.Transmit(":TL#");
         return this;
      }
   }

   #endregion // Patched497StateMachine
}
