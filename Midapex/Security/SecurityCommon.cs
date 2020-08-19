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
using System.Security.Cryptography;

namespace Midapex.Security
{
    public static class SecurityCommon
    {
        /// <summary>
        /// 随机生成一指定长度的字节数组
        /// </summary>
        /// <param name="byteLength"></param>
        /// <returns></returns>
        public static byte[] GenerateBytes(int byteLength)
        {
            byte[] buff = new Byte[byteLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // 该数组已使用密码增强的随机字节进行填充
            rng.GetBytes(buff);
            return buff;
        }

        public static string Md5( string org)
        {
            byte[] bytes = MD5CryptoServiceProvider.Create().ComputeHash(Encoding.Unicode.GetBytes(org));
            return Convert.ToBase64String(bytes);
        }
    }
}
