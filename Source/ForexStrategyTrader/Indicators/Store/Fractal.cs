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
    public class Fractal : Indicator
    {
        public Fractal()
        {
            IndicatorName = "Fractal";
            PossibleSlots = SlotTypes.Open | SlotTypes.Close;

            IndicatorAuthor = "Miroslav Popov";
            IndicatorVersion = "2.0";
            IndicatorDescription = "Bundled in FSB distribution.";
        }

        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            if (SlotType == SlotTypes.Open)
            {
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Enter long at an Up Fractal",
                        "Enter long at a Down Fractal"
                    };
            }
            else if (SlotType == SlotTypes.Close)
            {
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Exit long at an Up Fractal",
                        "Exit long at a Down Fractal"
                    };
            }
            else
                IndParam.ListParam[0].ItemList = new[]
                    {
                        "Not Defined"
                    };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption = "Visibility";
            IndParam.ListParam[1].ItemList = new[]
                {
                    "The fractal is visible",
                    "The fractal can be shadowed"
                };
            IndParam.ListParam[1].Index = 0;
            IndParam.ListParam[1].Text = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Is the fractal visible from the current market point.";

            IndParam.NumParam[0].Caption = "Vertical shift";
            IndParam.NumParam[0].Value = 0;
            IndParam.NumParam[0].Min = -2000;
            IndParam.NumParam[0].Max = +2000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "A vertical shift above Up Fractal and below Down Fractal.";
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Reading the parameters
            bool isVisible = IndParam.ListParam[1].Text == "The fractal is visible";
            double shift = IndParam.NumParam[0].Value*Point;
            const int firstBar = 8;

            var adFrUp = new double[Bars];
            var adFrDn = new double[Bars];

            for (int bar = 8; bar < Bars - 1; bar++)
            {
                if (High[bar - 1] < High[bar - 2] && High[bar] < High[bar - 2])
                {
                    // Fractal type 1
                    if (High[bar - 4] < High[bar - 2] &&
                        High[bar - 3] < High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];

                    // Fractal type 2
                    if (High[bar - 5] < High[bar - 2] &&
                        High[bar - 4] < High[bar - 2] &&
                        High[bar - 3] == High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];

                    // Fractal type 3, 4
                    if (High[bar - 6] < High[bar - 2] &&
                        High[bar - 5] < High[bar - 2] &&
                        High[bar - 4] == High[bar - 2] &&
                        High[bar - 3] <= High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];

                    // Fractal type 5
                    if (High[bar - 7] < High[bar - 2] &&
                        High[bar - 6] < High[bar - 2] &&
                        High[bar - 5] == High[bar - 2] &&
                        High[bar - 4] < High[bar - 2] &&
                        High[bar - 3] == High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];

                    // Fractal type 6
                    if (High[bar - 7] < High[bar - 2] &&
                        High[bar - 6] < High[bar - 2] &&
                        High[bar - 5] == High[bar - 2] &&
                        High[bar - 4] == High[bar - 2] &&
                        High[bar - 3] < High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];

                    // Fractal type 7
                    if (High[bar - 8] < High[bar - 2] &&
                        High[bar - 7] < High[bar - 2] &&
                        High[bar - 6] == High[bar - 2] &&
                        High[bar - 5] < High[bar - 2] &&
                        High[bar - 4] == High[bar - 2] &&
                        High[bar - 3] < High[bar - 2])
                        adFrUp[bar + 1] = High[bar - 2];
                }

                if (Low[bar - 1] > Low[bar - 2] && Low[bar] > Low[bar - 2])
                {
                    // Fractal type 1
                    if (Low[bar - 4] > Low[bar - 2] &&
                        Low[bar - 3] > Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];

                    // Fractal type 2
                    if (Low[bar - 5] > Low[bar - 2] &&
                        Low[bar - 4] > Low[bar - 2] &&
                        Low[bar - 3] == Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];

                    // Fractal type 3, 4
                    if (Low[bar - 6] > Low[bar - 2] &&
                        Low[bar - 5] > Low[bar - 2] &&
                        Low[bar - 4] == Low[bar - 2] &&
                        Low[bar - 3] >= Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];

                    // Fractal type 5
                    if (Low[bar - 7] > Low[bar - 2] &&
                        Low[bar - 6] > Low[bar - 2] &&
                        Low[bar - 5] == Low[bar - 2] &&
                        Low[bar - 4] > Low[bar - 2] &&
                        Low[bar - 3] == Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];

                    // Fractal type 6
                    if (Low[bar - 7] > Low[bar - 2] &&
                        Low[bar - 6] > Low[bar - 2] &&
                        Low[bar - 5] == Low[bar - 2] &&
                        Low[bar - 4] == Low[bar - 2] &&
                        Low[bar - 3] > Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];

                    // Fractal type 7
                    if (Low[bar - 8] > Low[bar - 2] &&
                        Low[bar - 7] > Low[bar - 2] &&
                        Low[bar - 6] == Low[bar - 2] &&
                        Low[bar - 5] > Low[bar - 2] &&
                        Low[bar - 4] == Low[bar - 2] &&
                        Low[bar - 3] > Low[bar - 2])
                        adFrDn[bar + 1] = Low[bar - 2];
                }
            }

            // Is visible
            if (isVisible)
                for (int bar = firstBar; bar < Bars; bar++)
                {
                    if (adFrUp[bar - 1] > 0 && Math.Abs(adFrUp[bar] - 0) < Epsilon &&
                        High[bar - 1] < adFrUp[bar - 1])
                        adFrUp[bar] = adFrUp[bar - 1];
                    if (adFrDn[bar - 1] > 0 && Math.Abs(adFrDn[bar] - 0) < Epsilon && Low[bar - 1] > adFrDn[bar - 1])
                        adFrDn[bar] = adFrDn[bar - 1];
                }
            else
                for (int iBar = firstBar; iBar < Bars; iBar++)
                {
                    if (Math.Abs(adFrUp[iBar] - 0) < Epsilon) adFrUp[iBar] = adFrUp[iBar - 1];
                    if (Math.Abs(adFrDn[iBar] - 0) < Epsilon) adFrDn[iBar] = adFrDn[iBar - 1];
                }

            // Saving the components
            Component = new IndicatorComp[4];

            Component[0] = new IndicatorComp
                {
                    CompName = "Up Fractal",
                    DataType = IndComponentType.IndicatorValue,
                    ChartType = IndChartType.Level,
                    ChartColor = Color.SpringGreen,
                    FirstBar = firstBar,
                    Value = adFrUp
                };

            Component[1] = new IndicatorComp
                {
                    CompName = "Down Fractal",
                    DataType = IndComponentType.IndicatorValue,
                    ChartType = IndChartType.Level,
                    ChartColor = Color.DarkRed,
                    FirstBar = firstBar,
                    Value = adFrDn
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

            if (SlotType == SlotTypes.Open)
            {
                Component[2].CompName = "Long position entry price";
                Component[2].DataType = IndComponentType.OpenLongPrice;
                Component[3].CompName = "Short position entry price";
                Component[3].DataType = IndComponentType.OpenShortPrice;
            }
            else if (SlotType == SlotTypes.Close)
            {
                Component[2].CompName = "Long position closing price";
                Component[2].DataType = IndComponentType.CloseLongPrice;
                Component[3].CompName = "Short position closing price";
                Component[3].DataType = IndComponentType.CloseShortPrice;
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at an Up Fractal":
                case "Exit long at an Up Fractal":
                    for (int iBar = firstBar; iBar < Bars; iBar++)
                    {
                        if (adFrUp[iBar] > Point)
                            Component[2].Value[iBar] = adFrUp[iBar] + shift;
                        if (adFrDn[iBar] > Point)
                            Component[3].Value[iBar] = adFrDn[iBar] - shift;
                    }
                    break;
                case "Enter long at a Down Fractal":
                case "Exit long at a Down Fractal":
                    for (int iBar = firstBar; iBar < Bars; iBar++)
                    {
                        if (adFrDn[iBar] > Point)
                            Component[2].Value[iBar] = adFrDn[iBar] - shift;
                        if (adFrUp[iBar] > Point)
                            Component[3].Value[iBar] = adFrUp[iBar] + shift;
                    }
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
                sUpperTrade = iShift + " points above ";
                sLowerTrade = iShift + " points below ";
            }
            else if (iShift == 0)
            {
                sUpperTrade = "at ";
                sLowerTrade = "at ";
            }
            else
            {
                sUpperTrade = -iShift + " points below ";
                sLowerTrade = -iShift + " points above ";
            }

            switch (IndParam.ListParam[0].Text)
            {
                case "Enter long at an Up Fractal":
                    EntryPointLongDescription = sUpperTrade + "an Up Fractal";
                    EntryPointShortDescription = sLowerTrade + "a Down Fractal";
                    break;
                case "Exit long at an Up Fractal":
                    ExitPointLongDescription = sUpperTrade + "an Up Fractal";
                    ExitPointShortDescription = sLowerTrade + "a Down Fractal";
                    break;
                case "Enter long at a Down Fractal":
                    EntryPointLongDescription = sLowerTrade + "a Down Fractal";
                    EntryPointShortDescription = sUpperTrade + "an Up Fractal";
                    break;
                case "Exit long at a Down Fractal":
                    ExitPointLongDescription = sLowerTrade + "a Down Fractal";
                    ExitPointShortDescription = sUpperTrade + "an Up Fractal";
                    break;
            }
        }

        public override string ToString()
        {
            return IndicatorName;
        }
    }
}