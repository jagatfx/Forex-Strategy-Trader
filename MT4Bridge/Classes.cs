// Classes
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace MT4Bridge
{
    public enum PeriodType
    {
        M1  = 1,
        M5  = 5,
        M15 = 15,
        M30 = 30,
        H1  = 60,
        H4  = 240,
        D1  = 1440,
        W1  = 10080,
        MN1 = 43200 
    }

    public enum OrderType
    {
        Buy       = 0,
        Sell      = 1,
        BuyLimit  = 2,
        SellLimit = 3,
        BuyStop   = 4,
        SellStop  = 5
    }

    public class TickEventArgs : EventArgs
    {
        public string Symbol { get; private set; }
        public PeriodType Period { get; private set; }
        public DateTime BarTime { get; private set; }
        public DateTime Time { get; private set; }
        public double Bid { get; private set; }
        public double Ask { get; private set; }
        public int Spread { get; private set; }
        public double TickValue { get; private set; }
        public double AccountBalance { get; private set; }
        public double AccountEquity { get; private set; }
        public double AccountProfit { get; private set; }
        public double AccountFreeMargin { get; private set; }
        public int PositionTicket { get; private set; }
        public int PositionType { get; private set; }
        public double PositionLots { get; private set; }
        public double PositionOpenPrice { get; private set; }
        public DateTime PositionOpenTime { get; private set; }
        public double PositionStopLoss { get; private set; }
        public double PositionTakeProfit { get; private set; }
        public double PositionProfit { get; private set; }
        public string PositionComment { get; private set; }
        public string Parameters { get; private set; }

        public TickEventArgs(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask,
                             int spread, double tickvalue,
                             double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                             int positionTicket, int positionType, double positionLots, double positionOpenPrice,
                             DateTime positionOpenTime,
                             double positionStopLoss, double positionTakeProfit, double positionProfit,
                             string positionComment,
                             string parameters)
        {
            Symbol = symbol;
            Period = period;
            BarTime = bartime;
            Time = time;
            Bid = bid;
            Ask = ask;
            Spread = spread;
            TickValue = tickvalue;
            AccountBalance = accountBalance;
            AccountEquity = accountEquity;
            AccountProfit = accountProfit;
            AccountFreeMargin = accountFreeMargin;
            PositionTicket = positionTicket;
            PositionType = positionType;
            PositionLots = positionLots;
            PositionOpenPrice = positionOpenPrice;
            PositionOpenTime = positionOpenTime;
            PositionStopLoss = positionStopLoss;
            PositionTakeProfit = positionTakeProfit;
            PositionProfit = positionProfit;
            PositionComment = positionComment;
            Parameters = parameters;
        }
    }

    public class PingInfo
    {
        public string Symbol { get; private set; }
        public PeriodType Period { get; private set; }
        public DateTime BarTime { get; private set; }
        public DateTime Time { get; private set; }
        public double Bid { get; private set; }
        public double Ask { get; private set; }
        public int Spread { get; private set; }
        public double TickValue { get; private set; }
        public double AccountBalance { get; private set; }
        public double AccountEquity { get; private set; }
        public double AccountProfit { get; private set; }
        public double AccountFreeMargin { get; private set; }
        public int PositionTicket { get; private set; }
        public int PositionType { get; private set; }
        public double PositionLots { get; private set; }
        public double PositionOpenPrice { get; private set; }
        public DateTime PositionOpenTime { get; private set; }
        public double PositionStopLoss { get; private set; }
        public double PositionTakeProfit { get; private set; }
        public double PositionProfit { get; private set; }
        public string PositionComment { get; private set; }
        public string Parameters { get; private set; }

        public PingInfo(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask,
                        int spread, double tickvalue,
                        double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                        int positionTicket, int positionType, double positionLots, double positionOpenPrice,
                        DateTime positionOpenTime,
                        double positionStopLoss, double positionTakeProfit, double positionProfit,
                        string positionComment,
                        string parameters)
        {
            Symbol = symbol;
            Period = period;
            BarTime = bartime;
            Time = time;
            Bid = bid;
            Ask = ask;
            Spread = spread;
            TickValue = tickvalue;
            AccountBalance = accountBalance;
            AccountEquity = accountEquity;
            AccountProfit = accountProfit;
            AccountFreeMargin = accountFreeMargin;
            PositionTicket = positionTicket;
            PositionType = positionType;
            PositionLots = positionLots;
            PositionOpenPrice = positionOpenPrice;
            PositionOpenTime = positionOpenTime;
            PositionStopLoss = positionStopLoss;
            PositionTakeProfit = positionTakeProfit;
            PositionProfit = positionProfit;
            PositionComment = positionComment;
            Parameters = parameters;
        }
    }

    public class SymbolInfo
    {
        public string Symbol { get; private set; }
        public double Bid { get; private set; }
        public double Ask { get; private set; }
        public double Point { get; private set; }
        public double Spread { get; private set; }
        public double StopLevel { get; private set; }
        public int Digits { get; private set; }

        public SymbolInfo(string symbol, double bid, double ask, int digits, double point, double spread,
                          double stoplevel)
        {
            Symbol = symbol;
            Bid = bid;
            Ask = ask;
            Digits = digits;
            Point = point;
            Spread = spread;
            StopLevel = stoplevel;
        }
    }

    public class MarketInfo
    {
        public double ModePoint { get; private set; }
        public double ModeDigits { get; private set; }
        public double ModeSpread { get; private set; }
        public double ModeStopLevel { get; private set; }
        public double ModeLotSize { get; private set; }
        public double ModeTickValue { get; private set; }
        public double ModeTickSize { get; private set; }
        public double ModeSwapLong { get; private set; }
        public double ModeSwapShort { get; private set; }
        public double ModeStarting { get; private set; }
        public double ModeExpiration { get; private set; }
        public double ModeTradeAllowed { get; private set; }
        public double ModeMinLot { get; private set; }
        public double ModeLotStep { get; private set; }
        public double ModeMaxLot { get; private set; }
        public double ModeSwapType { get; private set; }
        public double ModeProfitCalcMode { get; private set; }
        public double ModeMarginCalcMode { get; private set; }
        public double ModeMarginInit { get; private set; }
        public double ModeMarginMaintenance { get; private set; }
        public double ModeMarginHedged { get; private set; }
        public double ModeMarginRequired { get; private set; }
        public double ModeFreezeLevel { get; private set; }

        public MarketInfo(
            double modePoint, double modeDigits, double modeSpread, double modeStopLevel, double modeLotSize,
            double modeTickValue, double modeTickSize, double modeSwapLong, double modeSwapShort, double modeStarting,
            double modeExpiration, double modeTradeAllowed, double modeMinLot, double modeLotStep, double modeMaxLot,
            double modeSwapType, double modeProfitCalcMode, double modeMarginCalcMode, double modeMarginInit,
            double modeMarginMaintenance, double modeMarginHedged, double modeMarginRequired, double modeFreezeLevel)
        {
            ModePoint = modePoint;
            ModeDigits = modeDigits;
            ModeSpread = modeSpread;
            ModeStopLevel = modeStopLevel;
            ModeLotSize = modeLotSize;
            ModeTickValue = modeTickValue;
            ModeTickSize = modeTickSize;
            ModeSwapLong = modeSwapLong;
            ModeSwapShort = modeSwapShort;
            ModeStarting = modeStarting;
            ModeExpiration = modeExpiration;
            ModeTradeAllowed = modeTradeAllowed;
            ModeMinLot = modeMinLot;
            ModeLotStep = modeLotStep;
            ModeMaxLot = modeMaxLot;
            ModeSwapType = modeSwapType;
            ModeProfitCalcMode = modeProfitCalcMode;
            ModeMarginCalcMode = modeMarginCalcMode;
            ModeMarginInit = modeMarginInit;
            ModeMarginMaintenance = modeMarginMaintenance;
            ModeMarginHedged = modeMarginHedged;
            ModeMarginRequired = modeMarginRequired;
            ModeFreezeLevel = modeFreezeLevel;
        }
    }

    public class AccountInfo
    {
        public string Name { get; private set; }
        public int Number { get; private set; }
        public string Company { get; private set; }
        public string Server { get; private set; }
        public string Currency { get; private set; }
        public int Leverage { get; private set; }
        public double Balance { get; private set; }
        public double Equity { get; private set; }
        public double Profit { get; private set; }
        public double Credit { get; private set; }
        public double Margin { get; private set; }
        public double FreeMarginMode { get; private set; }
        public double FreeMargin { get; private set; }
        public int StopOutMode { get; private set; }
        public int StopOutLevel { get; private set; }
        public bool IsDemo { get; private set; }

        public AccountInfo(string name, int number, string company, string server, string currency, int leverage,
                           double balance,
                           double equity, double profit, double credit, double margin, double freemarginmode,
                           double freemargin,
                           int stopoutmode, int stopoutlevel, bool isDemo)
        {
            Name = name;
            Number = number;
            Company = company;
            Server = server;
            Currency = currency;
            Leverage = leverage;
            Balance = balance;
            Equity = equity;
            Profit = profit;
            Credit = credit;
            Margin = margin;
            FreeMarginMode = freemarginmode;
            FreeMargin = freemargin;
            StopOutMode = stopoutmode;
            StopOutLevel = stopoutlevel;
            IsDemo = isDemo;
        }
    }

    public class TerminalInfo
    {
        public string TerminalName { get; private set; }
        public string TerminalCompany { get; private set; }
        public string TerminalPath { get; private set; }
        public string ExpertVersion { get; private set; }
        public string LibraryVersion { get; private set; }

        public TerminalInfo(string terminalName, string terminalCompany, string terminalPath, string expertVersion, string libraryVersion)
        {
            TerminalName    = terminalName;
            TerminalCompany = terminalCompany;
            TerminalPath    = terminalPath;
            ExpertVersion   = expertVersion;
            LibraryVersion  = libraryVersion;
        }
    }

    public class OrderInfo
    {
        public int Ticket { get; private set; }
        public int Magic { get; private set; }
        public OrderType Type { get; private set; }
        public string Symbol { get; private set; }
        public double Lots { get; private set; }
        public double Price { get; private set; }
        public double ClosePrice { get; private set; }
        public double StopLoss { get; private set; }
        public double TakeProfit { get; private set; }
        public double Profit { get; private set; }
        public DateTime Time { get; private set; }
        public DateTime CloseTime { get; private set; }
        public DateTime Expire { get; private set; }

        public OrderInfo(int ticket, string symbol, OrderType type, double lots,
            double price, double stoploss, double takeprofit, DateTime time,
            DateTime closetime, double closeprice, double profit, int magic, DateTime expire)
        {
            Ticket     = ticket;
            Symbol     = symbol;
            Type       = type;
            Lots       = lots;
            Price      = price;
            StopLoss   = stoploss;
            TakeProfit = takeprofit;
            Time       = time;
            CloseTime  = closetime;
            ClosePrice = closeprice;
            Profit     = profit;
            Magic      = magic;
            Expire     = expire;
        }
    }

    internal struct Response
    {
        public bool OK { get; private set; }
        public int Code { get; private set; }

        public Response(bool ok) : this(ok, 0)
        { }

        public Response(bool ok, int code) : this()
        {
            OK   = ok;
            Code = code;
        }
    }
}
