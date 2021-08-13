namespace INFOTECH
{
    partial class FormCaps
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
            this.m_labelDPI = new System.Windows.Forms.Label();
            this.m_comboDPI = new System.Windows.Forms.ComboBox();
            this.m_labelColor = new System.Windows.Forms.Label();
            this.m_comboColor = new System.Windows.Forms.ComboBox();
            this.m_labelFormat = new System.Windows.Forms.Label();
            this.m_comboFormat = new System.Windows.Forms.ComboBox();
            this.m_labelAutofeed = new System.Windows.Forms.Label();
            this.m_checkboxAutofeed = new System.Windows.Forms.CheckBox();
            this.m_labelPages = new System.Windows.Forms.Label();
            this.m_numericPages = new System.Windows.Forms.NumericUpDown();
            this.m_labelDuplex = new System.Windows.Forms.Label();
            this.m_checkboxDuplex = new System.Windows.Forms.CheckBox();
            this.m_labelWhiteZone = new System.Windows.Forms.Label();
            this.m_groupboxManageSettings = new System.Windows.Forms.GroupBox();
            this.m_buttonCancel = new System.Windows.Forms.Button();
            this.m_buttonSave = new System.Windows.Forms.Button();
            this.m_groupboxManageSettings.SuspendLayout();
            this.SuspendLayout();
            //
            // m_labelDPI
            //
            this.m_labelDPI.Location = new System.Drawing.Point(15, 35);
            this.m_labelDPI.Name = "m_labelDPI";
            this.m_labelDPI.Size = new System.Drawing.Size(100, 15);
            this.m_labelDPI.Text = "DPI";
            // 
            // m_comboDPI
            //
            this.m_comboDPI.Location = new System.Drawing.Point(150, 30);
            this.m_comboDPI.Name = "m_comboDPI";
            this.m_comboDPI.Size = new System.Drawing.Size(200, 15);
            this.m_comboDPI.TabIndex = 1;
            //
            // m_labelColor
            //
            this.m_labelColor.Location = new System.Drawing.Point(15, 70);
            this.m_labelColor.Name = "m_labelColor";
            this.m_labelColor.Size = new System.Drawing.Size(100, 15);
            this.m_labelColor.Text = "Колір";
            //
            // m_comboColor
            //
            this.m_comboColor.Location = new System.Drawing.Point(150, 65);
            this.m_comboColor.Name = "m_comboColor";
            this.m_comboColor.Size = new System.Drawing.Size(200, 15);
            this.m_comboColor.TabIndex = 2;
            //
            // m_labelFormat
            //
            this.m_labelFormat.Location = new System.Drawing.Point(15, 105);
            this.m_labelFormat.Name = "m_labelFormat";
            this.m_labelFormat.Size = new System.Drawing.Size(100, 15);
            this.m_labelFormat.Text = "Формат";
            //
            // m_comboFormat
            //
            this.m_comboFormat.Location = new System.Drawing.Point(150, 105);
            this.m_comboFormat.Name = "m_comboFormat";
            this.m_comboFormat.Size = new System.Drawing.Size(200, 15);
            this.m_comboFormat.TabIndex = 3;
            //
            // m_labelAutofeed
            //
            this.m_labelAutofeed.Location = new System.Drawing.Point(15, 140);
            this.m_labelAutofeed.Name = "m_labelAutofeed";
            this.m_labelAutofeed.Size = new System.Drawing.Size(100, 15);
            this.m_labelAutofeed.Text = "Автоподача";
            //
            // m_checkboxAutofeed
            //
            this.m_checkboxAutofeed.Location = new System.Drawing.Point(150, 140);
            this.m_checkboxAutofeed.Name = "m_checkboxAutofeed";
            this.m_checkboxAutofeed.Size = new System.Drawing.Size(15, 15);
            //
            // m_labelPages
            //
            this.m_labelPages.Location = new System.Drawing.Point(15, 175);
            this.m_labelPages.Name = "m_labelPages";
            this.m_labelPages.Size = new System.Drawing.Size(100, 15);
            this.m_labelPages.Text = "Сторінки";
            //
            // m_numericPages
            //
            this.m_numericPages.Location = new System.Drawing.Point(150, 175);
            this.m_numericPages.Name = "m_numericPages";
            this.m_numericPages.Size = new System.Drawing.Size(200, 15);
            this.m_numericPages.TabIndex = 4;
            //
            // m_labelDuplex
            //
            this.m_labelDuplex.Location = new System.Drawing.Point(15, 210);
            this.m_labelDuplex.Name = "m_labelDuplex";
            this.m_labelDuplex.Size = new System.Drawing.Size(100, 15);
            this.m_labelDuplex.Text = "Дуплекс";
            //
            // m_checkboxDuplex
            //
            this.m_checkboxDuplex.Location = new System.Drawing.Point(150, 210);
            this.m_checkboxDuplex.Name = "m_checkboxDuplex";
            this.m_checkboxDuplex.Size = new System.Drawing.Size(15, 15);
            //
            // m_labelWhiteZone
            //
            this.m_labelWhiteZone.Location = new System.Drawing.Point(15, 245);
            this.m_labelWhiteZone.Name = "m_labelWhiteZone";
            this.m_labelWhiteZone.Size = new System.Drawing.Size(100, 15);
            this.m_labelWhiteZone.Text = "Біла зона";
            //
            // m_buttonCancel
            //
            this.m_buttonCancel.Location = new System.Drawing.Point(15, 300);
            this.m_buttonCancel.Name = "m_buttonShowDriverUi";
            this.m_buttonCancel.Size = new System.Drawing.Size(120, 20);
            this.m_buttonCancel.TabIndex = 5;
            this.m_buttonCancel.Text = "Скасувати";
            this.m_buttonCancel.UseVisualStyleBackColor = true;
            //
            // m_buttonSave
            //
            this.m_buttonSave.Location = new System.Drawing.Point(150, 300);
            this.m_buttonSave.Name = "m_buttonShowDriverUi";
            this.m_buttonSave.Size = new System.Drawing.Size(120, 20);
            this.m_buttonSave.TabIndex = 6;
            this.m_buttonSave.Text = "Зберегти";
            this.m_buttonSave.UseVisualStyleBackColor = true;
            //
            // m_groupboxManageSettings
            // 
            this.m_groupboxManageSettings.Controls.Add(this.m_labelDPI);
            this.m_groupboxManageSettings.Controls.Add(this.m_comboDPI);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelColor);
            this.m_groupboxManageSettings.Controls.Add(this.m_comboColor);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelFormat);
            this.m_groupboxManageSettings.Controls.Add(this.m_comboFormat);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelAutofeed);
            this.m_groupboxManageSettings.Controls.Add(this.m_checkboxAutofeed);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelPages);
            this.m_groupboxManageSettings.Controls.Add(this.m_numericPages);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelDuplex);
            this.m_groupboxManageSettings.Controls.Add(this.m_checkboxDuplex);
            this.m_groupboxManageSettings.Controls.Add(this.m_labelWhiteZone);
            this.m_groupboxManageSettings.Controls.Add(this.m_buttonCancel);
            this.m_groupboxManageSettings.Controls.Add(this.m_buttonSave);
            this.m_groupboxManageSettings.Location = new System.Drawing.Point(15, 15);
            this.m_groupboxManageSettings.Name = "m_groupboxManageSettings";
            this.m_groupboxManageSettings.Size = new System.Drawing.Size(370, 350);
            this.m_groupboxManageSettings.TabStop = false;
            this.m_groupboxManageSettings.Text = "Параметри сканування";
            // 
            // FormSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 380);
            this.Controls.Add(this.m_groupboxManageSettings);
            this.Name = "MIA-SETUP";
            this.Text = "Налаштування МІА: Сканування";
            this.Icon = Properties.Resources.Default;
            this.m_groupboxManageSettings.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.Label m_labelDPI;
        private System.Windows.Forms.ComboBox m_comboDPI;
        private System.Windows.Forms.Label m_labelColor;
        private System.Windows.Forms.ComboBox m_comboColor;
        private System.Windows.Forms.Label m_labelFormat;
        private System.Windows.Forms.ComboBox m_comboFormat;
        private System.Windows.Forms.Label m_labelAutofeed;
        private System.Windows.Forms.CheckBox m_checkboxAutofeed;
        private System.Windows.Forms.Label m_labelPages;
        private System.Windows.Forms.NumericUpDown m_numericPages;
        private System.Windows.Forms.Label m_labelDuplex;
        private System.Windows.Forms.CheckBox m_checkboxDuplex;
        private System.Windows.Forms.Label m_labelWhiteZone;
        private System.Windows.Forms.Button m_buttonCancel;
        private System.Windows.Forms.Button m_buttonSave;
        private System.Windows.Forms.GroupBox m_groupboxManageSettings;
    }
}