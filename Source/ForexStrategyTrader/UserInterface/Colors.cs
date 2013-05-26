// Colors Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Structure Colors
    /// </summary>
    public static class LayoutColors
    {
        private static string[] _asColorList;
        private static Dictionary<String, String> _dictColorFiles;
        private static XmlDocument _xmlColors;

        /// <summary>
        /// Constructor
        /// </summary>
        static LayoutColors()
        {
            SetColorsDefault();
        }

        /// <summary>
        /// Gets the language files list.
        /// </summary>
        public static string[] ColorSchemeList
        {
            get { return _asColorList; }
        }

        public static int DepthCaption { get; private set; }
        public static int DepthControl { get; private set; }

        // Workspace
        public static Color ColorFormBack { get; private set; }
        public static Color ColorControlBack { get; private set; }
        public static Color ColorControlText { get; private set; }
        public static Color ColorCaptionBack { get; private set; }
        public static Color ColorCaptionText { get; private set; }
        public static Color ColorEvenRowBack { get; private set; }

        public static Color ColorOddRowBack
        {
            get { return ColorControlBack; }
        }

        public static Color ColorSignalRed { get; private set; }

        // Slots
        public static Color ColorSlotCaptionBackAveraging { get; private set; }
        public static Color ColorSlotCaptionBackOpen { get; private set; }
        public static Color ColorSlotCaptionBackOpenFilter { get; private set; }
        public static Color ColorSlotCaptionBackClose { get; private set; }
        public static Color ColorSlotCaptionBackCloseFilter { get; private set; }
        public static Color ColorSlotCaptionText { get; private set; }
        public static Color ColorSlotBackground { get; private set; }
        public static Color ColorSlotIndicatorText { get; private set; }
        public static Color ColorSlotLogicText { get; private set; }
        public static Color ColorSlotParamText { get; private set; }
        public static Color ColorSlotValueText { get; private set; }
        public static Color ColorSlotDash { get; private set; }

        // Charts
        public static Color ColorChartBack { get; private set; }
        public static Color ColorChartFore { get; private set; }
        public static Color ColorChartGrid { get; private set; }
        public static Color ColorChartCross { get; private set; }
        public static Color ColorLabelBack { get; private set; }
        public static Color ColorLabelText { get; private set; }
        public static Color ColorTradeLong { get; private set; }
        public static Color ColorTradeShort { get; private set; }
        public static Color ColorTradeClose { get; private set; }
        public static Color ColorVolume { get; private set; }
        public static Color ColorBarWhite { get; private set; }
        public static Color ColorBarBlack { get; private set; }
        public static Color ColorBarBorder { get; private set; }
        public static Color ColorChartBalanceLine { get; private set; }
        public static Color ColorChartEquityLine { get; private set; }

        /// <summary>
        /// Sets the default color scheme.
        /// </summary>
        private static void SetColorsDefault()
        {
            DepthCaption = 25;
            DepthControl = 10;

            // Workspace
            ColorFormBack = Color.FromArgb(235, 245, 245);
            ColorControlBack = Color.FromArgb(245, 255, 255);
            ColorControlText = Color.FromArgb(0, 50, 50);
            ColorCaptionBack = Color.FromArgb(102, 153, 204);
            ColorCaptionText = Color.FromArgb(255, 255, 255);
            ColorEvenRowBack = Color.FromArgb(255, 255, 255);
            ColorSignalRed = Color.FromArgb(255, 0, 0);

            // Slots
            ColorSlotCaptionBackAveraging = Color.FromArgb(150, 100, 100);
            ColorSlotCaptionBackOpen = Color.FromArgb(102, 153, 51);
            ColorSlotCaptionBackOpenFilter = Color.FromArgb(102, 153, 153);
            ColorSlotCaptionBackClose = Color.FromArgb(204, 102, 51);
            ColorSlotCaptionBackCloseFilter = Color.FromArgb(210, 140, 140);
            ColorSlotCaptionText = Color.FromArgb(255, 255, 255);
            ColorSlotBackground = Color.FromArgb(245, 255, 255);
            ColorSlotIndicatorText = Color.FromArgb(80, 130, 180);
            ColorSlotLogicText = Color.FromArgb(0, 51, 51);
            ColorSlotParamText = Color.FromArgb(51, 153, 153);
            ColorSlotValueText = Color.FromArgb(51, 153, 153);
            ColorSlotDash = Color.FromArgb(204, 204, 153);

            // Chart
            ColorChartBack = Color.FromArgb(245, 255, 255);
            ColorChartFore = Color.FromArgb(0, 50, 50);
            ColorBarWhite = Color.FromArgb(225, 225, 225);
            ColorBarBlack = Color.FromArgb(30, 30, 30);
            ColorBarBorder = Color.FromArgb(0, 0, 0);
            ColorTradeLong = Color.FromArgb(30, 160, 30);
            ColorTradeShort = Color.FromArgb(225, 30, 30);
            ColorTradeClose = Color.FromArgb(225, 160, 30);
            ColorVolume = Color.FromArgb(150, 0, 210);
            ColorLabelBack = Color.FromArgb(102, 102, 153);
            ColorLabelText = Color.FromArgb(255, 255, 255);
            ColorChartGrid = Color.FromArgb(204, 204, 204);
            ColorChartCross = Color.FromArgb(153, 163, 204);
            ColorChartBalanceLine = Color.FromArgb(102, 102, 153);
            ColorChartEquityLine = Color.FromArgb(225, 204, 51);
        }

        /// <summary>
        /// Loads the color scheme from a file
        /// </summary>
        public static void LoadColorScheme(string sColorScheme)
        {
            try
            {
                _xmlColors = new XmlDocument();
                _xmlColors.Load(sColorScheme);

                // Workspace
                ColorFormBack = ParseColor("FormBack");
                ColorControlBack = ParseColor("ControlBack");
                ColorControlText = ParseColor("ControlText");
                ColorCaptionBack = ParseColor("CaptionBack");
                ColorCaptionText = ParseColor("CaptionText");
                ColorEvenRowBack = ParseColor("EvenRowBack");
                ColorSignalRed = ParseColor("SignalRed");

                // Strategy Slots
                ColorSlotCaptionBackAveraging = ParseColor("SlotCaptionBackProperties");
                ColorSlotCaptionBackOpen = ParseColor("SlotCaptionBackOpen");
                ColorSlotCaptionBackOpenFilter = ParseColor("SlotCaptionBackOpenFilter");
                ColorSlotCaptionBackClose = ParseColor("SlotCaptionBackClose");
                ColorSlotCaptionBackCloseFilter = ParseColor("SlotCaptionBackCloseFilter");
                ColorSlotCaptionText = ParseColor("SlotCaptionText");
                ColorSlotBackground = ParseColor("SlotBack");
                ColorSlotIndicatorText = ParseColor("SlotIndicatorText");
                ColorSlotLogicText = ParseColor("SlotLogicText");
                ColorSlotParamText = ParseColor("SlotParamText");
                ColorSlotValueText = ParseColor("SlotValueText");
                ColorSlotDash = ParseColor("SlotDash");

                // Chart
                ColorChartBack = ParseColor("ChartBack");
                ColorChartFore = ParseColor("ChartFore");
                ColorBarWhite = ParseColor("BarWhite");
                ColorBarBlack = ParseColor("BarBlack");
                ColorBarBorder = ParseColor("BarBorder");
                ColorTradeLong = ParseColor("TradeLong");
                ColorTradeShort = ParseColor("TradeShort");
                ColorTradeClose = ParseColor("TradeClose");
                ColorVolume = ParseColor("Volume");
                ColorChartGrid = ParseColor("ChartGrid");
                ColorChartCross = ParseColor("ChartCross");
                ColorLabelBack = ParseColor("LabelBack");
                ColorLabelText = ParseColor("LabelText");
                ColorChartBalanceLine = ParseColor("ChartBalanceLine");
                ColorChartEquityLine = ParseColor("ChartEquityLine");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Language.T("Load a color scheme..."));
            }
        }

        /// <summary>
        /// Parses the color from the given xml node
        /// </summary>
        private static Color ParseColor(string node)
        {
            int r = int.Parse(_xmlColors.SelectNodes("color/" + node).Item(0).Attributes.Item(0).InnerText);
            int g = int.Parse(_xmlColors.SelectNodes("color/" + node).Item(0).Attributes.Item(1).InnerText);
            int b = int.Parse(_xmlColors.SelectNodes("color/" + node).Item(0).Attributes.Item(2).InnerText);

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// Initializes the languages.
        /// </summary>
        public static void InitColorSchemes()
        {
            _dictColorFiles = new Dictionary<string, string>();
            string colorDirectory = Data.ColorDir;

            if (Directory.Exists(colorDirectory) && Directory.GetFiles(colorDirectory).Length > 0)
            {
                string[] asColorFiles = Directory.GetFiles(colorDirectory);

                foreach (string sLangFile in asColorFiles)
                {
                    if (sLangFile.EndsWith(".xml", true, null))
                    {
                        try
                        {
                            string colorScheme = Path.GetFileNameWithoutExtension(sLangFile);
                            if (colorScheme != null)
                            {
                                _dictColorFiles.Add(colorScheme, sLangFile);

                                if (colorScheme == Configs.ColorScheme)
                                {
                                    LoadColorScheme(sLangFile);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Color Scheme", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                    }
                }
            }

            _asColorList = new string[_dictColorFiles.Count];
            _dictColorFiles.Keys.CopyTo(_asColorList, 0);
            Array.Sort(_asColorList);
        }
    }
}