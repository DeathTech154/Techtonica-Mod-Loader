using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techtonica_Mod_Loader
{
    public static class ProgramData
    {
        public static bool isDebugBuild {
            get {
                #if DEBUG
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        public static class Paths {
            private static string dataFolder {
                get {
                    if(!isDebugBuild) {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Techtonica-Mod-Loader Data";
                    }
                    else {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Techtonica-Mod-Loader Data - Debug";
                    }
                }
            }
            public static string settingsFile = $"{dataFolder}\\Settings.json";
        }
    }
}
