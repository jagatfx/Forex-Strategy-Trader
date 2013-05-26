//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System;
using System.Drawing;
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    public class TopBottomPrice : Indicator
    {
        public TopBottomPrice()
        {
            IndicatorName = "Top Bottom Price";
            PossibleSlots = SlotTypes.Open | SlotTypes.OpenFilter | SlotTypes.Close | SlotTypes.CloseFilter;

            IndicatorAuthor = "Miroslav Popov";
            IndicatorVersion = "2.0";
            IndicatorDescription = "Bundled in FSB distribution.";
        }

        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            // Setting up the indicator parameters
            IndParam.IndicatorType = TypeOfIndicator.Additional;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (SlotType == SlotTypes.Open)
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Enter long at the top price",
                        "Enter long at the bottom price"
                    };
            else if (SlotType == SlotTypes.OpenFilter)
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "The bar opens below the top price",
                        "The bar opens above the top price",
                        "The bar opens below the bottom price",
                        "The bar opens above the bottom price",
                        "The position opens below the top price",
                        "The position opens above the top price",
                        "The position opens below the bottom price",
                        "The position opens above the bottom price"
                    };
            else if (SlotType == SlotTypes.Close)
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Exit long at the top price",
                        "Exit long at the bottom price"
                    };
            else if (SlotType == SlotTypes.CloseFilter)
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "The bar closes below the top price",
                        "The bar closes above the top price",
                        "The bar closes below the bottom price",
                        "The bar closes above the bottom price"
                    };
            else
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Not Defined"
                    };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption = "Base price";
            IndParam.ListParam[1].ItemList = new[] {"High & Low"};
            IndParam.ListParam[1].Index = 0;
            IndParam.ListParam[1].Text = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Used price from the indicator.";

            IndParam.ListParam[2].Caption = "Base period";
            IndParam.ListParam[2].ItemList = new[] {"Previous bar", "Previous day", "Previous week", "Previous month"};
            IndParam.ListParam[2].Index = 1;
            IndParam.ListParam[2].Text = IndParam.ListParam[2].ItemList[IndParam.ListParam[2].Index];
            IndParam.ListParam[2].Enabled = true;
            IndParam.ListParam[2].ToolTip = "The period, the top/bottom prices are based on.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value = 0;
            IndParam.NumParam[0].Min = -2000;
            IndParam.NumParam[0].Max = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above the top and below the bottom price.";
        }

        private bool IsPeriodChanged(int bar)
        {
            bool bIsPeriodChanged = false;
            switch (IndParam.ListParam[2].Index)
            {
                case 0: // Previous bar
                    bIsPeriodChanged = true;
                    break;
                case 1: // Previous day
                    bIsPeriodChanged = Time[bar].Day != Time[bar - 1].Day;
                    break;
                case 2: // Previous week
                    bIsPeriodChanged = Time[bar].DayOfWeek <= DayOfWeek.Wednesday &&
                                       Time[bar - 1].DayOfWeek > DayOfWeek.Wednesday;
                    break;
                case 3: // Previous month
                    bIsPeriodChanged = Time[bar].Month != Time[bar - 1].Month;
                    break;
            }

            return bIsPeriodChanged;
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Reading the parameters
            double shift = IndParam.NumParam[0].Value*Point;
            const int firstBar = 1;

            // Calculation
            var adTopPrice = new double[Bars];
            var adBottomPrice = new double[Bars];

            adTopPrice[0] = 0;
            adBottomPrice[0] = 0;

            double dTop = double.MinValue;
            double dBottom = double.MaxValue;

            for (int bar = 1; bar < Bars; bar++)
            {
                if (High[bar - 1] > dTop)
                    dTop = High[bar - 1];
                if (Low[bar - 1] < dBottom)
                    dBottom = Low[bar - 1];

                if (IsPeriodChanged(bar))
                {
                    adTopPrice[bar] = dTop;
                    adBottomPrice[bar] = dBottom;
                    dTop = double.MinValue;
                    dBottom = double.MaxValue;
                }
                else
                {
                    adTopPrice[bar] = adTopPrice[bar - 1];
                    adBottomPrice[bar] = adBottomPrice[bar - 1];
                }
            }

            var adUpperBand = new double[Bars];
            var adLowerBand = new double[Bars];
            for (int bar = firstBar; bar < Bars; bar++)
            {
                adUpperBand[bar] = adTopPrice[bar] + shift;
                adLowerBand[bar] = adBottomPrice[bar] - shift;
            }

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp
                {
                    CompName = "Top price",
                    DataType = IndComponentType.IndicatorValue,
                    ChartType = IndChartType.Level,
                    ChartColor = Color.DarkGreen,
                    FirstBar = firstBar,
                    Value = adTopPrice
                };

            Component[1] = new IndicatorComp
                {
                    CompName = "Bottom price",
                    DataType = IndComponentType.IndicatorValue,
                    ChartType = IndChartType.Level,
                    ChartColor = Color.DarkRed,
                    FirstBar = firstBar,
                    Value = adBottomPrice
                };

            Component[2] = new IndicatorComp
                {
                    ChartType = IndChartType.NoChart,
                    FirstBar = firstBar,
                    Value = new double[Bars]
                };

            Component[3] = new IndicatorComp
                {
                    ChartType = IndChartType.NoChart,
                    FirstBar = firstBar,
                    Value = new double[Bars]
                };

            // Sets the Component's type
            if (SlotType == SlotTypes.Open)
            {
                Component[2].CompName = "Long position entry price";
                Component[2].DataType = IndComponentType.OpenLongPrice;
                Component[3].CompName = "Short position entry price";
                Component[3].DataType = IndComponentType.OpenShortPrice;
            }
            else if (SlotType == SlotTypes.OpenFilter)
            {
                Component[2].CompName = "Is long entry allowed";
                Component[2].DataType = IndComponentType.AllowOpenLong;
                Component[3].CompName = "Is short entry allowed";
                Component[3].DataType = IndComponentType.AllowOpenShort;
            }
            else if (SlotType == SlotTypes.Close)
            {
                Component[2].CompName = "Long position closing price";
                Component[2].DataType = IndComponentType.CloseLongPrice;
                Component[3].CompName = "Short position closing price";
                Component[3].DataType = IndComponentType.CloseShortPrice;
            }
            else if (SlotType == SlotTypes.CloseFilter)
            {
                Component[2].CompName = "Close out long position";
                Component[2].DataType = IndComponentType.ForceCloseLong;
                Component[3].CompName = "Close out short position";
                Component[3].DataType = IndComponentType.ForceCloseShort;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the top price":
                case "Exit long at the top price":
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "Enter long at the bottom price":
                case "Exit long at the bottom price":
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The bar opens below the top price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_opens_below_the_Upper_Band);
                    break;
                case "The bar opens above the top price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_opens_above_the_Upper_Band);
                    break;
                case "The bar opens below the bottom price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_opens_below_the_Lower_Band);
                    break;
                case "The bar opens above the bottom price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_opens_above_the_Lower_Band);
                    break;
                case "The bar closes below the top price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_opens_below_the_Upper_Band);
                    break;
                case "The bar closes above the top price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_closes_above_the_Upper_Band);
                    break;
                case "The bar closes below the bottom price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_closes_below_the_Lower_Band);
                    break;
                case "The bar closes above the bottom price":
                    BandIndicatorLogic(firstBar, 0, adUpperBand, adLowerBand, ref Component[2], ref Component[3],
                                       BandIndLogic.The_bar_closes_above_the_Lower_Band);
                    break;
                case "The position opens above the top price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted top price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted bottom price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens below the top price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted top price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted bottom price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adUpperBand;
                    Component[3].Value = adLowerBand;
                    break;
                case "The position opens above the bottom price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted bottom price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyHigher;
                    Component[3].CompName = "Shifted top price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellLower;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
                case "The position opens below the bottom price":
                    Component[0].DataType = IndComponentType.Other;
                    Component[1].DataType = IndComponentType.Other;
                    Component[2].CompName = "Shifted bottom price";
                    Component[2].DataType = IndComponentType.OpenLongPrice;
                    Component[2].PosPriceDependence = PositionPriceDependence.PriceBuyLower;
                    Component[3].CompName = "Shifted top price";
                    Component[3].DataType = IndComponentType.OpenShortPrice;
                    Component[3].PosPriceDependence = PositionPriceDependence.PriceSellHigher;
                    Component[2].Value = adLowerBand;
                    Component[3].Value = adUpperBand;
                    break;
            }
        }

        public override void SetDescription()
        {
            var iShift = (int) IndParam.NumParam[0].Value;

            string sUpperTrade;
            string sLowerTrade;

            if (iShift > 0)
            {
                sUpperTrade = iShift + " points above the ";
                sLowerTrade = iShift + " points below the ";
            }
            else if (iShift == 0)
            {
                if (IndParam.ListParam[0].Text == "Enter long at the top price" ||
                    IndParam.ListParam[0].Text == "Enter long at the bottom price" ||
                    IndParam.ListParam[0].Text == "Exit long at the top price" ||
                    IndParam.ListParam[0].Text == "Exit long at the bottom price")
                {
                    sUpperTrade = "at the ";
                    sLowerTrade = "at the ";
                }
                else
                {
                    sUpperTrade = "the ";
                    sLowerTrade = "the ";
                }
            }
            else
            {
                sUpperTrade = -iShift + " points below the ";
                sLowerTrade = -iShift + " points above the ";
            }

            string sPeriod = "of the " + IndParam.ListParam[2].Text.ToLower();
            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at the top price":
                    EntryPointLongDescription = sUpperTrade + "top price " + sPeriod;
                    EntryPointShortDescription = sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "Enter long at the bottom price":
                    EntryPointLongDescription = sLowerTrade + "bottom price " + sPeriod;
                    EntryPointShortDescription = sUpperTrade + "top price " + sPeriod;
                    break;
                case "Exit long at the top price":
                    ExitPointLongDescription = sUpperTrade + "top price " + sPeriod;
                    ExitPointShortDescription = sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "Exit long at the bottom price":
                    ExitPointLongDescription = sLowerTrade + "bottom price " + sPeriod;
                    ExitPointShortDescription = sUpperTrade + "top price " + sPeriod;
                    break;

                case "The bar opens below the top price":
                    EntryFilterLongDescription = "the bar opens lower than " + sUpperTrade + "top price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar opens above the top price":
                    EntryFilterLongDescription = "the bar opens higher than " + sUpperTrade + "top price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens lower than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar opens below the bottom price":
                    EntryFilterLongDescription = "the bar opens lower than " + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens higher than " + sUpperTrade + "top price " + sPeriod;
                    break;
                case "The bar opens above the bottom price":
                    EntryFilterLongDescription = "the bar opens higher than " + sLowerTrade + "bottom price " + sPeriod;
                    EntryFilterShortDescription = "the bar opens lower than " + sUpperTrade + "top price " + sPeriod;
                    break;

                case "The position opens below the top price":
                    EntryFilterLongDescription = "the position opens lower than " + sUpperTrade + "top price " + sPeriod;
                    EntryFilterShortDescription = "the position opens higher than " + sLowerTrade + "bottom price " +
                                                  sPeriod;
                    break;
                case "The position opens above the top price":
                    EntryFilterLongDescription = "the position opens higher than " + sUpperTrade + "top price " +
                                                 sPeriod;
                    EntryFilterShortDescription = "the position opens lower than " + sLowerTrade + "bottom price " +
                                                  sPeriod;
                    break;
                case "The position opens below the bottom price":
                    EntryFilterLongDescription = "the position opens lower than " + sLowerTrade + "bottom price " +
                                                 sPeriod;
                    EntryFilterShortDescription = "the position opens higher than " + sUpperTrade + "top price " +
                                                  sPeriod;
                    break;
                case "The position opens above the bottom price":
                    EntryFilterLongDescription = "the position opens higher than " + sLowerTrade + "bottom price " +
                                                 sPeriod;
                    EntryFilterShortDescription = "the position opens lower than " + sUpperTrade + "top price " +
                                                  sPeriod;
                    break;

                case "The bar closes below the top price":
                    ExitFilterLongDescription = "the bar closes lower than " + sUpperTrade + "top price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes higher than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar closes above the top price":
                    ExitFilterLongDescription = "the bar closes higher than " + sUpperTrade + "top price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes lower than " + sLowerTrade + "bottom price " + sPeriod;
                    break;
                case "The bar closes below the bottom price":
                    ExitFilterLongDescription = "the bar closes lower than " + sLowerTrade + "bottom price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes higher than " + sUpperTrade + "top price " + sPeriod;
                    break;
                case "The bar closes above the bottom price":
                    ExitFilterLongDescription = "the bar closes higher than " + sLowerTrade + "bottom price " + sPeriod;
                    ExitFilterShortDescription = "the bar closes lower than " + sUpperTrade + "top price " + sPeriod;
                    break;
            }
        }

        public override string ToString()
        {
            return IndicatorName + " (" +
                   IndParam.ListParam[2].Text + ", " + // Base period
                   IndParam.NumParam[0].ValueToString + ")"; // Vertical shift
        }
    }
}