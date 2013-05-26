// Balance_Chart Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    public struct BalanceChartUnit
    {
        public double Balance { get; set; }
        public double Equity { get; set; }
        public DateTime Time { get; set; }
    }

    /// <summary>
    /// Draws a balance chart
    /// </summary>
    public sealed class BalanceChart : Panel
    {
        private const int Border = 2;
        private const int Space = 5;
        private readonly float _captionHeight;
        private readonly string _chartTitle;
        private readonly Font _font;
        private readonly StringFormat _stringFormatCaption;
        private PointF[] _apntBalance;
        private PointF[] _apntEquity;
        private double[] _balanceData;
        private float _balanceY;
        private Brush _brushFore;
        private Rectangle _chartArea;
        private int _chartPoints;
        private int _cntLabels;
        private float _deltaLabels;
        private double[] _equityData;
        private float _equityY;
        private int _labelWidth;
        private int _maxBalance;
        private int _maxEquity;
        private int _maxValue;
        private int _minBalance;
        private int _minEquity;
        private int _minValue;
        private float _netBalance;
        private float _netEquity;
        private Pen _penBorder;
        private Pen _penGrid;
        private RectangleF _rectfCaption;
        private float _scaleX;
        private float _scaleY;
        private DateTime _startTime;
        private int _stepLabels;
        private int _xLeft;
        private int _xRight;
        private int _yBottom;
        private int _yTop;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public BalanceChart()
        {
            SetColors();

            // Chart Title
            _chartTitle = Language.T("Balance / Equity Chart");
            _font = new Font(Font.FontFamily, 9);
            _captionHeight = Math.Max(_font.Height, 18);
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _stringFormatCaption = new StringFormat
                                       {
                                           Alignment = StringAlignment.Center,
                                           LineAlignment = StringAlignment.Center,
                                           Trimming = StringTrimming.EllipsisCharacter,
                                           FormatFlags = StringFormatFlags.NoWrap
                                       };

            _penGrid.DashStyle = DashStyle.Dash;
            _penGrid.DashPattern = new float[] {4, 2};
        }

        /// <summary>
        /// Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(BalanceChartUnit[] data, int points)
        {
            if (data == null || points < 1)
                return;

            _balanceData = new double[points];
            _equityData = new double[points];
            for (int p = 0; p < points; p++)
            {
                _balanceData[p] = data[p].Balance;
                _equityData[p] = data[p].Equity;
            }

            _maxBalance = int.MinValue;
            _minBalance = int.MaxValue;
            _maxEquity = int.MinValue;
            _minEquity = int.MaxValue;

            foreach (double balance in _balanceData)
            {
                if (balance > _maxBalance) _maxBalance = (int) balance;
                if (balance < _minBalance) _minBalance = (int) balance;
            }

            foreach (double equity in _equityData)
            {
                if (equity > _maxEquity) _maxEquity = (int) equity;
                if (equity < _minEquity) _minEquity = (int) equity;
            }

            _startTime = data[0].Time;

            InitChart();
        }

        /// <summary>
        /// Refreshes the chart.
        /// </summary>
        public void RefreshChart()
        {
            Invalidate(_chartArea);
        }

        /// <summary>
        /// Sets the chart params
        /// </summary>
        private void InitChart()
        {
            if (_balanceData == null || _balanceData.Length < 1)
                return;

            _chartPoints = Math.Max(_balanceData.Length, _equityData.Length);

            if (_chartPoints == 0) return;

            _maxValue = Math.Max(_maxBalance, _maxEquity) + 1;
            _minValue = Math.Min(_minBalance, _minEquity) - 1;
            _minValue = (int) (Math.Floor(_minValue/10f)*10);

            _yTop = (int) _captionHeight + 2*Space + 1;
            _yBottom = ClientSize.Height - Space - Border - Font.Height;

            Graphics g = CreateGraphics();
            _labelWidth =
                (int)
                Math.Max(g.MeasureString(_minValue.ToString("F2"), Font).Width,
                         g.MeasureString(_maxValue.ToString("F2"), Font).Width);
            g.Dispose();

            _labelWidth = Math.Max(_labelWidth, 30);
            _xLeft = Border + Space;
            _xRight = ClientSize.Width - Border - Space - _labelWidth - 6;
            _scaleX = (_xRight - 2*Space - Border - 10)/(float) (_chartPoints - 1);

            _cntLabels = Math.Max((_yBottom - _yTop)/40, 1);
            _deltaLabels = (float) Math.Max(Math.Round((_maxValue - _minValue)/(float) _cntLabels), 10);
            _stepLabels = (int) Math.Ceiling(_deltaLabels/10)*10;
            _cntLabels = (int) Math.Ceiling((_maxValue - _minValue)/(float) _stepLabels);
            _maxValue = _minValue + _cntLabels*_stepLabels;
            _scaleY = (_yBottom - _yTop)/(_cntLabels*(float) _stepLabels);

            _apntBalance = new PointF[_chartPoints];
            _apntEquity = new PointF[_chartPoints];

            int index = 0;
            foreach (double balance in _balanceData)
            {
                _apntBalance[index].X = _xLeft + index*_scaleX;
                _apntBalance[index].Y = (float) (_yBottom - (balance - _minValue)*_scaleY);
                index++;
            }

            index = 0;
            foreach (double equity in _equityData)
            {
                _apntEquity[index].X = _xLeft + index*_scaleX;
                _apntEquity[index].Y = (float) (_yBottom - (equity - _minValue)*_scaleY);
                index++;
            }

            _netBalance = (float) _balanceData[_balanceData.Length - 1];
            _balanceY = _yBottom - (_netBalance - _minValue)*_scaleY;

            _netEquity = (float) _equityData[_equityData.Length - 1];
            _equityY = _yBottom - (_netEquity - _minValue)*_scaleY;
        }

        /// <summary>
        /// Paints the chart
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
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

            // Caption bar
            Data.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), _rectfCaption,
                         _stringFormatCaption);

            // Border
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (_balanceData == null || _balanceData.Length < 1 ||
                _equityData == null || _equityData.Length < 1)
                return;

            // Grid and Price labels
            for (int iLabel = _minValue; iLabel <= _maxValue; iLabel += _stepLabels)
            {
                var iLabelY = (int) (_yBottom - (iLabel - _minValue)*_scaleY);
                g.DrawString(iLabel.ToString(".00"), Font, _brushFore, _xRight, iLabelY - Font.Height/2 - 1);
                g.DrawLine(_penGrid, _xLeft, iLabelY, _xRight, iLabelY);
            }

            // Equity and Balance lines
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), _apntEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), _apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), _xLeft - 1, _yTop - Space, _xLeft - 1,
                       _yBottom + 1 + Font.Height);

            // Equity price label.
            var pntEquity = new Point(_xRight - Space + 2, (int) (_equityY - _font.Height/2.0 - 1));
            var sizeEquity = new Size(_labelWidth + Space, _font.Height + 2);
            string equity = (_netEquity.ToString("F2"));
            var apEquity = new[]
                               {
                                   new PointF(_xRight - Space - 8, _equityY),
                                   new PointF(_xRight - Space, _equityY - sizeEquity.Height/2),
                                   new PointF(_xRight - Space + sizeEquity.Width + 5, _equityY - sizeEquity.Height/2),
                                   new PointF(_xRight - Space + sizeEquity.Width + 5, _equityY + sizeEquity.Height/2),
                                   new PointF(_xRight - Space, _equityY + sizeEquity.Height/2),
                               };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartEquityLine), apEquity);
            g.DrawString(equity, _font, new SolidBrush(LayoutColors.ColorChartBack), pntEquity);

            // Balance price label.
            var pntBalance = new Point(_xRight - Space + 2, (int) (_balanceY - _font.Height/2.0 - 1));
            var sizeBalance = new Size(_labelWidth + Space, _font.Height + 2);
            string balance = (_netBalance.ToString("F2"));
            var apBalance = new[]
                                {
                                    new PointF(_xRight - Space - 8, _balanceY),
                                    new PointF(_xRight - Space, _balanceY - sizeBalance.Height/2),
                                    new PointF(_xRight - Space + sizeBalance.Width + 5, _balanceY - sizeBalance.Height/2),
                                    new PointF(_xRight - Space + sizeBalance.Width + 5, _balanceY + sizeBalance.Height/2),
                                    new PointF(_xRight - Space, _balanceY + sizeBalance.Height/2),
                                };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartBalanceLine), apBalance);
            g.DrawString(balance, _font, new SolidBrush(LayoutColors.ColorChartBack), pntBalance);

            // Chart Text
            string chartText = _startTime.ToString(CultureInfo.InvariantCulture);
            g.DrawString(chartText, _font, new SolidBrush(LayoutColors.ColorChartFore), _xLeft, _yBottom);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _chartArea = new Rectangle(Border, (int) _captionHeight, ClientSize.Width - 2*Border,
                                       ClientSize.Height - Border - (int) _captionHeight);

            InitChart();
            Invalidate();
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            _brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            _penGrid = new Pen(LayoutColors.ColorChartGrid);
            _penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }
    }
}