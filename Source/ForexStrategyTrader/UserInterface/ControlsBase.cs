// Controls class 
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public partial class Controls : MenuAndStatusBar
    {
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
        ///     Changes the active tab page.
        /// </summary>
        protected void ChangeTabPage(int index)
        {
            TabControlBase.SelectedIndex = index;
            TabControlBaseSelectedIndexChanged(new Object(), new EventArgs());
        }

        /// <summary>
        ///     Sets tab pages and menu items.
        /// </summary>
        private void TabControlBaseSelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabControlBase.SelectedTab == TabPageStatus)
            {
                DisposeChart();
                MiTabStatus.Checked = true;
                MiTabStrategy.Checked = false;
                MiTabChart.Checked = false;
                MiTabAccount.Checked = false;
                MiTabJournal.Checked = false;
                MiTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageStrategy)
            {
                DisposeChart();
                MiTabStatus.Checked = false;
                MiTabStrategy.Checked = true;
                MiTabChart.Checked = false;
                MiTabAccount.Checked = false;
                MiTabJournal.Checked = false;
                MiTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageChart)
            {
                CreateChart();
                MiTabStatus.Checked = false;
                MiTabStrategy.Checked = false;
                MiTabChart.Checked = true;
                MiTabAccount.Checked = false;
                MiTabJournal.Checked = false;
                MiTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageAccount)
            {
                DisposeChart();
                MiTabStatus.Checked = false;
                MiTabStrategy.Checked = false;
                MiTabChart.Checked = false;
                MiTabAccount.Checked = true;
                MiTabJournal.Checked = false;
                MiTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageJournal)
            {
                DisposeChart();
                PageJournalSelected();
                MiTabStatus.Checked = false;
                MiTabStrategy.Checked = false;
                MiTabChart.Checked = false;
                MiTabAccount.Checked = false;
                MiTabJournal.Checked = true;
                MiTabOperation.Checked = false;
            }
            else if (TabControlBase.SelectedTab == TabPageOperation)
            {
                DisposeChart();
                MiTabStatus.Checked = false;
                MiTabStrategy.Checked = false;
                MiTabChart.Checked = false;
                MiTabAccount.Checked = false;
                MiTabJournal.Checked = false;
                MiTabOperation.Checked = true;
            }

            Configs.LastTab = TabControlBase.SelectedIndex;
        }

        /// <summary>
        ///     Sets colors of tab pages.
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