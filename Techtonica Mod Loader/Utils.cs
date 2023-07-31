using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techtonica_Mod_Loader
{
    public static class DebugUtils
    {
        public static void CrashIfDebug(string errorMessage) {
            if (ProgramData.isDebugBuild) {
                throw new Exception(errorMessage);
            }
        }

        public static void SendDebugLine(string Str) {
            Console.WriteLine(Str);
            Debug.WriteLine(Str);
        }
    }

    public static class StringUtils
    {
        // DeathTech: TODO: Push this to a utiltiy section.
        public static string RemoveFromBack(string Original, int Entries, string Seperator)
        {
            string[] Splited = Original.Split(Seperator);
            int SplitLength = Splited.Length;
            string NewString = "";
            int count = 0;
            foreach (string Split in Splited)
            {
                if (count == 0)
                {
                    NewString = NewString + Split;
                    count = count + 1;
                }
                else if (count < (SplitLength - Entries))
                {
                    NewString = NewString + Seperator + Split;
                    count = count + 1;
                }
            }
            return NewString;
        }
    }
}
