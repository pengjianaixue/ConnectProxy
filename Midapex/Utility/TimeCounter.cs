// ***************************************************************
//  version:  1.0   date: 11/27/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Utility
{
    /// <summary>
    /// 计时器类(是线程安全的)
    /// </summary>
    public class TimeCounter : StartableBase
    {
        public TimeCounter()
        {
            Reset();
        }

        object syncObject = new object();

        long startTime;
        long spanTicks;

        public void Reset()
        {
            lock(syncObject)
                startTime = DateTime.Now.Ticks;
        }

        public long Milliseconds
        {
            get { return (long)(Ticks / 10000); }
        }

        public long Seconds
        {
            get { return (long)(Ticks / 10000000); }
        }

        /// <summary>
        /// 从计时器启动到停止时的时间间隔，如果计时器还没有停止，就代表是到当前时间的时间间隔
        /// </summary>
        public long Ticks
        {
            get
            {
                lock (syncObject)
                {
                    if (!IsRun)
                    {
                        return spanTicks;
                    }
                    else
                    {
                        return DateTime.Now.Ticks - startTime;
                    }
                }
            }
        }

        /// <summary>
        /// 开始计时
        /// </summary>
        protected override void OnStart()
        {
            Reset();
        }

        /// <summary>
        /// 停止计时
        /// </summary>
        protected override void OnStop()
        {
            lock (syncObject)
                spanTicks = DateTime.Now.Ticks - startTime;
        }
    }
}
