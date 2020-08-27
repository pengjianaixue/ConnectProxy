using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using ConnectProxy.ComPortControl;
namespace ConnectProxy
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);            
            Application.Run(new ConnctConfig());
        }
    }
}
