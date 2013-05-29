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
using System.Timers;
using FST_Launcher.Interfaces;

namespace FST_Launcher.Helpers
{
    public class TimeHelper : ITimeHelper
    {
        public void StartCountDown(int seconds)
        {
            var timer = new Timer {Interval = seconds*1000, AutoReset = false};
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        public event EventHandler CountDownElapsed;

        private void Timer_Elapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            EventHandler handler = CountDownElapsed;
            if (handler != null) handler(this, EventArgs.Empty);
        }
    }
}