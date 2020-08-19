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

namespace Midapex.Net.Tcp
{
    /// <summary>
    /// 基于消息的客户端类
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public class MessageBlockClient<TSession> : TcpClientBase<TSession> 
        where TSession : MessageBlockSession, new()
    {
        public MessageBlockClient(string hostNameOrAddress, int listenPort)
            : base(hostNameOrAddress, listenPort)
        {
            EnableCheckHeartBeat = true; 
        }
      
        public MessageBlockClient()
        {
            EnableCheckHeartBeat = true; //默认启动检查心跳功能
        }

        protected override void OnCreateSession()
        {
            base.OnCreateSession();
            Session.OnReceivedMessageBlock += new EventHandler<MessageBlockArgs>(SessionOnReceivedMessageBlock);
        }

        void SessionOnReceivedMessageBlock(object sender, MessageBlockArgs e)
        {
            if(e.MessageBlock.Type ==  MessageBlockType.HeartBeat && EnableCheckHeartBeat)
            {
                Session.TimeCounter.Reset(); //Refresh the heart Beat timer        
                NetDebuger.PrintDebugMessage(Session, "Recv server heart Beat");    
            }
            else
            {
                OnReceivedMessageBlock(e.MessageBlock);
            }
        }

        protected virtual void OnReceivedMessageBlock(MessageBlock mb)
        {
            OnReceivedData(mb.Body);
        }

        public virtual void Send(MessageBlock mb)
        {
            base.Send(mb.ToDataBlock());
        }
        
        public override void Send(byte[] data)
        {
            Send(new MessageBlock(data));
        }

        public override void Send(byte[] data, int startIndex, int length)
        {
            Send(new MessageBlock(data, startIndex, length));
        }

        public override void Send(DataBlock dataBlock)
        {
            Send(new MessageBlock(dataBlock));
        }

        protected override void CheckHeartBeatCallBack(object o)
        {
            //If client is on line, go on send heart Beat singal
            if (IsConnected)
            {
                base.CheckHeartBeatCallBack(o);
            }

            if (IsConnected)//如果没有掉线，继续发送心跳信号
            {
                MessageBlock heartBeatMB = new MessageBlock(MessageBlockType.HeartBeat);
                Send(heartBeatMB);
                NetDebuger.PrintDebugMessage(Session, "Send Heart Beat");
            }
        }
    }
}
