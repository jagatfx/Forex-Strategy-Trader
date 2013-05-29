//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using System.Drawing;

namespace FST_Launcher
{
    public interface ISettings
    {
        string FSTPath { get; set; }
        string Arguments { get; set; }
        Color BackColor { get; set; }
        Color ForeColor { get; set; }
        int ShutDownTime { get; set; }

        string PathSettings { get; set; }
        void SetDefaults();
        void LoadSettings();
    }
}