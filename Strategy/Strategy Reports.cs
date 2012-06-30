// Strategy Reports
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2006 - 2012 Miroslav Popov - All rights reserved.
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Forex_Strategy_Trader
{
    public partial class Strategy
    {
        /// <summary>
        /// Saves the strategy in BBCode format.
        /// </summary>
        public string GenerateBBCode()
        {
            string stage = String.Empty;
            if (Data.IsProgramBeta)
                stage = " " + Language.T("Beta");

            string strBBCode = "";
            string nl = Environment.NewLine;
            string nl2 = Environment.NewLine + Environment.NewLine;

            strBBCode += "Strategy name: [b]" + StrategyName + "[/b]" + nl;
            strBBCode += Data.ProgramName + " v" + Data.ProgramVersion + stage + nl;
            strBBCode += "Exported on: " + DateTime.Now + nl;
            strBBCode += nl;

            // Description
            strBBCode += "Description:" + nl;

            if (Description != "")
            {
                if (!Data.IsStrDescriptionRelevant())
                    strBBCode += "(This description might be outdated!)" + nl2;

                strBBCode += Description + nl2;
            }
            else
                strBBCode += "   None." + nl2;

            strBBCode += UseAccountPercentEntry ? "Use account % for margin round to whole lots" + nl : "";
            string tradingUnit = UseAccountPercentEntry ? "% of the account for margin" : "";
            strBBCode += "Maximum open lots: " + MaxOpenLots.ToString("F2") + nl;
            strBBCode += "Entry lots: " + EntryLots.ToString("F2") + tradingUnit + nl;
            if (SameSignalAction == SameDirSignalAction.Add || SameSignalAction == SameDirSignalAction.Winner)
                strBBCode += "Adding lots: " + AddingLots.ToString("F2") + tradingUnit + nl;
            if (OppSignalAction == OppositeDirSignalAction.Reduce)
                strBBCode += "Reducing lots: " + ReducingLots.ToString("F2") + tradingUnit + nl;
            if (UseMartingale)
                strBBCode += "Martingale money management multiplier: " + MartingaleMultiplier.ToString("F2") + nl;

            strBBCode += "[b][color=#966][Strategy Properties][/color][/b]" + nl;
            if (SameSignalAction == SameDirSignalAction.Add)
                strBBCode += "     A same direction signal - Adds to the position" + nl;
            else if (SameSignalAction == SameDirSignalAction.Winner)
                strBBCode += "     A same direction signal - Adds to a winning position" + nl;
            else if (SameSignalAction == SameDirSignalAction.Nothing)
                strBBCode += "     A same direction signal - Does nothing" + nl;

            if (OppSignalAction == OppositeDirSignalAction.Close)
                strBBCode += "     An opposite direction signal - Closes the position" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                strBBCode += "     An opposite direction signal - Reduces the position" + nl;
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                strBBCode += "     An opposite direction signal - Reverses the position" + nl;
            else
                strBBCode += "     An opposite direction signal - Does nothing" + nl;

            strBBCode += "     Permanent Stop Loss - " + (Data.Strategy.UsePermanentSL ? (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentSL.ToString(CultureInfo.InvariantCulture) : "None") + "" + nl;
            strBBCode += "     Permanent Take Profit - " + (Data.Strategy.UsePermanentTP ? (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? "(Abs) " : "") + Data.Strategy.PermanentTP.ToString(CultureInfo.InvariantCulture) : "None") + "" + nl;
            strBBCode += "     Break Even - " + (Data.Strategy.UseBreakEven ? Data.Strategy.BreakEven.ToString(CultureInfo.InvariantCulture) : "None") + "" + nl;
            strBBCode += nl;

            // Add the slots.
            foreach (IndicatorSlot indSlot in Slot)
            {
                string slotTypeName;
                string slotColor;
                switch (indSlot.SlotType)
                {
                    case SlotTypes.Open:
                        slotTypeName = "Opening Point of the Position";
                        slotColor = "#693";
                        break;
                    case SlotTypes.OpenFilter:
                        slotTypeName = "Opening Logic Condition";
                        slotColor = "#699";
                        break;
                    case SlotTypes.Close:
                        slotTypeName = "Closing Point of the Position";
                        slotColor = "#d63";
                        break;
                    case SlotTypes.CloseFilter:
                        slotTypeName = "Closing Logic Condition";
                        slotColor = "#d99";
                        break;
                    default:
                        slotTypeName = "";
                        slotColor = "#000";
                        break;
                }

                strBBCode += "[b][color=" + slotColor + "][" + slotTypeName + "][/color][/b]" + nl;
                strBBCode += "     [b][color=blue]" + indSlot.IndicatorName + "[/color][/b]" + nl;

                // Add the list params.
                foreach (ListParam listParam in indSlot.IndParam.ListParam)
                    if (listParam.Enabled)
                    {
                        if (listParam.Caption == "Logic")
                            strBBCode += "     [b][color=#066]" +
                                (Configs.UseLogicalGroups && (indSlot.SlotType == SlotTypes.OpenFilter || indSlot.SlotType == SlotTypes.CloseFilter) ?
                                "[" + (indSlot.LogicalGroup.Length == 1 ? " " + indSlot.LogicalGroup + " " : indSlot.LogicalGroup) + "]   " : "") + listParam.Text + "[/color][/b]" + nl;
                        else
                            strBBCode += "     " + listParam.Caption + "  -  " + listParam.Text + nl;
                    }

                // Add the num params.
                foreach (NumericParam numParam in indSlot.IndParam.NumParam)
                    if (numParam.Enabled)
                        strBBCode += "     " + numParam.Caption + "  -  " + numParam.ValueToString + nl;

                // Add the check params.
                foreach (CheckParam checkParam in indSlot.IndParam.CheckParam)
                    if (checkParam.Enabled)
                        strBBCode += "     " + checkParam.Caption + "  -  " + (checkParam.Checked ? "Yes" : "No") + nl;

                strBBCode += nl;
            }

            return strBBCode;
        }

        /// <summary>
        /// Generate Overview in HTML code
        /// </summary>
        /// <returns>the HTML code</returns>
        public string GenerateHTMLOverview()
        {
            var sb = new StringBuilder();
            // Header
            sb.AppendLine("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.1//EN\" \"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd\">");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\" xml:lang=\"en\">");
            sb.AppendLine("<head><meta http-equiv=\"content-type\" content=\"text/html;charset=utf-8\" />");
            sb.AppendLine("<title>" + StrategyName + "</title>");
            sb.AppendLine("<style type=\"text/css\">");
            sb.AppendLine("body {padding: 0 10px 10px 10px; margin: 0px; background-color: #fff; color: #003; font-size: 16px}");
            sb.AppendLine(".content h1 {font-size: 1.4em;}");
            sb.AppendLine(".content h2 {font-size: 1.2em;}");
            sb.AppendLine(".content h3 {font-size: 1em;}");
            sb.AppendLine(".content p { }");
            sb.AppendLine(".content p.fsb_go_top {text-align: center;}");
            sb.AppendLine(".fsb_strategy_slot {font-family: sans-serif; width: 30em; margin: 2px auto; text-align: center; background-color: #f3ffff; }");
            sb.AppendLine(".fsb_strategy_slot table tr td {text-align: left; color: #033; font-size: 75%;}");
            sb.AppendLine(".fsb_properties_slot {color: #fff; padding: 2px 0px; background: #966; }");
            sb.AppendLine(".fsb_open_slot {color: #fff; padding: 2px 0; background: #693; }");
            sb.AppendLine(".fsb_close_slot {color: #fff; padding: 2px 0; background: #d63; }");
            sb.AppendLine(".fsb_open_filter_slot {color: #fff; padding: 2px 0; background: #699;}");
            sb.AppendLine(".fsb_close_filter_slot {color: #fff; padding: 2px 0; background: #d99;}");
            sb.AppendLine(".fsb_str_indicator {padding: 5px 0; color: #6090c0;}");
            sb.AppendLine(".fsb_str_logic {padding-bottom: 5px; color: #066;}");
            sb.AppendLine(".fsb_table {margin: 0 auto; border: 2px solid #003; border-collapse: collapse;}");
            sb.AppendLine(".fsb_table th {border: 1px solid #006; text-align: center; background: #ccf; border-bottom-width: 2px;}");
            sb.AppendLine(".fsb_table td {border: 1px solid #006;}");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class=\"content\" id=\"fsb_header\">");

            // Description
            sb.AppendLine("<h2 id=\"description\">" + Language.T("Description") + "</h2>");
            if (Description != String.Empty)
            {
                string strStrategyDescription = Description.Replace(Environment.NewLine, "<br />");
                strStrategyDescription = strStrategyDescription.Replace("&", "&amp;");
                strStrategyDescription = strStrategyDescription.Replace("\"", "&quot;");

                if (!Data.IsStrDescriptionRelevant())
                    sb.AppendLine("<p style=\"color: #a00\">" + "(" + Language.T("This description might be outdated!") + ")" + "</p>");

                sb.AppendLine("<p>" + strStrategyDescription + "</p>");
            }
            else
                sb.AppendLine("<p>" + Language.T("None") + ".</p>");

            // Strategy Logic
            sb.AppendLine();
            sb.AppendLine("<h2 id=\"logic\">" + Language.T("Logic") + "</h2>");

            // Opening
            sb.AppendLine("<h3>" + Language.T("Opening (Entry Signal)") + "</h3>");
            sb.AppendLine(OpeningLogicHTMLReport().ToString());

            // Closing
            sb.AppendLine("<h3>" + Language.T("Closing (Exit Signal)") + "</h3>");
            sb.AppendLine(ClosingLogicHTMLReport().ToString());

            // Averaging
            sb.AppendLine("<h3>" + Language.T("Handling of Additional Entry Signals") + "**</h3>");
            sb.AppendLine(AveragingHTMLReport().ToString());

            // Trading Sizes
            sb.AppendLine("<h3>" + Language.T("Trading Size") + "</h3>");
            sb.AppendLine(TradingSizeHTMLReport().ToString());

            // Protection
            sb.AppendLine("<h3>" + Language.T("Permanent Protection") + "</h3>");
            if (!Data.Strategy.UsePermanentSL)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not provide a permanent loss limitation.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Stop Loss limits the loss of a position to") + (Data.Strategy.PermanentSLType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentSL);
                sb.AppendLine(Language.T("pips per open lot (plus the charged spread and rollover).") + "</p>");
            }

            if (!Data.Strategy.UsePermanentTP)
            {
                sb.AppendLine("<p>" + Language.T("The strategy does not use a Permanent Take Profit.") + "</p>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("The Permanent Take Profit closes a position at") + (Data.Strategy.PermanentTPType == PermanentProtectionType.Absolute ? " (Abs) " : " ") + Data.Strategy.PermanentTP);
                sb.AppendLine(Language.T("pips profit.") + "</p>");
            }

            if (Data.Strategy.UseBreakEven)
            {
                sb.AppendLine("<p>" + Language.T("The position's Stop Loss will be set to Break Even price when the profit reaches") + " " + Data.Strategy.BreakEven);
                sb.AppendLine(Language.T("pips") + "." + "</p>");
            }

            sb.AppendLine("<p>--------------<br />");
            sb.AppendLine("* " + Language.T("Use the indicator value from the previous bar for all asterisk-marked indicators!") + "<br />");
            sb.AppendLine("** " + Language.T("The averaging rules apply to the entry signals only. Exit signals close a position. They cannot open, add or reduce one."));
            sb.AppendLine("</p>");

            // Footer
            sb.AppendLine("</div></body></html>");

            return sb.ToString();
        }

        /// <summary>
        /// Generates a HTML report about the opening logic.
        /// </summary>
        StringBuilder OpeningLogicHTMLReport()
        {
            var sb = new StringBuilder();
            string indicatorName = Data.Strategy.Slot[0].IndicatorName;

            Indicator indOpen = Indicator_Store.ConstructIndicator(indicatorName, SlotTypes.Open);
            indOpen.IndParam = Data.Strategy.Slot[0].IndParam;
            indOpen.SetDescription(SlotTypes.Open);

            // Logical groups of the opening conditions.
            var opengroups = new List<string>();
            for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
            {
                string group = Data.Strategy.Slot[slot].LogicalGroup;
                if (!opengroups.Contains(group) && group != "All")
                    opengroups.Add(group); // Adds all groups except "All"
            }
            if (opengroups.Count == 0 && Data.Strategy.OpenFilters > 0)
                opengroups.Add("All"); // If all the slots are in "All" group, adds "All" to the list.

            // Long position
            string openLong = "<p>";

            if (Data.Strategy.SameSignalAction == SameDirSignalAction.Add)
                openLong = Language.T("Open a new long position or add to an existing position");
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                openLong = Language.T("Open a new long position or add to a winning position");
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                openLong = Language.T("Open a new long position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                openLong += " " + Language.T("or close a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                openLong += " " + Language.T("or reduce a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                openLong += " " + Language.T("or reverse a short position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                openLong += "";

            openLong += " " + indOpen.EntryPointLongDescription;

            if (Data.Strategy.OpenFilters == 0)
                openLong += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                openLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openLong += " " + Language.T("when") + ":</p>";
            else
                openLong += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openLong);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterLongDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }

                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            // Short position
            string openShort = "<p>";

            if (Data.Strategy.SameSignalAction == SameDirSignalAction.Add)
                openShort = Language.T("Open a new short position or add to an existing position");
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                openShort = Language.T("Open a new short position or add to a winning position");
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                openShort = Language.T("Open a new short position");

            if (OppSignalAction == OppositeDirSignalAction.Close)
                openShort += " " + Language.T("or close a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reduce)
                openShort += " " + Language.T("or reduce a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Reverse)
                openShort += " " + Language.T("or reverse a long position");
            else if (OppSignalAction == OppositeDirSignalAction.Nothing)
                openShort += "";

            openShort += " " + indOpen.EntryPointShortDescription;

            if (Data.Strategy.OpenFilters == 0)
                openShort += ".</p>";
            else if (Data.Strategy.OpenFilters == 1)
                openShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (opengroups.Count > 1)
                openShort += " " + Language.T("when") + ":</p>";
            else
                openShort += " " + Language.T("when all the following logic conditions are satisfied") + ":</p>";

            sb.AppendLine(openShort);

            // Open Filters
            if (Data.Strategy.OpenFilters > 0)
            {
                int groupnumb = 1;
                if (opengroups.Count > 1)
                    sb.AppendLine("<ul>");

                foreach (string group in opengroups)
                {
                    if (opengroups.Count > 1)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        groupnumb++;
                    }

                    sb.AppendLine("<ul>");
                    int indInGroup = 0;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "All")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = 1; slot <= Data.Strategy.OpenFilters; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "All")
                            continue;

                        Indicator indOpenFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.OpenFilter);
                        indOpenFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indOpenFilter.SetDescription(SlotTypes.OpenFilter);

                        if (indnumb < indInGroup)
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + "; " + Language.T("and") + "</li>");
                        else
                            sb.AppendLine("<li>" + indOpenFilter.EntryFilterShortDescription + ".</li>");

                        indnumb++;
                    }
                    sb.AppendLine("</ul>");

                    if (opengroups.Count > 1)
                        sb.AppendLine("</li>");
                }
                if (opengroups.Count > 1)
                    sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the closing logic.
        /// </summary>
        StringBuilder ClosingLogicHTMLReport()
        {
            var sb = new StringBuilder();

            int closingSlotNmb = Data.Strategy.CloseSlot;
            string indicatorName = Data.Strategy.Slot[closingSlotNmb].IndicatorName;

            Indicator indClose = Indicator_Store.ConstructIndicator(indicatorName, SlotTypes.Close);
            indClose.IndParam = Data.Strategy.Slot[closingSlotNmb].IndParam;
            indClose.SetDescription(SlotTypes.Close);

            bool isGroups = false;
            var closegroups = new List<string>();

            if (Data.Strategy.CloseFilters > 0)
                foreach (IndicatorSlot slot in Data.Strategy.Slot)
                {
                    if (slot.SlotType == SlotTypes.CloseFilter)
                    {
                        if (slot.LogicalGroup == "all" && Data.Strategy.CloseFilters > 1)
                            isGroups = true;

                        if (closegroups.Contains(slot.LogicalGroup))
                            isGroups = true;
                        else if (slot.LogicalGroup != "all")
                            closegroups.Add(slot.LogicalGroup);
                    }
                }

            if (closegroups.Count == 0 && Data.Strategy.CloseFilters > 0)
                closegroups.Add("all"); // If all the slots are in "all" group, adds "all" to the list.


            // Long position
            string closeLong = "<p>" + Language.T("Close an existing long position") + " " + indClose.ExitPointLongDescription;

            if (Data.Strategy.CloseFilters == 0)
                closeLong += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                closeLong += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeLong += " " + Language.T("when") + ":</p>";
            else
                closeLong += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(closeLong);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterLongDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            // Short position
            string closeShort = "<p>" + Language.T("Close an existing short position") + " " + indClose.ExitPointShortDescription;

            if (Data.Strategy.CloseFilters == 0)
                closeShort += ".</p>";
            else if (Data.Strategy.CloseFilters == 1)
                closeShort += " " + Language.T("when the following logic condition is satisfied") + ":</p>";
            else if (isGroups)
                closeShort += " " + Language.T("when") + ":</p>";
            else
                closeShort += " " + Language.T("when at least one of the following logic conditions is satisfied") + ":</p>";

            sb.AppendLine(closeShort);

            // Close Filters
            if (Data.Strategy.CloseFilters > 0)
            {
                int groupnumb = 1;
                sb.AppendLine("<ul>");

                foreach (string group in closegroups)
                {
                    if (isGroups)
                    {
                        sb.AppendLine("<li>" + (groupnumb == 1 ? "" : Language.T("or") + " ") + Language.T("logical group [#] is satisfied").Replace("#", group) + ":");
                        sb.AppendLine("<ul>");
                        groupnumb++;
                    }

                    int indInGroup = 0;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                        if (Data.Strategy.Slot[slot].LogicalGroup == group || Data.Strategy.Slot[slot].LogicalGroup == "all")
                            indInGroup++;

                    int indnumb = 1;
                    for (int slot = closingSlotNmb + 1; slot < Data.Strategy.Slots; slot++)
                    {
                        if (Data.Strategy.Slot[slot].LogicalGroup != group && Data.Strategy.Slot[slot].LogicalGroup != "all")
                            continue;

                        Indicator indCloseFilter = Indicator_Store.ConstructIndicator(Data.Strategy.Slot[slot].IndicatorName, SlotTypes.CloseFilter);
                        indCloseFilter.IndParam = Data.Strategy.Slot[slot].IndParam;
                        indCloseFilter.SetDescription(SlotTypes.CloseFilter);

                        if (isGroups)
                        {
                            if (indnumb < indInGroup)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("and") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        else
                        {
                            if (slot < Data.Strategy.Slots - 1)
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + "; " + Language.T("or") + "</li>");
                            else
                                sb.AppendLine("<li>" + indCloseFilter.ExitFilterShortDescription + ".</li>");
                        }
                        indnumb++;
                    }

                    if (isGroups)
                    {
                        sb.AppendLine("</ul>");
                        sb.AppendLine("</li>");
                    }
                }

                sb.AppendLine("</ul>");
            }

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the averaging logic.
        /// </summary>
        StringBuilder AveragingHTMLReport()
        {
            var sb = new StringBuilder();

            // Same direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the direction of the present position:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.SameSignalAction == SameDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No averaging is allowed. Cancel any additional orders which are in the same direction."));
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Winner)
                sb.AppendLine(Language.T("Add to a winning position but not to a losing one. If the position is at a loss, cancel the additional entry order. Do not exceed the maximum allowed number of lots to open."));
            else if (Data.Strategy.SameSignalAction == SameDirSignalAction.Add)
                sb.AppendLine(Language.T("Add to the position no matter if it is at a profit or loss. Do not exceed the maximum allowed number of lots to open."));
            sb.AppendLine("</li></ul>");

            // Opposite direction
            sb.AppendLine("<p>" + Language.T("Entry signal in the opposite direction:") + "</p>");

            sb.AppendLine("<ul><li>");
            if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Nothing)
                sb.AppendLine(Language.T("No modification of the present position is allowed. Cancel any additional orders which are in the opposite direction."));
            else if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reduce)
                sb.AppendLine(Language.T("Reduce the present position. If its amount is lower than or equal to the specified reducing lots, close it."));
            else if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Close)
                sb.AppendLine(Language.T("Close the present position regardless of its amount or result. Do not open a new position until the next entry signal has been raised."));
            else if (Data.Strategy.OppSignalAction == OppositeDirSignalAction.Reverse)
                sb.AppendLine(Language.T("Close the existing position and open a new one in the opposite direction using the entry rules."));
            sb.AppendLine("</li></ul>");

            return sb;
        }

        /// <summary>
        /// Generates a HTML report about the trading sizes.
        /// </summary>
        StringBuilder TradingSizeHTMLReport()
        {
            var sb = new StringBuilder();

            if (UseAccountPercentEntry)
            {
                sb.AppendLine("<p>" + Language.T("Trade percent of your account.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + EntryLots + Language.T("% of the account equity") + ".</li>");
                if (SameSignalAction == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (SameSignalAction == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + AddingLots + Language.T("% of the account equity") + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (OppSignalAction == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + ReducingLots + Language.T("% of the account equity") + ".</li>");
                if (OppSignalAction == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + EntryLots + Language.T("% of the account equity") + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }
            else
            {
                sb.AppendLine("<p>" + Language.T("Always trade a constant number of lots.") + "</p>");

                sb.AppendLine("<ul>");
                sb.AppendLine("<li>" + Language.T("Opening of a new position") + " - " + Plural("lot", EntryLots) + ".</li>");
                if (SameSignalAction == SameDirSignalAction.Winner)
                    sb.AppendLine("<li>" + Language.T("Adding to a winning position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (SameSignalAction == SameDirSignalAction.Add)
                    sb.AppendLine("<li>" + Language.T("Adding to a position") + " - " + Plural("lot", AddingLots) + ". " + Language.T("Do not open more than") + " " + Plural("lot", MaxOpenLots) + ".</li>");
                if (OppSignalAction == OppositeDirSignalAction.Reduce)
                    sb.AppendLine("<li>" + Language.T("Reducing a position") + " - " + Plural("lot", ReducingLots) + " " + Language.T("from the current position.") + "</li>");
                if (OppSignalAction == OppositeDirSignalAction.Reverse)
                    sb.AppendLine("<li>" + Language.T("Reversing a position") + " - " + Plural("lot", EntryLots) + " " + Language.T("in the opposite direction.") + "</li>");
                sb.AppendLine("</ul>");
            }

            if (UseMartingale)
            {
                sb.AppendLine("<p>" + Language.T("Apply Martingale money management system with multiplier of") + " " + MartingaleMultiplier.ToString("F2") + "." + "</p>");
            }

            return sb;
        }

        /// <summary>
        /// Represents the strategy in a readable form.
        /// </summary>
        public override string ToString()
        {
            string nl = Environment.NewLine;
            string str = String.Empty;
            str += "Strategy Name - " + StrategyName + nl;
            str += "Symbol - " + Symbol + nl;
            str += "Period - " + DataPeriod + nl;
            str += "Same dir signal - " + SameSignalAction + nl;
            str += "Opposite dir signal - " + OppSignalAction + nl;
            str += "Use account % entry - " + UseAccountPercentEntry + nl;
            str += "Max open lots - " + MaxOpenLots + nl;
            str += "Entry lots - " + EntryLots + nl;
            str += "Adding lots - " + AddingLots + nl;
            str += "Reducing lots - " + ReducingLots + nl;
            str += "Use Martingale MM - " + UseMartingale + nl;
            str += "Martingale multiplier - " + MartingaleMultiplier + nl;
            str += "Use Permanent S/L - " + UsePermanentSL + nl;
            str += "Permanent S/L - " + PermanentSLType + " " + PermanentSL + nl;
            str += "Use Permanent T/P - " + UsePermanentTP + nl;
            str += "Permanent T/P - " + PermanentTPType + " " + PermanentTP + nl;
            str += "Use Break Even - " + UseBreakEven + nl;
            str += "Break Even - " + BreakEven + nl + nl;
            str += "Description:" + nl + Description + nl + nl;

            for (int slot = 0; slot < Slots; slot++)
            {
                str += Slot[slot].SlotType + nl;
                str += Slot[slot].IndicatorName + nl;
                str += Slot[slot].IndParam + nl + nl;
            }

            return str;
        }

        /// <summary>
        /// Appends "s" when plural
        /// </summary>
        static string Plural(string unit, double number)
        {
            if (Math.Abs(number - 1) > 0.0000001 && Math.Abs(number + 1) > 0.0000001)
                unit += "s";

            return number + " " + Language.T(unit);
        }
    }
}