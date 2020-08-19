using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Midapex.Net.Tcp;
using System.Globalization;
using System.Threading;

namespace Midapex.Net.Ftp
{
    public class FtpSession:CommandSession
    {
        #region Constractor
        
        public FtpSession()
        {
            Statue = FtpSessionStatue.NotLogin;
            CurrentDir = Root;
            InitFtpHandlers();
        }

        #endregion

        #region Filed
        FtpSessionStatue statue;
        string password;
        string currentDir;
        string userName;
        internal FtpServer ftpServer;
        FtpDataConnection dataConn;
        bool binaryMode;
        internal FtpUser user;
        string renamePath;
        //虚拟路径分隔符号
        char virtualPathSpliter = '/';
        char localPathSpliter;

        public char LocalPathSpliter
        {
            get { return localPathSpliter; }
            set { localPathSpliter = value; }
        }

        const string Root = "/";
        int opertionTimeout = 60000; //20秒

        /// <summary>
        /// 文件传输开始位置
        /// </summary>
        
        long restartPos;
        #endregion

        #region Property

        public bool BinaryMode
        {
            get { return binaryMode; }
            set { binaryMode = value; }
        }

        public FtpSessionStatue Statue
        {
            get { return statue; }
            set { statue = value; }
        }

        public string HomeDir
        {
            get { return user.HomeDir; }
            set { user.HomeDir = value; }
        }

        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string CurrentDir
        {
            get { return currentDir; }
            set { currentDir = value; }
        }

        #endregion

        internal void Response(string command)
        {
            NetDebuger.PrintDebugMessage(this, command);
            ftpServer.Send(this, command + NewLines[0]);
        }

        public override void Close()
        {
            CloseDataConn();
            if (user != null)
            {
                user.ConnCount--;
            }

            base.Close();
        }

        void CreateDataConnection()
        {
            //关闭以前的数据连接
            CloseDataConn();

            //生成新的数据连接
            dataConn = new FtpDataConnection();
            dataConn.Start();

            IPEndPoint localPoint = (IPEndPoint)dataConn.Socket.LocalEndPoint;
            string ipAddress = ftpServer.PasvIPSetting.Replace(".", ",");
            StringBuilder reply = new StringBuilder();
            reply.Append("227 Entering Passive Mode(");
            reply.Append(ipAddress);
            reply.Append(",");
            reply.Append(localPoint.Port / 256);
            reply.Append(",");
            reply.Append(localPoint.Port % 256);
            reply.Append(").");

            Response(reply.ToString());

            try
            {
                //在规定时间内等待客户端的数据连接
                dataConn.Accept(opertionTimeout);
            }
            catch (TimeoutException)
            {
                CloseDataConn();
                NetDebuger.PrintDebugMessage("Timeout  wait for data connection");
            }
            catch
            {
                CloseDataConn();
                NetDebuger.PrintDebugMessage("Interrupted for wait for data connection");
            }
        }

        /// <summary>
        /// 检查路径
        /// </summary>
        /// <param name="relativeDir">虚拟目录</param>
        /// <param name="ftpOption">检查选项</param>
        /// <param name="localPath">本地路径</param>
        /// <returns>操作结果</returns>
        string GetLocalPath(string virtualPath, FtpOption option, out bool isFile)
        {
            string localPath = string.Empty;

            //保持与Unix文件系统一致
            virtualPath = virtualPath.Replace(new string(localPathSpliter,1), new string(virtualPathSpliter,1));
            virtualPath = virtualPath.Replace(new string(virtualPathSpliter, 2), new string(virtualPathSpliter, 1));

            if (string.IsNullOrEmpty(virtualPath))
            {
                if (option == FtpOption.List)
                    virtualPath = Root;
                else
                    throw new DirectoryNotFoundException(virtualPath);
            }
            else if ( !virtualPath.StartsWith(Root) )
            {
                if (!CurrentDir.EndsWith(virtualPathSpliter.ToString()))
                {
                    virtualPath = CurrentDir + virtualPathSpliter.ToString() + virtualPath;
                }
                else
                {
                    virtualPath = CurrentDir +  virtualPath;
                }
            }

            //NetDebuger.PrintDebugMessage(this,"VIRTUAL PATH:"+virtualPath);
            bool exist = MapVirtualPathToLocalPath(virtualPath, out localPath,out isFile);
            //NetDebuger.PrintDebugMessage(this, "LOCAL PATH:" + localPath);

            //如果当前不是在生成新的文件和目录，就抛出路径不存在的异常
            if( !exist && (option != FtpOption.Create && option != FtpOption.Upload))
            {
                throw new DirectoryNotFoundException(virtualPath);
            }

            CheckAccess(virtualPath,localPath, option);

            return localPath;
        }

        private void CheckAccess(string virtualPath, string localPath, FtpOption option)
        {
            //避免出现访问/../../..的缺陷
            if (localPath.IndexOf(user.HomeDir) == -1)
                throw new DirectoryNotFoundException(virtualPath);

            //不允许读取
            if ((option == FtpOption.List || option == FtpOption.Download))
            {
                if (!user.AllowRead)
                    throw new AccessDeniedException(virtualPath);
            }
            else //不允许写入
            {
                if(!user.AllowWrite)
                    throw new AccessDeniedException(virtualPath);
            }
        }

        /// <summary>
        /// 把虚拟路径映射到本地路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径</param>
        /// <param name="localPath">本地路径</param>
        /// <returns>本地路径存在返回true,否则返回false</returns>
        private bool MapVirtualPathToLocalPath(string virtualPath, out string localPath, out bool isFile)
        {
            //保持与Unix文件系统一致
            virtualPath = virtualPath.Replace("\\", virtualPathSpliter.ToString());
            virtualPath = virtualPath.Replace("//", virtualPathSpliter.ToString());

            string[] dirParties = virtualPath.Split(new char[] { virtualPathSpliter });

            localPath = HomeDir; 

            foreach (string part in dirParties)
            {
                if (part != "..") //上一级目录
                {
                    part.TrimEnd(new char[] { '.' });
                    part.TrimStart(new char[] { '.' });
                }

                if (!string.IsNullOrEmpty(part))
                {
                    if (part == "..") //退到上一级目录
                    {
                        //去掉目录末端的分隔符号
                        localPath = localPath.TrimEnd(new char[] { localPathSpliter });

                        //去掉最后面的目录部分，达到退到上一级目录的目的
                        localPath = StringEx.TrimEnd(localPath, localPathSpliter.ToString());
                    }
                    else
                    {
                        //添加到下级目录
                        localPath = localPath + localPathSpliter.ToString() + part;
                    }
                }
            }

            //去掉双重的目录分隔符号
            localPath = localPath.Replace(localPathSpliter.ToString() + localPathSpliter.ToString(), localPathSpliter.ToString());

            if (Directory.Exists(localPath))
            {
                isFile = false;
                return true;
            }
            else if (File.Exists(localPath))
            {
                isFile = true;
                return true;
            }
            else
            {
                isFile = true;
                return false;
            }
        }

        /// <summary>
        /// 获得虚拟路径
        /// </summary>
        /// <param name="localPath">本地路径</param>
        /// <returns>返回相对路径，失败返回NULL</returns>
        internal string MapLocalPathToVirtualPath(string localDir)
        {
            if( !localDir.EndsWith( localPathSpliter.ToString() ) )
            {
                localDir += localPathSpliter.ToString();
            }

            if ( !HomeDir.EndsWith(localPathSpliter.ToString()))
            {
                HomeDir += localPathSpliter.ToString();
            }

            if(localDir == HomeDir)
            {
                return Root;
            }
            else
            {
                //Unix上的文件名需要大小写匹配
                if (localDir.IndexOf(HomeDir) == -1)
                {
                    NetDebuger.PrintErrorMessage(this, "MAP LOCAL DIR:" + localDir + " TO HOME DIR:"+ HomeDir + " FAIL");
                    return Root;
                }
                else
                {
                    localDir = localDir.Replace(HomeDir, Root);
                    //把本地路径的分隔符号，替换为虚拟路径的分隔符号
                    return localDir.Replace(new string(localPathSpliter,1), new string(virtualPathSpliter,1)).
                        Replace(new string(virtualPathSpliter,2),new string(virtualPathSpliter,1));
                }
            }
        }

        /// <summary>
        /// 发送目录内容列表。（已经建立了数据连接的基础上）
        /// </summary>
        /// <param name="ftpCmd"></param>
        internal void SendPathList(FtpCommand ftpCmd)
        {
            CheckDataConnExist();

            try
            {
                Statue = FtpSessionStatue.List;
                
                if (ftpCmd.Parameters.Count == 0)
                {
                    ftpCmd.Parameters.Add("");
                }

                string virtualPath = ftpCmd.Parameters[0].TrimEnd( '*');
                
                if(virtualPath.StartsWith("-"))
                {
                    //TODO:参数检查功能
                    virtualPath = "";
                }
                
                if (virtualPath == "")
                {
                    virtualPath = CurrentDir;
                }
                bool isFile;

                string localPath = GetLocalPath(virtualPath, FtpOption.List, out isFile);

                StringBuilder sb = new StringBuilder();

                //列举目录
                if (!isFile)
                {
                    Response("150 Opening ASCII mode data connection for directory list."); 

                    DirectoryInfo localDir = new DirectoryInfo(localPath);
                    foreach (DirectoryInfo dir in localDir.GetDirectories())
                    {
                        WriteFileInfo(sb, dir.Name, 0, dir.LastWriteTime, false);
                    }

                    //列举文件
                    foreach (FileInfo file in localDir.GetFiles())
                    {
                        WriteFileInfo(sb, file.Name, file.Length, file.LastWriteTime, true);
                    }
                }
                else
                {
                    Response("150 Opening ASCII mode data connection for directory list."); 
                    FileInfo file = new FileInfo(localPath);
                    WriteFileInfo(sb, file.Name, file.Length, file.LastWriteTime, true);
                }
               
                //NetDebuger.PrintDebugMessage(this, sb.ToString());
                dataConn.Send(new DataBlock(Encoding.GetBytes(sb.ToString())), opertionTimeout);
                Response("226 Transfer complete.");
            }
            catch (System.IO.IOException e)
            {
                throw new InternalException(e.Message);
            }
            finally
            {
                CloseDataConn();
                Statue = FtpSessionStatue.Wait;
            }
            
        }

        private void CheckDataConnExist()
        {
            if (dataConn == null)
            {
                throw new DataConnNotReadyException();
            }
        }

        private void CloseDataConn()
        {
            if (dataConn != null)
            {
                //如果当前的dataConn还在处于等待连接或者发送数据状态，就取消这些操作
                dataConn.AsyncHelper.CancelAsyncOperation();
                dataConn.Stop();
                dataConn = null;
            }
        }

        private void WriteFileInfo(StringBuilder sb, string name, long length, DateTime dateTime, bool isFile)
        {
            if (isFile)
            {
                sb.Append("-rwxr-xr-x");
            }
            else
            {
                sb.Append("drwxr-xr-x");
            }
            
            sb.Append(" 1 user group ");
            //14个空格
            string strFiller = "              ";
            sb.Append(strFiller.Substring(0, strFiller.Length - length.ToString().Length));
            sb.Append(length);
            //sb.Append(dateTime.ToString(" MMM %2d %2H:%2M ", new CultureInfo( "en-US", false ).DateTimeFormat));
            string mouth = dateTime.ToString("MMM", new CultureInfo("en-US", false).DateTimeFormat);
            string writeDate = string.Format(" {0} {1:00} {2:00}:{3:00} ", mouth, dateTime.Day, dateTime.Hour, dateTime.Minute);
            sb.Append(writeDate);
             
            sb.Append(name);
            sb.Append(NewLines[0]);
        }
       
        internal void ChangeDir(FtpCommand ftpCmd)
        {
            string newDir = ftpCmd.Parameters[0];
            bool isFile;
            string localPath = GetLocalPath(newDir, FtpOption.List, out isFile);
            if (!isFile)
            {
                CurrentDir = MapLocalPathToVirtualPath(localPath);
                Response("250 \"" + CurrentDir + "\" is current directory.");
            }
            else
            {
                throw new DirectoryNotFoundException(newDir);
            }
        }

        delegate void FtpCommandHandler(FtpCommand cmd);
        Dictionary<string, FtpCommandHandler> ftpHandlers = new Dictionary<string,FtpCommandHandler>();

        void InitFtpHandlers()
        {
            ftpHandlers.Add("FEAT", FEAT);
            ftpHandlers.Add("MDTM", MDTM);
            ftpHandlers.Add("USER", USER);
            ftpHandlers.Add("PASS", PASS);
            ftpHandlers.Add("CDUP", CDUP);
            ftpHandlers.Add("XCUP", CDUP);
            ftpHandlers.Add("STOR", STOR);
            ftpHandlers.Add("MKD", MKD);
            ftpHandlers.Add("RMD", RMD);
            ftpHandlers.Add("DELE", DELE);
            ftpHandlers.Add("RNTO", RNTO);
            ftpHandlers.Add("RNFR", RNFR);
            ftpHandlers.Add("QUIT", QUIT);
            ftpHandlers.Add("SIZE", SIZE);
            ftpHandlers.Add("RETR", RETR);
            ftpHandlers.Add("REST", REST);
            ftpHandlers.Add("OPTS", OPTS);
            ftpHandlers.Add("SYST", SYST);
            ftpHandlers.Add("PWD", PWD);
            ftpHandlers.Add("XPWD", PWD);
            ftpHandlers.Add("TYPE", TYPE);
            ftpHandlers.Add("PASV", PASV);
            ftpHandlers.Add("CWD", CWD);
            ftpHandlers.Add("LIST", LIST);
            ftpHandlers.Add("NLST", LIST);
            ftpHandlers.Add("NOOP", NOOP);
            ftpHandlers.Add("ALLO", NOOP);
        }

        /// <summary>
        /// 分发FTP请求
        /// </summary>
        /// <param name="cmdText"></param>
        internal void SwitchFtpRequest(string cmdText)
        {
            NetDebuger.PrintErrorMessage(this, cmdText);
            if (string.IsNullOrEmpty(cmdText))
                return;

            FtpCommand cmd;

            int index = cmdText.IndexOf(' '); //把命令和参数部分分开
            if (index != -1)
            {
                //把命令转成大写
                cmd = new FtpCommand(cmdText.Substring(0, index).ToUpper());
                cmd.Parameters.Add(cmdText.Substring(index + 1));
            }
            else
            {
                //把命令转成大写
                cmd = new FtpCommand(cmdText.ToUpper());
            }

            //会话重新开始计时
            TimeCounter.Reset();

            try
            {
                if (!ftpHandlers.ContainsKey(cmd.Command))
                    throw new CommandNotImplementedException();

                //分发各个命令
                ftpHandlers[cmd.Command].Invoke(cmd);
            }
            catch (FtpException e)
            {
                Response(e.Message);
            }
            catch (Exception e)
            {
                NetDebuger.PrintErrorMessage(this, e.ToString());
            }
        }

        private void FEAT(FtpCommand cmd)
        {
            Response("211-Features:\r\n MDTM\r\n SIZE\r\n REST STREAM\r\n PASV\r\n211 End");
        }

        private void MDTM(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            string virtualPath = cmd.Parameters[0];
            bool isFile;
            string localPath = GetLocalPath(virtualPath, FtpOption.List, out isFile);

            if (isFile)
            {
                FileInfo info = new FileInfo(localPath);
                Response("213 " + string.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}",
                    info.LastWriteTime.Year, info.LastWriteTime.Month, info.LastWriteTime.Day,
                    info.LastWriteTime.Hour, info.LastWriteTime.Minute, info.LastWriteTime.Second));
            }
            else
            {
                throw new FileNotFoundException(virtualPath);
            }
        }

        private void CDUP(FtpCommand cmd)
        {
            CheckLogin();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("/..");
            ChangeDir(cmd);
        }

        private void STOR(FtpCommand cmd)
        {
            CheckLogin();
            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            CheckDataConnExist();

            restartPos = 0;

            try
            {
                Statue = FtpSessionStatue.Upload;
                bool isFile;
                string localPath = GetLocalPath(cmd.Parameters[0], FtpOption.Upload,out isFile);
                dataConn.MaxLengthOfUpload = user.MaxUploadFileLength;
                dataConn.RecvFile = new FileStream(localPath, FileMode.Create);
                Response("150 Opening BINARY mode data connection for file transfer.");

                if (dataConn.ReceiveFile(localPath, restartPos))
                    Response("226 Transfer complete.");
                else
                {
                    NetDebuger.PrintErrorMessage(this, dataConn.AsyncHelper.Exception.Message);
                    Response("426 Connection closed; transfer aborted.");
                }
            }
            catch (FtpException)
            {
                throw;
            }
            catch (Exception e)
            {
                NetDebuger.PrintErrorMessage(this, e.ToString());
                throw new InternalException("store file");
            }
            finally
            {
                CloseDataConn();
                Statue = FtpSessionStatue.Wait;
            }
        }

        private void MKD(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();
            bool isFile;
            string localPath = GetLocalPath(cmd.Parameters[0], FtpOption.Create,out isFile);

            if(isFile)
                new DirectoryNotFoundException(cmd.Parameters[0]);

            try
            {
                Directory.CreateDirectory(localPath);
            }
            catch (System.Exception)
            {
                throw new InternalException("create dir");
            }

            Response("250 Created directory successfully");
        }

        private void RMD(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            bool isFile;
            string localPath = GetLocalPath(cmd.Parameters[0], FtpOption.Delete,out isFile);

            if (isFile)
                throw new DirectoryNotFoundException(cmd.Parameters[0]);

            try
            {
                Directory.Delete(localPath);
            }
            catch (System.Exception)
            {
                throw new InternalException("delete dir");
            }

            Response("250 Deleted directory successfully");
        }

        private void DELE(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();
            bool isFile;
            string localPath = GetLocalPath(cmd.Parameters[0], FtpOption.Delete,out isFile);
            
            if (!isFile)
                throw new FileNotFoundException(cmd.Parameters[0]);

            try
            {
                File.Delete(localPath);
            }
            catch (System.Exception)
            {
                throw new InternalException("delete file");
            }

            Response("250 Deleted file successfully");
        }

        private void RNTO(FtpCommand cmd)
        {
            CheckLogin();

            try
            {
                if (cmd.Parameters.Count == 0)
                    throw new SyntaxException();

                if (string.IsNullOrEmpty(renamePath))
                    throw new BadSeqCommandsException();

                bool isFile;
                string destPath = GetLocalPath(cmd.Parameters[0], FtpOption.Create,out isFile);

                try
                {
                    if (Directory.Exists(renamePath))
                    {
                        Directory.Move(renamePath, destPath);
                    }
                    else
                    {
                        File.Move(renamePath, destPath);
                    }
                }
                catch (System.Exception)
                {
                    throw new InternalException("rename path");
                }

                Response("250 Rename successful.");
            }
            finally
            {
                renamePath = null;
            }
        }

        

        private void RNFR(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();
            bool isFile;
            renamePath = GetLocalPath(cmd.Parameters[0], FtpOption.List,out isFile); //列举路径
            Response("350 File or directory exists, ready for destination name.");
        }

        private void HELP(FtpCommand cmd)
        {
            CheckLogin();
            Response("500 No Help Available.");
        }

        private void NOOP(FtpCommand cmd)
        {
            CheckLogin();
            Response("200 NOOP Command Successful.");
        }

        private void SYST(FtpCommand cmd)
        {
            CheckLogin();
            Response("215 UNIX Type: L8");
        }

        private void QUIT(FtpCommand cmd)
        {
            CheckLogin();
            this.Statue = FtpSessionStatue.NotLogin;
            Response("220 " + ftpServer.ByeMessage);
        }

        private void SIZE(FtpCommand cmd)
        {
            CheckLogin();

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            string file = cmd.Parameters[0];
            if (string.IsNullOrEmpty(file))
                throw new SyntaxException();
            bool isFile;
            string localPath = GetLocalPath(file, FtpOption.List,out isFile);
            long length;
            try
            {
                if (isFile)
                {
                    FileInfo info = new FileInfo(localPath);
                    length = info.Length;
                    Response("213 " + info.Length.ToString());
                }
                else
                {
                    throw new FileNotFoundException(file);
                }
            }
            catch (FtpException)
            {
                throw;
            }
            catch (System.Exception e)
            {
                throw new InternalException(e.Message);
            }
        }

        List<Thread> threads = new List<Thread>();

        private void RETR(FtpCommand cmd)
        {
            CheckLogin();
            SendFile(cmd);
        }

        private void SendFile(object arg)
        {
            FtpCommand cmd = arg as FtpCommand;

            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            CheckDataConnExist();

            try
            {
                Statue = FtpSessionStatue.Download;
                bool isFile;
                string localPath = GetLocalPath(cmd.Parameters[0], FtpOption.Download, out isFile);
                if (isFile)
                {
                    Response("150 Opening BINARY mode data connection for file transfer.");

                    if (dataConn.SendFile(localPath, restartPos))
                        Response("226 Transfer complete.");
                    else
                        Response("426 Connection closed; transfer aborted.");
                }
                else
                {
                    throw new FileNotFoundException(cmd.Parameters[0]);
                }
            }
            catch (FtpException)
            {
                throw;
            }
            catch (Exception e)
            {
                NetDebuger.PrintErrorMessage(this, e.ToString());
                throw new InternalException(e.Message);
            }
            finally
            {
                CloseDataConn();
                Statue = FtpSessionStatue.Wait;
            }
        }

        private void REST(FtpCommand ftpCmd)
        {
            CheckLogin();

            if(ftpCmd.Parameters.Count==0)
            {
                throw new SyntaxException();
            }

            long temp = long.Parse(ftpCmd.Parameters[0]);

            if (temp < 0)
                throw new SyntaxException();
            
            restartPos = temp;

            Response(string.Format("350 Restarting at {0}.", restartPos));
            
        }

        private void OPTS(FtpCommand ftpCmd)
        {
            CheckLogin();

            if (ftpCmd.Parameters.Count == 0)
                throw new SyntaxException();

            if (ftpCmd.Parameters[0] == "utf8 on")
            {
                Encoding = Encoding.UTF8;
                Response("200 UTF enabled mode.");
            }
            else if (ftpCmd.Parameters[0] == "utf8 off")
            {
                Encoding = Encoding.Default;
                Response("200 ASCII enabled mode.");
            }
            else
            {
                throw new SyntaxException();
            }
        }

        private void LIST(FtpCommand ftpCmd)
        {
            CheckLogin();
            SendPathList(ftpCmd);
        }

        private void CWD(FtpCommand ftpCmd)
        {
            CheckLogin();
            ChangeDir(ftpCmd);
        }

        private void PASV(FtpCommand ftpCmd)
        {
            CheckLogin();
            CreateDataConnection(); 
        }

        private void TYPE(FtpCommand ftpCmd)
        {
            CheckLogin();

            if (ftpCmd.Parameters.Count == 0)
                throw new SyntaxException();
            string arg = ftpCmd.Parameters[0];
            if (arg == "A")
            {
                binaryMode = false;
                Response("200 ASCII transfer mode active.");
            }
            else if (arg == "I")
            {
                binaryMode = true;
                Response("200 Binary transfer mode active.");
            }
            else
            {
                throw new UnknownTransferModeException();
            }
        }

        private void PWD(FtpCommand ftpCmd)
        {
            CheckLogin();
            Response("257 \"" + CurrentDir + "\" is current directory.");
        }

        private void PASS(FtpCommand ftpCmd)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                throw new NeedUserInfoException();
            }
            else
            {
                if (ftpCmd.Parameters.Count == 0)
                    throw new SyntaxException();

                Password = ftpCmd.Parameters[0];
                if (ftpServer.ValidateUser(this))
                {
                    if (user.ConnCount + 1 > user.MaxConnectionCount)
                    {
                        Response("421 Too many users logged in for this account.");
                        
                        this.user = null;
                    }
                    else
                    {
                        Statue = FtpSessionStatue.Wait;
                        user.ConnCount++;
                        Response("230 User successfully logged in.");
                    }
                }
                else
                {
                    throw new LoginFailException();
                }
            }
        }

        public override bool IsActive(int timeOut)
        {
            NetDebuger.PrintDebugMessage(this, "STATE:" + Statue.ToString());

            //会话超时，而且当前状态或者未登陆
            if ( !base.IsActive(timeOut) && (Statue == FtpSessionStatue.Wait || Statue == FtpSessionStatue.NotLogin))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 保证已经登陆
        /// </summary>
        void CheckLogin()
        {
            if (Statue == FtpSessionStatue.NotLogin)
                throw new NotLoginException();
        }

        private void USER(FtpCommand cmd)
        {
            if (cmd.Parameters.Count == 0)
                throw new SyntaxException();

            //重新登陆，取消以前的状态
            Statue = FtpSessionStatue.NotLogin;
            UserName = cmd.Parameters[0];

            if (ftpServer.ValidateUser(this))
            {
                Statue = FtpSessionStatue.Wait;
                Response("230 User successfully logged in.");
            }
            else
            {
                Response("331 Password required for " + UserName);
            }
        }
    }
}
