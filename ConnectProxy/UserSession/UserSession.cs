using ConnectProxy.FSM;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy.UserSession
{
    class ProxyUserSession
    {
        public ProxyUserSession(FSMConfiguration fsmConfiguration)
        {
            telnetFSM = new TelnetFSM(fsmConfiguration);

        }

        public void stopSession()
        {
            telnetFSM.stopFSM();
        }
        public void restartSession(FSMConfiguration fsmConfiguration)
        {
            telnetFSM.restartFSM(fsmConfiguration);
        }
        public void SessionRequsetHandle(TelnetAppSession session , StringRequestInfo stringRequestInfo)
        {
            telnetFSM.Run(session, stringRequestInfo);
        }
        private TelnetFSM telnetFSM;
    }
}
