using System;
using System.Collections.Generic;
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
using System.IO;
using System.IO.Compression;

namespace Techtonica_Mod_Loader.Dependencies
{
    public static class Dependency
    {

        public static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                DebugUtils.SendDebugLine("BepInEx File download cancelled.");
            }

            if (e.Error != null)
            {
                DebugUtils.SendDebugLine(e.Error.ToString());
            }
            else
            {
                ZipFile.ExtractToDirectory(ProgramData.BepFullPath, ProgramData.BepPath);
            }
        }

        public static int HandleDependencies(ProgramData.DependencyStatusEnum Status)
        {
            //if (Status == ProgramData.DependencyStatusEnum.MissingBepFolder)
            //{
            //    GenerateBepFolders();
            //    ProgramData.EnumDownloadStatus ZipDownloadStatus;
            //    ZipDownloadStatus = MainWindow.DownloadFile("https://github.com/BepInEx/BepInEx/releases/download/v5.4.21/BepInEx_x64_5.4.21.0.zip", ProgramData.BepFullPath);
            //    if (ZipDownloadStatus == ProgramData.EnumDownloadStatus.FAIL)
            //    {
            //        DebugUtils.SendDebugLine("Downloading BepInEx failed somehow.");
            //    }
            //}
            return 0;
        }

        public static int GenerateBepFolders()
        {
            try
            {
                DebugUtils.SendDebugLine("Creating BEP directory!");
                Directory.CreateDirectory("Dependencies/BepInEx");
                Directory.CreateDirectory("Dependencies/BepInEx/plugins");
                Directory.CreateDirectory("Plugins");
                Directory.CreateDirectory("Plugins/Library");
                DebugUtils.SendDebugLine("Directory created.");
            }
            catch
            {
                DebugUtils.SendDebugLine("An error occured during directory creation.");
                return 1;
            }
            return 0;
        }

        // DeathTech: TODO: Push to seperate file.
        public static ProgramData.DependencyStatusEnum CheckDependencies()
        {
            bool BepFolderExists = Directory.Exists("Dependencies/BepInEx");
            bool BepFilesValid = true;
            if (!BepFolderExists)
            {
                DebugUtils.SendDebugLine("BEP Folder Missing!");
                return ProgramData.DependencyStatusEnum.MissingBepFolder;
            }
            foreach (string FilePath in ProgramData.BepFilesList)
            {
                bool Exists = File.Exists(FilePath);
                if (Exists == false)
                {
                    DebugUtils.SendDebugLine("Missing file at path: " + FilePath);
                    BepFilesValid = false;
                }
            }
            if (!BepFilesValid)
            {
                DebugUtils.SendDebugLine("Bep is missing expected files.");
                return ProgramData.DependencyStatusEnum.MissingBepFiles;
            }
            else
            {
                DebugUtils.SendDebugLine("No issues with Dependencies!");
                return ProgramData.DependencyStatusEnum.OK;
            }
        }
    }
}
