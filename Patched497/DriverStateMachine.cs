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
      /// <summary>
      /// The master current state is always set once the driver is created.
      /// It represents the main interface generally and for the primary axis specifically.
      /// It can have a state that represents the primary axis, or dual axes.
      /// E.g. SiderealTrackingState is it's default.
      ///      SlewPrimaryAxisState is a state that controls just the primary axis.
      ///      SlewToCoordinatesState is a state that controls both axes.
      /// </summary>
      public static DriverStateBase MasterCurrentState = null;
      /// <summary>
      /// The SecondaryAxisCurrentState is set to SecondaryAxisTrackingState by default.
      /// It represents a secondary interface to control the secondary axis.
      /// Provided preconditions are met (see the state transition diagrams in the docs)
      /// it can be set to SlewSecondaryAxisState or PulseGuideSecondaryAxisState from
      /// the SecondaryAxisTrackingState.
      /// For dual axis movement, SlewToAltAzState and SlewToCoordinatesState is is set to 
      /// the same state as teh MasterCurrentState. Thus any call on it will call the correct
      /// state transitioning operation on the DualAxisMovingState.
      /// </summary>
      public static DriverStateBase SecondaryAxisCurrentState = null;

      /// <summary>
      /// Save the rate last set on the primary axis so that we don't unnecessarily set the
      /// rate if it has not changed from when we last set the rate.
      /// 
      /// This is mainly to see if only issuing :RA{+ve/-ve rate}# commands when the rate
      /// changes instead of with every movement using :RA...# fixes issues with stability
      /// slewing at those custom rates.
      /// </summary>
      public static double lastPrimaryAxisRate = 0.0;

      /// <summary>
      /// Save the rate last set on the secondary axis so that we don't unnecessarily set the
      /// rate if it has not changed from when we last set the rate.
      /// 
      /// This is mainly to see if only issuing :RE{+ve/-ve rate}# commands when the rate
      /// changes instead of with every movement using :RA...# fixes issues with stability
      /// slewing at those custom rates.
      /// </summary>
      public static double lastSecondaryAxisRate = 0.0;

      /// <summary>
      /// Property setting understands whether the state applies to a single axis
      /// or dual axes.
      /// 
      /// The basic idea is you can just set CurrentState with the state you want and it
      /// enforces the rules around whether that state can only be applied to a specific axis.
      /// </summary>
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

      internal static ASCOM.Utilities.Serial serialPort = null;
      protected Util utilities = null;
      public DriverStateBase() 
      {
         utilities = new Util();
      }

      /// <summary>
      /// All the methods in this region are responsible for a state transition.
      /// Derived classes override ONLY those operations and state transitioning methods
      /// that are valid for the state they represent.
      /// 
      /// Thus any request for an invalid operation on a given state calls the base class
      /// method which returns this - the instance of the existing state instead of a new
      /// state and no state transition occurs nor any inavlid operation for the given state.
      /// 
      /// All state transition operations MUST be only called from a task queued to the MountTaskHandler.WorkQueue.
      /// This is thread safe. The state transitioning task is performed ONLY in the MountTaskHandler worker thread.
      /// </summary>
      #region DefaultStateTransitioningMethods
      
      /// <summary>
      /// Connect to the provided port. Overrides return the next state on
      /// successful connection.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase Connect(int Port)
      {
         return this;
      }
      /// <summary>
      /// Disconnect from the mount. Overrides return the next state on 
      /// successful disconnection.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase Disconnect()
      {
         return this;
      }
      /// <summary>
      /// Park the telesdcope and disconnect from the mount. Overrides return the next state on 
      /// successful park.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase Park()
      {
         return this;
      }
      /// <summary>
      /// Overrides should issue the commands necessary to ensure the mount is tracking.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase ResumeTracking()
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent Slew...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase MoveAxisNorth(Rate rate)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent Slew...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase MoveAxisSouth(Rate rate)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent Slew...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase MoveAxisEast(Rate rate)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent Slew...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase MoveAxisWest(Rate rate)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the result of MoveAxis...North/South/East/West based upon axis
      /// and +ve or -ve rate.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase MoveAxis(TelescopeAxes Axis, double rate)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the result of a request to resume tracking. This is for 
      /// use in this class hierachy only. i.e. It is not publically exposed to the client.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase InternalResumeTracking()   
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the result of InternalResumeTracking from the state representing
      /// the provided axis.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase ResumeTracking(TelescopeAxes axis)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent PulseGuide...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase PulseGuideNorth(int durationMs)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent PulseGuide...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase PulseGuideSouth(int durationMs)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent PulseGuide...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase PulseGuideEast(int durationMs)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the relevent PulseGuide...AxisState after issuing the commands
      /// to the mount to do so.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase PulseGuideWest(int durationMs)
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the result of PulseGuide...North/South/East/West based the given Direction.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase PulseGuide(GuideDirections Direction, int Duration)
      {
         return this;
      }
      /// <summary>
      /// To ensure that IsPulseGuiding on the driver returns true for the duration (ms) of a pulse guide
      /// we run a cancellable task for the expected duration of the pulse guide that calls this method
      /// when the duration has elapsed. Thus a client can ask if the axis is still pulse guiding in the absence
      /// of the mount providing the ability to determine this.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      internal virtual DriverStateBase InternalPulseGuideComplete()
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the axes to their tracking states after stopping all slewing.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase AbortSlew()
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the issue the commands for sidereal tracking to the mount and return SiderealTrackingState if it is valid for the axis. 
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase TrackSidereal()
      {
         return this;
      }
      /// <summary>
      /// Overrides should return the issue the commands for lunar tracking to the mount and return  the LunarTrackingState if it is valid for the axis. 
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase TrackLunar()
      {
         return this;
      }
      /// <summary>
      /// Overrides should issue the commands to slew to the Alt/Az and return the DualAxisMovingState SlewToAltAzState if that state transition is valid.
      /// Interruption is by AbortSlew().
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         return this;
      }
      /// <summary>
      /// Overrides should issue the commands to slew to the celestial coordinates and return the DualAxisMovingState SlewToCoordinatesState if that state transition is valid.
      /// Interruption is by AbortSlew().
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         return this;
      }
      /// <summary>
      /// Overrides should issue the commands to sync the mount to the given Alt/Az. This is in all likelihood valid on any connected state.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase SyncToAltAz(double Az, double Alt)
      {
         return this;
      }
      /// <summary>
      /// Overrides should issue the commands to sync the mount to the given celestial coordinates. This is in all likelihood valid on any connected state.
      /// </summary>
      /// <returns>Existing state - no-op.</returns>
      public virtual DriverStateBase SyncToCoordinates(double RA, double Dec)
      {
         return this;
      }
      #endregion

      /// <summary>
      /// These methods largely exist to avoid slower means (polymorphic cast) of determining whether a current state represents a particular condition of the mount.
      /// Individual derived classes should only override and return true for the exact methods below that correspond to the state they represent for the mount.
      /// </summary>
      #region DefaultStateInterrogationMethods

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
      /// <summary>
      /// This is used in DualAxisMovingState subclasses to poll the mount and determine automatically when
      /// the mount says the slew is complete.
      /// 
      /// This default returns false. Overrides should interrogate the mount as required.
      /// </summary>
      /// <returns>false</returns>
      internal virtual bool InternalIsHwSlewing() { return false; }
      public virtual bool IsDualAxisState() { return !IsPrimaryAxisState() && !IsSecondaryAxisState(); }
      public virtual bool IsPrimaryAxisState() { return false; }
      public virtual bool IsSecondaryAxisState() { return false; }
      
      #endregion

      /// <summary>
      /// The methods found here should be overriden by all connected states. They allow the mount to be interrogated
      /// for its values for the given property requested.
      /// </summary>
      #region ConnectedStateMethods

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

      #endregion
   }

   /// <summary>
   /// All state requests and state changing operations come through here
   /// and we ensure that no interleaved serial coms can happen for starters.
   /// The state machine executes in here and therefore requires no locks AND
   /// prevents all manner of invalid state transitions or operations invalid
   /// on any given state.
   /// </summary>
   public class MountTaskHandler
   {
      private static MountTaskHandler instance = null;
      /// <summary>
      /// This property ensures that MountTaskHandler is a singleton. There can be only one.
      /// </summary>
      public static MountTaskHandler Instance
      {
         get
         {
            if (instance == null)
            {
               instance = new MountTaskHandler();
            }
            return instance;
         }
      }

      /// <summary>
      /// Thread safe queue of tasks requested of the mount.
      /// </summary>
      public BlockingCollection<Action> WorkQueue = new BlockingCollection<Action>();

      /// <summary>
      /// Worker thread in effect that invokes the queued task otherwise sleeps.
      /// </summary>
      private MountTaskHandler()
      {
         Task.Factory.StartNew(() =>
         {
            Action func;
            while (true)
            {
               if (WorkQueue.TryTake(out func, 100))
               {
                  try
                  {
                     func.Invoke();
                  }
                  catch (Exception e)
                  {
                     // If the requested task throws an exception, we clear the serial port. Testing has shown
                     // that things go bad if the port receive queue has flotsom left over from some failed task
                     // and you request something new be done. i.e. The new task gets fed leftovers from the previous
                     // task.
                     DriverStateBase.serialPort.ClearBuffers();
                     Telescope.LogMessage("Exception in HBX work queue thread - \n", e.Message);
                  }
               }
            }
         });
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
         // Here we ensure our TrackingState matches the mount.
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

            // This is a hack. If you issue :RA# or :RE# commands and have not first moved the scope
            // by :Me#/:Mw# or by :Mn#/:Ms#, the :RA#/:RE# is ignored and you take off at whatever the
            // rate last set on the HBX is. So a :RA0.004# (sidereal) slew can suddently take off at 6.5
            // degrees per second!
            //
            // We are just going to jog the scope here and stop all slews when done.
            serialPort.Transmit(":RG#");
            serialPort.Transmit(":Me#");
            Thread.Sleep(1);
            serialPort.Transmit(":Q#");
            serialPort.Transmit(":Mw#");
            Thread.Sleep(1);
            serialPort.Transmit(":Q#");

            // Now kick off tracking.
            // This checks the tracking rate of the mount to ensure we start out with the tracking
            // rate that matches what the mount says. i.e. Lunar or Sidereal.
            MasterCurrentState = TrackingStateHelper.TrackingStateForMount(serialPort);
            SecondaryAxisCurrentState = new SecondaryAxisTrackingState();
            return MasterCurrentState;
         }
         catch(Exception e)
         {
            // Put the serial port back to disconnected and requiring reinitialisation.
            if (serialPort != null)
            {
               serialPort.Connected = false;
               serialPort.Dispose();
               serialPort = null;
            }
            throw e;
         }
      }
   }

   /// <summary>
   /// ParkedState is a special disconnected state. IsParked() will always return true.
   /// Valid state transitions:
   ///    ParkedState->Connect(Port)->TrackingState on both axes. ONLY if the mount has been powered off/on again.
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
      /// <summary>
      /// Override of Disconnect that disconnects the serial port.
      /// </summary>
      /// <returns>DisconnectedState. Mount is not parked and left as it was.</returns>
      public override DriverStateBase Disconnect()
      {
         if (serialPort != null)
         {
            // Make sure we cannot leave the mount in a slewing state.
            serialPort.Transmit(":Q#");
            serialPort.Connected = false;
            serialPort.Dispose();
            serialPort = null;
         }

         return new DisconnectedState();
      }

      /// <summary>
      /// Override of Park that parks the mount and disconnects the serial port.
      /// </summary>
      /// <returns>ParkedState. To reconnect the mount will have to be switched off and on again.</returns>
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

      /// <summary>
      /// If anyone asks, we are connected.
      /// </summary>
      /// <returns>Always true.</returns>
      public override bool IsConnected() { return true; }

      /// <summary>
      /// Any connected state can tell you about mount position, date, time, etc.
      /// </summary>
      #region ConnectedStateMethodOverrides

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
         // Don't always get a response! Never seen this fail more than once.
         // I will wear the result if we recurse to death, but that never seems
         // to happen.
         try
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
         catch (Exception)
         {
            // try again...
            return Declination();
         }
      }

      public override double RightAscension()
      {
         // Don't always get a response! Never seen this fail more than once.
         // I will wear the result if we recurse to death, but that never seems
         // to happen.
         try
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
         catch(Exception)
         {
            // try again...
            return RightAscension();
         }
      }

      /// <summary>
      /// Let any connected state ask if the mount is REALLY slewing or not.
      /// Only ever call from somewhere already in the MountTaskHandler worker thread.
      /// </summary>
      /// <returns>true if the mount reports that it is slewing.</returns>
      internal override bool InternalIsHwSlewing()
      {
         serialPort.Transmit(":D#");
         string indicator = serialPort.ReceiveTerminated("#").Replace("#", "");
         bool hwSlewing = indicator.Length > 0;
         return hwSlewing;
      }

      // Let any connected state ask if the mount is REALLY slewing or not.
      // Only ever call from somewhere already in the MountTaskHandler worker thread.
      internal override bool InternalIsHwTracking()
      {
         serialPort.Transmit(":GW#");
         string status = serialPort.ReceiveTerminated("#").Replace("#", "");
         return status.Contains("<T>");
      }

      public override double SiderealTime()
      {
         serialPort.Transmit(":GS#");
         string sidereal = serialPort.ReceiveTerminated("#").Replace("#", "");
         int hours = int.Parse(sidereal.Substring(0, 2));
         int min = int.Parse(sidereal.Substring(3, 2));
         int sec = int.Parse(sidereal.Substring(6, 2));
         return hours + (min / 60.0) + (sec / 3600.0);
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
         if (result[0] != '1')
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
         if (result[0] != '1')
         {
            throw new ASCOM.InvalidValueException("Mount did not accept " + newLon.ToString() + "(" + utilities.DegreesToDM(newLon, "*", "") + ") as a valid longitude.");
         }
      }
      #endregion

      /// <summary>
      /// If told, arbitraily stop any slewing and track lunar.
      /// </summary>
      /// <returns>LunarTrackingState.</returns>
      public override DriverStateBase TrackLunar()
      {
         AbortSlew();
         return new LunarTrackingState();
      }

      /// <summary>
      /// If told, arbitraily stop any slewing and track sidereal.
      /// </summary>
      /// <returns>SiderealTrackingState.</returns>
      public override DriverStateBase TrackSidereal()
      {
         AbortSlew();
         return new SiderealTrackingState();
      }

      /// <summary>
      /// Get the UTC date from the mount.
      /// </summary>
      /// <returns>DateTime that is the UTC date.</returns>
      public override DateTime UTCDate()
      {
         // Get the mount local time. Add the UTC offset to get UTC time.
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
   }

   /// <summary>
   /// Represents any state where we are moving the scope but not tracking. i.e. Slew, pulse guide, etc.
   /// Valid state transitions are:
   ///    AxisMovingState->ResumeTracking()->Sidereal/Lunar TrackingState/SecondaryAxisTrackingState
   /// </summary>
   public abstract class AxisMovingState : ConnectedState
   {
      /// <summary>
      /// This is the state we have replaced and the state we should return to when done.
      /// On any completion ResumeTracking() should be returned from this state.
      /// </summary>
      protected DriverStateBase SavedAxisState = null;
      /// <summary>
      /// This allows us to communicate with the task that is launched in the worker thread. And to determine that the task still exists.
      /// </summary>
      internal Task<bool> HandleToCurrentScopeMoveTask = null;
      /// <summary>
      /// This is a signal primitive that the task when executing in the worker thread will check for a request that the task be cancelled.
      /// </summary>
      private CancellationTokenSource scopeMovementCancellationTokenSource = null;
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

      /// <summary>
      /// If our ResumeTracking is called we are being told to return to our saved axis state and it should resume tracking.
      /// i.e. LunarTrackingState might like to issue commands for lunar tracking to be on the safe side, likewise sidereal tracking.
      /// </summary>
      /// <returns>The result of resuming tracking on the SavedAxisState.</returns>
      public override DriverStateBase ResumeTracking()
      {
         return SavedAxisState.ResumeTracking();
      }

      /// <summary>
      /// ASCOM has a Slewing property so regardless of axis, we can only report that something somewhere is slewing.
      /// </summary>
      /// <returns>true if either axis is slewing.</returns>
      public override bool IsSlewing() 
      {
         return MasterCurrentState.InternalIsSlewing() || SecondaryAxisCurrentState.InternalIsSlewing();
      }

      /// <summary>
      /// ASCOM has a IsPulseGuiding() method so regardless of axis, we can only report that something somewhere is pulse guiding.
      /// </summary>
      /// <returns>true if either axis is pulse guiding.</returns>
      public override bool IsPulseGuiding()
      {
         return MasterCurrentState.InternalIsPulseGuiding() || SecondaryAxisCurrentState.InternalIsPulseGuiding();
      }
   }

   /// <summary>
   /// Moving state on the primary axis that saves away the MasterCurrentState as the 
   /// state to return to when ResumeTracking() is called.
   /// </summary>
   public abstract class PrimaryAxisMovingState : AxisMovingState
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
   public abstract class SecondaryAxisMovingState : AxisMovingState
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
   ///    DualAxisSlewingState->AbortSlew()->Sidereal/Lunar Tracking and SecondaryAxisTrackingState.
   ///    DualAxisSlewingState->ResumeTracking()->Sidereal/Lunar Tracking and SecondaryAxisTrackingState.
   /// </summary>
   public abstract class DualAxisSlewingState : AxisMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      /// <summary>
      /// Need to preserve the secondary axis state too.
      /// </summary>
      private DriverStateBase SecondaryAxisState;

      /// <summary>
      /// Store away both axes states for restoration when done.
      /// </summary>
      public DualAxisSlewingState()
      {
         SavedAxisState = MasterCurrentState;
         SecondaryAxisState = SecondaryAxisCurrentState;
         // We will occupy both axes... we're greedy like that.
         SecondaryAxisCurrentState = this;
      }

      /// <summary>
      /// Special abort slew that stops all slewing and returns us to the saved axis states.
      /// </summary>
      /// <returns></returns>
      public override DriverStateBase AbortSlew()
      {
         if (HandleToCurrentScopeMoveTask != null)
         {
            ScopeMovementCancellationTokenSource.Cancel();
         }
         // The abort slew will hopefully cause slews waited on to finish
         // Dual axis slew so kill slews in both axes.
         serialPort.Transmit(":Q#");

         return ResumeTracking();
      }

      /// <summary>
      /// Override of resume tracking that calls ResumeTracking() on both axes.
      /// </summary>
      /// <returns>The SavedAxisState for the primary axis. i.e. The master state.</returns>
      /// <sideeffect>Also restores the SecondaryAxisCurrentState.</sideeffect>
      public override DriverStateBase ResumeTracking()
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
      /// <summary>
      /// Get a Tuple representing the rate command string and whether to issue the command or not
      /// based upon what rate we last set for the axis.
      /// </summary>
      /// <returns>Well I thought an empty string would do but then its too easy to ignore and use directly in a 
      /// command to the mount without checking if that is the last rate we applied, so I went all complicated to be
      /// a pain in the ar$e and MAKE the programmer take note.
      /// 
      /// Tuple< bool, string > first is whether the rate differs from last, second is the command string.</returns>
      public static Tuple<bool, string> RateToRateCommandString(TelescopeAxes Axis, Rate rate)
      {
         if (Axis == TelescopeAxes.axisTertiary )
         {
            throw new InvalidValueException("Tertiary axis not supported by driver.");
         }

         string command;
         if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.Sidereal))
         {
            command = ":RG#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.Siderealx2))
         {
            command = ":RC#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.SlewHalfDegreePerSec))
         {
            command = ":RM#";
         }
         else if (AxisRates.ratesApproxEqual(rate.Minimum, ASCOM.LX90.AxisRates.SlewSixPointFiveDegreePerSec))
         {
            command = ":RS#";
         }
         else if (rate.Minimum > ASCOM.LX90.AxisRates.Siderealx2 && rate.Minimum < ASCOM.LX90.AxisRates.SlewSixPointFiveDegreePerSec)
         {
            if (Axis == TelescopeAxes.axisPrimary)
            {
               command = ":RA" + String.Format("{0:0.0####;-0.0####}", Telescope.customRateReverseLR ? -rate.Minimum : rate.Minimum) + "#";
            }
            else
            {
               command = ":RE" + String.Format("{0:0.0####;-0.0####}", Telescope.customRateReverseUD ? -rate.Minimum : rate.Minimum) + "#";
            }
         }
         else
         {
            // Fallback rate...
            command = ":RS#";
         }

         if (Axis == TelescopeAxes.axisPrimary)
         {
            Tuple<bool, string> retVals = new Tuple<bool, string>(AxisRates.ratesApproxEqual(DriverStateBase.lastPrimaryAxisRate, rate.Minimum), command);
            DriverStateBase.lastPrimaryAxisRate = rate.Minimum;
            return retVals;
         }
         else
         {
            Tuple<bool, string> retVals = new Tuple<bool, string>(AxisRates.ratesApproxEqual(DriverStateBase.lastSecondaryAxisRate, rate.Minimum), command);
            DriverStateBase.lastSecondaryAxisRate = rate.Minimum;
            return retVals;
         }
      }

      /// <summary>
      /// Slews westOrNorth if true, else eastOrSouth if false. Whether E/W or N/S is determined by the set Axis.
      /// </summary>
      /// <returns>Serial command string for movement.</returns>
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
   internal class SlewPrimaryAxisState : PrimaryAxisMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      private bool movingW;

      /// <summary>
      /// Move the axis E at the requested rate.
      /// </summary>
      /// <returns>this after issuing the command to move the mount.</returns>
      internal override DriverStateBase MoveAxisEast(Rate rate)
      {
         movingW = false;
         return InternalMoveAxis(rate);
      }
      /// <summary>
      /// Move the axis W at the requested rate.
      /// </summary>
      /// <returns>this after issuing the command to move the mount.</returns>
      internal override DriverStateBase MoveAxisWest(Rate rate)
      {
         movingW = true;
         return InternalMoveAxis(rate);
      }
      /// <summary>
      /// Actually issue the commands to the mount and return this.
      /// </summary>
      /// <returns>this</returns>
      private DriverStateBase InternalMoveAxis(Rate rate)
      {
         // First set desired slew rate.
         Tuple<bool, string> rateCommand = MoveAxisHelper.RateToRateCommandString(TelescopeAxes.axisPrimary, rate);
         if (rateCommand.Item1)
         {
            serialPort.Transmit(rateCommand.Item2);
         }
         // Then start the axis moving and leave it moving.
         serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisPrimary, movingW));
         return this;
      }
      /// <summary>
      /// Slew direction aware AbortSlew. Thus slews on the other axis are unaffected.
      /// </summary>
      /// <returns>The SavedAxisState.ResumeTracking() result.</returns>
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
      /// <summary>
      /// Given the fact this might be the MasterCurrentState, this ensures that we can kick off a slew for the secondary axis from
      /// the MasterCurrentState.
      /// 
      /// If the request is a MoveAxis for us, we ignore it. Protocol is AbortSlew() first, then issue a new MoveAxis command.
      /// </summary>
      /// <returns>New secondary axis state or this.</returns>
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
      public override DriverStateBase ResumeTracking(TelescopeAxes Axis)
      {
         // Worst that can happen is we return this.
         if (Axis == TelescopeAxes.axisTertiary)
         {
            throw new ASCOM.InvalidValueException("Tertiary axis is not supported.");
         }
         // InternalResumeTracking so we can't infinitely recurse to death.
         return MoveAxisHelper.MoveAxisStateFor(Axis).InternalResumeTracking();
      }
      /// <summary>
      /// Go back to whatever tracking state is valid for our axis.
      /// </summary>
      /// <returns>Correct saved state for the axis.</returns>
      public override DriverStateBase ResumeTracking()
      {
         return InternalResumeTracking();
      }
      internal override DriverStateBase InternalResumeTracking()
      {
         // Looks like AbortSlew is all that is required.
         return AbortSlew();
      }
   }

   /// <summary>
   /// Single axis state for slew on the secondary axis. Returns to saved secondary axis state when done or aborted.
   /// Provides a specialised AbortSlew that only aborts the slew on the moving N/S axis.
   /// </summary>
   internal class SlewSecondaryAxisState : SecondaryAxisMovingState
   {
      internal override bool InternalIsSlewing() { return true; }
      private bool movingN = true;
      /// <summary>
      /// Move the axis N at the requested rate.
      /// </summary>
      /// <returns>this after issuing the command to move the mount.</returns>
      internal override DriverStateBase MoveAxisNorth(Rate rate)
      {
         movingN = true;
         return InternalMoveAxis(rate);
      }
      /// <summary>
      /// Move the axis S at the requested rate.
      /// </summary>
      /// <returns>this after issuing the command to move the mount.</returns>
      internal override DriverStateBase MoveAxisSouth(Rate rate)
      {
         movingN = false;
         return InternalMoveAxis(rate);
      }
      private DriverStateBase InternalMoveAxis(Rate rate)
      {
         // First set desired slew rate.
         Tuple<bool, string> rateCommand = MoveAxisHelper.RateToRateCommandString(TelescopeAxes.axisPrimary, rate);
         if (rateCommand.Item1)
         {
            serialPort.Transmit(rateCommand.Item2);
         }
         // Then slew in the direction that will move the telescope in the direction requested (note: this may require Up/Dn reversal).
         serialPort.Transmit(MoveAxisHelper.AxisToMovementDirectionCommandString(TelescopeAxes.axisSecondary, movingN));
         return this;
      }
      /// <summary>
      /// Slew direction aware AbortSlew. Thus slews on the other axis are unaffected.
      /// </summary>
      /// <returns>The SavedAxisState.ResumeTracking() result.</returns>
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
      /// <summary>
      /// Resume tracking on the secondary axis.
      /// </summary>
      public override DriverStateBase ResumeTracking()
      {
         return InternalResumeTracking();
      }
      internal override DriverStateBase InternalResumeTracking()
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
      /// <summary>
      /// Given the axis and degrees per second rate, obtain the appropriate PrimaryAxis/SecondaryAxis moving state
      /// and request the move N/S/E/W as applies.
      /// </summary>
      /// <returns>Axis moving state for the given axis, direction and rate.</returns>
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

      /// <summary>
      /// Translate the ASCOM Axis represented to the MasterCurrentState (primary axis) or SecondaryAxisCurrentState as applies.
      /// </summary>
      /// <returns>Current state object for the given axis.</returns>
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
   /// PulseGuidePrimaryAxisState. IsPulseGuiding() returns true.
   /// Valid state transitions are those of PrimaryAxisMovingState.
   /// 
   /// This state exists to ensure that we don't attempt a pulse guide on an axis until the prior pulse
   /// guide is likely to be complete on that axis. It also does range checking on the pulse guide
   /// time in ms.
   /// 
   /// A new pulse guide request on an already pulse guiding axis results in the current pulse guide being
   /// cancelled (its task is stopped) and the issuing of a new pulse guide command. This behaviour seems to
   /// be what is expected by the likes of PHD/2 Guiding.
   /// 
   /// @caveat We presume the time asked for is based upon the guide rate we provide.
   /// </summary>
   internal class PulseGuidePrimaryAxisState : PrimaryAxisMovingState
   {
      /// <summary>
      /// PulseGuidePrimaryAxisState can redirect requests to secondary axis state based upon the given Direction.
      /// If the pulse guide request is for the primary axis, the current pulse guide is aborted and the new
      /// PulseGuideEast/West() result returned as the next state.
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
      /// <summary>
      /// Pulse guide West for the given duration.
      /// </summary>
      /// <returns>The new pulse guding state.</returns>
      internal override DriverStateBase PulseGuideWest(int durationMs)
      {
         guideW = true;
         return InternalPulseGuide(durationMs);
      }
      /// <summary>
      /// Pulse guide East for the given duration.
      /// </summary>
      /// <returns>The new pulse guding state.</returns>
      internal override DriverStateBase PulseGuideEast(int durationMs)
      {
         guideW = false;
         return InternalPulseGuide(durationMs);
      }
      /// <summary>
      /// Here we apply the driver setup pulse guiding algorithm. Broadly these are:
      ///    1. Pulse guide commands. These are the Meade #497 serial commands for pulse guiding and include the duration.
      ///    2. #RA...# command. We set the rate using the #RA...# command and start a task that runs for approximately the duration
      ///       and then stops the slewing at the rate specified.
      ///    3. Meade mount move commands at sidereal rate and start a task that runs for approximately the duration
      ///       and then stops the slewing at the rate specified. 
      /// </summary>
      /// <returns>The new pulse guiding state for the primary axis.</returns>
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

            // Only issue :RA if the rate is not what we have already set.
            if (!AxisRates.ratesApproxEqual(guideRate, lastPrimaryAxisRate))
            {
               serialPort.Transmit(":RA" + String.Format("{0:0.0####;-0.0####}", guideRate) + "#");
               DriverStateBase.lastPrimaryAxisRate = guideRate;
            }
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
               MountTaskHandler.Instance.WorkQueue.Add(() =>
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
               MountTaskHandler.Instance.WorkQueue.Add(() =>
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

      public override DriverStateBase AbortSlew()
      {
         return InternalPulseGuideComplete();
      }
   }

   /// <summary>
   /// PulseGuideSecondaryAxisState. IsPulseGuiding() returns true.
   /// Valid state transitions are those of SecondaryAxisMovingState.
   /// 
   /// This state exists to ensure that we don't attempt a pulse guide on an axis until the prior pulse
   /// guide is likely to be complete on that axis. It also does range checking on the pulse guide
   /// time in ms.
   /// 
   /// A new pulse guide request on an already pulse guiding axis results in the current pulse guide being
   /// cancelled (its task is stopped) and the issuing of a new pulse guide command. This behaviour seems to
   /// be what is expected by the likes of PHD/2 Guiding.
   /// 
   /// @caveat We presume the time asked for is based upon the guide rate we provide.
   /// </summary>
   internal class PulseGuideSecondaryAxisState : SecondaryAxisMovingState
   {
      internal override bool InternalIsPulseGuiding() { return true; }
      private bool guideN;
      /// <summary>
      /// Pulse guide North for the given duration.
      /// </summary>
      /// <returns>The new pulse guding state.</returns>
      internal override DriverStateBase PulseGuideNorth(int durationMs)
      {
         guideN = true;
         return InternalPulseGuide(durationMs);
      }
      /// <summary>
      /// Pulse guide South for the given duration.
      /// </summary>
      /// <returns>The new pulse guding state.</returns>
      internal override DriverStateBase PulseGuideSouth(int durationMs)
      {
         guideN = false;
         return InternalPulseGuide(durationMs);
      }
      /// <summary>
      /// Here we apply the driver setup pulse guiding algorithm. Broadly these are:
      ///    1. Pulse guide commands. These are the Meade #497 serial commands for pulse guiding and include the duration.
      ///    2. #RE...# command. We set the rate using the #RE...# command and start a task that runs for approximately the duration
      ///       and then stops the slewing at the rate specified.
      ///    3. Meade mount move commands at sidereal rate and start a task that runs for approximately the duration
      ///       and then stops the slewing at the rate specified. 
      /// </summary>
      /// <returns>The new pulse guiding state for the secondary axis.</returns>
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

            // Only issue :RE if the rate is not what we have already set.
            if (!AxisRates.ratesApproxEqual(guideRate, DriverStateBase.lastSecondaryAxisRate))
            {
               serialPort.Transmit(":RE" + String.Format("{0:0.0####;-0.0####}", guideRate) + "#");
               DriverStateBase.lastSecondaryAxisRate = guideRate;
            }
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
               MountTaskHandler.Instance.WorkQueue.Add(() =>
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
               MountTaskHandler.Instance.WorkQueue.Add(() =>
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

      public override DriverStateBase AbortSlew()
      {
         return InternalPulseGuideComplete();
      }
   }

   /// <summary>
   /// Slewing to Alt/Az async. Synchronous versions are not supported.
   /// Valid state transitions are those of DualAxisSlewingState.
   /// 
   /// The mount is polled in a lazy worker thread so that we don't prematurely return
   /// false for Slewing(). i.e. When the HW returns not polling, we call ResumeTracking().
   /// 
   /// Otherwise AbortSlew() will return us to a tracking state on both axes.
   /// </summary>
   internal class SlewToAltAzState : DualAxisSlewingState
   {
      /// <summary>
      /// Command the mount to slew to the given Alt/Az. The new state is set for MasterCurrentState and SecondaryCurrentState so that all
      /// operation requests on either axis are routed to the correct DualAxisMovingState.
      /// </summary>
      /// <caveats>Alt/Az is only valid for an Alt/Az mounted scope. Alt/Az is base relative and not horizon relative.</caveats>
      /// <returns>SlewToAltAzState.</returns>
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
                     MountTaskHandler.Instance.WorkQueue.Add(() => hwSlewingTask.RunSynchronously());
                     if (isHwSlewing = hwSlewingTask.Result)
                     {
                        ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Don't hammer the mount. We are doing a slew.
                        System.Threading.Thread.Sleep(100);
                     }
                  }

                  MountTaskHandler.Instance.WorkQueue.Add(() => 
                     // If still the expected state, then this returns all the
                     // axes states to what they were before we kicked off the slew.
                     CurrentState = MasterCurrentState.ResumeTracking());
                  return true;
               }
               catch (System.OperationCanceledException)
               {
                  // Means Abort so no slew completion and states all returned to what they should be.
                  return false;
               }
               catch (Exception e)
               {
                  MountTaskHandler.Instance.WorkQueue.Add(() => 
                     CurrentState = MasterCurrentState.AbortSlew());
                  throw e;
               }
            }, ScopeMovementCancellationTokenSource.Token);
         }
         catch (Exception e)
         {
            MountTaskHandler.Instance.WorkQueue.Add(() => 
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
   /// false for Slewing(). i.e. When the HW returns not polling, we call ResumeTracking().
   /// 
   /// Otherwise AbortSlew() will return us to a tracking state on both axes.
   /// </summary>
   internal class SlewToCoordinatesState : DualAxisSlewingState
   {
      /// <summary>
      /// Command the mount to slew to the given celestial coordinates. The new state is set for MasterCurrentState and SecondaryCurrentState so that all
      /// operation requests on either axis are routed to the correct DualAxisMovingState.
      /// </summary>
      /// <returns>SlewToCoordinatesState.</returns>
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
                     MountTaskHandler.Instance.WorkQueue.Add(() => hwSlewingTask.RunSynchronously());
                     if (isHwSlewing = hwSlewingTask.Result)
                     {
                        ScopeMovementCancellationTokenSource.Token.ThrowIfCancellationRequested();
                        // Don't hammer the mount. We are doing a slew.
                        System.Threading.Thread.Sleep(100);
                     }
                  }

                  // HW no longer reports slewing so queue slew complete.
                  MountTaskHandler.Instance.WorkQueue.Add(() =>
                     // If still the expected state, then this returns all the
                     // axes states to what they were before we kicked off the slew.
                     CurrentState = MasterCurrentState.ResumeTracking());
                  return true;
               }
               catch (System.OperationCanceledException)
               {
                  // Means Abort so no slew completion and states all returned to what they should be.
                  return false;
               }
               catch (Exception e)
               {
                  MountTaskHandler.Instance.WorkQueue.Add(() =>
                     CurrentState = MasterCurrentState.AbortSlew());
                  throw e;
               }
            }, ScopeMovementCancellationTokenSource.Token);
         }
         catch (Exception e)
         {
            MountTaskHandler.Instance.WorkQueue.Add(() =>
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
      public override DriverStateBase ResumeTracking(TelescopeAxes Axis)
      {
         // Worst that can happen is we return this.
         if (Axis == TelescopeAxes.axisTertiary)
         {
            throw new ASCOM.InvalidValueException("Tertiary axis is not supported.");
         }
         // InternalResumeTracking so we can't infinitely recurse to death.
         return MoveAxisHelper.MoveAxisStateFor(Axis).InternalResumeTracking();
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

      /// <summary>
      /// If we are tracking we can slew to Alt/Az.
      /// </summary>
      /// <returns>New dual axis Alt/Az slewing state if tracking otherwise no-op.</returns>
      public override DriverStateBase SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         if (MasterCurrentState.IsTracking())
            return (new SlewToAltAzState()).SlewToAltAzAsync(Azimuth, Altitude);
         else
            return this;
      }

      /// <summary>
      /// If we are tracking we can slew to celestial coordinates.
      /// </summary>
      /// <returns>New dual axis celestial coordinates slewing state if tracking otherwise no-op.</returns>
      public override DriverStateBase SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         if (MasterCurrentState.IsTracking())
            return (new SlewToCoordinatesState()).SlewToCoordinatesAsync(RightAscension, Declination);
         else
            return this;
      }

      /// <summary>
      /// Sync the tracking mount to an Alt/Az. i.e. Now slewing mount can have its Alt/Az syncd mid slew.
      /// </summary>
      /// <returns>this.</returns>
      public override DriverStateBase SyncToAltAz(double Azimuth, double Altitude)
      {
         if (!MasterCurrentState.IsTracking())
            return this;
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

         serialPort.Transmit(":CM#");
         serialPort.ReceiveTerminated("#");
         return this;
      }

      /// <summary>
      /// Sync the tracking mount to celestial coordinates. i.e. Now slewing mount can have its celestial coordinates syncd mid slew.
      /// </summary>
      /// <returns>this.</returns>
      public override DriverStateBase SyncToCoordinates(double RightAscension, double Declination)
      {
         if (!MasterCurrentState.IsTracking())
            return this;
         string RA = utilities.DegreesToHMS(RightAscension);
         RA = RA.Substring(0, 5) + Math.Round(Decimal.Parse(RA.Substring(6, 2)) / new Decimal(60), 1).ToString();
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

      /// <summary>
      /// Pulse guide north for the given duration if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
      ///   3. The other axis is doing a pulse guide - we don't care.
      /// </summary>
      /// <returns>New pulse guide state north for given duration if no one is slewing.</returns>
      internal override DriverStateBase PulseGuideNorth(int durationMs)
      {
         if (!MasterCurrentState.InternalIsSlewing())
            return (new PulseGuideSecondaryAxisState()).PulseGuideNorth(durationMs);
         else
            return this;
      }

      /// <summary>
      /// Pulse guide south for the given duration if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
      ///   3. The other axis is doing a pulse guide - we don't care.
      /// </summary>
      /// <returns>New pulse guide state south for given duration if no one is slewing.</returns>
      internal override DriverStateBase PulseGuideSouth(int durationMs)
      {
         if (!MasterCurrentState.InternalIsSlewing())
            return (new PulseGuideSecondaryAxisState()).PulseGuideSouth(durationMs);
         else
            return this;
      }

      /// <summary>
      /// Move the axis north at the supplied rate if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we don't care.
      ///   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
      /// </summary>
      /// <returns>New axis moving state north at the given rate if no one is slewing.</returns>
      internal override DriverStateBase MoveAxisNorth(Rate rate)
      {
         if (!MasterCurrentState.IsPulseGuiding())
            return (new SlewSecondaryAxisState()).MoveAxisNorth(rate);
         else
            return this;
      }

      /// <summary>
      /// Move the axis south at the supplied rate if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we don't care.
      ///   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
      /// </summary>
      /// <returns>New axis moving state south at the given rate if no one is slewing.</returns>
      internal override DriverStateBase MoveAxisSouth(Rate rate)
      {
         if (!MasterCurrentState.IsPulseGuiding())
            return (new SlewSecondaryAxisState()).MoveAxisSouth(rate);
         else
            return this;
      }
   }

   /// <summary>
   /// This represents the primary axis tracking state. We are always moving at sidereal or lunar rate, never still.
   /// </summary>
   partial class PrimaryAxisTrackingState : TrackingState
   {
      /// <summary>
      /// Pulse guide East for the given duration.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
      ///   3. The other axis is doing a pulse guide - we don't care.
      /// </summary>
      /// <returns>New pulse guiding state if no one is slewing, otherwise a no-op.</returns>
      internal override DriverStateBase PulseGuideEast(int durationMs)
      {
         if (!SecondaryAxisCurrentState.InternalIsSlewing())
            return (new PulseGuidePrimaryAxisState()).PulseGuideEast(durationMs);
         else
            return this;
      }

      /// <summary>
      /// Pulse guide West for the given duration.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we do care. Mixing pulse guiding and a slew is nonsense.
      ///   3. The other axis is doing a pulse guide - we don't care.
      /// </summary>
      /// <returns>New pulse guiding state if no one is slewing, otherwise a no-op.</returns>
      internal override DriverStateBase PulseGuideWest(int durationMs)
      {
         if (!SecondaryAxisCurrentState.InternalIsSlewing())
            return (new PulseGuidePrimaryAxisState()).PulseGuideWest(durationMs);
         else
            return this;
      }

      /// <summary>
      /// Move the primary axis East at the given rate if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we don't care.
      ///   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
      /// </summary>
      /// <returns>New eastward slewing state if valid otherwise a no-op.</returns>
      internal override DriverStateBase MoveAxisEast(Rate rate)
      {
         if (!SecondaryAxisCurrentState.IsPulseGuiding())
            return (new SlewPrimaryAxisState()).MoveAxisEast(rate);
         else
            return this;
      }

      /// <summary>
      /// Move the primary axis West at the given rate if valid.
      /// 
      /// This axis is not slewing, that we know.
      /// The other axis can't be doing a dual axis goto because this method is not
      /// callable if that is the case.
      /// So the only possibilities are:
      ///   1. The other axis is tracking - we don't care.
      ///   2. The other axis is doing a single axis slew - we don't care.
      ///   3. The other axis is doing a pulse guide - we do care. Mixing pulse guiding and a slew is nonsense.
      /// </summary>
      /// <returns>New westward slewing state if valid otherwise a no-op.</returns>
      internal override DriverStateBase MoveAxisWest(Rate rate)
      {
         if (!SecondaryAxisCurrentState.IsPulseGuiding())
            return (new SlewPrimaryAxisState()).MoveAxisWest(rate);
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
