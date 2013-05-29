// Fancy_Message_Box Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    internal sealed class FancyMessageBox : Form
    {
        private WebBrowser Browser { get; set; }
        private Button BtnClose { get; set; }
        private FancyPanel PnlBase { get; set; }
        private Panel PnlControl { get; set; }

        private int _height = 230;
        private int _width = 380;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public FancyMessageBox(string text, string title)
        {
            PnlBase = new FancyPanel();
            PnlControl = new Panel();
            Browser = new WebBrowser();
            BtnClose = new Button();

            Text = title;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            TopMost = true;
            AcceptButton = BtnClose;

            PnlBase.Parent = this;

            Browser.Parent = PnlBase;
            Browser.AllowNavigation = false;
            Browser.AllowWebBrowserDrop = false;
            Browser.DocumentText = GetText(text, title);
            Browser.Dock = DockStyle.Fill;
            Browser.TabStop = false;
            Browser.IsWebBrowserContextMenuEnabled = false;
            Browser.WebBrowserShortcutsEnabled = true;

            PnlControl.Parent = this;
            PnlControl.Dock = DockStyle.Bottom;
            PnlControl.BackColor = Color.Transparent;

            BtnClose.Parent = PnlControl;
            BtnClose.Text = Language.T("Close");
            BtnClose.Name = "Close";
            BtnClose.Click += BtnCloseClick;
            BtnClose.UseVisualStyleBackColor = true;
        }

        public int BoxWidth
        {
            set { _width = value; }
        }

        public int BoxHeight
        {
            set { _height = value; }
        }

        /// <summary>
        /// Button Close OnClick.
        /// </summary>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Form OnLoad.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width = _width;
            Height = _height;
        }

        /// <summary>
        /// OnResize.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var iButtonHeight = (int) (Data.VerticalDlu*15.5);
            var iButtonWidth = (int) (Data.HorizontalDlu*60);
            var iBtnVertSpace = (int) (Data.VerticalDlu*5.5);
            var iBtnHrzSpace = (int) (Data.HorizontalDlu*3);
            int iBorder = iBtnHrzSpace;

            PnlControl.Height = iButtonHeight + 2*iBtnVertSpace;

            PnlBase.Size = new Size(ClientSize.Width - 2*iBorder, PnlControl.Top - iBorder);
            PnlBase.Location = new Point(iBorder, iBorder);

            BtnClose.Size = new Size(iButtonWidth, iButtonHeight);
            BtnClose.Location = new Point(ClientSize.Width - BtnClose.Width - iBtnHrzSpace, iBtnVertSpace);
        }

        /// <summary>
        /// Form OnPaint.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        private string GetText(string text, string title)
        {
            // Header
            string header = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
            header += "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
            header += "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />";
            header += "<title>" + title + "</title><style>";
            header += "body {margin: 0px; font-size: 14px; background-color: #fffffd}";
            header += ".content {padding: 5px;}";
            header += ".content h1 {margin: 0.5em 0 0.2em 0; font-weight: bold; font-size: 1.1em; color: #000033;}";
            header += ".content h2 {margin: 0.5em 0 0.2em 0; font-weight: bold; font-size: 1.0em; color: #000033;}";
            header += ".content p {margin-left: 5px; color: #000033;}";
            header += "</style></head>";
            header += "<body>";
            header += "<div class=\"content\">";

            // Footer
            const string footer = "</div></body></html>";

            return header + text + footer;
        }
    }
}