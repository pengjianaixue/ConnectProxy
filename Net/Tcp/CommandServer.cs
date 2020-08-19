// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Tcp
{
    /// <summary>
    /// 文本命令服务器类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class CommandServer<TSession>:TcpServerBase<TSession> 
        where TSession:CommandSession, new()
    {
        public CommandServer()
        {
            this.Encoding = new UnicodeEncoding();
            NewLines = new string[]{ "\r\n"};
        }

      public CommandServer(int port)
            : base(port)
        {
        }

      public CommandServer(int port, int capacity)
            : base(port, capacity)
        {
        }

        string[] newLines;

        public string[] NewLines
        {
            get { return newLines; }
            set { newLines = value; }
        }

        Encoding encoding;

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        protected override bool OnCreateSession(TSession newSession)
        {
            newSession.Encoding = Encoding;
            newSession.NewLines = NewLines;
            newSession.ReceivedCommand += new EventHandler<TEventArgs<string>>(ReceivedCommand);
            return true;
        }

        void ReceivedCommand(object sender, TEventArgs<string> e)
        {
            OnReceivedCommand((TSession)sender, e.Param);
        }

        protected virtual void OnReceivedCommand(TSession session, string command)
        {
            NetDebuger.PrintDebugMessage(session, command);
        }

        public void Send(TSession session , string cmd)
        {
            Send(session, Encoding.GetBytes(cmd));
        }
    }
}
