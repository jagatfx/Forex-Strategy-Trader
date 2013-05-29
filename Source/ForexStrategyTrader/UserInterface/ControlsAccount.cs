// Controls Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private BalanceChart balanceChart;

        /// <summary>
        /// Initializes page Account.
        /// </summary>
        private void InitializePageAccount()
        {
            TabPageAccount.Name = "tabPageAccount";
            TabPageAccount.Text = Language.T("Account");
            TabPageAccount.ImageIndex = 3;
            TabPageAccount.BackColor = LayoutColors.ColorFormBack;

            balanceChart = new BalanceChart {Parent = TabPageAccount, Dock = DockStyle.Fill};
            balanceChart.UpdateChartData(Data.BalanceData, Data.BalanceDataPoints);
            balanceChart.Invalidate();
        }

        /// <summary>
        /// Sets the colors of tab page Account.
        /// </summary>
        private void SetAccountColors()
        {
            balanceChart.SetColors();
            balanceChart.Invalidate();
        }

        /// <summary>
        /// Updates the chart.
        /// </summary>
        protected void UpdateBalanceChart(BalanceChartUnit[] balanceData, int balancePoints)
        {
            if (balanceChart.InvokeRequired)
            {
                balanceChart.BeginInvoke(new UpdateBalanceChartDelegate(UpdateBalanceChart),
                                          new object[] {balanceData, balancePoints});
            }
            else
            {
                balanceChart.UpdateChartData(balanceData, balancePoints);
                balanceChart.RefreshChart();
            }
        }

        #region Nested type: UpdateBalanceChartDelegate

        private delegate void UpdateBalanceChartDelegate(BalanceChartUnit[] balanceData, int balancePoints);

        #endregion
    }
}