using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace FileTransferClient
{
    class Program
    {
        static void Main(string[] args)
        {
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
                byte[] fileBuff = new byte[1024];
                fileBuff = System.Text.Encoding.ASCII.GetBytes(filePureName);
                clientStream.Write(fileBuff, 0, filePureName.Length);
                while ((readLength = fileStream.Read(fileBuff, 0, fileBuff.Length)) > 0)
                {
                    clientStream.Write(fileBuff, 0, readLength);
                }
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
