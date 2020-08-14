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

namespace ConnectProxy
{
    public partial class ConnctConfig : Form
    {

        public ConnctConfig()
        {
            InitializeComponent();
            validComportList = SerialPort.GetPortNames();
            comboBox_ComportList.DataSource = validComportList;
            TelnetFSM telnetFSM = new TelnetFSM();
            

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
        public string tcaTSLsetPath  { get; private set ;}= null;
        public string[] validComportList = null;

        private void button_RefreshComportList_Click(object sender, EventArgs e)
        {
            validComportList = SerialPort.GetPortNames();
            comboBox_ComportList.DataSource = validComportList;
        }
    }
}
