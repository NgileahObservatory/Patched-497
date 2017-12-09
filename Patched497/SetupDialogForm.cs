using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using ASCOM.Utilities;
using ASCOM.LX90;

namespace ASCOM.LX90
{
   [ComVisible(false)]					// Form not registered for COM!
   public partial class SetupDialogForm : Form
   {
      public SetupDialogForm()
      {
         InitializeComponent();
         // Initialise current values of user settings from the ASCOM Profile
         InitUI();
      }

      private void cmdOK_Click(object sender, EventArgs e) // OK button event handler
      {
         // Place any validation constraint checks here
         // Update the state variables with results from the dialogue
         Telescope.comPort = (string)comboBoxComPort.SelectedItem;
         Telescope.tl.Enabled = chkTrace.Checked;
         Telescope.verbose = verboseCheckBox.Checked;
         Telescope.guideSlewCoefDirection = applySlewCoefEast.Checked 
            ? ASCOM.DeviceInterface.GuideDirections.guideEast 
            : ASCOM.DeviceInterface.GuideDirections.guideWest;
         Telescope.guideSlewCoef = (double) guideSlewCoefUpDown.Value;
         Telescope.guideRate = (double) guideRateUpDown.Value;
         Telescope.focalLength = (double) focalLenghtUpDown.Value;
         Telescope.aperture = (double) apertureUpDown.Value;
         Telescope.secondaryDiameter = (double) secondaryDiaUpDown.Value;
         Telescope.elevation = (double) elevationUpDown.Value;
         Telescope.hasCustomRates = customRates.Checked;
         Telescope.customRateReverseLR = customRateReverseLRSlewCheckBox.Checked;
         Telescope.customRateReverseUD = customRateReverseUpDnSlewCheckBox.Checked;
         Telescope.guideRateAlgorithm = raReGuideRadioButton.Checked && customRates.Checked
            ? Telescope.guideRateAlgorithmRaRe
            : (moveGuideRadioButton.Checked
               ? Telescope.guideRateAlgorithmMove
               : Telescope.guideRateAlgorithmDefault);
      }

      private void cmdCancel_Click(object sender, EventArgs e) // Cancel button event handler
      {
         Close();
      }

      private void BrowseToAscom(object sender, EventArgs e) // Click on ASCOM logo event handler
      {
         try
         {
            System.Diagnostics.Process.Start("http://ascom-standards.org/");
         }
         catch (System.ComponentModel.Win32Exception noBrowser)
         {
            if (noBrowser.ErrorCode == -2147467259)
               MessageBox.Show(noBrowser.Message);
         }
         catch (System.Exception other)
         {
            MessageBox.Show(other.Message);
         }
      }

      private void InitUI()
      {
         chkTrace.Checked = Telescope.tl.Enabled;
         // set the list of com ports to those that are currently available
         comboBoxComPort.Items.Clear();
         comboBoxComPort.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());      // use System.IO because it's static
         // select the current port if possible
         if (comboBoxComPort.Items.Contains(Telescope.comPort))
         {
            comboBoxComPort.SelectedItem = Telescope.comPort;
         }
         applySlewCoefEast.Checked = Telescope.guideSlewCoefDirection == ASCOM.DeviceInterface.GuideDirections.guideEast;
         applySlewCoefWest.Checked = !applySlewCoefEast.Checked;
         guideSlewCoefUpDown.Value = new Decimal(Telescope.guideSlewCoef);
         guideRateUpDown.Value = new Decimal(Telescope.guideRate);
         focalLenghtUpDown.Value = new Decimal(Telescope.focalLength);
         apertureUpDown.Value = new Decimal(Telescope.aperture);
         secondaryDiaUpDown.Value = new Decimal(Telescope.secondaryDiameter);
         elevationUpDown.Value = new Decimal(Telescope.elevation);
         customRates.Checked = Telescope.hasCustomRates;
         customRateDirectionGroupBox.Enabled = customRates.Checked;
         customRateReverseLRSlewCheckBox.Checked = Telescope.customRateReverseLR;
         customRateReverseUpDnSlewCheckBox.Checked = Telescope.customRateReverseUD;
         pulseGuideRadioButton.Checked = Telescope.guidingIsPulseGuideCommands();
         raReGuideRadioButton.Enabled = Telescope.hasCustomRates;
         raReGuideRadioButton.Checked = Telescope.guidingIsRaReCommands();
         moveGuideRadioButton.Checked = Telescope.guidingIsMoveCommands();
         verboseCheckBox.Enabled = Telescope.tl.Enabled;
         verboseCheckBox.Checked = Telescope.verbose;
      }

      private void customRates_CheckedChanged(object sender, EventArgs e)
      {
         customRateDirectionGroupBox.Enabled = customRates.Checked;
         raReGuideRadioButton.Enabled = customRates.Checked;
         if (!customRates.Checked && raReGuideRadioButton.Checked)
         {
            pulseGuideRadioButton.Checked = true;
         }
      }

      private void chkTrace_CheckedChanged(object sender, EventArgs e)
      {
         verboseCheckBox.Enabled = chkTrace.Checked;
         if (!verboseCheckBox.Enabled)
         {
            verboseCheckBox.Checked = false;
         }
      }
   }
}