//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    public class TrailingStopLimit : Indicator
    {
        public TrailingStopLimit()
        {
            IndicatorName = "Trailing Stop Limit";
            PossibleSlots = SlotTypes.Close;
            WarningMessage = "The Trailing Stop Limit indicator trails once per bar." +
                             " It means that the indicator doesn't move the position's SL at every new top / bottom, as in the real trade, but only when a new bar begins." +
                             " The Stop Loss remains constant during the whole bar. Take Profit level is constant by definition.";

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
            IndParam.ListParam[0].ItemList = new[]
                {
                    "Exit at Trailing Stop Loss or at a constant Take Profit level"
                };
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "Logic of application of the indicator.";

            IndParam.ListParam[1].Caption = "Trailing mode";
            IndParam.ListParam[1].ItemList = new[]
                {
                    "Trails once a bar"
                };
            IndParam.ListParam[1].Index = 0;
            IndParam.ListParam[1].Text = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Mode of operation of Trailing Stop.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Initial Stop Loss";
            IndParam.NumParam[0].Value = 200;
            IndParam.NumParam[0].Min = 5;
            IndParam.NumParam[0].Max = 5000;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The initial Stop Loss value (in points).";

            IndParam.NumParam[1].Caption = "Take Profit";
            IndParam.NumParam[1].Value = 200;
            IndParam.NumParam[1].Min = 5;
            IndParam.NumParam[1].Max = 5000;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The constant Take Profit value (in points).";
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp
                {
                    CompName = "Trailing Stop for a transferred position",
                    DataType = IndComponentType.Other,
                    ShowInDynInfo = false,
                    FirstBar = 1,
                    Value = new double[Bars]
                };
        }

        public override void SetDescription()
        {
            var iStopLoss = (int) IndParam.NumParam[0].Value;
            var iTakeProfit = (int) IndParam.NumParam[1].Value;

            ExitPointLongDescription =
                "at the Trailing Stop level or at the constant Take Profit level. Initial Stop Loss: " + iStopLoss +
                " points; Take Profit: " + iTakeProfit + " points";
            ExitPointShortDescription =
                "at the Trailing Stop level or at the constant Take Profit level. Initial Stop Loss: " + iStopLoss +
                " points; Take Profit: " + iTakeProfit + " points";
        }

        public override string ToString()
        {
            return IndicatorName + " (" +
                   IndParam.NumParam[0].ValueToString + ", " + // Stop Loss
                   IndParam.NumParam[1].ValueToString + ")"; // Take Profit
        }
    }
}