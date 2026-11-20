namespace MedClinic.Views
{
    partial class StatisticsView
    {
        /// <summary>Обязательная переменная конструктора.</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>Освобождение ресурсов.</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        private void InitializeComponent()
        {
            this.panelTop = new System.Windows.Forms.Panel();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.dtpFrom = new System.Windows.Forms.DateTimePicker();
            this.lblDash = new System.Windows.Forms.Label();
            this.dtpTo = new System.Windows.Forms.DateTimePicker();
            this.btnThisMonth = new System.Windows.Forms.Button();
            this.btnLastMonth = new System.Windows.Forms.Button();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.lblPeriodInfo = new System.Windows.Forms.Label();
            this.panelCards = new System.Windows.Forms.Panel();
            this.cardTotal = new System.Windows.Forms.Panel();
            this.lblTotalCaption = new System.Windows.Forms.Label();
            this.lblTotalValue = new System.Windows.Forms.Label();
            this.cardAverage = new System.Windows.Forms.Panel();
            this.lblAverageCaption = new System.Windows.Forms.Label();
            this.lblAverageValue = new System.Windows.Forms.Label();
            this.cardMedian = new System.Windows.Forms.Panel();
            this.lblMedianCaption = new System.Windows.Forms.Label();
            this.lblMedianValue = new System.Windows.Forms.Label();
            this.cardVariance = new System.Windows.Forms.Panel();
            this.lblVarianceCaption = new System.Windows.Forms.Label();
            this.lblVarianceValue = new System.Windows.Forms.Label();
            this.cardSigma = new System.Windows.Forms.Panel();
            this.lblSigmaCaption = new System.Windows.Forms.Label();
            this.lblSigmaValue = new System.Windows.Forms.Label();
            this.dgvDaily = new System.Windows.Forms.DataGridView();
            this.colDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.panelChart = new System.Windows.Forms.Panel();
            this.lblChartTitle = new System.Windows.Forms.Label();
            this.formsPlot = new ScottPlot.FormsPlot();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.lblFormula = new System.Windows.Forms.Label();
            this.lblMinMax = new System.Windows.Forms.Label();
            this.panelTop.SuspendLayout();
            this.panelCards.SuspendLayout();
            this.cardTotal.SuspendLayout();
            this.cardAverage.SuspendLayout();
            this.cardMedian.SuspendLayout();
            this.cardVariance.SuspendLayout();
            this.cardSigma.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaily)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.panelChart.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelTop.BackColor = System.Drawing.Color.White;
            this.panelTop.Controls.Add(this.lblPeriod);
            this.panelTop.Controls.Add(this.dtpFrom);
            this.panelTop.Controls.Add(this.lblDash);
            this.panelTop.Controls.Add(this.dtpTo);
            this.panelTop.Controls.Add(this.btnThisMonth);
            this.panelTop.Controls.Add(this.btnLastMonth);
            this.panelTop.Controls.Add(this.btnCalculate);
            this.panelTop.Controls.Add(this.lblPeriodInfo);
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1172, 84);
            this.panelTop.TabIndex = 0;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoSize = true;
            this.lblPeriod.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPeriod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblPeriod.Location = new System.Drawing.Point(16, 18);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Size = new System.Drawing.Size(57, 17);
            this.lblPeriod.TabIndex = 0;
            this.lblPeriod.Text = "Период:";
            // 
            // dtpFrom
            // 
            this.dtpFrom.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFrom.Location = new System.Drawing.Point(80, 15);
            this.dtpFrom.Name = "dtpFrom";
            this.dtpFrom.Size = new System.Drawing.Size(130, 25);
            this.dtpFrom.TabIndex = 1;
            // 
            // lblDash
            // 
            this.lblDash.AutoSize = true;
            this.lblDash.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblDash.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblDash.Location = new System.Drawing.Point(216, 18);
            this.lblDash.Name = "lblDash";
            this.lblDash.Size = new System.Drawing.Size(16, 17);
            this.lblDash.TabIndex = 2;
            this.lblDash.Text = "—";
            // 
            // dtpTo
            // 
            this.dtpTo.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTo.Location = new System.Drawing.Point(236, 15);
            this.dtpTo.Name = "dtpTo";
            this.dtpTo.Size = new System.Drawing.Size(130, 25);
            this.dtpTo.TabIndex = 3;
            // 
            // btnThisMonth
            // 
            this.btnThisMonth.BackColor = System.Drawing.Color.White;
            this.btnThisMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnThisMonth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(215)))));
            this.btnThisMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnThisMonth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnThisMonth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnThisMonth.Location = new System.Drawing.Point(382, 13);
            this.btnThisMonth.Name = "btnThisMonth";
            this.btnThisMonth.Size = new System.Drawing.Size(140, 28);
            this.btnThisMonth.TabIndex = 4;
            this.btnThisMonth.Text = "Текущий месяц";
            this.btnThisMonth.UseVisualStyleBackColor = false;
            this.btnThisMonth.Click += new System.EventHandler(this.btnThisMonth_Click);
            // 
            // btnLastMonth
            // 
            this.btnLastMonth.BackColor = System.Drawing.Color.White;
            this.btnLastMonth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLastMonth.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(208)))), ((int)(((byte)(215)))));
            this.btnLastMonth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLastMonth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnLastMonth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnLastMonth.Location = new System.Drawing.Point(530, 13);
            this.btnLastMonth.Name = "btnLastMonth";
            this.btnLastMonth.Size = new System.Drawing.Size(140, 28);
            this.btnLastMonth.TabIndex = 5;
            this.btnLastMonth.Text = "Прошлый месяц";
            this.btnLastMonth.UseVisualStyleBackColor = false;
            this.btnLastMonth.Click += new System.EventHandler(this.btnLastMonth_Click);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.btnCalculate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCalculate.FlatAppearance.BorderSize = 0;
            this.btnCalculate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCalculate.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnCalculate.ForeColor = System.Drawing.Color.White;
            this.btnCalculate.Location = new System.Drawing.Point(1000, 12);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(156, 30);
            this.btnCalculate.TabIndex = 6;
            this.btnCalculate.Text = "Рассчитать";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // lblPeriodInfo
            // 
            this.lblPeriodInfo.AutoSize = true;
            this.lblPeriodInfo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPeriodInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblPeriodInfo.Location = new System.Drawing.Point(16, 52);
            this.lblPeriodInfo.Name = "lblPeriodInfo";
            this.lblPeriodInfo.Size = new System.Drawing.Size(300, 15);
            this.lblPeriodInfo.TabIndex = 7;
            this.lblPeriodInfo.Text = "Выберите период и нажмите «Рассчитать»";
            // 
            // panelCards
            // 
            this.panelCards.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelCards.BackColor = System.Drawing.Color.White;
            this.panelCards.Controls.Add(this.cardTotal);
            this.panelCards.Controls.Add(this.cardAverage);
            this.panelCards.Controls.Add(this.cardMedian);
            this.panelCards.Controls.Add(this.cardVariance);
            this.panelCards.Controls.Add(this.cardSigma);
            this.panelCards.Location = new System.Drawing.Point(0, 84);
            this.panelCards.Name = "panelCards";
            this.panelCards.Size = new System.Drawing.Size(1172, 110);
            this.panelCards.TabIndex = 1;
            // 
            // cardTotal
            // 
            this.cardTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(247)))), ((int)(((byte)(252)))));
            this.cardTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardTotal.Controls.Add(this.lblTotalCaption);
            this.cardTotal.Controls.Add(this.lblTotalValue);
            this.cardTotal.Location = new System.Drawing.Point(16, 8);
            this.cardTotal.Name = "cardTotal";
            this.cardTotal.Size = new System.Drawing.Size(218, 92);
            this.cardTotal.TabIndex = 0;
            // 
            // lblTotalCaption
            // 
            this.lblTotalCaption.AutoSize = true;
            this.lblTotalCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblTotalCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(104)))), ((int)(((byte)(116)))));
            this.lblTotalCaption.Location = new System.Drawing.Point(14, 12);
            this.lblTotalCaption.Name = "lblTotalCaption";
            this.lblTotalCaption.Size = new System.Drawing.Size(120, 15);
            this.lblTotalCaption.TabIndex = 0;
            this.lblTotalCaption.Text = "Всего записей";
            // 
            // lblTotalValue
            // 
            this.lblTotalValue.AutoSize = true;
            this.lblTotalValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTotalValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.lblTotalValue.Location = new System.Drawing.Point(12, 34);
            this.lblTotalValue.Name = "lblTotalValue";
            this.lblTotalValue.Size = new System.Drawing.Size(34, 41);
            this.lblTotalValue.TabIndex = 1;
            this.lblTotalValue.Text = "0";
            // 
            // cardAverage
            // 
            this.cardAverage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(248)))), ((int)(((byte)(243)))));
            this.cardAverage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardAverage.Controls.Add(this.lblAverageCaption);
            this.cardAverage.Controls.Add(this.lblAverageValue);
            this.cardAverage.Location = new System.Drawing.Point(246, 8);
            this.cardAverage.Name = "cardAverage";
            this.cardAverage.Size = new System.Drawing.Size(218, 92);
            this.cardAverage.TabIndex = 1;
            // 
            // lblAverageCaption
            // 
            this.lblAverageCaption.AutoSize = true;
            this.lblAverageCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblAverageCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(104)))), ((int)(((byte)(116)))));
            this.lblAverageCaption.Location = new System.Drawing.Point(14, 12);
            this.lblAverageCaption.Name = "lblAverageCaption";
            this.lblAverageCaption.Size = new System.Drawing.Size(150, 15);
            this.lblAverageCaption.TabIndex = 0;
            this.lblAverageCaption.Text = "В среднем за день";
            // 
            // lblAverageValue
            // 
            this.lblAverageValue.AutoSize = true;
            this.lblAverageValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblAverageValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(80)))));
            this.lblAverageValue.Location = new System.Drawing.Point(12, 34);
            this.lblAverageValue.Name = "lblAverageValue";
            this.lblAverageValue.Size = new System.Drawing.Size(66, 41);
            this.lblAverageValue.TabIndex = 1;
            this.lblAverageValue.Text = "0,00";
            // 
            // cardMedian
            // 
            this.cardMedian.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(243)))), ((int)(((byte)(252)))));
            this.cardMedian.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardMedian.Controls.Add(this.lblMedianCaption);
            this.cardMedian.Controls.Add(this.lblMedianValue);
            this.cardMedian.Location = new System.Drawing.Point(476, 8);
            this.cardMedian.Name = "cardMedian";
            this.cardMedian.Size = new System.Drawing.Size(218, 92);
            this.cardMedian.TabIndex = 2;
            // 
            // lblMedianCaption
            // 
            this.lblMedianCaption.AutoSize = true;
            this.lblMedianCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMedianCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(104)))), ((int)(((byte)(116)))));
            this.lblMedianCaption.Location = new System.Drawing.Point(14, 12);
            this.lblMedianCaption.Name = "lblMedianCaption";
            this.lblMedianCaption.Size = new System.Drawing.Size(58, 15);
            this.lblMedianCaption.TabIndex = 0;
            this.lblMedianCaption.Text = "Медиана";
            // 
            // lblMedianValue
            // 
            this.lblMedianValue.AutoSize = true;
            this.lblMedianValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblMedianValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(85)))), ((int)(((byte)(60)))), ((int)(((byte)(140)))));
            this.lblMedianValue.Location = new System.Drawing.Point(12, 34);
            this.lblMedianValue.Name = "lblMedianValue";
            this.lblMedianValue.Size = new System.Drawing.Size(66, 41);
            this.lblMedianValue.TabIndex = 1;
            this.lblMedianValue.Text = "0,00";
            // 
            // cardVariance
            // 
            this.cardVariance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(246)))), ((int)(((byte)(238)))));
            this.cardVariance.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardVariance.Controls.Add(this.lblVarianceCaption);
            this.cardVariance.Controls.Add(this.lblVarianceValue);
            this.cardVariance.Location = new System.Drawing.Point(706, 8);
            this.cardVariance.Name = "cardVariance";
            this.cardVariance.Size = new System.Drawing.Size(218, 92);
            this.cardVariance.TabIndex = 3;
            // 
            // lblVarianceCaption
            // 
            this.lblVarianceCaption.AutoSize = true;
            this.lblVarianceCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblVarianceCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(104)))), ((int)(((byte)(116)))));
            this.lblVarianceCaption.Location = new System.Drawing.Point(14, 12);
            this.lblVarianceCaption.Name = "lblVarianceCaption";
            this.lblVarianceCaption.Size = new System.Drawing.Size(69, 15);
            this.lblVarianceCaption.TabIndex = 0;
            this.lblVarianceCaption.Text = "Дисперсия";
            // 
            // lblVarianceValue
            // 
            this.lblVarianceValue.AutoSize = true;
            this.lblVarianceValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblVarianceValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(176)))), ((int)(((byte)(106)))), ((int)(((byte)(20)))));
            this.lblVarianceValue.Location = new System.Drawing.Point(12, 34);
            this.lblVarianceValue.Name = "lblVarianceValue";
            this.lblVarianceValue.Size = new System.Drawing.Size(66, 41);
            this.lblVarianceValue.TabIndex = 1;
            this.lblVarianceValue.Text = "0,00";
            // 
            // cardSigma
            // 
            this.cardSigma.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.cardSigma.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cardSigma.Controls.Add(this.lblSigmaCaption);
            this.cardSigma.Controls.Add(this.lblSigmaValue);
            this.cardSigma.Location = new System.Drawing.Point(936, 8);
            this.cardSigma.Name = "cardSigma";
            this.cardSigma.Size = new System.Drawing.Size(218, 92);
            this.cardSigma.TabIndex = 4;
            // 
            // lblSigmaCaption
            // 
            this.lblSigmaCaption.AutoSize = true;
            this.lblSigmaCaption.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSigmaCaption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(104)))), ((int)(((byte)(116)))));
            this.lblSigmaCaption.Location = new System.Drawing.Point(14, 12);
            this.lblSigmaCaption.Name = "lblSigmaCaption";
            this.lblSigmaCaption.Size = new System.Drawing.Size(122, 15);
            this.lblSigmaCaption.TabIndex = 0;
            this.lblSigmaCaption.Text = "Отклонение (σ)";
            // 
            // lblSigmaValue
            // 
            this.lblSigmaValue.AutoSize = true;
            this.lblSigmaValue.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblSigmaValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.lblSigmaValue.Location = new System.Drawing.Point(12, 34);
            this.lblSigmaValue.Name = "lblSigmaValue";
            this.lblSigmaValue.Size = new System.Drawing.Size(66, 41);
            this.lblSigmaValue.TabIndex = 1;
            this.lblSigmaValue.Text = "0,00";
            // 
            // dgvDaily
            // 
            this.dgvDaily.AllowUserToAddRows = false;
            this.dgvDaily.AllowUserToDeleteRows = false;
            this.dgvDaily.AllowUserToResizeRows = false;
            this.dgvDaily.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.dgvDaily.AutoGenerateColumns = false;
            this.dgvDaily.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDaily.BackgroundColor = System.Drawing.Color.White;
            this.dgvDaily.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvDaily.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgvDaily.ColumnHeadersHeight = 34;
            this.dgvDaily.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgvDaily.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDay,
            this.colCount});
            this.dgvDaily.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDaily.EnableHeadersVisualStyles = false;
            this.dgvDaily.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.dgvDaily.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(235)))), ((int)(((byte)(240)))));
            this.dgvDaily.Location = new System.Drawing.Point(0, 0);
            this.dgvDaily.MultiSelect = false;
            this.dgvDaily.Name = "dgvDaily";
            this.dgvDaily.ReadOnly = true;
            this.dgvDaily.RowHeadersVisible = false;
            this.dgvDaily.RowTemplate.Height = 28;
            this.dgvDaily.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvDaily.Size = new System.Drawing.Size(640, 400);
            this.dgvDaily.TabIndex = 0;
            // 
            // colDay
            // 
            this.colDay.DataPropertyName = "DayText";
            this.colDay.FillWeight = 60F;
            this.colDay.HeaderText = "Дата";
            this.colDay.Name = "colDay";
            this.colDay.ReadOnly = true;
            // 
            // colCount
            // 
            this.colCount.DataPropertyName = "Count";
            this.colCount.FillWeight = 40F;
            this.colCount.HeaderText = "Количество записей";
            this.colCount.Name = "colCount";
            this.colCount.ReadOnly = true;
            // 
            // splitMain
            // 
            this.splitMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.splitMain.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.splitMain.Location = new System.Drawing.Point(0, 194);
            this.splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.dgvDaily);
            this.splitMain.Panel1MinSize = 380;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.panelChart);
            this.splitMain.Panel2MinSize = 300;
            this.splitMain.Size = new System.Drawing.Size(1172, 400);
            this.splitMain.SplitterDistance = 640;
            this.splitMain.SplitterWidth = 6;
            this.splitMain.TabIndex = 2;
            // 
            // panelChart
            // 
            this.panelChart.BackColor = System.Drawing.Color.White;
            this.panelChart.Controls.Add(this.formsPlot);
            this.panelChart.Controls.Add(this.lblChartTitle);
            this.panelChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelChart.Location = new System.Drawing.Point(0, 0);
            this.panelChart.Name = "panelChart";
            this.panelChart.Size = new System.Drawing.Size(526, 400);
            this.panelChart.TabIndex = 0;
            // 
            // lblChartTitle
            // 
            this.lblChartTitle.BackColor = System.Drawing.Color.White;
            this.lblChartTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblChartTitle.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblChartTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(74)))), ((int)(((byte)(105)))));
            this.lblChartTitle.Location = new System.Drawing.Point(0, 0);
            this.lblChartTitle.Name = "lblChartTitle";
            this.lblChartTitle.Padding = new System.Windows.Forms.Padding(12, 0, 0, 0);
            this.lblChartTitle.Size = new System.Drawing.Size(526, 30);
            this.lblChartTitle.TabIndex = 0;
            this.lblChartTitle.Text = "Число записей по дням";
            this.lblChartTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // formsPlot
            // 
            this.formsPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formsPlot.Location = new System.Drawing.Point(0, 30);
            this.formsPlot.Name = "formsPlot";
            this.formsPlot.Size = new System.Drawing.Size(526, 370);
            this.formsPlot.TabIndex = 1;
            // 
            // panelBottom
            // 
            this.panelBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.panelBottom.Controls.Add(this.lblFormula);
            this.panelBottom.Controls.Add(this.lblMinMax);
            this.panelBottom.Location = new System.Drawing.Point(0, 594);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1172, 46);
            this.panelBottom.TabIndex = 3;
            // 
            // lblFormula
            // 
            this.lblFormula.AutoSize = true;
            this.lblFormula.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblFormula.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblFormula.Location = new System.Drawing.Point(16, 15);
            this.lblFormula.Name = "lblFormula";
            this.lblFormula.Size = new System.Drawing.Size(700, 15);
            this.lblFormula.TabIndex = 0;
            this.lblFormula.Text = "Среднее = Σxᵢ / n;  медиана — середина упорядоченного ряда;  дисперсия D = Σ(xᵢ − x̄)² / n;  σ = √D.  В расчёт входят все дни периода, включая дни без записей.";
            // 
            // lblMinMax
            // 
            this.lblMinMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMinMax.AutoSize = true;
            this.lblMinMax.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblMinMax.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(117)))), ((int)(((byte)(125)))));
            this.lblMinMax.Location = new System.Drawing.Point(950, 15);
            this.lblMinMax.Name = "lblMinMax";
            this.lblMinMax.Size = new System.Drawing.Size(180, 15);
            this.lblMinMax.TabIndex = 1;
            this.lblMinMax.Text = "Минимум: —   Максимум: —";
            // 
            // StatisticsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panelBottom);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.panelCards);
            this.Controls.Add(this.panelTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Name = "StatisticsView";
            this.Size = new System.Drawing.Size(1172, 640);
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelCards.ResumeLayout(false);
            this.cardTotal.ResumeLayout(false);
            this.cardTotal.PerformLayout();
            this.cardAverage.ResumeLayout(false);
            this.cardAverage.PerformLayout();
            this.cardMedian.ResumeLayout(false);
            this.cardMedian.PerformLayout();
            this.cardVariance.ResumeLayout(false);
            this.cardVariance.PerformLayout();
            this.cardSigma.ResumeLayout(false);
            this.cardSigma.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDaily)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.panelChart.ResumeLayout(false);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.DateTimePicker dtpFrom;
        private System.Windows.Forms.Label lblDash;
        private System.Windows.Forms.DateTimePicker dtpTo;
        private System.Windows.Forms.Button btnThisMonth;
        private System.Windows.Forms.Button btnLastMonth;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Label lblPeriodInfo;
        private System.Windows.Forms.Panel panelCards;
        private System.Windows.Forms.Panel cardTotal;
        private System.Windows.Forms.Label lblTotalCaption;
        private System.Windows.Forms.Label lblTotalValue;
        private System.Windows.Forms.Panel cardAverage;
        private System.Windows.Forms.Label lblAverageCaption;
        private System.Windows.Forms.Label lblAverageValue;
        private System.Windows.Forms.Panel cardMedian;
        private System.Windows.Forms.Label lblMedianCaption;
        private System.Windows.Forms.Label lblMedianValue;
        private System.Windows.Forms.Panel cardVariance;
        private System.Windows.Forms.Label lblVarianceCaption;
        private System.Windows.Forms.Label lblVarianceValue;
        private System.Windows.Forms.Panel cardSigma;
        private System.Windows.Forms.Label lblSigmaCaption;
        private System.Windows.Forms.Label lblSigmaValue;
        private System.Windows.Forms.DataGridView dgvDaily;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCount;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel panelChart;
        private System.Windows.Forms.Label lblChartTitle;
        private ScottPlot.FormsPlot formsPlot;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Label lblFormula;
        private System.Windows.Forms.Label lblMinMax;
    }
}
