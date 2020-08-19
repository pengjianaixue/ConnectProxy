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
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace Midapex.Net.Tcp
{
    /// <summary>
    /// TCP客户端基类
    /// </summary>
    public class TcpClientBase<TSession> : HeartBeatChecker
        where TSession : TcpSession, new() 
    {
        public TcpClientBase()
        {
        }

        public TcpClientBase(string host, int port)
            :this()
        {
            this.Host = host;
            this.Port = port;
        }
        /*
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="datas"></param>
        public  void Send(IList<ArraySegment<byte>> datas)
        {
            try
            {
                if (!IsConnected)
                {
                    NetDebuger.PrintErrorMessage("在发送数据以前，必须先连接服务器。");
                    return;
                }

                Socket.BeginSend(datas, SocketFlags.None,
                    new AsyncCallback(SendEndCallBack),  datas);
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                RaiseErrorEvent(e);
            }
        }
        */

        /// <summary>
        /// 发送数据块
        /// </summary>
        /// <param name="dataBlock"></param>
        public virtual void Send(DataBlock dataBlock)
        {
            SafeSend(dataBlock);
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        public virtual void Send(byte[] data, int startIndex, int length)
        {
            SafeSend(new DataBlock(data,startIndex,length));
        }

        //private void 
        private void SafeSend(DataBlock target)
        {
            lock (sendQueue.SyncRoot)
            {
                sendQueue.Enqueue(target);//添加到发送列表
                NetDebuger.PrintDebugMessage("Send Queue Length:" + sendQueue.Count.ToString());

                if (sendQueue.Count == 1)
                {
                    AtomSend(target);
                }
            }
        }

        private void AtomSend(DataBlock target)
        {
            try
            {
                Debug.Assert(target != null);

                if(Socket!=null)
                    Socket.BeginSend(target.Buffer, target.ReadIndex, target.DataLength, SocketFlags.None,
                                new AsyncCallback(SendEndCallBack),target);
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 100538)
                {
                    RaiseDropLineEvent();
                }
                else
                {
                    RaiseErrorEvent(e);
                }
            }
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="data"></param>
        public virtual void Send(byte[] data)
        {
            SafeSend(new DataBlock(data));
        }

        #region 属性和字段
        Socket socket;
        private string host;
        private int port;
        private TSession session;
        object tag;
        protected bool isConnected;

        Queue sendQueue = new Queue();

        /// <summary>
        /// 关联的用户对象
        /// </summary>
        public object Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        /// <summary>
        /// 客户端是否已经与服务器建立数据连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        public TSession Session
        {
            get { return session; }
        }

        public int Port
        {
            get { return port; }
            set { port = value;}
        }

        public string Host
        {
            get { return host; }
            set { host = value; }
        }

        public Socket Socket
        {
            get { return socket; }
        }

        #endregion

        #region 受保护的成员
        /// <summary>
        /// 建立Tcp连接后处理过程
        /// </summary>
        /// <param name="iar">异步Socket</param>
        private void ConnectCallBack(IAsyncResult iar)
        {
            try
            {
                socket.EndConnect(iar);
                session = new TSession();

                session.Socket = this.Socket; //两者持有同样的Socket
                session.OnReceivedData += new EventHandler<DataBlockArgs>(SessionOnReceivedData);

                OnCreateSession();
                //建立连接后应该立即接收数据
                WaitForData();
                OnConnectServer();
            }
            catch (Exception e)
            {
                //连接服务器失败
                OnConnectServerFail(e);
            }
        }

        /// <summary>
        /// 回话被创建，接收数据之前被调用
        /// </summary>
        protected virtual void OnCreateSession()
        {
        }

        private void SendEndCallBack(IAsyncResult parameter)
        {
            try
            {
                if (Socket != null)
                {
                    int count = Socket.EndSend(parameter);
                    object data = parameter.AsyncState;
                    lock(sendQueue.SyncRoot)
                    {
                        object head = sendQueue.Dequeue();
                        Debug.Assert(head == data);
                        if (sendQueue.Count != 0)
                        {
                            head = sendQueue.Peek();
                            AtomSend(head as DataBlock);
                        }
                    }

                    OnSendEnd(data as DataBlock);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                RaiseErrorEvent(e);
            }
        }

        protected virtual void OnConnectServerFail(Exception e)
        {
            
        }

        private void RaiseErrorEvent(SocketException e)
        {
            OnNetError(e);
            RaiseDropLineEvent();
        }

        protected virtual void OnNetError(SocketException e)
        {

        }

        /// <summary>
        /// 等待接收数据
        /// </summary>
        void WaitForData()
        {
            try
            {
                if (Socket != null && Session != null)
                {
                    Socket.BeginReceive(session.Buffer.Buffer,
                        session.Buffer.WriteIndex, session.Buffer.WritableLength,
                        SocketFlags.None, new AsyncCallback(ReceiveDataCallBack), null);
                }
            }
            catch (ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch (SocketException e)
            {
                if (e.ErrorCode == 100537)
                {
                    RaiseDropLineEvent();
                }
                else
                {
                    RaiseErrorEvent(e);
                }
            }
        }

        /// <summary>
        /// 原始通讯连接已经建立
        /// </summary>
        protected virtual void OnConnectServer()
        {
            isConnected = true;
            StartHeartBeat();
            OnBuildDataConnection();
        }
       
        /// <summary>
        /// 数据连接已经建立
        /// </summary>
        protected virtual void OnBuildDataConnection()
        {
        }

        protected virtual void StartHeartBeat()
        {
            /*如果EnableCheckHeartBeat=true,会启动心跳检查,这样就不能调用基类的OnStart()函数
             * 服务器设定一个心跳超时时间，客户端检查超时的时间应该与此一致。客户端程序会在该时间
             * 的二分之一时间内，发送一个心跳包，服务器会返回一个心跳包，这样客户端就能够知道服务器段能够正确的响应。
             */
            if (EnableCheckHeartBeat)
            {
                checkTimer = new Timer(new TimerCallback(CheckHeartBeatCallBack), null,
                    HeartBeatPeriod / 2, HeartBeatPeriod / 2);
                NetDebuger.PrintDebugMessage("Start heart Beat checker, Period:" + HeartBeatPeriod + "(ms)");
                Session.TimeCounter.Start();
            }
        }

        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="dataBlock">数据块</param>
        protected virtual void OnReceivedData(DataBlock dataBlock)
        {
        }

        protected virtual void OnSendEnd(DataBlock target)
        {
        }


        void SessionOnReceivedData(object sender, DataBlockArgs e)
        {
            OnReceivedData(e.DataBlock);
        }

        protected void ReceiveDataCallBack(IAsyncResult iar)
        {
            try
            {
                if(Disposed)
                {
                    return;
                }

                if( Socket == null )
                {
                    RaiseDropLineEvent();
                    return;
                }

                int readCount = 0;

                lock (Socket)
                {
                    readCount = Socket.EndReceive(iar);
                }

                 if (0 == readCount)
                 {
                     RaiseDropLineEvent();
                 }
                 else
                 {
                     if(Session == null)
                     {
                         return;
                     }

                     lock(Session)
                     {
                         session.ReceivedData(readCount);
                         WaitForData();
                     }
                 }
            }
            catch(ObjectDisposedException)
            {
                RaiseDropLineEvent();
            }
            catch(SocketException e)
            {
                RaiseErrorEvent(e);
            }
        }

        protected virtual void OnDropLine()
        {
        }

        private void RaiseDropLineEvent()
        {
            NetDebuger.PrintDebugMessage("Raise Drop Line Event");
            
            lock (syncSessionObj)
            {
                NetDebuger.PrintDebugMessage("Lock drop");

                bool temp = IsConnected;
                if (temp)
                {
                    Stop();
                    OnDropLine();
                }

                NetDebuger.PrintDebugMessage("UnLock drop");
            }
        }

        #endregion

        protected override void OnStart()
        {
            IPAddress[] hostIPAddress = Dns.GetHostAddresses(Host);

            if (hostIPAddress.Length == 0)
            {
                throw new NetException("Get host ddress fail");
            }

            isConnected = false;
            sendQueue.Clear();
            IPEndPoint iep = new IPEndPoint(hostIPAddress[0], Port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Socket.BeginConnect(iep, ConnectCallBack, null);

            NetDebuger.PrintDebugMessage(string.Format("Connecting server:{0}:{1}...", hostIPAddress[0].ToString(), port));
        }

        object syncSessionObj = new object();

        protected override void OnStop()
        {
            if (!Disposed && IsConnected)
            {
                base.OnStop();
                isConnected = false;
                Session.Close();

                //todo:多线程安全考虑
                /*
                if (IsRun && Session != null)
                {
                    lock (syncSessionObj)
                    {
                        base.OnStop(); //Stop heart Beater

                        Session.Close();
                        session = null;
                        socket = null;
                        isConnected = false;
                    }
                }*/
            }
        }

        protected override void Free(bool dispodedByUser)
        {
            if(dispodedByUser)
            {
                Stop();
            }

            base.Free(dispodedByUser);
        }

        protected override void CheckHeartBeatCallBack(object o)
        {
            if(Session !=null)
            {
                if (!Session.IsActive(HeartBeatPeriod))
                {
                    Stop();
                }
            }
        }
    }
}
