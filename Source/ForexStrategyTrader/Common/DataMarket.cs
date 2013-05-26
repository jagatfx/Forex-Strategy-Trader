//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Collections.Generic;
using ForexStrategyBuilder.Infrastructure.Enums;
using MT4Bridge;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Base class containing the data.
    /// </summary>
    public static partial class Data
    {
        private const double Epsilon = 0.000001;
        private const int BalanceLenght = 2000;

        private static double bid;
        private static double ask;
        private static double close;
        private static DateTime serverTime = DateTime.Now;
        private static List<double> listTicks = new List<double>();
        private static Dictionary<DateTime, BarStats> barStats = new Dictionary<DateTime, BarStats>();
        private static readonly DateTime FstStartTime = DateTime.Now;
        public static bool IsConnected { get; set; }
        public static int ConnectionId { get; set; }
        public static string TerminalName { get; set; }
        public static string ExpertVersion { get; set; }
        public static string LibraryVersion { get; set; }

        // The current instrument's properties.
        public static InstrumentProperties InstrProperties { get; set; }

        public static string Symbol
        {
            get { return InstrProperties.Symbol; }
        }

        public static DataPeriod Period { get; set; }

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
            get { return bid; }
            set
            {
                OldBid = bid;
                bid = value;
            }
        }

        public static double OldBid { get; private set; }

        public static double Ask
        {
            get { return ask; }
            set
            {
                OldAsk = ask;
                ask = value;
            }
        }

        public static double OldAsk { get; private set; }

        public static double LastClose
        {
            get { return close; }
            set
            {
                OldClose = close;
                close = value;
            }
        }

        public static double OldClose { get; private set; }

        public static DateTime ServerTime
        {
            get { return serverTime; }
            set { serverTime = value; }
        }

        public static List<double> ListTicks
        {
            get { return listTicks; }
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

        public static BalanceChartUnit[] BalanceData { get; private set; }
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

        public static int ConsecutiveLosses { get; set; }
        public static double ActivatedStopLoss { get; set; }
        public static double ActivatedTakeProfit { get; set; }
        public static double Closed_SL_TP_Lots { get; set; }

        public static Dictionary<DateTime, BarStats> BarStatistics
        {
            get { return barStats; }
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

        // Failed Close Order
        public static bool IsFailedCloseOrder { get; set; }
        public static int CloseOrderTickCounter { get; set; }
        public static bool IsSentCloseOrder { get; set; }

        public static void ResetBidAskClose()
        {
            OldBid = OldAsk = OldClose = 0;
            bid = ask = close = 0;
        }

        public static void SetTick(double tick)
        {
            listTicks.Add(tick);
            if (listTicks.Count > 60)
                listTicks.RemoveRange(0, 1);
        }

        public static void ResetTicks()
        {
            listTicks = new List<double>();
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
                var chartUnit = new BalanceChartUnit {Time = time, Balance = balance, Equity = equity};

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
            BalanceData = new BalanceChartUnit[BalanceLenght];
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
            if (!barStats.ContainsKey(barOpenTime))
            {
                barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, PositionOpenPrice, PositionLots));
            }
            else
            {
                barStats[barOpenTime].PositionDir = PositionDirection;
                barStats[barOpenTime].PositionPrice = PositionOpenPrice;
                barStats[barOpenTime].PositionLots = PositionLots;
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

            if (!barStats.ContainsKey(barOpenTime))
                barStats.Add(barOpenTime, new BarStats(barOpenTime, PositionDirection, PositionOpenPrice, PositionLots));

            barStats[barOpenTime].Operations.Add(new Operation(barOpenTime, operationType, DateTime.Now, operationLots,
                                                               operationPrice));

            if (barStats.ContainsKey(Time[0]))
                barStats.Remove(Time[0]);
        }

        public static void ResetBarStats()
        {
            barStats = new Dictionary<DateTime, BarStats>();
        }
    }
}