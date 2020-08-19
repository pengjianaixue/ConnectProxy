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
using System.IO;

namespace Midapex.Security
{
    public class SymmetricCryptServiceBase:ISymmetricCryptService
    {
        SymmetricAlgorithm sa;

        public SymmetricAlgorithm SA
        {
            get { return sa; }
            set { sa = value; }
        }
       
        public SymmetricCryptServiceBase()
        {
            SA = new TripleDESCryptoServiceProvider();
            
            SA.Key= SecurityCommon.GenerateBytes(SA.KeySize/8);

            SA.IV = SecurityCommon.GenerateBytes(SA.IV.Length);

            sa.Mode = CipherMode.CBC;
            sa.Padding = PaddingMode.ISO10126;
        }

        #region ISymmetricCryptService ≥…‘±

        public byte[] Encrypt(ArraySegment<byte> inBuffer)
        {
             using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, sa.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    //sa.Padding = padding;
                    cs.Write(inBuffer.Array, inBuffer.Offset,inBuffer.Count);
                    cs.FlushFinalBlock();

                    return ms.ToArray();
                }

            }
        }

        public byte[] Decrypt(ArraySegment<byte> inBuffer)
        {
            using (MemoryStream ms = new MemoryStream(inBuffer.Array,inBuffer.Offset,inBuffer.Count))
            {
                //sa.Padding = padding;

                using (CryptoStream cs = new CryptoStream(ms, sa.CreateDecryptor(), CryptoStreamMode.Read))
                using (BinaryReader b = new BinaryReader(cs))
                {
                    //ms.Position = 0;
                    //sa.Padding = padding;
                    return b.ReadBytes(8192);
                }

            }
        }

        #endregion
    }
}
