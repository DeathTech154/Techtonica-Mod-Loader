using Microsoft.Win32;
using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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

        public static void OpenURL(string url) {
            ProcessStartInfo info = new ProcessStartInfo() {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(info);
        }

        public static async Task<System.Windows.Controls.Image> GetImageFromURL(string url) {
            try {
                using (WebClient webClient = new WebClient()) {
                    byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(url));

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(imageData);
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    System.Windows.Controls.Image image = new System.Windows.Controls.Image();
                    image.Source = bitmapImage;
                    image.Width = bitmapImage.PixelWidth;
                    image.Height = bitmapImage.PixelHeight;
                    image.Stretch = System.Windows.Media.Stretch.UniformToFill;
                    return image;
                }
            }
            catch (Exception ex) {
                string error = $"Error loading image: {ex.Message}";
                DebugUtils.SendDebugLine(error);
                return null;
            }
        }

        public static async Task<SvgViewbox> GetSVGViewboxFromURL(string url) {
            try {
                using (WebClient webClient = new WebClient()) {
                    byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(url));
                    string svgText = Encoding.UTF8.GetString(imageData);

                    int widthStart = svgText.IndexOf("width=\"") + 7;
                    int widthEnd = svgText.IndexOf("\"", widthStart + 1);
                    double width = double.Parse(svgText.Substring(widthStart, widthEnd - widthStart));

                    int heightStart = svgText.IndexOf("height=\"") + 8;
                    int heightEnd = svgText.IndexOf("\"", heightStart + 1);
                    double height = double.Parse(svgText.Substring(heightStart, heightEnd - heightStart));

                    SvgViewbox svg = new SvgViewbox();
                    svg.Source = new Uri(url);
                    svg.Width = width;
                    svg.Height = height;

                    return svg;
                }
            }
            catch (Exception ex) {
                string error = $"Error loading svg: {ex.Message}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return null;
            }
        }
    }

    public static class FileStructureUtils
    {
        public static void CreateFolderStructure() {
            foreach(string folder in ProgramData.Paths.folders) {
                Directory.CreateDirectory(folder);
            }
        }

        public static void ClearUnzipFolder() {
            DeleteFolder(ProgramData.Paths.unzipFolder);
            Directory.CreateDirectory(ProgramData.Paths.unzipFolder);
        }

        public static bool SearchForConfigFile(string folder, out string configFile) {
            configFile = "Not Found";
            
            string[] files = Directory.GetFiles(folder);
            foreach(string file in files) {
                if (file.EndsWith(".cfg")) {
                    configFile = file;
                    return true;
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach(string directory in directories) {
                if(SearchForConfigFile(directory, out configFile)) {
                    return true;
                }
            }

            return false;
        }

        public static bool SearchForMarkdownFile(string folder, out string markdownFile) {
            markdownFile = "Not Found";

            string[] files = Directory.GetFiles(folder);
            foreach(string file in files) {
                if (file.EndsWith(".md")) {
                    markdownFile = file;
                    return true;
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach(string directory in directories) {
                if(SearchForConfigFile(directory, out markdownFile)) {
                    return true;
                }
            }

            return false;
        }

        public static List<string> SearchForDllFiles(string folder) {
            List<string> dllFiles = new List<string>();
            
            string[] files = Directory.GetFiles(folder);
            foreach(string file in files) {
                if (file.EndsWith(".dll")) {
                    dllFiles.Add(file);
                }
            }

            string[] directories = Directory.GetDirectories(folder);
            foreach(string directory in directories) {
                dllFiles.AddRange(SearchForDllFiles(directory));
            }

            return dllFiles;
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

        public static string GetProgramDirectory() {
            return Directory.GetParent(AppContext.BaseDirectory).FullName;
        }

        public static bool FindSteamGameFolder() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"); // Gets steam folder location from registry.
            if (key != null) {
                string steamPath = (string)key.GetValue("SteamPath");
                key.Close();
                DebugUtils.SendDebugLine(steamPath);
                string gameLocation = steamPath + @"/steamapps/common/Techtonica";
                DebugUtils.SendDebugLine(gameLocation);
                
                Settings.userSettings.gameFolder = gameLocation;
                ProgramData.Paths.bepInExConfigFolder = $"{gameLocation}/BepInEx/config";
                ProgramData.Paths.bepInExPatchersFolder = $"{gameLocation}/BepInEx/patchers";
                ProgramData.Paths.bepInExPluginsFolder = $"{gameLocation}/BepInEx/plugins";
                Settings.Save();
                return true;
    }
            else {
                string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                foreach (char letter in letters) {
                    string steamPath = $"{letter}:/steam/steamapps/common/Techtonica/Techtonica.exe";
                    string xboxPath = $"{letter}:/XBoxGames/Techtonica/Content/Techtonica.exe";
            
                    if (File.Exists(xboxPath)) {
                        string newPath = $"{letter}:/XBoxGames/Techtonica/Content";
                        Settings.userSettings.gameFolder = newPath;
                        Settings.Save();
                        return true;
                    }
                    else if(File.Exists(steamPath)) {
                        string newPath = $"{letter}:/steam/steamapps/common/Techtonica";
                        Settings.userSettings.gameFolder = newPath;
                        Settings.Save();
                        return true;
                    }
                }
            }

            DebugUtils.SendDebugLine("Error: Failed to obtain game path.");
            return false;
        }
    }
}