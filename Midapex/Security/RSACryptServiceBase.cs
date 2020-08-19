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
using System.Security.Cryptography.Xml;


namespace Midapex.Security
{
    public class RSACryptServiceBase:IRSACryptService
    {
        protected  RSACryptServiceBase()
        {
        }

        RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

        #region IRSACryptService ≥…‘±

        public byte[] Decrypt(byte[] inBuffer, string pvk)
        {
            rsa.FromXmlString(pvk);
            return rsa.Decrypt(inBuffer, false);
        }

        public byte[] Encrypt(byte[] inBuffer, string puk)
        {
            rsa.FromXmlString(puk);
            return rsa.Encrypt(inBuffer, false);
        }

        #endregion
    }

    
}
