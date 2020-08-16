using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tiger.Ruma;
using Tiger.Ruma.WcfInterface;
using TigerApplicationServiceClient;
using TSLControlClient.TslControl;
using log4net.Appender;

namespace ConnectProxy.TCALoader
{
    class TCAControler
    {
        public TCAControler()
        {
            //cpriPortMapping.Add("1A", 0);
            //cpriPortMapping.Add("1B", 1);
            //cpriPortMapping.Add("2A", 2);
            //cpriPortMapping.Add("2B", 3);
            //cpriPortMapping.Add("3A", 4);
            //cpriPortMapping.Add("3B", 5);
            //cpriPortMapping.Add("4A", 6);
            //cpriPortMapping.Add("4B", 7);
            //cpriPortMapping.Add("5A", 8);
            //cpriPortMapping.Add("5B", 9);
            //cpriPortMapping.Add("6A", 10);
            //cpriPortMapping.Add("6B", 11);
            //cpriPortMapping.Add("7A", 12);
            //cpriPortMapping.Add("7B", 13);
            //cpriPortMapping.Add("8A", 14);
            //cpriPortMapping.Add("8B", 15);
            //logRecorder = new FileAppender(ILayout layout, "./TCALoader/" + DateTime.Now.ToString() + "log.txt", true);
            //logRecorder.Writer.WriteLineAsync();
        }
        ~TCAControler()
        {
            
        }
        // Interface
        #region Interface
        public bool startTCA(RunTimeError error, string addresss, string exePath = "")
        {
            //rumaClient = RumaControlClientFactory.Create(addresss);
            //System.Console.WriteLine(rumaClient.RuMaUtilities.GetProductInfo());
            try
            {
                tas = new ApplicationControl(addresss);
                string[] tsls = tas.GetTslList();
                if (tsls.Length == 0)
                {
                    tas.StartTsl();
                    tsls = tas.GetTslList();
                    if (tsls.Length == 0)
                    {
                        error.Errordescription = "Can not start TCA TSL";
                        return false;
                    }
                }
                tsl = new TslControlClient(tsls[0]);
                string[] hwSnrs = tsl.GetHws();
                if (hwSnrs.Length == 0)
                {
                    error.Errordescription = "Can not get HW link";
                    return false;
                }
                string[] uris = tsl.GetServiceList(ToolType.ID_RUMA);
                string toolUri;
                if (uris.Length == 1)
                {
                    toolUri = uris[0];
                }
                else if (uris.Length > 1)
                {
                    toolUri = uris[uris.Length - 1];
                }
                else
                {
                    toolUri = tsl.StartService(ToolType.ID_RUMA, hwSnrs[0]);
                }
                //error CpriPort mapping to  Port number

                this.rumaClient = RumaControlClientFactory.Create(toolUri);
                this.rCpriDataFlow = this.rumaClient.CpriDataFlow;
                this.rCarrierConfig = this.rumaClient.CarrierConfig;
                this.rCpriConfig = this.rumaClient.CpriConfig;
                this.rServerBase = this.rumaClient.PlatformUtilities;
                this.rTriggerConfig = this.rumaClient.TriggerConfig;
                this.rRULoader = this.rumaClient.OoM.RULoader;
                this.rDebugPortConfig = this.rumaClient.DebugPortConfig;
            }
            catch (System.Exception e)
            {
                WriteTraceText(error,"StartPlayBack" + e.Message);
                error.Errordescription = "startTCA error : " + e.Message;
            }
            return true;
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
        public bool loadLMC(RunTimeError error, string lmcPath, string cpriPort, string physPos = "1", string restart = "true")
        {
            try
            {
                return Convert.ToBoolean(rRULoader.UpgradeRU(lmcPath, cpriPortMapping[cpriPort], Convert.ToUInt64(physPos), boolDic[restart]));
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Load LMC error : " + e.Message);
                return false;
            }

        }
        //rumaster start
        public void StartPlayBack(RunTimeError error, string cpriport, string flow)
        {

            try
            {
                rCpriDataFlow.StartPlayBack(cpriport, getFlowsDataType(flow));
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Start PlayBack error : " + e.Message);
            }
        }
        //rumaster stop
        public void StopPlayBack(RunTimeError error, string cpriport, string flow)
        {
            try
            {
                rCpriDataFlow.StopPlayBack(cpriport, getFlowsDataType(flow));
            }
            catch (Exception e)
            {
                WriteTraceText(error, "StopPlayBack error :" + e.Message);

            }
        }
        //rumaster Start Capture
        public void StartCapture(RunTimeError error, string cpriport, string flow)
        {
            try
            {
                rCpriDataFlow.StartCapture(cpriport, flowDataTypeDic[flow]);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Start Capture error :" + e.Message);
            }
        }
        //rumaster Stop Capture
        public void StopCapture(RunTimeError error, string cpriport, string flow)
        {

            try
            {
                rCpriDataFlow.StopCapture(cpriport, flowDataTypeDic[flow]);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Stop Capture error : " + e.Message);
            }
        }
        public void DeleteAllCarriers(RunTimeError error, string cpriport, string flowdirection)
        {
            try
            {
                rCarrierConfig.DeleteAllCarriers(cpriport, flowDirectionDic[flowdirection]);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Delete All Carriers error : " + e.Message);
            }
        }
        public void DeleteCarrier(RunTimeError error, string cpriport, string flowdirection, string carrierindex)
        {
            try
            {
                rCarrierConfig.DeleteCarrier(cpriport, flowDirectionDic[flowdirection], byte.Parse(carrierindex));
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Delete Carriers error : " + e.Message);
            }
        }


        public string AddCarrier(RunTimeError error, string cpriport, string flowdirection, string carrierid, string axccontainer, string frequency, string technology)
        {
            try
            {
                byte carrierIndex = byte.Parse("0");
                string consistencyWarning = "none";
                rCarrierConfig.AddCarrier(cpriport, flowDirectionDic[flowdirection], byte.Parse(carrierid), uint.Parse(axccontainer), freqDic[frequency], technologyDic[technology], out carrierIndex, out consistencyWarning);

                return carrierIndex.ToString() + "|" + consistencyWarning;
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Add Carrier  error : " + e.Message);
                return "";
            }
        }

        private string showCarrierData(CarrierData data)
        {
            //#cpriPort#flowDirection#axcContainerGroupLength#bfPeriod#carrierId#carrierNumber#enabled#fsInfo#gain#frequency##axcContainer#syncMode#technology 
            string strdata = "";

            string carrierNumber = data.carrierNumber.ToString();
            strdata = strdata + "Number=" + carrierNumber;
            if (data.enabled == true)
            {
                strdata = strdata + "Enabled=" + "true";
            }
            else if (data.enabled == false)
            {
                strdata = strdata + "Enabled=" + "false";
            }
            string carrierId = data.carrierId.ToString();
            strdata = strdata + "Id=" + carrierId;
            string tech = data.technology.ToString();
            strdata = strdata + "Technology=" + tech;
            string Frequency = data.sampleFrequency.ToString();
            strdata = strdata + "SampleFrequency=" + Frequency;

            string startAxcContainer = data.startAxcContainer.ToString();
            strdata = strdata + "AxcContainer=" + startAxcContainer;

            Gain gaindata = data.gain;

            if (gaindata.GainEnable == true)
            {
                strdata = strdata + "GainEnable=" + "true";
            }
            else if (gaindata.GainEnable == false)
            {
                strdata = strdata + "GainEnable=" + "false";
            }

            string gaindb = gaindata.GainDb.ToString();
            strdata = strdata + "GainDb=" + gaindb;
            string gainfact = gaindata.GainFact.ToString();
            strdata = strdata + "GainFact=" + gainfact;


            FsInfo fsinfodata = data.fsInfo;
            string addr = fsinfodata.Addr.ToString();
            strdata = strdata + "FsInfoAddress=" + addr;
            string hf = fsinfodata.Hf.ToString();
            strdata = strdata + "FsInfoHF=" + hf;
            string bf = fsinfodata.Bf.ToString();
            strdata = strdata + "FsInfoBF=" + bf;
            string bfoffset = fsinfodata.BfOffset.ToString();
            strdata = strdata + "FsInfoBfOffset=" + bfoffset;


            string axcContainerGroupLength = data.axcContainerGroupLength.ToString();
            strdata = strdata + "axcContainerGroupLength=" + axcContainerGroupLength;
            string bfPeriod = data.bfPeriod.ToString();
            strdata = strdata + "bfPeriod=" + bfPeriod;

            string syncmode = data.syncMode.ToString();
            strdata = strdata + "syncMode=" + syncmode;

            //WriteTraceText(strdata);
            return strdata;

        }
        public void SetAxcContainerFormat(RunTimeError error, string cpriport, string flowdirection, string format)
        {
            try
            {
                rCpriDataFlow.SetAxcContainerFormat(cpriport, flowDirectionDic[flowdirection], axcContainerFormatDic[format]);
            }
            catch (Exception e)
            {

                WriteTraceText(error, "Rumaster Set AxcContainerFormat  error : " + e.Message);
            }
        }
        public string GetAxcContainerFormat(RunTimeError error, string cpriport, string flowdirection)
        {
            string getdata = "";
            try
            {
                getdata = rCpriDataFlow.GetAxcContainerFormat(cpriport, flowDirectionDic[flowdirection]).ToString();
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Get AxcContainerFormat  error : " + e.Message);
            }
            return getdata;
        }
        public string CpriGetFsInfo_RX(RunTimeError error, string cpriport, string carrierIndex)
        {
            string getdata = "";
            FsInfo currentFsInfo = new FsInfo();
            FsInfo axcFsInfo = new FsInfo();
            FsInfo customFsInfo = new FsInfo();
            try
            {
                rCarrierConfig.CpriGetFsInfo_RX(cpriport, byte.Parse(carrierIndex), out currentFsInfo, out axcFsInfo, out customFsInfo);

                if (currentFsInfo != null && axcFsInfo != null && customFsInfo != null)
                {
                    string currentFsInfoValue = currentFsInfo.Addr.ToString() + "|" + currentFsInfo.Bf.ToString() + "|" + currentFsInfo.BfOffset.ToString() + "|" + currentFsInfo.Hf.ToString();
                    string axcFsInfoValue = axcFsInfo.Addr.ToString() + "|" + axcFsInfo.Bf.ToString() + "|" + axcFsInfo.BfOffset.ToString() + "|" + axcFsInfo.Hf.ToString();
                    string customFsInfoValue = customFsInfo.Addr.ToString() + "|" + customFsInfo.Bf.ToString() + "|" + customFsInfo.BfOffset.ToString() + "|" + customFsInfo.Hf.ToString();

                    getdata = currentFsInfoValue + "," + axcFsInfoValue + "," + customFsInfoValue;
                }
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Get Carrier Config  error : " + e.Message);
            }
            return getdata;
        }
        public string GetCarrierConfig(RunTimeError error, string cpriport, string flowdirection, string carrierindex)
        {
            string getdata = "";

            try
            {
                CarrierData data = rCarrierConfig.GetCarrierConfig(cpriport, flowDirectionDic[flowdirection], byte.Parse(carrierindex));
                getdata = showCarrierData(data);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Get Carrier Config  error : " + e.Message);
            }
            return getdata;
        }
        public void SetCarrierConfig(RunTimeError error, string cpriport, string flowdirection, string carriernumber, string enabled, string carrierid, string technology, string frequency, string axccontainer, string gain, string fsinfo, string axcContainerGroupLength, string bfPeriod, string syncmode)
        {
            try
            {
                FsInfo fsinfoData = new FsInfo();
                string[] fsinfostr = fsinfo.Split();
                if (fsinfostr.Length >= 4)
                {
                    fsinfoData.Addr = byte.Parse(fsinfostr[0]);
                    fsinfoData.Hf = byte.Parse(fsinfostr[1]);
                    fsinfoData.Bf = byte.Parse(fsinfostr[2]);
                    fsinfoData.BfOffset = byte.Parse(fsinfostr[3]);
                }

                Gain gainData = new Gain();
                string[] gainstr = gain.Split();
                if (fsinfostr.Length >= 3)
                {
                    gainData.GainEnable = boolDic[gainstr[0]];
                    gainData.GainDb = double.Parse(gainstr[1]);
                    gainData.GainFact = double.Parse(gainstr[2]);
                }

                CarrierData data = new CarrierData();
                data.axcContainerGroupLength = byte.Parse(axcContainerGroupLength);
                data.bfPeriod = byte.Parse(bfPeriod);
                data.carrierId = byte.Parse(carrierid);
                data.carrierNumber = byte.Parse(carriernumber);
                data.enabled = boolDic[enabled];
                data.sampleFrequency = freqDic[frequency];
                data.technology = technologyDic[technology];
                data.startAxcContainer = byte.Parse(axccontainer);
                data.syncMode = syncModeDic[syncmode];
                data.fsInfo = fsinfoData;
                data.gain = gainData;


                rCarrierConfig.SetCarrierConfig(cpriport, flowDirectionDic[flowdirection], data);
                //showCarrierData(data);


            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Set Carrier Config  error : " + e.Message);
            }
        }


        public void SetCpriConfig(RunTimeError error, string paraname, string cpriport, string paravalue)
        {
            try
            {
                switch (paraname)
                {
                    case "LineRate":
                        rCpriConfig.SetLineRate(cpriport, lineRateDic[paravalue]);
                        break;
                    case "CPRIVer":
                        rCpriConfig.SetCpriVersion(cpriport, cpriVerDic[paravalue]);
                        break;
                    case "ScrambSeed":
                        rCpriConfig.SetScramblingSeed(cpriport, uint.Parse(paravalue));
                        break;
                    case "LinkState":
                        rCpriConfig.SetLinkState(cpriport, cpriLinkStateDic[paravalue]);
                        break;
                    case "LinkMode":
                        rCpriConfig.SetLinkMode(cpriport, cpriLinkModeDic[paravalue]);
                        break;
                    case "L1Reset":
                        rCpriConfig.PerformL1Reset(cpriport, cpriL1ResetDic[paravalue]);
                        break;
                    default:
                        WriteTraceText(error,"Undefined Rumaster Set CPRI Config parament:  " + paraname);
                        break;
                }
            }
            catch (Exception e)
            {
                WriteTraceText(error,"Rumaster Set CPRI Config: " + paraname + " error : " + e.Message);
            }
        }
        public string[] getHwSn(RunTimeError error)
        {
            try
            {
                return tsl.GetHws();
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Get HwSn  error : " + e.Message);
                return null;
            }

        }
        public void SetupClockTriggerSource(RunTimeError error, string triggerPort, string instanceName)
        {
            try
            {
                rTriggerConfig.SetupClockTriggerSource(triggerPort, clockInstanceNameDic[instanceName]);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Setup Clock Trigger Source error : " + e.Message);
            }
        }
        public void SetCpriTriggerSource(RunTimeError error, string triggerPort, string cpriport, string cpriTriggerSource)
        {
            try
            {
                rTriggerConfig.SetCpriTriggerSource(triggerPort, cpriport, cpriTrigSourceDic[cpriTriggerSource]);
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster Set Cpri Trigger Source error : " + e.Message);
            }
        }
        //================IQ file====================
        public void IQFileClearFile(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.IQFileClearFile(cpriport, fileName);
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
            }

        }
        public void IQFileAdd(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.IQFileAdd(cpriport, fileName);
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
            }
        }
        public void IQFileSetCurrentByName(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.IQFileSetCurrentByName(cpriport, fileName);
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
            }
        }

        public string IQFileGetCurrent(RunTimeError error, string cpriport)
        {
            try
            {
                return rCpriDataFlow.IQFileGetCurrent(cpriport);
            }
            catch (Exception exs)
            {
                WriteTraceText(error,"Rumaster " + cpriport + " IQ File Get Current  error : " + exs.Message);
                return "";
            }

        }

        public string[] IQFilesGetList(RunTimeError error, string cpriport)
        {
            try
            {
                return rCpriDataFlow.IQListFiles(cpriport);
            }
            catch (Exception exs)
            {
                WriteTraceText(error, "Rumaster " + cpriport + " IQ File Get List  error : " + exs.Message);
                string[] empty = new string[] { ""};
                return empty;
            }

        }

        public string SetIQFile(RunTimeError error, string cpriport, string filepath)
        {
            try
            {
                string[] IQFileList = rCpriDataFlow.IQListFiles(cpriport);
                foreach (String filename in IQFileList)
                {
                    rCpriDataFlow.IQFileClearFile(cpriport, filename);
                }

                rCpriDataFlow.IQFileAdd(cpriport, filepath);
                rCpriDataFlow.IQFileSetCurrentByName(cpriport, filepath);
                return "True";
            }
            catch (Exception e)
            {
                WriteTraceText(error, "Rumaster set IQ file  error : " + e.Message);
                return "False";
            }
        }
        //================CPC file====================
        public void CpcFileClearFile(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.CpcFileClearFile(cpriport, fileName);
            }
            catch (Exception exs)
            {
                WriteTraceText(error, "Rumaster CpcFile Clear File error : " + exs.Message);
            }

        }

        public void CpcFilesClearAll(RunTimeError error, string cpriport)
        {
            try
            {
                rCpriDataFlow.CpcFilesClearAll(cpriport);
            }
            catch (Exception exs)
            {
                WriteTraceText(error,exs.Message);
            }

        }

        public void CpcFileAdd(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.CpcFileAdd(cpriport, fileName);
            }
            catch (Exception exs)
            {
                WriteTraceText(error, "Rumaster CpcFile Add error : " + exs.Message);
            }
        }
        public string CpcFileGetCurrent(RunTimeError error, string cpriport)
        {
            try
            {
                return rCpriDataFlow.CpcFileGetCurrent(cpriport);
            }
            catch (Exception exs)
            {
                WriteTraceText(error,"Rumaster " + cpriport + " CPC File Get Current error : " + exs.Message);
                return "";
            }
        }
        public void CpcFileSetCurrent(RunTimeError error, string cpriport, string fileName)
        {
            try
            {
                rCpriDataFlow.CpcFileSetCurrent(cpriport, fileName);
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
            }
        }

        public void CpcFileSetLoopLength(RunTimeError error, string cpriport, string fileName, string loopLength)
        {
            try
            {
                rCpriDataFlow.CpcFileSetLoopLength(cpriport, fileName, int.Parse(loopLength));
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
            }
        }

        public string CpcListFiles(RunTimeError error, string cpriport)
        {
            try
            {
                return string.Join("|", rCpriDataFlow.CpcListFiles(cpriport));
            }
            catch (Exception exs)
            {
                WriteTraceText(error,exs.Message);
                return "";
            }

        }
        public void CpcSetAxcMode(RunTimeError error, string cpriport, string txAxcMode)
        {
            try
            {
                rCpriDataFlow.CpcSetAxcMode(cpriport, txAxcModeDic[txAxcMode]);
            }
            catch (Exception exs)
            {
                WriteTraceText(error,exs.Message);
            }
        }

        public string SetCPCfile(RunTimeError error, string cpriport, string filepath)
        {
            try
            {
                rCpriDataFlow.CpcFilesClearAll(cpriport);
                rCpriDataFlow.CpcFileAdd(cpriport, filepath);
                rCpriDataFlow.CpcSetAxcMode(cpriport, Tiger.Ruma.TxAxcMode.CPC_FILES);
                rCpriDataFlow.CpcFileSetLoopLength(cpriport, filepath, 2);
                rCpriDataFlow.CpcFileSetCurrent(cpriport, filepath);
                return "True";
            }
            catch (Exception e)
            {
                WriteTraceText(error,e.Message);
                return "False";
            }
        }
        #endregion

        #region privateFunction
        private void WriteTraceText(RunTimeError error, string mes)
        {
            error.Errordescription = mes;
            return;
        }
        private FlowDataType[] getFlowsDataType(string flow)
        {
            if (flow == "all")
            {
                FlowDataType[] flows = { FlowDataType.AXC, FlowDataType.IQ };
                return flows;
            }
            else if (flow == "axc")
            {
                FlowDataType[] flows = { FlowDataType.AXC };
                return flows;
            }
            else if (flow == "iq")
            {
                FlowDataType[] flows = { FlowDataType.IQ };
                return flows;
            }
            else
            {
                FlowDataType[] flows = { FlowDataType.AXC, FlowDataType.IQ };
                return flows;
            }
        }
        #endregion

        // data member
        public Tiger.Ruma.IRumaCpriDataFlow rCpriDataFlow;
        public Tiger.Ruma.IRumaCarrierConfig rCarrierConfig;
        public Tiger.Ruma.IRumaCpriConfig rCpriConfig;
        public Tiger.Ruma.IRumaServerBase rServerBase;
        public Tiger.Ruma.IRumaTriggerConfig rTriggerConfig;
        public Tiger.Ruma.IRULoader rRULoader;
        public Tiger.Ruma.IRumaDebugPortConfig rDebugPortConfig;

        #region TypeDict
        public static readonly Dictionary<string, CpriFlowDirection> flowDirectionDic = new Dictionary<string, CpriFlowDirection>
        {
            {"TX", CpriFlowDirection.TX},
            {"RX", CpriFlowDirection.RX}
        };
        public static readonly Dictionary<string, SampleFrequency> freqDic = new Dictionary<string, SampleFrequency>
        {
            {"Frequency_30_72", SampleFrequency.Frequency_30_72},
            {"Frequency_15_36", SampleFrequency.Frequency_15_36},
            {"Frequency_23_04", SampleFrequency.Frequency_23_04},
            {"Frequency_19_20", SampleFrequency.Frequency_19_20},
            {"Frequency_7_68", SampleFrequency.Frequency_7_68},
            {"Frequency_3_84", SampleFrequency.Frequency_3_84},
            {"Frequency_1_92", SampleFrequency.Frequency_1_92},
            {"Frequency_0_96", SampleFrequency.Frequency_0_96}
        };
        public static readonly Dictionary<string, uint> axcDic = new Dictionary<string, uint>
        {
            {"Frequency_30_72", 8},
            {"Frequency_15_36", 4},
            {"Frequency_23_04", 6},
            {"Frequency_19_20", 5},
            {"Frequency_7_68", 2},
            {"Frequency_3_84", 1},
            //{"Frequency_1_92", SampleFrequency.Frequency_1_92},
            //{"Frequency_0_96", SampleFrequency.Frequency_0_96}
        };
        public static readonly Dictionary<string, Technology> technologyDic = new Dictionary<string, Technology>
        {
            {"LTE", Technology.LTE},
            {"GSM", Technology.GSM},
            {"CDMA", Technology.CDMA},
            {"WCDMA_5_BIT", Technology.WCDMA_5_BIT},
            {"WCDMA_7_BIT", Technology.WCDMA_7_BIT}
        };
        public static readonly Dictionary<string, bool> boolDic = new Dictionary<string, bool>
        {
            {"true", true},
            {"false", false}
        };
        public static readonly Dictionary<string, LogExportFormat> logExportFormatDic = new Dictionary<string, LogExportFormat>
        {
            {"csv", LogExportFormat.CSV},
            {"xml", LogExportFormat.XML}
        };

        public static readonly Dictionary<string, SyncMode> syncModeDic = new Dictionary<string, SyncMode>
        {
            {"FSINFO", SyncMode.FSINFO},
            {"CUSTOM", SyncMode.CUSTOM},
            {"RX_TIMING", SyncMode.RX_TIMING}
        };
        public static readonly Dictionary<string, LineRate> lineRateDic = new Dictionary<string, LineRate>
        {
            {"LR1_2", LineRate.LR1_2},
            {"LR2_5", LineRate.LR2_5},
            {"LR4_9", LineRate.LR4_9},
            {"LR9_8", LineRate.LR9_8},
            {"LR10_1", LineRate.LR10_1}
        };
        public static readonly Dictionary<string, CpriVersion> cpriVerDic = new Dictionary<string, CpriVersion>
        {
            {"VERSION_1", CpriVersion.VERSION_1},
            {"VERSION_2", CpriVersion.VERSION_2}
        };

        public static readonly Dictionary<string, LTU> ltuDic = new Dictionary<string, LTU>
        {
            {"INT_REF", LTU.INT_REF},
            {"EXT_REF", LTU.EXT_REF},
            {"APP_REF1", LTU.APP_REF1},
            {"APP_REF2", LTU.APP_REF2},
            {"APP_REF3", LTU.APP_REF3},
            {"APP_REF4", LTU.APP_REF4},
            {"APP_REF5", LTU.APP_REF5},
            {"APP_REF6", LTU.APP_REF6}
        };
        public static readonly Dictionary<string, FlowDataType> flowDataTypeDic = new Dictionary<string, FlowDataType>
        {
            {"NONE", FlowDataType.NONE},
            {"IQ", FlowDataType.IQ},
            {"AXC", FlowDataType.AXC},
            {"ECP", FlowDataType.ECP},
            {"AXC_ECP", FlowDataType.AXC_ECP},
            {"IQ_AXC_ECP", FlowDataType.IQ_AXC_ECP}
        };
        public static readonly Dictionary<string, FlowStartCondition> flowStartConditionDic = new Dictionary<string, FlowStartCondition>
        {
            {"NONE", FlowStartCondition.NONE},
            {"TRIG_IN", FlowStartCondition.TRIG_IN},
            {"CPRI_TIME", FlowStartCondition.CPRI_TIME},
            {"FIRST_NON_IDLE", FlowStartCondition.FIRST_NON_IDLE},
            {"FSINFO", FlowStartCondition.FSINFO},
            {"FRAME_PRE_START", FlowStartCondition.FRAME_PRE_START},
            {"RADIO_FRAME", FlowStartCondition.RADIO_FRAME}
        };
        public static readonly Dictionary<string, FlowStopCondition> flowStopConditionDic = new Dictionary<string, FlowStopCondition>
        {
            {"NEVER", FlowStopCondition.NEVER},
            {"FLOW_STOP_COND_CPRI_TIME", FlowStopCondition.FLOW_STOP_COND_CPRI_TIME},
            {"FLOW_STOP_COND_TRIG_IN", FlowStopCondition.FLOW_STOP_COND_TRIG_IN},
            {"CPRI_TIME_LENGTH", FlowStopCondition.CPRI_TIME_LENGTH}
        };

        public static readonly Dictionary<string, FlowDataMode> flowDataModeDic = new Dictionary<string, FlowDataMode>
        {
            {"Carrier", FlowDataMode.Carrier},
            {"RAW", FlowDataMode.RAW}
        };
        public static readonly Dictionary<string, ClockInstanceName> clockInstanceNameDic = new Dictionary<string, ClockInstanceName>
        {
            {"CLK_122_0", ClockInstanceName.CLK_122_0},
            {"CLK_122_180", ClockInstanceName.CLK_122_180}
        };
        public static readonly Dictionary<string, CpriTrigSource> cpriTrigSourceDic = new Dictionary<string, CpriTrigSource>
        {
            {"CPC", CpriTrigSource.CPC},
            {"CTT", CpriTrigSource.CTT},
            {"DYNAMIC_GAIN", CpriTrigSource.DYNAMIC_GAIN},
            {"FSINFO_CHANGED", CpriTrigSource.FSINFO_CHANGED},
            {"RXK285", CpriTrigSource.RXK285},
            {"TXK285", CpriTrigSource.TXK285}
        };
        public static readonly Dictionary<string, TriggerStaticOutputLevel> triggerStaticOutputLevelDic = new Dictionary<string, TriggerStaticOutputLevel>
        {
            {"high", TriggerStaticOutputLevel.high},
            {"low", TriggerStaticOutputLevel.low}
        };
        public static readonly Dictionary<string, CpriLinkState> cpriLinkStateDic = new Dictionary<string, CpriLinkState>
        {
            {"CPRI_LINK_STATE_ENABLE",CpriLinkState.CPRI_LINK_STATE_ENABLE},
            { "CPRI_LINK_STATE_DISABLE",CpriLinkState.CPRI_LINK_STATE_DISABLE}
        };
        public static readonly Dictionary<string, CpriLinkMode> cpriLinkModeDic = new Dictionary<string, CpriLinkMode>
        {
            {"RX",CpriLinkMode.RX},
            { "TX",CpriLinkMode.TX},
            { "RXTX",CpriLinkMode.RXTX}
        };
        public static readonly Dictionary<string, bool> cpriL1ResetDic = new Dictionary<string, bool>
        {
            {"true",true},
            { "false",false},
        };
        public static readonly Dictionary<string, TxAxcMode> txAxcModeDic = new Dictionary<string, TxAxcMode>
        {
            {"AUTO_AXC",TxAxcMode.AUTO_AXC},
            {"CPC_FILES",TxAxcMode.CPC_FILES},
            {"CPC_MERGE",TxAxcMode.CPC_MERGE},
            {"NO_AXC",TxAxcMode.NO_AXC},
        };
        public static readonly Dictionary<string, ExportFormat> exportFormatDic = new Dictionary<string, ExportFormat>
        {
            {"cul",ExportFormat.Cul},
            {"cru",ExportFormat.Raw},
            {"ccu",ExportFormat.Ccu},
            {"cul2",ExportFormat.Cul2},
            {"cul3",ExportFormat.Cul3},
        };
        public static readonly Dictionary<string, AxcContainerFormat> axcContainerFormatDic = new Dictionary<string, AxcContainerFormat>
        {
            {"AXC_20_BIT",AxcContainerFormat.AXC_20_BIT},
            {"AXC_24_BIT",AxcContainerFormat.AXC_24_BIT},
            {"AXC_30_BIT",AxcContainerFormat.AXC_30_BIT},
        };
        #endregion
        private IRumaControlClient rumaClient;
        private ApplicationControl tas;
        private TslControlClient tsl;
        private static Dictionary<string, ulong> cpriPortMapping = new Dictionary<string, ulong>();
        private readonly FileAppender logRecorder;
        
    }
}
