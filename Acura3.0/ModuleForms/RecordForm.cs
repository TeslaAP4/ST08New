﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Acura3._0.Classes;
using AcuraLibrary.Forms;

namespace Acura3._0.ModuleForms
{
    public partial class RecordForm : ModuleBaseForm
    {
        public RecordForm()
        {
            InitializeComponent();
        }

        public override void InitialReset()
        {

        }

        public override void Initial()
        {
            bInitialOk = true;
        }

        public override void RunReset()
        {

        }

        public override void Run()
        {

        }

        public override void ServoOn()
        {

        }

        public override void ServoOff()
        {

        }

        public override void StopRun()
        {

        }

        public override void StartRun()
        {

        }

        //private static readonly object Lock = new object();

        //public void LogShow(string Textshow, bool OK)
        //{
        //    lock (Lock)
        //    {
        //        RefreshDifferentThreadUI(R_Show, () =>
        //        {
        //            if (R_Show.TextLength > 10000)
        //            {
        //                R_Show.Clear();
        //            }
        //            if (OK == true)
        //                R_Show.SelectionColor = Color.Black;
        //            else
        //                R_Show.SelectionColor = Color.Red;
        //            R_Show.AppendText(DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss ffff") + "  " + Textshow + Environment.NewLine);
        //            R_Show.SelectionStart = R_Show.TextLength; R_Show.ScrollToCaret();
        //            SaveFile.SaveLog(Textshow);
        //        });
        //    }
        //}

        public void RefreshDifferentThreadUI(Control control, Action action)
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

        private void T_RefreshPressure1_Tick(object sender, EventArgs e)
        {
            if (MiddleLayer.PCBA_ScrewFasten_Module1F.pressureQueue.Count > 0)
            {
                PressureCurves_Chart1.ShowCurves(MiddleLayer.PCBA_ScrewFasten_Module1F.pressureQueue.Dequeue());
            }
        }

        private void T_RefreshPressure2_Tick(object sender, EventArgs e)
        {
            if (MiddleLayer.PCBA_ScrewFasten_Module2F.pressureQueue.Count > 0)
            {
                PressureCurves_Chart2.ShowCurves(MiddleLayer.PCBA_ScrewFasten_Module2F.pressureQueue.Dequeue());
            }
        }
    }
}
