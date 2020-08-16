namespace ConnectProxy
{
    partial class ConnctConfig
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnctConfig));
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox_configration = new System.Windows.Forms.GroupBox();
            this.button_RefreshComportList = new System.Windows.Forms.Button();
            this.comboBox_ComportList = new System.Windows.Forms.ComboBox();
            this.button_tcaFolderBrowser = new System.Windows.Forms.Button();
            this.TCAPath = new System.Windows.Forms.Label();
            this.textBox_TCATSLPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tcaFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog_TCA = new System.Windows.Forms.OpenFileDialog();
            this.panel1.SuspendLayout();
            this.groupBox_configration.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox_configration);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(572, 548);
            this.panel1.TabIndex = 0;
            // 
            // groupBox_configration
            // 
            this.groupBox_configration.Controls.Add(this.button_RefreshComportList);
            this.groupBox_configration.Controls.Add(this.comboBox_ComportList);
            this.groupBox_configration.Controls.Add(this.button_tcaFolderBrowser);
            this.groupBox_configration.Controls.Add(this.TCAPath);
            this.groupBox_configration.Controls.Add(this.textBox_TCATSLPath);
            this.groupBox_configration.Controls.Add(this.label1);
            this.groupBox_configration.Location = new System.Drawing.Point(12, 12);
            this.groupBox_configration.Name = "groupBox_configration";
            this.groupBox_configration.Size = new System.Drawing.Size(548, 524);
            this.groupBox_configration.TabIndex = 2;
            this.groupBox_configration.TabStop = false;
            this.groupBox_configration.Text = "Configration";
            // 
            // button_RefreshComportList
            // 
            this.button_RefreshComportList.Image = global::ConnectProxy.Properties.Resources.Open_Folder;
            this.button_RefreshComportList.Location = new System.Drawing.Point(421, 65);
            this.button_RefreshComportList.Name = "button_RefreshComportList";
            this.button_RefreshComportList.Size = new System.Drawing.Size(79, 23);
            this.button_RefreshComportList.TabIndex = 4;
            this.button_RefreshComportList.Text = "Refresh";
            this.button_RefreshComportList.UseVisualStyleBackColor = true;
            this.button_RefreshComportList.Click += new System.EventHandler(this.button_RefreshComportList_Click);
            // 
            // comboBox_ComportList
            // 
            this.comboBox_ComportList.FormattingEnabled = true;
            this.comboBox_ComportList.Location = new System.Drawing.Point(141, 67);
            this.comboBox_ComportList.Name = "comboBox_ComportList";
            this.comboBox_ComportList.Size = new System.Drawing.Size(274, 20);
            this.comboBox_ComportList.TabIndex = 3;
            // 
            // button_tcaFolderBrowser
            // 
            this.button_tcaFolderBrowser.Image = global::ConnectProxy.Properties.Resources.Open_Folder;
            this.button_tcaFolderBrowser.Location = new System.Drawing.Point(421, 24);
            this.button_tcaFolderBrowser.Name = "button_tcaFolderBrowser";
            this.button_tcaFolderBrowser.Size = new System.Drawing.Size(79, 23);
            this.button_tcaFolderBrowser.TabIndex = 2;
            this.button_tcaFolderBrowser.Text = "Browser";
            this.button_tcaFolderBrowser.UseVisualStyleBackColor = true;
            this.button_tcaFolderBrowser.Click += new System.EventHandler(this.button_tcaFolderBrowser_Click);
            // 
            // TCAPath
            // 
            this.TCAPath.AutoSize = true;
            this.TCAPath.Location = new System.Drawing.Point(12, 28);
            this.TCAPath.Name = "TCAPath";
            this.TCAPath.Size = new System.Drawing.Size(107, 12);
            this.TCAPath.TabIndex = 0;
            this.TCAPath.Text = "TCA Path(TSL.exe)";
            // 
            // textBox_TCATSLPath
            // 
            this.textBox_TCATSLPath.Location = new System.Drawing.Point(141, 26);
            this.textBox_TCATSLPath.Name = "textBox_TCATSLPath";
            this.textBox_TCATSLPath.Size = new System.Drawing.Size(274, 21);
            this.textBox_TCATSLPath.TabIndex = 1;
            this.textBox_TCATSLPath.TextChanged += new System.EventHandler(this.textBox_TCATSLPath_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(119, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "Defalut Serial Port";
            // 
            // tcaFolderBrowserDialog
            // 
            this.tcaFolderBrowserDialog.ShowNewFolderButton = false;
            // 
            // openFileDialog_TCA
            // 
            this.openFileDialog_TCA.FileName = "TSL.exe";
            // 
            // ConnctConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(572, 548);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConnctConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConnctConfig";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConnctConfig_FormClosed);
            this.panel1.ResumeLayout(false);
            this.groupBox_configration.ResumeLayout(false);
            this.groupBox_configration.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBox_TCATSLPath;
        private System.Windows.Forms.Label TCAPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_configration;
        private System.Windows.Forms.FolderBrowserDialog tcaFolderBrowserDialog;
        private System.Windows.Forms.ComboBox comboBox_ComportList;
        private System.Windows.Forms.Button button_tcaFolderBrowser;
        private System.Windows.Forms.OpenFileDialog openFileDialog_TCA;
        private System.Windows.Forms.Button button_RefreshComportList;
    }
}

