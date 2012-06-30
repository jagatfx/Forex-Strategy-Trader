// Live Content
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
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
    public class LiveContent
    {
        private const string UpdateFileURL = "http://forexsb.com/products/fst-update.xml";

        private readonly List<LinkItem> _brokers = new List<LinkItem>();
        private readonly List<LinkItem> _links = new List<LinkItem>();
        private readonly string _pathUpdateFile;
        private readonly BackgroundWorker bgWorker;

        private readonly ToolStripMenuItem miForex;
        private readonly ToolStripMenuItem miLiveContent;
        private readonly LinkPanel pnlForexBrokers;
        private readonly LinkPanel pnlUsefulLinks;

        private XmlDocument _xmlUpdate = new XmlDocument();

        /// <summary>
        /// Public constructor
        /// </summary>
        public LiveContent(string pathSystem, ToolStripMenuItem miLiveContent, ToolStripMenuItem miForex,
                           LinkPanel pnlUsefulLinks, LinkPanel pnlForexBrokers)
        {
            this.miLiveContent = miLiveContent;
            this.miForex = miForex;
            this.pnlUsefulLinks = pnlUsefulLinks;
            this.pnlForexBrokers = pnlForexBrokers;

            _pathUpdateFile = Path.Combine(pathSystem, "fst-update.xml");

            try
            {
                LoadConfigFile();

                ReadFXBrokers();
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
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWork;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private void DoWork(object sender, DoWorkEventArgs e)
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
        private void UpdateLiveContentConfig()
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
        private void LoadConfigFile()
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
        private void SaveConfigFile()
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
        private void CheckProgramsVersionNumber()
        {
            string text = "";

            int iProgramVersion = int.Parse(_xmlUpdate.SelectSingleNode("update/versions/release").InnerText);
            if (Configs.CheckForUpdates && iProgramVersion > Data.ProgramID)
            {
                // A newer version was published
                text = Language.T("New Version");
            }
            else
            {
                int iBetaVersion = int.Parse(_xmlUpdate.SelectSingleNode("update/versions/beta").InnerText);
                if (Configs.CheckForNewBeta && iBetaVersion > Data.ProgramID)
                {
                    // A newer beta version was published
                    text = Language.T("New Beta");
                }
            }

            if (text != "")
            {
                miLiveContent.Text = text;
                miLiveContent.Visible = true;
                miLiveContent.Click += MenuLiveContentOnClick;
            }
        }

        /// <summary>
        /// Opens the Live Content browser
        /// </summary>
        private void MenuLiveContentOnClick(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/");
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
        private void HideMenuItemLiveContent()
        {
            miLiveContent.Visible = false;
            miLiveContent.Click -= MenuLiveContentOnClick;
        }

        /// <summary>
        /// Reads the FX brokers from the XML file.
        /// </summary>
        private void ReadFXBrokers()
        {
            XmlNodeList xmlListBrokers = _xmlUpdate.GetElementsByTagName("broker");

            foreach (XmlNode nodeBroker in xmlListBrokers)
            {
                string text = nodeBroker.SelectSingleNode("text").InnerText;
                string url = nodeBroker.SelectSingleNode("url").InnerText;
                string comment = nodeBroker.SelectSingleNode("comment").InnerText;
                _brokers.Add(new LinkItem(text, url, comment));
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private void SetBrokersInMenu()
        {
            foreach (LinkItem broker in _brokers)
            {
                var miBroker = new ToolStripMenuItem
                                   {
                                       Text = broker.Text + "...",
                                       Image = Resources.globe,
                                       Tag = broker.Url,
                                       ToolTipText = broker.Comment
                                   };
                miBroker.Click += MenuForexContentsOnClick;

                miForex.DropDownItems.Add(miBroker);
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private void SetBrokersInLinkPanel()
        {
            foreach (LinkItem broker in _brokers)
                pnlForexBrokers.AddLink(broker);

            pnlForexBrokers.SetLinks();
        }

        /// <summary>
        /// Reads the links from the xml file.
        /// </summary>
        private void ReadLinks()
        {
            XmlNodeList xmlListLinks = _xmlUpdate.GetElementsByTagName("link");

            foreach (XmlNode link in xmlListLinks)
            {
                string text = link.SelectSingleNode("text").InnerText;
                string url = link.SelectSingleNode("url").InnerText;
                string comment = link.SelectSingleNode("comment").InnerText;
                _links.Add(new LinkItem(text, url, comment));
            }
        }

        /// <summary>
        /// Sets the brokers in the menu
        /// </summary>
        private void SetLinksInLinkPanel()
        {
            foreach (LinkItem link in _links)
                pnlUsefulLinks.AddLink(link);

            pnlUsefulLinks.SetLinks();
        }

        /// <summary>
        /// Opens the forex news
        /// </summary>
        private void MenuForexContentsOnClick(object sender, EventArgs e)
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