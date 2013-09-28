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
using System.Windows.Forms;
using FST_Launcher.Helpers;
using FST_Launcher.Interfaces;

namespace FST_Launcher
{
    public sealed partial class LauncherForm : Form, ILauncherForm
    {
        private const int WmCopydata = 0x4A;
        private const int ScClose = 0xF060;
        private readonly ILauncherPresenter presenter;
        private bool closeRequested;
        private Size? mouseGrabOffset;

        public LauncherForm()
        {
            InitializeComponent();
        }

        public LauncherForm(ILauncherPresenter presenter)
            : this()
        {
            this.presenter = presenter;
        }

        public void SetColors(Color backColor, Color foreColor)
        {
            BackColor = backColor;
            ForeColor = foreColor;

            listBoxOutput.BackColor = backColor;
            listBoxOutput.ForeColor = foreColor;
            lblApplicationName.ForeColor = foreColor;
        }

        public void UpdateStatus(string record)
        {
            listBoxOutput.Invoke((MethodInvoker) (() => listBoxOutput.Items.Add(record)));
        }

        public void CloseLauncher()
        {
            Invoke((MethodInvoker) Close);
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WmCopydata)
            {
                var dataStruct = (CopyDataStruct) message.GetLParam(typeof (CopyDataStruct));
                presenter.ManageIncomingMessage(dataStruct.LpData);
            }
            else if (message.WParam.ToInt64() == ScClose)
            {
                closeRequested = true;
            }

            base.WndProc(ref message);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            mouseGrabOffset = new Size(e.Location);
            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            mouseGrabOffset = null;
            base.OnMouseUp(e);

            if (closeRequested)
                Close();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (mouseGrabOffset.HasValue)
                Location = Cursor.Position - mouseGrabOffset.Value;
            base.OnMouseMove(e);
        }

        private void FormLauncher_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}