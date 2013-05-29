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
using System.Runtime.InteropServices;

namespace ForexStrategyBuilder
{
    public struct CopyDataStruct
    {
        public int CbData;
        public IntPtr DwData;
        [MarshalAs(UnmanagedType.LPStr)]
        public string LpData;
    }
}