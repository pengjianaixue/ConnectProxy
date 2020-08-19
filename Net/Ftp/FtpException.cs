using System;
using System.Collections.Generic;
using System.Text;

namespace Midapex.Net.Ftp
{
    public class FtpException:NetException
    {
        public FtpException(string message)
            :base(message)
        {
        }
    }

    public class CommandNotImplementedException:FtpException
    {
        public CommandNotImplementedException()
            : base("502 Command not implemented.")
        {
        }
    }

    public class NeedLoginException:FtpException
    {
        public NeedLoginException()
            :base("530 Please login with USER and PASS.")
        {

        }
    }

    public class NeedUserInfoException : FtpException
    {
        public NeedUserInfoException()
            : base("503 Login with USER first.")
        {
        }
    }

    public class LoginFailException:FtpException
    {
        public LoginFailException()
            :base("530 Not logged in, user or password incorrect!")
        {
        }
    }

    public class AccessDeniedException:FtpException
    {
        public AccessDeniedException(string path)
            :base("550 \""+path+"\": Permission denied.")
        {

        }
    }

    public class DirectoryNotFoundException:FtpException
    {
        public DirectoryNotFoundException(string path)
            : base("550 \""+path+"\": Directory not found.")
        { 
        }
    }

    public class FileNotFoundException : FtpException
    {
        public FileNotFoundException(string file)
            : base("550 \"" + file + "\": File not found.")
        {

        }
    }

    public class DirectoryExistsException : FtpException
    {
        public DirectoryExistsException(string dir)
            : base("550 Directory already exists.")
        {
        }
    }

    public class InternalException : FtpException
    {
        public InternalException(string desp)
            : base("450 Internal error for " + desp)
        {
        }
    }

    public class BadSeqCommandsException : FtpException
    {
        public BadSeqCommandsException()
            : base("503 Commands bad sequence .")
        {
        }
    }

    public class SyntaxException : FtpException
    {
        public SyntaxException()
            :base("501 Syntax error in arguments.")
        {}
    }

    public class DataConnNotReadyException : FtpException
    {
        public DataConnNotReadyException()
            : base("425 Data connection is not ready.")
        {
        }
    }

    /// <summary>
    /// 未登录的异常
    /// </summary>
    public class NotLoginException : FtpException
    {
        public NotLoginException()
            : base("530 Please authenticate firtst.")
        {
        }
    }

    public class UnknownTransferModeException:FtpException
    {
        public UnknownTransferModeException()
            : base("550 Error - unknown transfer mode.")
        {
        }
    }
}
