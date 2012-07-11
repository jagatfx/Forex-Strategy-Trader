// Bar Stats
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// The direction of trade.
    /// </summary>
    public enum TradeDirection
    {
        None,
        Long,
        Short,
        Both
    }

    /// <summary>
    /// The incoming tick type depending on its position in the bar.
    /// </summary>
    public enum TickType
    {
        Open,
        OpenClose,
        Regular,
        Close,
        AfterClose
    }

    /// <summary>
    /// The positions' direction
    /// </summary>
    public enum PosDirection
    {
        None,
        Long,
        Short,
        Closed
    }

    /// <summary>
    /// Order direction
    /// </summary>
    public enum OrderDirection
    {
        Buy,
        Sell
    }

    /// <summary>
    /// Entry / exit price type depending on the position of the bar.
    /// </summary>
    public enum StrategyPriceType
    {
        Unknown,
        Open,
        Close,
        Indicator,
        CloseAndReverse
    }

    /// <summary>
    /// Type of operation send to the terminal.
    /// </summary>
    public enum OperationType
    {
        Buy,
        Sell,
        Close,
        Modify
    }

    public class BarStats
    {
        private PosDirection _posDir = PosDirection.None;
        private bool _posFlag; // It shows if there was a position during this bar.

        public BarStats(DateTime barTime, PosDirection posDir, double posPrice, double posLots)
        {
            Operations = new List<Operation>();
            BarTime = barTime;
            PositionDir = posDir;
            PositionPrice = posPrice;
            PositionLots = posLots;
        }

        private BarStats()
        {
            BarTime = DateTime.MinValue;
            Operations = new List<Operation>();
        }

        private DateTime BarTime { get; set; }

        public PosDirection PositionDir
        {
            get
            {
                if (_posFlag && _posDir == PosDirection.None)
                    return PosDirection.Closed;
                return _posDir;
            }
            set
            {
                if (_posDir == PosDirection.Long ||
                    _posDir == PosDirection.Short)
                {
                    _posFlag = true;
                }
                _posDir = value;
            }
        }

        public double PositionPrice { get; set; }
        public double PositionLots { get; set; }
        public List<Operation> Operations { get; private set; }

        public BarStats Clone()
        {
            var barStats = new BarStats
                               {
                                   BarTime = BarTime,
                                   _posDir = _posDir,
                                   PositionPrice = PositionPrice,
                                   PositionLots = PositionLots,
                                   _posFlag = _posFlag,
                                   Operations = Operations.GetRange(0, Operations.Count)
                               };

            return barStats;
        }
    }
}