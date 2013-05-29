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
using System.Windows.Forms;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Draws a small balance chart
    /// </summary>
    public sealed class TickChart : Panel
    {
        private const int Border = 2;
        private readonly string caption;
        private readonly float captionHeight;
        private readonly Font fontCaption;
        private readonly StringFormat stringFormatCaption;
        private Brush brushCaption;
        private Rectangle chartArea;
        private Pen penBorder;
        private Pen penChart;
        private double point;
        private RectangleF rectfCaption;

        private double[] tickData;

        /// <summary>
        ///     Constructor
        /// </summary>
        public TickChart(string caption)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            this.caption = caption;

            fontCaption = new Font(Font.FontFamily, 9);
            stringFormatCaption = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

            captionHeight = Math.Max(fontCaption.Height, 18);

            SetColors();
        }

        /// <summary>
        ///     Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
            penChart = new Pen(LayoutColors.ColorChartBalanceLine, 2);
        }

        /// <summary>
        ///     Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(double pointSize, double[] tickList)
        {
            point = pointSize;
            tickData = tickList;
        }

        /// <summary>
        ///     Refreshes the chart.
        /// </summary>
        public void RefreshChart()
        {
            Invalidate(chartArea);
        }

        /// <summary>
        ///     Sets the chart params
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
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

            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(caption, fontCaption, brushCaption, rectfCaption, stringFormatCaption);
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (tickData == null || tickData.Length < 2)
            {
                string text = Language.T("Waiting for ticks...");
                g.DrawString(text, fontCaption, penChart.Brush, chartArea);
                DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
                return;
            }

            int ticks = tickData.Length;
            double maximum = double.MinValue;
            double minimum = double.MaxValue;
            foreach (double tick in tickData)
            {
                if (maximum < tick) maximum = tick;
                if (minimum > tick) minimum = tick;
            }

            maximum += point;
            minimum -= point;

            const int space = Border + 2;
            const int xLeft = space;
            int xRight = ClientSize.Width - space;
            double scaleX = (xRight - xLeft)/((double) ticks - 1);

            int yTop = (int) captionHeight + space;
            int yBottom = ClientSize.Height - space;
            double scaleY = (yBottom - yTop)/(maximum - minimum);

            int index = 0;
            var apntTick = new PointF[ticks];
            foreach (double tick in tickData)
            {
                apntTick[index].X = (float) (xLeft + index*scaleX);
                apntTick[index].Y = (float) (yBottom - (tick - minimum)*scaleY);
                index++;
            }

            g.DrawLines(penChart, apntTick);
            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     Invalidates the chart after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            chartArea = new Rectangle(Border, (int) captionHeight, ClientSize.Width - 2*Border,
                                      ClientSize.Height - Border - (int) captionHeight);
            Invalidate();
        }
    }
}