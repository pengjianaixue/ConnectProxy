using System;
using System.Collections.Generic;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketService;
using ConnectProxy.UserSession;
using ConnectProxy.FSM;
using System.Threading.Tasks;

namespace ConnectProxy.TelnetServerSim
{
    

    using ModeDic = Dictionary<string, Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>>;
    using ActionDic = Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>;
    //class TelnetRequestInfo : RequestInfo<string>
    //{

    //}
    class TelnetAppSession : AppSession<TelnetAppSession>
    {
        
        public string PropmtSymbol { get; set; } = "$ ";
        public override void Send(string message)
        {
            base.SocketSession.Client.SendData(System.Text.Encoding.Default.GetBytes(message + System.Environment.NewLine));
        }
        public void baseSend(string message)
        {
            base.Send(message);
        }
        public void sendPropmt()
        {
            base.SocketSession.Client.SendData(System.Text.Encoding.Default.GetBytes(PropmtSymbol));

        }
        public void sendWithAppendPropmt(string message)
        {
            this.Send(message);
            this.sendPropmt();
        }
        public void sendNoNewLine(string message)
        {
            base.SocketSession.Client.SendData(System.Text.Encoding.Default.GetBytes(message));

        }
        protected override void HandleUnknownRequest(StringRequestInfo requestInfo)
        {
            System.Console.WriteLine(requestInfo.Key);
        }
    }
    class TelnetAppServer : AppServer<TelnetAppSession>
    {
        //public void CLearAllEvent()
        //{
        //    if (base.NewRequestReceived == null) return;
        //    Delegate[] dels = base.NewRequestReceived.GetInvocationList();
        //    foreach (Delegate del in dels)
        //    {
        //        object delObj = del.GetType().GetProperty("Method").GetValue(del, null);
        //        string funcName = (string)delObj.GetType().GetProperty("Name").GetValue(delObj, null);////方法名
        //        Console.WriteLine(funcName);
        //        base.NewRequestReceived -= del as MasterEventHandler;
        //    }
        //}
    }
    interface RequstAction
    {
        void run(TelnetAppSession session, StringRequestInfo requestInfo);

    }

    class TelnetServer
    {

        public TelnetServer(FSMConfiguration fsmConfiguration)
        {
            _fsmConfiguration = fsmConfiguration;
            baseActionDic.Add("", sendPromt);
            baseActionDic.Add("help", printHelp);
            baseActionDic.Add("Exit", sessionClose);
            startServer(_fsmConfiguration.serverPort);
        }
        public bool startServer(string port)
        {
            telnetServer = new TelnetAppServer();
            if (!telnetServer.Setup(port.ToInt32()))
            {
                return false;
            }
            telnetServer.Start();
            telnetServer.NewSessionConnected += new SessionHandler<TelnetAppSession>(appServer_NewSessionConnected);
            telnetServer.NewRequestReceived += appServer_NewRequestReceived;
            telnetServer.SessionClosed += TelnetServer_SessionClosed;
            //resetRequsetAction();
            return true;
        }

        private void TelnetServer_SessionClosed(TelnetAppSession session, CloseReason value)
        {
            if (sessionDic.ContainsKey(session.SessionID))
            {
                sessionDic[session.SessionID].stopSession();
                sessionDic.Remove(session.SessionID);
            }
        }
        public void restartServer()
        {
            foreach (var item in sessionDic)
            {
                item.Value.stopSession();
            }
            telnetServer.Stop();
            startServer(_fsmConfiguration.serverPort);
        }
        public void stopServer()
        {
            telnetServer.Stop();
        }
        private void appServer_NewSessionConnected(TelnetAppSession session)
        {
            session.sendNoNewLine(string.Format("Welcome to digital brain telnet server{0}Server Info: Address: {1},Port: {2}{3}Time: {4}{5}",
                System.Environment.NewLine, session.LocalEndPoint.Address.MapToIPv4(), session.Config.Port, System.Environment.NewLine,
                session.StartTime.ToString(), System.Environment.NewLine + session.PropmtSymbol));
            ProxyUserSession proxyUserSession = new ProxyUserSession(_fsmConfiguration);
            sessionDic.Add(session.SessionID, proxyUserSession);

        }
        //public void resetRequsetAction()
        //{
        //    telnetServer.NewRequestReceived += new RequestHandler<TelnetAppSession, StringRequestInfo>(appServer_NewRequestReceived);
        //    RequsetActionList.Add(appServer_NewRequestReceived);

        //}
        //public void registerRequsetAction(RequestHandler<TelnetAppSession, StringRequestInfo> requestHandle)
        //{
        //    //telnetServer.SetupCommands()
        //    foreach (var item in RequsetActionList)
        //    {
        //        telnetServer.NewRequestReceived -= item;
        //    }
        //    RequsetActionList.Clear();
        //    RequsetActionList.Add(requestHandle);
        //    telnetServer.NewRequestReceived += requestHandle;

        //}
        //public void registerAction(string actionName, Action<TelnetAppSession, StringRequestInfo> actionFunction, string ModeName = "Base")
        //{
        //    serverAction.Add(actionName, actionFunction);
        //}
        //public ActionDic serverAction
        //{
        //    get;
        //    set;
        //}
        private void appServer_NewRequestReceived(TelnetAppSession session, StringRequestInfo requestInfo)
        {

            #region async
            //Task.Run(() =>
            //{
            //    if (sessionDic.ContainsKey(session.SessionID))
            //    {
            //        sessionDic[session.SessionID].SessionRequsetHandle(session, requestInfo);
            //    }
            //});
            #endregion
            //TODO  maybe for  preformance reason need make it async Run
            if (sessionDic.ContainsKey(session.SessionID))
            {
                sessionDic[session.SessionID].SessionRequsetHandle(session, requestInfo);
            }
            
        }
        public void updateFSMConfiguration(FSMConfiguration fsmConfiguration)
        {
            _fsmConfiguration = fsmConfiguration;
        }

        //public delegate void undateFSMConfiguration();

        #region BaseCommand
        private void sendPromt(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            session.sendPropmt();
        }
        private void printHelp(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            foreach (var item in baseActionDic)
            {
                if (item.Key.Length != 0)
                {
                    session.Send(item.Key);
                }
            }
            session.sendPropmt();
        }
        private void sessionClose(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            session.Close();
        }
        #endregion
        public bool isConnect { get; set; }
        //public event EventHandler<string> clientSeeionClosed;
        private List<RequestHandler<TelnetAppSession, StringRequestInfo>> RequsetActionList = new List<RequestHandler<TelnetAppSession, StringRequestInfo>>();
        private ModeDic modeDic = new ModeDic();
        private ActionDic baseActionDic = new ActionDic();
        private TelnetAppServer telnetServer ;
        private Dictionary<string, ProxyUserSession> sessionDic = new Dictionary<string, ProxyUserSession>();
        private FSMConfiguration _fsmConfiguration;
    }
}
