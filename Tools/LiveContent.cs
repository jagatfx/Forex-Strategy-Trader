// Live Content
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    public static class LiveContent
    {
        private const string UpdateFileURL = "http://forexsb.com/products/fst-update.xml";

        private static readonly List<LinkItem> Brokers = new List<LinkItem>();
        private static readonly List<LinkItem> Links = new List<LinkItem>();
        private static string _pathUpdateFile;
        private static BackgroundWorker _bgWorker;

        private static ToolStripMenuItem _miForex;
        private static ToolStripMenuItem _miLiveContent;
        private static LinkPanel _pnlForexBrokers;
        private static LinkPanel _pnlUsefulLinks;

        private static XmlDocument _xmlUpdate = new XmlDocument();

        /// <summary>
        /// Public constructor
        /// </summary>
        public static void CheckForUpdate(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex,
                                          LinkPanel pnlUsefulLinks, LinkPanel pnlForexBrokers)
        {
            _miLiveContent = miLiveContent;
            _miForex = miForex;
            _pnlUsefulLinks = pnlUsefulLinks;
            _pnlForexBrokers = pnlForexBrokers;

            _pathUpdateFile = Path.Combine(pathSystem, "fst-update.xml");

            try
            {
                LoadConfigFile();

                ReadFxBrokers();
                SetBrokersInMenu();
                SetBrokersInLinkPanel();

                ReadLinks();
                SetLinksInLinkPanel();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            // BackGroundWorker
            _bgWorker = new BackgroundWorker();
            _bgWorker.DoWork += DoWork;
            _bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private static void DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                UpdateLiveContentConfig();
                CheckProgramsVersionNumber();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Update the config file if it is necessary
        /// </summary>
        private static void UpdateLiveContentConfig()
        {
            var url = new Uri(UpdateFileURL);
            var webClient = new WebClient();
            try
            {
                _xmlUpdate.LoadXml(webClient.DownloadString(url));
                SaveConfigFile();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Load config file 
        /// </summary>
        private static void LoadConfigFile()
        {
            try
            {
                if (!File.Exists(_pathUpdateFile))
                {
                    _xmlUpdate = new XmlDocument {InnerXml = Resources.fst_update};
                }
                else
                {
                    _xmlUpdate.Load(_pathUpdateFile);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Save config file
        /// </summary>
        private static void SaveConfigFile()
        {
            try
            {
                _xmlUpdate.Save(_pathUpdateFile);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Checks the program version
        /// </summary>
        private static void CheckProgramsVersionNumber()
        {
            string text = "";

            int programVersion = int.Parse(GetNodeInnerText(_xmlUpdate, "update/versions/release"));
            if (Configs.CheckForUpdates && programVersion > Data.ProgramID)
            {
                // A newer version was published
                text = Language.T("New Version");
            }
            else
            {
                int betaVersion = int.Parse(GetNodeInnerText(_xmlUpdate, "update/versions/beta"));
                if (Configs.CheckForNewBeta && betaVersion > Data.ProgramID)
                {
                    // A newer beta version was published
                    text = Language.T("New Beta");
                }
            }

            if (text != "")
            {
                _miLiveContent.Text = text;
                _miLiveContent.Visible = true;
                _miLiveContent.Click += MenuLiveContentOnClick;
            }
        }

        /// <summary>
        /// Opens the Live Content browser
        /// </summary>
        private static void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/download");
                HideMenuItemLiveContent();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Hides the Live Content menu item.
        /// </summary>
        private static void HideMenuItemLiveContent()
        {
            _miLiveContent.Visible = false;
            _miLiveContent.Click -= MenuLiveContentOnClick;
        }

        /// <summary>
        /// Reads the FX brokers from the XML file.
        /// </summary>
        private static void ReadFxBrokers()
        {
            XmlNodeList xmlListBrokers = _xmlUpdate.GetElementsByTagName("broker");

            foreach (XmlNode nodeBroker in xmlListBrokers)
            {
                string text = GetNodeInnerText(nodeBroker, "text");
                string url = GetNodeInnerText(nodeBroker, "url");
                string comment = GetNodeInnerText(nodeBroker, "comment");
                Brokers.Add(new LinkItem(text, url, comment));
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private static void SetBrokersInMenu()
        {
            foreach (LinkItem broker in Brokers)
            {
                var miBroker = new ToolStripMenuItem
                                   {
                                       Text = broker.Text + "...",
                                       Image = Resources.globe,
                                       Tag = broker.Url,
                                       ToolTipText = broker.Comment
                                   };
                miBroker.Click += MenuForexContentsOnClick;

                _miForex.DropDownItems.Add(miBroker);
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private static void SetBrokersInLinkPanel()
        {
            foreach (LinkItem broker in Brokers)
                _pnlForexBrokers.AddLink(broker);

            _pnlForexBrokers.SetLinks();
        }

        /// <summary>
        /// Reads the links from the xml file.
        /// </summary>
        private static void ReadLinks()
        {
            XmlNodeList xmlListLinks = _xmlUpdate.GetElementsByTagName("link");

            foreach (XmlNode link in xmlListLinks)
            {
                string text = GetNodeInnerText(link, "text");
                string url = GetNodeInnerText(link, "url");
                string comment = GetNodeInnerText(link, "comment");
                Links.Add(new LinkItem(text, url, comment));
            }
        }

        private static string GetNodeInnerText(XmlNode node, string nodeText)
        {
            XmlNode selectSingleNode = node.SelectSingleNode(nodeText);
            return selectSingleNode != null ? selectSingleNode.InnerText : null;
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private static void SetLinksInLinkPanel()
        {
            foreach (LinkItem link in Links)
                _pnlUsefulLinks.AddLink(link);

            _pnlUsefulLinks.SetLinks();
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        private static void MenuForexContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            try
            {
                Process.Start((string) mi.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}