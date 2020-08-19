using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Ftp
{
    enum FtpOptionResult
    {
        AccessDenied,
        NotFindPath,
        OK,
    }

    public enum FtpSessionStatue
    {
        /// <summary>
        /// 已经登陆，空闲状态
        /// </summary>
        Wait,
        /// <summary>
        /// 未登陆
        /// </summary>
        NotLogin,
        /// <summary>
        /// 正在执行List命令
        /// </summary>
        List,
        /// <summary>
        /// 正在上传文件
        /// </summary>
        Upload,
        /// <summary>
        /// 正在下载文件
        /// </summary>
        Download,
    }

    enum FtpOption
    {
        List,
        Create,
        Upload,
        Download,
        Delete
    }
}
