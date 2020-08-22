using System;
using System.Collections.Generic;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketService;

namespace ConnectProxy.TelnetServerSim
{
    

    using ModeDic = Dictionary<string, Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>>;
    using ActionDic = Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>;
    class TelnetAppSession : AppSession<TelnetAppSession>
    {

        //public event EventHandler<> seeionClosed;

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
        protected override void OnSessionClosed(CloseReason reason)
        {


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

        public TelnetServer()
        {
            baseActionDic.Add("", sendPromt);
            baseActionDic.Add("help", printHelp);
            baseActionDic.Add("Exit", sessionClose);
        }
        public bool startServer(string port)
        {
            if (!telnetServer.Setup(port.ToInt32()))
            {
                return false;
            }
            if (!telnetServer.Start())
            {
                return false;
            }
            telnetServer.NewSessionConnected += new SessionHandler<TelnetAppSession>(appServer_NewSessionConnected);
            telnetServer.SessionClosed += TelnetServer_SessionClosed;
            resetRequsetAction();
            return true;
        }

        private void TelnetServer_SessionClosed(TelnetAppSession session, CloseReason value)
        {
            isConnect = false;
        }
        //public event EventHandler<string> modeChangInfo;
        public void stopServer()
        {
            telnetServer.Stop();
        }
        private void appServer_NewSessionConnected(TelnetAppSession session)
        {
            session.Send(string.Format("Welcome to  digital brain telnet server\r\nServer Info: Address: {0},Port: {1}\r\nTime: {2}",
                session.LocalEndPoint.Address.MapToIPv4(), session.Config.Port, session.StartTime.ToString()));
            isConnect = true;
        }
        public void resetRequsetAction()
        {
            telnetServer.NewRequestReceived += new RequestHandler<TelnetAppSession, StringRequestInfo>(appServer_NewRequestReceived);
            RequsetActionList.Add(appServer_NewRequestReceived);

        }
        public void registerRequsetAction(RequestHandler<TelnetAppSession, StringRequestInfo> requestHandle)
        {
            //telnetServer.SetupCommands()
            foreach (var item in RequsetActionList)
            {
                telnetServer.NewRequestReceived -= item;
            }
            RequsetActionList.Clear();
            RequsetActionList.Add(requestHandle);
            telnetServer.NewRequestReceived += requestHandle;

        }
        public void registerAction(string actionName, Action<TelnetAppSession, StringRequestInfo> actionFunction, string ModeName = "Base")
        {
            serverAction.Add(actionName, actionFunction);
        }
        public ActionDic serverAction
        {
            get;
            set;
        }
        public bool isConnect {get;set;}
        private void appServer_NewRequestReceived(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            if (baseActionDic.ContainsKey(requestInfo.Key))
            {
                baseActionDic[requestInfo.Key](session, requestInfo);
            }
        }
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
        private List<RequestHandler<TelnetAppSession, StringRequestInfo>> RequsetActionList = new List<RequestHandler<TelnetAppSession, StringRequestInfo>>();
        private ModeDic modeDic = new ModeDic();
        private ActionDic baseActionDic = new ActionDic();
        private TelnetAppServer telnetServer = new TelnetAppServer();
    }
}
