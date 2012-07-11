using System;
using System.IO;

namespace Forex_Strategy_Trader
{
    public class Logger
    {
        private FileInfo _logFileInfo;

        public void CreateLogFile(string fileNameHeader)
        {
            string time = DateTime.Now.ToString("yyyy.MM.dd_hhmmss");
            string name = fileNameHeader + time + ".log";
            string path = Path.Combine(Environment.CurrentDirectory, @"Logs\" + name);

            try
            {
                _logFileInfo = new FileInfo(path);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            WriteLogLine("Log file created by FST " + strVersion);
        }

        public void WriteLogLine(string text)
        {
            try
            {
                using (StreamWriter sw = _logFileInfo.AppendText())
                    sw.WriteLine(DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + ", " + text);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
