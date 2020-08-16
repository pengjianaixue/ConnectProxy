﻿using ConnectProxy.TCALoader;
using ConnectProxy.TelnetServerSim;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConnectProxy.TCAControl
{
    class TCACommandWarpper
    {
        enum ErrorCode:int
        {
            
            TCANotStart = -1,
            OperationalSuccess,
            OperationalFail
        }
        public TCACommandWarpper(string tcaPath)
        {
            tslPath = tcaPath;
            // using the reflection get the command function and to run
            warpperType = Type.GetType("ConnectProxy.TCAControl.TCACommandWarpper");
            warpperObj = this;
            MethodInfo[] TCACommandMethod = warpperType.GetMethods();
            foreach (var item in TCACommandMethod)
            {
                string[] methodDeclarationInfo = item.ToString().Split('(');
                string[] methodName = methodDeclarationInfo[0].Split(' ');
                tcaCommandMethod.Add(methodName[1], item);
            }

        }
        #region Interface
        // using the reflection get the command function and to run
        public void callTCACommand(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (!tCAisOpen)
            {
                AppSession.Send("Error:" + string.Format("{0,4}", ErrorCode.TCANotStart));
                return;
            }
            if (stringRequestInfo.Key.Length == 0)
            {
                return;
            }
            if (tcaCommandMethod.ContainsKey(stringRequestInfo.Key))
            {
                object[]  parameters = new object[] { AppSession, stringRequestInfo };
                tcaCommandMethod[stringRequestInfo.Key].Invoke(this, parameters);
            }

        }

        public void startTCAProgramm(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            RunTimeError runTimeError = new RunTimeError();
            if (tslPath.Length == 0 )
            {
                AppSession.sendNoNewLine("please set the Lab PC TCA(TSL.exe) path");
                return;
            }
            if (!tCAControl.startTCA(runTimeError, "localhost", tslPath))
            {
                AppSession.sendWithAppendPropmt("open TCA fail:"+ runTimeError.Errordescription);
            }
            tCAisOpen = true;

        }
        
        public void stopTCA(RunTimeError error)
        {
            //RumaControlClientFactory.StopAllTools();
        }
        public string getTCAControlLog()
        {
            return null;
        }
        // error How to transfer lmc to lab pc
        public void loadLMC(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (!isVaildCheck())
            {
                AppSession.sendWithAppendPropmt("please start TCA");
                return;
            }
            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            if (!tCAControl.loadLMC(error, getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2), getParameter(stringRequestInfo, 3)))
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }

        }
        //rumaster start
        public void StartPlayBack(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (!isVaildCheck())
            {
                AppSession.sendWithAppendPropmt("please start TCA");
                return;
            }
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.StartPlayBack(error,getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        //rumaster stop
        public void StopPlayBack(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (!isVaildCheck())
            {
                AppSession.sendWithAppendPropmt("please start TCA");
                return;
            }
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.StopPlayBack(error,getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        //rumaster Start Capture
        public void StartCapture(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (!isVaildCheck())
            {
                AppSession.sendWithAppendPropmt("please start TCA");
                return;
            }
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.StartCapture(error, getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        //rumaster Stop Capture
        public void StopCapture(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.StopCapture(error, getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        public void DeleteAllCarriers(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.DeleteAllCarriers(error, getParameter(stringRequestInfo, 1), getParameter(stringRequestInfo, 2));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        public void DeleteCarrier(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if(getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            tCAControl.DeleteCarrier(error, getParameter(stringRequestInfo, 1), 
                getParameter(stringRequestInfo, 2), getParameter(stringRequestInfo, 3));
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }

        public void AddCarrier(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 6)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.AddCarrier(error, paramMeter[1],
                paramMeter[2], paramMeter[3], paramMeter[4], paramMeter[5], paramMeter[6]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        public void SetAxcContainerFormat(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetAxcContainerFormat(error, paramMeter[1],
                paramMeter[2], paramMeter[3]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
            }
        }
        public void GetAxcContainerFormat(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string getdata  = tCAControl.GetAxcContainerFormat(error, paramMeter[1],
                paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            AppSession.sendNoNewLine(getdata);
        }

        public void CpriGetFsInfo_RX(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string getdata = tCAControl.CpriGetFsInfo_RX(error, paramMeter[1],
                paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            AppSession.sendNoNewLine(getdata);
        }
        public void GetCarrierConfig(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {


            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string getdata = tCAControl.GetCarrierConfig(error, paramMeter[1],
                paramMeter[2],paramMeter[3]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            AppSession.sendNoNewLine(getdata);
            
        }
        public void SetCarrierConfig(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 13)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetCarrierConfig(error, paramMeter[1],
                paramMeter[2], paramMeter[3], paramMeter[4], paramMeter[5], paramMeter[6],
                paramMeter[7], paramMeter[8], paramMeter[9], paramMeter[10], 
                paramMeter[11], paramMeter[12], paramMeter[13]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }


        public void SetCpriConfig(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetCpriConfig(error, paramMeter[1],
                paramMeter[2], paramMeter[3]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }
        public void getHwSn(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 0)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] hwSn =  tCAControl.getHwSn(error);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            foreach (var item in hwSn)
            {
                AppSession.Send(item);
            }

        }
        public void SetupClockTriggerSource(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetupClockTriggerSource(error, paramMeter[1],paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }
        public void SetCpriTriggerSource(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetCpriTriggerSource(error, paramMeter[1], paramMeter[2], paramMeter[3]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }
        //================IQ file====================
        public void IQFileClearFile(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.IQFileClearFile(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }

        }
        public void IQFileAdd(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.IQFileAdd(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            
        }
        public void IQFileSetCurrentByName(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.IQFileSetCurrentByName(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            
        }

        public void IQFileGetCurrent(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {


            if (getParameterNumber(stringRequestInfo) != 1)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string data =  tCAControl.IQFileGetCurrent(error, paramMeter[1]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            AppSession.Send(data);

        }

        public void IQFilesGetList(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 1)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string[] data = tCAControl.IQFilesGetList(error, paramMeter[1]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            foreach (var item in data)
            {
                AppSession.Send(item);
            }

        }

        public void SetIQFile(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetIQFile(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }
        //================CPC file====================
        public void CpcFileClearFile(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcFileClearFile(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }

        }

        public void CpcFilesClearAll(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 1)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcFilesClearAll(error, paramMeter[1]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }

        }

        public void CpcFileAdd(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcFileAdd(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            
        }
        public void CpcFileGetCurrent(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {

            if (getParameterNumber(stringRequestInfo) != 1)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string data =  tCAControl.CpcFileGetCurrent(error, paramMeter[1]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            AppSession.Send(data);
        }
        public void CpcFileSetCurrent(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcFileSetCurrent(error, paramMeter[1],paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
            
        }

        public void CpcFileSetLoopLength(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 3)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcFileSetLoopLength(error, paramMeter[1], paramMeter[2], paramMeter[3]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }

        public void CpcListFiles(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 1)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            string data =  tCAControl.CpcListFiles(error, paramMeter[1]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }

        }
        public void CpcSetAxcMode(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.CpcSetAxcMode(error, paramMeter[1],paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }

        public void SetCPCfile(TelnetAppSession AppSession, StringRequestInfo stringRequestInfo)
        {
            if (getParameterNumber(stringRequestInfo) != 2)
            {
                sendParameterError(AppSession);
                return;
            }
            RunTimeError error = new RunTimeError();
            string[] paramMeter = stringRequestInfo.Parameters;
            tCAControl.SetCPCfile(error, paramMeter[1], paramMeter[2]);
            if (error.IsError)
            {
                AppSession.sendWithAppendPropmt(error.Errordescription);
                return;
            }
        }
        #endregion
        private void sendParameterError(TelnetAppSession AppSession)
        {
            AppSession.sendWithAppendPropmt("parameter number error");
        }
        private int getParameterNumber(StringRequestInfo stringRequestInfo)
        {
            return stringRequestInfo.Parameters.Length;
        }
        private string getParameter(StringRequestInfo stringRequestInfo,uint index)
        {
            return stringRequestInfo.Parameters[index];
        }
        private bool isVaildCheck()
        {
            return tCAisOpen;
        }
        private Dictionary<string, MethodInfo> tcaCommandMethod = new Dictionary<string, MethodInfo>();
        private Type warpperType;
        private Object warpperObj;
        private string tslPath = "";
        private bool tCAisOpen = false;
        private TCAControler tCAControl = new TCAControler(); 
    }
}