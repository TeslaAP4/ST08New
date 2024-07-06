using Acura3._0.Classes;
using Acura3._0.FunctionForms;
using AcuraLibrary;
using AcuraLibrary.Forms;
//using CFX.Structures.JAG;
using JabilSDK;
using JabilSDK.Controls;
using JabilSDK.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SystemCommon.Communication;
using static Acura3._0.FunctionForms.LogForm;

namespace Acura3._0.ModuleForms
{
    public partial class ConveyorForm : ModuleBaseForm
    {
        #region data
        public bool MachineAvailable1;  //工位1出料信号
        public bool MachineAvailable2;  //工位2出料信号
        public bool Station1Start;   //工位1开始工作信号
        public bool Station2Start;   //工位2开始工作信号
        //public bool Dryrun1;   //工位1空跑有料
        //public bool Dryrun2;   //工位2空跑有料
        public bool Station1_Discharging;  //工位1出料运行信号
        public bool Station1_Discharging_Finish;
        public bool Station2_Discharging;  //工位2出料运行信号
        public bool Station2_Discharging_Finish;
        public bool UpMachineAvailable_SMEMA;  //按钮模拟要料smema

        public bool DownMachineReady_SMEMA;  //按钮模拟出料smema
        public bool StationMachineIn1;  //工位1入料信号
        public bool B_ScrewStationReadySignal = false;// Screw station ready to request board from buffer
        public bool StationMachineIn2;  //工位2入料信号
        public bool ConveyorBStation1Robot1Start; //流道B工位1机器手开始信号
        public bool ConveyorBStation1Robot1Comp;  //流道B工位1机器手完成信号
        public bool ConveyorBStation2Robot2Start; //流道B工位2机器手开始信号
        public bool ConveyorBStation2Robot2Comp;  //流道B工位2机器手完成信号
                                                  //public bool B_ScrewStation1_Available = false; //Screw station1 available to conveyorB buffer station
                                                  //public bool B_ScrewStation2_Available = false; //Screw station2 available to conveyorB buffer station
                                                  //public bool ConveyorBStart1;
                                                  //public bool ConveyorBStart2;


        public bool B_ConveyorA_Buffer_Ready = false;
        public bool B_ConveyorB_Buffer_Ready = false; //ConvetorB buffer request board signal
        public bool B_ConveyorB_Station1_Ready = false; //ConvetorB station2 request board signal
        public bool B_ConveyorB_Station2_Ready = false; //ConvetorB station3 request board signal

        public bool B_RFID1_ResultNG = false;
        public bool B_RFID2_ResultNG = false;
        public bool B_RFID3_ResultNG = false;
        public bool B_RFID4_ResultNG = false;

        public bool ByPass => MiddleLayer.SystemF.GetSettingValue("PSet", "ByPass");
        public bool Dryrun => MiddleLayer.SystemF.GetSettingValue("PSet", "Dryrun");
        public bool DisableRFID => GetSettingValue("PSet", "DisableRFID");
        public int TimeOut => GetSettingValue("PSet", "TimeOut");

        Fanuc_RobotControl fanuc_RobotControl = new Fanuc_RobotControl();
        JTimer J_AxisAutoTm = new JTimer();
        public JTimer Conveyor1Timeout = new JTimer();
        public JTimer Conveyor2Timeout = new JTimer();
        public JTimer Conveyor3Timeout = new JTimer();
        public JTimer Conveyor4Timeout = new JTimer();
        public JTimer Conveyor5Timeout = new JTimer();
        public JTimer ConveyorA_Buffer_Timeout = new JTimer();
        public JTimer ConveyorB_Buffer_Timeout = new JTimer();
        //int Inindex = 0;
        #endregion

        #region Forms & Init
        public MESForm MesF = new MESForm();
        public ConveyorForm()
        {
            InitializeComponent();
            FlowChartMessage.ResetTimerRaise += FlowChartMessage_ResetTimerRaise;
            SetDoubleBuffer(plProductionSetting);
            RFID1.ReceiveHandler();
            RFID2.ReceiveHandler();
            RFID3.ReceiveHandler();
            RFID4.ReceiveHandler();
        }

        private void FlowChartMessage_ResetTimerRaise(object sender, EventArgs e)
        {
            RunTM.Restart();
        }
        #endregion

        #region Override Method     
        public override void AfterProductionSetting()
        {

        }

        public override void IntoProductionSettingPage()
        {

        }

        public override void AlwaysRun()
        {

        }

        public override void InitialReset()
        {
            flowChart0_11.TaskReset();
        }

        public override void RunReset()
        {
            flowChart72.TaskReset();
            flowChart1.TaskReset();
            flowChart8.TaskReset();
            flowChart18.TaskReset();
            flowChart71.TaskReset();
            flowChart38.TaskReset();
            flowChart29.TaskReset();
        }

        public override void Initial()
        {
            flowChart0_11.TaskRun();
        }

        public override void Run()
        {
            flowChart72.TaskRun();
            flowChart1.TaskRun();
            flowChart8.TaskRun();
            flowChart18.TaskRun();
            flowChart71.TaskRun();
            flowChart38.TaskRun();
            flowChart29.TaskRun();
        }

        public override void StartRun()
        {
            RunTM.Restart();
        }

        public override void StopRun()
        {

        }
        #endregion

        #region RFID
        public enum RFIDResult
        {
            OK,
            NG,
            NA,
            Fail
        }
        public bool WriteToRFID(int processid, bool result, RFID _RFID)
        {
            _RFID.RFID_Clear(GetRecipeValue("RSet", "A_RFIDId"));
            string data = $"{processid}{(result ? "OK" : "NG")}";
            return _RFID.RFID_Write(GetRecipeValue("RSet", "A_RFIDId"), data, "0", data.Length.ToString());
        }

        public RFIDResult ReadRFID(int processid, RFID _RFID)
        {
            string result = _RFID.RFID_ReadDataString(GetRecipeValue("RSet", "A_RFIDId"), "", "0", "100");
            result = result.Replace(" ", "").Replace("\0", "");

            if (result.ToLower().Contains("fail"))
                return RFIDResult.Fail;
            if (result.IndexOf(processid.ToString()) < 0)
                return RFIDResult.NA;
            if (result.Substring(result.IndexOf(processid.ToString()) + 2).Contains("OK"))
            {
                return RFIDResult.OK;
            }
            else
                return RFIDResult.NG;
        }

        /// <summary>
        /// 返回两个指定字符串之间的字符串
        /// </summary>
        /// <param name="sourse"></param>
        /// <param name="startstr"></param>
        /// <param name="endstr"></param>
        /// <returns></returns>
        public string MidStrEx_New(string sourse, string startstr, string endstr)
        {
            Regex rg = new Regex("(?<=(" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            return rg.Match(sourse).Value;
            //Regex rg = new Regex("(?<=" + startstr + "))[.\\s\\S]*?(?=(" + endstr + "))", RegexOptions.Multiline | RegexOptions.Singleline);
            //return rg.Match(sourse).Value;
        }
        #endregion
        private void SetDoubleBuffer(Control cont)
        {
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, cont, new object[] { true });
        }

        private bool Delay(JTimer timer, int TimeOut)
        {
            bool ret = false;
            if (timer.IsOn(TimeOut))
            {
                ret = true;
            }
            return ret;
        }

        public ProductData Station1Data;
        public ProductData Station2Data;
        public ProductData Station3Data;
        public ProductData Station4Data;

        List<ProductData> ProductDataList1 = new List<ProductData>();
        List<ProductData> ProductDataList2 = new List<ProductData>();

        public struct ProductData
        {
            public string UID;
            public string GUID;
            public DateTime StartTime;
            public DateTime EndTime;
            public DateTime Station1StartTime;
            public DateTime Station1EndTime;
            public DateTime Station2StartTime;
            public DateTime Station2EndTime;
            public DateTime Station3StartTime;
            public DateTime Station3EndTime;
            public DateTime Station4StartTime;
            public DateTime Station4EndTime;
        }

        public RFID RFID1 = new RFID();
        public RFID RFID2 = new RFID();
        public RFID RFID3 = new RFID();
        public RFID RFID4 = new RFID();

        List<RFID_Struct> RFID_DataList1 = new List<RFID_Struct>();
        List<RFID_Struct> RFID_DataList2 = new List<RFID_Struct>();

        public struct RFID_Struct
        {
            public RFID _RFID;
            public bool B_Result;
            public string S_RFID;

            public RFID_Struct(RFID rFID, bool result, string rFIDData)
            {
                _RFID = rFID;
                B_Result = result;
                S_RFID = rFIDData;
            }
        }

        public RFID_Struct myRFID1 = new RFID_Struct();
        public RFID_Struct myRFID2 = new RFID_Struct();
        public RFID_Struct myRFID3 = new RFID_Struct();
        public RFID_Struct myRFID4 = new RFID_Struct();


        private void B_RFIDAConnect_Click(object sender, EventArgs e)
        {
            if (RFID1.connect)
            {
                TextDataShow("Device connected", R_RFIDADataShow, true);
                return;
            }
            if (RFID1.RFID_Connect(GetRecipeValue("RSet", "A_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "A_RFIDPort")), GetRecipeValue("RSet", "A_RFIDId")))
            {
                B_RFIDAConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDADataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDADataShow, false);
        }


        private void B_RFIDBConnect_Click(object sender, EventArgs e)
        {
            if (RFID2.connect)
            {
                TextDataShow("Device connected", R_RFIDBDataShow, true);
                return;
            }
            if (RFID2.RFID_Connect(GetRecipeValue("RSet", "B_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "B_RFIDPort")), GetRecipeValue("RSet", "B_RFIDId")))
            {
                B_RFIDBConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDBDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDBDataShow, false);
        }


        private void B_RFIDCConnect_Click(object sender, EventArgs e)
        {
            if (RFID3.connect)
            {
                TextDataShow("Device connected", R_RFIDCDataShow, true);
                return;
            }
            if (RFID3.RFID_Connect(GetRecipeValue("RSet", "C_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "C_RFIDPort")), GetRecipeValue("RSet", "C_RFIDId")))
            {
                B_RFIDCConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDCDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDCDataShow, false);
        }


        private void B_RFIDDConnect_Click(object sender, EventArgs e)
        {
            if (RFID4.connect)
            {
                TextDataShow("Device connected", R_RFIDDDataShow, true);
                return;
            }
            if (RFID4.RFID_Connect(GetRecipeValue("RSet", "DRFIDIp"), ushort.Parse(GetRecipeValue("RSet", "D_RFIDPort")), GetRecipeValue("RSet", "D_RFIDId")))
            {
                B_RFIDDConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDDDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDDDataShow, false);
        }


        private void B_RFIDADisconnect_Click(object sender, EventArgs e)
        {
            RFID1.RFID_DisConnect();
            B_RFIDAConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDADataShow, true);
        }


        private void B_RFIDBDisconnect_Click(object sender, EventArgs e)
        {
            RFID2.RFID_DisConnect();
            B_RFIDBConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDBDataShow, true);
        }


        private void B_RFIDCDisconnect_Click(object sender, EventArgs e)
        {
            RFID3.RFID_DisConnect();
            B_RFIDCConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDCDataShow, true);
        }


        private void B_RFIDDDisconnect_Click(object sender, EventArgs e)
        {
            RFID4.RFID_DisConnect();
            B_RFIDDConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDDDataShow, true);
        }


        private void B_RFIDAReadUID_Click(object sender, EventArgs e)
        {
            if (!RFID1.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + RFID1.RFID_ReadUID(GetRecipeValue("RSet", "A_RFIDId")), R_RFIDADataShow, true);
        }


        private void B_RFIDBReadUID_Click(object sender, EventArgs e)
        {
            if (!RFID2.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + RFID2.RFID_ReadUID(GetRecipeValue("RSet", "B_RFIDId")), R_RFIDBDataShow, true);
        }


        private void B_RFIDCReadUID_Click(object sender, EventArgs e)
        {
            if (!RFID3.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + RFID3.RFID_ReadUID(GetRecipeValue("RSet", "C_RFIDId")), R_RFIDCDataShow, true);
        }


        private void B_RFIDDReadUID_Click(object sender, EventArgs e)
        {
            if (!RFID4.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + RFID4.RFID_ReadUID(GetRecipeValue("RSet", "D_RFIDId")), R_RFIDDDataShow, true);
        }


        private void B_RFIDAReadData_Click(object sender, EventArgs e)
        {
            if (!RFID1.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            string pos = T_RFIDAOAdderss.Text == "" ? "0" : T_RFIDAOAdderss.Text;
            string len = T_RFIDAEndAddress.Text == "" ? "16" : T_RFIDAEndAddress.Text;
            TextDataShow("Read data successfully: " + RFID1.RFID_ReadDataTostring(GetRecipeValue("RSet", "A_RFIDId"), pos, len), R_RFIDADataShow, true);
        }


        private void B_RFIDBReadData_Click(object sender, EventArgs e)
        {
            if (!RFID2.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            string pos = T_RFIDBOAdderss.Text == "" ? "0" : T_RFIDBOAdderss.Text;
            string len = T_RFIDBEndAddress.Text == "" ? "16" : T_RFIDBEndAddress.Text;
            TextDataShow("Read data successfully: " + RFID2.RFID_ReadDataTostring(GetRecipeValue("RSet", "B_RFIDId"), pos, len), R_RFIDBDataShow, true);
        }


        private void B_RFIDCReadData_Click(object sender, EventArgs e)
        {
            if (!RFID3.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            string pos = T_RFIDCOAdderss.Text == "" ? "0" : T_RFIDCOAdderss.Text;
            string len = T_RFIDCEndAddress.Text == "" ? "16" : T_RFIDCEndAddress.Text;
            TextDataShow("Read data successfully: " + RFID3.RFID_ReadDataTostring(GetRecipeValue("RSet", "C_RFIDId"), pos, len), R_RFIDCDataShow, true);
        }


        private void B_RFIDDReadData_Click(object sender, EventArgs e)
        {
            if (!RFID4.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            string pos = T_RFIDDOAdderss.Text == "" ? "0" : T_RFIDDOAdderss.Text;
            string len = T_RFIDDEndAddress.Text == "" ? "16" : T_RFIDDEndAddress.Text;
            TextDataShow("Read data successfully: " + RFID4.RFID_ReadDataTostring(GetRecipeValue("RSet", "D_RFIDId"), pos, len), R_RFIDDDataShow, true);
        }


        private void B_RFIDAWriteData_Click(object sender, EventArgs e)
        {
            if (!RFID1.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            if (T_RFIDAWriteData.Text == "")
            {
                TextDataShow("Parameter error", R_RFIDADataShow, false);
                return;
            }
            string pos = T_RFIDAOAdderss.Text == "" ? "0" : T_RFIDAOAdderss.Text;
            string len = T_RFIDAEndAddress.Text == "" ? "16" : T_RFIDAEndAddress.Text;
            TextDataShow("Write data successfully: " + RFID1.RFID_Write(GetRecipeValue("RSet", "A_RFIDId"), T_RFIDAWriteData.Text, pos, len), R_RFIDADataShow, true);
        }


        private void B_RFIDBWriteData_Click(object sender, EventArgs e)
        {
            if (!RFID2.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            if (T_RFIDBWriteData.Text == "")
            {
                TextDataShow("Parameter error", R_RFIDBDataShow, false);
                return;
            }
            string pos = T_RFIDBOAdderss.Text == "" ? "0" : T_RFIDBOAdderss.Text;
            string len = T_RFIDBEndAddress.Text == "" ? "16" : T_RFIDBEndAddress.Text;
            TextDataShow("Write data successfully: " + RFID2.RFID_Write(GetRecipeValue("RSet", "B_RFIDId"), T_RFIDBWriteData.Text, pos, len), R_RFIDBDataShow, true);
        }


        private void B_RFIDCWriteData_Click(object sender, EventArgs e)
        {
            if (!RFID3.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            if (T_RFIDCWriteData.Text == "")
            {
                TextDataShow("Parameter error", R_RFIDCDataShow, false);
                return;
            }
            string pos = T_RFIDCOAdderss.Text == "" ? "0" : T_RFIDCOAdderss.Text;
            string len = T_RFIDCEndAddress.Text == "" ? "16" : T_RFIDCEndAddress.Text;
            TextDataShow("Write data successfully: " + RFID3.RFID_Write(GetRecipeValue("RSet", "C_RFIDId"), T_RFIDCWriteData.Text, pos, len), R_RFIDCDataShow, true);
        }


        private void B_RFIDDWriteData_Click(object sender, EventArgs e)
        {
            if (!RFID4.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            if (T_RFIDDWriteData.Text == "")
            {
                TextDataShow("Parameter error", R_RFIDDDataShow, false);
                return;
            }
            string pos = T_RFIDDOAdderss.Text == "" ? "0" : T_RFIDDOAdderss.Text;
            string len = T_RFIDDEndAddress.Text == "" ? "16" : T_RFIDDEndAddress.Text;
            TextDataShow("Write data successfully: " + RFID4.RFID_Write(GetRecipeValue("RSet", "D_RFIDId"), T_RFIDDWriteData.Text, pos, len), R_RFIDDDataShow, true);
        }


        private void B_RFIDAClearData_Click(object sender, EventArgs e)
        {
            if (!RFID1.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + RFID1.RFID_Clear(GetRecipeValue("RSet", "A_RFIDId")), R_RFIDADataShow, true);
        }


        private void B_RFIDBClearData_Click(object sender, EventArgs e)
        {
            if (!RFID2.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + RFID2.RFID_Clear(GetRecipeValue("RSet", "B_RFIDId")), R_RFIDBDataShow, true);
        }


        private void B_RFIDCClearData_Click(object sender, EventArgs e)
        {
            if (!RFID3.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + RFID3.RFID_Clear(GetRecipeValue("RSet", "C_RFIDId")), R_RFIDCDataShow, true);
        }


        private void B_RFIDDClearData_Click(object sender, EventArgs e)
        {
            if (!RFID4.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + RFID4.RFID_Clear(GetRecipeValue("RSet", "D_RFIDId")), R_RFIDDDataShow, true);
        }


        public static void RefreshDifferentThreadUI(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                Action refreshUI = new Action(action);
                control.Invoke(refreshUI);
            }
            else
            {
                action.Invoke();
            }
        }


        public void TextDataShow(string Textshow, RichTextBox richText, bool OK)
        {
            RefreshDifferentThreadUI(richText, () =>
            {
                if (richText.TextLength > 10000)
                {
                    richText.Clear();
                }
                if (OK == true)
                    richText.SelectionColor = Color.Green;
                else
                    richText.SelectionColor = Color.Red;
                richText.AppendText(Textshow + Environment.NewLine);
                richText.SelectionStart = richText.TextLength; richText.ScrollToCaret();
            });
        }


        public bool C_DelayMs(int delayMilliseconds)
        {
            DateTime now = DateTime.Now;
            Double s;
            do
            {
                TimeSpan spand = DateTime.Now - now;
                s = spand.TotalMilliseconds + spand.Seconds * 1000;
                Application.DoEvents();
            }
            while (s < delayMilliseconds);
            return true;
        }

        public AlarmClass.AlarmDataClass AlarmContent = new AlarmClass.AlarmDataClass();

        public void ConveyorAlarm(string AlarmCode, FlowChartMessage flow, bool btnSkipEnable = true)
        {
            try
            {
                JSDK.Alarm.Show(AlarmCode);
                if (flow != null && JSDK.Alarm.IsExistInSummary(AlarmCode, ref AlarmContent))
                {
                    flow.msgForm.btnSkip.Enabled = btnSkipEnable;
                    flow.Title = "Conveyor Module";
                    flow.Content = AlarmCode + "-" + AlarmContent.Content;
                }
            }
            catch (Exception ex)
            {
            }
        }

        #region UI Update
        private void CvyTim_UpdateUI_Tick(object sender, EventArgs e)
        {

        }
        #endregion


        private FCResultType flowChart0_11_FlowRun(object sender, EventArgs e)
        {
            MachineAvailable1 = false;
            MachineAvailable2 = false;
            Station1Start = false;
            Station2Start = false;

            B_ScrewStationReadySignal = false;
            Station1_Discharging = false;
            Station1_Discharging_Finish = false;
            Station2_Discharging = false;
            Station2_Discharging_Finish = false;
            UpMachineAvailable_SMEMA = false;
            DownMachineReady_SMEMA = false;
            StationMachineIn1 = false;
            StationMachineIn2 = false;
            ConveyorBStation1Robot1Start = false;
            ConveyorBStation1Robot1Comp = false;
            ConveyorBStation2Robot2Start = false;
            ConveyorBStation2Robot2Comp = false;
            B_ConveyorA_Buffer_Ready = false;
            B_ConveyorB_Buffer_Ready = false;
            B_ConveyorB_Station1_Ready = false;
            B_ConveyorB_Station2_Ready = false;
            B_RFID1_ResultNG = false;
            B_RFID2_ResultNG = false;
            B_RFID3_ResultNG = false;
            B_RFID4_ResultNG = false;
            bInitialOk = true;
            RFID_DataList1.Clear();
            RFID_DataList2.Clear();
            ProductDataList1.Clear();
            ProductDataList2.Clear();
            Conveyor1Timeout.Restart();
            Conveyor2Timeout.Restart();
            Conveyor3Timeout.Restart();
            Conveyor4Timeout.Restart();
            ConveyorA_Buffer_Timeout.Restart();
            ConveyorB_Buffer_Timeout.Restart();
            return FCResultType.IDLE;
        }


        private FCResultType flowChart55_FlowRun(object sender, EventArgs e)
        {
            if (DisableRFID || RFID1.RFID_Connect(GetRecipeValue("RSet", "A_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "A_RFIDPort")), GetRecipeValue("RSet", "A_RFIDId")))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3080");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart56_FlowRun(object sender, EventArgs e)
        {
            if (DisableRFID || RFID2.RFID_Connect(GetRecipeValue("RSet", "B_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "B_RFIDPort")), GetRecipeValue("RSet", "B_RFIDId")))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3081");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart57_FlowRun(object sender, EventArgs e)
        {
            if (DisableRFID || RFID3.RFID_Connect(GetRecipeValue("RSet", "C_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "C_RFIDPort")), GetRecipeValue("RSet", "C_RFIDId")))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3082");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart58_FlowRun(object sender, EventArgs e)
        {
            if (DisableRFID || RFID4.RFID_Connect(GetRecipeValue("RSet", "D_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "D_RFIDPort")), GetRecipeValue("RSet", "D_RFIDId")))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3083");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart66_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station1_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3087");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart68_FlowRun(object sender, EventArgs e)
        {
            bInitialOk = true;
            return FCResultType.IDLE;
        }

        private FCResultType flowChart67_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.On();
            if (IB_Conveyor1_Station1_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3096");
            }
            return FCResultType.IDLE;
        }

        #region autoflow
        private FCResultType flowChart1_FlowRun_2(object sender, EventArgs e)
        {
            OB_Conveyor1_MotorForward.On();
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_FlowRun_1(object sender, EventArgs e)
        {
            B_ScrewStationReadySignal = true;
            if (B_ConveyorA_Buffer_Ready)
            {
                if (IB_SF1_BoardStop.IsOff() && IB_Conveyor1_Staiton2_BoardStop.IsOff() && !StationMachineIn1 && !Station1Start && !Station1_Discharging)
                {
                    StationMachineIn1 = true;
                    Conveyor1Timeout.Restart();
                    return FCResultType.NEXT;
                }
                if (IB_SF2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn2 && !Station2Start && !Station2_Discharging)
                {
                    StationMachineIn2 = true;
                    OB_Conveyor1_Station2_StopCylinder.On();
                    OB_Conveyor1_Station3_StopCylinder.Off();
                    Conveyor1Timeout.Restart();
                    return FCResultType.CASE1;
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart3_FlowRun(object sender, EventArgs e)
        {
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart60_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                Conveyor1Timeout.Restart();
                return FCResultType.CASE2;
            }
            if (Dryrun || DisableRFID)
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            Station1Data.UID = RFID1.RFID_ReadUID(GetRecipeValue("RSet", "A_RFIDId"));
            Station1Data.GUID = SysPara.CFX.WorkStarted(Station1Data.UID, 1, Station1Data.StartTime);
            Conveyor1Timeout.Restart();
            if (ReadRFID(31, RFID1) == RFIDResult.OK)
            {
                return FCResultType.NEXT;
            }
            B_RFID1_ResultNG = true;
            return FCResultType.CASE2;
        }

        private FCResultType flowChart39_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station1_Jacking.On())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3084");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_FlowRun_1(object sender, EventArgs e)
        {
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart4_FlowRun(object sender, EventArgs e)
        {
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart59_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                Conveyor1Timeout.Restart();
                return FCResultType.CASE2;
            }
            if (Dryrun || DisableRFID)
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            Station2Data.UID = RFID2.RFID_ReadUID(GetRecipeValue("RSet", "B_RFIDId"));
            Station2Data.GUID = SysPara.CFX.WorkStarted(Station2Data.UID, 1, Station2Data.StartTime);
            Conveyor1Timeout.Restart();
            if (ReadRFID(31, RFID2) == RFIDResult.OK)
            {
                return FCResultType.NEXT;
            }
            B_RFID2_ResultNG = true;
            return FCResultType.CASE2;
        }

        private FCResultType flowChart42_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station2_Jacking.On())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3085");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart40_FlowRun(object sender, EventArgs e)
        {
            OB_Station1_MotorForward.On();
            //OB_SF1_MotorForward.On();   // 7.5 空跑
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }


        private FCResultType flowChart41_FlowRun(object sender, EventArgs e)
        {
            if (IB_SF1_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.SystemF.ErrorDataLogShow(Text, flowChart41.Text);
                ConveyorAlarm("3105", flowChartMessage3, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart5_FlowRun(object sender, EventArgs e)
        {
            Station1Start = true;
            StationMachineIn1 = false;
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart43_FlowRun(object sender, EventArgs e)
        {
            OB_Station2_MotorForward.On();
            OB_SF2_MotorForward.On();
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart44_FlowRun(object sender, EventArgs e)
        {
            if (IB_SF2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.SystemF.ErrorDataLogShow(Text, flowChart44.Text);
                ConveyorAlarm("3106", flowChartMessage4, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart6_FlowRun_1(object sender, EventArgs e)
        {
            if (ByPass || B_RFID2_ResultNG)
            {
                return FCResultType.NEXT;
            }
            Station2Start = true;
            StationMachineIn2 = false;
            Conveyor1Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart18_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                myRFID2 = new RFID_Struct(RFID2, true, "OK");
            }
            Conveyor3Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart9_FlowRun_1(object sender, EventArgs e)
        {
            if (MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOn())
            {
                JSDK.Alarm.Show("A5117", "Please check if there is product on the conveyorA");
            }

            if ((MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn1 && !Station2_Discharging_Finish)
                || (Dryrun && MachineAvailable1) || ByPass)
            {
                Station1_Discharging = true;
                Station1_Discharging_Finish = true;
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }

            return FCResultType.IDLE;
        }

        private FCResultType flowChart45_FlowRun(object sender, EventArgs e)
        {
            OB_Station1_MotorReverse.On();
            //OB_SF1_MotorReverse.On();  // 7.5空跑
            Conveyor2Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart46_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station1_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3087");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart47_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.Off();
            if (IB_Conveyor1_Station2_StopCylinderUp.IsOn())
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(TimeOut))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("3091");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart48_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.Off();
            if (IB_Conveyor1_Station3_StopCylinderUp.IsOn())
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(TimeOut))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("3092");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart15_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station1_BoardStop.IsOn())
            {
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart16_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart17_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.Off();
            if (IB_Conveyor1_Station2_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3091");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart19_FlowRun(object sender, EventArgs e)
        {
            if (ByPass || B_RFID2_ResultNG)
            {
                Conveyor2Timeout.Restart();
                return FCResultType.CASE2;
            }
            if ((MachineAvailable2 && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !Station1_Discharging_Finish && !StationMachineIn2)
                /*|| (Dryrun && MachineAvailable2)*/)
            {
                Station2_Discharging = true;
                Station2_Discharging_Finish = true;
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart20_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station2_Jacking.On())
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor3Timeout.IsOn(TimeOut))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("3085");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart49_FlowRun(object sender, EventArgs e)
        {
            OB_Station2_MotorReverse.On();
            //OB_SF2_MotorReverse.On();  // 7.5空跑
            Conveyor3Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart21_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_Staiton3_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart50_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station2_Jacking.Off())
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor3Timeout.IsOn(TimeOut))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("3088");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart22_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            if (IB_Conveyor1_Station3_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor3Timeout.IsOn(TimeOut))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("3098");
            }
            return FCResultType.IDLE;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpMachineAvailable_SMEMA = true;
        }

        private FCResultType flowChart7_FlowRun(object sender, EventArgs e)
        {
            Conveyor2Timeout.Restart();
            Station1_Discharging_Finish = false;
            MachineAvailable1 = false;
            RFID_DataList1.Add(myRFID1);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart10_FlowRun(object sender, EventArgs e)
        {
            Conveyor3Timeout.Restart();
            Station2_Discharging = false;
            Station2_Discharging_Finish = false;
            MachineAvailable2 = false;
            RFID_DataList1.Add(myRFID2);
            return FCResultType.NEXT;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownMachineReady_SMEMA = true;
        }

        private FCResultType flowChart72_FlowRun(object sender, EventArgs e)
        {
            ConveyorA_Buffer_Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart73_FlowRun(object sender, EventArgs e)
        {
            ConveyorA_Buffer_Timeout.Restart();
            if (IB_Conveyor1_Staiton1_BoardStop.IsOn() || Dryrun)
            {
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart74_FlowRun(object sender, EventArgs e)
        {
            OB_LocalMachineReady_SMEMA.On();
            ConveyorA_Buffer_Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart75_FlowRun(object sender, EventArgs e)
        {
            if (IB_UpMachineAvailable_SMEMA.IsOn() || UpMachineAvailable_SMEMA)
            {
                Conveyor1Timeout.Restart();
                UpMachineAvailable_SMEMA = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart76_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_BoardIn.IsOn())
            {
                Station1Data.StartTime = DateTime.Now;
                Station2Data.StartTime = DateTime.Now;
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorA_Buffer_Timeout.IsOn(50000))
            {
                ConveyorA_Buffer_Timeout.Restart();
                MiddleLayer.SystemF.ErrorDataLogShow(Text, flowChart76.Text);
                ConveyorAlarm("3103", flowChartMessage1, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart77_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_Staiton1_BoardStop.IsOn())
            {
                C_DelayMs(500);
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorA_Buffer_Timeout.IsOn(50000))
            {
                ConveyorA_Buffer_Timeout.Restart();
                MiddleLayer.SystemF.ErrorDataLogShow(Text, flowChart77.Text);
                ConveyorAlarm("3104", flowChartMessage2, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart78_FlowRun(object sender, EventArgs e)
        {
            OB_LocalMachineReady_SMEMA.Off();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart79_FlowRun(object sender, EventArgs e)
        {
            B_ConveyorA_Buffer_Ready = true;
            ConveyorA_Buffer_Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart85_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart80_FlowRun(object sender, EventArgs e)
        {
            if (B_ScrewStationReadySignal)
            {
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart81_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.On();
            if (IB_Conveyor1_Station1_StopCylinderDown.IsOn())
            {
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorA_Buffer_Timeout.IsOn(TimeOut))
            {
                ConveyorA_Buffer_Timeout.Restart();
                JSDK.Alarm.Show("3096");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart82_FlowRun(object sender, EventArgs e)
        {
            if (!B_ScrewStationReadySignal)
            {
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart83_FlowRun(object sender, EventArgs e)
        {
            B_ConveyorA_Buffer_Ready = false;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart84_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.Off();
            if (IB_Conveyor1_Station1_StopCylinderUp.IsOn())
            {
                ConveyorA_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorA_Buffer_Timeout.IsOn(TimeOut))
            {
                ConveyorA_Buffer_Timeout.Restart();
                JSDK.Alarm.Show("3090");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart8_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                myRFID1 = new RFID_Struct(RFID1, true, "OK");
            }
            Conveyor2Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart9_FlowRun(object sender, EventArgs e)
        {
            if (ByPass || B_RFID1_ResultNG)
            {
                Conveyor2Timeout.Restart();
                return FCResultType.CASE2;
            }

            if ((MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn1 && !Station2_Discharging_Finish && !StationMachineIn2)
                /*|| (Dryrun && MachineAvailable1)*/)
            {
                Station1_Discharging = true;
                Station1_Discharging_Finish = true;
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }

            return FCResultType.IDLE;
        }

        private FCResultType flowChart11_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station1_Jacking.On())
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(TimeOut))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("3084");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart12_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_Staiton2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart13_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.On();
            if (IB_Conveyor1_Station2_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(TimeOut))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("3097");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart14_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            if (IB_Conveyor1_Station3_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(TimeOut))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("3098");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart23_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.Off();
            if (IB_Conveyor1_Station3_StopCylinderUp.IsOn())
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor3Timeout.IsOn(TimeOut))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("3092");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart71_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_MotorForward.On();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart86_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station1_BoardStop.IsOn() || (Dryrun && C_DelayMs(1500)))
            {
                B_ConveyorB_Buffer_Ready = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart88_FlowRun(object sender, EventArgs e)
        {
            if (B_ConveyorB_Station1_Ready)
            {
                ConveyorB_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart92_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart89_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station1_StopCylinder.On();
            if (IB_Conveyor2_Station1_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                ConveyorB_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorB_Buffer_Timeout.IsOn(TimeOut))
            {
                ConveyorB_Buffer_Timeout.Restart();
                JSDK.Alarm.Show("3099");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart90_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station1_StopCylinder.Off();
            if (IB_Conveyor2_Station1_StopCylinderUp.IsOn())
            {
                ConveyorB_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ConveyorB_Buffer_Timeout.IsOn(TimeOut))
            {
                ConveyorB_Buffer_Timeout.Restart();
                JSDK.Alarm.Show("3093");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart93_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart91_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart38_FlowRun(object sender, EventArgs e)
        {
            Conveyor4Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart87_FlowRun(object sender, EventArgs e)
        {
            Conveyor4Timeout.Restart();
            if (IB_Conveyor2_Station2_BoardStop.IsOn())
            {
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart94_FlowRun(object sender, EventArgs e)
        {
            Conveyor4Timeout.Restart();
            B_ConveyorB_Station1_Ready = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart95_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1500)))
            {
                Conveyor4Timeout.Restart();
                B_ConveyorB_Station1_Ready = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart96_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ByPass)
            {
                return FCResultType.CASE2;
            }
            if (DisableRFID)
            {
                if (RFID_DataList1[0].B_Result)
                {
                    myRFID3 = new RFID_Struct(RFID3, true, "OK");
                    RFID_DataList1.RemoveAt(0);
                    Conveyor4Timeout.Restart();
                    return FCResultType.NEXT;
                }
                else
                {
                    myRFID3 = new RFID_Struct(RFID3, false, "NG");
                    RFID_DataList1.RemoveAt(0);
                    Conveyor4Timeout.Restart();
                    return FCResultType.CASE2;
                }
            }
            if (ReadRFID(32, RFID3) == RFIDResult.NA && ReadRFID(33, RFID3) == RFIDResult.NA)
            {
                RFID_DataList1.RemoveAt(0);
                Conveyor4Timeout.Restart();
                return FCResultType.CASE2;
            }
            else if (ReadRFID(32, RFID3) == RFIDResult.NG || ReadRFID(33, RFID3) == RFIDResult.NG)
            {
                RFID_DataList1.RemoveAt(0);
                Conveyor4Timeout.Restart();
                return FCResultType.CASE2;
            }
            else if (ReadRFID(32, RFID3) == RFIDResult.OK || ReadRFID(33, RFID3) == RFIDResult.OK)
            {
                RFID_DataList1.RemoveAt(0);
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart97_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor2_Station1_Jacking.On())
            {
                Conveyor4Timeout.Restart();
                C_DelayMs(500);
                return FCResultType.NEXT;
            }
            if (Conveyor4Timeout.IsOn(TimeOut))
            {
                Conveyor4Timeout.Restart();
                JSDK.Alarm.Show("3086");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart98_FlowRun(object sender, EventArgs e)
        {
            ConveyorBStation1Robot1Start = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart99_FlowRun(object sender, EventArgs e)
        {
            if (ConveyorBStation1Robot1Comp)
            {
                ConveyorBStation1Robot1Comp = false;
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart100_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor2_Station1_Jacking.Off())
            {
                Conveyor4Timeout.Restart();
                C_DelayMs(500);
                return FCResultType.NEXT;
            }
            if (Conveyor4Timeout.IsOn(TimeOut))
            {
                Conveyor4Timeout.Restart();
                JSDK.Alarm.Show("3089");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart101_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || DisableRFID)
            {
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (myRFID3.B_Result)
            {
                WriteToRFID(34, true, RFID3);
            }
            else
                WriteToRFID(34, false, RFID3);
            Conveyor4Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart102_FlowRun(object sender, EventArgs e)
        {
            if (B_ConveyorB_Station2_Ready)
            {
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart25_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station2_StopCylinder.On();
            if (IB_Conveyor2_Station2_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor4Timeout.IsOn(TimeOut))
            {
                Conveyor4Timeout.Restart();
                JSDK.Alarm.Show("3100");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart26_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station2_StopCylinder.Off();
            if (IB_Conveyor2_Station2_StopCylinderUp.IsOn())
            {
                RFID_DataList2.Add(myRFID3);
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor4Timeout.IsOn(TimeOut))
            {
                Conveyor4Timeout.Restart();
                JSDK.Alarm.Show("3094");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart27_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart61_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart62_FlowRun(object sender, EventArgs e)
        {
            myRFID3.B_Result = false;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart63_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart29_FlowRun(object sender, EventArgs e)
        {
            Conveyor5Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart30_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station3_BoardStop.IsOn())
            {
                Conveyor5Timeout.Restart();
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart51_FlowRun(object sender, EventArgs e)
        {
            Conveyor5Timeout.Restart();
            B_ConveyorB_Station2_Ready = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart52_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station3_BoardStop.IsOn() || (Dryrun && C_DelayMs(1500)))
            {
                B_ConveyorB_Station2_Ready = false;
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart53_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (ByPass)
            {
                return FCResultType.CASE1;
            }
            if (DisableRFID)
            {
                if (RFID_DataList2[0].B_Result)
                {
                    Conveyor5Timeout.Restart();
                    myRFID4 = new RFID_Struct(RFID4, true, "OK");
                    RFID_DataList2.RemoveAt(0);
                    return FCResultType.NEXT;
                }
                else
                {
                    Conveyor5Timeout.Restart();
                    myRFID4 = new RFID_Struct(RFID4, false, "NG");
                    RFID_DataList2.RemoveAt(0);
                    return FCResultType.CASE3;
                }
            }

            if (ReadRFID(34, RFID4) == RFIDResult.NA)
            {
                RFID_DataList2.RemoveAt(0);
                Conveyor5Timeout.Restart();
                return FCResultType.CASE3;
            }
            else if (ReadRFID(34, RFID4) == RFIDResult.NG)
            {
                RFID_DataList2.RemoveAt(0);
                Conveyor5Timeout.Restart();
                return FCResultType.CASE3;
            }
            else if (ReadRFID(34, RFID4) == RFIDResult.OK)
            {
                RFID_DataList2.RemoveAt(0);
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            else
            {
                ConveyorAlarm("5173", flowChartMessage5);
                Conveyor5Timeout.Restart();
                return FCResultType.CASE2;
            }
        }

        private FCResultType flowChart31_FlowRun(object sender, EventArgs e)
        {
            ConveyorBStation2Robot2Start = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart32_FlowRun(object sender, EventArgs e)
        {
            if (ConveyorBStation2Robot2Comp)
            {
                ConveyorBStation2Robot2Comp = false;
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart34_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || DisableRFID)
            {
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (myRFID4.B_Result)
            {
                if (!WriteToRFID(35, true, RFID4))
                {
                    ConveyorAlarm("5177", flowChartMessage6);
                    Conveyor5Timeout.Restart();
                    return FCResultType.CASE2;
                }
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            else
            {
                if (!WriteToRFID(16, false, RFID4))
                {
                    ConveyorAlarm("5177", flowChartMessage6);
                    Conveyor5Timeout.Restart();
                    return FCResultType.CASE2;
                }
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart35_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                return FCResultType.NEXT;
            }
            OB_LocalMachineAvailable_SMEMA.On();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart69_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                return FCResultType.NEXT;
            }
            if (IB_DownMachineReady_SMEMA.IsOn() || DownMachineReady_SMEMA)
            {
                Conveyor5Timeout.Restart();
                DownMachineReady_SMEMA = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart36_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station3_StopCylinder.On();
            if (IB_Conveyor2_Station3_StopCylinderDown.IsOn())
            {
                C_DelayMs(500);
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor5Timeout.IsOn(TimeOut))
            {
                Conveyor5Timeout.Restart();
                JSDK.Alarm.Show("3101");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart37_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station3_StopCylinder.Off();
            if (IB_Conveyor2_Station3_StopCylinderUp.IsOn())
            {
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor5Timeout.IsOn(TimeOut))
            {
                Conveyor5Timeout.Restart();
                JSDK.Alarm.Show("3095");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart28_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Boardout.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart70_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Boardout.IsOff() || (Dryrun && C_DelayMs(1000)))
            {
                //if (DataList2.Count > 0)
                //    DataList2.RemoveAt(0);
                OB_LocalMachineAvailable_SMEMA.Off();
                OB_LocalMachineWorkNG_SMEMA.Off();
                Conveyor5Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart103_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
            //if (IB_DownMachineReady_SMEMA.IsOff())
            //{
            //    Conveyor5Timeout.Restart();
            //    return FCResultType.NEXT;
            //}
            //return FCResultType.IDLE;
        }

        private FCResultType flowChart104_FlowRun(object sender, EventArgs e)
        {
            //OB_LocalMachineAvailable_SMEMA.Off();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart105_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart65_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart33_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station1_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3087");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart54_FlowRun(object sender, EventArgs e)
        {
            OB_Station1_MotorForward.Off();
            OB_SF1_MotorForward.Off();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart106_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station2_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3088");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart107_FlowRun(object sender, EventArgs e)
        {
            OB_Station2_MotorForward.Off();
            OB_SF2_MotorForward.Off();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart108_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor1_Station2_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3088");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart109_FlowRun(object sender, EventArgs e)
        {
            if (CYL_Conveyor2_Station1_Jacking.Off())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3089");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart111_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.On();
            if (IB_Conveyor1_Station2_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3097");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart113_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            if (IB_Conveyor1_Station3_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3098");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart110_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station1_StopCylinder.On();
            if (IB_Conveyor2_Station1_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3099");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart112_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station2_StopCylinder.On();
            if (IB_Conveyor2_Station2_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3100");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart120_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station3_StopCylinder.On();
            if (IB_Conveyor2_Station3_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3101");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart114_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_MotorForward.On();
            OB_Conveyor1_MotorReverse.Off();
            OB_Conveyor2_MotorForward.On();
            OB_Conveyor2_MotorReverse.Off();
            OB_SF1_MotorForward.On();
            OB_SF1_MotorReverse.Off();
            OB_SF2_MotorForward.On();
            OB_SF2_MotorReverse.Off();
            OB_Station1_MotorForward.On();
            OB_Station1_MotorReverse.Off();
            OB_Station2_MotorForward.On();
            OB_Station2_MotorReverse.Off();
            if (Conveyor1Timeout.IsOn(10000))
            {
                OB_Conveyor1_MotorForward.Off();
                OB_Conveyor2_MotorForward.Off();
                OB_SF1_MotorForward.Off();
                OB_SF2_MotorForward.Off();
                OB_Station1_MotorForward.Off();
                OB_Station2_MotorForward.Off();
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart115_FlowRun(object sender, EventArgs e)
        {
            bool A = IB_Conveyor1_BoardIn.IsOn();
            bool B = IB_Conveyor2_Boardout.IsOn();
            bool C = IB_Conveyor1_Staiton1_BoardStop.IsOn();
            bool D = IB_Conveyor1_Staiton2_BoardStop.IsOn();
            bool E = IB_Conveyor1_Staiton3_BoardStop.IsOn();
            bool F = IB_Conveyor2_Station1_BoardStop.IsOn();
            bool G = IB_Conveyor2_Station2_BoardStop.IsOn();
            bool H = IB_Conveyor2_Station3_BoardStop.IsOn();
            Conveyor1Timeout.Restart();
            if (A || B || C || D || E || F || G || H)
            {
                JSDK.Alarm.Show("3102");
                return FCResultType.IDLE;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart116_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.Off();
            if (IB_Conveyor1_Station1_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3090");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart117_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.Off();
            if (IB_Conveyor1_Station2_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3091");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart118_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.Off();
            if (IB_Conveyor1_Station3_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3092");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart119_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station1_StopCylinder.Off();
            if (IB_Conveyor2_Station1_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3093");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart121_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station2_StopCylinder.Off();
            if (IB_Conveyor2_Station2_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3094");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart122_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station3_StopCylinder.Off();
            if (IB_Conveyor2_Station3_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3095");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart123_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.Off();
            if (IB_Conveyor1_Station2_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3091");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart124_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || IB_Conveyor1_Staiton2_BoardStop.IsOn())
            {
                B_ScrewStationReadySignal = false;
                C_DelayMs(600);
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart125_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart126_FlowRun(object sender, EventArgs e)
        {
            StationMachineIn1 = false;
            MachineAvailable1 = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart129_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.Off();
            if (IB_Conveyor1_Station3_StopCylinderUp.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3092");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart130_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.On();
            if (IB_Conveyor1_Station2_StopCylinderDown.IsOn())
            {
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(TimeOut))
            {
                Conveyor1Timeout.Restart();
                JSDK.Alarm.Show("3097");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart131_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || IB_Conveyor1_Staiton3_BoardStop.IsOn())
            {
                B_ScrewStationReadySignal = false;
                C_DelayMs(600);
                Conveyor1Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }
        #endregion

        private FCResultType flowChart132_FlowRun(object sender, EventArgs e)
        {
            StationMachineIn2 = false;
            MachineAvailable2 = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart133_FlowRun(object sender, EventArgs e)
        {
            B_ConveyorB_Buffer_Ready = true;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart135_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart127_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || DisableRFID)
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (myRFID1.B_Result)
            {
                WriteToRFID(32, true, RFID1);
            }
            else
            {
                WriteToRFID(32, false, RFID1);
            }
            Conveyor2Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart136_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || IB_Conveyor1_Staiton3_BoardStop.IsOn())
            {
                C_DelayMs(600);
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart138_FlowRun(object sender, EventArgs e)
        {
            if (B_ConveyorB_Buffer_Ready)
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart139_FlowRun(object sender, EventArgs e)
        {
            if (!B_ConveyorB_Buffer_Ready)
            {
                Conveyor2Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart128_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun || DisableRFID)
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            if (myRFID2.B_Result)
            {
                WriteToRFID(33, true, RFID2);
            }
            else
            {
                WriteToRFID(33, false, RFID2);
            }
            Conveyor3Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart142_FlowRun(object sender, EventArgs e)
        {
            if (B_ConveyorB_Buffer_Ready)
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart24_FlowRun(object sender, EventArgs e)
        {
            if (!B_ConveyorB_Buffer_Ready)
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart140_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart143_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                Conveyor2Timeout.Restart();
                Station1_Discharging = false;
                return FCResultType.NEXT;
            }
            if (IB_Conveyor1_Staiton2_BoardStop.IsOff())
            {
                Conveyor2Timeout.Restart();
                Station1_Discharging = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart134_FlowRun(object sender, EventArgs e)
        {
            if (!B_ConveyorB_Station1_Ready)
            {
                ConveyorB_Buffer_Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart137_FlowRun(object sender, EventArgs e)
        {
            if (!B_ConveyorB_Station2_Ready)
            {
                Conveyor4Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart64_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart141_FlowRun(object sender, EventArgs e)
        {
            OB_LocalMachineWorkNG_SMEMA.On();
            return FCResultType.NEXT;
        }
    }
}

