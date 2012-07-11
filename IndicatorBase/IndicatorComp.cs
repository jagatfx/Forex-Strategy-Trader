// IndicatorComp Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.Drawing;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Indicator's component.
    /// </summary>
    public class IndicatorComp
    {
        /// <summary>
        /// Public constructor
        /// </summary>
        public IndicatorComp()
        {
            CompName = "Not defined";
            DataType = IndComponentType.NotDefined;
            ChartType = IndChartType.NoChart;
            ChartColor = Color.Red;
            FirstBar = 0;
            UsePreviousBar = 0;
            ShowInDynInfo = true;
            Value = new double[] { };
            PosPriceDependence = PositionPriceDependence.None;
        }

        /// <summary>
        /// The component's name
        /// </summary>
        public string CompName { get; set; }

        /// <summary>
        /// The component's data type
        /// </summary>
        public IndComponentType DataType { get; set; }

        /// <summary>
        /// The component's chart type
        /// </summary>
        public IndChartType ChartType { get; set; }

        /// <summary>
        /// The component's chart color
        /// </summary>
        public Color ChartColor { get; set; }

        /// <summary>
        /// The component's first bar
        /// </summary>
        public int FirstBar { get; set; }

        /// <summary>
        /// The indicator uses the previous bar value
        /// </summary>
        public int UsePreviousBar { get; set; }

        /// <summary>
        /// Whether the component has to be shown on dynamic info or not?
        /// </summary>
        public bool ShowInDynInfo { get; set; }

        /// <summary>
        /// Whether the component depends of the position entry price.
        /// </summary>
        public PositionPriceDependence PosPriceDependence { get; set; }

        /// <summary>
        /// The component's data value
        /// </summary>
        public double[] Value { get; set; }

        /// <summary>
        /// Returns a copy.
        /// </summary>
        public IndicatorComp Clone()
        {
            var indicatorComp = new IndicatorComp
            {
                CompName = CompName,
                DataType = DataType,
                ChartType = ChartType,
                ChartColor = ChartColor,
                FirstBar = FirstBar,
                UsePreviousBar = UsePreviousBar,
                ShowInDynInfo = ShowInDynInfo,
                PosPriceDependence = PosPriceDependence
            };


            if (Value != null)
            {
                indicatorComp.Value = new double[Value.Length];
                Value.CopyTo(indicatorComp.Value, 0);
            }

            return indicatorComp;
        }
    }
}
