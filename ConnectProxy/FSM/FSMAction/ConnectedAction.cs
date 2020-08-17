using Appccelerate.StateMachine;
using ConnectProxy.ComPortControl;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConnectProxy.TelnetFSM;
using System.IO.Ports;

namespace ConnectProxy.FSM.FSMAction
{
    class ConnectedAction:IFSMAction
    {   

        public ConnectedAction(ref FSMData fSMData)
        {
            connectedfSMData = fSMData;
            connectedRequestHandleAction.Add("RuCommand", tryEnterRuCommandsMode);
            connectedRequestHandleAction.Add("CT11Commands", tryEnterCT11Mode);
            connectedRequestHandleAction.Add("help", connectedRequestPrintHelp);

        }
        private void connectedRequestPrintHelp(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            foreach (var item in connectedRequestHandleAction)
            {
                AppSession.Send(item.Key);
            }
            AppSession.sendPropmt();
        }
        private void tryEnterCT11Mode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            this.connectedfSMData.elevator.Fire(Events.CT11Command);
        }
        private void tryEnterRuCommandsMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (!connectedfSMData.ruSerialPort.isOpen)
            {
                RunTimeError runTimeError = new RunTimeError();
                if ((stringRequestInfo.GetFirstParam().Length == 0 && connectedfSMData.comPortName == null)
                    || !SerialPort.GetPortNames().ToList().Contains(connectedfSMData.comPortName) )
                {
                    printHelp();
                    return;
                }
                else if (stringRequestInfo.GetFirstParam().Length > 0 && !connectedfSMData.ruSerialPort.openComport(stringRequestInfo.GetFirstParam(), runTimeError))
                {
                    AppSession.sendWithAppendPropmt(string.Format("open serial port:{0} fail: " + runTimeError.Errordescription, stringRequestInfo.GetFirstParam()));
                    return;
                }
                else if(stringRequestInfo.GetFirstParam().Length == 0 && SerialPort.GetPortNames().ToList().Contains(connectedfSMData.comPortName))
                {
                    if (!connectedfSMData.ruSerialPort.openComport(stringRequestInfo.GetFirstParam(), runTimeError))
                    {
                        AppSession.sendWithAppendPropmt(string.Format("open serial port:{0} fail: " + runTimeError.Errordescription, connectedfSMData.comPortName));
                    }

                }
                else if (stringRequestInfo.GetFirstParam().Equals("Portlist"))
                {
                    foreach (var item in RuSerialPort.getSerialPortList())
                    {
                        AppSession.Send(item);
                    }
                    AppSession.sendPropmt();
                    return;
                }
                this.connectedfSMData.elevator.Fire(Events.RuCommand);
            }
            this.connectedfSMData.elevator.Fire(Events.RuCommand);

            void printHelp()
            {
                AppSession.sendWithAppendPropmt("RuCommand [ Portlist | serial name[COM3]]");
            }
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (stringRequestInfo.Key.Length == 0)
            {
                AppSession.sendPropmt();
            }
            else if (connectedRequestHandleAction.ContainsKey(stringRequestInfo.Key))
            {
                connectedRequestHandleAction[stringRequestInfo.Key](AppSession, stringRequestInfo);
            }
            else
            {
                AppSession.Send(@"Unkonw Command, Please use [" + "help" + "] list the vaild command");
                AppSession.sendPropmt();
            }
        }

        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> connectedRequestHandleAction
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private FSMData connectedfSMData;
    }
}
