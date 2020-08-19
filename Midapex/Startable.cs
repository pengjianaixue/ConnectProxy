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

namespace Midapex
{
    /// <summary>
    /// Interface for the class that can be started
    /// </summary>
    public interface IStartable
    {
        void Start();
        void Stop();

        bool IsRun
        {
            get;
        }

        bool CanRun
        {
            get;
        }

        event EventHandler< TEventArgs<IStartable> > OnStartableStatueChange;
    }

    public abstract class StartableBase:IStartable
    {
        protected bool isRun;
        protected bool canRun;

        protected abstract void OnStart();
        protected abstract void OnStop();

        #region IStartable 成员

        public virtual void Start()
        {
            if (IsRun)
            {
                throw new RuntimeException("对象已经在运行，请先停止对象再启动");
            }

            OnStart();
            isRun = true;
            EventHandler<TEventArgs<IStartable>> temp = this.OnStartableStatueChange;
            if(temp!=null)
                temp(this, new TEventArgs<IStartable>(this));
        }

        public virtual void Stop()
        {
            if (!IsRun)
                return;

            OnStop();
            isRun = false;
            EventHandler<TEventArgs<IStartable>> temp = this.OnStartableStatueChange;
            if(temp!=null)
                temp(this, new TEventArgs<IStartable>(this));
        }

        public bool IsRun
        {
            get { return isRun; }
        }

        public bool CanRun
        {
            get { return canRun; }
        }

        public event EventHandler<TEventArgs<IStartable>> OnStartableStatueChange;

        #endregion
    }

}
