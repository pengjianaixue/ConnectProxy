using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ConnectProxy.FileTransfer
{


    
    class FileTransferServer
    {
        static string clacFileMD5(FileStream stream)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(stream);
            byte[] b = md5.Hash;
            md5.Clear();
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();

        }
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
            string recviedFileMD5 = "";
            bool isEnterFileContexet = false;
            string fileName = homeDir + "/";
            if (!Directory.Exists(fileName))
            {
                Directory.CreateDirectory(fileName);
            }
            try
            {
                TcpClient tcpClient = (TcpClient)obj;
                byte[] buffer = new byte[1024*1024];
                byte[] bufferPackCombine = null;
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
                        string headerInfo = Encoding.Default.GetString(buffer,0, readLength);
                        string fileNameflag = "#--";
                        string fileMD5flag = "-#-";
                        if (headerInfo.Contains(fileNameflag))
                        {
                            if (bufferPackCombine != null &&  bufferPackCombine.Length !=0)
                            {
                                headerInfo = Encoding.Default.GetString(bufferPackCombine) + headerInfo;
                            }
                            string[] splitchar = new string[] { fileNameflag };
                            string[] recviStrSplit =  headerInfo.Split(splitchar,StringSplitOptions.None);
                            if (recviStrSplit.Length == 0)
                            {
                                return;
                            }
                            fileName = fileName + recviStrSplit[0];
                            fileStream = new FileStream(fileName, FileMode.Create);
                            isEnterFileContexet = true;
                            if ( recviStrSplit.Length==2 &&  (recviStrSplit[1].Length!=0))
                            {
                                fileStream.Write(Encoding.Default.GetBytes(recviStrSplit[1]), 0, recviStrSplit[1].Length);
                            }
                        }
                        else if (headerInfo.Contains(fileMD5flag))
                        {
                            string[] splitchar = new string[] { fileMD5flag };
                            string[] recviStrSplit = headerInfo.Split(splitchar, StringSplitOptions.None);
                            if (recviStrSplit.Length == 0)
                            {
                                return;
                            }
                            recviedFileMD5 = recviStrSplit[0];
                            if (recviStrSplit.Length > 1 && recviStrSplit[1].Length > 0)
                            {
                                bufferPackCombine = Encoding.Default.GetBytes(recviStrSplit[1]);
                            }
                        }
                    }
                }
                recviIsSuccess = true;
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                string reponse = "";
                bool fileIsUnbroken = false;
                fileStream = new FileStream(fileName, FileMode.Open);
                if (clacFileMD5(fileStream) != recviedFileMD5)
                {
                    reponse = "File transmit fail, the file recvied is not broken";
                    stream.Write(Encoding.Default.GetBytes(reponse), 0, reponse.Length);
                }
                else
                {
                    fileIsUnbroken = true;
                    reponse = "File transmit uccess";
                    stream.Write(Encoding.Default.GetBytes(reponse), 0, reponse.Length);
                }
                fileStream.Close();
                if (!fileIsUnbroken)
                {
                    File.Delete(fileName);
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
