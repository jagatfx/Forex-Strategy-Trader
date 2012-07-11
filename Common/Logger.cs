using System;
using System.Collections.Generic;
using System.IO;

namespace Forex_Strategy_Trader
{
    public class Logger
    {
        private FileInfo _logFileInfo;
        private bool _isCreated;
        private readonly List<string> _bufferList = new List<string>();

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

            _isCreated = true;
            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(); 
            WriteLogLine("Log file created by FST " + strVersion);

            if (_bufferList.Count > 0)
                foreach (var line in _bufferList)
                    WriteLogLine(line);

            _bufferList.Clear();
        }

        public void WriteLogLine(string text)
        {
            string message = DateTime.Now.ToString("yyyy.MM.dd hh:mm:ss") + ", " + text;

            if (!_isCreated)
            {
                _bufferList.Add("Buffer: " + message);
                return;
            }

            try
            {
                using (StreamWriter sw = _logFileInfo.AppendText())
                    sw.WriteLine(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
