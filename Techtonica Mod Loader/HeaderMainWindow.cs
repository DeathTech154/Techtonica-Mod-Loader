using System;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;
using System.Reflection;
using System.IO;

namespace Techtonica_Mod_Loader
{
    /// <summary>
    /// Header file for MainWindow.xaml.cs
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow current => (MainWindow)Application.Current.MainWindow;
        public string GameLocation;
        public Brush LaunchVanillaDisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public string SelfLoc = Directory.GetParent(AppContext.BaseDirectory).FullName;
        public static string BepPath = "Dependencies/BepInEx";
        public static string BepName = "BepInEx_x64_5.4.21.0.zip";
        public static string BepFullPath = "Dependencies/BepInEx/BepInEx_x64_5.4.21.0.zip";
        public DependencyStatusEnum DependancyStatus = DependencyStatusEnum.OK;

        public enum DependencyStatusEnum { OK, MissingBepFolder, MissingBepFiles }
        public enum EnumDownloadStatus { SUCCESS, DOWNLOADING,FAIL }
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