using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRM.DAL.Util
{
    public static class Utils
    {
        static string filename = string.Format("{0}//{1:dd-MMM-yyyy}.txt", System.Configuration.ConfigurationManager.AppSettings["Logfile"], DateTime.Now);

        public static string GeneratePassword(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }


        public static string GenerateSessionToken(int lowercase, int uppercase, int numerics)
        {
            string lowers = "abcdefghijklmnopqrstuvwxyz";
            string uppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string number = "0123456789";

            Random random = new Random();

            string generated = "!";
            for (int i = 1; i <= lowercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    lowers[random.Next(lowers.Length - 1)].ToString()
                );

            for (int i = 1; i <= uppercase; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    uppers[random.Next(uppers.Length - 1)].ToString()
                );

            for (int i = 1; i <= numerics; i++)
                generated = generated.Insert(
                    random.Next(generated.Length),
                    number[random.Next(number.Length - 1)].ToString()
                );

            return generated.Replace("!", string.Empty);

        }

        public static List<T>[] Partition<T>(List<T> list, int totalPartitions)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            if (totalPartitions < 1)
                throw new ArgumentOutOfRangeException("totalPartitions");

            List<T>[] partitions = new List<T>[totalPartitions];

            int maxSize = (int)Math.Ceiling(list.Count / (double)totalPartitions);
            int k = 0;

            for (int i = 0; i < partitions.Length; i++)
            {
                partitions[i] = new List<T>();
                for (int j = k; j < k + maxSize; j++)
                {
                    if (j >= list.Count)
                        break;
                    partitions[i].Add(list[j]);
                }
                k += maxSize;
            }

            return partitions;
        }


        public static void Log(string message)
        {
            using (StreamWriter w = File.AppendText("c:/logs/workspace_log.txt"))
            {
                w.WriteLine(message + " - " + DateTime.Now);

            }
        }

        /*
         * Added by Johnbosco
         * Aimed at improving Error logging
         */
        public static void LogInfo(string MethodName, object message)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss"));
                sb.AppendLine("caller: " + MethodName + "\n: " + message);

                using (System.IO.StreamWriter str = new System.IO.StreamWriter(filename, true))
                {
                    str.WriteLineAsync(sb.ToString());
                }
            }
            catch { }
        }

        public static string LogError(Exception ex)
        {
            string message = "";
            try
            {
                message = GetExceptionMessages(ex);
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Date and Time: " + DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss"));
                sb.AppendLine("ErrorMessage: \n " + message);
                sb.AppendLine(ex.StackTrace);

                using (System.IO.StreamWriter str = new System.IO.StreamWriter(filename, true))
                {
                    str.WriteLineAsync(sb.ToString());
                }
            }
            catch { }
            return message;
        }

        static string GetExceptionMessages(Exception ex)
        {
            string ret = string.Empty;
            if (ex != null)
            {
                ret = ex.Message;
                if (ex.InnerException != null)
                    ret = ret + "\n" + GetExceptionMessages(ex.InnerException);
            }
            return ret;
        }
    }
}
