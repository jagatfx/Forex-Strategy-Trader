// Operation Tab
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public partial class Controls
    {
        private Button BtnBuy { get; set; }
        private Button BtnClose { get; set; }
        private Button BtnModify { get; set; }
        private Button BtnSell { get; set; }
        private Color ColorParameter { get; set; }
        private Label LblBidAsk { get; set; }
        private Label LblBreakEven { get; set; }
        private Label LblLots { get; set; }
        private Label LblStopLoss { get; set; }
        private Label LblSymbol { get; set; }
        private Label LblTakeProfit { get; set; }
        private Label LblTrailingStop { get; set; }
        private NumericUpDown NudBreakEven { get; set; }
        private NumericUpDown NudLots { get; set; }
        private NumericUpDown NudStopLoss { get; set; }
        private NumericUpDown NudTakeProfit { get; set; }
        private NumericUpDown NudTrailingStop { get; set; }
        private Panel PnlHolder { get; set; }
        private FancyPanel PnlManualTrade { get; set; }
        private TickChart TickChart { get; set; }

        protected double OperationLots
        {
            get { return (double) NudLots.Value; }
        }

        protected int OperationStopLoss
        {
            get { return (int) NudStopLoss.Value; }
        }

        protected int OperationTakeProfit
        {
            get { return (int) NudTakeProfit.Value; }
        }

        protected int OperationBreakEven
        {
            get { return (int) NudBreakEven.Value; }
        }

        protected int OperationTrailingStop
        {
            get { return (int) NudTrailingStop.Value; }
        }

        /// <summary>
        ///     Initializes Operation tab page.
        /// </summary>
        private void InitializePageOperation()
        {
            TabPageOperation.Name = "tabPageOperation";
            TabPageOperation.Text = Language.T("Operation");
            TabPageOperation.ImageIndex = 5;
            TabPageOperation.BackColor = LayoutColors.ColorFormBack;

            PnlManualTrade = new FancyPanel(Language.T("Manual Operation Execution"))
                {Parent = TabPageOperation, Dock = DockStyle.Fill};
            PnlManualTrade.Resize += PnlManualTradeResize;

            PnlHolder = new Panel { Parent = PnlManualTrade, BackColor = Color.Transparent, Size = new Size((int)(750 * Data.HDpiScale), 350) };

            LblSymbol = new Label
                {
                    Parent = PnlHolder,
                    Text = "Symbol",
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Font = new Font(Font.FontFamily, 18, FontStyle.Bold)
                };
            LblSymbol.Height = LblSymbol.Font.Height;
            LblSymbol.Width = (int) (180*Data.HDpiScale);
            LblSymbol.TextAlign = ContentAlignment.MiddleRight;
            LblSymbol.Location = new Point(5, 35);

            LblBidAsk = new Label
                {
                    Parent = PnlHolder,
                    Text = "Bid / Ask",
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Font = new Font(Font.FontFamily, 18, FontStyle.Bold),
                    Width = 295,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Location = new Point(LblSymbol.Right + Space, 35)
                };
            LblBidAsk.Height = LblBidAsk.Font.Height;

            LblLots = new Label
                {
                    Parent = PnlHolder,
                    Text = Language.T("Lots"),
                    Font = new Font(Font.FontFamily, 11),
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Width = (int) (90*Data.HDpiScale)
                };
            LblLots.Height = LblLots.Font.Height;
            LblLots.TextAlign = ContentAlignment.MiddleRight;
            LblLots.Location = new Point(5, 81);
            var lblWidth = (int) (100*Data.HDpiScale);
            LblStopLoss = new Label
                {
                    Parent = PnlHolder,
                    Text = Language.T("Stop Loss"),
                    Font = new Font(Font.FontFamily, 11),
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Location = new Point(5, 121),
                    Width = lblWidth,
                    TextAlign = ContentAlignment.MiddleRight
                };

            LblTakeProfit = new Label
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    Text = Language.T("Take Profit"),
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Location = new Point(5, 151),
                    Width = lblWidth,
                    TextAlign = ContentAlignment.MiddleRight
                };

            LblBreakEven = new Label
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    Text = Language.T("Break Even"),
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Location = new Point(5, 191),
                    Width = lblWidth,
                    TextAlign = ContentAlignment.MiddleRight
                };

            LblTrailingStop = new Label
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    Text = Language.T("Trailing Stop"),
                    BackColor = Color.Transparent,
                    ForeColor = LayoutColors.ColorControlText,
                    Location = new Point(5, 221),
                    Width = lblWidth,
                    TextAlign = ContentAlignment.MiddleRight
                };


            int nudLeft = LblTrailingStop.Right + Space;

            NudLots = new NumericUpDown
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    TextAlign = HorizontalAlignment.Center,
                    Width = 80,
                    Location = new Point(nudLeft, 81)
                };
            NudLots.BeginInit();
            NudLots.Minimum = 0.1M;
            NudLots.Maximum = 100;
            NudLots.Increment = 0.1M;
            NudLots.Value = 1;
            NudLots.DecimalPlaces = 1;
            NudLots.EndInit();

            NudStopLoss = new NumericUpDown
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    TextAlign = HorizontalAlignment.Center,
                    Width = 80,
                    Location = new Point(nudLeft, 121)
                };
            NudStopLoss.BeginInit();
            NudStopLoss.Minimum = 0;
            NudStopLoss.Maximum = 5000;
            NudStopLoss.Increment = 1;
            NudStopLoss.Value = 0;
            NudStopLoss.DecimalPlaces = 0;
            NudStopLoss.EndInit();
            NudStopLoss.ValueChanged += ParameterValueChanged;

            ColorParameter = NudStopLoss.ForeColor;

            NudTakeProfit = new NumericUpDown
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    TextAlign = HorizontalAlignment.Center,
                    Width = 80,
                    Location = new Point(nudLeft, 151)
                };
            NudTakeProfit.BeginInit();
            NudTakeProfit.Minimum = 0;
            NudTakeProfit.Maximum = 5000;
            NudTakeProfit.Increment = 1;
            NudTakeProfit.Value = 0;
            NudTakeProfit.DecimalPlaces = 0;
            NudTakeProfit.EndInit();
            NudTakeProfit.ValueChanged += ParameterValueChanged;

            NudBreakEven = new NumericUpDown
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    TextAlign = HorizontalAlignment.Center,
                    Width = 80,
                    Location = new Point(nudLeft, 191)
                };
            NudBreakEven.BeginInit();
            NudBreakEven.Minimum = 0;
            NudBreakEven.Maximum = 5000;
            NudBreakEven.Increment = 1;
            NudBreakEven.Value = 0;
            NudBreakEven.DecimalPlaces = 0;
            NudBreakEven.EndInit();
            NudBreakEven.ValueChanged += ParameterValueChanged;

            NudTrailingStop = new NumericUpDown
                {
                    Parent = PnlHolder,
                    Font = new Font(Font.FontFamily, 11),
                    TextAlign = HorizontalAlignment.Center,
                    Width = 80,
                    Location = new Point(nudLeft, 221)
                };
            NudTrailingStop.BeginInit();
            NudTrailingStop.Minimum = 0;
            NudTrailingStop.Maximum = 5000;
            NudTrailingStop.Increment = 1;
            NudTrailingStop.Value = 0;
            NudTrailingStop.DecimalPlaces = 0;
            NudTrailingStop.EndInit();
            NudTrailingStop.ValueChanged += ParameterValueChanged;

            int btnLeft = NudTrailingStop.Right + 2 * Space;

            BtnSell = new Button
                {
                    Name = "btnSell",
                    Parent = PnlHolder,
                    Image = Resources.btn_operation_sell,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Text = Language.T("Sell"),
                    Width = 145,
                    Height = 40,
                    Font = new Font(Font.FontFamily, 16),
                    ForeColor = Color.Crimson,
                    Location = new Point(btnLeft, 80),
                    UseVisualStyleBackColor = true
                };
            BtnSell.Click += BtnOperationClick;

            BtnBuy = new Button
                {
                    Name = "btnBuy",
                    Parent = PnlHolder,
                    Image = Resources.btn_operation_buy,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Text = Language.T("Buy"),
                    Width = 145,
                    Height = 40,
                    Font = new Font(Font.FontFamily, 16),
                    ForeColor = Color.Green,
                    Location = new Point(BtnSell.Right + Space, 80),
                    UseVisualStyleBackColor = true
                };
            BtnBuy.Click += BtnOperationClick;

            BtnClose = new Button
                {
                    Name = "btnClose",
                    Parent = PnlHolder,
                    Image = Resources.btn_operation_close,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Text = Language.T("Close"),
                    Width = 295,
                    Height = 40,
                    Font = new Font(Font.FontFamily, 16, FontStyle.Bold),
                    ForeColor = Color.DarkOrange,
                    Location = new Point(btnLeft, 126),
                    UseVisualStyleBackColor = true
                };
            BtnClose.Click += BtnOperationClick;

            BtnModify = new Button
                {
                    Name = "btnModify",
                    Parent = PnlHolder,
                    Image = Resources.recalculate,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    Text = Language.T("Modify Stop Loss and Take Profit"),
                    ForeColor = Color.Navy,
                    Width = 295,
                    Location = new Point(btnLeft, 172),
                    UseVisualStyleBackColor = true
                };
            BtnModify.Click += BtnOperationClick;

            int chartLeft = BtnModify.Right + 2 * Space;
            TickChart = new TickChart(Language.T("Tick Chart"))
                {
                    Parent = PnlHolder,
                    Size = new Size(250, 200),
                    Location = new Point(chartLeft, 81)
                };
        }

        /// <summary>
        ///     Sets the controls position on resizing.
        /// </summary>
        private void PnlManualTradeResize(object sender, EventArgs e)
        {
            if (PnlHolder.Width < PnlManualTrade.Width)
            {
                int shift = (PnlManualTrade.Width - PnlHolder.Width)/2;
                PnlHolder.Location = new Point(shift, 0);
            }
            else
                PnlHolder.Location = new Point(0, 0);
        }

        /// <summary>
        ///     Sets the lot parameters.
        /// </summary>
        protected void SetNumUpDownLots(double minlot, double lotstep, double maxlot)
        {
            NudLots.BeginInit();
            NudLots.Minimum = (decimal) minlot;
            NudLots.Increment = (decimal) lotstep;
            NudLots.Maximum = (decimal) maxlot;
            NudLots.DecimalPlaces = lotstep < 0.1 ? 2 : lotstep < 1 ? 1 : 0;
            NudLots.EndInit();
        }

        /// <summary>
        ///     Execute operation
        /// </summary>
        public virtual void BtnOperationClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Validates the Stop or Limit parameters.
        /// </summary>
        private void ParameterValueChanged(object sender, EventArgs e)
        {
            var nud = (NumericUpDown) sender;
            if (nud.Value > 0 && (double) nud.Value < Data.InstrProperties.StopLevel)
                nud.ForeColor = Color.Red;
            else
                nud.ForeColor = ColorParameter;
        }

        /// <summary>
        ///     Sets the colors of tab page Operation.
        /// </summary>
        private void SetOperationColors()
        {
            TabPageOperation.BackColor = LayoutColors.ColorFormBack;
            PnlManualTrade.SetColors();
            LblBidAsk.ForeColor = LayoutColors.ColorControlText;
            LblSymbol.ForeColor = LayoutColors.ColorControlText;
            LblLots.ForeColor = LayoutColors.ColorControlText;
            LblStopLoss.ForeColor = LayoutColors.ColorControlText;
            LblTakeProfit.ForeColor = LayoutColors.ColorControlText;
            LblBreakEven.ForeColor = LayoutColors.ColorControlText;
            LblTrailingStop.ForeColor = LayoutColors.ColorControlText;
            TickChart.SetColors();
        }

        /// <summary>
        ///     Sets the lblBidAsk.Text
        /// </summary>
        protected void SetLblBidAskText(string text)
        {
            if (LblBidAsk.InvokeRequired)
            {
                LblBidAsk.BeginInvoke(new SetLblBidAskTextDelegate(SetLblBidAskText), new object[] {text});
            }
            else
            {
                LblBidAsk.Text = text;
            }
        }

        /// <summary>
        ///     Sets the lblSymbol.Text
        /// </summary>
        protected void SetLblSymbolText(string text)
        {
            if (LblSymbol.InvokeRequired)
            {
                LblSymbol.BeginInvoke(new SetLblBidAskTextDelegate(SetLblSymbolText), new object[] {text});
            }
            else
            {
                LblSymbol.Text = text;
            }
        }

        /// <summary>
        ///     Updates the Tick Chart.
        /// </summary>
        protected void UpdateTickChart(double point, double[] tickList)
        {
            if (TickChart.InvokeRequired)
            {
                TickChart.BeginInvoke(new UpdateTickChartDelegate(UpdateTickChart), new object[] {point, tickList});
            }
            else
            {
                TickChart.UpdateChartData(point, tickList);
                TickChart.RefreshChart();
            }
        }

        #region Nested type: SetLblBidAskTextDelegate

        private delegate void SetLblBidAskTextDelegate(string text);

        #endregion

        #region Nested type: UpdateTickChartDelegate

        private delegate void UpdateTickChartDelegate(double point, double[] tickList);

        #endregion
    }
}