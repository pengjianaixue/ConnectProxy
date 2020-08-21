using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Sockets;

namespace ConnectProxy.FileTransfer
{
    class FileTransferServer
    {

        public FileTransferServer()
        {
            listener.Start();
            runFlag = true;
            acceptThread = new Thread(acceptTcpClient);
            acceptThread.Start();
        }

        private void acceptTcpClient()
        {
            while (runFlag)
            {
                var tcpClient =  listener.AcceptTcpClient();
                recviThread = new Thread(recviFile);
                recviThread.Start(tcpClient);
            }
        }

        public void recviFile(object obj)
        {
            bool isEnterFileContexet = false;
            string fileName = homeDir + "/";
            try
            {
                //Task<TcpClient> tcpClientTask = (Task<TcpClient>)obj;
                //TcpClient tcpClient = tcpClientTask.Result;
                TcpClient tcpClient = (TcpClient)obj;
                byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                NetworkStream stream = tcpClient.GetStream();
                int readLength = 0;
                while ((readLength = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (isEnterFileContexet)
                    {
                        fileStream.Write(buffer, 0, readLength);
                    }
                    else
                    {
                        string getfileName = System.Text.Encoding.Default.GetString(buffer,0, readLength);
                        string flag = "#--";
                        if (getfileName.Contains(flag))
                        {
                            string[] splitchar = new string[] { flag };
                            string[] recviStrSplit =  getfileName.Split(splitchar,StringSplitOptions.None);
                            if (recviStrSplit.Length == 0)
                            {
                                return;
                            }
                            fileName = fileName + recviStrSplit[0];
                            fileStream = new FileStream(fileName, FileMode.Create);
                            isEnterFileContexet = true;
                            if (recviStrSplit.Length==2)
                            {
                                fileStream.Write(System.Text.Encoding.Default.GetBytes(recviStrSplit[1]), 0, recviStrSplit[1].Length);
                            }
                        }
                    }
                }
                recviIsSuccess = true;
                if (fileStream != null)
                {
                    fileStream.FlushAsync();
                    fileStream.Close();
                }
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                recviIsSuccess = false;
                if (fileStream != null)
                {
                    fileStream.Close();
                }
                if (isEnterFileContexet)
                {
                    File.Delete(fileName);
                }
            }
            
        }
        public void stopServer()
        {
            listener.Stop();
            runFlag = false;
            acceptThread.Abort();
            if (recviThread!= null)
            {
                recviThread.Abort();
            }
            

        }
        public bool recviIsSuccess { get; private set; } = false;
        private string homeDir = Environment.CurrentDirectory + "/RecviFile";
        private bool runFlag = false;
        private Thread acceptThread = null;
        private Thread recviThread = null;
        private TcpListener listener = new TcpListener(System.Net.IPAddress.Any, 12001);
        private FileStream fileStream = null;
    }

    //class LmcFtpServer
    //{

    //    public LmcFtpServer()
    //    {
    //        server.Capacity = 1000;
    //        server.HeartBeatPeriod = 120000;
    //        FtpUser user = new FtpUser("ftp");
    //        user.Password = "ftp";
    //        user.AllowWrite = true;
    //        user.MaxConnectionCount = 1;
    //        user.MaxUploadFileLength = 1024 * 1024 * 100;
    //        server.AddUser(user);
    //        string homeDir = Environment.CurrentDirectory + "/RecviFile";
    //        if (!Directory.Exists(homeDir))
    //        {
    //            Directory.CreateDirectory(homeDir);
    //        }
    //        user.HomeDir = homeDir;
    //        server.AnonymousUser.HomeDir = Environment.CurrentDirectory;
    //    }
    //    public void start()
    //    {
    //        server.Start();
    //    }
    //    public void restart()
    //    {
    //        server.Stop();
    //        server.Start();
    //    }
    //    public void stopServer()
    //    {
    //        server.Stop();
    //    }
    //    private FtpServer server = new FtpServer();
    //}

}
