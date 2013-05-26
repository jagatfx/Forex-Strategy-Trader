// AboutScreen Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public sealed class AboutScreen : Form
    {
        private Button BtnOk { get; set; }
        private Label LblContacts { get; set; }
        private Label LblCopyright { get; set; }
        private Label LblExpertVersion { get; set; }
        private Label LblLibraryVersion { get; set; }
        private Label LblProgramName { get; set; }
        private Label LblProgramVersion { get; set; }
        private Label LblSupportForum { get; set; }
        private Label LblWebsite { get; set; }
        private LinkLabel LinkEmail { get; set; }
        private LinkLabel LinkForum { get; set; }
        private LinkLabel LinkWebsite { get; set; }
        private LinkLabel LinkCredits { get; set; }
        private PictureBox PictureLogo { get; set; }
        private FancyPanel PnlBase { get; set; }

        public AboutScreen()
        {
            PnlBase = new FancyPanel();
            LblProgramName = new Label();
            LblProgramVersion = new Label();
            LblLibraryVersion = new Label();
            LblExpertVersion = new Label();
            LblCopyright = new Label();
            LblWebsite = new Label();
            LblSupportForum = new Label();
            LblContacts = new Label();
            PictureLogo = new PictureBox();
            LinkWebsite = new LinkLabel();
            LinkForum = new LinkLabel();
            LinkEmail = new LinkLabel();
            LinkCredits = new LinkLabel();
            BtnOk = new Button();

            // Panel Base
            PnlBase.Parent = this;

            PictureLogo.TabStop = false;
            PictureLogo.BackColor = Color.Transparent;
            PictureLogo.Image = Resources.Logo;

            LblProgramName.AutoSize = true;
            LblProgramName.Font = new Font("Microsoft Sans Serif", 16F, FontStyle.Bold);
            LblProgramName.ForeColor = LayoutColors.ColorControlText;
            LblProgramName.BackColor = Color.Transparent;
            LblProgramName.Text = Data.ProgramName;

            LblProgramVersion.AutoSize = true;
            LblProgramVersion.Font = new Font("Microsoft Sans Serif", 12F);
            LblProgramVersion.ForeColor = LayoutColors.ColorControlText;
            LblProgramVersion.BackColor = Color.Transparent;
            LblProgramVersion.Text = Language.T("Program version") + ": " + Data.ProgramVersion +
                                     (Data.IsProgramBeta ? " " + Language.T("Beta") : "");

            LblLibraryVersion.AutoSize = true;
            LblLibraryVersion.Font = new Font("Microsoft Sans Serif", 10F);
            LblLibraryVersion.ForeColor = LayoutColors.ColorControlText;
            LblLibraryVersion.BackColor = Color.Transparent;
            LblLibraryVersion.Text = Language.T("Library version") + ": " + Data.LibraryVersion;

            // label4
            LblExpertVersion.AutoSize = true;
            LblExpertVersion.Font = new Font("Microsoft Sans Serif", 10F);
            LblExpertVersion.ForeColor = LayoutColors.ColorControlText;
            LblExpertVersion.BackColor = Color.Transparent;
            LblExpertVersion.Text = Language.T("Expert version") + ": " + Data.ExpertVersion;

            // label5
            LblCopyright.AutoSize = true;
            LblCopyright.Font = new Font("Microsoft Sans Serif", 10F);
            LblCopyright.ForeColor = LayoutColors.ColorControlText;
            LblCopyright.BackColor = Color.Transparent;
            LblCopyright.Text = "Copyright (c) 2013 Miroslav Popov" + Environment.NewLine +
                                Language.T("Distributor") + " - Forex Software Ltd." + Environment.NewLine +
                                Environment.NewLine +
                                Language.T("This is a freeware program!");

            // label6
            LblWebsite.AutoSize = true;
            LblWebsite.ForeColor = LayoutColors.ColorControlText;
            LblWebsite.BackColor = Color.Transparent;
            LblWebsite.Text = Language.T("Website") + ":";

            // label7
            LblSupportForum.AutoSize = true;
            LblSupportForum.ForeColor = LayoutColors.ColorControlText;
            LblSupportForum.BackColor = Color.Transparent;
            LblSupportForum.Text = Language.T("Support forum") + ":";

            // label8
            LblContacts.AutoSize = true;
            LblContacts.ForeColor = LayoutColors.ColorControlText;
            LblContacts.BackColor = Color.Transparent;
            LblContacts.Text = Language.T("Contacts") + ":";

            // llWebsite
            LinkWebsite.AutoSize = true;
            LinkWebsite.TabStop = true;
            LinkWebsite.BackColor = Color.Transparent;
            LinkWebsite.Text = "http://forexsb.com";
            LinkWebsite.Tag = "http://forexsb.com/";
            LinkWebsite.LinkClicked += WebsiteLinkClicked;

            // llForum
            LinkForum.AutoSize = true;
            LinkForum.TabStop = true;
            LinkForum.BackColor = Color.Transparent;
            LinkForum.Text = "http://forexsb.com/forum";
            LinkForum.Tag = "http://forexsb.com/forum/";
            LinkForum.LinkClicked += WebsiteLinkClicked;

            // llEmail
            LinkEmail.AutoSize = true;
            LinkEmail.TabStop = true;
            LinkEmail.BackColor = Color.Transparent;
            LinkEmail.Text = "info@forexsb.com";
            LinkEmail.Tag = "mailto:info@forexsb.com";
            LinkEmail.LinkClicked += WebsiteLinkClicked;

            // LlCredits
            LinkCredits.AutoSize = true;
            LinkCredits.TabStop = true;
            LinkCredits.BackColor = Color.Transparent;
            LinkCredits.Text = Language.T("Credits and Contributors");
            LinkCredits.Tag = "http://forexsb.com/wiki/credits";
            LinkCredits.LinkClicked += WebsiteLinkClicked;

            // Button Base
            BtnOk.Parent = this;
            BtnOk.Text = Language.T("Ok");
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += BtnOkClick;

            // AboutScreen
            PnlBase.Controls.Add(LblProgramName);
            PnlBase.Controls.Add(LblProgramVersion);
            PnlBase.Controls.Add(LblLibraryVersion);
            PnlBase.Controls.Add(LblExpertVersion);
            PnlBase.Controls.Add(LblCopyright);
            PnlBase.Controls.Add(LblWebsite);
            PnlBase.Controls.Add(LblSupportForum);
            PnlBase.Controls.Add(LblContacts);
            PnlBase.Controls.Add(LinkWebsite);
            PnlBase.Controls.Add(LinkForum);
            PnlBase.Controls.Add(LinkEmail);
            PnlBase.Controls.Add(LinkCredits);
            PnlBase.Controls.Add(PictureLogo);

            StartPosition = FormStartPosition.CenterScreen;
            Text = Language.T("About") + " " + Data.ProgramName;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            BackColor = LayoutColors.ColorFormBack;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(360, 350);
        }

        /// <summary>
        /// Form On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int hrzSpace = btnHrzSpace;

            BtnOk.Size = new Size(buttonWidth, buttonHeight);
            BtnOk.Location = new Point(ClientSize.Width - BtnOk.Width - hrzSpace,
                                       ClientSize.Height - BtnOk.Height - btnVertSpace);
            PnlBase.Size = new Size(ClientSize.Width - 2*hrzSpace, BtnOk.Top - hrzSpace - btnVertSpace);
            PnlBase.Location = new Point(hrzSpace, hrzSpace);

            PictureLogo.Location = new Point(10, 3);
            PictureLogo.Size = new Size(48, 48);
            LblProgramName.Location = new Point(63, 10);
            LblProgramVersion.Location = new Point(65, 45);
            LblLibraryVersion.Location = new Point(66, 65);
            LblExpertVersion.Location = new Point(66, 85);
            LblCopyright.Location = new Point(67, 117);
            LblWebsite.Location = new Point(67, 200);
            LblSupportForum.Location = new Point(67, 220);
            LblContacts.Location = new Point(68, 240);
            LinkWebsite.Location = new Point(LblSupportForum.Right + 5, LblWebsite.Top);
            LinkForum.Location = new Point(LblSupportForum.Right + 5, LblSupportForum.Top);
            LinkEmail.Location = new Point(LblSupportForum.Right + 5, LblContacts.Top);
            LinkCredits.Location = new Point((PnlBase.Width - LinkCredits.Width) / 2, 270);
            PnlBase.Invalidate();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// Connects to the web site
        /// </summary>
        private void WebsiteLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var label = sender as Label;
            if (label == null) return;
            try
            {
                Process.Start(label.Tag.ToString());
            }
            catch (Exception exception)
            {
                Console.WriteLine("WebsiteLinkClicked: " + exception.Message);
            }
        }

        /// <summary>
        /// Closes the form
        /// </summary>
        private void BtnOkClick(object sender, EventArgs e)
        {
            Close();
        }
    }
}