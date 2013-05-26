//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public static class LiveContent
    {
        private const string UpdateFileUrl = "http://forexsb.com/products/fst-update.xml";

        private static readonly List<LinkItem> Brokers = new List<LinkItem>();
        private static readonly List<LinkItem> Links = new List<LinkItem>();
        private static string pathUpdateFile;
        private static BackgroundWorker bgWorker;

        private static ToolStripMenuItem miForex;
        private static ToolStripMenuItem miLiveContent;
        private static LinkPanel pnlForexBrokers;
        private static LinkPanel pnlUsefulLinks;

        private static XmlDocument xmlUpdate = new XmlDocument();

        public static void CheckForUpdate(string pathSystem,
                                          ToolStripMenuItem liveContent,
                                          ToolStripMenuItem forex,
                                          LinkPanel usefulLinks,
                                          LinkPanel forexBrokers)
        {
            miLiveContent = liveContent;
            miForex = forex;
            pnlUsefulLinks = usefulLinks;
            pnlForexBrokers = forexBrokers;

            pathUpdateFile = Path.Combine(pathSystem, "fst-update.xml");

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
            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWork;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
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
        ///     Update the config file if it is necessary
        /// </summary>
        private static void UpdateLiveContentConfig()
        {
            var url = new Uri(UpdateFileUrl);
            var webClient = new WebClient();
            try
            {
                xmlUpdate.LoadXml(webClient.DownloadString(url));
                SaveConfigFile();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Load config file
        /// </summary>
        private static void LoadConfigFile()
        {
            try
            {
                if (!File.Exists(pathUpdateFile))
                {
                    xmlUpdate = new XmlDocument {InnerXml = Resources.fst_update};
                }
                else
                {
                    xmlUpdate.Load(pathUpdateFile);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Save config file
        /// </summary>
        private static void SaveConfigFile()
        {
            try
            {
                xmlUpdate.Save(pathUpdateFile);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Checks the program version
        /// </summary>
        private static void CheckProgramsVersionNumber()
        {
            string text = "";

            int programVersion = int.Parse(GetNodeInnerText(xmlUpdate, "update/versions/release"));
            if (Configs.CheckForUpdates && programVersion > Data.ProgramId)
            {
                // A newer version was published
                text = Language.T("New Version");
            }
            else
            {
                int betaVersion = int.Parse(GetNodeInnerText(xmlUpdate, "update/versions/beta"));
                if (Configs.CheckForNewBeta && betaVersion > Data.ProgramId)
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
        ///     Opens the Live Content browser
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
        ///     Hides the Live Content menu item.
        /// </summary>
        private static void HideMenuItemLiveContent()
        {
            miLiveContent.Visible = false;
            miLiveContent.Click -= MenuLiveContentOnClick;
        }

        /// <summary>
        ///     Reads the FX brokers from the XML file.
        /// </summary>
        private static void ReadFxBrokers()
        {
            XmlNodeList xmlListBrokers = xmlUpdate.GetElementsByTagName("broker");

            foreach (XmlNode nodeBroker in xmlListBrokers)
            {
                string text = GetNodeInnerText(nodeBroker, "text");
                string url = GetNodeInnerText(nodeBroker, "url");
                string comment = GetNodeInnerText(nodeBroker, "comment");
                Brokers.Add(new LinkItem(text, url, comment));
            }
        }

        /// <summary>
        ///     Sets the brokers in the menu
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

                miForex.DropDownItems.Add(miBroker);
            }
        }

        /// <summary>
        ///     Sets the brokers in the menu
        /// </summary>
        private static void SetBrokersInLinkPanel()
        {
            foreach (LinkItem broker in Brokers)
                pnlForexBrokers.AddLink(broker);

            pnlForexBrokers.SetLinks();
        }

        /// <summary>
        ///     Reads the links from the xml file.
        /// </summary>
        private static void ReadLinks()
        {
            XmlNodeList xmlListLinks = xmlUpdate.GetElementsByTagName("link");

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
        ///     Sets the brokers in the menu
        /// </summary>
        private static void SetLinksInLinkPanel()
        {
            foreach (LinkItem link in Links)
                pnlUsefulLinks.AddLink(link);

            pnlUsefulLinks.SetLinks();
        }

        /// <summary>
        ///     Opens the forex news
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