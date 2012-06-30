// Bridge
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using System.IO;

namespace MT4Bridge
{
    public class Bridge
    {
        Server _server;
        Client _client;
        internal BarsManager BarsManager = new BarsManager();

        int _code;
        public int LastError { get { return _code; } }

        ~Bridge()
        {
            Stop();
        }

        public void Start(int id)
        {
            _server = new Server(this, id);
            _client = new Client(this, id);
        }

        public void Stop()
        {
            if (_server != null) {
                try {
                    _server.Stop();
                } finally {
                    _server = null;
                }
            }
        }

        const string LogFilename = "bridge.log";
        static bool _isWriteLog = true;
        /// <summary>
        /// Sets if Bridge writes log file.
        /// </summary>
        public bool WriteLog { set { _isWriteLog = value; } }

        /// <summary>
        /// Writes a massage to the log file.
        /// </summary>
        internal static void Log(string message)
        {
            if (!_isWriteLog)
                return;
            
            lock (typeof(Bridge))
            {
                try
                {
                    using (StreamWriter sw = File.AppendText(LogFilename))
                        sw.WriteLine(DateTime.Now.ToString(CultureInfo.InvariantCulture) + " - " + message);
                }
                catch (Exception exception)
                { Console.WriteLine(exception.Message);}
            }
        }

        public PingInfo GetPingInfo()
        {
            PingInfo ping = _client.Ping();
            return ping;
        }

        public delegate void TickEventHandler(object source, TickEventArgs e);
        public event TickEventHandler OnTick;
        internal void Tick(string symbol, PeriodType period, DateTime bartime, DateTime time, double bid, double ask, int spread, double tickvalue,
                double accountBalance, double accountEquity, double accountProfit, double accountFreeMargin,
                int positionTicket, int positionType, double positionLots, double positionOpenPrice, DateTime positionOpenTime,
                double positionStopLoss, double positionTakeProfit, double positionProfit, string positionComment, string parameters)
        {
            if (OnTick != null)
            {
                var tickea = new TickEventArgs(symbol, period, bartime, time, bid, ask, spread, tickvalue,
                    accountBalance, accountEquity, accountProfit, accountFreeMargin,
                    positionTicket, positionType, positionLots, positionOpenPrice, positionOpenTime,
                    positionStopLoss, positionTakeProfit, positionProfit, positionComment, parameters);

                OnTick.BeginInvoke(this, tickea, null, null);
            }
        }

        public void ResetBarsManager()
        {
            BarsManager = new BarsManager();
        }

        public Bars GetBars(string symbol, PeriodType period)
        {
            return BarsManager.GetBars(symbol, period, _client);
        }

        public SymbolInfo GetSymbolInfo(string symbol)
        {
            return _client.Symbol(symbol);
        }

        public AccountInfo GetAccountInfo()
        {
            return _client.Account();
        }

        public double GetMarketInfo(string symbol, int mode)
        {
            return _client.MarketInfo(symbol, mode);
        }

        public MarketInfo GetMarketInfoAll(string symbol)
        {
            return _client.MarketInfoAll(symbol);
        }

        public TerminalInfo GetTerminalInfo()
        {
            return _client.Terminal();
        }

        public int[] Orders() { return Orders(null); }
        public int[] Orders(string symbol)
        {
            return _client.Orders(symbol);
        }

        public OrderInfo OrderInfo(int ticket)
        {
            return _client.OrderInfo(ticket);
        }

        bool SaveCode(Response response)
        {
            _code = response.Code;
            return response.OK;
        }

        public int OrderSend(string symbol, OrderType type, double lots, double price, int slippage, double stoploss, double takeprofit, string parameters)
        {
            return OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, 0, DateTime.MinValue, parameters);
        }
        public int OrderSend(string symbol, OrderType type, double lots, double price, int slippage, double stoploss, double takeprofit, int magic, DateTime expire, string parameters)
        {
            Response rc = _client.OrderSend(symbol, type, lots, price, slippage, stoploss, takeprofit, magic, expire, parameters);
            return SaveCode(rc) ? rc.Code : -1;
        }

        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, string parameters)
        {
            return OrderModify(ticket, price, stoploss, takeprofit, DateTime.MinValue, parameters);
        }
        public bool OrderModify(int ticket, double price, double stoploss, double takeprofit, DateTime expire, string parameters)
        {
            return SaveCode(_client.OrderModify(ticket, price, stoploss, takeprofit, expire, parameters));
        }

        public bool OrderClose(int ticket, double lots, double price, int slippage)
        {
            Response rc = _client.OrderClose(ticket, lots, price, slippage);
            return SaveCode(rc);
        }

        public bool OrderDelete(int ticket)
        {
            return SaveCode(_client.OrderDelete(ticket));
        }
    }
}
