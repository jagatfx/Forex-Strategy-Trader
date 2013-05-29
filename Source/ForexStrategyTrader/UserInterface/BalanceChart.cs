//==============================================================
// Forex Strategy Builder
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
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public struct BalanceChartUnit
    {
        public double Balance { get; set; }
        public double Equity { get; set; }
        public DateTime Time { get; set; }
    }

    /// <summary>
    ///     Draws a balance chart
    /// </summary>
    public sealed class BalanceChart : Panel
    {
        private const int Border = 2;
        private const int Space = 5;
        private readonly float captionHeight;
        private readonly string chartTitle;
        private readonly Font font;
        private readonly StringFormat stringFormatCaption;
        private PointF[] apntBalance;
        private PointF[] apntEquity;
        private double[] balanceData;
        private float balanceY;
        private Brush brushFore;
        private Rectangle chartArea;
        private int chartPoints;
        private int cntLabels;
        private float deltaLabels;
        private double[] equityData;
        private float equityY;
        private int labelWidth;
        private int maxBalance;
        private int maxEquity;
        private int maxValue;
        private int minBalance;
        private int minEquity;
        private int minValue;
        private float netBalance;
        private float netEquity;
        private Pen penBorder;
        private Pen penGrid;
        private RectangleF rectfCaption;
        private float scaleX;
        private float scaleY;
        private DateTime startTime;
        private int stepLabels;
        private int xLeft;
        private int xRight;
        private int yBottom;
        private int yTop;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        public BalanceChart()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            SetColors();

            // Chart Title
            chartTitle = Language.T("Balance / Equity Chart");
            font = new Font(Font.FontFamily, 9);
            captionHeight = Math.Max(font.Height, 18);
            rectfCaption = new RectangleF(0, 0, ClientSize.Width, captionHeight);
            stringFormatCaption = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.NoWrap
                };

            penGrid.DashStyle = DashStyle.Dash;
            penGrid.DashPattern = new float[] {4, 2};
        }

        /// <summary>
        ///     Sets data to be displayed.
        /// </summary>
        public void UpdateChartData(BalanceChartUnit[] data, int points)
        {
            if (data == null || points < 1)
                return;

            balanceData = new double[points];
            equityData = new double[points];
            for (int p = 0; p < points; p++)
            {
                balanceData[p] = data[p].Balance;
                equityData[p] = data[p].Equity;
            }

            maxBalance = int.MinValue;
            minBalance = int.MaxValue;
            maxEquity = int.MinValue;
            minEquity = int.MaxValue;

            foreach (double balance in balanceData)
            {
                if (balance > maxBalance) maxBalance = (int) balance;
                if (balance < minBalance) minBalance = (int) balance;
            }

            foreach (double equity in equityData)
            {
                if (equity > maxEquity) maxEquity = (int) equity;
                if (equity < minEquity) minEquity = (int) equity;
            }

            startTime = data[0].Time;

            InitChart();
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
        private void InitChart()
        {
            if (balanceData == null || balanceData.Length < 1)
                return;

            chartPoints = Math.Max(balanceData.Length, equityData.Length);

            if (chartPoints == 0) return;

            maxValue = Math.Max(maxBalance, maxEquity) + 1;
            minValue = Math.Min(minBalance, minEquity) - 1;
            minValue = (int) (Math.Floor(minValue/10f)*10);

            yTop = (int) captionHeight + 2*Space + 1;
            yBottom = ClientSize.Height - Space - Border - Font.Height;

            Graphics g = CreateGraphics();
            labelWidth =
                (int)
                Math.Max(g.MeasureString(minValue.ToString("F2"), Font).Width,
                         g.MeasureString(maxValue.ToString("F2"), Font).Width);
            g.Dispose();

            labelWidth = Math.Max(labelWidth, 30);
            xLeft = Border + Space;
            xRight = ClientSize.Width - Border - Space - labelWidth - 6;
            scaleX = (xRight - 2*Space - Border - 10)/(float) (chartPoints - 1);

            cntLabels = Math.Max((yBottom - yTop)/40, 1);
            deltaLabels = (float) Math.Max(Math.Round((maxValue - minValue)/(float) cntLabels), 10);
            stepLabels = (int) Math.Ceiling(deltaLabels/10)*10;
            cntLabels = (int) Math.Ceiling((maxValue - minValue)/(float) stepLabels);
            maxValue = minValue + cntLabels*stepLabels;
            scaleY = (yBottom - yTop)/(cntLabels*(float) stepLabels);

            apntBalance = new PointF[chartPoints];
            apntEquity = new PointF[chartPoints];

            int index = 0;
            foreach (double balance in balanceData)
            {
                apntBalance[index].X = xLeft + index*scaleX;
                apntBalance[index].Y = (float) (yBottom - (balance - minValue)*scaleY);
                index++;
            }

            index = 0;
            foreach (double equity in equityData)
            {
                apntEquity[index].X = xLeft + index*scaleX;
                apntEquity[index].Y = (float) (yBottom - (equity - minValue)*scaleY);
                index++;
            }

            netBalance = (float) balanceData[balanceData.Length - 1];
            balanceY = yBottom - (netBalance - minValue)*scaleY;

            netEquity = (float) equityData[equityData.Length - 1];
            equityY = yBottom - (netEquity - minValue)*scaleY;
        }

        /// <summary>
        ///     Paints the chart
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

            // Caption bar
            Data.GradientPaint(g, rectfCaption, LayoutColors.ColorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(chartTitle, Font, new SolidBrush(LayoutColors.ColorCaptionText), rectfCaption,
                         stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, captionHeight, 1, ClientSize.Height);
            g.DrawLine(penBorder, ClientSize.Width - Border + 1, captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            if (balanceData == null || balanceData.Length < 1 ||
                equityData == null || equityData.Length < 1)
            {
                DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
                return;
            }

            // Grid and Price labels
            for (int iLabel = minValue; iLabel <= maxValue; iLabel += stepLabels)
            {
                var iLabelY = (int) (yBottom - (iLabel - minValue)*scaleY);
                g.DrawString(iLabel.ToString(".00"), Font, brushFore, xRight, iLabelY - Font.Height/2 - 1);
                g.DrawLine(penGrid, xLeft, iLabelY, xRight, iLabelY);
            }

            // Equity and Balance lines
            g.DrawLines(new Pen(LayoutColors.ColorChartEquityLine), apntEquity);
            g.DrawLines(new Pen(LayoutColors.ColorChartBalanceLine), apntBalance);

            // Coordinate axes
            g.DrawLine(new Pen(LayoutColors.ColorChartFore), xLeft - 1, yTop - Space, xLeft - 1,
                       yBottom + 1 + Font.Height);

            // Equity price label.
            var pntEquity = new Point(xRight - Space + 2, (int) (equityY - font.Height/2.0 - 1));
            var sizeEquity = new Size(labelWidth + Space, font.Height + 2);
            string equity = (netEquity.ToString("F2"));
            var halfEquity = (int) (sizeEquity.Height/2.0);
            var apEquity = new[]
                {
                    new PointF(xRight - Space - 8, equityY),
                    new PointF(xRight - Space, equityY - halfEquity),
                    new PointF(xRight - Space + sizeEquity.Width + 5, equityY - halfEquity),
                    new PointF(xRight - Space + sizeEquity.Width + 5, equityY + halfEquity),
                    new PointF(xRight - Space, equityY + halfEquity)
                };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartEquityLine), apEquity);
            g.DrawString(equity, font, new SolidBrush(LayoutColors.ColorChartBack), pntEquity);

            // Balance price label.
            var pntBalance = new Point(xRight - Space + 2, (int) (balanceY - font.Height/2.0 - 1));
            var sizeBalance = new Size(labelWidth + Space, font.Height + 2);
            var halfBalance = (int) (sizeBalance.Height/2.0);
            string balance = (netBalance.ToString("F2"));
            var apBalance = new[]
                {
                    new PointF(xRight - Space - 8, balanceY),
                    new PointF(xRight - Space, balanceY - halfBalance),
                    new PointF(xRight - Space + sizeBalance.Width + 5, balanceY - halfBalance),
                    new PointF(xRight - Space + sizeBalance.Width + 5, balanceY + halfBalance),
                    new PointF(xRight - Space, balanceY + halfBalance)
                };
            g.FillPolygon(new SolidBrush(LayoutColors.ColorChartBalanceLine), apBalance);
            g.DrawString(balance, font, new SolidBrush(LayoutColors.ColorChartBack), pntBalance);

            // Chart Text
            string chartText = startTime.ToString(CultureInfo.InvariantCulture);
            g.DrawString(chartText, font, new SolidBrush(LayoutColors.ColorChartFore), xLeft, yBottom);

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

            InitChart();
            Invalidate();
        }

        /// <summary>
        ///     Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            brushFore = new SolidBrush(LayoutColors.ColorChartFore);
            penGrid = new Pen(LayoutColors.ColorChartGrid);
            penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }
    }
}