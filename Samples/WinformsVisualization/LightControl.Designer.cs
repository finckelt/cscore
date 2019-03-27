namespace WinformsVisualization
{
    partial class LightControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tblLight = new System.Windows.Forms.TableLayoutPanel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnToggleOnOff = new System.Windows.Forms.Button();
            this.btnCurrentColor = new System.Windows.Forms.Button();
            this.chkSelected = new System.Windows.Forms.CheckBox();
            this.btnLowFrequencyColor = new System.Windows.Forms.Button();
            this.btnMidFrequencyColor = new System.Windows.Forms.Button();
            this.btnHighFrequencyColor = new System.Windows.Forms.Button();
            this.chkEnableFrequencyColors = new System.Windows.Forms.CheckBox();
            this.colorCurrent = new System.Windows.Forms.ColorDialog();
            this.lightBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tblLight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tblLight
            // 
            this.tblLight.ColumnCount = 4;
            this.tblLight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblLight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLight.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblLight.Controls.Add(this.chkEnableFrequencyColors, 0, 1);
            this.tblLight.Controls.Add(this.btnToggleOnOff, 3, 0);
            this.tblLight.Controls.Add(this.btnCurrentColor, 2, 0);
            this.tblLight.Controls.Add(this.lblTitle, 0, 0);
            this.tblLight.Controls.Add(this.btnLowFrequencyColor, 1, 1);
            this.tblLight.Controls.Add(this.btnMidFrequencyColor, 2, 1);
            this.tblLight.Controls.Add(this.btnHighFrequencyColor, 3, 1);
            this.tblLight.Controls.Add(this.chkSelected, 1, 0);
            this.tblLight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblLight.Location = new System.Drawing.Point(0, 0);
            this.tblLight.Name = "tblLight";
            this.tblLight.RowCount = 2;
            this.tblLight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLight.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblLight.Size = new System.Drawing.Size(827, 76);
            this.tblLight.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lblTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.lightBindingSource, "DisplayName", true));
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.Green;
            this.lblTitle.Location = new System.Drawing.Point(3, 12);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(41, 13);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "label1";
            // 
            // btnToggleOnOff
            // 
            this.btnToggleOnOff.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnToggleOnOff.BackColor = System.Drawing.Color.White;
            this.btnToggleOnOff.Location = new System.Drawing.Point(742, 7);
            this.btnToggleOnOff.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.btnToggleOnOff.Name = "btnToggleOnOff";
            this.btnToggleOnOff.Size = new System.Drawing.Size(75, 23);
            this.btnToggleOnOff.TabIndex = 2;
            this.btnToggleOnOff.Text = "Turn On";
            this.btnToggleOnOff.UseVisualStyleBackColor = false;
            this.btnToggleOnOff.Click += new System.EventHandler(this.btnToggleOnOff_Click);
            // 
            // btnCurrentColor
            // 
            this.btnCurrentColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCurrentColor.BackColor = System.Drawing.Color.White;
            this.btnCurrentColor.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.lightBindingSource, "CurrentColor", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged, "Null", "Current Color : {0}"));
            this.btnCurrentColor.Location = new System.Drawing.Point(647, 7);
            this.btnCurrentColor.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.btnCurrentColor.Name = "btnCurrentColor";
            this.btnCurrentColor.Size = new System.Drawing.Size(75, 23);
            this.btnCurrentColor.TabIndex = 3;
            this.btnCurrentColor.Tag = "Current Color : {0}";
            this.btnCurrentColor.Text = "Current Color :";
            this.btnCurrentColor.UseVisualStyleBackColor = false;
            this.btnCurrentColor.Click += new System.EventHandler(this.btnCurrentColor_Click);
            // 
            // chkSelected
            // 
            this.chkSelected.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkSelected.AutoSize = true;
            this.chkSelected.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.lightBindingSource, "Selected", true));
            this.chkSelected.Location = new System.Drawing.Point(555, 10);
            this.chkSelected.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.chkSelected.Name = "chkSelected";
            this.chkSelected.Size = new System.Drawing.Size(68, 17);
            this.chkSelected.TabIndex = 4;
            this.chkSelected.Text = "Selected";
            this.chkSelected.UseVisualStyleBackColor = true;
            // 
            // btnLowFrequencyColor
            // 
            this.btnLowFrequencyColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLowFrequencyColor.BackColor = System.Drawing.Color.White;
            this.btnLowFrequencyColor.Location = new System.Drawing.Point(552, 45);
            this.btnLowFrequencyColor.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.btnLowFrequencyColor.Name = "btnLowFrequencyColor";
            this.btnLowFrequencyColor.Size = new System.Drawing.Size(75, 23);
            this.btnLowFrequencyColor.TabIndex = 5;
            this.btnLowFrequencyColor.Tag = "Low Color : {0}";
            this.btnLowFrequencyColor.Text = "Low Color : ";
            this.btnLowFrequencyColor.UseVisualStyleBackColor = false;
            this.btnLowFrequencyColor.Click += new System.EventHandler(this.btnLowFrequencyColor_Click);
            // 
            // btnMidFrequencyColor
            // 
            this.btnMidFrequencyColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnMidFrequencyColor.BackColor = System.Drawing.Color.White;
            this.btnMidFrequencyColor.Location = new System.Drawing.Point(647, 45);
            this.btnMidFrequencyColor.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.btnMidFrequencyColor.Name = "btnMidFrequencyColor";
            this.btnMidFrequencyColor.Size = new System.Drawing.Size(75, 23);
            this.btnMidFrequencyColor.TabIndex = 6;
            this.btnMidFrequencyColor.Tag = "Mid Color :  {0}";
            this.btnMidFrequencyColor.Text = "Mid Color : ";
            this.btnMidFrequencyColor.UseVisualStyleBackColor = false;
            this.btnMidFrequencyColor.Click += new System.EventHandler(this.btnMidFrequencyColor_Click);
            // 
            // btnHighFrequencyColor
            // 
            this.btnHighFrequencyColor.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnHighFrequencyColor.BackColor = System.Drawing.Color.White;
            this.btnHighFrequencyColor.Location = new System.Drawing.Point(742, 45);
            this.btnHighFrequencyColor.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.btnHighFrequencyColor.Name = "btnHighFrequencyColor";
            this.btnHighFrequencyColor.Size = new System.Drawing.Size(75, 23);
            this.btnHighFrequencyColor.TabIndex = 7;
            this.btnHighFrequencyColor.Tag = "High Color : {0}";
            this.btnHighFrequencyColor.Text = "High Color :";
            this.btnHighFrequencyColor.UseVisualStyleBackColor = false;
            this.btnHighFrequencyColor.Click += new System.EventHandler(this.btnHighFrequencyColor_Click);
            // 
            // chkEnableFrequencyColors
            // 
            this.chkEnableFrequencyColors.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkEnableFrequencyColors.AutoSize = true;
            this.chkEnableFrequencyColors.DataBindings.Add(new System.Windows.Forms.Binding("Checked", this.lightBindingSource, "EnableColorFrequencies", true));
            this.chkEnableFrequencyColors.Location = new System.Drawing.Point(199, 48);
            this.chkEnableFrequencyColors.Name = "chkEnableFrequencyColors";
            this.chkEnableFrequencyColors.Size = new System.Drawing.Size(144, 17);
            this.chkEnableFrequencyColors.TabIndex = 8;
            this.chkEnableFrequencyColors.Text = "Enable Frequency Colors";
            this.chkEnableFrequencyColors.UseVisualStyleBackColor = true;
            // 
            // colorCurrent
            // 
            this.colorCurrent.AnyColor = true;
            this.colorCurrent.Color = System.Drawing.Color.BlanchedAlmond;
            this.colorCurrent.FullOpen = true;
            // 
            // lightBindingSource
            // 
            this.lightBindingSource.DataSource = typeof(MagicLedTests.Light);
            this.lightBindingSource.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.lightBindingSource_BindingComplete);
            // 
            // LightControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tblLight);
            this.Name = "LightControl";
            this.Size = new System.Drawing.Size(827, 76);
            this.Load += new System.EventHandler(this.LightControl_Load);
            this.tblLight.ResumeLayout(false);
            this.tblLight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lightBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblLight;
        private System.Windows.Forms.BindingSource lightBindingSource;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnToggleOnOff;
        private System.Windows.Forms.Button btnCurrentColor;
        private System.Windows.Forms.CheckBox chkSelected;
        private System.Windows.Forms.CheckBox chkEnableFrequencyColors;
        private System.Windows.Forms.Button btnLowFrequencyColor;
        private System.Windows.Forms.Button btnMidFrequencyColor;
        private System.Windows.Forms.Button btnHighFrequencyColor;
        private System.Windows.Forms.ColorDialog colorCurrent;
    }
}
