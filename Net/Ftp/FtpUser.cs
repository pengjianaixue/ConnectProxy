using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Ftp
{
    /// <summary>
    /// 使用FTP服务的用户类
    /// </summary>
    public class FtpUser
    {
        public FtpUser(string userName)
        {
            UserName = userName;
            AllowRead = true;
            AllowWrite = false;

            //默认不进行限制
            MaxUploadFileLength = long.MaxValue;
            MaxConnectionCount = int.MaxValue;
            connLimitPerIP = int.MaxValue;
        }

        string userName;
        string password;
        bool allowRead;
        bool allowWrite;
        string homeDir;
        int maxConnCount;
        long maxUploadFileLength;
        int connLimitPerIP;
        int connCount;

        /// <summary>
        /// 使用该帐号的FTP连接数目
        /// </summary>
        public int ConnCount
        {
            get { return connCount; }
            set { connCount = value; }
        }

        /// <summary>
        /// 每个IP的最大连接数
        /// </summary>
        public int ConnectionLimitPerIP
        {
            get { return connLimitPerIP; }
            set { connLimitPerIP = value; }
        }

        /// <summary>
        /// 限制上传的最大文件长度
        /// (超过这个值，数据连接会被关闭,传输失败)
        /// </summary>
        public long MaxUploadFileLength
        {
            get { return maxUploadFileLength; }
            set { maxUploadFileLength = value; }
        }

        /// <summary>
        /// 使用该帐号的连接最大连接数
        /// </summary>
        public int MaxConnectionCount
        {
            get { return maxConnCount; }
            set { maxConnCount = value; }
        }

        /// <summary>
        /// 用户使用的本地目录(绝对路径)
        /// </summary>
        public string HomeDir
        {
            get { return homeDir; }
            set { homeDir = value; }
        }

        /// <summary>
        /// 是否允许读(包括主要是FTP的List和Download操作)
        /// </summary>
        public bool AllowRead
        {
            get { return allowRead; }
            set { allowRead = value; }
        }

        /// <summary>
        /// 是否允许写（主要涉及FTP中的Delete，Upload和Create操作)
        /// </summary>
        public bool AllowWrite
        {
            get { return allowWrite; }
            set { allowWrite = value; }
        }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password
        {
            get { return password; }
            set { password = value; }
        }
    }
}
