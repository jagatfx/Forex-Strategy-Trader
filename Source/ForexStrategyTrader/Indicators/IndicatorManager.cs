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
using ForexStrategyBuilder.Indicators.Store;

namespace ForexStrategyBuilder.Indicators
{
    public static class IndicatorManager
    {
        private static readonly Dictionary<string, Indicator> OriginalIndicators = new Dictionary<string, Indicator>();

        // Stores the custom indicators
        private static readonly SortableDictionary<string, Indicator> CustomIndicators =
            new SortableDictionary<string, Indicator>();

        // Stores all the indicators
        private static readonly SortableDictionary<string, Indicator> AllIndicators =
            new SortableDictionary<string, Indicator>();

        /// <summary>
        ///     Constructor.
        /// </summary>
        static IndicatorManager()
        {
            ClosingIndicatorsWithClosingFilters = new List<string>();
            CloseFilterIndicators = new List<string>();
            OpenFilterIndicators = new List<string>();
            ClosePointIndicators = new List<string>();
            OpenPointIndicators = new List<string>();
            AddOriginalIndicators();

            foreach (var record in OriginalIndicators)
                AllIndicators.Add(record.Key, record.Value);
        }

        /// <summary>
        ///     Gets the names of all the original indicators
        /// </summary>
        public static IEnumerable<string> OriginalIndicatorNames
        {
            get { return new List<string>(OriginalIndicators.Keys); }
        }

        /// <summary>
        ///     Gets the names of all custom indicators
        /// </summary>
        public static List<string> CustomIndicatorNames
        {
            get { return new List<string>(CustomIndicators.Keys); }
        }

        /// <summary>
        ///     Gets the names of all Opening Point indicators.
        /// </summary>
        public static List<string> OpenPointIndicators { get; private set; }

        /// <summary>
        ///     Gets the names of all Closing Point indicators.
        /// </summary>
        public static List<string> ClosePointIndicators { get; private set; }

        /// <summary>
        ///     Gets the names of all Opening Filter indicators.
        /// </summary>
        public static List<string> OpenFilterIndicators { get; private set; }

        /// <summary>
        ///     Gets the names of all Closing Filter indicators.
        /// </summary>
        public static List<string> CloseFilterIndicators { get; private set; }

        /// <summary>
        ///     Gets the names of all losing Point indicators that allow use of Closing Filter indicators.
        /// </summary>
        public static List<string> ClosingIndicatorsWithClosingFilters { get; private set; }

        /// <summary>
        ///     Gets the names of all indicators.
        /// </summary>
        public static List<string> AllIndicatorsNames
        {
            get { return new List<string>(AllIndicators.Keys); }
        }

        /// <summary>
        ///     Adds all indicators to the store.
        /// </summary>
        private static void AddOriginalIndicators()
        {
            OriginalIndicators.Add("Accelerator Oscillator", new AcceleratorOscillator());
            OriginalIndicators.Add("Account Percent Stop", new AccountPercentStop());
            OriginalIndicators.Add("Accumulation Distribution", new AccumulationDistribution());
            OriginalIndicators.Add("ADX", new ADX());
            OriginalIndicators.Add("Alligator", new Alligator());
            OriginalIndicators.Add("Aroon Histogram", new AroonHistogram());
            OriginalIndicators.Add("ATR MA Oscillator", new AtrMaOscillator());
            OriginalIndicators.Add("ATR Stop", new AtrStop());
            OriginalIndicators.Add("Average True Range", new AverageTrueRange());
            OriginalIndicators.Add("Awesome Oscillator", new AwesomeOscillator());
            OriginalIndicators.Add("Balance of Power", new BalanceOfPower());
            OriginalIndicators.Add("Bar Closing", new BarClosing());
            OriginalIndicators.Add("Bar Opening", new BarOpening());
            OriginalIndicators.Add("Bar Range", new BarRange());
            OriginalIndicators.Add("BBP MA Oscillator", new BbpMaOscillator());
            OriginalIndicators.Add("Bears Power", new BearsPower());
            OriginalIndicators.Add("Bollinger Bands", new BollingerBands());
            OriginalIndicators.Add("Bulls Bears Power", new BullsBearsPower());
            OriginalIndicators.Add("Bulls Power", new BullsPower());
            OriginalIndicators.Add("CCI MA Oscillator", new CciMAOscillator());
            OriginalIndicators.Add("Close and Reverse", new CloseAndReverse());
            OriginalIndicators.Add("Commodity Channel Index", new CommodityChannelIndex());
            OriginalIndicators.Add("Cumulative Sum", new CumulativeSum());
            OriginalIndicators.Add("Data Bars Filter", new DataBarsFilter());
            OriginalIndicators.Add("Date Filter", new DateFilter());
            OriginalIndicators.Add("Day Closing", new DayClosing());
            OriginalIndicators.Add("Day Closing 2", new DayClosing2());
            OriginalIndicators.Add("Day of Week", new DaysOfWeek());
            OriginalIndicators.Add("Day Opening", new DayOpening());
            OriginalIndicators.Add("DeMarker", new DeMarker());
            OriginalIndicators.Add("Detrended Oscillator", new DetrendedOscillator());
            OriginalIndicators.Add("Directional Indicators", new DirectionalIndicators());
            OriginalIndicators.Add("Donchian Channel", new DonchianChannel());
            OriginalIndicators.Add("Ease of Movement", new EaseOfMovement());
            OriginalIndicators.Add("Enter Once", new EnterOnce());
            OriginalIndicators.Add("Entry Hour", new EntryHour());
            OriginalIndicators.Add("Entry Time", new EntryTime());
            OriginalIndicators.Add("Envelopes", new Envelopes());
            OriginalIndicators.Add("Exit Hour", new ExitHour());
            OriginalIndicators.Add("Fisher Transform", new FisherTransform());
            OriginalIndicators.Add("Force Index", new ForceIndex());
            OriginalIndicators.Add("Fractal", new Fractal());
            OriginalIndicators.Add("Heiken Ashi", new HeikenAshi());
            OriginalIndicators.Add("Hourly High Low", new HourlyHighLow());
            OriginalIndicators.Add("Ichimoku Kinko Hyo", new IchimokuKinkoHyo());
            OriginalIndicators.Add("Inside Bar", new InsideBar());
            OriginalIndicators.Add("Keltner Channel", new KeltnerChannel());
            OriginalIndicators.Add("Long or Short", new LongOrShort());
            OriginalIndicators.Add("Lot Limiter", new LotLimiter());
            OriginalIndicators.Add("MA Oscillator", new MAOscillator());
            OriginalIndicators.Add("MACD Histogram", new MACDHistogram());
            OriginalIndicators.Add("MACD", new MACD());
            OriginalIndicators.Add("Market Facilitation Index", new MarketFacilitationIndex());
            OriginalIndicators.Add("Momentum MA Oscillator", new MomentumMAOscillator());
            OriginalIndicators.Add("Momentum", new Momentum());
            OriginalIndicators.Add("Money Flow Index", new MoneyFlowIndex());
            OriginalIndicators.Add("Money Flow", new MoneyFlow());
            OriginalIndicators.Add("Moving Average", new MovingAverage());
            OriginalIndicators.Add("Moving Averages Crossover", new MovingAveragesCrossover());
            OriginalIndicators.Add("N Bars Exit", new NBarsExit());
            OriginalIndicators.Add("Narrow Range", new NarrowRange());
            OriginalIndicators.Add("OBOS MA Oscillator", new ObosMaOscillator());
            OriginalIndicators.Add("On Balance Volume", new OnBalanceVolume());
            OriginalIndicators.Add("Oscillator of ATR", new OscillatorOfAtr());
            OriginalIndicators.Add("Oscillator of BBP", new OscillatorOfBbp());
            OriginalIndicators.Add("Oscillator of CCI", new OscillatorOfCci());
            OriginalIndicators.Add("Oscillator of MACD", new OscillatorOfMACD());
            OriginalIndicators.Add("Oscillator of Momentum", new OscillatorOfMomentum());
            OriginalIndicators.Add("Oscillator of OBOS", new OscillatorOfObos());
            OriginalIndicators.Add("Oscillator of ROC", new OscillatorOfRoc());
            OriginalIndicators.Add("Oscillator of RSI", new OscillatorOfRsi());
            OriginalIndicators.Add("Oscillator of Trix", new OscillatorOfTrix());
            OriginalIndicators.Add("Overbought Oversold Index", new OverboughtOversoldIndex());
            OriginalIndicators.Add("Parabolic SAR", new ParabolicSar());
            OriginalIndicators.Add("Percent Change", new PercentChange());
            OriginalIndicators.Add("Pivot Points", new PivotPoints());
            OriginalIndicators.Add("Previous Bar Closing", new PreviousBarClosing());
            OriginalIndicators.Add("Previous Bar Opening", new PreviousBarOpening());
            OriginalIndicators.Add("Previous High Low", new PreviousHighLow());
            OriginalIndicators.Add("Price Move", new PriceMove());
            OriginalIndicators.Add("Price Oscillator", new PriceOscillator());
            OriginalIndicators.Add("Random Filter", new RandomFilter());
            OriginalIndicators.Add("Rate of Change", new RateOfChange());
            OriginalIndicators.Add("Relative Vigor Index", new RelativeVigorIndex());
            OriginalIndicators.Add("ROC MA Oscillator", new RocMaOscillator());
            OriginalIndicators.Add("Ross Hook", new RossHook());
            OriginalIndicators.Add("Round Number", new RoundNumber());
            OriginalIndicators.Add("RSI MA Oscillator", new RsiMaOscillator());
            OriginalIndicators.Add("RSI", new RSI());
            OriginalIndicators.Add("Standard Deviation", new StandardDeviation());
            OriginalIndicators.Add("Starc Bands", new StarcBands());
            OriginalIndicators.Add("Steady Bands", new SteadyBands());
            OriginalIndicators.Add("Stochastics", new Stochastics());
            OriginalIndicators.Add("Stop Limit", new StopLimit());
            OriginalIndicators.Add("Stop Loss", new StopLoss());
            OriginalIndicators.Add("Take Profit", new TakeProfit());
            OriginalIndicators.Add("Top Bottom Price", new TopBottomPrice());
            OriginalIndicators.Add("Trailing Stop Limit", new TrailingStopLimit());
            OriginalIndicators.Add("Trailing Stop", new TrailingStop());
            OriginalIndicators.Add("Trix Index", new TrixIndex());
            OriginalIndicators.Add("Trix MA Oscillator", new TrixMAOscillator());
            OriginalIndicators.Add("Week Closing", new WeekClosing());
            OriginalIndicators.Add("Week Closing 2", new WeekClosing2());
            OriginalIndicators.Add("Williams' Percent Range", new WilliamsPercentRange());
        }

        /// <summary>
        ///     Gets the names of all indicators for a given slot type.
        /// </summary>
        public static List<string> GetIndicatorNames(SlotTypes slotType)
        {
            var list = new List<string>();

            foreach (var record in AllIndicators)
                if (record.Value.TestPossibleSlot(slotType))
                    list.Add(record.Value.IndicatorName);

            return list;
        }

        /// <summary>
        ///     Resets the custom indicators in the custom indicators list.
        /// </summary>
        public static void ResetCustomIndicators(IEnumerable<Indicator> indicatorList)
        {
            CustomIndicators.Clear();

            if (indicatorList == null)
                return;

            foreach (Indicator indicator in indicatorList)
                if (!CustomIndicators.ContainsKey(indicator.IndicatorName))
                    CustomIndicators.Add(indicator.IndicatorName, indicator);

            CustomIndicators.Sort();
        }

        /// <summary>
        ///     Clears the indicator list and adds to it the original indicators.
        /// </summary>
        public static void CombineAllIndicators()
        {
            AllIndicators.Clear();

            foreach (var record in OriginalIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);

            foreach (var record in CustomIndicators)
                if (!AllIndicators.ContainsKey(record.Key))
                    AllIndicators.Add(record.Key, record.Value);
                else if (record.Value.OverrideMainIndicator)
                    AllIndicators[record.Key] = record.Value;

            AllIndicators.Sort();

            OpenPointIndicators = GetIndicatorNames(SlotTypes.Open);
            ClosePointIndicators = GetIndicatorNames(SlotTypes.Close);
            OpenFilterIndicators = GetIndicatorNames(SlotTypes.OpenFilter);
            CloseFilterIndicators = GetIndicatorNames(SlotTypes.CloseFilter);

            foreach (string indicatorName in ClosePointIndicators)
            {
                Indicator indicator = ConstructIndicator(indicatorName);
                if (indicator.AllowClosingFilters)
                    ClosingIndicatorsWithClosingFilters.Add(indicatorName);
            }
        }

        public static Indicator ConstructIndicator(string indicatorName)
        {
            Indicator indicator = AllIndicators[indicatorName];
            var instance = (Indicator)Activator.CreateInstance(indicator.GetType());
            instance.CustomIndicator = indicator.CustomIndicator;
            instance.OverrideMainIndicator = indicator.OverrideMainIndicator;
            instance.LoaddedFromDll = indicator.LoaddedFromDll;
            return instance;
        }
    }
}