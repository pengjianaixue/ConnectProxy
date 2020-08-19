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
using System.Security.Cryptography.Xml;
using System.Security;
using System.Security.Cryptography;

namespace Midapex.Security
{
    public class RSAKey
    {
        public RSAKey(string xmlString)
        {
            this.xmlString = xmlString;
        }

        static public RSAKey CreateKey()
        {
            RSAKeyValue rsa = new RSAKeyValue();
            return new RSAKey(rsa.Key.ToXmlString(true));
        }

        string xmlString;

        public string XmlString
        {
            get { return xmlString; }
        }

        public string PublicKey
        {
            get
            {
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlString);
                return rsa.ToXmlString(false);
            }
        }
    }
}
