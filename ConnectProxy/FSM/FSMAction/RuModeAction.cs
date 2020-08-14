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

namespace ConnectProxy.FSM.FSMAction
{
    class RuModeAction:IFSMAction
    {   

        public RuModeAction(ref FSMData fSMData)
        {
            ruModeFSMData = fSMData;
            //connectedRequestHandleAction.Add("RuCommand", ruCommandsMode);
            RuModeActionDic.Add("SendCT11Command", ct11CommandSend);
            RuModeActionDic.Add("ExitRuMode", exitRuMode);
        }
        private void exitRuMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            ruModeFSMData.elevator.Fire(TelnetFSM.Events.GoBack);
        }
        private void ct11CommandSend(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            
            
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (stringRequestInfo.Key.Length != 0 && RuModeActionDic.ContainsKey(stringRequestInfo.Key))
            {
                RuModeActionDic[stringRequestInfo.Key](AppSession, stringRequestInfo);
            }
            System.Console.WriteLine(stringRequestInfo.Body);
            AppSession.sendNoNewLine(ruModeFSMData.ruSerialPort.sendAndRecvi(stringRequestInfo.Body));
            
        }

        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> RuModeActionDic
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private FSMData ruModeFSMData;
    }
}
