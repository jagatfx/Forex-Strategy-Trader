// FailedCloseOrder 
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ForexStrategyBuilder.Properties;

namespace ForexStrategyBuilder.UserInterface
{
    public sealed class FailedCloseOrder : FancyPanel
    {
        private const int ResendInterval = 30;
        private const int SoundInterval = 3;
        private Timer _timerResendOrder;
        private Timer _timerSound;

        public FailedCloseOrder(Controls controls)
            : base(Language.T("Warning"), Color.Yellow, Color.Black)
        {
            BaseControls = controls;
            Height = 170;
            SetPanel();
            Enabled = false;
        }

        private Controls BaseControls { get; set; }
        private PictureBox WarningIcon { get; set; }
        private Label LblWarningMessage { get; set; }
        private Label LblSubMessage { get; set; }
        private Button BtnClosePosition { get; set; }
        private Button BtnStopSound { get; set; }
        private Button BtnCloseWarnign { get; set; }

        private void SetPanel()
        {
            WarningIcon = new PictureBox
                              {
                                  Image = SystemIcons.Warning.ToBitmap(),
                                  Parent = this,
                                  Location = new Point(10, 20),
                                  BackColor = Color.Transparent
                              };

            LblWarningMessage = new Label
                                    {
                                        Parent = this,
                                        Text = Language.T("Close order failed!"),
                                        AutoSize = true,
                                        BackColor = Color.Transparent,
                                        ForeColor = LayoutColors.ColorControlText,
                                        Font = new Font(Font.FontFamily, Font.Size + 2, FontStyle.Bold),
                                        TextAlign = ContentAlignment.MiddleCenter,
                                        Location = new Point(WarningIcon.Right + 20, WarningIcon.Top + 5),
                                    };

            LblSubMessage = new Label
                                {
                                    Parent = this,
                                    Text =
                                        Language.T(
                                            "FST will resend the close order at every ## seconds. See Journal records for more info.")
                                        .Replace("##", ResendInterval.ToString(CultureInfo.InvariantCulture)),
                                    AutoSize = true,
                                    BackColor = Color.Transparent,
                                    ForeColor = LayoutColors.ColorControlText,
                                    TextAlign = ContentAlignment.MiddleCenter,
                                    Location = new Point(WarningIcon.Right + 20, LblWarningMessage.Bottom + 5),
                                };

            BtnClosePosition = new Button
                                   {
                                       Name = "btnClose",
                                       Parent = this,
                                       Image = Resources.btn_operation_close,
                                       ImageAlign = ContentAlignment.MiddleLeft,
                                       Text = Language.T("Close Position"),
                                       Width = 295,
                                       Height = 40,
                                       Font = new Font(Font.FontFamily, 16, FontStyle.Bold),
                                       ForeColor = Color.DarkOrange,
                                       Location = new Point(WarningIcon.Right + 20, LblSubMessage.Bottom + 20),
                                       UseVisualStyleBackColor = true
                                   };
            BtnClosePosition.Click += BaseControls.BtnOperationClick;

            BtnStopSound = new Button
                               {
                                   Name = "btnStopSound",
                                   Parent = this,
                                   Text = Language.T("Stop Sound"),
                                   AutoSize = true,
                                   Location = new Point(BtnClosePosition.Left, BtnClosePosition.Bottom + 10),
                                   UseVisualStyleBackColor = true
                               };
            BtnStopSound.Click += BtnStopSoundClick;

            BtnCloseWarnign = new Button
                                  {
                                      Name = "btnStopSound",
                                      Parent = this,
                                      Text = Language.T("Close Warning Message"),
                                      AutoSize = true,
                                      Location = new Point(BtnStopSound.Right + 10, BtnClosePosition.Bottom + 10),
                                      UseVisualStyleBackColor = true
                                  };
            BtnCloseWarnign.Click += BtnCloseWarnignClick;

            _timerSound = new Timer {Interval = SoundInterval*1000};
            _timerSound.Tick += TimerSoundTick;

            _timerResendOrder = new Timer {Interval = ResendInterval*1000};
            _timerResendOrder.Tick += TimerResendOrderTick;
        }

        private void BtnCloseWarnignClick(object sender, EventArgs e)
        {
            _timerSound.Stop();
            _timerResendOrder.Stop();

            Height = 0;
            Enabled = false;
            Visible = false;

            BaseControls.DeactivateFailedCloseOrderWarning();
        }

        private void BtnStopSoundClick(object sender, EventArgs e)
        {
            BtnStopSound.Enabled = false;
            _timerSound.Stop();
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (Enabled)
            {
                _timerSound.Start();
                _timerResendOrder.Start();
            }
            else
            {
                _timerSound.Stop();
                _timerResendOrder.Stop();
            }
        }

        private void TimerSoundTick(object sender, EventArgs e)
        {
            Data.SoundError.Play();
        }

        private void TimerResendOrderTick(object sender, EventArgs e)
        {
            BaseControls.BtnOperationClick(BtnClosePosition, new EventArgs());
            Data.SoundError.Play();
        }
    }
}