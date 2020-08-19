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
using System.Net;
using System.Net.Sockets;

namespace Midapex.Net.Tcp
{
    /// <summary>
    /// 基于消息的服务器类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class MessageBlockServer<TSession>:TcpServerBase<TSession> where TSession:MessageBlockSession, new()
    {
        int maxMessageLength = 64*1024;

        /// <summary>
        /// 消息最大长度
        /// </summary>
        public int MaxMessageLength
        {
            get { return maxMessageLength; }
            set { maxMessageLength = value; }
        }

        public MessageBlockServer()
        {
            EnableCheckHeartBeat = true;//默认启动检查心跳功能
        }

        public MessageBlockServer(int port)
            :base(port)
        {
            EnableCheckHeartBeat = true;
        }

        public MessageBlockServer(int port, int capacity)
            :base(port, capacity)
        {
            EnableCheckHeartBeat = true;
        }

        protected override bool OnCreateSession(TSession newSession)
        {
            newSession.MaxMessageLength = MaxMessageLength;
            newSession.OnReceivedMessageBlock += new EventHandler<MessageBlockArgs>(SessionOnReceivedMessageBlock);
            base.OnCreateSession(newSession);
            return true;
        }

        void SessionOnReceivedMessageBlock(object sender, MessageBlockArgs e)
        {
            if(EnableCheckHeartBeat)
            {
                if (e.MessageBlock.Type == MessageBlockType.HeartBeat)
                {
                    TSession session = (TSession)sender;
                    //Todo:多线程安全
                    session.TimeCounter.Reset(); //定时器开始新的计时
                    Send(session, e.MessageBlock);
                    NetDebuger.PrintDebugMessage(session, "Heartbeat");
                    return;
                }
            }

            OnReceivedMessageBlock((TSession)sender, e.MessageBlock);
        }

        protected virtual void OnReceivedMessageBlock(TSession session, MessageBlock mb)
        {
            OnReceivedData(session, mb.Body);
        }
     
        public virtual void Send(TSession session, MessageBlock mb)
        {
            base.Send(session, mb.ToDataBlock());
        }

        /// <summary>
        /// 把数据封装到信息中发送
        /// </summary>
        /// <param name="session"></param>
        /// <param name="data"></param>
        public override void Send(TSession session, DataBlock data)
        {
            Send(session, new MessageBlock(data));
        }

        public override void Send(TSession session, byte[] data)
        {
            Send(session,new MessageBlock(data));
        }

        public override void Send(TSession session, byte[] data, int startIndex, int length)
        {
            Send(session, new MessageBlock(data,startIndex,length));
        }
    }
}
