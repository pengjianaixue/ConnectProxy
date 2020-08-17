using Appccelerate.StateMachine;
using ConnectProxy.ComPortControl;
using ConnectProxy.FSM.FSMAction;
using ConnectProxy.TCALoader;
using ConnectProxy.TelnetServerSim;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ConnectProxy.TelnetFSM;

namespace ConnectProxy.FSM
{
    struct FSMData
    {
        public ActiveStateMachine<States, Events> elevator;
        public TelnetServer telnetServer;
        public TCAControler tCALoader;
        public RuSerialPort ruSerialPort;
        public string tcaTSLPath ;
        public string comPortName ;
        public CT11ModeAction cT11ModeAction;
        public RuModeAction ruModeAction;
    }
}
