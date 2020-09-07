using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectProxy.TCALoader;
using log4net.Core;
using System.IO.Ports;
using System.IO;
using ConnectProxy.ConfigLoad;
using ConnectProxy.FSM;
using ConnectProxy.FileTransfer;
using ConnectProxy.TelnetServerSim;
using ConnectProxy.FSM.FSMAction;

namespace ConnectProxy
{
    public partial class ConnctConfig : Form
    {

        public ConnctConfig()
        {
            InitializeComponent();
            this.showToolStripMenuItem.Click += showToolStripMenuItem_Click;
            this.exitToolStripMenuItem.Click += exitMenuItem_Click;
            validComportList = SerialPort.GetPortNames();
            //lmcFtpServer.start();
            if (File.Exists(configFileName))
            {
                tcaTSLsetPath = IniFileOperator.getKeyValue("tslPath", "", configFileName);
                defaultComportNmae = IniFileOperator.getKeyValue("defaultComPortName", "", configFileName);
                serverPort = IniFileOperator.getKeyValue("serverPort", "", configFileName);
                textBox_TCATSLPath.Text = tcaTSLsetPath;
                textBox_ServerPort.Text = serverPort;
                comboBox_ComportList.Text = defaultComportNmae;
                fSMConfiguration.tslPath = tcaTSLsetPath;
                fSMConfiguration.defaultComPortName = defaultComportNmae;
                fSMConfiguration.serverPort = serverPort;
                telnetServer = new TelnetServer(fSMConfiguration);
            }
            else
            {
                MessageBox.Show("Can not load the configuration file, please configure the infomation and restart the server !",
                    "configuration load error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox_ServerPort.Text = "11000";
                IniFileOperator.setKeyValue("serverPort", "11000", configFileName);
                comboBox_ComportList.DataSource = validComportList;
            }
        }
        private void ConnctConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            //lmcFtpServer.stopServer();
            
        }

        private void button_tcaFolderBrowser_Click(object sender, EventArgs e)
        {

            if (openFileDialog_TCA.ShowDialog().Equals(DialogResult.OK))
            {
                tcaTSLsetPath = openFileDialog_TCA.FileNames[0];
                textBox_TCATSLPath.Text = tcaTSLsetPath;
            }
        }

        private void textBox_TCATSLPath_TextChanged(object sender, EventArgs e)
        {
            string tslPath = textBox_TCATSLPath.Text;
            if (File.Exists(tslPath))
            {
                tcaTSLsetPath = tslPath;
                fSMConfiguration.tslPath = tcaTSLsetPath;
                if (telnetServer != null)
                {
                    telnetServer.updateFSMConfiguration(fSMConfiguration);
                }
                IniFileOperator.setKeyValue("tslPath", tcaTSLsetPath, configFileName);
            }
            else if (tslPath.Length == 0)
            {
                return;
            }
            else
            {
                textBox_TCATSLPath.Clear();
                MessageBox.Show("Can not find the [TSL.exe] from folder:" + tslPath, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void button_RefreshComportList_Click(object sender, EventArgs e)
        {
            validComportList = SerialPort.GetPortNames();
            comboBox_ComportList.DataSource = validComportList;
        }
        private void comboBox_ComportList_SelectedIndexChanged(object sender, EventArgs e)
        {

            defaultComportNmae = comboBox_ComportList.SelectedValue.ToString();
            fSMConfiguration.defaultComPortName = defaultComportNmae;
            if (telnetServer != null)
            {
                telnetServer.updateFSMConfiguration(fSMConfiguration);
            }
            IniFileOperator.setKeyValue("defaultComPortName", defaultComportNmae, configFileName);
        }

        private void textBox_ServerPort_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (int.Parse(textBox_ServerPort.Text) > 65535 || int.Parse(textBox_ServerPort.Text) < 0 || int.Parse(textBox_ServerPort.Text) == 12001)
                {
                    MessageBox.Show("please input the number between the 0-65535,and don't use the special port(like 22,21) and server reserve prot: 12001  ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox_ServerPort.Text = "11000";
                    return;
                }
                serverPort = textBox_ServerPort.Text;
                fSMConfiguration.serverPort = serverPort;
                if (telnetServer != null)
                {
                    telnetServer.updateFSMConfiguration(fSMConfiguration);
                }
                IniFileOperator.setKeyValue("serverPort", serverPort, configFileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private void button_RestartServer_Click(object sender, EventArgs e)
        {
            if (telnetServer == null)
            {
                telnetServer = new TelnetServer(fSMConfiguration);
            }
            else
            {
                telnetServer.restartServer();
            }

        }
        public string tcaTSLsetPath { get; private set; } = null;
        public string[] validComportList = null;
        public string defaultComportNmae = null;
        //private LmcFtpServer lmcFtpServer = new LmcFtpServer();
        private string serverPort = "";
        private string configFileName = "./config.ini";
        private TelnetServer telnetServer = null;
        private FSMConfiguration fSMConfiguration = new FSMConfiguration();
        private FileTransferServer fileTransferServer = new FileTransferServer();
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();


        }
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure to close the Server?", "Confirm",
               System.Windows.Forms.MessageBoxButtons.YesNo,
               System.Windows.Forms.MessageBoxIcon.Warning) == System.Windows.Forms.DialogResult.Yes)
            {
                foreach (var item in TCAIcolishAction.ComportDic)
                {
                    RunTimeError runTimeError = new RunTimeError();
                    TCAControler.DestroyCOMPort(runTimeError, (int)item.Value.Second);
                }
                fileTransferServer.stopServer();
                notifyIcon_hide.Visible = false;
                this.Close();
                this.Dispose();
                Application.Exit();
            }

        }


        private void notifyIcon_hide_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.notifyIcon_hide.Visible = true;
        }

        private void ConnctConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }
}
