using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace FileTransferClient
{

    
    class Program
    {
        static void AsyncCallback(object obj)
        {
            NetworkStream readStream = (NetworkStream)obj;
            byte[] readbuff = new byte[1024];
            while (readStream.CanRead)
            {
                readStream.Read(readbuff, 0, readbuff.Length);
                System.String readString = Encoding.Default.GetString(readbuff);
                if (readString.Length > 0)
                {
                    System.Console.WriteLine(readString);
                    break;
                }
            }
        }
        static void Main(string[] args)
        {
            Thread readThread = null;
            Queue<Tuple<byte[], int, UInt64>> sendQueue = new Queue<Tuple<byte[], int, UInt64>>();
            bool sendFlag = true;
            Mutex mutex = new Mutex(false);
            if (args.Length!= 2)
            {
                System.Console.WriteLine( string.Format("paramerters number is {0} not correctly , please cehck!", args.Length));
                usage();
                return;
            }
            string ipAddress = args[0];
            string fileName = args[1];
           
            if (!File.Exists(fileName))
            {
                System.Console.WriteLine("file is exists,please input the right file!");
            }
            string filePureName =  System.IO.Path.GetFileName(fileName);
            filePureName = filePureName + "#--";
            try
            {
                TcpClient tcpClient = new TcpClient(ipAddress, 12001);
                var clientStream = tcpClient.GetStream();
                //readThread = new Thread(AsyncCallback);
                //readThread.Start(clientStream);


                int readLength = 0;
                byte[] fileBuff = new byte[128];
                FileStream fileStream = new FileStream(fileName, FileMode.Open);

                string fileMD5 = clacFileMD5(fileStream);
                System.Console.WriteLine(String.Format("File:{0} MD5 value:{1}  ", fileName, fileMD5));
                string fileMD5Value = fileMD5 + "-##-";
                fileStream.Close();
                fileBuff = System.Text.Encoding.ASCII.GetBytes(fileMD5Value);
                clientStream.Write(fileBuff, 0, fileMD5Value.Length);
                Thread.Sleep(10);

                fileBuff = System.Text.Encoding.ASCII.GetBytes(filePureName);
                clientStream.Write(fileBuff, 0, filePureName.Length);
                Thread.Sleep(10);
                #region syncSend
                fileStream = new FileStream(fileName, FileMode.Open);
                byte[] readFileBuff = new byte[tcpClient.SendBufferSize];
                while ((readLength = fileStream.Read(readFileBuff, 0, readFileBuff.Length)) > 0)
                {
                    clientStream.Write(readFileBuff, 0, readLength);
                }
                #endregion
                #region AsyncSend
                //Thread readFileTask = new Thread(() =>
                //{
                //    UInt64 PackOder = 0;
                //    byte[] readFileBuff = new byte[tcpClient.SendBufferSize];
                //    while ((readLength = fileStream.Read(readFileBuff, 0, readFileBuff.Length)) > 0)
                //    {
                //        Tuple<byte[], int, UInt64> sendFilePair = new Tuple<byte[], int, UInt64>(readFileBuff, readLength, PackOder);
                //        mutex.WaitOne();
                //        PackOder++;
                //        sendQueue.Enqueue(sendFilePair);
                //        mutex.ReleaseMutex();
                //    };
                //    sendFlag = false;
                //}
                //);
                //Thread sendFileTask = new Thread(() =>
                //{
                //    while (sendFlag || sendQueue.Count != 0)
                //    {
                //        mutex.WaitOne();
                //        if (sendQueue.Count == 0)
                //        {
                //            Thread.Yield();
                //            mutex.ReleaseMutex();
                //            continue;
                //        }
                //        Tuple<byte[], int, UInt64> readFileBuff = null;
                //        readFileBuff = sendQueue.Dequeue();
                //        mutex.ReleaseMutex();
                //        clientStream.Write(readFileBuff.Item1, 0, readFileBuff.Item2);
                //        //mutex.ReleaseMutex();
                //    }
                //}
                //);
                //readFileTask.Start();
                //sendFileTask.Start();
                //readFileTask.Join();
                //sendFileTask.Join();
                #endregion
                //readThread.Join();
                tcpClient.Close();
                fileStream.Close();
                System.Console.WriteLine(string.Format("File: {0} Send success", fileName));
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine("Error: "+ ex.Message);
            }
        }
        static void usage()
        {
            System.Console.WriteLine("FileTranster [remote ipaddress] [fileName]");
        }
        static string clacFileMD5(FileStream stream)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(stream);
            byte[] b = md5.Hash;
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < b.Length; i++)
            {
                sb.Append(b[i].ToString("X2"));
            }
            return sb.ToString();

        }
    }
}
