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
            TelnetServer telnetServer = new TelnetServer();
            //telnetServer.registerAction("Base", telnetHelloFunction);
            ////telnetServer.registerAction("RuCommand", RuCommandMode);
            ////telnetServer.registerMode("RuCommand");
            //telnetServer.registerRequsetAction(RuCommandMode);
            //telnetServer.startServer("12345");
            TelnetFSM telnetFSM = new TelnetFSM();
            Application.Run(new ConnctConfig());
        }
        public static ComPortControl.RuSerialPort ruSerialPort = null;
        public static void RuCommandMode(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            if (ruSerialPort == null)
            {
                if (requestInfo.GetFirstParam().Length == 0)
                {
                    session.Send("Use RuCommand <serialport name[COM3]>");
                    return;
                }
                ruSerialPort = new RuSerialPort();
                if (!ruSerialPort.openComport(requestInfo.GetFirstParam()))
                {
                    ruSerialPort = null;
                    session.Send("open serial port fail,please check serial port number");
                }
            }
            else
            {
                session.sendNoNewLine(ruSerialPort.sendAndRecvi(requestInfo.Body));
            }
        }
        public static void telnetHelloFunction(TelnetAppSession session, StringRequestInfo requestInfo)
        {

            string[] Parameters = requestInfo.Parameters;
            requestInfo.Parameters.CopyTo(Parameters,0);
            switch (requestInfo.GetFirstParam().ToUpper())
            {
                case ("ECHO"):
                    session.Send(requestInfo.Body);
                    //TelnetServer.sendWithPromt(session, requestInfo.Body);
                    break;
                case ("ADD"):
                    session.Send(Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
                    //TelnetServer.sendWithPromt(session, Parameters.Select(p => Convert.ToInt32(p)).Sum().ToString());
                    break;
                case ("MULT"):
                    var result = 1;
                    foreach (var factor in Parameters.Select(p => Convert.ToInt32(p)))
                    {
                        result *= factor;
                    }
                    session.Send(result.ToString());
                    //TelnetServer.sendWithPromt(session, result.ToString());
                    break;
                default:
                    session.Send("");
                    //TelnetServer.sendWithPromt(session, "");
                    break;
            }
        }
    }
}
