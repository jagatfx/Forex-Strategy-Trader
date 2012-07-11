// Forex Strategy Trader
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Draws a small balance chart
    /// </summary>
    public sealed class TickChart : Panel
    {
        private readonly string _caption;
        private readonly float _captionHeight;
        private readonly Font _fontCaption;
        private readonly StringFormat _stringFormatCaption;
        private const int Border = 2;
        private Brush _brushCaption;
        private Rectangle _chartArea;
        private Pen _penBorder;
        private Pen _penChart;
        private double _point;
        private RectangleF _rectfCaption;

        private double[] _tickData;

        /// <summary>
        /// Constructor
        /// </summary>
        public TickChart(string caption)
        {
            _caption = caption;

            _fontCaption = new Font(Font.FontFamily, 9);
            _stringFormatCaption = new StringFormat
                                       {
                                           Alignment = StringAlignment.Center,
                                           LineAlignment = StringAlignment.Center,
                                           Trimming = StringTrimming.EllipsisCharacter,
                                           FormatFlags = StringFormatFlags.NoWrap
                                       };

            _captionHeight = Math.Max(_fontCaption.Height, 18);

            SetColors();
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            _brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            _penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
            _penChart = new Pen(LayoutColors.ColorChartBalanceLine, 2);
        }

        /// <summary>
        /// Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(double point, double[] tickList)
        {
            _point = point;
            _tickData = tickList;
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

            Data.GradientPaint(g, _rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(_caption, _fontCaption, _brushCaption, _rectfCaption, _stringFormatCaption);
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (_tickData == null || _tickData.Length < 2)
            {
                string text = Language.T("Waiting for ticks...");
                g.DrawString(text, _fontCaption, _penChart.Brush, _chartArea);
                return;
            }

            int ticks = _tickData.Length;
            double maximum = double.MinValue;
            double minimum = double.MaxValue;
            foreach (double tick in _tickData)
            {
                if (maximum < tick) maximum = tick;
                if (minimum > tick) minimum = tick;
            }

            maximum += _point;
            minimum -= _point;

            const int space = Border + 2;
            const int xLeft = space;
            int xRight = ClientSize.Width - space;
            double scaleX = (xRight - xLeft)/((double) ticks - 1);

            int yTop = (int) _captionHeight + space;
            int yBottom = ClientSize.Height - space;
            double scaleY = (yBottom - yTop)/(maximum - minimum);

            int index = 0;
            var apntTick = new PointF[ticks];
            foreach (double tick in _tickData)
            {
                apntTick[index].X = (float) (xLeft + index*scaleX);
                apntTick[index].Y = (float) (yBottom - (tick - minimum)*scaleY);
                index++;
            }

            g.DrawLines(_penChart, apntTick);
        }

        /// <summary>
        /// Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            _chartArea = new Rectangle(Border, (int) _captionHeight, ClientSize.Width - 2*Border,
                                      ClientSize.Height - Border - (int) _captionHeight);
            Invalidate();
        }
    }
}