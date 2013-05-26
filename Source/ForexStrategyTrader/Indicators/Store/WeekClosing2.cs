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
using ForexStrategyBuilder.Infrastructure.Entities;
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators.Store
{
    public class WeekClosing2 : Indicator
    {
        public WeekClosing2()
        {
            IndicatorName = "Week Closing 2";
            PossibleSlots = SlotTypes.Close;
            AllowClosingFilters = true;
            IsGeneratable = false;

            if (IsBacktester)
                WarningMessage = "This indicator is designed to be used in the trader." + Environment.NewLine +
                                 "It works as Week Closing indicator in the backtester.";
            else
                WarningMessage = "The indicator sends a close signal at first tick after the selected time." +
                                 Environment.NewLine +
                                 "It prevents opening of new positions after the closing time on the same day." +
                                 Environment.NewLine +
                                 "The indicator uses the server time that comes from the broker together with ticks.";

            IndicatorAuthor = "Miroslav Popov";
            IndicatorVersion = "2.0";
            IndicatorDescription = "Bundled in FSB distribution.";
        }

        public override void Initialize(SlotTypes slotType)
        {
            SlotType = slotType;

            IndParam.IndicatorType = TypeOfIndicator.DateTime;
            IndParam.ExecutionTime = ExecutionTime.AtBarClosing;

            // The ComboBox parameters
            IndParam.ListParam[0].Caption = "Logic";
            IndParam.ListParam[0].ItemList = new[] {"Exit the market at the end of the day"};
            IndParam.ListParam[0].Index = 0;
            IndParam.ListParam[0].Text = IndParam.ListParam[0].ItemList[IndParam.ListParam[0].Index];
            IndParam.ListParam[0].Enabled = true;
            IndParam.ListParam[0].ToolTip = "The execution price of all exit orders.";

            IndParam.ListParam[1].Caption = "Base price";
            IndParam.ListParam[1].ItemList = new[] {"Close"};
            IndParam.ListParam[1].Index = 0;
            IndParam.ListParam[1].Text = IndParam.ListParam[1].ItemList[IndParam.ListParam[1].Index];
            IndParam.ListParam[1].Enabled = true;
            IndParam.ListParam[1].ToolTip = "Exit price of the position.";

            // The NumericUpDown parameters
            IndParam.NumParam[0].Caption = "Day close hour";
            IndParam.NumParam[0].Value = 23;
            IndParam.NumParam[0].Min = 0;
            IndParam.NumParam[0].Max = 23;
            IndParam.NumParam[0].Enabled = true;
            IndParam.NumParam[0].ToolTip = "The hour we want to close at.";

            IndParam.NumParam[1].Caption = "Day close min";
            IndParam.NumParam[1].Value = 59;
            IndParam.NumParam[1].Min = 0;
            IndParam.NumParam[1].Max = 59;
            IndParam.NumParam[1].Enabled = true;
            IndParam.NumParam[1].ToolTip = "The minutes of the closing hour";

            IndParam.NumParam[2].Caption = "Friday close hour";
            IndParam.NumParam[2].Value = 19;
            IndParam.NumParam[2].Min = 0;
            IndParam.NumParam[2].Max = 23;
            IndParam.NumParam[2].Enabled = true;
            IndParam.NumParam[2].ToolTip = "The hour we want to close at on Friday.";

            IndParam.NumParam[3].Caption = "Friday close min";
            IndParam.NumParam[3].Value = 59;
            IndParam.NumParam[3].Min = 0;
            IndParam.NumParam[3].Max = 59;
            IndParam.NumParam[3].Enabled = true;
            IndParam.NumParam[3].ToolTip = "The minutes of the closing hour";
        }

        public override void Calculate(IDataSet dataSet)
        {
            DataSet = dataSet;

            if (IsBacktester)
            {
                CalculateForBacktester();
                return;
            }

            var fridayClosingHour = (int) IndParam.NumParam[0].Value;
            var fridayClosingMin = (int) IndParam.NumParam[1].Value;

            // Calculation
            DateTime time = ServerTime;
            var fridayTime = new DateTime(time.Year, time.Month, time.Day, fridayClosingHour, fridayClosingMin, 0);

            const int firstBar = 1;
            var adClosePrice = new double[Bars];

            // Calculation of the logic
            for (int bar = firstBar; bar < Bars - 1; bar++)
            {
                if (Time[bar].DayOfWeek > DayOfWeek.Wednesday &&
                    Time[bar + 1].DayOfWeek < DayOfWeek.Wednesday)
                    adClosePrice[bar] = Close[bar];
                else
                    adClosePrice[bar] = 0;
            }

            var adAllowOpenLong = new double[Bars];
            var adAllowOpenShort = new double[Bars];

            for (int bar = 1; bar < Bars; bar++)
            {
                adAllowOpenLong[bar] = 1;
                adAllowOpenShort[bar] = 1;
            }

            // Check the last bar
            if (time.DayOfWeek == DayOfWeek.Friday)
                if (time >= fridayTime)
                {
                    adClosePrice[Bars - 1] = Close[Bars - 1];
                    // Prevent entries after closing time
                    adAllowOpenLong[Bars - 1] = 0;
                    adAllowOpenShort[Bars - 1] = 0;
                }

            // Saving the components
            Component = new IndicatorComp[3];

            Component[0] = new IndicatorComp
                {
                    CompName = "Week Closing",
                    DataType = IndComponentType.ClosePrice,
                    ChartType = IndChartType.NoChart,
                    ShowInDynInfo = false,
                    FirstBar = firstBar,
                    Value = adClosePrice
                };

            Component[1] = new IndicatorComp
                {
                    DataType = IndComponentType.AllowOpenLong,
                    CompName = "Is long entry allowed",
                    ChartType = IndChartType.NoChart,
                    ShowInDynInfo = false,
                    FirstBar = 2,
                    Value = adAllowOpenLong
                };

            Component[2] = new IndicatorComp
                {
                    DataType = IndComponentType.AllowOpenShort,
                    CompName = "Is short entry allowed",
                    ChartType = IndChartType.NoChart,
                    ShowInDynInfo = false,
                    FirstBar = 2,
                    Value = adAllowOpenShort
                };
        }

        private void CalculateForBacktester()
        {
            // Calculation
            const int firstBar = 1;
            var adBars = new double[Bars];

            // Calculation of the logic
            for (int bar = 0; bar < Bars - 1; bar++)
            {
                if (Time[bar].DayOfWeek > DayOfWeek.Wednesday &&
                    Time[bar + 1].DayOfWeek < DayOfWeek.Wednesday)
                    adBars[bar] = Close[bar];
                else
                    adBars[bar] = 0;
            }

            // Check the last bar
            TimeSpan tsBarClosing = Time[Bars - 1].TimeOfDay.Add(new TimeSpan(0, (int) Period, 0));
            var tsDayClosing = new TimeSpan(24, 0, 0);
            if (Time[Bars - 1].DayOfWeek == DayOfWeek.Friday && tsBarClosing == tsDayClosing)
                adBars[Bars - 1] = Close[Bars - 1];

            // Saving the components
            Component = new IndicatorComp[1];

            Component[0] = new IndicatorComp
                {
                    CompName = "Week Closing",
                    DataType = IndComponentType.ClosePrice,
                    ChartType = IndChartType.NoChart,
                    ShowInDynInfo = false,
                    FirstBar = firstBar,
                    Value = adBars
                };
        }

        public override void SetDescription()
        {
            ExitPointLongDescription = "at the end of the week";
            ExitPointShortDescription = "at the end of the week";
        }

        public override string ToString()
        {
            return IndicatorName;
        }
    }
}