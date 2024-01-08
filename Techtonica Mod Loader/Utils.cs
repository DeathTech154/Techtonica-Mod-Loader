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
            
            GenerateDownloadSVG();

            GenerateUpdateSVG();
            GenerateDonateSVG();
            GenerateConfigureSVG();
            GenerateViewModPageSVG();
            GenerateDeleteSVG();
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

        public static bool VerifyGameFolder() {
            if (string.IsNullOrEmpty(Settings.userSettings.gameFolder)) return false;
            else return File.Exists($"{Settings.userSettings.gameFolder}/Techtonica.exe");
        }

        // Private Functions

        private static void GenerateSVG(string name, string svg) {
            string path = $"{ProgramData.Paths.resourcesFolder}/{name}.svg";
            if (File.Exists(path)) return;
            File.WriteAllText(path, svg);
        }

        private static void GenerateInfoSVG() {
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
            GenerateSVG("Info", svg);
        }

        private static void GenerateWarningSVG() {
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
            GenerateSVG("Warning", svg);
        }

        private static void GenerateErrorSVG() {
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
            GenerateSVG("Error", svg);
        }
        
        private static void GenerateUpdateSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Update.svg""
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
     inkscape:zoom=""1.1739077""
     inkscape:cx=""126.50057""
     inkscape:cy=""289.20501""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
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
       id=""rect843""
       style=""fill:#ffff16;stroke:none;stroke-width:2.85941;stroke-linecap:square;paint-order:fill markers stroke;fill-opacity:1""
       d=""m 256.0521,1.4393963 178.89297,178.8929737 2e-5,0.10449 -357.890451,-1e-5 v -0.10451 L 255.94761,1.4393715 c 0.0289,-0.028935 0.0755,-0.02897 0.10449,2.12e-5 z""
       sodipodi:nodetypes=""cccccssc"" />
    <rect
       style=""fill:#ffff16;stroke:none;stroke-width:2.87977;stroke-linecap:square;paint-order:fill markers stroke;fill-opacity:1""
       id=""rect1306""
       width=""125.12022""
       height=""330.09476""
       x=""193.4399""
       y=""180.47275""
       ry=""0.076415896"" />
  </g>
</svg>
";
            GenerateSVG("Update", svg);
        }

        private static void GenerateDonateSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Donate.svg""
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
     inkscape:zoom=""1.1739077""
     inkscape:cx=""56.648404""
     inkscape:cy=""324.98295""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
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
    <g
       aria-label=""♥""
       id=""text6761""
       style=""font-size:835.519px;line-height:1.25;stroke-width:20.888;stroke:none;fill:#ffff16;fill-opacity:1"">
      <path
         d=""M 256.81594,512.00007 Q 244.16892,463.85982 220.50676,421.83909 197.25257,379.41039 129.93781,290.47331 80.573651,225.19839 69.15054,207.65575 50.384001,179.09798 41.816667,155.43582 q -8.159365,-24.07013 -8.159365,-48.54823 0,-45.284471 30.189651,-75.88209 30.189651,-30.59761928 74.658187,-30.59761928 44.87651,0 77.92194,31.82152328 24.88606,23.662159 40.38886,70.578506 Q 270.27889,56.7075 294.75698,32.637373 328.61835,-8.7529421e-5 373.08689,-8.7529421e-5 q 44.06057,0 74.65819,30.597618529421 30.59762,30.189651 30.59762,72.210379 0,36.71714 -17.95061,76.69803 -17.9506,39.57292 -69.3546,104.03191 -66.90679,84.44943 -97.50441,138.7092 -24.07013,42.83667 -36.71714,89.75302 z""
         id=""path7437""
         style=""stroke:none;fill:#ffff16;fill-opacity:1"" />
    </g>
  </g>
</svg>
";
            GenerateSVG("Donate", svg);
        }

        private static void GenerateConfigureSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Configure.svg""
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
     inkscape:zoom=""0.83007813""
     inkscape:cx=""-216.24471""
     inkscape:cy=""470.43765""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""207.00093,384""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
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
    <g
       id=""g8999""
       transform=""matrix(1.6,0,0,1.6,-153.6,-153.6)"">
      <circle
         style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:26.9135;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
         id=""path7876""
         cx=""256""
         cy=""256""
         r=""107.65385"" />
      <g
         id=""g8090"">
        <rect
           style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980""
           width=""64""
           height=""64""
           x=""224""
           y=""96""
           ry=""0.15772727"" />
        <rect
           style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-9""
           width=""64""
           height=""64""
           x=""224""
           y=""352""
           ry=""0.15772727"" />
      </g>
      <g
         id=""g8194"">
        <g
           id=""g8090-6""
           transform=""rotate(30,256,256)"">
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
        <g
           id=""g8090-6-1""
           transform=""rotate(60,256,256)"">
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-4""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-3""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
      </g>
      <g
         id=""g8194-1""
         transform=""matrix(-1,0,0,1,512,0)"">
        <g
           id=""g8090-6-6""
           transform=""rotate(30,256,256)"">
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-3""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-2""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
        <g
           id=""g8090-6-1-9""
           transform=""rotate(60,256,256)"">
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-7-4-5""
             width=""64""
             height=""64""
             x=""224""
             y=""96""
             ry=""0.15772727"" />
          <rect
             style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
             id=""rect7980-9-7-3-0""
             width=""64""
             height=""64""
             x=""224""
             y=""352""
             ry=""0.15772727"" />
        </g>
      </g>
      <g
         id=""g8090-6-1-3""
         transform=""rotate(90,256,256)"">
        <rect
           style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-7-4-4""
           width=""64""
           height=""64""
           x=""224""
           y=""96""
           ry=""0.15772727"" />
        <rect
           style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:5.49667;stroke-linecap:square;paint-order:fill markers stroke""
           id=""rect7980-9-7-3-8""
           width=""64""
           height=""64""
           x=""224""
           y=""352""
           ry=""0.15772727"" />
      </g>
    </g>
  </g>
</svg>
";
            GenerateSVG("Configure", svg);
        }

        private static void GenerateViewModPageSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""ViewModPage.svg""
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
     inkscape:zoom=""1.1739077""
     inkscape:cx=""99.241189""
     inkscape:cy=""267.05676""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""207.00093,384""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""314.42824,673.43059""
       orientation=""0,-1""
       id=""guide9987"" />
    <sodipodi:guide
       position=""360.80941,455.98118""
       orientation=""0,-1""
       id=""guide10173"" />
    <sodipodi:guide
       position=""181.91059,414.41882""
       orientation=""1,0""
       id=""guide10175"" />
    <sodipodi:guide
       position=""324.06588,414.41882""
       orientation=""1,0""
       id=""guide10177"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <g
       id=""g13340"">
      <g
         id=""g13286"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-opacity:1;stroke-miterlimit:4;stroke-dasharray:none""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5""
           sodipodi:nodetypes=""cc"" />
      </g>
      <g
         id=""g13286-0""
         transform=""matrix(1,0,0,-1,0,512)"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-4""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-2"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-9""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-8""
           sodipodi:nodetypes=""cc"" />
      </g>
    </g>
    <g
       id=""g13340-2""
       transform=""rotate(90,256,256)"">
      <g
         id=""g13286-7"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-7""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-1"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-7""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-83""
           sodipodi:nodetypes=""cc"" />
      </g>
      <g
         id=""g13286-0-5""
         transform=""matrix(1,0,0,-1,0,512)"">
        <circle
           style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:15.938;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
           id=""path7876-4-8""
           cx=""256""
           cy=""256""
           r=""248.03113"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 8.4329412,256 H 503.56706""
           id=""path10022-2-5"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""M 43.971765,128 C 184.92235,56.01882 327.07764,56.01882 468.02823,128""
           id=""path10076-9-1""
           sodipodi:nodetypes=""cc"" />
        <path
           style=""fill:none;stroke:#ffff16;stroke-width:16.9737;stroke-linecap:butt;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
           d=""m 17.381149,184.06667 c 158.627301,-53.30824 318.610391,-53.30824 477.237701,0""
           id=""path10076-5-8-3""
           sodipodi:nodetypes=""cc"" />
      </g>
    </g>
  </g>
</svg>
";
            GenerateSVG("ViewModPage", svg);
        }

        private static void GenerateDeleteSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Delete.svg""
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
     inkscape:zoom=""0.83007813""
     inkscape:cx=""143.96235""
     inkscape:cy=""366.83294""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,256""
       orientation=""0,1""
       id=""guide1388""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""256,319.48061""
       orientation=""0,1""
       id=""guide8062""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""386.10824,128""
       orientation=""0,1""
       id=""guide8086""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""314.42824,673.43059""
       orientation=""0,-1""
       id=""guide9987"" />
    <sodipodi:guide
       position=""440.01782,432.03564""
       orientation=""0,-1""
       id=""guide10173"" />
    <sodipodi:guide
       position=""99.27496,392.5591""
       orientation=""-1,0""
       id=""guide10175""
       inkscape:label=""""
       inkscape:locked=""false""
       inkscape:color=""rgb(0,0,255)"" />
    <sodipodi:guide
       position=""324.06588,414.41882""
       orientation=""1,0""
       id=""guide10177"" />
  </sodipodi:namedview>
  <defs
     id=""defs2"" />
  <g
     inkscape:label=""Layer 1""
     inkscape:groupmode=""layer""
     id=""layer1"">
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:18.384;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13584""
       width=""249.45009""
       height=""383.3671""
       x=""131.27496""
       y=""119.4409""
       ry=""0.089272805"" />
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:16.7024;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13688""
       width=""385.0488""
       height=""50.256187""
       x=""63.475605""
       y=""59.992672""
       ry=""0.082174592"" />
    <rect
       style=""fill:none;fill-opacity:1;stroke:#ffff16;stroke-width:13.9318;stroke-linecap:square;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1;paint-order:fill markers stroke""
       id=""rect13688-3""
       width=""253.9023""
       height=""53.026787""
       x=""129.04886""
       y=""6.9658899""
       ry=""0.086704835"" />
    <path
       style=""fill:none;stroke:#ffff16;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 256,177.75078 V 445.58486""
       id=""path13825"" />
    <path
       style=""fill:none;stroke:#ffff16;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 327.21236,177.75078 V 445.58486""
       id=""path13825-7"" />
    <path
       style=""fill:none;stroke:#ffff16;stroke-width:16.7396;stroke-linecap:round;stroke-linejoin:miter;stroke-miterlimit:4;stroke-dasharray:none;stroke-opacity:1""
       d=""M 178.48567,177.75078 V 445.58486""
       id=""path13825-7-3"" />
  </g>
</svg>
";
            GenerateSVG("Delete", svg);
        }

        private static void GenerateDownloadSVG() {
            string svg = @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>
<!-- Created with Inkscape (http://www.inkscape.org/) -->

<svg
   width=""512""
   height=""512""
   viewBox=""0 0 512 512""
   version=""1.1""
   id=""svg5""
   inkscape:version=""1.1.1 (3bf5ae0d25, 2021-09-20)""
   sodipodi:docname=""Download.svg""
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
     inkscape:zoom=""1.1739077""
     inkscape:cx=""126.50057""
     inkscape:cy=""289.20501""
     inkscape:window-width=""2560""
     inkscape:window-height=""1009""
     inkscape:window-x=""1912""
     inkscape:window-y=""-8""
     inkscape:window-maximized=""1""
     inkscape:current-layer=""layer1"">
    <sodipodi:guide
       position=""434.94509,331.56314""
       orientation=""0,-1""
       id=""guide1388"" />
    <sodipodi:guide
       position=""256,311.38798""
       orientation=""-1,0""
       id=""guide1390""
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
    <g
       id=""g1456""
       transform=""matrix(1,0,0,-1,0,511.98516)"">
      <path
         id=""rect843""
         style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:2.85941;stroke-linecap:square;paint-order:fill markers stroke""
         d=""m 256.0521,1.4393963 178.89297,178.8929737 2e-5,0.10449 -357.890451,-1e-5 v -0.10451 L 255.94761,1.4393715 c 0.0289,-0.028935 0.0755,-0.02897 0.10449,2.12e-5 z""
         sodipodi:nodetypes=""cccccssc"" />
      <rect
         style=""fill:#ffff16;fill-opacity:1;stroke:none;stroke-width:2.87977;stroke-linecap:square;paint-order:fill markers stroke""
         id=""rect1306""
         width=""125.12022""
         height=""330.09476""
         x=""193.4399""
         y=""180.47275""
         ry=""0.076415896"" />
    </g>
  </g>
</svg>
";
            GenerateSVG("Download", svg);
        }
    }
}