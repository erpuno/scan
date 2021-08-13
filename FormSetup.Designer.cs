namespace INFOTECH
{
    partial class FormSetup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSetup));
            this.m_labelUseUiSettings = new System.Windows.Forms.Label();
	        this.m_labelUseFormCapsUiSettings = new System.Windows.Forms.Label();
            this.m_buttonShowDriverUi = new System.Windows.Forms.Button();
            this.m_buttonSaveUiSettings = new System.Windows.Forms.Button();
	        this.m_buttonShowCapsDiverUi = new System.Windows.Forms.Button();
	        this.m_buttonSaveFormCapsSettings = new System.Windows.Forms.Button();
            this.m_labelSelectDestinationFolder = new System.Windows.Forms.Label();
            this.m_textboxFolder = new System.Windows.Forms.TextBox();
            this.m_buttonSelectDestinationFolder = new System.Windows.Forms.Button();
            this.m_buttonUseUiSettings = new System.Windows.Forms.Button();
	        this.m_buttonUseFormCapsUiSettings = new System.Windows.Forms.Button();
            this.m_textboxUseUiSettings = new System.Windows.Forms.TextBox();
	        this.m_textboxUseFormCapsUiSettings = new System.Windows.Forms.TextBox();
            this.m_groupboxCreateUiSetting = new System.Windows.Forms.GroupBox();
            this.m_groupboxManageSettings = new System.Windows.Forms.GroupBox();
            this.m_buttonDeleteSetting = new System.Windows.Forms.Button();
            this.m_groupboxImageDestination = new System.Windows.Forms.GroupBox();
            this.m_groupboxManageSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_labelUseUiSettings
            // 
            this.m_labelUseUiSettings.AutoSize = true;
            this.m_labelUseUiSettings.Location = new System.Drawing.Point(30, 327);
            this.m_labelUseUiSettings.Name = "m_labelUseUiSettings";
            this.m_labelUseUiSettings.Size = new System.Drawing.Size(305, 13);
            this.m_labelUseUiSettings.Text = "Налаштування сканера для наступного сканування:";
            // 
	        // m_labelUseFormCapsUiSettings
            // 
            this.m_labelUseFormCapsUiSettings.AutoSize = true;
            this.m_labelUseFormCapsUiSettings.Location = new System.Drawing.Point(30, 367);
            this.m_labelUseFormCapsUiSettings.Name = "m_labelUseUiSettings";
            this.m_labelUseFormCapsUiSettings.Size = new System.Drawing.Size(305, 13);
            this.m_labelUseFormCapsUiSettings.Text = "Налаштування FormCaps для наступного сканування:";
            // 
            // m_buttonShowDriverUi
            // 
            this.m_buttonShowDriverUi.Location = new System.Drawing.Point(33, 133);
            this.m_buttonShowDriverUi.Name = "m_buttonShowDriverUi";
            this.m_buttonShowDriverUi.Size = new System.Drawing.Size(294, 23);
            this.m_buttonShowDriverUi.TabIndex = 1;
            this.m_buttonShowDriverUi.Text = "Змінити налаштування драйвера (спочатку цю)...";
            this.m_buttonShowDriverUi.UseVisualStyleBackColor = true;
            this.m_buttonShowDriverUi.Click += new System.EventHandler(this.m_buttonSetup_Click);
            // 
	        // m_buttonShowCapsDiverUi
    	    // 
	        this.m_buttonShowCapsDiverUi.Location = new System.Drawing.Point(33, 199);
    	    this.m_buttonShowCapsDiverUi.Name = "m_buttonSaveFormCapsSettings";
	        this.m_buttonShowCapsDiverUi.Size = new System.Drawing.Size(294, 23);
    	    this.m_buttonShowCapsDiverUi.TabIndex = 3;
	        this.m_buttonShowCapsDiverUi.Text = "Змінити налаштування FormsCaps";
    	    this.m_buttonShowCapsDiverUi.UseVisualStyleBackColor = true;
	        this.m_buttonShowCapsDiverUi.Click += new System.EventHandler(this.m_buttonFormCaps_Click);
	        // 
            // m_buttonSaveUiSettings
            // 
            this.m_buttonSaveUiSettings.Location = new System.Drawing.Point(33, 166);
            this.m_buttonSaveUiSettings.Name = "m_buttonSaveUiSettings";
            this.m_buttonSaveUiSettings.Size = new System.Drawing.Size(294, 23);
            this.m_buttonSaveUiSettings.TabIndex = 2;
            this.m_buttonSaveUiSettings.Text = "Зберегти налаштування драйвера... (потім цю)";
            this.m_buttonSaveUiSettings.UseVisualStyleBackColor = true;
            this.m_buttonSaveUiSettings.Click += new System.EventHandler(this.m_buttonSaveas_Click);
            // 
	        // m_buttonSaveFormCapsSettings
	        // 
    	    this.m_buttonSaveFormCapsSettings.Location = new System.Drawing.Point(33, 232);
	        this.m_buttonSaveFormCapsSettings.Name = "m_buttonSaveFormCapsSettings";
	        this.m_buttonSaveFormCapsSettings.Size = new System.Drawing.Size(294, 23);
    	    this.m_buttonSaveFormCapsSettings.TabIndex = 4;
	        this.m_buttonSaveFormCapsSettings.Text = "Зберегти налаштування FormsCaps";
	        this.m_buttonSaveFormCapsSettings.UseVisualStyleBackColor = true;
    	    //this.m_buttonSaveFormCapsSettings.Click += new System.EventHandler(this.m_buttonSaveas_Click);
	        // 
            // m_labelSelectDestinationFolder
            // 
            this.m_labelSelectDestinationFolder.AutoSize = true;
            this.m_labelSelectDestinationFolder.Location = new System.Drawing.Point(30, 38);
            this.m_labelSelectDestinationFolder.Name = "m_labelSelectDestinationFolder";
            this.m_labelSelectDestinationFolder.Size = new System.Drawing.Size(302, 13);
            this.m_labelSelectDestinationFolder.Text = "Виберіть папку для зберігання зображень:";
            // 
            // m_textboxFolder
            // 
            this.m_textboxFolder.Location = new System.Drawing.Point(33, 56);
            this.m_textboxFolder.Name = "m_textboxFolder";
            this.m_textboxFolder.Size = new System.Drawing.Size(262, 20);
            this.m_textboxFolder.TabIndex = 10;
            this.m_textboxFolder.TextChanged += new System.EventHandler(this.m_textboxFolder_TextChanged);
            // 
            // m_buttonSelectDestinationFolder
            // 
            this.m_buttonSelectDestinationFolder.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_buttonSelectDestinationFolder.BackgroundImage")));
            this.m_buttonSelectDestinationFolder.Location = new System.Drawing.Point(301, 54);
            this.m_buttonSelectDestinationFolder.Name = "m_buttonSelectDestinationFolder";
            this.m_buttonSelectDestinationFolder.Size = new System.Drawing.Size(26, 23);
            this.m_buttonSelectDestinationFolder.TabIndex = 11;
            this.m_buttonSelectDestinationFolder.UseVisualStyleBackColor = true;
            this.m_buttonSelectDestinationFolder.Click += new System.EventHandler(this.m_buttonBrowse_Click);
            // 
            // m_buttonUseUiSettings
            // 
            this.m_buttonUseUiSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_buttonUseUiSettings.BackgroundImage")));
            this.m_buttonUseUiSettings.Location = new System.Drawing.Point(301, 343);
            this.m_buttonUseUiSettings.Name = "m_buttonUseUiSettings";
            this.m_buttonUseUiSettings.Size = new System.Drawing.Size(26, 23);
            this.m_buttonUseUiSettings.TabIndex = 6;
            this.m_buttonUseUiSettings.Text = "1";
            this.m_buttonUseUiSettings.UseVisualStyleBackColor = true;
            this.m_buttonUseUiSettings.Click += new System.EventHandler(this.m_buttonUseUiSettings_Click);
            // 
            // m_textboxUseUiSettings
            // 
            this.m_textboxUseUiSettings.Location = new System.Drawing.Point(33, 345);
            this.m_textboxUseUiSettings.Name = "m_textboxUseUiSettings";
            this.m_textboxUseUiSettings.Size = new System.Drawing.Size(262, 20);
            this.m_textboxUseUiSettings.TabIndex = 5;
            // 
	        // m_buttonUseFormCapsUiSettings
            // 
            this.m_buttonUseFormCapsUiSettings.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("m_buttonUseUiSettings.BackgroundImage")));
            this.m_buttonUseFormCapsUiSettings.Location = new System.Drawing.Point(301, 383);
            this.m_buttonUseFormCapsUiSettings.Name = "m_buttonUseFormCapsUiSettings";
            this.m_buttonUseFormCapsUiSettings.Size = new System.Drawing.Size(26, 23);
            this.m_buttonUseFormCapsUiSettings.TabIndex = 8;
            this.m_buttonUseFormCapsUiSettings.Text = "2";
            this.m_buttonUseFormCapsUiSettings.UseVisualStyleBackColor = true;
            //this.m_buttonUseFormCapsUiSettings.Click += new System.EventHandler(this.m_buttonUseUiSettings_Click);
            // 
            // m_textboxUseFormCapsUiSettings
            // 
            this.m_textboxUseFormCapsUiSettings.Location = new System.Drawing.Point(33, 385);
            this.m_textboxUseFormCapsUiSettings.Name = "m_textboxUseFormCapsUiSettings";
            this.m_textboxUseFormCapsUiSettings.Size = new System.Drawing.Size(262, 20);
            this.m_textboxUseFormCapsUiSettings.TabIndex = 7;
            // 
            // m_groupboxCreateUiSetting
            // 
            this.m_groupboxCreateUiSetting.Location = new System.Drawing.Point(15, 103);
            this.m_groupboxCreateUiSetting.Name = "m_groupboxCreateUiSetting";
            this.m_groupboxCreateUiSetting.Size = new System.Drawing.Size(329, 175);
            this.m_groupboxCreateUiSetting.TabStop = false;
            this.m_groupboxCreateUiSetting.Text = "Створити налаштування драйвера";
            // 
            // m_groupboxManageSettings
            // 
            this.m_groupboxManageSettings.Controls.Add(this.m_buttonDeleteSetting);
            this.m_groupboxManageSettings.Location = new System.Drawing.Point(15, 296);
            this.m_groupboxManageSettings.Name = "m_groupboxManageSettings";
            this.m_groupboxManageSettings.Size = new System.Drawing.Size(329, 149);
            this.m_groupboxManageSettings.TabIndex = 9;
            this.m_groupboxManageSettings.TabStop = false;
            this.m_groupboxManageSettings.Text = "Налаштування драйвера";
            // 
            // m_buttonDeleteSetting
            // 
            this.m_buttonDeleteSetting.Location = new System.Drawing.Point(195, 117);
            this.m_buttonDeleteSetting.Name = "m_buttonDeleteSetting";
            this.m_buttonDeleteSetting.Size = new System.Drawing.Size(117, 23);
            this.m_buttonDeleteSetting.TabIndex = 1;
            this.m_buttonDeleteSetting.Text = "Видалити";
            this.m_buttonDeleteSetting.UseVisualStyleBackColor = true;
            this.m_buttonDeleteSetting.Click += new System.EventHandler(this.m_buttonDeleteSetting_Click);
            // 
            // m_groupboxImageDestination
            // 
            this.m_groupboxImageDestination.Location = new System.Drawing.Point(15, 12);
            this.m_groupboxImageDestination.Name = "m_groupboxImageDestination";
            this.m_groupboxImageDestination.Size = new System.Drawing.Size(329, 78);
            this.m_groupboxImageDestination.TabStop = false;
            this.m_groupboxImageDestination.Text = "Папка де складати зображення";
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(359, 450);
            this.Controls.Add(this.m_buttonUseUiSettings);
	        this.Controls.Add(this.m_buttonUseFormCapsUiSettings);
            this.Controls.Add(this.m_textboxUseUiSettings);
	        this.Controls.Add(this.m_textboxUseFormCapsUiSettings);
            this.Controls.Add(this.m_buttonSelectDestinationFolder);
            this.Controls.Add(this.m_textboxFolder);
            this.Controls.Add(this.m_labelSelectDestinationFolder);
            this.Controls.Add(this.m_buttonSaveUiSettings);
	        this.Controls.Add(this.m_buttonSaveFormCapsSettings);
            this.Controls.Add(this.m_buttonShowDriverUi);
	        this.Controls.Add(this.m_buttonShowCapsDiverUi);
            this.Controls.Add(this.m_labelUseUiSettings);
	        this.Controls.Add(this.m_labelUseFormCapsUiSettings);
            this.Controls.Add(this.m_groupboxCreateUiSetting);
            this.Controls.Add(this.m_groupboxManageSettings);
            this.Controls.Add(this.m_groupboxImageDestination);
            this.Name = "MIA-SETUP";
            this.Text = "Налаштування МІА: Сканування";
            this.Icon = Properties.Resources.Default;
            this.m_groupboxManageSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label m_labelUseUiSettings;
    	private System.Windows.Forms.Label m_labelUseFormCapsUiSettings;
        private System.Windows.Forms.Button m_buttonShowDriverUi;
	    private System.Windows.Forms.Button m_buttonShowCapsDiverUi;
        private System.Windows.Forms.Button m_buttonSaveUiSettings;
	    private System.Windows.Forms.Button m_buttonSaveFormCapsSettings;
        private System.Windows.Forms.Label m_labelSelectDestinationFolder;
        private System.Windows.Forms.TextBox m_textboxFolder;
        private System.Windows.Forms.Button m_buttonSelectDestinationFolder;
        private System.Windows.Forms.Button m_buttonUseUiSettings;
	    private System.Windows.Forms.Button m_buttonUseFormCapsUiSettings;
        private System.Windows.Forms.TextBox m_textboxUseUiSettings;
	    private System.Windows.Forms.TextBox m_textboxUseFormCapsUiSettings;
        private System.Windows.Forms.GroupBox m_groupboxCreateUiSetting;
        private System.Windows.Forms.GroupBox m_groupboxManageSettings;
        private System.Windows.Forms.Button m_buttonDeleteSetting;
        private System.Windows.Forms.GroupBox m_groupboxImageDestination;
    }
}