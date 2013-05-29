// Actions OnClick
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Infrastructure.Exceptions;
using MT4Bridge;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Class Actions : Controls
    /// </summary>
    public sealed partial class Actions
    {
        /// <summary>
        ///     Opens the averaging parameters dialog.
        /// </summary>
        protected override void PnlAveragingClick(object sender, EventArgs e)
        {
            EditStrategyProperties();
        }

        /// <summary>
        ///     Opens the indicator parameters dialog.
        /// </summary>
        protected override void PnlSlotMouseUp(object sender, MouseEventArgs e)
        {
            var panel = (Panel) sender;
            var tag = (int) panel.Tag;
            if (e.Button == MouseButtons.Left)
                EditSlot(tag);
        }

        /// <summary>
        ///     Strategy panel menu items clicked
        /// </summary>
        protected override void SlotContextMenuClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            var tag = (int) mi.Tag;
            switch (mi.Name)
            {
                case "Edit":
                    EditSlot(tag);
                    break;
                case "Upwards":
                    MoveSlotUpwards(tag);
                    break;
                case "Downwards":
                    MoveSlotDownwards(tag);
                    break;
                case "Duplicate":
                    DuplicateSlot(tag);
                    break;
                case "Delete":
                    RemoveSlot(tag);
                    break;
            }
        }

        /// <summary>
        ///     MenuChangeTabs_OnClick
        /// </summary>
        protected override void MenuChangeTabs_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            if (mi.Checked)
                return;

            var tag = (int) mi.Tag;
            ChangeTabPage(tag);
        }

        /// <summary>
        ///     Performs actions after the button add open filter was clicked.
        /// </summary>
        protected override void BtnAddOpenFilterClick(object sender, EventArgs e)
        {
            AddOpenFilter();
        }

        /// <summary>
        ///     Performs actions after the button add close filter was clicked.
        /// </summary>
        protected override void BtnAddCloseFilterClick(object sender, EventArgs e)
        {
            AddCloseFilter();
        }

        /// <summary>
        ///     Remove the corresponding indicator slot.
        /// </summary>
        protected override void BtnRemoveSlotClick(object sender, EventArgs e)
        {
            var slot = (int) ((Button) sender).Tag;
            RemoveSlot(slot);
        }

        /// <summary>
        ///     Load a color scheme.
        /// </summary>
        protected override void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            if (!mi.Checked)
            {
                Configs.ColorScheme = mi.Name;
            }
            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;

            LoadColorScheme();
        }

        /// <summary>
        ///     Gradient View Changed
        /// </summary>
        protected override void MenuGradientView_OnClick(object sender, EventArgs e)
        {
            Configs.GradientView = ((ToolStripMenuItem) sender).Checked;
            PnlWorkspace.Invalidate(true);
            SetColors();
        }


        /// <summary>
        ///     Strategy IO
        /// </summary>
        protected override void BtnStrategyIoClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;

            switch (btn.Name)
            {
                case "New":
                    NewStrategy();
                    break;
                case "Open":
                    ShowOpenFileDialog();
                    break;
                case "Save":
                    SaveStrategy();
                    break;
                case "SaveAs":
                    SaveAsStrategy();
                    break;
            }
        }

        /// <summary>
        ///     Loads the default strategy.
        /// </summary>
        protected override void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
            NewStrategy();
        }

        /// <summary>
        ///     Opens the dialog form OpenFileDialog.
        /// </summary>
        protected override void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
            ShowOpenFileDialog();
        }

        /// <summary>
        ///     Saves the strategy.
        /// </summary>
        protected override void MenuFileSave_OnClick(object sender, EventArgs e)
        {
            SaveStrategy();
        }

        /// <summary>
        ///     Opens the dialog form SaveFileDialog.
        /// </summary>
        protected override void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
            SaveAsStrategy();
        }

        /// <summary>
        ///     Undoes the strategy.
        /// </summary>
        protected override void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
            UndoStrategy();
        }

        /// <summary>
        ///     Copies the strategy to clipboard.
        /// </summary>
        protected override void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
            XmlDocument xmlDoc = StrategyXML.CreateStrategyXmlDoc(Data.Strategy);
            Clipboard.SetText(xmlDoc.InnerXml);
        }

        /// <summary>
        ///     Pastes a strategy from clipboard.
        /// </summary>
        protected override void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                return;

            var xmlDoc = new XmlDocument();
            var strategyXml = new StrategyXML();
            Strategy tempStrategy;

            try
            {
                xmlDoc.InnerXml = Clipboard.GetText();
                tempStrategy = strategyXml.ParseXmlStrategy(xmlDoc);
            }
            catch (MissingIndicatorException exception)
            {
                string message = string.Format(
                    "{2}{1}{0}{1}Please find this indicator in Repository or in Custom Indicators forum.",
                    exception.Message, Environment.NewLine, "Cannot load the strategy.");
                MessageBox.Show(message, "Load strategy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
                return;
            }

            OnStrategyChange();

            Data.Strategy = tempStrategy;
            Data.StrategyName = tempStrategy.StrategyName;
            Data.Strategy.StrategyName = tempStrategy.StrategyName;

            Data.SetStrategyIndicators();
            RebuildStrategyLayout();
            SetSrategyOverview();

            SetFormText();
            Data.IsStrategyChanged = false;
            Data.LoadedSavedStrategy = Data.StrategyPath;
            Data.StackStrategy.Clear();

            CalculateStrategy(true);
            AfterStrategyOpening();
        }

        /// <summary>
        ///     Loads a dropped strategy.
        /// </summary>
        protected override void LoadDroppedStrategy(string filePath)
        {
            Data.StrategyDir = Path.GetDirectoryName(filePath);
            LoadStrategyFile(filePath);
        }

        /// <summary>
        ///     Opens the strategy settings dialogue.
        /// </summary>
        protected override void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
            UsePreviousBarValueChange();
        }

        /// <summary>
        ///     Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected override void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
            var publisher = new StrategyPublish();
            publisher.Show();
        }

        /// <summary>
        ///     Tools menu
        /// </summary>
        protected override void MenuTools_OnClick(object sender, EventArgs e)
        {
            string menuItemName = ((ToolStripMenuItem) sender).Name;

            switch (menuItemName)
            {
                case "Reset settings":
                    ResetSettings();
                    break;
                case "miResetTrader":
                    ResetTrader();
                    break;
                case "miInstallExpert":
                    InstallMTFiles();
                    break;
                case "miNewTranslation":
                    MakeNewTranslation();
                    break;
                case "miEditTranslation":
                    EditTranslation();
                    break;
                case "miShowEnglishPhrases":
                    Language.ShowPhrases(1);
                    break;
                case "miShowAltPhrases":
                    Language.ShowPhrases(2);
                    break;
                case "miShowAllPhrases":
                    Language.ShowPhrases(3);
                    break;
                case "miOpenIndFolder":
                    try
                    {
                        Process.Start(Data.SourceFolder);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    break;
                case "miReloadInd":
                    Cursor = Cursors.WaitCursor;
                    ReloadCustomIndicators();
                    Cursor = Cursors.Default;
                    break;
                case "miCheckInd":
                    CustomIndicators.TestCustomIndicators();
                    break;
                case "CommandConsole":
                    ShowCommandConsole();
                    break;
            }
        }

        /// <summary>
        ///     Installs MT Expert and Library files.
        /// </summary>
        private void InstallMTFiles()
        {
            try
            {
                Process.Start(Data.UserFilesDir + @"\MetaTrader\Install MT Files.exe");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        ///     Manual operation execution.
        /// </summary>
        public override void BtnOperationClick(object sender, EventArgs e)
        {
            if (!Data.IsConnected)
            {
                if (Configs.PlaySounds)
                    Data.SoundError.Play();
                return;
            }

            var btn = (Button) sender;

            switch (btn.Name)
            {
                case "btnBuy":
                    {
                        const OrderType type = OrderType.Buy;
                        string symbol = Data.Symbol;
                        double lots = NormalizeEntrySize(OperationLots);
                        double price = Data.Ask;
                        int slippage = Configs.AutoSlippage
                                           ? (int) Data.InstrProperties.Spread*3
                                           : Configs.SlippageEntry;

                        int stopLossPips;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss = stopLossPips > 0 ? Data.Bid - Data.InstrProperties.Point*stopLossPips : 0;
                        double takeprofit = OperationTakeProfit > 0
                                                ? Data.Bid + Data.InstrProperties.Point*OperationTakeProfit
                                                : 0;

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                                       Language.T("An entry order sent") + ": " +
                                                       Language.T("Buy") + " {0} " +
                                                       LotOrLots(lots) + " " +
                                                       Language.T("at") + " {1}, " +
                                                       Language.T("Stop Loss") + " {2}, " +
                                                       Language.T("Take Profit") + " {3}", lots,
                                                       price.ToString(Data.FF), stoploss.ToString(Data.FF),
                                                       takeprofit.ToString(Data.FF));
                        var jmsg = new JournalMessage(JournalIcons.OrderBuy, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        int response = bridge.OrderSend(symbol, type, lots, price, slippage, stopLossPips,
                                                        OperationTakeProfit, parameters);

                        if (response >= 0)
                        {
                            Data.AddBarStats(OperationType.Buy, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {
                            // Error in operation execution.
                            ReportOperationError();
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
                case "btnSell":
                    {
                        const OrderType type = OrderType.Sell;
                        string symbol = Data.Symbol;
                        double lots = NormalizeEntrySize(OperationLots);
                        double price = Data.Bid;
                        int slippage = Configs.AutoSlippage
                                           ? (int) Data.InstrProperties.Spread*3
                                           : Configs.SlippageEntry;

                        int stopLossPips;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss = stopLossPips > 0 ? Data.Ask + Data.InstrProperties.Point*stopLossPips : 0;
                        double takeprofit = OperationTakeProfit > 0
                                                ? Data.Ask - Data.InstrProperties.Point*OperationTakeProfit
                                                : 0;

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                                       Language.T("An entry order sent") + ": " +
                                                       Language.T("Sell") + " {0} " +
                                                       LotOrLots(lots) + " " +
                                                       Language.T("at") + " {1}, " +
                                                       Language.T("Stop Loss") + " {2}, " +
                                                       Language.T("Take Profit") + " {3}", lots,
                                                       price.ToString(Data.FF), stoploss.ToString(Data.FF),
                                                       takeprofit.ToString(Data.FF));
                        var jmsg = new JournalMessage(JournalIcons.OrderSell, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        int response = bridge.OrderSend(symbol, type, lots, price, slippage, stopLossPips,
                                                        OperationTakeProfit, parameters);

                        if (response >= 0)
                        {
                            Data.AddBarStats(OperationType.Sell, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {
                            // Error in operation execution.
                            ReportOperationError();
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
                case "btnClose":
                    {
                        string symbol = Data.Symbol;
                        double lots = NormalizeEntrySize(Data.PositionLots);
                        double price = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
                        int slippage = Configs.AutoSlippage ? (int) Data.InstrProperties.Spread*6 : Configs.SlippageExit;
                        int ticket = Data.PositionTicket;

                        if (ticket == 0)
                        {
                            // No position.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();
                            return;
                        }

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                                       Language.T("An exit order sent") + ": " +
                                                       Language.T("Close") + " {0} " +
                                                       LotOrLots(lots) + " " +
                                                       Language.T("at") + " {1}",
                                                       lots, price.ToString(Data.FF));
                        var jmsg = new JournalMessage(JournalIcons.OrderClose, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);

                        bool responseOk = bridge.OrderClose(ticket, lots, price, slippage);

                        if (responseOk)
                            Data.AddBarStats(OperationType.Close, lots, price);
                        else
                            ReportOperationError();

                        Data.WrongStopLoss = 0;
                        Data.WrongTakeProf = 0;
                        Data.WrongStopsRetry = 0;
                    }
                    break;
                case "btnModify":
                    {
                        string symbol = Data.Symbol;
                        double lots = NormalizeEntrySize(Data.PositionLots);
                        double price = Data.PositionDirection == PosDirection.Long ? Data.Bid : Data.Ask;
                        int ticket = Data.PositionTicket;
                        double sign = Data.PositionDirection == PosDirection.Long ? 1 : -1;

                        if (ticket == 0)
                        {
                            // No position.
                            if (Configs.PlaySounds)
                                Data.SoundError.Play();
                            return;
                        }

                        if (Configs.PlaySounds)
                            Data.SoundOrderSent.Play();

                        int stopLossPips;
                        if (OperationStopLoss > 0 && OperationTrailingStop > 0)
                            stopLossPips = Math.Min(OperationStopLoss, OperationTrailingStop);
                        else
                            stopLossPips = Math.Max(OperationStopLoss, OperationTrailingStop);

                        double stoploss = stopLossPips > 0 ? price - sign*Data.InstrProperties.Point*stopLossPips : 0;
                        double takeprofit = OperationTakeProfit > 0
                                                ? price + sign*Data.InstrProperties.Point*OperationTakeProfit
                                                : 0;

                        string message = string.Format(symbol + " " + Data.PeriodMTStr + " " +
                                                       Language.T("A modify order sent") + ": " +
                                                       Language.T("Stop Loss") + " {0}, " +
                                                       Language.T("Take Profit") + " {1}",
                                                       stoploss.ToString(Data.FF),
                                                       takeprofit.ToString(Data.FF));
                        var jmsg = new JournalMessage(JournalIcons.Recalculate, DateTime.Now, message);
                        AppendJournalMessage(jmsg);
                        Log(message);

                        string parameters = "TS1=" + OperationTrailingStop + ";BRE=" + OperationBreakEven;

                        bool responseOk = bridge.OrderModify(ticket, price, stopLossPips, OperationTakeProfit,
                                                             parameters);

                        if (responseOk)
                        {
                            Data.AddBarStats(OperationType.Modify, lots, price);
                            Data.WrongStopLoss = 0;
                            Data.WrongTakeProf = 0;
                            Data.WrongStopsRetry = 0;
                        }
                        else
                        {
                            ReportOperationError();
                            Data.WrongStopLoss = stopLossPips;
                            Data.WrongTakeProf = OperationTakeProfit;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        ///     Use logical groups menu item.
        /// </summary>
        protected override void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            if (mi.Checked)
            {
                Configs.UseLogicalGroups = mi.Checked;
                RebuildStrategyLayout();
                return;
            }

            // Check if the current strategy uses logical groups
            bool usefroup = false;
            var closegroup = new List<string>();
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                if (slot.SlotType == SlotTypes.OpenFilter && slot.LogicalGroup != "A")
                    usefroup = true;

                if (slot.SlotType == SlotTypes.CloseFilter)
                {
                    if (closegroup.Contains(slot.LogicalGroup) || slot.LogicalGroup == "all")
                        usefroup = true;
                    else
                        closegroup.Add(slot.LogicalGroup);
                }
            }

            if (!usefroup)
            {
                Configs.UseLogicalGroups = false;
                RebuildStrategyLayout();
            }
            else
            {
                MessageBox.Show(
                    Language.T("The strategy requires logical groups.") + Environment.NewLine +
                    Language.T("\"Use Logical Groups\" option cannot be switched off."),
                    Language.T("Logical Groups"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                mi.Checked = true;
            }
        }

        /// <summary>
        ///     Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected override void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.MAX_ENTRY_FILTERS = (int) mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int) m.Tag == Configs.MAX_ENTRY_FILTERS);

            RebuildStrategyLayout();
        }

        /// <summary>
        ///     Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected override void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.MAX_EXIT_FILTERS = (int) mi.Tag;

            foreach (ToolStripMenuItem m in mi.Owner.Items)
                m.Checked = ((int) m.Tag == Configs.MAX_EXIT_FILTERS);

            RebuildStrategyLayout();
        }

        /// <summary>
        ///     Reset settings
        /// </summary>
        private void ResetSettings()
        {
            DialogResult result = MessageBox.Show(
                Language.T("Do you want to reset all settings?") + Environment.NewLine + Environment.NewLine +
                Language.T("Restart the program to activate the changes!"),
                Language.T("Reset Settings"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
                Configs.ResetParams();
        }

        /// <summary>
        ///     Reset data and stats.
        /// </summary>
        private void ResetTrader()
        {
            tickLocalTime = DateTime.Now; // Prevents ping for one second.
            StopTrade();
            Data.IsConnected = false;

            bridge.ResetBarsManager();

            Data.ResetBidAskClose();
            Data.ResetAccountStats();
            Data.ResetPositionStats();
            Data.ResetBarStats();
            Data.ResetTicks();

            UpdateTickChart(Data.InstrProperties.Point, Data.ListTicks.ToArray());
            UpdateBalanceChart(Data.BalanceData, Data.BalanceDataPoints);
            UpdateChart();
        }

        /// <summary>
        ///     Starts the Calculator.
        /// </summary>
        private void ShowCommandConsole()
        {
            var commandConsole = new CommandConsole();
            commandConsole.Show();
        }

        /// <summary>
        ///     Makes new language file.
        /// </summary>
        private void MakeNewTranslation()
        {
            var nt = new NewTranslation();
            nt.Show();
        }

        /// <summary>
        ///     Edit translation.
        /// </summary>
        private void EditTranslation()
        {
            var et = new EditTranslation();
            et.Show();
        }
    }
}