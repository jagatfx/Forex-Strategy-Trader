// Edit_Translation Form
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ForexStrategyBuilder
{
    /// <summary>
    /// Edit Translation
    /// </summary>
    internal sealed class EditTranslation : Form
    {
        private const int Textboxes = 8;
        private static Dictionary<String, String> _dictLanguage;
        private string[] _asAlt;
        private string[] _asMain;
        private bool _isProgramChange = true;
        private bool _isTranslChanged;
        private int _phrases;

        /// <summary>
        /// Constructor
        /// </summary>
        public EditTranslation()
        {
            // The form
            MaximizeBox = false;
            MinimizeBox = false;
            Icon = Data.Icon;
            BackColor = LayoutColors.ColorFormBack;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Text = Language.T("Edit Translation");
            FormClosing += ActionsFormClosing;

            // Controls
            PnlCommon = new FancyPanel(Language.T("Common Parameters"));
            PnlPhrases = new FancyPanel(Language.T("English Phrase - Translated Phrase"));
            AlblInputNames = new Label[5];
            AtbxInputValues = new TextBox[5];
            AtbxMain = new TextBox[Textboxes];
            AtbxAlt = new TextBox[Textboxes];
            ScrollBar = new VScrollBar();
            TbxSearch = new TextBox();
            BtnSearch = new Button();
            BtnUntranslated = new Button();
            BtnAccept = new Button();
            BtnCancel = new Button();

            // Common
            PnlCommon.Parent = this;

            // Phrases
            PnlPhrases.Parent = this;

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
                                        Configs.Language,
                                        Language.LanguageFileName,
                                        Language.Author,
                                        Language.AuthorsWebsite,
                                        Language.AuthorsEmail
                                    };

            // Common parameters
            for (int i = 0; i < asInputNames.Length; i++)
            {
                AlblInputNames[i] = new Label
                {
                    Parent = PnlCommon,
                    ForeColor = LayoutColors.ColorControlText,
                    BackColor = Color.Transparent,
                    AutoSize = true,
                    Text = asInputNames[i]
                };

                AtbxInputValues[i] = new TextBox { Parent = PnlCommon, Text = asInputValues[i] };
            }

            // Phrases
            for (int i = 0; i < Textboxes; i++)
            {
                AtbxMain[i] = new TextBox { Parent = PnlPhrases, Multiline = true, ReadOnly = true, ForeColor = Color.DarkGray };

                AtbxAlt[i] = new TextBox { Parent = PnlPhrases, Multiline = true, Tag = i };
                AtbxAlt[i].TextChanged += EditTranslationTextChanged;
            }

            // Vertical ScrollBar
            ScrollBar.Parent = PnlPhrases;
            ScrollBar.Visible = true;
            ScrollBar.Enabled = true;
            ScrollBar.ValueChanged += ScrollBarValueChanged;
            ScrollBar.TabStop = true;

            // TextBox Search
            TbxSearch.Parent = this;
            TbxSearch.TextChanged += TbxSearchTextChanged;

            // Button Search
            BtnSearch.Parent = this;
            BtnSearch.Name = "Search";
            BtnSearch.Text = Language.T("Search");
            BtnSearch.Click += BtnClick;
            BtnSearch.UseVisualStyleBackColor = true;

            // Button Untranslated
            BtnUntranslated.Parent = this;
            BtnUntranslated.Name = "Untranslated";
            BtnUntranslated.Text = Language.T("Not Translated");
            BtnUntranslated.Click += BtnClick;
            BtnUntranslated.UseVisualStyleBackColor = true;

            // Button Cancel
            BtnCancel.Parent = this;
            BtnCancel.Name = "Cancel";
            BtnCancel.Text = Language.T("Cancel");
            BtnCancel.DialogResult = DialogResult.Cancel;
            BtnCancel.Click += BtnClick;
            BtnCancel.UseVisualStyleBackColor = true;

            // Button Accept
            BtnAccept.Parent = this;
            BtnAccept.Name = "Accept";
            BtnAccept.Text = Language.T("Accept");
            BtnAccept.DialogResult = DialogResult.OK;
            BtnAccept.Click += BtnClick;
            BtnAccept.Enabled = false;
            BtnAccept.UseVisualStyleBackColor = true;
        }

        private FancyPanel PnlCommon { get; set; }
        private FancyPanel PnlPhrases { get; set; }
        private Label[] AlblInputNames { get; set; }
        private TextBox[] AtbxInputValues { get; set; }
        private TextBox[] AtbxMain { get; set; }
        private TextBox[] AtbxAlt { get; set; }
        private VScrollBar ScrollBar { get; set; }
        private TextBox TbxSearch { get; set; }
        private Button BtnSearch { get; set; }
        private Button BtnUntranslated { get; set; }
        private Button BtnAccept { get; set; }
        private Button BtnCancel { get; set; }

        /// <summary>
        /// Performs initialization.
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Size = new Size(760, 587);
            InitParams();
            SetTextBoxes();
            ScrollBar.Focus();
        }

        /// <summary>
        /// Recalculates the sizes and positions of the controls after resizing.
        /// </summary>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            var buttonHeight = (int)(Data.VerticalDlu * 15.5);
            var buttonWidth = (int)(Data.HorizontalDlu * 60);
            var btnVertSpace = (int)(Data.VerticalDlu * 5.5);
            var btnHrzSpace = (int)(Data.HorizontalDlu * 3);
            int border = btnHrzSpace;

            // pnlCommon
            PnlCommon.Size = new Size(ClientSize.Width - 2 * border, 85);
            PnlCommon.Location = new Point(border, border);

            Graphics g = CreateGraphics();
            int maxLabelLenght = 0;
            foreach (Label label in AlblInputNames)
            {
                var lenght = (int)g.MeasureString(label.Text, Font).Width;
                if (lenght > maxLabelLenght)
                    maxLabelLenght = lenght;
            }
            g.Dispose();

            int labelWidth = maxLabelLenght + border;
            int textBoxWidth = (PnlCommon.ClientSize.Width - 4 * border - 3 * labelWidth) / 3;

            int shift = 26;
            const int vertSpace = 2;
            int number = 0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (i == 0 && j == 2) continue;

                    int xLabel = border + j * (labelWidth + textBoxWidth) + j * border;
                    int yLabel = i * buttonHeight + (i + 1) * vertSpace + shift;
                    int xTextBox = border + labelWidth + j * (labelWidth + textBoxWidth) + j * border;
                    int yTextBox = i * buttonHeight + (i + 1) * vertSpace + shift - 2;

                    AlblInputNames[number].Location = new Point(xLabel, yLabel);

                    AtbxInputValues[number].Width = textBoxWidth;
                    AtbxInputValues[number].Location = new Point(xTextBox, yTextBox);

                    number++;
                }
            }

            AtbxInputValues[0].Enabled = false;
            AtbxInputValues[1].Enabled = false;

            // pnlPhrases
            PnlPhrases.Size = new Size(ClientSize.Width - 2 * border,
                                       ClientSize.Height - buttonHeight - 2 * btnVertSpace - border - PnlCommon.Bottom);
            PnlPhrases.Location = new Point(border, PnlCommon.Bottom + border);

            shift = 22;
            textBoxWidth = (PnlPhrases.ClientSize.Width - 4 * border - ScrollBar.Width) / 2;
            int iTextBoxHeight = (PnlPhrases.ClientSize.Height - shift - (Textboxes + 1) * border) / Textboxes;

            for (int i = 0; i < Textboxes; i++)
            {
                int xMain = border;
                int yMain = i * iTextBoxHeight + (i + 1) * border + shift;
                int xAlt = 2 * border + textBoxWidth;
                int yAlt = i * iTextBoxHeight + (i + 1) * border + shift;

                AtbxMain[i].Size = new Size(textBoxWidth, iTextBoxHeight);
                AtbxAlt[i].Size = new Size(textBoxWidth, iTextBoxHeight);
                AtbxMain[i].Location = new Point(xMain, yMain);
                AtbxAlt[i].Location = new Point(xAlt, yAlt);
            }

            ScrollBar.Height = AtbxAlt[Textboxes - 1].Bottom - AtbxAlt[0].Top;
            ScrollBar.Location = new Point(PnlPhrases.ClientSize.Width - border - ScrollBar.Width, AtbxAlt[0].Top);

            // tbxSearch
            TbxSearch.Size = new Size(3 * buttonWidth / 2, buttonHeight);
            TbxSearch.Location = new Point(btnHrzSpace, ClientSize.Height - buttonHeight - btnVertSpace + 2);

            // Button Search
            BtnSearch.Size = new Size(buttonWidth, buttonHeight);
            BtnSearch.Location = new Point(TbxSearch.Right + btnHrzSpace,
                                           ClientSize.Height - buttonHeight - btnVertSpace);

            // Button Untranslated
            BtnUntranslated.Size = new Size(buttonWidth, buttonHeight);
            BtnUntranslated.Location = new Point(BtnSearch.Right + btnHrzSpace,
                                                 ClientSize.Height - buttonHeight - btnVertSpace);

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
        /// Inits the params.
        /// </summary>
        private void InitParams()
        {
            _dictLanguage = Language.Translation;
            _phrases = _dictLanguage.Values.Count;

            _asMain = new string[_phrases];
            _asAlt = new string[_phrases];
            _dictLanguage.Keys.CopyTo(_asMain, 0);
            _dictLanguage.Values.CopyTo(_asAlt, 0);

            ScrollBar.SmallChange = 1;
            ScrollBar.LargeChange = (Textboxes / 2);
            ScrollBar.Maximum = _phrases - Textboxes + ScrollBar.LargeChange - 1;
            ScrollBar.Value = 0;
        }

        /// <summary>
        /// The translation is edited;
        /// </summary>
        private void EditTranslationTextChanged(object sender, EventArgs e)
        {
            if (_isProgramChange) return;

            var tb = (TextBox)sender;
            int index = ScrollBar.Value + (int)tb.Tag;

            _asAlt[index] = tb.Text;
            _isTranslChanged = true;
            BtnAccept.Enabled = Configs.Language != "English";
        }

        /// <summary>
        /// Scroll Bar value changed.
        /// </summary>
        private void ScrollBarValueChanged(object sender, EventArgs e)
        {
            SetTextBoxes();
        }

        /// <summary>
        /// Sets the phrases in the text boxes.
        /// </summary>
        private void SetTextBoxes()
        {
            _isProgramChange = true;

            for (int i = 0; i < Textboxes; i++)
            {
                AtbxMain[i].Text = _asMain[ScrollBar.Value + i];
                AtbxAlt[i].Text = _asAlt[ScrollBar.Value + i];
            }

            _isProgramChange = false;
        }

        /// <summary>
        /// Button click
        /// </summary>
        private void BtnClick(object sender, EventArgs e)
        {
            var button = (Button)sender;
            string buttonName = button.Name;

            if (buttonName == "Search")
            {
                SearchPhrase(TbxSearch.Text);
            }

            if (buttonName == "Untranslated")
            {
                SearchUntranslatedPhrase();
            }

            if (buttonName == "Accept")
            {
                SaveTranslation();
                Language.InitLanguages();

                Close();
            }

            if (buttonName == "Cancel")
            {
                Close();
            }
        }

        /// <summary>
        /// Searches a phrase.
        /// </summary>
        private void TbxSearchTextChanged(object sender, EventArgs e)
        {
            SearchPhrase(TbxSearch.Text);
        }

        /// <summary>
        /// Searches a phrase.
        /// </summary>
        private void SearchPhrase(string phrase)
        {
            phrase = phrase.ToLower();
            phrase = phrase.Trim();

            if (phrase == "")
                return;

            for (int i = ScrollBar.Value + 1; i < _phrases; i++)
            {
                if (_asMain[i].ToLower().Contains(phrase) || _asAlt[i].ToLower().Contains(phrase))
                {
                    ScrollBar.Value = Math.Min(i, _phrases - Textboxes);
                    return;
                }
            }

            for (int i = 0; i < ScrollBar.Value + 1; i++)
            {
                if (_asMain[i].ToLower().Contains(phrase) || _asAlt[i].ToLower().Contains(phrase))
                {
                    ScrollBar.Value = Math.Min(i, _phrases - Textboxes);
                    return;
                }
            }
        }

        /// <summary>
        /// Searches for a untranslated phrase.
        /// </summary>
        private void SearchUntranslatedPhrase()
        {
            for (int i = ScrollBar.Value + 1; i < _phrases; i++)
            {
                if (_asMain[i] != _asAlt[i]) continue;
                ScrollBar.Value = Math.Min(i, _phrases - Textboxes);
                return;
            }

            for (int i = 0; i < ScrollBar.Value + 1; i++)
            {
                if (_asMain[i] != _asAlt[i]) continue;
                ScrollBar.Value = Math.Min(i, _phrases - Textboxes);
                return;
            }
        }

        /// <summary>
        /// Check whether the strategy have been changed.
        /// </summary>
        private void ActionsFormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_isTranslChanged || Configs.Language == "English") return;
            DialogResult dr = MessageBox.Show(Language.T("Do you want to accept the changes?"),
                                              Data.ProgramName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

            switch (dr)
            {
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                case DialogResult.Yes:
                    SaveTranslation();
                    Close();
                    break;
                case DialogResult.No:
                    _isTranslChanged = false;
                    Close();
                    break;
            }
        }

        /// <summary>
        /// Saves the translation
        /// </summary>
        private void SaveTranslation()
        {
            string author = AtbxInputValues[2].Text;
            string website = AtbxInputValues[3].Text;
            string contacts = AtbxInputValues[4].Text;

            for (int i = 0; i < _phrases; i++)
                _dictLanguage[_asMain[i]] = _asAlt[i];

            Language.SaveLangFile(_dictLanguage, author, website, contacts);

            _isTranslChanged = false;
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