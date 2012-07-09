// Data Market
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using MT4Bridge;

namespace Forex_Strategy_Trader
{
    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public static partial class Data
    {
        private const double Epsilon = 0.000001;
        private const int BalanceLenght = 2000;

        private static double _bid;
        private static double _ask;
        private static double _close;
        private static DateTime _serverTime = DateTime.Now;
        private static List<double> _listTicks = new List<double>();
        private static Dictionary<DateTime, BarStats> _barStats = new Dictionary<DateTime, BarStats>();
        private static readonly DateTime FstStartTime = DateTime.Now;
        public static bool IsConnected { get; set; }
        public static int ConnectionID { get; set; }
        public static string TerminalName { get; set; }
        public static string ExpertVersion { get; set; }
        public static string LibraryVersion { get; set; }

        // The current instrument's properties.
        public static Instrument_Properties InstrProperties { get; set; }

        public static string Symbol
        {
            get { return InstrProperties.Symbol; }
        }

        public static DataPeriods Period { get; set; }

        // Bar's data.
        public static int Bars { get; set; }
        public static DateTime[] Time { get; set; }
        public static double[] Open { get; set; }
        public static double[] High { get; set; }
        public static double[] Low { get; set; }
        public static double[] Close { get; set; }
        public static int[] Volume { get; set; }

        public static double Bid
        {
            get { return _bid; }
            set
            {
                OldBid = _bid;
                _bid = value;
            }
        }

        public static double OldBid { get; private set; }

        public static double Ask
        {
            get { return _ask; }
            set
            {
                OldAsk = _ask;
                _ask = value;
            }
        }

        public static double OldAsk { get; private set; }

        public static double LastClose
        {
            get { return _close; }
            set
            {
                OldClose = _close;
                _close = value;
            }
        }

        public static double OldClose { get; private set; }

        public static DateTime ServerTime
        {
            get { return _serverTime; }
            set { _serverTime = value; }
        }

        public static List<double> ListTicks
        {
            get { return _listTicks; }
        }

        // Account condition.
        public static string AccountName { get; set; }
        public static int AccountNumber { get; set; }
        public static bool IsDemoAccount { get; set; }
        public static string AccountCurrency { get; set; }
        public static double AccountBalance { get; private set; }
        public static double AccountEquity { get; private set; }
        public static double AccountProfit { get; private set; }
        public static double AccountFreeMargin { get; private set; }

        public static Balance_Chart_Unit[] BalanceData { get; private set; }
        public static int BalanceDataPoints { get; private set; }
        public static bool IsBalanceDataChganged { get; private set; }

        public static int PositionTicket { get; private set; }
        public static int PositionType { get; private set; }
        public static double PositionLots { get; private set; }
        public static double PositionOpenPrice { get; private set; }
        public static DateTime PositionOpenTime { get; private set; }
        public static double PositionStopLoss { get; private set; }
        public static double PositionTakeProfit { get; private set; }
        public static double PositionProfit { get; private set; }
        public static string PositionComment { get; private set; }
        public static PosDirection PositionDirection
        {
            get
            {
                PosDirection dir;
                switch (PositionType)
                {
                    case (int) OrderType.Buy:
                        dir = PosDirection.Long;
                        break;
                    case (int) OrderType.Sell:
                        dir = PosDirection.Short;
                        break;
                    default:
                        dir = PosDirection.None;
                        break;
                }

                return dir;
            }
        }

        public static int ConsecutiveLosses{ get; set; }
        public static double ActivatedStopLoss { get; set; }
        public static double ActivatedTakeProfit { get; set; }
        public static double ClosedSLTPLots { get; set; }

        public static Dictionary<DateTime, BarStats> BarStatistics
        {
            get { return _barStats; }
        }

        private static DateTime DemoTradeStartTime { get; set; }
        private static DateTime LiveTradeStartTime { get; set; }
        private static int SecondsDemoTrading { get; set; }
        private static int SecondsLiveTrading { get; set; }
        public static int SavedStrategies { get; set; }

        // Wrong set SL or TP
        public static int WrongStopLoss { get; set; }
        public static int WrongTakeProf { get; set; }
        public static int WrongStopsRetry { get; set; }

        public static void ResetBidAskClose()
        {
            OldBid = OldAsk = OldClose = 0;
            _bid = _ask = _close = 0;
        }

        public static void SetTick(double tick)
        {
            _listTicks.Add(tick);
            if (_listTicks.Count > 60)
                _listTicks.RemoveRange(0, 1);
        }

        public static void ResetTicks()
        {
            _listTicks = new List<double>();
        }

        public static bool SetCurrentAccount(DateTime time, double balance, double equity, double profit,
                                             double freeMargin)
        {
            bool balanceChanged = false;
            bool equityChanged = false;
            IsBalanceDataChganged = false;
            if (Math.Abs(AccountBalance - balance) > 0.01) balanceChanged = true;
            if (Math.Abs(AccountEquity - equity) > 0.01) equityChanged = true;

            AccountBalance = balance;
            AccountEquity = equity;
            AccountProfit = profit;
            AccountFreeMargin = freeMargin;

            if (balance > 0.01 && (equityChanged || balanceChanged))
            {
                var chartUnit = new Balance_Chart_Unit {Time = time, Balance = balance, Equity = equity};

                if (BalanceDataPoints == 0)
                {
                    BalanceData[BalanceDataPoints] = chartUnit;
                    BalanceDataPoints++;
                }

                if (BalanceDataPoints == BalanceLenght)
                {
                    for (int i = 0; i < BalanceLenght - 1; i++)
                        BalanceData[i] = BalanceData[i + 1];
                    BalanceDataPoints = BalanceLenght - 1;
                }

                if (BalanceDataPoints < BalanceLenght)
                {
                    BalanceData[BalanceDataPoints] = chartUnit;
                    BalanceDataPoints++;
                }

                IsBalanceDataChganged = true;
            }

            return balanceChanged;
        }

        public static void ResetAccountStats()
        {
            AccountBalance = 0;
            AccountEquity = 0;
            AccountProfit = 0;
            AccountFreeMargin = 0;

            BalanceDataPoints = 0;
            BalanceData = new Balance_Chart_Unit[BalanceLenght];
        }

        public static bool SetCurrentPosition(int ticket, int type, double lots, double price, DateTime opentime,
                                              double stoploss, double takeprofit, double profit, string comment)
        {
            bool changed = PositionType != type ||
                           Math.Abs(PositionLots - lots) > Epsilon ||
                           Math.Abs(PositionOpenPrice - price) > Epsilon ||
                           Math.Abs(PositionStopLoss - stoploss) > Epsilon ||
                           Math.Abs(PositionTakeProfit - takeprofit) > Epsilon ||
                           PositionComment != comment;

            PositionTicket = ticket;
            PositionType = type;
            PositionLots = lots;
            PositionOpenPrice = price;
            PositionOpenTime = opentime;
            PositionStopLoss = stoploss;
            PositionTakeProfit = takeprofit;
            PositionProfit = profit;
            PositionComment = comment;

            DateTime barOpenTime = Time[Bars - 1];
            if (!_barStats.ContainsKey(barOpenTime))
            {
                _barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, PositionOpenPrice, PositionLots));
            }
            else
            {
                _barStats[barOpenTime].PositionDir = PositionDirection;
                _barStats[barOpenTime].PositionPrice = PositionOpenPrice;
                _barStats[barOpenTime].PositionLots = PositionLots;
            }

            if (changed && Configs.PlaySounds)
                SoundPositionChanged.Play();

            return changed;
        }

        public static void ResetPositionStats()
        {
            PositionTicket = 0;
            PositionType = -1;
            PositionLots = 0;
            PositionOpenPrice = 0;
            PositionOpenTime = DateTime.MinValue;
            PositionStopLoss = 0;
            PositionTakeProfit = 0;
            PositionProfit = 0;
            PositionComment = "";
        }

        public static void AddBarStats(OperationType operationType, double operationLots, double operationPrice)
        {
            DateTime barOpenTime = Time[Bars - 1];

            if (!_barStats.ContainsKey(barOpenTime))
                _barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, PositionOpenPrice, PositionLots));

            _barStats[barOpenTime].Operations.Add(new Operation(barOpenTime, operationType, DateTime.Now, operationLots,
                                                                operationPrice));

            if (_barStats.ContainsKey(Time[0]))
                _barStats.Remove(Time[0]);
        }

        public static void ResetBarStats()
        {
            _barStats = new Dictionary<DateTime, BarStats>();
        }
    }
}