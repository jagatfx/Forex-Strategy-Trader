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
using System.IO;
using FST_Launcher.Interfaces;

namespace FST_Launcher
{
    public class LauncherPresenter : ILauncherPresenter
    {
        private readonly IIoManager ioManager;
        private readonly ITimeHelper timeHelper;
        private readonly ISettings settings;
        private ILauncherForm view;

        public LauncherPresenter(IIoManager ioManager, ITimeHelper timeHelper, ISettings settings)
        {
            if (ioManager == null) throw new ArgumentNullException("ioManager");
            if (timeHelper == null) throw new ArgumentNullException("timeHelper");
            if (settings == null) throw new ArgumentNullException("settings");

            this.ioManager = ioManager;
            this.timeHelper = timeHelper;
            this.settings = settings;

            timeHelper.CountDownElapsed += TimeHelper_CountDownElapsed;
        }

        public void SetView(ILauncherForm launcherForm)
        {
            if (launcherForm == null) throw new ArgumentNullException("launcherForm");
            view = launcherForm;

            settings.PathSettings = @"FST_Launcher.xml";
            settings.SetDefaults();
            settings.LoadSettings();

            view.SetColors(settings.BackColor, settings.ForeColor);
        }

        public void Proceede()
        {
            timeHelper.StartCountDown(settings.ShutDownTime);
            StartApplication();
        }

        public void ManageIncomingMassage(string messageText)
        {
            view.UpdateStatus(messageText);
        }

        public void VisitWebsite()
        {
            ioManager.VisitWebLink(@"http:\\forexsb.com\");
        }

        private void TimeHelper_CountDownElapsed(object sender, EventArgs e)
        {
            view.Close();
        }

        private void StartApplication()
        {
            view.UpdateStatus("- loading application...");

            string path = Path.Combine(ioManager.CurrentDirectory, settings.FSTPath);
            if (ioManager.FileExists(path))
                ioManager.RunFile(path, settings.Arguments);
            else
                view.UpdateStatus("Error: cannot find FST!");
        }
    }
}