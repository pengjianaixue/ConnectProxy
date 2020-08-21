using Appccelerate.StateMachine;
using ConnectProxy.ComPortControl;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            RuSpecialControlActionDic.Add("InterrupterRuProcess", controlC_Action);
            RuSpecialControlActionDic.Add("ExitRuProcess", controlD_Action);
            RuSpecialControlActionDic.Add("SuspendRuProcess", controlD_Action);
            //serialRecviThread = new Thread(this.serialReviParameterizedThreadStart);
        }
        private void exitRuMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            isEnterRuCommandMode = false;
            isRunningRuCommandMode = false;
            ruModeFSMData.ruSerialPort.stopForwardRecviThread();
            ruModeFSMData.ruSerialPort.close();
            ruModeFSMData.elevator.Fire(TelnetFSM.Events.GoBack);
        }
        private void ct11CommandSend(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            ruModeFSMData.ruSerialPort.suspendForwardRecviThread();
            isRunningRuCommandMode = false;
            ruModeFSMData.tCACommand.callTCACommand(AppSession, stringRequestInfo);
        }
        private void controlD_Action(StringRequestInfo stringRequestInfo)
        {
            byte[] controlCChar = new byte[1];
            controlCChar[0] = 0x4;
            ruModeFSMData.ruSerialPort.send(System.Text.Encoding.Default.GetString(controlCChar));
        }
        private void controlZ_Action(StringRequestInfo stringRequestInfo)
        {
            byte[] controlCChar = new byte[1];
            controlCChar[0] = 0x26;
            ruModeFSMData.ruSerialPort.send(System.Text.Encoding.Default.GetString(controlCChar));
        }
        private void controlC_Action(StringRequestInfo stringRequestInfo)
        {
            byte[] controlCChar = new byte[1];
            controlCChar[0] = 0x3;
            ruModeFSMData.ruSerialPort.send(System.Text.Encoding.Default.GetString(controlCChar));
        }
        private void ruForward(StringRequestInfo stringRequestInfo)
        {
            if (RuSpecialControlActionDic.ContainsKey(stringRequestInfo.Key))
            {
                RuSpecialControlActionDic[stringRequestInfo.Key](stringRequestInfo);
                return;
            }
            string ruCommand = stringRequestInfo.Key + " " + stringRequestInfo.Body;
            ruModeFSMData.ruSerialPort.send(ruCommand);
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (stringRequestInfo.Key.Length != 0 && RuModeActionDic.ContainsKey(stringRequestInfo.Key))
            {
                RuModeActionDic[stringRequestInfo.Key](AppSession, stringRequestInfo);
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
            ruForward(stringRequestInfo);

        }

        //private ParameterizedThreadStart serialRecviParamter;
        //private Thread serialRecviThread = null; 
        private bool isEnterRuCommandMode = false;
        private bool isRunningRuCommandMode = false;
        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> RuModeActionDic
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private Dictionary<string, Action<StringRequestInfo>> RuSpecialControlActionDic
            = new Dictionary<string, Action<StringRequestInfo>>();
        private FSMData ruModeFSMData;
    }
}
