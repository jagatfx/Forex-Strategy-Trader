// Instrument Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    public class Instrument
    {
        readonly InstrumentProperties _instrProperties;
        readonly int _period;
        int _bars;
        Bar[] _dataBar;

        // General instrument info
        public string   Symbol { get { return _instrProperties.Symbol; } }
        public int      Period { get { return _period; } }
        public double   Point  { get { return _instrProperties.Point; } }
        public int      Bars   { get { return _bars; } }
        
        // Bar info
        public DateTime	Time	(int iBar) { return _dataBar[iBar].Time  ; }
        public double	Open	(int iBar) { return _dataBar[iBar].Open  ; }
        public double	High	(int iBar) { return _dataBar[iBar].High  ; }
        public double	Low		(int iBar) { return _dataBar[iBar].Low   ; }
        public double	Close	(int iBar) { return _dataBar[iBar].Close ; }
        public int		Volume	(int iBar) { return _dataBar[iBar].Volume; }

        /// <summary>
        /// Constructor
        /// </summary>
        public Instrument(InstrumentProperties instrProperties, int iPeriod)
        {
            _instrProperties = instrProperties;
            _period = iPeriod;
        }

        /// <summary>
        /// Loads the data file
        /// </summary>
        /// <returns>0 - success</returns>
        public int LoadResourceData()
        {
            string data = Properties.Resources.EURUSD1440;

            var parser = new Data_Parser(data);
            int result = parser.Parse();

            if (result == 0)
            {
                _dataBar = parser.Bar;
                _bars = parser.Bars;
            }

            return result;
        }
    }
}