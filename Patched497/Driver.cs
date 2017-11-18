//tabs=4
// --------------------------------------------------------------------------------
// ASCOM Telescope driver for LX90
//
// Description:	Dick Seymour and others have patched the Meade #497 handbox ROM adding various
//                features. Noticably are the custom guide rates and focuser (Meade APM #909)
//                control.
//
//                With a patched Meade #497, 0.33, 0.67 and 1.00 × sidereal rate are possible.
//
//                Additionally the LX90 mounts can be badly behaved in terms of the real guide rate
//                they achieve and some applications seem to be confused by guide rates other than
//                pure sidereal.
//
//                This driver seeks to remedy some of that behavour such as when guiding E is stop
//                - which is guaranteed to be sidereal rate E, and guiding W fails to behave as the
//                W going equivalent. 
//
//                Guiding for the Meade #497 is guide E or W at the guide rate for time t then return
//                to sidereal rate. So if t is constant and t(East) = n px and t(West) = m px, 
//                then n/m * t(West) will ensure that the same requested t (West) is scaled such
//                that the same number of px are moved for any given requested identical t.
//
//                E.g. 1s (East) = 14 px. 1s (West) = 8 px. So every requested guide East for 1s moves
//                nearly twice as far as every 1s guide West for 1s. We can't change the guide rate, but
//                we can change the length of time we guide East or West for every requested time unit.
//                
//                1s (East) = 14 px. 1s (West) becomes 14/8 * 1s = 1.75s (West) = 14 px.
//
//                Generalising we can call one Faster and the other Slower. We can guide for longer in the
//                Slower by applying a coefficient to the requested guide time length (Slower):
//
//                i.e. t (Faster) = n px. t (Slower) = m px. t (Faster) = n px = n/m * t (Slower)
//
//                Conversely you can decide to "slow" the faster direction down. That is guide for less time in the
//                Faster direction by applying a coefficient to the requested guide time length (Faster):
//
//                i.e. t (Faster) = n px. t (Slower) = m px. t (Slower) = m px = m/n * t (Faster)
//
//                Either way we use a simple coeficient to scale the length of a guide slew on either the Faster
//                or Slower direction such that the same requested time t on either yields the same number of px moved.
//
//                The driver set up provides the ability to set sidereal rate and an asymmetric correction that
//                can be independently applied to either the E or W direction.
//
// Implements:	ASCOM Telescope interface version: 3
// Author:		(CRK) Carl Knight. SleeplessAtKnight@gmail.com
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// 17-09-2017	CRK	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code canbe deleted and this definition removed.
#define Telescope

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Runtime.InteropServices;

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Utilities;
using ASCOM.DeviceInterface;
using System.Globalization;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace ASCOM.LX90
{
   //
   // Your driver's DeviceID is ASCOM.LX90.Telescope
   //
   // The Guid attribute sets the CLSID for ASCOM.LX90.Telescope
   // The ClassInterface/None addribute prevents an empty interface called
   // _LX90 from being created and used as the [default] interface
   //
   // TODO Replace the not implemented exceptions with code to implement the function or
   // throw the appropriate ASCOM exception.
   //

   /// <summary>
   /// ASCOM Telescope Driver for LX90.
   /// </summary>
   [Guid("d883df41-8dd5-4682-8269-66f21a20e54c")]
   [ClassInterface(ClassInterfaceType.None)]
   public class Telescope : ITelescopeV3
   {
      /// <summary>
      /// ASCOM DeviceID (COM ProgID) for this driver.
      /// The DeviceID is used by ASCOM applications to load the driver at runtime.
      /// </summary>
      internal static string driverID = "ASCOM.LX90.Telescope";
      // TODO Change the descriptive string for your driver then remove this line
      /// <summary>
      /// Driver description that displays in the ASCOM Chooser.
      /// </summary>
      private static string driverDescription = "Patched Meade #497 (Etx/LX90)";

      internal static string comPortProfileName = "COM Port"; // Constants used for Profile persistence
      internal static string comPortDefault = "COM1";
      internal static string traceStateProfileName = "Trace Level";
      internal static string traceStateDefault = "false";
      internal static string verboseProfilename = "Trace Verbose";
      internal static string verboseDefault = "false";
      internal static string guideSlewCoefDirectionProfileName = "Guide Slew Coef Direction";
      internal static string guideSlewCoefDirectionDefault = "East";
      internal static string guideSlewCoefProfileName = "Guide Slew Coef";
      internal static string guideSlewCoefDefault = "1.00";
      internal static string guideRateProfileName = "Guide Rate";
      internal static string guideRateDefault = "1.00";
      internal static string focalLengthProfileName = "Focal Length";
      internal static string focalLengthDefault = "3048";
      internal static string apertureProfileName = "Aperture";
      internal static string apertureDefault = "304.8";
      internal static string secondaryDiameterProfileName = "Secondary Diameter";
      internal static string secondaryDiameterDefault = "100.0";
      internal static string elevationProfileName = "Elevation";
      internal static string elevationDefault = "100";
      internal static string hasCustomRatesProfileName = "Has RA/Dec Axis Custom Rates";
      internal static string hasCustomRatesDefault = "true";
      internal static string negRAMovesEProfileName = "Negative :RA# moves E";
      internal static string negRAMovesEDefault = "true";
      internal static string negREMovesNProfileName = "Negative :RE# moves N";
      internal static string negREMovesNDefault = "true";
      internal static string guideRateAlgorithmProfileName = "Guide rate algorithm";
      internal static string guideRateAlgorithmDefault = "Pulse Guide";
      internal static string guideRateAlgorithmRaRe = ":RA#/:RE#";
      internal static string guideRateAlgorithmMove = "Move Axis";

      internal static string comPort; // Variables to hold the currrent device configuration
      internal static GuideDirections guideSlewCoefDirection;
      internal static double guideSlewCoef;
      internal static double guideRate;
      internal static double focalLength;
      internal static double aperture;
      internal static double secondaryDiameter;
      internal static double elevation;
      internal static bool hasCustomRates;
      internal static bool negRAMovesE;
      internal static bool negREMovesN;
      internal static string guideRateAlgorithm;
      internal static bool verbose;

      /// <summary>
      /// Private variable to hold an ASCOM Utilities object
      /// </summary>
      private Util utilities;

      /// <summary>
      /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
      /// </summary>
      private AstroUtils astroUtilities;

      /// <summary>
      /// Variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
      /// </summary>
      internal static TraceLogger tl;

      /// <summary>
      /// Initializes a new instance of the <see cref="LX90"/> class.
      /// Must be public for COM registration.
      /// </summary>
      public Telescope()
      {
         tl = new TraceLogger("", "LX90");
         ReadProfile(); // Read device configuration from the ASCOM Profile store

         if (tl.Enabled)
         {
            LogMessage("Telescope - ", "Starting initialisation");
         }

         utilities = new Util(); //Initialise util object
         astroUtilities = new AstroUtils(); // Initialise astro utilities object

         Task initialiseTask = new Task(() =>
         {
            if (DriverStateBase.MasterCurrentState == null)
            {
               DriverStateBase.CurrentState = new DisconnectedState();
            }
            else if(DriverStateBase.MasterCurrentState.IsConnected())
            {
               if (tl.Enabled)
               {
                  LogMessage("Telescope - ", "Already connected.");
               }
            }
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => initialiseTask.RunSynchronously());
         initialiseTask.Wait();

         if (tl.Enabled)
         {
            LogMessage("Telescope - ", "Completed initialisation");
         }
      }


      //
      // PUBLIC COM INTERFACE ITelescopeV3 IMPLEMENTATION
      //

      #region Common properties and methods.

      /// <summary>
      /// Meade serial comms are very inconsistent and alas don't fit a pattern that
      /// makes these methods useful.
      /// </summary>
      public void CommandBlind(string Command, bool Raw = false)
      {
         throw new ASCOM.NotImplementedException("CommandBlind");
      }
      /// <summary>
      /// Meade serial comms are very inconsistent and alas don't fit a pattern that
      /// makes these methods useful.
      /// </summary>
      public bool CommandBool(string Command, bool Raw = false)
      {
         throw new ASCOM.NotImplementedException("CommandBool");
      }
      /// <summary>
      /// Meade serial comms are very inconsistent and alas don't fit a pattern that
      /// makes these methods useful.
      /// </summary>
      public string CommandString(string Command, bool Raw = false)
      {
         throw new ASCOM.NotImplementedException("CommandString");
      }

      /// <summary>
      /// Displays the Setup Dialog form.
      /// If the user clicks the OK button to dismiss the form, then
      /// the new settings are saved, otherwise the old values are reloaded.
      /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
      /// </summary>
      public void SetupDialog()
      {
         // consider only showing the setup dialog if not connected
         // or call a different dialog if connected
         if (IsConnected)
            System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

         using (SetupDialogForm F = new SetupDialogForm())
         {
            var result = F.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
               WriteProfile(); // Persist device configuration values to the ASCOM Profile store
            }
         }
      }

      private ArrayList supportedActions = new ArrayList();
      public ArrayList SupportedActions
      {
         get
         {
            if (tl.Enabled)
            {
               LogMessage("SupportedActions - ", "Returning arraylist");
            }
            if (supportedActions.Count == 0)
            {
               supportedActions.Add("AbortSlew".ToLower());
               supportedActions.Add("Action".ToLower());
               supportedActions.Add("CanMoveAxis".ToLower());
               supportedActions.Add("AxisRates".ToLower());
               supportedActions.Add("MoveAxis".ToLower());
               supportedActions.Add("Park".ToLower());
               supportedActions.Add("PulseGuide".ToLower());
               supportedActions.Add("SetupDialog".ToLower());
               supportedActions.Add("SlewToAltAzAsync".ToLower());
               supportedActions.Add("SlewToCoordinatesAsync".ToLower());
               supportedActions.Add("SlewToTargetAsync".ToLower());
            }
            return supportedActions;
         }
      }

      public string Action(string actionName, string actionParameters)
      {
         int actionIdx = supportedActions.BinarySearch(actionName.ToLower().Replace("telescope:", ""));
         if (actionIdx >= 0)
         {
            return String.Format("{0}({1}) is supported. See the ASCOM.ITelescopeV3 interface to check parameters and return types.", actionName, actionParameters);
         }
         if (tl.Enabled)
         {
            LogMessage("Action - ", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
         }
         throw new ASCOM.ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
      }

      public void Dispose()
      {
         Task ensureDisconnectedTask = new Task(() =>
         {
            DriverStateBase.MasterCurrentState.Disconnect();
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => ensureDisconnectedTask.RunSynchronously());
         ensureDisconnectedTask.Wait();
         // Clean up the tracelogger and util objects
         tl.Enabled = false;
         tl.Dispose();
         tl = null;
         utilities.Dispose();
         utilities = null;
         astroUtilities.Dispose();
         astroUtilities = null;
      }

      public bool Connected
      {
         get
         {
            Task<bool> isConnectedTask = new Task<bool>(() => 
            { 
               return DriverStateBase.MasterCurrentState.IsConnected(); 
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => isConnectedTask.RunSynchronously());
            isConnectedTask.Wait();
            bool connected = isConnectedTask.Result;
            if (verbose)
            {
               LogMessage("Connected - ", "Get {0}", connected.ToString());
            }
            return connected;
         }
         set
         {
            Task connectTask = new Task(() =>
            {
               if (value != DriverStateBase.MasterCurrentState.IsConnected())
               {
                  if (verbose)
                  {
                     LogMessage("Connected - ", "Set {0}", value.ToString());
                  }

                  if (value)
                  {
                     DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.Connect(int.Parse(comPort.Replace("COM","")));
                  }
                  else
                  {
                     DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.Disconnect();
                  }
               }
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => connectTask.RunSynchronously());
            connectTask.Wait();
         }
      }

      public string Description
      {
         // TODO customise this device description
         get
         {
            if (tl.Enabled)
            {
               LogMessage("Description Get - ", driverDescription);
            }
            return driverDescription;
         }
      }

      public string DriverInfo
      {
         get
         {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // TODO customise this driver description
            string driverInfo = driverDescription + " Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            if (tl.Enabled)
            {
               LogMessage("DriverInfo Get - ", driverInfo.ToString());
            }
            return driverInfo;
         }
      }

      public string DriverVersion
      {
         get
         {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            if (tl.Enabled)
            {
               LogMessage("DriverVersion Get - ", driverVersion);
            }
            return driverVersion;
         }
      }

      public short InterfaceVersion
      {
         // set by the driver wizard
         get
         {
            if (tl.Enabled)
            {
               LogMessage("InterfaceVersion Get - ", "3");
            }
            return Convert.ToInt16("3");
         }
      }

      public string Name
      {
         get
         {
            string name = "Patched #497 (Etx/LX90)";
            if (tl.Enabled)
            {
               LogMessage("Name Get - ", name);
            }
            return name;
         }
      }

      #endregion

      #region ITelescope Implementation

      internal void throwOnDisconnectedOrParked() //throws ASCOM.ParkedException, ASCOM.NotConnectedException
      {
         if (AtPark)
         {
            throw new ASCOM.ParkedException();
         }
         else if (!Connected)
         {
            throw new ASCOM.NotConnectedException();
         }
      }

      public void AbortSlew()
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            LogMessage("AbortSlew - ", "Abort in progress...");
         }
         Task abortSlewTask = new Task(() =>
         {
            // Current state looks at the state and updates the correct axis based upon
            // the returned state.
            DriverStateBase.CurrentState = DriverStateBase.SecondaryAxisCurrentState.AbortSlew();
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.AbortSlew();
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => abortSlewTask.RunSynchronously());
         abortSlewTask.Wait();
         if (tl.Enabled)
         {
            LogMessage("AbortSlew - ", "Abort complete.");
         }
      }

      public AlignmentModes AlignmentMode
      {
         get
         {
            Task<AlignmentModes> alignmentModeTask = new Task<AlignmentModes>(() =>
            {
               return DriverStateBase.MasterCurrentState.AlignmentMode();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => alignmentModeTask.RunSynchronously());
            alignmentModeTask.Wait();
            if (verbose)
            {
               LogMessage("AlignmentMode Get - ", alignmentModeTask.Result.ToString());
            }
            return alignmentModeTask.Result;
         }
      }

      public double Altitude
      {
         get
         {
            Task<double> altTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.Altitude();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => altTask.RunSynchronously());
            altTask.Wait();
            if (verbose)
            {
               LogMessage("Altitude Get - ", altTask.Result.ToString());
            }
            return altTask.Result;
         }
      }

      public double ApertureArea
      {
         get
         {
            double secondaryRadius = 0.5 * secondaryDiameter;
            double secondaryArea = Math.PI * (secondaryRadius * secondaryRadius); // mm^2
            double apertureRadius = 0.5 * aperture;
            double apertureArea = Math.PI * (apertureRadius * apertureRadius) - secondaryArea; // mm^2
            apertureArea /= 10E6; // mm^2 to m^2
            if(verbose)
            {
               LogMessage("ApertureArea Get - ", apertureArea.ToString());
            }
            return apertureArea;
         }
      }

      public double ApertureDiameter
      {
         get
         {
            double apertureDia = aperture / 1000; // mm to m
            if (verbose)
            {
               LogMessage("ApertureDiameter Get - ", apertureDia.ToString());
            }
            return apertureDia;
         }
      }

      public bool AtHome
      {
         get
         {
            if (verbose)
            {
               LogMessage("AtHome Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool AtPark
      {
         get
         {
            Task<bool> atParkTask = new Task< bool >(() =>
            {
               return DriverStateBase.MasterCurrentState.IsParked();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => atParkTask.RunSynchronously());
            atParkTask.Wait();
            if (verbose)
            {
               LogMessage("AtPark Get - ", atParkTask.Result.ToString());
            }
            return atParkTask.Result;
         }
      }

      public IAxisRates AxisRates(TelescopeAxes Axis)
      {
         if (verbose)
         {
            LogMessage("AxisRates Get - ", Axis.ToString());
         }
         return new AxisRates(Axis);
      }

      public double Azimuth
      {
         get
         {
            Task<double> azTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.Azimuth();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => azTask.RunSynchronously());
            azTask.Wait();
            if (verbose)
            {
               LogMessage("Azimuth Get - ", azTask.Result.ToString());
            }
            return azTask.Result;
         }
      }

      public bool CanFindHome
      {
         get
         {
            // Apparently it can. :hF# is the command.
            // Implement if there is ever demand.
            if (verbose)
            {
               LogMessage("CanFindHome Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanMoveAxis(TelescopeAxes Axis)
      {
         if (verbose)
         {
            LogMessage("CanMoveAxis Get - ", Axis.ToString());
         }
         switch (Axis)
         {
            case TelescopeAxes.axisPrimary: return true; // Az
            case TelescopeAxes.axisSecondary: return true; // Alt
            case TelescopeAxes.axisTertiary: return false; // No such thing.
            default: throw new InvalidValueException("CanMoveAxis", Axis.ToString(), "0 and 1");
         }
      }

      public bool CanPark
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yep can park.
            if (verbose)
            {
               LogMessage("CanPark Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanPulseGuide
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yep can pulse guide.
            if (verbose)
            {
               LogMessage("CanPulseGuide Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanSetDeclinationRate
      {
         get
         {
            // Offset tracking not supported.
            if (verbose)
            {
               LogMessage("CanSetDeclinationRate Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSetGuideRates
      {
         get
         {
            // Guide rates at present are static.
            return false;
         }
      }

      public bool CanSetPark
      {
         get
         {
            // No. Park position is firmware based.
            if (verbose)
            {
               LogMessage("CanSetPark Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSetPierSide
      {
         get
         {
            // Mount is not on a flippable mount.
            if (verbose)
            {
               LogMessage("CanSetPierSide Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSetRightAscensionRate
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Offset tracking unsupported.
            if (verbose)
            {
               LogMessage("CanSetRightAscensionRate Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSetTracking
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yes, sidereal/lunar tracking can be turned on and off.
            if (verbose)
            {
               LogMessage("CanSetTracking Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanSlew
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Can slew to RA and Dec async only.
            if (verbose)
            {
               LogMessage("CanSlew Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSlewAltAz
      {
         get
         {
            // Can slew to Alt/Az async only
            if (verbose)
            {
               LogMessage("CanSlewAltAz Get - ", false.ToString());
            }
            return false;
         }
      }

      public bool CanSlewAltAzAsync
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yes. Does so in a future task.
            if (verbose)
            {
               LogMessage("CanSlewAltAzAsync Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanSlewAsync
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yes. Does so in a future task
            if (verbose)
            {
               LogMessage("CanSlewAsync Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanSync
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yes. Have to set the RA/Dec first.
            if (verbose)
            {
               LogMessage("CanSync Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanSyncAltAz
      {
         get
         {
            throwOnDisconnectedOrParked();
            // Yes, have to set the Alt/Az for the selected object first.
            if (verbose)
            {
               LogMessage("CanSyncAltAz Get - ", true.ToString());
            }
            return true;
         }
      }

      public bool CanUnpark
      {
         get
         {
            // No. Cannot unpark. When parked, that's it.
            if (verbose)
            {
               LogMessage("CanUnpark Get - ", false.ToString());
            }
            return false;
         }
      }

      public double Declination
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> declTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.Declination();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => declTask.RunSynchronously());
            declTask.Wait();
            if (verbose)
            {
               LogMessage("Declination Get - ", declTask.Result.ToString());
            }
            return declTask.Result;
         }
      }

      public double DeclinationRate
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> declRateTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.DeclinationRate();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => declRateTask.RunSynchronously());
            declRateTask.Wait();
            if (verbose)
            {
               LogMessage("DeclinationRate Get - ", declRateTask.Result.ToString());
            }
            return declRateTask.Result;
         }
         set
         {
            if (verbose)
            {
               LogMessage("DeclinationRate Set -", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("DeclinationRate", true);
         }
      }

      public PierSide DestinationSideOfPier(double RightAscension, double Declination)
      {
         // Does not apply.
         if (verbose)
         {
            LogMessage("DestinationSideOfPier Get -", "Not implemented");
         }
         throw new ASCOM.PropertyNotImplementedException("DestinationSideOfPier", false);
      }

      public bool DoesRefraction
      {
         // Nope, does not do this.
         get
         {
            if (verbose)
            {
               LogMessage("DoesRefraction Get -", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("DoesRefraction", false);
         }
         set
         {
            if (verbose)
            {
               LogMessage("DoesRefraction Set -", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("DoesRefraction", true);
         }
      }

      public EquatorialCoordinateType EquatorialSystem
      {
         get
         {
            EquatorialCoordinateType equatorialSystem = EquatorialCoordinateType.equLocalTopocentric;
            if (verbose)
            {
               LogMessage("DeclinationRate Get - ", equatorialSystem.ToString());
            }
            return equatorialSystem;
         }
      }

      public void FindHome()
      {
         //LogMessage("FindHome", "Mount finding home position...");
         //TODO: This is a special slew state.
         //CommandBlind(":hF#", true);
         throw new ASCOM.MethodNotImplementedException("FindHome");
      }

      public double FocalLength
      {
         // No not supported set up dialog has it though.
         get
         {
            double focalLengthMeters = focalLength / 1000; // mm to m
            if (verbose)
            {
               LogMessage("FocalLength Get - ", focalLengthMeters.ToString());
            }
            return focalLengthMeters;
         }
      }

      public double GuideRateDeclination
      {
         // Only guide rate in RA is supported.
         get
         {
            return GuideRateRightAscension;
         }
         set
         {
            GuideRateRightAscension = value;
         }
      }

      public double GuideRateRightAscension
      {
         get
         {
            // Units are degrees/second.
            return guideRate * LX90.AxisRates.Sidereal;
         }
         set
         {
            throw new ASCOM.InvalidValueException("RA guide rate is found in the driver set up. This should reflect the patch guide rate (33%, 66%, 100%) you selected.");
         }
      }

      public void MoveAxis(TelescopeAxes Axis, double rate)
      {
         throwOnDisconnectedOrParked();
         Task slewTask = new Task(() =>
         {
            if (rate != 0.0)
            {
               DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.MoveAxis(Axis, rate);
            }
            else
            {
               DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.StopMoveAxis(Axis);
            }
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => slewTask.RunSynchronously());
         slewTask.Wait();
      }

      /// <summary>
      /// Slewing should only return true for any of the slew commands, SideOfPier or MoveAxis..
      /// </summary>
      public bool Slewing
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<bool> isSlewingTask = new Task<bool>(() =>
            {
               return DriverStateBase.MasterCurrentState.IsSlewing()
                  || DriverStateBase.SecondaryAxisCurrentState.IsSlewing();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => isSlewingTask.RunSynchronously());
            isSlewingTask.Wait();
            if (verbose)
            {
               LogMessage("Slewing Get - ", isSlewingTask.Result.ToString());
            }
            return isSlewingTask.Result;
         }
      }

      public void Park()
      {
         throwOnDisconnectedOrParked();
         Task parkTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.Park();
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => parkTask.RunSynchronously());
         parkTask.Wait();
      }

      internal static bool guidingIsPulseGuideCommands()
      {
         return !guidingIsRaReCommands() && !guidingIsMoveCommands();
      }

      internal static bool guidingIsRaReCommands()
      {
         return hasCustomRates && guideRateAlgorithm.Equals(guideRateAlgorithmRaRe);
      }

      internal static bool guidingIsMoveCommands()
      {
         return guideRateAlgorithm.Equals(guideRateAlgorithmMove);
      }

      public void PulseGuide(GuideDirections Direction, int Duration)
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            string directionStr;
            switch (Direction)
            {
               case GuideDirections.guideEast:
                  directionStr = "E";
                  break;
               case GuideDirections.guideWest:
                  directionStr = "W";
                  break;
               case GuideDirections.guideNorth:
                  directionStr = "N";
                  break;
               default:
                  directionStr = "S";
                  break;
            }
            LogMessage("Pulse Guiding - ", directionStr + " for " + Duration.ToString() + "ms.");
         }
         Task pulseGuideTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.PulseGuide(Direction, Duration);
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => pulseGuideTask.RunSynchronously());
         // This waits only as long as the item is in the queue and then runs.
         pulseGuideTask.Wait();
      }

      public bool IsPulseGuiding
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<bool> isPulseGuidingTask = new Task<bool>(() =>
            {
               return DriverStateBase.MasterCurrentState.IsPulseGuiding();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => isPulseGuidingTask.RunSynchronously());
            isPulseGuidingTask.Wait();
            if (verbose)
            {
               LogMessage("IsPulseGuiding Get - ", isPulseGuidingTask.Result.ToString());
            }
            return isPulseGuidingTask.Result;
         }
      }

      public double RightAscension
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> raTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.RightAscension();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => raTask.RunSynchronously());
            raTask.Wait();
            if (verbose)
            {
               LogMessage("RightAscension Get - ", raTask.Result.ToString());
            }
            return raTask.Result;
         }
      }

      public double RightAscensionRate
      {
         get
         {
            Task<double> raRateTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.RightAscensionRate();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => raRateTask.RunSynchronously());
            raRateTask.Wait();
            if (verbose)
            {
               LogMessage("RightAscensionRate Get - ", raRateTask.Result.ToString());
            }
            return raRateTask.Result;
         }
         set
         {
            if (verbose)
            {
               LogMessage("RightAscensionRate Set - ", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("RightAscensionRate", true);
         }
      }

      public void SetPark()
      {
         if (verbose)
         {
            LogMessage("SetPark - ", "Not implemented");
         }
         throw new ASCOM.MethodNotImplementedException("SetPark");
      }

      public PierSide SideOfPier
      {
         get
         {
            if (verbose)
            {
               LogMessage("SideOfPier Get - ", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("SideOfPier", false);
         }
         set
         {
            if (verbose)
            {
               LogMessage("SideOfPier Set - ", "Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("SideOfPier", true);
         }
      }

      public double SiderealTime
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> siderealTimeTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.SiderealTime();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => siderealTimeTask.RunSynchronously());
            siderealTimeTask.Wait();
            if (verbose)
            {
               LogMessage("SiderealTime Get - ", siderealTimeTask.Result.ToString());
            }
            return siderealTimeTask.Result;
         }
      }

      public double SiteElevation
      {
         get
         {
            if (verbose)
            {
               LogMessage("SiteElevation Get - ", elevation.ToString());
            }
            return elevation;
         }
         set
         {
            if (value < 0.0 || value > 12000.0)
            {
               if (verbose)
               {
                  LogMessage("SiteElevation Set - Invalid value ", value.ToString());
               }
               throw new ASCOM.InvalidValueException("SiteElevation. " + value.ToString() + " is out of range 0..12000.");
            }
            if (verbose)
            {
               LogMessage("SiteElevation Set - {0}", value.ToString());
            }
            elevation = Math.Round(value);
         }
      }

      public double SiteLatitude
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> siteLatTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.SiteLatitude();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => siteLatTask.RunSynchronously());
            siteLatTask.Wait();
            if (verbose)
            {
               LogMessage("SiteLatitude Get - ", siteLatTask.Result.ToString());
            }
            return siteLatTask.Result;
         }
         set
         {
            if (verbose)
            {
               LogMessage("SiteLatitude Set - ", value.ToString());
            }
            Task setSiteLatTask = new Task(() =>
            {
               DriverStateBase.MasterCurrentState.SetSiteLatitude(value);
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => setSiteLatTask.RunSynchronously());
            setSiteLatTask.Wait();
         }
      }

      public double SiteLongitude
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<double> siteLonTask = new Task<double>(() =>
            {
               return DriverStateBase.MasterCurrentState.SiteLongitude();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => siteLonTask.RunSynchronously());
            siteLonTask.Wait();
            if (verbose)
            {
               LogMessage("SiteLongitude Get - ", siteLonTask.Result.ToString());
            }
            return siteLonTask.Result;
         }
         set
         {
            if (verbose)
            {
               LogMessage("SiteLongitude Set - ", value.ToString());
            }
            Task setSiteLonTask = new Task(() =>
            {
               DriverStateBase.MasterCurrentState.SetSiteLongitude(value);
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => setSiteLonTask.RunSynchronously());
            setSiteLonTask.Wait();
         }
      }

      private short slewSettleTime = 12;
      public short SlewSettleTime
      {
         get
         {
            return slewSettleTime;
         }
         set
         {
            slewSettleTime = value;
         }
      }

      public void SlewToAltAzAsync(double Azimuth, double Altitude)
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            LogMessage("SlewToAltAzAsync", "Az = " + Azimuth.ToString() + " Alt = " + Altitude.ToString());
         }
         Task slewTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.SlewToAltAzAsync(Azimuth, Altitude);
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => slewTask.RunSynchronously());
         slewTask.Wait();
      }

      public void SlewToAltAz(double Azimuth, double Altitude)
      {
         throw new MethodNotImplementedException("SlewToAltAz not implemented by this driver. Use SlewToAltAzAsync.");
      }

      public void SlewToCoordinates(double RightAscension, double Declination)
      {
         throw new MethodNotImplementedException("SlewToCoordinates not implemented by this driver. Use SlewToCoordinatesAsync.");
      }

      public void SlewToCoordinatesAsync(double RightAscension, double Declination)
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            LogMessage("SlewToCoordinatesAsync", "RA = " + RightAscension.ToString() + " Dec = " + Declination.ToString());
         }
         Task slewTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.SlewToCoordinatesAsync(RightAscension, Declination);
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => slewTask.RunSynchronously());
         slewTask.Wait();
      }

      public void SlewToTarget()
      {
         throw new MethodNotImplementedException("SlewToTarget not implemented by this driver. Use SlewToTargetAsync.");
      }

      public void SlewToTargetAsync()
      {
         if (tl.Enabled)
         {
            LogMessage("SlewToTargetAsync", "RA = " + TargetRightAscension.ToString() + " Dec = " + TargetDeclination.ToString());
         }
         SlewToCoordinatesAsync(TargetRightAscension, TargetDeclination);
      }

      public void SyncToAltAz(double Azimuth, double Altitude)
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            LogMessage("SyncToAltAz", "Az = " + Azimuth.ToString() + " Alt = " + Altitude.ToString());
         }
         Task syncTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.SyncToAltAz(Azimuth, Altitude);
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => syncTask.RunSynchronously());
         syncTask.Wait();
      }

      public void SyncToCoordinates(double RightAscension, double Declination)
      {
         throwOnDisconnectedOrParked();
         if (tl.Enabled)
         {
            LogMessage("SyncToCoordinates", "RA = " + RightAscension.ToString() + " Dec = " + Declination.ToString());
         }
         Task syncTask = new Task(() =>
         {
            DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.SyncToCoordinates(Azimuth, Altitude);
         });
         Patched497Queue.Instance.WorkQueue.Enqueue(() => syncTask.RunSynchronously());
         syncTask.Wait();
      }

      public void SyncToTarget()
      {
         if (tl.Enabled)
         {
            LogMessage("SyncToTarget", "RA = " + TargetRightAscension.ToString() + " Dec = " + TargetDeclination.ToString());
         }
         SyncToCoordinates(TargetRightAscension, TargetDeclination);
      }

      // Magic out of range number to show we have not set it first.
      private double targetDeclination = 91;
      public double TargetDeclination
      {
         get
         {
            if (targetDeclination > 90.0)
            {
               if (tl.Enabled)
               {
                  LogMessage("TargetDeclination Get - ", " must be set before used. Fetching telescope position as TargetDeclination.");
               }
               targetDeclination = Declination;
            }

            return targetDeclination;
         }
         set
         {
            if (value < -90.0 || value > 90.0)
               throw new ASCOM.InvalidValueException("TargetDeclination must be in the range -90..+90");
            targetRightAscension = value;
         }
      }

      // Magic out of range number to show we have not set it first.
      private double targetRightAscension = -1.0;
      public double TargetRightAscension
      {
         get
         {
            if (targetRightAscension < 0.0)
            {
               if (tl.Enabled)
               {
                  LogMessage("TargetRightAscension Get - ", " must be set before used. Fetching telescope position as TargetRightAscension.");
               }
               TargetRightAscension = RightAscension;
            }

            return TargetRightAscension;
         }
         set
         {
            if (value < 0.0 || value > 24.0)
               throw new ASCOM.InvalidValueException("TargetRightAscension must be in the range 0..24");
            targetRightAscension = value;
         }
      }

      public bool Tracking
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<bool> trackingTask = new Task<bool>(() =>
            {
               return DriverStateBase.MasterCurrentState.IsTracking();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => trackingTask.RunSynchronously());
            trackingTask.Wait();
            if (tl.Enabled)
            {
               LogMessage("Tracking Get - ", trackingTask.Result.ToString());
            }
            return trackingTask.Result;
         }
         set
         {
            throwOnDisconnectedOrParked();
            if (!value)
               return; // Not an option.
            if (tl.Enabled)
            {
               LogMessage("Tracking Set - ", value.ToString());
            }
            Task setTrackingTask = new Task(() =>
            {
               if (TrackingRate == DriveRates.driveSidereal && DriverStateBase.MasterCurrentState.IsTrackingLunar())
               {
                  DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.TrackSidereal();
               }
               else if (TrackingRate == DriveRates.driveLunar && DriverStateBase.MasterCurrentState.IsTrackingSidereal())
               {
                  DriverStateBase.CurrentState = DriverStateBase.MasterCurrentState.TrackLunar();
               }
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => setTrackingTask.RunSynchronously());
            setTrackingTask.Wait();
         }
      }

      public DriveRates trackingRate = DriveRates.driveSidereal;
      public DriveRates TrackingRate
      {
         get
         {
            // It really is the last tracking rate set...
            return trackingRate;
         }
         set
         {
            throwOnDisconnectedOrParked();
            if (value != DriveRates.driveSidereal || value != DriveRates.driveLunar)
               throw new ASCOM.InvalidValueException("Only driver rates lunar and sidereal are supported.");
            if (tl.Enabled)
            {
               LogMessage("TrackingRate Set - ", value.ToString());
            }
            if (value != trackingRate)
            {
               trackingRate = value;
               Tracking = true;
            }
         }
      }

      public ITrackingRates TrackingRates
      {
         get
         {
            ITrackingRates trackingRates = new TrackingRates();
            if (verbose)
            {
               LogMessage("TrackingRates Get - ", trackingRates.ToString());
               foreach (DriveRates driveRate in trackingRates)
               {
                  LogMessage("TrackingRates Get - ", driveRate.ToString());
               }
            }
            return trackingRates;
         }
      }

      public DateTime UTCDate
      {
         get
         {
            throwOnDisconnectedOrParked();
            Task<DateTime> utcDateTask = new Task<DateTime>(() =>
            {
               return DriverStateBase.MasterCurrentState.UTCDate();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => utcDateTask.RunSynchronously());
            utcDateTask.Wait();
            if (verbose)
            {
               LogMessage("UTCDate", "Get - {0}", utcDateTask.Result.ToLongDateString());
            }
            return utcDateTask.Result;
         }
         set
         {
            if (verbose)
            {
               LogMessage("UTCDate", "Set - Not implemented");
            }
            throw new ASCOM.PropertyNotImplementedException("UTCDate");
         }
      }

      public void Unpark()
      {
         if (verbose)
         {
            LogMessage("Unpark", "Not implemented");
         }
         throw new ASCOM.MethodNotImplementedException("Unpark");
      }

      #endregion

      #region Private properties and methods
      // here are some useful properties and methods that can be used as required
      // to help with driver development

      #region ASCOM Registration

      // Register or unregister driver for ASCOM. This is harmless if already
      // registered or unregistered. 
      //
      /// <summary>
      /// Register or unregister the driver with the ASCOM Platform.
      /// This is harmless if the driver is already registered/unregistered.
      /// </summary>
      /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
      private static void RegUnregASCOM(bool bRegister)
      {
         using (var P = new ASCOM.Utilities.Profile())
         {
            P.DeviceType = "Telescope";
            if (bRegister)
            {
               P.Register(driverID, driverDescription);
            }
            else
            {
               P.Unregister(driverID);
            }
         }
      }

      /// <summary>
      /// This function registers the driver with the ASCOM Chooser and
      /// is called automatically whenever this class is registered for COM Interop.
      /// </summary>
      /// <param name="t">Type of the class being registered, not used.</param>
      /// <remarks>
      /// This method typically runs in two distinct situations:
      /// <list type="numbered">
      /// <item>
      /// In Visual Studio, when the project is successfully built.
      /// For this to work correctly, the option <c>Register for COM Interop</c>
      /// must be enabled in the project settings.
      /// </item>
      /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
      /// </list>
      /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
      /// </remarks>
      [ComRegisterFunction]
      public static void RegisterASCOM(Type t)
      {
         RegUnregASCOM(true);
      }

      /// <summary>
      /// This function unregisters the driver from the ASCOM Chooser and
      /// is called automatically whenever this class is unregistered from COM Interop.
      /// </summary>
      /// <param name="t">Type of the class being registered, not used.</param>
      /// <remarks>
      /// This method typically runs in two distinct situations:
      /// <list type="numbered">
      /// <item>
      /// In Visual Studio, when the project is cleaned or prior to rebuilding.
      /// For this to work correctly, the option <c>Register for COM Interop</c>
      /// must be enabled in the project settings.
      /// </item>
      /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
      /// </list>
      /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
      /// </remarks>
      [ComUnregisterFunction]
      public static void UnregisterASCOM(Type t)
      {
         RegUnregASCOM(false);
      }

      #endregion

      /// <summary>
      /// Returns true if there is a valid connection to the driver hardware
      /// </summary>
      private bool IsConnected
      {
         get
         {
            Task<bool> isConnectedTask = new Task<bool>(() =>
            {
               return DriverStateBase.MasterCurrentState.IsConnected();
            });
            Patched497Queue.Instance.WorkQueue.Enqueue(() => isConnectedTask.RunSynchronously());
            isConnectedTask.Wait();
            return isConnectedTask.Result;
         }
      }

      /// <summary>
      /// Read the device configuration from the ASCOM Profile store
      /// </summary>
      internal void ReadProfile()
      {
         using (Profile driverProfile = new Profile())
         {
            driverProfile.DeviceType = "Telescope";
            tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
            comPort = driverProfile.GetValue(driverID, comPortProfileName, string.Empty, comPortDefault);
            guideSlewCoefDirection = driverProfile.GetValue(driverID, guideSlewCoefDirectionProfileName, string.Empty, guideSlewCoefDirectionDefault).Equals("East")
               ? GuideDirections.guideEast
               : GuideDirections.guideWest;
            guideSlewCoef = Double.Parse(driverProfile.GetValue(driverID, guideSlewCoefProfileName, string.Empty, guideSlewCoefDefault));
            guideRate = Double.Parse(driverProfile.GetValue(driverID, guideRateProfileName, string.Empty, guideRateDefault));
            focalLength = Double.Parse(driverProfile.GetValue(driverID, focalLengthProfileName, string.Empty, focalLengthDefault));
            aperture = Double.Parse(driverProfile.GetValue(driverID, apertureProfileName, string.Empty, apertureDefault));
            secondaryDiameter = Double.Parse(driverProfile.GetValue(driverID, secondaryDiameterProfileName, string.Empty, secondaryDiameterDefault));
            elevation = Double.Parse(driverProfile.GetValue(driverID, elevationProfileName, string.Empty, elevationDefault));
            hasCustomRates = Boolean.Parse(driverProfile.GetValue(driverID, hasCustomRatesProfileName, string.Empty, hasCustomRatesDefault));
            negRAMovesE = Boolean.Parse(driverProfile.GetValue(driverID, negRAMovesEProfileName, string.Empty, negRAMovesEDefault));
            negREMovesN = Boolean.Parse(driverProfile.GetValue(driverID, negREMovesNProfileName, string.Empty, negREMovesNDefault));
            guideRateAlgorithm = driverProfile.GetValue(driverID, guideRateAlgorithmProfileName, string.Empty, guideRateAlgorithmDefault);
            verbose = tl.Enabled && Boolean.Parse(driverProfile.GetValue(driverID, verboseProfilename, string.Empty, verboseDefault));
         }
      }

      /// <summary>
      /// Write the device configuration to the  ASCOM  Profile store
      /// </summary>
      internal void WriteProfile()
      {
         using (Profile driverProfile = new Profile())
         {
            driverProfile.DeviceType = "Telescope";
            driverProfile.WriteValue(driverID, traceStateProfileName, tl.Enabled.ToString());
            driverProfile.WriteValue(driverID, verboseProfilename, verbose.ToString());
            driverProfile.WriteValue(driverID, comPortProfileName, comPort);
            driverProfile.WriteValue(driverID, guideSlewCoefDirectionProfileName, 
               (guideSlewCoefDirection == GuideDirections.guideEast
                  ? "East"
                  :"West"));
            driverProfile.WriteValue(driverID, guideSlewCoefProfileName, guideSlewCoef.ToString());
            driverProfile.WriteValue(driverID, guideRateProfileName, guideRate.ToString());
            driverProfile.WriteValue(driverID, focalLengthProfileName, focalLength.ToString());
            driverProfile.WriteValue(driverID, apertureProfileName, aperture.ToString());
            driverProfile.WriteValue(driverID, secondaryDiameterProfileName, secondaryDiameter.ToString());
            driverProfile.WriteValue(driverID, elevationProfileName, elevation.ToString());
            driverProfile.WriteValue(driverID, hasCustomRatesProfileName, hasCustomRates.ToString());
            driverProfile.WriteValue(driverID, negRAMovesEProfileName, negRAMovesE.ToString());
            driverProfile.WriteValue(driverID, negREMovesNProfileName, negREMovesN.ToString());
            driverProfile.WriteValue(driverID, guideRateAlgorithmProfileName, guideRateAlgorithm);
         }
      }

      /// <summary>
      /// Log helper function that takes formatted strings and arguments
      /// </summary>
      /// <param name="identifier"></param>
      /// <param name="message"></param>
      /// <param name="args"></param>
      internal static void LogMessage(string identifier, string message, params object[] args)
      {
         var msg = string.Format(message, args);
         tl.LogMessage(identifier, msg);
         System.Console.Write(identifier + msg.ToString() + "\n");
      }
      #endregion
   }
}
