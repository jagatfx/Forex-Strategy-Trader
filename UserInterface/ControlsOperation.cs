// Operation Tab
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
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
        private NumericUpDown NUDBreakEven { get; set; }
        private NumericUpDown NUDLots { get; set; }
        private NumericUpDown NUDStopLoss { get; set; }
        private NumericUpDown NUDTakeProfit { get; set; }
        private NumericUpDown NUDTrailingStop { get; set; }
        private Panel PnlHolder { get; set; }
        private FancyPanel PnlManualTrade { get; set; }
        private TickChart TickChart { get; set; }

        protected double OperationLots
        {
            get { return (double) NUDLots.Value; }
        }

        protected int OperationStopLoss
        {
            get { return (int) NUDStopLoss.Value; }
        }

        protected int OperationTakeProfit
        {
            get { return (int) NUDTakeProfit.Value; }
        }

        protected int OperationBreakEven
        {
            get { return (int) NUDBreakEven.Value; }
        }

        protected int OperationTrailingStop
        {
            get { return (int) NUDTrailingStop.Value; }
        }

        /// <summary>
        /// Initializes Operation tab page.
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

            PnlHolder = new Panel {Parent = PnlManualTrade, BackColor = Color.Transparent, Size = new Size(750, 350)};

            LblBidAsk = new Label
                            {
                                Parent = PnlHolder,
                                Text = "Bid / Ask",
                                BackColor = Color.Transparent,
                                ForeColor = LayoutColors.ColorControlText,
                                Font = new Font(Font.FontFamily, 18, FontStyle.Bold),
                                Width = 295,
                                TextAlign = ContentAlignment.MiddleCenter,
                                Location = new Point(190, 35)
                            };

            LblSymbol = new Label
                            {
                                Parent = PnlHolder,
                                Text = "Symbol",
                                BackColor = Color.Transparent,
                                ForeColor = LayoutColors.ColorControlText,
                                Font = new Font(Font.FontFamily, 18, FontStyle.Bold)
                            };
            LblSymbol.Height = LblSymbol.Font.Height;
            LblSymbol.Width = 180;
            LblSymbol.TextAlign = ContentAlignment.MiddleRight;
            LblSymbol.Location = new Point(5, 35);

            LblLots = new Label
                          {
                              Parent = PnlHolder,
                              Text = Language.T("Lots"),
                              Font = new Font(Font.FontFamily, 11),
                              BackColor = Color.Transparent,
                              ForeColor = LayoutColors.ColorControlText,
                              Width = 90
                          };
            LblLots.Height = LblLots.Font.Height;
            LblLots.TextAlign = ContentAlignment.MiddleRight;
            LblLots.Location = new Point(5, 81);

            LblStopLoss = new Label
                              {
                                  Parent = PnlHolder,
                                  Text = Language.T("Stop Loss"),
                                  Font = new Font(Font.FontFamily, 11),
                                  BackColor = Color.Transparent,
                                  ForeColor = LayoutColors.ColorControlText,
                                  Location = new Point(5, 121),
                                  Width = 90,
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
                                    Width = 90,
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
                                   Width = 90,
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
                                      Width = 90,
                                      TextAlign = ContentAlignment.MiddleRight
                                  };

            NUDLots = new NumericUpDown
                          {
                              Parent = PnlHolder,
                              Font = new Font(Font.FontFamily, 11),
                              TextAlign = HorizontalAlignment.Center,
                              Width = 80,
                              Location = new Point(100, 81)
                          };
            NUDLots.BeginInit();
            NUDLots.Minimum = 0.1M;
            NUDLots.Maximum = 100;
            NUDLots.Increment = 0.1M;
            NUDLots.Value = 1;
            NUDLots.DecimalPlaces = 1;
            NUDLots.EndInit();

            NUDStopLoss = new NumericUpDown
                              {
                                  Parent = PnlHolder,
                                  Font = new Font(Font.FontFamily, 11),
                                  TextAlign = HorizontalAlignment.Center,
                                  Width = 80,
                                  Location = new Point(100, 121)
                              };
            NUDStopLoss.BeginInit();
            NUDStopLoss.Minimum = 0;
            NUDStopLoss.Maximum = 5000;
            NUDStopLoss.Increment = 1;
            NUDStopLoss.Value = 0;
            NUDStopLoss.DecimalPlaces = 0;
            NUDStopLoss.EndInit();
            NUDStopLoss.ValueChanged += ParameterValueChanged;

            ColorParameter = NUDStopLoss.ForeColor;

            NUDTakeProfit = new NumericUpDown
                                {
                                    Parent = PnlHolder,
                                    Font = new Font(Font.FontFamily, 11),
                                    TextAlign = HorizontalAlignment.Center,
                                    Width = 80,
                                    Location = new Point(100, 151)
                                };
            NUDTakeProfit.BeginInit();
            NUDTakeProfit.Minimum = 0;
            NUDTakeProfit.Maximum = 5000;
            NUDTakeProfit.Increment = 1;
            NUDTakeProfit.Value = 0;
            NUDTakeProfit.DecimalPlaces = 0;
            NUDTakeProfit.EndInit();
            NUDTakeProfit.ValueChanged += ParameterValueChanged;

            NUDBreakEven = new NumericUpDown
                               {
                                   Parent = PnlHolder,
                                   Font = new Font(Font.FontFamily, 11),
                                   TextAlign = HorizontalAlignment.Center,
                                   Width = 80,
                                   Location = new Point(100, 191)
                               };
            NUDBreakEven.BeginInit();
            NUDBreakEven.Minimum = 0;
            NUDBreakEven.Maximum = 5000;
            NUDBreakEven.Increment = 1;
            NUDBreakEven.Value = 0;
            NUDBreakEven.DecimalPlaces = 0;
            NUDBreakEven.EndInit();
            NUDBreakEven.ValueChanged += ParameterValueChanged;

            NUDTrailingStop = new NumericUpDown
                                  {
                                      Parent = PnlHolder,
                                      Font = new Font(Font.FontFamily, 11),
                                      TextAlign = HorizontalAlignment.Center,
                                      Width = 80,
                                      Location = new Point(100, 221)
                                  };
            NUDTrailingStop.BeginInit();
            NUDTrailingStop.Minimum = 0;
            NUDTrailingStop.Maximum = 5000;
            NUDTrailingStop.Increment = 1;
            NUDTrailingStop.Value = 0;
            NUDTrailingStop.DecimalPlaces = 0;
            NUDTrailingStop.EndInit();
            NUDTrailingStop.ValueChanged += ParameterValueChanged;

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
                              Location = new Point(190, 80),
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
                             Location = new Point(340, 80),
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
                               Location = new Point(190, 126),
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
                                Location = new Point(190, 172),
                                UseVisualStyleBackColor = true
                            };
            BtnModify.Click += BtnOperationClick;

            TickChart = new TickChart(Language.T("Tick Chart"))
                            {Parent = PnlHolder, Size = new Size(250, 200), Location = new Point(495, 81)};
        }

        /// <summary>
        /// Sets the controls position on resizing.
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
        /// Sets the lot parameters.
        /// </summary>
        protected void SetNumUpDownLots(double minlot, double lotstep, double maxlot)
        {
            NUDLots.BeginInit();
            NUDLots.Minimum = (decimal) minlot;
            NUDLots.Increment = (decimal) lotstep;
            NUDLots.Maximum = (decimal) maxlot;
            NUDLots.DecimalPlaces = lotstep < 0.1 ? 2 : lotstep < 1 ? 1 : 0;
            NUDLots.EndInit();
        }

        /// <summary>
        /// Execute operation
        /// </summary>
        public virtual void BtnOperationClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Validates the Stop or Limit parameters.
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
        /// Sets the colors of tab page Operation.
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
        /// Sets the lblBidAsk.Text
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
        /// Sets the lblSymbol.Text
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
        /// Updates the Tick Chart.
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