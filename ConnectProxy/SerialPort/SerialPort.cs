using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;
using ConnectProxy.TelnetServerSim;

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
        public static string[] getSerialPortList()
        {
            return SerialPort.GetPortNames();
        }
        public bool openComport(string ComportName ,RunTimeError runTimeError)
        {
            if (comPortLists.Contains(ComportName))
            {
                try
                {
                    ruSerialPort.PortName = ComportName;
                    ruSerialPort.Open();
                    terminalPropmt = autoDetectPropmt();
                    isOpen = true;
                    return true;
                }
                catch (System.Exception ex)
                {
                    runTimeError.Errordescription = ex.Message;
                    isOpen = false;
                    return false;
                }
            }
            runTimeError.Errordescription = "can not find the special serial port on this envirment";
            isOpen = false;
            return false;
        }
        public string recviUntilPropmt(bool useCustomPropmtChar = false, string propmt="")
        {
            if (useCustomPropmtChar)
            {
                return read(propmt);
            }
            else
            {

                return read(terminalPropmt);
            }

        }
        public void startForwardRecviThread(TelnetAppSession AppSession)
        {
            telnetAppSession = AppSession;
            recviThreadRunningFlag = true;
            recviThreadRunControl = true;
            recviThread = new Thread(recviandForward);
            recviThread.Start();
        }
        public void suspendForwardRecviThread()
        {
            if (recviThreadRunningFlag)
            {
                recviThread.Suspend();
                recviThreadRunningFlag = false;
            }
        }
        public void resumeForwardRecviThread()
        {
            if (!recviThreadRunningFlag)
            {
                recviThread.Resume();
                recviThreadRunningFlag = true;
            }
        }
        public void stopForwardRecviThread()
        {
            recviThreadRunningFlag = false;
            recviThreadRunControl = false;
            if (recviThread != null)
            {
                recviThread.Join();
            }
            
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
        private string read(string untilString = "")
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
        private void recviandForward()
        {
            string recviMsg = "";
            while (recviThreadRunControl)
            {
                recviMsg = read();
                telnetAppSession.sendNoNewLine(recviMsg);
            }
        }
        private string autoDetectPropmt()
        {
            string propmt = "";
            int propmtCounter = 0;
            int timeOutConunter = 0;
            while (propmtCounter < 3 && timeOutConunter < 200)
            {
                send("");
                Thread.Sleep(10);
                string temp = ruSerialPort.ReadExisting();
                if (temp!= propmt)
                {
                    propmt = temp;
                    propmtCounter++;
                }
                timeOutConunter++;
            }
            return propmt;

        }
        private string terminalPropmt = null;
        private bool recviThreadRunControl = false;
        private bool recviThreadRunningFlag = false;
        private TelnetAppSession telnetAppSession = null;
        private Thread recviThread = null;
        public bool isOpen { get; private set; } = false;
        private string[] comPortLists = null;
        private SerialPort ruSerialPort = new SerialPort();
    }
}
