using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubicsGamesNetworking.Tools
{
    public static class Logger
    {
        private static bool LogInConsole = true;
        private static bool ErrorInConsole = true;

        public static void SetLogInConsole(bool log)
        {
            LogInConsole = log;
        }

        public static void SetErrorInConsole(bool log)
        {
            ErrorInConsole = log;
        }

        public static void Log(string msg)
        {
            if (!LogInConsole)
            {
                return;
            }
            Console.WriteLine(getTime() + "[Server][Log] " + msg);
        }

        public static void Error(string msg)
        {
            if (!ErrorInConsole)
            {
                return;
            }
            Console.WriteLine(getTime() + "[Server][Error] " + msg);
        }

        public static void Log(Connection con, string msg)
        {
            if (!LogInConsole)
            {
                return;
            }
            if (con != null)
            {
                Console.WriteLine(getTime() + "[Client][" + con.getUID().ToString() + "][Log] " + msg);
            }
            else
            {
                Console.WriteLine(getTime() + "[Client][Log] " + msg);
            }
        }

        public static void Error(Connection con, string msg)
        {
            if (!ErrorInConsole)
            {
                return;
            }
            if (con != null)
            {
                Console.WriteLine(getTime() + "[Client][" + con.getUID().ToString() + "][Error] " + msg);
            }
            else
            {
                Console.WriteLine(getTime() + "[Client][Error] " + msg);
            }
        }

        public static string getTime()
        {
            String hours = "00", minutes = "00", seconds = "00";
            DateTime Time = DateTime.Now;
            if (Time != null)
            {
                if (Time.Hour > 9)
                {
                    hours = Time.Hour.ToString();
                }
                else
                {
                    hours = "0" + Time.Hour.ToString();
                }
                if (Time.Minute > 9)
                {
                    minutes = Time.Minute.ToString();
                }
                else
                {
                    minutes = "0" + Time.Minute.ToString();
                }
                if (Time.Second > 9)
                {
                    seconds = Time.Second.ToString();
                }
                else
                {
                    seconds = "0" + Time.Second.ToString();
                }
            }
            return "[" + hours + ":" + minutes + ":" + seconds + "]";
        }
    }
}
