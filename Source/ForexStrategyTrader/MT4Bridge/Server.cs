﻿// Server
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Globalization;
using MT4Bridge.NamedPipes;

namespace MT4Bridge
{
    internal class Server : IPipeServer
    {
        static readonly string UserName = System.Windows.Forms.SystemInformation.UserName;
        static readonly string ServerPipeName = "MT4-FST_" + UserName + "-";
        static int    _serverID;
        static string PipeName { get { return ServerPipeName + _serverID.ToString(CultureInfo.InvariantCulture); } }

        ServerPipe _pipe;
        private readonly Bridge _bridge;

        public Server(Bridge bridge, int id)
        {
            _serverID = id;
            _bridge = bridge;
            _pipe = new ServerPipe(PipeName, this);
        }

        ~Server()
        {
            Stop();
        }

        public void Stop()
        {
            if (_pipe != null) {
                try {
                    _pipe.Dispose();
                } finally {
                    _pipe = null;
                }
            }
        }

        public string Serve(string request)
        {
            if (request.Length < 2)
                return "ER Bad Request";

            string   cmd  = request.Substring(0, 2).ToUpper();
            string[] args = request.Length > 3 ? request.Substring(3).Split(new[] {' '}) : null;

            switch (cmd) {
                case "TI":
                    if (args == null || args.Length != 28)
                        return "ER Invalid Number of Arguments";

                    return Tick(args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8], args[9], args[10],
                                args[11], args[12], args[13], args[14], args[15], args[16], args[17], args[18], args[19], args[20],
                                args[21], args[22], args[23], args[24], args[25], args[26], args[27]);
            }
            return "ER Bad Command";
        }

        /// <summary>
        /// Converts MT timestamp to DateTime
        /// </summary>
        private DateTime FromTimestamp(int timestamp)
        {
            var time = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return time.AddSeconds(timestamp);
        }

        private string Tick(string symbol,   string aperiod,  string atime, string abid, string aask,   string aspread, string atickvalue,
                            string abartime, string aopen,    string ahigh, string alow, string aclose, string avolume, string abartime10,
                            string aaccountBalance,   string aaccountEquity,      string aaccountProfit,  string aaccountFreeMargin,
                            string apositionTicket,   string apositionType,       string apositionLots,   string apositionOpenPrice, string apositionOpenTime,
                            string apositionStopLoss, string apositionTakeProfit, string apositionProfit, string apositionComment,
                            string aparameters)
        {
            try 
            {
                var        period    = (PeriodType)(int.Parse(aperiod));
                DateTime   time      = FromTimestamp(int.Parse(atime));
                double     bid       = StringToDouble(abid);
                double     ask       = StringToDouble(aask);
                int        spread    = int.Parse(aspread);
                double     tickvalue = StringToDouble(atickvalue);

                DateTime bartime = FromTimestamp(int.Parse(abartime));
                double   open    = StringToDouble(aopen);
                double   high    = StringToDouble(ahigh);
                double   low     = StringToDouble(alow);
                double   close   = StringToDouble(aclose);
                int      volume  = int.Parse(avolume);

                DateTime bartime10 = FromTimestamp(int.Parse(abartime10));

                double   accountBalance     = StringToDouble(aaccountBalance);
                double   accountEquity      = StringToDouble(aaccountEquity);
                double   accountProfit      = StringToDouble(aaccountProfit);
                double   accountFreeMargin  = StringToDouble(aaccountFreeMargin);
                int      positionTicket     = int.Parse(apositionTicket);
                int      positionType       = int.Parse(apositionType);
                double   positionLots       = StringToDouble(apositionLots);
                double   positionOpenPrice  = StringToDouble(apositionOpenPrice);
                DateTime positionOpenTime   = FromTimestamp(int.Parse(apositionOpenTime));
                double   positionStopLoss   = StringToDouble(apositionStopLoss);
                double   positionTakeProfit = StringToDouble(apositionTakeProfit);
                double   positionProfit     = StringToDouble(apositionProfit);
                string   positionComment    = apositionComment;
                string   parameters         = aparameters;

                _bridge.BarsManager.UpdateBar(symbol, period, bartime, open, high, low, close, volume, bartime10);

                _bridge.Tick(symbol, period, bartime, time, bid, ask, spread, tickvalue,
                    accountBalance,   accountEquity,      accountProfit,  accountFreeMargin,
                    positionTicket,   positionType,       positionLots,   positionOpenPrice, positionOpenTime,
                    positionStopLoss, positionTakeProfit, positionProfit, positionComment,
                    parameters);
                
                return "OK";
            } catch (ArgumentException) {
                return "ER Invalid Argument";
            } catch (FormatException) {
                return "ER Invalid Argument";
            }
        }

        double StringToDouble(string input)
        {
            string sDecimalPoint = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(sDecimalPoint))
            {
                input = input.Replace(".", sDecimalPoint);
                input = input.Replace(",", sDecimalPoint);
            }

            double number;

            try
            {
                number = double.Parse(input);
            }
            catch
            {
                number = double.NaN;
            }

            return number;
        }
    }
}
