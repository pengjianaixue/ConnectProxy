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

namespace Midapex.Net.Tcp
{
    public enum HandshakeType
    {
        ClientHello,   //客户端Hello 
        ServerHello,  //服务器Hello

        ServerKeyExchange, //交换服务器公匙
        ClientKeyExchange, //交换客户端共匙

        ClientSymmetricKey,//对称加密方法私匙
        ServerGetSymmetricKey, //服务器收到客户端对称加密方法私匙的回复

        ClientSymmetricIV, //对称加密方法初始化向量
        ServerGetSymmetricIV,//服务器收到客户端对称加密方法初始化向量的回复

        ServerFinished, 　//服务器认证结束
        ClientFinished,　　//客户端认证结束

        OK//握手成功
    }
}
