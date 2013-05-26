//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using ForexStrategyBuilder.Indicators;

namespace ForexStrategyBuilder.Infrastructure.Interfaces
{
    public interface IIndicator
    {
        /// <summary>
        ///     Current data set;
        /// </summary>
        IDataSet DataSet { get; set; }

        /// <summary>
        ///     Gets or sets the indicator name.
        /// </summary>
        string IndicatorName { get; }

        /// <summary>
        ///     Gets or sets the indicator current parameters.
        /// </summary>
        IndicatorParam IndParam { get; set; }

        /// <summary>
        ///     Type of the slot for the current instance.
        /// </summary>
        SlotTypes SlotType { get; }

        /// <summary>
        ///     Gets if the default group is "All"
        /// </summary>
        bool IsDeafultGroupAll { get; set; }

        /// <summary>
        ///     If the chart is drown in separated panel.
        /// </summary>
        bool SeparatedChart { get; }

        /// <summary>
        ///     Gets the indicator components.
        /// </summary>
        IndicatorComp[] Component { get; }

        /// <summary>
        ///     Gets the indicator's special values.
        /// </summary>
        double[] SpecialValues { get; }

        /// <summary>
        ///     Gets the indicator's min value.
        /// </summary>
        double SeparatedChartMinValue { get; }

        /// <summary>
        ///     Gets the indicator's max value.
        /// </summary>
        double SeparatedChartMaxValue { get; }

        /// <summary>
        ///     Shows if the indicator is custom.
        /// </summary>
        bool CustomIndicator { get; set; }

        /// <summary>
        ///     Shows if the indicator is loadded from a dll.
        /// </summary>
        bool LoaddedFromDll { get; set; }

        /// <summary>
        ///     Gets the version text of the indicator
        /// </summary>
        string IndicatorVersion { get; set; }

        /// <summary>
        ///     Gets the author's name of the indicator
        /// </summary>
        string IndicatorAuthor { get; set; }

        /// <summary>
        ///     Gets the description text of the indicator
        /// </summary>
        string IndicatorDescription { get; set; }

        /// <summary>
        ///     Gets or sets a warning message about the indicator
        /// </summary>
        string WarningMessage { get; }

        /// <summary>
        ///     Shows if a closing point indicator can be used with closing logic conditions.
        /// </summary>
        bool AllowClosingFilters { get; }

        /// <summary>
        ///     Gets the indicator Entry Point Long Description
        /// </summary>
        string EntryPointLongDescription { get; }

        /// <summary>
        ///     Gets the indicator Entry Point Short Description
        /// </summary>
        string EntryPointShortDescription { get; }

        /// <summary>
        ///     Gets the indicator Exit Point Long Description
        /// </summary>
        string ExitPointLongDescription { get; }

        /// <summary>
        ///     Gets the indicator Exit Point Short Description
        /// </summary>
        string ExitPointShortDescription { get; }

        /// <summary>
        ///     Gets the indicator Entry Filter Description
        /// </summary>
        string EntryFilterLongDescription { get; }

        /// <summary>
        ///     Gets the indicator Exit Filter Description
        /// </summary>
        string ExitFilterLongDescription { get; }

        /// <summary>
        ///     Gets the indicator Entry Filter Description
        /// </summary>
        string EntryFilterShortDescription { get; }

        /// <summary>
        ///     Gets the indicator Exit Filter Description
        /// </summary>
        string ExitFilterShortDescription { get; }

        /// <summary>
        ///     Gets or sets UsePreviousBarValue parameter.
        /// </summary>
        bool UsePreviousBarValue { get; set; }

        /// <summary>
        ///     Replaces main indicator with the same name.
        /// </summary>
        bool OverrideMainIndicator { get; set; }

        /// <summary>
        ///     Gets environment type: true for a builder and false for a trader.
        /// </summary>
        bool IsBacktester { get; }

        /// <summary>
        ///     Gets or sets if Generator can use this indicator.
        /// </summary>
        bool IsGeneratable { get; set; }

        /// <summary>
        ///     Tests if this is one of the possible slots.
        /// </summary>
        /// <param name="slotType">The slot we test.</param>
        /// <returns>True if the slot is possible.</returns>
        bool TestPossibleSlot(SlotTypes slotType);

        /// <summary>
        ///     Initializes indicator parameters.
        /// </summary>
        void Initialize(SlotTypes slotType);

        /// <summary>
        ///     Calculates the components
        /// </summary>
        void Calculate(IDataSet dataSet);

        /// <summary>
        ///     Sets the indicator logic description
        /// </summary>
        void SetDescription();
    }
}