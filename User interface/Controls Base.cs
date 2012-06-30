// Controls class 
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls : Menu_and_StatusBar
    {
        /// <summary>
        /// The default constructor.
        /// </summary>
        protected Controls()
        {
            var imageList = new ImageList {ImageSize = new Size(20, 22)};
            imageList.Images.Add(Resources.tab_status);
            imageList.Images.Add(Resources.tab_strategy);
            imageList.Images.Add(Resources.tab_chart);
            imageList.Images.Add(Resources.tab_account);
            imageList.Images.Add(Resources.tab_journal);
            imageList.Images.Add(Resources.tab_operation);

            TabControlBase = new TabControl
                                 {
                                     Name = "tabControlBase",
                                     Parent = pnlWorkspace,
                                     Dock = DockStyle.Fill,
                                     ImageList = imageList,
                                     HotTrack = true
                                 };
            TabControlBase.SelectedIndexChanged += TabControlBaseSelectedIndexChanged;

            TabPageStatus = new TabPage();
            TabPageStrategy = new TabPage();
            TabPageChart = new TabPage();
            TabPageAccount = new TabPage();
            TabPageJournal = new TabPage();
            TabPageOperation = new TabPage();

            TabControlBase.Controls.Add(TabPageStatus);
            TabControlBase.Controls.Add(TabPageStrategy);
            TabControlBase.Controls.Add(TabPageChart);
            TabControlBase.Controls.Add(TabPageAccount);
            TabControlBase.Controls.Add(TabPageJournal);
            TabControlBase.Controls.Add(TabPageOperation);

            Initialize_StripTrade();
            Initialize_PageStatus();
            InitializePageStrategy();
            Initialize_PageChart();
            Initialize_PageAccount();
            Initialize_PageJournal();
            Initialize_PageOperation();
        }

        private TabControl TabControlBase { get; set; }

        private TabPage TabPageStatus { get; set; }
        private TabPage TabPageStrategy { get; set; }
        private TabPage TabPageChart { get; set; }
        private TabPage TabPageAccount { get; set; }
        private TabPage TabPageJournal { get; set; }
        private TabPage TabPageOperation { get; set; }

        /// <summary>
        /// Changes the active tab page.
        /// </summary>
        protected void ChangeTabPage(int index)
        {
            TabControlBase.SelectedIndex = index;
            TabControlBaseSelectedIndexChanged(new Object(), new EventArgs());
        }

        /// <summary>
        /// Sets tab pages and menu items.
        /// </summary>
        private void TabControlBaseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabControlBase.SelectedTab == TabPageStatus)
            {
                DisposeChart();
                miTabStatus.Checked = true;
                miTabStrategy.Checked = false;
                miTabChart.Checked = false;
                miTabAccount.Checked = false;
                miTabJournal.Checked = false;
                miTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageStrategy)
            {
                DisposeChart();
                miTabStatus.Checked = false;
                miTabStrategy.Checked = true;
                miTabChart.Checked = false;
                miTabAccount.Checked = false;
                miTabJournal.Checked = false;
                miTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageChart)
            {
                CreateChart();
                miTabStatus.Checked = false;
                miTabStrategy.Checked = false;
                miTabChart.Checked = true;
                miTabAccount.Checked = false;
                miTabJournal.Checked = false;
                miTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageAccount)
            {
                DisposeChart();
                miTabStatus.Checked = false;
                miTabStrategy.Checked = false;
                miTabChart.Checked = false;
                miTabAccount.Checked = true;
                miTabJournal.Checked = false;
                miTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageJournal)
            {
                DisposeChart();
                PageJournalSelected();
                miTabStatus.Checked = false;
                miTabStrategy.Checked = false;
                miTabChart.Checked = false;
                miTabAccount.Checked = false;
                miTabJournal.Checked = true;
                miTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageOperation)
            {
                DisposeChart();
                miTabStatus.Checked = false;
                miTabStrategy.Checked = false;
                miTabChart.Checked = false;
                miTabAccount.Checked = false;
                miTabJournal.Checked = false;
                miTabOperation.Checked = true;
            }

            Configs.LastTab = TabControlBase.SelectedIndex;
        }

        /// <summary>
        /// Sets colors of tab pages.
        /// </summary>
        protected void SetColors()
        {
            SetStatusColors();
            SetStrategyColors();
            SetChartColors();
            SetAccountColors();
            SetJournalColors();
            SetOperationColors();
        }
    }
}