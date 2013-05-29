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
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;
using ForexStrategyBuilder.Utils;

namespace ForexStrategyBuilder
{
    public enum JournalIcons
    {
        Information,
        System,
        Warning,
        Error,
        Ok,
        Currency,
        Blocked,
        Globe,
        StartTrading,
        StopTrading,
        OrderBuy,
        OrderSell,
        OrderClose,
        Recalculate,
        BarOpen,
        BarClose,
        PosBuy,
        PosSell,
        PosSquare
    };

    public class JournalMessage
    {
        public JournalMessage(JournalIcons icon, DateTime time, string message)
        {
            Icon = icon;
            Time = time;
            Message = message;
        }

        public JournalIcons Icon { get; private set; }
        public DateTime Time { get; private set; }
        public string Message { get; private set; }
    }

    public class Journal : Panel
    {
        private const float IconHeight = 16;
        private const float IconWidth = 16;
        private const float MaxMessageWidth = 400;
        private const int Space = 2;
        private Brush brushParams;
        private Color colorBackroundEvenRows;
        private Color colorBackroundOddRows;
        private Font fontMessage;
        private HScrollBar hScrollBar;
        private List<JournalMessage> messages;
        private float pnlHeight;
        private float pnlWidth;
        private float requiredHeight;
        private float rowHeight;
        private int rows;
        private float timeWidth;
        private VScrollBar vScrollBar;
        private int visibleRows;

        /// <summary>
        ///     Default Constructor
        /// </summary>
        public Journal()
        {
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        ///     Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.Opaque,
                     true);

            messages = new List<JournalMessage>();

            // Data row
            fontMessage = new Font(Font.FontFamily, 9);
            rowHeight = Math.Max(fontMessage.Height + 4, 18F);

            Graphics g = CreateGraphics();
            timeWidth = g.MeasureString(DateTime.Now.ToString(Data.Df + " HH:mm:ss"), fontMessage).Width;
            g.Dispose();

            hScrollBar = new HScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Bottom,
                    Enabled = false,
                    Visible = false,
                    SmallChange = 10,
                    LargeChange = 30
                };
            hScrollBar.Scroll += ScrollBarScroll;

            vScrollBar = new VScrollBar
                {
                    Parent = this,
                    Dock = DockStyle.Right,
                    TabStop = true,
                    Enabled = false,
                    Visible = false,
                    SmallChange = 1,
                    LargeChange = 3,
                    Maximum = 20
                };
            vScrollBar.Scroll += ScrollBarScroll;
        }

        /// <summary>
        ///     Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            colorBackroundEvenRows = LayoutColors.ColorEvenRowBack;
            colorBackroundOddRows = LayoutColors.ColorOddRowBack;
            brushParams = new SolidBrush(LayoutColors.ColorControlText);
        }

        /// <summary>
        ///     Gets image for the icon type.
        /// </summary>
        /// <param name="icon"></param>
        private Image GetImage(JournalIcons icon)
        {
            Image image;
            switch (icon)
            {
                case JournalIcons.Information:
                    image = Resources.journal_information;
                    break;
                case JournalIcons.System:
                    image = Resources.journal_system;
                    break;
                case JournalIcons.Warning:
                    image = Resources.journal_warning;
                    break;
                case JournalIcons.Error:
                    image = Resources.journal_error;
                    break;
                case JournalIcons.Ok:
                    image = Resources.journal_ok;
                    break;
                case JournalIcons.Currency:
                    image = Resources.currency;
                    break;
                case JournalIcons.Blocked:
                    image = Resources.journal_blocked;
                    break;
                case JournalIcons.Globe:
                    image = Resources.globe;
                    break;
                case JournalIcons.StartTrading:
                    image = Resources.journal_start_trading;
                    break;
                case JournalIcons.StopTrading:
                    image = Resources.journal_stop_trading;
                    break;
                case JournalIcons.OrderBuy:
                    image = Resources.ord_buy;
                    break;
                case JournalIcons.OrderSell:
                    image = Resources.ord_sell;
                    break;
                case JournalIcons.OrderClose:
                    image = Resources.ord_close;
                    break;
                case JournalIcons.Recalculate:
                    image = Resources.recalculate;
                    break;
                case JournalIcons.BarOpen:
                    image = Resources.journal_bar_open;
                    break;
                case JournalIcons.BarClose:
                    image = Resources.journal_bar_close;
                    break;
                case JournalIcons.PosBuy:
                    image = Resources.pos_buy;
                    break;
                case JournalIcons.PosSell:
                    image = Resources.pos_sell;
                    break;
                case JournalIcons.PosSquare:
                    image = Resources.pos_square;
                    break;
                default:
                    image = Resources.journal_error;
                    break;
            }

            return image;
        }

        /// <summary>
        ///     Update message list.
        /// </summary>
        public void UpdateMessages(List<JournalMessage> newMessages)
        {
            messages = newMessages;

            rows = messages.Count;
            requiredHeight = rows*rowHeight;

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        ///     Clears all the messages.
        /// </summary>
        public void ClearMessages()
        {
            messages = new List<JournalMessage>();

            rows = messages.Count;
            requiredHeight = rows*rowHeight;

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        ///     Selects the vertical scroll bar.
        /// </summary>
        public void SelectVScrollBar()
        {
            vScrollBar.Select();
        }

        /// <summary>
        ///     On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            var bitmap = new Bitmap(ClientSize.Width, ClientSize.Height);
            Graphics g = Graphics.FromImage(bitmap);

            for (int row = 0; row*rowHeight < pnlHeight; row++)
            {
                float vertPos = row*rowHeight;

                // Row background
                var rectRow = new RectangleF(0, vertPos, pnlWidth, rowHeight);
                g.FillRectangle(
                    Math.Abs(row%2f) > 0.001
                        ? new SolidBrush(colorBackroundEvenRows)
                        : new SolidBrush(colorBackroundOddRows), rectRow);

                if (row + vScrollBar.Value >= rows)
                    continue;

                var pointMessage = new PointF(IconWidth + 2*Space, vertPos);
                int index = rows - row - vScrollBar.Value - 1;
                g.DrawImage(GetImage(messages[index].Icon), Space, vertPos, IconWidth, IconHeight);
                string text = messages[index].Time.ToString(Data.Df + " HH:mm:ss") + "   " + messages[index].Message;
                g.DrawString(text, fontMessage, brushParams, pointMessage);
            }

            DIBSection.DrawOnPaint(e.Graphics, bitmap, Width, Height);
        }

        /// <summary>
        ///     On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        ///     Scroll Bars status
        /// </summary>
        private void CalculateScrollBarStatus()
        {
            pnlWidth = ClientSize.Width;
            pnlHeight = ClientSize.Height;

            bool mustHorisontal = pnlWidth < IconWidth + timeWidth + MaxMessageWidth + 2*Space;
            bool mustVertical = pnlHeight < requiredHeight;
            bool isHorisontal;
            bool isVertical;

            if (mustHorisontal && mustVertical)
            {
                isVertical = true;
                isHorisontal = true;
            }
            else if (mustHorisontal)
            {
                isHorisontal = true;
                pnlHeight = ClientSize.Height - hScrollBar.Height;
                isVertical = pnlHeight < requiredHeight;
            }
            else if (mustVertical)
            {
                isVertical = true;
                pnlWidth = ClientSize.Width - vScrollBar.Width - 2*Space;
                isHorisontal = pnlWidth < IconWidth + timeWidth + MaxMessageWidth + 2*Space;
            }
            else
            {
                isHorisontal = false;
                isVertical = false;
            }

            if (isHorisontal)
                pnlHeight = ClientSize.Height - hScrollBar.Height;

            if (isVertical)
                pnlWidth = ClientSize.Width - vScrollBar.Width - 2*Space;

            vScrollBar.Enabled = isVertical;
            vScrollBar.Visible = isVertical;
            hScrollBar.Enabled = isHorisontal;
            hScrollBar.Visible = isHorisontal;

            hScrollBar.Value = 0;
            if (isHorisontal)
            {
                var poinShort = (int) (IconWidth + timeWidth + MaxMessageWidth + 2*Space - pnlWidth);
                hScrollBar.Maximum = poinShort + hScrollBar.LargeChange - Space;
            }

            visibleRows = (int) Math.Min((pnlHeight/rowHeight), rows);

            vScrollBar.Value = 0;
            vScrollBar.Maximum = rows - visibleRows + vScrollBar.LargeChange - 1;
        }

        /// <summary>
        ///     ScrollBar_Scroll
        /// </summary>
        private void ScrollBarScroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }
    }
}