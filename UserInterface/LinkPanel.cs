// Forex Strategy Trader
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    public class LinkItem
    {
        /// <summary>
        /// Public constructor.
        /// </summary>
        public LinkItem(string text, string url, string comment)
        {
            Text = text;
            Url = url;
            Comment = comment;
        }

        /// <summary>
        /// Sets the text of link.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Sets the web address of link.
        /// </summary>
        public string Url { get; private set; }

        /// <summary>
        /// Sets the comment to the link.
        /// </summary>
        public string Comment { get; private set; }
    }

    public class LinkPanel : FancyPanel
    {
        private readonly FlowLayoutPanel _holder = new FlowLayoutPanel();
        private readonly List<LinkItem> _links = new List<LinkItem>();

        /// <summary>
        /// Constructor.
        /// </summary>
        public LinkPanel(string caption) :
            base(caption)
        {
        }

        /// <summary>
        /// Adds a link to the LinkPanel.
        /// </summary>
        public void AddLink(LinkItem link)
        {
            _links.Add(link);
        }

        /// <summary>
        /// Arranges the link labels.
        /// </summary>
        public void SetLinks()
        {
            int linksHeight = 0;
            int linksWidth = 0;

            foreach (LinkItem link in _links)
            {
                var label = new LinkLabel
                                {
                                    BackColor = Color.Transparent,
                                    AutoSize = false,
                                    Width = _holder.ClientSize.Width - 15,
                                    Height = Font.Height + 3,
                                    AutoEllipsis = true,
                                    LinkBehavior = LinkBehavior.NeverUnderline,
                                    Text = "     " + link.Text,
                                    Image = Resources.globe,
                                    ImageAlign = ContentAlignment.MiddleLeft,
                                    TextAlign = ContentAlignment.MiddleLeft,
                                    Tag = link.Url
                                };
                label.Font = new Font(label.Font.FontFamily, label.Font.Size, FontStyle.Regular);
                label.Margin = new Padding(0, 5, 0, 0);
                label.Padding = new Padding(0);
                label.LinkClicked += Label_LinkClicked;

                if (link.Comment != string.Empty)
                {
                    var tooltip = new ToolTip();
                    tooltip.SetToolTip(label, link.Comment);
                }

                _holder.Controls.Add(label);
                linksHeight = label.Bottom;
                linksWidth = label.Right;
            }

            _holder.Parent = this;
            _holder.BackColor = Color.Transparent;
            _holder.FlowDirection = FlowDirection.TopDown;
            _holder.Padding = new Padding(10, 0, 5, 5);
            _holder.AutoScroll = true;
            _holder.AutoScrollMinSize = new Size(linksWidth + 5, linksHeight + 5);
            _holder.Scroll += LinkHolderScroll;
        }

        /// <summary>
        /// Repaints panel after scrolling.
        /// </summary>
        private void LinkHolderScroll(object sender, ScrollEventArgs e)
        {
            Invalidate(InnerRectangle);
        }

        /// <summary>
        /// Opens the link in the default browser.
        /// </summary>
        private void Label_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var url = (string) ((LinkLabel) sender).Tag;

            try
            {
                Process.Start(url);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Arranges controls size.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);

            _holder.Location = InnerRectangle.Location;
            _holder.Size = InnerRectangle.Size;
        }
    }
}