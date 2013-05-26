// Starting_Tips Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace ForexStrategyBuilder
{
    internal sealed class StartingTips : Form
    {
        private WebBrowser Browser { get; set; }
        private Button BtnClose { get; set; }
        private Button BtnNextTip { get; set; }
        private Button BtnPrevTip { get; set; }
        private CheckBox ChboxShow { get; set; }
        private FancyPanel PnlBase { get; set; }
        private Panel PnlControl { get; set; }

        private readonly XmlDocument _xmlTips;
        private string _currentTip;
        private string _footer;
        private string _header;
        private int _indexTip;

        private bool _showAllTips;
        private bool _showTips;
        private int _tipsCount;

        /// <summary>
        /// Public Constructor
        /// </summary>
        public StartingTips()
        {
            PnlBase = new FancyPanel();
            PnlControl = new Panel();
            Browser = new WebBrowser();
            ChboxShow = new CheckBox();
            BtnNextTip = new Button();
            BtnPrevTip = new Button();
            BtnClose = new Button();

            _xmlTips = new XmlDocument();

            Text = Language.T("Tip of the Day");
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            TopMost = true;

            PnlBase.Parent = this;

            Browser.Parent = PnlBase;
            Browser.AllowNavigation = true;
            Browser.AllowWebBrowserDrop = false;
            Browser.DocumentText = Language.T("Loading...");
            Browser.Dock = DockStyle.Fill;
            Browser.TabStop = false;
            Browser.DocumentCompleted += BrowserDocumentCompleted;
            Browser.IsWebBrowserContextMenuEnabled = false;
            Browser.WebBrowserShortcutsEnabled = false;

            PnlControl.Parent = this;
            PnlControl.Dock = DockStyle.Bottom;
            PnlControl.BackColor = Color.Transparent;

            ChboxShow.Parent = PnlControl;
            ChboxShow.Text = Language.T("Show a tip");
            ChboxShow.Checked = Configs.ShowStartingTip;
            ChboxShow.TextAlign = ContentAlignment.MiddleLeft;
            ChboxShow.AutoSize = true;
            ChboxShow.ForeColor = LayoutColors.ColorControlText;
            ChboxShow.CheckStateChanged += ChboxShowCheckStateChanged;

            BtnNextTip.Parent = PnlControl;
            BtnNextTip.Text = Language.T("Next Tip");
            BtnNextTip.Name = "Next";
            BtnNextTip.Click += Navigate;
            BtnNextTip.UseVisualStyleBackColor = true;

            BtnPrevTip.Parent = PnlControl;
            BtnPrevTip.Text = Language.T("Previous Tip");
            BtnPrevTip.Name = "Previous";
            BtnPrevTip.Click += Navigate;
            BtnPrevTip.UseVisualStyleBackColor = true;

            BtnClose.Parent = PnlControl;
            BtnClose.Text = Language.T("Close");
            BtnClose.Name = "Close";
            BtnClose.Click += Navigate;
            BtnClose.UseVisualStyleBackColor = true;

            LoadStartingTips();
        }

        public bool ShowAllTips
        {
            set
            {
                _showAllTips = value;
                Browser.IsWebBrowserContextMenuEnabled = true;
                Browser.WebBrowserShortcutsEnabled = true;
            }
        }

        private int TipsCount
        {
            get { return _tipsCount; }
        }

        /// <summary>
        /// On Load
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width = (int)(Data.HorizontalDLU * 240);
            Height = (int)(Data.VerticalDLU * 140);
        }

        /// <summary>
        /// On Resize
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            var buttonHeight = (int)(Data.VerticalDLU * 15.5);
            var buttonWidth = (int)(Data.HorizontalDLU * 60);
            var btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            var btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int border = btnHrzSpace;

            PnlControl.Height = buttonHeight + 2 * btnVertSpace;

            PnlBase.Size = new Size(ClientSize.Width - 2 * border, PnlControl.Top - border);
            PnlBase.Location = new Point(border, border);

            ChboxShow.Location = new Point(btnHrzSpace, btnVertSpace + 5);

            BtnClose.Size = new Size(buttonWidth, buttonHeight);
            BtnClose.Location = new Point(ClientSize.Width - BtnNextTip.Width - btnHrzSpace, btnVertSpace);

            BtnNextTip.Size = new Size(buttonWidth, buttonHeight);
            BtnNextTip.Location = new Point(BtnClose.Left - BtnNextTip.Width - btnHrzSpace, btnVertSpace);

            BtnPrevTip.Size = new Size(buttonWidth, buttonHeight);
            BtnPrevTip.Location = new Point(BtnNextTip.Left - BtnPrevTip.Width - btnHrzSpace, btnVertSpace);

            // Resize if necessary
            if (BtnPrevTip.Left - ChboxShow.Right < btnVertSpace)
                Width += btnVertSpace - BtnPrevTip.Left + ChboxShow.Right;
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }

        /// <summary>
        /// The Document is ready
        /// </summary>
        private void BrowserDocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            Browser.DocumentCompleted -= BrowserDocumentCompleted;
            _indexTip--;
            ShowTip(true);
        }

        /// <summary>
        /// Change starting options
        /// </summary>
        private void ChboxShowCheckStateChanged(object sender, EventArgs e)
        {
            _showTips = ChboxShow.Checked;
            Configs.ShowStartingTip = _showTips;
            Configs.SaveConfigs();
        }

        /// <summary>
        /// Navigate
        /// </summary>
        private void Navigate(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            switch (btn.Name)
            {
                case "Previous":
                    ShowTip(false);
                    break;
                case "Next":
                    ShowTip(true);
                    break;
                case "Close":
                    Close();
                    break;
            }
        }

        /// <summary>
        /// Show random tip
        /// </summary>
        private void ShowTip(bool bNextTip)
        {
            if (TipsCount == 0)
                return;

            if (bNextTip)
            {
                if (_indexTip < TipsCount - 1)
                    _indexTip++;
                else
                    _indexTip = 0;
            }
            else
            {
                if (_indexTip > 0)
                    _indexTip--;
                else
                    _indexTip = TipsCount - 1;
            }

            if (_showAllTips)
            {
                var sbTips = new StringBuilder(TipsCount);

                var xmlNodeList = _xmlTips.SelectNodes("tips/tip");
                if (xmlNodeList != null)
                    foreach (XmlNode node in xmlNodeList)
                        sbTips.AppendLine(node.InnerXml);

                Browser.DocumentText = _header + sbTips + _footer;
            }
            else
            {
                var xmlNodeList = _xmlTips.SelectNodes("tips/tip");
                if (xmlNodeList != null)
                {
                    var xmlNode = xmlNodeList.Item(_indexTip);
                    if (xmlNode != null) _currentTip = xmlNode.InnerXml;
                }

                Browser.DocumentText = _header.Replace("###", (_indexTip + 1).ToString(CultureInfo.InvariantCulture)) + _currentTip + _footer;

                Configs.CurrentTipNumber = _indexTip;
            }
        }

        /// <summary>
        /// Load tips config file
        /// </summary>
        private void LoadStartingTips()
        {
            // Header
            _header = "<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">";
            _header += "<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">";
            _header += "<head><meta http-equiv=\"content-type\" content=\"text/html; charset=UTF-8\" />";
            _header += "<title>Tip of the Day</title><style>";
            _header += "body {margin: 0px; font-size: 14px; background-color: #fffffd}";
            _header += ".number {font-size: 9px}";
            _header += ".content {padding: 0 5px 5px 5px;}";
            _header += ".content h1 {margin: 0; font-weight: bold; font-size: 14px; color: #000033; text-align: center;}";
            _header += ".content p {margin-top: 0.5em; margin-bottom: 2px; color: #000033; text-indent: 1em;}";
            _header += "</style></head>";
            _header += "<body>";
            _header += "<div class=\"content\">";
            _header += "<div class=\"number\">(###)</div>";

            // Footer
            _footer = "</div></body></html>";

            _indexTip = Configs.CurrentTipNumber + 1;

            if (_showAllTips) _indexTip = 0;

            string sStartingTipsDir = Data.SystemDir + @"StartingTips";

            if (Directory.Exists(sStartingTipsDir) && Directory.GetFiles(sStartingTipsDir).Length > 0)
            {
                string[] asLangFiles = Directory.GetFiles(sStartingTipsDir);

                foreach (string langFile in asLangFiles)
                {
                    if (!langFile.EndsWith(".xml", true, null)) continue;
                    try
                    {
                        var xmlLanguage = new XmlDocument();
                        xmlLanguage.Load(langFile);
                        XmlNode node = xmlLanguage.SelectSingleNode("tips//language");

                        if (node == null)
                        {
                            // There is no language specified int the language file
                            string messageText = "Starting tip file: " + langFile + Environment.NewLine +
                                                  Environment.NewLine + "The language is not specified!";
                            MessageBox.Show(messageText, "Tips of the Day File Loading", MessageBoxButtons.OK,
                                            MessageBoxIcon.Exclamation);
                        }
                        else if (node.InnerText == Configs.Language)
                        {
                            // It looks OK
                            _xmlTips.Load(langFile);
                            var xmlNodeList = _xmlTips.SelectNodes("tips/tip");
                            if (xmlNodeList != null)
                                _tipsCount = xmlNodeList.Count;
                        }
                    }
                    catch (Exception e)
                    {
                        string messageText = "Starting tip file: " + langFile + Environment.NewLine +
                                              Environment.NewLine +
                                              "Error in the starting tip file!" + Environment.NewLine +
                                              Environment.NewLine + e.Message;
                        MessageBox.Show(messageText, "Tips of the Day File Loading", MessageBoxButtons.OK,
                                        MessageBoxIcon.Exclamation);
                    }
                }
            }

            if (Configs.Language != "English" && TipsCount == 0)
            {
                try
                {
                    // The tips file
                    _xmlTips.Load(Data.SystemDir + "StartingTips" + Path.DirectorySeparatorChar + "English.xml");
                    var xmlNodeList = _xmlTips.SelectNodes("tips/tip");
                    if (xmlNodeList != null) _tipsCount = xmlNodeList.Count;
                }
                catch (Exception e)
                {
                    string messageText = "Starting tip file \"English.xml\"" + Environment.NewLine +
                                          Environment.NewLine +
                                          "Error in the starting tip file!" + Environment.NewLine + Environment.NewLine +
                                          e.Message;
                    MessageBox.Show(messageText, "Tips of the Day File Loading", MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                }
            }
        }
    }
}
