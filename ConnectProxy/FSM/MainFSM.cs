using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using ConnectProxy.ComPortControl;
using ConnectProxy.FSM;
using ConnectProxy.TCALoader;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using ConnectProxy.TelnetServerSim;
using ConnectProxy.FSM.FSMAction;

namespace ConnectProxy
{
    class TelnetFSM
    {
        public enum States
        {
            StartServer,
            WaitConncet,
            Connncted,
            RuMode,
            CT11Mode,
            NeedServerOpen
        }
        public enum Events
        {
            Disconnect,
            Connncted,
            RuCommand,
            CT11Command,
            GoBack,
            ServerOpend,
        }
        public TelnetFSM()
        {
            #region Sample

            //builder.In(States.OnFloor)
            //   .ExecuteOnEntry(this.AnnounceFloor)
            //   .ExecuteOnExit(Beep)
            //   .ExecuteOnExit(Beep) // just beep a second time
            //   .On(Events.CloseDoor).Goto(States.DoorClosed)
            //   .On(Events.OpenDoor).Goto(States.DoorOpen)
            //   .On(Events.GoUp)
            //       .If(CheckOverload).Goto(States.MovingUp)
            //       .Otherwise().Execute(this.AnnounceOverload)
            //   .On(Events.GoDown)
            //       .If(CheckOverload).Goto(States.MovingDown)
            //       .Otherwise().Execute(this.AnnounceOverload);
            //builder.In(States.Moving)
            //    .On(Events.Stop).Goto(States.OnFloor);
            //proxyRunMachine.DefineHierarchyOn(States.WaitConncet);

            //fSMData.elevator.AddExtension(new Appccelerate.Log4Net.StateMachineLogExtension<States, Events>("fSMData.elevator"));
            #endregion      

            
            fSMData.ruSerialPort = new RuSerialPort();
            fSMData.tCALoader = new TCAControler();
            fSMData.telnetServer = new TelnetServer();
            proxyRunMachine.WithInitialState(States.NeedServerOpen);
            //proxyRunMachine.DefineHierarchyOn(States.CT11Mode).WithHistoryType(HistoryType.Deep)
            proxyRunMachine.In(States.NeedServerOpen).ExecuteOnEntry(startServer).On(Events.ServerOpend).Goto(States.WaitConncet).Execute(waitForConnect);
            proxyRunMachine.In(States.WaitConncet).On(Events.Connncted).Goto(States.Connncted).Execute(connected);
            proxyRunMachine.In(States.Connncted).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            proxyRunMachine.In(States.Connncted).On(Events.Disconnect).Goto(States.WaitConncet).Execute(waitForConnect);
            proxyRunMachine.In(States.CT11Mode).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            proxyRunMachine.In(States.CT11Mode).On(Events.GoBack).Goto(States.Connncted).Execute(connected);
            proxyRunMachine.In(States.RuMode).On(Events.CT11Command).Goto(States.CT11Mode).Execute(ct11Mode);
            proxyRunMachine.In(States.RuMode).On(Events.GoBack).Goto(States.Connncted).Execute(connected);
            // self roll back
            proxyRunMachine.In(States.WaitConncet).On(Events.Disconnect).Goto(States.WaitConncet).Execute(waitForConnect);
            //proxyRunMachine.In(States.Connncted).On(Events.Connncted).Goto(States.Connncted).Execute(connected);
            //proxyRunMachine.In(States.CT11Mode).On(Events.CT11Command).Goto(States.CT11Mode).Execute(ct11Mode);
            //proxyRunMachine.In(States.RuMode).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            var definition = proxyRunMachine.Build();
            fSMData.elevator = definition.CreateActiveStateMachine();
            fSMData.elevator.Start();

            //Action must Instantiate after FSM member Instantiate
            connectedAction = new ConnectedAction(ref fSMData);
            ruModeAction = new RuModeAction(ref fSMData);
            ct11ModeAction = new CT11ModeAction(ref fSMData);

        }
        public void stopFSM()
        {
            this.fSMData.elevator.Stop();
        }
        private void connected()
        {
            fSMData.telnetServer.registerRequsetAction(connectedRequestHandler);
        }
        private void startServer()
        {
            fSMData.telnetServer.startServer("11000");
            this.fSMData.elevator.Fire(Events.ServerOpend);
        }
        private void waitForConnect()
        {
            if (fSMData.telnetServer.isConnect)
            {
                this.fSMData.elevator.Fire(Events.Connncted);
            }
            else
            {
                this.fSMData.elevator.Fire(Events.Disconnect);
            }
        }
        private void ruMode()
        {
            fSMData.telnetServer.registerRequsetAction(RuCommandMode);
        }
        private void ct11Mode()
        {
            fSMData.telnetServer.registerRequsetAction(CT11CommandMode);
        }
        private void connectedRequestHandler(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            connectedAction.runAction(AppSession, stringRequestInfo);
        }
        
        private void RuCommandMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            ruModeAction.runAction(AppSession, stringRequestInfo);

        }
        private void CT11CommandMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

        }
        private StateMachineDefinitionBuilder<States, Events> proxyRunMachine = new StateMachineDefinitionBuilder<States, Events>();
        private FSMData fSMData;
        private ConnectedAction connectedAction = null;
        private CT11ModeAction ct11ModeAction = null;
        private RuModeAction ruModeAction = null;
        #region ActionDic
        //private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> connectedRequestHandleAction
        //    = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        //private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> enterRuCommandsModeAction
        //    = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        //private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> enterCT11CommandsModeAction
        //    = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        //private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> RuCommandsModeAction
        //    = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        //private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> CT11CommandsModeAction
        //    = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        #endregion

        
    }
}
