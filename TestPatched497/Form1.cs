using System;
using System.Windows.Forms;
using System.Timers;
using System.Threading.Tasks;
using ASCOM.Utilities;

namespace ASCOM.LX90
{
   public partial class LX90TestForm : Form
   {

      private static ASCOM.DriverAccess.Telescope driver = null;
      private static LX90TestForm form = null;
      private static System.Windows.Forms.Timer refreshTimer = null;

      private static void refreshStateData(object source, EventArgs args)
      {
         try
         {
            if (form != null && driver != null && driver.Connected)
            {
               refreshTimer.Stop();
               Util utilities = new Util();
               DateTime scopeUtc = driver.UTCDate;
               form.utcDateLabel.Text = scopeUtc.ToShortDateString() + " " + scopeUtc.ToShortTimeString();
               form.latLabel.Text = String.Format("{0:0.000}", driver.SiteLatitude);
               form.lonLabel.Text = String.Format("{0:0.000}", driver.SiteLongitude);
               form.elevatonLabel.Text = driver.SiteElevation.ToString() + "m";
               form.raLabel.Text = String.Format("{0:0.00000}", utilities.HoursToHMS(driver.RightAscension));
               form.decLabel.Text = String.Format("{0:0.00000}", utilities.DegreesToDMS(driver.Declination));
               form.azLabel.Text = String.Format("{0:0.00000}", driver.Azimuth);
               form.altLabel.Text = String.Format("{0:0.00000}", driver.Altitude);
               form.lstLabel.Text = String.Format("{0:0.00000}",driver.SiderealTime);
               refreshTimer.Start();
            }
         }
         catch(Exception e)
         {
            // Just don't do anything.
            Console.Write(e.Message);
         }
      }

      public LX90TestForm()
      {
         InitializeComponent();
         SetUIState();
         form = this;
         refreshStateData(null, null);
         refreshTimer = new System.Windows.Forms.Timer();
         refreshTimer.Tick += new EventHandler(refreshStateData);
         refreshTimer.Interval = 5000;
         refreshTimer.Start();
      }

      private void Form1_FormClosing(object sender, FormClosingEventArgs e)
      {
         if (IsConnected)
            driver.Connected = false;

         Properties.Settings.Default.Save();
      }

      private void buttonChoose_Click(object sender, EventArgs e)
      {
         Properties.Settings.Default.DriverId = ASCOM.DriverAccess.Telescope.Choose(Properties.Settings.Default.DriverId);
         SetUIState();
      }

      private void buttonConnect_Click(object sender, EventArgs e)
      {
         if (IsConnected)
         {
            driver.Connected = false;
         }
         else
         {
            driver = new ASCOM.DriverAccess.Telescope(Properties.Settings.Default.DriverId);
            driver.Connected = true;
            trackingRateComboBox.Text = driver.TrackingRate == ASCOM.DeviceInterface.DriveRates.driveSidereal
               ? "Sidereal"
               : "Lunar";
         }
         SetUIState();
      }

      private void SetUIState()
      {
         bool connectedNow = IsConnected;
         buttonConnect.Enabled = !string.IsNullOrEmpty(Properties.Settings.Default.DriverId);
         buttonChoose.Enabled = !connectedNow;
         buttonConnect.Text = connectedNow ? "Disconnect" : "Connect";
         trackingRateLabel.Enabled = trackingRateComboBox.Enabled = connectedNow;
         slewRateLabel.Enabled = slewRateComboBox.Enabled = connectedNow;
         primaryAxisGroupBox.Enabled = connectedNow;
         secondaryAxisGroupBox.Enabled = connectedNow;
         bothAxesGroupBox.Enabled = connectedNow;
         parkButton.Enabled = connectedNow;
         stopButton.Enabled = connectedNow;
         wCheckBox.Enabled = connectedNow;
         eCheckBox.Enabled = connectedNow;
         nCheckBox.Enabled = connectedNow;
         sCheckBox.Enabled = connectedNow;
         ewPulseGuideNumericUpDown.Enabled = connectedNow && ewPulseGuidingRadioButton.Checked;
         nsPulseGuideNumericUpDown.Enabled = connectedNow && nsPulseGuidingRadioButton.Checked;
         stressRAGuidingButton.Enabled = connectedNow && ewPulseGuidingRadioButton.Checked;
         stressREGuidingButton.Enabled = connectedNow && nsPulseGuidingRadioButton.Checked;
      }

      private bool IsConnected
      {
         get
         {
            return ((driver != null) && (driver.Connected == true));
         }
      }

      private void onParkClick(object sender, EventArgs e)
      {
         if (IsConnected)
         {
            driver.Park();
            while (!driver.AtPark)
               System.Threading.Thread.Sleep(1000);
            SetUIState();
         }
      }

      private void stopAllSlews(object sender, EventArgs e)
      {
         if (IsConnected)
         {
            driver.AbortSlew();
            while (driver.Slewing)
               System.Threading.Thread.Sleep(1000);
            SetUIState();
         }
      }

      private double translateRateStringToDeg()
      {
         DeviceInterface.IAxisRates rates = driver.AxisRates(DeviceInterface.TelescopeAxes.axisPrimary);
         if (slewRateComboBox.Text.Equals("Sidereal"))
            return rates[2].Minimum; // 1 based, go figure.
         else if(slewRateComboBox.Text.Equals("Sidereal × 2"))
            return rates[3].Minimum;
         else if(slewRateComboBox.Text.Equals("Sidereal × 8"))
            return rates.Count == 5 ? rates[3].Minimum : rates[4].Minimum;
         else if(slewRateComboBox.Text.Equals("Sidereal × 16"))
            return rates.Count == 5 ? rates[3].Minimum : rates[5].Minimum;
         else if(slewRateComboBox.Text.Equals("Sidereal × 64"))
            return rates.Count == 5 ? rates[3].Minimum : rates[6].Minimum;
         else if(slewRateComboBox.Text.Equals("0.5°/Sec"))
            return rates.Count == 5 ? rates[4].Minimum : rates[7].Minimum;
         else if(slewRateComboBox.Text.Equals("1.5°/Sec"))
            return rates.Count == 5 ? rates[4].Minimum : rates[8].Minimum;
         else if(slewRateComboBox.Text.Equals("3°/Sec"))
            return rates.Count == 5 ? rates[4].Minimum : rates[9].Minimum;
         else // 6.5°/Sec
            return rates.Count == 5 ? rates[5].Minimum : rates[10].Minimum;
      }

      private void wCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         eCheckBox.Enabled = !wCheckBox.Checked;
         if (!wCheckBox.Checked)
         {
            // Did we pulse guide or slew? Pulse guide will finish when it finishes.
            // Slew can be aborted.
            if (driver.Slewing)
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisPrimary, 0.0);
         }
         else
         {
            // Check what is selected other than tracking and do that.
            if (ewPulseGuidingRadioButton.Checked)
               driver.PulseGuide(DeviceInterface.GuideDirections.guideWest, Convert.ToInt32(Math.Round(ewPulseGuideNumericUpDown.Value, 0)));
            else
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisPrimary, translateRateStringToDeg());
         }
      }

      private void eCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         wCheckBox.Enabled = !eCheckBox.Checked;
         if (!eCheckBox.Checked)
         {
            // Did we pulse guide or slew? Pulse guide will finish when it finishes.
            // Slew can be aborted.
            if (driver.Slewing)
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisPrimary, 0.0);
         }
         else
         {
            // Check what is selected other than tracking and do that.
            if (ewPulseGuidingRadioButton.Checked)
               driver.PulseGuide(DeviceInterface.GuideDirections.guideEast, Convert.ToInt32(Math.Round(ewPulseGuideNumericUpDown.Value, 0)));
            else
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisPrimary, -translateRateStringToDeg());
         }
      }

      private void nCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         sCheckBox.Enabled = !nCheckBox.Checked;
         if (!nCheckBox.Checked)
         {
            // Did we pulse guide or slew? Pulse guide will finish when it finishes.
            // Slew can be aborted.
            if (driver.Slewing)
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisSecondary, 0.0);
         }
         else
         {
            // Check what is selected other than tracking and do that.
            if (nsPulseGuidingRadioButton.Checked)
               driver.PulseGuide(DeviceInterface.GuideDirections.guideNorth, Convert.ToInt32(Math.Round(nsPulseGuideNumericUpDown.Value, 0)));
            else
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisSecondary, translateRateStringToDeg());
         }
      }

      private void sCheckBox_CheckedChanged(object sender, EventArgs e)
      {
         nCheckBox.Enabled = !sCheckBox.Checked;
         if (!sCheckBox.Checked)
         {
            // Did we pulse guide or slew? Pulse guide will finish when it finishes.
            // Slew can be aborted.
            if (driver.Slewing)
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisSecondary, 0.0);
         }
         else
         {
            // Check what is selected other than tracking and do that.
            if (nsPulseGuidingRadioButton.Checked)
               driver.PulseGuide(DeviceInterface.GuideDirections.guideSouth, Convert.ToInt32(Math.Round(nsPulseGuideNumericUpDown.Value, 0)));
            else
               driver.MoveAxis(DeviceInterface.TelescopeAxes.axisSecondary, -translateRateStringToDeg());
         }
      }

      private void slewToAltAzButton_Click(object sender, EventArgs e)
      {
         try
         {
            double az = double.Parse(azTextBox.Text);
            double alt = double.Parse(altTextBox.Text);
            if (az >= 0.0 && az <= 360.0)
            {
               if (alt >= 0.0 && alt <= 90)
               {
                  driver.SlewToAltAzAsync(az, alt);
               }
            }
         }
         catch(Exception)
         {
            // Just do nothing...
         }
      }

      private void slewToCoordButton_Click(object sender, EventArgs e)
      {
         try
         {
            Util utilities = new Util();
            double ra = utilities.HMSToHours(raTextBox.Text);
            double dec = utilities.DMSToDegrees(decTextBox.Text);
            if (ra >= 0.0 && ra <= 24.0)
            {
               if (dec >= -90.0 && dec <= 90)
               {
                  driver.SlewToCoordinatesAsync(ra, dec);
               }
            }
         }
         catch (Exception)
         {
            // Just do nothing...
         }
      }

      private void ewPulseGuidingRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         ewPulseGuideNumericUpDown.Enabled = ewPulseGuidingRadioButton.Checked;
      }

      private void nsPulseGuidingRadioButton_CheckedChanged(object sender, EventArgs e)
      {
         nsPulseGuideNumericUpDown.Enabled = nsPulseGuidingRadioButton.Checked;
      }

      private static System.Windows.Forms.Timer stressRATimer = null;
      private static bool lastRAE = false;
      private static double lastRA = 0.0;
      private static System.Windows.Forms.Timer stressRETimer = null;
      private static bool lastREN = false;
      private static double lastDec = 0.0;
      private static void refreshRAStateData(object source, EventArgs args)
      {
         try
         {
            if (form != null && driver != null && driver.Connected)
            {
               stressRATimer.Stop();
               Util utilities = new Util();
               //double RA = driver.RightAscension;
               //if (source == null && args == null)
               //{
               //   lastRA = RA;
               //}
               if (lastRAE)
               {
                  //if (lastRA <= RA && source != null && args != null)
                  //{
                  //   MessageBox.Show("No movement in RA or movement in wrong direction. RA now " 
                  //      + String.Format("{0:0.00000}", utilities.HoursToHMS(RA))
                  //      + " vs "
                  //      + String.Format("{0:0.00000}", utilities.HoursToHMS(lastRA)), ":RA# Guiding Failure", MessageBoxButtons.OK);
                  //   return;
                  //}
                  //else
                  //{
                     driver.PulseGuide(DeviceInterface.GuideDirections.guideWest,10000);
                  //}
                  lastRAE = false;
               }
               else
               {
                  //if( lastRA >= RA && source != null && args != null)
                  //{
                  //   MessageBox.Show("No movement in RA or movement in wrong direction. RA now "
                  //      + String.Format("{0:0.00000}", utilities.HoursToHMS(RA))
                  //      + " vs "
                  //      + String.Format("{0:0.00000}", utilities.HoursToHMS(lastRA)), ":RA# Guiding Failure", MessageBoxButtons.OK);
                  //   return;
                  //}
                  //else
                  //{
                     driver.PulseGuide(DeviceInterface.GuideDirections.guideEast, 10000);
                  //}
                  lastRAE = true;
               }
               //lastRA = RA;
               stressRATimer.Start();
            }
         }
         catch (Exception e)
         {
            // Just don't do anything.
            Console.Write(e.Message);
            MessageBox.Show(e.Message, "Exception in timer handler.", MessageBoxButtons.OK);
         }
      }

      private static void refreshREStateData(object source, EventArgs args)
      {
         try
         {
            if (form != null && driver != null && driver.Connected)
            {
               stressRETimer.Stop();
               Util utilities = new Util();
               //double Dec = driver.Declination;
               //if (source == null && args == null)
               //{
               //   lastDec = Dec;
               //}
               if (lastREN)
               {
                  //if (lastDec <= Dec && source != null && args != null)
                  //{
                  //   MessageBox.Show("No movement in Dec or movement in wrong direction."
                  //      + String.Format("{0:0.00000}", utilities.DegreesToDMS(Dec))
                  //      + " vs "
                  //      + String.Format("{0:0.00000}", utilities.DegreesToDMS(lastDec)), ":RE# Guiding Failure", MessageBoxButtons.OK);
                  //   return;
                  //}
                  //else
                  //{
                     driver.PulseGuide(DeviceInterface.GuideDirections.guideSouth, 10000);
                  //}
                  lastREN = false;
               }
               else
               {
                  //if (lastDec >= Dec && source != null && args != null)
                  //{
                  //   MessageBox.Show("No movement in Dec or movement in wrong direction."
                  //      + String.Format("{0:0.00000}", utilities.DegreesToDMS(Dec))
                  //      + " vs "
                  //      + String.Format("{0:0.00000}", utilities.DegreesToDMS(lastDec)), ":RE# Guiding Failure", MessageBoxButtons.OK);
                  //   return;
                  //}
                  //else
                  //{
                     driver.PulseGuide(DeviceInterface.GuideDirections.guideNorth, 10000);
                  //}
                  lastREN = true;
               }
               //lastDec = Dec;
               stressRETimer.Start();
            }
         }
         catch (Exception e)
         {
            // Just don't do anything.
            Console.Write(e.Message);
            MessageBox.Show(e.Message, "Exception in timer handler.", MessageBoxButtons.OK);
         }
      }

      private void stressRAGuidingButton_Click(object sender, EventArgs e)
      {
         if (stressRATimer == null)
         {
            //refreshTimer.Stop();
            stressRATimer = new System.Windows.Forms.Timer();
            refreshRAStateData(null, null);
            stressRATimer.Tick += new EventHandler(refreshRAStateData);
            stressRATimer.Interval = 10000;
            stressRATimer.Start();
         }
         else
         {
            stressRATimer.Stop();
            stressRATimer = null;
            //if (stressRETimer == null)
            //   refreshTimer.Start();
         }
      }

      private void stressREGuidingButton_Click(object sender, EventArgs e)
      {
         if (stressRETimer == null)
         {
            //refreshTimer.Stop();
            stressRETimer = new System.Windows.Forms.Timer();
            refreshREStateData(null, null);
            stressRETimer.Tick += new EventHandler(refreshREStateData);
            stressRETimer.Interval = 10000;
            stressRETimer.Start();
         }
         else
         {
            stressRETimer.Stop();
            stressRETimer = null;
            //if (stressRATimer == null)
            //   refreshTimer.Start();
         }
      }
   }
}
