// Status page - Controls class 
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
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private Button BtnShowAccountInfo { get; set; }
        private Button BtnShowBars{ get; set; }
        private Button BtnShowMarketInfo{ get; set; }
        private Label LblConnection{ get; set; }
        private FancyPanel PnlConnection{ get; set; }
        private FancyPanel PnlDataInfoBase{ get; set; }
        private Panel PnlDataInfoButtons{ get; set; }
        protected LinkPanel PnlForexBrokers{ get; private set; }
        private InfoPanel PnlMarketInfo{ get; set; }
        protected LinkPanel PnlUsefulLinks{ get; private set; }
        private TextBox TbxDataInfo{ get; set; }

        /// <summary>
        /// Sets the controls in tabPageStatus
        /// </summary>
        private void InitializePageStatus()
        {
            // tabPageStatus
            TabPageStatus.Name = "tabPageStatus";
            TabPageStatus.Text = Language.T("Status");
            TabPageStatus.ImageIndex = 0;
            TabPageStatus.Resize += TabPageStatusResize;

            // Panel Connection
            PnlConnection = new FancyPanel(Language.T("Connection Status")) {Parent = TabPageStatus};

            // lblConnection
            LblConnection = new Label
                                {
                                    Name = "lblConnection",
                                    Parent = PnlConnection,
                                    Text = Language.T("Not Connected. You have to connect to a MetaTrader terminal."),
                                    TextAlign = ContentAlignment.MiddleLeft
                                };

            // Panel Data Info
            PnlDataInfoBase = new FancyPanel(Language.T("Data Info")) {Parent = TabPageStatus};
            PnlDataInfoBase.Padding = new Padding(2, (int) PnlDataInfoBase.CaptionHeight, 2, 2);

            TbxDataInfo = new TextBox
                              {
                                  Parent = PnlDataInfoBase,
                                  BorderStyle = BorderStyle.None,
                                  Dock = DockStyle.Fill,
                                  TabStop = false,
                                  Multiline = true,
                                  AcceptsReturn = true,
                                  AcceptsTab = true,
                                  WordWrap = false,
                                  ScrollBars = ScrollBars.Vertical,
                                  Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((204)))
                              };

            PnlDataInfoButtons = new Panel {Parent = PnlDataInfoBase, Dock = DockStyle.Top};
            PnlDataInfoButtons.Paint += PnlDataInfoButtons_Paint;

            BtnShowMarketInfo = new Button {Parent = PnlDataInfoButtons, Text = Language.T("Market Info")};
            BtnShowMarketInfo.Click += BtnShowMarketInfoClick;
            BtnShowMarketInfo.UseVisualStyleBackColor = true;

            BtnShowAccountInfo = new Button {Parent = PnlDataInfoButtons, Text = Language.T("Account Info")};
            BtnShowAccountInfo.Click += BtnShowAccountInfoClick;
            BtnShowAccountInfo.UseVisualStyleBackColor = true;

            BtnShowBars = new Button {Parent = PnlDataInfoButtons, Text = Language.T("Loaded Bars")};
            BtnShowBars.Click += BtnShowBarsClick;
            BtnShowBars.UseVisualStyleBackColor = true;

            PnlMarketInfo = new InfoPanel(Language.T("Market Information")) {Parent = TabPageStatus};
            PnlUsefulLinks = new LinkPanel(Language.T("Useful Links")) {Parent = TabPageStatus};
            PnlForexBrokers = new LinkPanel(Language.T("Forex Brokers")) {Parent = TabPageStatus};

            SetStatusColors();
        }

        private void PnlDataInfoButtons_Paint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;

            // Paint the panel background
            Data.GradientPaint(g, pnl.ClientRectangle, LayoutColors.ColorControlBack, LayoutColors.DepthCaption);
        }

        /// <summary>
        /// Sets controls size and position after resizing.
        /// </summary>
        private void TabPageStatusResize(object sender, EventArgs e)
        {
            TabPageStatus.SuspendLayout();

            const int border = 2;

            int iWidth = TabPageStatus.ClientSize.Width;
            int iHeight = TabPageStatus.ClientSize.Height;
            PnlMarketInfo.Size = new Size(220, 150);
            PnlMarketInfo.Location = new Point(iWidth - PnlMarketInfo.Width, 0);

            PnlConnection.Size = new Size(PnlMarketInfo.Left - Space, 110);
            PnlConnection.Location = new Point(0, 0);

            LblConnection.Size = new Size(PnlConnection.Width - 2*Space - 2*border, 21);
            LblConnection.Location = new Point(border + Space, (int) PnlConnection.CaptionHeight + Space);

            PnlDataInfoBase.Size = new Size(PnlConnection.Width, iHeight - PnlConnection.Bottom - Space);
            PnlDataInfoBase.Location = new Point(0, PnlConnection.Bottom + Space);

            PnlUsefulLinks.Size = new Size(PnlMarketInfo.Width, (iHeight - PnlMarketInfo.Bottom - 2*Space)/2);
            PnlUsefulLinks.Location = new Point(PnlMarketInfo.Left, PnlMarketInfo.Bottom + Space);

            PnlForexBrokers.Size = new Size(PnlMarketInfo.Width, iHeight - PnlUsefulLinks.Bottom - Space);
            PnlForexBrokers.Location = new Point(PnlMarketInfo.Left, PnlUsefulLinks.Bottom + Space);

            const int buttonWith = 100;
            int buttonHeight = BtnShowAccountInfo.Height;
            PnlDataInfoButtons.Height = buttonHeight + 2*Space;

            BtnShowMarketInfo.Width = buttonWith;
            BtnShowAccountInfo.Width = buttonWith;
            BtnShowBars.Width = buttonWith;
            BtnShowMarketInfo.Location = new Point(Space, Space);
            BtnShowAccountInfo.Location = new Point(BtnShowMarketInfo.Right + Space, Space);
            BtnShowBars.Location = new Point(BtnShowAccountInfo.Right + Space, Space);

            TabPageStatus.ResumeLayout();
        }

        /// <summary>
        /// sets colors of controls in Status page.
        /// </summary>
        private void SetStatusColors()
        {
            TabPageStatus.BackColor = LayoutColors.ColorFormBack;

            LblConnection.BackColor = Color.Transparent;
            LblConnection.ForeColor = LayoutColors.ColorControlText;

            TbxDataInfo.BackColor = LayoutColors.ColorControlBack;
            TbxDataInfo.ForeColor = LayoutColors.ColorControlText;

            PnlDataInfoBase.SetColors();
            PnlMarketInfo.SetColors();
            PnlConnection.SetColors();
            PnlUsefulLinks.SetColors();
            PnlForexBrokers.SetColors();
        }

        protected virtual void BtnShowMarketInfoClick(object sender, EventArgs e)
        {
        }

        protected virtual void BtnShowBarsClick(object sender, EventArgs e)
        {
        }

        protected virtual void BtnShowAccountInfoClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Sets the lblConnection.Text
        /// </summary>
        protected void SetLblConnectionText(string text)
        {
            if (LblConnection.InvokeRequired)
            {
                LblConnection.BeginInvoke(new SetLblConnectionDelegate(SetLblConnectionText), new object[] {text});
            }
            else
            {
                LblConnection.Text = text;
            }
        }

        /// <summary>
        /// Sets the tbxBarData.Text
        /// </summary>
        protected void SetBarDataText(string text)
        {
            if (TbxDataInfo.InvokeRequired)
            {
                TbxDataInfo.BeginInvoke(new SetBarDataTextDelegate(SetBarDataText), new object[] {text});
            }
            else
            {
                TbxDataInfo.Text = text;
            }
        }

        /// <summary>
        /// Sets the tbxBarData.Text
        /// </summary>
        protected void UpdateStatusPageMarketInfo(string[] values)
        {
            if (PnlMarketInfo.InvokeRequired)
            {
                PnlMarketInfo.BeginInvoke(new UpdateMarketInfoDelegate(UpdateStatusPageMarketInfo),
                                          new object[] {values});
            }
            else
            {
                string caption = Language.T("Market Information");
                var parameters = new[]
                                     {
                                         Language.T("Symbol"),
                                         Language.T("Period"),
                                         Language.T("Lot size"),
                                         Language.T("Point"),
                                         Language.T("Spread"),
                                         Language.T("Swap long"),
                                         Language.T("Swap short")
                                     };
                PnlMarketInfo.Update(parameters, values, caption);
            }
        }

        #region Nested type: SetBarDataTextDelegate

        private delegate void SetBarDataTextDelegate(string text);

        #endregion

        #region Nested type: SetLblConnectionDelegate

        private delegate void SetLblConnectionDelegate(string text);

        #endregion

        #region Nested type: UpdateMarketInfoDelegate

        private delegate void UpdateMarketInfoDelegate(string[] info);

        #endregion
    }
}