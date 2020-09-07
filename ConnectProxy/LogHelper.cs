﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy
{
    class LogHelper
    {
        
        public static readonly log4net.ILog loginfo = log4net.LogManager.GetLogger("infoAppender");//这里的 loginfo 和 log4net.config 里的名字要一样
        //public static readonly log4net.ILog logerror = log4net.LogManager.GetLogger("logerror");//这里的 logerror 和 log4net.config 里的名字要一样
        public static void WriteLog(string info)
        {
            if (loginfo.IsInfoEnabled)
            {
                loginfo.Info(info);
            }
        }
    }
}