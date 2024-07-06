namespace Acura3._0.ModuleForms
{
    partial class RecordForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RecordForm));
            this.T_RefreshPressure1 = new System.Windows.Forms.Timer(this.components);
            this.cogRecord_Gantry2 = new Cognex.VisionPro.CogRecordDisplay();
            this.cogRecord_Robot4 = new Cognex.VisionPro.CogRecordDisplay();
            this.cogRecord_Robot1 = new Cognex.VisionPro.CogRecordDisplay();
            this.cogRecord_Gantry1 = new Cognex.VisionPro.CogRecordDisplay();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cogRecord_Robot2 = new Cognex.VisionPro.CogRecordDisplay();
            this.cogRecord_Robot3 = new Cognex.VisionPro.CogRecordDisplay();
            this.Cog_CoverAssembly1 = new Cognex.VisionPro.CogRecordDisplay();
            this.Cog_CoverAssembly2 = new Cognex.VisionPro.CogRecordDisplay();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PressureCurves_Chart1 = new PressureCurves.PressureCurves();
            this.PressureCurves_Chart2 = new PressureCurves.PressureCurves();
            this.T_RefreshPressure2 = new System.Windows.Forms.Timer(this.components);
            this.plMachineStatus.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SettingData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecipeData)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Gantry2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Gantry1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cog_CoverAssembly1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cog_CoverAssembly2)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // plMaintenance
            // 
            this.plMaintenance.Enabled = false;
            this.plMaintenance.Margin = new System.Windows.Forms.Padding(4);
            this.plMaintenance.Size = new System.Drawing.Size(1695, 643);
            // 
            // plProductionSetting
            // 
            this.plProductionSetting.Enabled = false;
            this.plProductionSetting.Margin = new System.Windows.Forms.Padding(4);
            this.plProductionSetting.Size = new System.Drawing.Size(799, 627);
            // 
            // plRecipeEditor
            // 
            this.plRecipeEditor.Enabled = false;
            this.plRecipeEditor.Margin = new System.Windows.Forms.Padding(4);
            this.plRecipeEditor.Size = new System.Drawing.Size(1695, 643);
            // 
            // plFlowInitial
            // 
            this.plFlowInitial.Enabled = false;
            this.plFlowInitial.Margin = new System.Windows.Forms.Padding(4);
            this.plFlowInitial.Size = new System.Drawing.Size(799, 627);
            // 
            // plFlowAuto
            // 
            this.plFlowAuto.Enabled = false;
            this.plFlowAuto.Margin = new System.Windows.Forms.Padding(4);
            this.plFlowAuto.Size = new System.Drawing.Size(799, 627);
            // 
            // plMachineStatus
            // 
            this.plMachineStatus.Controls.Add(this.tabControl2);
            this.plMachineStatus.Margin = new System.Windows.Forms.Padding(4);
            this.plMachineStatus.Size = new System.Drawing.Size(1695, 643);
            // 
            // plMotionSetup
            // 
            this.plMotionSetup.Enabled = false;
            this.plMotionSetup.Margin = new System.Windows.Forms.Padding(4);
            this.plMotionSetup.Size = new System.Drawing.Size(799, 627);
            // 
            // plMotorControl
            // 
            this.plMotorControl.Enabled = false;
            this.plMotorControl.Margin = new System.Windows.Forms.Padding(4);
            this.plMotorControl.Size = new System.Drawing.Size(799, 627);
            // 
            // T_RefreshPressure1
            // 
            this.T_RefreshPressure1.Enabled = true;
            this.T_RefreshPressure1.Interval = 10;
            this.T_RefreshPressure1.Tick += new System.EventHandler(this.T_RefreshPressure1_Tick);
            // 
            // cogRecord_Gantry2
            // 
            this.cogRecord_Gantry2.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Gantry2.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Gantry2.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Gantry2.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Gantry2.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Gantry2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Gantry2.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Gantry2.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Gantry2.Location = new System.Drawing.Point(3, 318);
            this.cogRecord_Gantry2.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Gantry2.MouseWheelSensitivity = 1D;
            this.cogRecord_Gantry2.Name = "cogRecord_Gantry2";
            this.cogRecord_Gantry2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Gantry2.OcxState")));
            this.cogRecord_Gantry2.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Gantry2.TabIndex = 71;
            // 
            // cogRecord_Robot4
            // 
            this.cogRecord_Robot4.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot4.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Robot4.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Robot4.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot4.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Robot4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Robot4.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Robot4.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Robot4.Location = new System.Drawing.Point(453, 318);
            this.cogRecord_Robot4.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Robot4.MouseWheelSensitivity = 1D;
            this.cogRecord_Robot4.Name = "cogRecord_Robot4";
            this.cogRecord_Robot4.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Robot4.OcxState")));
            this.cogRecord_Robot4.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Robot4.TabIndex = 70;
            // 
            // cogRecord_Robot1
            // 
            this.cogRecord_Robot1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot1.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Robot1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Robot1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot1.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Robot1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Robot1.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Robot1.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Robot1.Location = new System.Drawing.Point(453, 3);
            this.cogRecord_Robot1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Robot1.MouseWheelSensitivity = 1D;
            this.cogRecord_Robot1.Name = "cogRecord_Robot1";
            this.cogRecord_Robot1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Robot1.OcxState")));
            this.cogRecord_Robot1.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Robot1.TabIndex = 69;
            // 
            // cogRecord_Gantry1
            // 
            this.cogRecord_Gantry1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Gantry1.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Gantry1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Gantry1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Gantry1.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Gantry1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Gantry1.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Gantry1.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Gantry1.Location = new System.Drawing.Point(3, 3);
            this.cogRecord_Gantry1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Gantry1.MouseWheelSensitivity = 1D;
            this.cogRecord_Gantry1.Name = "cogRecord_Gantry1";
            this.cogRecord_Gantry1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Gantry1.OcxState")));
            this.cogRecord_Gantry1.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Gantry1.TabIndex = 68;
            // 
            // label12
            // 
            this.label12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label12.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Navy;
            this.label12.Location = new System.Drawing.Point(1353, 290);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(445, 25);
            this.label12.TabIndex = 66;
            this.label12.Text = "Robot4   Pick and Place";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Navy;
            this.label11.Location = new System.Drawing.Point(903, 290);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(444, 25);
            this.label11.TabIndex = 65;
            this.label11.Text = "Robot3   Detection -2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label10.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Navy;
            this.label10.Location = new System.Drawing.Point(1353, 605);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(445, 27);
            this.label10.TabIndex = 64;
            this.label10.Text = "Robot4   Screw";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label9.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Navy;
            this.label9.Location = new System.Drawing.Point(453, 605);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(444, 27);
            this.label9.TabIndex = 62;
            this.label9.Text = "Robot3  Detection -4";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Navy;
            this.label8.Location = new System.Drawing.Point(453, 290);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(444, 25);
            this.label8.TabIndex = 60;
            this.label8.Text = "Robot3   Detection -1";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.White;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Navy;
            this.label5.Location = new System.Drawing.Point(3, 290);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(444, 25);
            this.label5.TabIndex = 0;
            this.label5.Text = "Robot 1  Screw";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label5, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label8, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label10, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label11, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Gantry1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Robot1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Robot4, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Gantry2, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label9, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Robot2, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cogRecord_Robot3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.Cog_CoverAssembly1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.Cog_CoverAssembly2, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 3, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(6, 7);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 46F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1801, 632);
            this.tableLayoutPanel1.TabIndex = 46;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Navy;
            this.label2.Location = new System.Drawing.Point(903, 605);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(444, 21);
            this.label2.TabIndex = 77;
            this.label2.Text = "Robot3  Detection -3";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Navy;
            this.label1.Location = new System.Drawing.Point(3, 605);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(444, 27);
            this.label1.TabIndex = 76;
            this.label1.Text = "Robot 2  Screw";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cogRecord_Robot2
            // 
            this.cogRecord_Robot2.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot2.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Robot2.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Robot2.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot2.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Robot2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Robot2.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Robot2.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Robot2.Location = new System.Drawing.Point(903, 3);
            this.cogRecord_Robot2.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Robot2.MouseWheelSensitivity = 1D;
            this.cogRecord_Robot2.Name = "cogRecord_Robot2";
            this.cogRecord_Robot2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Robot2.OcxState")));
            this.cogRecord_Robot2.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Robot2.TabIndex = 72;
            // 
            // cogRecord_Robot3
            // 
            this.cogRecord_Robot3.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot3.ColorMapLowerRoiLimit = 0D;
            this.cogRecord_Robot3.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.cogRecord_Robot3.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.cogRecord_Robot3.ColorMapUpperRoiLimit = 1D;
            this.cogRecord_Robot3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cogRecord_Robot3.DoubleTapZoomCycleLength = 2;
            this.cogRecord_Robot3.DoubleTapZoomSensitivity = 2.5D;
            this.cogRecord_Robot3.Location = new System.Drawing.Point(903, 318);
            this.cogRecord_Robot3.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.cogRecord_Robot3.MouseWheelSensitivity = 1D;
            this.cogRecord_Robot3.Name = "cogRecord_Robot3";
            this.cogRecord_Robot3.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("cogRecord_Robot3.OcxState")));
            this.cogRecord_Robot3.Size = new System.Drawing.Size(444, 284);
            this.cogRecord_Robot3.TabIndex = 73;
            // 
            // Cog_CoverAssembly1
            // 
            this.Cog_CoverAssembly1.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.Cog_CoverAssembly1.ColorMapLowerRoiLimit = 0D;
            this.Cog_CoverAssembly1.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.Cog_CoverAssembly1.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.Cog_CoverAssembly1.ColorMapUpperRoiLimit = 1D;
            this.Cog_CoverAssembly1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Cog_CoverAssembly1.DoubleTapZoomCycleLength = 2;
            this.Cog_CoverAssembly1.DoubleTapZoomSensitivity = 2.5D;
            this.Cog_CoverAssembly1.Location = new System.Drawing.Point(1353, 3);
            this.Cog_CoverAssembly1.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.Cog_CoverAssembly1.MouseWheelSensitivity = 1D;
            this.Cog_CoverAssembly1.Name = "Cog_CoverAssembly1";
            this.Cog_CoverAssembly1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Cog_CoverAssembly1.OcxState")));
            this.Cog_CoverAssembly1.Size = new System.Drawing.Size(445, 284);
            this.Cog_CoverAssembly1.TabIndex = 74;
            // 
            // Cog_CoverAssembly2
            // 
            this.Cog_CoverAssembly2.ColorMapLowerClipColor = System.Drawing.Color.Black;
            this.Cog_CoverAssembly2.ColorMapLowerRoiLimit = 0D;
            this.Cog_CoverAssembly2.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
            this.Cog_CoverAssembly2.ColorMapUpperClipColor = System.Drawing.Color.Black;
            this.Cog_CoverAssembly2.ColorMapUpperRoiLimit = 1D;
            this.Cog_CoverAssembly2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Cog_CoverAssembly2.DoubleTapZoomCycleLength = 2;
            this.Cog_CoverAssembly2.DoubleTapZoomSensitivity = 2.5D;
            this.Cog_CoverAssembly2.Location = new System.Drawing.Point(1353, 318);
            this.Cog_CoverAssembly2.MouseWheelMode = Cognex.VisionPro.Display.CogDisplayMouseWheelModeConstants.Zoom1;
            this.Cog_CoverAssembly2.MouseWheelSensitivity = 1D;
            this.Cog_CoverAssembly2.Name = "Cog_CoverAssembly2";
            this.Cog_CoverAssembly2.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("Cog_CoverAssembly2.OcxState")));
            this.Cog_CoverAssembly2.Size = new System.Drawing.Size(445, 284);
            this.Cog_CoverAssembly2.TabIndex = 75;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tabPage1);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Location = new System.Drawing.Point(5, 3);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1821, 892);
            this.tabControl2.TabIndex = 47;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1813, 856);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Vision ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.tableLayoutPanel2);
            this.tabPage4.Location = new System.Drawing.Point(4, 32);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(1813, 856);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "Pressure Curves";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.label4, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.PressureCurves_Chart1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.PressureCurves_Chart2, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1666, 566);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.White;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Navy;
            this.label4.Location = new System.Drawing.Point(3, 532);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(827, 34);
            this.label4.TabIndex = 2;
            this.label4.Text = "Jacking  Axis 1";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Navy;
            this.label3.Location = new System.Drawing.Point(836, 532);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(827, 34);
            this.label3.TabIndex = 1;
            this.label3.Text = "Jacking  Axis 2";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PressureCurves_Chart1
            // 
            this.PressureCurves_Chart1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PressureCurves_Chart1.Location = new System.Drawing.Point(5, 6);
            this.PressureCurves_Chart1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.PressureCurves_Chart1.Name = "PressureCurves_Chart1";
            this.PressureCurves_Chart1.Size = new System.Drawing.Size(823, 520);
            this.PressureCurves_Chart1.TabIndex = 3;
            // 
            // PressureCurves_Chart2
            // 
            this.PressureCurves_Chart2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PressureCurves_Chart2.Location = new System.Drawing.Point(838, 6);
            this.PressureCurves_Chart2.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.PressureCurves_Chart2.Name = "PressureCurves_Chart2";
            this.PressureCurves_Chart2.Size = new System.Drawing.Size(823, 520);
            this.PressureCurves_Chart2.TabIndex = 4;
            // 
            // T_RefreshPressure2
            // 
            this.T_RefreshPressure2.Enabled = true;
            this.T_RefreshPressure2.Interval = 10;
            this.T_RefreshPressure2.Tick += new System.EventHandler(this.T_RefreshPressure2_Tick);
            // 
            // RecordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1709, 707);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "RecordForm";
            this.Text = "RecordForm";
            this.plMachineStatus.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SettingData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RecipeData)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Gantry2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Gantry1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cogRecord_Robot3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cog_CoverAssembly1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Cog_CoverAssembly2)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        public System.Windows.Forms.Timer T_RefreshPressure1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Gantry1;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Robot1;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Robot4;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Gantry2;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Robot2;
        public Cognex.VisionPro.CogRecordDisplay cogRecord_Robot3;
        public Cognex.VisionPro.CogRecordDisplay Cog_CoverAssembly1;
        public Cognex.VisionPro.CogRecordDisplay Cog_CoverAssembly2;

        public Cognex.VisionPro.CogRecordDisplay CogRecord_Gantry1
        {
            get => cogRecord_Gantry1;
            set => cogRecord_Gantry1 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Robot1
        {
            get => cogRecord_Robot1;
            set => cogRecord_Robot1 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Robot4
        {
            get => cogRecord_Robot4;
            set => cogRecord_Robot4 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Gantry2
        {
            get => cogRecord_Gantry2;
            set => cogRecord_Gantry2 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Robot2
        {
            get => cogRecord_Robot2;
            set => cogRecord_Robot2 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Robot3
        {
            get => cogRecord_Robot3;
            set => cogRecord_Robot3 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_FeederPick
        {
            get => Cog_CoverAssembly1;
            set => Cog_CoverAssembly1 = value;
        }
        public Cognex.VisionPro.CogRecordDisplay CogRecord_Place
        {
            get => Cog_CoverAssembly2;
            set => Cog_CoverAssembly2 = value;
        }


        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        public System.Windows.Forms.Timer T_RefreshPressure2;
        public PressureCurves.PressureCurves PressureCurves_Chart1;
        public PressureCurves.PressureCurves PressureCurves_Chart2;
    }
}