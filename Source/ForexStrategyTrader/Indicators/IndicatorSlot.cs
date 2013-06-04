//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Text;
using ForexStrategyBuilder.Infrastructure.Enums;

namespace ForexStrategyBuilder.Infrastructure.Entities
{
    public class IndicatorSlot
    {
        public IndicatorSlot()
        {
            SlotNumber = 0;
            SlotType = SlotTypes.NotDefined;
            LogicalGroup = "";
            IsDefined = false;
            IsCalculated = false;
            IndicatorName = "Not defined";
            IndParam = new IndicatorParam();
            SeparatedChart = false;
            Component = new IndicatorComp[] {};
            SpecValue = new double[] {};
            MinValue = double.MaxValue;
            MaxValue = double.MinValue;
            SignalShiftType = SignalShiftType.At;
            SignalShift = 0;
            IndicatorSymbol = "";
            IndicatorPeriod = DataPeriod.M1;
        }

        #region IIndicatorSlot Members

        /// <summary>
        ///     Gets or sets the number of the slot.
        /// </summary>
        public int SlotNumber { get; set; }

        /// <summary>
        ///     Gets or sets the type of the slot.
        /// </summary>
        public SlotTypes SlotType { get; set; }

        /// <summary>
        ///     Gets or sets the logical group of the slot.
        /// </summary>
        public string LogicalGroup { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the indicator is defined.
        /// </summary>
        public bool IsDefined { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the slot needs calculation.
        /// </summary>
        public bool IsCalculated { get; set; }

        /// <summary>
        ///     Gets or sets the indicator name.
        /// </summary>
        public string IndicatorName { get; set; }

        /// <summary>
        ///     Gets or sets the indicator parameters.
        /// </summary>
        public IndicatorParam IndParam { get; set; }

        /// <summary>
        ///     If the chart is drown in separated panel.
        /// </summary>
        public bool SeparatedChart { get; set; }

        /// <summary>
        ///     Gets or sets an indicator component.
        /// </summary>
        public IndicatorComp[] Component { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's special values.
        /// </summary>
        public double[] SpecValue { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's min value.
        /// </summary>
        public double MinValue { get; set; }

        /// <summary>
        ///     Gets or sets an indicator's max value.
        /// </summary>
        public double MaxValue { get; set; }

        /// <summary>
        ///     Gets or sets signal shift type.
        /// </summary>
        public SignalShiftType SignalShiftType { get; set; }

        /// <summary>
        ///     Gets or sets signal shift.
        /// </summary>
        public int SignalShift { get; set; }

        /// <summary>
        ///     Gets or sets indicator symbol. Empty string means default.
        /// </summary>
        public string IndicatorSymbol { get; set; }

        /// <summary>
        ///     Gets or sets indicator period. M1 means default.
        /// </summary>
        public DataPeriod IndicatorPeriod { get; set; }

        /// <summary>
        ///     Gets a description of the advanced parameters.
        /// </summary>
        public string AdvancedParamsToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(string.Format("Logical group: {0}", LogicalGroup));
            sb.AppendLine(string.Format("Signal shift: {0} {1}", SignalShiftType, SignalShift));
            string symbol = string.IsNullOrEmpty(IndicatorSymbol) ? "Default" : IndicatorSymbol;
            string period = IndicatorPeriod == DataPeriod.M1 ? "Default" : IndicatorPeriod.ToString();
            sb.AppendLine(string.Format("Symbol: {0}", symbol));
            sb.AppendLine(string.Format("Period: {0}", period));
            return sb.ToString();
        }


        /// <summary>
        ///     Returns a copy
        /// </summary>
        public IndicatorSlot Clone()
        {
            IndicatorSlot slot = ShallowCopy();

            if (Component != null && Component.Length > 0)
            {
                slot.Component = new IndicatorComp[Component.Length];
                for (int i = 0; i < Component.Length; i++)
                    slot.Component[i] = Component[i].Clone();
            }

            return slot;
        }

        /// <summary>
        ///     Returns a shallow copy.
        /// </summary>
        public IndicatorSlot ShallowCopy()
        {
            var slot = new IndicatorSlot
                {
                    SlotNumber = SlotNumber,
                    SlotType = SlotType,
                    LogicalGroup = LogicalGroup,
                    IsDefined = IsDefined,
                    IsCalculated = IsCalculated,
                    IndicatorName = IndicatorName,
                    SeparatedChart = SeparatedChart,
                    MinValue = MinValue,
                    MaxValue = MaxValue,
                    IndParam = IndParam.Clone(),
                    SpecValue = new double[SpecValue.Length],
                    SignalShiftType = SignalShiftType,
                    SignalShift = SignalShift,
                    IndicatorSymbol = IndicatorSymbol,
                    IndicatorPeriod = IndicatorPeriod,
                };

            SpecValue.CopyTo(slot.SpecValue, 0);

            return slot;
        }

        #endregion
    }
}