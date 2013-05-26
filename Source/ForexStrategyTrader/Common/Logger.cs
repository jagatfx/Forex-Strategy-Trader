// Logger class
// Part of Forex Strategy Trader
// Website http://forexsb.com/
// Copyright (c) 2009 - 2012 Miroslav Popov - All rights reserved!
// This code or any part of it cannot be used in other applications without a permission.

using System;
using System.Collections.Generic;
using System.IO;

namespace ForexStrategyBuilder
{
    public class Logger
    {
        private FileInfo _logFileInfo;
        private bool _isCreated;
        private readonly List<string> _bufferList = new List<string>();

        public void CreateLogFile(string fileNameHeader)
        {
            string time = DateTime.Now.ToString("yyyy.MM.dd_HHmmss");
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
            string message = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss") + ", " + text;

            if (!_isCreated)
            {
                _bufferList.Add("Buffer: " + message);
                return;
            }

            try
            {
                using (StreamWriter sw = _logFileInfo.AppendText())
                {
                    sw.WriteLine(message);
                    sw.Flush();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
