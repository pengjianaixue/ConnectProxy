// ***************************************************************
//  version:  1.0   date: 12/01/2007
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
using System.Collections;

namespace Midapex
{
    /// <summary>
    /// 为强类型集合提供基类（泛型）
    /// </summary>
    /// <typeparam name="T">需要实现的集合强类型</typeparam>
    public class CollectionBase<T>:CollectionBase
    {
        public T this[int index]
        {
            get { return (T)List[index]; }
            set { List[index] = value; }
        }

        public int Add(T value)
        {
            return List.Add(value);
        }

        public int IndexOf( T value ) 
        {
            return List.IndexOf(value);
        }

       public void Insert( int index, T value )
       {
            List.Insert( index, value );
       }

       public void Remove( T value )
       {
            List.Remove( value );
       }

       public bool Contains( T value )
       {
           return List.Contains(value);
       }

    }
}
