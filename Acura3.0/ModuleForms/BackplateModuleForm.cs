﻿using Acura3._0.Classes;
using Acura3.New.Device;
using AcuraLibrary.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Acura3._0.ModuleForms
{
    public partial class BackplateModuleForm : ModuleBaseForm
    {
        public BackplateModuleForm()
        {
            InitializeComponent();
            FlowChartMessage.PauseRaise += FlowChartMessage_PauseRaise;
            FlowChartMessage.ResetTimerRaise += FlowChartMessage_ResetRaise;
            SetDoubleBuffer(plProductionSetting);
        }

        #region Override Method

        public override void AlwaysRun()
        {
        }

        public override void InitialReset()
        {
        }

        public override void Initial()
        {
        }

        public override void RunReset()
        {
        }

        public override void Run()
        {
        }

        public override void StartRun()
        {
            RunTM.Restart();
        }

        public override void StopRun()
        {
        }

        public override void ServoOn()
        {
        }

        public override void ServoOff()
        {
        }

        public override void SetSpeed(int SpeedRatio)
        {

        }

        #endregion

        #region Local Method
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

        #region //延时函数
        // 延时函数
        public static bool B_DelayMs(int delayMilliseconds)
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

        #region Robot
        public Fanuc_RobotControl fanuc_Robot = new Fanuc_RobotControl();
        private void B_ConnectRobot_Click(object sender, EventArgs e)
        {
            if (fanuc_Robot.ConnectToRobot())
            {
                B_ConnectResult.Text = "ConnectSuccess";
                B_ConnectResult.BackColor = Color.Green;
            }
            else
            {
                B_ConnectResult.Text = "ConnectFail";
                B_ConnectResult.BackColor = Color.Red;
            }
        }
        private void B_ReadR_Click(object sender, EventArgs e)
        {
            if (fanuc_Robot.ReflashRobotR())
            {
                int RNum = Convert.ToInt32(N_ReadRAdress.Value);
                T_ReadData.Text = fanuc_Robot.D_ReadRobotR[RNum].ToString();
            }
        }
        private void B_WritePR_Click(object sender, EventArgs e)
        {
            double[] d = { 1, 2, 3, 4, 5, 6 };
            d[0] = Convert.ToDouble(N_WritePRX.Value);
            d[1] = Convert.ToDouble(N_WritePRY.Value);
            d[2] = Convert.ToDouble(N_WritePRZ.Value);
            d[3] = Convert.ToDouble(N_WritePRA.Value);
            d[4] = Convert.ToDouble(N_WritePRB.Value);
            d[5] = Convert.ToDouble(N_WritePRC.Value);
            if (fanuc_Robot.WriteToRobotPR(Convert.ToInt16(N_WritePRAdress.Value), d))
            {
                B_WritePR.BackColor = Color.Green;
            }
            else
            {
                B_WritePR.BackColor = Color.Red;
            }
        }
        private void B_WriteR_Click(object sender, EventArgs e)
        {
            if (fanuc_Robot.WriteToRobotR(Convert.ToInt32(N_WriteRAdress.Value), Convert.ToDouble(N_WriteData.Value)))
            {
                B_WriteR.BackColor = Color.Green;
            }
            else
            {
                B_WriteR.BackColor = Color.Red;
            }
        }
        private void B_ReadPR_Click(object sender, EventArgs e)
        {
            int PRNum = Convert.ToInt16(N_ReadPRAddress.Value);
            fanuc_Robot.ReadFromRobotPR(PRNum);
            T_ReadPRX.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 0].ToString();
            T_ReadPRY.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 1].ToString();
            T_ReadPRZ.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 2].ToString();
            T_ReadPRA.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 3].ToString();
            T_ReadPRB.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 4].ToString();
            T_ReadPRC.Text = fanuc_Robot.D_ReadRobotPR[PRNum, 5].ToString();
        }
        #endregion

        #region Axis
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure Add Points ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);

            if (reult == DialogResult.No)
            {
                return;
            }
            DataTable D_dt = RecipeData.Tables["T_BackplateAxis"];
            int I_Rows = D_dt.Rows.Count;
            DataRow D_dr = D_dt.NewRow();
            D_dr[0] = (I_Rows + 1);
            D_dr[1] = "";
            D_dr[2] = "";
            D_dt.Rows.Add(D_dr);
            //LogShow(SysPara.UserName + "  " + "Add screw points the number" + D_dr[0] + "point" + "X:" + D_dr[1] + "  " + "Y:" + D_dr[2] + "  " + "Z:" + D_dr[3], true);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Sure delete Points ?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataGridView D_dgv = D_ShuttlePoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                DataTable D_dt = RecipeData.Tables["T_BackplateAxis"];
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
            DataGridView D_dgv = D_ShuttlePoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                DataTable D_dt = RecipeData.Tables["T_BackplateAxis"];
                DataRow D_dr = D_dt.Rows[D_dgv.CurrentRow.Index];
                D_dr[0] = D_dr[0];
                D_dr[1] = "";//AxisX.GetCommandPosition().ToString("F3");
                D_dr[2] = "";// AxisY.GetCommandPosition().ToString("F3");
                //LogShow(SysPara.UserName + "  " + "StressPos datagridview Row " + D_dr[0] + " replace point " + "X:" + D_dr[1] + " " + "Y:" + D_dr[2] + " " + "Z:" + D_dr[3], true);
            }
            else
            {
                MessageBox.Show("faied, datasheet have no data");
                // LogShow(SysPara.UserName + "  " + "replace faied, datasheet have no data", false);
            }
        }
        private void btnGoto_Click(object sender, EventArgs e)
        {
            DialogResult reult = MessageBox.Show("Confirm to move to the selected point?", "", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            if (reult == DialogResult.No)
            {
                return;
            }
            DataGridView D_dgv = D_ShuttlePoint;
            if ((D_dgv.CurrentRow != null) && (D_dgv.CurrentRow.Index >= 0))
            {
                AxisPos axisPos = GetPointList(D_dgv.CurrentRow.Cells[2].Value.ToString());
                double a = axisPos.Position;
            }
            else
            {
                MessageBox.Show("faied, datasheet have no data");
                //LogShow(SysPara.UserName + "  " + "delete faied, datasheet have no data", false);
            }
        }

        // Relevant point data
        public static List<AxisPos> L_FlipAxisPosData = new List<AxisPos>();

        /// <summary>
        /// Axis Pos Struct
        /// </summary>
        public struct AxisPos
        {
            public string Index;
            public double Position;
            public string Remark;
        }

        /// <summary>
        /// Get data for a point
        /// </summary>
        /// <param name="pointName">Point name</param>
        /// <returns></returns>
        public static AxisPos GetPointList(string pointName)
        {
            LoadHPointList();
            AxisPos modelHPostData = new AxisPos();
            for (int i = 0; i < L_FlipAxisPosData.Count; i++)
            {
                if (L_FlipAxisPosData[i].Remark== pointName)
                {
                    modelHPostData = L_FlipAxisPosData[i];
                }
            }
            return modelHPostData;
        }

        /// <summary>
        /// Data in a table in RecipeData
        /// </summary>
        public static void LoadHPointList()
        {
            L_FlipAxisPosData.Clear();
            System.Data.DataTable H1dt = MiddleLayer.BackplateModuleF.RecipeData.Tables["T_BackplateAxis"];
            for (int i = 0; i < H1dt.Rows.Count; i++)
            {
                System.Data.DataRow dr = H1dt.Rows[i];
                AxisPos ToSolderPoint = new AxisPos
                {

                    Index = dr["Index"].ToString(),
                    Position = Convert.ToDouble(dr["Position"]),
                    Remark = dr["Remark"].ToString()
                };
                L_FlipAxisPosData.Add(ToSolderPoint);
            }
        }

        #endregion

        public IX_1000 IX_Keyence;

        string IP => GetRecipeValue("RSet", "IX_IP");
        int Port => GetRecipeValue("RSet", "IX_Port");
        private void button2_Click(object sender, EventArgs e)
        {
            btnConnect.BackColor = ConnectIX() ? Color.LimeGreen : Color.Red;
        }
        public bool ConnectIX()
        {
            if (!IX_Keyence.Status)
            {
                return IX_Keyence.Connect();
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IX_Keyence.Disconnect())
            {
                btnConnect.BackColor = Color.Red;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string result = IX_Keyence.Trigger();
            if (result != null)
            {
                richTextBox1.Text += result + Environment.NewLine;
            }
            else
            {
                richTextBox1.Text += "Null" + Environment.NewLine;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }
    }
}