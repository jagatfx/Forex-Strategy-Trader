// Trade Settings
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public sealed class TradeSettings : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public TradeSettings()
        {
            PnlSettings = new FancyPanel();

            LblCloseAdvance = new Label();
            LblSlippageEntry = new Label();

            CbxLongLogicPrice = new ComboBox();
            ChbAutoSlippage = new CheckBox();
            NUDCloseAdvance = new NumericUpDown();
            NUDSlippageEntry = new NumericUpDown();
            NUDSlippageExit = new NumericUpDown();
            LblSlippageExit = new Label();
            LblLongLogicPrice = new Label();
            LblMinChartBars = new Label();
            NUDMinChartBars = new NumericUpDown();

            BtnDefault = new Button();
            BtnCancel = new Button();
            BtnAccept = new Button();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Trade Settings");

            // pnlAveraging
            PnlSettings.Parent = this;

            // ComboBox Long Logic Price
            CbxLongLogicPrice.Parent = PnlSettings;
            CbxLongLogicPrice.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxLongLogicPrice.Items.AddRange(new object[] {"Bid", "Ask", "Close"});
            CbxLongLogicPrice.Text = Configs.LongTradeLogicPrice;

            // Label close advance
            LblCloseAdvance.Parent = PnlSettings;
            LblCloseAdvance.ForeColor = LayoutColors.ColorControlText;
            LblCloseAdvance.BackColor = Color.Transparent;
            LblCloseAdvance.AutoSize = true;
            LblCloseAdvance.Text = Language.T("'Bar Closing' time advance in seconds");

            // Check Box Auto Slippage
            ChbAutoSlippage.Parent = PnlSettings;
            ChbAutoSlippage.ForeColor = LayoutColors.ColorControlText;
            ChbAutoSlippage.BackColor = Color.Transparent;
            ChbAutoSlippage.AutoSize = true;
            ChbAutoSlippage.Checked = Configs.AutoSlippage;
            ChbAutoSlippage.Text = Language.T("Auto slippage depending on the spread.");
            ChbAutoSlippage.CheckedChanged += ChbAutoSlippageCheckedChanged;

            // Label Entry slippage
            LblSlippageEntry.Parent = PnlSettings;
            LblSlippageEntry.ForeColor = LayoutColors.ColorControlText;
            LblSlippageEntry.BackColor = Color.Transparent;
            LblSlippageEntry.AutoSize = true;
            LblSlippageEntry.Text = Language.T("Slippage for entry orders");

            // Label Entry slippage
            LblSlippageExit.Parent = PnlSettings;
            LblSlippageExit.ForeColor = LayoutColors.ColorControlText;
            LblSlippageExit.BackColor = Color.Transparent;
            LblSlippageExit.AutoSize = true;
            LblSlippageExit.Text = Language.T("Slippage for exit orders");

            // NumericUpDown Entry Lots
            NUDCloseAdvance.Parent = PnlSettings;
            NUDCloseAdvance.BeginInit();
            NUDCloseAdvance.Minimum = 1;
            NUDCloseAdvance.Maximum = 15;
            NUDCloseAdvance.Increment = 1;
            NUDCloseAdvance.Value = Configs.BarCloseAdvance;
            NUDCloseAdvance.DecimalPlaces = 0;
            NUDCloseAdvance.TextAlign = HorizontalAlignment.Center;
            NUDCloseAdvance.EndInit();

            // Label lblLongLogicPrice
            LblLongLogicPrice.Parent = PnlSettings;
            LblLongLogicPrice.ForeColor = LayoutColors.ColorControlText;
            LblLongLogicPrice.BackColor = Color.Transparent;
            LblLongLogicPrice.AutoSize = true;
            LblLongLogicPrice.Text = Language.T("Long logic rules base price");

            // NUD Entry slippage
            NUDSlippageEntry.Parent = PnlSettings;
            NUDSlippageEntry.BeginInit();
            NUDSlippageEntry.Minimum = 0;
            NUDSlippageEntry.Maximum = 1000;
            NUDSlippageEntry.Increment = 1;
            NUDSlippageEntry.Value = Configs.SlippageEntry;
            NUDSlippageEntry.DecimalPlaces = 0;
            NUDSlippageEntry.TextAlign = HorizontalAlignment.Center;
            NUDSlippageEntry.Enabled = !Configs.AutoSlippage;
            NUDSlippageEntry.EndInit();

            // NUD Exit slippage
            NUDSlippageExit.Parent = PnlSettings;
            NUDSlippageExit.BeginInit();
            NUDSlippageExit.Minimum = 0;
            NUDSlippageExit.Maximum = 1000;
            NUDSlippageExit.Increment = 1;
            NUDSlippageExit.Value = Configs.SlippageExit;
            NUDSlippageExit.DecimalPlaces = 0;
            NUDSlippageExit.TextAlign = HorizontalAlignment.Center;
            NUDSlippageExit.Enabled = !Configs.AutoSlippage;
            NUDSlippageExit.EndInit();

            // Label lblMinChartBars
            LblMinChartBars.Parent = PnlSettings;
            LblMinChartBars.ForeColor = LayoutColors.ColorControlText;
            LblMinChartBars.BackColor = Color.Transparent;
            LblMinChartBars.AutoSize = true;
            LblMinChartBars.Text = Language.T("Minimum number of bars in the chart");

            // NUD Exit slippage
            NUDMinChartBars.Parent = PnlSettings;
            NUDMinChartBars.BeginInit();
            NUDMinChartBars.Minimum = 300;
            NUDMinChartBars.Maximum = 5000;
            NUDMinChartBars.Increment = 1;
            NUDMinChartBars.Value = Configs.MinChartBars;
            NUDMinChartBars.DecimalPlaces = 0;
            NUDMinChartBars.TextAlign = HorizontalAlignment.Center;
            NUDMinChartBars.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "Default";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.Click += BtnAcceptClick;
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlSettings { get; set; }

        private Label LblCloseAdvance { get; set; }
        private Label LblSlippageEntry { get; set; }
        private Label LblSlippageExit { get; set; }
        private Label LblLongLogicPrice { get; set; }
        private Label LblMinChartBars { get; set; }

        private ComboBox CbxLongLogicPrice { get; set; }
        private CheckBox ChbAutoSlippage { get; set; }
        private NumericUpDown NUDCloseAdvance { get; set; }
        private NumericUpDown NUDSlippageEntry { get; set; }
        private NumericUpDown NUDSlippageExit { get; set; }
        private NumericUpDown NUDMinChartBars { get; set; }

        private Button BtnDefault { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }

        /// <summary>
        /// Changes the visibility of NUD slippage.
        /// </summary>
        private void ChbAutoSlippageCheckedChanged(object sender, EventArgs e)
        {
            NUDSlippageEntry.Enabled = !ChbAutoSlippage.Checked;
            NUDSlippageExit.Enabled = !ChbAutoSlippage.Checked;
        }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);

            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 245);

            BtnAccept.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int hrzSpace = btnHrzSpace;
            const int border = 2;

            PnlSettings.Size = new Size(ClientSize.Width - 2*hrzSpace, ClientSize.Height - hrzSpace - buttonHeight - 2*btnVertSpace);
            PnlSettings.Location = new Point(hrzSpace, hrzSpace);

            const int cBxWith = 60;
            const int nudWidth = 60;
            int cBxLeft = PnlSettings.ClientSize.Width - cBxWith - hrzSpace - border;
            int nudLeft = PnlSettings.ClientSize.Width - nudWidth - btnHrzSpace - border;

            LblLongLogicPrice.Location = new Point(btnHrzSpace, 19);
            CbxLongLogicPrice.Width = cBxWith;
            CbxLongLogicPrice.Location = new Point(cBxLeft, 15);

            LblCloseAdvance.Location = new Point(btnHrzSpace, 47);
            NUDCloseAdvance.Size = new Size(nudWidth, buttonHeight);
            NUDCloseAdvance.Location = new Point(nudLeft, 45);

            ChbAutoSlippage.Location = new Point(btnHrzSpace + 3, 77);

            LblSlippageEntry.Location = new Point(btnHrzSpace, 107);
            NUDSlippageEntry.Size = new Size(nudWidth, buttonHeight);
            NUDSlippageEntry.Location = new Point(nudLeft, 105);

            LblSlippageExit.Location = new Point(btnHrzSpace, 137);
            NUDSlippageExit.Size = new Size(nudWidth, buttonHeight);
            NUDSlippageExit.Location = new Point(nudLeft, 135);

            LblMinChartBars.Location = new Point(btnHrzSpace, 167);
            NUDMinChartBars.Size = new Size(nudWidth, buttonHeight);
            NUDMinChartBars.Location = new Point(nudLeft, 165);

            int buttonWidth = (PnlSettings.Width - 2*btnHrzSpace)/3;

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight); 
            BtnAccept.Location = new Point(BtnDefault.Left - buttonWidth - btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            CbxLongLogicPrice.Text = "Close";
            NUDCloseAdvance.Value = 3;
            NUDSlippageEntry.Value = 5;
            NUDSlippageExit.Value = 10;
            NUDMinChartBars.Value = 400;
        }

        /// <summary>
        /// Button Default Click
        /// </summary>
        private void BtnAcceptClick(object sender, EventArgs e)
        {
            Configs.LongTradeLogicPrice = CbxLongLogicPrice.Text;
            Configs.BarCloseAdvance = (int) NUDCloseAdvance.Value;
            Configs.AutoSlippage = ChbAutoSlippage.Checked;
            Configs.SlippageEntry = (int) NUDSlippageEntry.Value;
            Configs.SlippageExit = (int) NUDSlippageExit.Value;
            Configs.MinChartBars = (int) NUDMinChartBars.Value;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}