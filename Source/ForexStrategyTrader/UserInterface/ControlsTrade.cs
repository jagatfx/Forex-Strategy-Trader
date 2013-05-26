// Controls - Trade
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Class Controls : Menu_and_StatusBar
    /// </summary>
    public partial class Controls
    {
        protected ToolStripButton TsbtnChangeID { get; private set; }
        protected ToolStripButton TsbtnConnectionGo { get; private set; }
        protected ToolStripButton TsbtnConnectionHelp { get; private set; }
        protected ToolStripButton TsbtnTrading { get; private set; }
        protected ToolStripLabel TslblConnection { get; private set; }
        protected ToolStripLabel TslblConnectionID { get; private set; }
        protected ToolStripTextBox TstbxConnectionID { get; private set; }

        private void InitializeStripTrade()
        {
            TsbtnConnectionHelp = new ToolStripButton
                                      {
                                          ToolTipText = Language.T("Help me get connected!"),
                                          DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                                          Image = Resources.help
                                      };
            TsbtnConnectionHelp.Click += ConnectionHelp_Click;
            TsTradeControl.Items.Add(TsbtnConnectionHelp);

            TslblConnectionID = new ToolStripLabel
                                    {Text = Language.T("Set connection ID"), Visible = Configs.MultipleInstances};
            TsTradeControl.Items.Add(TslblConnectionID);

            TstbxConnectionID = new ToolStripTextBox
                                    {
                                        Width = 100,
                                        BorderStyle = BorderStyle.FixedSingle,
                                        Visible = Configs.MultipleInstances
                                    };
            TstbxConnectionID.KeyPress += TstbxConnectionIDKeyPress;
            TsTradeControl.Items.Add(TstbxConnectionID);

            TsbtnConnectionGo = new ToolStripButton
                                    {
                                        ToolTipText = Language.T("Go"),
                                        Image = Resources.go_right,
                                        Width = 22,
                                        Visible = Configs.MultipleInstances,
                                        Enabled = false
                                    };
            TsbtnConnectionGo.Click += TsbtConnectionGoClick;
            TsTradeControl.Items.Add(TsbtnConnectionGo);

            TsbtnChangeID = new ToolStripButton
                                {
                                    Text = "ID ",
                                    ToolTipText = Language.T("Click to change the connection ID."),
                                    Width = 100,
                                    Enabled = true
                                };
            TsbtnChangeID.Click += TsbtChangeIDClick;
            TsbtnChangeID.Visible = false;
            TsTradeControl.Items.Add(TsbtnChangeID);

            TslblConnection = new ToolStripLabel
                                  {
                                      Text = Language.T("Not Connected"),
                                      AutoSize = false,
                                      Width = 200,
                                      Visible = !Configs.MultipleInstances
                                  };
            TsTradeControl.Items.Add(TslblConnection);

            TsbtnTrading = new ToolStripButton
                               {
                                   Text = Language.T("Start Automatic Execution"),
                                   DisplayStyle = ToolStripItemDisplayStyle.ImageAndText,
                                   Image = Resources.play,
                                   Enabled = false,
                                   Visible = !Configs.MultipleInstances
                               };
            TsbtnTrading.Click += TsbtTradingClick;
            TsTradeControl.Items.Add(TsbtnTrading);

            if (Data.IsProgramBeta)
            {
                var tslWarning = new ToolStripLabel
                                     {
                                         ForeColor = Color.Tomato,
                                         Text = Language.T("Beta version. Test carefully!"),
                                         Alignment = ToolStripItemAlignment.Right
                                     };
                tslWarning.Click += TslWarning_Click;
                TsTradeControl.Items.Add(tslWarning);
            }
        }

        protected virtual void TstbxConnectionIDKeyPress(object sender, KeyPressEventArgs e)
        {
        }

        /// <summary>
        /// Button Change ID clicked.
        /// </summary>
        protected virtual void TsbtChangeIDClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Button Connection Go clicked.
        /// </summary>
        protected virtual void TsbtConnectionGoClick(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Shows connection help.
        /// </summary>
        private void ConnectionHelp_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(@"http://forexsb.com/wiki/fst/connection");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        /// <summary>
        /// Hides the warning button.
        /// </summary>
        private void TslWarning_Click(object sender, EventArgs e)
        {
            var label = (ToolStripLabel) sender;
            label.Visible = false;
        }

        protected virtual void TsbtTradingClick(object sender, EventArgs e)
        {
        }
    }
}