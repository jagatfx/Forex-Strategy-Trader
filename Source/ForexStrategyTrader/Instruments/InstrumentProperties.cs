// Instrument_Properties Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Contains the instrument properties.
    /// </summary>
    public class InstrumentProperties
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InstrumentProperties(string symbol)
        {
            Symbol = symbol;
            Digits = 4;
            LotSize = 10000;
            StopLevel = 5;
            Spread = 2;
            SwapLong = 1;
            SwapShort = -1;
            TickValue = LotSize*Point;
            MinLot = 0.01;
            MaxLot = 100;
            LotStep = 0.01;
            MarginRequired = 1000;
        }

        public string Symbol { get; set; }
        public int Digits { get; set; }
        public int LotSize { get; set; }
        public double StopLevel { get; set; }
        public double Spread { get; set; }
        public double SwapLong { get; set; }
        public double SwapShort { get; set; }
        public double TickValue { get; set; }
        public double MinLot { get; set; }
        public double MaxLot { get; set; }
        public double LotStep { get; set; }
        public double MarginRequired { get; set; }

        public double Point
        {
            get { return (1/Math.Pow(10, Digits)); }
        }

        /// <summary>
        /// Clones the Instrument_Properties.
        /// </summary>
        public InstrumentProperties Clone()
        {
            var copy = new InstrumentProperties(Symbol)
                           {
                               Symbol = Symbol,
                               Digits = Digits,
                               LotSize = LotSize,
                               Spread = Spread,
                               StopLevel = StopLevel,
                               SwapLong = SwapLong,
                               SwapShort = SwapShort,
                               TickValue = TickValue,
                               MinLot = MinLot,
                               MaxLot = MaxLot,
                               LotStep = LotStep,
                               MarginRequired = MarginRequired
                           };


            return copy;
        }
    }
}