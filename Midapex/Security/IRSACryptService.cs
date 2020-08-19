// ***************************************************************
//  ICryptService.cs   version:  1.4    date: 06/06/2006
//  -------------------------------------------------------------
//	author:		Yangjun Deng
// 	email:		Midapexsoft@gmail.com
// 	purpose:	
//  -------------------------------------------------------------
//  Copyright (C) 2006 - Midapexsoft All Rights Reserved
// ***************************************************************
using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Security
{
    public interface IRSACryptService
    {
        byte[] Decrypt(byte[] inBuffer, string pvk);
        byte[] Encrypt(byte[] inBuffer, string puk);

    }
}
