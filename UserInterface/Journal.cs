// Forex Strategy Trader
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    public enum JournalIcons
    {
        Information,
        System,
        Warning,
        Error,
        OK,
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
        private Brush _brushParams;
        private Color _colorBackroundEvenRows;
        private Color _colorBackroundOddRows;
        private Font _fontMessage;
        private HScrollBar _hScrollBar;
        private float _height;
        private List<JournalMessage> _messages;
        private float _requiredHeight;
        private float _rowHeight;
        private int _rows;
        private float _timeWidth;
        private VScrollBar _vScrollBar;
        private int _visibleRows;
        private float _width;

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Journal()
        {
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            _messages = new List<JournalMessage>();

            // Data row
            _fontMessage = new Font(Font.FontFamily, 9);
            _rowHeight = Math.Max(_fontMessage.Height + 4, 18F);

            Graphics g = CreateGraphics();
            _timeWidth = g.MeasureString(DateTime.Now.ToString(Data.DF + " HH:mm:ss"), _fontMessage).Width;
            g.Dispose();

            _hScrollBar = new HScrollBar
                              {
                                  Parent = this,
                                  Dock = DockStyle.Bottom,
                                  Enabled = false,
                                  Visible = false,
                                  SmallChange = 10,
                                  LargeChange = 30
                              };
            _hScrollBar.Scroll += ScrollBarScroll;

            _vScrollBar = new VScrollBar
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
            _vScrollBar.Scroll += ScrollBarScroll;
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            _colorBackroundEvenRows = LayoutColors.ColorEvenRowBack;
            _colorBackroundOddRows = LayoutColors.ColorOddRowBack;
            _brushParams = new SolidBrush(LayoutColors.ColorControlText);
        }

        /// <summary>
        /// Gets image for the icon type.
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
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
                case JournalIcons.OK:
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
        /// Update message list.
        /// </summary>
        public void UpdateMessages(List<JournalMessage> newMessages)
        {
            _messages = newMessages;

            _rows = _messages.Count;
            _requiredHeight = _rows*_rowHeight;

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        /// Clears all the messages.
        /// </summary>
        public void ClearMessages()
        {
            _messages = new List<JournalMessage>();

            _rows = _messages.Count;
            _requiredHeight = _rows*_rowHeight;

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        /// Selects the vertical scroll bar.
        /// </summary>
        public void SelectVScrollBar()
        {
            _vScrollBar.Select();
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            for (int row = 0; row*_rowHeight < _height; row++)
            {
                float vertPos = row*_rowHeight;

                // Row background
                var rectRow = new RectangleF(Space, vertPos, _width, _rowHeight);
                g.FillRectangle(
                    Math.Abs(row%2f) > 0.001
                        ? new SolidBrush(_colorBackroundEvenRows)
                        : new SolidBrush(_colorBackroundOddRows), rectRow);

                if (row + _vScrollBar.Value >= _rows)
                    continue;

                var pointMessage = new PointF(IconWidth + 2*Space, vertPos);
                int index = _rows - row - _vScrollBar.Value - 1;
                g.DrawImage(GetImage(_messages[index].Icon), Space, vertPos, IconWidth, IconHeight);
                string text = _messages[index].Time.ToString(Data.DF + " HH:mm:ss") + "   " + _messages[index].Message;
                g.DrawString(text, _fontMessage, _brushParams, pointMessage);
            }
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            CalculateScrollBarStatus();
            Invalidate();
        }

        /// <summary>
        /// Scroll Bars status
        /// </summary>
        private void CalculateScrollBarStatus()
        {
            _width = ClientSize.Width;
            _height = ClientSize.Height;

            bool mustHorisontal = _width < IconWidth + _timeWidth + MaxMessageWidth + 2*Space;
            bool mustVertical = _height < _requiredHeight;
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
                _height = ClientSize.Height - _hScrollBar.Height;
                isVertical = _height < _requiredHeight;
            }
            else if (mustVertical)
            {
                isVertical = true;
                _width = ClientSize.Width - _vScrollBar.Width - 2*Space;
                isHorisontal = _width < IconWidth + _timeWidth + MaxMessageWidth + 2*Space;
            }
            else
            {
                isHorisontal = false;
                isVertical = false;
            }

            if (isHorisontal)
                _height = ClientSize.Height - _hScrollBar.Height;

            if (isVertical)
                _width = ClientSize.Width - _vScrollBar.Width - 2*Space;

            _vScrollBar.Enabled = isVertical;
            _vScrollBar.Visible = isVertical;
            _hScrollBar.Enabled = isHorisontal;
            _hScrollBar.Visible = isHorisontal;

            _hScrollBar.Value = 0;
            if (isHorisontal)
            {
                var iPoinShort = (int) (IconWidth + _timeWidth + MaxMessageWidth + 2*Space - _width);
                _hScrollBar.Maximum = iPoinShort + _hScrollBar.LargeChange - Space;
            }

            _visibleRows = (int) Math.Min((_height/_rowHeight), _rows);

            _vScrollBar.Value = 0;
            _vScrollBar.Maximum = _rows - _visibleRows + _vScrollBar.LargeChange - 1;
        }

        /// <summary>
        /// ScrollBar_Scroll
        /// </summary>
        private void ScrollBarScroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }
    }
}
