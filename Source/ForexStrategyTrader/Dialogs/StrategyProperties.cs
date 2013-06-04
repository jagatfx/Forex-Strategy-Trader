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
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder
{
    public sealed class StrategyProperties : Form
    {
        private readonly ToolTip toolTip = new ToolTip();

        public StrategyProperties()
        {
            PermanentTP = 100;
            PermanentTPType = PermanentProtectionType.Relative;
            UsePermanentTP = false;
            BreakEven = 100;
            UseBreakEven = false;
            PermanentSL = 100;
            PermanentSLType = PermanentProtectionType.Relative;
            UsePermanentSL = false;
            PnlAveraging = new FancyPanel(Language.T("Handling of Additional Entry Signals"),
                                          LayoutColors.ColorSlotCaptionBackAveraging, LayoutColors.ColorSlotCaptionText);
            PnlAmounts = new FancyPanel(Language.T("Trading Size"), LayoutColors.ColorSlotCaptionBackAveraging,
                                        LayoutColors.ColorSlotCaptionText);
            PnlProtection = new FancyPanel(Language.T("Permanent Protection"),
                                           LayoutColors.ColorSlotCaptionBackAveraging,
                                           LayoutColors.ColorSlotCaptionText);

            LblPercent1 = new Label();
            LblPercent2 = new Label();
            LblPercent3 = new Label();

            LblAveragingSameDir = new Label();
            LblAveragingOppDir = new Label();

            CbxSameDirAction = new ComboBox();
            CbxOppDirAction = new ComboBox();
            NudMaxOpenLots = new NumericUpDown();
            RbConstantUnits = new RadioButton();
            RbVariableUnits = new RadioButton();
            NudEntryLots = new NumericUpDown();
            NudAddingLots = new NumericUpDown();
            NudReducingLots = new NumericUpDown();
            LblMaxOpenLots = new Label();
            LblEntryLots = new Label();
            LblAddingLots = new Label();
            LblReducingLots = new Label();

            ChbPermaSL = new CheckBox();
            CbxPermaSLType = new ComboBox();
            NudPermaSL = new NumericUpDown();
            ChbPermaTP = new CheckBox();
            CbxPermaTPType = new ComboBox();
            NudPermaTP = new NumericUpDown();
            ChbBreakEven = new CheckBox();
            NudBreakEven = new NumericUpDown();
            CbxUseMartingale = new CheckBox();
            NudMartingaleMultiplier = new NumericUpDown();


            BtnDefault = new Button();
            BtnCancel = new Button();
            BtnAccept = new Button();

            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            AcceptButton = BtnAccept;
            Text = Language.T("Strategy Properties");

            // pnlAveraging
            PnlAveraging.Parent = this;

            // pnlAmounts
            PnlAmounts.Parent = this;

            // pnlProtection
            PnlProtection.Parent = this;

            // Label Same dir action
            LblAveragingSameDir.Parent = PnlAveraging;
            LblAveragingSameDir.ForeColor = LayoutColors.ColorControlText;
            LblAveragingSameDir.BackColor = Color.Transparent;
            LblAveragingSameDir.AutoSize = true;
            LblAveragingSameDir.Text = Language.T("Next same direction signal behaviour");

            // Label Opposite dir action
            LblAveragingOppDir.Parent = PnlAveraging;
            LblAveragingOppDir.ForeColor = LayoutColors.ColorControlText;
            LblAveragingOppDir.BackColor = Color.Transparent;
            LblAveragingOppDir.AutoSize = true;
            LblAveragingOppDir.Text = Language.T("Next opposite direction signal behaviour");

            // ComboBox SameDirAction
            CbxSameDirAction.Parent = PnlAveraging;
            CbxSameDirAction.Name = "cbxSameDirAction";
            CbxSameDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxSameDirAction.Items.AddRange(new object[]
                {Language.T("Nothing"), Language.T("Winner"), Language.T("Add")});
            CbxSameDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxSameDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Winner - adds to a winning position.") + Environment.NewLine +
                               Language.T("Add - adds to all positions."));

            // ComboBox OppDirAction
            CbxOppDirAction.Parent = PnlAveraging;
            CbxOppDirAction.Name = "cbxOppDirAction";
            CbxOppDirAction.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxOppDirAction.Items.AddRange(new object[]
                {
                    Language.T("Nothing"), Language.T("Reduce"), Language.T("Close"),
                    Language.T("Reverse")
                });
            CbxOppDirAction.SelectedIndex = 0;
            toolTip.SetToolTip(CbxOppDirAction,
                               Language.T("Nothing - cancels the additional orders.") + Environment.NewLine +
                               Language.T("Reduce - reduces or closes a position.") + Environment.NewLine +
                               Language.T("Close - closes the position.") + Environment.NewLine +
                               Language.T("Reverse - reverses the position."));

            // Label MaxOpen Lots
            LblMaxOpenLots.Parent = PnlAmounts;
            LblMaxOpenLots.ForeColor = LayoutColors.ColorControlText;
            LblMaxOpenLots.BackColor = Color.Transparent;
            LblMaxOpenLots.AutoSize = true;
            LblMaxOpenLots.Text = Language.T("Maximum number of open lots");

            // NumericUpDown MaxOpen Lots
            NudMaxOpenLots.Parent = PnlAmounts;
            NudMaxOpenLots.Name = "nudMaxOpenLots";
            NudMaxOpenLots.BeginInit();
            NudMaxOpenLots.Minimum = 0.01M;
            NudMaxOpenLots.Maximum = 100;
            NudMaxOpenLots.Increment = 0.01M;
            NudMaxOpenLots.Value = (decimal) MaxOpenLots;
            NudMaxOpenLots.DecimalPlaces = 2;
            NudMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            NudMaxOpenLots.EndInit();

            // Radio Button Constant Units
            RbConstantUnits.Parent = PnlAmounts;
            RbConstantUnits.ForeColor = LayoutColors.ColorControlText;
            RbConstantUnits.BackColor = Color.Transparent;
            RbConstantUnits.Checked = !UseAccountPercentEntry;
            RbConstantUnits.AutoSize = true;
            RbConstantUnits.Name = "rbConstantUnits";
            RbConstantUnits.Text = Language.T("Trade a constant number of lots");

            // Radio Button Variable Units
            RbVariableUnits.Parent = PnlAmounts;
            RbVariableUnits.ForeColor = LayoutColors.ColorControlText;
            RbVariableUnits.BackColor = Color.Transparent;
            RbVariableUnits.Checked = UseAccountPercentEntry;
            RbVariableUnits.AutoSize = false;
            RbVariableUnits.Name = "rbVariableUnits";
            RbVariableUnits.Text =
                Language.T(
                    "Trade a variable number of lots depending on your current account equity. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            LblEntryLots.Parent = PnlAmounts;
            LblEntryLots.ForeColor = LayoutColors.ColorControlText;
            LblEntryLots.BackColor = Color.Transparent;
            LblEntryLots.AutoSize = true;
            LblEntryLots.Text = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            NudEntryLots.Parent = PnlAmounts;
            NudEntryLots.Name = "nudEntryLots";
            NudEntryLots.BeginInit();
            NudEntryLots.Minimum = 0.01M;
            NudEntryLots.Maximum = 100;
            NudEntryLots.Increment = 0.01M;
            NudEntryLots.Value = (decimal) EntryLots;
            NudEntryLots.DecimalPlaces = 2;
            NudEntryLots.TextAlign = HorizontalAlignment.Center;
            NudEntryLots.EndInit();

            // Label Entry Lots %
            LblPercent1.Parent = PnlAmounts;
            LblPercent1.ForeColor = LayoutColors.ColorControlText;
            LblPercent1.BackColor = Color.Transparent;
            LblPercent1.AutoSize = true;
            LblPercent1.Text = "%";

            // Label Adding Lots
            LblAddingLots.Parent = PnlAmounts;
            LblAddingLots.ForeColor = LayoutColors.ColorControlText;
            LblAddingLots.BackColor = Color.Transparent;
            LblAddingLots.AutoSize = true;
            LblAddingLots.Text = Language.T("In case of addition - number of lots to add");

            // NumericUpDown Adding Lots
            NudAddingLots.Parent = PnlAmounts;
            NudAddingLots.Name = "nudAddingLots";
            NudAddingLots.BeginInit();
            NudAddingLots.Minimum = 0.01M;
            NudAddingLots.Maximum = 100;
            NudAddingLots.Increment = 0.01M;
            NudAddingLots.Value = (decimal) AddingLots;
            NudAddingLots.DecimalPlaces = 2;
            NudAddingLots.TextAlign = HorizontalAlignment.Center;
            NudAddingLots.EndInit();

            // Label Adding Lots %
            LblPercent2.Parent = PnlAmounts;
            LblPercent2.ForeColor = LayoutColors.ColorControlText;
            LblPercent2.BackColor = Color.Transparent;
            LblPercent2.AutoSize = true;
            LblPercent2.Text = "%";

            // Label Reducing Lots
            LblReducingLots.Parent = PnlAmounts;
            LblReducingLots.ForeColor = LayoutColors.ColorControlText;
            LblReducingLots.BackColor = Color.Transparent;
            LblReducingLots.AutoSize = true;
            LblReducingLots.Text = Language.T("In case of reduction - number of lots to close");

            // NumericUpDown Reducing Lots
            NudReducingLots.Parent = PnlAmounts;
            NudReducingLots.Name = "nudReducingLots";
            NudReducingLots.BeginInit();
            NudReducingLots.Minimum = 0.01M;
            NudReducingLots.Maximum = 100;
            NudReducingLots.Increment = 0.01M;
            NudReducingLots.Value = (decimal) ReducingLots;
            NudReducingLots.DecimalPlaces = 2;
            NudReducingLots.TextAlign = HorizontalAlignment.Center;
            NudReducingLots.EndInit();

            // Label Reducing Lots %
            LblPercent3.Parent = PnlAmounts;
            LblPercent3.ForeColor = LayoutColors.ColorControlText;
            LblPercent3.BackColor = Color.Transparent;
            LblPercent3.AutoSize = true;
            LblPercent3.Text = "%";

            // CheckBox Use Martingale
            CbxUseMartingale.Name = "cbxUseMartingale";
            CbxUseMartingale.Parent = PnlAmounts;
            CbxUseMartingale.ForeColor = LayoutColors.ColorControlText;
            CbxUseMartingale.BackColor = Color.Transparent;
            CbxUseMartingale.AutoCheck = true;
            CbxUseMartingale.AutoSize = true;
            CbxUseMartingale.Checked = false;
            CbxUseMartingale.Text = Language.T("Martingale money management multiplier");

            // NumericUpDown Martingale Multiplier
            NudMartingaleMultiplier.Parent = PnlAmounts;
            NudMartingaleMultiplier.Name = "nudMartingaleMultiplier";
            NudMartingaleMultiplier.BeginInit();
            NudMartingaleMultiplier.Minimum = 0.01M;
            NudMartingaleMultiplier.Maximum = 10;
            NudMartingaleMultiplier.Increment = 0.01m;
            NudMartingaleMultiplier.DecimalPlaces = 2;
            NudMartingaleMultiplier.Value = 2;
            NudMartingaleMultiplier.TextAlign = HorizontalAlignment.Center;
            NudMartingaleMultiplier.EndInit();

            // CheckBox Permanent Stop Loss
            ChbPermaSL.Parent = PnlProtection;
            ChbPermaSL.ForeColor = LayoutColors.ColorControlText;
            ChbPermaSL.BackColor = Color.Transparent;
            ChbPermaSL.AutoCheck = true;
            ChbPermaSL.AutoSize = true;
            ChbPermaSL.Name = "chbPermaSL";
            ChbPermaSL.Text = Language.T("Permanent Stop Loss");

            // ComboBox cbxPermaSLType
            CbxPermaSLType.Parent = PnlProtection;
            CbxPermaSLType.Name = "cbxPermaSLType";
            CbxPermaSLType.Visible = false;
            CbxPermaSLType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaSLType.Items.AddRange(new object[] {Language.T("Relative"), Language.T("Absolute")});
            CbxPermaSLType.SelectedIndex = 0;

            // NumericUpDown Permanent S/L
            NudPermaSL.Parent = PnlProtection;
            NudPermaSL.Name = "nudPermaSL";
            NudPermaSL.BeginInit();
            NudPermaSL.Minimum = 5;
            NudPermaSL.Maximum = 5000;
            NudPermaSL.Increment = 1;
            NudPermaSL.Value = PermanentSL;
            NudPermaSL.TextAlign = HorizontalAlignment.Center;
            NudPermaSL.EndInit();

            // CheckBox Permanent Take Profit
            ChbPermaTP.Parent = PnlProtection;
            ChbPermaTP.ForeColor = LayoutColors.ColorControlText;
            ChbPermaTP.BackColor = Color.Transparent;
            ChbPermaTP.AutoCheck = true;
            ChbPermaTP.AutoSize = true;
            ChbPermaTP.Name = "chbPermaTP";
            ChbPermaTP.Text = Language.T("Permanent Take Profit");

            // ComboBox cbxPermaTPType
            CbxPermaTPType.Parent = PnlProtection;
            CbxPermaTPType.Name = "cbxPermaTPType";
            CbxPermaTPType.Visible = false;
            CbxPermaTPType.DropDownStyle = ComboBoxStyle.DropDownList;
            CbxPermaTPType.Items.AddRange(new object[] {Language.T("Relative"), Language.T("Absolute")});
            CbxPermaTPType.SelectedIndex = 0;

            // NumericUpDown Permanent Take Profit
            NudPermaTP.Parent = PnlProtection;
            NudPermaTP.Name = "nudPermaTP";
            NudPermaTP.BeginInit();
            NudPermaTP.Minimum = 5;
            NudPermaTP.Maximum = 5000;
            NudPermaTP.Increment = 1;
            NudPermaTP.Value = PermanentTP;
            NudPermaTP.TextAlign = HorizontalAlignment.Center;
            NudPermaTP.EndInit();

            // CheckBox Break Even
            ChbBreakEven.Parent = PnlProtection;
            ChbBreakEven.ForeColor = LayoutColors.ColorControlText;
            ChbBreakEven.BackColor = Color.Transparent;
            ChbBreakEven.AutoCheck = true;
            ChbBreakEven.AutoSize = true;
            ChbBreakEven.Name = "chbBreakEven";
            ChbBreakEven.Text = Language.T("Break Even") + " [" + Language.T("pips") + "]";

            // NumericUpDown Break Even
            NudBreakEven.Parent = PnlProtection;
            NudBreakEven.Name = "nudBreakEven";
            NudBreakEven.BeginInit();
            NudBreakEven.Minimum = 5;
            NudBreakEven.Maximum = 5000;
            NudBreakEven.Increment = 1;
            NudBreakEven.Value = BreakEven;
            NudBreakEven.TextAlign = HorizontalAlignment.Center;
            NudBreakEven.EndInit();

            //Button Default
            BtnDefault.Parent = this;
            BtnDefault.Name = "Default";
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlAveraging { get; set; }
        private FancyPanel PnlAmounts { get; set; }
        private FancyPanel PnlProtection { get; set; }

        private Label LblAveragingSameDir { get; set; }
        private Label LblAveragingOppDir { get; set; }
        private Label LblMaxOpenLots { get; set; }
        private Label LblEntryLots { get; set; }
        private Label LblAddingLots { get; set; }
        private Label LblReducingLots { get; set; }
        private Label LblPercent1 { get; set; }
        private Label LblPercent2 { get; set; }
        private Label LblPercent3 { get; set; }

        private ComboBox CbxSameDirAction { get; set; }
        private ComboBox CbxOppDirAction { get; set; }
        private CheckBox ChbPermaSL { get; set; }
        private CheckBox ChbPermaTP { get; set; }
        private CheckBox ChbBreakEven { get; set; }
        private CheckBox CbxUseMartingale { get; set; }
        private RadioButton RbConstantUnits { get; set; }
        private RadioButton RbVariableUnits { get; set; }
        private NumericUpDown NudMaxOpenLots { get; set; }
        private NumericUpDown NudEntryLots { get; set; }
        private NumericUpDown NudAddingLots { get; set; }
        private NumericUpDown NudReducingLots { get; set; }
        private NumericUpDown NudPermaSL { get; set; }
        private NumericUpDown NudPermaTP { get; set; }
        private NumericUpDown NudBreakEven { get; set; }
        private NumericUpDown NudMartingaleMultiplier { get; set; }
        private ComboBox CbxPermaSLType { get; set; }
        private ComboBox CbxPermaTPType { get; set; }

        private Button BtnDefault { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }

        public SameDirSignalAction SameDirAverg { get; set; }
        public OppositeDirSignalAction OppDirAverg { get; set; }
        public bool UsePermanentSL { get; set; }
        public PermanentProtectionType PermanentSLType { get; set; }
        public int PermanentSL { get; set; }
        public bool UseBreakEven { get; set; }
        public int BreakEven { get; set; }
        public bool UsePermanentTP { get; set; }
        public PermanentProtectionType PermanentTPType { get; set; }
        public int PermanentTP { get; set; }
        public bool UseAccountPercentEntry { get; set; }
        public double MaxOpenLots { get; set; }
        public double EntryLots { get; set; }
        public double AddingLots { get; set; }
        public double ReducingLots { get; set; }
        public bool UseMartingale { get; set; }
        public double MartingaleMultiplier { get; set; }

        /// <summary>
        ///     Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CbxSameDirAction.SelectedIndexChanged += ParamChanged;
            CbxOppDirAction.SelectedIndexChanged += ParamChanged;
            RbConstantUnits.CheckedChanged += ParamChanged;
            RbVariableUnits.CheckedChanged += ParamChanged;
            NudMaxOpenLots.ValueChanged += ParamChanged;
            NudEntryLots.ValueChanged += ParamChanged;
            NudAddingLots.ValueChanged += ParamChanged;
            NudReducingLots.ValueChanged += ParamChanged;
            ChbPermaSL.CheckedChanged += ParamChanged;
            CbxPermaSLType.SelectedIndexChanged += ParamChanged;
            NudPermaSL.ValueChanged += ParamChanged;
            ChbPermaTP.CheckedChanged += ParamChanged;
            CbxPermaTPType.SelectedIndexChanged += ParamChanged;
            NudPermaTP.ValueChanged += ParamChanged;
            ChbBreakEven.CheckedChanged += ParamChanged;
            NudBreakEven.ValueChanged += ParamChanged;
            CbxUseMartingale.CheckedChanged += ParamChanged;
            NudMartingaleMultiplier.ValueChanged += ParamChanged;

            var buttonWidth = (int) (Data.HorizontalDlu*60);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);

            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 504);

            BtnAccept.Focus();
        }

        /// <summary>
        ///     Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDlu*15.5);
            var btnVertSpace = (int) (Data.VerticalDlu*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDlu*3);
            int space = btnHrzSpace;
            const int border = 2;

            // pnlAveraging
            PnlAveraging.Size = new Size(ClientSize.Width - 2*space, 84);
            PnlAveraging.Location = new Point(space, space);

            // pnlAmounts
            PnlAmounts.Size = new Size(ClientSize.Width - 2*space, 252);
            PnlAmounts.Location = new Point(space, PnlAveraging.Bottom + space);

            // pnlProtection
            PnlProtection.Size = new Size(ClientSize.Width - 2*space, 112);
            PnlProtection.Location = new Point(space, PnlAmounts.Bottom + space);

            // Averaging
            const int cbxWith = 80;
            int cbxLeft = PnlAveraging.ClientSize.Width - cbxWith - space - border;

            CbxSameDirAction.Width = cbxWith;
            LblAveragingSameDir.Location = new Point(btnHrzSpace, space + 25);
            CbxSameDirAction.Location = new Point(cbxLeft, space + 21);

            if (LblAveragingSameDir.Right + space > CbxSameDirAction.Left)
            {
                Width += LblAveragingSameDir.Right + space - CbxSameDirAction.Left;
                return;
            }

            CbxOppDirAction.Width = cbxWith;
            LblAveragingOppDir.Location = new Point(btnHrzSpace, buttonHeight + 2*space + 23);
            CbxOppDirAction.Location = new Point(cbxLeft, buttonHeight + 2*space + 19);

            if (LblAveragingOppDir.Right + space > CbxOppDirAction.Left)
            {
                Width += LblAveragingOppDir.Right + space - CbxOppDirAction.Left;
                return;
            }

            // Amounts
            const int nudWidth = 60;
            int nudLeft = PnlAmounts.ClientSize.Width - nudWidth - btnHrzSpace - border;

            LblMaxOpenLots.Location = new Point(btnHrzSpace, 0*buttonHeight + space + 25);
            NudMaxOpenLots.Size = new Size(nudWidth, buttonHeight);
            NudMaxOpenLots.Location = new Point(nudLeft, 0*buttonHeight + space + 22);

            if (LblMaxOpenLots.Right + space > NudMaxOpenLots.Left)
            {
                Width += LblMaxOpenLots.Right + space - NudMaxOpenLots.Left;
                return;
            }

            RbConstantUnits.Location = new Point(btnHrzSpace + 3, 55);
            RbVariableUnits.Location = new Point(btnHrzSpace + 3, 79);

            // Measuring rbVariableUnits size
            RbVariableUnits.Size = new Size(PnlAmounts.ClientSize.Width - 2*btnHrzSpace, 2*buttonHeight);
            int deltaWidth = 0;
            Graphics g = CreateGraphics();
            while (g.MeasureString(RbVariableUnits.Text, RbVariableUnits.Font, RbVariableUnits.Width - 10).Height >
                   3*RbVariableUnits.Font.Height)
            {
                deltaWidth++;
                RbVariableUnits.Width++;
            }
            g.Dispose();
            if (deltaWidth > 0)
            {
                Width += deltaWidth;
                return;
            }

            LblEntryLots.Location = new Point(btnHrzSpace, 139);
            NudEntryLots.Size = new Size(nudWidth, buttonHeight);
            NudEntryLots.Location = new Point(nudLeft, 137);
            LblPercent1.Location = new Point(NudEntryLots.Left - 15, LblEntryLots.Top);

            if (LblEntryLots.Right + space > LblPercent1.Left)
            {
                Width += LblEntryLots.Right + space - LblPercent1.Left;
                return;
            }

            LblAddingLots.Location = new Point(btnHrzSpace, 167);
            NudAddingLots.Size = new Size(nudWidth, buttonHeight);
            NudAddingLots.Location = new Point(nudLeft, 165);
            LblPercent2.Location = new Point(NudAddingLots.Left - 15, LblAddingLots.Top);

            if (LblAddingLots.Right + space > LblPercent2.Left)
            {
                Width += LblAddingLots.Right + space - LblPercent2.Left;
                return;
            }

            LblReducingLots.Location = new Point(btnHrzSpace, 195);
            NudReducingLots.Size = new Size(nudWidth, buttonHeight);
            NudReducingLots.Location = new Point(nudLeft, 193);
            LblPercent3.Location = new Point(NudReducingLots.Left - 15, LblReducingLots.Top);

            if (LblReducingLots.Right + space > LblPercent3.Left)
            {
                Width += LblReducingLots.Right + space - LblPercent3.Left;
                return;
            }

            CbxUseMartingale.Location = new Point(btnHrzSpace + 2, 223);
            NudMartingaleMultiplier.Size = new Size(nudWidth, buttonHeight);
            NudMartingaleMultiplier.Location = new Point(nudLeft, 221);

            const int rightComboBxWith = 95;
            int comboBxLeft = nudLeft - space - rightComboBxWith;


            // Permanent Stop Loss
            NudPermaSL.Size = new Size(nudWidth, buttonHeight);
            NudPermaSL.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 23);
            CbxPermaSLType.Width = rightComboBxWith;
            CbxPermaSLType.Location = new Point(comboBxLeft, 0*buttonHeight + 1*space + 23);
            ChbPermaSL.Location = new Point(btnHrzSpace + 3, 0*buttonHeight + 1*space + 24);

            // Permanent Take Profit
            NudPermaTP.Size = new Size(nudWidth, buttonHeight);
            NudPermaTP.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 21);
            CbxPermaTPType.Width = rightComboBxWith;
            CbxPermaTPType.Location = new Point(comboBxLeft, 1*buttonHeight + 2*space + 21);
            ChbPermaTP.Location = new Point(btnHrzSpace + 3, 1*buttonHeight + 2*space + 22);

            // Break Even
            NudBreakEven.Size = new Size(nudWidth, buttonHeight);
            NudBreakEven.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 19);
            ChbBreakEven.Location = new Point(btnHrzSpace + 3, 2*buttonHeight + 3*space + 20);

            int buttonWidth = (PnlAveraging.Width - 2*btnHrzSpace)/3;

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Default
            BtnDefault.Size = new Size(buttonWidth, buttonHeight);
            BtnDefault.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                            ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnDefault.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);
        }

        /// <summary>
        ///     Sets the controls' text
        /// </summary>
        public void SetParams()
        {
            // ComboBox sameDirAverg
            switch (SameDirAverg)
            {
                case SameDirSignalAction.Nothing:
                    CbxSameDirAction.SelectedIndex = 0;
                    break;
                case SameDirSignalAction.Winner:
                    CbxSameDirAction.SelectedIndex = 1;
                    break;
                case SameDirSignalAction.Add:
                    CbxSameDirAction.SelectedIndex = 2;
                    break;
            }

            // ComboBox oppDirAverg
            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
                CbxOppDirAction.Enabled = false;

            // ComboBox oppDirAverg
            switch (OppDirAverg)
            {
                case OppositeDirSignalAction.Nothing:
                    CbxOppDirAction.SelectedIndex = 0;
                    break;
                case OppositeDirSignalAction.Reduce:
                    CbxOppDirAction.SelectedIndex = 1;
                    break;
                case OppositeDirSignalAction.Close:
                    CbxOppDirAction.SelectedIndex = 2;
                    break;
                case OppositeDirSignalAction.Reverse:
                    CbxOppDirAction.SelectedIndex = 3;
                    break;
            }

            ChbPermaSL.Checked = UsePermanentSL;
            ChbPermaTP.Checked = UsePermanentTP;
            ChbBreakEven.Checked = UseBreakEven;

            CbxPermaSLType.Enabled = UsePermanentSL;
            CbxPermaSLType.SelectedIndex = (int) PermanentSLType;

            CbxPermaTPType.Enabled = UsePermanentTP;
            CbxPermaTPType.SelectedIndex = (int) PermanentTPType;

            // NumericUpDown nudPermaSL
            NudPermaSL.Enabled = ChbPermaSL.Checked;
            NudPermaSL.Value = PermanentSL;

            // NumericUpDown nudPermaTP
            NudPermaTP.Enabled = ChbPermaTP.Checked;
            NudPermaTP.Value = PermanentTP;

            // NumericUpDown nudBreakEven
            NudBreakEven.Enabled = ChbBreakEven.Checked;
            NudBreakEven.Value = BreakEven;

            // Use account percent entry
            RbConstantUnits.Checked = !UseAccountPercentEntry;
            RbVariableUnits.Checked = UseAccountPercentEntry;

            NudMaxOpenLots.Value = (decimal) MaxOpenLots;

            NudEntryLots.Value = (decimal) EntryLots;
            NudAddingLots.Value = (decimal) AddingLots;
            NudReducingLots.Value = (decimal) ReducingLots;

            LblPercent1.Visible = UseAccountPercentEntry;
            LblPercent2.Visible = UseAccountPercentEntry;
            LblPercent3.Visible = UseAccountPercentEntry;

            CbxUseMartingale.Checked = UseMartingale;
            NudMartingaleMultiplier.Value = (decimal) MartingaleMultiplier;
            NudMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;
        }

        /// <summary>
        ///     Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            switch (((Control) sender).Name)
            {
                case "cbxSameDirAction":
                    SameDirAverg = (SameDirSignalAction) CbxSameDirAction.SelectedIndex;
                    break;
                case "chbPermaSL":
                    NudPermaSL.Enabled = ChbPermaSL.Checked;
                    CbxPermaSLType.Enabled = ChbPermaSL.Checked;
                    UsePermanentSL = ChbPermaSL.Checked;
                    break;
                case "cbxPermaSLType":
                    PermanentSLType = (PermanentProtectionType) CbxPermaSLType.SelectedIndex;
                    break;
                case "nudPermaSL":
                    PermanentSL = (int) NudPermaSL.Value;
                    break;
                case "chbPermaTP":
                    NudPermaTP.Enabled = ChbPermaTP.Checked;
                    CbxPermaTPType.Enabled = ChbPermaTP.Checked;
                    UsePermanentTP = ChbPermaTP.Checked;
                    break;
                case "cbxPermaTPType":
                    PermanentTPType = (PermanentProtectionType) CbxPermaTPType.SelectedIndex;
                    break;
                case "nudPermaTP":
                    PermanentTP = (int) NudPermaTP.Value;
                    break;
                case "chbBreakEven":
                    NudBreakEven.Enabled = ChbBreakEven.Checked;
                    UseBreakEven = ChbBreakEven.Checked;
                    break;
                case "nudBreakEven":
                    BreakEven = (int) NudBreakEven.Value;
                    break;
                case "cbxOppDirAction":
                    OppDirAverg = (OppositeDirSignalAction) CbxOppDirAction.SelectedIndex;
                    break;
                case "rbConstantUnits":
                    UseAccountPercentEntry = false;
                    LblPercent1.Visible = UseAccountPercentEntry;
                    LblPercent2.Visible = UseAccountPercentEntry;
                    LblPercent3.Visible = UseAccountPercentEntry;
                    break;
                case "rbVariableUnits":
                    UseAccountPercentEntry = true;
                    LblPercent1.Visible = UseAccountPercentEntry;
                    LblPercent2.Visible = UseAccountPercentEntry;
                    LblPercent3.Visible = UseAccountPercentEntry;
                    break;
                case "nudMaxOpenLots":
                    MaxOpenLots = (double) NudMaxOpenLots.Value;
                    break;
                case "nudEntryLots":
                    EntryLots = (double) NudEntryLots.Value;
                    break;
                case "nudAddingLots":
                    AddingLots = (double) NudAddingLots.Value;
                    break;
                case "nudReducingLots":
                    ReducingLots = (double) NudReducingLots.Value;
                    break;
                case "cbxUseMartingale":
                    UseMartingale = CbxUseMartingale.Checked;
                    NudMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;
                    break;
                case "nudMartingaleMultiplier":
                    MartingaleMultiplier = (double) NudMartingaleMultiplier.Value;
                    break;
            }


            if (!UseAccountPercentEntry && EntryLots > MaxOpenLots)
            {
                EntryLots = MaxOpenLots;
                NudEntryLots.Value = (decimal) EntryLots;
            }
        }

        /// <summary>
        ///     Button Default Click
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            SameDirAverg = SameDirSignalAction.Nothing;

            if (Data.Strategy.Slot[Data.Strategy.CloseSlot].IndicatorName == "Close and Reverse")
            {
                CbxOppDirAction.Enabled = false;
                OppDirAverg = OppositeDirSignalAction.Reverse;
            }
            else
            {
                OppDirAverg = OppositeDirSignalAction.Nothing;
            }

            int defaultSltp = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;

            UsePermanentSL = false;
            PermanentSLType = PermanentProtectionType.Relative;
            PermanentSL = defaultSltp;
            UsePermanentTP = false;
            PermanentTPType = PermanentProtectionType.Relative;
            PermanentTP = defaultSltp;
            UseBreakEven = false;
            BreakEven = defaultSltp;
            UseAccountPercentEntry = false;
            MaxOpenLots = 20;
            EntryLots = 1;
            AddingLots = 1;
            ReducingLots = 1;
            UseMartingale = false;
            MartingaleMultiplier = 2;

            SetParams();
        }

        /// <summary>
        ///     Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}