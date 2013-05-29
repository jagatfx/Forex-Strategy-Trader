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
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Properties;
using MT4Bridge;

namespace ForexStrategyBuilder
{
    public sealed partial class Actions
    {
        private const double Epsilon = 0.000001;
        private readonly object lockerDataFeed = new object();
        private readonly object lockerTickPing = new object();
        private int accountReconnect;
        private double activationReportedAt;
        private DateTime barOpenTimeForLastCloseEvent = DateTime.MinValue;
        private DateTime barOpenTimeForLastCloseTick = DateTime.MinValue;
        private DateTime barOpenTimeForLastOpenTick = DateTime.MinValue;
        private Bridge bridge;
        private bool isSetRootDataError;
        private bool isTrading;
        private bool nullPing = true;
        private DataPeriod periodReconnect;
        private int pingAttempt;
        private string symbolReconnect;
        private DateTime tickLocalTime = DateTime.MinValue;
        private DateTime tickServerTime = DateTime.MinValue;
        private Timer timerPing;

        /// <summary>
        ///     Initializes data feed.
        /// </summary>
        private void InitDataFeed()
        {
            tickLocalTime = DateTime.MinValue;

            bridge = new Bridge {WriteLog = Configs.BridgeWritesLog};
            bridge.Start(Data.ConnectionId);
            bridge.OnTick += Bridge_OnTick;

            timerPing = new Timer {Interval = 1000};
            timerPing.Tick += TimerPingTick;
            timerPing.Start();
        }

        /// <summary>
        ///     Reinitializes data feed.
        /// </summary>
        private void DeinitDataFeed()
        {
            if (timerPing != null)
                timerPing.Stop();

            StopTrade();

            if (bridge == null) return;
            bridge.OnTick -= Bridge_OnTick;
            bridge.Stop();
        }

        /// <summary>
        ///     Pings the server in order to check the connection.
        /// </summary>
        private void TimerPingTick(object sender, EventArgs e)
        {
            if (DateTime.Now < tickLocalTime.AddSeconds(1))
                return; // The last tick was soon enough.

            lock (lockerTickPing)
            {
                PingInfo ping = bridge.GetPingInfo();

                if (ping == null && !nullPing)
                {
                    // Wrong ping.
                    pingAttempt++;
                    if ((pingAttempt == 1 || pingAttempt%10 == 0) && JournalShowSystemMessages)
                    {
                        string message = Language.T("Unsuccessful ping") + " No " + pingAttempt + ".";
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now, message);
                        AppendJournalMessage(jmsgsys);
                        Log(message);
                    }
                    if (pingAttempt == 30)
                    {
                        string message = Language.T("There is no connection with MetaTrader.");
                        var jmsgsys = new JournalMessage(JournalIcons.Warning, DateTime.Now, message);
                        AppendJournalMessage(jmsgsys);
                        if (Configs.PlaySounds)
                            Data.SoundError.Play();
                        Log(message);
                    }
                    if (pingAttempt < 60)
                    {
                        SetConnIcon(pingAttempt < 30 ? 3 : 4);
                        return;
                    }

                    Disconnect();
                }
                else if (ping != null)
                {
                    // Successful ping.
                    nullPing = false;
                    bool bUpdateData = false;
                    if (!Data.IsConnected || IsChartChangeged(ping.Symbol, (DataPeriod) (int) ping.Period))
                    {
                        // Disconnected or chart change.
                        pingAttempt = 0;

                        if (JournalShowSystemMessages)
                        {
                            string msgsys = ping.Symbol + " " + ping.Period.ToString() + " " +
                                            Language.T("Successful ping.");
                            var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now, msgsys);
                            AppendJournalMessage(jmsgsys);
                            Log(msgsys);
                        }
                        StopTrade();
                        if (!UpdateDataFeedInfo(ping.Time, ping.Symbol, (DataPeriod) (int) ping.Period))
                            return;

                        Data.Bid = ping.Bid;
                        Data.Ask = ping.Ask;
                        Data.InstrProperties.Spread = ping.Spread;
                        Data.InstrProperties.TickValue = ping.TickValue;

                        Data.IsConnected = true;
                        bUpdateData = true;
                        SetFormText();

                        SetConnIcon(1);
                        SetTradeStrip();

                        if (Configs.PlaySounds)
                            Data.SoundConnect.Play();

                        TerminalInfo te = bridge.GetTerminalInfo();
                        string connection = Language.T("Connected to a MetaTrader terminal.");
                        if (te != null)
                        {
                            connection = string.Format(
                                Language.T("Connected to") + " {0} " + Language.T("by") + " {1}", te.TerminalName,
                                te.TerminalCompany);
                            Data.ExpertVersion = te.ExpertVersion;
                            Data.LibraryVersion = te.LibraryVersion;
                            Data.TerminalName = te.TerminalName;

                            if (Configs.WriteLogFile)
                            {
                                string fileNameHeader = ping.Symbol + "_" + ping.Period + "_" + "ID" + Data.ConnectionId +
                                                        "_";
                                Data.Logger.CreateLogFile(fileNameHeader);
                            }
                        }

                        SetLblConnectionText(connection);
                        string market = string.Format("{0} {1}", ping.Symbol, ping.Period);
                        SetConnMarketText(market);
                        string message = market + " " + connection;
                        var jmsg = new JournalMessage(JournalIcons.Ok, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);

                        // Check for reconnection.
                        if (IsRestartTrade())
                            StartTrade(); // Restart trade.
                    }
                    else if (pingAttempt > 0 && JournalShowSystemMessages)
                    {
                        // After a wrong ping.
                        pingAttempt = 0;

                        string message = ping.Symbol + " " + ping.Period.ToString() + " " +
                                         Language.T("Successful ping.");
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now, message);
                        AppendJournalMessage(jmsgsys);
                        Log(message);
                    }

                    bool isNewPrice = Math.Abs(Data.Bid - ping.Bid) > Data.InstrProperties.Point/2;
                    DateTime dtPingServerTime = tickServerTime.Add(DateTime.Now - tickLocalTime);

                    string sBid = ping.Bid.ToString(Data.Ff);
                    string sAsk = ping.Ask.ToString(Data.Ff);
                    SetLblBidAskText(sBid + " / " + sAsk);

                    Data.Bid = ping.Bid;
                    Data.Ask = ping.Ask;
                    Data.InstrProperties.Spread = ping.Spread;
                    Data.InstrProperties.TickValue = ping.TickValue;

                    Data.ServerTime = ping.Time;

                    bool isAccChanged = Data.SetCurrentAccount(ping.Time, ping.AccountBalance, ping.AccountEquity,
                                                               ping.AccountProfit, ping.AccountFreeMargin);
                    bool isPosChanged = Data.SetCurrentPosition(ping.PositionTicket, ping.PositionType,
                                                                ping.PositionLots, ping.PositionOpenPrice,
                                                                ping.PositionOpenTime,
                                                                ping.PositionStopLoss, ping.PositionTakeProfit,
                                                                ping.PositionProfit, ping.PositionComment);

                    ParseAndSetParametrs(ping.Parameters);
                    LogActivated_SL_TP();

                    SetDataAndCalculate(ping.Symbol, ping.Period, dtPingServerTime, isNewPrice, bUpdateData);

                    SetEquityInfoText(string.Format("{0:F2} {1}", ping.AccountEquity, Data.AccountCurrency));
                    ShowCurrentPosition(isPosChanged);

                    if (isAccChanged)
                    {
                        string message = string.Format(
                            Language.T("Account Balance") + " {0:F2}, " +
                            Language.T("Equity") + " {1:F2}, " +
                            Language.T("Profit") + " {2:F2}, " +
                            Language.T("Free Margin") + " {3:F2}",
                            ping.AccountBalance, ping.AccountEquity, ping.AccountProfit, ping.AccountFreeMargin);
                        var jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);
                    }

                    if (Data.IsBalanceDataChganged)
                        UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);

                    SetTickInfoText(string.Format("{0} {1} / {2}", ping.Time.ToString("HH:mm:ss"), sBid, sAsk));
                    SetConnIcon(1);

                    // Sends OrderModify on SL/TP errors
                    if (IsWrongStopsExecution())
                        ResendWrongStops();

                    // Check for failed close order.
                    CheckForFailedCloseOrder(ping.PositionLots);
                }
            }
        }

        /// <summary>
        ///     Bridge OnTick
        /// </summary>
        private void Bridge_OnTick(object source, TickEventArgs tea)
        {
            lock (lockerTickPing)
            {
                if (pingAttempt > 0 && JournalShowSystemMessages)
                {
                    string msgsys = tea.Symbol + " " + tea.Period + " " +
                                    Language.T("Tick received after an unsuccessful ping.");
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now, msgsys);
                    AppendJournalMessage(jmsgsys);
                    Log(msgsys);
                }
                pingAttempt = 0;

                if (!Data.IsConnected)
                    return;

                tickLocalTime = DateTime.Now;
                tickServerTime = tea.Time;
                if (IsChartChangeged(tea.Symbol, (DataPeriod) (int) tea.Period))
                {
                    StopTrade();
                    Data.IsConnected = false;
                    SetFormText();

                    if (Configs.PlaySounds)
                        Data.SoundDisconnect.Play();

                    string message = tea.Symbol + " " + tea.Period + " " +
                                     Language.T("Tick received from a different chart!");
                    var jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, message);
                    AppendJournalMessage(jmsg);
                    Log(message);

                    return;
                }

                bool bNewPrice = Math.Abs(Data.Bid - tea.Bid) > Data.InstrProperties.Point/2;

                Data.Bid = tea.Bid;
                Data.Ask = tea.Ask;
                Data.InstrProperties.Spread = tea.Spread;
                Data.InstrProperties.TickValue = tea.TickValue;

                Data.ServerTime = tea.Time;

                Data.SetTick(tea.Bid);

                bool isAccChanged = Data.SetCurrentAccount(tea.Time, tea.AccountBalance, tea.AccountEquity,
                                                           tea.AccountProfit, tea.AccountFreeMargin);
                bool isPosChanged = Data.SetCurrentPosition(tea.PositionTicket, tea.PositionType, tea.PositionLots,
                                                            tea.PositionOpenPrice, tea.PositionOpenTime,
                                                            tea.PositionStopLoss, tea.PositionTakeProfit,
                                                            tea.PositionProfit, tea.PositionComment);

                ParseAndSetParametrs(tea.Parameters);
                LogActivated_SL_TP();

                const bool updateData = true;
                SetDataAndCalculate(tea.Symbol, tea.Period, tea.Time, bNewPrice, updateData);

                string bidText = tea.Bid.ToString(Data.Ff);
                string askText = tea.Ask.ToString(Data.Ff);
                SetLblBidAskText(bidText + " / " + askText);

                // Tick data label
                if (JournalShowTicks)
                {
                    string tickInfo = string.Format("{0} {1} {2} {3} / {4}", tea.Symbol, tea.Period,
                                                    tea.Time.ToString("HH:mm:ss"), bidText, askText);
                    var jmsg = new JournalMessage(JournalIcons.Globe, DateTime.Now, tickInfo);
                    AppendJournalMessage(jmsg);
                }

                UpdateTickChart(Data.InstrProperties.Point, Data.ListTicks.ToArray());
                SetEquityInfoText(string.Format("{0:F2} {1}", tea.AccountEquity, Data.AccountCurrency));
                ShowCurrentPosition(isPosChanged);

                if (isAccChanged)
                {
                    string message = string.Format(
                        Language.T("Account Balance") + " {0:F2}, " +
                        Language.T("Equity") + " {1:F2}, " +
                        Language.T("Profit") + " {2:F2}, " +
                        Language.T("Free Margin") + " {3:F2}",
                        tea.AccountBalance, tea.AccountEquity, tea.AccountProfit,
                        tea.AccountFreeMargin);
                    var jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now, message);
                    AppendJournalMessage(jmsg);
                    Log(message);
                }

                if (Data.IsBalanceDataChganged)
                    UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);

                SetTickInfoText(string.Format("{0} {1} / {2}", tea.Time.ToString("HH:mm:ss"), bidText, askText));
                SetConnIcon(2);

                // Sends OrderModify on SL/TP errors
                if (IsWrongStopsExecution())
                    ResendWrongStops();

                // Check for failed close order.
                CheckForFailedCloseOrder(tea.PositionLots);
            }
        }

        /// <summary>
        ///     Stops connection to MT
        /// </summary>
        private void Disconnect()
        {
            nullPing = true;
            pingAttempt = 0;
            if (Data.IsConnected && Configs.PlaySounds)
                Data.SoundDisconnect.Play();

            Data.IsConnected = false;
            StopTrade();

            string message = Language.T("Not Connected");
            var jmsg = new JournalMessage(JournalIcons.Blocked, DateTime.Now, message);
            AppendJournalMessage(jmsg);
            Log(message);

            Data.Bid = 0;
            Data.Ask = 0;
            Data.SetCurrentAccount(DateTime.MinValue, 0, 0, 0, 0);
            bool isPosChanged = Data.SetCurrentPosition(0, -1, 0, 0, DateTime.MinValue, 0, 0, 0, "");
            ShowCurrentPosition(isPosChanged);
            SetEquityInfoText(string.Format("{0} {1}", 0, Data.AccountCurrency));
            UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);
            SetTradeStrip();
            SetConnMarketText(Language.T("Not Connected"));
            SetLblConnectionText(Language.T("Not Connected"));
            SetConnIcon(0);
            SetTickInfoText("");
            SetLblSymbolText("");
            SetFormText();
        }

        /// <summary>
        ///     Check if the incoming data is from the same chart.
        /// </summary>
        private bool IsChartChangeged(string symbol, DataPeriod period)
        {
            if (!Data.IsConnected)
                return true;

            if (Data.Symbol != symbol || Data.Period != period)
                return true;

            return false;
        }

        /// <summary>
        ///     Sets the instrument's properties after connecting;
        /// </summary>
        private bool UpdateDataFeedInfo(DateTime time, string symbol, DataPeriod period)
        {
            lock (lockerDataFeed)
            {
                Data.ResetBidAskClose();
                Data.ResetAccountStats();
                Data.ResetPositionStats();
                Data.ResetBarStats();
                Data.ResetTicks();

                // Reads market info from the chart
                MT4Bridge.MarketInfo marketInfo = bridge.GetMarketInfoAll(symbol);
                if (marketInfo == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot update market info."));
                        AppendJournalMessage(jmsgsys);
                        Log(jmsgsys.Message);
                    }
                    return false;
                }

                // Sets instrument properties
                Data.Period = period;
                Data.InstrProperties.Symbol = symbol;
                Data.InstrProperties.LotSize = (int) marketInfo.ModeLotSize;
                Data.InstrProperties.MinLot = marketInfo.ModeMinLot;
                Data.InstrProperties.MaxLot = marketInfo.ModeMaxLot;
                Data.InstrProperties.LotStep = marketInfo.ModeLotStep;
                Data.InstrProperties.Digits = (int) marketInfo.ModeDigits;
                Data.InstrProperties.Spread = marketInfo.ModeSpread;
                Data.InstrProperties.SwapLong = marketInfo.ModeSwapLong;
                Data.InstrProperties.SwapShort = marketInfo.ModeSwapShort;
                Data.InstrProperties.TickValue = marketInfo.ModeTickValue;
                Data.InstrProperties.StopLevel = marketInfo.ModeStopLevel;
                Data.InstrProperties.MarginRequired = marketInfo.ModeMarginRequired;

                SetNumUpDownLots(marketInfo.ModeMinLot, marketInfo.ModeLotStep, marketInfo.ModeMaxLot);

                // Sets Market Info
                var values = new[]
                    {
                        symbol,
                        Data.DataPeriodToString(period),
                        marketInfo.ModeLotSize.ToString(CultureInfo.InvariantCulture),
                        marketInfo.ModePoint.ToString("F" + marketInfo.ModeDigits.ToString(CultureInfo.InvariantCulture))
                        ,
                        marketInfo.ModeSpread.ToString(CultureInfo.InvariantCulture),
                        marketInfo.ModeSwapLong.ToString(CultureInfo.InvariantCulture),
                        marketInfo.ModeSwapShort.ToString(CultureInfo.InvariantCulture)
                    };
                UpdateStatusPageMarketInfo(values);

                Bars bars = bridge.GetBars(symbol, (PeriodType) (int) period);
                if (bars == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot receive bars!"));
                        AppendJournalMessage(jmsgsys);
                        Log(jmsgsys.Message);
                    }
                    return false;
                }
                if (bars.Count < MaxBarsCount((int) period))
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsgsys = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot receive enough bars!"));
                        AppendJournalMessage(jmsgsys);
                        Log(jmsgsys.Message);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                     symbol + " " + (PeriodType) (int) period + " " +
                                                     Language.T("Market data updated, bars downloaded."));
                    AppendJournalMessage(jmsgsys);
                    Log(jmsgsys.Message);
                }

                // Account Information.
                AccountInfo account = bridge.GetAccountInfo();
                if (account == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsgsys = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot receive account information!"));
                        AppendJournalMessage(jmsgsys);
                        Log(jmsgsys.Message);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                     symbol + " " + (PeriodType) (int) period + " " +
                                                     Language.T("Account information received."));
                    AppendJournalMessage(jmsgsys);
                    Log(jmsgsys.Message);
                }
                Data.AccountName = account.Name;
                Data.IsDemoAccount = account.IsDemo;
                Data.AccountCurrency = account.Currency;
                Data.SetCurrentAccount(time, account.Balance, account.Equity, account.Profit, account.FreeMargin);
                UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);

                SetTradeStrip();
                SetLblSymbolText(symbol);
            }

            return true;
        }

        /// <summary>
        ///     Copies data to Data and calculates.
        /// </summary>
        private void SetDataAndCalculate(string symbol, PeriodType period, DateTime time, bool isPriceChange,
                                         bool isUpdateData)
        {
            lock (lockerDataFeed)
            {
                bool isUpdateChart = isUpdateData;

                Bars bars = bridge.GetBars(symbol, period);

                if (bars == null && JournalShowSystemMessages)
                {
                    isSetRootDataError = true;
                    Data.SoundError.Play();
                    var jmsgsys = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                     symbol + " " + period + " " +
                                                     Language.T("Cannot receive bars!"));
                    AppendJournalMessage(jmsgsys);
                    Log(jmsgsys.Message);
                    return;
                }
                if (bars != null && (bars.Count < MaxBarsCount((int) period) && JournalShowSystemMessages))
                {
                    isSetRootDataError = true;
                    Data.SoundError.Play();
                    var jmsgsys = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                     symbol + " " + period + " " +
                                                     Language.T("Cannot receive enough bars!"));
                    AppendJournalMessage(jmsgsys);
                    Log(jmsgsys.Message);
                    return;
                }
                if (isSetRootDataError && JournalShowSystemMessages)
                {
                    isSetRootDataError = false;
                    var jmsgsys = new JournalMessage(JournalIcons.Information, DateTime.Now,
                                                     symbol + " " + period + " " +
                                                     Language.T("Enough bars received!"));
                    AppendJournalMessage(jmsgsys);
                    Log(jmsgsys.Message);
                }

                if (bars != null)
                {
                    int countBars = bars.Count;

                    if (countBars < 400)
                        return;

                    if (Data.Bars != countBars ||
                        Data.Time[countBars - 1] != bars.Time[countBars - 1] ||
                        Data.Volume[countBars - 1] != bars.Volume[countBars - 1] ||
                        Math.Abs(Data.Close[countBars - 1] - bars.Close[countBars - 1]) > Data.InstrProperties.Point/2d)
                    {
                        if (Data.Bars == countBars && Data.Time[countBars - 1] == bars.Time[countBars - 1] &&
                            Data.Time[countBars - 10] == bars.Time[countBars - 10])
                        {
                            // Update the last bar only.
                            Data.Open[countBars - 1] = bars.Open[countBars - 1];
                            Data.High[countBars - 1] = bars.High[countBars - 1];
                            Data.Low[countBars - 1] = bars.Low[countBars - 1];
                            Data.Close[countBars - 1] = bars.Close[countBars - 1];
                            Data.Volume[countBars - 1] = bars.Volume[countBars - 1];
                        }
                        else
                        {
                            // Update all the bars.
                            Data.Bars = countBars;
                            Data.Time = new DateTime[countBars];
                            Data.Open = new double[countBars];
                            Data.High = new double[countBars];
                            Data.Low = new double[countBars];
                            Data.Close = new double[countBars];
                            Data.Volume = new int[countBars];
                            bars.Time.CopyTo(Data.Time, 0);
                            bars.Open.CopyTo(Data.Open, 0);
                            bars.High.CopyTo(Data.High, 0);
                            bars.Low.CopyTo(Data.Low, 0);
                            bars.Close.CopyTo(Data.Close, 0);
                            bars.Volume.CopyTo(Data.Volume, 0);
                        }

                        Data.LastClose = Data.Close[countBars - 1];
                        CalculateStrategy(true);
                        isUpdateChart = true;
                    }
                }

                bool isBarChanged = IsBarChanged(Data.Time[Data.Bars - 1]);

                if (isTrading)
                {
                    TickType tickType = GetTickType((DataPeriods) (int) period, Data.Time[Data.Bars - 1], time,
                                                    Data.Volume[Data.Bars - 1]);

                    if (tickType == TickType.Close || isPriceChange || isBarChanged)
                    {
                        if (JournalShowSystemMessages && tickType != TickType.Regular)
                        {
                            var icon = JournalIcons.Warning;
                            string text = string.Empty;
                            if (tickType == TickType.Open)
                            {
                                icon = JournalIcons.BarOpen;
                                text = Language.T("A Bar Open event!");
                            }
                            else if (tickType == TickType.Close)
                            {
                                icon = JournalIcons.BarClose;
                                text = Language.T("A Bar Close event!");
                            }
                            else if (tickType == TickType.AfterClose)
                            {
                                icon = JournalIcons.Warning;
                                text = Language.T("A new tick arrived after a Bar Close event!");
                            }
                            var jmsgsys = new JournalMessage(icon, DateTime.Now,
                                                             symbol + " " + Data.PeriodMtStr + " " +
                                                             time.ToString("HH:mm:ss") + " " + text);
                            AppendJournalMessage(jmsgsys);
                            Log(jmsgsys.Message);
                        }

                        if (isBarChanged && tickType == TickType.Regular)
                        {
                            if (JournalShowSystemMessages)
                            {
                                var jmsgsys = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                                                 symbol + " " + Data.PeriodMtStr + " " +
                                                                 time.ToString("HH:mm:ss") + " A Bar Changed event!");
                                AppendJournalMessage(jmsgsys);
                                Log(jmsgsys.Message);
                            }

                            tickType = TickType.Open;
                        }

                        if (tickType == TickType.Open && barOpenTimeForLastCloseEvent == Data.Time[Data.Bars - 3])
                        {
                            if (JournalShowSystemMessages)
                            {
                                var jmsgsys = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                                                 symbol + " " + Data.PeriodMtStr + " " +
                                                                 time.ToString("HH:mm:ss") +
                                                                 " A secondary Bar Close event!");
                                AppendJournalMessage(jmsgsys);
                                Log(jmsgsys.Message);
                            }
                            tickType = TickType.OpenClose;
                        }

                        CalculateTrade(tickType);
                        isUpdateChart = true;

                        if (tickType == TickType.Close || tickType == TickType.OpenClose)
                            barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];
                    }
                }

                if (isUpdateChart)
                    UpdateChart();
            }
        }

        /// <summary>
        ///     Shows if the bar was changed.
        /// </summary>
        private bool IsBarChanged(DateTime open)
        {
            bool barChanged = barOpenTimeForLastOpenTick != open;
            barOpenTimeForLastOpenTick = open;

            return barChanged;
        }

        /// <summary>
        ///     Gets the tick type depending on its time.
        /// </summary>
        private TickType GetTickType(DataPeriods period, DateTime open, DateTime time, int volume)
        {
            var tick = TickType.Regular;
            bool isOpen = volume == 1 && barOpenTimeForLastOpenTick != open;
            bool isClose = (open.AddMinutes((int) period) - time) < TimeSpan.FromSeconds(Configs.BarCloseAdvance);

            if (isOpen)
            {
                barOpenTimeForLastCloseTick = DateTime.MinValue;
                tick = TickType.Open;
            }

            if (isClose)
            {
                if (barOpenTimeForLastCloseTick == open)
                {
                    tick = TickType.AfterClose;
                }
                else
                {
                    barOpenTimeForLastCloseTick = open;
                    tick = isOpen ? TickType.OpenClose : TickType.Close;
                }
            }

            return tick;
        }

        /// <summary>
        ///     Returns the count of chart's bars.
        /// </summary>
        private int MaxBarsCount(int period)
        {
            return Math.Max(2*1440/period + 10, Configs.MinChartBars);
        }

        /// <summary>
        ///     Manipulates additional params coming from MetaTrader.
        /// </summary>
        private void ParseAndSetParametrs(string parameters)
        {
            if (string.IsNullOrEmpty(parameters)) return;

            string[] splitParams = parameters.Split(new[] {';'});

            foreach (string param in splitParams)
            {
                string[] pair = param.Split(new[] {'='}, 2);
                string name = pair[0];
                string rowValue = pair[1];

                switch (name)
                {
                    case "cl":
                        Data.ConsecutiveLosses = int.Parse(rowValue);
                        break;
                    case "aSL":
                        Data.ActivatedStopLoss = StringToDouble(rowValue);
                        break;
                    case "aTP":
                        Data.ActivatedTakeProfit = StringToDouble(rowValue);
                        break;
                    case "al":
                        Data.Closed_SL_TP_Lots = StringToDouble(rowValue);
                        break;
                }
            }
        }

        private void LogActivated_SL_TP()
        {
            if (Data.ActivatedStopLoss < Epsilon && Data.ActivatedTakeProfit < Epsilon)
            {
                // There is nothing to report.
                activationReportedAt = 0;
                return;
            }

            if (Data.ActivatedStopLoss > Epsilon && Math.Abs(Data.ActivatedStopLoss - activationReportedAt) < Epsilon ||
                Data.ActivatedTakeProfit > Epsilon &&
                Math.Abs(Data.ActivatedTakeProfit - activationReportedAt) < Epsilon)
            {
                // Activation was already reported.
                return;
            }

            if (Data.ActivatedStopLoss > Epsilon && Data.ActivatedTakeProfit > Epsilon)
            {
                // Expert was not sure which one was activated, so reported both.
                Data.AddBarStats(OperationType.Close, Data.Closed_SL_TP_Lots, Data.ActivatedStopLoss);
                string message = Data.Symbol + " " + Data.PeriodMtStr + " " +
                                 Language.T("Position closed at") + " " + Data.ActivatedStopLoss.ToString(Data.Ff) +
                                 ", " +
                                 Language.T("Closed Lots") + " " + Data.Closed_SL_TP_Lots.ToString("F2");
                var msg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
                AppendJournalMessage(msg);
                Log(message);
                activationReportedAt = Data.ActivatedStopLoss;
            }
            else if (Data.ActivatedStopLoss > Epsilon)
            {
                // Activated Stop Loss
                Data.AddBarStats(OperationType.Close, Data.Closed_SL_TP_Lots, Data.ActivatedStopLoss);
                string message = Data.Symbol + " " + Data.PeriodMtStr + " " +
                                 Language.T("Activated Stop Loss at") + " " + Data.ActivatedStopLoss.ToString(Data.Ff) +
                                 ", " +
                                 Language.T("Closed Lots") + " " + Data.Closed_SL_TP_Lots.ToString("F2");
                var msg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
                AppendJournalMessage(msg);
                Log(message);
                activationReportedAt = Data.ActivatedStopLoss;
            }
            else if (Data.ActivatedTakeProfit > Epsilon)
            {
                // Activated Take Profit
                Data.AddBarStats(OperationType.Close, Data.Closed_SL_TP_Lots, Data.ActivatedTakeProfit);
                string message = Data.Symbol + " " + Data.PeriodMtStr + " " +
                                 Language.T("Activated Take Profit at") + " " +
                                 Data.ActivatedTakeProfit.ToString(Data.Ff) + ", " +
                                 Language.T("Closed Lots") + " " + Data.Closed_SL_TP_Lots.ToString("F2");
                var msg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
                AppendJournalMessage(msg);
                Log(message);
                activationReportedAt = Data.ActivatedTakeProfit;
            }
        }

        private void CheckForFailedCloseOrder(double lots)
        {
            // Check for failed close order.
            if (Data.IsSentCloseOrder && lots > Epsilon)
            {
                Data.CloseOrderTickCounter++;
                // Wait for some ticks or pings to pass.
                if (Data.CloseOrderTickCounter > 5)
                {
                    Data.IsFailedCloseOrder = true;
                    Data.IsSentCloseOrder = false;
                    Data.CloseOrderTickCounter = 0;

                    ActivateFailedCloseOrder();
                }
            }

                // Position was closed.
            else if (Data.IsSentCloseOrder && lots < Epsilon)
            {
                Data.IsSentCloseOrder = false;
            }

                // Check for successful close.
            else if (Data.IsFailedCloseOrder && lots < Epsilon)
            {
                Data.IsFailedCloseOrder = false;
                Data.IsSentCloseOrder = false;
                Data.CloseOrderTickCounter = 0;

                RecoverFailedCloseOrder();
            }
        }

        /// <summary>
        ///     Show warning message and start resending.
        /// </summary>
        private void ActivateFailedCloseOrder()
        {
            string message = Language.T("Activated resending of failed close orders.");
            var jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now, message);
            AppendJournalMessage(jmsg);
            Log(message);

            ActivateWarningMessage();
        }

        /// <summary>
        ///     Close warning message and stop resending.
        /// </summary>
        private void RecoverFailedCloseOrder()
        {
            string message = Language.T("Deactivated resending of failed close orders.");
            var jmsg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
            AppendJournalMessage(jmsg);
            Log(message);

            DeactivateWarningMessage();
        }

        /// <summary>
        ///     Starts the trade.
        /// </summary>
        private void StartTrade()
        {
            if (Data.Strategy.FirstBar > Data.Bars)
            {
                string errorMessage = string.Format("The strategy requires {0} bars, but there are {1} bars loaded.",
                                                    Data.Strategy.FirstBar, Data.Bars);
                errorMessage += Environment.NewLine +
                                Language.T("Check \"Trade settings -> Minimum number of bars in the chart\" option.");
                MessageBox.Show(errorMessage, Language.T("Strategy"));
                return;
            }

            isTrading = true;

            // Resets trade global variables.
            InitTrade();

            Data.SetStartTradingTime();
            string message = Data.Symbol + " " + Data.PeriodMtStr + " " + Language.T("Automatic trade started.");
            var msg = new JournalMessage(JournalIcons.StartTrading, DateTime.Now, message);
            AppendJournalMessage(msg);
            Log(message);

            symbolReconnect = Data.Symbol;
            periodReconnect = Data.Period;
            accountReconnect = Data.AccountNumber;
            barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];

            SetTradeStrip();
        }

        /// <summary>
        ///     Stops the trade.
        /// </summary>
        private void StopTrade()
        {
            if (!isTrading)
                return;

            isTrading = false;

            DeinitTrade();

            Data.SetStopTradingTime();

            string message = Data.Symbol + " " + Data.PeriodMtStr + " " + Language.T("Automatic trade stopped.");
            var msg = new JournalMessage(JournalIcons.StopTrading, DateTime.Now, message);
            AppendJournalMessage(msg);
            Log(message);
            SetTradeStrip();
        }

        private bool IsRestartTrade()
        {
            // Restart trade if we reopen expert on the previous chart.
            if (symbolReconnect == Data.Symbol &&
                periodReconnect == Data.Period &&
                accountReconnect == Data.AccountNumber) return true;

            // Start trade once after starting FSB from the auto start script.
            if (Data.StartAutotradeWhenConnected)
            {
                Data.StartAutotradeWhenConnected = false;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Shows current position on the status bar.
        /// </summary>
        private void ShowCurrentPosition(bool showInJournal)
        {
            if (!Data.IsConnected)
            {
                SetPositionInfoText(null, String.Empty);
                return;
            }

            string format = Data.Ff;
            string text = Language.T("Square");
            var icon = JournalIcons.PosSquare;
            Image img = Resources.pos_square;
            if (Data.PositionTicket > 0)
            {
                img = (Data.PositionType == 0 ? Resources.pos_buy : Resources.pos_sell);
                icon = (Data.PositionType == 0 ? JournalIcons.PosBuy : JournalIcons.PosSell);
                text = string.Format((Data.PositionType == 0 ? Language.T("Long") : Language.T("Short")) + " {0} " +
                                     LotOrLots(Data.PositionLots) + " " + Language.T("at") + " {1}, " +
                                     Language.T("Stop Loss") + " {2}, " +
                                     Language.T("Take Profit") + " {3}, " +
                                     Language.T("Profit") + " {4} " + Data.AccountCurrency,
                                     Data.PositionLots,
                                     Data.PositionOpenPrice.ToString(format),
                                     Data.PositionStopLoss.ToString(format), Data.PositionTakeProfit.ToString(format),
                                     Data.PositionProfit.ToString("F2"));
            }

            SetPositionInfoText(img, text);

            if (showInJournal)
            {
                string message = string.Format(Data.Symbol + " " + Data.PeriodMtStr + " " + text);
                var jmsg = new JournalMessage(icon, DateTime.Now, message);
                AppendJournalMessage(jmsg);
                Log(message);
            }
        }

        /// <summary>
        ///     Sets the trade button's icon and text.
        /// </summary>
        private void SetTradeStrip()
        {
            string connectText = Data.IsConnected
                                     ? Language.T("Connected to") + " " + Data.Symbol + " " + Data.PeriodMtStr
                                     : Language.T("Not Connected");

            SetTradeStripThreadSafely(connectText, Data.IsConnected, isTrading);
        }

        private void SetTradeStripThreadSafely(string connectText, bool isConnectedNow, bool isTradingNow)
        {
            if (TsTradeControl.InvokeRequired)
            {
                TsTradeControl.BeginInvoke(new SetTradeStripDelegate(SetTradeStripThreadSafely),
                                           new object[] {connectText, isConnectedNow, isTradingNow});
            }
            else
            {
                // Sets label Connection
                TslblConnection.Text = connectText;

                // Sets button Automatic Execution
                if (isTradingNow)
                {
                    TsbtnTrading.Image = Resources.stop;
                    TsbtnTrading.Text = Language.T("Stop Automatic Execution");
                }
                else
                {
                    TsbtnTrading.Image = Resources.play;
                    TsbtnTrading.Text = Language.T("Start Automatic Execution");
                }
                TsbtnTrading.Enabled = isConnectedNow;
                TsbtnConnectionHelp.Visible = !isConnectedNow;
            }
        }

        /// <summary>
        ///     Menu Multiple Instances Changed
        /// </summary>
        protected override void MenuMultipleInstances_OnClick(object sender, EventArgs e)
        {
            Configs.MultipleInstances = ((ToolStripMenuItem) sender).Checked;
            if (Configs.MultipleInstances)
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();

                TsTradeControl.SuspendLayout();
                TsbtnChangeID.Visible = false;
                TslblConnection.Visible = false;
                TsbtnTrading.Visible = false;

                TslblConnectionID.Visible = true;
                TstbxConnectionID.Visible = true;
                TsbtnConnectionGo.Visible = true;
                TsTradeControl.ResumeLayout();
            }
            else
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();
                Data.ConnectionId = 0;

                TsTradeControl.SuspendLayout();
                TsbtnChangeID.Visible = false;
                TslblConnection.Visible = true;
                TsbtnTrading.Visible = true;

                TslblConnectionID.Visible = false;
                TstbxConnectionID.Visible = false;
                TsbtnConnectionGo.Visible = false;
                TsTradeControl.ResumeLayout();

                InitDataFeed();
            }

            symbolReconnect = "";
        }

        /// <summary>
        ///     ID changed.
        /// </summary>
        protected override void TstbxConnectionIdKeyPress(object sender, KeyPressEventArgs e)
        {
            TsbtnConnectionGo.Enabled = true;
            if (e.KeyChar == (char) Keys.Return)
                ConnectionGo();
        }

        /// <summary>
        ///     Sets Connection ID
        /// </summary>
        private void ConnectionGo()
        {
            try
            {
                int id = int.Parse(TstbxConnectionID.Text);
                if (id >= 0)
                {
                    Data.ConnectionId = id;
                    Disconnect();

                    TsTradeControl.SuspendLayout();
                    TsbtnChangeID.Text = string.Format("ID = {0}", TstbxConnectionID.Text);
                    TsbtnChangeID.Visible = true;
                    TslblConnection.Visible = true;
                    TsbtnTrading.Visible = true;
                    TslblConnectionID.Visible = false;
                    TstbxConnectionID.Visible = false;
                    TsbtnConnectionGo.Visible = false;
                    TsTradeControl.ResumeLayout();

                    InitDataFeed();
                }
                else
                {
                    TstbxConnectionID.Text = "";
                }
            }
            catch
            {
                TsbtnConnectionGo.Enabled = false;
                TstbxConnectionID.Text = "";
            }
        }

        /// <summary>
        ///     Button Change ID clicked.
        /// </summary>
        protected override void TsbtChangeIdClick(object sender, EventArgs e)
        {
            Disconnect();
            DeinitDataFeed();

            TsTradeControl.SuspendLayout();
            TsbtnChangeID.Visible = false;
            TslblConnection.Visible = false;
            TsbtnTrading.Visible = false;

            TslblConnectionID.Visible = true;
            TstbxConnectionID.Visible = true;
            TsbtnConnectionGo.Visible = true;
            TsTradeControl.ResumeLayout();
        }

        /// <summary>
        ///     Button Connection Go clicked.
        /// </summary>
        protected override void TsbtConnectionGoClick(object sender, EventArgs e)
        {
            ConnectionGo();
        }

        protected override void TsbtTradingClick(object sender, EventArgs e)
        {
            if (isTrading)
            {
                StopTrade();

                symbolReconnect = "";
                periodReconnect = DataPeriod.W1;
                accountReconnect = 0;
            }
            else
            {
                StartTrade();
            }
        }

        protected override void BtnShowAccountInfoClick(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            AccountInfo ai = bridge.GetAccountInfo();
            if (ai == null)
            {
                SetBarDataText("   " + Language.T("Cannot receive account information!"));
                return;
            }

            var parameters = new[]
                {
                    "Name",
                    "Number",
                    "Company",
                    "Server",
                    "Currency",
                    "Leverage",
                    "Balance",
                    "Equity",
                    "Profit",
                    "Credit",
                    "Margin",
                    "Free margin mode",
                    "Free margin",
                    "Stop out mode",
                    "Stop out level",
                    "Is demo account"
                };

            var values = new[]
                {
                    ai.Name,
                    ai.Number.ToString(CultureInfo.InvariantCulture),
                    ai.Company,
                    ai.Server,
                    ai.Currency,
                    ai.Leverage.ToString(CultureInfo.InvariantCulture),
                    ai.Balance.ToString(CultureInfo.InvariantCulture),
                    ai.Equity.ToString(CultureInfo.InvariantCulture),
                    ai.Profit.ToString(CultureInfo.InvariantCulture),
                    ai.Credit.ToString(CultureInfo.InvariantCulture),
                    ai.Margin.ToString(CultureInfo.InvariantCulture),
                    ai.FreeMarginMode.ToString(CultureInfo.InvariantCulture),
                    ai.FreeMargin.ToString(CultureInfo.InvariantCulture),
                    ai.StopOutMode.ToString(CultureInfo.InvariantCulture),
                    ai.StopOutLevel.ToString(CultureInfo.InvariantCulture),
                    ai.IsDemo ? "Yes" : "No"
                };

            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Length; i++)
            {
                sb.AppendLine(string.Format("      {0,-25} {1}", parameters[i], values[i]));
            }
            SetBarDataText(sb.ToString());
        }

        protected override void BtnShowMarketInfoClick(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            MT4Bridge.MarketInfo mi = bridge.GetMarketInfoAll(Data.Symbol);
            if (mi == null)
            {
                SetBarDataText("   " + Language.T("Cannot update market info."));
                return;
            }

            var asMiParams = new[]
                {
                    "Point",
                    "Digit",
                    "Spread",
                    "Stop Level",
                    "Lot Size",
                    "Tick Value",
                    "Tick Size",
                    "Swap Long",
                    "Swap Short",
                    "Starting",
                    "Expiration",
                    "Trade Allowed",
                    "Min Lot",
                    "Lot Step",
                    "Max Lot",
                    "Swap Type",
                    "Profit Calc Mode",
                    "Margin Calc Mode",
                    "Margin Init",
                    "Margin Maintenance",
                    "Margin Hedged",
                    "Margin Required",
                    "Freeze Level",
                    "Markup"
                };

            var asMiValues = new[]
                {
                    mi.ModePoint.ToString("F" + mi.ModeDigits.ToString(CultureInfo.InvariantCulture)),
                    mi.ModeDigits.ToString(CultureInfo.InvariantCulture),
                    mi.ModeSpread.ToString(CultureInfo.InvariantCulture),
                    mi.ModeStopLevel.ToString(CultureInfo.InvariantCulture),
                    mi.ModeLotSize.ToString(CultureInfo.InvariantCulture),
                    mi.ModeTickValue.ToString(CultureInfo.InvariantCulture),
                    mi.ModeTickSize.ToString("F" + mi.ModeDigits.ToString(CultureInfo.InvariantCulture)),
                    mi.ModeSwapLong.ToString(CultureInfo.InvariantCulture),
                    mi.ModeSwapShort.ToString(CultureInfo.InvariantCulture),
                    mi.ModeStarting.ToString(CultureInfo.InvariantCulture),
                    mi.ModeExpiration.ToString(CultureInfo.InvariantCulture),
                    mi.ModeTradeAllowed.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMinLot.ToString(CultureInfo.InvariantCulture),
                    mi.ModeLotStep.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMaxLot.ToString(CultureInfo.InvariantCulture),
                    mi.ModeSwapType.ToString(CultureInfo.InvariantCulture),
                    mi.ModeProfitCalcMode.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMarginCalcMode.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMarginInit.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMarginMaintenance.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMarginHedged.ToString(CultureInfo.InvariantCulture),
                    mi.ModeMarginRequired.ToString(CultureInfo.InvariantCulture),
                    mi.ModeFreezeLevel.ToString(CultureInfo.InvariantCulture),
                    Math.Round((Data.LastClose - Data.Bid)/Data.InstrProperties.Point)
                        .ToString(CultureInfo.InvariantCulture)
                };

            var sb = new StringBuilder();
            for (int i = 0; i < asMiParams.Length; i++)
            {
                sb.AppendLine(string.Format("      {0,-20} {1}", asMiParams[i], asMiValues[i]));
            }
            SetBarDataText(sb.ToString());

            // Sets Market Info
            var asValue = new[]
                {
                    Data.Symbol,
                    Data.DataPeriodToString(Data.Period),
                    mi.ModeLotSize.ToString(CultureInfo.InvariantCulture),
                    mi.ModePoint.ToString("F" + mi.ModeDigits.ToString(CultureInfo.InvariantCulture)),
                    mi.ModeSpread.ToString(CultureInfo.InvariantCulture),
                    mi.ModeSwapLong.ToString(CultureInfo.InvariantCulture),
                    mi.ModeSwapShort.ToString(CultureInfo.InvariantCulture)
                };
            UpdateStatusPageMarketInfo(asValue);
        }

        protected override void BtnShowBarsClick(object sender, EventArgs e)
        {
            var sb = new StringBuilder(Data.Bars + 2);

            sb.AppendLine("                  " +
                          Data.Symbol + " " + Data.PeriodMtStr + " " +
                          Data.Time[Data.Bars - 1].ToString(CultureInfo.InvariantCulture));
            sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                                        "No", "Bar open time", "Open", "High", "Low", "Close", "Volume"));

            for (int i = 0; i < Data.Bars; i++)
            {
                sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                                            i + 1, Data.Time[i].ToString(Data.Df) + " " + Data.Time[i].ToString("HH:mm"),
                                            Data.Open[i], Data.High[i], Data.Low[i], Data.Close[i], Data.Volume[i]));
            }

            SetBarDataText(sb.ToString());
        }

        /// <summary>
        ///     Converts a string to a double number.
        /// </summary>
        private double StringToDouble(string input)
        {
            string decimalPoint = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(decimalPoint))
            {
                input = input.Replace(".", decimalPoint);
                input = input.Replace(",", decimalPoint);
            }

            return double.Parse(input);
        }

        #region Nested type: SetTradeStripDelegate

        private delegate void SetTradeStripDelegate(string connectText, bool isConnectedNow, bool isTradingNow);

        #endregion
    }
}