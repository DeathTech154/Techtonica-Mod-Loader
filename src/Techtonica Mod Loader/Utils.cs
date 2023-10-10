using AutoUpdaterDotNET;
using Microsoft.Win32;
using MyLogger;
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
using Techtonica_Mod_Loader.Classes;
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
            if(GetUserConfirmation("Open Link?", $"Do you want to open this link?\n{url}")) {
                ProcessStartInfo info = new ProcessStartInfo() {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(info);
            }
        }

        public static async Task<System.Windows.Controls.Image> GetImageFromURL(string url) {
            BitmapImage bitmapImage = new BitmapImage();
            if (ImageCache.IsImageCached(url)) {
                bitmapImage = new BitmapImage(new Uri(ImageCache.GetImagePath(url)));
            }
            else {
                try {
                    using (WebClient webClient = new WebClient()) {
                        byte[] imageData = await webClient.DownloadDataTaskAsync(new Uri(url));

                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(imageData);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        if (Settings.userSettings.cacheImages) {
                            ImageCache.CacheImage(url);
                        }
                    }
                }
                catch (Exception e) {
                    string error = $"Error loading image: {e.Message}";
                    Log.Error(error);
                    return null;
                }
                
            }

            System.Windows.Controls.Image image = new System.Windows.Controls.Image();
            image.Source = bitmapImage;
            image.Width = bitmapImage.PixelWidth;
            image.Height = bitmapImage.PixelHeight;
            image.Stretch = System.Windows.Media.Stretch.UniformToFill;
            return image;
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
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return null;
            }
        }

        public static void ShowDownloadingGui(Mod mod) {
            MainWindow.current.downloadingGUI.ShowForMod(mod);
            MainWindow.current.downloadingGUI.Visibility = System.Windows.Visibility.Visible;
        }

        public static void ShowInstallingGui() {
            MainWindow.current.downloadingGUI.SetToInstalling();
        }

        public static void HideInstallingGui() {
            MainWindow.current.downloadingGUI.Visibility = System.Windows.Visibility.Hidden;
        }
    }

    public static class FileStructureUtils
    {
        public static void CreateFolderStructure() {
            foreach(string folder in ProgramData.Paths.folders) {
                Directory.CreateDirectory(folder);
            }
        }

        public static void GenerateSVGFiles() {
            GenerateInfoSVG();
            GenerateWarningSVG();
            GenerateErrorSVG();
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

        public static bool FindGameFolder() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"); // Gets steam folder location from registry.
            if (key != null) {
                string steamPath = (string)key.GetValue("SteamPath");
                key.Close();
                Log.Debug($"Steam Path: {steamPath}");
                string gameLocation = steamPath + @"/steamapps/common/Techtonica";
                Log.Debug($"Game Folder: {gameLocation}");
                
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

            Log.Warning("Failed to obtain game path");
            return false;
        }

        // Private Functions

        private static void GenerateInfoSVG() {
            string path = $"{ProgramData.Paths.resourcesFolder}/Info.svg";
            if (File.Exists(path)) return;

            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Info.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""482.86467""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#ffffff;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566886,473.74379 256,42.214922 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#ffffff;fill-opacity:1;stroke:#ffffff;stroke-width:10.5445;stroke-opacity:1""
       x=""198.12881""
       y=""442.35748""
       id=""text2077""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""198.12881""
         y=""442.35748""
         style=""fill:#ffffff;fill-opacity:1;stroke:#ffffff;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
            File.WriteAllText(path, svg);
        }

        private static void GenerateWarningSVG() {
            string path = $"{ProgramData.Paths.resourcesFolder}/Warning.svg";
            if(File.Exists(path)) return;

            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Warning.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""482.86467""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#fff00f;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566878,473.74379 256,42.214924 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#fff00f;fill-opacity:1;stroke:#fff00f;stroke-width:10.5445;stroke-opacity:1""
       x=""-313.87119""
       y=""-133.64253""
       id=""text2077""
       transform=""scale(-1)""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""-313.87119""
         y=""-133.64253""
         style=""fill:#fff00f;fill-opacity:1;stroke:#fff00f;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
            File.WriteAllText(path, svg);
        }

        private static void GenerateErrorSVG() {
            string path = $"{ProgramData.Paths.resourcesFolder}/Error.svg";
            if(File.Exists(path)) return;

            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Error.svg""
   xmlns:inkscape=""http://www.inkscape.org/namespaces/inkscape""
   xmlns:sodipodi=""http://sodipodi.sourceforge.net/DTD/sodipodi-0.dtd""
   xmlns:xlink=""http://www.w3.org/1999/xlink""
   xmlns=""http://www.w3.org/2000/svg""
   xmlns:svg=""http://www.w3.org/2000/svg"">
  <sodipodi:namedview
     id=""namedview7""
     pagecolor=""#505050""
     bordercolor=""#eeeeee""
     borderopacity=""1""
     inkscape:pageshadow=""0""
     inkscape:pageopacity=""0""
     inkscape:pagecheckerboard=""0""
     inkscape:document-units=""px""
     showgrid=""false""
     units=""px""
     showguides=""true""
     inkscape:guide-bbox=""true""
     inkscape:zoom=""1.138""
     inkscape:cx=""481.98595""
     inkscape:cy=""235.94025""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""-8""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""0,45.980582""
       orientation=""-1,0""
       id=""guide989""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""-18.640777,0""
       orientation=""0,1""
       id=""guide991""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""512,0""
       orientation=""-1,0""
       id=""guide1069""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,535.72584""
       orientation=""-1,0""
       id=""guide1109""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""231.10721,443.405""
       orientation=""0,1""
       id=""guide1111""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"">
    <linearGradient
       inkscape:collect=""always""
       id=""linearGradient7077"">
      <stop
         style=""stop-color:#ff0000;stop-opacity:1;""
         offset=""0""
         id=""stop7073"" />
      <stop
         style=""stop-color:#ff0000;stop-opacity:0;""
         offset=""1""
         id=""stop7075"" />
    </linearGradient>
    <linearGradient
       inkscape:collect=""always""
       xlink:href=""#linearGradient7077""
       id=""linearGradient7079""
       x1=""-283.10266""
       y1=""-288.00001""
       x2=""-228.89734""
       y2=""-288.00001""
       gradientUnits=""userSpaceOnUse"" />
  </defs>
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <path
       style=""fill:none;fill-opacity:1;stroke:#ff0000;stroke-width:7.91742;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 6.8566886,473.74379 256,42.214922 505.14331,473.74379 Z""
       id=""path854""
       sodipodi:nodetypes=""cccc"" />
    <text
       xml:space=""preserve""
       style=""font-style:normal;font-weight:normal;font-size:421.78px;line-height:1.25;font-family:sans-serif;fill:#ff0000;fill-opacity:1;stroke:#ff0000;stroke-width:10.5445;stroke-opacity:1""
       x=""-313.87119""
       y=""-133.64253""
       id=""text2077""
       transform=""scale(-1)""><tspan
         sodipodi:role=""line""
         id=""tspan2075""
         x=""-313.87119""
         y=""-133.64253""
         style=""fill:#ff0000;fill-opacity:1;stroke:#ff0000;stroke-width:10.5445;stroke-opacity:1"">i</tspan></text>
  </g>
</svg>
";
            File.WriteAllText(path, svg);
        }
    }
}