// Strategy_Publish Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    public sealed class StrategyPublish : Form
    {
        /// <summary>
        /// Make a form
        /// </summary>
        public StrategyPublish()
        {
            PnlBBCodeBase = new FancyPanel();
            PnlInfoBase = new FancyPanel();
            TxboxBBCode = new TextBox();
            LblInformation = new Label();
            BtnClose = new Button();
            BtnConnect = new Button();

            // BBCode_viewer
            AcceptButton = BtnClose;
            BackColor = LayoutColors.ColorFormBack;
            Icon = Data.Icon;
            Controls.Add(BtnConnect);
            Controls.Add(BtnClose);
            Controls.Add(PnlBBCodeBase);
            Controls.Add(PnlInfoBase);
            MinimumSize = new Size(400, 400);
            Text = Language.T("Publish a Strategy");

            PnlBBCodeBase.Padding = new Padding(4, 4, 2, 2);
            PnlInfoBase.Padding = new Padding(4, 4, 2, 2);

            // TxboxBBCode
            TxboxBBCode.Parent = PnlBBCodeBase;
            TxboxBBCode.BorderStyle = BorderStyle.None;
            TxboxBBCode.Dock = DockStyle.Fill;
            TxboxBBCode.BackColor = LayoutColors.ColorControlBack;
            TxboxBBCode.ForeColor = LayoutColors.ColorControlText;
            TxboxBBCode.Multiline = true;
            TxboxBBCode.AcceptsReturn = true;
            TxboxBBCode.AcceptsTab = true;
            TxboxBBCode.ScrollBars = ScrollBars.Vertical;
            TxboxBBCode.KeyDown += TxboxBBCode_KeyDown;
            TxboxBBCode.Text = Data.Strategy.GenerateBBCode();

            // LblInformation
            LblInformation.Parent = PnlInfoBase;
            LblInformation.Dock = DockStyle.Fill;
            LblInformation.BackColor = Color.Transparent;
            LblInformation.ForeColor = LayoutColors.ColorControlText;
            string strInfo = Language.T("Publishing a strategy in the program's forum:") + Environment.NewLine +
                             "1) " + Language.T("Open a new topic in the forum;") + Environment.NewLine +
                             "2) " + Language.T("Copy / Paste the following code;") + Environment.NewLine +
                             "3) " + Language.T("Describe the strategy.");
            LblInformation.Text = strInfo;

            // BtnClose
            BtnClose.Text = Language.T("Close");
            BtnClose.Click += BtnCloseClick;
            BtnClose.UseVisualStyleBackColor = true;

            // BtnConnect
            BtnConnect.Text = Language.T("Connect to") + " http://forexsb.com/forum";
            BtnConnect.Click += BtnConnectClick;
            BtnConnect.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlBBCodeBase { get; set; }
        private FancyPanel PnlInfoBase { get; set; }
        private TextBox TxboxBBCode { get; set; }
        private Label LblInformation { get; set; }
        private Button BtnClose { get; set; }
        private Button BtnConnect { get; set; }

        /// <summary>
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var buttonWidth = (int)(Data.HorizontalDLU * 60);
            var btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            ClientSize = new Size(4 * buttonWidth + 3 * btnHrzSpace, 480);
            MinimumSize = new Size(Width, 300);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int)(Data.VerticalDLU * 15.5);
            var buttonWidth = (int)(Data.HorizontalDLU * 60);
            var btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            var btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int border = btnHrzSpace;

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Connect
            BtnConnect.Size = new Size(3 * buttonWidth, buttonHeight);
            BtnConnect.Location = new Point(BtnClose.Left - BtnConnect.Width - btnHrzSpace,
                                            ClientSize.Height - buttonHeight - btnVertSpace);

            // PnlInfoBase
            PnlInfoBase.Size = new Size(ClientSize.Width - 2 * border, 65);
            PnlInfoBase.Location = new Point(border, border);

            // PnlBBCodeBase
            PnlBBCodeBase.Size = new Size(ClientSize.Width - 2 * border,
                                          BtnClose.Top - PnlInfoBase.Bottom - btnVertSpace - border);
            PnlBBCodeBase.Location = new Point(border, PnlInfoBase.Bottom + border);
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Accept Ctrl-A
        /// </summary>
        private void TxboxBBCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Control || (e.KeyCode != Keys.A)) return;
            ((TextBox)sender).SelectAll();
            e.SuppressKeyPress = true;
            e.Handled = true;
        }

        /// <summary>
        /// Connects to the forum
        /// </summary>
        private void BtnConnectClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/forum/");
            }
            catch (Exception exception)
            {
                Console.WriteLine("StrategyPublish.BtnConnectClick: " + exception.Message);
            }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
