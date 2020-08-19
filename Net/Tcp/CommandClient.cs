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
    /// 文本命令客户端类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class CommandClient<TSession> : TcpClientBase<TSession> where TSession : CommandSession, new()
    {
        public CommandClient()
        {
            Encoding = new UnicodeEncoding();
            NewLines = new string[]{ "\r\n"}; 
        }

        string[] newLines;
        Encoding encoding;

        public string[] NewLines
        {
            get { return newLines; }
            set { newLines = value; }
        }

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        protected override void OnCreateSession()
        {
            Session.Encoding = Encoding;
            Session.NewLines = this.NewLines;

            Session.ReceivedCommand += new EventHandler<TEventArgs<string>>(ReceivedCommand);
        }
        
        void ReceivedCommand(object sender, TEventArgs<string> e)
        {
            OnReceivedCommand((TSession)sender, e.Param);
        }

        /// <summary>
        /// 收到命令
        /// </summary>
        /// <param name="session"></param>
        /// <param name="command"></param>
        protected virtual void OnReceivedCommand(TSession session, string command)
        {
        }

        public void Send( string cmd)
        {
            Send(Encoding.GetBytes(cmd));
        }
    }
}
