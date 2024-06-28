using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Acura3._0.Classes;
using AcuraLibrary.Forms;
using AlphaRap.Classes;
using Cognex.VisionPro;
using JabilSDK;
using JabilSDK.Enums;
using JabilSDK.Forms;
using NPFanucRobotDLL;
using static JabilSDK.AlarmClass;

namespace Acura3._0.ModuleForms
{
    public partial class CoverAssembly : ModuleBaseForm
    {
        public CoverAssembly()
        {
            InitializeComponent();
            MessageForm.MuteRaise += MessageForm_MuteRaise;
            FlowChartMessage.PauseRaise += FlowChartMessage_PauseRaise;
            FlowChartMessage.ResetTimerRaise += FlowChartMessage_ResetRaise;
            SetDoubleBuffer(plProductionSetting);
            ReadProductData();
        }
        #region Local Method
        private void MessageForm_MuteRaise(object sender, EventArgs e)
        {
            MiddleLayer.SystemF.BuzzOff();
        }

        private void FlowChartMessage_PauseRaise(object sender, EventArgs e)
        {
            MiddleLayer.StopRun();
        }

        private void FlowChartMessage_ResetRaise(object sender, EventArgs e)
        {
            RunTM.Restart();
        }

        private void SetDoubleBuffer(Control cont)
        {
            typeof(Control).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, cont, new object[] { true });
        }

        #endregion
        public override void InitialReset()
        {
            flowChart1.TaskReset();

            B_Feeder_PickFlow = false;
            B_Assembly_PlaceFlow = false;
            B_VisionPick_Flow = false;
            B_VisionPlace_Flow = false;
            B_Feeder_TheSecondCover_VisonNG = false;
            B_Screw_VisionFlow = false;
            B_ScrewFlow = false;
            B_Process = false;
            B_ThrowScrewFlow = false;
            B_GetScrewData = false;
            feeder_Snap_index = 1;
            feeder_Pick_index = 1;
            workplace_Snap_index = 1;
            I_SnapNGtimes = 0;
            I_ScrewCount = 1;
            offSetX = 0;
            offSetY = 0;
            offSetA = 0;
        }

        public override void Initial()
        {
            flowChart1.TaskRun();
        }

        public override void RunReset()
        {
            flowChart5.TaskReset();
            flowChart18.TaskReset();
            flowChart41.TaskReset();
            flowChart78.TaskReset();
            flowChart83.TaskReset();
            flowChart39.TaskReset();
            flowChart40.TaskReset();
        }

        public override void Run()
        {
            flowChart5.TaskRun();
            flowChart18.TaskRun();
            flowChart41.TaskRun();
            flowChart78.TaskRun();
            flowChart83.TaskRun();
            flowChart39.TaskRun();
            flowChart40.TaskRun();
        }

        public override void ServoOn()
        {
            MTR_Jacking.ServoOn();
        }

        public override void ServoOff()
        {
            MTR_Jacking.ServoOff();
        }

        public override void StopRun()
        {
            F_Robot.RobotPause(OB_Robot_Maintain);
        }

        public override void StartRun()
        {
            F_Robot.RobotContinue(OB_Robot_Maintain, OB_Robot_Start);
        }

        public bool B_ByPassVision { get => GetSettingValue("PSet", "ByPassVisionResult"); }

        public bool B_ByPassScrew { get => GetSettingValue("PSet", "ByPassScrewResult"); }

        public bool B_ByPassDisplacement { get => GetSettingValue("PSet", "ByPassDisplacementResult"); }

        public bool B_ModuleDryRun { get => MiddleLayer.SystemF.GetSettingValue("PSet", "CoverAssemblyDryrun"); }

        public double offSetX_Min { get => GetRecipeValue("RSet", "offSetX_Min"); }

        public double offSetX_Max { get => GetRecipeValue("RSet", "offSetX_Max"); }

        public double offSetY_Min { get => GetRecipeValue("RSet", "offSetY_Min"); }

        public double offSetY_Max { get => GetRecipeValue("RSet", "offSetY_Max"); }

        public double offSetA_Min { get => GetRecipeValue("RSet", "offSetA_Min"); }

        public double offSetA_Max { get => GetRecipeValue("RSet", "offSetA_Max"); }

        /// <summary>
        /// 初始化流程时间实例化
        /// </summary>
        public JTimer J_Initial = new JTimer();

        /// <summary>
        /// 自动流程时间实例化
        /// </summary>
        public JTimer J_AutoRun = new JTimer();

        #region //机器人启动参数

        public bool RobotStart()
        {
            OB_Robot_Maintain.Off();
            OB_Robot_Program1.Off();
            DelayMs(1000);
            OB_Robot_Stop.On();
            DelayMs(1000);
            OB_Robot_Stop.Off();
            OB_Robot_Start.Off();
            OB_Robot_Maintain.On();
            OB_Robot_Enable.On();
            OB_Robot_Reset.On();
            DelayMs(1000);
            OB_Robot_Reset.Off();
            DelayMs(1000);
            OB_Robot_Start.On();
            DelayMs(1000);
            OB_Robot_Start.Off();
            OB_Robot_Program1.On();
            return true;
        }
        #endregion

        public bool B_Feeder_PickFlow = false;

        public bool B_VisionPick_Flow = false;

        public bool B_VisionPlace_Flow = false;

        public bool B_Assembly_PlaceFlow = false;

        public bool B_Screw_VisionFlow = false;

        public bool B_ScrewFlow = false;

        public bool B_ThrowScrewFlow = false;

        public bool B_Auto_NinePoint_Calibration = false;

        public bool B_VaccumON = false;

        public bool B_Process = false;

        public bool B_GetScrewData = false;

        int feeder_Snap_index = 1;
        int feeder_Pick_index = 1;
        int workplace_Snap_index = 1;
        int I_ScrewCount = 1;
        int calibration_Index = 1;

        public struct ScrewData
        {
            public int screwName;
            public DateTime screwTime;
            public double finalTorque;
            public double Angle;
            public double cycleTime;
            public int NumbleOfTurns;
            public double DisplacementValue;
            public bool State;
        }

        #region Atlas
        public EthernetPF PFClient = new EthernetPF();
        public bool bSubscribe;
        public bool bComStart;
        public bool bConnect;
        /// <summary>
        /// 电批连接方法
        /// </summary>
        public void BConnect()
        {
            bConnect = PFClient.Connect();
            bComStart = PFClient.StartCommunication();
            bSubscribe = PFClient.SubscribeLastTighteningResult();
            PFKeepAlive();
            PFClient.CommunicationAlive();
        }

        public bool PFKeepAlive()
        {
            string Response = PFClient.SendAndWaitForResponse(MID.M9999, TimeSpan.FromSeconds(5));

            if (Response == null)
            {
                return false;
            }
            if (Response.Contains("00209999"))//00209999
            {
                PFClient.CommunicationAlive();
                return true;
            }
            PFClient.CommunicationAlive();
            return false;
        }
        private void btnConnectScrew_Click(object sender, EventArgs e)
        {
            PFClient.IP = GetSettingValue("RSet", "ScrewIP");
            PFClient.Port = GetSettingValue("RSet", "ScrewPort");
            BConnect();
            if (bConnect && bComStart && bSubscribe)
            {
                btnDisScrew.BackColor = Color.Green;
            }
            else
            {
                btnDisScrew.BackColor = Color.Red;
            }
        }

        private void btnDisScrew_Click(object sender, EventArgs e)
        {
            PFClient.Disconnect();
            bConnect = false;
            btnDisScrew.BackColor = Color.White;
            //ScrewDriver.Disconnect();
            //btnConnectScrew.BackColor = Color.Transparent;
        }

        private void btn_ReadScrewData_Click(object sender, EventArgs e)
        {
            PFClient.SubscribeLastTighteningResult();
            string str = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 结果:" + PFClient.LastTighteningResult.TIGHTENING_STATUS.ToString() + " 扭力:" + PFClient.LastTighteningResult.TORQUE.ToString() + " 角度:" + PFClient.LastTighteningResult.ANGLE.ToString() + "\r\n";
            T_ScrewData.AppendText(str);
        }

        #endregion

        public ScrewData screwData;
        //public void WriteScrewData(ScrewData screwData)
        //{
        //    RefreshDifferentThreadUI(D_ScrewResultsShow, () =>
        //    {
        //        int index = D_ScrewResultsShow.Rows.Add();
        //        D_ScrewResultsShow.ClearSelection();
        //        D_ScrewResultsShow.Rows[index].Cells[0].Value = screwData.screwName.ToString();
        //        D_ScrewResultsShow.Rows[index].Cells[1].Value = screwData.finalTorque.ToString("f3");
        //        D_ScrewResultsShow.Rows[index].Cells[2].Value = screwData.Angle.ToString("f3");
        //        D_ScrewResultsShow.Rows[index].Cells[3].Value = screwData.cycleTime.ToString();
        //        D_ScrewResultsShow.Rows[index].Cells[4].Value = screwData.NumbleOfTurns.ToString();
        //        D_ScrewResultsShow.Rows[index].Cells[5].Value = screwData.DisplacementValue.ToString();
        //        D_ScrewResultsShow.Rows[index].Cells[6].Value = screwData.State ? "OK" : "NG";
        //        if (screwData.State) { D_ScrewResultsShow.Rows[index].DefaultCellStyle.BackColor = Color.Green; }
        //        else { D_ScrewResultsShow.Rows[index].DefaultCellStyle.BackColor = Color.Red; }
        //        D_ScrewResultsShow.Rows[index].Selected = true;
        //        D_ScrewResultsShow.FirstDisplayedScrollingRowIndex = index;
        //    });
        //}

        public void ScrewDataShowUI(ScrewData screwData)
        {
            //CT = DateTime.Now.Subtract(TimeStart).TotalSeconds.ToString("F2");
            string[] data = new string[7];
            data[0] = screwData.screwName.ToString();
            data[1] = screwData.screwTime.ToString("yyyy-MM-dd HH:mm:ss");
            data[2] = screwData.finalTorque.ToString("f3");
            data[3] = screwData.Angle.ToString("f3");
            data[4] = screwData.cycleTime.ToString();
            data[5] = screwData.DisplacementValue.ToString();
            data[6] = screwData.State ? "OK" : "NG";

            RefreshDifferentThreadUI(D_ScrewResultsShow, () =>
            {
                textBox28.Text = Guid.NewGuid().ToString();
                D_ScrewResultsShow.Rows.Insert(0, data);
                D_ScrewResultsShow.Rows[0].Cells[2].Style.ForeColor = screwData.State ? Color.LimeGreen : Color.Red;
                D_ScrewResultsShow.Rows[0].Cells[3].Style.ForeColor = screwData.State ? Color.LimeGreen : Color.Red;
                D_ScrewResultsShow.Rows[0].Cells[6].Style.BackColor = screwData.State ? Color.LimeGreen : Color.Red;
                D_ScrewResultsShow.Refresh();
                D_ScrewResultsShow.Update();
                SaveProductData();
            });
        }

        public Fanuc_RobotControl F_Robot = new Fanuc_RobotControl();
        public AlarmClass.AlarmDataClass AlarmContent = new AlarmClass.AlarmDataClass();

        public double offSetX;

        public double offSetY;

        public double offSetA;

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

        private void btnConnect_Click(object sender, EventArgs e)
        {
            F_Robot.RobotIP = GetSettingValue("PSet", "RobotIP");
            btnConnect.BackColor = F_Robot.ConnectToRobot() ? Color.LimeGreen : Color.Red;
        }

        private void btnDisConnect_Click(object sender, EventArgs e)
        {
            if (F_Robot.Robot.Disconnect())
            {
                btnConnect.BackColor = Color.Transparent;
            }
        }

        private void B_ReadR_Click(object sender, EventArgs e)
        {
            if (F_Robot.ReflashRobotR())
            {
                int RNum = Convert.ToInt32(N_ReadRAdress.Value);
                T_ReadData.Text = F_Robot.D_ReadRobotR[RNum].ToString();
            }

        }

        private void B_WriteR_Click(object sender, EventArgs e)
        {
            if (F_Robot.WriteToRobotR(Convert.ToInt32(N_WriteRAdress.Value), Convert.ToDouble(N_WriteData.Value)))
            {
                MessageBox.Show("Write Finish");
            }
            else
            {
                MessageBox.Show("Write Failed");
            }
        }

        private void B_WritePR_Click(object sender, EventArgs e)
        {
            double[] d = { 1, 2, 3, 4, 5, 6 };
            d[0] = Convert.ToDouble(N_WritePR1.Value);
            d[1] = Convert.ToDouble(N_WritePR2.Value);
            d[2] = Convert.ToDouble(N_WritePR3.Value);
            d[3] = Convert.ToDouble(N_WritePR4.Value);
            d[4] = Convert.ToDouble(N_WritePR5.Value);
            d[5] = Convert.ToDouble(N_WritePR6.Value);
            if (F_Robot.WriteToRobotPR(Convert.ToInt16(N_WritePRAdress.Value), d))
            {
                MessageBox.Show("Write Finish");
            }
            else
            {
                MessageBox.Show("Write Failed");
            }
        }

        private void B_ReadPR_Click(object sender, EventArgs e)
        {
            int PRNum = Convert.ToInt16(N_ReadPRAddress.Value);
            F_Robot.ReadFromRobotPR(PRNum);
            T_ReadPRX.Text = F_Robot.D_ReadRobotPR[PRNum, 0].ToString();
            T_ReadPRY.Text = F_Robot.D_ReadRobotPR[PRNum, 1].ToString();
            T_ReadPRZ.Text = F_Robot.D_ReadRobotPR[PRNum, 2].ToString();
            T_ReadPRA.Text = F_Robot.D_ReadRobotPR[PRNum, 3].ToString();
            T_ReadPRB.Text = F_Robot.D_ReadRobotPR[PRNum, 4].ToString();
            T_ReadPRC.Text = F_Robot.D_ReadRobotPR[PRNum, 5].ToString();
        }
        #region JackingAxis

        public struct JackingAxisPos
        {
            public string Index;
            public double Position;
            public string Remark;
        }

        /// <summary>
        /// Manual Move Axis bool
        /// </summary>
        public bool B_AxisManualMove = false;

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure Add Points ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataTable D_dt = RecipeData.Tables["T_JackingShaft"];
            int I_Rows = D_dt.Rows.Count;
            DataRow D_dr = D_dt.NewRow();
            D_dr[0] = (I_Rows + 1);
            D_dr[1] = MTR_Jacking.GetCommandPosition().ToString("F3");
            D_dr[2] = "";
            D_dt.Rows.Add(D_dr);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure delete Points ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataGridView D_dgv = D_PcbPoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                DataTable D_dt = RecipeData.Tables["T_JackingShaft"];
                D_dt.Rows.RemoveAt(D_dgv.CurrentRow.Index);
                //LogShow(SysPara.UserName + "  " + "delete stress point", true);
            }
            else
            {
                MessageBox.Show("faied, datasheet have no data");
                //LogShow(SysPara.UserName + "  " + "delete faied, datasheet have no data", false);
            }
        }

        private void btnRepalce_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure Replace Points ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataGridView D_dgv = D_PcbPoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                DataTable D_dt = RecipeData.Tables["T_JackingShaft"];
                DataRow D_dr = D_dt.Rows[D_dgv.CurrentRow.Index];
                D_dr[0] = D_dr[0];
                D_dr[1] = MTR_Jacking.GetCommandPosition().ToString("F3");
                D_dr[2] = "";// AxisY.GetCommandPosition().ToString("F3");
                             //LogShow(SysPara.UserName + "  " + "StressPos datagridview Row " + D_dr[0] + " replace point " + "X:" + D_dr[1] + " " + "Y:" + D_dr[2] + " " + "Z:" + D_dr[3], true);
            }
            else
            {
                MessageBox.Show("faied, datasheet have no data");
                // LogShow(SysPara.UserName + "  " + "replace faied, datasheet have no data", false);
            }
        }

        public void SetAxisSpeed()
        {
            MTR_Jacking.WorkSpeed = GetSettingValue("MSet", "WorkSpeedAxisJacking");
            MTR_Jacking.Acceleration = GetSettingValue("MSet", "AccelerationAxisJacking");
            MTR_Jacking.Deceleration = GetSettingValue("MSet", "DecelerationAxisJacking");
            MTR_Jacking.SpeedRatio = GetSettingValue("MSet", "AxisSpeedRatio");
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Confirm to move to the selected point?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataGridView D_dgv = D_PcbPoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                SetAxisSpeed();
                JackingAxisPos axisPos = GetJackingPointList(D_dgv.CurrentRow.Cells[2].Value.ToString());
                B_AxisManualMove = false;
                while (!B_AxisManualMove)
                {
                    if (MTR_Jacking.Goto(axisPos.Position))
                    {
                        B_AxisManualMove = true;
                        J_AutoRun.Restart();
                        MessageBox.Show("Move Finsh", "", MessageBoxButtons.OK);
                        //LogShow(SysPara.UserName + "  " + "X:" + axisPos.x + " " + "Y:" + axisPos.y + " " + "Z:" + axisPos.z, true);
                    }
                    if (J_AutoRun.IsOn(50000))
                    {
                        B_AxisManualMove = true;
                        J_AutoRun.Restart();
                        //LogShow(SysPara.UserName + "  " + "X:" + D_dr[1] + " " + "Y:" + D_dr[2] + " " + "Z:" + D_dr[3], false);
                        JSDK.Alarm.Show("0205");
                    }
                    Thread.Sleep(100);
                    Application.DoEvents();
                }
            }
            else
            {
                MessageBox.Show("faied, datasheet have no data");
            }
        }

        // Relevant point data
        public static List<JackingAxisPos> JackingAxisPosData = new List<JackingAxisPos>();


        /// <summary>
        /// Get data for a point
        /// </summary>
        /// <param name="pointName">Point name</param>
        /// <returns></returns>
        public JackingAxisPos GetJackingPointList(string pointName)
        {
            LoadJackingPointList();
            JackingAxisPos modelHPostData = new JackingAxisPos();
            for (int i = 0; i < JackingAxisPosData.Count; i++)
            {
                if (JackingAxisPosData[i].Remark == pointName)
                {
                    modelHPostData = JackingAxisPosData[i];
                }
            }
            return modelHPostData;
        }


        /// <summary>
        /// Data in a table in RecipeData
        /// </summary>
        public void LoadJackingPointList()
        {
            JackingAxisPosData.Clear();
            System.Data.DataTable H1dt = RecipeData.Tables["T_JackingShaft"];
            for (int i = 0; i < H1dt.Rows.Count; i++)
            {
                System.Data.DataRow dr = H1dt.Rows[i];
                JackingAxisPos ToSolderPoint = new JackingAxisPos
                {

                    Index = dr["Index"].ToString(),
                    Position = Convert.ToDouble(dr["AxisJacking"]),
                    Remark = dr["Annotation"].ToString()
                };
                JackingAxisPosData.Add(ToSolderPoint);
            }
        }
        #endregion

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            lbSpeedRatio.Text = trackBar1.Value.ToString();
        }

        private FCResultType flowChart1_FlowRun(object sender, EventArgs e)
        {
            J_Initial.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart1.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_FlowRun(object sender, EventArgs e)
        {
            F_Robot.RobotIP = GetSettingValue("PSet", "RobotIP");
            if (F_Robot.ConnectToRobot())
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2.Text} finish", true);
                return FCResultType.NEXT;
            }
            J_Initial.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2.Text} overtime", false);
            MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart2.Text);
            GantryAlarm("4201", flowChartMessage1);
            return FCResultType.CASE2;
        }

        private FCResultType flowChart3_FlowRun(object sender, EventArgs e)
        {
            RobotStart();
            J_Initial.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart3.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart29_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRecipe(1))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart29.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart67_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRobotMode(1))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart67.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart77_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun)
            {
                if (F_Robot.SetDryRun(1))
                {
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart77.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            else
            {
                if (F_Robot.SetDryRun(0))
                {
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart77.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart8_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.ReflashRobotR())
            {
                if (F_Robot.d_ReadRobotR[35] == 1)
                {
                    B_VaccumON = true;
                }
                else
                {
                    B_VaccumON = false;
                }
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart8.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart6_FlowRun(object sender, EventArgs e)
        {
            if (B_VaccumON)
            {
                F_Robot.SetTaskIndex(2);
            }
            else
            {
                F_Robot.SetTaskIndex(1);
            }

            if (F_Robot.SetRobotTask(1))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        AlarmDataClass alarmDataClass = default(AlarmDataClass);

        /// <summary>
        /// RobotAlarm
        /// </summary>
        /// <param name="flow">FlowChartMessage</param>
        /// <param name="title">title</param>
        /// <param name="content">content</param>
        public void RobotAlarm(FlowChartMessage flow = null, bool btnSkipEnable = true)
        {
            try
            {
                if (flow != null)
                {
                    flow.msgForm.btnSkip.Enabled = btnSkipEnable;
                }
                switch (F_Robot.GetAlarmID())
                {
                    case -1:
                        JSDK.Alarm.Show("4033");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4033", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -2:
                        JSDK.Alarm.Show("4034");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4034", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -3:
                        JSDK.Alarm.Show("4035");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4035", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -4:
                        JSDK.Alarm.Show("4036");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4036", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -5:
                        JSDK.Alarm.Show("4037");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4037", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -6:
                        JSDK.Alarm.Show("4038");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4038", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -7:
                        JSDK.Alarm.Show("4039");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4039", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -8:
                        JSDK.Alarm.Show("4040");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4040", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -9:
                        JSDK.Alarm.Show("4041");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4041", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -10:
                        JSDK.Alarm.Show("4042");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4042", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -11:
                        JSDK.Alarm.Show("4043");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4043", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -12:
                        JSDK.Alarm.Show("4044");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4044", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -13:
                        JSDK.Alarm.Show("4045");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4045", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -14:
                        JSDK.Alarm.Show("4046");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4046", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -15:
                        JSDK.Alarm.Show("4047");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4047", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -16:
                        JSDK.Alarm.Show("4048");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4048", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -17:
                        JSDK.Alarm.Show("4049");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4049", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -18:
                        JSDK.Alarm.Show("4050");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4050", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -19:
                        JSDK.Alarm.Show("4051");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4051", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    case -20:
                        JSDK.Alarm.Show("4052");
                        if (flow != null && JSDK.Alarm.IsExistInSummary("4052", ref alarmDataClass))
                        {
                            flow.Title = "CoverAssembly Module";
                            flow.Content = alarmDataClass.Content;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private FCResultType flowChart7_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart7.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_Initial.Restart();
                RobotAlarm(Alarm1_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart7.Text);
                return FCResultType.CASE2;
            }
            if (J_Initial.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart7.Text} alarm", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart7.Text);
                RefreshDifferentThreadUI(flowChartMessage7, () =>
                {
                    flowChartMessage7.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage7.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4202", flowChartMessage7);
                return FCResultType.CASE3;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart75_FlowRun(object sender, EventArgs e)
        {
            bInitialOk = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart75.Text} finish", true);
            return FCResultType.IDLE;
        }

        private FCResultType flowChart5_FlowRun(object sender, EventArgs e)
        {
            SetAxisSpeed();
            int speed = GetSettingValue("MSet", "RobotSpeedRatio");
            F_Robot.SetSpeed(speed);
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart5.Text} finish", true);
            if (B_VaccumON)
            {
                B_VaccumON = false;
                return FCResultType.CASE1;
            }
            return FCResultType.NEXT;
        }

        private FCResultType flowChart53_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "DryRun Mode: " + $"{this.Text} Module {flowChart53.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (IB_CoverFeeder_Ready.IsOn())
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart53.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart13_FlowRun(object sender, EventArgs e)
        {
            B_Feeder_PickFlow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart13.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart10_FlowRun(object sender, EventArgs e)
        {
            if (!B_Feeder_PickFlow)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart10.Text} finish", true);
                if (IB_CoverFeeder_LackMaterial.IsOn() /*&& (IB_Buffer1_SensorON.IsOff() || IB_Buffer2_SensorON.IsOff())*/)
                {
                    return FCResultType.CASE1;
                }
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        public DateTime TimeStart;
        public String CT;

        private FCResultType flowChart25_FlowRun(object sender, EventArgs e)
        {
            if (MiddleLayer.ConveyorF.ConveyorBStation2Robot2Start || B_Start)
            {
                TimeStart = DateTime.Now;
                B_Start = false;
                MiddleLayer.ConveyorF.ConveyorBStation2Robot2Start = false;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart25.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart30_FlowRun(object sender, EventArgs e)
        {
            B_Assembly_PlaceFlow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart30.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart12_FlowRun(object sender, EventArgs e)
        {
            if (!B_Assembly_PlaceFlow)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart12.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart9_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.ConveyorF.ConveyorBStation2Robot2Comp = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart9.Text} finish", true);
            J_AutoRun.Restart();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart15_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(1) && F_Robot.SetRobotTask(3))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart15.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart18_FlowRun(object sender, EventArgs e)
        {
            if (B_Feeder_PickFlow)
            {
                offSetX = 0;
                offSetY = 0;
                offSetA = 0;
                feeder_Snap_index = 1;
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart18.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart19_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(feeder_Pick_index == 1 ? feeder_Snap_index * feeder_Pick_index : feeder_Snap_index + feeder_Pick_index))
            {
                if (F_Robot.SetRobotTask(3))
                {
                    J_AutoRun.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart19.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart20_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
                {
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart20.Text} finish", true);
                    return FCResultType.NEXT;
                }
                if (F_Robot.ReflashRobotR())
                {
                    if (workplace_Snap_index == 1)
                    {
                        RobotX1 = F_Robot.d_ReadRobotR[41];
                        RobotY1 = F_Robot.d_ReadRobotR[42];
                    }
                    else
                    {
                        RobotX2 = F_Robot.d_ReadRobotR[41];
                        RobotY2 = F_Robot.d_ReadRobotR[42];
                    }
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart20.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm2_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart20.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart20.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart20.Text);
                RefreshDifferentThreadUI(flowChartMessage11, () =>
                {
                    flowChartMessage11.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage11.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage11);
                return FCResultType.CASE3;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart21_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.On();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart21.Text} finish", true);
            return FCResultType.NEXT;
        }

        public ICogImage imageSnap;
        public ICogRecord imageShow;
        public Dictionary<string, object> D_result = new Dictionary<string, object>();

        bool B_Snap = false;
        int I_SnapNGtimes = 0;
        bool B_VisionOK = false;
        bool B_Feeder_TheSecondCover_VisonNG = false;

        private FCResultType flowChart22_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                DelayMs(500);
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "DryRun Mode: " + $"{this.Text} Module {flowChart22.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (feeder_Snap_index == 1 || feeder_Snap_index == 3)
            {
                RefreshDifferentThreadUI(Vision_CoverPickTask1, () =>
                {
                    if (Vision_CoverPickTask1.Snap(out imageSnap))
                    {
                        B_Snap = true;
                    }
                    else
                    {
                        B_Snap = false;
                    }
                });
            }
            else
            {
                RefreshDifferentThreadUI(Vision_CoverPickTask2, () =>
                {
                    if (Vision_CoverPickTask2.Snap(out imageSnap))
                    {
                        B_Snap = true;
                    }
                    else
                    {
                        B_Snap = false;
                    }
                });
            }
            if (B_Snap)
            {
                I_SnapNGtimes = 0;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart22.Text} finish", true);
                return FCResultType.NEXT;
            }
            I_SnapNGtimes++;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart22.Text} finish", true);
            return FCResultType.CASE2;
        }

        private FCResultType flowChart36_FlowRun(object sender, EventArgs e)
        {
            if (I_SnapNGtimes >= 3)
            {
                I_SnapNGtimes = 0;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart36.Text} CCD Snap NG", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart36.Text);
                GantryAlarm("8002", flowChartMessage22);
                return FCResultType.CASE2;
            }
            return FCResultType.CASE1;
        }

        private FCResultType flowChart23_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.Off();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart23.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart24_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                DelayMs(400);
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "DryRun Mode: " + $"{this.Text} Module {flowChart24.Text} finish", true);
                feeder_Snap_index++;
                if (feeder_Snap_index > 2)
                {
                    feeder_Snap_index = 1;
                    return FCResultType.NEXT;
                }
                return FCResultType.CASE1;
            }
            if (feeder_Pick_index == 1)
            {
                RefreshDifferentThreadUI(Vision_CoverPickTask1, () =>
                {
                    if (Vision_CoverPickTask1.AutoRun(out D_result, out imageShow))
                    {
                        RefreshDifferentThreadUI(MiddleLayer.RecordF.Cog_CoverAssembly1, () =>
                        {
                            MiddleLayer.RecordF.Cog_CoverAssembly1.Record = imageShow;
                            MiddleLayer.RecordF.Cog_CoverAssembly1.AutoFit = true;
                            MiddleLayer.RecordF.Cog_CoverAssembly1.Fit();
                        });
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }
                });
            }
            else
            {
                RefreshDifferentThreadUI(Vision_CoverPickTask2, () =>
                {
                    Vision_CoverPickTask2.SetInput("X1", Convert.ToDouble(D_result["X"]) + RobotX2 - RobotX1);
                    Vision_CoverPickTask2.SetInput("Y1", Convert.ToDouble(D_result["Y"]) + RobotY2 - RobotY1);
                    if (Vision_CoverPickTask2.RunTopCamera(out PickMark, out imageShow))
                    {
                        offSetX = Math.Round(PickMark.x, 3);
                        offSetY = Math.Round(PickMark.y, 3);
                        offSetA = Math.Round(PickMark.r, 3);
                        RefreshDifferentThreadUI(MiddleLayer.RecordF.Cog_CoverAssembly1, () =>
                        {
                            MiddleLayer.RecordF.Cog_CoverAssembly1.Record = imageShow;
                            MiddleLayer.RecordF.Cog_CoverAssembly1.AutoFit = true;
                            MiddleLayer.RecordF.Cog_CoverAssembly1.Fit();
                        });
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }
                });
            }
            if (B_VisionOK)
            {
                if (CheckVision_Limit())
                {
                    feeder_Snap_index++;
                    if (feeder_Snap_index > 2)
                    {
                        feeder_Snap_index = 1;
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart24.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart24.Text} finish", true);
                    return FCResultType.CASE1;
                }
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart24.Text} offset out range", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart24.Text);
                GantryAlarm("8004", flowChartMessage25, false);
                return FCResultType.CASE2;
            }
            else
            {
                feeder_Snap_index = 1;
                feeder_Pick_index++;
                if (feeder_Pick_index > 2)
                {
                    feeder_Pick_index = 1;
                    B_Feeder_TheSecondCover_VisonNG = true;
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart24.Text} finish", true);
                    return FCResultType.NEXT;
                }
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart24.Text} finish", true);
                return FCResultType.CASE3;
            }
        }

        public bool CheckVision_Limit()
        {
            bool a = offSetX >= offSetX_Min && offSetX <= offSetX_Max ? true : false;
            bool b = offSetY >= offSetY_Min && offSetY <= offSetY_Max ? true : false;
            bool c = offSetA >= offSetA_Min && offSetA <= offSetA_Max ? true : false;
            return a && b && c ? true : false;
        }

        private FCResultType flowChart38_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart38.Text} finish", true);
            return FCResultType.CASE1;
        }

        private FCResultType flowChart26_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                offSetX = 0;
                offSetY = 0;
                offSetA = 0;
            }

            if (F_Robot.SetTaskIndex(feeder_Pick_index))
            {
                if (F_Robot.SetVisionResultX(offSetX) && F_Robot.SetVisionResultY(offSetY) && F_Robot.SetVisionResultA(offSetA))
                {
                    if (F_Robot.SetRobotTask(4))
                    {
                        J_AutoRun.Restart();
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart26.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart28_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart28.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm5_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart28.Text);
                return FCResultType.CASE2;
            }

            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart28.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart28.Text);
                RefreshDifferentThreadUI(flowChartMessage15, () =>
                {
                    flowChartMessage15.msgForm.btnRetry.Text = "Continue Wait";
                });
                GantryAlarm("4203", flowChartMessage15, false);
                return FCResultType.CASE3;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart35_FlowRun(object sender, EventArgs e)
        {
            feeder_Pick_index++;
            if (feeder_Pick_index > 2)
            {
                feeder_Pick_index = 1;
            }
            B_Feeder_PickFlow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart35.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart37_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart37.Text} finish", true);
            return FCResultType.NEXT;
        }



        private FCResultType flowChart41_FlowRun(object sender, EventArgs e)
        {
            if (B_Assembly_PlaceFlow)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart41.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart42_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(workplace_Snap_index))
            {
                if (F_Robot.SetRobotTask(5))
                {
                    J_AutoRun.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart42.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart43_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
                {
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart43.Text} finish", true);
                    return FCResultType.NEXT;
                }
                if (F_Robot.ReflashRobotR())
                {
                    if (workplace_Snap_index == 1)
                    {
                        RobotX1 = F_Robot.d_ReadRobotR[41];
                        RobotY1 = F_Robot.d_ReadRobotR[42];
                    }
                    else
                    {
                        RobotX2 = F_Robot.d_ReadRobotR[41];
                        RobotY2 = F_Robot.d_ReadRobotR[42];
                    }
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart43.Text} finish", true);
                    return FCResultType.NEXT;
                }
            }

            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm3_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart43.Text);
                return FCResultType.CASE2;
            }

            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart43.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart43.Text);
                RefreshDifferentThreadUI(flowChartMessage13, () =>
                {
                    flowChartMessage13.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage13.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage13);
                return FCResultType.CASE3;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart44_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.On();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart44.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart45_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                DelayMs(400);
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "DryRun Mode: " + $"{this.Text} Module {flowChart45.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (workplace_Snap_index == 1)
            {
                RefreshDifferentThreadUI(Vision_CoverPlaceTask1, () =>
                {
                    if (Vision_CoverPlaceTask1.Snap(out imageSnap))
                    {
                        B_Snap = true;
                    }
                    else
                    {
                        B_Snap = false;
                    }
                });
            }
            else
            {
                RefreshDifferentThreadUI(Vision_CoverPlaceTask2, () =>
                {
                    if (Vision_CoverPlaceTask2.Snap(out imageSnap))
                    {
                        B_Snap = true;
                    }
                    else
                    {
                        B_Snap = false;
                    }
                });
            }
            if (B_Snap)
            {
                I_SnapNGtimes = 0;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart45.Text} finish", true);
                return FCResultType.NEXT;
            }
            I_SnapNGtimes++;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart45.Text} finish", true);
            return FCResultType.CASE2;
        }

        private FCResultType flowChart47_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.Off();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart47.Text} finish", true);
            return FCResultType.NEXT;
        }

        VppComp.Point3D PickMark = null;
        VppComp.Point3D PlaceMark = null;
        Dictionary<string, object> placemake1 = new Dictionary<string, object>();
        VppComp.Point3D placemake2 = new VppComp.Point3D();
        object place1 = null;
        object place2 = null;

        private FCResultType flowChart48_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                workplace_Snap_index++;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "DryRun Mode: " + $"{this.Text} Module {flowChart48.Text} finish", true);
                if (workplace_Snap_index > 2)
                {
                    workplace_Snap_index = 1;
                    return FCResultType.NEXT;
                }
                return FCResultType.CASE1;
            }

            CogRecordDisplay crd = MiddleLayer.RecordF.Cog_CoverAssembly2;
            if (workplace_Snap_index == 1)
            {
                RefreshDifferentThreadUI(Vision_CoverPlaceTask1, () =>
                {

                    if (Vision_CoverPlaceTask1.Run(out place1, ref crd))
                    {
                        placemake1 = (Dictionary<string, object>)place1;
                        RefreshDifferentThreadUI(MiddleLayer.RecordF.Cog_CoverAssembly2, () =>
                        {
                            MiddleLayer.RecordF.Cog_CoverAssembly2.Record = imageShow;
                            MiddleLayer.RecordF.Cog_CoverAssembly2.AutoFit = true;
                            MiddleLayer.RecordF.Cog_CoverAssembly2.Fit();
                        });
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }
                });
                MiddleLayer.RecordF.Cog_CoverAssembly2 = crd;
            }
            else
            {
                RefreshDifferentThreadUI(Vision_CoverPlaceTask2, () =>
                {
                    Vision_CoverPlaceTask2.SetInput("X1", (double)placemake1["X"] + RobotX2 - RobotX1);
                    Vision_CoverPlaceTask2.SetInput("Y1", (double)placemake1["Y"] + RobotY2 - RobotY1);
                    if (Vision_CoverPlaceTask2.Run(out place2, ref crd))
                    {
                        placemake2 = (VppComp.Point3D)place2;
                        offSetX = Math.Round(placemake2.x, 2);
                        offSetY = Math.Round(placemake2.y, 2);
                        offSetA = Math.Round(placemake2.r, 2);
                        RefreshDifferentThreadUI(MiddleLayer.RecordF.Cog_CoverAssembly2, () =>
                        {
                            MiddleLayer.RecordF.Cog_CoverAssembly2.Record = imageShow;
                            MiddleLayer.RecordF.Cog_CoverAssembly2.AutoFit = true;
                            MiddleLayer.RecordF.Cog_CoverAssembly2.Fit();
                        });
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }

                });
                MiddleLayer.RecordF.Cog_CoverAssembly2 = crd;
            }
            if (B_VisionOK)
            {
                if (CheckVision_Limit())
                {
                    workplace_Snap_index++;
                    if (workplace_Snap_index > 2)
                    {
                        workplace_Snap_index = 1;
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart48.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart48.Text} finish", true);
                    return FCResultType.CASE1;
                }
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart48.Text} offset out range ", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart48.Text);
                GantryAlarm("8004", flowChartMessage26, false);
                return FCResultType.CASE2;
            }
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart48.Text} Vision vpp run error ", false);
            MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart48.Text);
            GantryAlarm("8003", flowChartMessage24, false);
            return FCResultType.CASE3;
        }

        private FCResultType flowChart50_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                offSetX = 0;
                offSetY = 0;
                offSetA = 0;
            }
            if (F_Robot.SetTaskIndex(1))
            {
                if (F_Robot.SetVisionResultX(offSetX) && F_Robot.SetVisionResultY(offSetY) && F_Robot.SetVisionResultA(offSetA))
                {
                    if (F_Robot.SetRobotTask(6))
                    {
                        J_AutoRun.Restart();
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart50.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart52_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart52.Text} finish", true);
                return FCResultType.NEXT;
            }

            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm6_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart52.Text);
                return FCResultType.CASE2;
            }

            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart52.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart52.Text);
                RefreshDifferentThreadUI(flowChartMessage19, () =>
                {
                    flowChartMessage19.msgForm.btnRetry.Text = "Continue Wait";
                });
                GantryAlarm("4203", flowChartMessage19, false);
                return FCResultType.CASE3;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart58_FlowRun(object sender, EventArgs e)
        {
            B_Assembly_PlaceFlow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart58.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart59_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart59.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart49_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart49.Text} finish", true);
            return FCResultType.CASE1;
        }

        private FCResultType flowChart11_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart11.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(flowChartMessage14, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart11.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart11.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart11.Text);
                RefreshDifferentThreadUI(flowChartMessage6, () =>
                {
                    flowChartMessage6.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage6.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage6);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart32_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart32.Text} finish", true);
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.NEXT;
        }

        INIHelper ProductionIni;
        public void SaveProductData()
        {
            try
            {
                string ProductionPath = $"{System.IO.Directory.GetCurrentDirectory()}\\ProductionData\\{this.Text} module.ini";
                ProductionIni = new INIHelper(ProductionPath);
                ProductionIni.WriteIniFile("ProductData", "OK", textBox29.Text);
                ProductionIni.WriteIniFile("ProductData", "NG", textBox30.Text);
            }
            catch (Exception ex)
            {
            }
        }

        public void ReadProductData()
        {
            try
            {
                string ProductionPath = $"{System.IO.Directory.GetCurrentDirectory()}\\ProductionData\\{this.Text} module.ini";
                if (File.Exists(ProductionPath))
                {
                    textBox29.Text = ProductionIni.ReadIniFile("ProductData", "OK", textBox29.Text);
                    textBox30.Text = ProductionIni.ReadIniFile("ProductData", "NG", textBox30.Text);
                }
            }
            catch (Exception ex)
            {

            }
        }

        bool bPick = false;
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure to start auto 9-points calibration ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

            if (reult == DialogResult.No)
            {
                return;
            }
            DialogResult reult1 = MessageBox.Show("Check robot flange app have no product ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult1 == DialogResult.No)
            {
                return;
            }
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    bPick = true;
                    break;
                case 1:
                    bPick = false;
                    break;
            }
            calibration_Index = 1;
            B_Auto_NinePoint_Calibration = true;
            flowChart101.TaskReset();
            Task.Run(() =>
            {
                while (B_Auto_NinePoint_Calibration)
                {
                    flowChart101.TaskRun();
                }
            });
            button4.Enabled = false;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            calibration_Index = 1;
            B_Auto_NinePoint_Calibration = false;
            button4.Enabled = true;
        }

        private FCResultType flowChart101_FlowRun(object sender, EventArgs e)
        {
            calibration_Index = 1;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart16_FlowRun(object sender, EventArgs e)
        {
            F_Robot.RobotIP = GetSettingValue("PSet", "RobotIP");
            if (F_Robot.ConnectToRobot())
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart17_FlowRun(object sender, EventArgs e)
        {
            if (RobotStart())
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart27_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRecipe(1))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart76_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRobotMode(1))
            {
                F_Robot.SetSpeed(5);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart33_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRobotTask(1))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart34_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart51_FlowRun(object sender, EventArgs e)
        {
            if (bPick)
            {
                if (F_Robot.WriteToRobotR(2, calibration_Index) && F_Robot.WriteToRobotR(1, 10))
                {
                    return FCResultType.NEXT;
                }
                return FCResultType.IDLE;
            }
            else
            {
                if (F_Robot.WriteToRobotR(2, calibration_Index) && F_Robot.WriteToRobotR(1, 11))
                {
                    return FCResultType.NEXT;
                }
                return FCResultType.IDLE;
            }
        }

        private FCResultType flowChart54_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart55_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.On();
            return FCResultType.NEXT;
        }

        private FCResultType flowChart57_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.Off();
            return FCResultType.NEXT;
        }

        Cognex.VisionPro.ICogRecord image;
        bool result = false;

        private FCResultType flowChart60_FlowRun(object sender, EventArgs e)
        {
            if (bPick)
            {
                RefreshDifferentThreadUI(Vision_Calibration, () =>
                {
                    if (Vision_Calibration.SetCalibPoint(Vision_CoverPickTask2.CameraSN, Vision_CoverPickTask2.WokId, calibration_Index - 1, RobotX, RobotY, out image))
                    {
                        RefreshDifferentThreadUI(cog_Auto9PointCalibration, () =>
                        {
                            cog_Auto9PointCalibration.Record = image;
                            cog_Auto9PointCalibration.AutoFit = true;
                            cog_Auto9PointCalibration.Fit();
                        });
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                });
            }
            else
            {
                RefreshDifferentThreadUI(Vision_Calibration, () =>
                {
                    if (Vision_Calibration.SetCalibPoint(Vision_CoverPlaceTask2.CameraSN, Vision_CoverPlaceTask2.WokId, calibration_Index - 1, RobotX, RobotY, out image))
                    {
                        RefreshDifferentThreadUI(cog_Auto9PointCalibration, () =>
                        {
                            cog_Auto9PointCalibration.Record = image;
                            cog_Auto9PointCalibration.AutoFit = true;
                            cog_Auto9PointCalibration.Fit();
                        });
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                });
            }
            if (result)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
            //DelayMs(1000);
            //return FCResultType.NEXT;
        }

        private FCResultType flowChart62_FlowRun(object sender, EventArgs e)
        {
            calibration_Index++;
            if (calibration_Index >= 10)
            {
                calibration_Index = 1;
                return FCResultType.NEXT;
            }
            return FCResultType.CASE1;
        }

        private FCResultType flowChart63_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart64_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(1) && F_Robot.SetRobotTask(2))
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart65_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart66_FlowRun(object sender, EventArgs e)
        {
            B_Auto_NinePoint_Calibration = false;
            button4.Enabled = true;
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetSpeed(5))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        bool B_Start = false;
        private void button5_Click(object sender, EventArgs e)
        {
            F_Robot.RobotContinue(OB_Robot_Maintain, OB_Robot_Start);
        }

        private FCResultType flowChart31_FlowRun(object sender, EventArgs e)
        {
            B_VisionPick_Flow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart31.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart61_FlowRun(object sender, EventArgs e)
        {
            if (!B_VisionPick_Flow)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart61.Text} finish", true);
                if (B_Feeder_TheSecondCover_VisonNG)
                {
                    B_Feeder_TheSecondCover_VisonNG = false;
                    return FCResultType.CASE3;
                }
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart73_FlowRun(object sender, EventArgs e)
        {
            B_VisionPlace_Flow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart73.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart74_FlowRun(object sender, EventArgs e)
        {
            if (!B_VisionPlace_Flow)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart74.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart78_FlowRun(object sender, EventArgs e)
        {
            if (B_VisionPick_Flow)
            {
                B_Process = true;
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart78.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart81_FlowRun(object sender, EventArgs e)
        {
            B_VisionPick_Flow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart81.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart82_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart82.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart83_FlowRun(object sender, EventArgs e)
        {
            if (B_VisionPlace_Flow)
            {
                B_Process = true;
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart83.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart84_FlowRun(object sender, EventArgs e)
        {
            B_VisionPlace_Flow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart84.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart85_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart85.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart79_FlowRun(object sender, EventArgs e)
        {
            if (!B_ScrewFlow)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart79.Text} finish", true);
                I_ScrewCount++;
                if (I_ScrewCount > 2 || !B_Process)
                {
                    I_ScrewCount = 1;
                    return FCResultType.NEXT;
                }
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart80_FlowRun(object sender, EventArgs e)
        {
            JackingAxisPos AxisZ = GetJackingPointList("SafePos");
            bool bFinsh = MTR_Jacking.Goto(AxisZ.Position);
            if (bFinsh)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart80.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "AxisTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart80.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart80.Text);
                GantryAlarm("3041", flowChartMessage4, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }


        private FCResultType flowChart86_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart69_FlowRun(object sender, EventArgs e)
        {
            JackingAxisPos AxisZ = GetJackingPointList("PreJackingPos");
            bool bFinsh = MTR_Jacking.Goto(AxisZ.Position);
            if (bFinsh)
            {
                J_AutoRun.Restart();
                MTR_Jacking.WorkSpeed = 1;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart69.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "AxisTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart69.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart103.Text);
                GantryAlarm("3041", flowChartMessage28, false);
            }
            return FCResultType.IDLE;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            B_Start = true;
        }

        private FCResultType flowChart94_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(1) && F_Robot.SetRobotTask(5))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart94.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart95_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart95.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(flowChartMessage10, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart95.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart95.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart95.Text);
                RefreshDifferentThreadUI(flowChartMessage9, () =>
                {
                    flowChartMessage9.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage9.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage9);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        double RobotX, RobotY, RobotX1, RobotY1, RobotX2, RobotY2;

        private FCResultType flowChart97_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart97.Text} finish", true);
            return FCResultType.CASE1;
        }

        private FCResultType flowChart98_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        bool B_CoverAssemblyFlow = false;
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Sure to Start CoverAssembly Flow ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.Cancel)
                return;
            B_CoverAssemblyFlow = true;
            feeder_Snap_index = 1;
            feeder_Pick_index = 1;
            workplace_Snap_index = 1;
            I_SnapNGtimes = 0;
            offSetX = 0;
            offSetY = 0;
            offSetA = 0;

            //F_Robot.RobotIP = GetRecipeValue("RSet", "RobotIP");
            //F_Robot.ConnectToRobot();
            RobotStart();
            F_Robot.SetSpeed(10);
            flowChart5.TaskReset();
            flowChart18.TaskReset();
            flowChart41.TaskReset();
            flowChart78.TaskReset();
            flowChart83.TaskReset();
            flowChart39.TaskReset();
            flowChart40.TaskReset();
            Task.Factory.StartNew(() =>
            {
                while (B_CoverAssemblyFlow)
                {
                    flowChart5.TaskRun();
                    flowChart18.TaskRun();
                    flowChart41.TaskRun();
                    flowChart78.TaskRun();
                    flowChart83.TaskRun();
                    flowChart39.TaskRun();
                    flowChart40.TaskRun();
                }
            });
            button3.Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            B_CoverAssemblyFlow = false;
            button3.Enabled = true;
        }

        private void btn_Gantry1Axis_Click(object sender, EventArgs e)
        {
            MotorControlForm MCF = new MotorControlForm();
            MCF.Initial(sender);
            MCF.ShowDialog();
        }

        private void T_CurrPos_Tick(object sender, EventArgs e)
        {
            if (plMotorControl.Visible)
            {
                T_McPosJacking.Text = MTR_Jacking.GetCommandPosition().ToString("F3");
            }
        }

        private void trackBar2_ValueChanged(object sender, EventArgs e)
        {
            lbSpeedRatio.Text = trackBar1.Value.ToString();
            MTR_Jacking.SpeedRatio = trackBar1.Value;
        }

        public void GantryAlarm(string AlarmCode, FlowChartMessage flow, bool btnSkipEnable = true, int index = 0)
        {
            try
            {
                JSDK.Alarm.Show(AlarmCode);
                if (flow != null && JSDK.Alarm.IsExistInSummary(AlarmCode, ref AlarmContent))
                {
                    flow.msgForm.btnSkip.Enabled = btnSkipEnable;
                    flow.Title = "CoverAssembly Module";
                    if (index != 0)
                    {
                        flow.Content = AlarmCode + "-" + AlarmContent.Content + $" (the number {index} Pressure value overlimt)";
                    }
                    else
                    {
                        flow.Content = AlarmCode + "-" + AlarmContent.Content;
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private FCResultType flowChart103_FlowRun(object sender, EventArgs e)
        {
            JackingAxisPos AxisZ = GetJackingPointList("SafePos");
            bool bFinsh = MTR_Jacking.Goto(AxisZ.Position);
            if (bFinsh)
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart103.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_Initial.IsOn(GetSettingValue("PSet", "AxisTimes")))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart103.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart103.Text);
                GantryAlarm("3041", flowChartMessage28, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart104_FlowRun(object sender, EventArgs e)
        {
            if (CYL_TransferCylinder.Off())
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart104.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_Initial.IsOn(GetSettingValue("PSet", "IOTimes")))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart104.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart104.Text);
                GantryAlarm("3045", flowChartMessage29, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_1_FlowRun(object sender, EventArgs e)
        {
            if (B_Screw_VisionFlow)
            {
                B_Process = true;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_1.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_7_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(I_ScrewCount) && F_Robot.SetRobotTask(7))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_7.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart4_2_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_2.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm4_01, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart4_2.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_2.Text} alarm", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart4_2.Text);
                RefreshDifferentThreadUI(Alarm4_02, () =>
                {
                    Alarm4_02.msgForm.btnRetry.Text = "Continue Wait";
                    Alarm4_02.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", Alarm4_02);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart109_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart110_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart14_FlowRun(object sender, EventArgs e)
        {
            if (CYL_TransferCylinder.On())
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart14.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "IOTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart14.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart14.Text);
                GantryAlarm("3044", flowChartMessage2, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart68_FlowRun(object sender, EventArgs e)
        {
            JackingAxisPos AxisZ = GetJackingPointList("JackingLimitPos");
            bool bFinsh = MTR_Jacking.Goto(AxisZ.Position);
            if (bFinsh)
            {
                J_AutoRun.Restart();
                SetAxisSpeed();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart68.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "AxisTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart68.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart68.Text);
                GantryAlarm("3041", flowChartMessage3, false);
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart70_FlowRun(object sender, EventArgs e)
        {
            B_Screw_VisionFlow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart70.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart71_FlowRun(object sender, EventArgs e)
        {
            if (!B_Screw_VisionFlow)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart71.Text} finish", true);
                if (!B_Process)
                {
                    return FCResultType.CASE2;
                }
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart72_FlowRun(object sender, EventArgs e)
        {
            B_ScrewFlow = true;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart72.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart4_3_FlowRun(object sender, EventArgs e)
        {
            OB_CCDLight.On();
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode : " + $"{this.Text} Module {flowChart4_3.Text} finish", true);
                return FCResultType.NEXT;
            }
            RefreshDifferentThreadUI(Vision_CoverPickTask1, () =>
            {
                if (Vision_CoverPickTask1.Snap(out imageSnap))
                {
                    B_Snap = true;
                }
                else
                {
                    B_Snap = false;
                }
            });
            if (B_Snap)
            {
                I_SnapNGtimes = 0;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_3.Text} finish", true);
                return FCResultType.NEXT;
            }
            I_SnapNGtimes++;
            if (I_SnapNGtimes < 3)
            {
                return FCResultType.CASE3;
            }
            I_SnapNGtimes = 0;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_3.Text} NG", false);
            MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart4_3.Text);
            GantryAlarm("3044", Alarm4_04);
            return FCResultType.CASE2;
        }

        private FCResultType flowChart106_FlowRun(object sender, EventArgs e)
        {
            B_Process = false;
            J_AutoRun.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart106.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart111_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        public bool DelayMs(int delayMilliseconds)
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

        object markPoint = null;
        VppComp.Point3D point3D;
        private FCResultType flowChart4_4_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode : " + $"{this.Text} Module {flowChart4_4.Text} finish", true);
                DelayMs(800);
                OB_CCDLight.Off();
                J_AutoRun.Restart();
                return FCResultType.NEXT;
            }
            CogRecordDisplay crd = MiddleLayer.RecordF.Cog_CoverAssembly1;
            if (I_ScrewCount == 1)
            {
                RefreshDifferentThreadUI(Vision_Screw1, () =>
                {
                    if (Vision_Screw1.Run(out markPoint, ref crd))
                    {
                        point3D = markPoint as VppComp.Point3D;
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }
                    MiddleLayer.RecordF.Cog_CoverAssembly1 = crd;
                });
            }
            else
            {
                RefreshDifferentThreadUI(Vision_Screw2, () =>
                {
                    if (Vision_Screw2.Run(out markPoint, ref crd))
                    {
                        point3D = markPoint as VppComp.Point3D;
                        B_VisionOK = true;
                    }
                    else
                    {
                        B_VisionOK = false;
                    }
                    MiddleLayer.RecordF.Cog_CoverAssembly2 = crd;
                });
            }
            if (B_VisionOK)
            {
                if (point3D.x >= offSetX_Min && point3D.x <= offSetX_Max && point3D.y >= offSetY_Min && point3D.y <= offSetY_Max)
                {
                    J_AutoRun.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_4.Text} finish", true);
                    return FCResultType.NEXT;
                }
                else
                {
                    J_AutoRun.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_4.Text} vision offset overlimit", false);
                    MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart4_4.Text);
                    GantryAlarm("8004", Alarm4_03);
                    return FCResultType.CASE2;
                }
            }
            else
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_4.Text} vision offset overlimit", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart4_4.Text);
                GantryAlarm("3045", Alarm4_03);
                return FCResultType.CASE2;
            }
        }

        private FCResultType flowChart4_6_FlowRun(object sender, EventArgs e)
        {
            B_Screw_VisionFlow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart4_6.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart107_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart108_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart112_FlowRun(object sender, EventArgs e)
        {
            I_ScrewCount = 1;
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_1_FlowRun(object sender, EventArgs e)
        {
            if (B_ScrewFlow)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_1.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        DateTime screwStartTime;
        private FCResultType flowChart2_17_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassVision)
            {
                offSetX = 0;
                offSetY = 0;
                offSetA = 0;
            }

            if (F_Robot.SetTaskIndex(I_ScrewCount) && F_Robot.SetVisionResultX(offSetX) && F_Robot.SetVisionResultX(offSetY) && F_Robot.SetVisionResultX(offSetA) && F_Robot.SetRobotTask(8))
            {
                J_AutoRun.Restart();
                screwStartTime = DateTime.Now;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_17.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart89_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart89.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(Alarm7_01);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart89.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart89.Text} alarm", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart89.Text);
                RefreshDifferentThreadUI(flowChartMessage17, () =>
                {
                    flowChartMessage17.msgForm.btnRetry.Text = "Continue Wait";
                });
                GantryAlarm("4203", flowChartMessage17, false);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart90_FlowRun(object sender, EventArgs e)
        {
            B_ThrowScrewFlow = true;
            J_AutoRun.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart90.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart91_FlowRun(object sender, EventArgs e)
        {
            if (!B_ThrowScrewFlow)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart91.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart2_10_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassScrew)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode :" + $"{this.Text} Module {flowChart2_10.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.ReflashRobotR())
            {
                if (F_Robot.D_ReadRobotR[39] == 1)
                {
                    if (F_Robot.WriteR(30, 0))
                    {
                        J_AutoRun.Restart();
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_10.Text} finish", true);
                        return FCResultType.NEXT;
                    }
                }
                if (F_Robot.D_ReadRobotR[39] == -1)
                {
                    if (F_Robot.WriteR(30, 0))
                    {
                        B_Process = false;
                        MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_10.Text} screw result NG", false);
                        JSDK.Alarm.Show("3046");
                        J_AutoRun.Restart();
                        return FCResultType.CASE2;
                    }
                }
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart2_12_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun || B_ByPassScrew)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode :" + $"{this.Text} Module {flowChart2_12.Text} finish", true);
                return FCResultType.NEXT;
            }
            B_GetScrewData = true;
            J_AutoRun.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_12.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_22_FlowRun(object sender, EventArgs e)
        {
            if (SysPara.IsDryRun || B_ModuleDryRun)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + "Dryrun Mode :" + $"{this.Text} Module {flowChart2_22.Text} finish", true);
                return FCResultType.NEXT;
            }
            string data = "";
            if (SysPara.newReadCOM.SPCom.ConnectStates())
            {
                data = SysPara.newReadCOM.ReadContent_COM("01 03 00 00 00 02 C4 0B");
                screwData.DisplacementValue = Convert.ToDouble(data);
                if ((Convert.ToDouble(data) < GetRecipeValue("RSet", "DisplacementMax") && Convert.ToDouble(data) > GetRecipeValue("RSet", "DisplacementMin")) || B_ByPassDisplacement)
                {
                    J_AutoRun.Restart();
                    MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_22.Text} finish", true);
                    return FCResultType.NEXT;
                }
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_22.Text} ---- Screw dispalcement height over limit", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart2_22.Text);
                GantryAlarm("3043", flowChartMessage20);
                return FCResultType.CASE1;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "IOTimes")))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_22.Text} overtime", false);
                SysPara.newReadCOM.DisconnectCom3();
                SysPara.newReadCOM.ConnectCom(GetSettingValue("RSet", "DisplacementCom"));
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart2_22.Text);
                GantryAlarm("3042", flowChartMessage21, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart92_FlowRun(object sender, EventArgs e)
        {
            B_Process = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart92.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart2_16_FlowRun(object sender, EventArgs e)
        {
            B_ScrewFlow = false;
            J_AutoRun.Restart();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart2_16.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart87_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart88_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.CASE1;
        }

        private FCResultType flowChart105_FlowRun(object sender, EventArgs e)
        {
            if (CYL_TransferCylinder.Off())
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart105.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "IOTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart105.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart105.Text);
                GantryAlarm("3045", flowChartMessage5, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart39_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetRobotTask(12))
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart39.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart40_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart40.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(flowChartMessage12, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart40.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart40.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart40.Text);
                RefreshDifferentThreadUI(flowChartMessage8, () =>
                {
                    flowChartMessage8.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage8.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage8);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart6_1_FlowRun(object sender, EventArgs e)
        {
            if (B_ThrowScrewFlow)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6_1.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart6_2_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.SetTaskIndex(1) && F_Robot.SetRobotTask(9))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6_2.Text} finish", true);
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart6_3_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.GetCurrentTaskState() == eRobotState.Done)
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6_3.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (F_Robot.GetCurrentTaskState() == eRobotState.Alarm)
            {
                J_AutoRun.Restart();
                RobotAlarm(flowChartMessage16, false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart6_3.Text);
                return FCResultType.CASE2;
            }
            if (J_AutoRun.IsOn(GetSettingValue("PSet", "RobotTimes")))
            {
                J_AutoRun.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6_3.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart6_3.Text);
                RefreshDifferentThreadUI(flowChartMessage18, () =>
                {
                    flowChartMessage18.msgForm.btnRetry.Text = "Continue Wait";
                    flowChartMessage18.msgForm.btnSkip.Text = "Move Again";
                });
                GantryAlarm("4203", flowChartMessage18);
                return FCResultType.CASE1;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart6_7_FlowRun(object sender, EventArgs e)
        {
            B_ThrowScrewFlow = false;
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart6_7.Text} finish", true);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart93_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart7_1_FlowRun(object sender, EventArgs e)
        {
            if (B_GetScrewData)
            {
                B_GetScrewData = false;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart7_2_FlowRun(object sender, EventArgs e)
        {
            if (PFClient.TighteningResultUpdated)
            {
                PFClient.TighteningResultUpdated = false;
                screwData.screwTime = DateTime.Now;
                screwData.screwName = I_ScrewCount;
                screwData.finalTorque = PFClient.LastTighteningResult.TORQUE;
                screwData.Angle = PFClient.LastTighteningResult.ANGLE;
                screwData.cycleTime = (DateTime.Now - screwStartTime).TotalSeconds;
                //screwData.NumbleOfTurns = 0;
                screwData.State = PFClient.LastTighteningResult.TIGHTENING_STATUS;
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart7_3_FlowRun(object sender, EventArgs e)
        {
            ScrewDataShowUI(screwData);
            return FCResultType.NEXT;
        }

        private FCResultType flowChart113_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            T_ScrewData.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            F_Robot.RobotPause(OB_Robot_Maintain);
        }

        private FCResultType flowChart114_FlowRun(object sender, EventArgs e)
        {
            PFClient.IP = GetSettingValue("RSet", "ScrewIP");
            PFClient.Port = GetSettingValue("RSet", "ScrewPort");
            BConnect();
            if (bConnect && bComStart && bSubscribe)
            {
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart114.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_Initial.IsOn(GetSettingValue("PSet", "IOTimes")))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart114.Text} overtime", false);
                JSDK.Alarm.Show("3046");
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart56_FlowRun(object sender, EventArgs e)
        {
            if (MTR_Jacking.Home())
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart56.Text} finish", true);
                return FCResultType.NEXT;
            }
            if (J_Initial.IsOn(GetSettingValue("PSet", "AxisTimes")))
            {
                J_Initial.Restart();
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart56.Text} overtime", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart56.Text);
                GantryAlarm("3040", flowChartMessage27, false);
                return FCResultType.CASE2;
            }
            return FCResultType.IDLE;
        }

        private FCResultType flowChart99_FlowRun(object sender, EventArgs e)
        {
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart99.Text} finish", true);
            return FCResultType.CASE3;
        }

        private FCResultType flowChart100_FlowRun(object sender, EventArgs e)
        {
            OB_CoverFeeder_PickFinish.On();
            OB_CoverFeeder_SafeHeight.On();
            DelayMs(200);
            OB_CoverFeeder_PickFinish.Off();
            OB_CoverFeeder_SafeHeight.Off();
            MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart100.Text} finish", true);
            return FCResultType.CASE3;
        }

        private FCResultType flowChart102_FlowRun(object sender, EventArgs e)
        {
            return FCResultType.NEXT;
        }

        private FCResultType flowChart46_FlowRun(object sender, EventArgs e)
        {
            if (I_SnapNGtimes >= 3)
            {
                I_SnapNGtimes = 0;
                MiddleLayer.RecordF.LogShow(SysPara.UserName + " " + $"{this.Text} Module {flowChart46.Text} CCD Snap NG", false);
                MiddleLayer.SystemF.ErrorDataLogShow(this.Text, flowChart46.Text);
                GantryAlarm("8002", flowChartMessage23, false);
                return FCResultType.CASE2;
            }
            return FCResultType.CASE1;
        }

        private FCResultType flowChart96_FlowRun(object sender, EventArgs e)
        {
            if (F_Robot.ReflashRobotR())
            {
                //F_Robot.SetSpeed(10);
                RobotX = F_Robot.d_ReadRobotR[41];
                RobotY = F_Robot.d_ReadRobotR[42];
                return FCResultType.NEXT;
            }
            return FCResultType.IDLE;
        }
    }
}
