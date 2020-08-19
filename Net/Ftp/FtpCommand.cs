using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Ftp
{
    class FtpCommand
    {
        public FtpCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                throw new FtpException("empty ftp command");

            this.Command = command;
        }

        string command;

        /// <summary>
        /// FTP命令
        /// </summary>
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        List<string> parameters = new List<string>();

        /// <summary>
        /// 参数列表
        /// </summary>
        public List<string> Parameters
        {
            get{return parameters ; }
        }
    }
}
