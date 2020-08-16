using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy.FSM.FSMAction
{
    class CT11ModeAction
    {
        public CT11ModeAction(ref FSMData fSMData)
        {
            ruModeFSMData = fSMData;
            CT11ModeActionDic.Add("SendRUCommand", RuCommandSend);
            CT11ModeActionDic.Add("ExitCT11Mode", exitCT11Mode);
        }
        private void exitCT11Mode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            isEnterRuCommandMode = false;
            isRunningRuCommandMode = false;
            ruModeFSMData.ruSerialPort.stopForwardRecviThread();
            ruModeFSMData.elevator.Fire(TelnetFSM.Events.GoBack);
        }
        private void RuCommandSend(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            ruModeFSMData.ruSerialPort.suspendForwardRecviThread();
            isRunningRuCommandMode = false;
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (stringRequestInfo.Key.Length != 0 && CT11ModeActionDic.ContainsKey(stringRequestInfo.Key))
            {
                CT11ModeActionDic[stringRequestInfo.Key](AppSession, stringRequestInfo);
                return;
            }
            if (!isEnterRuCommandMode)
            {
                isEnterRuCommandMode = true;
                ruModeFSMData.ruSerialPort.startForwardRecviThread(AppSession);
            }
            if (!isRunningRuCommandMode)
            {
                isRunningRuCommandMode = true;
                ruModeFSMData.ruSerialPort.resumeForwardRecviThread();
            }
            string ruCommand = stringRequestInfo.Key + " " + stringRequestInfo.Body;
            System.Console.WriteLine(ruCommand);
            ruModeFSMData.ruSerialPort.send(ruCommand);
        }

        //private ParameterizedThreadStart serialRecviParamter;
        //private Thread serialRecviThread = null; 
        private bool isEnterRuCommandMode = false;
        private bool isRunningRuCommandMode = false;
        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> CT11ModeActionDic
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private FSMData ruModeFSMData;
    }
}
