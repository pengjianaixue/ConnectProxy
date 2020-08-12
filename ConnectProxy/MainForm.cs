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

namespace ConnectProxy
{
    public partial class ConnctConfig : Form
    {
        
        public ConnctConfig()
        {
            InitializeComponent();
            
            //_tcaControler = new TCAControler();
            //RunTimeError startTCAError = new RunTimeError();
            //if (!_tcaControler.startTCA(startTCAError, "127.0.0.1", @"C:\Program Files (x86)\Ericsson\TCA\Bin\TSL\Release\TSL.exe"))
            //{
            //    System.Console.WriteLine(startTCAError.Errordescription);
            //}
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void ConnctConfig_FormClosed(object sender, FormClosedEventArgs e)
        {
            RunTimeError error = new RunTimeError();
            //_tcaControler.stopTCA(error);
        }
        private TCAControler _tcaControler;
    }
}
