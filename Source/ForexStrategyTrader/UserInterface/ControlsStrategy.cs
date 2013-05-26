// Controls Strategy
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
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private WebBrowser BrowserOverview { get; set; }
        private FancyPanel PnlBrawserBase { get; set; }
        private Panel PnlOverviewBase { get; set; }
        private Panel PnlStrategyBase { get; set; }
        private StrategyLayout StrategyLayout { get; set; }

        private ToolStrip TsStrategy { get; set; }

        /// <summary>
        /// Sets the controls in tabPageStrategy
        /// </summary>
        private void InitializePageStrategy()
        {
            // tabPageStrategy
            TabPageStrategy.Name = "tabPageStrategy";
            TabPageStrategy.Text = Language.T("Strategy");
            TabPageStrategy.ImageIndex = 1;
            TabPageStrategy.Resize += TabPageStrategyResize;

            PnlOverviewBase = new Panel {Parent = TabPageStrategy, Dock = DockStyle.Fill};

            PnlStrategyBase = new Panel {Parent = TabPageStrategy, Dock = DockStyle.Left};

            // Panel Browser Base
            PnlBrawserBase = new FancyPanel(Language.T("Strategy Overview"));
            PnlBrawserBase.Padding = new Padding(2, (int) PnlBrawserBase.CaptionHeight, 2, 2);
            PnlBrawserBase.Parent = PnlOverviewBase;
            PnlBrawserBase.Dock = DockStyle.Fill;

            // BrowserOverview
            BrowserOverview = new WebBrowser
                                  {
                                      Parent = PnlBrawserBase,
                                      Dock = DockStyle.Fill,
                                      WebBrowserShortcutsEnabled = false,
                                      AllowWebBrowserDrop = false
                                  };

            // StrategyLayout
            StrategyLayout = new StrategyLayout(Data.Strategy.Clone()) {Parent = PnlStrategyBase};
            StrategyLayout.BtnAddOpenFilter.Click += BtnAddOpenFilterClick;
            StrategyLayout.BtnAddCloseFilter.Click += BtnAddCloseFilterClick;

            // ToolStrip Strategy
            TsStrategy = new ToolStrip {Parent = PnlStrategyBase, Dock = DockStyle.None, AutoSize = false};

            // Button tsbtStrategyNew
            var tsbtStrategyNew = new ToolStripButton
                                      {
                                          Name = "New",
                                          DisplayStyle = ToolStripItemDisplayStyle.Image,
                                          Image = Resources.strategy_new
                                      };
            tsbtStrategyNew.Click += BtnStrategyIoClick;
            tsbtStrategyNew.ToolTipText = Language.T("Open the default strategy \"New.xml\".");
            TsStrategy.Items.Add(tsbtStrategyNew);

            // Button tsbtStrategyOpen
            var tsbtStrategyOpen = new ToolStripButton
                                       {
                                           Name = "Open",
                                           DisplayStyle = ToolStripItemDisplayStyle.Image,
                                           Image = Resources.strategy_open
                                       };
            tsbtStrategyOpen.Click += BtnStrategyIoClick;
            tsbtStrategyOpen.ToolTipText = Language.T("Open a strategy.");
            TsStrategy.Items.Add(tsbtStrategyOpen);

            // Button tsbtStrategySave
            var tsbtStrategySave = new ToolStripButton
                                       {
                                           Name = "Save",
                                           DisplayStyle = ToolStripItemDisplayStyle.Image,
                                           Image = Resources.strategy_save
                                       };
            tsbtStrategySave.Click += BtnStrategyIoClick;
            tsbtStrategySave.ToolTipText = Language.T("Save the strategy.");
            TsStrategy.Items.Add(tsbtStrategySave);

            // Button tsbtStrategySaveAs
            var tsbtStrategySaveAs = new ToolStripButton
                                         {
                                             Name = "SaveAs",
                                             DisplayStyle = ToolStripItemDisplayStyle.Image,
                                             Image = Resources.strategy_save_as
                                         };
            tsbtStrategySaveAs.Click += BtnStrategyIoClick;
            tsbtStrategySaveAs.ToolTipText = Language.T("Save a copy of the strategy.");
            TsStrategy.Items.Add(tsbtStrategySaveAs);

            TsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyUndo
            var tsbtStrategyUndo = new ToolStripButton
                                       {
                                           Name = "Undo",
                                           DisplayStyle = ToolStripItemDisplayStyle.Image,
                                           Image = Resources.strategy_undo
                                       };
            tsbtStrategyUndo.Click += MenuStrategyUndo_OnClick;
            tsbtStrategyUndo.ToolTipText = Language.T("Undo the last change in the strategy.");
            TsStrategy.Items.Add(tsbtStrategyUndo);

            // Button tsbtStrategyCopy
            var tsbtStrategyCopy = new ToolStripButton
                                       {
                                           Name = "Copy",
                                           DisplayStyle = ToolStripItemDisplayStyle.Image,
                                           Image = Resources.copy
                                       };
            tsbtStrategyCopy.Click += MenuStrategyCopy_OnClick;
            tsbtStrategyCopy.ToolTipText = Language.T("Copy the entire strategy to the clipboard.");
            TsStrategy.Items.Add(tsbtStrategyCopy);

            // Button tsbtStrategyPaste
            var tsbtStrategyPaste = new ToolStripButton
                                        {
                                            Name = "Paste",
                                            DisplayStyle = ToolStripItemDisplayStyle.Image,
                                            Image = Resources.paste
                                        };
            tsbtStrategyPaste.Click += MenuStrategyPaste_OnClick;
            tsbtStrategyPaste.ToolTipText = Language.T("Load a strategy from the clipboard.");
            TsStrategy.Items.Add(tsbtStrategyPaste);

            TsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyZoomIn
            var tsbtStrategyZoomIn = new ToolStripButton
                                         {
                                             Name = "ZoomIn",
                                             DisplayStyle = ToolStripItemDisplayStyle.Image,
                                             Image = Resources.strategy_zoom_in
                                         };
            tsbtStrategyZoomIn.Click += BtnStrategyZoomClick;
            tsbtStrategyZoomIn.ToolTipText = Language.T("Expand the information in the strategy slots.");
            TsStrategy.Items.Add(tsbtStrategyZoomIn);

            // Button tsbtStrategyZoomOut
            var tsbtStrategyZoomOut = new ToolStripButton
                                          {
                                              Name = "ZoomOut",
                                              DisplayStyle = ToolStripItemDisplayStyle.Image,
                                              Image = Resources.strategy_zoom_out
                                          };
            tsbtStrategyZoomOut.Click += BtnStrategyZoomClick;
            tsbtStrategyZoomOut.ToolTipText = Language.T("Reduce the information in the strategy slots.");
            TsStrategy.Items.Add(tsbtStrategyZoomOut);

            TsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategyDescription
            var tsbtStrategyDescription = new ToolStripButton
                                              {
                                                  Name = "Description",
                                                  DisplayStyle = ToolStripItemDisplayStyle.Image,
                                                  Image = Resources.strategy_description
                                              };
            tsbtStrategyDescription.Click += BtnStrategyDescriptionClick;
            tsbtStrategyDescription.ToolTipText = Language.T("Edit the strategy description.");
            TsStrategy.Items.Add(tsbtStrategyDescription);

            // Button tsbtStrategyPublish
            var tsbtStrategyPublish = new ToolStripButton
                                          {
                                              Name = "Publish",
                                              DisplayStyle = ToolStripItemDisplayStyle.Image,
                                              Image = Resources.strategy_publish
                                          };
            tsbtStrategyPublish.Click += MenuStrategyBBcode_OnClick;
            tsbtStrategyPublish.ToolTipText = Language.T("Publish the strategy in the program's forum.");
            TsStrategy.Items.Add(tsbtStrategyPublish);

            TsStrategy.Items.Add(new ToolStripSeparator());

            // Button tsbtStrategySettings
            var tsbtStrategySettings = new ToolStripButton
                                           {
                                               Name = "Settings",
                                               DisplayStyle = ToolStripItemDisplayStyle.Image,
                                               Image = Resources.strategy_settings
                                           };
            tsbtStrategySettings.Click += BtnStrategySettings_Click;
            tsbtStrategySettings.ToolTipText = Language.T("Trade settings.");
            TsStrategy.Items.Add(tsbtStrategySettings);

            SetStrategyColors();
            RebuildStrategyLayout();
            SetSrategyOverview();
        }

        /// <summary>
        /// TabPageStrategy_Resize
        /// </summary>
        private void TabPageStrategyResize(object sender, EventArgs e)
        {
            PnlStrategyBase.Width = TabPageStrategy.ClientSize.Width/2;

            TsStrategy.Width = PnlStrategyBase.Width - Space;
            TsStrategy.Location = Point.Empty;

            StrategyLayout.Width = PnlStrategyBase.Width - Space;
            StrategyLayout.Height = TabPageStrategy.ClientSize.Height - TsStrategy.Bottom - Space;
            StrategyLayout.Location = new Point(0, TsStrategy.Bottom + Space);
        }

        /// <summary>
        /// Sets the colors of tab page Strategy.
        /// </summary>
        private void SetStrategyColors()
        {
            TabPageStrategy.BackColor = LayoutColors.ColorFormBack;
            PnlBrawserBase.SetColors();
            SetSrategyOverview();
        }

        /// <summary>
        /// Creates a new strategy layout using Data.Strategy
        /// </summary>
        protected void RebuildStrategyLayout()
        {
            StrategyLayout.RebuildStrategyControls(Data.Strategy.Clone());
            StrategyLayout.PnlProperties.Click += PnlAveragingClick;
            for (int slot = 0; slot < Data.Strategy.Slots; slot++)
            {
                var miEdit = new ToolStripMenuItem
                                 {
                                     Text = Language.T("Edit") + "...",
                                     Image = Resources.edit,
                                     Name = "Edit",
                                     Tag = slot
                                 };
                miEdit.Click += SlotContextMenuClick;

                var miUpwards = new ToolStripMenuItem
                                    {
                                        Text = Language.T("Move Up"),
                                        Image = Resources.up_arrow,
                                        Name = "Upwards",
                                        Tag = slot
                                    };
                miUpwards.Click += SlotContextMenuClick;
                miUpwards.Enabled = (slot > 1 &&
                                     Data.Strategy.Slot[slot].SlotType == Data.Strategy.Slot[slot - 1].SlotType);

                var miDownwards = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Move Down"),
                                          Image = Resources.down_arrow,
                                          Name = "Downwards",
                                          Tag = slot
                                      };
                miDownwards.Click += SlotContextMenuClick;
                miDownwards.Enabled = (slot < Data.Strategy.Slots - 1 &&
                                       Data.Strategy.Slot[slot].SlotType == Data.Strategy.Slot[slot + 1].SlotType);

                var miDuplicate = new ToolStripMenuItem
                                      {
                                          Text = Language.T("Duplicate"),
                                          Image = Resources.duplicate,
                                          Name = "Duplicate",
                                          Tag = slot
                                      };
                miDuplicate.Click += SlotContextMenuClick;
                miDuplicate.Enabled = (Data.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter &&
                                       Data.Strategy.OpenFilters < Strategy.MaxOpenFilters ||
                                       Data.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter &&
                                       Data.Strategy.CloseFilters < Strategy.MaxCloseFilters);

                var miDelete = new ToolStripMenuItem
                                   {
                                       Text = Language.T("Delete"),
                                       Image = Resources.close_button,
                                       Name = "Delete",
                                       Tag = slot
                                   };
                miDelete.Click += SlotContextMenuClick;
                miDelete.Enabled = (Data.Strategy.Slot[slot].SlotType == SlotTypes.OpenFilter ||
                                    Data.Strategy.Slot[slot].SlotType == SlotTypes.CloseFilter);

                StrategyLayout.ApnlSlot[slot].ContextMenuStrip = new ContextMenuStrip();
                StrategyLayout.ApnlSlot[slot].ContextMenuStrip.Items.AddRange(new ToolStripItem[]
                                                                                  {
                                                                                      miEdit, miUpwards, miDownwards,
                                                                                      miDuplicate, miDelete
                                                                                  });

                StrategyLayout.ApnlSlot[slot].MouseClick += PnlSlotMouseUp;
                if (slot != Data.Strategy.OpenSlot && slot != Data.Strategy.CloseSlot)
                    StrategyLayout.AbtnRemoveSlot[slot].Click += BtnRemoveSlotClick;
            }
        }

        /// <summary>
        /// Repaint the strategy slots without changing its kind and count.
        /// </summary>
        protected void RepaintStrategyLayout()
        {
            StrategyLayout.RepaintStrategyControls(Data.Strategy.Clone());
        }

        /// <summary>
        /// Rearranges the strategy slots without changing its kind and count.
        /// </summary>
        private void RearangeStrategyLayout()
        {
            StrategyLayout.RearangeStrategyControls();
        }

        /// <summary>
        /// Opens the averaging parameters dialog.
        /// </summary>
        protected virtual void PnlAveragingClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void PnlSlotMouseUp(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// Click on a strategy slot
        /// </summary>
        protected virtual void SlotContextMenuClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add open filter was clicked.
        /// </summary>
        protected virtual void BtnAddOpenFilterClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Performs actions after the button add close filter was clicked.
        /// </summary>
        protected virtual void BtnAddCloseFilterClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Removes the corresponding slot.
        /// </summary>
        protected virtual void BtnRemoveSlotClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// IO strategy
        /// </summary>
        protected virtual void BtnStrategyIoClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Changes the slot size
        /// </summary>
        private void BtnStrategyZoomClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;

            switch (btn.Name)
            {
                case "ZoomIn":
                    if (StrategyLayout.SlotMinMidMax == SlotSizeMinMidMax.Min)
                    {
                        StrategyLayout.SlotMinMidMax = SlotSizeMinMidMax.Mid;
                    }
                    else if (StrategyLayout.SlotMinMidMax == SlotSizeMinMidMax.Mid)
                    {
                        StrategyLayout.SlotMinMidMax = SlotSizeMinMidMax.Max;
                    }
                    break;
                case "ZoomOut":
                    if (StrategyLayout.SlotMinMidMax == SlotSizeMinMidMax.Max)
                    {
                        StrategyLayout.SlotMinMidMax = SlotSizeMinMidMax.Mid;
                    }
                    else if (StrategyLayout.SlotMinMidMax == SlotSizeMinMidMax.Mid)
                    {
                        StrategyLayout.SlotMinMidMax = SlotSizeMinMidMax.Min;
                    }
                    break;
            }

            RearangeStrategyLayout();
        }

        /// <summary>
        /// View / edit the strategy description.
        /// </summary>
        private void BtnStrategyDescriptionClick(object sender, EventArgs e)
        {
            string oldInfo = Data.Strategy.Description;
            var si = new StrategyDescription();
            si.ShowDialog();
            if (oldInfo == Data.Strategy.Description) return;
            Data.SetStrategyIndicators();
            SetSrategyOverview();
            Data.IsStrategyChanged = true;
        }

        /// <summary>
        /// Sets the strategy overview
        /// </summary>
        protected void SetSrategyOverview()
        {
            BrowserOverview.DocumentText = Data.Strategy.GenerateHtmlOverview();
        }

        /// <summary>
        /// Trade settings
        /// </summary>
        private void BtnStrategySettings_Click(object sender, EventArgs e)
        {
            var ts = new TradeSettings();
            ts.ShowDialog();
        }
    }
}