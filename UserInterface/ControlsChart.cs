// Controls - ChartPage
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        Chart _chart;

        /// <summary>
        /// Initializes page Chart.
        /// </summary>
        private void InitializePageChart()
        {
            TabPageChart.Name = "tabPageChart";
            TabPageChart.Text = Language.T("Chart");
            TabPageChart.ImageIndex = 2;
        }

        /// <summary>
        /// Chart tab page is shown.
        /// </summary>
        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_chart == null) return;
            ChartData chartData = GetChartDataObject();
            _chart.InitChart(chartData);
        }

        /// <summary>
        /// Creates a new chart.
        /// </summary>
        void CreateChart()
        {
            if (TabControlBase.SelectedTab != TabPageChart) return;
            ChartData chartData = GetChartDataObject();
            _chart = new Chart(chartData) {Parent = TabPageChart, Dock = DockStyle.Fill};
            _chart.InitChart(chartData);
        }

        /// <summary>
        /// Disposes the chart.
        /// </summary>
        private void DisposeChart()
        {
            if (_chart != null)
            {
                try
                {
                    _chart.Dispose();
                }
                finally
                {
                    _chart = null;
                }
            }
        }

        DateTime    _chartTime   = DateTime.Now;
        DateTime    _chartTime10 = DateTime.Now;
        string      _chartSymbol = "";
        DataPeriods _chartPeriod = DataPeriods.day;
        int         _chartBars;


        /// <summary>
        /// Updates the chart.
        /// </summary>
        protected void UpdateChart()
        {
            if (_chart == null)
                return;
           
            bool repaintChart = (
                _chartSymbol != Data.Symbol ||
                _chartPeriod != Data.Period ||
                _chartBars   != Data.Bars   || 
                _chartTime   != Data.Time[Data.Bars - 1] ||
                _chartTime10 != Data.Time[Data.Bars - 11]);

            _chartSymbol = Data.Symbol;
            _chartPeriod = Data.Period;
            _chartBars   = Data.Bars;
            _chartTime   = Data.Time[Data.Bars - 1];
            _chartTime10 = Data.Time[Data.Bars - 11];

            // Prepares chart data.
            ChartData chartData = GetChartDataObject();

            try
            {
                UpdateChartThreadSafely(repaintChart, chartData);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Language.T("Indicator Chart"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                DisposeChart();
                CreateChart();
            }
        }

        delegate void UpdateChartDelegate(bool repaintChart, ChartData chartData);
        void UpdateChartThreadSafely(bool repaintChart, ChartData chartData)
        {
            if (_chart.InvokeRequired)
            {
                _chart.BeginInvoke(new UpdateChartDelegate(UpdateChartThreadSafely), new object[] { repaintChart, chartData });
            }
            else
            {
                _chart.UpdateChartOnTick(repaintChart, chartData);
            }
        }

        ChartData GetChartDataObject()
        {
            var chartData = new ChartData
                                {
                                    InstrumentProperties = Data.InstrProperties.Clone(),
                                    Bars = Data.Bars,
                                    Time = new DateTime[Data.Bars],
                                    Open = new double[Data.Bars],
                                    High = new double[Data.Bars],
                                    Low = new double[Data.Bars],
                                    Close = new double[Data.Bars],
                                    Volume = new int[Data.Bars]
                                };
            Data.Time.CopyTo(chartData.Time, 0);
            Data.Open.CopyTo(chartData.Open, 0);
            Data.High.CopyTo(chartData.High, 0);
            Data.Low.CopyTo(chartData.Low, 0);
            Data.Close.CopyTo(chartData.Close, 0);
            Data.Volume.CopyTo(chartData.Volume, 0);
            chartData.StrategyName = Data.StrategyName;
            chartData.Strategy  = Data.Strategy.Clone();
            chartData.FirstBar  = Data.FirstBar;
            chartData.Symbol    = Data.Symbol;
            chartData.PeriodStr = Data.PeriodStr;
            chartData.Bid       = Data.Bid;
            chartData.BarStatistics = new Dictionary<DateTime, BarStats>();
            foreach (KeyValuePair<DateTime, BarStats> timeStat in Data.BarStatistics)
                chartData.BarStatistics.Add(timeStat.Key, timeStat.Value.Clone());
            chartData.PositionDirection  = Data.PositionDirection;
            chartData.PositionOpenPrice  = Data.PositionOpenPrice;
            chartData.PositionProfit     = Data.PositionProfit;
            chartData.PositionStopLoss   = Data.PositionStopLoss;
            chartData.PositionTakeProfit = Data.PositionTakeProfit;

            return chartData;
        }

        /// <summary>
        /// Sets colors of the chart.
        /// </summary>
        void SetChartColors()
        {
            DisposeChart();
            CreateChart();
        }
    }
}
