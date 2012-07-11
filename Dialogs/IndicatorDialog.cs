// Indicator_Dialog Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    //    ###################################################################################
    //    # +-----------+ |--------------------------- Slot Type -------------------(!)(i)| #
    //    # |           |                                                                   #
    //    # |           | |------------------------- lblIndicator ------------------------| #
    //    # |           |                                                                   #
    //    # |           | |----------------- aLblList[0] ----------------------| | Group  | #
    //    # |           | |----------------- aCbxList[0] ----------------------| |cbxGroup| #
    //    # |           |                                                                   #
    //    # |           | |--------- aLblList[1] --------| |--------- aLblList[2] --------| #
    //    # |           | |--------- aCbxList[1] --------| |--------- aCbxList[2] --------| #
    //    # |           |                                                                   #
    //    # |           | |--------- aLblList[3] --------| |--------- aLblList[4] --------| #
    //    # |           | |--------- aCbxList[3] --------| |--------- aCbxList[4] --------| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[0]||aNudNumeric[0]| |aLblNumeric[1]||aNudNumeric[1]| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[2]||aNudNumeric[2]| |aLblNumeric[3]||aNudNumeric[3]| #
    //    # |           |                                                                   #
    //    # |           | |aLblNumeric[4]||aNudNumeric[4]| |aLblNumeric[5]||aNudNumeric[5]| #
    //    # |           |                                                                   #
    //    # |           | |v|------- aChbCheck[0] -------| |v|------- aChbCheck[1] -------| #
    //    # |           |                                                                   #
    //    # |           |                                                                   #
    //    # +-----------+ [   Accept   ]    [  Default  ]     [   Help   ]   [  Cancel  ]   #
    //    #                                                                                 #
    //    ###################################################################################

    /// <summary>
    /// Form dialog contains controls for adjusting the indicator's parameters.
    /// </summary>
    public sealed class IndicatorDialog : Form
    {
        private const int Border = 2;
        private readonly List<IndicatorSlot> _closingConditions = new List<IndicatorSlot>();
        private readonly int _slot;
        private readonly string _slotTitle;
        private readonly ToolTip _toolTip = new ToolTip();
        private bool _closingSlotsRemoved;
        private string _description;
        private string _indicatorName;

        private bool _isPaintAllowed;
        private bool _oppSignalSet;
        private string _warningMessage = "";

        /// <summary>
        /// Constructor
        /// </summary>
        public IndicatorDialog(int slotNumb, SlotTypes slotType, bool isSlotDefined)
        {
            _slot = slotNumb;
            SlotType = slotType;

            if (slotType == SlotTypes.Open)
            {
                _slotTitle = Language.T("Opening Point of the Position");
                PnlParameters = new FancyPanel(_slotTitle, LayoutColors.ColorSlotCaptionBackOpen,
                                               LayoutColors.ColorSlotCaptionText);
                PnlTreeViewBase = new FancyPanel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackOpen,
                                                 LayoutColors.ColorSlotCaptionText);
            }
            else if (slotType == SlotTypes.OpenFilter)
            {
                _slotTitle = Language.T("Opening Logic Condition");
                PnlParameters = new FancyPanel(_slotTitle, LayoutColors.ColorSlotCaptionBackOpenFilter,
                                               LayoutColors.ColorSlotCaptionText);
                PnlTreeViewBase = new FancyPanel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackOpenFilter,
                                                 LayoutColors.ColorSlotCaptionText);
            }
            else if (slotType == SlotTypes.Close)
            {
                _slotTitle = Language.T("Closing Point of the Position");
                PnlParameters = new FancyPanel(_slotTitle, LayoutColors.ColorSlotCaptionBackClose,
                                               LayoutColors.ColorSlotCaptionText);
                PnlTreeViewBase = new FancyPanel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackClose,
                                                 LayoutColors.ColorSlotCaptionText);
            }
            else
            {
                _slotTitle = Language.T("Closing Logic Condition");
                PnlParameters = new FancyPanel(_slotTitle, LayoutColors.ColorSlotCaptionBackCloseFilter,
                                               LayoutColors.ColorSlotCaptionText);
                PnlTreeViewBase = new FancyPanel(Language.T("Indicators"), LayoutColors.ColorSlotCaptionBackCloseFilter,
                                                 LayoutColors.ColorSlotCaptionText);
            }

            TrvIndicators = new TreeView();
            BtnAccept = new Button();
            BtnHelp = new Button();
            BtnDefault = new Button();
            BtnCancel = new Button();

            LblIndicatorInfo = new Label();
            LblIndicatorWarning = new Label();
            LblIndicator = new Label();
            LblGroup = new Label();
            CbxGroup = new ComboBox();
            ListLabel = new Label[5];
            ListParam = new ComboBox[5];
            NumLabel = new Label[6];
            NumParam = new NUD[6];
            CheckParam = new CheckBox[2];

            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = Data.Icon;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            AcceptButton = BtnAccept;
            CancelButton = BtnCancel;
            Text = Language.T("Logic and Parameters of the Indicators");

            // Panel TreeViewBase
            PnlTreeViewBase.Parent = this;
            PnlTreeViewBase.Padding = new Padding(Border, (int) PnlTreeViewBase.CaptionHeight, Border, Border);

            // TreeView trvIndicators
            TrvIndicators.Parent = PnlTreeViewBase;
            TrvIndicators.Dock = DockStyle.Fill;
            TrvIndicators.BackColor = LayoutColors.ColorControlBack;
            TrvIndicators.ForeColor = LayoutColors.ColorControlText;
            TrvIndicators.BorderStyle = BorderStyle.None;
            TrvIndicators.NodeMouseDoubleClick += TrvIndicatorsNodeMouseDoubleClick;
            TrvIndicators.KeyPress += TrvIndicatorsKeyPress;

            PnlParameters.Parent = this;

            // lblIndicatorInfo
            LblIndicatorInfo.Parent = PnlParameters;
            LblIndicatorInfo.Size = new Size(16, 16);
            LblIndicatorInfo.BackColor = Color.Transparent;
            LblIndicatorInfo.BackgroundImage = Resources.information;
            LblIndicatorInfo.Click += LblIndicatorInfoClick;
            LblIndicatorInfo.MouseEnter += Label_MouseEnter;
            LblIndicatorInfo.MouseLeave += Label_MouseLeave;

            // LAbel Indicator Warning
            LblIndicatorWarning.Parent = PnlParameters;
            LblIndicatorWarning.Size = new Size(16, 16);
            LblIndicatorWarning.BackColor = Color.Transparent;
            LblIndicatorWarning.BackgroundImage = Resources.warning;
            LblIndicatorWarning.Visible = false;
            LblIndicatorWarning.Click += LblIndicatorWarningClick;
            LblIndicatorWarning.MouseEnter += Label_MouseEnter;
            LblIndicatorWarning.MouseLeave += Label_MouseLeave;

            // Label Indicator
            LblIndicator.Parent = PnlParameters;
            LblIndicator.TextAlign = ContentAlignment.MiddleCenter;
            LblIndicator.Font = new Font(Font.FontFamily, 14, FontStyle.Bold);
            LblIndicator.ForeColor = LayoutColors.ColorSlotIndicatorText;
            LblIndicator.BackColor = Color.Transparent;

            // Label aLblList[0]
            ListLabel[0] = new Label
                               {
                                   Parent = PnlParameters,
                                   TextAlign = ContentAlignment.BottomCenter,
                                   ForeColor = LayoutColors.ColorControlText,
                                   BackColor = Color.Transparent
                               };

            // ComboBox aCbxList[0]
            ListParam[0] = new ComboBox {Parent = PnlParameters, DropDownStyle = ComboBoxStyle.DropDownList};
            ListParam[0].SelectedIndexChanged += ParamChanged;

            // Logical Group
            LblGroup = new Label
                           {
                               Parent = PnlParameters,
                               TextAlign = ContentAlignment.BottomCenter,
                               ForeColor = LayoutColors.ColorControlText,
                               BackColor = Color.Transparent,
                               Text = Language.T("Group")
                           };

            CbxGroup = new ComboBox {Parent = PnlParameters};
            if (slotType == SlotTypes.OpenFilter)
                CbxGroup.Items.AddRange(new object[] {"A", "B", "C", "D", "E", "F", "G", "H", "All"});
            if (slotType == SlotTypes.CloseFilter)
                CbxGroup.Items.AddRange(new object[] {"a", "b", "c", "d", "e", "f", "g", "h", "all"});
            CbxGroup.SelectedIndexChanged += GroupChanged;
            CbxGroup.DropDownStyle = ComboBoxStyle.DropDownList;
            _toolTip.SetToolTip(CbxGroup, Language.T("The logical group of the slot."));

            // ListParams
            for (int i = 1; i < 5; i++)
            {
                ListLabel[i] = new Label
                                   {
                                       Parent = PnlParameters,
                                       TextAlign = ContentAlignment.BottomCenter,
                                       ForeColor = LayoutColors.ColorControlText,
                                       BackColor = Color.Transparent
                                   };

                ListParam[i] = new ComboBox {Parent = PnlParameters, Enabled = false};
                ListParam[i].SelectedIndexChanged += ParamChanged;
                ListParam[i].DropDownStyle = ComboBoxStyle.DropDownList;
            }

            // NumParams
            for (int i = 0; i < 6; i++)
            {
                NumLabel[i] = new Label
                                  {
                                      Parent = PnlParameters,
                                      TextAlign = ContentAlignment.MiddleRight,
                                      ForeColor = LayoutColors.ColorControlText,
                                      BackColor = Color.Transparent
                                  };

                NumParam[i] = new NUD
                                  {Parent = PnlParameters, TextAlign = HorizontalAlignment.Center, Enabled = false};
                NumParam[i].ValueChanged += ParamChanged;
            }

            // CheckParams
            for (int i = 0; i < 2; i++)
            {
                CheckParam[i] = new CheckBox
                                    {
                                        Parent = PnlParameters,
                                        CheckAlign = ContentAlignment.MiddleLeft,
                                        TextAlign = ContentAlignment.MiddleLeft
                                    };
                CheckParam[i].CheckedChanged += ParamChanged;
                CheckParam[i].ForeColor = LayoutColors.ColorControlText;
                CheckParam[i].BackColor = Color.Transparent;
                CheckParam[i].Enabled = false;
            }

            // Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.UseVisualStyleBackColor = true;

            // Button Default
            BtnDefault.Parent = this;
            BtnDefault.Text = Language.T("Default");
            BtnDefault.Click += BtnDefaultClick;
            BtnDefault.UseVisualStyleBackColor = true;

            // Button Help
            BtnHelp.Parent = this;
            BtnHelp.Text = Language.T("Help");
            BtnHelp.Click += BtnHelp_Click;
            BtnHelp.UseVisualStyleBackColor = true;

            // Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.UseVisualStyleBackColor = true;

            SetTreeViewIndicators();

            // ComboBoxindicator index selection.
            if (isSlotDefined)
            {
                TreeNode[] atrn = TrvIndicators.Nodes.Find(Data.Strategy.Slot[_slot].IndParam.IndicatorName, true);
                TrvIndicators.SelectedNode = atrn[0];
                UpdateFromIndicatorParam(Data.Strategy.Slot[_slot].IndParam);
                SetLogicalGroup();
                CalculateIndicator();
            }
            else
            {
                string sDefaultIndicator;
                if (slotType == SlotTypes.Open)
                    sDefaultIndicator = "Bar Opening";
                else if (slotType == SlotTypes.OpenFilter)
                    sDefaultIndicator = "Accelerator Oscillator";
                else if (slotType == SlotTypes.Close)
                    sDefaultIndicator = "Bar Closing";
                else
                    sDefaultIndicator = "Accelerator Oscillator";

                TreeNode[] atrn = TrvIndicators.Nodes.Find(sDefaultIndicator, true);
                TrvIndicators.SelectedNode = atrn[0];
                TrvIndicatorsLoadIndicator();
            }

            OppSignalBehaviour = Data.Strategy.OppSignalAction;

            if (slotType == SlotTypes.Close && Data.Strategy.CloseFilters > 0)
                for (int iSlot = Data.Strategy.CloseSlot + 1; iSlot < Data.Strategy.Slots; iSlot++)
                    _closingConditions.Add(Data.Strategy.Slot[iSlot].Clone());
        }

        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }
        private Button BtnDefault { get; set; }
        private Button BtnHelp { get; set; }
        private ComboBox CbxGroup { get; set; }
        private Label LblGroup { get; set; }
        private Label LblIndicator { get; set; }
        private Label LblIndicatorInfo { get; set; }
        private Label LblIndicatorWarning { get; set; }
        private OppositeDirSignalAction OppSignalBehaviour { get; set; }
        private FancyPanel PnlParameters { get; set; }
        private FancyPanel PnlTreeViewBase { get; set; }
        private SlotTypes SlotType { get; set; }
        private TreeView TrvIndicators { get; set; }

        /// <summary>
        /// Gets or sets the caption of a ComboBox control.
        /// </summary>
        private Label[] ListLabel { get; set; }

        /// <summary>
        /// Gets or sets the parameters of a ComboBox control.
        /// </summary>
        private ComboBox[] ListParam { get; set; }

        /// <summary>
        /// Gets or sets the caption of a NumericUpDown control.
        /// </summary>
        private Label[] NumLabel { get; set; }

        /// <summary>
        /// Gets or sets the parameters of a NumericUpDown control.
        /// </summary>
        private NUD[] NumParam { get; set; }

        /// <summary>
        /// Gets or sets the parameters of a CheckBox control.
        /// </summary>
        private CheckBox[] CheckParam { get; set; }

// ---------------------------------------------------------------------------

        /// <summary>
        /// OnLoad
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Width = 670;
            Height = 570;

            MinimumSize = new Size(Width, Height);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int) (Data.VerticalDLU*15.5);
            var buttonWidth = (int) (Data.HorizontalDLU*60);
            var btnVertSpace = (int) (Data.VerticalDLU*5.5);
            var btnHrzSpace = (int) (Data.HorizontalDLU*3);
            int space = btnHrzSpace;
            int textHeight = Font.Height;
            int controlHeight = Font.Height + 4;

            int rightColumnWight = 4*buttonWidth + 3*btnHrzSpace;
            int pnlTreeViewWidth = ClientSize.Width - rightColumnWight - 3*space;

            // Panel pnlTreeViewBase
            PnlTreeViewBase.Size = new Size(pnlTreeViewWidth, ClientSize.Height - 2*space);
            PnlTreeViewBase.Location = new Point(space, space);

            int rightColumnLeft = pnlTreeViewWidth + 2*space;
            const int nudWidth = 65;

            // pnlParameterBase
            PnlParameters.Width = rightColumnWight;
            PnlParameters.Location = new Point(rightColumnLeft, space);

            //Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(rightColumnLeft, ClientSize.Height - btnVertSpace - buttonHeight);

            //Button Default
            BtnDefault.Size = BtnAccept.Size;
            BtnDefault.Location = new Point(BtnAccept.Right + btnHrzSpace, BtnAccept.Top);

            //Button Help
            BtnHelp.Size = BtnAccept.Size;
            BtnHelp.Location = new Point(BtnDefault.Right + btnHrzSpace, BtnAccept.Top);

            //Button Cancel
            BtnCancel.Size = BtnAccept.Size;
            BtnCancel.Location = new Point(BtnHelp.Right + btnHrzSpace, BtnAccept.Top);

            // lblIndicatorInfo
            LblIndicatorInfo.Location = new Point(PnlParameters.Width - LblIndicatorInfo.Width - 1, 1);

            // lblIndicatorWarning
            LblIndicatorWarning.Location = new Point(LblIndicatorInfo.Left - LblIndicatorWarning.Width - 1, 1);

            // lblIndicator
            LblIndicator.Size = new Size(PnlParameters.ClientSize.Width - 2*Border - 2*space,
                                         3*LblIndicator.Font.Height/2);
            LblIndicator.Location = new Point(Border + space, (int) PnlParameters.CaptionHeight + space);

            // Logical Group
            Graphics g = CreateGraphics();
            LblGroup.Width = (int) g.MeasureString(Language.T("Group"), Font).Width + 10;
            g.Dispose();
            LblGroup.Height = textHeight;
            LblGroup.Location = new Point(PnlParameters.ClientSize.Width - Border - space - LblGroup.Width,
                                          LblIndicator.Bottom + space);

            // ComboBox Groups
            CbxGroup.Size = new Size(LblGroup.Width, controlHeight);
            CbxGroup.Location = new Point(LblGroup.Left, ListLabel[0].Bottom + space);

            int rightShift = Configs.UseLogicalGroups &&
                             (SlotType == SlotTypes.OpenFilter || SlotType == SlotTypes.CloseFilter)
                                 ? (LblGroup.Width + space)
                                 : 0;

            // Label Logic
            ListLabel[0].Size = new Size(PnlParameters.ClientSize.Width - 2*Border - 2*space - rightShift, textHeight);
            ListLabel[0].Location = new Point(Border + space, LblIndicator.Bottom + space);

            // ComboBox Logic
            ListParam[0].Size = new Size(PnlParameters.ClientSize.Width - 2*Border - 2*space - rightShift, controlHeight);
            ListParam[0].Location = new Point(Border + space, ListLabel[0].Bottom + space);

            // ListParams
            for (int m = 0; m < 2; m++)
                for (int n = 0; n < 2; n++)
                {
                    int i = 2*m + n + 1;
                    int x = (ListParam[1].Width + space)*n + space + Border;
                    int y = (textHeight + controlHeight + 3*space)*m + ListParam[0].Bottom + 2*space;

                    ListLabel[i].Size = new Size((PnlParameters.ClientSize.Width - 3*space - 2*Border)/2, textHeight);
                    ListLabel[i].Location = new Point(x, y);

                    ListParam[i].Size = new Size((PnlParameters.ClientSize.Width - 3*space - 2*Border)/2, controlHeight);
                    ListParam[i].Location = new Point(x, y + textHeight + space);
                }

            // NumParams
            for (int m = 0; m < 3; m++)
                for (int n = 0; n < 2; n++)
                {
                    int i = 2*m + n;
                    int lblWidth = (PnlParameters.ClientSize.Width - 5*space - 2*nudWidth - 2*Border)/2;
                    NumLabel[i].Size = new Size(lblWidth, controlHeight);
                    NumLabel[i].Location = new Point((lblWidth + nudWidth + 2*space)*n + space + Border,
                                                     (controlHeight + 2*space)*m + 2*space + ListParam[3].Bottom);

                    NumParam[i].Size = new Size(nudWidth, controlHeight);
                    NumParam[i].Location = new Point(NumLabel[i].Right + space, NumLabel[i].Top);
                }

            // CheckParams
            for (int i = 0; i < 2; i++)
            {
                int chbWidth = (PnlParameters.ClientSize.Width - 3*space - 2*Border)/2;
                CheckParam[i].Size = new Size(chbWidth, controlHeight);
                CheckParam[i].Location = new Point((space + chbWidth)*i + space + Border, NumParam[4].Bottom + space);
            }

            PnlParameters.ClientSize = new Size(PnlParameters.ClientSize.Width, CheckParam[0].Bottom + space + Border);

            ClientSize = new Size(ClientSize.Width, PnlParameters.Bottom + 2*btnVertSpace + buttonHeight);
        }

        /// <summary>
        /// Sets the controls' parameters.
        /// </summary>
        private void UpdateFromIndicatorParam(IndicatorParam ip)
        {
            _indicatorName = ip.IndicatorName;
            LblIndicator.Text = _indicatorName;

            _isPaintAllowed = false;

            // List params
            for (int i = 0; i < 5; i++)
            {
                ListParam[i].Items.Clear();
                ListParam[i].Items.AddRange(ip.ListParam[i].ItemList);
                ListLabel[i].Text = ip.ListParam[i].Caption;
                ListParam[i].SelectedIndex = ip.ListParam[i].Index;
                ListParam[i].Enabled = ip.ListParam[i].Enabled;
                _toolTip.SetToolTip(ListParam[i], ip.ListParam[i].ToolTip);
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                NumParam[i].BeginInit();
                NumLabel[i].Text = ip.NumParam[i].Caption;
                NumParam[i].Minimum = (decimal) ip.NumParam[i].Min;
                NumParam[i].Maximum = (decimal) ip.NumParam[i].Max;
                NumParam[i].Value = (decimal) ip.NumParam[i].Value;
                NumParam[i].DecimalPlaces = ip.NumParam[i].Point;
                NumParam[i].Increment = (decimal) Math.Pow(10, -ip.NumParam[i].Point);
                NumParam[i].Enabled = ip.NumParam[i].Enabled;
                NumParam[i].EndInit();
                _toolTip.SetToolTip(NumParam[i],
                                    ip.NumParam[i].ToolTip + Environment.NewLine + "Minimum: " + NumParam[i].Minimum +
                                    " Maximum: " + NumParam[i].Maximum);
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                CheckParam[i].Text = ip.CheckParam[i].Caption;
                CheckParam[i].Checked = ip.CheckParam[i].Checked;
                _toolTip.SetToolTip(CheckParam[i], ip.CheckParam[i].ToolTip);

                if (Data.AutoUsePrvBarValue && ip.CheckParam[i].Caption == "Use previous bar value")
                    CheckParam[i].Enabled = false;
                else
                    CheckParam[i].Enabled = ip.CheckParam[i].Enabled;
            }

            _isPaintAllowed = true;
        }

        /// <summary>
        /// Sets the logical group of the slot.
        /// </summary>
        private void SetLogicalGroup()
        {
            if (SlotType == SlotTypes.OpenFilter || SlotType == SlotTypes.CloseFilter)
            {
                string group = Data.Strategy.Slot[_slot].LogicalGroup;
                if (string.IsNullOrEmpty(group))
                    SetDefaultGroup();
                else
                {
                    if (group.ToLower() == "all")
                        CbxGroup.SelectedIndex = CbxGroup.Items.Count - 1;
                    else
                        CbxGroup.SelectedIndex = char.ConvertToUtf32(group.ToLower(), 0) - char.ConvertToUtf32("a", 0);
                }
            }
        }

        /// <summary>
        /// Sets the default logical group of the slot.
        /// </summary>
        private void SetDefaultGroup()
        {
            if (SlotType == SlotTypes.OpenFilter)
            {
                if (_indicatorName == "Data Bars Filter" ||
                    _indicatorName == "Date Filter" ||
                    _indicatorName == "Day of Month" ||
                    _indicatorName == "Enter Once" ||
                    _indicatorName == "Entry Time" ||
                    _indicatorName == "Long or Short" ||
                    _indicatorName == "Lot Limiter" ||
                    _indicatorName == "Random Filter")
                    CbxGroup.SelectedIndex = CbxGroup.Items.Count - 1; // "All" group.
                else
                    CbxGroup.SelectedIndex = 0;
            }

            if (SlotType == SlotTypes.CloseFilter)
            {
                int index = _slot - Data.Strategy.CloseSlot - 1;
                CbxGroup.SelectedIndex = index;
            }
        }

        /// <summary>
        /// Sets the trvIndicators nodes
        /// </summary>
        private void SetTreeViewIndicators()
        {
            var trnAll = new TreeNode {Name = "trnAll", Text = Language.T("All"), Tag = false};

            var trnIndicators = new TreeNode {Name = "trnIndicators", Text = Language.T("Indicators"), Tag = false};

            var trnAdditional = new TreeNode {Name = "trnAdditional", Text = Language.T("Additional"), Tag = false};

            var trnOscillatorOfIndicators = new TreeNode
                                                {
                                                    Name = "trnOscillatorOfIndicators",
                                                    Text = Language.T("Oscillator of Indicators"),
                                                    Tag = false
                                                };

            var trnIndicatorsMAOscillator = new TreeNode
                                                {
                                                    Name = "trnIndicatorMA",
                                                    Text = Language.T("Indicator's MA Oscillator"),
                                                    Tag = false
                                                };

            var trnDateTime = new TreeNode {Name = "trnDateTime", Text = Language.T("Date/Time Functions"), Tag = false};

            var trnCustomIndicators = new TreeNode
                                          {
                                              Name = "trnCustomIndicators",
                                              Text = Language.T("Custom Indicators"),
                                              Tag = false
                                          };

            TrvIndicators.Nodes.AddRange(new[]
                                             {
                                                 trnAll, trnIndicators, trnAdditional, trnOscillatorOfIndicators,
                                                 trnIndicatorsMAOscillator, trnDateTime, trnCustomIndicators
                                             });

            foreach (string indicatorName in IndicatorStore.GetIndicatorNames(SlotType))
            {
                // Checks the indicator name in the list of forbidden indicators.
                bool toContinue = false;
                foreach (string forbiden in Data.IndicatorsForBacktestOnly)
                    if (indicatorName == forbiden)
                    {
                        toContinue = true;
                        break;
                    }
                if (toContinue)
                    continue;

                var trn = new TreeNode {Tag = true, Name = indicatorName, Text = indicatorName};
                trnAll.Nodes.Add(trn);

                Indicator indicator = IndicatorStore.ConstructIndicator(indicatorName, SlotType);
                TypeOfIndicator type = indicator.IndParam.IndicatorType;

                if (indicator.CustomIndicator)
                {
                    var trnCustom = new TreeNode {Tag = true, Name = indicatorName, Text = indicatorName};
                    trnCustomIndicators.Nodes.Add(trnCustom);
                }

                var trnGroups = new TreeNode {Tag = true, Name = indicatorName, Text = indicatorName};

                if (type == TypeOfIndicator.Indicator)
                {
                    trnIndicators.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.Additional)
                {
                    trnAdditional.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.OscillatorOfIndicators)
                {
                    trnOscillatorOfIndicators.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.IndicatorsMA)
                {
                    trnIndicatorsMAOscillator.Nodes.Add(trnGroups);
                }
                else if (type == TypeOfIndicator.DateTime)
                {
                    trnDateTime.Nodes.Add(trnGroups);
                }
            }
        }

        /// <summary>
        /// Loads the default parameters for the chosen indicator.
        /// </summary>
        private void TrvIndicatorsNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!(bool) TrvIndicators.SelectedNode.Tag)
                return;

            TrvIndicatorsLoadIndicator();
        }

        /// <summary>
        /// Loads the default parameters for the chosen indicator.
        /// </summary>
        private void TrvIndicatorsKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != ' ')
                return;

            if (!(bool) TrvIndicators.SelectedNode.Tag)
                return;

            TrvIndicatorsLoadIndicator();
        }

        /// <summary>
        /// Loads an Indicator
        /// </summary>
        private void TrvIndicatorsLoadIndicator()
        {
            Indicator indicator = IndicatorStore.ConstructIndicator(TrvIndicators.SelectedNode.Text, SlotType);
            UpdateFromIndicatorParam(indicator.IndParam);
            SetDefaultGroup();
            CalculateIndicator();
        }

        /// <summary>
        /// Loads the defaults parameters for the selected indicator.
        /// </summary>
        private void BtnDefaultClick(object sender, EventArgs e)
        {
            Indicator indicator = IndicatorStore.ConstructIndicator(_indicatorName, SlotType);
            UpdateFromIndicatorParam(indicator.IndParam);
            SetDefaultGroup();
            CalculateIndicator();
        }

        /// <summary>
        /// Shows help for the selected indicator.
        /// </summary>
        private void BtnHelp_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("http://forexsb.com/wiki/indicators/start");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Sets the slot group.
        /// </summary>
        private void GroupChanged(object sender, EventArgs e)
        {
            if (SlotType == SlotTypes.OpenFilter || SlotType == SlotTypes.CloseFilter)
                Data.Strategy.Slot[_slot].LogicalGroup = CbxGroup.Text;

            ParamChanged(sender, e);
        }

        /// <summary>
        /// Calls Preview()
        /// </summary>
        private void ParamChanged(object sender, EventArgs e)
        {
            CalculateIndicator();
        }

        /// <summary>
        /// Calculates the selected indicator chart.
        /// </summary>
        private void CalculateIndicator()
        {
            if (!Data.IsData || !_isPaintAllowed) return;

            SetOppositeSignalBehaviour();
            SetClosingLogicConditions();

            Indicator indicator = IndicatorStore.ConstructIndicator(_indicatorName, SlotType);

            // List params
            for (int i = 0; i < 5; i++)
            {
                indicator.IndParam.ListParam[i].Index = ListParam[i].SelectedIndex;
                indicator.IndParam.ListParam[i].Text = ListParam[i].Text;
                indicator.IndParam.ListParam[i].Enabled = ListParam[i].Enabled;
            }

            // Numeric params
            for (int i = 0; i < 6; i++)
            {
                indicator.IndParam.NumParam[i].Value = (double) NumParam[i].Value;
                indicator.IndParam.NumParam[i].Enabled = NumParam[i].Enabled;
            }

            // Check params
            for (int i = 0; i < 2; i++)
            {
                indicator.IndParam.CheckParam[i].Checked = CheckParam[i].Checked;
                indicator.IndParam.CheckParam[i].Enabled = CheckParam[i].Enabled;
                indicator.IndParam.CheckParam[i].Enabled = CheckParam[i].Text == "Use previous bar value" ||
                                                           CheckParam[i].Enabled;
            }

            indicator.Calculate(SlotType);

            //Sets the Data.Strategy.
            Data.Strategy.Slot[_slot].IndicatorName = indicator.IndicatorName;
            Data.Strategy.Slot[_slot].IndParam = indicator.IndParam;
            Data.Strategy.Slot[_slot].Component = indicator.Component;
            Data.Strategy.Slot[_slot].SeparatedChart = indicator.SeparatedChart;
            Data.Strategy.Slot[_slot].SpecValue = indicator.SpecialValues;
            Data.Strategy.Slot[_slot].MinValue = indicator.SeparatedChartMinValue;
            Data.Strategy.Slot[_slot].MaxValue = indicator.SeparatedChartMaxValue;
            Data.Strategy.Slot[_slot].IsDefined = true;

            // Searches the indicators' components to determine the Data.FirstBar. 
            Data.FirstBar = Data.Strategy.SetFirstBar();

            // Checks "Use previous bar value".
            if (Data.Strategy.AdjustUsePreviousBarValue())
            {
                for (int i = 0; i < 2; i++)
                    if (indicator.IndParam.CheckParam[i].Caption == "Use previous bar value")
                        CheckParam[i].Checked = Data.Strategy.Slot[_slot].IndParam.CheckParam[i].Checked;
            }

            SetIndicatorNotification(indicator);
        }

        /// <summary>
        /// Sets the indicator overview and warning message.
        /// </summary>
        private void SetIndicatorNotification(Indicator indicator)
        {
            // Warning message.
            _warningMessage = indicator.WarningMessage;
            LblIndicatorWarning.Visible = !string.IsNullOrEmpty(_warningMessage);

            // Set description.
            indicator.SetDescription(SlotType);
            _description = "Long position:" + Environment.NewLine;
            if (SlotType == SlotTypes.Open)
            {
                _description += "   Open a long position " + indicator.EntryPointLongDescription + "." +
                                Environment.NewLine + Environment.NewLine;
                _description += "Short position:" + Environment.NewLine;
                _description += "   Open a short position " + indicator.EntryPointShortDescription + ".";
            }
            else if (SlotType == SlotTypes.OpenFilter)
            {
                _description += "   Open a long position when " + indicator.EntryFilterLongDescription + "." +
                                Environment.NewLine + Environment.NewLine;
                _description += "Short position:" + Environment.NewLine;
                _description += "   Open a short position when " + indicator.EntryFilterShortDescription + ".";
            }
            else if (SlotType == SlotTypes.Close)
            {
                _description += "   Close a long position " + indicator.ExitPointLongDescription + "." +
                                Environment.NewLine + Environment.NewLine;
                _description += "Short position:" + Environment.NewLine;
                _description += "   Close a short position " + indicator.ExitPointShortDescription + ".";
            }
            else
            {
                _description += "   Close a long position when " + indicator.ExitFilterLongDescription + "." +
                                Environment.NewLine + Environment.NewLine;
                _description += "Short position:" + Environment.NewLine;
                _description += "   Close a short position when " + indicator.ExitFilterShortDescription + ".";
            }

            for (int i = 0; i < 2; i++)
                if (indicator.IndParam.CheckParam[i].Caption == "Use previous bar value")
                    _description += Environment.NewLine + "-------------" + Environment.NewLine + "* Use the value of " +
                                    indicator.IndicatorName + " from the previous bar.";

            _toolTip.SetToolTip(LblIndicatorInfo, _description);
        }

        /// <summary>
        /// Sets or restores the closing logic conditions.
        /// </summary>
        private void SetClosingLogicConditions()
        {
            bool isClosingFiltersAllowed = IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(_indicatorName);

            // Removes or recovers closing logic slots.
            if (SlotType == SlotTypes.Close && !isClosingFiltersAllowed && Data.Strategy.CloseFilters > 0)
            {
                // Removes all the closing logic conditions.
                Data.Strategy.RemoveAllCloseFilters();
                _closingSlotsRemoved = true;
            }
            else if (SlotType == SlotTypes.Close && isClosingFiltersAllowed && _closingSlotsRemoved)
            {
                foreach (IndicatorSlot inslot in _closingConditions)
                {
                    // Recovers all the closing logic conditions.
                    Data.Strategy.AddCloseFilter();
                    Data.Strategy.Slot[Data.Strategy.Slots - 1] = inslot.Clone();
                }
                _closingSlotsRemoved = false;
            }
        }

        /// <summary>
        /// Sets or restores the opposite signal behaviour.
        /// </summary>
        private void SetOppositeSignalBehaviour()
        {
            // Changes opposite signal behaviour.
            if (SlotType == SlotTypes.Close && _indicatorName == "Close and Reverse" &&
                OppSignalBehaviour != OppositeDirSignalAction.Reverse)
            {
                // Sets the strategy opposite signal to Reverse.
                Data.Strategy.OppSignalAction = OppositeDirSignalAction.Reverse;
                _oppSignalSet = true;
            }
            else if (SlotType == SlotTypes.Close && _indicatorName != "Close and Reverse" && _oppSignalSet)
            {
                // Recovers the original opposite signal.
                Data.Strategy.OppSignalAction = OppSignalBehaviour;
                _oppSignalSet = false;
            }
        }

        /// <summary>
        /// Shows the indicator description
        /// </summary>
        private void LblIndicatorInfoClick(object sender, EventArgs e)
        {
            MessageBox.Show(_description, _slotTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows the indicator warning
        /// </summary>
        private void LblIndicatorWarningClick(object sender, EventArgs e)
        {
            MessageBox.Show(_warningMessage, _indicatorName, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        /// <summary>
        /// Changes the background color of a label when the mouse leaves.
        /// </summary>
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            var lbl = (Label) sender;
            lbl.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Changes the background color of a label when the mouse enters.
        /// </summary>
        private void Label_MouseEnter(object sender, EventArgs e)
        {
            var lbl = (Label) sender;
            lbl.BackColor = Color.Orange;
        }
    }
}