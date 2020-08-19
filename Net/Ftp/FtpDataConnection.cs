using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Midapex.Net.Tcp;
using System.IO;

namespace Midapex.Net.Ftp
{
    class FtpDataConnection : TcpServerBase<TcpSession>
    {
        public FtpDataConnection()
        {
            Capacity = 1; //只允许一个连接
        }

        AsyncHelper async = new AsyncHelper();

        public AsyncHelper AsyncHelper
        {
            get { return async; }
            set { async = value; }
        }

        long maxLengthOfUpload = long.MaxValue;

        public long MaxLengthOfUpload
        {
            get { return maxLengthOfUpload; }
            set { maxLengthOfUpload = value; }
        }

        protected override void OnStart()
        {
            base.OnStart();
            //开始接收等待连接的异步操作
            async.BeginAsyncOperation();
        }

        protected override void OnCloseSession(TcpSession session)
        {
            base.OnCloseSession(session);

            //如果对方的Socket关闭，表示数据连接断开，发送异步操作完成的标志。
            async.EndAsyncOperation();
        }

        protected override bool OnCreateSession(TcpSession newSession)
        {
            //接收数据连接的异步操作完成
            async.EndAsyncOperation();
            return true;
        }

        public void Accept(int timeOut)
        {
            async.WaitAsyncResult(timeOut);
        }

        protected override void OnSendEnd(TcpSession session, int sendCount)
        {
            //发送数据完成
            async.EndAsyncOperation();
        }

        public bool Send(DataBlock data,int timeOut)
        {
            //没有连接，不能发送数据
            if (Sessions.Count != 0)
            {
                async.BeginAsyncOperation();
                Send(Sessions[0], data);
                async.WaitAsyncResult(timeOut);
                return true;
            }

            return false;
        }

        protected internal override void ReportError(TcpSession session, Exception e)
        {
            base.ReportError(session, e);

            //触发发生错误的事件
            async.EndAsyncOperationWithError(e);
        }

        int recvFileLength = 0;

        protected override void OnReceivedData(TcpSession session, DataBlock dataBlock)
        {
            try
            {
                recvFile.Write(dataBlock.Buffer, dataBlock.ReadIndex, dataBlock.DataLength);
                recvFileLength += dataBlock.DataLength;
            }
            catch (System.Exception e)
            {
                AsyncHelper.EndAsyncOperationWithError(e);
            }
            
            if(recvFileLength > MaxLengthOfUpload)
            {
                AsyncHelper.EndAsyncOperationWithError(new NetException("upload file's length limit"));
            }

            dataBlock.Reset();
        }

        public bool SendFile(string localPath, long restartPos)
        {
            using (FileStream file = new FileStream(localPath, FileMode.Open, FileAccess.Read))
            {
                file.Seek(restartPos, SeekOrigin.Begin);
                DataBlock data = new DataBlock(4096);
                bool continueSend = true;
                long sendCount = 0;
                long sendLength = file.Length - restartPos;

                while (continueSend)
                {
                    data.Reset();
                    int count = file.Read(data.Buffer, 0, data.WritableLength);
                    if (count == 0)
                        break; //read end
                    data.WriteIndex = count;

                    continueSend = Send(data, 60000); //60秒发送最多8K数据，如果不能发送，放弃此次发送。
                    if (continueSend)
                        sendCount += count;
                }

                if (sendCount == sendLength)
                    return true;
                else
                    return false; //发送了一部分，没有完全发送
            }
        }

        FileStream recvFile;

        public FileStream RecvFile
        {
            get { return recvFile; }
            set { recvFile = value; }
        }

        public bool ReceiveFile(string localPath, long restartPos)
        {
            try
            {
                recvFileLength = 0;
                async.BeginAsyncOperation();
                AsyncHelper.WaitAsyncResult(6 * 60 * 60 * 1000); //必须在6个小时内完成一个文件的传输
            }
            catch (System.Exception)
            {
                recvFile.Close();

                try
                {
                    File.Delete(localPath);
                }
                catch (Exception)
                {
                }
                
                return false;
            }
            finally
            {
                recvFile.Close();
            }
        
            return true;
        }
    }
}
