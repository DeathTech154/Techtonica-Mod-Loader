using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoUpdaterDotNET;
using System.Diagnostics;
using Microsoft.Win32;
using System.ComponentModel;
using System.Reflection;
using System.Net;
using Techtonica_Mod_Loader.Classes.Globals;
using Techtonica_Mod_Loader.Classes;
using Techtonica_Mod_Loader.Panels;

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
        public static bool skipLoadingScreenDelay = false;

        public static class Paths 
        {
            // Folders
            public static string dataFolder {
                get {
                    if (!isDebugBuild) {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Techtonica-Mod-Loader Data";
                    }
                    else {
                        return $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\Techtonica-Mod-Loader Data - Debug";
                    }
                }
            }
            public static string modsFolder = $"{dataFolder}\\Mods";
            public static string resourcesFolder = $"{dataFolder}\\Resources";

            // Files
            public static string settingsFile = $"{dataFolder}\\Settings.json";
            public static string profilesFile = $"{dataFolder}\\Profiles.json";
            public static string modsFile = $"{dataFolder}\\Mods.json";

            public static void createFolderStructure() {
                Directory.CreateDirectory(dataFolder);
                Directory.CreateDirectory(modsFolder);
                Directory.CreateDirectory(resourcesFolder);
            }
        }
        public static MainWindow current => (MainWindow)Application.Current.MainWindow;
        public static string GameLocation;
        public static Brush LaunchVanillaDisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public static string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public static string SelfLoc = Directory.GetParent(AppContext.BaseDirectory).FullName;
        public static string BepPath = "Dependencies/BepInEx";
        public static string BepName = "BepInEx_x64_5.4.21.0.zip";
        public static string BepFullPath = "Dependencies/BepInEx/BepInEx_x64_5.4.21.0.zip";
        public static DependencyStatusEnum DependancyStatus = DependencyStatusEnum.OK;

        public enum DependencyStatusEnum { OK, MissingBepFolder, MissingBepFiles }
        public enum EnumDownloadStatus { SUCCESS, DOWNLOADING, FAIL }
        // DeathTech: Todo: Refactory this eyesore.
        public static string[] BepFilesList = {
            "Dependencies/BepInEx/doorstop_config.ini",
            "Dependencies/BepInEx/winhttp.dll",
            "Dependencies/BepInEx/BepInEx/core/0Harmony.dll",
            "Dependencies/BepInEx/BepInEx/core/0Harmony.xml",
            "Dependencies/BepInEx/BepInEx/core/0Harmony20.dll",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.dll",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.xml",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.Harmony.dll",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.Harmony.xml",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.Preloader.dll",
            "Dependencies/BepInEx/BepInEx/core/BepInEx.Preloader.xml",
            "Dependencies/BepInEx/BepInEx/core/Mono.Cecil.dll",
            "Dependencies/BepInEx/BepInEx/core/Mono.Cecil.Mdb.dll",
            "Dependencies/BepInEx/BepInEx/core/Mono.Cecil.Pdb.dll",
            "Dependencies/BepInEx/BepInEx/core/Mono.Cecil.Rocks.dll",
            "Dependencies/BepInEx/BepInEx/core/MonoMod.RuntimeDetour.dll",
            "Dependencies/BepInEx/BepInEx/core/MonoMod.RuntimeDetour.xml",
            "Dependencies/BepInEx/BepInEx/core/MonoMod.Utils.dll",
            "Dependencies/BepInEx/BepInEx/core/MonoMod.Utils.xml"
        };
    }
}
