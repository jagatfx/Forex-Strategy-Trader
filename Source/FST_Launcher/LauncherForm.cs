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
        private readonly ILauncherPresenter presenter;
        private const int WmCopydata = 0x4A;

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
            lblApplicationName.ForeColor = foreColor;
            listBoxOutput.ForeColor = foreColor;
        }

        public void UpdateStatus(string record)
        {
            listBoxOutput.Invoke((MethodInvoker) (() => listBoxOutput.Items.Add(record)));
        }

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == WmCopydata)
            {
                var dataStruct = (CopyDataStruct)message.GetLParam(typeof(CopyDataStruct));
                presenter.ManageIncomingMassage(dataStruct.LpData);
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