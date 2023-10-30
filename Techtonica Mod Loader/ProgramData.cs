﻿using System;
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
        public static bool skipLoadingScreenDelay = true;
        public static bool isDownloading = false;
        public static string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public const string bepInExID = "b9a5a1bd-81d8-4913-a46e-70ca7734628c";
        public static ModListSortOption currentSortOption = ModListSortOption.Alphabetical;

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
            public static string unzipFolder = $"{dataFolder}\\Unzip";
            public static string markdownFiles = $"{dataFolder}\\MarkdownFiles";
            public static string imageCache = $"{dataFolder}\\ImageCache";

            public static List<string> folders = new List<string>() {
                dataFolder,
                modsFolder,
                resourcesFolder,
                unzipFolder,
                markdownFiles,
                imageCache
            };
            
            public static string gameFolder => Settings.userSettings.gameFolder;
            public static string bepInExConfigFolder => $"{gameFolder}/BepInEx/config";
            public static string bepInExPatchersFolder => $"{gameFolder}/BepInEx/patchers";
            public static string bepInExPluginsFolder => $"{gameFolder}/BepInEx/plugins";

            // Files
            public static string settingsFile = $"{dataFolder}\\Settings.json";
            public static string profilesFile = $"{dataFolder}\\Profiles.json";
            public static string modsFile = $"{dataFolder}\\Mods.json";
            public static string tempZipFile = $"{dataFolder}\\Temp.zip";
            public static string tempMarkdownFile = $"{unzipFolder}\\README.md";
            public static string imageCacheMapFile = $"{dataFolder}\\ImageCacheMap.json";
            public static string logFile = $"{dataFolder}\\TML.log";
        }
    }
}
