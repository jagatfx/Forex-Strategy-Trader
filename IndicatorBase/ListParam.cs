// ListParam Class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Describes a parameter that has to be selected from a list.
    /// </summary>
    public class ListParam
    {
        /// <summary>
        /// Zeroing the parameters.
        /// </summary>
        public ListParam()
        {
            Caption = String.Empty;
            ItemList = new[] { "" };
            Index = 0;
            Text = String.Empty;
            Enabled = false;
            ToolTip = String.Empty;
        }

        /// <summary>
        /// Gets or sets the text describing the parameter.
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Gets or sets the list of parameter values.
        /// </summary>
        public string[] ItemList { get; set; }

        /// <summary>
        /// Gets or sets the text associated whit this parameter.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the index specifying the currently selected item.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether the control can respond to user interaction.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the text of tool tip associated with this control.
        /// </summary>
        public string ToolTip { get; set; }

        /// <summary>
        /// Returns a copy
        /// </summary>
        public ListParam Clone()
        {
            var listParam = new ListParam
            {
                Caption = Caption,
                ItemList = new string[ItemList.Length],
                Index = Index,
                Text = Text,
                Enabled = Enabled,
                ToolTip = ToolTip
            };
            ItemList.CopyTo(listParam.ItemList, 0);
            return listParam;
        }
    }
}
