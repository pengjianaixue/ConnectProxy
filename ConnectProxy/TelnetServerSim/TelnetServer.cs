using System;
using System.Collections.Generic;
using SuperSocket.Common;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketService;

namespace ConnectProxy.TelnetServerSim
{
    //using ModeDic = Dictionary<string, Dictionary<string, RequstAction>>;
    //using ActionDic = Dictionary<string, RequstAction>;
    using ModeDic = Dictionary<string, Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>>;
    using ActionDic = Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>;
    class TelnetAppSession : AppSession<TelnetAppSession>
    {

        //
        //     Sends the specified message.
        //
        //   message:
        //     The message.

        public string PropmtSymbol { get; set; }
        public override void Send(string message)
        {
            base.Send(message);
        }
        public void sendNoNewLine(string message)
        {
            base.SocketSession.Client.SendData(System.Text.Encoding.Default.GetBytes(message));

        }
    }
    class TelnetAppServer : AppServer<TelnetAppSession>
    {

    }
    interface RequstAction
    {
        void run(TelnetAppSession session, StringRequestInfo requestInfo);

    }

    class TelnetServer
    {

        public TelnetServer()
        {

            registerAction("", sendPromt);
            registerAction("help", printHelp);
            registerAction("Exit", sessionClose);

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
            //telnetServer.Logger.Error()
            telnetServer.NewSessionConnected += new SessionHandler<TelnetAppSession>(appServer_NewSessionConnected);
            telnetServer.NewRequestReceived  += new RequestHandler<TelnetAppSession, StringRequestInfo>(appServer_NewRequestReceived);
            return true;
        }
        public void stopServer()
        {
            telnetServer.Stop();
        }
        private void appServer_NewSessionConnected(TelnetAppSession session)
        {
            session.Send(string.Format("Welcome to  digital brain telnet server\r\nClient Info: Address: {0},Port: {1}\r\nTime: {2}",
                session.RemoteEndPoint.Address.ToString(), session.Config.Port, session.StartTime.ToString()));

        }
        private void appServer_NewRequestReceived(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            if (requestInfo.Key == "ModeExit")
            {
                isInMode = false;
                currentMode = null;
            }
            if (isInMode)
            {
                actionDic[currentMode](session, requestInfo);
            }
            else if(modeDic.ContainsKey(requestInfo.Key))
            {
                isInMode = true;
                currentMode = requestInfo.Key;
                actionDic[currentMode](session, requestInfo);
            }
            if (actionDic.ContainsKey(requestInfo.Key))
            {
                actionDic[requestInfo.Key](session, requestInfo);
            }
            else
            {
                actionDic[""](session, requestInfo);
            }
        }

        public void sendPromt(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            session.sendNoNewLine("$ ");
        }
        //internal class sendPromt : RequstAction
        //{
        //    public void run(TelnetAppSession session, StringRequestInfo requestInfo)
        //    {
        //        session.sendNoNewLine("$");
        //    }
        //}
        //internal class printHelp : RequstAction
        //{
        //    public void run(TelnetAppSession session, StringRequestInfo requestInfo)
        //    {
        //        session.sendNoNewLine("$");
        //    }
        //}

        private void printHelp(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            foreach (var item in actionDic)
            {
                if (item.Key.Length != 0)
                {
                    session.Send(item.Key);
                }
            }
            session.sendNoNewLine("$");
        }
        private void sessionClose(TelnetAppSession session, StringRequestInfo requestInfo)
        {
            session.Close();
        }
        public void registerMode(string modeName)
        {
            if (modeDic.ContainsKey(modeName))
            {
                return;
            }
            else
            {
                modeDic.Add(modeName,new ActionDic());
            }
        }
        //public void registerAction(string actionName, RequstAction actionFunction, string ModeName = "Base")
        //{
        //    //modeDic[ModeName].Add(actionName, actionFunction);
        //    actionDic.Add(actionName, actionFunction);
        //}
        public void registerAction(string actionName, Action<TelnetAppSession, StringRequestInfo> actionFunction, string ModeName = "Base")
        {
            //modeDic[ModeName].Add(actionName, actionFunction);
            actionDic.Add(actionName, actionFunction);
        }
        private ModeDic modeDic = new ModeDic();
        private ActionDic actionDic = new ActionDic();
        private TelnetAppServer telnetServer = new TelnetAppServer();
        private bool isInMode = false;
        private string currentMode = null;
    }
}
