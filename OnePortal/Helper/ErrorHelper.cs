using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OnePortal.Helper
{
    public class ErrorHelper
    {
        public static void Log(string message)
        {
            string path = "C:/log";
            using (StreamWriter w = File.AppendText(Path.Combine(path, DateTime.Now.ToShortDateString() + ".txt")))
            {
                w.WriteLine(message + " - " + DateTime.Now);

            }
        }
    }
}