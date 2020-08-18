using ConnectProxy.TCAControl;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy.FSM.FSMAction
{
    class CT11ModeAction:IFSMAction
    {
        public CT11ModeAction(ref FSMData fSMData)
        {
            _fsmData = fSMData;
            tslPath = fSMData.tcaTSLPath;
            tCACommandWarpper = fSMData.tCACommand;
            CT11ModeActionDic.Add("SendRUCommand", RuCommandSend);
            CT11ModeActionDic.Add("ExitCT11Mode", exitCT11Mode);
        }
        private void exitCT11Mode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            //isEnterRuCommandMode = false;
            //isRunningRuCommandMode = false;
            //ruModeFSMData.ruSerialPort.stopForwardRecviThread();
            _fsmData.elevator.Fire(TelnetFSM.Events.GoBack);
        }

        private void ruSerialPortSendAndRecvi(StringRequestInfo stringRequestInfo, TelnetAppSession AppSession)
        {
            _fsmData.ruSerialPort.send(stringRequestInfo.Parameters.ToList().ToString());
            AppSession.sendNoNewLine(_fsmData.ruSerialPort.recviUntilPropmt());
        }

        private void RuCommandSend(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (_fsmData.ruSerialPort.isOpen)
            {
                ruSerialPortSendAndRecvi(stringRequestInfo, AppSession);
            }
            else
            {
                RunTimeError runTimeError = new RunTimeError();
                if (!_fsmData.ruSerialPort.openComport(_fsmData.comPortName, runTimeError))
                {
                    AppSession.sendNoNewLine("open comport failed,please check the comport number!");
                    return;
                }
                ruSerialPortSendAndRecvi(stringRequestInfo, AppSession);
            }
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (stringRequestInfo.Key.Length != 0 )
            {
                tCACommandWarpper.callTCACommand(AppSession, stringRequestInfo);
                return;
            }
        }

        //private ParameterizedThreadStart serialRecviParamter;
        //private Thread serialRecviThread = null; 
        private string tslPath = "";
        private TCACommandWarpper tCACommandWarpper;
        private bool isEnterRuCommandMode = false;
        private bool isRunningRuCommandMode = false;
        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> CT11ModeActionDic
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private FSMData _fsmData;
    }
}
