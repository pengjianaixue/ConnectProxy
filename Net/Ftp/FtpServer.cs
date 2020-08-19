using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Midapex.Net.Tcp;

namespace Midapex.Net.Ftp
{
    public class FtpServer:CommandServer<FtpSession>
    {
        Dictionary<string, FtpUser> users = new Dictionary<string, FtpUser>();
        string serverIp;
        string byeMessage;
        string welcomeMessage;
        bool allowAnonymous;
        List<string> ipFilter = new List<string>();
        FtpUser anonymousUser;

        public FtpServer()
        {
            EnableCheckHeartBeat = true;

            //1分钟内客户端没有反应，就踢出客户端
            HeartBeatPeriod = 60000;
 
            //默认FTP监听端口
            Port = 12001;
            
            //FTP协议使用的命令分隔符号
            NewLines = new string[]{ "\r\n" };

            WelcomeMessage = "220 Midapex Village FTP Server V2.1 ready.";
            ByeMessage = "Bye.";

            //默认使用中文字符集合来操作，如果是其他的语言，请重新指定。
            Encoding = Encoding.GetEncoding("utf-8");

            //默认允许匿名登陆
            AllowAnonymous = true;

            //默认设置匿名用户的信息
            anonymousUser = new FtpUser("anonymous");

        }

        public FtpUser AnonymousUser
        {
            get { return anonymousUser; }
        }

        /// <summary>
        /// IP过滤器
        /// </summary>
        public List<string> IPFilter
        {
            get { return ipFilter; }
        }

        /// <summary>
        /// 用户集合
        /// </summary>
        public Dictionary<string,FtpUser> Users
        {
            get { return users; }
        }
        
        /// <summary>
        /// 指定PASV命令需要指定FTP Server的IP地址，如果不指定，系统会自动选择一个服务器的IP地址
        /// </summary>
        public string PasvIPSetting
        {
            get { return serverIp; }
            set { serverIp = value; }
        }
      
        /// <summary>
        /// 再见信息
        /// </summary>
        public string ByeMessage
        {
            get { return byeMessage; }
            set { byeMessage = value; }
        }
        
        /// <summary>
        /// 欢迎信息
        /// </summary>
        public string WelcomeMessage
        {
            get { return welcomeMessage; }
            set {
                if (value == null)
                    throw new ArgumentNullException();
                if (value.Length > 75)
                {
                    throw new NetException("Welcom message is too long");
                }
                welcomeMessage = value; 
            }
        }

        /// <summary>
        /// 是否允许匿名登陆
        /// </summary>
        public bool AllowAnonymous
        {
            get { return allowAnonymous; }
            set { allowAnonymous = value; }
        }

        protected override bool OnCreateSession(FtpSession newSession)
        {
            if (!base.OnCreateSession(newSession))
                return false;
            
            //进行IP限制检查
            if (!CheckClientIP(newSession))
                return false;

            newSession.ftpServer = this;
            newSession.Response(WelcomeMessage);

            //通过盘符来判断运行的环境
            if ( AnonymousUser.HomeDir[1] == ':') //windows OS Path()
                newSession.LocalPathSpliter = '\\';
            else
                newSession.LocalPathSpliter = '/';  //Unix OS Path

            return true;
        }

        /// <summary>
        /// 检查是否是有效IP
        /// </summary>
        /// <param name="newSession"></param>
        /// <returns>如果客户端的IP被限制返回false,否则返回true</returns>
        protected virtual bool CheckClientIP(FtpSession newSession)
        {
            IPEndPoint remote = newSession.Socket.RemoteEndPoint as IPEndPoint;
            string remoteIP = remote.Address.ToString();
            foreach (string ip in IPFilter)
            {
                if (remoteIP == ip)
                {
                    NetDebuger.PrintDebugMessage(newSession,"Server rejected the client:"+remoteIP);
                    return false;
                }
            }
            NetDebuger.PrintDebugMessage(newSession, "Server accepted the client:" + remoteIP);
            return true;
        }
       
        protected override void OnReceivedCommand(FtpSession session, string cmdText)
        {
            session.SwitchFtpRequest(cmdText);
        }

        /// <summary>
        /// 是否是一个合法的用户
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected internal virtual bool ValidateUser(FtpSession session)
        {
            if (AllowAnonymous && session.UserName == AnonymousUser.UserName)
            {
                session.user = AnonymousUser;
                return true;
            }

            try
            {
                FtpUser user = Users[session.UserName];
                if (user.Password == session.Password)
                {
                    session.user = user;
                   
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (KeyNotFoundException)
            {
                return false;
            }
        }

        protected override void OnStart()
        {
            if (AllowAnonymous && string.IsNullOrEmpty(AnonymousUser.HomeDir))
                throw new NetException("Not set anonymous user's home directory");

            //如果为服务器绑定一个IP,就需要自动获取一个IP地址
            if (string.IsNullOrEmpty(PasvIPSetting))
            {
                string hostName = Dns.GetHostName();
                IPAddress[] ips = Dns.GetHostAddresses(hostName);
                foreach (IPAddress ip in ips)
                {
                    //只能使用IPV4网络，不支持IPV6
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        PasvIPSetting = ip.ToString();
                        break;
                    }
                    continue;
                }

                if (string.IsNullOrEmpty(PasvIPSetting))
                    throw new NetException("No available network interface");
            }

            base.OnStart();
        }

        protected internal override void ReportError(FtpSession session, Exception e)
        {
            NetDebuger.PrintErrorMessage(e.ToString());
        }

        /// <summary>
        /// 添加一个用户
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(FtpUser user)
        {
            Users.Add(user.UserName, user);
        }

        /// <summary>
        /// 删除一个用户
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(FtpUser user)
        {
            Users.Remove(user.UserName);
        }
    }
}
