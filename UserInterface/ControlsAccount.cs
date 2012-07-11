// Controls Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private BalanceChart _balanceChart;

        /// <summary>
        /// Initializes page Account.
        /// </summary>
        private void InitializePageAccount()
        {
            TabPageAccount.Name = "tabPageAccount";
            TabPageAccount.Text = Language.T("Account");
            TabPageAccount.ImageIndex = 3;
            TabPageAccount.BackColor = LayoutColors.ColorFormBack;

            _balanceChart = new BalanceChart {Parent = TabPageAccount, Dock = DockStyle.Fill};
            _balanceChart.UpdateChartData(Data.BalanceData, Data.BalanceDataPoints);
            _balanceChart.Invalidate();
        }

        /// <summary>
        /// Sets the colors of tab page Account.
        /// </summary>
        private void SetAccountColors()
        {
            _balanceChart.SetColors();
            _balanceChart.Invalidate();
        }

        /// <summary>
        /// Updates the chart.
        /// </summary>
        protected void UpdateBalanceChart(BalanceChartUnit[] balanceData, int balancePoints)
        {
            if (_balanceChart.InvokeRequired)
            {
                _balanceChart.BeginInvoke(new UpdateBalanceChartDelegate(UpdateBalanceChart),
                                          new object[] {balanceData, balancePoints});
            }
            else
            {
                _balanceChart.UpdateChartData(balanceData, balancePoints);
                _balanceChart.RefreshChart();
            }
        }

        #region Nested type: UpdateBalanceChartDelegate

        private delegate void UpdateBalanceChartDelegate(BalanceChartUnit[] balanceData, int balancePoints);

        #endregion
    }
}