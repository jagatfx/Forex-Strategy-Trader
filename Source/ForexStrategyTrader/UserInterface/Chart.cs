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
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public enum ChartButtons
    {
        Grid,
        Cross,
        Volume,
        Orders,
        PositionLots,
        PositionPrice,
        ZoomIn,
        ZoomOut,
        Refresh,
        TrueCharts,
        Shift,
        AutoScroll,
        DInfoUp,
        DInfoDwn,
        DynamicInfo
    }

    /// <summary>
    ///     Class Indicator Chart : Form
    /// </summary>
    public sealed class Chart : Panel
    {
        private const int ChartRightShift = 80;
        private readonly Brush brushBack;
        private readonly Brush brushDiIndicator;
        private readonly Brush brushDynamicInfo;
        private readonly Brush brushEvenRows;
        private readonly Brush brushFore;
        private readonly Brush brushLabelBkgrd;
        private readonly Brush brushLabelFore;
        private readonly Brush brushTradeClose;
        private readonly Brush brushTradeLong;
        private readonly Brush brushTradeShort;
        private readonly Color colorBarBlack1;
        private readonly Color colorBarBlack2;
        private readonly Color colorBarWhite1;
        private readonly Color colorBarWhite2;
        private readonly Color colorClosedTrade1;
        private readonly Color colorClosedTrade2;
        private readonly Color colorLongTrade1;
        private readonly Color colorLongTrade2;
        private readonly Color colorShortTrade1;
        private readonly Color colorShortTrade2;
        private readonly Font font;
        private readonly Font fontDi; // Font for Dynamic info
        private readonly Font fontDiInd; // Font for Dynamic info Indicators
        private readonly Pen penBarBorder;
        private readonly Pen penBarThick;
        private readonly Pen penCross;
        private readonly Pen penGrid;
        private readonly Pen penGridSolid;
        private readonly Pen penTradeClose;
        private readonly Pen penTradeLong;
        private readonly Pen penTradeShort;
        private readonly Pen penVolume;
        private readonly int spcBottom; // pnlPrice bottom margin
        private readonly int spcLeft; // pnlPrice left margin
        private readonly int spcRight; // pnlPrice right margin
        private readonly int spcTop; // pnlPrice top margin
        private int[] aiInfoType; // 0 - text; 1 - Indicator; 
        private string[] asInfoTitle;
        private string[] asInfoValue;
        private int barOld;

        private int barPixels = 9;
        private int chartBars;
        private ChartData chartData;
        private string chartTitle;
        private int chartWidth;
        private int countLabels; // The count of price labels on the vertical axe.
        private double deltaGrid; // The distance between two vertical label in price.
        private int dynInfoScrollValue;
        private int dynInfoWidth;
        private int firstBar;
        private int indPanels;
        private int infoRows;
        private bool isCandleChart = true;
        private bool isChartAutoScroll;
        private bool isChartShift;
        private bool isCrossShown;
        private bool isCtrlKeyPressed;
        private bool isDebug;
        private bool isDrawDinInfo;
        private bool isGridShown;
        private bool isInfoPanelShown;
        private bool isMouseInIndicatorChart;
        private bool isMouseInPriceChart;
        private bool isOrdersShown;
        private bool isPositionLotsShown;
        private bool isPositionPriceShown;
        private bool isTrueChartsShown;
        private bool isVolumeShown;
        private int lastBar;
        private double maxPrice;

        private int maxVolume; // Max Volume in the chart
        private double minPrice;
        private int mouseX;
        private int mouseXOld;
        private int mouseY;
        private int mouseYOld;
        private bool[] repeatedIndicator;
        private double scaleYVol; // The scale for drawing the Volume

        private double[] sepChartMaxValue;
        private double[] sepChartMinValue;

        private Size szDate;
        private Size szDateL;
        private Size szPrice;
        private Size szSL;
        private int verticalScale = 1;
        private int xDynInfoCol2;
        private int xLeft; // pnlPrice left coordinate
        private int xRight; // pnlPrice right coordinate
        private int yBottom; // pnlPrice bottom coordinate
        private int yBottomText; // pnlPrice bottom coordinate for date text
        private double yScale;
        private int yTop; // pnlPrice top coordinate

// ------------------------------------------------------------
        /// <summary>
        ///     The default constructor.
        /// </summary>
        public Chart(ChartData data)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            chartData = data;

            BackColor = LayoutColors.ColorFormBack;
            Padding = new Padding(0);

            PnlCharts = new Panel {Parent = this, Dock = DockStyle.Fill};

            PnlInfo = new Panel {Parent = this, BackColor = LayoutColors.ColorControlBack, Dock = DockStyle.Right};
            PnlInfo.Paint += PnlInfoPaint;

            barPixels = Configs.ChartZoom;
            isGridShown = Configs.ChartGrid;
            isCrossShown = Configs.ChartCross;
            isVolumeShown = Configs.ChartVolume;
            isPositionLotsShown = Configs.ChartLots;
            isOrdersShown = Configs.ChartOrders;
            isPositionPriceShown = Configs.ChartPositionPrice;
            isInfoPanelShown = Configs.ChartInfoPanel;
            isTrueChartsShown = Configs.ChartTrueCharts;
            isChartShift = Configs.ChartShift;
            isChartAutoScroll = Configs.ChartAutoScroll;

            dynInfoScrollValue = 0;

            font = new Font(Font.FontFamily, 8);

            // Dynamic info fonts
            fontDi = new Font(Font.FontFamily, 9);
            fontDiInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();
            szDate = g.MeasureString("99/99 99:99", font).ToSize();
            szDateL = g.MeasureString("99/99/99 99:99", font).ToSize();
            // TODO checking exact price with.
            szPrice = g.MeasureString("9.99999", font).ToSize();
            szSL = g.MeasureString(" SL", font).ToSize();
            g.Dispose();

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            PnlInfo.Visible = isInfoPanelShown;
            PnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 2, 0) : new Padding(0);

            PnlCharts.Resize += PnlChartsResize;
            PnlPrice.Resize += PnlPriceResize;

            spcTop = font.Height;
            spcBottom = font.Height*8/5;
            spcLeft = 0;
            spcRight = szPrice.Width + szSL.Width + 2;

            brushBack = new SolidBrush(LayoutColors.ColorChartBack);
            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            brushLabelBkgrd = new SolidBrush(LayoutColors.ColorLabelBack);
            brushLabelFore = new SolidBrush(LayoutColors.ColorLabelText);
            brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            brushDiIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            brushEvenRows = new SolidBrush(LayoutColors.ColorEvenRowBack);
            brushTradeLong = new SolidBrush(LayoutColors.ColorTradeLong);
            brushTradeShort = new SolidBrush(LayoutColors.ColorTradeShort);
            brushTradeClose = new SolidBrush(LayoutColors.ColorTradeClose);

            penGrid = new Pen(LayoutColors.ColorChartGrid);
            penGridSolid = new Pen(LayoutColors.ColorChartGrid);
            penCross = new Pen(LayoutColors.ColorChartCross);
            penVolume = new Pen(LayoutColors.ColorVolume);
            penBarBorder = new Pen(LayoutColors.ColorBarBorder);
            penBarThick = new Pen(LayoutColors.ColorBarBorder, 3);
            penTradeLong = new Pen(LayoutColors.ColorTradeLong);
            penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            penGrid.DashStyle = DashStyle.Dash;
            penGrid.DashPattern = new float[] {4, 2};

            colorBarWhite1 = Data.ColorChanage(LayoutColors.ColorBarWhite, 30);
            colorBarWhite2 = Data.ColorChanage(LayoutColors.ColorBarWhite, -30);
            colorBarBlack1 = Data.ColorChanage(LayoutColors.ColorBarBlack, 30);
            colorBarBlack2 = Data.ColorChanage(LayoutColors.ColorBarBlack, -30);

            colorLongTrade1 = Data.ColorChanage(LayoutColors.ColorTradeLong, 30);
            colorLongTrade2 = Data.ColorChanage(LayoutColors.ColorTradeLong, -30);
            colorShortTrade1 = Data.ColorChanage(LayoutColors.ColorTradeShort, 30);
            colorShortTrade2 = Data.ColorChanage(LayoutColors.ColorTradeShort, -30);
            colorClosedTrade1 = Data.ColorChanage(LayoutColors.ColorTradeClose, 30);
            colorClosedTrade2 = Data.ColorChanage(LayoutColors.ColorTradeClose, -30);
        }

        private Panel PnlCharts { get; set; }
        private Panel PnlInfo { get; set; }
        private Panel PnlPrice { get; set; }
        private Panel[] PnlInd { get; set; }
        private Splitter[] SplitterInd { get; set; }
        private HScrollBar ScrollBar { get; set; }

        private ToolStrip StripButtons { get; set; }
        private ToolStripButton[] ChartButtons { get; set; }

        /// <summary>
        ///     Performs post initialization settings.
        /// </summary>
        public void InitChart(ChartData data)
        {
            chartData = data;
            ScrollBar.Select();
        }

        /// <summary>
        ///     Updates the chart after a tick.
        /// </summary>
        public void UpdateChartOnTick(bool repaintChart, ChartData data)
        {
            chartData = data;

            if (repaintChart)
                SetupChartTitle();

            bool updateWholeChart = repaintChart;
            double oldMaxPrice = maxPrice;
            double oldMinPrice = minPrice;

            if (isChartAutoScroll || repaintChart)
                SetFirstLastBar();

            SetPriceChartMinMaxValues();

            if (Math.Abs(maxPrice - oldMaxPrice) > chartData.InstrumentProperties.Point ||
                Math.Abs(minPrice - oldMinPrice) > chartData.InstrumentProperties.Point)
                updateWholeChart = true;

            if (updateWholeChart)
            {
                PnlPrice.Invalidate();
            }
            else
            {
                int left = spcLeft + (chartBars - 2)*barPixels;
                int width = PnlPrice.ClientSize.Width - left;
                var rect = new Rectangle(left, 0, width, yBottom + 1);
                PnlPrice.Invalidate(rect);
            }

            for (int i = 0; i < PnlInd.Length; i++)
            {
                var slot = (int) PnlInd[i].Tag;
                oldMaxPrice = sepChartMaxValue[slot];
                oldMinPrice = sepChartMinValue[slot];
                SetSepChartsMinMaxValues(i);
                if (Math.Abs(sepChartMaxValue[slot] - oldMaxPrice) > 0.000001 ||
                    Math.Abs(sepChartMinValue[slot] - oldMinPrice) > 0.000001)
                    updateWholeChart = true;

                if (updateWholeChart)
                {
                    PnlInd[i].Invalidate();
                }
                else
                {
                    int left = spcLeft + (chartBars - 2)*barPixels;
                    int width = PnlInd[i].ClientSize.Width - left;
                    var rect = new Rectangle(left, 0, width, yBottom + 1);
                    PnlInd[i].Invalidate(rect);
                }
            }

            if (isInfoPanelShown && !isCrossShown)
                GenerateDynamicInfo(lastBar);
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            isCtrlKeyPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        ///     Create and sets the indicator panels.
        /// </summary>
        private void SetupIndicatorPanels()
        {
            PnlPrice = new Panel {Parent = PnlCharts, Dock = DockStyle.Fill, BackColor = LayoutColors.ColorChartBack};
            PnlPrice.MouseLeave += PnlPriceMouseLeave;
            PnlPrice.MouseMove += PnlPriceMouseMove;
            PnlPrice.MouseDown += PanelMouseDown;
            PnlPrice.MouseUp += PanelMouseUp;
            PnlPrice.Paint += PnlPricePaint;

            StripButtons = new ToolStrip {Parent = PnlCharts};

            sepChartMinValue = new double[chartData.Strategy.Slots];
            sepChartMaxValue = new double[chartData.Strategy.Slots];

            // Indicator panels
            indPanels = 0;
            var asIndicatorTexts = new string[chartData.Strategy.Slots];
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                SlotTypes slotType = chartData.Strategy.Slot[slot].SlotType;
                Indicator indicator = IndicatorManager.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName);
                indicator.Initialize(slotType);
                indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
                asIndicatorTexts[slot] = indicator.ToString();
                indPanels += chartData.Strategy.Slot[slot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            repeatedIndicator = new bool[chartData.Strategy.Slots];
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                repeatedIndicator[slot] = false;
                for (int i = 0; i < slot; i++)
                    repeatedIndicator[slot] = asIndicatorTexts[slot] == asIndicatorTexts[i];
            }

            PnlInd = new Panel[indPanels];
            SplitterInd = new Splitter[indPanels];
            for (int i = 0; i < indPanels; i++)
            {
                SplitterInd[i] = new Splitter
                    {
                        Parent = PnlCharts,
                        BorderStyle = BorderStyle.None,
                        Dock = DockStyle.Bottom,
                        Height = 2
                    };

                PnlInd[i] = new Panel
                    {Parent = PnlCharts, Dock = DockStyle.Bottom, BackColor = LayoutColors.ColorControlBack};
                PnlInd[i].Paint += PnlIndPaint;
                PnlInd[i].MouseMove += PnlIndMouseMove;
                PnlInd[i].MouseLeave += PnlIndMouseLeave;
                PnlInd[i].MouseDown += PanelMouseDown;
                PnlInd[i].MouseUp += PanelMouseUp;
                PnlInd[i].Tag = i; // A temporary tag.
            }

            int index = 0;
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (!chartData.Strategy.Slot[slot].SeparatedChart) continue;
                PnlInd[index].Tag = slot; // The real tag.
                index++;
            }

            ScrollBar = new HScrollBar {Parent = PnlCharts, Dock = DockStyle.Bottom, TabStop = true, SmallChange = 1};
            ScrollBar.ValueChanged += ScrollValueChanged;
            ScrollBar.MouseWheel += ScrollMouseWheel;
            ScrollBar.KeyUp += ScrollKeyUp;
            ScrollBar.KeyDown += ScrollKeyDown;

            for (int i = 0; i < indPanels; i++)
                PnlInd[i].Resize += PnlIndResize;
        }

        /// <summary>
        ///     Sets up the chart's buttons.
        /// </summary>
        private void SetupButtons()
        {
            ChartButtons = new ToolStripButton[15];
            for (int i = 0; i < 15; i++)
            {
                ChartButtons[i] = new ToolStripButton
                    {Tag = (ChartButtons) i, DisplayStyle = ToolStripItemDisplayStyle.Image};
                ChartButtons[i].Click += ButtonChartClick;
                StripButtons.Items.Add(ChartButtons[i]);
                if (i > 11)
                    ChartButtons[i].Alignment = ToolStripItemAlignment.Right;
                if (i == 1 || i == 5 || i == 7 || i == 8)
                    StripButtons.Items.Add(new ToolStripSeparator());
            }

            // Grid
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Grid].Image = Resources.chart_grid;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Grid].ToolTipText = Language.T("Grid") + "   G";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Grid].Checked = Configs.ChartGrid;

            // Cross
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Cross].Image = Resources.chart_cross;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Cross].ToolTipText = Language.T("Cross") + "   C";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Cross].Checked = Configs.ChartCross;

            // Volume
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Volume].Image = Resources.chart_volume;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Volume].ToolTipText = Language.T("Volume") + "   V";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Volume].Checked = Configs.ChartVolume;

            // Orders
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Orders].Image = Resources.chart_entry_points;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Orders].ToolTipText = Language.T("Orders") + "   O";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Orders].Checked = Configs.ChartOrders;

            // Position lots
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionLots].Image = Resources.chart_lots;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionLots].ToolTipText =
                Language.T("Position lots") + "   L";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionLots].Checked = Configs.ChartLots;

            // Position price
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionPrice].Image = Resources.chart_pos_price;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionPrice].ToolTipText =
                Language.T("Position price") + "   P";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionPrice].Checked = Configs.ChartPositionPrice;

            // Zoom in
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.ZoomIn].Image = Resources.chart_zoom_in;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.ZoomIn].ToolTipText = Language.T("Zoom in") + "   +";

            // Zoom out
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.ZoomOut].Image = Resources.chart_zoom_out;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.ZoomOut].ToolTipText = Language.T("Zoom out") + "   -";

            // Refresh
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Refresh].Image = Resources.chart_refresh;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Refresh].ToolTipText = Language.T("Refresh chart") +
                                                                                        "   F5";

            // True Charts
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.TrueCharts].Image = Resources.chart_true_charts;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.TrueCharts].Checked = Configs.ChartTrueCharts;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.TrueCharts].ToolTipText =
                Language.T("True indicator charts") + "   T";

            // Shift
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Shift].Image = Resources.chart_shift;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Shift].ToolTipText = Language.T("Chart shift") +
                                                                                      "   S";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Shift].Checked = Configs.ChartShift;

            // Auto Scroll
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.AutoScroll].Image = Resources.chart_auto_scroll;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.AutoScroll].ToolTipText = Language.T("Auto scroll") +
                                                                                           "   R";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.AutoScroll].Checked = Configs.ChartAutoScroll;

            // Show dynamic info
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DynamicInfo].Image = Resources.chart_dyninfo;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DynamicInfo].Checked = Configs.ChartInfoPanel;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DynamicInfo].ToolTipText =
                Language.T("Show / hide info panel") + "   I";

            // Move Dynamic Info Down
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].Image = Resources.chart_dinfo_down;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].ToolTipText = Language.T("Move info down") +
                                                                                         "   Z";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].Visible = isInfoPanelShown;

            // Move Dynamic Info Up
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Image = Resources.chart_dinfo_up;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].ToolTipText = Language.T("Move info up") +
                                                                                        "   A";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Visible = isInfoPanelShown;
        }

        /// <summary>
        ///     Sets the chart's parameters.
        /// </summary>
        private void SetFirstLastBar()
        {
            ScrollBar.Minimum = chartData.FirstBar;
            ScrollBar.Maximum = chartData.Bars - 1;

            int shift = isChartShift ? ChartRightShift : 0;
            chartBars = (chartWidth - shift - 7)/barPixels;
            chartBars = Math.Min(chartBars, chartData.Bars - chartData.FirstBar);
            firstBar = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            firstBar = Math.Min(firstBar, chartData.Bars - 1);
            lastBar = Math.Max(firstBar + chartBars - 1, firstBar);

            ScrollBar.Value = firstBar;
            ScrollBar.LargeChange = Math.Max(chartBars, 1);
        }

        /// <summary>
        ///     Sets the min and the max values of price shown on the chart.
        /// </summary>
        private void SetPriceChartMinMaxValues()
        {
            // Searching the min and the max price and volume
            maxPrice = double.MinValue;
            minPrice = double.MaxValue;
            maxVolume = int.MinValue;
            double spread = chartData.InstrumentProperties.Spread*chartData.InstrumentProperties.Point;
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                if (chartData.High[bar] + spread > maxPrice) maxPrice = chartData.High[bar] + spread;
                if (chartData.Low[bar] < minPrice) minPrice = chartData.Low[bar];
                if (chartData.Volume[bar] > maxVolume) maxVolume = chartData.Volume[bar];
            }

            double pricePixel = (maxPrice - minPrice)/(yBottom - yTop);
            if (isVolumeShown)
                minPrice -= pricePixel*30;
            else if (isPositionLotsShown)
                minPrice -= pricePixel*10;

            maxPrice += pricePixel*verticalScale;
            minPrice -= pricePixel*verticalScale;

            // Grid
            double deltaPoint = (chartData.InstrumentProperties.Digits == 5 ||
                                 chartData.InstrumentProperties.Digits == 3)
                                    ? chartData.InstrumentProperties.Point*100
                                    : chartData.InstrumentProperties.Point*10;
            int roundStep = Math.Max(chartData.InstrumentProperties.Digits - 1, 1);
            countLabels = Math.Max((yBottom - yTop)/35, 1);
            deltaGrid = Math.Max(Math.Round((maxPrice - minPrice)/countLabels, roundStep), deltaPoint);
            minPrice = Math.Round(minPrice, roundStep) - deltaPoint;
            countLabels = (int) Math.Ceiling((maxPrice - minPrice)/deltaGrid);
            maxPrice = minPrice + countLabels*deltaGrid;
            yScale = (yBottom - yTop)/(countLabels*deltaGrid);
            scaleYVol = maxVolume > 0 ? 40.0/maxVolume : 0; // 40 - the highest volume line
        }

        /// <summary>
        ///     Sets parameter of separated charts
        /// </summary>
        private void SetSepChartsMinMaxValues(int index)
        {
            Panel panel = PnlInd[index];
            var slot = (int) panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in chartData.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value) < 0.00001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            sepChartMaxValue[slot] = maxValue;
            sepChartMinValue[slot] = minValue;
        }

        /// <summary>
        ///     Sets the indicator chart title
        /// </summary>
        private void SetupChartTitle()
        {
            chartTitle = chartData.StrategyName + " " + chartData.Symbol + " " + chartData.PeriodStr + " (" +
                         chartData.Bars + " bars)";

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                    if (component.ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }

                if (isChart)
                {
                    Indicator indicator =
                        IndicatorManager.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName);
                    indicator.Initialize(chartData.Strategy.Slot[slot].SlotType);
                    indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
                    if (!chartTitle.Contains(indicator.ToString()))
                        chartTitle += Environment.NewLine + indicator;
                }
            }
        }

        /// <summary>
        ///     Sets the sizes of the panels after resizing.
        /// </summary>
        private void PnlChartsResize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetFirstLastBar();
            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            GenerateDynamicInfo(lastBar);
            dynInfoScrollValue = 0;
        }

        /// <summary>
        ///     Calculates the panels' height
        /// </summary>
        private void SetAllPanelsHeight()
        {
            int availableHeight = PnlCharts.ClientSize.Height - StripButtons.Height - ScrollBar.Height - indPanels*2;
            int pnlIndHeight = availableHeight/(2 + indPanels);

            foreach (Panel panel in PnlInd)
                panel.Height = pnlIndHeight;
        }

        /// <summary>
        ///     Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        private void PnlPriceResize(object sender, EventArgs e)
        {
            xLeft = spcLeft;
            xRight = PnlPrice.ClientSize.Width - spcRight;
            chartWidth = Math.Max(xRight - xLeft, 0);
            yTop = spcTop;
            yBottom = PnlPrice.ClientSize.Height - spcBottom;
            yBottomText = PnlPrice.ClientSize.Height - spcBottom*5/8 - 4;

            SetPriceChartMinMaxValues();

            PnlPrice.Invalidate();
        }

        /// <summary>
        ///     Invalidates the panels
        /// </summary>
        private void PnlIndResize(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            var slot = (int) panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(firstBar, component.FirstBar); bar <= lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in chartData.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value) < 0.0001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            sepChartMaxValue[slot] = maxValue;
            sepChartMinValue[slot] = minValue;

            panel.Invalidate();
        }

        /// <summary>
        ///     Paints the panel PnlPrice
        /// </summary>
        private void PnlPricePaint(object sender, PaintEventArgs e)
        {
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            try
            {
                g.Clear(LayoutColors.ColorChartBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (chartBars == 0)
            {
                DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
                return;
            }

            // Grid
            for (double label = minPrice;
                 label <= maxPrice + chartData.InstrumentProperties.Point;
                 label += deltaGrid)
            {
                var labelY = (int) (yBottom - (label - minPrice)*yScale);
                g.DrawString(label.ToString(Data.FF), font, brushFore, xRight, labelY - Font.Height/2 - 1);
                if (isGridShown || Math.Abs(label - minPrice) < 0.00001)
                    g.DrawLine(penGrid, spcLeft, labelY, xRight, labelY);
                else
                    g.DrawLine(penGrid, xRight - 5, labelY, xRight, labelY);
            }
            for (int vertLineBar = lastBar;
                 vertLineBar > firstBar;
                 vertLineBar -= (szDate.Width + 10)/barPixels + 1)
            {
                int xVertLine = (vertLineBar - firstBar)*barPixels + spcLeft + barPixels/2 - 1;
                if (isGridShown)
                    g.DrawLine(penGrid, xVertLine, yTop, xVertLine, yBottom + 2);
                string date = String.Format("{0} {1}", chartData.Time[vertLineBar].ToString(Data.DFS),
                                            chartData.Time[vertLineBar].ToString("HH:mm"));
                g.DrawString(date, font, brushFore, xVertLine - szDate.Width/2, yBottomText);
            }

            // Draws Volume, Lots and Bars
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                int x = (bar - firstBar)*barPixels + spcLeft;
                int xCenter = x + (barPixels - 1)/2 - 1;
                var yOpen = (int) (yBottom - (chartData.Open[bar] - minPrice)*yScale);
                var yHigh = (int) (yBottom - (chartData.High[bar] - minPrice)*yScale);
                var yLow = (int) (yBottom - (chartData.Low[bar] - minPrice)*yScale);
                var yClose = (int) (yBottom - (chartData.Close[bar] - minPrice)*yScale);
                var yVolume = (int) (yBottom - chartData.Volume[bar]*scaleYVol);

                // Draw the volume
                if (isVolumeShown && yVolume != yBottom)
                    g.DrawLine(penVolume, x + barPixels/2 - 1, yVolume, x + barPixels/2 - 1, yBottom);

                // Draw position's lots
                if (isPositionLotsShown && chartData.BarStatistics.ContainsKey(chartData.Time[bar]))
                {
                    PosDirection dir = chartData.BarStatistics[chartData.Time[bar]].PositionDir;
                    if (dir != PosDirection.None)
                    {
                        double lots = chartData.BarStatistics[chartData.Time[bar]].PositionLots;
                        var iPosHight = (int) (Math.Max(lots*3, 2));
                        int iPosY = yBottom - iPosHight + 1;
                        var rect = new Rectangle(x - 1, iPosY, barPixels, iPosHight);
                        LinearGradientBrush lgBrush;
                        if (dir == PosDirection.Long)
                            lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                        else if (dir == PosDirection.Short)
                            lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                        else
                            lgBrush = new LinearGradientBrush(rect, colorClosedTrade1, colorClosedTrade2, 0f);
                        rect = new Rectangle(x, iPosY, barPixels - 2, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (isCandleChart)
                {
                    g.DrawLine(barPixels < 29 ? penBarBorder : penBarThick, xCenter, yLow, xCenter, yHigh);

                    if (yClose < yOpen)
                    {
                        // White bar
                        var rect = new Rectangle(x + 1, yClose, barPixels - 5, yOpen - yClose);
                        var lgBrush = new LinearGradientBrush(rect, colorBarWhite1, colorBarWhite2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(penBarBorder, rect);
                    }
                    else if (yClose > yOpen)
                    {
                        // Black bar
                        var rect = new Rectangle(x + 1, yOpen, barPixels - 5, yClose - yOpen);
                        var lgBrush = new LinearGradientBrush(rect, colorBarBlack1, colorBarBlack2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(penBarBorder, rect);
                    }
                    else
                    {
                        // Cross
                        g.DrawLine(barPixels < 29 ? penBarBorder : penBarThick, x + 1, yClose, x + barPixels - 4,
                                   yClose);
                    }
                }
                else
                {
                    if (barPixels <= 16)
                    {
                        g.DrawLine(penBarBorder, xCenter, yLow, xCenter, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarBorder, x, yOpen, xCenter, yOpen);
                            g.DrawLine(penBarBorder, xCenter, yClose, x + barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarBorder, x, yClose, x + barPixels - 3, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(penBarThick, xCenter, yLow + 2, xCenter, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(penBarThick, x + 1, yOpen, xCenter, yOpen);
                            g.DrawLine(penBarThick, xCenter - 1, yClose, x + barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(penBarThick, x, yClose, x + barPixels - 3, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(spcLeft, yTop, xRight, yBottom - yTop));
            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot].SeparatedChart || repeatedIndicator[slot]) continue;

                int cloudUp = -1; // For Ichimoku and similar
                int cloudDown = -1; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int indicatorValueShift = 1;
                foreach (ListParam listParam in chartData.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < chartData.Strategy.Slot[slot].Component.Length; comp++)
                {
                    var pen = new Pen(chartData.Strategy.Slot[slot].Component[comp].ChartColor);
                    var penTc = new Pen(chartData.Strategy.Slot[slot].Component[comp].ChartColor)
                        {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                    if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {
                        // Line
                        if (isTrueChartsShown)
                        {
                            // True Charts
                            var point = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = spcLeft + (bar - firstBar)*barPixels + 1 +
                                        indicatorValueShift*(barPixels - 5);
                                var y = (int) Math.Round(yBottom - (value - minPrice)*yScale);

                                if (Math.Abs(value) < 0.0001)
                                    point[bar - firstBar] = point[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    point[bar - firstBar] = new Point(x, y);
                            }

                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                // All bars except the last one
                                int i = bar - firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                                if (bar == firstBar && isIndicatorValueAtClose)
                                {
                                    // First bar
                                    double value = chartData.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = spcLeft + (bar - firstBar)*barPixels;
                                    var y = (int) Math.Round(yBottom - (value - minPrice)*yScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (barPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 7, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 6, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTc, x + barPixels - 4, y + 2, x + barPixels - 4,
                                                       point[i].Y - 2);
                                        else
                                            g.DrawLine(penTc, x + barPixels - 4, y - 2, x + barPixels - 4,
                                                       point[i].Y + 2);
                                    }
                                }

                                if (bar < lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);

                                    if (barPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                        else
                                            g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i + 1].Y > point[i].Y)
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                       point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                       point[i + 1].Y + 2);
                                    }
                                }

                                if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                                {
                                    // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 5, point[i].Y);
                                }
                            }
                        }
                        else
                        {
                            var aPoint = new Point[lastBar - firstBar + 1];
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - firstBar)*barPixels + barPixels/2 - 1 + spcLeft;
                                var y = (int) (yBottom - (dValue - minPrice)*yScale);

                                if (Math.Abs(dValue) < 0.0001)
                                    aPoint[bar - firstBar] = aPoint[Math.Max(bar - firstBar - 1, 0)];
                                else
                                    aPoint[bar - firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);
                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {
                        // Dots
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar)*barPixels + barPixels/2 - 1 + spcLeft;
                            var y = (int) (yBottom - (dValue - minPrice)*yScale);
                            if (barPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {
                        // Level
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - firstBar)*barPixels + spcLeft;
                            var y = (int) (yBottom - (dValue - minPrice)*yScale);
                            g.DrawLine(pen, x, y, x + barPixels - 1, y);
                        }
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp >= 0 && cloudDown >= 0)
                {
                    var apntUp = new PointF[lastBar - firstBar + 1];
                    var apntDown = new PointF[lastBar - firstBar + 1];
                    for (int bar = firstBar; bar <= lastBar; bar++)
                    {
                        double dValueUp = chartData.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = chartData.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        apntUp[bar - firstBar].X = (bar - firstBar)*barPixels + barPixels/2 - 1 + spcLeft;
                        apntUp[bar - firstBar].Y = (int) (yBottom - (dValueUp - minPrice)*yScale);
                        apntDown[bar - firstBar].X = (bar - firstBar)*barPixels + barPixels/2 - 1 + spcLeft;
                        apntDown[bar - firstBar].Y = (int) (yBottom - (dValueDown - minPrice)*yScale);
                    }

                    var pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(apntUp[0].X, 0), apntUp[0]);
                    pathUp.AddLines(apntUp);
                    pathUp.AddLine(apntUp[lastBar - firstBar], new PointF(apntUp[lastBar - firstBar].X, 0));
                    pathUp.AddLine(new PointF(apntUp[lastBar - firstBar].X, 0), new PointF(apntUp[0].X, 0));

                    var pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(apntDown[0].X, 0), apntDown[0]);
                    pathDown.AddLines(apntDown);
                    pathDown.AddLine(apntDown[lastBar - firstBar], new PointF(apntDown[lastBar - firstBar].X, 0));
                    pathDown.AddLine(new PointF(apntDown[lastBar - firstBar].X, 0), new PointF(apntDown[0].X, 0));

                    Color colorUp = Color.FromArgb(50, chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    var penUp = new Pen(chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    var penDown = new Pen(chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    penUp.DashStyle = DashStyle.Dash;
                    penDown.DashStyle = DashStyle.Dash;

                    Brush brushUp = new SolidBrush(colorUp);
                    Brush brushDown = new SolidBrush(colorDown);

                    var regionUp = new Region(pathUp);
                    regionUp.Exclude(pathDown);
                    g.FillRegion(brushDown, regionUp);

                    var regionDown = new Region(pathDown);
                    regionDown.Exclude(pathUp);
                    g.FillRegion(brushUp, regionDown);

                    g.DrawLines(penUp, apntUp);
                    g.DrawLines(penDown, apntDown);
                }
            }
            g.ResetClip();

            // Draws position price and deals.
            for (int bar = firstBar; bar <= lastBar; bar++)
            {
                DateTime bartime = chartData.Time[bar];
                if (!chartData.BarStatistics.ContainsKey(bartime))
                    continue;

                int x = (bar - firstBar)*barPixels + spcLeft;

                // Draws the position's price
                if (isPositionPriceShown)
                {
                    double price = chartData.BarStatistics[bartime].PositionPrice;
                    if (price > chartData.InstrumentProperties.Point)
                    {
                        var yPrice = (int) (yBottom - (price - minPrice)*yScale);

                        if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Long)
                        {
                            // Long
                            g.DrawLine(penTradeLong, x, yPrice, x + barPixels - 2, yPrice);
                        }
                        else if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Short)
                        {
                            // Short
                            g.DrawLine(penTradeShort, x, yPrice, x + barPixels - 2, yPrice);
                        }
                        else if (chartData.BarStatistics[bartime].PositionDir == PosDirection.Closed)
                        {
                            // Closed
                            g.DrawLine(penTradeClose, x, yPrice, x + barPixels - 2, yPrice);
                        }
                    }
                }

                // Draw the deals
                if (isOrdersShown)
                {
                    foreach (Operation operation in chartData.BarStatistics[bartime].Operations)
                    {
                        var yOrder = (int) (yBottom - (operation.OperationPrice - minPrice)*yScale);

                        if (operation.OperationType == OperationType.Buy)
                        {
                            // Buy
                            var pen = new Pen(brushTradeLong, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 4, yOrder);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder - 3);
                            }
                            else if (barPixels > 9)
                            {
                                int d = (barPixels - 1)/2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x, yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder - d);
                                g.DrawLine(pen, x2 + 1, yOrder - d + 1, x1 + d/2 + 1, yOrder - d + 1);
                                g.DrawLine(pen, x2, yOrder - d, x2, yOrder - d/2);
                            }
                        }
                        else if (operation.OperationType == OperationType.Sell)
                        {
                            // Sell
                            var pen = new Pen(brushTradeShort, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder + 1, x + 4, yOrder + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder + 3);
                            }
                            else if (barPixels > 9)
                            {
                                int d = (barPixels - 1)/2 - 1;
                                int x1 = x + d;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x, yOrder + 1, x1 + 1, yOrder + 1);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder + d);
                                g.DrawLine(pen, x1 + d/2 + 1, yOrder + d, x2, yOrder + d);
                                g.DrawLine(pen, x2, yOrder + d, x2, yOrder + d/2 + 1);
                            }
                        }
                        else if (operation.OperationType == OperationType.Close)
                        {
                            // Close
                            var pen = new Pen(brushTradeClose, 2);
                            if (barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + barPixels - 1, yOrder);
                            }
                            else if (barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 7, yOrder);
                                g.DrawLine(pen, x + 5, yOrder - 2, x + 5, yOrder + 2);
                            }
                            else if (barPixels > 9)
                            {
                                int d = (barPixels - 1)/2 - 1;
                                int x1 = x + d + 1;
                                int x2 = x + barPixels - 3;
                                g.DrawLine(pen, x, yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder + d/2, x2, yOrder - d/2);
                                g.DrawLine(pen, x1, yOrder - d/2, x2, yOrder + d/2);
                            }
                        }
                    }
                }
            }

            // Bid price label.
            var yBid = (int) (yBottom - (chartData.Bid - minPrice)*yScale);
            var pBid = new Point(xRight, yBid - szPrice.Height/2);
            string sBid = (chartData.Bid.ToString(Data.FF));
            int xBidRight = xRight + szPrice.Width + 1;
            var apBid = new[]
                {
                    new PointF(xRight - 6, yBid),
                    new PointF(xRight, yBid - szPrice.Height/2),
                    new PointF(xBidRight, yBid - szPrice.Height/2 - 1),
                    new PointF(xBidRight, yBid + szPrice.Height/2 + 1),
                    new PointF(xRight, yBid + szPrice.Height/2)
                };

            // Position price.
            if (isPositionPriceShown &&
                (chartData.PositionDirection == PosDirection.Long || chartData.PositionDirection == PosDirection.Short))
            {
                var yPos = (int) (yBottom - (chartData.PositionOpenPrice - minPrice)*yScale);
                var pPos = new Point(xRight, yPos - szPrice.Height/2);
                string sPos = (chartData.PositionOpenPrice.ToString(Data.FF));
                var brushText = new SolidBrush(LayoutColors.ColorChartBack);

                if (chartData.PositionOpenPrice > minPrice && chartData.PositionOpenPrice < maxPrice)
                {
                    var penPos = new Pen(LayoutColors.ColorTradeLong);
                    if (chartData.PositionDirection == PosDirection.Short)
                        penPos = new Pen(LayoutColors.ColorTradeShort);
                    var apPos = new[]
                        {
                            new PointF(xRight - 6, yPos),
                            new PointF(xRight, yPos - szPrice.Height/2),
                            new PointF(xRight + szPrice.Width, yPos - szPrice.Height/2),
                            new PointF(xRight + szPrice.Width, yPos + szPrice.Height/2),
                            new PointF(xRight, yPos + szPrice.Height/2),
                            new PointF(xRight - 6, yPos)
                        };
                    g.FillPolygon(brushBack, apPos);
                    g.DrawString(sPos, font, brushFore, pPos);
                    g.DrawLines(penPos, apPos);
                }

                // Profit Arrow
                Pen penProfit = chartData.PositionProfit > 0
                                    ? new Pen(LayoutColors.ColorTradeLong, 7)
                                    : new Pen(LayoutColors.ColorTradeShort, 7);
                penProfit.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(penProfit, xBidRight + 9, yPos, xBidRight + 9, yBid);

                // Close Price
                IndicatorSlot slot = chartData.Strategy.Slot[chartData.Strategy.CloseSlot];
                if (slot.IndParam.ExecutionTime != ExecutionTime.AtBarClosing)
                {
                    double dClosePrice = 0;
                    for (int iComp = 0; iComp < slot.Component.Length; iComp++)
                    {
                        IndComponentType compType = slot.Component[iComp].DataType;
                        if (chartData.PositionDirection == PosDirection.Long &&
                            compType == IndComponentType.CloseLongPrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                        else if (chartData.PositionDirection == PosDirection.Short &&
                                 compType == IndComponentType.CloseShortPrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                        else if (compType == IndComponentType.ClosePrice ||
                                 compType == IndComponentType.OpenClosePrice)
                            dClosePrice = slot.Component[iComp].Value[chartData.Bars - 1];
                    }
                    if (dClosePrice > minPrice && dClosePrice < maxPrice)
                    {
                        var yClose = (int) (yBottom - (dClosePrice - minPrice)*yScale);
                        var pClose = new Point(xRight, yClose - szPrice.Height/2);
                        string sClose = (dClosePrice.ToString(Data.FF) + " X");
                        var apClose = new[]
                            {
                                new PointF(xRight - 6, yClose),
                                new PointF(xRight, yClose - szPrice.Height/2),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yClose - szPrice.Height/2 - 1),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yClose + szPrice.Height/2 + 1),
                                new PointF(xRight, yClose + szPrice.Height/2)
                            };
                        g.FillPolygon(new SolidBrush(LayoutColors.ColorTradeClose), apClose);
                        g.DrawString(sClose, font, brushText, pClose);
                    }

                    // Take Profit
                    if (chartData.PositionTakeProfit > minPrice && chartData.PositionTakeProfit < maxPrice)
                    {
                        var yLimit = (int) (yBottom - (chartData.PositionTakeProfit - minPrice)*yScale);
                        var pLimit = new Point(xRight, yLimit - szPrice.Height/2);
                        string sLimit = (chartData.PositionTakeProfit.ToString(Data.FF) + " TP");
                        var apLimit = new[]
                            {
                                new PointF(xRight - 6, yLimit),
                                new PointF(xRight, yLimit - szPrice.Height/2),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yLimit - szPrice.Height/2 - 1),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yLimit + szPrice.Height/2 + 1),
                                new PointF(xRight, yLimit + szPrice.Height/2)
                            };
                        var brushTakeProffit = new SolidBrush(LayoutColors.ColorTradeLong);
                        g.FillPolygon(brushTakeProffit, apLimit);
                        g.DrawString(sLimit, font, brushText, pLimit);
                    }

                    // Stop Loss
                    if (chartData.PositionStopLoss > minPrice && chartData.PositionStopLoss < maxPrice)
                    {
                        var yStop = (int) (yBottom - (chartData.PositionStopLoss - minPrice)*yScale);
                        var pStop = new Point(xRight, yStop - szPrice.Height/2);
                        string sStop = (chartData.PositionStopLoss.ToString(Data.FF) + " SL");
                        var apStop = new[]
                            {
                                new PointF(xRight - 6, yStop),
                                new PointF(xRight, yStop - szPrice.Height/2),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yStop - szPrice.Height/2 - 1),
                                new PointF(xRight + szPrice.Width + szSL.Width - 2,
                                           yStop + szPrice.Height/2 + 1),
                                new PointF(xRight, yStop + szPrice.Height/2)
                            };
                        var brushStopLoss = new SolidBrush(LayoutColors.ColorTradeShort);
                        g.FillPolygon(brushStopLoss, apStop);
                        g.DrawString(sStop, font, brushText, pStop);
                    }
                }
            }

            // Draws Bid price label.
            g.FillPolygon(brushLabelBkgrd, apBid);
            g.DrawString(sBid, font, brushLabelFore, pBid);

            // Cross
            if (isCrossShown && mouseX > xLeft - 1 && mouseX < xRight + 1)
            {
                int bar = (mouseX - spcLeft)/barPixels;
                bar = Math.Max(0, bar);
                bar = Math.Min(chartBars - 1, bar);
                bar += firstBar;
                bar = Math.Min(chartData.Bars - 1, bar);

                // Vertical positions
                var point = new Point(mouseX - szDateL.Width/2, yBottomText);
                var rec = new Rectangle(point, szDateL);

                // Vertical line
                if (isMouseInPriceChart && mouseY > yTop - 1 && mouseY < yBottom + 1)
                {
                    g.DrawLine(penCross, mouseX, yTop, mouseX, mouseY - 10);
                    g.DrawLine(penCross, mouseX, mouseY + 10, mouseX, yBottomText);
                }
                else if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.DrawLine(penCross, mouseX, yTop, mouseX, yBottomText);
                }

                // Date Window
                if (isMouseInPriceChart || isMouseInIndicatorChart)
                {
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sDate = chartData.Time[bar].ToString(Data.DF) + " " + chartData.Time[bar].ToString("HH:mm");
                    g.DrawString(sDate, font, brushLabelFore, point);
                }

                if (isMouseInPriceChart && mouseY > yTop - 1 && mouseY < yBottom + 1)
                {
                    // Horizontal positions
                    point = new Point(xRight, mouseY - szPrice.Height/2);
                    rec = new Rectangle(point, szPrice);
                    // Horizontal line
                    g.DrawLine(penCross, xLeft, mouseY, mouseX - 10, mouseY);
                    g.DrawLine(penCross, mouseX + 10, mouseY, xRight, mouseY);
                    // Price Window
                    g.FillRectangle(brushLabelBkgrd, rec);
                    g.DrawRectangle(penCross, rec);
                    string sPrice = ((yBottom - mouseY)/yScale + minPrice).ToString(Data.FF);
                    g.DrawString(sPrice, font, brushLabelFore, point);
                }
            }

            // Chart title
            g.DrawString(chartTitle, font, brushFore, spcLeft, 0);
            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Paints the panel PnlInd
        /// </summary>
        private void PnlIndPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            try
            {
                g.Clear(LayoutColors.ColorChartBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (chartBars == 0)
            {
                DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
                return;
            }

            int topSpace = font.Height/2 + 2;
            int bottomSpace = font.Height/2;

            var slot = (int) pnl.Tag;
            double minValue = sepChartMinValue[slot];
            double maxValue = sepChartMaxValue[slot];

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace)/(Math.Max(maxValue - minValue, 0.0001));

            // Grid
            String format;
            double deltaLabel;
            int xGridRight = pnl.ClientSize.Width - spcRight + 2;

            // Zero line
            double label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, xRight, labelYZero - font.Height/2 - 1);
                g.DrawLine(penGridSolid, spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue;
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, xRight, labelYMin - font.Height/2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }
            label = maxValue;
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), font, brushFore, xRight, labelYMax - font.Height/2 - 1);
                if (isGridShown)
                    g.DrawLine(penGrid, spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }
            if (chartData.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < chartData.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = chartData.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        var labelY = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
                        if (Math.Abs(labelY - labelYZero) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMin) < font.Height) continue;
                        if (Math.Abs(labelY - labelYMax) < font.Height) continue;
                        deltaLabel = Math.Abs(label);
                        format = deltaLabel < 10
                                     ? "F4"
                                     : deltaLabel < 100
                                           ? "F3"
                                           : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(format), font, brushFore, xRight, labelY - font.Height/2 - 1);
                        if (isGridShown)
                            g.DrawLine(penGrid, spcLeft, labelY, xGridRight, labelY);
                        else
                            g.DrawLine(penGrid, xGridRight - 5, labelY, xGridRight, labelY);
                    }
                }

            if (isGridShown)
            {
                // Vertical lines
                string date = chartData.Time[firstBar].ToString("dd.MM") + " " +
                              chartData.Time[firstBar].ToString("HH:mm");
                var dateWidth = (int) g.MeasureString(date, font).Width;
                for (int vertLineBar = lastBar;
                     vertLineBar > firstBar;
                     vertLineBar -= (dateWidth + 10)/barPixels + 1)
                {
                    int xVertLine = spcLeft + (vertLineBar - firstBar)*barPixels + barPixels/2 - 1;
                    g.DrawLine(penGrid, xVertLine, topSpace, xVertLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int indicatorValueShift = 1;
            foreach (ListParam listParam in chartData.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in chartData.Strategy.Slot[slot].Component)
            {
                if (component.ChartType == IndChartType.Histogram)
                {
                    // Histogram
                    double zero = 0;
                    if (zero < minValue) zero = minValue;
                    if (zero > maxValue) zero = maxValue;
                    var y0 = (int) (pnl.ClientSize.Height - 5 - (zero - minValue)*scale);

                    var penGreen = new Pen(LayoutColors.ColorTradeLong);
                    var penRed = new Pen(LayoutColors.ColorTradeShort);

                    bool isPrevBarGreen = false;

                    if (isTrueChartsShown)
                    {
                        // True Chart Histogram
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = spcLeft + (bar - firstBar)*barPixels + barPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.00001 && isPrevBarGreen)
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penGreen, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    if (y != y0)
                                    {
                                        if (y > y0)
                                            g.DrawLine(penRed, x, y0, x, y);
                                        else if (y < y0 - 2)
                                            g.DrawLine(penRed, x, y0 - 2, x, y);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*barPixels + barPixels - 4;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.00001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    isPrevBarGreen = true;
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    isPrevBarGreen = false;
                                }
                            }
                        }
                        else
                        {
                            for (int bar = firstBar; bar <= lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*barPixels + barPixels/2 - 1;
                                var y = (int) Math.Round(pnl.ClientSize.Height - 7 - (value - minValue)*scale);

                                if (value > prevValue || Math.Abs(value - prevValue) < 0.00001 && isPrevBarGreen)
                                {
                                    g.DrawLine(penGreen, x, y + 1, x, y - 1);
                                    g.DrawLine(penGreen, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penGreen, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penGreen, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = true;
                                    }
                                }
                                else
                                {
                                    g.DrawLine(penRed, x, y + 1, x, y - 1);
                                    g.DrawLine(penRed, x - 1, y, x + 1, y);
                                    if (y != y0)
                                    {
                                        if (y > y0 + 3)
                                            g.DrawLine(penRed, x, y0, x, y - 3);
                                        else if (y < y0 - 5)
                                            g.DrawLine(penRed, x, y0 - 2, x, y + 3);
                                        isPrevBarGreen = false;
                                    }
                                }
                            }
                        }
                    }

                    if (!isTrueChartsShown)
                    {
                        // Regular Histogram Chart
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - firstBar)*barPixels + spcLeft + 1;
                            var y = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (value - minValue)*scale);

                            LinearGradientBrush lgBrush;
                            Rectangle rect;
                            if (value > prevValue || Math.Abs(value - prevValue) < 0.00001 && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorLongTrade1, colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 4, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = true;
                            }
                            else
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, colorShortTrade1, colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, barPixels - 4, y0 - y);
                                }
                                else
                                    continue;
                                g.FillRectangle(lgBrush, rect);
                                isPrevBarGreen = false;
                            }
                        }
                    }
                }

                if (component.ChartType == IndChartType.Line)
                {
                    // Line
                    if (isTrueChartsShown)
                    {
                        // True Charts
                        var pen = new Pen(component.ChartColor);
                        var penTc = new Pen(component.ChartColor)
                            {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                        int yIndChart = pnl.ClientSize.Height - bottomSpace;

                        var point = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = spcLeft + (bar - firstBar)*barPixels + 1 + indicatorValueShift*(barPixels - 5);
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                            point[bar - firstBar] = new Point(x, y);
                        }

                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            // All bars except the last one
                            int i = bar - firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == firstBar && isIndicatorValueAtClose)
                            {
                                // First bar
                                double value = component.Value[bar - 1];
                                int x = spcLeft + (bar - firstBar)*barPixels;
                                var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (barPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 7, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 6, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + barPixels - 4, y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTc, x + barPixels - 4, y + 2, x + barPixels - 4, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTc, x + barPixels - 4, y - 2, x + barPixels - 4, point[i].Y + 2);
                                }
                            }

                            if (bar < lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (barPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 3, point[i].Y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X - 2, point[i].Y);
                                    else
                                        g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i + 1].X, point[i].Y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i + 1].Y > point[i].Y)
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                   point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTc, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                   point[i + 1].Y + 2);
                                }
                            }

                            if (bar == lastBar && !isIndicatorValueAtClose && barPixels > 3)
                            {
                                // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + barPixels - 5, point[i].Y);
                            }
                        }
                    }

                    if (!isTrueChartsShown)
                    {
                        // Regular Line Chart
                        var points = new Point[lastBar - firstBar + 1];
                        for (int bar = firstBar; bar <= lastBar; bar++)
                        {
                            double dValue = component.Value[bar];
                            int x = (bar - firstBar)*barPixels + barPixels/2 - 1 + spcLeft;
                            var y = (int) (pnl.ClientSize.Height - bottomSpace - (dValue - minValue)*scale);
                            points[bar - firstBar] = new Point(x, y);
                        }
                        g.DrawLines(new Pen(component.ChartColor), points);
                    }
                }
            }

            // Vertical cross line
            if (isCrossShown && (isMouseInIndicatorChart || isMouseInPriceChart) && mouseX > xLeft - 1 &&
                mouseX < xRight + 1)
                g.DrawLine(penCross, mouseX, 0, mouseX, pnl.ClientSize.Height);

            // Chart title

            Indicator indicator = IndicatorManager.ConstructIndicator(chartData.Strategy.Slot[slot].IndicatorName);
            indicator.Initialize(chartData.Strategy.Slot[slot].SlotType);
            indicator.IndParam = chartData.Strategy.Slot[slot].IndParam;
            string title = indicator.ToString();
            Size sizeTitle = g.MeasureString(title, Font).ToSize();
            g.FillRectangle(brushBack, new Rectangle(spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(title, Font, brushFore, spcLeft + 2, 0);

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Invalidates the panels
        /// </summary>
        private void InvalidateAllPanels()
        {
            PnlPrice.Invalidate();
            foreach (Panel panel in PnlInd)
                panel.Invalidate();
        }

        /// <summary>
        ///     Sets the width of the info panel
        /// </summary>
        private void SetupDynInfoWidth()
        {
            asInfoTitle = new string[200];
            aiInfoType = new int[200];
            infoRows = 0;

            // Dynamic info titles
            asInfoTitle[infoRows++] = Language.T("Bar number");
            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Open lots");
            asInfoTitle[infoRows++] = Language.T("Position price");

            for (int iSlot = 0; iSlot < chartData.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = chartData.Strategy.Slot[iSlot].IndicatorName +
                                          (chartData.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) asInfoTitle[infoRows++] = indComp.CompName;
            }

            Graphics g = CreateGraphics();

            int iMaxLenght = 0;
            foreach (string str in asInfoTitle)
            {
                var iLenght = (int) g.MeasureString(str, fontDi).Width;
                if (iMaxLenght < iLenght) iMaxLenght = iLenght;
            }

            xDynInfoCol2 = iMaxLenght + 10;
            var maxInfoWidth = (int) g.MeasureString("99/99/99     ", fontDi).Width;

            g.Dispose();

            dynInfoWidth = xDynInfoCol2 + maxInfoWidth + (isDebug ? 30 : 5);

            PnlInfo.ClientSize = new Size(dynInfoWidth, PnlInfo.ClientSize.Height);
        }

        /// <summary>
        ///     Sets the dynamic info panel
        /// </summary>
        private void SetupDynamicInfo()
        {
            asInfoTitle = new string[200];
            aiInfoType = new int[200];
            infoRows = 0;

            asInfoTitle[infoRows++] = Language.T("Date");
            asInfoTitle[infoRows++] = Language.T("Opening time");
            asInfoTitle[infoRows++] = Language.T("Opening price");
            asInfoTitle[infoRows++] = Language.T("Highest price");
            asInfoTitle[infoRows++] = Language.T("Lowest price");
            asInfoTitle[infoRows++] = Language.T("Closing price");
            asInfoTitle[infoRows++] = Language.T("Volume");
            asInfoTitle[infoRows++] = "";
            asInfoTitle[infoRows++] = Language.T("Position direction");
            asInfoTitle[infoRows++] = Language.T("Open lots");
            asInfoTitle[infoRows++] = Language.T("Position price");

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo)
                        compToShow++;

                if (compToShow == 0)
                    continue;

                asInfoTitle[infoRows++] = "";
                aiInfoType[infoRows] = 1;
                asInfoTitle[infoRows++] = chartData.Strategy.Slot[slot].IndicatorName +
                                          (chartData.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");

                foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo)
                        asInfoTitle[infoRows++] = indComp.CompName;
            }
        }

        /// <summary>
        ///     Generates the DynamicInfo.
        /// </summary>
        private void GenerateDynamicInfo(int barNumb)
        {
            if (!isInfoPanelShown) return;

            int bar;

            if (barNumb != chartData.Bars - 1)
            {
                barNumb = Math.Max(0, barNumb);
                barNumb = Math.Min(chartBars - 1, barNumb);

                bar = firstBar + barNumb;
                bar = Math.Min(chartData.Bars - 1, bar);
            }
            else
                bar = barNumb;

            if (barOld == bar && bar != lastBar) return;
            barOld = bar;

            int row = 0;
            asInfoValue = new String[200];
            asInfoValue[row++] = chartData.Time[bar].ToString(Data.DF);
            asInfoValue[row++] = chartData.Time[bar].ToString("HH:mm");
            if (isDebug)
            {
                asInfoValue[row++] = chartData.Open[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = chartData.High[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = chartData.Low[bar].ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = chartData.Close[bar].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                asInfoValue[row++] = chartData.Open[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.High[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.Low[bar].ToString(Data.FF);
                asInfoValue[row++] = chartData.Close[bar].ToString(Data.FF);
            }
            asInfoValue[row++] = chartData.Volume[bar].ToString(CultureInfo.InvariantCulture);

            asInfoValue[row++] = "";
            DateTime baropen = chartData.Time[bar];
            if (chartData.BarStatistics.ContainsKey(baropen))
            {
                asInfoValue[row++] = Language.T(chartData.BarStatistics[baropen].PositionDir.ToString());
                asInfoValue[row++] =
                    chartData.BarStatistics[baropen].PositionLots.ToString(CultureInfo.InvariantCulture);
                asInfoValue[row++] = chartData.BarStatistics[baropen].PositionPrice.ToString(Data.FF);
            }
            else
            {
                asInfoValue[row++] = Language.T("Square");
                asInfoValue[row++] = "   -";
                asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < chartData.Strategy.Slots; slot++)
            {
                if (chartData.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    asInfoValue[row++] = "";
                    asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in chartData.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataTipe = indComp.DataType;
                            if (indDataTipe == IndComponentType.AllowOpenLong ||
                                indDataTipe == IndComponentType.AllowOpenShort ||
                                indDataTipe == IndComponentType.ForceClose ||
                                indDataTipe == IndComponentType.ForceCloseLong ||
                                indDataTipe == IndComponentType.ForceCloseShort)
                                asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (isDebug)
                                {
                                    asInfoValue[row++] = indComp.Value[bar].ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFr = dl < 10
                                                     ? "F5"
                                                     : dl < 100
                                                           ? "F4"
                                                           : dl < 1000
                                                                 ? "F3"
                                                                 : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (Math.Abs(indComp.Value[bar]) > 0.00001)
                                        asInfoValue[row++] = indComp.Value[bar].ToString(sFr);
                                    else
                                        asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            PnlInfo.Invalidate(new Rectangle(xDynInfoCol2, 0, dynInfoWidth - xDynInfoCol2, PnlInfo.ClientSize.Height));
        }

        /// <summary>
        ///     Paints the panel PnlInfo.
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            if (!isInfoPanelShown) return;

            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            try
            {
                g.Clear(LayoutColors.ColorControlBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            int iRowHeight = fontDi.Height + 1;
            var size = new Size(dynInfoWidth, iRowHeight);

            for (int i = 0; i < infoRows; i++)
            {
                var point0 = new Point(0, i*iRowHeight + 1);
                var point1 = new Point(5, i*iRowHeight);
                var point2 = new Point(xDynInfoCol2, i*iRowHeight);

                if (Math.Abs(i%2f - 0) > 0.0001)
                    g.FillRectangle(brushEvenRows, new Rectangle(point0, size));

                if (aiInfoType[i + dynInfoScrollValue] == 1)
                    g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDiInd, brushDiIndicator, point1);
                else
                    g.DrawString(asInfoTitle[i + dynInfoScrollValue], fontDi, brushDynamicInfo, point1);
                g.DrawString(asInfoValue[i + dynInfoScrollValue], fontDi, brushDynamicInfo, point2);
            }

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        private void PnlPriceMouseMove(object sender, MouseEventArgs e)
        {
            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (mouseX > xRight)
                {
                    if (mouseY > mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();

                    return;
                }

                int newScrollValue = ScrollBar.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int) (ScrollBar.SmallChange*0.1*(100 - barPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int) (ScrollBar.SmallChange*0.1*(100 - barPixels));

                if (newScrollValue < ScrollBar.Minimum)
                    newScrollValue = ScrollBar.Minimum;
                else if (newScrollValue > ScrollBar.Maximum + 1 - ScrollBar.LargeChange)
                    newScrollValue = ScrollBar.Maximum + 1 - ScrollBar.LargeChange;

                ScrollBar.Value = newScrollValue;
            }

            // Determines the shown bar.
            int shownBar = lastBar;
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                // Moving inside the chart
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    isMouseInPriceChart = true;
                    isDrawDinInfo = true;
                    shownBar = (e.X - xLeft)/barPixels;
                    if (isCrossShown)
                        PnlPrice.Cursor = Cursors.Cross;
                }
                    // Escaping from the chart
                else
                {
                    isMouseInPriceChart = false;
                    shownBar = lastBar;
                    PnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (mouseX >= xLeft && mouseX <= xRight)
            {
                // Entering into the chart
                isMouseInPriceChart = true;
                isDrawDinInfo = true;
                shownBar = (e.X - xLeft)/barPixels;
                if (isCrossShown)
                    PnlPrice.Cursor = Cursors.Cross;
            }

            if (!isCrossShown)
                return;

            var path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                if (mouseYOld >= yTop && mouseYOld <= yBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, mouseYOld, PnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(xRight - 1, mouseYOld - font.Height/2 - 1, szPrice.Width + 2,
                                                    font.Height + 2));
                }
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));
            }

            // Adding the new positions
            if (mouseX >= xLeft && mouseX <= xRight)
            {
                if (mouseY >= yTop && mouseY <= yBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, mouseY, PnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(xRight - 1, mouseY - font.Height/2 - 1, szPrice.Width + 2,
                                                    font.Height + 2));
                }
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseX, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseX - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));
            }
            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
            {
                var path1 = new GraphicsPath(FillMode.Winding);
                if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                    path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                    path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                PnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);
        }

        /// <summary>
        ///     Deletes the cross and Dynamic Info
        /// </summary>
        private void PnlPriceMouseLeave(object sender, EventArgs e)
        {
            PnlPrice.Cursor = Cursors.Default;
            isMouseInPriceChart = false;

            if (!isCrossShown)
                return;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            var path = new GraphicsPath(FillMode.Winding);

            // Horizontal Line
            path.AddRectangle(new Rectangle(0, mouseYOld, PnlPrice.ClientSize.Width, 1));
            // PriceBox
            path.AddRectangle(new Rectangle(xRight - 1, mouseYOld - font.Height/2 - 1, szPrice.Width + 2,
                                            font.Height + 2));
            // Vertical Line
            path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
            // DateBox
            path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                            font.Height + 3));

            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
                PnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

            if (isInfoPanelShown)
                GenerateDynamicInfo(lastBar);
        }

        /// <summary>
        ///     Mouse moves inside a chart
        /// </summary>
        private void PnlIndMouseMove(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = e.X;
            mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = ScrollBar.Value;

                if (mouseX > mouseXOld)
                    newScrollValue -= (int) Math.Round(ScrollBar.SmallChange*0.1*(100 - barPixels));
                else if (mouseX < mouseXOld)
                    newScrollValue += (int) Math.Round(ScrollBar.SmallChange*0.1*(100 - barPixels));

                if (newScrollValue < ScrollBar.Minimum)
                    newScrollValue = ScrollBar.Minimum;
                else if (newScrollValue > ScrollBar.Maximum + 1 - ScrollBar.LargeChange)
                    newScrollValue = ScrollBar.Maximum + 1 - ScrollBar.LargeChange;

                ScrollBar.Value = newScrollValue;
            }

            // Determines the shown bar.
            int shownBar = lastBar;
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                if (mouseX >= xLeft && mouseX <= xRight)
                {
                    // Moving inside the chart
                    isMouseInIndicatorChart = true;
                    isDrawDinInfo = true;
                    shownBar = (e.X - xLeft)/barPixels;
                    if (isCrossShown)
                        panel.Cursor = Cursors.Cross;
                }
                else
                {
                    // Escaping from the bar area of chart
                    isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                    shownBar = lastBar;
                }
            }
            else if (mouseX >= xLeft && mouseX <= xRight)
            {
                // Entering into the chart
                isMouseInIndicatorChart = true;
                isDrawDinInfo = true;
                PnlInfo.Invalidate();
                shownBar = (e.X - xLeft)/barPixels;
                if (isCrossShown)
                    panel.Cursor = Cursors.Cross;
            }

            if (!isCrossShown)
                return;

            var path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (mouseXOld >= xLeft && mouseXOld <= xRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));
            }

            // Adding the new positions
            if (mouseX >= xLeft && mouseX <= xRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(mouseX, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseX - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));
            }
            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < indPanels; i++)
            {
                var path1 = new GraphicsPath(FillMode.Winding);
                if (mouseXOld > xLeft - 1 && mouseXOld < xRight + 1)
                    path1.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                if (mouseX > xLeft - 1 && mouseX < xRight + 1)
                    path1.AddRectangle(new Rectangle(mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                PnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);
        }

        /// <summary>
        ///     Mouse leaves a chart.
        /// </summary>
        private void PnlIndMouseLeave(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = Cursors.Default;

            isMouseInIndicatorChart = false;

            mouseXOld = mouseX;
            mouseYOld = mouseY;
            mouseX = -1;
            mouseY = -1;
            barOld = -1;

            if (isCrossShown)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(mouseXOld - szDateL.Width/2 - 1, yBottomText - 1, szDateL.Width + 2,
                                                font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < indPanels; i++)
                    PnlInd[i].Invalidate(new Rectangle(mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
            }
        }

        /// <summary>
        ///     Mouse Button Up
        /// </summary>
        private void PanelMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = isCrossShown ? Cursors.Cross : Cursors.Default;
            ScrollBar.Focus();
        }

        /// <summary>
        ///     Mouse Button Down
        /// </summary>
        private void PanelMouseDown(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            if (panel == PnlPrice && mouseX > xRight)
                panel.Cursor = Cursors.SizeNS;
            else if (!isCrossShown)
                panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        ///     Sets the parameters when scrolling.
        /// </summary>
        private void ScrollValueChanged(object sender, EventArgs e)
        {
            firstBar = ScrollBar.Value;
            lastBar = Math.Min(chartData.Bars - 1, firstBar + chartBars - 1);
            lastBar = Math.Max(lastBar, firstBar);

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);

            InvalidateAllPanels();

            barOld = 0;
            if (isInfoPanelShown && isDrawDinInfo && isCrossShown)
            {
                int selectedBar = (mouseX - spcLeft)/barPixels;
                GenerateDynamicInfo(selectedBar);
            }
        }

        /// <summary>
        ///     Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        private void ScrollMouseWheel(object sender, MouseEventArgs e)
        {
            if (isCtrlKeyPressed)
            {
                if (e.Delta > 0)
                    ZoomIn();
                if (e.Delta < 0)
                    ZoomOut();
            }
            else
            {
                int newScrollValue = ScrollBar.Value +
                                     ScrollBar.LargeChange*e.Delta/SystemInformation.MouseWheelScrollLines/120;

                if (newScrollValue < ScrollBar.Minimum)
                    newScrollValue = ScrollBar.Minimum;
                else if (newScrollValue > ScrollBar.Maximum + 1 - ScrollBar.LargeChange)
                    newScrollValue = ScrollBar.Maximum + 1 - ScrollBar.LargeChange;

                ScrollBar.Value = newScrollValue;
            }
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        private void ScrollKeyUp(object sender, KeyEventArgs e)
        {
            isCtrlKeyPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        ///     Call KeyUp method
        /// </summary>
        private void ScrollKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                isCtrlKeyPressed = true;
        }

        /// <summary>
        ///     Changes chart's settings after a button click.
        /// </summary>
        private void ButtonChartClick(object sender, EventArgs e)
        {
            var tsButton = (ToolStripButton) sender;
            var buton = (ChartButtons) tsButton.Tag;

            switch (buton)
            {
                case ForexStrategyBuilder.ChartButtons.Grid:
                    ShortcutKeyUp(new KeyEventArgs(Keys.G));
                    break;
                case ForexStrategyBuilder.ChartButtons.Cross:
                    ShortcutKeyUp(new KeyEventArgs(Keys.C));
                    break;
                case ForexStrategyBuilder.ChartButtons.Volume:
                    ShortcutKeyUp(new KeyEventArgs(Keys.V));
                    break;
                case ForexStrategyBuilder.ChartButtons.Orders:
                    ShortcutKeyUp(new KeyEventArgs(Keys.O));
                    break;
                case ForexStrategyBuilder.ChartButtons.PositionLots:
                    ShortcutKeyUp(new KeyEventArgs(Keys.L));
                    break;
                case ForexStrategyBuilder.ChartButtons.PositionPrice:
                    ShortcutKeyUp(new KeyEventArgs(Keys.P));
                    break;
                case ForexStrategyBuilder.ChartButtons.ZoomIn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Add));
                    break;
                case ForexStrategyBuilder.ChartButtons.ZoomOut:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Subtract));
                    break;
                case ForexStrategyBuilder.ChartButtons.Refresh:
                    ShortcutKeyUp(new KeyEventArgs(Keys.F5));
                    break;
                case ForexStrategyBuilder.ChartButtons.TrueCharts:
                    ShortcutKeyUp(new KeyEventArgs(Keys.T));
                    break;
                case ForexStrategyBuilder.ChartButtons.Shift:
                    ShortcutKeyUp(new KeyEventArgs(Keys.S));
                    break;
                case ForexStrategyBuilder.ChartButtons.AutoScroll:
                    ShortcutKeyUp(new KeyEventArgs(Keys.R));
                    break;
                case ForexStrategyBuilder.ChartButtons.DInfoDwn:
                    ShortcutKeyUp(new KeyEventArgs(Keys.Z));
                    break;
                case ForexStrategyBuilder.ChartButtons.DInfoUp:
                    ShortcutKeyUp(new KeyEventArgs(Keys.A));
                    break;
                case ForexStrategyBuilder.ChartButtons.DynamicInfo:
                    ShortcutKeyUp(new KeyEventArgs(Keys.I));
                    break;
            }
        }

        /// <summary>
        ///     Shortcut keys
        /// </summary>
        private void ShortcutKeyUp(KeyEventArgs e)
        {
            // Zoom in
            if (!e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                ZoomIn();
            }
                // Zoom out
            else if (!e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                ZoomOut();
            }
                // Vertical scale increase
            else if (e.Control && (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus))
            {
                VerticalScaleIncrease();
            }
                // Vertical scale decrease
            else if (e.Control && (e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus))
            {
                VerticalScaleDecrease();
            }
            else if (e.KeyCode == Keys.Space)
            {
                isCandleChart = !isCandleChart;
                PnlPrice.Invalidate();
            }
                // Refresh
            else if (e.KeyCode == Keys.F5)
            {
                SetFirstLastBar();
                SetPriceChartMinMaxValues();
                for (int i = 0; i < PnlInd.Length; i++)
                    SetSepChartsMinMaxValues(i);
                InvalidateAllPanels();
            }
                // Grid
            else if (e.KeyCode == Keys.G)
            {
                isGridShown = !isGridShown;
                Configs.ChartGrid = isGridShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Grid].Checked = isGridShown;
                InvalidateAllPanels();
            }
                // Cross
            else if (e.KeyCode == Keys.C)
            {
                isCrossShown = !isCrossShown;
                Configs.ChartCross = isCrossShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Cross].Checked = isCrossShown;
                InvalidateAllPanels();
                if (isCrossShown)
                {
                    GenerateDynamicInfo((mouseX - xLeft)/barPixels);
                    PnlPrice.Cursor = Cursors.Cross;
                    foreach (Panel pnlind in PnlInd)
                        pnlind.Cursor = Cursors.Cross;
                }
                else
                {
                    GenerateDynamicInfo(chartData.Bars - 1);
                    PnlPrice.Cursor = Cursors.Default;
                    foreach (Panel pnlind in PnlInd)
                        pnlind.Cursor = Cursors.Default;
                }
            }
                // Volume
            else if (e.KeyCode == Keys.V)
            {
                isVolumeShown = !isVolumeShown;
                Configs.ChartVolume = isVolumeShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Volume].Checked = isVolumeShown;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
                // Lots
            else if (e.KeyCode == Keys.L)
            {
                isPositionLotsShown = !isPositionLotsShown;
                Configs.ChartLots = isPositionLotsShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionLots].Checked = isPositionLotsShown;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
                // Orders
            else if (e.KeyCode == Keys.O)
            {
                isOrdersShown = !isOrdersShown;
                Configs.ChartOrders = isOrdersShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Orders].Checked = isOrdersShown;
                PnlPrice.Invalidate();
            }
                // Position price
            else if (e.KeyCode == Keys.P)
            {
                isPositionPriceShown = !isPositionPriceShown;
                Configs.ChartPositionPrice = isPositionPriceShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionPrice].Checked = isPositionPriceShown;
                PnlPrice.Invalidate();
            }
                // True Charts
            else if (e.KeyCode == Keys.T)
            {
                isTrueChartsShown = !isTrueChartsShown;
                Configs.ChartTrueCharts = isTrueChartsShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.TrueCharts].Checked = isTrueChartsShown;
                InvalidateAllPanels();
            }
                // Chart shift
            else if (e.KeyCode == Keys.S)
            {
                isChartShift = !isChartShift;
                Configs.ChartShift = isChartShift;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Shift].Checked = isChartShift;
                SetFirstLastBar();
            }
                // Chart Auto Scroll
            else if (e.KeyCode == Keys.R)
            {
                isChartAutoScroll = !isChartAutoScroll;
                Configs.ChartAutoScroll = isChartAutoScroll;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.AutoScroll].Checked = isChartAutoScroll;
                SetFirstLastBar();
            }
                // Dynamic info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                dynInfoScrollValue += 5;
                dynInfoScrollValue = dynInfoScrollValue > infoRows - 5 ? infoRows - 5 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Dynamic info scroll up
            else if (e.KeyCode == Keys.A)
            {
                dynInfoScrollValue -= 5;
                dynInfoScrollValue = dynInfoScrollValue < 0 ? 0 : dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Show info panel
            else if (e.KeyCode == Keys.I || e.KeyCode == Keys.F2)
            {
                isInfoPanelShown = !isInfoPanelShown;
                Configs.ChartInfoPanel = isInfoPanelShown;
                PnlInfo.Visible = isInfoPanelShown;
                PnlCharts.Padding = isInfoPanelShown ? new Padding(0, 0, 4, 0) : new Padding(0);
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Visible = isInfoPanelShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].Visible = isInfoPanelShown;
            }
                // Debug
            else if (e.KeyCode == Keys.F12)
            {
                isDebug = !isDebug;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                PnlInfo.Invalidate();
            }
        }

        /// <summary>
        ///     Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleDecrease()
        {
            if (verticalScale > 10)
            {
                verticalScale -= 10;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
        }

        /// <summary>
        ///     Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleIncrease()
        {
            if (verticalScale < 300)
            {
                verticalScale += 10;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
        }

        /// <summary>
        ///     Zooms the chart in.
        /// </summary>
        private void ZoomIn()
        {
            barPixels += 4;
            if (barPixels > 45)
                barPixels = 45;

            int oldChartBars = chartBars;

            chartBars = chartWidth/barPixels;
            if (chartBars > chartData.Bars - chartData.FirstBar)
                chartBars = chartData.Bars - chartData.FirstBar;

            if (lastBar < chartData.Bars - 1)
            {
                firstBar += (oldChartBars - chartBars)/2;
                if (firstBar > chartData.Bars - chartBars)
                    firstBar = chartData.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            ScrollBar.Value = firstBar;
            ScrollBar.LargeChange = chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = barPixels;
        }

        /// <summary>
        ///     Zooms the chart out.
        /// </summary>
        private void ZoomOut()
        {
            barPixels -= 4;
            if (barPixels < 9)
                barPixels = 9;

            int oldChartBars = chartBars;

            chartBars = chartWidth/barPixels;
            if (chartBars > chartData.Bars - chartData.FirstBar)
                chartBars = chartData.Bars - chartData.FirstBar;

            if (lastBar < chartData.Bars - 1)
            {
                firstBar -= (chartBars - oldChartBars)/2;
                if (firstBar < chartData.FirstBar)
                    firstBar = chartData.FirstBar;

                if (firstBar > chartData.Bars - chartBars)
                    firstBar = chartData.Bars - chartBars;
            }
            else
            {
                firstBar = Math.Max(chartData.FirstBar, chartData.Bars - chartBars);
            }

            lastBar = firstBar + chartBars - 1;

            ScrollBar.Value = firstBar;
            ScrollBar.LargeChange = chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = barPixels;
        }
    }
}