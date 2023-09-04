using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Techtonica_Mod_Loader.Windows;

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
  
    public static partial class StringUtils
    {
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

    public static class GuiUtils 
    {
        public static void ShowShader() {
            MainWindow.current.shader.Visibility = System.Windows.Visibility.Visible;
        }

        public static void HideShader() {
            MainWindow.current.shader.Visibility = System.Windows.Visibility.Hidden;
        }

        public static void ShowInfoMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowInfo(title, description, closeButtonText);
        }

        public static void ShowWarningMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowWarning(title, description, closeButtonText);
        }

        public static void ShowErrorMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.ShowError(title, description, closeButtonText);
        }

        public static bool GetUserConfirmation(string title, string description) {
            return GetYesNoWindow.GetYesNo(title, description);
        }
    }

    public static class FileStructureUtils
    {
        public static void CreateFolderStructure() {
            Directory.CreateDirectory(ProgramData.Paths.dataFolder);
            Directory.CreateDirectory(ProgramData.Paths.modsFolder);
            Directory.CreateDirectory(ProgramData.Paths.resourcesFolder);
            Directory.CreateDirectory(ProgramData.Paths.unzipFolder);
        }

        public static void ClearUnzipFolder() {
            DeleteFolder(ProgramData.Paths.unzipFolder);
            Directory.CreateDirectory(ProgramData.Paths.unzipFolder);
        }

        public static List<string> CopyFolder(string source, string destination) {
            string[] files = Directory.GetFiles(source);
            string[] folders = Directory.GetDirectories(source);

            List<string> copiedFiles = new List<string>();

            foreach(string file in files) {
                string newPath = file.Replace(source, destination);
                if (File.Exists(newPath)) {
                    File.Delete(newPath);
                }

                File.Copy(file, newPath);
                copiedFiles.Add(newPath);
            }

            foreach(string folder in folders) {
                string newPath = folder.Replace(source, destination);
                Directory.CreateDirectory(newPath);
                List<string> copiedSubFiles = CopyFolder(folder, newPath);
                copiedFiles.AddRange(copiedSubFiles);
            }

            copiedFiles = copiedFiles.Distinct().ToList();
            return copiedFiles;
        }

        public static void DeleteFolder(string folder) {
            string[] files = Directory.GetFiles(folder);
            string[] subFolders = Directory.GetDirectories(folder);

            foreach (string file in files) {
                File.Delete(file);
            }

            foreach (string subFolder in subFolders) {
                DeleteFolder(subFolder);
            }

            Directory.Delete(folder);
        }
    }
}