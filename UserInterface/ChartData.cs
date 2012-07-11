// Chart_Data Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    public class ChartData
    {
        public string StrategyName { get; set; }
        public string Symbol { get; set; }
        public string PeriodStr { get; set; }
        public double Bid { get; set; }
        public PosDirection PositionDirection { get; set; }
        public double PositionOpenPrice { get; set; }
        public double PositionProfit { get; set; }
        public double PositionTakeProfit { get; set; }
        public double PositionStopLoss { get; set; }
        public Instrument_Properties InstrumentProperties { get; set; }
        public int Bars { get; set; }
        public DateTime[] Time { get; set; }
        public double[] Open { get; set; }
        public double[] High { get; set; }
        public double[] Low { get; set; }
        public double[] Close { get; set; }
        public int[] Volume { get; set; }
        public int FirstBar { get; set; }
        public Strategy Strategy { get; set; }
        public Dictionary<DateTime, BarStats> BarStatistics { get; set; }
    }
}