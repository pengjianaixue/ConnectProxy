// ***************************************************************
// version:  2.0    date: 04/08/2008
//  -------------------------------------------------------------
// 
//  -------------------------------------------------------------
// previous version:  1.4    date: 05/11/2006
//  -------------------------------------------------------------
//  (C) 2006-2008  Midapex All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Tcp
{
    /// <summary>
    ///  安全连接未建立的异常
    /// </summary>
    public class NotBuildSecureConnectionException:NetSecureException
    {
        public NotBuildSecureConnectionException(string message)
            :base(message)
        {
        }

        public NotBuildSecureConnectionException()
            :base("不能与未建立安全通信连接的远端通信。")
        { 
        }
    }
}
