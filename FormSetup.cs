
using System;
using System.IO;
using System.Security.Permissions;
using System.Windows.Forms;
using TWAIN32;

namespace INFOTECH
{
    public partial class FormSetup : Form
    {
        ///////////////////////////////////////////////////////////////////////////////
        // Public Methods...
        ///////////////////////////////////////////////////////////////////////////////
        #region Public Methods...

        /// <summary>
        /// Our constructor...
        /// </summary>
        /// <param name="a_twaincstool"></param>
        [PermissionSet(SecurityAction.LinkDemand, Name = "FullTrust", Unrestricted = false)]
        public FormSetup(FormScan a_formscan, ref TWAIN a_twain, string a_szProductDirectory)
        {
            TWAIN.STS sts = TWAIN.STS.SUCCESS;
            string szStatus;
            string szCapability = "";
            string szUsrUiSettings;

            // Init stuff...
            InitializeComponent();
            Console.WriteLine("FormSetup constructor");

            // More init stuff...
            this.FormClosing += new FormClosingEventHandler(FormSetup_FormClosing);

            // Init more stuff (the order matters).  ApplicationData means the following:
            // Windows:     C:\Users\USERNAME\AppData\Roaming (or C:\Documents and Settings\USERNAME\Application Data on XP)
            // Linux:       /home/USERNAME/.config (or /root/.config for superuser)
            // Mac OS X:    /Users/USERNAME/.config (or /var/root/.config for superuser)

            m_formscan = a_formscan;
            m_twain = a_twain;
            m_szTwainscanFolder = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),"twain"), "INFOTECH");
            m_szSettingsFolder = Path.Combine(m_szTwainscanFolder,"settings");
            m_szSettingsFolder = Path.Combine(m_szSettingsFolder, a_szProductDirectory);
            if (!Directory.Exists(m_szTwainscanFolder))
            {
                try
                {
                    Directory.CreateDirectory(m_szTwainscanFolder);
                }
                catch (Exception exception)
                {
                    TWAIN32.Log.Error("exception - " + exception.Message);
                    m_szTwainscanFolder = Directory.GetCurrentDirectory();
                }
            }

            // Restore values...
            m_textboxFolder.Text = RestoreFolder();
            m_textboxUseUiSettings.Text = "";

            // Check for support of Custom DS Data...
            szStatus = "";
            TWAIN.TW_CAPABILITY twcapability = default(TWAIN.TW_CAPABILITY);
            m_twain.CsvToCapability(ref twcapability, ref szStatus, "CAP_CUSTOMDSDATA,0,0,0");
            sts = m_twain.DatCapability(TWAIN.DG.CONTROL, TWAIN.MSG.GETCURRENT, ref twcapability);
            szCapability = m_twain.CapabilityToCsv(twcapability, true);
            Console.WriteLine("GetCaps(): {0}", sts);
            Console.WriteLine("Condition: {0}", !szCapability.EndsWith(",1") && !szCapability.EndsWith(",TRUE"));

            if ((sts != TWAIN.STS.SUCCESS) || (!szCapability.EndsWith(",1") && !szCapability.EndsWith(",TRUE")))
            {
                m_labelUseUiSettings.Enabled = false;
                m_textboxUseUiSettings.Enabled = false;
                m_buttonSaveUiSettings.Enabled = false;
                m_buttonUseUiSettings.Enabled = false;
            }

            // Restore the last saved snapshot...
            else
            {
                m_textboxUseUiSettings.Text = RestoreSetting();
                if (m_textboxUseUiSettings.Text != "")
                {
                    szUsrUiSettings = Path.Combine(m_szSettingsFolder, m_textboxUseUiSettings.Text);
                    if (File.Exists(szUsrUiSettings))
                    {
                        m_formscan.RestoreSnapshot(szUsrUiSettings);
                    }
                    else
                    {
                        m_textboxUseUiSettings.Text = "";
                    }
                }
            }
        }

        /// <summary>
        /// Let the caller know if we can take snapshots...
        /// </summary>
        /// <returns></returns>
        public bool IsCustomDsDataSupported()
        {
            return (m_buttonUseUiSettings.Enabled);
        }

        /// <summary>
        /// Place to put images (empty is okay)...
        /// </summary>
        /// <returns></returns>
        public string GetImageFolder()
        {
            return (m_textboxFolder.Text);
        }

        #endregion


        ///////////////////////////////////////////////////////////////////////////////
        // Private Methods...
        ///////////////////////////////////////////////////////////////////////////////
        #region Private Methods...

        /// <summary>
        /// Validate...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((m_textboxFolder.Text != "") && !Directory.Exists(m_textboxFolder.Text))
            {
                Console.WriteLine("Folder: {0}", m_textboxFolder.Text);
                MessageBox.Show("Please enter a valid Destination Folder.");
                e.Cancel = true;
            }
            if ((m_textboxUseUiSettings.Text != "") && !File.Exists(Path.Combine(m_szSettingsFolder,m_textboxUseUiSettings.Text)))
            {
                Console.WriteLine("Combine: {0} {1}", m_szSettingsFolder, m_textboxUseUiSettings.Text);
                MessageBox.Show("Please enter a valid UI Setting name, or clear the entry.");
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Get the folder path...
        /// </summary>
        /// <returns></returns>
        private string RestoreFolder()
        {
            try
            {
                string szSaveSpot = m_szTwainscanFolder;
                if (!Directory.Exists(szSaveSpot))
                {
                    return ("");
                }
                string szFile = Path.Combine(szSaveSpot, "folder");
                if (File.Exists(szFile))
                {
                    return (File.ReadAllText(szFile));
                }
                return ("");
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                return ("");
            }
        }

        /// <summary>
        /// Get the setting...
        /// </summary>
        /// <returns></returns>
        private string RestoreSetting()
        {
            try
            {
                if (!Directory.Exists(m_szSettingsFolder))
                {
                    return ("");
                }
                string szFile = Path.Combine(m_szSettingsFolder, "current");
                if (File.Exists(szFile))
                {
                    return (File.ReadAllText(szFile));
                }
                return ("");
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                return ("");
            }
        }

        /// <summary>
        /// Remember the folder path for the next time the app runs...
        /// </summary>
        /// <param name="a_szFolder"></param>
        private void SaveFolder(string a_szFolder)
        {
            try
            {
                string szSaveSpot = m_szTwainscanFolder;
                if (!Directory.Exists(szSaveSpot))
                {
                    Directory.CreateDirectory(szSaveSpot);
                }
                File.WriteAllText(Path.Combine(szSaveSpot, "folder"), a_szFolder);
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                // Just let it slide...
            }
        }

        /// <summary>
        /// Remember the setting for the next time the app runs...
        /// </summary>
        /// <param name="a_szFolder"></param>
        private void SaveSetting(string a_szFolder)
        {
            try
            {
                if (!Directory.Exists(m_szSettingsFolder))
                {
                    Directory.CreateDirectory(m_szSettingsFolder);
                }
                File.WriteAllText(Path.Combine(m_szSettingsFolder, "current"), a_szFolder);
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                // Just let it slide...
            }
        }

        #endregion


        ///////////////////////////////////////////////////////////////////////////////
        // Private Form Controls...
        ///////////////////////////////////////////////////////////////////////////////
        #region Private Form Controls...

        /// <summary>
        /// Browse for a place to save image files...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderbrowserdialog = new FolderBrowserDialog();
            folderbrowserdialog.SelectedPath = m_textboxFolder.Text;
            folderbrowserdialog.ShowNewFolderButton = true;
            if (folderbrowserdialog.ShowDialog() == DialogResult.OK)
            {
                m_textboxFolder.Text = folderbrowserdialog.SelectedPath;
                SaveFolder(m_textboxFolder.Text);
            }
        }

        /// <summary>
        /// Delete the setting...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonDeleteSetting_Click(object sender, EventArgs e)
        {
            bool blDeleted;

            // Nothing to do if it's empty...
            if (m_textboxUseUiSettings.Text == "")
            {
                return;
            }

            // If it's not real, tell the user...
            if (!File.Exists(Path.Combine(m_szSettingsFolder, m_textboxUseUiSettings.Text)))
            {
                MessageBox.Show("'" + m_textboxUseUiSettings.Text + "' not found...");
                return;
            }

            // Get confirmation...
            DialogResult dialogresult = MessageBox.Show("Delete '" + m_textboxUseUiSettings.Text + "'?","Confirm",MessageBoxButtons.YesNo);
            if (dialogresult == System.Windows.Forms.DialogResult.No)
            {
                return;
            }

            // Delete it...
            try
            {
                blDeleted = true;
                File.Delete(Path.Combine(m_szSettingsFolder, m_textboxUseUiSettings.Text));
            }
            catch (Exception exception)
            {
                TWAIN32.Log.Error("exception - " + exception.Message);
                blDeleted = false;
                MessageBox.Show("Failed to delete setting...");
            }

            // Clear the text box...
            if (blDeleted)
            {
                m_textboxUseUiSettings.Text = "";
            }
        }

        /// <summary>
        /// Save the current driver's settings...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonSaveas_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefiledialog = new SaveFileDialog();
            savefiledialog.InitialDirectory = m_szSettingsFolder;
            savefiledialog.CheckFileExists = false;
            savefiledialog.CheckPathExists = true;
            savefiledialog.Filter = "Settings|*.settings";
            if (!Directory.Exists(savefiledialog.InitialDirectory))
            {
                try
                {
                    Directory.CreateDirectory(savefiledialog.InitialDirectory);
                }
                catch (Exception exception)
                {
                    TWAIN32.Log.Error("exception - " + exception.Message);
                    return;
                }
            }
            if (savefiledialog.ShowDialog() == DialogResult.OK)
            {
                m_textboxUseUiSettings.Text = Path.GetFileName(savefiledialog.FileName);
                m_formscan.SaveSnapshot(Path.Combine(m_szSettingsFolder,m_textboxUseUiSettings.Text));
                SaveSetting(m_textboxUseUiSettings.Text);
            }
        }

        /// <summary>
        /// Bring up the driver's non-scanning UI...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonSetup_Click(object sender, EventArgs e)
        {
            TWAIN.STS sts;
            m_formscan.ClearEvents();
            TWAIN.TW_USERINTERFACE twuserinterface = default(TWAIN.TW_USERINTERFACE);

            string csv;
            if (IsCustomDsDataSupported()) { csv = "FALSE,FALSE," + this.Handle; }
            else { csv = "TRUE,FALSE," + this.Handle; }
            Console.WriteLine("TWAIN UI: [CONTROL] [ENABLEDSUIONLY] CSV: " + csv);
            m_twain.Rollback(TWAIN.STATE.S2);
            m_twain.CsvToUserinterface(ref twuserinterface, csv);
            sts = m_twain.DatUserinterface(TWAIN.DG.CONTROL, TWAIN.MSG.ENABLEDSUIONLY, ref twuserinterface);
            Console.WriteLine("EnableDSM_UI("+csv+"): {0}", sts);
        }

        /// <summary>
        /// Pick the settings for a scan session...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_buttonUseUiSettings_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfiledialog = new OpenFileDialog();
            openfiledialog.InitialDirectory = m_szSettingsFolder;
            openfiledialog.CheckFileExists = true;
            openfiledialog.CheckPathExists = true;
            openfiledialog.Multiselect = false;
            openfiledialog.Filter = "Settings|*.settings";
            openfiledialog.FilterIndex = 0;
            if (!Directory.Exists(openfiledialog.InitialDirectory))
            {
                try
                {
                    Directory.CreateDirectory(openfiledialog.InitialDirectory);
                }
                catch (Exception exception)
                {
                    TWAIN32.Log.Error("exception - " + exception.Message);
                    MessageBox.Show("Unable to create settings folder...'" + m_szSettingsFolder + "'");
                    return;
                }
            }
            if (openfiledialog.ShowDialog() == DialogResult.OK)
            {
                m_textboxUseUiSettings.Text = Path.GetFileName(openfiledialog.FileName);
                m_formscan.RestoreSnapshot(Path.Combine(m_szSettingsFolder, m_textboxUseUiSettings.Text));
                SaveSetting(m_textboxUseUiSettings.Text);
            }
        }

        /// <summary>
        /// Keep us updated with changes to the save image path...
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_textboxFolder_TextChanged(object sender, EventArgs e)
        {
            SaveFolder(m_textboxFolder.Text);
        }

        #endregion


        ///////////////////////////////////////////////////////////////////////////////
        // Private Attributes...
        ///////////////////////////////////////////////////////////////////////////////
        #region Private Attributes...

        /// <summary>
        /// The folder we persist data to...
        /// </summary>
        private string m_szTwainscanFolder;

        /// <summary>
        /// The settings folder...
        /// </summary>
        private string m_szSettingsFolder;

        /// <summary>
        /// Access to the TWAIN driver...
        /// </summary>
        private TWAIN m_twain;

        /// <summary>
        /// Access to our main form...
        /// </summary>
        private FormScan m_formscan;

        #endregion
    }
}
