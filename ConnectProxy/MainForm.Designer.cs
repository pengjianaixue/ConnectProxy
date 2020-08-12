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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnctConfig));
            this.panel1 = new System.Windows.Forms.Panel();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.TCAPath = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.textBox1);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TCAPath);
            this.panel1.Location = new System.Drawing.Point(2, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 497);
            this.panel1.TabIndex = 0;
            // 
            // TCAPath
            // 
            this.TCAPath.AutoSize = true;
            this.TCAPath.Location = new System.Drawing.Point(54, 63);
            this.TCAPath.Name = "TCAPath";
            this.TCAPath.Size = new System.Drawing.Size(76, 13);
            this.TCAPath.TabIndex = 0;
            this.TCAPath.Text = "TCA TSL Path";
            this.TCAPath.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(136, 60);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(54, 95);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "TCA TSL Path";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(136, 95);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 1;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // ConnctConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 512);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConnctConfig";
            this.Text = "ConnctConfig";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ConnctConfig_FormClosed);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label TCAPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
    }
}

