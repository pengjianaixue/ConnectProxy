using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectProxy.TCAControl;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System.Threading;
using ConnectProxy.ComPortControl;
using System.Web.UI;
namespace ConnectProxy.FSM.FSMAction
{
    class TCAIcolishAction : IFSMAction
    {

        public TCAIcolishAction(ref FSMData fSMData)
        {
            _fsmData = fSMData;
            _TCAIcolishActionDic.Add("ExitIcolishMode", exitIcolishMode);
            TCACommandWarpper.ComportCreated += TCACommandWarpper_ComportCreated;
        }
        private void TCACommandWarpper_ComportCreated(object sender, Tuple<string, string> e)
        {
            Pair pair = new Pair(e.Item2, 0);
            ComportDic.Add(e.Item1, pair);
        }
        private bool openVirtualChannel(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo, out int PortObjectID)
        {
            return TCACommandWarpper.CreateComport(AppSession, stringRequestInfo, out PortObjectID);
        }
        private bool openComPort(RunTimeError runTimeError ,string comPortName)
        {
            ruSerialPort = new RuSerialPort();
            return ruSerialPort.openComport(comPortName, runTimeError);
        }
        private void exitIcolishMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            _fsmData.elevator.Fire(TelnetFSM.Events.GoBack);
        }
        public void runAction(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (stringRequestInfo.Key.Length != 0)
            {
                if (stringRequestInfo.Key == "ExitIcolishMode")
                {
                    exitIcolishMode(AppSession, stringRequestInfo);
                }
                if (!_isEnterMode)
                {
                    if (stringRequestInfo.Key == "OpenVirtualChannel")
                    {
                        RunTimeError runTimeError = new RunTimeError();
                        string cpriPort = stringRequestInfo.GetFirstParam();
                        string comPortName = "";
                        int objectID = 0;
                        if (!ComportDic.ContainsKey(cpriPort))
                        {
                            
                            if (!openVirtualChannel(AppSession, stringRequestInfo, out objectID))
                            {
                                return;
                            }
                        }
                        ComportDic[cpriPort].Second = objectID;
                        comPortName = (string)ComportDic[cpriPort].First;
                        if (isOpen)
                        {
                            ruSerialPort.stopForwardRecviThread();
                            ruSerialPort.close();
                            isOpen = false;
                        }
                        if (openComPort(runTimeError, comPortName))
                        {
                            isOpen = true;
                            ruSerialPort.startForwardRecviThread(AppSession);
                            _isEnterMode = true;
                        }
                        else
                        {
                            AppSession.Send(runTimeError.Errordescription + "\n&");
                        }
                    }
                    else
                    {
                        AppSession.Send("please OpenVirtualChannel: OpenVirtualChannel [CpriPort] \r\n\t like:  OpenVirtualChannel 1A \n&");
                    }
                }
                else
                {
                    string cmd = "";
                    if (stringRequestInfo.GetFirstParam().Length > 0)
                    {
                        cmd = stringRequestInfo.Key + " " + stringRequestInfo.Body;
                    }
                    else
                    {
                        cmd = stringRequestInfo.Key;
                    }
                    ruSerialPort.send(cmd);
                }
            }
            else
            {
                AppSession.sendNoNewLine("&");
            }
            
        }
        private bool isOpen { get; set; }
        private bool _isEnterMode = false;
        private Dictionary<string, Action<TelnetAppSession, StringRequestInfo>> _TCAIcolishActionDic
            = new Dictionary<string, Action<TelnetAppSession, StringRequestInfo>>();
        private FSMData _fsmData;
        public static Dictionary<string, Pair> ComportDic = new Dictionary<string, Pair>();
        //private Thread _icolishComportThread;
        private RuSerialPort ruSerialPort;
    }
}
