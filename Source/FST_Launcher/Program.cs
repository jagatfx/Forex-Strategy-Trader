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
using System.Threading;
using System.Windows.Forms;
using FST_Launcher.Helpers;
using FST_Launcher.Interfaces;

namespace FST_Launcher
{
    internal static class Program
    {
        private static readonly Mutex AppMutex = new Mutex(true, "{BC6AD68A-0378-4246-A630-FFC661FCBAF4}");
        static ILauncherPresenter presenter;

        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            if (!AppMutex.WaitOne(TimeSpan.Zero, true)) return;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            IIoManager ioManager = new IoManager();
            ITimeHelper timeHelper = new TimeHelper();
            ISettings settings = new Settings();
            presenter = new LauncherPresenter(ioManager, timeHelper, settings);
            var form = new LauncherForm(presenter);
            presenter.SetView(form);

            Application.Idle += Application_Idle;
            Application.Run(form);

            AppMutex.ReleaseMutex();
        }

        static void Application_Idle(object sender, EventArgs e)
        {
            Application.Idle -= Application_Idle;
            presenter.Proceede();
        }
    }
}