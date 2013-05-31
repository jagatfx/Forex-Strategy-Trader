//==============================================================
// Forex Strategy Builder
// Copyright © Miroslav Popov. All rights reserved.
//==============================================================
// THIS CODE IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
// A PARTICULAR PURPOSE.
//==============================================================

using Microsoft.Win32;

namespace ForexStrategyBuilder.Utils
{
    public class DotNetVersionChecker
    {
        public bool IsDonNet35Installed()
        {
            const string name = @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5";
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey(name);
            if (subKey == null)
                return false;

            string version = subKey.GetValue("Version").ToString();
            return version.StartsWith("3.5");
        }
    }
}