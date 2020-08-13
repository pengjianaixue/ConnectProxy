using Appccelerate.StateMachine;
using Appccelerate.StateMachine.Machine;
using ConnectProxy.ComPortControl;
using ConnectProxy.TCALoader;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;

namespace ConnectProxy
{
    class TelnetFSM
    {
        //class States : IComparable
        //{
        //    int IComparable.CompareTo(object obj)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        //class Events : IComparable
        //{
        //    int IComparable.CompareTo(object obj)
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
        private enum States
        {
            StartServer,
            WaitConncet,
            Connncted,
            RuMode,
            CT11Mode,
            NeedServerOpen
        }

        private enum Events
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

            //elevator.AddExtension(new Appccelerate.Log4Net.StateMachineLogExtension<States, Events>("Elevator"));

            proxyRunMachine.WithInitialState(States.NeedServerOpen);
            //proxyRunMachine.DefineHierarchyOn(States.CT11Mode).WithHistoryType(HistoryType.Deep)
            proxyRunMachine.In(States.NeedServerOpen).ExecuteOnEntry(startServer).On(Events.ServerOpend).Goto(States.WaitConncet).Execute(waitForConnect);
            proxyRunMachine.In(States.WaitConncet).On(Events.Connncted).Goto(States.Connncted).Execute(connected);
            proxyRunMachine.In(States.Connncted).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            proxyRunMachine.In(States.Connncted).On(Events.Disconnect).Goto(States.WaitConncet).Execute(waitForConnect);
            proxyRunMachine.In(States.CT11Mode).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            proxyRunMachine.In(States.CT11Mode).On(Events.GoBack).Goto(States.Connncted).Execute(ruMode);
            proxyRunMachine.In(States.RuMode).On(Events.CT11Command).Goto(States.CT11Mode).Execute(ct11Mode);
            proxyRunMachine.In(States.RuMode).On(Events.GoBack).Goto(States.Connncted).Execute(ruMode);
            // self roll back
            proxyRunMachine.In(States.WaitConncet).On(Events.Disconnect).Goto(States.WaitConncet).Execute(waitForConnect);
            //proxyRunMachine.In(States.Connncted).On(Events.Connncted).Goto(States.Connncted).Execute(connected);
            //proxyRunMachine.In(States.CT11Mode).On(Events.CT11Command).Goto(States.CT11Mode).Execute(ct11Mode);
            //proxyRunMachine.In(States.RuMode).On(Events.RuCommand).Goto(States.RuMode).Execute(ruMode);
            var definition = proxyRunMachine.Build();
            elevator = definition.CreateActiveStateMachine();
            elevator.Start();
        }
        public void stopFSM()
        {
            this.elevator.Stop();
        }
        private void connected()
        {
            telnetServer.registerRequsetAction(connectedRequestHandler);
        }
        private void startServer()
        {
            telnetServer.startServer("12345");
            this.elevator.Fire(Events.ServerOpend);
        }
        private void waitForConnect()
        {
            if (telnetServer.isConnect)
            {
                this.elevator.Fire(Events.Connncted);
            }
            else
            {
                this.elevator.Fire(Events.Disconnect);
            }
        }
        private void ruMode()
        {
            telnetServer.registerRequsetAction(RuCommandMode);
        }
        private void ct11Mode()
        {
            telnetServer.registerRequsetAction(CT11CommandMode);
        }

        private void connectedRequestHandler(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> commandsList = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> {{ "RuCommand", tryEnterRuCommandsMode }, { "CT11Commands", tryEnterCT11Mode } };
            if (stringRequestInfo.Key.Length==0)
            {   
                AppSession.sendNoNewLine("$");
            }else if (commandsList.ContainsKey(stringRequestInfo.Key))
            {
                commandsList[stringRequestInfo.Key](AppSession, stringRequestInfo);
            }
            else if(stringRequestInfo.Key == "help") 
            {
                printHelp();
            }
            else
            {
                AppSession.Send(@"Unkonw Command, Please use ["+ "help" + "] list the vaild command");
                AppSession.sendPropmt();
            }
            void printHelp()
            {
                foreach (var item in commandsList)
                {
                    AppSession.Send(item.Key);
                }
                AppSession.sendPropmt();
            }
        }
        private void tryEnterRuCommandsMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (!ruSerialPort.isOpen)
            {
                if (stringRequestInfo.GetFirstParam().Length == 0)
                {
                    AppSession.Send("Use RuCommand <serialport name[COM3]>");
                    return;
                }
                if (!ruSerialPort.openComport(stringRequestInfo.GetFirstParam()))
                {
                    AppSession.Send("open serial port fail,please check serial port number");
                    return;
                }
                this.elevator.Fire(Events.RuCommand);
            }
            
        }
        private void tryEnterCT11Mode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            
            if (ruSerialPort == null)
            {
                if (stringRequestInfo.GetFirstParam().Length == 0)
                {
                    AppSession.Send("Use RuCommand <serialport name[COM3]>");
                    return;
                }
                if (!ruSerialPort.openComport(stringRequestInfo.GetFirstParam()))
                {
                    AppSession.Send("open serial port fail,please check serial port number");
                }
            }
            else
            {
                AppSession.sendNoNewLine(ruSerialPort.sendAndRecvi(stringRequestInfo.Body));
            }
            this.elevator.Fire(Events.CT11Command);
        }
        private void RuCommandMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            AppSession.sendNoNewLine(ruSerialPort.sendAndRecvi(stringRequestInfo.Body));

        }
        private void CT11CommandMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            

        }
        private StateMachineDefinitionBuilder<States, Events> proxyRunMachine = new StateMachineDefinitionBuilder<States, Events>();
        private readonly ActiveStateMachine<States, Events> elevator;
        private readonly TelnetServer telnetServer = new TelnetServer();
        private readonly TCAControler tCALoader = new TCAControler();
        private readonly RuSerialPort ruSerialPort = new RuSerialPort();
    }
}
