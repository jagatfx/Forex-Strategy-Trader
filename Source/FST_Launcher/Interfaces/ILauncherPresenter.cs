//==============================================================
// Forex Strategy Trader
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

namespace FST_Launcher.Interfaces
{
    public interface ILauncherPresenter
    {
        void SetView(ILauncherForm launcherForm);
        void ManageIncomingMassage(string messageText);
        void VisitWebsite();
        void Proceede();
    }
}