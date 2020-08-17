﻿using System;
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

namespace ConnectProxy
{
    public partial class ConnctConfig : Form
    {

        public ConnctConfig()
        {
            InitializeComponent();
            validComportList = SerialPort.GetPortNames();
            if (File.Exists(configFileName))
            {
                tcaTSLsetPath = IniFileOperator.getKeyValue("tslPath", "", configFileName);
                defaultComportNmae = IniFileOperator.getKeyValue("defaultComPortName", "", configFileName);
                serverPort= IniFileOperator.getKeyValue("serverPort", "", configFileName);
                unsafe
                {
                    textBox_TCATSLPath.Text = tcaTSLsetPath;
                    textBox_ServerPort.Text = serverPort;
                    comboBox_ComportList.Text = defaultComportNmae;
                    fSMConfiguration.tslPath = tcaTSLsetPath;
                    fSMConfiguration.defaultComPortName = defaultComportNmae;
                    fSMConfiguration.serverPort = serverPort;
                    telnetFSM = new TelnetFSM(fSMConfiguration);
                }
                
            }
            else
            {
                MessageBox.Show("Can not load the configuration file, please configure the infomation and restart the server !",
                    "configuration load error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox_ServerPort.Text = "11000";
                IniFileOperator.setKeyValue(serverPort, "11000", configFileName);
                comboBox_ComportList.DataSource = validComportList;
            }
        }
        private void ConnctConfig_FormClosed(object sender, FormClosedEventArgs e)
        {

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
            IniFileOperator.setKeyValue("defaultComPortName", defaultComportNmae, configFileName);
        }

        private void textBox_ServerPort_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(textBox_ServerPort.Text) > 65535 || int.Parse(textBox_ServerPort.Text) < 0)
            {
                MessageBox.Show("please input the number between the 0-65535,and don't use the special port(like 22,21)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            serverPort = textBox_ServerPort.Text;
            fSMConfiguration.serverPort = serverPort;
            IniFileOperator.setKeyValue("serverPort", serverPort, configFileName);
        }
        private void button_RestartServer_Click(object sender, EventArgs e)
        {
            if (telnetFSM == null)
            {
                telnetFSM = new TelnetFSM(fSMConfiguration);
            }
            else
            {
                telnetFSM.restartFSM(fSMConfiguration);
            }

        }
        public string tcaTSLsetPath { get; private set; } = null;
        public string[] validComportList = null;
        public  unsafe string  defaultComportNmae = null;
        private unsafe string serverPort = "";
        private string configFileName = "./config.ini";
        private TelnetFSM telnetFSM = null;
        private FSMConfiguration fSMConfiguration = new FSMConfiguration();

        
    }
}
