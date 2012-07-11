// Actions Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Actions : Controls
    /// </summary>
    public partial class Actions : Controls
    {
        /// <summary>
        /// The starting point of the application
        /// </summary>
        [STAThread]
        public static void Main(params string[] input)
        {
            UpdateSplashScreeStatus("Loading...");
            Data.Start();
            Configs.LoadConfigs();

            CheckIfStartedFromCmdLine(input);
            if (Data.IsAutoStart)
                SetAutostartSettings();

            // Checks if this is the only running copy of FST.
            if (!Configs.MultipleInstances)
            {
                Process[] procs = Process.GetProcessesByName(Data.ProgramName);
                if (procs.Length > 1)
                {
                    RemoveSplashScreen();
                    MessageBox.Show(
                        "Forex Strategy Trader is already running! You can allow multiple instances of the program from Tools menu.",
                        Data.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    return;
                }
            }

            Language.InitLanguages();
            LayoutColors.InitColorSchemes();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Actions());
        }

        /// <summary>
        /// The default constructor
        /// </summary>
        private Actions()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(785, 560);
            MinimumSize = new Size(600, 370);
            Icon = Data.Icon;
            Text = Data.ProgramName;
            FormClosing += ActionsFormClosing;
            Application.Idle += ApplicationIdle;

            // Load a data file
            LoadInstrument();

            // Prepare custom indicators
            UpdateSplashScreeStatus("Loading custom indicators...");
            if (Configs.LoadCustomIndicators)
                CustomIndicators.LoadCustomIndicators();

            // Load a strategy
            UpdateSplashScreeStatus("Loading strategy...");
            string strategyPath = Data.StrategyPath;
            if (Configs.LastStrategy != "" && (Configs.RememberLastStr || Data.ConnectionID > 0) )
            {
                string lastStrategy = Path.GetDirectoryName(Configs.LastStrategy);
                if (lastStrategy != "")
                    lastStrategy = Configs.LastStrategy;
                else
                {
                    string path = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                    lastStrategy = Path.Combine(path, Configs.LastStrategy);
                }
                if (File.Exists(lastStrategy))
                    strategyPath = lastStrategy;
            }

            if (OpenStrategy(strategyPath) == 0)
            {
                CalculateStrategy(true);
                AfterStrategyOpening();
            }

            ChangeTabPage(Configs.LastTab);

            var liveContent = new LiveContent(Data.SystemDir, MILiveContent, MIForex, PnlUsefulLinks, PnlForexBrokers);

            // Starting tips
            if (Configs.ShowStartingTip)
            {
                var startingTips = new StartingTips();
                startingTips.Show();
            }

            UpdateSplashScreeStatus("Loading user interface...");
        }

        private static void CheckIfStartedFromCmdLine(params string[] input)
        {
            if (input.Length != 3) return;
            Data.IsAutoStart = true;
            Data.ConnectionID = int.Parse(input[0]);
            if (input[1] == "yes") Data.StartAutotradeWhenConnected = true;
            Configs.LastStrategy = input[2] + ".xml";
        }

        private static void SetAutostartSettings()
        {
            Configs.LastTab = 4;
            Configs.MultipleInstances = true;
            Configs.CheckForNewBeta = false;
            Configs.CheckForUpdates = false;
            Configs.JournalShowSystemMessages = false;
            Configs.RememberLastStr = false;
            Configs.ShowStartingTip = false;
            Configs.LoadCustomIndicators = true;
            Configs.ShowCustomIndicators = false;
        }

        /// <summary>
        /// Application idle
        /// </summary>
        private void ApplicationIdle(object sender, EventArgs e)
        {
            Application.Idle -= ApplicationIdle;
            RemoveSplashScreen();

            SetSrategyOverview();

            if (!Configs.MultipleInstances)
                InitDataFeed();

            if (Data.IsAutoStart)
                StartTradeWhenConnectionEstablished();
        }

        private void StartTradeWhenConnectionEstablished()
        {
                InitDataFeed();
                TstbxConnectionID.Text = Data.ConnectionID.ToString(CultureInfo.InvariantCulture);
                ConnectionGo();
        }

        /// <summary>
        /// Removes the splash screen.
        /// </summary>
        private static void RemoveSplashScreen()
        {
            string sLockFile = GetLockFile();
            if (!string.IsNullOrEmpty(sLockFile))
                File.Delete(sLockFile);
        }

        /// <summary>
        /// Updates the splash screen label.
        /// </summary>
        private static void UpdateSplashScreeStatus(string comment)
        {
            try
            {
                string lockFile = GetLockFile();
                if(string.IsNullOrEmpty(lockFile)) return;
                TextWriter tw = new StreamWriter(lockFile, false);
                tw.WriteLine(comment);
                tw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// The lockfile name will be passed automatically by Splash.exe as a  
        /// command line argument -lockfile="c:\temp\C1679A85-A4FA-48a2-BF77-E74F73E08768.lock"
        /// </summary>
        /// <returns>Lock file path</returns>
        private static string GetLockFile()
        {
            foreach (string arg in Environment.GetCommandLineArgs())
                if (arg.StartsWith("-lockfile="))
                    return arg.Replace("-lockfile=", String.Empty);

            return string.Empty;
        }

        /// <summary>
        /// Checks whether the strategy have been saved or not
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = WhetherSaveChangedStrategy();

            if (dialogResult == DialogResult.Yes)
                SaveStrategy();
            else if (dialogResult == DialogResult.Cancel)
                e.Cancel = true;

            if (!e.Cancel)
            {
                // Remember the last used strategy
                if (Configs.RememberLastStr)
                {
                    if (Data.LoadedSavedStrategy != "")
                    {
                        string strategyPath = Path.GetDirectoryName(Data.LoadedSavedStrategy) + "\\";
                        string defaultPath = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
                        if (strategyPath == defaultPath)
                            Data.LoadedSavedStrategy = Path.GetFileName(Data.LoadedSavedStrategy);
                    }
                    Configs.LastStrategy = Data.LoadedSavedStrategy;
                }

                DeinitDataFeed();

                if(!Data.IsAutoStart)
                {
                    Configs.SaveConfigs();
                    Hide();
                    Data.SendStats();
                }
            }
        }

// ---------------------------------------------------------- //

        /// <summary>
        /// Edits the Strategy Properties Slot
        /// </summary>
        private void EditStrategyProperties()
        {
            var strprp = new StrategyProperties
                             {
                                 SameDirAverg = Data.Strategy.SameSignalAction,
                                 OppDirAverg = Data.Strategy.OppSignalAction,
                                 UseAccountPercentEntry = Data.Strategy.UseAccountPercentEntry,
                                 MaxOpenLots = Data.Strategy.MaxOpenLots,
                                 EntryLots = Data.Strategy.EntryLots,
                                 AddingLots = Data.Strategy.AddingLots,
                                 ReducingLots = Data.Strategy.ReducingLots,
                                 UsePermanentSL = Data.Strategy.UsePermanentSL,
                                 PermanentSLType = Data.Strategy.PermanentSLType,
                                 PermanentSL = Data.Strategy.PermanentSL,
                                 UsePermanentTP = Data.Strategy.UsePermanentTP,
                                 PermanentTPType = Data.Strategy.PermanentTPType,
                                 PermanentTP = Data.Strategy.PermanentTP,
                                 UseBreakEven = Data.Strategy.UseBreakEven,
                                 BreakEven = Data.Strategy.BreakEven,
                                 UseMartingale = Data.Strategy.UseMartingale,
                                 MartingaleMultiplier = Data.Strategy.MartingaleMultiplier
                             };
            strprp.SetParams();
            strprp.ShowDialog();

            if (strprp.DialogResult != DialogResult.OK) return;
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());

            Data.Strategy.SameSignalAction = strprp.SameDirAverg;
            Data.Strategy.OppSignalAction = strprp.OppDirAverg;
            Data.Strategy.UseAccountPercentEntry = strprp.UseAccountPercentEntry;
            Data.Strategy.MaxOpenLots = strprp.MaxOpenLots;
            Data.Strategy.EntryLots = strprp.EntryLots;
            Data.Strategy.AddingLots = strprp.AddingLots;
            Data.Strategy.ReducingLots = strprp.ReducingLots;
            Data.Strategy.UsePermanentSL = strprp.UsePermanentSL;
            Data.Strategy.PermanentSLType = strprp.PermanentSLType;
            Data.Strategy.PermanentSL = strprp.PermanentSL;
            Data.Strategy.UsePermanentTP = strprp.UsePermanentTP;
            Data.Strategy.PermanentTPType = strprp.PermanentTPType;
            Data.Strategy.PermanentTP = strprp.PermanentTP;
            Data.Strategy.UseBreakEven = strprp.UseBreakEven;
            Data.Strategy.BreakEven = strprp.BreakEven;
            Data.Strategy.UseMartingale = strprp.UseMartingale;
            Data.Strategy.MartingaleMultiplier = strprp.MartingaleMultiplier;

            RebuildStrategyLayout();
            SetSrategyOverview();

            Data.IsStrategyChanged = true;

            CalculateStrategy(false);
        }

        /// <summary>
        /// Edits the Strategy Slot
        /// </summary>
        /// <param name="iSlot">The slot number</param>
        private void EditSlot(int iSlot)
        {
            SlotTypes slotType = Data.Strategy.Slot[iSlot].SlotType;
            bool bIsDefined = Data.Strategy.Slot[iSlot].IsDefined;

            //We put the current Strategy into the stack only if this function is called from the
            //button SlotButton. If it is called from Add/Remove Filters the stack is already updated.
            if (bIsDefined)
            {
                Data.StackStrategy.Push(Data.Strategy.Clone());
            }

            var id = new IndicatorDialog(iSlot, slotType, bIsDefined);
            id.ShowDialog();

            if (id.DialogResult == DialogResult.OK)
            {
                OnStrategyChange();

                Data.IsStrategyChanged = true;

                RebuildStrategyLayout();
                SetSrategyOverview();
            }
            else
            {
                // Cancel was pressed
                UndoStrategy();
            }
        }

        /// <summary>
        /// Moves a Slot Upwards
        /// </summary>
        private void MoveSlotUpwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterUpwards(iSlotToMove);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);
        }

        /// <summary>
        /// Moves a Slot Downwards
        /// </summary>
        private void MoveSlotDownwards(int iSlotToMove)
        {
            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.MoveFilterDownwards(iSlotToMove);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);
        }

        /// <summary>
        /// Duplicates a Slot
        /// </summary>
        private void DuplicateSlot(int iSlotToDuplicate)
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.DuplicateFilter(iSlotToDuplicate);

            Data.IsStrategyChanged = true;

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(true);
        }

        /// <summary>
        /// Adds a new Open filter
        /// </summary>
        private void AddOpenFilter()
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddOpenFilter();
            EditSlot(Data.Strategy.OpenFilters);
        }

        /// <summary>
        /// Adds a new Close filter
        /// </summary>
        private void AddCloseFilter()
        {
            OnStrategyChange();

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.AddCloseFilter();
            EditSlot(Data.Strategy.Slots - 1);
        }

        /// <summary>
        /// Removes a strategy slot.
        /// </summary>
        /// <param name="iSlot">Slot to remove</param>
        private void RemoveSlot(int iSlot)
        {
            OnStrategyChange();

            Data.IsStrategyChanged = true;

            Data.StackStrategy.Push(Data.Strategy.Clone());
            Data.Strategy.RemoveFilter(iSlot);

            RebuildStrategyLayout();
            SetSrategyOverview();

            CalculateStrategy(false);
        }

        /// <summary>
        /// Undoes the strategy
        /// </summary>
        private void UndoStrategy()
        {
            OnStrategyChange();

            if (Data.StackStrategy.Count <= 1)
            {
                Data.IsStrategyChanged = false;
            }

            if (Data.StackStrategy.Count > 0)
            {
                Data.Strategy = Data.StackStrategy.Pop();

                RebuildStrategyLayout();
                SetSrategyOverview();

                CalculateStrategy(true);
            }
        }

        /// <summary>
        /// Performs actions when UPBV has been changed
        /// </summary>
        private void UsePreviousBarValueChange()
        {
            if (MIStrategyAUPBV.Checked == false)
            {
                // Confirmation Message
                string messageText = Language.T("Are you sure you want to control \"Use previous bar value\" manually?");
                DialogResult dialogResult = MessageBox.Show(messageText, Language.T("Use previous bar value"),
                                                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (dialogResult == DialogResult.Yes)
                {
                    // OK, we are sure
                    OnStrategyChange();

                    Data.AutoUsePrvBarValue = false;

                    foreach (IndicatorSlot indicatorSlot in Data.Strategy.Slot)
                        foreach (CheckParam checkParam in indicatorSlot.IndParam.CheckParam)
                            if (checkParam.Caption == "Use previous bar value")
                                checkParam.Enabled = true;
                }
                else
                {
                    // Not just now
                    MIStrategyAUPBV.Checked = true;
                }
            }
            else
            {
                OnStrategyChange();

                Data.AutoUsePrvBarValue = true;
                Data.Strategy.AdjustUsePreviousBarValue();
                RepaintStrategyLayout();
                CalculateStrategy(true);
            }
        }

        /// <summary>
        /// Ask for saving the changed strategy
        /// </summary>
        private DialogResult WhetherSaveChangedStrategy()
        {
            DialogResult dr = DialogResult.No;
            if (Data.IsStrategyChanged)
            {
                string sMessageText = Language.T("Do you want to save the current strategy?") + "\r\n" +
                                      Data.StrategyName;
                dr = MessageBox.Show(sMessageText, Data.ProgramName, MessageBoxButtons.YesNoCancel,
                                     MessageBoxIcon.Question);
            }

            return dr;
        }

        /// <summary>
        /// LoadInstrument
        /// </summary>
        private void LoadInstrument()
        {
            const string symbol = "EURUSD";
            const DataPeriods dataPeriod = DataPeriods.day;

            var instrProperties = new InstrumentProperties(symbol);
            var instrument = new Instrument(instrProperties, (int) dataPeriod);
            int loadResourceData = instrument.LoadResourceData();

            if (instrument.Bars <= 0 || loadResourceData != 0) return;

            Data.InstrProperties = instrProperties.Clone();
            Data.Bars = instrument.Bars;
            Data.Period = dataPeriod;
            Data.Time = new DateTime[Data.Bars];
            Data.Open = new double[Data.Bars];
            Data.High = new double[Data.Bars];
            Data.Low = new double[Data.Bars];
            Data.Close = new double[Data.Bars];
            Data.Volume = new int[Data.Bars];

            for (int bar = 0; bar < Data.Bars; bar++)
            {
                Data.Open[bar] = instrument.Open(bar);
                Data.High[bar] = instrument.High(bar);
                Data.Low[bar] = instrument.Low(bar);
                Data.Close[bar] = instrument.Close(bar);
                Data.Time[bar] = instrument.Time(bar);
                Data.Volume[bar] = instrument.Volume(bar);
            }

            Data.IsData = true;
        }

        /// <summary>
        /// Open a strategy file
        /// </summary>
        private void ShowOpenFileDialog()
        {
            var opendlg = new OpenFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml",
                                  Title = Language.T("Open Strategy")
                              };


            if (opendlg.ShowDialog() == DialogResult.OK)
            {
                LoadStrategyFile(opendlg.FileName);
            }
        }

        /// <summary>
        /// New Strategy
        /// </summary>
        private void NewStrategy()
        {
            Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);
            string strategyfullName = Path.Combine(Data.StrategyDir, "New.xml");
            LoadStrategyFile(strategyfullName);
        }

        /// <summary>
        /// Loads the strategy given.
        /// </summary>
        private void LoadStrategyFile(string strategyfullName)
        {
            try
            {
                OnStrategyChange();

                OpenStrategy(strategyfullName);
                CalculateStrategy(true);
                AfterStrategyOpening();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, Text);
            }
        }

        /// <summary>
        ///Reloads the Custom Indicators.
        /// </summary>
        private void ReloadCustomIndicators()
        {
            // Check if the strategy contains custom indicators
            bool bStrategyHasCustomIndicator = false;
            foreach (IndicatorSlot slot in Data.Strategy.Slot)
            {
                // Searching the strategy slots for a custom indicator
                if (IndicatorStore.CustomIndicatorNames.Contains(slot.IndicatorName))
                {
                    bStrategyHasCustomIndicator = true;
                    break;
                }
            }

            // Reload all the custom indicators
            CustomIndicators.LoadCustomIndicators();

            if (bStrategyHasCustomIndicator)
            {
                // Load and calculate a new strategy
                Data.StrategyDir = Path.Combine(Data.ProgramDir, Data.DefaultStrategyDir);

                if (OpenStrategy(Path.Combine(Data.StrategyDir, "New.xml")) == 0)
                {
                    CalculateStrategy(true);
                    AfterStrategyOpening();
                }
            }
        }

        /// <summary>
        /// Reads the strategy from a file.
        /// </summary>
        /// <param name="strategyName">The strategy name.</param>
        /// <returns>0 - success.</returns>
        private int OpenStrategy(string strategyName)
        {
            try
            {
                if (File.Exists(strategyName) && Strategy.Load(strategyName))
                {
                    // Successfully opened
                    Data.Strategy.StrategyName = Path.GetFileNameWithoutExtension(strategyName);
                    Data.StrategyDir = Path.GetDirectoryName(strategyName);
                    Data.StrategyName = Path.GetFileName(strategyName);
                }
                else
                {
                    Strategy.GenerateNew();
                    string sMessageText = Language.T("The strategy could not be loaded correctly!") +
                                          Environment.NewLine + Language.T("A new strategy has been generated!");
                    MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK,
                                    MessageBoxIcon.Exclamation);
                    Data.LoadedSavedStrategy = "";
                }

                Data.SetStrategyIndicators();

                RebuildStrategyLayout();
                SetSrategyOverview();

                SetFormText();
                Data.IsStrategyChanged = false;
                Data.LoadedSavedStrategy = Data.StrategyPath;

                Data.StackStrategy.Clear();
            }
            catch
            {
                Strategy.GenerateNew();
                string sMessageText = Language.T("The strategy could not be loaded correctly!") + Environment.NewLine +
                                      Language.T("A new strategy has been generated!");
                MessageBox.Show(sMessageText, Language.T("Strategy Loading"), MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                Data.LoadedSavedStrategy = "";
                SetFormText();
                RebuildStrategyLayout();
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveStrategy()
        {
            if (Data.StrategyName == "New.xml")
            {
                SaveAsStrategy();
            }
            else
            {
                try
                {
                    Data.Strategy.Save(Data.StrategyPath);
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.SavedStrategies++;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                }
            }
        }

        /// <summary>
        /// Save the current strategy
        /// </summary>
        private void SaveAsStrategy()
        {
            //Creates a dialog form SaveFileDialog
            var savedlg = new SaveFileDialog
                              {
                                  InitialDirectory = Data.StrategyDir,
                                  FileName = Path.GetFileName(Data.StrategyName),
                                  AddExtension = true,
                                  Title = Language.T("Save the Strategy As"),
                                  Filter = Language.T("Strategy file") + " (*.xml)|*.xml"
                              };

            if (savedlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Data.StrategyName = Path.GetFileName(savedlg.FileName);
                    Data.StrategyDir = Path.GetDirectoryName(savedlg.FileName);
                    Data.Strategy.Save(savedlg.FileName);
                    Data.IsStrategyChanged = false;
                    Data.LoadedSavedStrategy = Data.StrategyPath;
                    Data.SavedStrategies++;
                    SetFormText();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, Text);
                }
            }
        }

        /// <summary>
        /// Calculates the strategy.
        /// </summary>
        /// <param name="recalcIndicators">true - to recalculate all the indicators.</param>
        private void CalculateStrategy(bool recalcIndicators)
        {
            // Calculates the indicators by slots if it's necessary
            if (recalcIndicators)
                Data.FirstBar = Data.Strategy.Calculate();
        }

        /// <summary>
        /// Stops trade and shows a message.
        /// </summary>
        private void AfterStrategyOpening()
        {
            StopTrade();
            var msg = new JournalMessage(JournalIcons.Information, DateTime.Now,
                                         Language.T("Strategy") + " \"" + Data.Strategy.StrategyName + "\" " +
                                         Language.T("loaded successfully."));
            AppendJournalMessage(msg);
        }

        /// <summary>
        /// Stops trade and selects Strategy page.
        /// </summary>
        private void OnStrategyChange()
        {
            // Stops auto trade
            StopTrade();

            // Selects Strategy tab page.
            ChangeTabPage(1);
        }

        /// <summary>
        /// Loads a color scheme.
        /// </summary>
        private void LoadColorScheme()
        {
            string colorSchemeFile = Path.Combine(Data.ColorDir, Configs.ColorScheme + ".xml");

            if (File.Exists(colorSchemeFile))
            {
                LayoutColors.LoadColorScheme(colorSchemeFile);
                SetColors();
            }
        }

        /// <summary>
        /// Sets the caption text of the application.
        /// </summary>
        private void SetFormText()
        {
            string connection = "";
            if (Configs.MultipleInstances)
                connection = "ID=" + Data.ConnectionID + " ";

            if (Data.IsConnected)
                connection += Data.Symbol + " " + Data.PeriodStr + ", ";

            string text = connection + Path.GetFileNameWithoutExtension(Data.StrategyName) + " - " + Data.ProgramName;

            SetFormTextThreadSafely(text);
        }

        private void SetFormTextThreadSafely(string text)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new SetFormTextDelegate(SetFormTextThreadSafely), new object[] {text});
            }
            else
            {
                Text = text;
            }
        }

        #region Nested type: SetFormTextDelegate

        private delegate void SetFormTextDelegate(string text);

        #endregion
    }
}