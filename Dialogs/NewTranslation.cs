// New_Translation Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2011 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    /// <summary>
    /// New Translation
    /// </summary>
    internal sealed class NewTranslation : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NewTranslation()
        {
            // The form
            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("New Translation");

            // Controls
            PnlInput = new FancyPanel(Language.T("Common Parameters"));
            AlblInputNames = new Label[5];
            AtbxInputValues = new TextBox[5];
            BtnAccept = new Button();
            BtnCancel = new Button();

            // Input
            PnlInput.Parent = this;

            // Input Names
            var asInputNames = new[]
                                   {
                                       Language.T("Language"),
                                       Language.T("File name"),
                                       Language.T("Author"),
                                       Language.T("Website"),
                                       Language.T("Contacts")
                                   };

            // Input Values
            var asInputValues = new[]
                                    {
                                        "Language",
                                        "Language",
                                        "Your Name",
                                        "http://forexsb.com",
                                        "info@forexsb.com"
                                    };

            // Input parameters
            for (int i = 0; i < asInputNames.Length; i++)
            {
                AlblInputNames[i] = new Label
                {
                    Parent = PnlInput,
                    ForeColor = LayoutColors.ColorControlText,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Text = asInputNames[i]
                };

                AtbxInputValues[i] = new TextBox { Parent = PnlInput, Text = asInputValues[i] };
            }

            //Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Click += BtnClick;
            BtnCancel.UseVisualStyleBackColor = true;

            //Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.Click += BtnClick;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlInput { get; set; }
        private Label[] AlblInputNames { get; set; }
        private TextBox[] AtbxInputValues { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ClientSize = new Size(335, 220);
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int)(Data.VerticalDLU * 15.5);
            var buttonWidth = (int)(Data.HorizontalDLU * 60);
            var btnVertSpace = (int)(Data.VerticalDLU * 5.5);
            var btnHrzSpace = (int)(Data.HorizontalDLU * 3);
            int border = btnHrzSpace;
            const int width = 195; // Right side controls

            PnlInput.Size = new Size(ClientSize.Width - 2 * border, 170);
            PnlInput.Location = new Point(border, border);

            int left = PnlInput.ClientSize.Width - width - btnHrzSpace - 1;

            int shift = 26;
            int vertSpace = 2;
            for (int i = 0; i < AlblInputNames.Length; i++)
            {
                AlblInputNames[i].Location = new Point(border, i * buttonHeight + (i + 1) * vertSpace + shift);
            }

            shift = 24;
            vertSpace = 2;
            for (int i = 0; i < AtbxInputValues.Length; i++)
            {
                AtbxInputValues[i].Width = width;
                AtbxInputValues[i].Location = new Point(left, i * buttonHeight + (i + 1) * vertSpace + shift);
            }

            // Button Cancel
            BtnCancel.Size = new Size(buttonWidth, buttonHeight);
            BtnCancel.Location = new Point(ClientSize.Width - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Accept
            BtnAccept.Size = new Size(buttonWidth, buttonHeight);
            BtnAccept.Location = new Point(BtnCancel.Left - buttonWidth - btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);
        }

        /// <summary>
        /// Button click
        /// </summary>
        private void BtnClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            string name = btn.Name;

            if (name == "Accept")
            {
                bool isCorrect = true;

                string language = AtbxInputValues[0].Text;
                string fileName = AtbxInputValues[1].Text + ".xml";
                string author = AtbxInputValues[2].Text;
                string website = AtbxInputValues[3].Text;
                string contacts = AtbxInputValues[4].Text;

                // Language
                if (language.Length < 2)
                {
                    MessageBox.Show("The language name must be at least two characters in length!", "Language",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                foreach (string lang in Language.LanguageList)
                    if (language == lang)
                    {
                        MessageBox.Show(
                            "A translation in this language exists already!" + Environment.NewLine +
                            "Change the language name.", "Language", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        isCorrect = false;
                    }

                // Language file name
                if (fileName.Length < 2)
                {
                    MessageBox.Show("The language file name must be at least two characters in length!",
                                    "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                if (Directory.Exists(Data.LanguageDir))
                {
                    string[] asFileNames = Directory.GetFiles(Data.LanguageDir);
                    foreach (string path in asFileNames)
                    {
                        if (fileName == Path.GetFileName(path))
                        {
                            MessageBox.Show(
                                "This file name exists already!" + Environment.NewLine + "Change the file name.",
                                "Language File Name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            isCorrect = false;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Could not find the language files directory!", "Language Files Directory",
                                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    isCorrect = false;
                }

                if (isCorrect)
                {
                    if (Language.GenerateNewLangFile(fileName, language, author, website, contacts))
                    {
                        Configs.Language = language;
                        string sMassage = "The new language file was successfully created." + Environment.NewLine +
                                          "Restart the program and edit the translation.";
                        MessageBox.Show(sMassage, "New Translation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    return;
            }

            Close();
        }

        /// <summary>
        /// Form On Paint
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            Data.GradientPaint(e.Graphics, ClientRectangle, LayoutColors.ColorFormBack, LayoutColors.DepthControl);
        }
    }
}
