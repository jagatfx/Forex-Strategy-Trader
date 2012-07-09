// Action Data Feed
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;
using MT4Bridge;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions
    {
        private readonly object _lockerDataFeed = new object();
        private readonly object _lockerTickPing = new object();
        private int _accountReconnect;
        private DateTime _barOpenTimeForLastCloseEvent = DateTime.MinValue;
        private DateTime _barOpenTimeForLastCloseTick = DateTime.MinValue;
        private DateTime _barOpenTimeForLastOpenTick = DateTime.MinValue;
        private Bridge _bridge;
        private bool _isSetRootDataError;
        private bool _isTrading;
        private bool _nullPing = true;
        private DataPeriods _periodReconnect;
        private int _pingAttempt;
        private string _symbolReconnect;
        private DateTime _tickLocalTime = DateTime.MinValue;
        private DateTime _tickServerTime = DateTime.MinValue;
        private Timer _timerPing;
        private const double Epsilon = 0.000001;

        /// <summary>
        /// Initializes data feed.
        /// </summary>
        private void InitDataFeed()
        {
            _tickLocalTime = DateTime.MinValue;

            _bridge = new Bridge {WriteLog = Configs.BridgeWritesLog};
            _bridge.Start(Data.ConnectionID);
            _bridge.OnTick += Bridge_OnTick;

            _timerPing = new Timer {Interval = 1000};
            _timerPing.Tick += TimerPingTick;
            _timerPing.Start();
        }

        /// <summary>
        /// Reinitializes data feed.
        /// </summary>
        private void DeinitDataFeed()
        {
            if (_timerPing != null)
                _timerPing.Stop();

            StopTrade();

            if (_bridge == null) return;
            _bridge.OnTick -= Bridge_OnTick;
            _bridge.Stop();
        }

        /// <summary>
        /// Pings the server in order to check the connection.
        /// </summary>
        private void TimerPingTick(object sender, EventArgs e)
        {
            if (DateTime.Now < _tickLocalTime.AddSeconds(1))
                return; // The last tick was soon enough.

            lock (_lockerTickPing)
            {
                PingInfo ping = _bridge.GetPingInfo();

                if (ping == null && !_nullPing)
                {
                    // Wrong ping.
                    _pingAttempt++;
                    if ((_pingAttempt == 1 || _pingAttempt%10 == 0) && JournalShowSystemMessages)
                    {
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         Language.T("Unsuccessful ping") + " No " + _pingAttempt + ".");
                        AppendJournalMessage(jmsgsys);
                    }
                    if (_pingAttempt == 30)
                    {
                        var jmsgsys = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                                         Language.T("There is no connection with MetaTrader."));
                        AppendJournalMessage(jmsgsys);
                        if (Configs.PlaySounds)
                            Data.SoundError.Play();
                    }
                    if (_pingAttempt < 60)
                    {
                        SetConnIcon(_pingAttempt < 30 ? 3 : 4);
                        return;
                    }

                    Disconnect();
                }
                else if (ping != null)
                {
                    // Successful ping.
                    _nullPing = false;
                    bool bUpdateData = false;
                    if (!Data.IsConnected || IsChartChangeged(ping.Symbol, (DataPeriods) (int) ping.Period))
                    {
                        // Disconnected or chart change.
                        _pingAttempt = 0;

                        if (JournalShowSystemMessages)
                        {
                            var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                             ping.Symbol + " " + ping.Period.ToString() + " " +
                                                             Language.T("Successful ping."));
                            AppendJournalMessage(jmsgsys);
                        }
                        StopTrade();
                        if (!UpdateDataFeedInfo(ping.Time, ping.Symbol, (DataPeriods) (int) ping.Period))
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

                        TerminalInfo te = _bridge.GetTerminalInfo();
                        string connection = Language.T("Connected to a MetaTrader terminal.");
                        if (te != null)
                        {
                            connection = string.Format(
                                Language.T("Connected to") + " {0} " + Language.T("by") + " {1}", te.TerminalName,
                                te.TerminalCompany);
                            Data.ExpertVersion = te.ExpertVersion;
                            Data.LibraryVersion = te.LibraryVersion;
                            Data.TerminalName = te.TerminalName;
                        }

                        SetLblConnectionText(connection);
                        string market = string.Format("{0} {1}", ping.Symbol, ping.Period);
                        SetConnMarketText(market);
                        var jmsg = new JournalMessage(JournalIcons.OK, DateTime.Now, market + " " + connection);
                        AppendJournalMessage(jmsg);

                        // Check for reconnection.
                        if (IsRestartTrade())
                            StartTrade(); // Restart trade.
                    }
                    else if (_pingAttempt > 0 && JournalShowSystemMessages)
                    {
                        // After a wrong ping.
                        _pingAttempt = 0;

                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         ping.Symbol + " " + ping.Period.ToString() + " " +
                                                         Language.T("Successful ping."));
                        AppendJournalMessage(jmsgsys);
                    }

                    bool isNewPrice = Math.Abs(Data.Bid - ping.Bid) > Data.InstrProperties.Point/2;
                    DateTime dtPingServerTime = _tickServerTime.Add(DateTime.Now - _tickLocalTime);

                    string sBid = ping.Bid.ToString(Data.FF);
                    string sAsk = ping.Ask.ToString(Data.FF);
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
                    LogActivatedSLTP();

                    SetDataAndCalculate(ping.Symbol, ping.Period, dtPingServerTime, isNewPrice, bUpdateData);

                    SetEquityInfoText(string.Format("{0:F2} {1}", ping.AccountEquity, Data.AccountCurrency));
                    ShowCurrentPosition(isPosChanged);

                    if (isAccChanged)
                    {
                        var jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now,
                                                      string.Format(
                                                          Language.T("Account Balance") + " {0:F2}, " +
                                                          Language.T("Equity") + " {1:F2}, " + Language.T("Profit") +
                                                          ", {2:F2}, " + Language.T("Free Margin") + " {3:F2}",
                                                          ping.AccountBalance, ping.AccountEquity, ping.AccountProfit,
                                                          ping.AccountFreeMargin));
                        AppendJournalMessage(jmsg);
                    }

                    if (Data.IsBalanceDataChganged)
                        UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);

                    SetTickInfoText(string.Format("{0} {1} / {2}", ping.Time.ToString("HH:mm:ss"), sBid, sAsk));
                    SetConnIcon(1);

                    // Sends OrderModify on SL/TP errors
                    if (IsWrongStopsExecution())
                        ResendWrongStops();
                }
            }
        }

        /// <summary>
        /// Bridge OnTick 
        /// </summary>
        private void Bridge_OnTick(object source, TickEventArgs tea)
        {
            lock (_lockerTickPing)
            {
                if (_pingAttempt > 0 && JournalShowSystemMessages)
                {
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                     tea.Symbol + " " + tea.Period + " " +
                                                     Language.T("Tick received after an unsuccessful ping."));
                    AppendJournalMessage(jmsgsys);
                }
                _pingAttempt = 0;

                if (!Data.IsConnected)
                    return;

                _tickLocalTime = DateTime.Now;
                _tickServerTime = tea.Time;
                if (IsChartChangeged(tea.Symbol, (DataPeriods) (int) tea.Period))
                {
                    StopTrade();
                    Data.IsConnected = false;
                    SetFormText();

                    if (Configs.PlaySounds)
                        Data.SoundDisconnect.Play();

                    var jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now,
                                                  tea.Symbol + " " + tea.Period + " " +
                                                  Language.T("Tick received from a different chart!"));
                    AppendJournalMessage(jmsg);

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
                LogActivatedSLTP();

                const bool updateData = true;
                SetDataAndCalculate(tea.Symbol, tea.Period, tea.Time, bNewPrice, updateData);

                string bidText = tea.Bid.ToString(Data.FF);
                string askText = tea.Ask.ToString(Data.FF);
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
                    var jmsg = new JournalMessage(JournalIcons.Currency, DateTime.Now,
                                                  string.Format(
                                                      Language.T("Account Balance") + " {0:F2}, " + Language.T("Equity") +
                                                      " {1:F2}, " + Language.T("Profit") + ", {2:F2}, " +
                                                      Language.T("Free Margin") + " {3:F2}",
                                                      tea.AccountBalance, tea.AccountEquity, tea.AccountProfit,
                                                      tea.AccountFreeMargin));
                    AppendJournalMessage(jmsg);
                }

                if (Data.IsBalanceDataChganged)
                    UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);

                SetTickInfoText(string.Format("{0} {1} / {2}", tea.Time.ToString("HH:mm:ss"), bidText, askText));
                SetConnIcon(2);

                // Sends OrderModify on SL/TP errors
                if (IsWrongStopsExecution())
                    ResendWrongStops();
            }
        }

        /// <summary>
        /// Stops connection to MT
        /// </summary>
        private void Disconnect()
        {
            _nullPing = true;
            _pingAttempt = 0;
            if (Data.IsConnected && Configs.PlaySounds)
                Data.SoundDisconnect.Play();

            Data.IsConnected = false;
            StopTrade();

            var jmsg = new JournalMessage(JournalIcons.Blocked, DateTime.Now, Language.T("Not Connected"));
            AppendJournalMessage(jmsg);

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
        /// Check if the incoming data is from the same chart.
        /// </summary>
        private bool IsChartChangeged(string symbol, DataPeriods period)
        {
            if (!Data.IsConnected)
                return true;

            if (Data.Symbol != symbol || Data.Period != period)
                return true;

            return false;
        }

        /// <summary>
        /// Sets the instrument's properties after connecting;
        /// </summary>
        private bool UpdateDataFeedInfo(DateTime time, string symbol, DataPeriods period)
        {
            lock (_lockerDataFeed)
            {
                Data.ResetBidAskClose();
                Data.ResetAccountStats();
                Data.ResetPositionStats();
                Data.ResetBarStats();
                Data.ResetTicks();

                // Reads market info from the chart
                MT4Bridge.MarketInfo marketInfo = _bridge.GetMarketInfoAll(symbol);
                if (marketInfo == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot update market info."));
                        AppendJournalMessage(jmsgsys);
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
                                     marketInfo.ModePoint.ToString("F" + marketInfo.ModeDigits.ToString(CultureInfo.InvariantCulture)),
                                     marketInfo.ModeSpread.ToString(CultureInfo.InvariantCulture),
                                     marketInfo.ModeSwapLong.ToString(CultureInfo.InvariantCulture),
                                     marketInfo.ModeSwapShort.ToString(CultureInfo.InvariantCulture)
                                 };
                UpdateStatusPageMarketInfo(values);

                Bars bars = _bridge.GetBars(symbol, (PeriodType) (int) period);
                if (bars == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                         symbol + " " + (PeriodType) (int) period + " " +
                                                         Language.T("Cannot receive bars!"));
                        AppendJournalMessage(jmsgsys);
                    }
                    return false;
                }
                if (bars.Count < MaxBarsCount((int) period))
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                      symbol + " " + (PeriodType) (int) period + " " +
                                                      Language.T("Cannot receive enough bars!"));
                        AppendJournalMessage(jmsg);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                     symbol + " " + (PeriodType) (int) period + " " +
                                                     Language.T("Market data updated, bars downloaded."));
                    AppendJournalMessage(jmsgsys);
                }

                // Account Information.
                AccountInfo account = _bridge.GetAccountInfo();
                if (account == null)
                {
                    if (JournalShowSystemMessages)
                    {
                        Data.SoundError.Play();
                        var jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                      symbol + " " + (PeriodType) (int) period + " " +
                                                      Language.T("Cannot receive account information!"));
                        AppendJournalMessage(jmsg);
                    }
                    return false;
                }
                if (JournalShowSystemMessages)
                {
                    var jmsgsys = new JournalMessage(JournalIcons.System, DateTime.Now,
                                                     symbol + " " + (PeriodType) (int) period + " " +
                                                     Language.T("Account information received."));
                    AppendJournalMessage(jmsgsys);
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
        /// Copies data to Data and calculates.
        /// </summary>
        private void SetDataAndCalculate(string symbol, PeriodType period, DateTime time, bool isPriceChange, bool isUpdateData)
        {
            lock (_lockerDataFeed)
            {
                bool isUpdateChart = isUpdateData;

                Bars bars = _bridge.GetBars(symbol, period);

                if (bars == null && JournalShowSystemMessages)
                {
                    _isSetRootDataError = true;
                    Data.SoundError.Play();
                    var jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                  symbol + " " + period + " " +
                                                  Language.T("Cannot receive bars!"));
                    AppendJournalMessage(jmsg);
                    return;
                }
                if (bars != null && (bars.Count < MaxBarsCount((int) period) && JournalShowSystemMessages))
                {
                    _isSetRootDataError = true;
                    Data.SoundError.Play();
                    var jmsg = new JournalMessage(JournalIcons.Error, DateTime.Now,
                                                  symbol + " " + period + " " +
                                                  Language.T("Cannot receive enough bars!"));
                    AppendJournalMessage(jmsg);
                    return;
                }
                if (_isSetRootDataError && JournalShowSystemMessages)
                {
                    _isSetRootDataError = false;
                    var jmsg = new JournalMessage(JournalIcons.Information, DateTime.Now,
                                                  symbol + " " + period + " " +
                                                  Language.T("Enough bars received!"));
                    AppendJournalMessage(jmsg);
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

                if (_isTrading)
                {
                    TickType tickType = GetTickType((DataPeriods) (int) period, Data.Time[Data.Bars - 1], time,
                                                    Data.Volume[Data.Bars - 1]);

                    if (tickType == TickType.Close || isPriceChange || isBarChanged)
                    {
                        if (JournalShowSystemMessages && tickType != TickType.Regular)
                        {
                            JournalIcons icon = JournalIcons.Warning;
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
                            var jmsg = new JournalMessage(icon, DateTime.Now,
                                                          symbol + " " + Data.PeriodMTStr + " " +
                                                          time.ToString("HH:mm:ss") + " " + text);
                            AppendJournalMessage(jmsg);
                        }

                        if (isBarChanged && tickType == TickType.Regular)
                        {
                            if (JournalShowSystemMessages)
                            {
                                var jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, symbol + " " +
                                                                                                  Data.PeriodMTStr + " " +
                                                                                                  time.ToString("HH:mm:ss") +
                                                                                                  " A Bar Changed event!");
                                AppendJournalMessage(jmsg);
                            }

                            tickType = TickType.Open;
                        }

                        if (tickType == TickType.Open && _barOpenTimeForLastCloseEvent == Data.Time[Data.Bars - 3])
                        {
                            if (JournalShowSystemMessages)
                            {
                                var jmsg = new JournalMessage(JournalIcons.Warning, DateTime.Now, symbol + " " +
                                                                                                  Data.PeriodMTStr + " " +
                                                                                                  time.ToString("HH:mm:ss") +
                                                                                                  " A secondary Bar Close event!");
                                AppendJournalMessage(jmsg);
                            }
                            tickType = TickType.OpenClose;
                        }

                        CalculateTrade(tickType);
                        isUpdateChart = true;

                        if (tickType == TickType.Close || tickType == TickType.OpenClose)
                            _barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];
                    }
                }

                if (isUpdateChart)
                    UpdateChart();
            }
        }

        /// <summary>
        /// Shows if the bar was changed.
        /// </summary>
        private bool IsBarChanged(DateTime open)
        {
            bool barChanged = _barOpenTimeForLastOpenTick != open;
            _barOpenTimeForLastOpenTick = open;

            return barChanged;
        }

        /// <summary>
        /// Gets the tick type depending on its time.
        /// </summary>
        private TickType GetTickType(DataPeriods period, DateTime open, DateTime time, int volume)
        {
            TickType tick = TickType.Regular;
            bool isOpen = volume == 1 && _barOpenTimeForLastOpenTick != open;
            bool isClose = (open.AddMinutes((int) period) - time) < TimeSpan.FromSeconds(Configs.BarCloseAdvance);

            if (isOpen)
            {
                _barOpenTimeForLastCloseTick = DateTime.MinValue;
                tick = TickType.Open;
            }

            if (isClose)
            {
                if (_barOpenTimeForLastCloseTick == open)
                {
                    tick = TickType.AfterClose;
                }
                else
                {
                    _barOpenTimeForLastCloseTick = open;
                    tick = isOpen ? TickType.OpenClose : TickType.Close;
                }
            }

            return tick;
        }

        /// <summary>
        /// Returns the count of chart's bars.
        /// </summary>
        private int MaxBarsCount(int period)
        {
            return Math.Max(2*1440/period + 10, Configs.MinChartBars);
        }

        /// <summary>
        /// Manipulates additional params coming from MetaTrader. 
        /// </summary>
        private void ParseAndSetParametrs(string parameters)
        {
            if(string.IsNullOrEmpty(parameters)) return;

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
                        Data.ActivatedStopLoss = double.Parse(rowValue);
                        break;
                    case "aTP":
                        Data.ActivatedTakeProfit = double.Parse(rowValue);
                        break;
                    case "al":
                        Data.ClosedSLTPLots = double.Parse(rowValue);
                        break;
                }
            }
        }

        private double _activationReportedAt;

        private void LogActivatedSLTP()
        {
            if (Data.ActivatedStopLoss > Epsilon && Math.Abs(Data.ActivatedStopLoss - _activationReportedAt) > Epsilon)
            {
                // Activated Stop Loss
                Data.AddBarStats(OperationType.Close, Data.ClosedSLTPLots, Data.ActivatedStopLoss);
                string message = "Activated Stop Loss at " + Data.ActivatedStopLoss.ToString("F5") + ", Closed Lots " + Data.ClosedSLTPLots.ToString("F2");
                var msg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
                AppendJournalMessage(msg);
                _activationReportedAt = Data.ActivatedStopLoss;
            }
            else if (Data.ActivatedTakeProfit > Epsilon && Math.Abs(Data.ActivatedTakeProfit - _activationReportedAt) > Epsilon)
            {
                // Activated Take Profit
                Data.AddBarStats(OperationType.Close, Data.ClosedSLTPLots, Data.ActivatedTakeProfit);
                string message = "Activated Take Profit at " + Data.ActivatedTakeProfit.ToString("F5") + ", Closed Lots " + Data.ClosedSLTPLots.ToString("F2");
                var msg = new JournalMessage(JournalIcons.Information, DateTime.Now, message);
                AppendJournalMessage(msg);
                _activationReportedAt = Data.ActivatedTakeProfit;
            }
            else
            {
                _activationReportedAt = 0;
            }
        }

        /// <summary>
        /// Starts the trade.
        /// </summary>
        private void StartTrade()
        {
            _isTrading = true;

            // Resets trade global variables.
            InitTrade();

            Data.SetStartTradingTime();
            var msg = new JournalMessage(JournalIcons.StartTrading, DateTime.Now, Language.T("Automatic trade started."));
            AppendJournalMessage(msg);

            _symbolReconnect = Data.Symbol;
            _periodReconnect = Data.Period;
            _accountReconnect = Data.AccountNumber;
            _barOpenTimeForLastCloseEvent = Data.Time[Data.Bars - 1];

            SetTradeStrip();
        }

        /// <summary>
        /// Stops the trade.
        /// </summary>
        private void StopTrade()
        {
            if (!_isTrading)
                return;

            _isTrading = false;

            DeinitTrade();

            Data.SetStopTradingTime();

            var msg = new JournalMessage(JournalIcons.StopTrading, DateTime.Now, Language.T("Automatic trade stopped."));
            AppendJournalMessage(msg);
            SetTradeStrip();
        }

        private bool IsRestartTrade()
        {
            // Restart trade if we reopen expert on the previous chart.
            if (_symbolReconnect == Data.Symbol &&
                _periodReconnect == Data.Period &&
                _accountReconnect == Data.AccountNumber) return true;

            // Start trade once after starting FSB from the autostart script.
            if (Data.StartAutotradeWhenConnected)
            {
                Data.StartAutotradeWhenConnected = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Shows current position on the status bar.
        /// </summary>
        private void ShowCurrentPosition(bool showInJournal)
        {
            if (!Data.IsConnected)
            {
                SetPositionInfoText(null, String.Empty);
                return;
            }

            string format = Data.FF;
            string text = Language.T("Square");
            JournalIcons icon = JournalIcons.PosSquare;
            Image img = Resources.pos_square;
            if (Data.PositionTicket > 0)
            {
                img = (Data.PositionType == 0 ? Resources.pos_buy : Resources.pos_sell);
                icon = (Data.PositionType == 0 ? JournalIcons.PosBuy : JournalIcons.PosSell);
                text = string.Format((Data.PositionType == 0 ? Language.T("Long") : Language.T("Short")) + " {0} " +
                                     LotOrLots(Data.PositionLots) + " " + Language.T("at") + " {1}, " +
                                     Language.T("Stop Loss") + " {2}, " + Language.T("Take Profit") + " {3}, " +
                                     Language.T("Profit") + " {4} " + Data.AccountCurrency,
                                     Data.PositionLots,
                                     Data.PositionOpenPrice.ToString(format),
                                     Data.PositionStopLoss.ToString(format), Data.PositionTakeProfit.ToString(format),
                                     Data.PositionProfit.ToString("F2"));
            }

            SetPositionInfoText(img, text);

            if (showInJournal)
            {
                var jmsg = new JournalMessage(icon, DateTime.Now,
                                              string.Format(Data.Symbol + " " + Data.PeriodMTStr + " " + text));
                AppendJournalMessage(jmsg);
            }
        }

        /// <summary>
        /// Sets the trade button's icon and text.
        /// </summary>
        private void SetTradeStrip()
        {
            string connectText = Data.IsConnected
                                     ? Language.T("Connected to") + " " + Data.Symbol + " " + Data.PeriodMTStr
                                     : Language.T("Not Connected");

            SetTradeStripThreadSafely(connectText, Data.IsConnected, _isTrading);
        }

        private void SetTradeStripThreadSafely(string connectText, bool isConnectedNow, bool isTradingNow)
        {
            if (tsTradeControl.InvokeRequired)
            {
                tsTradeControl.BeginInvoke(new SetTradeStripDelegate(SetTradeStripThreadSafely),
                                           new object[] {connectText, isConnectedNow, isTradingNow});
            }
            else
            {
                // Sets label Connection
                tslblConnection.Text = connectText;

                // Sets button Automatic Execution
                if (isTradingNow)
                {
                    tsbtnTrading.Image = Resources.stop;
                    tsbtnTrading.Text = Language.T("Stop Automatic Execution");
                }
                else
                {
                    tsbtnTrading.Image = Resources.play;
                    tsbtnTrading.Text = Language.T("Start Automatic Execution");
                }
                tsbtnTrading.Enabled = isConnectedNow;
                tsbtnConnectionHelp.Visible = !isConnectedNow;
            }
        }

        /// <summary>
        /// Menu Multiple Instances Changed
        /// </summary>
        protected override void MenuMultipleInstances_OnClick(object sender, EventArgs e)
        {
            Configs.MultipleInstances = ((ToolStripMenuItem) sender).Checked;
            if (Configs.MultipleInstances)
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();

                tsTradeControl.SuspendLayout();
                tsbtnChangeID.Visible = false;
                tslblConnection.Visible = false;
                tsbtnTrading.Visible = false;

                tslblConnectionID.Visible = true;
                tstbxConnectionID.Visible = true;
                tsbtnConnectionGo.Visible = true;
                tsTradeControl.ResumeLayout();
            }
            else
            {
                Configs.SaveConfigs();
                Disconnect();
                DeinitDataFeed();
                Data.ConnectionID = 0;

                tsTradeControl.SuspendLayout();
                tsbtnChangeID.Visible = false;
                tslblConnection.Visible = true;
                tsbtnTrading.Visible = true;

                tslblConnectionID.Visible = false;
                tstbxConnectionID.Visible = false;
                tsbtnConnectionGo.Visible = false;
                tsTradeControl.ResumeLayout();

                InitDataFeed();
            }

            _symbolReconnect = "";
        }

        /// <summary>
        /// ID changed.
        /// </summary>
        protected override void TstbxConnectionID_KeyPress(object sender, KeyPressEventArgs e)
        {
            tsbtnConnectionGo.Enabled = true;
            if (e.KeyChar == (char) Keys.Return)
                ConnectionGo();
        }

        /// <summary>
        /// Sets Connection ID
        /// </summary>
        private void ConnectionGo()
        {
            try
            {
                int id = int.Parse(tstbxConnectionID.Text);
                if (id >= 0)
                {
                    Data.ConnectionID = id;
                    Disconnect();

                    tsTradeControl.SuspendLayout();
                    tsbtnChangeID.Text = string.Format("ID = {0}", tstbxConnectionID.Text);
                    tsbtnChangeID.Visible = true;
                    tslblConnection.Visible = true;
                    tsbtnTrading.Visible = true;
                    tslblConnectionID.Visible = false;
                    tstbxConnectionID.Visible = false;
                    tsbtnConnectionGo.Visible = false;
                    tsTradeControl.ResumeLayout();

                    InitDataFeed();
                }
                else
                {
                    tstbxConnectionID.Text = "";
                }
            }
            catch
            {
                tsbtnConnectionGo.Enabled = false;
                tstbxConnectionID.Text = "";
            }
        }

        /// <summary>
        /// Button Change ID clicked.
        /// </summary>
        protected override void TsbtChangeID_Click(object sender, EventArgs e)
        {
            Disconnect();
            DeinitDataFeed();

            tsTradeControl.SuspendLayout();
            tsbtnChangeID.Visible = false;
            tslblConnection.Visible = false;
            tsbtnTrading.Visible = false;

            tslblConnectionID.Visible = true;
            tstbxConnectionID.Visible = true;
            tsbtnConnectionGo.Visible = true;
            tsTradeControl.ResumeLayout();
        }

        /// <summary>
        /// Button Connection Go clicked.
        /// </summary>
        protected override void TsbtConnectionGo_Click(object sender, EventArgs e)
        {
            ConnectionGo();
        }

        protected override void TsbtTrading_Click(object sender, EventArgs e)
        {
            if (_isTrading)
            {
                StopTrade();

                _symbolReconnect = "";
                _periodReconnect = DataPeriods.week;
                _accountReconnect = 0;
            }
            else
            {
                StartTrade();
            }
        }

        protected override void BtnShowAccountInfo_Click(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            AccountInfo ai = _bridge.GetAccountInfo();
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

        protected override void BtnShowMarketInfo_Click(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                SetBarDataText("   " + Language.T("Not Connected"));
                return;
            }

            MT4Bridge.MarketInfo mi = _bridge.GetMarketInfoAll(Data.Symbol);
            if (mi == null)
            {
                SetBarDataText("   " + Language.T("Cannot update market info."));
                return;
            }

            var asMIParams = new[]
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

            var asMIValues = new[]
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
                                     Math.Round((Data.LastClose - Data.Bid)/Data.InstrProperties.Point).ToString(CultureInfo.InvariantCulture)
                                 };

            var sb = new StringBuilder();
            for (int i = 0; i < asMIParams.Length; i++)
            {
                sb.AppendLine(string.Format("      {0,-20} {1}", asMIParams[i], asMIValues[i]));
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

        protected override void BtnShowBars_Click(object sender, EventArgs e)
        {
            var sb = new StringBuilder(Data.Bars + 2);

            sb.AppendLine("                  " +
                          Data.Symbol + " " + Data.PeriodMTStr + " " +
                          Data.Time[Data.Bars - 1].ToString(CultureInfo.InvariantCulture));
            sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                                        "No", "Bar open time", "Open", "High", "Low", "Close", "Volume"));

            for (int i = 0; i < Data.Bars; i++)
            {
                sb.AppendLine(string.Format("  {0,-5} {1,-16} {2,-8} {3,-8} {4,-8} {5,-8} {6}",
                                            i + 1, Data.Time[i].ToString(Data.DF) + " " + Data.Time[i].ToString("HH:mm"),
                                            Data.Open[i], Data.High[i], Data.Low[i], Data.Close[i], Data.Volume[i]));
            }

            SetBarDataText(sb.ToString());
        }

        #region Nested type: SetTradeStripDelegate

        private delegate void SetTradeStripDelegate(string connectText, bool isConnectedNow, bool isTradingNow);

        #endregion
    }
}