using System;
// ***************************************************************
//  version:  1.0   date: 12/03/2007
//  -------------------------------------------------------------
//  
//  -------------------------------------------------------------
//  (C)2007 Midapex All Rights Reserved
// ***************************************************************
// 
// ***************************************************************
using System.Collections.Generic;
using System.Text;

namespace Midapex
{
    /// <summary>
    /// 描述一个插件的特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PlusAttribute:Attribute
    {
        public  PlusAttribute(string name)
        {
            this.name = name;
            this.version = "1.0.0.0";
        }

        string manufacturer;
        /// <summary>
        /// 制造商
        /// </summary>
        public string Manufacturer
        {
            get { return manufacturer; }
            set { manufacturer = value; }
        }

        string name;
        /// <summary>
        /// 插件的名称
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        string version;
        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        string description;
        /// <summary>
        /// 插件描述
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
    }
}
