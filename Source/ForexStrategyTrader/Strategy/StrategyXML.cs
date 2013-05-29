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
using System.Globalization;
using System.Windows.Forms;
using System.Xml;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Exceptions;

namespace ForexStrategyBuilder
{
    public class StrategyXML
    {
        /// <summary>
        ///     Represents the Strategy as a XmlDocument.
        /// </summary>
        public static XmlDocument CreateStrategyXmlDoc(Strategy strategy)
        {
            // Create the XmlDocument.
            var xmlDocStrategy = new XmlDocument();
            xmlDocStrategy.LoadXml("<strategy></strategy>");

            //Create the XML declaration.
            XmlDeclaration xmldecl = xmlDocStrategy.CreateXmlDeclaration("1.0", null, null);

            //Add new node to the document.
            XmlElement root = xmlDocStrategy.DocumentElement;
            xmlDocStrategy.InsertBefore(xmldecl, root);

            AppendStringElement(xmlDocStrategy, "programName", Data.ProgramName);
            AppendStringElement(xmlDocStrategy, "programVersion", Data.ProgramVersion);
            AppendStringElement(xmlDocStrategy, "strategyName", strategy.StrategyName);
            AppendStringElement(xmlDocStrategy, "instrumentSymbol", strategy.Symbol);
            AppendStringElement(xmlDocStrategy, "instrumentPeriod", (int)strategy.DataPeriod);
            AppendStringElement(xmlDocStrategy, "sameDirSignalAction", strategy.SameSignalAction.ToString());
            AppendStringElement(xmlDocStrategy, "oppDirSignalAction", strategy.OppSignalAction.ToString());

            // Add the Permanent Stop Loss
            XmlElement newElem = xmlDocStrategy.CreateElement("permanentStopLoss");
            newElem.InnerText = strategy.PermanentSL.ToString(CultureInfo.InvariantCulture);
            newElem.SetAttribute("usePermanentSL", strategy.UsePermanentSL.ToString(CultureInfo.InvariantCulture));
            newElem.SetAttribute("permanentSLType", strategy.PermanentSLType.ToString());
            if (xmlDocStrategy.DocumentElement != null) xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Permanent Take Profit
            newElem = xmlDocStrategy.CreateElement("permanentTakeProfit");
            newElem.InnerText = strategy.PermanentTP.ToString(CultureInfo.InvariantCulture);
            newElem.SetAttribute("usePermanentTP", strategy.UsePermanentTP.ToString(CultureInfo.InvariantCulture));
            newElem.SetAttribute("permanentTPType", strategy.PermanentTPType.ToString());
            if (xmlDocStrategy.DocumentElement != null) xmlDocStrategy.DocumentElement.AppendChild(newElem);

            // Add the Break Even
            newElem = xmlDocStrategy.CreateElement("breakEven");
            newElem.InnerText = strategy.BreakEven.ToString(CultureInfo.InvariantCulture);
            newElem.SetAttribute("useBreakEven", strategy.UseBreakEven.ToString(CultureInfo.InvariantCulture));
            if (xmlDocStrategy.DocumentElement != null) xmlDocStrategy.DocumentElement.AppendChild(newElem);

            AppendStringElement(xmlDocStrategy, "maxOpenLots", strategy.MaxOpenLots);
            AppendStringElement(xmlDocStrategy, "useAccountPercentEntry", strategy.UseAccountPercentEntry);
            AppendStringElement(xmlDocStrategy, "entryLots", strategy.EntryLots);
            AppendStringElement(xmlDocStrategy, "addingLots", strategy.AddingLots);
            AppendStringElement(xmlDocStrategy, "reducingLots", strategy.ReducingLots);
            AppendStringElement(xmlDocStrategy, "useMartingale", strategy.UseMartingale);
            AppendStringElement(xmlDocStrategy, "martingaleMultiplier", strategy.MartingaleMultiplier);
            AppendStringElement(xmlDocStrategy, "description", strategy.Description);

            // Add the slots.
            AppendStringElement(xmlDocStrategy, "openFilters", strategy.OpenFilters);
            AppendStringElement(xmlDocStrategy, "closeFilters", strategy.CloseFilters);
            for (int slot = 0; slot < strategy.Slots; slot++)
            {
                IndicatorSlot stratSlot = strategy.Slot[slot];
                SlotTypes slType = stratSlot.SlotType;

                // Add a slot element.
                XmlElement newSlot = xmlDocStrategy.CreateElement("slot");
                newSlot.SetAttribute("slotNumber", slot.ToString(CultureInfo.InvariantCulture));
                newSlot.SetAttribute("slotType", slType.ToString());

                if (slType == SlotTypes.OpenFilter || slType == SlotTypes.CloseFilter)
                    newSlot.SetAttribute("logicalGroup", stratSlot.LogicalGroup);

                // Add an element.
                newElem = xmlDocStrategy.CreateElement("indicatorName");
                newElem.InnerText = stratSlot.IndicatorName;
                newSlot.AppendChild(newElem);

                // Add the list parameters.
                for (int param = 0; param < stratSlot.IndParam.ListParam.Length; param++)
                {
                    if (!stratSlot.IndParam.ListParam[param].Enabled) continue;

                    // Add an element.
                    XmlElement newListElem = xmlDocStrategy.CreateElement("listParam");
                    newListElem.SetAttribute("paramNumber", param.ToString(CultureInfo.InvariantCulture));

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("caption");
                    newElem.InnerText = stratSlot.IndParam.ListParam[param].Caption;
                    newListElem.AppendChild(newElem);

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("index");
                    newElem.InnerText = stratSlot.IndParam.ListParam[param].Index.ToString(CultureInfo.InvariantCulture);
                    newListElem.AppendChild(newElem);

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("value");
                    newElem.InnerText = stratSlot.IndParam.ListParam[param].Text;
                    newListElem.AppendChild(newElem);

                    newSlot.AppendChild(newListElem);
                }

                // Add the num parameters.
                for (int param = 0; param < stratSlot.IndParam.NumParam.Length; param++)
                {
                    if (!stratSlot.IndParam.NumParam[param].Enabled) continue;

                    // Add an element.
                    XmlElement newNumElem = xmlDocStrategy.CreateElement("numParam");
                    newNumElem.SetAttribute("paramNumber", param.ToString(CultureInfo.InvariantCulture));

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("caption");
                    newElem.InnerText = stratSlot.IndParam.NumParam[param].Caption;
                    newNumElem.AppendChild(newElem);

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("value");
                    newElem.InnerText = stratSlot.IndParam.NumParam[param].ValueToString;
                    newNumElem.AppendChild(newElem);

                    newSlot.AppendChild(newNumElem);
                }

                // Add the check parameters.
                for (int param = 0; param < stratSlot.IndParam.CheckParam.Length; param++)
                {
                    if (!stratSlot.IndParam.CheckParam[param].Enabled) continue;

                    // Add an element.
                    XmlElement newCheckElem = xmlDocStrategy.CreateElement("checkParam");
                    newCheckElem.SetAttribute("paramNumber", param.ToString(CultureInfo.InvariantCulture));

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("caption");
                    newElem.InnerText = stratSlot.IndParam.CheckParam[param].Caption;
                    newCheckElem.AppendChild(newElem);

                    // Add an element.
                    newElem = xmlDocStrategy.CreateElement("value");
                    newElem.InnerText =
                        stratSlot.IndParam.CheckParam[param].Checked.ToString(CultureInfo.InvariantCulture);
                    newCheckElem.AppendChild(newElem);

                    newSlot.AppendChild(newCheckElem);
                }

                if (xmlDocStrategy.DocumentElement != null)
                    xmlDocStrategy.DocumentElement.AppendChild(newSlot);
            }

            return xmlDocStrategy;
        }


        /// <summary>
        ///     Pareses a strategy from a xml document.
        /// </summary>
        public Strategy ParseXmlStrategy(XmlDocument xmlDocStrategy)
        {
            // Read the number of slots
            int openFilters = int.Parse(xmlDocStrategy.GetElementsByTagName("openFilters")[0].InnerText);
            int closeFilters = int.Parse(xmlDocStrategy.GetElementsByTagName("closeFilters")[0].InnerText);

            // Create the strategy.
            // ReSharper disable UseObjectOrCollectionInitializer
            var tempStrategy = new Strategy(openFilters, closeFilters);
            // ReSharper restore UseObjectOrCollectionInitializer

            // Same and Opposite direction Actions
            tempStrategy.SameSignalAction =
                (SameDirSignalAction)
                Enum.Parse(typeof(SameDirSignalAction),
                           xmlDocStrategy.GetElementsByTagName("sameDirSignalAction")[0].InnerText);
            tempStrategy.OppSignalAction =
                (OppositeDirSignalAction)
                Enum.Parse(typeof(OppositeDirSignalAction),
                           xmlDocStrategy.GetElementsByTagName("oppDirSignalAction")[0].InnerText);

            // Market
            tempStrategy.Symbol = xmlDocStrategy.GetElementsByTagName("instrumentSymbol")[0].InnerText;
            string period = xmlDocStrategy.GetElementsByTagName("instrumentPeriod")[0].InnerText;
            tempStrategy.DataPeriod = ImportDataPeriod(period);

            // Permanent Stop Loss
            tempStrategy.PermanentSL =
                Math.Abs(int.Parse(xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].InnerText));
            // Math.Abs() removes the negative sign from previous versions.
            XmlAttributeCollection xmlAttributeCollection =
                xmlDocStrategy.GetElementsByTagName("permanentStopLoss")[0].Attributes;
            if (xmlAttributeCollection != null)
            {
                tempStrategy.UsePermanentSL = bool.Parse(xmlAttributeCollection["usePermanentSL"].InnerText);
                try
                {
                    tempStrategy.PermanentSLType =
                        (PermanentProtectionType)
                        Enum.Parse(typeof(PermanentProtectionType), xmlAttributeCollection["permanentSLType"].InnerText);
                }
                catch
                {
                    tempStrategy.PermanentSLType = PermanentProtectionType.Relative;
                }
            }

            // Permanent Take Profit
            tempStrategy.PermanentTP = int.Parse(xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].InnerText);
            XmlAttributeCollection attributeCollection =
                xmlDocStrategy.GetElementsByTagName("permanentTakeProfit")[0].Attributes;
            if (attributeCollection != null)
            {
                tempStrategy.UsePermanentTP = bool.Parse(attributeCollection["usePermanentTP"].InnerText);
                try
                {
                    tempStrategy.PermanentTPType =
                        (PermanentProtectionType)
                        Enum.Parse(typeof(PermanentProtectionType), attributeCollection["permanentTPType"].InnerText);
                }
                catch
                {
                    tempStrategy.PermanentTPType = PermanentProtectionType.Relative;
                }
            }

            // Break Even
            try
            {
                tempStrategy.BreakEven = int.Parse(xmlDocStrategy.GetElementsByTagName("breakEven")[0].InnerText);
                XmlAttributeCollection attributes = xmlDocStrategy.GetElementsByTagName("breakEven")[0].Attributes;
                if (attributes != null)
                    tempStrategy.UseBreakEven = bool.Parse(attributes["useBreakEven"].InnerText);
            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }

            // Money Management
            try
            {
                tempStrategy.UseAccountPercentEntry =
                    bool.Parse(xmlDocStrategy.GetElementsByTagName("useAccountPercentEntry")[0].InnerText);
            }
            catch
            {
                tempStrategy.UseAccountPercentEntry =
                    bool.Parse(xmlDocStrategy.GetElementsByTagName("useAcountPercentEntry")[0].InnerText);
            }
            tempStrategy.MaxOpenLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("maxOpenLots")[0].InnerText);
            tempStrategy.EntryLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("entryLots")[0].InnerText);
            tempStrategy.AddingLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("addingLots")[0].InnerText);
            tempStrategy.ReducingLots = StringToDouble(xmlDocStrategy.GetElementsByTagName("reducingLots")[0].InnerText);
            try
            {
                tempStrategy.UseMartingale =
                    bool.Parse(xmlDocStrategy.GetElementsByTagName("useMartingale")[0].InnerText);
                tempStrategy.MartingaleMultiplier =
                    StringToDouble(xmlDocStrategy.GetElementsByTagName("martingaleMultiplier")[0].InnerText);
            }
            catch
            {
                tempStrategy.UseMartingale = false;
                tempStrategy.MartingaleMultiplier = 2.0;
            }

            // Description
            tempStrategy.Description = xmlDocStrategy.GetElementsByTagName("description")[0].InnerText;

            // Strategy name.
            tempStrategy.StrategyName = xmlDocStrategy.GetElementsByTagName("strategyName")[0].InnerText;

            // Reading the slots
            XmlNodeList xmlSlotList = xmlDocStrategy.GetElementsByTagName("slot");
            for (int slot = 0; slot < xmlSlotList.Count; slot++)
            {
                XmlNodeList xmlSlotTagList = xmlSlotList[slot].ChildNodes;

                XmlAttributeCollection collection = xmlSlotList[slot].Attributes;
                if (collection != null)
                {
                    // Identify the Slot Type
                    var slotType = (SlotTypes)Enum.Parse(typeof(SlotTypes), collection["slotType"].InnerText);

                    // Logical group
                    if (slotType == SlotTypes.OpenFilter || slotType == SlotTypes.CloseFilter)
                    {
                        XmlAttributeCollection attributes = collection;
                        XmlNode nodeGroup = attributes.GetNamedItem("logicalGroup");
                        string defGroup = GetDefaultGroup(slotType, slot, tempStrategy.CloseSlot);
                        if (nodeGroup != null)
                        {
                            string group = nodeGroup.InnerText;
                            tempStrategy.Slot[slot].LogicalGroup = @group;
                            if (@group != defGroup && @group.ToLower() != "all" && !Configs.UseLogicalGroups)
                            {
                                MessageBox.Show(
                                    Language.T("The strategy requires logical groups.") + Environment.NewLine +
                                    Language.T("\"Use Logical Groups\" option was temporarily switched on."),
                                    Language.T("Logical Groups"),
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                                Configs.UseLogicalGroups = true;
                            }
                        }
                        else
                            tempStrategy.Slot[slot].LogicalGroup = defGroup;
                    }

                    // Indicator name.
                    string indicatorName = xmlSlotTagList[0].InnerText;
                    if (!IndicatorManager.AllIndicatorsNames.Contains(indicatorName))
                    {
                        string message = string.Format("Indicator \"{0}\" was not found.", indicatorName);
                        throw new MissingIndicatorException(message);
                    }

                    Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
                    indicator.Initialize(slotType);

                    for (int tag = 1; tag < xmlSlotTagList.Count; tag++)
                    {
                        // List parameters
                        if (xmlSlotTagList[tag].Name == "listParam")
                        {
                            XmlAttributeCollection attributes = xmlSlotTagList[tag].Attributes;
                            if (attributes != null)
                            {
                                int listParam = int.Parse(attributes["paramNumber"].InnerText);
                                XmlNode xmlListParamNode = xmlSlotTagList[tag].FirstChild;

                                indicator.IndParam.ListParam[listParam].Caption = xmlListParamNode.InnerText;

                                xmlListParamNode = xmlListParamNode.NextSibling;
                                if (xmlListParamNode != null)
                                {
                                    int index = int.Parse(xmlListParamNode.InnerText);
                                    indicator.IndParam.ListParam[listParam].Index = index;
                                    indicator.IndParam.ListParam[listParam].Text =
                                        indicator.IndParam.ListParam[listParam].ItemList[index];
                                }
                            }
                        }

                        // Numeric parameters
                        if (xmlSlotTagList[tag].Name == "numParam")
                        {
                            XmlNode xmlNumParamNode = xmlSlotTagList[tag].FirstChild;
                            XmlAttributeCollection attributes = xmlSlotTagList[tag].Attributes;
                            if (attributes != null)
                            {
                                int numParam = int.Parse(attributes["paramNumber"].InnerText);
                                indicator.IndParam.NumParam[numParam].Caption = xmlNumParamNode.InnerText;

                                xmlNumParamNode = xmlNumParamNode.NextSibling;
                                if (xmlNumParamNode != null)
                                {
                                    string numParamValue = xmlNumParamNode.InnerText;
                                    numParamValue = numParamValue.Replace(',', Data.PointChar);
                                    numParamValue = numParamValue.Replace('.', Data.PointChar);
                                    float value = float.Parse(numParamValue);

                                    // Removing of the Stop Loss negative sign used in previous versions.
                                    string parCaption = indicator.IndParam.NumParam[numParam].Caption;
                                    if (parCaption == "Trailing Stop" ||
                                        parCaption == "Initial Stop Loss" ||
                                        parCaption == "Stop Loss")
                                        value = Math.Abs(value);
                                    indicator.IndParam.NumParam[numParam].Value = value;
                                }
                            }
                        }

                        // Check parameters
                        if (xmlSlotTagList[tag].Name == "checkParam")
                        {
                            XmlNode xmlCheckParamNode = xmlSlotTagList[tag].FirstChild;
                            XmlAttributeCollection attributes = xmlSlotTagList[tag].Attributes;
                            if (attributes != null)
                            {
                                int checkParam = int.Parse(attributes["paramNumber"].InnerText);
                                indicator.IndParam.CheckParam[checkParam].Caption = xmlCheckParamNode.InnerText;

                                xmlCheckParamNode = xmlCheckParamNode.NextSibling;
                                if (xmlCheckParamNode != null)
                                    indicator.IndParam.CheckParam[checkParam].Checked =
                                        bool.Parse(xmlCheckParamNode.InnerText);
                            }
                        }
                    }

                    // Calculate the indicator.
                    indicator.Calculate(Data.DataSet);
                    tempStrategy.Slot[slot].IndicatorName = indicator.IndicatorName;
                    tempStrategy.Slot[slot].IndParam = indicator.IndParam;
                    tempStrategy.Slot[slot].Component = indicator.Component;
                    tempStrategy.Slot[slot].SeparatedChart = indicator.SeparatedChart;
                    tempStrategy.Slot[slot].SpecValue = indicator.SpecialValues;
                    tempStrategy.Slot[slot].MinValue = indicator.SeparatedChartMinValue;
                    tempStrategy.Slot[slot].MaxValue = indicator.SeparatedChartMaxValue;
                }
                tempStrategy.Slot[slot].IsDefined = true;
            }

            return tempStrategy;
        }

        /// <summary>
        ///     Converts a string to a double number.
        /// </summary>
        private double StringToDouble(string input)
        {
            string decimalPoint = NumberFormatInfo.CurrentInfo.NumberDecimalSeparator;

            if (!input.Contains(decimalPoint))
            {
                input = input.Replace(".", decimalPoint);
                input = input.Replace(",", decimalPoint);
            }

            return double.Parse(input);
        }

        /// <summary>
        ///     Gets the default logical group of the slot.
        /// </summary>
        private string GetDefaultGroup(SlotTypes slotType, int slotIndex, int closeSlotIndex)
        {
            string group = "";

            if (slotType == SlotTypes.OpenFilter)
            {
                group = "A";
            }
            if (slotType == SlotTypes.CloseFilter)
            {
                int index = slotIndex - closeSlotIndex - 1;
                group = char.ConvertFromUtf32(char.ConvertToUtf32("a", 0) + index);
            }

            return group;
        }


        private static void AppendStringElement(XmlDocument xmlDocument, string element, string value)
        {
            AppendString(xmlDocument, element, value);
        }

        private static void AppendStringElement(XmlDocument xmlDocument, string element, int value)
        {
            AppendString(xmlDocument, element, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendStringElement(XmlDocument xmlDocument, string element, double value)
        {
            AppendString(xmlDocument, element, value.ToString(CultureInfo.InvariantCulture));
        }

        private static void AppendStringElement(XmlDocument xmlDocument, string element, bool value)
        {
            AppendString(xmlDocument, element, value.ToString());
        }

        private static void AppendString(XmlDocument xmlDocument, string element, string text)
        {
            XmlElement newElem = xmlDocument.CreateElement(element);
            newElem.InnerText = text;
            if (xmlDocument.DocumentElement != null)
                xmlDocument.DocumentElement.AppendChild(newElem);
        }

        private static DataPeriod ImportDataPeriod(string text)
        {
            try
            {
                return (DataPeriod)Enum.Parse(typeof(DataPeriod), text);
            }
            catch (Exception)
            {
                if (text == "min1")
                    return DataPeriod.M1;
                if (text == "min5")
                    return DataPeriod.M5;
                if (text == "min15")
                    return DataPeriod.M15;
                if (text == "min30")
                    return DataPeriod.M30;
                if (text == "hour1")
                    return DataPeriod.H1;
                if (text == "hour4")
                    return DataPeriod.H4;
                if (text == "day")
                    return DataPeriod.D1;
                if (text == "week")
                    return DataPeriod.W1;
            }

            return DataPeriod.D1;
        }
    }
}