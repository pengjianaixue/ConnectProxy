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
using Midapex.Security;

namespace Midapex.Net.Tcp
{
    /// <summary>
    /// 安全通信会话类
    /// </summary>
    public class SecureSession:MessageBlockSession
    {
        public SecureSession()
        {
            SymmetricCryptService = new SymmetricCryptServiceBase();
        }

        HandshakeType handshake;
        public HandshakeType Handshake
        {
            get { return handshake; }
            set { handshake = value; }
        }

        /// <summary>
        /// 在握手的时候使用非对称加密方法的通信远端的公匙
        /// </summary>
        string publicKey;

        public string RemotePublicKey
        {
            get { return publicKey; }
            set { publicKey = value; }
        }

        /// <summary>
        /// 是否已经建立安全连接
        /// </summary>
        public bool IsBuildSecureConnection
        {
            get
            {
                return Handshake == HandshakeType.OK;
            }
        }

        public void CheckPrePhase(HandshakeType type)
        {
            if (Handshake != type)
            {
                throw new NetSecureException("错误的安全通信建立顺序");
            }
        }

        ISymmetricCryptService symmetricCryptService;

        /// <summary>
        /// 通信端使用的对称加密方法(如果在服务器端使用，应该用客户端的key和IV来设置它)
        /// </summary>
        public ISymmetricCryptService SymmetricCryptService
        {
            get { return symmetricCryptService; }
            set { symmetricCryptService = value; }
        }

        
    }
}
