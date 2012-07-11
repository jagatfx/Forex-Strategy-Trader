// Controls class 
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
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
    public partial class Controls : MenuAndStatusBar
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
                                     Parent = PnlWorkspace,
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

            InitializeStripTrade();
            InitializePageStatus();
            InitializePageStrategy();
            InitializePageChart();
            InitializePageAccount();
            InitializePageJournal();
            InitializePageOperation();
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
                MITabStatus.Checked = true;
                MITabStrategy.Checked = false;
                MITabChart.Checked = false;
                MITabAccount.Checked = false;
                MITabJournal.Checked = false;
                MITabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageStrategy)
            {
                DisposeChart();
                MITabStatus.Checked = false;
                MITabStrategy.Checked = true;
                MITabChart.Checked = false;
                MITabAccount.Checked = false;
                MITabJournal.Checked = false;
                MITabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageChart)
            {
                CreateChart();
                MITabStatus.Checked = false;
                MITabStrategy.Checked = false;
                MITabChart.Checked = true;
                MITabAccount.Checked = false;
                MITabJournal.Checked = false;
                MITabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageAccount)
            {
                DisposeChart();
                MITabStatus.Checked = false;
                MITabStrategy.Checked = false;
                MITabChart.Checked = false;
                MITabAccount.Checked = true;
                MITabJournal.Checked = false;
                MITabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageJournal)
            {
                DisposeChart();
                PageJournalSelected();
                MITabStatus.Checked = false;
                MITabStrategy.Checked = false;
                MITabChart.Checked = false;
                MITabAccount.Checked = false;
                MITabJournal.Checked = true;
                MITabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageOperation)
            {
                DisposeChart();
                MITabStatus.Checked = false;
                MITabStrategy.Checked = false;
                MITabChart.Checked = false;
                MITabAccount.Checked = false;
                MITabJournal.Checked = false;
                MITabOperation.Checked = true;
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