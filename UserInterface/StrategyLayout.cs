// Strategy Layout
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Forex_Strategy_Trader.Properties;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Represents the strategies slots into a readable form
    /// </summary>
    public class StrategyLayout : Panel
    {
        private const int Border = 2;
        private const int Space = 4;
        public readonly Button BtnAddCloseFilter; // Add an Close Filter indicator slot
        public readonly Button BtnAddOpenFilter; // Add an Open Filter indicator slot
        private readonly Button _btnClosingFilterHelp;
        private readonly FlowLayoutPanel _flowLayoutStrategy; // Contains the strategy slots
        private readonly string _slotPropertiesTipText = Language.T("Averaging, Trading size, Protection.");
        private readonly string _slotToolTipText = Language.T("Long position logic.");

        private readonly ToolTip _toolTip;
        private readonly VScrollBar _vScrollBarStrategy; // Vertical scrollbar
        public Button[] AbtnRemoveSlot; // Removes the indicator slot
        public Panel[] ApnlSlot; // Indicator's parameters
        public Panel PnlProperties; // Strategy properties panel
        private SlotSizeMinMidMax _slotMinMidMax;
        private int _slots;
        private Strategy _strategy;

        /// <summary>
        /// Initializes the strategy field
        /// </summary>
        public StrategyLayout(Strategy strategy)
        {
            _strategy = strategy;
            _slots = strategy.Slots;
            _slotMinMidMax = SlotSizeMinMidMax.Mid;
            _toolTip = new ToolTip();
            _flowLayoutStrategy = new FlowLayoutPanel();
            _vScrollBarStrategy = new VScrollBar();
            ApnlSlot = new Panel[_slots];
            PnlProperties = new Panel();

            for (int iSlot = 0; iSlot < _slots; iSlot++)
                ApnlSlot[iSlot] = new Panel();

            AbtnRemoveSlot = new Button[_slots];
            for (int iSlot = 0; iSlot < _slots; iSlot++)
                AbtnRemoveSlot[iSlot] = new Button();

            // FlowLayoutStrategy
            _flowLayoutStrategy.Parent = this;
            _flowLayoutStrategy.AutoScroll = false;

            //VScrollBarStrategy
            _vScrollBarStrategy.Parent = this;
            _vScrollBarStrategy.TabStop = true;
            _vScrollBarStrategy.Scroll += VScrollBarStrategyScroll;

            // btnAddOpenFilter
            BtnAddOpenFilter = new Button
                                   {
                                       Tag = strategy.OpenSlot,
                                       Text = Language.T("Add an Opening Logic Condition"),
                                       Margin = new Padding(30, 0, 0, Space),
                                       UseVisualStyleBackColor = true
                                   };
            _toolTip.SetToolTip(BtnAddOpenFilter, Language.T("Add a new entry logic slot to the strategy."));

            // btnAddCloseFilter
            BtnAddCloseFilter = new Button
                                    {
                                        Tag = strategy.CloseSlot,
                                        Text = Language.T("Add a Closing Logic Condition"),
                                        Margin = new Padding(30, 0, 0, Space),
                                        UseVisualStyleBackColor = true
                                    };
            _toolTip.SetToolTip(BtnAddCloseFilter, Language.T("Add a new exit logic slot to the strategy."));

            // btnClosingFilterHelp
            _btnClosingFilterHelp = new Button
                                        {
                                            Image = Resources.info,
                                            Margin = new Padding(2, 2, 0, Space),
                                            TabStop = false
                                        };
            _btnClosingFilterHelp.Click += BtnClosingFilterHelp_Click;
            _btnClosingFilterHelp.UseVisualStyleBackColor = true;
        }

        /// <summary>
        /// Sets the size of the strategy's slots
        /// </summary>
        public SlotSizeMinMidMax SlotMinMidMax
        {
            get { return _slotMinMidMax; }
            set { _slotMinMidMax = value; }
        }

        /// <summary>
        /// Initializes the strategy slots
        /// </summary>
        private void InitializeStrategySlots()
        {
            ApnlSlot = new Panel[_slots];
            AbtnRemoveSlot = new Button[_slots];

            // Strategy properties panel
            PnlProperties = new Panel {Cursor = Cursors.Hand, Tag = 100, Margin = new Padding(0, 0, 0, Space)};
            PnlProperties.Paint += PnlPropertiesPaint;
            PnlProperties.Resize += PnlSlot_Resize;
            _toolTip.SetToolTip(PnlProperties, _slotPropertiesTipText);

            // Slot panels settings
            for (int iSlot = 0; iSlot < _slots; iSlot++)
            {
                ApnlSlot[iSlot] = new Panel {Cursor = Cursors.Hand, Tag = iSlot, Margin = new Padding(0, 0, 0, Space)};
                ApnlSlot[iSlot].Paint += PnlSlotPaint;
                ApnlSlot[iSlot].Resize += PnlSlot_Resize;
                _toolTip.SetToolTip(ApnlSlot[iSlot], _slotToolTipText);

                if (iSlot != _strategy.OpenSlot && iSlot != _strategy.CloseSlot)
                {
                    // RemoveSlot buttons
                    AbtnRemoveSlot[iSlot] = new Button
                                                {
                                                    Parent = ApnlSlot[iSlot],
                                                    Tag = iSlot,
                                                    Cursor = Cursors.Arrow,
                                                    BackgroundImage = Resources.close_button,
                                                    UseVisualStyleBackColor = true
                                                };
                    _toolTip.SetToolTip(AbtnRemoveSlot[iSlot], Language.T("Discard the logic condition."));
                }
            }

            // Adds the controls to the flow layout
            _flowLayoutStrategy.Controls.Add(PnlProperties);
            for (int iSlot = 0; iSlot < _slots; iSlot++)
            {
                if (iSlot == _strategy.CloseSlot)
                    _flowLayoutStrategy.Controls.Add(BtnAddOpenFilter);
                _flowLayoutStrategy.Controls.Add(ApnlSlot[iSlot]);
            }
            _flowLayoutStrategy.Controls.Add(BtnAddCloseFilter);
            _flowLayoutStrategy.Controls.Add(_btnClosingFilterHelp);
        }

        /// <summary>
        /// Calculates the position of the controls
        /// </summary>
        private void ArrangeStrategyControls()
        {
            int iWidth = ClientSize.Width;
            int iHeight = ClientSize.Height;
            int iTotalHeight = PnlSlotCalculateTotalHeight(iWidth);
            if (iTotalHeight < iHeight)
            {
                _vScrollBarStrategy.Enabled = false;
                _vScrollBarStrategy.Visible = false;
            }
            else
            {
                iWidth = ClientSize.Width - _vScrollBarStrategy.Width;
                iTotalHeight = PnlSlotCalculateTotalHeight(iWidth);
                _vScrollBarStrategy.Enabled = true;
                _vScrollBarStrategy.Visible = true;
                _vScrollBarStrategy.Value = 0;
                _vScrollBarStrategy.SmallChange = 100;
                _vScrollBarStrategy.LargeChange = 200;
                _vScrollBarStrategy.Maximum = Math.Max(iTotalHeight - iHeight + 220, 0);
                _vScrollBarStrategy.Location = new Point(iWidth, 0);
                _vScrollBarStrategy.Height = iHeight;
            }

            _flowLayoutStrategy.Location = Point.Empty;
            _flowLayoutStrategy.Width = iWidth;
            _flowLayoutStrategy.Height = iTotalHeight;

            // Strategy properties panel size
            int iPnlPropertiesWidth = _flowLayoutStrategy.ClientSize.Width;
            int iPnlPropertiesHeight = PnlPropertiesCalculateHeight();
            PnlProperties.Size = new Size(iPnlPropertiesWidth, iPnlPropertiesHeight);

            // Sets the strategy slots size
            for (int iSlot = 0; iSlot < _slots; iSlot++)
            {
                int iStrWidth = _flowLayoutStrategy.ClientSize.Width;
                int iStrHeight = PnlSlotCalculateHeight(iSlot, iStrWidth);
                ApnlSlot[iSlot].Size = new Size(iStrWidth, iStrHeight);
            }

            int iButtonWidth = _flowLayoutStrategy.ClientSize.Width - 60;
            var iButtonHeight = (int) (Font.Height*1.7);
            BtnAddOpenFilter.Size = new Size(iButtonWidth, iButtonHeight);
            BtnAddCloseFilter.Size = new Size(iButtonWidth, iButtonHeight);
            _btnClosingFilterHelp.Size = new Size(iButtonHeight - 4, iButtonHeight - 4);
        }

        /// <summary>
        /// Sets add new slot buttons
        /// </summary>
        private void SetAddSlotButtons()
        {
            // Shows or not btnAddOpenFilter
            BtnAddOpenFilter.Enabled = _strategy.OpenFilters < Strategy.MaxOpenFilters;

            bool isClosingFiltersAllowed =
                IndicatorStore.ClosingIndicatorsWithClosingFilters.Contains(
                    _strategy.Slot[_strategy.CloseSlot].IndicatorName);

            // Shows or not btnAddCloseFilter
            BtnAddCloseFilter.Enabled = (_strategy.CloseFilters < Strategy.MaxCloseFilters && isClosingFiltersAllowed);

            // Shows or not btnClosingFilterHelp
            _btnClosingFilterHelp.Visible = !isClosingFiltersAllowed;
        }

        /// <summary>
        /// The Scrolling moves the flowLayout
        /// </summary>
        private void VScrollBarStrategyScroll(object sender, ScrollEventArgs e)
        {
            var vscroll = (VScrollBar) sender;
            _flowLayoutStrategy.Location = new Point(0, -vscroll.Value);
        }

        /// <summary>
        /// Rebuilds all the controls in panel Strategy
        /// </summary>
        public void RebuildStrategyControls(Strategy strategy)
        {
            _strategy = strategy;
            _slots = strategy.Slots;
            _flowLayoutStrategy.SuspendLayout();
            _flowLayoutStrategy.Controls.Clear();
            InitializeStrategySlots();
            ArrangeStrategyControls();
            SetAddSlotButtons();
            _flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Rearrange all controls in panel Strategy.
        /// </summary>
        public void RearangeStrategyControls()
        {
            _flowLayoutStrategy.SuspendLayout();
            ArrangeStrategyControls();
            _flowLayoutStrategy.ResumeLayout();
        }

        /// <summary>
        /// Repaints the strategy slots
        /// </summary>
        /// <param name="strategy">The strategy</param>
        public void RepaintStrategyControls(Strategy strategy)
        {
            _strategy = strategy;
            _slots = strategy.Slots;
            foreach (Panel pnl in ApnlSlot)
                pnl.Invalidate();
            PnlProperties.Invalidate();
        }

        /// <summary>
        /// Panel Strategy Resize
        /// </summary>
        private void PnlSlot_Resize(object sender, EventArgs e)
        {
            var pnl = (Panel) sender;
            pnl.Invalidate();
        }

        /// <summary>
        /// Calculates the height of the Panel Slot
        /// </summary>
        private int PnlSlotCalculateHeight(int slot, int width)
        {
            var fontCaption = new Font(Font.FontFamily, 9f);
            int iVPosition = Math.Max(fontCaption.Height, 18) + 3;

            var fontIndicator = new Font(Font.FontFamily, 11f);
            iVPosition += fontIndicator.Height;

            if (_slotMinMidMax == SlotSizeMinMidMax.Min)
                return iVPosition + 5;

            // Calculate the height of Logic string
            if (_strategy.Slot[slot].IndParam.ListParam[0].Enabled)
            {
                Graphics g = CreateGraphics();
                const float fPadding = Space;

                var stringFormatLogic = new StringFormat
                                            {
                                                Alignment = StringAlignment.Center,
                                                LineAlignment = StringAlignment.Center,
                                                Trimming = StringTrimming.EllipsisCharacter,
                                                FormatFlags = StringFormatFlags.NoClip
                                            };

                string sValue = _strategy.Slot[slot].IndParam.ListParam[0].Text;
                var fontLogic = new Font(Font.FontFamily, 10.5f, FontStyle.Regular);
                SizeF sizeValue = g.MeasureString(sValue, fontLogic, (int) (width - 2*fPadding), stringFormatLogic);
                iVPosition += (int) sizeValue.Height;
                g.Dispose();
            }

            if (_slotMinMidMax == SlotSizeMinMidMax.Mid)
                return iVPosition + 6;

            var fontParam = new Font(Font.FontFamily, 9f, FontStyle.Regular);

            // List Params
            for (int i = 1; i < 5; i++)
                iVPosition += _strategy.Slot[slot].IndParam.ListParam[i].Enabled ? fontParam.Height : 0;

            // NumericParam
            foreach (NumericParam nump in _strategy.Slot[slot].IndParam.NumParam)
                iVPosition += nump.Enabled ? fontParam.Height : 0;

            // CheckParam
            foreach (CheckParam checkp in _strategy.Slot[slot].IndParam.CheckParam)
                iVPosition += checkp.Enabled ? fontParam.Height : 0;

            iVPosition += 10;

            return iVPosition;
        }

        /// <summary>
        /// Calculates the height of the Averaging Panel
        /// </summary>
        private int PnlPropertiesCalculateHeight()
        {
            var fontCaption = new Font(Font.FontFamily, 9f);
            int iVPosition = Math.Max(fontCaption.Height, 18) + 3;

            var fontAveraging = new Font(Font.FontFamily, 9f);

            if (_slotMinMidMax == SlotSizeMinMidMax.Min)
                iVPosition += fontAveraging.Height;
            else
                iVPosition += 5*fontAveraging.Height + 5;

            return iVPosition + 8;
        }

        /// <summary>
        /// Calculates the total height of the Panel Slot
        /// </summary>
        private int PnlSlotCalculateTotalHeight(int width)
        {
            int iTotalHeight = 0;

            for (int iSlot = 0; iSlot < _slots; iSlot++)
                iTotalHeight += Space + PnlSlotCalculateHeight(iSlot, width);

            iTotalHeight += 2*BtnAddCloseFilter.Height + Space;

            iTotalHeight += Space + PnlPropertiesCalculateHeight();

            return iTotalHeight;
        }

        /// <summary>
        /// Panel Slot Paint
        /// </summary>
        private void PnlSlotPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            var iSlot = (int) pnl.Tag;
            int iWidth = pnl.ClientSize.Width;
            SlotTypes slotType = _strategy.GetSlotType(iSlot);

            Color colorBackground = LayoutColors.ColorSlotBackground;
            Color colorCaptionText = LayoutColors.ColorSlotCaptionText;
            Color colorCaptionBackOpen = LayoutColors.ColorSlotCaptionBackOpen;
            Color colorCaptionBackOpenFilter = LayoutColors.ColorSlotCaptionBackOpenFilter;
            Color colorCaptionBackClose = LayoutColors.ColorSlotCaptionBackClose;
            Color colorCaptionBackCloseFilter = LayoutColors.ColorSlotCaptionBackCloseFilter;
            Color colorIndicatorNameText = LayoutColors.ColorSlotIndicatorText;
            Color colorLogicText = LayoutColors.ColorSlotLogicText;
            Color colorParamText = LayoutColors.ColorSlotParamText;
            Color colorValueText = LayoutColors.ColorSlotValueText;
            Color colorDash = LayoutColors.ColorSlotDash;

            // Caption
            string stringCaptionText = string.Empty;
            Color colorCaptionBack = LayoutColors.ColorSignalRed;

            switch (slotType)
            {
                case SlotTypes.Open:
                    stringCaptionText = Language.T("Opening Point of the Position");
                    colorCaptionBack = colorCaptionBackOpen;
                    break;
                case SlotTypes.OpenFilter:
                    stringCaptionText = Language.T("Opening Logic Condition");
                    colorCaptionBack = colorCaptionBackOpenFilter;
                    break;
                case SlotTypes.Close:
                    stringCaptionText = Language.T("Closing Point of the Position");
                    colorCaptionBack = colorCaptionBackClose;
                    break;
                case SlotTypes.CloseFilter:
                    stringCaptionText = Language.T("Closing Logic Condition");
                    colorCaptionBack = colorCaptionBackCloseFilter;
                    break;
            }

            var penBorder = new Pen(Data.ColorChanage(colorCaptionBack, -LayoutColors.DepthCaption), Border);

            var fontCaptionText = new Font(Font.FontFamily, 9);
            float fCaptionHeight = Math.Max(fontCaptionText.Height, 18);
            float fCaptionWidth = iWidth;
            Brush brushCaptionText = new SolidBrush(colorCaptionText);
            var stringFormatCaption = new StringFormat
                                          {
                                              LineAlignment = StringAlignment.Center,
                                              Trimming = StringTrimming.EllipsisCharacter,
                                              FormatFlags = StringFormatFlags.NoWrap,
                                              Alignment = StringAlignment.Center
                                          };

            var rectfCaption = new RectangleF(0, 0, fCaptionWidth, fCaptionHeight);
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);

            if (iSlot != _strategy.OpenSlot && iSlot != _strategy.CloseSlot)
            {
                int iButtonDimentions = (int) fCaptionHeight - 2;
                int iButtonX = iWidth - iButtonDimentions - 1;
                AbtnRemoveSlot[iSlot].Size = new Size(iButtonDimentions, iButtonDimentions);
                AbtnRemoveSlot[iSlot].Location = new Point(iButtonX, 1);

                float fCaptionTextWidth = g.MeasureString(stringCaptionText, fontCaptionText).Width;
                float fCaptionTextX = Math.Max((fCaptionWidth - fCaptionTextWidth)/2f, 0);
                var pfCaptionText = new PointF(fCaptionTextX, 0);
                var sfCaptionText = new SizeF(iButtonX - fCaptionTextX, fCaptionHeight);
                rectfCaption = new RectangleF(pfCaptionText, sfCaptionText);
                stringFormatCaption.Alignment = StringAlignment.Near;
            }
            g.DrawString(stringCaptionText, fontCaptionText, brushCaptionText, rectfCaption, stringFormatCaption);

            // Border
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.Height);
            g.DrawLine(penBorder, pnl.Width - Border + 1, fCaptionHeight, pnl.Width - Border + 1, pnl.Height);
            g.DrawLine(penBorder, 0, pnl.Height - Border + 1, pnl.Width, pnl.Height - Border + 1);

            // Paints the panel
            var rectfPanel = new RectangleF(Border, fCaptionHeight, pnl.Width - 2*Border,
                                            pnl.Height - fCaptionHeight - Border);
            Data.GradientPaint(g, rectfPanel, colorBackground, LayoutColors.DepthControl);

            int iVPosition = (int) fCaptionHeight + 3;

            // Indicator name
            var stringFormatIndicatorName = new StringFormat
                                                {
                                                    Alignment = StringAlignment.Center,
                                                    LineAlignment = StringAlignment.Center,
                                                    Trimming = StringTrimming.EllipsisCharacter,
                                                    FormatFlags = StringFormatFlags.NoWrap
                                                };
            string sIndicatorName = _strategy.Slot[iSlot].IndicatorName;
            var fontIndicator = new Font(Font.FontFamily, 11f, FontStyle.Regular);
            Brush brushIndName = new SolidBrush(colorIndicatorNameText);
            float fIndNameHeight = fontIndicator.Height;
            float fGroupWidth = 0;
            if (Configs.UseLogicalGroups && (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter))
            {
                string sLogicalGroup = "[" + _strategy.Slot[iSlot].LogicalGroup + "]";
                fGroupWidth = g.MeasureString(sLogicalGroup, fontIndicator).Width;
                var rectGroup = new RectangleF(0, iVPosition, fGroupWidth, fIndNameHeight);
                g.DrawString(sLogicalGroup, fontIndicator, brushIndName, rectGroup, stringFormatIndicatorName);
            }
            stringFormatIndicatorName.Trimming = StringTrimming.EllipsisCharacter;
            float fIndicatorWidth = g.MeasureString(sIndicatorName, fontIndicator).Width;

            RectangleF rectIndName = iWidth >= 2*fGroupWidth + fIndicatorWidth
                                         ? new RectangleF(0, iVPosition, iWidth, fIndNameHeight)
                                         : new RectangleF(fGroupWidth, iVPosition, iWidth - fGroupWidth, fIndNameHeight);

            g.DrawString(sIndicatorName, fontIndicator, brushIndName, rectIndName, stringFormatIndicatorName);
            iVPosition += (int) fIndNameHeight;

            if (_slotMinMidMax == SlotSizeMinMidMax.Min)
                return;

            // Logic
            var stringFormatLogic = new StringFormat
                                        {
                                            Alignment = StringAlignment.Center,
                                            LineAlignment = StringAlignment.Center,
                                            Trimming = StringTrimming.EllipsisCharacter,
                                            FormatFlags = StringFormatFlags.NoClip
                                        };

            float fPadding = Space;

            if (_strategy.Slot[iSlot].IndParam.ListParam[0].Enabled)
            {
                string sValue = _strategy.Slot[iSlot].IndParam.ListParam[0].Text;
                var fontLogic = new Font(Font.FontFamily, 10.5f, FontStyle.Regular);
                SizeF sizeValue = g.MeasureString(sValue, fontLogic, (int) (iWidth - 2*fPadding), stringFormatLogic);
                var rectValue = new RectangleF(fPadding, iVPosition, iWidth - 2*fPadding, sizeValue.Height);
                Brush brushLogic = new SolidBrush(colorLogicText);

                g.DrawString(sValue, fontLogic, brushLogic, rectValue, stringFormatLogic);
                iVPosition += (int) sizeValue.Height;
            }

            if (_slotMinMidMax == SlotSizeMinMidMax.Mid)
                return;

            // Parameters
            var stringFormat = new StringFormat
                                   {Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap};

            var fontParam = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            var fontValue = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Brush brushParam = new SolidBrush(colorParamText);
            Brush brushValue = new SolidBrush(colorValueText);
            var penDash = new Pen(colorDash);

            // Find Maximum width of the strings
            float fMaxParamWidth = 0;
            float fMaxValueWidth = 0;

            for (int i = 1; i < 5; i++)
            {
                if (!_strategy.Slot[iSlot].IndParam.ListParam[i].Enabled)
                    continue;

                string sParam = _strategy.Slot[iSlot].IndParam.ListParam[i].Caption;
                string sValue = _strategy.Slot[iSlot].IndParam.ListParam[i].Text;
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            foreach (NumericParam numericParam in _strategy.Slot[iSlot].IndParam.NumParam)
            {
                if (!numericParam.Enabled) continue;

                string sParam = numericParam.Caption;
                string sValue = numericParam.ValueToString;
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            foreach (CheckParam checkParam in _strategy.Slot[iSlot].IndParam.CheckParam)
            {
                if (!checkParam.Enabled) continue;

                string sParam = checkParam.Caption;
                string sValue = checkParam.Checked ? "Yes" : "No";
                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                SizeF sizeValue = g.MeasureString(sValue, fontValue);

                if (fMaxParamWidth < sizeParam.Width)
                    fMaxParamWidth = sizeParam.Width;

                if (fMaxValueWidth < sizeValue.Width)
                    fMaxValueWidth = sizeValue.Width;
            }

            // Padding Param Padding Dash Padding Value Padding 
            const float fDashWidth = 5;
            float fNecessaryWidth = 4*fPadding + fMaxParamWidth + fMaxValueWidth + fDashWidth;

            fPadding = iWidth > fNecessaryWidth
                           ? Math.Max((pnl.ClientSize.Width - fMaxParamWidth - fMaxValueWidth - fDashWidth)/6, fPadding)
                           : 2;

            float fTabParam = 2*fPadding;
            float fTabDash = fTabParam + fMaxParamWidth + fPadding;
            float fTabValue = fTabDash + fDashWidth + fPadding;

            // List Params
            for (int i = 1; i < 5; i++)
            {
                if (!_strategy.Slot[iSlot].IndParam.ListParam[i].Enabled)
                    continue;

                string sParam = _strategy.Slot[iSlot].IndParam.ListParam[i].Caption;
                string sValue = _strategy.Slot[iSlot].IndParam.ListParam[i].Text;
                var pointParam = new PointF(fTabParam, iVPosition);
                var pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                var pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                var pointValue = new PointF(fTabValue, iVPosition);
                var sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                var rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }

            // Num Params
            foreach (NumericParam numericParam in _strategy.Slot[iSlot].IndParam.NumParam)
            {
                if (!numericParam.Enabled) continue;

                string sParam = numericParam.Caption;
                string sValue = numericParam.ValueToString;
                var pointParam = new PointF(fTabParam, iVPosition);
                var pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                var pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                var pointValue = new PointF(fTabValue, iVPosition);
                var sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                var rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }

            // Check Params
            foreach (CheckParam checkParam in _strategy.Slot[iSlot].IndParam.CheckParam)
            {
                if (!checkParam.Enabled) continue;

                string sParam = checkParam.Caption;
                string sValue = checkParam.Checked ? "Yes" : "No";
                var pointParam = new PointF(fTabParam, iVPosition);
                var pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                var pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                var pointValue = new PointF(fTabValue, iVPosition);
                var sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                var rectfValue = new RectangleF(pointValue, sizefValue);

                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;
            }
        }

        /// <summary>
        /// Panel properties Paint
        /// </summary>
        private void PnlPropertiesPaint(object sender, PaintEventArgs e)
        {
            var pnl = (Panel) sender;
            Graphics g = e.Graphics;
            int iWidth = pnl.ClientSize.Width;

            Color colorCaptionBack = LayoutColors.ColorSlotCaptionBackAveraging;
            Color colorCaptionText = LayoutColors.ColorSlotCaptionText;
            Color colorBackground = LayoutColors.ColorSlotBackground;
            Color colorLogicText = LayoutColors.ColorSlotLogicText;
            Color colorDash = LayoutColors.ColorSlotDash;

            // Caption
            string stringCaptionText = Language.T("Strategy Properties");
            var fontCaptionText = new Font(Font.FontFamily, 9);
            float fCaptionHeight = Math.Max(fontCaptionText.Height, 18);
            float fCaptionWidth = iWidth;
            Brush brushCaptionText = new SolidBrush(colorCaptionText);
            var stringFormatCaption = new StringFormat
                                          {
                                              LineAlignment = StringAlignment.Center,
                                              Trimming = StringTrimming.EllipsisCharacter,
                                              FormatFlags = StringFormatFlags.NoWrap,
                                              Alignment = StringAlignment.Center
                                          };

            var rectfCaption = new RectangleF(0, 0, fCaptionWidth, fCaptionHeight);
            Data.GradientPaint(g, rectfCaption, colorCaptionBack, LayoutColors.DepthCaption);
            g.DrawString(stringCaptionText, fontCaptionText, brushCaptionText, rectfCaption, stringFormatCaption);

            // Border
            var penBorder = new Pen(Data.ColorChanage(colorCaptionBack, -LayoutColors.DepthCaption), Border);
            g.DrawLine(penBorder, 1, fCaptionHeight, 1, pnl.Height);
            g.DrawLine(penBorder, pnl.Width - Border + 1, fCaptionHeight, pnl.Width - Border + 1, pnl.Height);
            g.DrawLine(penBorder, 0, pnl.Height - Border + 1, pnl.Width, pnl.Height - Border + 1);

            // Paint the panel's background
            var rectfPanel = new RectangleF(Border, fCaptionHeight, pnl.Width - 2*Border,
                                            pnl.Height - fCaptionHeight - Border);
            Data.GradientPaint(g, rectfPanel, colorBackground, LayoutColors.DepthControl);

            int iVPosition = (int) fCaptionHeight + 2;

            var stringFormat = new StringFormat
                                   {Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap};

            var fontParam = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            var fontValue = new Font(Font.FontFamily, 9f, FontStyle.Regular);
            Brush brushParam = new SolidBrush(colorLogicText);
            Brush brushValue = new SolidBrush(colorLogicText);
            var penDash = new Pen(colorDash);

            string strPermaSL = _strategy.UsePermanentSL
                                    ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") +
                                      _strategy.PermanentSL.ToString(CultureInfo.InvariantCulture)
                                    : Language.T("None");
            string strPermaTP = _strategy.UsePermanentTP
                                    ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") +
                                      _strategy.PermanentTP.ToString(CultureInfo.InvariantCulture)
                                    : Language.T("None");
            string strBreakEven = _strategy.UseBreakEven
                                      ? _strategy.BreakEven.ToString(CultureInfo.InvariantCulture)
                                      : Language.T("None");

            if (_slotMinMidMax == SlotSizeMinMidMax.Min)
            {
                string sParam = Language.T(_strategy.SameSignalAction.ToString()) + "; " +
                                Language.T(_strategy.OppSignalAction.ToString()) + "; " +
                                "SL-" + strPermaSL + "; " +
                                "TP-" + strPermaTP + "; " +
                                "BE-" + strBreakEven;

                SizeF sizeParam = g.MeasureString(sParam, fontParam);
                float fMaxParamWidth = sizeParam.Width;

                // Padding Param Padding Dash Padding Value Padding 
                float fPadding = Space;
                float fNecessaryWidth = 2*fPadding + fMaxParamWidth;

                fPadding = iWidth > fNecessaryWidth ? Math.Max((pnl.ClientSize.Width - fMaxParamWidth)/2, fPadding) : 2;

                float fTabParam = fPadding;

                var pointParam = new PointF(fTabParam, iVPosition);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
            }
            else
            {
                // Find Maximum width of the strings
                var asParams = new[]
                                   {
                                       Language.T("Same direction signal"),
                                       Language.T("Opposite direction signal"),
                                       Language.T("Permanent Stop Loss"),
                                       Language.T("Permanent Take Profit"),
                                       Language.T("Break Even")
                                   };

                var asValues = new[]
                                   {
                                       Language.T(_strategy.SameSignalAction.ToString()),
                                       Language.T(_strategy.OppSignalAction.ToString()),
                                       strPermaSL,
                                       strPermaTP,
                                       strBreakEven
                                   };

                float fMaxParamWidth = 0;
                foreach (string param in asParams)
                {
                    if (g.MeasureString(param, fontParam).Width > fMaxParamWidth)
                        fMaxParamWidth = g.MeasureString(param, fontParam).Width;
                }

                float fMaxValueWidth = 0;
                foreach (string value in asValues)
                {
                    if (g.MeasureString(value, fontParam).Width > fMaxValueWidth)
                        fMaxValueWidth = g.MeasureString(value, fontParam).Width;
                }

                // Padding Param Padding Dash Padding Value Padding 
                float fPadding = Space;
                const float fDashWidth = 5;
                float fNecessaryWidth = 4*fPadding + fMaxParamWidth + fMaxValueWidth + fDashWidth;

                if (iWidth > fNecessaryWidth)
                {
                    // 2*Padding Param Padding Dash Padding Value 2*Padding 
                    fPadding = Math.Max((pnl.ClientSize.Width - fMaxParamWidth - fMaxValueWidth - fDashWidth)/6,
                                        fPadding);
                }
                else
                {
                    fPadding = 2;
                }

                float fTabParam = 2*fPadding;
                float fTabDash = fTabParam + fMaxParamWidth + fPadding;
                float fTabValue = fTabDash + fDashWidth + fPadding;

                // Same direction
                string sParam = Language.T("Same direction signal");
                string sValue = Language.T(_strategy.SameSignalAction.ToString());
                var pointParam = new PointF(fTabParam, iVPosition);
                var pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                var pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                var pointValue = new PointF(fTabValue, iVPosition);
                var sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                var rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Opposite direction
                sParam = Language.T("Opposite direction signal");
                sValue = Language.T(_strategy.OppSignalAction.ToString());
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Permanent Stop Loss
                sParam = Language.T("Permanent Stop Loss");
                sValue = strPermaSL;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height + 2;

                // Permanent Take Profit
                sParam = Language.T("Permanent Take Profit");
                sValue = strPermaTP;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
                iVPosition += fontValue.Height;

                // Break Even
                sParam = Language.T("Break Even");
                sValue = strBreakEven;
                pointParam = new PointF(fTabParam, iVPosition);
                pointDash1 = new PointF(fTabDash, iVPosition + fontParam.Height/2 + 2);
                pointDash2 = new PointF(fTabDash + fDashWidth, iVPosition + fontParam.Height/2 + 2);
                pointValue = new PointF(fTabValue, iVPosition);
                sizefValue = new SizeF(Math.Max(iWidth - fTabValue, 0), fontValue.Height + 2);
                rectfValue = new RectangleF(pointValue, sizefValue);
                g.DrawString(sParam, fontParam, brushParam, pointParam);
                g.DrawLine(penDash, pointDash1, pointDash2);
                g.DrawString(sValue, fontValue, brushValue, rectfValue, stringFormat);
            }
        }

        /// <summary>
        /// Shows Closing Filter Help.
        /// </summary>
        private void BtnClosingFilterHelp_Click(object sender, EventArgs e)
        {
            const string text =
                "You can use Closing Logic Conditions only if the Closing Point of the Position slot contains one of the following indicators:";
            string inicators = Environment.NewLine;
            foreach (string indicator in IndicatorStore.ClosingIndicatorsWithClosingFilters)
                inicators += " - " + indicator + Environment.NewLine;
            MessageBox.Show(Language.T(text) + inicators, Language.T("Closing Logic Condition"), MessageBoxButtons.OK,
                            MessageBoxIcon.Asterisk);
        }

        /// <summary>
        /// Arranges the controls after resizing
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            _flowLayoutStrategy.SuspendLayout();
            ArrangeStrategyControls();
            _flowLayoutStrategy.ResumeLayout();
        }
    }
}