// Action Trade
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using MT4Bridge;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions
    {
        private static int _nBarsExit;
        private StrategyPriceType _closeStrPriceType;
        private ExecutionTime _closeTimeExec;
        private List<string> _closingLogicGroups;

        // Logical Groups
        private Dictionary<string, bool> _groupsAllowLong;
        private Dictionary<string, bool> _groupsAllowShort;
        private bool _isEnteredLong; // Whether we have already entered long during this bar.
        private bool _isEnteredShort; // Whether we have already entered short during this bar.
        private double _maximumLots;
        private StrategyPriceType _openStrPriceType;
        private ExecutionTime _openTimeExec;
        private List<string> _openingLogicGroups;
        private DateTime _timeLastEntryBar; // The time of last executed entry;

        /// <summary>
        /// Initializes the global variables.
        /// </summary>
        private void InitTrade()
        {
            // Sets the maximum lots
            _maximumLots = 100;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.IndicatorName == "Lot Limiter")
                    _maximumLots = (int) slot.IndParam.NumParam[0].Value;
            _maximumLots = Math.Min(_maximumLots, Data.Strategy.MaxOpenLots);

            _openTimeExec = Data.Strategy.Slot[Data.Strategy.OpenSlot].IndParam.ExecutionTime;
            _openStrPriceType = StrategyPriceType.Unknown;
            if (_openTimeExec == ExecutionTime.AtBarOpening)
                _openStrPriceType = StrategyPriceType.Open;
            else if (_openTimeExec == ExecutionTime.AtBarClosing)
                _openStrPriceType = StrategyPriceType.Close;
            else
                _openStrPriceType = StrategyPriceType.Indicator;

            _closeTimeExec = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.ExecutionTime;
            _closeStrPriceType = StrategyPriceType.Unknown;
            if (_closeTimeExec == ExecutionTime.AtBarOpening)
                _closeStrPriceType = StrategyPriceType.Open;
            else if (_closeTimeExec == ExecutionTime.AtBarClosing)
                _closeStrPriceType = StrategyPriceType.Close;
            else if (_closeTimeExec == ExecutionTime.CloseAndReverse)
                _closeStrPriceType = StrategyPriceType.CloseAndReverse;
            else
                _closeStrPriceType = StrategyPriceType.Indicator;

            if (Configs.UseLogicalGroups)
            {
                Data.Strategy.Slot[Data.Strategy.OpenSlot].LogicalGroup = "All";
                    // Allows calculation of open slot for each group.
                Data.Strategy.Slot[Data.Strategy.CloseSlot].LogicalGroup = "All";
                    // Allows calculation of close slot for each group.

                _groupsAllowLong = new Dictionary<string, bool>();
                _groupsAllowShort = new Dictionary<string, bool>();
                for (int slot = Data.Strategy.OpenSlot; slot < Data.Strategy.CloseSlot; slot++)
                {
                    if (!_groupsAllowLong.ContainsKey(Data.Strategy.Slot[slot].LogicalGroup))
                        _groupsAllowLong.Add(Data.Strategy.Slot[slot].LogicalGroup, false);
                    if (!_groupsAllowShort.ContainsKey(Data.Strategy.Slot[slot].LogicalGroup))
                        _groupsAllowShort.Add(Data.Strategy.Slot[slot].LogicalGroup, false);
                }

                // List of logical groups
                _openingLogicGroups = new List<string>();
                foreach (var kvp in _groupsAllowLong)
                    _openingLogicGroups.Add(kvp.Key);


                // Logical groups of the closing conditions.
                _closingLogicGroups = new List<string>();
                for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                {
                    string group = Data.Strategy.Slot[slot].LogicalGroup;
                    if (!_closingLogicGroups.Contains(group) && group != "all")
                        _closingLogicGroups.Add(group); // Adds all groups except "all"
                }

                if (_closingLogicGroups.Count == 0)
                    _closingLogicGroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.
            }

            // Search if N Bars Exit is present as CloseFilter, could be any slot after first closing slot. - Krog
            _nBarsExit = 0;
            for (int slot = Data.Strategy.CloseSlot; slot < Data.Strategy.Slots; slot++)
            {
                if (Data.Strategy.Slot[slot].IndicatorName == "N Bars Exit")
                {
                    _nBarsExit = (int) Data.Strategy.Slot[slot].IndParam.NumParam[0].Value;
                    break;
                }
            }
        }

        /// <summary>
        /// Reinitializes global variables.
        /// </summary>
        private void DeinitTrade()
        {
            if (Configs.UseLogicalGroups)
            {
                Data.Strategy.Slot[Data.Strategy.OpenSlot].LogicalGroup = ""; // Delete the group of open slot.
                Data.Strategy.Slot[Data.Strategy.CloseSlot].LogicalGroup = ""; // Delete the group of close slot.
            }

            _nBarsExit = 0;
        }

        /// <summary>
        /// Checks whether entry price was reached.
        /// </summary>
        private TradeDirection AnalyseEntryPrice()
        {
            int bar = Data.Bars - 1;

            double buyPrice = 0;
            double sellPrice = 0;
            foreach (IndicatorComp component in Data.Strategy.Slot[Data.Strategy.OpenSlot].Component)
            {
                IndComponentType compType = component.DataType;
                if (compType == IndComponentType.OpenLongPrice)
                    buyPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    sellPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = component.Value[bar];
            }

            double basePrice = Data.Bid;
            double oldPrice = Data.OldBid;

            switch (Configs.LongTradeLogicPrice)
            {
                case "Ask":
                    basePrice = Data.Ask;
                    oldPrice = Data.OldAsk;
                    break;
                case "Close":
                    basePrice = Data.LastClose;
                    oldPrice = Data.OldClose;
                    break;
            }

            bool canOpenLong = (buyPrice > oldPrice + Epsilon && buyPrice < basePrice + Epsilon) ||
                               (buyPrice > basePrice - Epsilon && buyPrice < oldPrice - Epsilon);

            bool canOpenShort = (sellPrice > Data.OldBid + Epsilon && sellPrice < Data.Bid + Epsilon) ||
                                (sellPrice > Data.Bid - Epsilon && sellPrice < Data.OldBid - Epsilon);

            TradeDirection direction = TradeDirection.None;

            if (canOpenLong && canOpenShort)
                direction = TradeDirection.Both;
            else if (canOpenLong)
                direction = TradeDirection.Long;
            else if (canOpenShort)
                direction = TradeDirection.Short;

            return direction;
        }

        /// <summary>
        /// Determines the direction of market entry.
        /// </summary>
        private TradeDirection AnalyseEntryDirection()
        {
            int bar = Data.Bars - 1;

            // Do not send entry order when we are not on time
            if (_openTimeExec == ExecutionTime.AtBarOpening &&
                Data.Strategy.Slot[Data.Strategy.OpenSlot].Component[0].Value[bar] < Epsilon) // TODO Inspect why we check exactly component 0 
                return TradeDirection.None;

            foreach (IndicatorSlot slot in Data.Strategy.Slot)
                if (slot.IndicatorName == "Enter Once")
                {
                    switch (slot.IndParam.ListParam[0].Text)
                    {
                        case "Enter no more than once a bar":
                            if (Data.Time[bar] == _timeLastEntryBar)
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a day":
                            if (Data.Time[bar].DayOfYear == _timeLastEntryBar.DayOfYear)
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a week":
                            if (Data.Time[bar].DayOfWeek >= _timeLastEntryBar.DayOfWeek &&
                                Data.Time[bar] < _timeLastEntryBar.AddDays(7))
                                return TradeDirection.None;
                            break;
                        case "Enter no more than once a month":
                            if (Data.Time[bar].Month == _timeLastEntryBar.Month)
                                return TradeDirection.None;
                            break;
                    }
                }

            // Determining of the buy/sell entry prices.
            double buyPrice = 0;
            double sellPrice = 0;
            foreach (IndicatorComp component in Data.Strategy.Slot[Data.Strategy.OpenSlot].Component)
            {
                IndComponentType compType = component.DataType;
                if (compType == IndComponentType.OpenLongPrice)
                    buyPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenShortPrice)
                    sellPrice = component.Value[bar];
                else if (compType == IndComponentType.OpenPrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = component.Value[bar];
            }

            // Decide whether to open 
            bool canOpenLong = buyPrice > Data.InstrProperties.Point;
            bool canOpenShort = sellPrice > Data.InstrProperties.Point;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in _openingLogicGroups)
                {
                    bool groupOpenLong = canOpenLong;
                    bool groupOpenShort = canOpenShort;

                    AnalyseEntryLogicConditions(bar, group, buyPrice, sellPrice, ref groupOpenLong, ref groupOpenShort);

                    _groupsAllowLong[group] = groupOpenLong;
                    _groupsAllowShort[group] = groupOpenShort;
                }

                bool groupLongEntry = false;
                foreach (var groupLong in _groupsAllowLong)
                    if ((_groupsAllowLong.Count > 1 && groupLong.Key != "All") || _groupsAllowLong.Count == 1)
                        groupLongEntry = groupLongEntry || groupLong.Value;

                bool groupShortEntry = false;
                foreach (var groupShort in _groupsAllowShort)
                    if ((_groupsAllowShort.Count > 1 && groupShort.Key != "All") || _groupsAllowShort.Count == 1)
                        groupShortEntry = groupShortEntry || groupShort.Value;

                canOpenLong = canOpenLong && groupLongEntry && _groupsAllowLong["All"];
                canOpenShort = canOpenShort && groupShortEntry && _groupsAllowShort["All"];
            }
            else
            {
                AnalyseEntryLogicConditions(bar, "A", buyPrice, sellPrice, ref canOpenLong, ref canOpenShort);
            }

            TradeDirection direction = TradeDirection.None;
            if (canOpenLong && canOpenShort)
                direction = TradeDirection.Both;
            else if (canOpenLong)
                direction = TradeDirection.Long;
            else if (canOpenShort)
                direction = TradeDirection.Short;

            return direction;
        }

        /// <summary>
        /// checks if the opening logic conditions allow long or short entry.
        /// </summary>
        private void AnalyseEntryLogicConditions(int bar, string group, double buyPrice, double sellPrice,
                                                 ref bool canOpenLong, ref bool canOpenShort)
        {
            for (int slot = Data.Strategy.OpenSlot; slot <= Data.Strategy.CloseSlot; slot++)
            {
                if (Configs.UseLogicalGroups &&
                    Data.Strategy.Slot[slot].LogicalGroup != group &&
                    Data.Strategy.Slot[slot].LogicalGroup != "All")
                    continue;

                foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                {
                    if (component.DataType == IndComponentType.AllowOpenLong && component.Value[bar] < 0.5)
                        canOpenLong = false;

                    if (component.DataType == IndComponentType.AllowOpenShort && component.Value[bar] < 0.5)
                        canOpenShort = false;

                    if (component.PosPriceDependence != PositionPriceDependence.None)
                    {
                        double indicatorValue = component.Value[bar - component.UsePreviousBar];
                        switch (component.PosPriceDependence)
                        {
                            case PositionPriceDependence.PriceBuyHigher:
                                canOpenLong = canOpenLong && buyPrice > indicatorValue + Epsilon;
                                break;
                            case PositionPriceDependence.PriceBuyLower:
                                canOpenLong = canOpenLong && buyPrice < indicatorValue - Epsilon;
                                break;
                            case PositionPriceDependence.PriceSellHigher:
                                canOpenShort = canOpenShort && sellPrice > indicatorValue + Epsilon;
                                break;
                            case PositionPriceDependence.PriceSellLower:
                                canOpenShort = canOpenShort && sellPrice < indicatorValue - Epsilon;
                                break;
                            case PositionPriceDependence.BuyHigherSellLower:
                                canOpenLong = canOpenLong && buyPrice > indicatorValue + Epsilon;
                                canOpenShort = canOpenShort && sellPrice < indicatorValue - Epsilon;
                                break;
                            case PositionPriceDependence.BuyLowerSelHigher:
                                canOpenLong = canOpenLong && buyPrice < indicatorValue - Epsilon;
                                canOpenShort = canOpenShort && sellPrice > indicatorValue + Epsilon;
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the size of an entry order.
        /// </summary>
        private double AnalyseEntrySize(OrderDirection ordDir, ref PosDirection newPosDir)
        {
            double size = 0;
            PosDirection posDir = Data.PositionDirection;

            // Orders modification on a fly
            // Checks whether we are on the market 
            if (posDir == PosDirection.Long || posDir == PosDirection.Short)
            {
                // We are on the market
                if (ordDir == OrderDirection.Buy && posDir == PosDirection.Long ||
                    ordDir == OrderDirection.Sell && posDir == PosDirection.Short)
                {
                    // In case of a Same Dir Signal 
                    switch (Data.Strategy.SameSignalAction)
                    {
                        case SameDirSignalAction.Add:
                            if (Data.PositionLots + TradingSize(Data.Strategy.AddingLots) <
                                _maximumLots + Data.InstrProperties.LotStep/2)
                            {
                                // Adding
                                size = TradingSize(Data.Strategy.AddingLots);
                                newPosDir = posDir;
                            }
                            else
                            {
                                // Cancel the Adding
                                size = 0;
                                newPosDir = posDir;
                            }
                            break;
                        case SameDirSignalAction.Winner:
                            if (Data.PositionProfit > Epsilon &&
                                Data.PositionLots + TradingSize(Data.Strategy.AddingLots) <
                                _maximumLots + Data.InstrProperties.LotStep/2)
                            {
                                // Adding
                                size = TradingSize(Data.Strategy.AddingLots);
                                newPosDir = posDir;
                            }
                            else
                            {
                                // Cancel the Adding
                                size = 0;
                                newPosDir = posDir;
                            }
                            break;
                        case SameDirSignalAction.Nothing:
                            size = 0;
                            newPosDir = posDir;
                            break;
                    }
                }
                else if (ordDir == OrderDirection.Buy && posDir == PosDirection.Short ||
                         ordDir == OrderDirection.Sell && posDir == PosDirection.Long)
                {
                    // In case of an Opposite Dir Signal 
                    switch (Data.Strategy.OppSignalAction)
                    {
                        case OppositeDirSignalAction.Reduce:
                            if (Data.PositionLots > TradingSize(Data.Strategy.ReducingLots))
                            {
                                // Reducing
                                size = TradingSize(Data.Strategy.ReducingLots);
                                newPosDir = posDir;
                            }
                            else
                            {
                                // Closing
                                size = Data.PositionLots;
                                newPosDir = PosDirection.Closed;
                            }
                            break;
                        case OppositeDirSignalAction.Close:
                            size = Data.PositionLots;
                            newPosDir = PosDirection.Closed;
                            break;
                        case OppositeDirSignalAction.Reverse:
                            size = Data.PositionLots + TradingSize(Data.Strategy.EntryLots);
                            newPosDir = posDir == PosDirection.Long ? PosDirection.Short : PosDirection.Long;
                            break;
                        case OppositeDirSignalAction.Nothing:
                            size = 0;
                            newPosDir = posDir;
                            break;
                    }
                }
            }
            else
            {
                // We are square on the market
                size = TradingSize(Data.Strategy.EntryLots);
                if (Data.Strategy.UseMartingale && Data.ConsecutiveLosses > 0)
                {
                    size = size * Math.Pow(Data.Strategy.MartingaleMultiplier, Data.ConsecutiveLosses);
                    size = NormalizeEntrySize(size);
                }
                size = Math.Min(size, _maximumLots);

                newPosDir = ordDir == OrderDirection.Buy ? PosDirection.Long : PosDirection.Short;
            }

            return size;
        }

        /// <summary>
        /// Checks if exit price was reached.
        /// </summary>
        private TradeDirection AnalyseExitPrice()
        {
            IndicatorSlot slot = Data.Strategy.Slot[Data.Strategy.CloseSlot];
            int bar = Data.Bars - 1;

            // Searching the exit price in the exit indicator slot.
            double buyPrice = 0;
            double sellPrice = 0;
            for (int comp = 0; comp < slot.Component.Length; comp++)
            {
                IndComponentType compType = slot.Component[comp].DataType;

                if (compType == IndComponentType.CloseLongPrice)
                    sellPrice = slot.Component[comp].Value[bar];
                else if (compType == IndComponentType.CloseShortPrice)
                    buyPrice = slot.Component[comp].Value[bar];
                else if (compType == IndComponentType.ClosePrice || compType == IndComponentType.OpenClosePrice)
                    buyPrice = sellPrice = slot.Component[comp].Value[bar];
            }

            // We can close if the closing price is higher than zero.
            bool canCloseLong = sellPrice > Data.InstrProperties.Point;
            bool canCloseShort = buyPrice > Data.InstrProperties.Point;

            // Check if the closing price was reached.
            if (canCloseLong)
            {
                canCloseLong = (sellPrice > Data.OldBid + Epsilon && sellPrice < Data.Bid + Epsilon) ||
                               (sellPrice > Data.Bid - Epsilon && sellPrice < Data.OldBid - Epsilon);
            }
            if (canCloseShort)
            {
                canCloseShort = (buyPrice > Data.OldBid + Epsilon && buyPrice < Data.Bid + Epsilon) ||
                                (buyPrice > Data.Bid - Epsilon && buyPrice < Data.OldBid - Epsilon);
            }

            // Determine the trading direction.
            TradeDirection direction = TradeDirection.None;

            if (canCloseLong && canCloseShort)
                direction = TradeDirection.Both;
            else if (canCloseLong)
                direction = TradeDirection.Short;
            else if (canCloseShort)
                direction = TradeDirection.Long;

            return direction;
        }

        /// <summary>
        /// Gets the direction of closing of the current position.
        /// </summary>
        private TradeDirection AnalyseExitDirection()
        {
            int bar = Data.Bars - 1;

            if (_closeTimeExec == ExecutionTime.AtBarClosing &&
                Data.Strategy.Slot[Data.Strategy.CloseSlot].Component[0].Value[bar] < Epsilon)  // TODO Inspect why we check exactly component 0 
                return TradeDirection.None;

            if (Data.Strategy.CloseFilters == 0)
                return TradeDirection.Both;

            if (_nBarsExit > 0)
                if (Data.PositionOpenTime.AddMinutes(_nBarsExit*(int) Data.Period) <
                    Data.Time[Data.Bars - 1].AddMinutes((int) Data.Period))
                    return TradeDirection.Both;

            TradeDirection direction = TradeDirection.None;

            if (Configs.UseLogicalGroups)
            {
                foreach (string group in _closingLogicGroups)
                {
                    TradeDirection groupDirection = TradeDirection.Both;

                    // Determining of the slot direction
                    for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        TradeDirection slotDirection = TradeDirection.None;
                        if (Data.Strategy.Slot[slot].LogicalGroup == group ||
                            Data.Strategy.Slot[slot].LogicalGroup == "all")
                        {
                            foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                                if (component.Value[bar] > 0)
                                    slotDirection = GetClosingDirection(slotDirection, component.DataType);

                            groupDirection = ReduceDirectionStatus(groupDirection, slotDirection);
                        }
                    }

                    direction = IncreaseDirectionStatus(direction, groupDirection);
                }
            }
            else
            {
                // Search close filters for a closing signal.
                for (int slot = Data.Strategy.CloseSlot + 1; slot < Data.Strategy.Slots; slot++)
                    foreach (IndicatorComp component in Data.Strategy.Slot[slot].Component)
                        if (component.Value[bar] > 0)
                            direction = GetClosingDirection(direction, component.DataType);
            }

            return direction;
        }

        /// <summary>
        /// Reduces the status of baseDirection to direction.
        /// </summary>
        private TradeDirection ReduceDirectionStatus(TradeDirection baseDirection, TradeDirection direction)
        {
            if (baseDirection == direction || direction == TradeDirection.Both)
                return baseDirection;

            if (baseDirection == TradeDirection.Both)
                return direction;

            return TradeDirection.None;
        }

        /// <summary>
        /// Increases the status of baseDirection to direction.
        /// </summary>
        private TradeDirection IncreaseDirectionStatus(TradeDirection baseDirection, TradeDirection direction)
        {
            if (baseDirection == direction || direction == TradeDirection.None)
                return baseDirection;

            if (baseDirection == TradeDirection.None)
                return direction;

            return TradeDirection.Both;
        }

        /// <summary>
        /// Adjusts the closing direction.
        /// </summary>
        private TradeDirection GetClosingDirection(TradeDirection baseDirection, IndComponentType compDataType)
        {
            TradeDirection newDirection = baseDirection;

            if (compDataType == IndComponentType.ForceClose)
            {
                newDirection = TradeDirection.Both;
            }
            else if (compDataType == IndComponentType.ForceCloseShort)
            {
                if (baseDirection == TradeDirection.None)
                    newDirection = TradeDirection.Long;
                else if (baseDirection == TradeDirection.Short)
                    newDirection = TradeDirection.Both;
            }
            else if (compDataType == IndComponentType.ForceCloseLong)
            {
                if (baseDirection == TradeDirection.None)
                    newDirection = TradeDirection.Short;
                else if (baseDirection == TradeDirection.Long)
                    newDirection = TradeDirection.Both;
            }

            return newDirection;
        }

        /// <summary>
        /// Calculates the Stop Loss distance.
        /// </summary>
        private double GetStopLossPips(double lots)
        {
            double indStop = double.MaxValue;
            bool isIndStop = true;

            switch (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName)
            {
                case "Account Percent Stop":
                    indStop =
                        AccountPercentStopPips(Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value,
                                               lots);
                    break;
                case "ATR Stop":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].Component[0].Value[Data.Bars - 1];
                    break;
                case "Stop Loss":
                case "Stop Limit":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    break;
                case "Trailing Stop":
                case "Trailing Stop Limit":
                    indStop = Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    break;
                default:
                    isIndStop = false;
                    break;
            }

            double permStop = Data.Strategy.UsePermanentSL ? Data.Strategy.PermanentSL : double.MaxValue;
            double stopLoss = 0;

            if (isIndStop || Data.Strategy.UsePermanentSL)
            {
                stopLoss = Math.Min(indStop, permStop);
                if (stopLoss < Data.InstrProperties.StopLevel)
                    stopLoss = Data.InstrProperties.StopLevel;
                stopLoss = Math.Round(stopLoss);
            }

            return stopLoss;
        }

        /// <summary>
        /// Calculates the Take Profit distance.
        /// </summary>
        private double GetTakeProfitPips()
        {
            double takeprofit = 0;
            double permLimit = Data.Strategy.UsePermanentTP ? Data.Strategy.PermanentTP : double.MaxValue;
            double indLimit = double.MaxValue;
            bool isIndLimit = true;
            IndicatorSlot closeSlot = Data.Strategy.Slot[Data.Strategy.CloseSlot];

            switch (closeSlot.IndicatorName)
            {
                case "Take Profit":
                    indLimit = closeSlot.IndParam.NumParam[0].Value;
                    break;
                case "Stop Limit":
                case "Trailing Stop Limit":
                    indLimit = closeSlot.IndParam.NumParam[1].Value;
                    break;
                default:
                    isIndLimit = false;
                    break;
            }

            if (isIndLimit || Data.Strategy.UsePermanentTP)
            {
                takeprofit = Math.Min(indLimit, permLimit);
                if (takeprofit < Data.InstrProperties.StopLevel)
                    takeprofit = Data.InstrProperties.StopLevel;
                takeprofit = Math.Round(takeprofit);
            }

            return takeprofit;
        }

        /// <summary>
        /// Sets and sends an entry order.
        /// </summary>
        private void DoEntryTrade(TradeDirection tradeDir)
        {
            double price;
            OrderDirection ordDir;
            OperationType opType;
            OrderType type;
            JournalIcons icon;

            if (_timeLastEntryBar != Data.Time[Data.Bars - 1])
                _isEnteredLong = _isEnteredShort = false;

            switch (tradeDir)
            {
                case TradeDirection.Long: // Buy
                    if (_isEnteredLong)
                        return;
                    price = Data.Ask;
                    ordDir = OrderDirection.Buy;
                    opType = OperationType.Buy;
                    type = OrderType.Buy;
                    icon = JournalIcons.OrderBuy;
                    break;
                case TradeDirection.Short: // Sell
                    if (_isEnteredShort)
                        return;
                    price = Data.Bid;
                    ordDir = OrderDirection.Sell;
                    opType = OperationType.Sell;
                    type = OrderType.Sell;
                    icon = JournalIcons.OrderSell;
                    break;
                default: // Wrong direction of trade.
                    return;
            }

            PosDirection newPosDir = PosDirection.None;
            double size = AnalyseEntrySize(ordDir, ref newPosDir);
            if (size < Data.InstrProperties.MinLot/2)
            {
                // The entry trade is cancelled.  
                return;
            }

            string symbol = Data.Symbol;
            double lots = size;
            int slippage = Configs.AutoSlippage ? (int) Data.InstrProperties.Spread*3 : Configs.SlippageEntry;
            double stoploss = GetStopLossPips(size);
            double takeprofit = GetTakeProfitPips();
            double point = Data.InstrProperties.Point;

            string stopLoss = "0";
            if (stoploss > 0)
            {
                double stopLossPrice = 0;
                if (newPosDir == PosDirection.Long)
                    stopLossPrice = Data.Bid - stoploss*point;
                else if (newPosDir == PosDirection.Short)
                    stopLossPrice = Data.Ask + stoploss*point;
                stopLoss = stopLossPrice.ToString(Data.FF);
            }

            string takeProfit = "0";
            if (takeprofit > 0)
            {
                double takeProfitPrice = 0;
                if (newPosDir == PosDirection.Long)
                    takeProfitPrice = Data.Bid + takeprofit*point;
                else if (newPosDir == PosDirection.Short)
                    takeProfitPrice = Data.Ask - takeprofit*point;
                takeProfit = takeProfitPrice.ToString(Data.FF);
            }

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                           Language.T("An entry order sent") + ": " +
                                           Language.T(ordDir.ToString()) + " {0} " +
                                           LotOrLots(lots) + " " +
                                           Language.T("at") + " {1}, " +
                                           Language.T("Stop Loss") + " {2}, " +
                                           Language.T("Take Profit") + " {3}",
                                           lots, price.ToString(Data.FF), stopLoss,
                                           takeProfit);
            var jmsg = new JournalMessage(icon, DateTime.Now, message);
            AppendJournalMessage(jmsg);
            Data.Logger.WriteLogLine(message);

            string parameters = OrderParameters();
            int response = _bridge.OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, parameters);

            if (response >= 0)
            {
                // The order was executed successfully.
                Data.AddBarStats(opType, lots, price);

                _timeLastEntryBar = Data.Time[Data.Bars - 1];
                if (type == OrderType.Buy)
                    _isEnteredLong = true;
                else
                    _isEnteredShort = true;

                Data.WrongStopLoss = 0;
                Data.WrongTakeProf = 0;
                Data.WrongStopsRetry = 0;
            }
            else
            {
                // Error in operation execution.
                ReportOperationError();
                Data.WrongStopLoss = (int) stoploss;
                Data.WrongTakeProf = (int) takeprofit;
            }
        }

        /// <summary>
        /// Sets and sends an exit order.
        /// </summary>
        private bool DoExitTrade()
        {
            string symbol = Data.Symbol;
            double lots = Data.PositionLots;
            double price = Data.PositionType == (int) OrderType.Buy ? Data.Bid : Data.Ask;
            int slippage = Configs.AutoSlippage ? (int) Data.InstrProperties.Spread*6 : Configs.SlippageExit;
            int ticket = Data.PositionTicket;

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                           Language.T("An exit order sent") + ": " +
                                           Language.T("Close") + " {0} " +
                                           LotOrLots(lots) + " " + Language.T("at") + " {1}",
                                           lots, price.ToString(Data.FF));
            var jmsg = new JournalMessage(JournalIcons.OrderClose, DateTime.Now,message);
            AppendJournalMessage(jmsg);
            Data.Logger.WriteLogLine(message);

            bool responseOK = _bridge.OrderClose(ticket, lots, price, slippage);

            if (responseOK)
                Data.AddBarStats(OperationType.Close, lots, price);
            else
                ReportOperationError();

            Data.WrongStopLoss = 0;
            Data.WrongTakeProf = 0;
            Data.WrongStopsRetry = 0;

            return responseOK;
        }

        /// <summary>
        /// Calculates a trade.
        /// </summary>
        private void CalculateTrade(TickType ticktype)
        {
            // Exit
            bool closeOK = false;
            if (_closeStrPriceType != StrategyPriceType.CloseAndReverse && Data.PositionTicket != 0)
            {
                if (_closeStrPriceType == StrategyPriceType.Open &&
                    (ticktype == TickType.Open || ticktype == TickType.OpenClose) ||
                    _closeStrPriceType == StrategyPriceType.Close &&
                    (ticktype == TickType.Close || ticktype == TickType.OpenClose))
                {
                    // Exit at Bar Open or Bar Close.
                    TradeDirection direction = AnalyseExitDirection();
                    if (direction == TradeDirection.Both ||
                        (direction == TradeDirection.Long && Data.PositionDirection == PosDirection.Short) ||
                        (direction == TradeDirection.Short && Data.PositionDirection == PosDirection.Long))
                        closeOK = DoExitTrade(); // Close the current position.
                }
                else if (_closeStrPriceType == StrategyPriceType.Indicator)
                {
                    // Exit at an indicator value.
                    TradeDirection priceReached = AnalyseExitPrice();
                    if (priceReached == TradeDirection.Long)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Long || direction == TradeDirection.Both)
                            if (Data.PositionDirection == PosDirection.Short)
                                closeOK = DoExitTrade(); // Close a short position.
                    }
                    else if (priceReached == TradeDirection.Short)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Short || direction == TradeDirection.Both)
                            if (Data.PositionDirection == PosDirection.Long)
                                closeOK = DoExitTrade(); // Close a long position.
                    }
                    else if (priceReached == TradeDirection.Both)
                    {
                        TradeDirection direction = AnalyseExitDirection();
                        if (direction == TradeDirection.Long || direction == TradeDirection.Short ||
                            direction == TradeDirection.Both)
                            closeOK = DoExitTrade(); // Close the current position.
                    }
                }
            }

            // Checks if we closed a position successfully.
            if (closeOK)
                return;

            // This is to prevent new entry after Bar Closing has been executed.
            if (_closeStrPriceType == StrategyPriceType.Close && ticktype == TickType.AfterClose)
                return;

            // Entry at Bar Open or Bar Close.
            if (_openStrPriceType == StrategyPriceType.Open &&
                (ticktype == TickType.Open || ticktype == TickType.OpenClose) ||
                _openStrPriceType == StrategyPriceType.Close &&
                (ticktype == TickType.Close || ticktype == TickType.OpenClose))
            {
                TradeDirection direction = AnalyseEntryDirection();
                if (direction == TradeDirection.Long || direction == TradeDirection.Short)
                    DoEntryTrade(direction);
            }
            else if (_openStrPriceType == StrategyPriceType.Indicator)
            {
                // Entry at an indicator value.
                TradeDirection priceReached = AnalyseEntryPrice();
                if (priceReached == TradeDirection.Long)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Long || direction == TradeDirection.Both)
                        DoEntryTrade(TradeDirection.Long);
                }
                else if (priceReached == TradeDirection.Short)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Short || direction == TradeDirection.Both)
                        DoEntryTrade(TradeDirection.Short);
                }
                else if (priceReached == TradeDirection.Both)
                {
                    TradeDirection direction = AnalyseEntryDirection();
                    if (direction == TradeDirection.Long || direction == TradeDirection.Short)
                        DoEntryTrade(direction);
                }
            }
        }

        /// <summary>
        /// Checks for failed set of SL and TP.
        /// </summary>
        private bool IsWrongStopsExecution()
        {
            const int maxRetry = 4;

            if (Data.PositionDirection == PosDirection.Closed ||
                Data.PositionLots < Epsilon ||
                Data.WrongStopsRetry >= maxRetry)
            {
                Data.WrongStopLoss = 0;
                Data.WrongTakeProf = 0;
                Data.WrongStopsRetry = 0;
                return false;
            }

            return Data.WrongStopLoss > 0 && Data.PositionStopLoss < Epsilon ||
                   Data.WrongTakeProf > 0 && Data.PositionTakeProfit < Epsilon;
        }

        /// <summary>
        /// Sets SL and TP after wrong execution.
        /// </summary>
        private void ResendWrongStops()
        {
            string symbol = Data.Symbol;
            double lots = NormalizeEntrySize(Data.PositionLots);
            double price = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
            int ticket = Data.PositionTicket;

            if (Configs.PlaySounds)
                Data.SoundOrderSent.Play();

            double stoploss = Data.WrongStopLoss;
            double takeprofit = Data.WrongTakeProf;

            string stopLoss = "0";
            if (stoploss > 0)
            {
                double stopLossPrice = 0;
                if (Data.PositionDirection == PosDirection.Long)
                    stopLossPrice = Data.Bid - stoploss*Data.InstrProperties.Point;
                else if (Data.PositionDirection == PosDirection.Short)
                    stopLossPrice = Data.Ask + stoploss*Data.InstrProperties.Point;
                stopLoss = stopLossPrice.ToString(Data.FF);
            }

            string takeProfit = "0";
            if (takeprofit > 0)
            {
                double takeProfitPrice = 0;
                if (Data.PositionDirection == PosDirection.Long)
                    takeProfitPrice = Data.Bid + takeprofit*Data.InstrProperties.Point;
                else if (Data.PositionDirection == PosDirection.Short)
                    takeProfitPrice = Data.Ask - takeprofit*Data.InstrProperties.Point;
                takeProfit = takeProfitPrice.ToString(Data.FF);
            }

            string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                           Language.T("A modify order sent") + ": " +
                                           Language.T("Stop Loss") + " {0}, " + Language.T("Take Profit") +
                                           " {1}", stopLoss, takeProfit);
            var jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, message);
            AppendJournalMessage(jmsg);
            Data.Logger.WriteLogLine(message);

            string parameters = "TS1=" + 0 + ";BRE=" + 0;

            bool responseOK = _bridge.OrderModify(ticket, price, stoploss, takeprofit, parameters);

            if (responseOK)
            {
                Data.AddBarStats(OperationType.Modify, lots, price);
                Data.WrongStopLoss = 0;
                Data.WrongTakeProf = 0;
                Data.WrongStopsRetry = 0;
            }
            else
            {
                ReportOperationError();
                Data.WrongStopsRetry++;
            }
        }

        /// <summary>
        /// Logs operation error in journal and log file.
        /// </summary>
        private void ReportOperationError()
        {
            if (Configs.PlaySounds)
                Data.SoundError.Play();

            string message;
            JournalMessage journalMessage;

            if (_bridge.LastError == 0)
            {
                message = Language.T("Operation execution") + ": " +
                          Language.T("MetaTrader is not responding!").Replace("MetaTrader",
                                                                              Data.TerminalName);
                journalMessage = new JournalMessage(JournalIcons.Warning, DateTime.Now, message);
            }
            else
            {
                message = Language.T("MetaTrader failed to execute order! Returned").Replace(
                    "MetaTrader", Data.TerminalName) + ": " +
                          MT4_Errors.ErrorDescription(_bridge.LastError);
                journalMessage = new JournalMessage(JournalIcons.Error, DateTime.Now, message);
            }
            AppendJournalMessage(journalMessage);
            Data.Logger.WriteLogLine(message);
        }


        /// <summary>
        /// Calculates the trading size in normalized lots
        /// </summary>
        private double TradingSize(double size)
        {
            if (Data.Strategy.UseAccountPercentEntry)
                size = (size/100)*Data.AccountEquity/Data.InstrProperties.MarginRequired;

            size = NormalizeEntrySize(size);

            return size;
        }

        /// <summary>
        /// Normalizes an entry order's size.
        /// </summary>
        private double NormalizeEntrySize(double size)
        {
            double minlot = Data.InstrProperties.MinLot;
            double maxlot = Data.InstrProperties.MaxLot;
            double lotstep = Data.InstrProperties.LotStep;

            if (size <= 0)
                return (0);

            var steps = (int) Math.Round((size - minlot)/lotstep);
            size = minlot + steps*lotstep;

            if (size <= minlot)
                return (minlot);

            if (size >= maxlot)
                return (maxlot);

            return size;
        }

        /// <summary>
        /// Converts account percentage to Stop Loss in pips.
        /// </summary>
        private double AccountPercentStopPips(double percent, double lots)
        {
            double balance = Data.AccountBalance;
            double moneyrisk = balance*percent/100;
            double spread = Data.InstrProperties.Spread;
            double tickvalue = Data.InstrProperties.TickValue;

            double stoploss = moneyrisk/(lots*tickvalue) - spread;

            return (stoploss);
        }

        /// <summary>
        /// Generates order parameters string
        /// </summary>
        private string OrderParameters()
        {
            // Trailing Stop
            int trailTrailingStop = 0;
            string trailingStopMode = "TS0";
            switch (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName)
            {
                case "Trailing Stop":
                case "Trailing Stop Limit":
                    trailTrailingStop = (int) Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.NumParam[0].Value;
                    if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndParam.ListParam[1].Text == "Trails at a new top/bottom")
                        trailingStopMode = "TS1";
                    break;
            }
            string trailingStopParam = trailingStopMode + "=" + trailTrailingStop;

            // Break Even
            int distanceBreakEven = 0;
            if (Data.Strategy.UseBreakEven)
                distanceBreakEven = Data.Strategy.BreakEven;
            string breakEvenParam = "BRE=" + distanceBreakEven;

            string parameters = trailingStopParam + ";" + breakEvenParam;

            return parameters;
        }

        private string LotOrLots(double lots)
        {
            return Math.Abs(lots - 1) < Epsilon
                ? Language.T("lot")
                : Language.T("lots");
        }
    }
}