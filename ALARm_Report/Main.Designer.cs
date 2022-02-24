

namespace ALARm
{
    partial class Main
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.conRoadRd = new MetroFramework.Controls.MetroComboBox();
            this.admRoadBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.conDistanceRd = new ALARm.controls.admControl();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.reporTabControl = new MetroFramework.Controls.MetroTabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.metroProgressBar1 = new MetroFramework.Controls.MetroProgressBar();
            this.btnReportProcess = new MetroFramework.Controls.MetroButton();
            this.lbReportTemplates = new ALARm.controls.CatalogListBox();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbPeriod = new ALARm_Report.controls.PeriodComboBox();
            this.cbReport = new ALARm.controls.CatalogComboBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.conRdVideoObjects = new ALARm.controls.rdVideoObjectsControl();
            this.metroPanel6 = new MetroFramework.Controls.MetroPanel();
            this.RdSaveButton = new MetroFramework.Controls.MetroButton();
            this.progressBar = new MetroFramework.Controls.MetroProgressBar();
            this.conTripsFiles = new ALARm.controls.tripsFilesControl();
            this.conTrips = new ALARm.controls.tripsControl();
            this.rbDet = new MetroFramework.Controls.MetroRadioButton();
            this.rbSvod = new MetroFramework.Controls.MetroRadioButton();
            this.rbAll = new MetroFramework.Controls.MetroRadioButton();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.admRoadBindingSource)).BeginInit();
            this.metroPanel2.SuspendLayout();
            this.reporTabControl.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.metroPanel4.SuspendLayout();
            this.metroPanel5.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.metroPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.conRoadRd);
            this.metroPanel1.Controls.Add(this.conDistanceRd);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 60);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.metroPanel1.Size = new System.Drawing.Size(159, 620);
            this.metroPanel1.TabIndex = 22;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // conRoadRd
            // 
            this.conRoadRd.DataSource = this.admRoadBindingSource;
            this.conRoadRd.DisplayMember = "Name";
            this.conRoadRd.Dock = System.Windows.Forms.DockStyle.Top;
            this.conRoadRd.FormattingEnabled = true;
            this.conRoadRd.ItemHeight = 23;
            this.conRoadRd.Location = new System.Drawing.Point(0, 5);
            this.conRoadRd.Name = "conRoadRd";
            this.conRoadRd.PromptText = "Дорога";
            this.conRoadRd.Size = new System.Drawing.Size(159, 29);
            this.conRoadRd.TabIndex = 22;
            this.conRoadRd.UseSelectable = true;
            this.conRoadRd.ValueMember = "Id";
            this.conRoadRd.SelectedIndexChanged += new System.EventHandler(this.conRoadRd_SelectedIndexChanged);
            // 
            // admRoadBindingSource
            // 
            this.admRoadBindingSource.DataSource = typeof(ALARm.Core.AdmRoad);
            // 
            // conDistanceRd
            // 
            this.conDistanceRd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.conDistanceRd.Location = new System.Drawing.Point(0, 5);
            this.conDistanceRd.Name = "conDistanceRd";
            this.conDistanceRd.Size = new System.Drawing.Size(159, 615);
            this.conDistanceRd.TabIndex = 21;
            this.conDistanceRd.UseSelectable = true;
            this.conDistanceRd.UnitSelectionChanged += new System.EventHandler(this.conDirectionRd_UnitSelectionChanged);
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.metroPanel5);
            this.metroPanel2.Controls.Add(this.reporTabControl);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(179, 60);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(942, 620);
            this.metroPanel2.TabIndex = 23;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            this.metroPanel2.Paint += new System.Windows.Forms.PaintEventHandler(this.metroPanel2_Paint);
            // 
            // reporTabControl
            // 
            this.reporTabControl.Controls.Add(this.tabPage2);
            this.reporTabControl.Controls.Add(this.tabPage1);
            this.reporTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reporTabControl.FontSize = MetroFramework.MetroTabControlSize.Small;
            this.reporTabControl.Location = new System.Drawing.Point(0, 0);
            this.reporTabControl.Multiline = true;
            this.reporTabControl.Name = "reporTabControl";
            this.reporTabControl.Padding = new System.Drawing.Point(6, 8);
            this.reporTabControl.SelectedIndex = 0;
            this.reporTabControl.Size = new System.Drawing.Size(942, 620);
            this.reporTabControl.SizeMode = System.Windows.Forms.TabSizeMode.FillToRight;
            this.reporTabControl.Style = MetroFramework.MetroColorStyle.Teal;
            this.reporTabControl.TabIndex = 4;
            this.reporTabControl.UseSelectable = true;
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage2.Controls.Add(this.metroPanel4);
            this.tabPage2.Controls.Add(this.metroPanel3);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(934, 582);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Другие отчеты";
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.lbReportTemplates);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(0, 40);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Size = new System.Drawing.Size(934, 542);
            this.metroPanel4.TabIndex = 24;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.metroProgressBar1);
            this.metroPanel5.Controls.Add(this.btnReportProcess);
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(3, 592);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Size = new System.Drawing.Size(934, 25);
            this.metroPanel5.TabIndex = 4;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            this.metroPanel5.Paint += new System.Windows.Forms.PaintEventHandler(this.metroPanel5_Paint);
            // 
            // metroProgressBar1
            // 
            this.metroProgressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroProgressBar1.Location = new System.Drawing.Point(0, 0);
            this.metroProgressBar1.Name = "metroProgressBar1";
            this.metroProgressBar1.Size = new System.Drawing.Size(794, 25);
            this.metroProgressBar1.TabIndex = 3;
            // 
            // btnReportProcess
            // 
            this.btnReportProcess.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnReportProcess.Location = new System.Drawing.Point(794, 0);
            this.btnReportProcess.Name = "btnReportProcess";
            this.btnReportProcess.Size = new System.Drawing.Size(140, 25);
            this.btnReportProcess.TabIndex = 2;
            this.btnReportProcess.Text = "Сформировать отчет";
            this.btnReportProcess.UseSelectable = true;
            this.btnReportProcess.Click += new System.EventHandler(this.btnReportProcess_Click);
            // 
            // lbReportTemplates
            // 
            this.lbReportTemplates.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReportTemplates.Location = new System.Drawing.Point(0, 0);
            this.lbReportTemplates.Margin = new System.Windows.Forms.Padding(2);
            this.lbReportTemplates.Name = "lbReportTemplates";
            this.lbReportTemplates.Size = new System.Drawing.Size(934, 542);
            this.lbReportTemplates.TabIndex = 3;
            this.lbReportTemplates.Title = "Ведомости";
            this.lbReportTemplates.TitleWidth = 924;
            this.lbReportTemplates.UseSelectable = true;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.comboBox1);
            this.metroPanel3.Controls.Add(this.label1);
            this.metroPanel3.Controls.Add(this.cbPeriod);
            this.metroPanel3.Controls.Add(this.cbReport);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(0, 0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Size = new System.Drawing.Size(934, 40);
            this.metroPanel3.TabIndex = 23;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Вывести все"});
            this.comboBox1.Location = new System.Drawing.Point(761, 6);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(166, 21);
            this.comboBox1.TabIndex = 11;
            this.comboBox1.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(722, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Дата";
            this.label1.Visible = false;
            // 
            // cbPeriod
            // 
            this.cbPeriod.Location = new System.Drawing.Point(429, 0);
            this.cbPeriod.Margin = new System.Windows.Forms.Padding(2);
            this.cbPeriod.Name = "cbPeriod";
            this.cbPeriod.Size = new System.Drawing.Size(279, 39);
            this.cbPeriod.TabIndex = 4;
            this.cbPeriod.Title = "Отчетный период";
            this.cbPeriod.TitleWidth = 130;
            this.cbPeriod.UseSelectable = true;
            this.cbPeriod.Load += new System.EventHandler(this.cbPeriod_Load);
            // 
            // cbReport
            // 
            this.cbReport.Dock = System.Windows.Forms.DockStyle.Left;
            this.cbReport.Location = new System.Drawing.Point(0, 0);
            this.cbReport.Margin = new System.Windows.Forms.Padding(2);
            this.cbReport.Name = "cbReport";
            this.cbReport.Size = new System.Drawing.Size(425, 40);
            this.cbReport.TabIndex = 2;
            this.cbReport.Title = "Тип";
            this.cbReport.TitleWidth = 100;
            this.cbReport.UseSelectable = true;
            this.cbReport.SelectionChanged += new System.EventHandler(this.cbReport_SelectionChanged);
            this.cbReport.Load += new System.EventHandler(this.cbReport_Load);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.tabPage1.Controls.Add(this.conRdVideoObjects);
            this.tabPage1.Controls.Add(this.metroPanel6);
            this.tabPage1.Controls.Add(this.conTripsFiles);
            this.tabPage1.Controls.Add(this.conTrips);
            this.tabPage1.Controls.Add(this.rbDet);
            this.tabPage1.Controls.Add(this.rbSvod);
            this.tabPage1.Controls.Add(this.rbAll);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(934, 662);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Видеобъекты";
            // 
            // conRdVideoObjects
            // 
            this.conRdVideoObjects.Location = new System.Drawing.Point(310, 95);
            this.conRdVideoObjects.Name = "conRdVideoObjects";
            this.conRdVideoObjects.Size = new System.Drawing.Size(500, 402);
            this.conRdVideoObjects.TabIndex = 29;
            this.conRdVideoObjects.UseSelectable = true;
            // 
            // metroPanel6
            // 
            this.metroPanel6.Controls.Add(this.RdSaveButton);
            this.metroPanel6.Controls.Add(this.progressBar);
            this.metroPanel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel6.HorizontalScrollbarBarColor = true;
            this.metroPanel6.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel6.HorizontalScrollbarSize = 10;
            this.metroPanel6.Location = new System.Drawing.Point(0, 637);
            this.metroPanel6.Name = "metroPanel6";
            this.metroPanel6.Size = new System.Drawing.Size(934, 25);
            this.metroPanel6.TabIndex = 30;
            this.metroPanel6.VerticalScrollbarBarColor = true;
            this.metroPanel6.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel6.VerticalScrollbarSize = 10;
            // 
            // RdSaveButton
            // 
            this.RdSaveButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.RdSaveButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.RdSaveButton.Location = new System.Drawing.Point(794, 0);
            this.RdSaveButton.Name = "RdSaveButton";
            this.RdSaveButton.Size = new System.Drawing.Size(140, 25);
            this.RdSaveButton.TabIndex = 33;
            this.RdSaveButton.Text = "Сформировать отчет";
            this.RdSaveButton.UseSelectable = true;
            this.RdSaveButton.Click += new System.EventHandler(this.RdSaveButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(0, 0);
            this.progressBar.Name = "progressBar";
            this.progressBar.ProgressBarStyle = System.Windows.Forms.ProgressBarStyle.Blocks;
            this.progressBar.Size = new System.Drawing.Size(934, 25);
            this.progressBar.TabIndex = 32;
            // 
            // conTripsFiles
            // 
            this.conTripsFiles.Location = new System.Drawing.Point(10, 295);
            this.conTripsFiles.Name = "conTripsFiles";
            this.conTripsFiles.Size = new System.Drawing.Size(300, 197);
            this.conTripsFiles.TabIndex = 28;
            this.conTripsFiles.UseSelectable = true;
            this.conTripsFiles.UnitSelectionChanged += new System.EventHandler(this.conTripsFiles_UnitSelectionChanged);
            // 
            // conTrips
            // 
            this.conTrips.Location = new System.Drawing.Point(10, 95);
            this.conTrips.Name = "conTrips";
            this.conTrips.Size = new System.Drawing.Size(300, 197);
            this.conTrips.TabIndex = 27;
            this.conTrips.UseSelectable = true;
            this.conTrips.UnitSelectionChanged += new System.EventHandler(this.conTrips_UnitSelectionChanged);
            // 
            // rbDet
            // 
            this.rbDet.AutoSize = true;
            this.rbDet.Location = new System.Drawing.Point(10, 69);
            this.rbDet.Name = "rbDet";
            this.rbDet.Size = new System.Drawing.Size(117, 15);
            this.rbDet.TabIndex = 26;
            this.rbDet.Text = "Детальный отчет";
            this.rbDet.UseSelectable = true;
            // 
            // rbSvod
            // 
            this.rbSvod.AutoSize = true;
            this.rbSvod.Location = new System.Drawing.Point(10, 48);
            this.rbSvod.Name = "rbSvod";
            this.rbSvod.Size = new System.Drawing.Size(106, 15);
            this.rbSvod.TabIndex = 25;
            this.rbSvod.Text = "Сводный отчет";
            this.rbSvod.UseSelectable = true;
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Checked = true;
            this.rbAll.Location = new System.Drawing.Point(10, 27);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(102, 15);
            this.rbAll.TabIndex = 24;
            this.rbAll.TabStop = true;
            this.rbAll.Text = "Полный отчет";
            this.rbAll.UseSelectable = true;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 700);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.Name = "Main";
            this.Text = "Генератор отчетов";
            this.Load += new System.EventHandler(this.Main_Load);
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.admRoadBindingSource)).EndInit();
            this.metroPanel2.ResumeLayout(false);
            this.reporTabControl.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.metroPanel4.ResumeLayout(false);
            this.metroPanel5.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel3.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.metroPanel6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private controls.admControl conDistanceRd;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroTabControl reporTabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private controls.rdVideoObjectsControl conRdVideoObjects;
        private controls.tripsFilesControl conTripsFiles;
        private controls.tripsControl conTrips;
        private MetroFramework.Controls.MetroRadioButton rbDet;
        private MetroFramework.Controls.MetroRadioButton rbSvod;
        private MetroFramework.Controls.MetroRadioButton rbAll;
        private System.Windows.Forms.TabPage tabPage2;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private controls.CatalogComboBox cbReport;
        private controls.CatalogListBox lbReportTemplates;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroProgressBar metroProgressBar1;
        private MetroFramework.Controls.MetroButton btnReportProcess;
        private MetroFramework.Controls.MetroPanel metroPanel6;
        private MetroFramework.Controls.MetroButton RdSaveButton;
        private MetroFramework.Controls.MetroProgressBar progressBar;
        private ALARm_Report.controls.PeriodComboBox cbPeriod;
        private MetroFramework.Controls.MetroComboBox conRoadRd;
        private System.Windows.Forms.BindingSource admRoadBindingSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}

