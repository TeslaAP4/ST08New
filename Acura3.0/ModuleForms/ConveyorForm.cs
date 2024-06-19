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
        public bool Stationwork1;  //工位1出料运行信号
        public bool Stationwork1Comp;
        public bool Stationwork2;  //工位2出料运行信号
        public bool Stationwork2Comp;
        public bool UpMachineAvailable_SMEMA;  //按钮模拟要料smema
        public bool BufferReady = false; //buffer station have board
        public bool DownMachineReady_SMEMA;  //按钮模拟出料smema
        public bool StationMachineIn1;  //工位1入料信号
        public bool B_ScrewStationReadySignal = false;// Screw station ready to request board from buffer
        public bool StationMachineIn2;  //工位2入料信号
        public bool ConveyorBStation1RobotStart1;  //流道B工位1机器手开始信号
        public bool ConveyorBStation1RobotComp1;  //流道B工位1机器手完成信号
        public bool ConveyorBStation2RobotStart2;  //流道B工位2机器手开始信号
        public bool ConveyorBStation2RobotComp2;  //流道B工位2机器手完成信号
        //public bool B_ScrewStation1_Available = false; //Screw station1 available to conveyorB buffer station
        //public bool B_ScrewStation2_Available = false; //Screw station2 available to conveyorB buffer station
        //public bool ConveyorBStart1;
        //public bool ConveyorBStart2;

        public bool B_ConveyorB_Station1_Ready = false; //ConvetorB station2 request board signal
        public bool B_ConveyorB_Station2_Ready = false; //ConvetorB station3 request board signal

        public bool ByPass;   //ByPass流道模式
        public bool Dryrun;   //空跑模式
        public bool RFID;


        Fanuc_RobotControl fanuc_RobotControl = new Fanuc_RobotControl();
        JTimer J_AxisAutoTm = new JTimer();
        public JTimer Conveyor1Timeout = new JTimer();
        public JTimer Conveyor2Timeout = new JTimer();
        public JTimer Conveyor3Timeout = new JTimer();
        public JTimer Conveyor4Timeout = new JTimer();
        public JTimer Conveyor5Timeout = new JTimer();
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
            A_syGoleRFID.ReceiveHandler();
            B_syGoleRFID.ReceiveHandler();
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
            flowChart1.TaskReset();
            flowChart8.TaskReset();
            flowChart18.TaskReset();
            flowChart25.TaskReset();
            flowChart32.TaskReset();
        }

        public override void Initial()
        {
            flowChart0_11.TaskRun();
        }

        public override void Run()
        {
            flowChart1.TaskRun();
            flowChart8.TaskRun();
            flowChart18.TaskRun();
            flowChart25.TaskRun();
            flowChart32.TaskRun();
        }

        public override void StartRun()
        {
            RunTM.Restart();
        }

        public override void StopRun()
        {

        }
        #endregion

        #region Functions
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

        #endregion

        #region UI Update
        private void CvyTim_UpdateUI_Tick(object sender, EventArgs e)
        {

        }
        #endregion

        #region RFID
        public SyGoleRFID A_syGoleRFID = new SyGoleRFID();
        public SyGoleRFID B_syGoleRFID = new SyGoleRFID();
        public SyGoleRFID C_syGoleRFID = new SyGoleRFID();
        public SyGoleRFID D_syGoleRFID = new SyGoleRFID();

        List<MyRFIDDataStruct> RFIDDataStructList1 = new List<MyRFIDDataStruct>();
        List<MyRFIDDataStruct> RFIDDataStructList2 = new List<MyRFIDDataStruct>();

        public struct MyRFIDDataStruct
        {
            public SyGoleRFID SyGoleRFID;
            public bool ResultBool;
            public String RFIDReadData;

            public MyRFIDDataStruct(SyGoleRFID syGoleRFID, bool resultBool, string rFIDReadData)
            {
                SyGoleRFID = syGoleRFID;
                ResultBool = resultBool;
                RFIDReadData = rFIDReadData;
            }
        }

        public MyRFIDDataStruct myRFID1 = new MyRFIDDataStruct();
        public MyRFIDDataStruct myRFID2 = new MyRFIDDataStruct();
        public MyRFIDDataStruct myRFID3 = new MyRFIDDataStruct();
        public MyRFIDDataStruct myRFID4 = new MyRFIDDataStruct();


        private void B_RFIDAConnect_Click(object sender, EventArgs e)
        {
            if (A_syGoleRFID.connect)
            {
                TextDataShow("Device connected", R_RFIDADataShow, true);
                return;
            }
            if (A_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "A_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "A_RFIDPort")), GetRecipeValue("RSet", "A_RFIDId")))
            {
                B_RFIDAConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDADataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDADataShow, false);
        }


        private void B_RFIDBConnect_Click(object sender, EventArgs e)
        {
            if (B_syGoleRFID.connect)
            {
                TextDataShow("Device connected", R_RFIDBDataShow, true);
                return;
            }
            if (B_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "B_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "B_RFIDPort")), GetRecipeValue("RSet", "B_RFIDId")))
            {
                B_RFIDBConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDBDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDBDataShow, false);
        }


        private void B_RFIDCConnect_Click(object sender, EventArgs e)
        {
            if (C_syGoleRFID.connect)
            {
                TextDataShow("Device connected", R_RFIDCDataShow, true);
                return;
            }
            if (C_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "C_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "C_RFIDPort")), GetRecipeValue("RSet", "C_RFIDId")))
            {
                B_RFIDCConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDCDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDCDataShow, false);
        }


        private void B_RFIDDConnect_Click(object sender, EventArgs e)
        {
            if (D_syGoleRFID.connect)
            {
                TextDataShow("Device connected", R_RFIDDDataShow, true);
                return;
            }
            if (D_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "DRFIDIp"), ushort.Parse(GetRecipeValue("RSet", "D_RFIDPort")), GetRecipeValue("RSet", "D_RFIDId")))
            {
                B_RFIDDConnect.Enabled = false;
                TextDataShow("Connection successful", R_RFIDDDataShow, true);
                return;
            }
            TextDataShow("Connection failure", R_RFIDDDataShow, false);
        }


        private void B_RFIDADisconnect_Click(object sender, EventArgs e)
        {
            A_syGoleRFID.RFID_DisConnect();
            B_RFIDAConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDADataShow, true);
        }


        private void B_RFIDBDisconnect_Click(object sender, EventArgs e)
        {
            B_syGoleRFID.RFID_DisConnect();
            B_RFIDBConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDBDataShow, true);
        }


        private void B_RFIDCDisconnect_Click(object sender, EventArgs e)
        {
            C_syGoleRFID.RFID_DisConnect();
            B_RFIDCConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDCDataShow, true);
        }


        private void B_RFIDDDisconnect_Click(object sender, EventArgs e)
        {
            D_syGoleRFID.RFID_DisConnect();
            B_RFIDDConnect.Enabled = true;
            TextDataShow("Disconnect", R_RFIDDDataShow, true);
        }


        private void B_RFIDAReadUID_Click(object sender, EventArgs e)
        {
            if (!A_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + A_syGoleRFID.RFID_ReadUID(GetRecipeValue("RSet", "A_RFIDId")), R_RFIDADataShow, true);
        }


        private void B_RFIDBReadUID_Click(object sender, EventArgs e)
        {
            if (!B_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + B_syGoleRFID.RFID_ReadUID(GetRecipeValue("RSet", "B_RFIDId")), R_RFIDBDataShow, true);
        }


        private void B_RFIDCReadUID_Click(object sender, EventArgs e)
        {
            if (!C_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + C_syGoleRFID.RFID_ReadUID(GetRecipeValue("RSet", "C_RFIDId")), R_RFIDCDataShow, true);
        }


        private void B_RFIDDReadUID_Click(object sender, EventArgs e)
        {
            if (!D_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            TextDataShow("UID read successfully: " + D_syGoleRFID.RFID_ReadUID(GetRecipeValue("RSet", "D_RFIDId")), R_RFIDDDataShow, true);
        }


        private void B_RFIDAReadData_Click(object sender, EventArgs e)
        {
            if (!A_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            string pos = T_RFIDAOAdderss.Text == "" ? "0" : T_RFIDAOAdderss.Text;
            string len = T_RFIDAEndAddress.Text == "" ? "16" : T_RFIDAEndAddress.Text;
            TextDataShow("Read data successfully: " + A_syGoleRFID.RFID_ReadDataTostring(GetRecipeValue("RSet", "A_RFIDId"), pos, len), R_RFIDADataShow, true);
        }


        private void B_RFIDBReadData_Click(object sender, EventArgs e)
        {
            if (!B_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            string pos = T_RFIDBOAdderss.Text == "" ? "0" : T_RFIDBOAdderss.Text;
            string len = T_RFIDBEndAddress.Text == "" ? "16" : T_RFIDBEndAddress.Text;
            TextDataShow("Read data successfully: " + B_syGoleRFID.RFID_ReadDataTostring(GetRecipeValue("RSet", "B_RFIDId"), pos, len), R_RFIDBDataShow, true);
        }


        private void B_RFIDCReadData_Click(object sender, EventArgs e)
        {
            if (!C_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            string pos = T_RFIDCOAdderss.Text == "" ? "0" : T_RFIDCOAdderss.Text;
            string len = T_RFIDCEndAddress.Text == "" ? "16" : T_RFIDCEndAddress.Text;
            TextDataShow("Read data successfully: " + C_syGoleRFID.RFID_ReadDataTostring(GetRecipeValue("RSet", "C_RFIDId"), pos, len), R_RFIDCDataShow, true);
        }


        private void B_RFIDDReadData_Click(object sender, EventArgs e)
        {
            if (!D_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            string pos = T_RFIDDOAdderss.Text == "" ? "0" : T_RFIDDOAdderss.Text;
            string len = T_RFIDDEndAddress.Text == "" ? "16" : T_RFIDDEndAddress.Text;
            TextDataShow("Read data successfully: " + D_syGoleRFID.RFID_ReadDataTostring(GetRecipeValue("RSet", "D_RFIDId"), pos, len), R_RFIDDDataShow, true);
        }


        private void B_RFIDAWriteData_Click(object sender, EventArgs e)
        {
            if (!A_syGoleRFID.connect)
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
            TextDataShow("Write data successfully: " + A_syGoleRFID.RFID_Write(GetRecipeValue("RSet", "A_RFIDId"), T_RFIDAWriteData.Text, pos, len), R_RFIDADataShow, true);
        }


        private void B_RFIDBWriteData_Click(object sender, EventArgs e)
        {
            if (!B_syGoleRFID.connect)
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
            TextDataShow("Write data successfully: " + B_syGoleRFID.RFID_Write(GetRecipeValue("RSet", "B_RFIDId"), T_RFIDBWriteData.Text, pos, len), R_RFIDBDataShow, true);
        }


        private void B_RFIDCWriteData_Click(object sender, EventArgs e)
        {
            if (!C_syGoleRFID.connect)
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
            TextDataShow("Write data successfully: " + C_syGoleRFID.RFID_Write(GetRecipeValue("RSet", "C_RFIDId"), T_RFIDCWriteData.Text, pos, len), R_RFIDCDataShow, true);
        }


        private void B_RFIDDWriteData_Click(object sender, EventArgs e)
        {
            if (!D_syGoleRFID.connect)
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
            TextDataShow("Write data successfully: " + D_syGoleRFID.RFID_Write(GetRecipeValue("RSet", "D_RFIDId"), T_RFIDDWriteData.Text, pos, len), R_RFIDDDataShow, true);
        }


        private void B_RFIDAClearData_Click(object sender, EventArgs e)
        {
            if (!A_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDADataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + A_syGoleRFID.RFID_Clear(GetRecipeValue("RSet", "A_RFIDId")), R_RFIDADataShow, true);
        }


        private void B_RFIDBClearData_Click(object sender, EventArgs e)
        {
            if (!B_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDBDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + B_syGoleRFID.RFID_Clear(GetRecipeValue("RSet", "B_RFIDId")), R_RFIDBDataShow, true);
        }


        private void B_RFIDCClearData_Click(object sender, EventArgs e)
        {
            if (!C_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDCDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + C_syGoleRFID.RFID_Clear(GetRecipeValue("RSet", "C_RFIDId")), R_RFIDCDataShow, true);
        }


        private void B_RFIDDClearData_Click(object sender, EventArgs e)
        {
            if (!D_syGoleRFID.connect)
            {
                TextDataShow("Device not connected", R_RFIDDDataShow, false);
                return;
            }
            TextDataShow("Succeeded in clearing data: " + D_syGoleRFID.RFID_Clear(GetRecipeValue("RSet", "D_RFIDId")), R_RFIDDDataShow, true);
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
        #endregion

        private FCResultType flowChart0_11_FlowRun(object sender, EventArgs e)
        {
            MachineAvailable1 = false;
            MachineAvailable2 = false;
            Station1Start = false;
            Station2Start = false;
            //Dryrun1 = false;
            //Dryrun2 = false;
            B_ScrewStationReadySignal = false;
            Stationwork1 = false;
            Stationwork1Comp = false;
            Stationwork2 = false;
            Stationwork2Comp = false;
            UpMachineAvailable_SMEMA = false;
            DownMachineReady_SMEMA = false;
            StationMachineIn1 = false;
            StationMachineIn2 = false;
            ConveyorBStation1RobotStart1 = false;
            ConveyorBStation1RobotComp1 = false;
            ConveyorBStation2RobotStart2 = false;
            ConveyorBStation2RobotComp2 = false;
            //ConveyorBStart1 = false;
            //ConveyorBStart2 = false;
            B_ScrewStation1_Available = false;
            B_ScrewStation2_Available = false;
            B_ConveyorB_Station1_Ready = false;
            B_ConveyorB_Station2_Ready = false;
            bInitialOk = true;
            ByPass = false;
            ByPass = MiddleLayer.SystemF.GetSettingValue("PSet", "ByPass");
            Dryrun = MiddleLayer.SystemF.GetSettingValue("PSet", "Dryrun");
            RFID = false;// MiddleLayer.SystemF.GetSettingValue("PSet", "UseRFID");
            RFIDDataStructList1.Clear();
            RFIDDataStructList2.Clear();
            return FCResultType.IDLE;
        }


        private FCResultType flowChart55_FlowRun(object sender, EventArgs e)
        {
            if (A_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "A_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "A_RFIDPort")), GetRecipeValue("RSet", "A_RFIDId")))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart56_FlowRun(object sender, EventArgs e)
        {
            if (A_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "B_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "B_RFIDPort")), GetRecipeValue("RSet", "B_RFIDId")))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart57_FlowRun(object sender, EventArgs e)
        {
            if (A_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "C_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "C_RFIDPort")), GetRecipeValue("RSet", "C_RFIDId")))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart58_FlowRun(object sender, EventArgs e)
        {
            if (A_syGoleRFID.RFID_Connect(GetRecipeValue("RSet", "D_RFIDIp"), ushort.Parse(GetRecipeValue("RSet", "D_RFIDPort")), GetRecipeValue("RSet", "D_RFIDId")))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart66_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_JackingCylinderDown.On();
            OB_Conveyor1_Station2_JackingCylinderDown.On();
            OB_Conveyor2_Station1_JackingCylinderDown.On();
            OB_Conveyor2_Station3_JackingCylinderDown.On();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart68_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.IDLE;
        }

        private FCResultType flowChart67_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.Off();
            OB_Conveyor1_Station2_StopCylinder.Off();
            OB_Conveyor1_Station3_StopCylinder.Off();
            OB_Conveyor2_Station1_StopCylinder.Off();
            OB_Conveyor2_Station2_StopCylinder.Off();
            OB_Conveyor2_Station3_StopCylinder.Off();
            return FCResultType.NEXT;
        }


        #region autoflow
        private FCResultType flowChart1_FlowRun_2(object sender, EventArgs e)
        {
            OB_Conveyor1_MotorForward.On();
            Conveyor1Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart1.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_FlowRun_1(object sender, EventArgs e)
        {
            if ((IB_SF1_BoardStop.IsOff() && IB_Conveyor1_Staiton2_BoardStop.IsOff() && !StationMachineIn1 && !Station1Start && !Stationwork1) ||
                (IB_SF2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn2 && !Station2Start && !Stationwork2))
            {
                B_ScrewStationReadySignal = true;
                if (BufferReady)
                {
                    if (IB_SF1_BoardStop.IsOff() && IB_Conveyor1_Staiton2_BoardStop.IsOff() && !StationMachineIn1 && !Station1Start && !Stationwork1)
                    {
                        StationMachineIn1 = true;
                        OB_Conveyor1_Station2_StopCylinder.Off();
                        Conveyor1Timeout.Restart();
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                    if (IB_SF2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn2 && !Station2Start && !Stationwork2)
                    {
                        StationMachineIn2 = true;
                        OB_Conveyor1_Station2_StopCylinder.On();
                        OB_Conveyor1_Station3_StopCylinder.Off();
                        Conveyor1Timeout.Restart();
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2.Text} finish", true);
                        return FCResultType.CASE1;
                    }
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart3_FlowRun_1(object sender, EventArgs e)
        {
            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2.Text} overtime", false);
                JSDK.Alarm.Show("A5115", "ConveyorA Station1 Product Load TIMEOUT!");
            }

            if (IB_Conveyor1_Staiton2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                B_ScrewStationReadySignal = false;
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart3.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart60_FlowRun(object sender, EventArgs e)
        {
            Conveyor1Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart60.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart39_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart39.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(500))
            {
                CYL_Conveyor1_Station1_Jacking.On();
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart39.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_FlowRun_1(object sender, EventArgs e)
        {
            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4.Text} overtime", false);
                JSDK.Alarm.Show("A5116", "ConveyorA Station2 Product Load TIMEOUT!");
            }

            if (IB_Conveyor1_Staiton3_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                B_ScrewStationReadySignal = false;
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart59_FlowRun(object sender, EventArgs e)
        {
            Conveyor1Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart59.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart42_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart39.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(500))
            {
                CYL_Conveyor1_Station2_Jacking.On();
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart42.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart40_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart40.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (IB_Conveyor1_Station1_JackingCylinderUp.IsOn())
            {
                OB_Station1_MotorForward.On();
                OB_SF1_MotorForward.On();
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart40.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart40.Text} overtime", false);
                JSDK.Alarm.Show("A5113", "ConveyorA Station1 Jacking Cylinder Up TIMEOUT");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart41_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart41.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart41.Text} overtime", false);
                JSDK.Alarm.Show("A5115", "ConveyorA Station1 Product Load TIMEOUT!");
            }

            if (IB_SF1_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart41.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart5_FlowRun_1(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart5.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(500))
            {
                OB_Station1_MotorForward.Off();
                OB_SF1_MotorForward.Off();

                Conveyor1Timeout.Restart();
                CYL_Conveyor1_Station1_Jacking.Off();
                Station1Start = true;
                StationMachineIn1 = false;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart5.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart43_FlowRun(object sender, EventArgs e)
        {
            if (OB_Conveyor1_Station2_JackingCylinderUp.IsOn())
            {
                OB_Station2_MotorForward.On();
                OB_SF2_MotorForward.On();
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart43.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart43.Text} overtime", false);
                JSDK.Alarm.Show("A5114", "ConveyorA Station2 Jacking Cylinder Up TIMEOUT");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart44_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart41.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (Conveyor1Timeout.IsOn(10000))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart44.Text} overtime", false);
                JSDK.Alarm.Show("A5116", "ConveyorA Station2 Product Load TIMEOUT!");
            }

            if (IB_SF2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor1Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart44.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart6_FlowRun_1(object sender, EventArgs e)
        {
            if (Conveyor1Timeout.IsOn(500))
            {
                OB_Station2_MotorForward.Off();
                OB_SF2_MotorForward.Off();

                Conveyor1Timeout.Restart();
                CYL_Conveyor1_Station2_Jacking.Off();
                Station2Start = true;
                StationMachineIn2 = false;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart18_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode : " + $"{this.Text} Module {flowChart18.Text} finish", true);
                myRFID2 = new MyRFIDDataStruct(B_syGoleRFID, true, "OK");
            }
            Conveyor3Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart18.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart9_FlowRun_1(object sender, EventArgs e)
        {
            if (MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOn())
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart9.Text} overtime", false);
                JSDK.Alarm.Show("A5117", "Please check if there is product on the conveyorA");
            }

            if ((MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn1 && !Stationwork2Comp)
                || (Dryrun && MachineAvailable1) || ByPass)
            {
                Stationwork1 = true;
                Stationwork1Comp = true;
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart9.Text} finish", true);
                return FCResultType.NEXT;
            }

            return FCResultType.IDLE;
        }

        private FCResultType flowChart45_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart45.Text} finish", true);
                return FCResultType.NEXT;
            }
            OB_Station1_MotorReverse.On();
            OB_SF1_MotorReverse.On();
            Conveyor2Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart45.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart46_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart46.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (Conveyor2Timeout.IsOn(500))
            {
                OB_Station1_MotorReverse.Off();
                OB_SF1_MotorReverse.Off();
                CYL_Conveyor1_Station1_Jacking.Off();
                if (IB_Conveyor1_Station1_JackingCylinderDown.IsOn())
                {
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart46.Text} finish", true);
                    Conveyor2Timeout.Restart();
                    return FCResultType.NEXT;
                }
            }

            if (Conveyor2Timeout.IsOn(10000))
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart46.Text} overtime", false);
                JSDK.Alarm.Show("A5123", "ConveyorA Station1 Jacking Cylinder Down TIMEOUT");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart47_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor2Timeout.IsOn(300))
            {
                Conveyor2Timeout.Restart();
                OB_Conveyor1_Station2_StopCylinder.Off();
                Stationwork1 = false;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart47.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart48_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor2Timeout.IsOn(300))
            {
                OB_Conveyor1_Station3_StopCylinder.Off();
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart48.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart15_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart15.Text} finish", true);
            if (IB_Conveyor2_Station1_BoardStop.IsOn() || Dryrun)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.CASE1;
        }

        private FCResultType flowChart16_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart17_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart24_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart19_FlowRun(object sender, EventArgs e)
        {
            if ((MachineAvailable2 && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !Stationwork1Comp && !StationMachineIn2)
                || (Dryrun && MachineAvailable2))
            {
                Stationwork2 = true;
                Stationwork2Comp = true;
                Conveyor3Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart19.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart20_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            CYL_Conveyor1_Station2_Jacking.On();
            if (IB_Conveyor1_Station2_JackingCylinderUp.IsOn())
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }

            if (Conveyor3Timeout.IsOn(10000))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("A5114", "ConveyorA Station2 Jacking Cylinder Up TIMEOUT");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart49_FlowRun(object sender, EventArgs e)
        {
            OB_Station2_MotorReverse.On();
            OB_SF2_MotorReverse.On();
            Conveyor3Timeout.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart21_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor3Timeout.IsOn(10000))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("A5122", "ConveyorA Station2 Product Unload TIMEOUT!");
            }
            if (IB_Conveyor1_Staiton3_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart50_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor3Timeout.IsOn(500))
            {
                OB_Station2_MotorReverse.Off();
                OB_SF2_MotorReverse.Off();
                CYL_Conveyor1_Station2_Jacking.Off();
                if (IB_Conveyor1_Station2_JackingCylinderDown.IsOn())
                {
                    Conveyor3Timeout.Restart();
                    return FCResultType.NEXT;
                }
            }

            if (Conveyor3Timeout.IsOn(10000))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("A5124", "ConveyorA Station2 Jacking Cylinder Down TIMEOUT");
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart22_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            if (IB_Conveyor1_Station3_StopCylinderDown.IsOn())
            {
                Conveyor3Timeout.Restart();
                return FCResultType.NEXT;
            }

            if (Conveyor3Timeout.IsOn(10000))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("A5126", "ConveyorA Station2 Stop Cylinder Down TIMEOUT");
            }
            return FCResultType.IDLE;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpMachineAvailable_SMEMA = true;
        }

        private FCResultType flowChart7_FlowRun(object sender, EventArgs e)
        {
            if ((IB_Conveyor1_Staiton3_BoardStop.IsOff() && Conveyor2Timeout.IsOn(1000)) || (Dryrun && C_DelayMs(1000)))
            {
                Conveyor2Timeout.Restart();
                ConveyorBStart1 = true;
                Stationwork1Comp = false;
                MachineAvailable1 = false;
                RFIDDataStructList1.Add(myRFID1);
                return FCResultType.NEXT;
            }

            if (Conveyor2Timeout.IsOn(10000))
            {
                Conveyor2Timeout.Restart();
                JSDK.Alarm.Show("A5121", "ConveyorA Station1 Product Unload TIMEOUT!");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart10_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_Staiton3_BoardStop.IsOff() || (Dryrun && C_DelayMs(1000)))
            {
                ConveyorBStart1 = true;
                Stationwork2 = false;
                Stationwork2Comp = false;
                MachineAvailable2 = false;
                Conveyor3Timeout.Restart();
                RFIDDataStructList1.Add(myRFID2);
                return FCResultType.NEXT;
            }

            if (Conveyor3Timeout.IsOn(10000))
            {
                Conveyor3Timeout.Restart();
                JSDK.Alarm.Show("A5122", "ConveyorA Station2 Product Unload TIMEOUT!");
            }
            return FCResultType.IDLE;
        }
                     
        private void button2_Click(object sender, EventArgs e)
        {
            DownMachineReady_SMEMA = true;
        }

        private FCResultType flowChart54_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart72_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart72.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart73_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode: " + $"{this.Text} Module {flowChart73.Text} finish", true);
                return FCResultType.CASE1;
            }
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart73.Text} finish", true);
            if (IB_Conveyor1_Staiton1_BoardStop.IsOn())
            {
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart74_FlowRun(object sender, EventArgs e)
        {
            OB_LocalMachineReady_SMEMA.On();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart74.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart75_FlowRun(object sender, EventArgs e)
        {
            if (IB_UpMachineAvailable_SMEMA.IsOn() || UpMachineAvailable_SMEMA)
            {
                Conveyor1Timeout.Restart();
                UpMachineAvailable_SMEMA = false;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart75.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart76_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_BoardIn.IsOn())
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart76.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart77_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor1_Staiton1_BoardStop.IsOn())
            {
                C_DelayMs(500);
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart77.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart78_FlowRun(object sender, EventArgs e)
        {
            OB_LocalMachineReady_SMEMA.Off();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart78.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart79_FlowRun(object sender, EventArgs e)
        {
            BufferReady = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart79.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart85_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart85.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart80_FlowRun(object sender, EventArgs e)
        {
            if (B_ScrewStationReadySignal)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart80.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart81_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station1_StopCylinder.On();
            if (IB_Conveyor1_Station1_StopCylinderDown.IsOn())
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart81.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart82_FlowRun(object sender, EventArgs e)
        {
            if (!B_ScrewStationReadySignal)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart82.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart83_FlowRun(object sender, EventArgs e)
        {
            BufferReady = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart83.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart84_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart84.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart8_FlowRun(object sender, EventArgs e)
        {
            if (Dryrun)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode : " + $"{this.Text} Module {flowChart8.Text} finish", true);
                myRFID1 = new MyRFIDDataStruct(A_syGoleRFID, true, "OK");
            }
            Conveyor2Timeout.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart8.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart9_FlowRun(object sender, EventArgs e)
        {
            if (MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOn())
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart9.Text} overtime", false);
                JSDK.Alarm.Show("A5117", "Please check if there is product on the conveyorA");
            }

            if ((MachineAvailable1 && IB_Conveyor1_Staiton2_BoardStop.IsOff() && IB_Conveyor1_Staiton3_BoardStop.IsOff() && !StationMachineIn1 && !Stationwork2Comp)
                || (Dryrun && MachineAvailable1) || ByPass)
            {
                Stationwork1 = true;
                Stationwork1Comp = true;
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart9.Text} finish", true);
                return FCResultType.NEXT;
            }

            return FCResultType.IDLE;
        }

        private FCResultType flowChart11_FlowRun(object sender, EventArgs e)
        {
            if (ByPass)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "ByPass Mode : " + $"{this.Text} Module {flowChart11.Text} finish", true);
                return FCResultType.NEXT;
            }
            OB_Conveyor1_Station2_StopCylinder.On();
            CYL_Conveyor1_Station1_Jacking.On();
            if (IB_Conveyor1_Station1_JackingCylinderUp.IsOn())
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart11.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (Conveyor2Timeout.IsOn(10000))
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart11.Text} overtime", false);
                JSDK.Alarm.Show("A5113", "ConveyorA Station1 Jacking Cylinder Up TIMEOUT");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart12_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor2Timeout.IsOn(10000))
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart12.Text} overtime", false);
                JSDK.Alarm.Show("A5121", "ConveyorA Station1 Product Unload TIMEOUT!");
            }
            if (IB_Conveyor1_Staiton2_BoardStop.IsOn() || (Dryrun && C_DelayMs(1000)) || (ByPass && IB_Conveyor1_Staiton2_BoardStop.IsOn()))
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart12.Text} finish", true);
                return FCResultType.NEXT;             
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart13_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station2_StopCylinder.On();
            if (IB_Conveyor1_Station2_StopCylinderDown.IsOn())
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart13.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (Conveyor2Timeout.IsOn(10000))
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart13.Text} overtime", false);
                JSDK.Alarm.Show("A5125", "ConveyorA Station1 Stop Cylinder Down TIMEOUT");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart14_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor1_Station3_StopCylinder.On();
            if (Dryrun && C_DelayMs(1000))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode : " + $"{this.Text} Module {flowChart14.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (IB_Conveyor1_Staiton3_BoardStop.IsOn() && IB_Conveyor1_Station3_StopCylinderDown.IsOn())
            {
                Conveyor2Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart14.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart23_FlowRun(object sender, EventArgs e)
        {
            if (Conveyor3Timeout.IsOn(300))
            {
                OB_Conveyor1_Station3_StopCylinder.Off();
                Conveyor3Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart23.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart71_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart71.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart86_FlowRun(object sender, EventArgs e)
        {
            if (IB_Conveyor2_Station1_BoardStop.IsOn() || (Dryrun && C_DelayMs(2000)))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart86.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart88_FlowRun(object sender, EventArgs e)
        {
            if (B_ConveyorB_Station1_Ready)
            {
                B_ConveyorB_Station1_Ready = false;
                ConveyorB_Buffer_Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart88.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart92_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart92.Text} finish", true);
            return FCResultType.CASE1;
        }

        private FCResultType flowChart89_FlowRun(object sender, EventArgs e)
        {
            OB_Conveyor2_Station1_StopCylinder.On();
            if (IB_Conveyor2_Station1_StopCylinderDown.IsOn())
            {
                ConveyorB_Buffer_Timeout.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart89.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart90_FlowRun(object sender, EventArgs e)
        {
            if (ConveyorB_Buffer_Timeout.IsOn(500))
            {
                OB_Conveyor2_Station1_StopCylinder.Off();
                if (IB_Conveyor2_Station1_StopCylinderUp.IsOn())
                {
                    ConveyorB_Buffer_Timeout.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart90.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart93_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart93.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart91_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart91.Text} finish", true);
            return FCResultType.NEXT;
        }
    }
    #endregion
}
