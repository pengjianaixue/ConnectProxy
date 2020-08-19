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
    /// 泛型事件参数（单个参数）
    /// </summary>
    /// <typeparam name="ParamType">事件参数类型</typeparam>
    public class TEventArgs<ParamType> : EventArgs
    {
        public TEventArgs(ParamType param)
        {
            this.param = param;
        }

        ParamType param;

        public ParamType Param
        {
            get { return param; }
        }
    }

    /// <summary>
    ///  泛型事件参数（两个参数）
    /// </summary>
    /// <typeparam name="Param1Type">事件参数１类型</typeparam>
    /// <typeparam name="Param2Type">事件参数２类型</typeparam>
    public class TEventArgs<Param1Type, Param2Type>:EventArgs
    {
        public TEventArgs(Param1Type param1, Param2Type param2)
        {
            this.param1 = param1;
            this.param2 = param2;
        }

        Param1Type param1;

        public Param1Type Param1
        {
            get { return param1; }
        }

        Param2Type param2;

        public Param2Type Param2
        {
            get { return param2; }
        }
    }

    /// <summary>
    ///  泛型事件参数（三个参数）
    /// </summary>
    /// <typeparam name="Param1Type">事件参数１类型</typeparam>
    /// <typeparam name="Param2Type">事件参数２类型</typeparam>
    /// <typeparam name="Param3Type">事件参数 3类型</typeparam>
    public class TEventArgs<Param1Type, Param2Type, Param3Type> : EventArgs
    {
        public TEventArgs(Param1Type param1, Param2Type param2,Param3Type param3)
        {
            this.param1 = param1;
            this.param2 = param2;
            this.param3 = param3;
        }

        Param1Type param1;
        Param2Type param2;
        Param3Type param3;

        public Param1Type Param1
        {
            get { return param1; }
        }
        public Param2Type Param2
        {
            get { return param2; }
        }

        public Param3Type Param3
        {
            get { return param3; }
            set { param3 = value; }
        }
    }
}
