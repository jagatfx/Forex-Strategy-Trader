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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    public class MenuAndStatusBar : Workspace
    {
        protected MenuAndStatusBar()
        {
            InitializeMenu();
            InitializeStatusBar();
        }

        protected ToolStripMenuItem MiForex { get; private set; }
        protected ToolStripMenuItem MiLiveContent { get; private set; }
        protected ToolStripMenuItem MiStrategyAUPBV { get; private set; }
        protected ToolStripMenuItem MiTabAccount { get; private set; }
        protected ToolStripMenuItem MiTabChart { get; private set; }
        protected ToolStripMenuItem MiTabJournal { get; private set; }
        protected ToolStripMenuItem MiTabOperation { get; private set; }
        protected ToolStripMenuItem MiTabStatus { get; private set; }
        protected ToolStripMenuItem MiTabStrategy { get; private set; }
        private ToolStripStatusLabel LblConnIcon { get; set; }
        private ToolStripStatusLabel LblConnMarket { get; set; }
        private ToolStripStatusLabel LblEquityInfo { get; set; }
        private ToolStripStatusLabel LblPositionInfo { get; set; }
        private ToolStripStatusLabel LblTickInfo { get; set; }

        /// <summary>
        ///     Sets the Main Menu.
        /// </summary>
        private void InitializeMenu()
        {
            // File
            var miFile = new ToolStripMenuItem(Language.T("File"));

            var miNew = new ToolStripMenuItem
                {
                    Text = Language.T("New"),
                    Image = Resources.strategy_new,
                    ShortcutKeys = Keys.Control | Keys.N,
                    ToolTipText = Language.T("Open the default strategy \"New.xml\".")
                };
            miNew.Click += MenuStrategyNew_OnClick;
            miFile.DropDownItems.Add(miNew);

            var miOpen = new ToolStripMenuItem
                {
                    Text = Language.T("Open..."),
                    Image = Resources.strategy_open,
                    ShortcutKeys = Keys.Control | Keys.O,
                    ToolTipText = Language.T("Open a strategy.")
                };
            miOpen.Click += MenuFileOpen_OnClick;
            miFile.DropDownItems.Add(miOpen);

            var miSave = new ToolStripMenuItem
                {
                    Text = Language.T("Save"),
                    Image = Resources.strategy_save,
                    ShortcutKeys = Keys.Control | Keys.S,
                    ToolTipText = Language.T("Save the strategy.")
                };
            miSave.Click += MenuFileSave_OnClick;
            miFile.DropDownItems.Add(miSave);

            var miSaveAs = new ToolStripMenuItem
                {
                    Text = Language.T("Save As") + "...",
                    Image = Resources.strategy_save_as,
                    ToolTipText = Language.T("Save a copy of the strategy.")
                };
            miSaveAs.Click += MenuFileSaveAs_OnClick;
            miFile.DropDownItems.Add(miSaveAs);

            miFile.DropDownItems.Add(new ToolStripSeparator());

            var miClose = new ToolStripMenuItem
                {
                    Text = Language.T("Exit"),
                    Image = Resources.exit,
                    ToolTipText = Language.T("Close the program."),
                    ShortcutKeys = Keys.Control | Keys.X
                };
            miClose.Click += MenuFileCloseOnClick;
            miFile.DropDownItems.Add(miClose);

            // Edit
            var miEdit = new ToolStripMenuItem(Language.T("Edit"));

            var miStrategyUndo = new ToolStripMenuItem
                {
                    Text = Language.T("Undo"),
                    Image = Resources.strategy_undo,
                    ToolTipText = Language.T("Undo the last change in the strategy."),
                    ShortcutKeys = Keys.Control | Keys.Z
                };
            miStrategyUndo.Click += MenuStrategyUndo_OnClick;
            miEdit.DropDownItems.Add(miStrategyUndo);

            miEdit.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyCopy = new ToolStripMenuItem
                {
                    Text = Language.T("Copy Strategy"),
                    ToolTipText = Language.T("Copy the entire strategy to the clipboard."),
                    Image = Resources.copy
                };
            miStrategyCopy.Click += MenuStrategyCopy_OnClick;
            miEdit.DropDownItems.Add(miStrategyCopy);

            var miStrategyPaste = new ToolStripMenuItem
                {
                    Text = Language.T("Paste Strategy"),
                    ToolTipText = Language.T("Load a strategy from the clipboard."),
                    Image = Resources.paste,
                    ShortcutKeys = Keys.Control | Keys.V
                };
            miStrategyPaste.Click += MenuStrategyPaste_OnClick;
            miEdit.DropDownItems.Add(miStrategyPaste);

            //View
            var miView = new ToolStripMenuItem(Language.T("View"));

            var miLanguage = new ToolStripMenuItem {Text = "Language", Image = Resources.lang};
            for (int i = 0; i < Language.LanguageList.Length; i++)
            {
                var miLang = new ToolStripMenuItem {Text = Language.LanguageList[i], Name = Language.LanguageList[i]};
                miLang.Checked = miLang.Name == Configs.Language;
                miLang.Click += Language_Click;
                miLanguage.DropDownItems.Add(miLang);
            }

            miView.DropDownItems.Add(miLanguage);

            var miLanguageTools = new ToolStripMenuItem
                {Text = Language.T("Language Tools"), Image = Resources.lang_tools};

            var miNewTranslation = new ToolStripMenuItem
                {
                    Name = "miNewTranslation",
                    Text = Language.T("Make New Translation") + "...",
                    Image = Resources.new_translation
                };
            miNewTranslation.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miNewTranslation);

            var miEditTranslation = new ToolStripMenuItem
                {
                    Name = "miEditTranslation",
                    Text = Language.T("Edit Current Translation") + "...",
                    Image = Resources.edit_translation
                };
            miEditTranslation.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miEditTranslation);

            miLanguageTools.DropDownItems.Add(new ToolStripSeparator());

            var miShowEnglishPhrases = new ToolStripMenuItem
                {
                    Name = "miShowEnglishPhrases",
                    Text = Language.T("Show English Phrases") + "...",
                    Image = Resources.view_translation
                };
            miShowEnglishPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowEnglishPhrases);

            var miShowAltPhrases = new ToolStripMenuItem
                {
                    Name = "miShowAltPhrases",
                    Text = Language.T("Show Translated Phrases") + "...",
                    Image = Resources.view_translation
                };
            miShowAltPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowAltPhrases);

            var miShowBothPhrases = new ToolStripMenuItem
                {
                    Name = "miShowAllPhrases",
                    Text = Language.T("Show All Phrases") + "...",
                    Image = Resources.view_translation
                };
            miShowBothPhrases.Click += MenuTools_OnClick;
            miLanguageTools.DropDownItems.Add(miShowBothPhrases);

            miView.DropDownItems.Add(miLanguageTools);

            miView.DropDownItems.Add(new ToolStripSeparator());

            MiTabStatus = new ToolStripMenuItem
                {
                    Text = Language.T("Status page"),
                    Name = "miStatus",
                    Tag = 0,
                    Checked = true,
                    ShortcutKeys = Keys.Control | Keys.D1
                };
            MiTabStatus.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabStatus);

            MiTabStrategy = new ToolStripMenuItem
                {
                    Text = Language.T("Strategy page"),
                    Name = "miStrategy",
                    Tag = 1,
                    Checked = false,
                    ShortcutKeys = Keys.Control | Keys.D2
                };
            MiTabStrategy.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabStrategy);

            MiTabChart = new ToolStripMenuItem
                {
                    Text = Language.T("Chart page"),
                    Name = "miChart",
                    Tag = 2,
                    Checked = false,
                    ShortcutKeys = Keys.Control | Keys.D3
                };
            MiTabChart.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabChart);

            MiTabAccount = new ToolStripMenuItem
                {
                    Text = Language.T("Account page"),
                    Name = "miAccount",
                    Tag = 3,
                    Checked = false,
                    ShortcutKeys = Keys.Control | Keys.D4
                };
            MiTabAccount.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabAccount);

            MiTabJournal = new ToolStripMenuItem
                {
                    Text = Language.T("Journal page"),
                    Name = "miJournal",
                    Tag = 4,
                    Checked = false,
                    ShortcutKeys = Keys.Control | Keys.D5
                };
            MiTabJournal.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabJournal);

            MiTabOperation = new ToolStripMenuItem
                {
                    Text = Language.T("Operation page"),
                    Name = "miOperation",
                    Tag = 5,
                    Checked = false,
                    ShortcutKeys = Keys.Control | Keys.D6
                };
            MiTabOperation.Click += MenuChangeTabs_OnClick;
            miView.DropDownItems.Add(MiTabOperation);

            miView.DropDownItems.Add(new ToolStripSeparator());

            var miLoadColor = new ToolStripMenuItem {Text = Language.T("Colour Scheme"), Image = Resources.palette};
            for (int i = 0; i < LayoutColors.ColorSchemeList.Length; i++)
            {
                var miColor = new ToolStripMenuItem
                    {Text = LayoutColors.ColorSchemeList[i], Name = LayoutColors.ColorSchemeList[i]};
                miColor.Checked = miColor.Name == Configs.ColorScheme;
                miColor.Click += MenuLoadColor_OnClick;
                miLoadColor.DropDownItems.Add(miColor);
            }

            miView.DropDownItems.Add(miLoadColor);

            var miGradientView = new ToolStripMenuItem
                {
                    Text = Language.T("Gradient View"),
                    Name = "miGradientView",
                    Checked = Configs.GradientView,
                    CheckOnClick = true
                };
            miGradientView.Click += MenuGradientView_OnClick;
            miView.DropDownItems.Add(miGradientView);

            // Strategy
            var miStrategy = new ToolStripMenuItem(Language.T("Strategy"));

            var miStrategyOverview = new ToolStripMenuItem
                {
                    Text = Language.T("Overview") + "...",
                    Image = Resources.overview,
                    ToolTipText = Language.T("See the strategy overview."),
                    ShortcutKeys = Keys.F4
                };
            miStrategyOverview.Click += MenuStrategyOverview_OnClick;
            miStrategy.DropDownItems.Add(miStrategyOverview);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyPublish = new ToolStripMenuItem
                {
                    Text = Language.T("Publish") + "...",
                    Image = Resources.publish_strategy,
                    ToolTipText = Language.T("Publish the strategy in the program's forum.")
                };
            miStrategyPublish.Click += MenuStrategyBBcode_OnClick;
            miStrategy.DropDownItems.Add(miStrategyPublish);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miUseLogicalGroups = new ToolStripMenuItem
                {
                    Text = Language.T("Use Logical Groups"),
                    ToolTipText =
                        Language.T("Groups add AND and OR logic interaction of the indicators."),
                    Checked = Configs.UseLogicalGroups,
                    CheckOnClick = true
                };
            miUseLogicalGroups.Click += MenuUseLogicalGroups_OnClick;
            miStrategy.DropDownItems.Add(miUseLogicalGroups);

            var miOpeningLogicConditions = new ToolStripMenuItem
                {
                    Text = Language.T("Max number of Opening Logic Conditions"),
                    Image = Resources.numb_gr
                };
            miStrategy.DropDownItems.Add(miOpeningLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                var miOpeningLogicSlots = new ToolStripMenuItem
                    {
                        Text = i.ToString(CultureInfo.InvariantCulture),
                        Tag = i,
                        Checked = (Configs.MAX_ENTRY_FILTERS == i)
                    };
                miOpeningLogicSlots.Click += MenuOpeningLogicSlots_OnClick;
                miOpeningLogicConditions.DropDownItems.Add(miOpeningLogicSlots);
            }

            var miClosingLogicConditions = new ToolStripMenuItem
                {
                    Text = Language.T("Max number of Closing Logic Conditions"),
                    Image = Resources.numb_br
                };
            miStrategy.DropDownItems.Add(miClosingLogicConditions);

            for (int i = 2; i < 9; i++)
            {
                var miClosingLogicSlots = new ToolStripMenuItem
                    {
                        Text = i.ToString(CultureInfo.InvariantCulture),
                        Tag = i,
                        Checked = (Configs.MAX_EXIT_FILTERS == i)
                    };
                miClosingLogicSlots.Click += MenuClosingLogicSlots_OnClick;
                miClosingLogicConditions.DropDownItems.Add(miClosingLogicSlots);
            }

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miStrategyRemember = new ToolStripMenuItem
                {
                    Text = Language.T("Remember the Last Strategy"),
                    ToolTipText = Language.T("Load the last used strategy at startup."),
                    Checked = Configs.RememberLastStr,
                    CheckOnClick = true
                };
            miStrategyRemember.Click += MenuStrategyRemember_OnClick;
            miStrategy.DropDownItems.Add(miStrategyRemember);

            MiStrategyAUPBV = new ToolStripMenuItem
                {
                    Text = Language.T("Auto Control of \"Use previous bar value\""),
                    ToolTipText =
                        Language.T(
                            "Provides automatic setting of the indicators' check box \"Use previous bar value\"."),
                    Checked = true,
                    CheckOnClick = true
                };
            MiStrategyAUPBV.Click += MenuStrategyAUPBV_OnClick;
            miStrategy.DropDownItems.Add(MiStrategyAUPBV);

            miStrategy.DropDownItems.Add(new ToolStripSeparator());

            var miStrategySettings = new ToolStripMenuItem
                {Text = Language.T("Trade Settings"), Image = Resources.strategy_settings};
            miStrategySettings.Click += MenuTradeSettings_OnClick;
            miStrategy.DropDownItems.Add(miStrategySettings);

            // Tools
            var miTools = new ToolStripMenuItem(Language.T("Tools"));

            var miCustomInd = new ToolStripMenuItem
                {
                    Name = "CustomIndicators",
                    Text = Language.T("Custom Indicators"),
                    Image = Resources.custom_ind
                };

            var miReloadInd = new ToolStripMenuItem
                {
                    Name = "miReloadInd",
                    Text = Language.T("Reload the Custom Indicators"),
                    Image = Resources.reload_ind,
                    ShortcutKeys = Keys.Control | Keys.I
                };
            miReloadInd.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miReloadInd);

            var miCheckInd = new ToolStripMenuItem
                {
                    Name = "miCheckInd",
                    Text = Language.T("Check the Custom Indicators"),
                    Image = Resources.check_ind
                };
            miCheckInd.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miCheckInd);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            var miOpenIndFolder = new ToolStripMenuItem
                {
                    Name = "miOpenIndFolder",
                    Text = Language.T("Open the Source Files Folder") + "...",
                    Image = Resources.folder_open
                };
            miOpenIndFolder.Click += MenuTools_OnClick;
            miCustomInd.DropDownItems.Add(miOpenIndFolder);

            var miCustIndForum = new ToolStripMenuItem
                {
                    Text = Language.T("Custom Indicators Forum") + "...",
                    Image = Resources.forum_icon,
                    Tag = "http://forexsb.com/forum/forum/30/"
                };
            miCustIndForum.Click += MenuHelpContentsOnClick;
            miCustomInd.DropDownItems.Add(miCustIndForum);

            miCustomInd.DropDownItems.Add(new ToolStripSeparator());

            var miLoadCstomInd = new ToolStripMenuItem
                {
                    Name = "miLoadCstomInd",
                    Text = Language.T("Load the Custom Indicators at Startup"),
                    Checked = Configs.LoadCustomIndicators,
                    CheckOnClick = true
                };
            miLoadCstomInd.Click += LoadCustomIndicators_OnClick;
            miCustomInd.DropDownItems.Add(miLoadCstomInd);

            var miShowCstomInd = new ToolStripMenuItem
                {
                    Name = "miShowCstomInd",
                    Text = Language.T("Show the Loaded Custom Indicators"),
                    Checked = Configs.ShowCustomIndicators,
                    CheckOnClick = true
                };
            miShowCstomInd.Click += ShowCustomIndicators_OnClick;
            miCustomInd.DropDownItems.Add(miShowCstomInd);

            miTools.DropDownItems.Add(miCustomInd);
            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miMultipleInstances = new ToolStripMenuItem
                {
                    Text = Language.T("Allow multiple working copies of FST"),
                    Name = "miMultipleInstances",
                    Checked = Configs.MultipleInstances,
                    CheckOnClick = true
                };
            miMultipleInstances.Click += MenuMultipleInstances_OnClick;
            miTools.DropDownItems.Add(miMultipleInstances);

            var miPlaySounds = new ToolStripMenuItem
                {
                    Text = Language.T("Play Sounds"),
                    Name = "miPlaySounds",
                    Checked = Configs.PlaySounds,
                    CheckOnClick = true
                };
            miPlaySounds.Click += MenuPlaySounds_OnClick;
            miTools.DropDownItems.Add(miPlaySounds);

            var miLogFile = new ToolStripMenuItem
                {
                    Text = Language.T("Write Log File"),
                    Name = "miLogFile",
                    Checked = Configs.WriteLogFile,
                    CheckOnClick = true
                };
            miLogFile.Click += MenuWriteLogFileOnClick;
            miTools.DropDownItems.Add(miLogFile);
            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miAdditional = new ToolStripMenuItem {Text = Language.T("Additional"), Image = Resources.tools};

            miTools.DropDownItems.Add(miAdditional);

            var miCommandConsole = new ToolStripMenuItem
                {
                    Name = "CommandConsole",
                    Text = Language.T("Command Console") + "...",
                    Image = Resources.prompt
                };
            miCommandConsole.Click += MenuTools_OnClick;
            miAdditional.DropDownItems.Add(miCommandConsole);

            miTools.DropDownItems.Add(new ToolStripSeparator());

            var miInstallExpert = new ToolStripMenuItem
                {
                    Name = "miInstallExpert",
                    Text = Language.T("Install MetaTrader Files"),
                    ToolTipText =
                        Language.T("Install the necessary files in the MetaTrader's folder."),
                    Image = Resources.expert_advisor
                };
            miInstallExpert.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miInstallExpert);

            var miResetTrader = new ToolStripMenuItem
                {
                    Name = "miResetTrader",
                    Text = Language.T("Reset Data and Statistics"),
                    ToolTipText =
                        Language.T(
                            "Reset the loaded data and statistics. It will stop the auto trading!"),
                    Image = Resources.recalculate
                };
            miResetTrader.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miResetTrader);

            var miResetConfigs = new ToolStripMenuItem
                {
                    Name = "Reset settings",
                    Text = Language.T("Reset Settings"),
                    ToolTipText =
                        Language.T(
                            "Reset the program settings to their default values. You need to restart!"),
                    Image = Resources.warning
                };
            miResetConfigs.Click += MenuTools_OnClick;
            miTools.DropDownItems.Add(miResetConfigs);

            // Help
            var miHelp = new ToolStripMenuItem(Language.T("Help"));

            var miTipOfTheDay = new ToolStripMenuItem
                {
                    Text = Language.T("Tip of the Day") + "...",
                    ToolTipText = Language.T("Show a tip."),
                    Image = Resources.hint,
                    Tag = "tips"
                };
            miTipOfTheDay.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miTipOfTheDay);

            var miHelpOnlineHelp = new ToolStripMenuItem
                {
                    Text = Language.T("Online Help") + "...",
                    Image = Resources.help,
                    ToolTipText = Language.T("Show the online help."),
                    Tag = "http://forexsb.com/wiki/fst/manual/start",
                    ShortcutKeys = Keys.F1
                };
            miHelpOnlineHelp.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpOnlineHelp);

            var miHelpForum = new ToolStripMenuItem
                {
                    Text = Language.T("Support Forum") + "...",
                    Image = Resources.forum_icon,
                    Tag = "http://forexsb.com/forum/",
                    ToolTipText = Language.T("Show the program's forum.")
                };
            miHelpForum.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpForum);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpDonateNow = new ToolStripMenuItem
                {
                    Text = Language.T("Contribute") + "...",
                    Image = Resources.contribute,
                    ToolTipText = Language.T("Donate, Support, Advertise!"),
                    Tag = "http://forexsb.com/wiki/contribution"
                };
            miHelpDonateNow.Click += MenuHelpContentsOnClick;
            miHelp.DropDownItems.Add(miHelpDonateNow);

            var miUsageStats = new ToolStripMenuItem
                {
                    Text = Language.T("Send anonymous usage statistics"),
                    Checked = Configs.SendUsageStats,
                    CheckOnClick = true
                };
            miUsageStats.Click += MenuUsageStats_OnClick;
            miHelp.DropDownItems.Add(miUsageStats);

            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpUpdates = new ToolStripMenuItem
                {
                    Text = Language.T("Check for Updates at Startup"),
                    Checked = Configs.CheckForUpdates,
                    CheckOnClick = true
                };
            miHelpUpdates.Click += MenuHelpUpdates_OnClick;
            miHelp.DropDownItems.Add(miHelpUpdates);

            var miHelpNewBeta = new ToolStripMenuItem
                {
                    Text = Language.T("Check for New Beta Versions"),
                    Checked = Configs.CheckForNewBeta,
                    CheckOnClick = true
                };
            miHelpNewBeta.Click += MenuHelpNewBeta_OnClick;
            miHelp.DropDownItems.Add(miHelpNewBeta);


            miHelp.DropDownItems.Add(new ToolStripSeparator());

            var miHelpAbout = new ToolStripMenuItem
                {
                    Text = Language.T("About") + " " + Data.ProgramName + "...",
                    ToolTipText = Language.T("Show the program information."),
                    Image = Resources.information
                };
            miHelpAbout.Click += MenuHelpAboutOnClick;
            miHelp.DropDownItems.Add(miHelpAbout);

            // Forex
            MiForex = new ToolStripMenuItem(Language.T("Forex"));

            var miForexBrokers = new ToolStripMenuItem
                {
                    Text = Language.T("Forex Brokers") + "...",
                    Image = Resources.forex_brokers,
                    Tag = @"http://forexsb.com/wiki/brokers"
                };
            miForexBrokers.Click += MenuForexContentsOnClick;

            MiForex.DropDownItems.Add(miForexBrokers);

            // LiveContent
            MiLiveContent = new ToolStripMenuItem(Language.T("New Version"))
                {
                    Alignment = ToolStripItemAlignment.Right,
                    BackColor = Color.Khaki,
                    ForeColor = Color.DarkGreen,
                    Visible = false
                };

            // Forex Forum
            var miForum = new ToolStripMenuItem(Resources.forum_icon)
                {
                    Alignment = ToolStripItemAlignment.Right,
                    Tag = "http://forexsb.com/forum/",
                    ToolTipText = Language.T("Show the program's forum.")
                };
            miForum.Click += MenuForexContentsOnClick;

            // MainMenu
            ToolStripItem[] mainMenu =
                {
                    miFile, miEdit, miView, miStrategy, miTools,
                    miHelp, MiForex, MiLiveContent, miForum
                };

            MainMenuStrip.Items.AddRange(mainMenu);
            MainMenuStrip.ShowItemToolTips = true;
        }

        /// <summary>
        ///     Sets the StatusBar
        /// </summary>
        private void InitializeStatusBar()
        {
            StatusStrip.GripStyle = ToolStripGripStyle.Hidden;
            StatusStrip.SizingGrip = false;

            LblEquityInfo = new ToolStripStatusLabel
                {
                    Image = Resources.currency,
                    Text = string.Format("{0} {1}", Data.AccountEquity, Data.AccountCurrency)
                };
            StatusStrip.Items.Add(LblEquityInfo);

            StatusStrip.Items.Add(new ToolStripSeparator());

            LblPositionInfo = new ToolStripStatusLabel
                {
                    Image = null,
                    Text = String.Empty,
                    Spring = true,
                    ImageAlign = ContentAlignment.MiddleLeft,
                    TextAlign = ContentAlignment.MiddleLeft
                };
            StatusStrip.Items.Add(LblPositionInfo);

            StatusStrip.Items.Add(new ToolStripSeparator());

            LblConnMarket = new ToolStripStatusLabel {Text = "Not Connected"};
            StatusStrip.Items.Add(LblConnMarket);

            LblTickInfo = new ToolStripStatusLabel();
            StatusStrip.Items.Add(LblTickInfo);

            StatusStrip.Items.Add(new ToolStripSeparator());

            LblConnIcon = new ToolStripStatusLabel
                {DisplayStyle = ToolStripItemDisplayStyle.Image, Image = Resources.not_connected};
            StatusStrip.Items.Add(LblConnIcon);
        }

        /// <summary>
        ///     Saves the current strategy
        /// </summary>
        protected virtual void MenuFileSave_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Opens the SaveAs menu
        /// </summary>
        protected virtual void MenuFileSaveAs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Opens a saved strategy
        /// </summary>
        protected virtual void MenuFileOpen_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Closes the program
        /// </summary>
        private void MenuFileCloseOnClick(object sender, EventArgs e)
        {
            Close();
        }

        // Sets the programs language
        private void Language_Click(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            if (!mi.Checked)
            {
                string sMessageText = Language.T("Restart the program to activate the changes!");
                MessageBox.Show(sMessageText, Language.T("Language Change"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                Configs.Language = mi.Name;
            }
            foreach (ToolStripMenuItem tsmi in mi.Owner.Items)
            {
                tsmi.Checked = false;
            }
            mi.Checked = true;
        }

        /// <summary>
        ///     Gradient View Changed
        /// </summary>
        protected virtual void MenuGradientView_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Play Sounds Changed
        /// </summary>
        private void MenuPlaySounds_OnClick(object sender, EventArgs e)
        {
            Configs.PlaySounds = ((ToolStripMenuItem) sender).Checked;
        }

        /// <summary>
        ///     WriteLogFile Changed
        /// </summary>
        private void MenuWriteLogFileOnClick(object sender, EventArgs e)
        {
            Configs.WriteLogFile = ((ToolStripMenuItem) sender).Checked;
            if (!Configs.WriteLogFile) return;
            string fileNameHeader = Data.Symbol + "_" + Data.Period + "_" +
                                    "ID" + Data.ConnectionId + "_";
            Data.Logger.CreateLogFile(fileNameHeader);
        }

        /// <summary>
        ///     Menu Multiple Instances Changed
        /// </summary>
        protected virtual void MenuMultipleInstances_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Load a color scheme
        /// </summary>
        protected virtual void MenuLoadColor_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Loads the default strategy
        /// </summary>
        protected virtual void MenuStrategyNew_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Opens the strategy settings dialogue
        /// </summary>
        protected virtual void MenuStrategyAUPBV_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Remember the last used strategy
        /// </summary>
        private void MenuStrategyRemember_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.RememberLastStr = mi.Checked;
            if (mi.Checked == false)
            {
                Configs.LastStrategy = "";
            }
        }

        /// <summary>
        ///     Opens the strategy overview window
        /// </summary>
        private void MenuStrategyOverview_OnClick(object sender, EventArgs e)
        {
            var so = new Browser(Language.T("Strategy Overview"), Data.Strategy.GenerateHtmlOverview());
            so.Show();
        }

        /// <summary>
        ///     Undoes the strategy
        /// </summary>
        protected virtual void MenuStrategyUndo_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Copies the strategy to clipboard.
        /// </summary>
        protected virtual void MenuStrategyCopy_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Pastes a strategy from clipboard.
        /// </summary>
        protected virtual void MenuStrategyPaste_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Menu MenuOpeningLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuOpeningLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Menu MenuClosingLogicSlots_OnClick
        /// </summary>
        protected virtual void MenuClosingLogicSlots_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Use logical groups menu item.
        /// </summary>
        protected virtual void MenuUseLogicalGroups_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Export the strategy in BBCode format - ready to post in the forum
        /// </summary>
        protected virtual void MenuStrategyBBcode_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Opens the about window
        /// </summary>
        private void MenuHelpAboutOnClick(object sender, EventArgs e)
        {
            var abScr = new AboutScreen();
            abScr.ShowDialog();
        }

        /// <summary>
        ///     Tools menu
        /// </summary>
        protected virtual void MenuTools_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     MenuChangeTabs_OnClick
        /// </summary>
        protected virtual void MenuChangeTabs_OnClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        ///     Trade Settings
        /// </summary>
        private void MenuTradeSettings_OnClick(object sender, EventArgs e)
        {
            var ts = new TradeSettings();
            ts.ShowDialog();
        }

        /// <summary>
        ///     Opens the help window
        /// </summary>
        private void MenuHelpContentsOnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;

            if ((string) mi.Tag == "tips")
            {
                var shv = new StartingTips();
                shv.Show();
                return;
            }

            try
            {
                Process.Start((string) mi.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        ///     Opens the forex news
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

        /// <summary>
        ///     Menu miHelpUpdates click
        /// </summary>
        private void MenuHelpUpdates_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.CheckForUpdates = mi.Checked;
        }

        /// <summary>
        ///     Menu miHelpNewBeta  click
        /// </summary>
        private void MenuHelpNewBeta_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.CheckForNewBeta = mi.Checked;
        }

        /// <summary>
        ///     Menu UsageStatistics click
        /// </summary>
        private void MenuUsageStats_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.SendUsageStats = mi.Checked;
        }

        /// <summary>
        ///     Menu LoadCustomIndicators click
        /// </summary>
        private void LoadCustomIndicators_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.LoadCustomIndicators = mi.Checked;
        }

        /// <summary>
        ///     Menu ShowCustomIndicators click
        /// </summary>
        private void ShowCustomIndicators_OnClick(object sender, EventArgs e)
        {
            var mi = (ToolStripMenuItem) sender;
            Configs.ShowCustomIndicators = mi.Checked;
        }

        /// <summary>
        ///     Sets the tsslEquityInfo
        /// </summary>
        protected void SetEquityInfoText(string text)
        {
            if (LblEquityInfo.Owner.InvokeRequired)
            {
                LblEquityInfo.Owner.BeginInvoke(new SetTickInfoTextDelegate(SetEquityInfoText), new object[] {text});
            }
            else
            {
                LblEquityInfo.Text = text;
            }
        }

        /// <summary>
        ///     Sets the tsslPositionInfo
        /// </summary>
        protected void SetPositionInfoText(Image image, string text)
        {
            if (LblPositionInfo.Owner.InvokeRequired)
            {
                LblPositionInfo.Owner.BeginInvoke(new SetPositionInfoTextDelegate(SetPositionInfoText),
                                                  new object[] {image, text});
            }
            else
            {
                LblPositionInfo.Image = image;
                LblPositionInfo.Text = text;
            }
        }

        /// <summary>
        ///     Sets the tsslConnMarket
        /// </summary>
        protected void SetConnMarketText(string text)
        {
            if (LblConnMarket.Owner.InvokeRequired)
            {
                LblConnMarket.Owner.BeginInvoke(new SetConnMarketTextDelegate(SetConnMarketText), new object[] {text});
            }
            else
            {
                LblConnMarket.Text = text;
            }
        }

        /// <summary>
        ///     Sets the tsslTickInfo
        /// </summary>
        protected void SetTickInfoText(string text)
        {
            if (LblTickInfo.Owner.InvokeRequired)
            {
                LblTickInfo.Owner.BeginInvoke(new SetTickInfoTextDelegate(SetTickInfoText), new object[] {text});
            }
            else
            {
                LblTickInfo.Text = text;
            }
        }

        /// <summary>
        ///     Sets the tsslConnIcon
        /// </summary>
        protected void SetConnIcon(int mode)
        {
            if (LblConnIcon.Owner.InvokeRequired)
            {
                LblConnIcon.Owner.BeginInvoke(new SetConnIconDelegate(SetConnIcon), new object[] {mode});
            }
            else
            {
                switch (mode)
                {
                    case 0: // Not connected
                        LblConnIcon.Image = Resources.not_connected;
                        break;
                    case 1: // Connected, No ticks
                        LblConnIcon.Image = Resources.connected_no_ticks;
                        break;
                    case 2: // Connected, Ticks
                        LblConnIcon.Image = Resources.connected;
                        break;
                    case 3: // Connected, Wrong ping
                        LblConnIcon.Image = Resources.connection_wrong_ping;
                        break;
                    case 4: // Connected, Wrong pings
                        LblConnIcon.Image = Resources.connection_wrong_pings;
                        break;
                }
            }
        }

        #region Nested type: SetConnIconDelegate

        private delegate void SetConnIconDelegate(int mode);

        #endregion

        #region Nested type: SetConnMarketTextDelegate

        private delegate void SetConnMarketTextDelegate(string text);

        #endregion

        #region Nested type: SetPositionInfoTextDelegate

        private delegate void SetPositionInfoTextDelegate(Image image, string text);

        #endregion

        #region Nested type: SetTickInfoTextDelegate

        private delegate void SetTickInfoTextDelegate(string text);

        #endregion
    }
}