// Data class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Media;
using System.Net;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;
using ForexStrategyBuilder.Properties;
using MT4Bridge;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///  Base class containing the data.
    /// </summary>
    public static partial class Data
    {
        private static string[] asStrategyIndicators;

        /// <summary>
        /// The default constructor.
        /// </summary>
        static Data()
        {
            IndicatorsForBacktestOnly = new[]
                                            {
                                                "Random Filter",
                                                "Date Filter",
                                                "Data Bars Filter",
                                                "Lot Limiter"
                                            };
            PositionOpenTime = DateTime.MinValue;
            PositionType = -1;
            BalanceData = new BalanceChartUnit[BalanceLenght];
            LibraryVersion = "unknown";
            ExpertVersion = "unknown";
            TerminalName = "MetaTrader";
            PointChar = '.';
            DFS = "dd.MM";
            DF = "dd.MM.yy";
            AutoUsePrvBarValue = true;
            SourceFolder = @"Indicators\";
            LibraryDir = @"Libraries\";
            UserFilesDir = @"User Files\";
            LoadedSavedStrategy = "";
            FirstBar = 40;
            StrategyName = "New.xml";
            StrategyDir = @"Strategies\";
            DefaultStrategyDir = @"Strategies\";
            ColorDir = @"Colors\";
            LanguageDir = @"Languages\";
            SystemDir = @"System\";
            ProgramDir = @"";
            IsProgramBeta = false;
            IsStrategyChanged = false;
            Debug = false;
            IsData = false;
            IsConnected = false;
            IsAutoStart = false;
            StartAutotradeWhenConnected = false;
            ConnectionId = 0;
            PositionComment = "";
            PositionProfit = 0;
            PositionTakeProfit = 0;
            PositionStopLoss = 0;
            PositionOpenPrice = 0;
            PositionLots = 0;
            PositionTicket = 0;
            WrongStopsRetry = 0;
            WrongTakeProf = 0;
            WrongStopLoss = 0;
            SecondsLiveTrading = 0;
            SavedStrategies = 0;
            SecondsDemoTrading = 0;
            LiveTradeStartTime = DateTime.Now;
            DemoTradeStartTime = DateTime.Now;
            ProgramName = "Forex Strategy Trader";
            ProgramVersion = Application.ProductVersion;
            string[] asVersion = ProgramVersion.Split('.');
            ProgramId = 1000000*int.Parse(asVersion[0]) + 10000*int.Parse(asVersion[1]) +
                        100*int.Parse(asVersion[2]) + int.Parse(asVersion[3]);
            Strategy.GenerateNew();
            StackStrategy = new Stack<Strategy>();
        }

        /// <summary>
        /// Gets the program name.
        /// </summary>
        public static string ProgramName { get; private set; }

        /// <summary>
        /// Gets the program version.
        /// </summary>
        public static string ProgramVersion { get; private set; }

        /// <summary>
        /// Gets the program Beta state.
        /// </summary>
        public static bool IsProgramBeta { get; private set; }

        /// <summary>
        /// Gets the program ID
        /// </summary>
        public static int ProgramId { get; private set; }

        /// <summary>
        /// Gets the program current working directory.
        /// </summary>
        public static string ProgramDir { get; private set; }

        /// <summary>
        ///     Gets the Users Files directory.
        /// </summary>
        public static string UserFilesDir { get; private set; }

        /// <summary>
        /// Gets the path to System Dir.
        /// </summary>
        public static string SystemDir { get; private set; }

        /// <summary>
        /// Gets the path to LanguageDir Dir.
        /// </summary>
        public static string LanguageDir { get; private set; }

        /// <summary>
        /// Gets the path to Color Scheme Dir.
        /// </summary>
        public static string ColorDir { get; private set; }

        /// <summary>
        /// Gets the path to Default Strategy Dir.
        /// </summary>
        public static string DefaultStrategyDir { get; private set; }

        /// <summary>
        ///     Gets the path to Library Dir.
        /// </summary>
        public static string LibraryDir { get; private set; }

        /// <summary>
        /// Gets or sets the path to dir Strategy.
        /// </summary>
        public static string StrategyDir { get; set; }

        /// <summary>
        /// Gets or sets the strategy name with extension.
        /// </summary>
        public static string StrategyName { get; set; }

        /// <summary>
        /// Gets the current strategy full path. 
        /// </summary>
        public static string StrategyPath
        {
            get { return Path.Combine(StrategyDir, StrategyName); }
        }

        public static bool IsStrategyChanged { get; set; }
        public static int FirstBar { get; set; }

        /// <summary>
        /// Gets or sets the custom indicators folder
        /// </summary>
        public static string SourceFolder { get; private set; }

        /// <summary>
        /// Gets or sets the strategy name for Configs.LastStrategy
        /// </summary>
        public static string LoadedSavedStrategy { get; set; }

        /// <summary>
        /// Gets the application's icon.
        /// </summary>
        public static Icon Icon
        {
            get { return Resources.Icon; }
        }

        /// <summary>
        /// The current strategy.
        /// </summary>
        public static Strategy Strategy { get; set; }

        /// <summary>
        /// The current strategy undo
        /// </summary>
        public static Stack<Strategy> StackStrategy { get; private set; }

        /// <summary>
        /// Debug mode
        /// </summary>
        public static bool Debug { get; set; }

        /// <summary>
        /// Sets or gets value of the AutoUsePrvBarValue
        /// </summary>
        public static bool AutoUsePrvBarValue { get; set; }

        /// <summary>
        /// Gets the number format.
        /// </summary>
        public static string FF
        {
            get { return "F" + InstrProperties.Digits.ToString(CultureInfo.InvariantCulture); }
        }

        /// <summary>
        /// Gets the date format.
        /// </summary>
        public static string DF { get; private set; }

        /// <summary>
        /// Gets the short date format.
        /// </summary>
        public static string DFS { get; private set; }

        /// <summary>
        /// Gets the point character
        /// </summary>
        public static char PointChar { get; private set; }

        /// <summary>
        /// Relative font height
        /// </summary>
        public static float VerticalDLU { get; set; }

        /// <summary>
        /// Relative font width
        /// </summary>
        public static float HorizontalDLU { get; set; }

        /// <summary>
        /// Gets connect sound
        /// </summary>
        public static SoundPlayer SoundConnect { get; private set; }

        /// <summary>
        /// Gets disconnect sound
        /// </summary>
        public static SoundPlayer SoundDisconnect { get; private set; }

        /// <summary>
        /// Gets error sound
        /// </summary>
        public static SoundPlayer SoundError { get; private set; }

        /// <summary>
        /// Gets order sent sound
        /// </summary>
        public static SoundPlayer SoundOrderSent { get; private set; }

        /// <summary>
        /// Gets position changed sound
        /// </summary>
        private static SoundPlayer SoundPositionChanged { get; set; }

        /// <summary>
        /// Gets indicators that are for back testing only.
        /// </summary>
        public static string[] IndicatorsForBacktestOnly { get; private set; }

        public static IDataSet DataSet { get; set; }


        public static bool IsData { get; set; }

        public static string PeriodStr
        {
            get { return DataPeriodToString(Period); }
        }

        public static string PeriodMTStr
        {
            get { return ((PeriodType) (int) Period).ToString(); }
        }

        public static bool IsAutoStart { get; set; }
        public static bool StartAutotradeWhenConnected { get; set; }

        public static Logger Logger { get; private set; }
        /// <summary>
        /// Initial settings.
        /// </summary>
        public static void Start()
        {
            // Sets the date format.
            if (DateTimeFormatInfo.CurrentInfo != null)
            {
                DF = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                if (DF == "dd/MM yyyy") DF = "dd/MM/yyyy"; // Fixes the Uzbek (Latin) issue
                DF = DF.Replace(" ", ""); // Fixes the Sloven issue
                char[] acDS = DateTimeFormatInfo.CurrentInfo.DateSeparator.ToCharArray();
                string[] asSS = DF.Split(acDS, 3);
                asSS[0] = asSS[0].Substring(0, 1) + asSS[0].Substring(0, 1);
                asSS[1] = asSS[1].Substring(0, 1) + asSS[1].Substring(0, 1);
                asSS[2] = asSS[2].Substring(0, 1) + asSS[2].Substring(0, 1);
                DF = asSS[0] + acDS[0].ToString(CultureInfo.InvariantCulture) + asSS[1] +
                             acDS[0].ToString(CultureInfo.InvariantCulture) + asSS[2];

                if (asSS[0].ToUpper() == "YY")
                    DFS = asSS[1] + acDS[0].ToString(CultureInfo.InvariantCulture) + asSS[2];
                else if (asSS[1].ToUpper() == "YY")
                    DFS = asSS[0] + acDS[0].ToString(CultureInfo.InvariantCulture) + asSS[2];
                else
                    DFS = asSS[0] + acDS[0].ToString(CultureInfo.InvariantCulture) + asSS[1];
            }

            // Point character
            CultureInfo culInf = CultureInfo.CurrentCulture;
            PointChar = culInf.NumberFormat.NumberDecimalSeparator.ToCharArray()[0];

            // Set the working directories
            ProgramDir = Directory.GetCurrentDirectory();
            UserFilesDir = Path.Combine(ProgramDir, UserFilesDir);

            StrategyDir = Path.Combine(UserFilesDir, DefaultStrategyDir);
            SourceFolder = Path.Combine(UserFilesDir, SourceFolder);
            SystemDir = Path.Combine(UserFilesDir, SystemDir);
            LibraryDir = Path.Combine(UserFilesDir, LibraryDir);
            LanguageDir = Path.Combine(SystemDir, LanguageDir);
            ColorDir = Path.Combine(SystemDir, ColorDir);

            try
            {
                SoundConnect = new SoundPlayer(Path.Combine(SystemDir, @"Sounds\connect.wav"));
                SoundDisconnect = new SoundPlayer(Path.Combine(SystemDir, @"Sounds\disconnect.wav"));
                SoundError = new SoundPlayer(Path.Combine(SystemDir, @"Sounds\error.wav"));
                SoundOrderSent = new SoundPlayer(Path.Combine(SystemDir, @"Sounds\order_sent.wav"));
                SoundPositionChanged = new SoundPlayer(Path.Combine(SystemDir, @"Sounds\position_changed.wav"));
            }
            catch
            {
                SoundConnect = new SoundPlayer(Resources.sound_connect);
                SoundDisconnect = new SoundPlayer(Resources.sound_disconnect);
                SoundError = new SoundPlayer(Resources.sound_error);
                SoundOrderSent = new SoundPlayer(Resources.sound_order_sent);
                SoundPositionChanged = new SoundPlayer(Resources.sound_position_changed);
            }

            Logger = new Logger();
        }

        // The names of the strategy indicators

        /// <summary>
        /// Sets the indicator names
        /// </summary>
        public static void SetStrategyIndicators()
        {
            asStrategyIndicators = new string[Strategy.Slots];
            for (int i = 0; i < Strategy.Slots; i++)
                asStrategyIndicators[i] = Strategy.Slot[i].IndicatorName;
        }

        /// <summary>
        /// It tells if the strategy description is relevant.
        /// </summary>
        public static bool IsStrDescriptionRelevant()
        {
            bool strategyIndicatorsChanged = Strategy.Slots != asStrategyIndicators.Length;

            if (strategyIndicatorsChanged == false)
            {
                for (int i = 0; i < Strategy.Slots; i++)
                    if (asStrategyIndicators[i] != Strategy.Slot[i].IndicatorName)
                        strategyIndicatorsChanged = true;
            }

            return !strategyIndicatorsChanged;
        }

        /// <summary>
        /// Sets the time when trading starts.
        /// </summary>
        public static void SetStartTradingTime()
        {
            if (IsDemoAccount)
                DemoTradeStartTime = DateTime.Now;
            else
                LiveTradeStartTime = DateTime.Now;
        }

        /// <summary>
        /// Sets the total trading time stats.
        /// </summary>
        public static void SetStopTradingTime()
        {
            if (IsDemoAccount && DemoTradeStartTime > DateTime.MinValue)
                SecondsDemoTrading += (int) (DateTime.Now - DemoTradeStartTime).TotalSeconds;
            else if (LiveTradeStartTime > DateTime.MinValue)
                SecondsLiveTrading += (int) (DateTime.Now - LiveTradeStartTime).TotalSeconds;

            DemoTradeStartTime = DateTime.MinValue;
            LiveTradeStartTime = DateTime.MinValue;
        }

        /// <summary>
        /// Collects usage statistics and sends them if it's allowed.
        /// </summary>
        public static void SendStats()
        {
            const string fileUrl = "http://forexsb.com/ustats/set-fst.php";

            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (nic.OperationalStatus == OperationalStatus.Up)
                {
                    mac = nic.GetPhysicalAddress().ToString();
                    break;
                }
            }

            string parameters = String.Empty;

            if (Configs.SendUsageStats)
            {
                parameters =
                    string.Format("?mac={0}&reg={1}&time={2}&dtt={3}&ltt={4}&str={5}",
                                  mac, RegionInfo.CurrentRegion.EnglishName,
                                  (int) (DateTime.Now - FstStartTime).TotalSeconds,
                                  SecondsDemoTrading, SecondsLiveTrading, SavedStrategies);
            }

            try
            {
                var webClient = new WebClient();
                Stream data = webClient.OpenRead(fileUrl + parameters);
                if (data != null) data.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Converts a data period from DataPeriods type to string.
        /// </summary>
        public static string DataPeriodToString(DataPeriod dataPeriod)
        {
            switch (dataPeriod)
            {
                case DataPeriod.M1:
                    return "1 " + Language.T("Minute");
                case DataPeriod.M5:
                    return "5 " + Language.T("Minutes");
                case DataPeriod.M15:
                    return "15 " + Language.T("Minutes");
                case DataPeriod.M30:
                    return "30 " + Language.T("Minutes");
                case DataPeriod.H1:
                    return "1 " + Language.T("Hour");
                case DataPeriod.H4:
                    return "4 " + Language.T("Hours");
                case DataPeriod.D1:
                    return "1 " + Language.T("Day");
                case DataPeriod.W1:
                    return "1 " + Language.T("Week");
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Color change
        /// </summary>
        /// <param name="colorBase">The base color</param>
        /// <param name="iDepth">Color change</param>
        /// <returns>The changed color</returns>
        public static Color ColorChanage(Color colorBase, int iDepth)
        {
            if (!Configs.GradientView)
                return colorBase;

            int r = Math.Max(Math.Min(colorBase.R + iDepth, 255), 0);
            int g = Math.Max(Math.Min(colorBase.G + iDepth, 255), 0);
            int b = Math.Max(Math.Min(colorBase.B + iDepth, 255), 0);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Paints a rectangle with gradient.
        /// </summary>
        public static void GradientPaint(Graphics g, RectangleF rect, Color color, int depth)
        {
            if (rect.Width <= 0 || rect.Height <= 0)
                return;

            if (depth > 0 && Configs.GradientView)
            {
                Color color1 = ColorChanage(color, +depth);
                Color color2 = ColorChanage(color, -depth);
                var rect1 = new RectangleF(rect.X, rect.Y - 1, rect.Width, rect.Height + 2);
                var lgrdBrush = new LinearGradientBrush(rect1, color1, color2, 90);
                g.FillRectangle(lgrdBrush, rect);
            }
            else
            {
                g.FillRectangle(new SolidBrush(color), rect);
            }
        }
    }
}