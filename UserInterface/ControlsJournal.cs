// Controls Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        private readonly List<JournalMessage> _messages = new List<JournalMessage>();
        private bool _isShowSystemMessages = Configs.JournalShowSystemMessages;
        private bool _isShowTicks = Configs.JournalShowTicks;
        private Journal Journal { get; set; }
        private ToolStrip TsJournal { get; set; }

        /// <summary>
        /// Gets or sets if the journal shows ticks.
        /// </summary>
        protected bool JournalShowTicks
        {
            get { return _isShowTicks; }
        }

        /// <summary>
        /// Gets or sets if the journal shows system messages.
        /// </summary>
        protected bool JournalShowSystemMessages
        {
            get { return _isShowSystemMessages; }
        }

        /// <summary>
        /// Initializes page Chart.
        /// </summary>
        private void InitializePageJournal()
        {
            // tabPageJournal
            TabPageJournal.Name = "tabPageJournal";
            TabPageJournal.Text = Language.T("Journal");
            TabPageJournal.ImageIndex = 4;

            Journal = new Journal {Parent = TabPageJournal, Dock = DockStyle.Fill};

            TsJournal = new ToolStrip {Parent = TabPageJournal, Dock = DockStyle.Top};

            var fontMessage = new Font(Font.FontFamily, 9);
            Graphics g = CreateGraphics();
            float fTimeWidth = g.MeasureString(DateTime.Now.ToString(Data.DF + " HH:mm:ss"), fontMessage).Width;
            g.Dispose();

            var lblTime = new ToolStripLabel(Language.T("Time")) {AutoSize = false, Width = 16 + (int) fTimeWidth - 5};
            TsJournal.Items.Add(lblTime);

            TsJournal.Items.Add(new ToolStripSeparator());

            var lblMessage = new ToolStripLabel(Language.T("Message")) {AutoSize = false, Width = 250};
            TsJournal.Items.Add(lblMessage);

            // Tool strip buttons
            var tsbClear = new ToolStripButton
                               {
                                   Image = Resources.clear,
                                   DisplayStyle = ToolStripItemDisplayStyle.Image,
                                   Alignment = ToolStripItemAlignment.Right,
                                   ToolTipText = Language.T("Clear journal's messages.")
                               };
            tsbClear.Click += TsbClearClick;
            TsJournal.Items.Add(tsbClear);

            var sep = new ToolStripSeparator {Alignment = ToolStripItemAlignment.Right};
            TsJournal.Items.Add(sep);

            var tscbxJounalLength = new ToolStripComboBox
                                        {
                                            Alignment = ToolStripItemAlignment.Right,
                                            DropDownStyle = ComboBoxStyle.DropDownList,
                                            AutoSize = false,
                                            Size = new Size(60, 25)
                                        };
            tscbxJounalLength.Items.AddRange(new object[] {"20", "200", "500", "1000", "5000", "10000"});
            tscbxJounalLength.SelectedItem = Configs.JournalLength.ToString(CultureInfo.InvariantCulture);
            tscbxJounalLength.ToolTipText = Language.T("Maximum messages in the journal.");
            tscbxJounalLength.SelectedIndexChanged += TscbxJounalLenghtSelectedIndexChanged;
            TsJournal.Items.Add(tscbxJounalLength);

            var tsbShowTicks = new ToolStripButton
                                   {
                                       Image = Resources.show_ticks,
                                       DisplayStyle = ToolStripItemDisplayStyle.Image,
                                       Alignment = ToolStripItemAlignment.Right,
                                       Checked = _isShowTicks,
                                       ToolTipText = Language.T("Show incoming ticks.")
                                   };
            tsbShowTicks.Click += TsbShowTicksClick;
            TsJournal.Items.Add(tsbShowTicks);

            var tsbShowSystemMessages = new ToolStripButton
                                            {
                                                Image = Resources.show_system_messages,
                                                DisplayStyle = ToolStripItemDisplayStyle.Image,
                                                Alignment = ToolStripItemAlignment.Right,
                                                Checked = _isShowSystemMessages,
                                                ToolTipText = Language.T("Show system messages.")
                                            };
            tsbShowSystemMessages.Click += TsbShowSystemsClick;
            TsJournal.Items.Add(tsbShowSystemMessages);

            var sep1 = new ToolStripSeparator {Alignment = ToolStripItemAlignment.Right};
            TsJournal.Items.Add(sep1);

            var tsbSaveJournal = new ToolStripButton
                                     {
                                         Image = Resources.save,
                                         DisplayStyle = ToolStripItemDisplayStyle.Image,
                                         Alignment = ToolStripItemAlignment.Right,
                                         Checked = _isShowSystemMessages,
                                         ToolTipText = Language.T("Save journal.")
                                     };
            tsbSaveJournal.Click += TsbSaveJournalClick;
            TsJournal.Items.Add(tsbSaveJournal);
        }

        /// <summary>
        /// Journal Length changed
        /// </summary>
        private void TscbxJounalLenghtSelectedIndexChanged(object sender, EventArgs e)
        {
            var comboBox = (ToolStripComboBox) sender;
            Configs.JournalLength = int.Parse(comboBox.SelectedItem.ToString());
            if (_messages.Count > Configs.JournalLength)
                _messages.RemoveRange(0, _messages.Count - Configs.JournalLength);

            TabPageJournal.Select();
            UpdateJournal(_messages);
        }

        /// <summary>
        /// Page journal was selected.
        /// </summary>
        private void PageJournalSelected()
        {
            Journal.SelectVScrollBar();
        }

        /// <summary>
        /// Clears the journal messages.
        /// </summary>
        private void TsbClearClick(object sender, EventArgs e)
        {
            _messages.Clear();
            Journal.ClearMessages();
        }

        /// <summary>
        /// Journal starts showing ticks.
        /// </summary>
        private void TsbShowTicksClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            btn.Checked = !btn.Checked;
            _isShowTicks = btn.Checked;
            Configs.JournalShowTicks = _isShowTicks;
        }

        /// <summary>
        /// Journal starts showing system messages.
        /// </summary>
        private void TsbShowSystemsClick(object sender, EventArgs e)
        {
            var btn = (ToolStripButton) sender;
            btn.Checked = !btn.Checked;
            _isShowSystemMessages = btn.Checked;
            Configs.JournalShowSystemMessages = _isShowSystemMessages;
        }

        /// <summary>
        /// Saves journal to a file.
        /// </summary>
        private void TsbSaveJournalClick(object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            foreach (JournalMessage message in _messages)
                sb.AppendLine(message.Time.ToString("yyyy-MM-dd hh:mm:ss") + "," + message.Message);

            string fileName = Data.Strategy.StrategyName + "_" + Data.Symbol + "_" + Data.PeriodMTStr + "_" +
                              Data.ConnectionID + ".log";

            SaveDataFile(fileName, sb);
        }

        /// <summary>
        /// Adds a message to the journal.
        /// </summary>
        protected void AppendJournalMessage(JournalMessage message)
        {
            _messages.Add(message);
            if (_messages.Count > Configs.JournalLength)
                _messages.RemoveRange(0, _messages.Count - Configs.JournalLength);

            UpdateJournal(_messages);

            if (Configs.WriteLogFile)
                Data.Logger.WriteLogLine(message.Message);
        }

        /// <summary>
        /// Updates journal.
        /// </summary>
        private void UpdateJournal(List<JournalMessage> newMessages)
        {
            if (Journal.InvokeRequired)
            {
                Journal.BeginInvoke(new UpdateJournalDelegate(UpdateJournal), new object[] {newMessages});
            }
            else
            {
                Journal.UpdateMessages(newMessages);
            }
        }

        /// <summary>
        /// Sets the colors of tab page Journal.
        /// </summary>
        private void SetJournalColors()
        {
            TabPageJournal.BackColor = LayoutColors.ColorFormBack;
            Journal.SetColors();
        }

        private void SaveDataFile(string fileName, StringBuilder data)
        {
            var sfdExport = new SaveFileDialog
                                {
                                    AddExtension = true,
                                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                    Title = Language.T("Save"),
                                    Filter =
                                        "Log file (*.log)|*.log|Excel file (*.xls)|*.xls|Text files (*.txt)|*.txt|All files (*.*)|*.*",
                                    FileName = fileName
                                };

            if (sfdExport.ShowDialog() != DialogResult.OK) return;
            try
            {
                var sw = new StreamWriter(sfdExport.FileName);
                sw.Write(data.ToString());
                sw.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        #region Nested type: UpdateJournalDelegate

        private delegate void UpdateJournalDelegate(List<JournalMessage> newMessages);

        #endregion
    }
}