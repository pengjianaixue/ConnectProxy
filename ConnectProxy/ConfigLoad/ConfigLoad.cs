using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConnectProxy.ConfigLoad
{
    public static class IniFileOperator
    {

        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);

        [DllImport("kernel32.dll")]
        private static extern int WritePrivateProfileString(string section,string key,string val,string filepath);

        public static string getKeyValue(string key, string def, string filename, string section = "ServerConfig")
        {
            StringBuilder value = new StringBuilder(1024);
            GetPrivateProfileString(section, key, def, value, 1024, filename);
            return value.ToString();
        }
        public static void setKeyValue(string key, string val, string filename, string section = "ServerConfig")
        {
            WritePrivateProfileString(section, key, val, filename);
        }
    }
}
