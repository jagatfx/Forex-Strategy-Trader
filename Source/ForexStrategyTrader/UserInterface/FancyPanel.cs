// Fancy_Panel
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    public class FancyPanel : Panel
    {
        private const int Border = 2;
        private readonly string _caption;
        private readonly bool _isShowCaption = true;
        private Brush _brushCaption;
        private float _captionHeight;
        private Color _colorCaptionBack;
        private Font _fontCaption;
        private float _height;
        private Pen _penBorder;
        private RectangleF _rectfCaption;
        private StringFormat _stringFormatCaption;
        private float _width;

        /// <summary>
        /// Default constructor
        /// </summary>
        public FancyPanel()
        {
            _caption = "";
            _isShowCaption = false;
            InitializeParameters();
            SetColors();
            Padding = new Padding(Border, 2*Border, Border, Border);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FancyPanel(string sCaption)
        {
            _caption = sCaption;
            InitializeParameters();
            SetColors();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public FancyPanel(string caption, Color colorCaption, Color colorCaptionText)
        {
            _caption = caption;
            _colorCaptionBack = colorCaption;
            _brushCaption = new SolidBrush(colorCaptionText);
            _penBorder = new Pen(Data.ColorChanage(colorCaption, -LayoutColors.DepthCaption), Border);

            InitializeParameters();
        }

        /// <summary>
        /// Gets the caption height.
        /// </summary>
        public float CaptionHeight
        {
            get { return _captionHeight; }
        }

        /// <summary>
        /// Gets the client rectangle.
        /// </summary>
        protected Rectangle InnerRectangle
        {
            get
            {
                return new Rectangle(Border, (int) _captionHeight, Width - 2*Border,
                                     Height - Border - (int) _captionHeight);
            }
        }

        /// <summary>
        /// Sets the panel colors
        /// </summary>
        public void SetColors()
        {
            _colorCaptionBack = LayoutColors.ColorCaptionBack;
            _brushCaption = new SolidBrush(LayoutColors.ColorCaptionText);
            _penBorder = new Pen(Data.ColorChanage(LayoutColors.ColorCaptionBack, -LayoutColors.DepthCaption), Border);
        }

        /// <summary>
        /// Initialize Parameters
        /// </summary>
        private void InitializeParameters()
        {
            _fontCaption = new Font(Font.FontFamily, 9);
            _stringFormatCaption = new StringFormat
                                       {
                                           Alignment = StringAlignment.Center,
                                           LineAlignment = StringAlignment.Center,
                                           Trimming = StringTrimming.EllipsisCharacter,
                                           FormatFlags = StringFormatFlags.NoWrap
                                       };

            _captionHeight = _isShowCaption ? Math.Max(_fontCaption.Height, 18) : 2*Border;
        }

        /// <summary>
        /// On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // Caption
            Data.GradientPaint(g, _rectfCaption, _colorCaptionBack, LayoutColors.DepthCaption);
            if (_isShowCaption)
            {
                g.DrawString(_caption, _fontCaption, _brushCaption, _rectfCaption, _stringFormatCaption);
            }
            g.DrawLine(_penBorder, 1, _captionHeight, 1, ClientSize.Height);
            g.DrawLine(_penBorder, ClientSize.Width - Border + 1, _captionHeight, ClientSize.Width - Border + 1,
                       ClientSize.Height);
            g.DrawLine(_penBorder, 0, ClientSize.Height - Border + 1, ClientSize.Width, ClientSize.Height - Border + 1);

            // Paint the panel background
            var rectClient = new RectangleF(Border, _captionHeight, _width, _height);
            Data.GradientPaint(g, rectClient, LayoutColors.ColorControlBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            if (_isShowCaption)
            {
                _width = ClientSize.Width - 2*Border;
                _height = ClientSize.Height - _captionHeight - Border;
                _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            }
            else
            {
                _width = ClientSize.Width - 2*Border;
                _height = ClientSize.Height - _captionHeight - Border;
                _rectfCaption = new RectangleF(0, 0, ClientSize.Width, _captionHeight);
            }

            Invalidate();
        }
    }
}