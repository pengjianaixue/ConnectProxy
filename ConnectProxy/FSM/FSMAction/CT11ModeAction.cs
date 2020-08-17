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
            ct11ModeAction = fSMData;
            tslPath = fSMData.tcaTSLPath;
            tCACommandWarpper = new TCACommandWarpper(tslPath);
            CT11ModeActionDic.Add("SendRUCommand", RuCommandSend);
            CT11ModeActionDic.Add("ExitCT11Mode", exitCT11Mode);
        }
        private void exitCT11Mode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            //isEnterRuCommandMode = false;
            //isRunningRuCommandMode = false;
            //ruModeFSMData.ruSerialPort.stopForwardRecviThread();
            ct11ModeAction.elevator.Fire(TelnetFSM.Events.GoBack);
        }
        private void RuCommandSend(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            ct11ModeAction.ruModeAction.runAction(AppSession, stringRequestInfo);

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
        private FSMData ct11ModeAction;
    }
}
