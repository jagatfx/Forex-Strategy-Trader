// Custom_Indicators class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// Stores result from the indicators test
    /// </summary>
    public struct CustomIndicatorsTestResult
    {
        public string ErrorReport { get; set; }
        public string OKReport { get; set; }
        public bool IsErrors { get; set; }
    }
}
