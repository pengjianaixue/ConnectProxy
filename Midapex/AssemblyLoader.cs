// ***************************************************************
//  version:  1.0   date: 11/28/2007
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
using System.Reflection;
using System.IO;

namespace Midapex
{
    public static class AssemblyLoader
    {
        public static void LoadTypes<AttributeType>( Assembly assembly,Dictionary<Type, AttributeType> types)
        {
            foreach (Module m in assembly.GetModules())
            {
                foreach (Type type in m.GetTypes())
                {
                    object[] attributes = type.GetCustomAttributes(typeof(AttributeType), true);
                    if (attributes.Length != 0)
                    {
                        types.Add(type, (AttributeType)attributes[0]);
                    }
                }
            }
        }


        /// <summary>
        /// 获得当前目录下的实现了指定特性的类型
        /// </summary>
        /// <typeparam name="AttributeType">指定特性</typeparam>
        /// <returns>类型和特性字典</returns>
        public static Dictionary<Type, AttributeType> LoadTypes<AttributeType>()
        {
            Dictionary<Type, AttributeType> types = new Dictionary<Type, AttributeType>();
            //得到进程入口程序集
            Assembly assembly = Assembly.GetEntryAssembly();
            if(assembly!=null) //ASP.NET中为空
            {
                LoadTypes<AttributeType>(assembly, types);
                //获得程序当前目录下的程序集
                string[] dllFiles = Directory.GetFiles(Path.GetDirectoryName(assembly.Location), "*.dll", SearchOption.TopDirectoryOnly);
                foreach (string dllFile in dllFiles)
                {
                    LoadTypes<AttributeType>(Assembly.LoadFile(dllFile), types);
                }
            }

            return types;
        }
    }
}
