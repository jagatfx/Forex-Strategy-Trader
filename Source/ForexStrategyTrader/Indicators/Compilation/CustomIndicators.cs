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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using ForexStrategyBuilder.Indicators;
using ForexStrategyBuilder.Library;

namespace ForexStrategyBuilder
{
    public static class CustomIndicators
    {
        private static readonly Libraries Libraries = new Libraries();

        /// <summary>
        ///     Load Source Files
        /// </summary>
        public static void LoadCustomIndicators()
        {
            var compiledDlls = new List<string>();
            var indicatorManager = new IndicatorCompilationManager();
            var errorReport = new StringBuilder();
            errorReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            bool isError = false;

            string libSettingsPath = Path.Combine(Data.SystemDir, "Libraries.xml");
            Libraries.LoadSettings(libSettingsPath);

            if (Directory.Exists(Data.SourceFolder))
            {
                string[] pathCsFiles = Directory.GetFiles(Data.SourceFolder, "*.cs");
                if (pathCsFiles.Length != 0)
                {
                    foreach (string sourcePath in pathCsFiles)
                    {
                        string errorMessages;
                        if (Libraries.IsSourceCompiled(sourcePath))
                            continue;

                        LibRecord record = indicatorManager.LoadCompileSourceFile(sourcePath, out errorMessages);

                        if (record != null)
                        {
                            Libraries.AddRecord(record);
                            compiledDlls.Add(Path.GetFileNameWithoutExtension(sourcePath));
                        }

                        if (string.IsNullOrEmpty(errorMessages)) continue;
                        isError = true;

                        errorReport.AppendLine("<h2>File name: " + Path.GetFileName(sourcePath) + "</h2>");
                        string error = errorMessages.Replace(Environment.NewLine, "</br>");
                        error = error.Replace("\t", "&nbsp; &nbsp; &nbsp;");
                        errorReport.AppendLine("<p>" + error + "</p>");
                    }
                }
            }
            if (Directory.Exists(Data.LibraryDir))
            {
                string[] pathDllFiles = Directory.GetFiles(Data.LibraryDir, "*.dll");
                if (pathDllFiles.Length != 0)
                {
                    foreach (string dllPath in pathDllFiles)
                    {
                        string fileName = Path.GetFileNameWithoutExtension(dllPath);
                        if (compiledDlls.Contains(fileName))
                            continue;

                        string errorMessages;

                        indicatorManager.LoadDllIndicator(dllPath, out errorMessages);

                        if (string.IsNullOrEmpty(errorMessages))
                            continue;
                        isError = true;

                        errorReport.AppendLine("<h2>File name: " + Path.GetFileName(dllPath) + "</h2>");
                        string error = errorMessages.Replace(Environment.NewLine, "</br>");
                        error = error.Replace("\t", "&nbsp; &nbsp; &nbsp;");
                        errorReport.AppendLine("<p>" + error + "</p>");
                    }
                }
            }
            Libraries.SaveSettings(libSettingsPath);

            // Adds the custom indicators
            IndicatorManager.ResetCustomIndicators(indicatorManager.CustomIndicatorsList);
            IndicatorManager.CombineAllIndicators();

            if (isError)
            {
                var msgBox = new FancyMessageBox(errorReport.ToString(), Language.T("Custom Indicators"))
                    {
                        BoxWidth = 550,
                        BoxHeight = 340,
                        TopMost = true
                    };
                msgBox.Show();
            }
        }

        /// <summary>
        ///     Tests the Custom Indicators.
        /// </summary>
        public static void TestCustomIndicators()
        {
            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWorkTestCustomIndicators;
            bgWorker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        ///     Does the job
        /// </summary>
        private static void DoWorkTestCustomIndicators(object sender, DoWorkEventArgs e)
        {
            bool isErrors = false;

            var errorReport = new StringBuilder();
            errorReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");

            var okReport = new StringBuilder();
            okReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            okReport.AppendLine("<p>");

            foreach (string indicatorName in IndicatorManager.CustomIndicatorNames)
            {
                string errorList;
                if (!IndicatorTester.CustomIndicatorThoroughTest(indicatorName, out errorList))
                {
                    isErrors = true;
                    errorReport.AppendLine("<h2>" + indicatorName + "</h2>");
                    string error = errorList.Replace(Environment.NewLine, "</br>");
                    error = error.Replace("\t", "&nbsp; &nbsp; &nbsp;");
                    errorReport.AppendLine("<p>" + error + "</p>");
                }
                else
                {
                    okReport.AppendLine(indicatorName + " - OK" + "<br />");
                }
            }

            okReport.AppendLine("</p>");

            var result = new CustomIndicatorsTestResult
                {
                    IsErrors = isErrors,
                    ErrorReport = errorReport.ToString(),
                    OkReport = okReport.ToString()
                };

            e.Result = result;
        }

        /// <summary>
        ///     Test is finished
        /// </summary>
        private static void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (CustomIndicatorsTestResult) e.Result;

            if (result.IsErrors)
            {
                var msgBoxError = new FancyMessageBox(result.ErrorReport, Language.T("Custom Indicators"))
                    {
                        BoxWidth = 550,
                        BoxHeight = 340,
                        TopMost = true
                    };
                msgBoxError.Show();
            }

            var msgBoxOk = new FancyMessageBox(result.OkReport, Language.T("Custom Indicators"))
                {
                    BoxWidth = 350,
                    BoxHeight = 280,
                    TopMost = true
                };
            msgBoxOk.Show();
        }

        /// <summary>
        ///     Shows the loaded custom indicators.
        /// </summary>
        public static void ShowLoadedCustomIndicators()
        {
            var loadedIndicators = new StringBuilder();
            loadedIndicators.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            loadedIndicators.AppendLine("<p>Loaded " + IndicatorManager.CustomIndicatorNames.Count + " indicators.</p>");
            loadedIndicators.AppendLine("<p>");
            foreach (string indicatorName in IndicatorManager.CustomIndicatorNames)
            {
                Indicator indicator = IndicatorManager.ConstructIndicator(indicatorName);
                string dll = indicator.LoaddedFromDll ? " (dll)" : " (cs)";
                loadedIndicators.AppendLine(indicatorName + dll + "</br>");
            }
            loadedIndicators.AppendLine("</p>");

            var msgBox = new FancyMessageBox(loadedIndicators.ToString(), Language.T("Custom Indicators"))
                {
                    BoxWidth = 480,
                    BoxHeight = 260,
                    TopMost = true
                };
            msgBox.Show();
        }

        #region Nested type: CustomIndicatorsTestResult

        /// <summary>
        ///     Stores result from the indicators test
        /// </summary>
        private struct CustomIndicatorsTestResult
        {
            public string ErrorReport { get; set; }
            public string OkReport { get; set; }
            public bool IsErrors { get; set; }
        }

        #endregion
    }
}