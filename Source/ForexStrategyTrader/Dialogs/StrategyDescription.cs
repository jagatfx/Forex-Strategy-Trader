// Strategy_Description Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    public sealed class StrategyDescription : Form
    {
        private String _oldInfo;

        /// <summary>
        /// Make a form
        /// </summary>
        public StrategyDescription()
        {
            PnlBase = new Panel();
            PnlWarnBase = new FancyPanel();
            LblWarning = new Label();
            PnlTbxBase = new FancyPanel(Language.T("Strategy Description"));
            TxboxInfo = new TextBox();
            BtnClose = new Button();
            BtnAccept = new Button();
            BtnClear = new Button();

            AcceptButton = BtnClose;
            BackColor = LayoutColors.ColorFormBack;
            Icon = Data.Icon;
            MinimumSize = new Size(400, 400);
            Text = Language.T("Strategy Description");
            FormClosing += ActionsFormClosing;

            Controls.Add(PnlBase);
            Controls.Add(BtnAccept);
            Controls.Add(BtnClose);
            Controls.Add(BtnClear);

            // PnlWarnBase
            PnlWarnBase.Parent = this;
            PnlWarnBase.Padding = new Padding(2, 4, 2, 2);

            // LblWarning
            LblWarning.Parent = PnlWarnBase;
            LblWarning.TextAlign = ContentAlignment.MiddleCenter;
            LblWarning.BackColor = Color.Transparent;
            LblWarning.ForeColor = LayoutColors.ColorControlText;
            LblWarning.AutoSize = false;
            LblWarning.Dock = DockStyle.Fill;
            if (Data.Strategy.Description != "")
            {
                if (!Data.IsStrDescriptionRelevant())
                {
                    LblWarning.Font = new Font(Font, FontStyle.Bold);
                    LblWarning.Text = Language.T("This description might be outdated!");
                }
                else
                    LblWarning.Text = Path.GetFileNameWithoutExtension(Data.StrategyName);
            }
            else
                LblWarning.Text = Language.T("You can write a description of the strategy!");

            PnlTbxBase.Parent = PnlBase;
            PnlTbxBase.Padding = new Padding(4, (int)PnlTbxBase.CaptionHeight + 1, 2, 3);
            PnlTbxBase.Dock = DockStyle.Fill;


            // TxboxInfo
            TxboxInfo.Parent = PnlTbxBase;
            TxboxInfo.Dock = DockStyle.Fill;
            TxboxInfo.BackColor = LayoutColors.ColorControlBack;
            TxboxInfo.ForeColor = LayoutColors.ColorControlText;
            TxboxInfo.BorderStyle = BorderStyle.None;
            TxboxInfo.Multiline = true;
            TxboxInfo.AcceptsReturn = true;
            TxboxInfo.AcceptsTab = true;
            TxboxInfo.ScrollBars = ScrollBars.Vertical;
            TxboxInfo.KeyDown += TxboxInfo_KeyDown;
            TxboxInfo.Text = Data.Strategy.Description;
            TxboxInfo.Select(0, 0);

            // BtnClose
            BtnClose.Text = Language.T("Close");
            BtnClose.Click += BtnCloseClick;
            BtnClose.UseVisualStyleBackColor = true;

            // BtnAccept
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.Click += BtnAcceptClick;
            BtnAccept.UseVisualStyleBackColor = true;

            // BtnClear
            BtnClear.Text = Language.T("Clear");
            BtnClear.Click += BtnClearClick;
            BtnClear.UseVisualStyleBackColor = true;

            _oldInfo = Data.Strategy.Description;
        }

        private Panel PnlBase { get; set; }
        private FancyPanel PnlWarnBase { get; set; }
        private Label LblWarning { get; set; }
        private FancyPanel PnlTbxBase { get; set; }
        private TextBox TxboxInfo { get; set; }
        private Button BtnClose { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnClear { get; set; }

        /// <summary>
        /// Initialization
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Size = new Size(300, 380);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int)(Data.VerticalDlu * 15.5);
            var buttonWidth = (int)(Data.HorizontalDlu * 60);
            var btnVertSpace = (int)(Data.VerticalDlu * 5.5);
            var btnHrzSpace = (int)(Data.HorizontalDlu * 3);
            int border = btnHrzSpace;

            // Label Warning
            PnlWarnBase.Size = new Size(ClientSize.Width - 2 * border, 30);
            PnlWarnBase.Location = new Point(border, border);

            // Button Close
            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                          ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnClose.Left - BtnAccept.Width - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Clear
            BtnClear.Size = new Size(buttonWidth, buttonHeight);
            BtnClear.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace);

            // pnlBase
            PnlBase.Size = new Size(ClientSize.Width - 2 * border,
                                    BtnClose.Top - btnVertSpace - border - PnlWarnBase.Bottom - border);
            PnlBase.Location = new Point(border, PnlWarnBase.Bottom + border);
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
        private void TxboxInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && (e.KeyCode == Keys.A))
            {
                ((TextBox)sender).SelectAll();
                e.SuppressKeyPress = true;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Accept the changes.
        /// </summary>
        private void BtnAcceptClick(object sender, EventArgs e)
        {
            Data.Strategy.Description = TxboxInfo.Text;
            _oldInfo = TxboxInfo.Text;
            Close();
        }

        /// <summary>
        /// Closes the form.
        /// </summary>
        private void BtnCloseClick(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Cleans the info.
        /// </summary>
        private void BtnClearClick(object sender, EventArgs e)
        {
            TxboxInfo.Text = "";
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (_oldInfo == TxboxInfo.Text) return;
            DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the changes?"),
                                              Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (dr)
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    Data.Strategy.Description = TxboxInfo.Text;
                    _oldInfo = TxboxInfo.Text;
                    Close();
                    break;
                case DialogResult.No:
                    _oldInfo = TxboxInfo.Text;
                    Close();
                    break;
            }
        }
    }
}
