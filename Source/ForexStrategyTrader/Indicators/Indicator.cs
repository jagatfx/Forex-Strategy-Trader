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
using ForexStrategyBuilder.Infrastructure.Enums;
using ForexStrategyBuilder.Infrastructure.Interfaces;

namespace ForexStrategyBuilder.Indicators
{
    public partial class Indicator : IIndicator
    {
        private string indicatorName;
        private SlotTypes slotType;

        /// <summary>
        /// The default constructor
        /// </summary>
        public Indicator()
        {
            indicatorName = string.Empty;
            PossibleSlots = SlotTypes.NotDefined;
            SeparatedChart = false;
            SeparatedChartMinValue = double.MaxValue;
            SeparatedChartMaxValue = double.MinValue;
            IsDiscreteValues = false;
            CustomIndicator = false;
            LoaddedFromDll = false;
            WarningMessage = string.Empty;
            AllowClosingFilters = false;

            SpecialValues = new double[] { };
            IndParam = new IndicatorParam();
            Component = new IndicatorComp[] { };

            IsBacktester = false;
            IsGeneratable = true;

            ExitFilterShortDescription = "Not defined";
            EntryFilterShortDescription = "Not defined";
            ExitFilterLongDescription = "Not defined";
            EntryFilterLongDescription = "Not defined";
            ExitPointShortDescription = "Not defined";
            ExitPointLongDescription = "Not defined";
            EntryPointShortDescription = "Not defined";
            EntryPointLongDescription = "Not defined";
        }

        /// <summary>
        ///     Gets or sets the possible slots
        /// </summary>
        protected SlotTypes PossibleSlots { private get; set; }

        /// <summary>
        ///     Shows if the indicator has discrete values.
        /// </summary>
        protected bool IsDiscreteValues { private get; set; }

        /// <summary>
        ///     Shows if the indicator is custom.
        /// </summary>
        public bool CustomIndicator { get; set; }

        /// <summary>
        ///     Shows if the indicator is loaded from a dll.
        /// </summary>
        public bool LoaddedFromDll { get; set; }

        /// <summary>
        ///     Gets the version text of the indicator
        /// </summary>
        public string IndicatorVersion { get; set; }

        /// <summary>
        ///     Gets the author's name of the indicator
        /// </summary>
        public string IndicatorAuthor { get; set; }

        /// <summary>
        ///     Gets the description text of the indicator
        /// </summary>
        public string IndicatorDescription { get; set; }

        /// <summary>
        ///     Time frame of the loaded historical data
        /// </summary>
        protected static DataPeriod Period
        {
            get { return Data.Period; }
        }

        /// <summary>
        ///     The minimal price change.
        /// </summary>
        protected static double Point
        {
            get { return Data.InstrProperties.Point; }
        }

        /// <summary>
        ///     Number of digits after the decimal point of the historical data.
        /// </summary>
        protected static int Digits
        {
            get { return Data.InstrProperties.Digits; }
        }

        /// <summary>
        ///     Number of loaded bars
        /// </summary>
        protected static int Bars
        {
            get { return Data.Bars; }
        }

        /// <summary>
        ///     Bar opening date and time
        /// </summary>
        protected static DateTime[] Time
        {
            get { return Data.Time; }
        }

        /// <summary>
        ///     Bar opening price
        /// </summary>
        protected static double[] Open
        {
            get { return Data.Open; }
        }

        /// <summary>
        ///     Bar highest price
        /// </summary>
        protected static double[] High
        {
            get { return Data.High; }
        }

        /// <summary>
        ///     Bar lowest price
        /// </summary>
        protected static double[] Low
        {
            get { return Data.Low; }
        }

        /// <summary>
        ///     Bar closing price
        /// </summary>
        protected static double[] Close
        {
            get { return Data.Close; }
        }

        /// <summary>
        ///     Bar volume
        /// </summary>
        protected static int[] Volume
        {
            get { return Data.Volume; }
        }

        protected double Epsilon
        {
            get { return 0.0000001; }
        }


        /// <summary>
        ///     Fake property.
        ///     It serves to provide future compatibility.
        /// </summary>
        public IDataSet DataSet { get; set; }


        /// <summary>
        ///     Type of the slot for the current instance.
        /// </summary>
        public SlotTypes SlotType
        {
            get { return slotType; }
            protected set
            {
                slotType = value;
                IndParam.SlotType = value;
            }
        }

        public bool IsDeafultGroupAll { get; set; }

        /// <summary>
        ///     Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName
        {
            get { return indicatorName; }
            protected set
            {
                indicatorName = value;
                IndParam.IndicatorName = value;
            }
        }

        public IndicatorParam IndParam { get; set; }
        public IndicatorComp[] Component { get; protected set; }


        public bool SeparatedChart { get; protected set; }
        public double SeparatedChartMinValue { get; protected set; }
        public double SeparatedChartMaxValue { get; protected set; }
        public double[] SpecialValues { get; protected set; }

        /// <summary>
        ///     Shows if a closing point indicator can be used with closing logic conditions.
        /// </summary>
        public bool AllowClosingFilters { get; protected set; }

        public string EntryPointLongDescription { get; protected set; }
        public string EntryPointShortDescription { get; protected set; }
        public string ExitPointLongDescription { get; protected set; }
        public string ExitPointShortDescription { get; protected set; }
        public string EntryFilterLongDescription { get; protected set; }
        public string ExitFilterLongDescription { get; protected set; }
        public string EntryFilterShortDescription { get; protected set; }
        public string ExitFilterShortDescription { get; protected set; }

        public bool UsePreviousBarValue { get; set; }

        public bool OverrideMainIndicator { get; set; }
        public string WarningMessage { get; protected set; }

        public bool IsBacktester { get; private set; }
        public bool IsGeneratable { get; set; }

        protected DateTime ServerTime { get { return Data.ServerTime; } }

        /// <summary>
        ///     Tests if this is one of the possible slots.
        /// </summary>
        /// <param name="slot">The slot we test.</param>
        /// <returns>True if the slot is possible.</returns>
        public bool TestPossibleSlot(SlotTypes slot)
        {
            if ((slot & PossibleSlots) == SlotTypes.Open)
                return true;

            if ((slot & PossibleSlots) == SlotTypes.OpenFilter)
                return true;

            if ((slot & PossibleSlots) == SlotTypes.Close)
                return true;

            if ((slot & PossibleSlots) == SlotTypes.CloseFilter)
                return true;

            return false;
        }

        public virtual void Initialize(SlotTypes slot)
        {
        }

        public virtual void Calculate(IDataSet dataSet)
        {
        }

        public virtual void SetDescription()
        {
        }
    }
}
