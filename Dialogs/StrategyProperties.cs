// Strategy_Properties Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Strategy Properties
    /// </summary>
    public sealed class StrategyProperties : Form
    {
        private readonly ToolTip _toolTip = new ToolTip();

        /// <summary>
        /// Constructor
        /// </summary>
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
            NUDMaxOpenLots = new NumericUpDown();
            RbConstantUnits = new RadioButton();
            RbVariableUnits = new RadioButton();
            NUDEntryLots = new NumericUpDown();
            NUDAddingLots = new NumericUpDown();
            NUDReducingLots = new NumericUpDown();
            LblMaxOpenLots = new Label();
            LblEntryLots = new Label();
            LblAddingLots = new Label();
            LblReducingLots = new Label();

            ChbPermaSL = new CheckBox();
            CbxPermaSLType = new ComboBox();
            NUDPermaSL = new NumericUpDown();
            ChbPermaTP = new CheckBox();
            CbxPermaTPType = new ComboBox();
            NUDPermaTP = new NumericUpDown();
            ChbBreakEven = new CheckBox();
            NUDBreakEven = new NumericUpDown();
            CbxUseMartingale = new CheckBox();
            NUDMartingaleMultiplier = new NumericUpDown();


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
            _toolTip.SetToolTip(CbxSameDirAction,
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
            _toolTip.SetToolTip(CbxOppDirAction,
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
            NUDMaxOpenLots.Parent = PnlAmounts;
            NUDMaxOpenLots.Name = "nudMaxOpenLots";
            NUDMaxOpenLots.BeginInit();
            NUDMaxOpenLots.Minimum = 0.01M;
            NUDMaxOpenLots.Maximum = 100;
            NUDMaxOpenLots.Increment = 0.01M;
            NUDMaxOpenLots.Value = (decimal) MaxOpenLots;
            NUDMaxOpenLots.DecimalPlaces = 2;
            NUDMaxOpenLots.TextAlign = HorizontalAlignment.Center;
            NUDMaxOpenLots.EndInit();

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
            RbVariableUnits.Text = Language.T("Trade a variable number of lots depending on your current account equity. The percentage values show the part of the account equity used to cover the required margin.");

            // Label Entry Lots
            LblEntryLots.Parent = PnlAmounts;
            LblEntryLots.ForeColor = LayoutColors.ColorControlText;
            LblEntryLots.BackColor = Color.Transparent;
            LblEntryLots.AutoSize = true;
            LblEntryLots.Text = Language.T("Number of entry lots for a new position");

            // NumericUpDown Entry Lots
            NUDEntryLots.Parent = PnlAmounts;
            NUDEntryLots.Name = "nudEntryLots";
            NUDEntryLots.BeginInit();
            NUDEntryLots.Minimum = 0.01M;
            NUDEntryLots.Maximum = 100;
            NUDEntryLots.Increment = 0.01M;
            NUDEntryLots.Value = (decimal) EntryLots;
            NUDEntryLots.DecimalPlaces = 2;
            NUDEntryLots.TextAlign = HorizontalAlignment.Center;
            NUDEntryLots.EndInit();

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
            NUDAddingLots.Parent = PnlAmounts;
            NUDAddingLots.Name = "nudAddingLots";
            NUDAddingLots.BeginInit();
            NUDAddingLots.Minimum = 0.01M;
            NUDAddingLots.Maximum = 100;
            NUDAddingLots.Increment = 0.01M;
            NUDAddingLots.Value = (decimal) AddingLots;
            NUDAddingLots.DecimalPlaces = 2;
            NUDAddingLots.TextAlign = HorizontalAlignment.Center;
            NUDAddingLots.EndInit();

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
            NUDReducingLots.Parent = PnlAmounts;
            NUDReducingLots.Name = "nudReducingLots";
            NUDReducingLots.BeginInit();
            NUDReducingLots.Minimum = 0.01M;
            NUDReducingLots.Maximum = 100;
            NUDReducingLots.Increment = 0.01M;
            NUDReducingLots.Value = (decimal) ReducingLots;
            NUDReducingLots.DecimalPlaces = 2;
            NUDReducingLots.TextAlign = HorizontalAlignment.Center;
            NUDReducingLots.EndInit();

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
            NUDMartingaleMultiplier.Parent = PnlAmounts;
            NUDMartingaleMultiplier.Name = "nudMartingaleMultiplier";
            NUDMartingaleMultiplier.BeginInit();
            NUDMartingaleMultiplier.Minimum = 0.01M;
            NUDMartingaleMultiplier.Maximum = 10;
            NUDMartingaleMultiplier.Increment = 0.01m;
            NUDMartingaleMultiplier.DecimalPlaces = 2;
            NUDMartingaleMultiplier.Value = 2;
            NUDMartingaleMultiplier.TextAlign = HorizontalAlignment.Center;
            NUDMartingaleMultiplier.EndInit();

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
            NUDPermaSL.Parent = PnlProtection;
            NUDPermaSL.Name = "nudPermaSL";
            NUDPermaSL.BeginInit();
            NUDPermaSL.Minimum = 5;
            NUDPermaSL.Maximum = 5000;
            NUDPermaSL.Increment = 1;
            NUDPermaSL.Value = PermanentSL;
            NUDPermaSL.TextAlign = HorizontalAlignment.Center;
            NUDPermaSL.EndInit();

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
            NUDPermaTP.Parent = PnlProtection;
            NUDPermaTP.Name = "nudPermaTP";
            NUDPermaTP.BeginInit();
            NUDPermaTP.Minimum = 5;
            NUDPermaTP.Maximum = 5000;
            NUDPermaTP.Increment = 1;
            NUDPermaTP.Value = PermanentTP;
            NUDPermaTP.TextAlign = HorizontalAlignment.Center;
            NUDPermaTP.EndInit();

            // CheckBox Break Even
            ChbBreakEven.Parent = PnlProtection;
            ChbBreakEven.ForeColor = LayoutColors.ColorControlText;
            ChbBreakEven.BackColor = Color.Transparent;
            ChbBreakEven.AutoCheck = true;
            ChbBreakEven.AutoSize = true;
            ChbBreakEven.Name = "chbBreakEven";
            ChbBreakEven.Text = Language.T("Break Even") + " [" + Language.T("pips") + "]";

            // NumericUpDown Break Even
            NUDBreakEven.Parent = PnlProtection;
            NUDBreakEven.Name = "nudBreakEven";
            NUDBreakEven.BeginInit();
            NUDBreakEven.Minimum = 5;
            NUDBreakEven.Maximum = 5000;
            NUDBreakEven.Increment = 1;
            NUDBreakEven.Value = BreakEven;
            NUDBreakEven.TextAlign = HorizontalAlignment.Center;
            NUDBreakEven.EndInit();

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
        private NumericUpDown NUDMaxOpenLots { get; set; }
        private NumericUpDown NUDEntryLots { get; set; }
        private NumericUpDown NUDAddingLots { get; set; }
        private NumericUpDown NUDReducingLots { get; set; }
        private NumericUpDown NUDPermaSL { get; set; }
        private NumericUpDown NUDPermaTP { get; set; }
        private NumericUpDown NUDBreakEven { get; set; }
        private NumericUpDown NUDMartingaleMultiplier { get; set; }
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
        /// Perform initializing
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            CbxSameDirAction.SelectedIndexChanged += ParamChanged;
            CbxOppDirAction.SelectedIndexChanged += ParamChanged;
            RbConstantUnits.CheckedChanged += ParamChanged;
            RbVariableUnits.CheckedChanged += ParamChanged;
            NUDMaxOpenLots.ValueChanged += ParamChanged;
            NUDEntryLots.ValueChanged += ParamChanged;
            NUDAddingLots.ValueChanged += ParamChanged;
            NUDReducingLots.ValueChanged += ParamChanged;
            ChbPermaSL.CheckedChanged += ParamChanged;
            CbxPermaSLType.SelectedIndexChanged += ParamChanged;
            NUDPermaSL.ValueChanged += ParamChanged;
            ChbPermaTP.CheckedChanged += ParamChanged;
            CbxPermaTPType.SelectedIndexChanged += ParamChanged;
            NUDPermaTP.ValueChanged += ParamChanged;
            ChbBreakEven.CheckedChanged += ParamChanged;
            NUDBreakEven.ValueChanged += ParamChanged;
            CbxUseMartingale.CheckedChanged += ParamChanged;
            NUDMartingaleMultiplier.ValueChanged += ParamChanged;

            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);

            ClientSize = new Size(3*buttonWidth + 4*btnHrzSpace, 504);

            BtnAccept.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
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
            NUDMaxOpenLots.Size = new Size(nudWidth, buttonHeight);
            NUDMaxOpenLots.Location = new Point(nudLeft, 0*buttonHeight + space + 22);

            if (LblMaxOpenLots.Right + space > NUDMaxOpenLots.Left)
            {
                Width += LblMaxOpenLots.Right + space - NUDMaxOpenLots.Left;
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
            NUDEntryLots.Size = new Size(nudWidth, buttonHeight);
            NUDEntryLots.Location = new Point(nudLeft, 137);
            LblPercent1.Location = new Point(NUDEntryLots.Left - 15, LblEntryLots.Top);

            if (LblEntryLots.Right + space > LblPercent1.Left)
            {
                Width += LblEntryLots.Right + space - LblPercent1.Left;
                return;
            }

            LblAddingLots.Location = new Point(btnHrzSpace, 167);
            NUDAddingLots.Size = new Size(nudWidth, buttonHeight);
            NUDAddingLots.Location = new Point(nudLeft, 165);
            LblPercent2.Location = new Point(NUDAddingLots.Left - 15, LblAddingLots.Top);

            if (LblAddingLots.Right + space > LblPercent2.Left)
            {
                Width += LblAddingLots.Right + space - LblPercent2.Left;
                return;
            }

            LblReducingLots.Location = new Point(btnHrzSpace, 195);
            NUDReducingLots.Size = new Size(nudWidth, buttonHeight);
            NUDReducingLots.Location = new Point(nudLeft, 193);
            LblPercent3.Location = new Point(NUDReducingLots.Left - 15, LblReducingLots.Top);

            if (LblReducingLots.Right + space > LblPercent3.Left)
            {
                Width += LblReducingLots.Right + space - LblPercent3.Left;
                return;
            }

            CbxUseMartingale.Location = new Point(btnHrzSpace + 2, 223);
            NUDMartingaleMultiplier.Size = new Size(nudWidth, buttonHeight);
            NUDMartingaleMultiplier.Location = new Point(nudLeft, 221);

            const int rightComboBxWith = 95;
            int comboBxLeft = nudLeft - space - rightComboBxWith;


            // Permanent Stop Loss
            NUDPermaSL.Size = new Size(nudWidth, buttonHeight);
            NUDPermaSL.Location = new Point(nudLeft, 0*buttonHeight + 1*space + 23);
            CbxPermaSLType.Width = rightComboBxWith;
            CbxPermaSLType.Location = new Point(comboBxLeft, 0*buttonHeight + 1*space + 23);
            ChbPermaSL.Location = new Point(btnHrzSpace + 3, 0*buttonHeight + 1*space + 24);

            // Permanent Take Profit
            NUDPermaTP.Size = new Size(nudWidth, buttonHeight);
            NUDPermaTP.Location = new Point(nudLeft, 1*buttonHeight + 2*space + 21);
            CbxPermaTPType.Width = rightComboBxWith;
            CbxPermaTPType.Location = new Point(comboBxLeft, 1*buttonHeight + 2*space + 21);
            ChbPermaTP.Location = new Point(btnHrzSpace + 3, 1*buttonHeight + 2*space + 22);

            // Break Even
            NUDBreakEven.Size = new Size(nudWidth, buttonHeight);
            NUDBreakEven.Location = new Point(nudLeft, 2*buttonHeight + 3*space + 19);
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
        /// Sets the controls' text
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
            NUDPermaSL.Enabled = ChbPermaSL.Checked;
            NUDPermaSL.Value = PermanentSL;

            // NumericUpDown nudPermaTP
            NUDPermaTP.Enabled = ChbPermaTP.Checked;
            NUDPermaTP.Value = PermanentTP;

            // NumericUpDown nudBreakEven
            NUDBreakEven.Enabled = ChbBreakEven.Checked;
            NUDBreakEven.Value = BreakEven;

            // Use account percent entry
            RbConstantUnits.Checked = !UseAccountPercentEntry;
            RbVariableUnits.Checked = UseAccountPercentEntry;

            NUDMaxOpenLots.Value = (decimal) MaxOpenLots;

            NUDEntryLots.Value = (decimal) EntryLots;
            NUDAddingLots.Value = (decimal) AddingLots;
            NUDReducingLots.Value = (decimal) ReducingLots;

            LblPercent1.Visible = UseAccountPercentEntry;
            LblPercent2.Visible = UseAccountPercentEntry;
            LblPercent3.Visible = UseAccountPercentEntry;

            CbxUseMartingale.Checked = UseMartingale;
            NUDMartingaleMultiplier.Value = (decimal) MartingaleMultiplier;
            NUDMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;
        }

        /// <summary>
        /// Sets the params values
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            switch (((Control) sender).Name)
            {
                case "cbxSameDirAction":
                    SameDirAverg = (SameDirSignalAction) CbxSameDirAction.SelectedIndex;
                    break;
                case "chbPermaSL":
                    NUDPermaSL.Enabled = ChbPermaSL.Checked;
                    CbxPermaSLType.Enabled = ChbPermaSL.Checked;
                    UsePermanentSL = ChbPermaSL.Checked;
                    break;
                case "cbxPermaSLType":
                    PermanentSLType = (PermanentProtectionType) CbxPermaSLType.SelectedIndex;
                    break;
                case "nudPermaSL":
                    PermanentSL = (int) NUDPermaSL.Value;
                    break;
                case "chbPermaTP":
                    NUDPermaTP.Enabled = ChbPermaTP.Checked;
                    CbxPermaTPType.Enabled = ChbPermaTP.Checked;
                    UsePermanentTP = ChbPermaTP.Checked;
                    break;
                case "cbxPermaTPType":
                    PermanentTPType = (PermanentProtectionType) CbxPermaTPType.SelectedIndex;
                    break;
                case "nudPermaTP":
                    PermanentTP = (int) NUDPermaTP.Value;
                    break;
                case "chbBreakEven":
                    NUDBreakEven.Enabled = ChbBreakEven.Checked;
                    UseBreakEven = ChbBreakEven.Checked;
                    break;
                case "nudBreakEven":
                    BreakEven = (int) NUDBreakEven.Value;
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
                    MaxOpenLots = (double) NUDMaxOpenLots.Value;
                    break;
                case "nudEntryLots":
                    EntryLots = (double) NUDEntryLots.Value;
                    break;
                case "nudAddingLots":
                    AddingLots = (double) NUDAddingLots.Value;
                    break;
                case "nudReducingLots":
                    ReducingLots = (double) NUDReducingLots.Value;
                    break;
                case "cbxUseMartingale":
                    UseMartingale = CbxUseMartingale.Checked;
                    NUDMartingaleMultiplier.Enabled = CbxUseMartingale.Checked;
                    break;
                case "nudMartingaleMultiplier":
                    MartingaleMultiplier = (double) NUDMartingaleMultiplier.Value;
                    break;
            }


            if (!UseAccountPercentEntry && EntryLots > MaxOpenLots)
            {
                EntryLots = MaxOpenLots;
                NUDEntryLots.Value = (decimal) EntryLots;
            }
        }

        /// <summary>
        /// Button Default Click
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

            int defaultSLTP = (Data.InstrProperties.Digits == 5 || Data.InstrProperties.Digits == 3) ? 1000 : 100;

            UsePermanentSL = false;
            PermanentSLType = PermanentProtectionType.Relative;
            PermanentSL = defaultSLTP;
            UsePermanentTP = false;
            PermanentTPType = PermanentProtectionType.Relative;
            PermanentTP = defaultSLTP;
            UseBreakEven = false;
            BreakEven = defaultSLTP;
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
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}