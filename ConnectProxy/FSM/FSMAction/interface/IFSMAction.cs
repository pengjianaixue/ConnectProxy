using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;

namespace ConnectProxy.FSM.FSMAction
{
    interface IFSMAction
    {
        //void runAction();
        void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo);
    }
}
