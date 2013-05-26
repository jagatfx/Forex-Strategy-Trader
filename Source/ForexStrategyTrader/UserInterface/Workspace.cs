// Workspace Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// This is the base application form.
    /// </summary>
    public class Workspace : Form
    {
        protected const int Space = 4;

        /// <summary>
        /// The default constructor sets the base controls.
        /// </summary>
        protected Workspace()
        {
            // Graphical measures
            Graphics g = CreateGraphics();
            SizeF sizeString = g.MeasureString("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890", Font);
            Data.HorizontalDLU = (sizeString.Width/62)/4;
            Data.VerticalDLU = sizeString.Height/8;
            g.Dispose();

            TsTradeControl = new ToolStrip();
            MainMenuStrip = new MenuStrip();
            PnlWorkspace = new Panel();
            StatusStrip = new StatusStrip();

            // Panel Workspace
            PnlWorkspace.Parent = this;
            PnlWorkspace.Dock = DockStyle.Fill;
            PnlWorkspace.Padding = new Padding(2);
            PnlWorkspace.AllowDrop = true;
            PnlWorkspace.DragEnter += Workspace_DragEnter;
            PnlWorkspace.DragDrop += WorkspaceDragDrop;

            // Tool Strip Trade control
            TsTradeControl.Parent = this;
            TsTradeControl.Dock = DockStyle.Top;

            // Main menu
            MainMenuStrip.Parent = this;
            MainMenuStrip.Dock = DockStyle.Top;

            // Status bar
            StatusStrip.Parent = this;
            StatusStrip.Dock = DockStyle.Bottom;
        }

        protected Panel PnlWorkspace { get; private set; }

        protected StatusStrip StatusStrip { get; private set; }
        protected ToolStrip TsTradeControl { get; private set; }

        private void Workspace_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All : DragDropEffects.None;
        }

        private void WorkspaceDragDrop(object sender, DragEventArgs e)
        {
            var s = (string[]) e.Data.GetData(DataFormats.FileDrop, true);
            string filePath = s[0];
            LoadDroppedStrategy(filePath);
        }

        protected virtual void LoadDroppedStrategy(string filePath)
        {
        }
    }
}