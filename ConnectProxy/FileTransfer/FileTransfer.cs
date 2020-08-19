using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Midapex.Net.Ftp;
using Midapex.Net;

namespace ConnectProxy.FileTransfer
{
    class FileTransferServer
    {



        
        //private BinaryWriter binaryWriter = null;
        //private FileStream fileStream = null;
    }

    class LmcFtpServer
    {

        public LmcFtpServer()
        {
            server.Capacity = 1000;
            server.HeartBeatPeriod = 120000;
            FtpUser user = new FtpUser("ftp");
            user.Password = "ftp";
            user.AllowWrite = true;
            user.MaxConnectionCount = 1;
            user.MaxUploadFileLength = 1024 * 1024 * 100;
            server.AddUser(user);
            string homeDir = Environment.CurrentDirectory + "/RecviFile";
            if (!Directory.Exists(homeDir))
            {
                Directory.CreateDirectory(homeDir);
            }
            user.HomeDir = homeDir;
            server.AnonymousUser.HomeDir = Environment.CurrentDirectory;
        }
        public void start()
        {
            server.Start();
        }
        public void restart()
        {
            server.Stop();
            server.Start();
        }
        public void stopServer()
        {
            server.Stop();
        }
        private FtpServer server = new FtpServer();
    }

}
