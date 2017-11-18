using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using ASCOM.DeviceInterface;
using System.Collections;
using System.Threading;

namespace ASCOM.LX90
{
   #region Rate class
   //
   // The Rate class implements IRate, and is used to hold values
   // for AxisRates. You do not need to change this class.
   //
   // The Guid attribute sets the CLSID for ASCOM.LX90.Rate
   // The ClassInterface/None addribute prevents an empty interface called
   // _Rate from being created and used as the [default] interface
   //
   [Guid("8366d67f-ecbd-4467-a4b4-07c4c31f644a")]
   [ClassInterface(ClassInterfaceType.None)]
   [ComVisible(true)]
   public class Rate : ASCOM.DeviceInterface.IRate
   {
      private double maximum = 0;
      private double minimum = 0;

      //
      // Default constructor - Internal prevents public creation
      // of instances. These are values for AxisRates.
      //
      internal Rate(double minimum, double maximum)
      {
         this.maximum = maximum;
         this.minimum = minimum;
      }

      #region Implementation of IRate

      public void Dispose()
      {
         throw new System.NotImplementedException();
      }

      public double Maximum
      {
         get { return this.maximum; }
         set { this.maximum = value; }
      }

      public double Minimum
      {
         get { return this.minimum; }
         set { this.minimum = value; }
      }

      #endregion
   }
   #endregion

   #region AxisRates
   //
   // AxisRates is a strongly-typed collection that must be enumerable by
   // both COM and .NET. The IAxisRates and IEnumerable interfaces provide
   // this polymorphism. 
   //
   // The Guid attribute sets the CLSID for ASCOM.LX90.AxisRates
   // The ClassInterface/None addribute prevents an empty interface called
   // _AxisRates from being created and used as the [default] interface
   //
   [Guid("04c8f818-d0cb-4185-ba8f-4138a26ca720")]
   [ClassInterface(ClassInterfaceType.None)]
   [ComVisible(true)]
   public class AxisRates : IAxisRates, IEnumerable
   {
      private TelescopeAxes axis;
      private readonly Rate[] rates;
      // Credit where credit is due, 
      // https://github.com/Ryoko/ASCOM-Celestron-Telescope-Driver-RS232-BT/blob/master/CelestroneDriver/Utils/Const.cs
      public static readonly double Sidereal = 360/(86400*0.9972695664); // 360 degrees/(UTC seconds in day * UTC seconds per sidereal second)
      public static readonly double Siderealx2 = Sidereal * 2; // degrees/sec
      public static readonly double Siderealx8 = Sidereal * 8; // and so on...
      public static readonly double Siderealx16 = Sidereal * 16;
      public static readonly double Siderealx64 = Sidereal * 64;
      public static readonly double SlewHalfDegreePerSec = 0.5;
      public static readonly double SlewOnePointFiveDegreePerSec = 1.5;
      public static readonly double SlewThreeDegreePerSec = 3.0;
      // TODO: Detect quiet slew and not return this option when it's on.
      public static readonly double SlewSixPointFiveDegreePerSec = 6.5;

      //
      // Constructor - Internal prevents public creation
      // of instances. Returned by Telescope.AxisRates.
      //
      internal AxisRates(TelescopeAxes axis)
      {
         this.axis = axis;
         //
         // This collection must hold zero or more Rate objects describing the 
         // rates of motion ranges for the Telescope.MoveAxis() method
         // that are supported by your driver. It is OK to leave this 
         // array empty, indicating that MoveAxis() is not supported.
         //
         // Note that we are constructing a rate array for the axis passed
         // to the constructor. Thus we switch() below, and each case should 
         // initialize the array for the rate for the selected axis.
         //
         switch (axis)
         {
            case TelescopeAxes.axisPrimary: // Treated as meaning RA/Az
            case TelescopeAxes.axisSecondary: // Treated as meaning Dec/Alt
               if (Telescope.hasCustomRates)
               {
                  this.rates = new Rate[]
                  {
                     // Rates from not moving to 6.5 degrees/sec
                     new Rate(0, SlewSixPointFiveDegreePerSec),
                     new Rate(Sidereal, Sidereal),
                     new Rate(Siderealx2, Siderealx2),
                     new Rate(Siderealx8, Siderealx8),
                     new Rate(Siderealx16, Siderealx16),
                     new Rate(Siderealx64, Siderealx64),
                     new Rate(SlewHalfDegreePerSec, SlewHalfDegreePerSec),
                     new Rate(SlewOnePointFiveDegreePerSec, SlewOnePointFiveDegreePerSec),
                     new Rate(SlewThreeDegreePerSec, SlewThreeDegreePerSec),
                     new Rate(SlewSixPointFiveDegreePerSec, SlewSixPointFiveDegreePerSec),
                  };
               }
               else
               {
                  this.rates = new Rate[]
                  {
                     // Rates from not sidereal to 6.5 degrees/sec
                     new Rate(Sidereal, SlewSixPointFiveDegreePerSec),
                     new Rate(Sidereal, Sidereal),       // Sidereal
                     new Rate(Siderealx2, Siderealx2),   // Second slowest.
                     new Rate(SlewThreeDegreePerSec, SlewThreeDegreePerSec), // 2nd Fastest.
                     new Rate(SlewSixPointFiveDegreePerSec, SlewSixPointFiveDegreePerSec), // Fastest
                  };
               }
               break;
            case TelescopeAxes.axisTertiary:
               this.rates = new Rate[0]; // Leave empty
               break;
         }
      }

      #region IAxisRates Members

      public int Count
      {
         get { return this.rates.Length; }
      }

      public void Dispose()
      {
         throw new System.NotImplementedException();
      }

      public IEnumerator GetEnumerator()
      {
         return rates.GetEnumerator();
      }

      public IRate this[int index]
      {
         get { return this.rates[index - 1]; }	// 1-based
      }

      #endregion

      static public bool ratesApproxEqual(double lhs, double rhs)
      {
         return Math.Abs(lhs - rhs) < 0.0000000001;
      }
   }
   #endregion

   #region TrackingRates
   //
   // TrackingRates is a strongly-typed collection that must be enumerable by
   // both COM and .NET. The ITrackingRates and IEnumerable interfaces provide
   // this polymorphism. 
   //
   // The Guid attribute sets the CLSID for ASCOM.LX90.TrackingRates
   // The ClassInterface/None addribute prevents an empty interface called
   // _TrackingRates from being created and used as the [default] interface
   //
   // This class is implemented in this way so that applications based on .NET 3.5
   // will work with this .NET 4.0 object.  Changes to this have proved to be challenging
   // and it is strongly suggested that it isn't changed.
   //
   [Guid("417b89b2-42de-4a49-871f-e13e874bcd86")]
   [ClassInterface(ClassInterfaceType.None)]
   [ComVisible(true)]
   public class TrackingRates : ITrackingRates, IEnumerable, IEnumerator
   {
      private readonly DriveRates[] trackingRates;

      // this is used to make the index thread safe
      private readonly ThreadLocal<int> pos = new ThreadLocal<int>(() => { return -1; });
      private static readonly object lockObj = new object();


      //
      // Default constructor - Internal prevents public creation
      // of instances. Returned by Telescope.AxisRates.
      //
      internal TrackingRates()
      {
         //
         // This array must hold ONE or more DriveRates values, indicating
         // the tracking rates supported by your telescope. The one value
         // (tracking rate) that MUST be supported is driveSidereal!
         //
         this.trackingRates = new[] 
         { 
            DriveRates.driveSidereal,
            DriveRates.driveLunar,
         };
      }

      #region ITrackingRates Members

      public int Count
      {
         get { return this.trackingRates.Length; }
      }

      public IEnumerator GetEnumerator()
      {
         pos.Value = -1;
         return this as IEnumerator;
      }

      public void Dispose()
      {
         throw new System.NotImplementedException();
      }

      public DriveRates this[int index]
      {
         get { return this.trackingRates[index - 1]; }	// 1-based
      }

      #endregion

      #region IEnumerable members

      public object Current
      {
         get
         {
            lock (lockObj)
            {
               if (pos.Value < 0 || pos.Value >= trackingRates.Length)
               {
                  throw new System.InvalidOperationException();
               }
               return trackingRates[pos.Value];
            }
         }
      }

      public bool MoveNext()
      {
         lock (lockObj)
         {
            if (++pos.Value >= trackingRates.Length)
            {
               return false;
            }
            return true;
         }
      }

      public void Reset()
      {
         pos.Value = -1;
      }
      #endregion
   }
   #endregion
}
