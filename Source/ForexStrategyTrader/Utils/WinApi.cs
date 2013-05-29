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
using System.Text;

namespace ForexStrategyBuilder
{
    /// <summary>
    ///     Selected Win AI Function Calls
    /// </summary>
    public static class WinApi
    {
        private const int WmCopydata = 0x4A;
        public const int WmUser = 0x400;
        private const int WmSyscommand = 0x0112;
        private const int ScClose = 0xF060;
        private const int SmCxscreen = 0;
        private const int SmCyscreen = 1;
        private const int SwpShowwindow = 64; // 0x0040
        private static readonly IntPtr HwndTop = IntPtr.Zero;

        private static int ScreenX
        {
            get { return GetSystemMetrics(SmCxscreen); }
        }

        private static int ScreenY
        {
            get { return GetSystemMetrics(SmCyscreen); }
        }

        public static int SendWindowsStringMessage(int hWnd, int wParam, string msg)
        {
            int result = 0;

            if (hWnd > 0)
            {
                byte[] sarr = Encoding.Default.GetBytes(msg);
                int len = sarr.Length;
                CopyDataStruct cds;
                cds.DwData = (IntPtr)100;
                cds.LpData = msg;
                cds.CbData = len + 1;
                result = SendMessage(hWnd, WmCopydata, wParam, ref cds);
            }

            return result;
        }

        public static void CloseWindow(int hWnd)
        {
            SendMessage(hWnd, WmSyscommand, ScClose, 0);
        }

        public static void SendWindowsMessage(int hWnd, int msg, int wParam, int lParam)
        {
            SendMessage(hWnd, msg, wParam, lParam);
        }

        public static int GetWindowId(string className, string windowName)
        {
            return FindWindow(className, windowName);
        }

        public static void SetWinFullScreen(IntPtr hwnd)
        {
            SetWindowPos(hwnd, HwndTop, 0, 0, ScreenX, ScreenY, SwpShowwindow);
        }

        [DllImport("User32.dll")]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int msg, int wParam, ref CopyDataStruct lParam);

        //For use with WM_COPYDATA and COPYDATASTRUCT
        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, int msg, int wParam, ref CopyDataStruct lParam);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(int hWnd, int msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(int hWnd, int msg, int wParam, int lParam);

        [DllImport("User32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(int hWnd);

        [DllImport("user32.dll", EntryPoint = "GetSystemMetrics")]
        private static extern int GetSystemMetrics(int which);

        [DllImport("user32.dll")]
        private static extern void SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height,
                                                uint flags);
    }
}