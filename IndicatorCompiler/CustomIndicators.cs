using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Forex_Strategy_Trader
{
    public static class CustomIndicators
    {
        private static IndicatorCompilationManager _indicatorManager;

        /// <summary>
        /// Load Source Files
        /// </summary>
        public static void LoadCustomIndicators()
        {
            _indicatorManager = new IndicatorCompilationManager();

            if (!Directory.Exists(Data.SourceFolder))
            {
                MessageBox.Show("Custom indicators folder does not exist!", Language.T("Custom Indicators"));
                IndicatorStore.ResetCustomIndicators(null);
                IndicatorStore.CombineAllIndicators();
                return;
            }

            string[] pathInputFiles = Directory.GetFiles(Data.SourceFolder, "*.cs");
            if (pathInputFiles.Length == 0)
            {
                IndicatorStore.ResetCustomIndicators(null);
                IndicatorStore.CombineAllIndicators();
                return;
            }

            var errorReport = new StringBuilder();
            errorReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            bool isError = false;

            foreach (string filePath in pathInputFiles)
            {
                string errorMessages;
                _indicatorManager.LoadCompileSourceFile(filePath, out errorMessages);

                if (!string.IsNullOrEmpty(errorMessages))
                {
                    isError = true;

                    errorReport.AppendLine("<h2>File name: " + Path.GetFileName(filePath) + "</h2>");
                    string error = errorMessages.Replace(Environment.NewLine, "</br>");
                    error = error.Replace("\t", "&nbsp; &nbsp; &nbsp;");
                    errorReport.AppendLine("<p>" + error + "</p>");
                }
            }

            // Adds the custom indicators
            IndicatorStore.ResetCustomIndicators(_indicatorManager.CustomIndicatorsList);
            IndicatorStore.CombineAllIndicators();

            if (isError)
            {
                var msgBox = new FancyMessageBox(errorReport.ToString(), Language.T("Custom Indicators"))
                                 {BoxWidth = 550, BoxHeight = 340, TopMost = true};
                msgBox.Show();
            }

            if (Configs.ShowCustomIndicators)
                ShowLoadedCustomIndicators();
        }

        /// <summary>
        /// Tests the Custom Indicators.
        /// </summary>
        public static void TestCustomIndicators()
        {
            var bgWorker = new BackgroundWorker();
            bgWorker.DoWork += DoWorkTestCustomIndicators;
            bgWorker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            bgWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the job
        /// </summary>
        private static void DoWorkTestCustomIndicators(object sender, DoWorkEventArgs e)
        {
            bool isErrors = false;

            var errorReport = new StringBuilder();
            errorReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");

            var okReport = new StringBuilder();
            okReport.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            okReport.AppendLine("<p>");

            foreach (string indicatorName in IndicatorStore.CustomIndicatorNames)
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
                             {IsErrors = isErrors, ErrorReport = errorReport.ToString(), OKReport = okReport.ToString()};

            e.Result = result;
        }

        /// <summary>
        /// Test is finished
        /// </summary>
        private static void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var result = (CustomIndicatorsTestResult) e.Result;

            if (result.IsErrors)
            {
                var msgBoxError = new FancyMessageBox(result.ErrorReport, Language.T("Custom Indicators"))
                                      {BoxWidth = 550, BoxHeight = 340, TopMost = true};
                msgBoxError.Show();
            }

            var msgBoxOK = new FancyMessageBox(result.OKReport, Language.T("Custom Indicators"))
                               {BoxWidth = 350, BoxHeight = 280, TopMost = true};
            msgBoxOK.Show();
        }

        /// <summary>
        /// Shows the loaded custom indicators.
        /// </summary>
        private static void ShowLoadedCustomIndicators()
        {
            if (_indicatorManager.CustomIndicatorsList.Count == 0)
                return;

            var loadedIndicators = new StringBuilder();
            loadedIndicators.AppendLine("<h1>" + Language.T("Custom Indicators") + "</h1>");
            loadedIndicators.AppendLine("<p>");
            foreach (Indicator indicator in _indicatorManager.CustomIndicatorsList)
                loadedIndicators.AppendLine(indicator + "</br>");
            loadedIndicators.AppendLine("</p>");

            var msgBox = new FancyMessageBox(loadedIndicators.ToString(), Language.T("Custom Indicators"))
                             {BoxWidth = 480, BoxHeight = 260, TopMost = true};
            msgBox.Show();
        }

    }
}