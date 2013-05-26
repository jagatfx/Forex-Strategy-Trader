// Chart : Panel
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Properties;

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
    /// Class Indicator Chart : Form
    /// </summary>
    public sealed class Chart : Panel
    {
        private const int ChartRightShift = 80;
        private readonly Brush _brushBack;
        private readonly Brush _brushDiIndicator;
        private readonly Brush _brushDynamicInfo;
        private readonly Brush _brushEvenRows;
        private readonly Brush _brushFore;
        private readonly Brush _brushLabelBkgrd;
        private readonly Brush _brushLabelFore;
        private readonly Brush _brushTradeClose;
        private readonly Brush _brushTradeLong;
        private readonly Brush _brushTradeShort;
        private readonly Color _colorBarBlack1;
        private readonly Color _colorBarBlack2;
        private readonly Color _colorBarWhite1;
        private readonly Color _colorBarWhite2;
        private readonly Color _colorClosedTrade1;
        private readonly Color _colorClosedTrade2;
        private readonly Color _colorLongTrade1;
        private readonly Color _colorLongTrade2;
        private readonly Color _colorShortTrade1;
        private readonly Color _colorShortTrade2;
        private readonly Font _font;
        private readonly Font _fontDi; // Font for Dynamic info
        private readonly Font _fontDiInd; // Font for Dynamic info Indicators
        private readonly Pen _penBarBorder;
        private readonly Pen _penBarThick;
        private readonly Pen _penCross;
        private readonly Pen _penGrid;
        private readonly Pen _penGridSolid;
        private readonly Pen _penTradeClose;
        private readonly Pen _penTradeLong;
        private readonly Pen _penTradeShort;
        private readonly Pen _penVolume;
        private readonly int _spcBottom; // pnlPrice bottom margin
        private readonly int _spcLeft; // pnlPrice left margin
        private readonly int _spcRight; // pnlPrice right margin
        private readonly int _spcTop; // pnlPrice top margin
        private int[] _aiInfoType; // 0 - text; 1 - Indicator; 
        private string[] _asInfoTitle;
        private string[] _asInfoValue;
        private int _barOld;

        private int _barPixels = 9;
        private int _chartBars;
        private ChartData _chartData;
        private string _chartTitle;
        private int _chartWidth;
        private int _countLabels; // The count of price labels on the vertical axe.
        private double _deltaGrid; // The distance between two vertical label in price.
        private int _dynInfoScrollValue;
        private int _dynInfoWidth;
        private int _firstBar;
        private int _indPanels;
        private int _infoRows;
        private bool _isCandleChart = true;
        private bool _isChartAutoScroll;
        private bool _isChartShift;
        private bool _isCrossShown;
        private bool _isCtrlKeyPressed;
        private bool _isDebug;
        private bool _isDrawDinInfo;
        private bool _isGridShown;
        private bool _isInfoPanelShown;
        private bool _isMouseInIndicatorChart;
        private bool _isMouseInPriceChart;
        private bool _isOrdersShown;
        private bool _isPositionLotsShown;
        private bool _isPositionPriceShown;
        private bool _isTrueChartsShown;
        private bool _isVolumeShown;
        private int _lastBar;
        private double _maxPrice;

        private int _maxVolume; // Max Volume in the chart
        private double _minPrice;
        private int _mouseX;
        private int _mouseXOld;
        private int _mouseY;
        private int _mouseYOld;
        private bool[] _repeatedIndicator;
        private double _scaleYVol; // The scale for drawing the Volume

        private double[] _sepChartMaxValue;
        private double[] _sepChartMinValue;

        private Size _szDate;
        private Size _szDateL;
        private Size _szPrice;
        private Size _szSL;
        private int _verticalScale = 1;
        private int _xDynInfoCol2;
        private int _xLeft; // pnlPrice left coordinate
        private int _xRight; // pnlPrice right coordinate
        private int _yBottom; // pnlPrice bottom coordinate
        private int _yBottomText; // pnlPrice bottom coordinate for date text
        private double _yScale;
        private int _yTop; // pnlPrice top coordinate

// ------------------------------------------------------------
        /// <summary>
        /// The default constructor.
        /// </summary>
        public Chart(ChartData chartData)
        {
            _chartData = chartData;

            BackColor = LayoutColors.ColorFormBack;
            Padding = new Padding(0);

            PnlCharts = new Panel {Parent = this, Dock = DockStyle.Fill};

            PnlInfo = new Panel {Parent = this, BackColor = LayoutColors.ColorControlBack, Dock = DockStyle.Right};
            PnlInfo.Paint += PnlInfoPaint;

            _barPixels = Configs.ChartZoom;
            _isGridShown = Configs.ChartGrid;
            _isCrossShown = Configs.ChartCross;
            _isVolumeShown = Configs.ChartVolume;
            _isPositionLotsShown = Configs.ChartLots;
            _isOrdersShown = Configs.ChartOrders;
            _isPositionPriceShown = Configs.ChartPositionPrice;
            _isInfoPanelShown = Configs.ChartInfoPanel;
            _isTrueChartsShown = Configs.ChartTrueCharts;
            _isChartShift = Configs.ChartShift;
            _isChartAutoScroll = Configs.ChartAutoScroll;

            _dynInfoScrollValue = 0;

            _font = new Font(Font.FontFamily, 8);

            // Dynamic info fonts
            _fontDi = new Font(Font.FontFamily, 9);
            _fontDiInd = new Font(Font.FontFamily, 10);

            Graphics g = CreateGraphics();
            _szDate = g.MeasureString("99/99 99:99", _font).ToSize();
            _szDateL = g.MeasureString("99/99/99 99:99", _font).ToSize();
            // TODO checking exact price with.
            _szPrice = g.MeasureString("9.99999", _font).ToSize();
            _szSL = g.MeasureString(" SL", _font).ToSize();
            g.Dispose();

            SetupDynInfoWidth();
            SetupIndicatorPanels();
            SetupButtons();
            SetupDynamicInfo();
            SetupChartTitle();

            PnlInfo.Visible = _isInfoPanelShown;
            PnlCharts.Padding = _isInfoPanelShown ? new Padding(0, 0, 2, 0) : new Padding(0);

            PnlCharts.Resize += PnlChartsResize;
            PnlPrice.Resize += PnlPriceResize;

            _spcTop = _font.Height;
            _spcBottom = _font.Height*8/5;
            _spcLeft = 0;
            _spcRight = _szPrice.Width + _szSL.Width + 2;

            _brushBack = new SolidBrush(LayoutColors.ColorChartBack);
            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _brushLabelBkgrd = new SolidBrush(LayoutColors.ColorLabelBack);
            _brushLabelFore = new SolidBrush(LayoutColors.ColorLabelText);
            _brushDynamicInfo = new SolidBrush(LayoutColors.ColorControlText);
            _brushDiIndicator = new SolidBrush(LayoutColors.ColorSlotIndicatorText);
            _brushEvenRows = new SolidBrush(LayoutColors.ColorEvenRowBack);
            _brushTradeLong = new SolidBrush(LayoutColors.ColorTradeLong);
            _brushTradeShort = new SolidBrush(LayoutColors.ColorTradeShort);
            _brushTradeClose = new SolidBrush(LayoutColors.ColorTradeClose);

            _penGrid = new Pen(LayoutColors.ColorChartGrid);
            _penGridSolid = new Pen(LayoutColors.ColorChartGrid);
            _penCross = new Pen(LayoutColors.ColorChartCross);
            _penVolume = new Pen(LayoutColors.ColorVolume);
            _penBarBorder = new Pen(LayoutColors.ColorBarBorder);
            _penBarThick = new Pen(LayoutColors.ColorBarBorder, 3);
            _penTradeLong = new Pen(LayoutColors.ColorTradeLong);
            _penTradeShort = new Pen(LayoutColors.ColorTradeShort);
            _penTradeClose = new Pen(LayoutColors.ColorTradeClose);

            _penGrid.DashStyle = DashStyle.Dash;
            _penGrid.DashPattern = new float[] {4, 2};

            _colorBarWhite1 = Data.ColorChanage(LayoutColors.ColorBarWhite, 30);
            _colorBarWhite2 = Data.ColorChanage(LayoutColors.ColorBarWhite, -30);
            _colorBarBlack1 = Data.ColorChanage(LayoutColors.ColorBarBlack, 30);
            _colorBarBlack2 = Data.ColorChanage(LayoutColors.ColorBarBlack, -30);

            _colorLongTrade1 = Data.ColorChanage(LayoutColors.ColorTradeLong, 30);
            _colorLongTrade2 = Data.ColorChanage(LayoutColors.ColorTradeLong, -30);
            _colorShortTrade1 = Data.ColorChanage(LayoutColors.ColorTradeShort, 30);
            _colorShortTrade2 = Data.ColorChanage(LayoutColors.ColorTradeShort, -30);
            _colorClosedTrade1 = Data.ColorChanage(LayoutColors.ColorTradeClose, 30);
            _colorClosedTrade2 = Data.ColorChanage(LayoutColors.ColorTradeClose, -30);
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
        /// Performs post initialization settings.
        /// </summary>
        public void InitChart(ChartData chartData)
        {
            _chartData = chartData;
            ScrollBar.Select();
        }

        /// <summary>
        /// Updates the chart after a tick.
        /// </summary>
        public void UpdateChartOnTick(bool repaintChart, ChartData chartData)
        {
            _chartData = chartData;

            if (repaintChart)
                SetupChartTitle();

            bool updateWholeChart = repaintChart;
            double oldMaxPrice = _maxPrice;
            double oldMinPrice = _minPrice;

            if (_isChartAutoScroll || repaintChart)
                SetFirstLastBar();

            SetPriceChartMinMaxValues();

            if (Math.Abs(_maxPrice - oldMaxPrice) > chartData.InstrumentProperties.Point ||
                Math.Abs(_minPrice - oldMinPrice) > chartData.InstrumentProperties.Point)
                updateWholeChart = true;

            if (updateWholeChart)
            {
                PnlPrice.Invalidate();
            }
            else
            {
                int left = _spcLeft + (_chartBars - 2)*_barPixels;
                int width = PnlPrice.ClientSize.Width - left;
                var rect = new Rectangle(left, 0, width, _yBottom + 1);
                PnlPrice.Invalidate(rect);
            }

            for (int i = 0; i < PnlInd.Length; i++)
            {
                var slot = (int) PnlInd[i].Tag;
                oldMaxPrice = _sepChartMaxValue[slot];
                oldMinPrice = _sepChartMinValue[slot];
                SetSepChartsMinMaxValues(i);
                if (Math.Abs(_sepChartMaxValue[slot] - oldMaxPrice) > 0.000001 ||
                    Math.Abs(_sepChartMinValue[slot] - oldMinPrice) > 0.000001)
                    updateWholeChart = true;

                if (updateWholeChart)
                {
                    PnlInd[i].Invalidate();
                }
                else
                {
                    int left = _spcLeft + (_chartBars - 2)*_barPixels;
                    int width = PnlInd[i].ClientSize.Width - left;
                    var rect = new Rectangle(left, 0, width, _yBottom + 1);
                    PnlInd[i].Invalidate(rect);
                }
            }

            if (_isInfoPanelShown && !_isCrossShown)
                GenerateDynamicInfo(_lastBar);
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            _isCtrlKeyPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        /// Create and sets the indicator panels.
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

            _sepChartMinValue = new double[_chartData.Strategy.Slots];
            _sepChartMaxValue = new double[_chartData.Strategy.Slots];

            // Indicator panels
            _indPanels = 0;
            var asIndicatorTexts = new string[_chartData.Strategy.Slots];
            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                var slotType = _chartData.Strategy.Slot[slot].SlotType;
                Indicator indicator = IndicatorManager.ConstructIndicator(_chartData.Strategy.Slot[slot].IndicatorName);
                indicator.Initialize(slotType);
                indicator.IndParam = _chartData.Strategy.Slot[slot].IndParam;
                asIndicatorTexts[slot] = indicator.ToString();
                _indPanels += _chartData.Strategy.Slot[slot].SeparatedChart ? 1 : 0;
            }

            // Repeated indicators
            _repeatedIndicator = new bool[_chartData.Strategy.Slots];
            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                _repeatedIndicator[slot] = false;
                for (int i = 0; i < slot; i++)
                    _repeatedIndicator[slot] = asIndicatorTexts[slot] == asIndicatorTexts[i];
            }

            PnlInd = new Panel[_indPanels];
            SplitterInd = new Splitter[_indPanels];
            for (int i = 0; i < _indPanels; i++)
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
            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                if (!_chartData.Strategy.Slot[slot].SeparatedChart) continue;
                PnlInd[index].Tag = slot; // The real tag.
                index++;
            }

            ScrollBar = new HScrollBar {Parent = PnlCharts, Dock = DockStyle.Bottom, TabStop = true, SmallChange = 1};
            ScrollBar.ValueChanged += ScrollValueChanged;
            ScrollBar.MouseWheel += ScrollMouseWheel;
            ScrollBar.KeyUp += ScrollKeyUp;
            ScrollBar.KeyDown += ScrollKeyDown;

            for (int i = 0; i < _indPanels; i++)
                PnlInd[i].Resize += PnlIndResize;
        }

        /// <summary>
        /// Sets up the chart's buttons.
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
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].Visible = _isInfoPanelShown;

            // Move Dynamic Info Up
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Image = Resources.chart_dinfo_up;
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].ToolTipText = Language.T("Move info up") +
                                                                                         "   A";
            ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Visible = _isInfoPanelShown;
        }

        /// <summary>
        /// Sets the chart's parameters.
        /// </summary>
        private void SetFirstLastBar()
        {
            ScrollBar.Minimum = _chartData.FirstBar;
            ScrollBar.Maximum = _chartData.Bars - 1;

            int shift = _isChartShift ? ChartRightShift : 0;
            _chartBars = (_chartWidth - shift - 7)/_barPixels;
            _chartBars = Math.Min(_chartBars, _chartData.Bars - _chartData.FirstBar);
            _firstBar = Math.Max(_chartData.FirstBar, _chartData.Bars - _chartBars);
            _firstBar = Math.Min(_firstBar, _chartData.Bars - 1);
            _lastBar = Math.Max(_firstBar + _chartBars - 1, _firstBar);

            ScrollBar.Value = _firstBar;
            ScrollBar.LargeChange = Math.Max(_chartBars, 1);
        }

        /// <summary>
        /// Sets the min and the max values of price shown on the chart.
        /// </summary>
        private void SetPriceChartMinMaxValues()
        {
            // Searching the min and the max price and volume
            _maxPrice = double.MinValue;
            _minPrice = double.MaxValue;
            _maxVolume = int.MinValue;
            double spread = _chartData.InstrumentProperties.Spread*_chartData.InstrumentProperties.Point;
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                if (_chartData.High[bar] + spread > _maxPrice) _maxPrice = _chartData.High[bar] + spread;
                if (_chartData.Low[bar] < _minPrice) _minPrice = _chartData.Low[bar];
                if (_chartData.Volume[bar] > _maxVolume) _maxVolume = _chartData.Volume[bar];
            }

            double pricePixel = (_maxPrice - _minPrice)/(_yBottom - _yTop);
            if (_isVolumeShown)
                _minPrice -= pricePixel*30;
            else if (_isPositionLotsShown)
                _minPrice -= pricePixel*10;

            _maxPrice += pricePixel*_verticalScale;
            _minPrice -= pricePixel*_verticalScale;

            // Grid
            double deltaPoint = (_chartData.InstrumentProperties.Digits == 5 ||
                                 _chartData.InstrumentProperties.Digits == 3)
                                    ? _chartData.InstrumentProperties.Point*100
                                    : _chartData.InstrumentProperties.Point*10;
            int roundStep = Math.Max(_chartData.InstrumentProperties.Digits - 1, 1);
            _countLabels = Math.Max((_yBottom - _yTop)/35, 1);
            _deltaGrid = Math.Max(Math.Round((_maxPrice - _minPrice)/_countLabels, roundStep), deltaPoint);
            _minPrice = Math.Round(_minPrice, roundStep) - deltaPoint;
            _countLabels = (int) Math.Ceiling((_maxPrice - _minPrice)/_deltaGrid);
            _maxPrice = _minPrice + _countLabels*_deltaGrid;
            _yScale = (_yBottom - _yTop)/(_countLabels*_deltaGrid);
            _scaleYVol = _maxVolume > 0 ? 40.0/_maxVolume : 0; // 40 - the highest volume line
        }

        /// <summary>
        /// Sets parameter of separated charts
        /// </summary>
        private void SetSepChartsMinMaxValues(int index)
        {
            Panel panel = PnlInd[index];
            var slot = (int) panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in _chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(_firstBar, component.FirstBar); bar <= _lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, _chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, _chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in _chartData.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value) < 0.00001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            _sepChartMaxValue[slot] = maxValue;
            _sepChartMinValue[slot] = minValue;
        }

        /// <summary>
        /// Sets the indicator chart title
        /// </summary>
        private void SetupChartTitle()
        {
            _chartTitle = _chartData.StrategyName + " " + _chartData.Symbol + " " + _chartData.PeriodStr + " (" +
                          _chartData.Bars + " bars)";

            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                if (_chartData.Strategy.Slot[slot].SeparatedChart) continue;

                bool isChart = false;
                foreach (IndicatorComp component in _chartData.Strategy.Slot[slot].Component)
                    if (component.ChartType != IndChartType.NoChart)
                    {
                        isChart = true;
                        break;
                    }

                if (isChart)
                {
                    Indicator indicator = IndicatorManager.ConstructIndicator(_chartData.Strategy.Slot[slot].IndicatorName);
                    indicator.Initialize(_chartData.Strategy.Slot[slot].SlotType);
                    indicator.IndParam = _chartData.Strategy.Slot[slot].IndParam;
                    if (!_chartTitle.Contains(indicator.ToString()))
                        _chartTitle += Environment.NewLine + indicator;
                }
            }
        }

        /// <summary>
        /// Sets the sizes of the panels after resizing.
        /// </summary>
        private void PnlChartsResize(object sender, EventArgs e)
        {
            SetAllPanelsHeight();
            SetFirstLastBar();
            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            GenerateDynamicInfo(_lastBar);
            _dynInfoScrollValue = 0;
        }

        /// <summary>
        /// Calculates the panels' height
        /// </summary>
        private void SetAllPanelsHeight()
        {
            int availableHeight = PnlCharts.ClientSize.Height - StripButtons.Height - ScrollBar.Height - _indPanels*2;
            int pnlIndHeight = availableHeight/(2 + _indPanels);

            foreach (Panel panel in PnlInd)
                panel.Height = pnlIndHeight;
        }

        /// <summary>
        /// Sets the parameters after resizing of the PnlPrice.
        /// </summary>
        private void PnlPriceResize(object sender, EventArgs e)
        {
            _xLeft = _spcLeft;
            _xRight = PnlPrice.ClientSize.Width - _spcRight;
            _chartWidth = Math.Max(_xRight - _xLeft, 0);
            _yTop = _spcTop;
            _yBottom = PnlPrice.ClientSize.Height - _spcBottom;
            _yBottomText = PnlPrice.ClientSize.Height - _spcBottom*5/8 - 4;

            SetPriceChartMinMaxValues();

            PnlPrice.Invalidate();
        }

        /// <summary>
        /// Invalidates the panels
        /// </summary>
        private void PnlIndResize(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            var slot = (int) panel.Tag;
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            foreach (IndicatorComp component in _chartData.Strategy.Slot[slot].Component)
                if (component.ChartType != IndChartType.NoChart)
                    for (int bar = Math.Max(_firstBar, component.FirstBar); bar <= _lastBar; bar++)
                    {
                        double value = component.Value[bar];
                        if (value > maxValue) maxValue = value;
                        if (value < minValue) minValue = value;
                    }

            minValue = Math.Min(minValue, _chartData.Strategy.Slot[slot].MinValue);
            maxValue = Math.Max(maxValue, _chartData.Strategy.Slot[slot].MaxValue);

            foreach (double value in _chartData.Strategy.Slot[slot].SpecValue)
                if (Math.Abs(value) < 0.0001)
                {
                    minValue = Math.Min(minValue, 0);
                    maxValue = Math.Max(maxValue, 0);
                }

            _sepChartMaxValue[slot] = maxValue;
            _sepChartMinValue[slot] = minValue;

            panel.Invalidate();
        }

        /// <summary>
        /// Paints the panel PnlPrice
        /// </summary>
        private void PnlPricePaint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            try
            {
                g.Clear(LayoutColors.ColorChartBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (_chartBars == 0) return;

            // Grid
            for (double label = _minPrice;
                 label <= _maxPrice + _chartData.InstrumentProperties.Point;
                 label += _deltaGrid)
            {
                var labelY = (int) (_yBottom - (label - _minPrice)*_yScale);
                g.DrawString(label.ToString(Data.FF), _font, _brushFore, _xRight, labelY - Font.Height/2 - 1);
                if (_isGridShown || Math.Abs(label - _minPrice) < 0.00001)
                    g.DrawLine(_penGrid, _spcLeft, labelY, _xRight, labelY);
                else
                    g.DrawLine(_penGrid, _xRight - 5, labelY, _xRight, labelY);
            }
            for (int vertLineBar = _lastBar;
                 vertLineBar > _firstBar;
                 vertLineBar -= (_szDate.Width + 10)/_barPixels + 1)
            {
                int xVertLine = (vertLineBar - _firstBar)*_barPixels + _spcLeft + _barPixels/2 - 1;
                if (_isGridShown)
                    g.DrawLine(_penGrid, xVertLine, _yTop, xVertLine, _yBottom + 2);
                string date = String.Format("{0} {1}", _chartData.Time[vertLineBar].ToString(Data.DFS),
                                            _chartData.Time[vertLineBar].ToString("HH:mm"));
                g.DrawString(date, _font, _brushFore, xVertLine - _szDate.Width/2, _yBottomText);
            }

            // Draws Volume, Lots and Bars
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                int x = (bar - _firstBar)*_barPixels + _spcLeft;
                int xCenter = x + (_barPixels - 1)/2 - 1;
                var yOpen = (int) (_yBottom - (_chartData.Open[bar] - _minPrice)*_yScale);
                var yHigh = (int) (_yBottom - (_chartData.High[bar] - _minPrice)*_yScale);
                var yLow = (int) (_yBottom - (_chartData.Low[bar] - _minPrice)*_yScale);
                var yClose = (int) (_yBottom - (_chartData.Close[bar] - _minPrice)*_yScale);
                var yVolume = (int) (_yBottom - _chartData.Volume[bar]*_scaleYVol);

                // Draw the volume
                if (_isVolumeShown && yVolume != _yBottom)
                    g.DrawLine(_penVolume, x + _barPixels/2 - 1, yVolume, x + _barPixels/2 - 1, _yBottom);

                // Draw position's lots
                if (_isPositionLotsShown && _chartData.BarStatistics.ContainsKey(_chartData.Time[bar]))
                {
                    PosDirection dir = _chartData.BarStatistics[_chartData.Time[bar]].PositionDir;
                    if (dir != PosDirection.None)
                    {
                        double lots = _chartData.BarStatistics[_chartData.Time[bar]].PositionLots;
                        var iPosHight = (int) (Math.Max(lots*3, 2));
                        int iPosY = _yBottom - iPosHight + 1;
                        var rect = new Rectangle(x - 1, iPosY, _barPixels, iPosHight);
                        LinearGradientBrush lgBrush;
                        if (dir == PosDirection.Long)
                            lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                        else if (dir == PosDirection.Short)
                            lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                        else
                            lgBrush = new LinearGradientBrush(rect, _colorClosedTrade1, _colorClosedTrade2, 0f);
                        rect = new Rectangle(x, iPosY, _barPixels - 2, iPosHight);
                        g.FillRectangle(lgBrush, rect);
                    }
                }

                // Draw the bar
                if (_isCandleChart)
                {
                    g.DrawLine(_barPixels < 29 ? _penBarBorder : _penBarThick, xCenter, yLow, xCenter, yHigh);

                    if (yClose < yOpen)
                    {
                        // White bar
                        var rect = new Rectangle(x + 1, yClose, _barPixels - 5, yOpen - yClose);
                        var lgBrush = new LinearGradientBrush(rect, _colorBarWhite1, _colorBarWhite2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(_penBarBorder, rect);
                    }
                    else if (yClose > yOpen)
                    {
                        // Black bar
                        var rect = new Rectangle(x + 1, yOpen, _barPixels - 5, yClose - yOpen);
                        var lgBrush = new LinearGradientBrush(rect, _colorBarBlack1, _colorBarBlack2, 5f);
                        g.FillRectangle(lgBrush, rect);
                        g.DrawRectangle(_penBarBorder, rect);
                    }
                    else
                    {
                        // Cross
                        g.DrawLine(_barPixels < 29 ? _penBarBorder : _penBarThick, x + 1, yClose, x + _barPixels - 4,
                                   yClose);
                    }
                }
                else
                {
                    if (_barPixels <= 16)
                    {
                        g.DrawLine(_penBarBorder, xCenter, yLow, xCenter, yHigh);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(_penBarBorder, x, yOpen, xCenter, yOpen);
                            g.DrawLine(_penBarBorder, xCenter, yClose, x + _barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(_penBarBorder, x, yClose, x + _barPixels - 3, yClose);
                        }
                    }
                    else
                    {
                        g.DrawLine(_penBarThick, xCenter, yLow + 2, xCenter, yHigh - 1);
                        if (yClose != yOpen)
                        {
                            g.DrawLine(_penBarThick, x + 1, yOpen, xCenter, yOpen);
                            g.DrawLine(_penBarThick, xCenter - 1, yClose, x + _barPixels - 3, yClose);
                        }
                        else
                        {
                            g.DrawLine(_penBarThick, x, yClose, x + _barPixels - 3, yClose);
                        }
                    }
                }
            }

            // Drawing the indicators in the chart
            g.SetClip(new RectangleF(_spcLeft, _yTop, _xRight, _yBottom - _yTop));
            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                if (_chartData.Strategy.Slot[slot].SeparatedChart || _repeatedIndicator[slot]) continue;

                int cloudUp = -1; // For Ichimoku and similar
                int cloudDown = -1; // For Ichimoku and similar

                bool isIndicatorValueAtClose = true;
                int indicatorValueShift = 1;
                foreach (ListParam listParam in _chartData.Strategy.Slot[slot].IndParam.ListParam)
                    if (listParam.Caption == "Base price" && listParam.Text == "Open")
                    {
                        isIndicatorValueAtClose = false;
                        indicatorValueShift = 0;
                    }

                for (int comp = 0; comp < _chartData.Strategy.Slot[slot].Component.Length; comp++)
                {
                    var pen = new Pen(_chartData.Strategy.Slot[slot].Component[comp].ChartColor);
                    var penTC = new Pen(_chartData.Strategy.Slot[slot].Component[comp].ChartColor)
                                    {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                    if (_chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Line)
                    {
                        // Line
                        if (_isTrueChartsShown)
                        {
                            // True Charts
                            var point = new Point[_lastBar - _firstBar + 1];
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = _chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = _spcLeft + (bar - _firstBar)*_barPixels + 1 +
                                        indicatorValueShift*(_barPixels - 5);
                                var y = (int) Math.Round(_yBottom - (value - _minPrice)*_yScale);

                                if (Math.Abs(value) < 0.0001)
                                    point[bar - _firstBar] = point[Math.Max(bar - _firstBar - 1, 0)];
                                else
                                    point[bar - _firstBar] = new Point(x, y);
                            }

                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                // All bars except the last one
                                int i = bar - _firstBar;

                                // The indicator value point
                                g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                                g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                                if (bar == _firstBar && isIndicatorValueAtClose)
                                {
                                    // First bar
                                    double value = _chartData.Strategy.Slot[slot].Component[comp].Value[bar - 1];
                                    int x = _spcLeft + (bar - _firstBar)*_barPixels;
                                    var y = (int) Math.Round(_yBottom - (value - _minPrice)*_yScale);

                                    int deltaY = Math.Abs(y - point[i].Y);
                                    if (_barPixels > 3)
                                    {
                                        // Horizontal part
                                        if (deltaY == 0)
                                            g.DrawLine(pen, x + 1, y, x + _barPixels - 7, y);
                                        else if (deltaY < 3)
                                            g.DrawLine(pen, x + 1, y, x + _barPixels - 6, y);
                                        else
                                            g.DrawLine(pen, x + 1, y, x + _barPixels - 4, y);
                                    }
                                    if (deltaY > 4)
                                    {
                                        // Vertical part
                                        if (point[i].Y > y)
                                            g.DrawLine(penTC, x + _barPixels - 4, y + 2, x + _barPixels - 4,
                                                       point[i].Y - 2);
                                        else
                                            g.DrawLine(penTC, x + _barPixels - 4, y - 2, x + _barPixels - 4,
                                                       point[i].Y + 2);
                                    }
                                }

                                if (bar < _lastBar)
                                {
                                    int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);

                                    if (_barPixels > 3)
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
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                       point[i + 1].Y - 2);
                                        else
                                            g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                       point[i + 1].Y + 2);
                                    }
                                }

                                if (bar == _lastBar && !isIndicatorValueAtClose && _barPixels > 3)
                                {
                                    // Last bar
                                    g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + _barPixels - 5, point[i].Y);
                                }
                            }
                        }
                        else
                        {
                            var aPoint = new Point[_lastBar - _firstBar + 1];
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double dValue = _chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                                int x = (bar - _firstBar)*_barPixels + _barPixels/2 - 1 + _spcLeft;
                                var y = (int) (_yBottom - (dValue - _minPrice)*_yScale);

                                if (Math.Abs(dValue) < 0.0001)
                                    aPoint[bar - _firstBar] = aPoint[Math.Max(bar - _firstBar - 1, 0)];
                                else
                                    aPoint[bar - _firstBar] = new Point(x, y);
                            }
                            g.DrawLines(pen, aPoint);
                        }
                    }
                    else if (_chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Dot)
                    {
                        // Dots
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double dValue = _chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - _firstBar)*_barPixels + _barPixels/2 - 1 + _spcLeft;
                            var y = (int) (_yBottom - (dValue - _minPrice)*_yScale);
                            if (_barPixels == 2)
                                g.FillRectangle(pen.Brush, x, y, 1, 1);
                            else
                            {
                                g.DrawLine(pen, x - 1, y, x + 1, y);
                                g.DrawLine(pen, x, y - 1, x, y + 1);
                            }
                        }
                    }
                    else if (_chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.Level)
                    {
                        // Level
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double dValue = _chartData.Strategy.Slot[slot].Component[comp].Value[bar];
                            int x = (bar - _firstBar)*_barPixels + _spcLeft;
                            var y = (int) (_yBottom - (dValue - _minPrice)*_yScale);
                            g.DrawLine(pen, x, y, x + _barPixels - 1, y);
                        }
                    }
                    else if (_chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudUp)
                    {
                        cloudUp = comp;
                    }
                    else if (_chartData.Strategy.Slot[slot].Component[comp].ChartType == IndChartType.CloudDown)
                    {
                        cloudDown = comp;
                    }
                }

                // Clouds
                if (cloudUp >= 0 && cloudDown >= 0)
                {
                    var apntUp = new PointF[_lastBar - _firstBar + 1];
                    var apntDown = new PointF[_lastBar - _firstBar + 1];
                    for (int bar = _firstBar; bar <= _lastBar; bar++)
                    {
                        double dValueUp = _chartData.Strategy.Slot[slot].Component[cloudUp].Value[bar];
                        double dValueDown = _chartData.Strategy.Slot[slot].Component[cloudDown].Value[bar];
                        apntUp[bar - _firstBar].X = (bar - _firstBar)*_barPixels + _barPixels/2 - 1 + _spcLeft;
                        apntUp[bar - _firstBar].Y = (int) (_yBottom - (dValueUp - _minPrice)*_yScale);
                        apntDown[bar - _firstBar].X = (bar - _firstBar)*_barPixels + _barPixels/2 - 1 + _spcLeft;
                        apntDown[bar - _firstBar].Y = (int) (_yBottom - (dValueDown - _minPrice)*_yScale);
                    }

                    var pathUp = new GraphicsPath();
                    pathUp.AddLine(new PointF(apntUp[0].X, 0), apntUp[0]);
                    pathUp.AddLines(apntUp);
                    pathUp.AddLine(apntUp[_lastBar - _firstBar], new PointF(apntUp[_lastBar - _firstBar].X, 0));
                    pathUp.AddLine(new PointF(apntUp[_lastBar - _firstBar].X, 0), new PointF(apntUp[0].X, 0));

                    var pathDown = new GraphicsPath();
                    pathDown.AddLine(new PointF(apntDown[0].X, 0), apntDown[0]);
                    pathDown.AddLines(apntDown);
                    pathDown.AddLine(apntDown[_lastBar - _firstBar], new PointF(apntDown[_lastBar - _firstBar].X, 0));
                    pathDown.AddLine(new PointF(apntDown[_lastBar - _firstBar].X, 0), new PointF(apntDown[0].X, 0));

                    Color colorUp = Color.FromArgb(50, _chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    Color colorDown = Color.FromArgb(50, _chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

                    var penUp = new Pen(_chartData.Strategy.Slot[slot].Component[cloudUp].ChartColor);
                    var penDown = new Pen(_chartData.Strategy.Slot[slot].Component[cloudDown].ChartColor);

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
            for (int bar = _firstBar; bar <= _lastBar; bar++)
            {
                DateTime bartime = _chartData.Time[bar];
                if (!_chartData.BarStatistics.ContainsKey(bartime))
                    continue;

                int x = (bar - _firstBar)*_barPixels + _spcLeft;

                // Draws the position's price
                if (_isPositionPriceShown)
                {
                    double price = _chartData.BarStatistics[bartime].PositionPrice;
                    if (price > _chartData.InstrumentProperties.Point)
                    {
                        var yPrice = (int) (_yBottom - (price - _minPrice)*_yScale);

                        if (_chartData.BarStatistics[bartime].PositionDir == PosDirection.Long)
                        {
                            // Long
                            g.DrawLine(_penTradeLong, x, yPrice, x + _barPixels - 2, yPrice);
                        }
                        else if (_chartData.BarStatistics[bartime].PositionDir == PosDirection.Short)
                        {
                            // Short
                            g.DrawLine(_penTradeShort, x, yPrice, x + _barPixels - 2, yPrice);
                        }
                        else if (_chartData.BarStatistics[bartime].PositionDir == PosDirection.Closed)
                        {
                            // Closed
                            g.DrawLine(_penTradeClose, x, yPrice, x + _barPixels - 2, yPrice);
                        }
                    }
                }

                // Draw the deals
                if (_isOrdersShown)
                {
                    foreach (Operation operation in _chartData.BarStatistics[bartime].Operations)
                    {
                        var yOrder = (int) (_yBottom - (operation.OperationPrice - _minPrice)*_yScale);

                        if (operation.OperationType == OperationType.Buy)
                        {
                            // Buy
                            var pen = new Pen(_brushTradeLong, 2);
                            if (_barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + _barPixels - 1, yOrder);
                            }
                            else if (_barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 4, yOrder);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder - 3);
                            }
                            else if (_barPixels > 9)
                            {
                                int d = (_barPixels - 1)/2 - 1;
                                int x1 = x + d;
                                int x2 = x + _barPixels - 3;
                                g.DrawLine(pen, x, yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder - d);
                                g.DrawLine(pen, x2 + 1, yOrder - d + 1, x1 + d/2 + 1, yOrder - d + 1);
                                g.DrawLine(pen, x2, yOrder - d, x2, yOrder - d/2);
                            }
                        }
                        else if (operation.OperationType == OperationType.Sell)
                        {
                            // Sell
                            var pen = new Pen(_brushTradeShort, 2);
                            if (_barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + _barPixels - 1, yOrder);
                            }
                            else if (_barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder + 1, x + 4, yOrder + 1);
                                pen.EndCap = LineCap.DiamondAnchor;
                                g.DrawLine(pen, x + 2, yOrder, x + 5, yOrder + 3);
                            }
                            else if (_barPixels > 9)
                            {
                                int d = (_barPixels - 1)/2 - 1;
                                int x1 = x + d;
                                int x2 = x + _barPixels - 3;
                                g.DrawLine(pen, x, yOrder + 1, x1 + 1, yOrder + 1);
                                g.DrawLine(pen, x1, yOrder, x2, yOrder + d);
                                g.DrawLine(pen, x1 + d/2 + 1, yOrder + d, x2, yOrder + d);
                                g.DrawLine(pen, x2, yOrder + d, x2, yOrder + d/2 + 1);
                            }
                        }
                        else if (operation.OperationType == OperationType.Close)
                        {
                            // Close
                            var pen = new Pen(_brushTradeClose, 2);
                            if (_barPixels < 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + _barPixels - 1, yOrder);
                            }
                            else if (_barPixels == 9)
                            {
                                g.DrawLine(pen, x, yOrder, x + 7, yOrder);
                                g.DrawLine(pen, x + 5, yOrder - 2, x + 5, yOrder + 2);
                            }
                            else if (_barPixels > 9)
                            {
                                int d = (_barPixels - 1)/2 - 1;
                                int x1 = x + d + 1;
                                int x2 = x + _barPixels - 3;
                                g.DrawLine(pen, x, yOrder, x1, yOrder);
                                g.DrawLine(pen, x1, yOrder + d/2, x2, yOrder - d/2);
                                g.DrawLine(pen, x1, yOrder - d/2, x2, yOrder + d/2);
                            }
                        }
                    }
                }
            }

            // Bid price label.
            var yBid = (int) (_yBottom - (_chartData.Bid - _minPrice)*_yScale);
            var pBid = new Point(_xRight, yBid - _szPrice.Height/2);
            string sBid = (_chartData.Bid.ToString(Data.FF));
            int xBidRight = _xRight + _szPrice.Width + 1;
            var apBid = new[]
                            {
                                new PointF(_xRight - 6, yBid),
                                new PointF(_xRight, yBid - _szPrice.Height/2),
                                new PointF(xBidRight, yBid - _szPrice.Height/2 - 1),
                                new PointF(xBidRight, yBid + _szPrice.Height/2 + 1),
                                new PointF(_xRight, yBid + _szPrice.Height/2)
                            };

            // Position price.
            if (_isPositionPriceShown &&
                (_chartData.PositionDirection == PosDirection.Long || _chartData.PositionDirection == PosDirection.Short))
            {
                var yPos = (int) (_yBottom - (_chartData.PositionOpenPrice - _minPrice)*_yScale);
                var pPos = new Point(_xRight, yPos - _szPrice.Height/2);
                string sPos = (_chartData.PositionOpenPrice.ToString(Data.FF));
                var brushText = new SolidBrush(LayoutColors.ColorChartBack);

                if (_chartData.PositionOpenPrice > _minPrice && _chartData.PositionOpenPrice < _maxPrice)
                {
                    var penPos = new Pen(LayoutColors.ColorTradeLong);
                    if (_chartData.PositionDirection == PosDirection.Short)
                        penPos = new Pen(LayoutColors.ColorTradeShort);
                    var apPos = new[]
                                    {
                                        new PointF(_xRight - 6, yPos),
                                        new PointF(_xRight, yPos - _szPrice.Height/2),
                                        new PointF(_xRight + _szPrice.Width, yPos - _szPrice.Height/2),
                                        new PointF(_xRight + _szPrice.Width, yPos + _szPrice.Height/2),
                                        new PointF(_xRight, yPos + _szPrice.Height/2),
                                        new PointF(_xRight - 6, yPos)
                                    };
                    g.FillPolygon(_brushBack, apPos);
                    g.DrawString(sPos, _font, _brushFore, pPos);
                    g.DrawLines(penPos, apPos);
                }

                // Profit Arrow
                Pen penProfit = _chartData.PositionProfit > 0
                                    ? new Pen(LayoutColors.ColorTradeLong, 7)
                                    : new Pen(LayoutColors.ColorTradeShort, 7);
                penProfit.EndCap = LineCap.ArrowAnchor;
                g.DrawLine(penProfit, xBidRight + 9, yPos, xBidRight + 9, yBid);

                // Close Price
                IndicatorSlot slot = _chartData.Strategy.Slot[_chartData.Strategy.CloseSlot];
                if (slot.IndParam.ExecutionTime != ExecutionTime.AtBarClosing)
                {
                    double dClosePrice = 0;
                    for (int iComp = 0; iComp < slot.Component.Length; iComp++)
                    {
                        IndComponentType compType = slot.Component[iComp].DataType;
                        if (_chartData.PositionDirection == PosDirection.Long &&
                            compType == IndComponentType.CloseLongPrice)
                            dClosePrice = slot.Component[iComp].Value[_chartData.Bars - 1];
                        else if (_chartData.PositionDirection == PosDirection.Short &&
                                 compType == IndComponentType.CloseShortPrice)
                            dClosePrice = slot.Component[iComp].Value[_chartData.Bars - 1];
                        else if (compType == IndComponentType.ClosePrice ||
                                 compType == IndComponentType.OpenClosePrice)
                            dClosePrice = slot.Component[iComp].Value[_chartData.Bars - 1];
                    }
                    if (dClosePrice > _minPrice && dClosePrice < _maxPrice)
                    {
                        var yClose = (int) (_yBottom - (dClosePrice - _minPrice)*_yScale);
                        var pClose = new Point(_xRight, yClose - _szPrice.Height/2);
                        string sClose = (dClosePrice.ToString(Data.FF) + " X");
                        var apClose = new[]
                                          {
                                              new PointF(_xRight - 6, yClose),
                                              new PointF(_xRight, yClose - _szPrice.Height/2),
                                              new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                         yClose - _szPrice.Height/2 - 1),
                                              new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                         yClose + _szPrice.Height/2 + 1),
                                              new PointF(_xRight, yClose + _szPrice.Height/2)
                                          };
                        g.FillPolygon(new SolidBrush(LayoutColors.ColorTradeClose), apClose);
                        g.DrawString(sClose, _font, brushText, pClose);
                    }

                    // Take Profit
                    if (_chartData.PositionTakeProfit > _minPrice && _chartData.PositionTakeProfit < _maxPrice)
                    {
                        var yLimit = (int) (_yBottom - (_chartData.PositionTakeProfit - _minPrice)*_yScale);
                        var pLimit = new Point(_xRight, yLimit - _szPrice.Height/2);
                        string sLimit = (_chartData.PositionTakeProfit.ToString(Data.FF) + " TP");
                        var apLimit = new[]
                                          {
                                              new PointF(_xRight - 6, yLimit),
                                              new PointF(_xRight, yLimit - _szPrice.Height/2),
                                              new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                         yLimit - _szPrice.Height/2 - 1),
                                              new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                         yLimit + _szPrice.Height/2 + 1),
                                              new PointF(_xRight, yLimit + _szPrice.Height/2)
                                          };
                        var brushTakeProffit = new SolidBrush(LayoutColors.ColorTradeLong);
                        g.FillPolygon(brushTakeProffit, apLimit);
                        g.DrawString(sLimit, _font, brushText, pLimit);
                    }

                    // Stop Loss
                    if (_chartData.PositionStopLoss > _minPrice && _chartData.PositionStopLoss < _maxPrice)
                    {
                        var yStop = (int) (_yBottom - (_chartData.PositionStopLoss - _minPrice)*_yScale);
                        var pStop = new Point(_xRight, yStop - _szPrice.Height/2);
                        string sStop = (_chartData.PositionStopLoss.ToString(Data.FF) + " SL");
                        var apStop = new[]
                                         {
                                             new PointF(_xRight - 6, yStop),
                                             new PointF(_xRight, yStop - _szPrice.Height/2),
                                             new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                        yStop - _szPrice.Height/2 - 1),
                                             new PointF(_xRight + _szPrice.Width + _szSL.Width - 2,
                                                        yStop + _szPrice.Height/2 + 1),
                                             new PointF(_xRight, yStop + _szPrice.Height/2)
                                         };
                        var brushStopLoss = new SolidBrush(LayoutColors.ColorTradeShort);
                        g.FillPolygon(brushStopLoss, apStop);
                        g.DrawString(sStop, _font, brushText, pStop);
                    }
                }
            }

            // Draws Bid price label.
            g.FillPolygon(_brushLabelBkgrd, apBid);
            g.DrawString(sBid, _font, _brushLabelFore, pBid);

            // Cross
            if (_isCrossShown && _mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
            {
                int bar = (_mouseX - _spcLeft)/_barPixels;
                bar = Math.Max(0, bar);
                bar = Math.Min(_chartBars - 1, bar);
                bar += _firstBar;
                bar = Math.Min(_chartData.Bars - 1, bar);

                // Vertical positions
                var point = new Point(_mouseX - _szDateL.Width/2, _yBottomText);
                var rec = new Rectangle(point, _szDateL);

                // Vertical line
                if (_isMouseInPriceChart && _mouseY > _yTop - 1 && _mouseY < _yBottom + 1)
                {
                    g.DrawLine(_penCross, _mouseX, _yTop, _mouseX, _mouseY - 10);
                    g.DrawLine(_penCross, _mouseX, _mouseY + 10, _mouseX, _yBottomText);
                }
                else if (_isMouseInPriceChart || _isMouseInIndicatorChart)
                {
                    g.DrawLine(_penCross, _mouseX, _yTop, _mouseX, _yBottomText);
                }

                // Date Window
                if (_isMouseInPriceChart || _isMouseInIndicatorChart)
                {
                    g.FillRectangle(_brushLabelBkgrd, rec);
                    g.DrawRectangle(_penCross, rec);
                    string sDate = _chartData.Time[bar].ToString(Data.DF) + " " + _chartData.Time[bar].ToString("HH:mm");
                    g.DrawString(sDate, _font, _brushLabelFore, point);
                }

                if (_isMouseInPriceChart && _mouseY > _yTop - 1 && _mouseY < _yBottom + 1)
                {
                    // Horizontal positions
                    point = new Point(_xRight, _mouseY - _szPrice.Height/2);
                    rec = new Rectangle(point, _szPrice);
                    // Horizontal line
                    g.DrawLine(_penCross, _xLeft, _mouseY, _mouseX - 10, _mouseY);
                    g.DrawLine(_penCross, _mouseX + 10, _mouseY, _xRight, _mouseY);
                    // Price Window
                    g.FillRectangle(_brushLabelBkgrd, rec);
                    g.DrawRectangle(_penCross, rec);
                    string sPrice = ((_yBottom - _mouseY)/_yScale + _minPrice).ToString(Data.FF);
                    g.DrawString(sPrice, _font, _brushLabelFore, point);
                }
            }

            // Chart title
            g.DrawString(_chartTitle, _font, _brushFore, _spcLeft, 0);
        }

        /// <summary>
        /// Paints the panel PnlInd
        /// </summary>
        private void PnlIndPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            try
            {
                g.Clear(LayoutColors.ColorChartBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            if (_chartBars == 0) return;

            int topSpace = _font.Height/2 + 2;
            int bottomSpace = _font.Height/2;

            var slot = (int) pnl.Tag;
            double minValue = _sepChartMinValue[slot];
            double maxValue = _sepChartMaxValue[slot];

            double scale = (pnl.ClientSize.Height - topSpace - bottomSpace)/(Math.Max(maxValue - minValue, 0.0001));

            // Grid
            String format;
            double deltaLabel;
            int xGridRight = pnl.ClientSize.Width - _spcRight + 2;

            // Zero line
            double label = 0;
            var labelYZero = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (label >= minValue && label <= maxValue)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), _font, _brushFore, _xRight, labelYZero - _font.Height/2 - 1);
                g.DrawLine(_penGridSolid, _spcLeft, labelYZero, xGridRight, labelYZero);
            }

            label = minValue;
            var labelYMin = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMin) >= _font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), _font, _brushFore, _xRight, labelYMin - _font.Height/2 - 1);
                if (_isGridShown)
                    g.DrawLine(_penGrid, _spcLeft, labelYMin, xGridRight, labelYMin);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMin, xGridRight, labelYMin);
            }
            label = maxValue;
            var labelYMax = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
            if (Math.Abs(labelYZero - labelYMax) >= _font.Height)
            {
                deltaLabel = Math.Abs(label);
                format = deltaLabel < 10
                             ? "F4"
                             : deltaLabel < 100 ? "F3" : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                g.DrawString(label.ToString(format), _font, _brushFore, _xRight, labelYMax - _font.Height/2 - 1);
                if (_isGridShown)
                    g.DrawLine(_penGrid, _spcLeft, labelYMax, xGridRight, labelYMax);
                else
                    g.DrawLine(_penGrid, xGridRight - 5, labelYMax, xGridRight, labelYMax);
            }
            if (_chartData.Strategy.Slot[slot].SpecValue != null)
                for (int i = 0; i < _chartData.Strategy.Slot[slot].SpecValue.Length; i++)
                {
                    label = _chartData.Strategy.Slot[slot].SpecValue[i];
                    if (label <= maxValue && label >= minValue)
                    {
                        var labelY = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (label - minValue)*scale);
                        if (Math.Abs(labelY - labelYZero) < _font.Height) continue;
                        if (Math.Abs(labelY - labelYMin) < _font.Height) continue;
                        if (Math.Abs(labelY - labelYMax) < _font.Height) continue;
                        deltaLabel = Math.Abs(label);
                        format = deltaLabel < 10
                                     ? "F4"
                                     : deltaLabel < 100
                                           ? "F3"
                                           : deltaLabel < 1000 ? "F2" : deltaLabel < 10000 ? "F1" : "F0";
                        g.DrawString(label.ToString(format), _font, _brushFore, _xRight, labelY - _font.Height/2 - 1);
                        if (_isGridShown)
                            g.DrawLine(_penGrid, _spcLeft, labelY, xGridRight, labelY);
                        else
                            g.DrawLine(_penGrid, xGridRight - 5, labelY, xGridRight, labelY);
                    }
                }

            if (_isGridShown)
            {
                // Vertical lines
                string date = _chartData.Time[_firstBar].ToString("dd.MM") + " " +
                              _chartData.Time[_firstBar].ToString("HH:mm");
                var dateWidth = (int) g.MeasureString(date, _font).Width;
                for (int vertLineBar = _lastBar;
                     vertLineBar > _firstBar;
                     vertLineBar -= (dateWidth + 10)/_barPixels + 1)
                {
                    int xVertLine = _spcLeft + (vertLineBar - _firstBar)*_barPixels + _barPixels/2 - 1;
                    g.DrawLine(_penGrid, xVertLine, topSpace, xVertLine, pnl.ClientSize.Height - bottomSpace);
                }
            }

            bool isIndicatorValueAtClose = true;
            int indicatorValueShift = 1;
            foreach (ListParam listParam in _chartData.Strategy.Slot[slot].IndParam.ListParam)
                if (listParam.Caption == "Base price" && listParam.Text == "Open")
                {
                    isIndicatorValueAtClose = false;
                    indicatorValueShift = 0;
                }

            // Indicator chart
            foreach (IndicatorComp component in _chartData.Strategy.Slot[slot].Component)
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

                    if (_isTrueChartsShown)
                    {
                        // True Chart Histogram
                        if (isIndicatorValueAtClose)
                        {
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar - 1];
                                double prevValue = component.Value[bar - 2];
                                int x = _spcLeft + (bar - _firstBar)*_barPixels + _barPixels/2 - 1;
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
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*_barPixels + _barPixels - 4;
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
                            for (int bar = _firstBar; bar <= _lastBar; bar++)
                            {
                                double value = component.Value[bar];
                                double prevValue = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*_barPixels + _barPixels/2 - 1;
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

                    if (!_isTrueChartsShown)
                    {
                        // Regular Histogram Chart
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            double prevValue = component.Value[bar - 1];
                            int x = (bar - _firstBar)*_barPixels + _spcLeft + 1;
                            var y = (int) Math.Round(pnl.ClientSize.Height - bottomSpace - (value - minValue)*scale);

                            LinearGradientBrush lgBrush;
                            Rectangle rect;
                            if (value > prevValue || Math.Abs(value - prevValue) < 0.00001 && isPrevBarGreen)
                            {
                                if (y > y0)
                                {
                                    rect = new Rectangle(x - 1, y0, _barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y0, _barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, _barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, _colorLongTrade1, _colorLongTrade2, 0f);
                                    rect = new Rectangle(x, y, _barPixels - 4, y0 - y);
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
                                    rect = new Rectangle(x - 1, y0, _barPixels - 3, y - y0);
                                    lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y0, _barPixels - 4, y - y0);
                                }
                                else if (y < y0)
                                {
                                    rect = new Rectangle(x - 1, y, _barPixels - 3, y0 - y);
                                    lgBrush = new LinearGradientBrush(rect, _colorShortTrade1, _colorShortTrade2, 0f);
                                    rect = new Rectangle(x, y, _barPixels - 4, y0 - y);
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
                    if (_isTrueChartsShown)
                    {
                        // True Charts
                        var pen = new Pen(component.ChartColor);
                        var penTC = new Pen(component.ChartColor)
                                        {DashStyle = DashStyle.Dash, DashPattern = new float[] {2, 1}};

                        int yIndChart = pnl.ClientSize.Height - bottomSpace;

                        var point = new Point[_lastBar - _firstBar + 1];
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double value = component.Value[bar];
                            int x = _spcLeft + (bar - _firstBar)*_barPixels + 1 + indicatorValueShift*(_barPixels - 5);
                            var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                            point[bar - _firstBar] = new Point(x, y);
                        }

                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            // All bars except the last one
                            int i = bar - _firstBar;

                            // The indicator value point
                            g.DrawLine(pen, point[i].X - 1, point[i].Y, point[i].X + 1, point[i].Y);
                            g.DrawLine(pen, point[i].X, point[i].Y - 1, point[i].X, point[i].Y + 1);

                            if (bar == _firstBar && isIndicatorValueAtClose)
                            {
                                // First bar
                                double value = component.Value[bar - 1];
                                int x = _spcLeft + (bar - _firstBar)*_barPixels;
                                var y = (int) Math.Round(yIndChart - (value - minValue)*scale);

                                int deltaY = Math.Abs(y - point[i].Y);
                                if (_barPixels > 3)
                                {
                                    // Horizontal part
                                    if (deltaY == 0)
                                        g.DrawLine(pen, x + 1, y, x + _barPixels - 7, y);
                                    else if (deltaY < 3)
                                        g.DrawLine(pen, x + 1, y, x + _barPixels - 6, y);
                                    else
                                        g.DrawLine(pen, x + 1, y, x + _barPixels - 4, y);
                                }
                                if (deltaY > 4)
                                {
                                    // Vertical part
                                    if (point[i].Y > y)
                                        g.DrawLine(penTC, x + _barPixels - 4, y + 2, x + _barPixels - 4, point[i].Y - 2);
                                    else
                                        g.DrawLine(penTC, x + _barPixels - 4, y - 2, x + _barPixels - 4, point[i].Y + 2);
                                }
                            }

                            if (bar < _lastBar)
                            {
                                int deltaY = Math.Abs(point[i + 1].Y - point[i].Y);
                                if (_barPixels > 3)
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
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y + 2, point[i + 1].X,
                                                   point[i + 1].Y - 2);
                                    else
                                        g.DrawLine(penTC, point[i + 1].X, point[i].Y - 2, point[i + 1].X,
                                                   point[i + 1].Y + 2);
                                }
                            }

                            if (bar == _lastBar && !isIndicatorValueAtClose && _barPixels > 3)
                            {
                                // Last bar
                                g.DrawLine(pen, point[i].X + 3, point[i].Y, point[i].X + _barPixels - 5, point[i].Y);
                            }
                        }
                    }

                    if (!_isTrueChartsShown)
                    {
                        // Regular Line Chart
                        var points = new Point[_lastBar - _firstBar + 1];
                        for (int bar = _firstBar; bar <= _lastBar; bar++)
                        {
                            double dValue = component.Value[bar];
                            int x = (bar - _firstBar)*_barPixels + _barPixels/2 - 1 + _spcLeft;
                            var y = (int) (pnl.ClientSize.Height - bottomSpace - (dValue - minValue)*scale);
                            points[bar - _firstBar] = new Point(x, y);
                        }
                        g.DrawLines(new Pen(component.ChartColor), points);
                    }
                }
            }

            // Vertical cross line
            if (_isCrossShown && (_isMouseInIndicatorChart || _isMouseInPriceChart) && _mouseX > _xLeft - 1 &&
                _mouseX < _xRight + 1)
                g.DrawLine(_penCross, _mouseX, 0, _mouseX, pnl.ClientSize.Height);

            // Chart title

            Indicator indicator = IndicatorManager.ConstructIndicator(_chartData.Strategy.Slot[slot].IndicatorName);
            indicator.Initialize(_chartData.Strategy.Slot[slot].SlotType);
            indicator.IndParam = _chartData.Strategy.Slot[slot].IndParam;
            string title = indicator.ToString();
            Size sizeTitle = g.MeasureString(title, Font).ToSize();
            g.FillRectangle(_brushBack, new Rectangle(_spcLeft, 0, sizeTitle.Width, sizeTitle.Height));
            g.DrawString(title, Font, _brushFore, _spcLeft + 2, 0);
        }

        /// <summary>
        ///  Invalidates the panels
        /// </summary>
        private void InvalidateAllPanels()
        {
            PnlPrice.Invalidate();
            foreach (Panel panel in PnlInd)
                panel.Invalidate();
        }

        /// <summary>
        /// Sets the width of the info panel
        /// </summary>
        private void SetupDynInfoWidth()
        {
            _asInfoTitle = new string[200];
            _aiInfoType = new int[200];
            _infoRows = 0;

            // Dynamic info titles
            _asInfoTitle[_infoRows++] = Language.T("Bar number");
            _asInfoTitle[_infoRows++] = Language.T("Date");
            _asInfoTitle[_infoRows++] = Language.T("Opening time");
            _asInfoTitle[_infoRows++] = Language.T("Opening price");
            _asInfoTitle[_infoRows++] = Language.T("Highest price");
            _asInfoTitle[_infoRows++] = Language.T("Lowest price");
            _asInfoTitle[_infoRows++] = Language.T("Closing price");
            _asInfoTitle[_infoRows++] = Language.T("Volume");
            _asInfoTitle[_infoRows++] = Language.T("Position direction");
            _asInfoTitle[_infoRows++] = Language.T("Open lots");
            _asInfoTitle[_infoRows++] = Language.T("Position price");

            for (int iSlot = 0; iSlot < _chartData.Strategy.Slots; iSlot++)
            {
                int iCompToShow = 0;
                foreach (IndicatorComp indComp in _chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) iCompToShow++;
                if (iCompToShow == 0) continue;

                _aiInfoType[_infoRows] = 1;
                _asInfoTitle[_infoRows++] = _chartData.Strategy.Slot[iSlot].IndicatorName +
                                            (_chartData.Strategy.Slot[iSlot].IndParam.CheckParam[0].Checked ? "*" : "");
                foreach (IndicatorComp indComp in _chartData.Strategy.Slot[iSlot].Component)
                    if (indComp.ShowInDynInfo) _asInfoTitle[_infoRows++] = indComp.CompName;
            }

            Graphics g = CreateGraphics();

            int iMaxLenght = 0;
            foreach (string str in _asInfoTitle)
            {
                var iLenght = (int) g.MeasureString(str, _fontDi).Width;
                if (iMaxLenght < iLenght) iMaxLenght = iLenght;
            }

            _xDynInfoCol2 = iMaxLenght + 10;
            var maxInfoWidth = (int) g.MeasureString("99/99/99     ", _fontDi).Width;

            g.Dispose();

            _dynInfoWidth = _xDynInfoCol2 + maxInfoWidth + (_isDebug ? 30 : 5);

            PnlInfo.ClientSize = new Size(_dynInfoWidth, PnlInfo.ClientSize.Height);
        }

        /// <summary>
        /// Sets the dynamic info panel
        /// </summary>
        private void SetupDynamicInfo()
        {
            _asInfoTitle = new string[200];
            _aiInfoType = new int[200];
            _infoRows = 0;

            _asInfoTitle[_infoRows++] = Language.T("Date");
            _asInfoTitle[_infoRows++] = Language.T("Opening time");
            _asInfoTitle[_infoRows++] = Language.T("Opening price");
            _asInfoTitle[_infoRows++] = Language.T("Highest price");
            _asInfoTitle[_infoRows++] = Language.T("Lowest price");
            _asInfoTitle[_infoRows++] = Language.T("Closing price");
            _asInfoTitle[_infoRows++] = Language.T("Volume");
            _asInfoTitle[_infoRows++] = "";
            _asInfoTitle[_infoRows++] = Language.T("Position direction");
            _asInfoTitle[_infoRows++] = Language.T("Open lots");
            _asInfoTitle[_infoRows++] = Language.T("Position price");

            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                int compToShow = 0;
                foreach (IndicatorComp indComp in _chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo)
                        compToShow++;

                if (compToShow == 0)
                    continue;

                _asInfoTitle[_infoRows++] = "";
                _aiInfoType[_infoRows] = 1;
                _asInfoTitle[_infoRows++] = _chartData.Strategy.Slot[slot].IndicatorName +
                                            (_chartData.Strategy.Slot[slot].IndParam.CheckParam[0].Checked ? "*" : "");

                foreach (IndicatorComp indComp in _chartData.Strategy.Slot[slot].Component)
                    if (indComp.ShowInDynInfo)
                        _asInfoTitle[_infoRows++] = indComp.CompName;
            }
        }

        /// <summary>
        /// Generates the DynamicInfo.
        /// </summary>
        private void GenerateDynamicInfo(int barNumb)
        {
            if (!_isInfoPanelShown) return;

            int bar;

            if (barNumb != _chartData.Bars - 1)
            {
                barNumb = Math.Max(0, barNumb);
                barNumb = Math.Min(_chartBars - 1, barNumb);

                bar = _firstBar + barNumb;
                bar = Math.Min(_chartData.Bars - 1, bar);
            }
            else
                bar = barNumb;

            if (_barOld == bar && bar != _lastBar) return;
            _barOld = bar;

            int row = 0;
            _asInfoValue = new String[200];
            _asInfoValue[row++] = _chartData.Time[bar].ToString(Data.DF);
            _asInfoValue[row++] = _chartData.Time[bar].ToString("HH:mm");
            if (_isDebug)
            {
                _asInfoValue[row++] = _chartData.Open[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _chartData.High[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _chartData.Low[bar].ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _chartData.Close[bar].ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _asInfoValue[row++] = _chartData.Open[bar].ToString(Data.FF);
                _asInfoValue[row++] = _chartData.High[bar].ToString(Data.FF);
                _asInfoValue[row++] = _chartData.Low[bar].ToString(Data.FF);
                _asInfoValue[row++] = _chartData.Close[bar].ToString(Data.FF);
            }
            _asInfoValue[row++] = _chartData.Volume[bar].ToString(CultureInfo.InvariantCulture);

            _asInfoValue[row++] = "";
            DateTime baropen = _chartData.Time[bar];
            if (_chartData.BarStatistics.ContainsKey(baropen))
            {
                _asInfoValue[row++] = Language.T(_chartData.BarStatistics[baropen].PositionDir.ToString());
                _asInfoValue[row++] =
                    _chartData.BarStatistics[baropen].PositionLots.ToString(CultureInfo.InvariantCulture);
                _asInfoValue[row++] = _chartData.BarStatistics[baropen].PositionPrice.ToString(Data.FF);
            }
            else
            {
                _asInfoValue[row++] = Language.T("Square");
                _asInfoValue[row++] = "   -";
                _asInfoValue[row++] = "   -";
            }

            for (int slot = 0; slot < _chartData.Strategy.Slots; slot++)
            {
                if (_chartData.Strategy.Slot[slot] != null)
                {
                    int compToShow = 0;
                    foreach (IndicatorComp indComp in _chartData.Strategy.Slot[slot].Component)
                        if (indComp.ShowInDynInfo) compToShow++;
                    if (compToShow == 0) continue;

                    _asInfoValue[row++] = "";
                    _asInfoValue[row++] = "";
                    foreach (IndicatorComp indComp in _chartData.Strategy.Slot[slot].Component)
                    {
                        if (indComp.ShowInDynInfo)
                        {
                            IndComponentType indDataTipe = indComp.DataType;
                            if (indDataTipe == IndComponentType.AllowOpenLong ||
                                indDataTipe == IndComponentType.AllowOpenShort ||
                                indDataTipe == IndComponentType.ForceClose ||
                                indDataTipe == IndComponentType.ForceCloseLong ||
                                indDataTipe == IndComponentType.ForceCloseShort)
                                _asInfoValue[row++] = (indComp.Value[bar] < 1 ? Language.T("No") : Language.T("Yes"));
                            else
                            {
                                if (_isDebug)
                                {
                                    _asInfoValue[row++] = indComp.Value[bar].ToString(CultureInfo.InvariantCulture);
                                }
                                else
                                {
                                    double dl = Math.Abs(indComp.Value[bar]);
                                    string sFR = dl < 10
                                                     ? "F5"
                                                     : dl < 100
                                                           ? "F4"
                                                           : dl < 1000
                                                                 ? "F3"
                                                                 : dl < 10000 ? "F2" : dl < 100000 ? "F1" : "F0";
                                    if (Math.Abs(indComp.Value[bar]) > 0.00001)
                                        _asInfoValue[row++] = indComp.Value[bar].ToString(sFR);
                                    else
                                        _asInfoValue[row++] = "   -";
                                }
                            }
                        }
                    }
                }
            }

            PnlInfo.Invalidate(new Rectangle(_xDynInfoCol2, 0, _dynInfoWidth - _xDynInfoCol2, PnlInfo.ClientSize.Height));
        }

        /// <summary>
        /// Paints the panel PnlInfo.
        /// </summary>
        private void PnlInfoPaint(object sender, PaintEventArgs e)
        {
            if (!_isInfoPanelShown) return;

            Graphics g = e.Graphics;
            try
            {
                g.Clear(LayoutColors.ColorControlBack);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            int iRowHeight = _fontDi.Height + 1;
            var size = new Size(_dynInfoWidth, iRowHeight);

            for (int i = 0; i < _infoRows; i++)
            {
                var point0 = new Point(0, i*iRowHeight + 1);
                var point1 = new Point(5, i*iRowHeight);
                var point2 = new Point(_xDynInfoCol2, i*iRowHeight);

                if (Math.Abs(i%2f - 0) > 0.0001)
                    g.FillRectangle(_brushEvenRows, new Rectangle(point0, size));

                if (_aiInfoType[i + _dynInfoScrollValue] == 1)
                    g.DrawString(_asInfoTitle[i + _dynInfoScrollValue], _fontDiInd, _brushDiIndicator, point1);
                else
                    g.DrawString(_asInfoTitle[i + _dynInfoScrollValue], _fontDi, _brushDynamicInfo, point1);
                g.DrawString(_asInfoValue[i + _dynInfoScrollValue], _fontDi, _brushDynamicInfo, point2);
            }
        }

        /// <summary>
        /// Invalidate Cross Old/New position and Dynamic Info
        /// </summary>
        private void PnlPriceMouseMove(object sender, MouseEventArgs e)
        {
            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = e.X;
            _mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                if (_mouseX > _xRight)
                {
                    if (_mouseY > _mouseYOld)
                        VerticalScaleDecrease();
                    else
                        VerticalScaleIncrease();

                    return;
                }

                int newScrollValue = ScrollBar.Value;

                if (_mouseX > _mouseXOld)
                    newScrollValue -= (int) (ScrollBar.SmallChange*0.1*(100 - _barPixels));
                else if (_mouseX < _mouseXOld)
                    newScrollValue += (int) (ScrollBar.SmallChange*0.1*(100 - _barPixels));

                if (newScrollValue < ScrollBar.Minimum)
                    newScrollValue = ScrollBar.Minimum;
                else if (newScrollValue > ScrollBar.Maximum + 1 - ScrollBar.LargeChange)
                    newScrollValue = ScrollBar.Maximum + 1 - ScrollBar.LargeChange;

                ScrollBar.Value = newScrollValue;
            }

            // Determines the shown bar.
            int shownBar = _lastBar;
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                // Moving inside the chart
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    _isMouseInPriceChart = true;
                    _isDrawDinInfo = true;
                    shownBar = (e.X - _xLeft)/_barPixels;
                    if (_isCrossShown)
                        PnlPrice.Cursor = Cursors.Cross;
                }
                    // Escaping from the chart
                else
                {
                    _isMouseInPriceChart = false;
                    shownBar = _lastBar;
                    PnlPrice.Cursor = Cursors.Default;
                }
            }
            else if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                // Entering into the chart
                _isMouseInPriceChart = true;
                _isDrawDinInfo = true;
                shownBar = (e.X - _xLeft)/_barPixels;
                if (_isCrossShown)
                    PnlPrice.Cursor = Cursors.Cross;
            }

            if (!_isCrossShown)
                return;

            var path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                if (_mouseYOld >= _yTop && _mouseYOld <= _yBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, _mouseYOld, PnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(_xRight - 1, _mouseYOld - _font.Height/2 - 1, _szPrice.Width + 2,
                                                    _font.Height + 2));
                }
                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));
            }

            // Adding the new positions
            if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                if (_mouseY >= _yTop && _mouseY <= _yBottom)
                {
                    // Horizontal Line
                    path.AddRectangle(new Rectangle(0, _mouseY, PnlPrice.ClientSize.Width, 1));
                    // PriceBox
                    path.AddRectangle(new Rectangle(_xRight - 1, _mouseY - _font.Height/2 - 1, _szPrice.Width + 2,
                                                    _font.Height + 2));
                }
                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseX - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));
            }
            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < _indPanels; i++)
            {
                var path1 = new GraphicsPath(FillMode.Winding);
                if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                    path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                    path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                PnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);
        }

        /// <summary>
        /// Deletes the cross and Dynamic Info
        /// </summary>
        private void PnlPriceMouseLeave(object sender, EventArgs e)
        {
            PnlPrice.Cursor = Cursors.Default;
            _isMouseInPriceChart = false;

            if (!_isCrossShown)
                return;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = -1;
            _mouseY = -1;
            _barOld = -1;

            var path = new GraphicsPath(FillMode.Winding);

            // Horizontal Line
            path.AddRectangle(new Rectangle(0, _mouseYOld, PnlPrice.ClientSize.Width, 1));
            // PriceBox
            path.AddRectangle(new Rectangle(_xRight - 1, _mouseYOld - _font.Height/2 - 1, _szPrice.Width + 2,
                                            _font.Height + 2));
            // Vertical Line
            path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
            // DateBox
            path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                            _font.Height + 3));

            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < _indPanels; i++)
                PnlInd[i].Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));

            if (_isInfoPanelShown)
                GenerateDynamicInfo(_lastBar);
        }

        /// <summary>
        /// Mouse moves inside a chart
        /// </summary>
        private void PnlIndMouseMove(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = e.X;
            _mouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                int newScrollValue = ScrollBar.Value;

                if (_mouseX > _mouseXOld)
                    newScrollValue -= (int) Math.Round(ScrollBar.SmallChange*0.1*(100 - _barPixels));
                else if (_mouseX < _mouseXOld)
                    newScrollValue += (int) Math.Round(ScrollBar.SmallChange*0.1*(100 - _barPixels));

                if (newScrollValue < ScrollBar.Minimum)
                    newScrollValue = ScrollBar.Minimum;
                else if (newScrollValue > ScrollBar.Maximum + 1 - ScrollBar.LargeChange)
                    newScrollValue = ScrollBar.Maximum + 1 - ScrollBar.LargeChange;

                ScrollBar.Value = newScrollValue;
            }

            // Determines the shown bar.
            int shownBar = _lastBar;
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                if (_mouseX >= _xLeft && _mouseX <= _xRight)
                {
                    // Moving inside the chart
                    _isMouseInIndicatorChart = true;
                    _isDrawDinInfo = true;
                    shownBar = (e.X - _xLeft)/_barPixels;
                    if (_isCrossShown)
                        panel.Cursor = Cursors.Cross;
                }
                else
                {
                    // Escaping from the bar area of chart
                    _isMouseInIndicatorChart = false;
                    panel.Cursor = Cursors.Default;
                    shownBar = _lastBar;
                }
            }
            else if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                // Entering into the chart
                _isMouseInIndicatorChart = true;
                _isDrawDinInfo = true;
                PnlInfo.Invalidate();
                shownBar = (e.X - _xLeft)/_barPixels;
                if (_isCrossShown)
                    panel.Cursor = Cursors.Cross;
            }

            if (!_isCrossShown)
                return;

            var path = new GraphicsPath(FillMode.Winding);

            // Adding the old positions
            if (_mouseXOld >= _xLeft && _mouseXOld <= _xRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));
            }

            // Adding the new positions
            if (_mouseX >= _xLeft && _mouseX <= _xRight)
            {
                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseX - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));
            }
            PnlPrice.Invalidate(new Region(path));

            for (int i = 0; i < _indPanels; i++)
            {
                var path1 = new GraphicsPath(FillMode.Winding);
                if (_mouseXOld > _xLeft - 1 && _mouseXOld < _xRight + 1)
                    path1.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
                if (_mouseX > _xLeft - 1 && _mouseX < _xRight + 1)
                    path1.AddRectangle(new Rectangle(_mouseX, 0, 1, PnlInd[i].ClientSize.Height));
                PnlInd[i].Invalidate(new Region(path1));
            }

            GenerateDynamicInfo(shownBar);
        }

        /// <summary>
        /// Mouse leaves a chart.
        /// </summary>
        private void PnlIndMouseLeave(object sender, EventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = Cursors.Default;

            _isMouseInIndicatorChart = false;

            _mouseXOld = _mouseX;
            _mouseYOld = _mouseY;
            _mouseX = -1;
            _mouseY = -1;
            _barOld = -1;

            if (_isCrossShown)
            {
                var path = new GraphicsPath(FillMode.Winding);

                // Vertical Line
                path.AddRectangle(new Rectangle(_mouseXOld, 0, 1, PnlPrice.ClientSize.Height));
                // DateBox
                path.AddRectangle(new Rectangle(_mouseXOld - _szDateL.Width/2 - 1, _yBottomText - 1, _szDateL.Width + 2,
                                                _font.Height + 3));

                PnlPrice.Invalidate(new Region(path));

                for (int i = 0; i < _indPanels; i++)
                    PnlInd[i].Invalidate(new Rectangle(_mouseXOld, 0, 1, PnlInd[i].ClientSize.Height));
            }
        }

        /// <summary>
        /// Mouse Button Up
        /// </summary>
        private void PanelMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            panel.Cursor = _isCrossShown ? Cursors.Cross : Cursors.Default;
            ScrollBar.Focus();
        }

        /// <summary>
        /// Mouse Button Down
        /// </summary>
        private void PanelMouseDown(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            if (panel == PnlPrice && _mouseX > _xRight)
                panel.Cursor = Cursors.SizeNS;
            else if (!_isCrossShown)
                panel.Cursor = Cursors.SizeWE;
        }

        /// <summary>
        /// Sets the parameters when scrolling.
        /// </summary>
        private void ScrollValueChanged(object sender, EventArgs e)
        {
            _firstBar = ScrollBar.Value;
            _lastBar = Math.Min(_chartData.Bars - 1, _firstBar + _chartBars - 1);
            _lastBar = Math.Max(_lastBar, _firstBar);

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);

            InvalidateAllPanels();

            _barOld = 0;
            if (_isInfoPanelShown && _isDrawDinInfo && _isCrossShown)
            {
                int selectedBar = (_mouseX - _spcLeft)/_barPixels;
                GenerateDynamicInfo(selectedBar);
            }
        }

        /// <summary>
        /// Scrolls the scrollbar when turning the mouse wheel.
        /// </summary>
        private void ScrollMouseWheel(object sender, MouseEventArgs e)
        {
            if (_isCtrlKeyPressed)
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
        /// Call KeyUp method
        /// </summary>
        private void ScrollKeyUp(object sender, KeyEventArgs e)
        {
            _isCtrlKeyPressed = false;

            ShortcutKeyUp(e);
        }

        /// <summary>
        /// Call KeyUp method
        /// </summary>
        private void ScrollKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control)
                _isCtrlKeyPressed = true;
        }

        /// <summary>
        /// Changes chart's settings after a button click.
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
        /// Shortcut keys
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
                _isCandleChart = !_isCandleChart;
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
                _isGridShown = !_isGridShown;
                Configs.ChartGrid = _isGridShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Grid].Checked = _isGridShown;
                InvalidateAllPanels();
            }
                // Cross
            else if (e.KeyCode == Keys.C)
            {
                _isCrossShown = !_isCrossShown;
                Configs.ChartCross = _isCrossShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Cross].Checked = _isCrossShown;
                InvalidateAllPanels();
                if (_isCrossShown)
                {
                    GenerateDynamicInfo((_mouseX - _xLeft)/_barPixels);
                    PnlPrice.Cursor = Cursors.Cross;
                    foreach (Panel pnlind in PnlInd)
                        pnlind.Cursor = Cursors.Cross;
                }
                else
                {
                    GenerateDynamicInfo(_chartData.Bars - 1);
                    PnlPrice.Cursor = Cursors.Default;
                    foreach (Panel pnlind in PnlInd)
                        pnlind.Cursor = Cursors.Default;
                }
            }
                // Volume
            else if (e.KeyCode == Keys.V)
            {
                _isVolumeShown = !_isVolumeShown;
                Configs.ChartVolume = _isVolumeShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Volume].Checked = _isVolumeShown;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
                // Lots
            else if (e.KeyCode == Keys.L)
            {
                _isPositionLotsShown = !_isPositionLotsShown;
                Configs.ChartLots = _isPositionLotsShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionLots].Checked = _isPositionLotsShown;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
                // Orders
            else if (e.KeyCode == Keys.O)
            {
                _isOrdersShown = !_isOrdersShown;
                Configs.ChartOrders = _isOrdersShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Orders].Checked = _isOrdersShown;
                PnlPrice.Invalidate();
            }
                // Position price
            else if (e.KeyCode == Keys.P)
            {
                _isPositionPriceShown = !_isPositionPriceShown;
                Configs.ChartPositionPrice = _isPositionPriceShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.PositionPrice].Checked = _isPositionPriceShown;
                PnlPrice.Invalidate();
            }
                // True Charts
            else if (e.KeyCode == Keys.T)
            {
                _isTrueChartsShown = !_isTrueChartsShown;
                Configs.ChartTrueCharts = _isTrueChartsShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.TrueCharts].Checked = _isTrueChartsShown;
                InvalidateAllPanels();
            }
                // Chart shift
            else if (e.KeyCode == Keys.S)
            {
                _isChartShift = !_isChartShift;
                Configs.ChartShift = _isChartShift;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.Shift].Checked = _isChartShift;
                SetFirstLastBar();
            }
                // Chart Auto Scroll
            else if (e.KeyCode == Keys.R)
            {
                _isChartAutoScroll = !_isChartAutoScroll;
                Configs.ChartAutoScroll = _isChartAutoScroll;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.AutoScroll].Checked = _isChartAutoScroll;
                SetFirstLastBar();
            }
                // Dynamic info scroll down
            else if (e.KeyCode == Keys.Z)
            {
                _dynInfoScrollValue += 5;
                _dynInfoScrollValue = _dynInfoScrollValue > _infoRows - 5 ? _infoRows - 5 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Dynamic info scroll up
            else if (e.KeyCode == Keys.A)
            {
                _dynInfoScrollValue -= 5;
                _dynInfoScrollValue = _dynInfoScrollValue < 0 ? 0 : _dynInfoScrollValue;
                PnlInfo.Invalidate();
            }
                // Show info panel
            else if (e.KeyCode == Keys.I || e.KeyCode == Keys.F2)
            {
                _isInfoPanelShown = !_isInfoPanelShown;
                Configs.ChartInfoPanel = _isInfoPanelShown;
                PnlInfo.Visible = _isInfoPanelShown;
                PnlCharts.Padding = _isInfoPanelShown ? new Padding(0, 0, 4, 0) : new Padding(0);
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoUp].Visible = _isInfoPanelShown;
                ChartButtons[(int) ForexStrategyBuilder.ChartButtons.DInfoDwn].Visible = _isInfoPanelShown;
            }
                // Debug
            else if (e.KeyCode == Keys.F12)
            {
                _isDebug = !_isDebug;
                SetupDynInfoWidth();
                SetupDynamicInfo();
                PnlInfo.Invalidate();
            }
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleDecrease()
        {
            if (_verticalScale > 10)
            {
                _verticalScale -= 10;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Changes vertical scale of the Price Chart
        /// </summary>
        private void VerticalScaleIncrease()
        {
            if (_verticalScale < 300)
            {
                _verticalScale += 10;
                SetPriceChartMinMaxValues();
                PnlPrice.Invalidate();
            }
        }

        /// <summary>
        /// Zooms the chart in.
        /// </summary>
        private void ZoomIn()
        {
            _barPixels += 4;
            if (_barPixels > 45)
                _barPixels = 45;

            int oldChartBars = _chartBars;

            _chartBars = _chartWidth/_barPixels;
            if (_chartBars > _chartData.Bars - _chartData.FirstBar)
                _chartBars = _chartData.Bars - _chartData.FirstBar;

            if (_lastBar < _chartData.Bars - 1)
            {
                _firstBar += (oldChartBars - _chartBars)/2;
                if (_firstBar > _chartData.Bars - _chartBars)
                    _firstBar = _chartData.Bars - _chartBars;
            }
            else
            {
                _firstBar = Math.Max(_chartData.FirstBar, _chartData.Bars - _chartBars);
            }

            _lastBar = _firstBar + _chartBars - 1;

            ScrollBar.Value = _firstBar;
            ScrollBar.LargeChange = _chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = _barPixels;
        }

        /// <summary>
        /// Zooms the chart out.
        /// </summary>
        private void ZoomOut()
        {
            _barPixels -= 4;
            if (_barPixels < 9)
                _barPixels = 9;

            int oldChartBars = _chartBars;

            _chartBars = _chartWidth/_barPixels;
            if (_chartBars > _chartData.Bars - _chartData.FirstBar)
                _chartBars = _chartData.Bars - _chartData.FirstBar;

            if (_lastBar < _chartData.Bars - 1)
            {
                _firstBar -= (_chartBars - oldChartBars)/2;
                if (_firstBar < _chartData.FirstBar)
                    _firstBar = _chartData.FirstBar;

                if (_firstBar > _chartData.Bars - _chartBars)
                    _firstBar = _chartData.Bars - _chartBars;
            }
            else
            {
                _firstBar = Math.Max(_chartData.FirstBar, _chartData.Bars - _chartBars);
            }

            _lastBar = _firstBar + _chartBars - 1;

            ScrollBar.Value = _firstBar;
            ScrollBar.LargeChange = _chartBars;

            SetPriceChartMinMaxValues();
            for (int i = 0; i < PnlInd.Length; i++)
                SetSepChartsMinMaxValues(i);
            InvalidateAllPanels();

            Configs.ChartZoom = _barPixels;
        }
    }
}