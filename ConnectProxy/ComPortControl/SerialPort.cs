using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace ConnectProxy.ComPortControl
{
    class RuSerialPort
    {
        public RuSerialPort()
        {
            ruSerialPort.BaudRate = 115200;
            ruSerialPort.DataBits = 8;
            ruSerialPort.Parity = Parity.None;
            ruSerialPort.StopBits = StopBits.One;
            comPortLists = SerialPort.GetPortNames();
           
        }
        public bool openComport(string ComportName)
        {
            if (comPortLists.Contains(ruSerialPort.PortName))
            {
                try
                {
                    ruSerialPort.PortName = ComportName;
                    ruSerialPort.Open();
                    isOpen = true;
                    return true;
                }
                catch (System.Exception ex)
                {
                    isOpen = false;
                    return false;
                }
            }
            return false;


        }
        public string sendAndRecvi(string cmd, string untilString = "")
        {
            try
            {
                send(cmd);
                return read(untilString);
            }
            catch (System.Exception ex)
            {
                return ex.ToString();
            }
            
        }
        public bool send(string cmd)
        {
            try
            {
                ruSerialPort.Write(cmd);
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            
        }
        public string read(string untilString = "")
        {
            try
            {
                if (untilString.Length == 0)
                {
                    return ruSerialPort.ReadExisting();
                }
                else
                {
                    return ruSerialPort.ReadTo(untilString);
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }
            
        }
        public bool close()
        {
            try
            {
                ruSerialPort.Close();
                isOpen = false;
                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
            
        }
        public bool isOpen { get; private set; } = false;
        private string[] comPortLists = null;
        private SerialPort ruSerialPort = new SerialPort();
    }
}
