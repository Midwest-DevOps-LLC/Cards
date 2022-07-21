using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogicLayer
{
    public static class Logging
    {
        internal static string _LogPath
        {
            get; set;
        }

        public static string LogPath
        {
            get
            {
                if (String.IsNullOrEmpty(_LogPath))
                {
                    return "log.txt";
                }
                else
                {
                    return _LogPath;
                }
            }
            set
            {
                _LogPath = value;
            }
        }

        public static void SaveLog(Log log)
        {
            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(LogPath, true))
            {
                sw.WriteLine("# " + log.time + " #");

                if (!String.IsNullOrEmpty(log.text))
                    sw.WriteLine(log.text);

                if (log.exception != null)
                    sw.WriteLine(log.exception.Message + log.exception.StackTrace + log.exception.Source + log.exception.InnerException + log.exception.TargetSite);

                sw.Close();
            }
        }


    }

    public class Log
    {
        public DateTime time;
        public string text;
        public Exception exception;
    }
}
