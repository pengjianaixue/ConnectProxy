using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace FileTransferClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Queue<Tuple<byte[], int>> sendQueue = new Queue<Tuple<byte[], int>>();
            bool sendFlag = true;
            Semaphore semaphore = new Semaphore(0, 1);
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
                FileStream fileStream = new FileStream(fileName, FileMode.Open);
                int readLength = 0;
                byte[] fileBuff = new byte[tcpClient.SendBufferSize];
                fileBuff = System.Text.Encoding.ASCII.GetBytes(filePureName);
                clientStream.Write(fileBuff, 0, filePureName.Length);
                clientStream.Flush();
                Thread.Sleep(10);
                Thread readFileTask =  new Thread (() =>
                {
                    byte[] readFileBuff = new byte[tcpClient.SendBufferSize];
                    while ((readLength = fileStream.Read(readFileBuff, 0, readFileBuff.Length)) > 0)
                    {
                        Tuple<byte[], int> sendFilePair = new Tuple<byte[], int>(readFileBuff, readLength);
                        mutex.WaitOne();
                        sendQueue.Enqueue(sendFilePair);
                        mutex.ReleaseMutex();
                    };
                    sendFlag = false;
                }
                );
                Thread sendFileTask = new Thread(() =>
                {
                    while (sendFlag || sendQueue.Count!=0)
                    {
                        mutex.WaitOne();
                        if (sendQueue.Count==0)
                        {
                            Thread.Yield();
                            mutex.ReleaseMutex();
                            continue;
                        }
                        Tuple<byte[], int> readFileBuff = null;
                        readFileBuff = sendQueue.Dequeue();
                        mutex.ReleaseMutex();
                        clientStream.Write(readFileBuff.Item1, 0, readFileBuff.Item2);
                    }
                }
                );
                readFileTask.Start();
                sendFileTask.Start();
                readFileTask.Join();
                sendFileTask.Join();
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
    }
}
