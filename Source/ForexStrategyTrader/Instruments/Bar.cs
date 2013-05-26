// Bar Structure
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Bar structure.
    /// </summary>
    public struct Bar
    {
        public DateTime Time { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public int Volume { get; set; }

        public override string ToString()
        {
            return String.Format("{0:D2}.{1:D2}.{2:D4}\t{3:D2}:{4:D2}\t{5:F5}\t{6:F5}\t{7:F5}\t{8:F5}\t{9:D6}",
                                 Time.Day, Time.Month, Time.Year, Time.Hour, Time.Minute, Open, High, Low, Close, Volume);
        }
    }
}