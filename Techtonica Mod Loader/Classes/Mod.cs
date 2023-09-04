using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shell;
using Techtonica_Mod_Loader.Classes.Globals;
using Techtonica_Mod_Loader.Panels;

namespace Techtonica_Mod_Loader.Classes
{
    public class Mod
    {
        public string id;
        public string name;
        public ModVersion version;
        public string tagline;
        public string description;

        public string link;
        public string iconLink;
        public string bannerLink;
        public string screenshot1Link;
        public string screenshot2Link;
        public string screenshot3Link;
        public string screenshot4Link;

        public string zipFileDownloadLink;
        public string zipFileLocation;
        public string configFileLocation;
        public List<string> installedFiles = new List<string>();
        public bool enabled;
        public bool canBeToggled = true;

        // Public Functions

        public void Download() {
            try {
                string targetLocation = $"{ProgramData.Paths.modsFolder}\\{name}.zip";

                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Dependencies.Dependency.DownloadFileCallback);
                webClient.DownloadFileAsync(new Uri(zipFileDownloadLink), targetLocation);
            }
            catch (Exception e){
                string error = $"Error occurred while downloading file: {e.Message}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                // return ProgramData.EnumDownloadStatus.FAIL;
            }
            //return ProgramData.EnumDownloadStatus.DOWNLOADING;
        }

        public void Install() {
            if (!CheckForZipFile() || !CheckGameFolder()) {
                return;
            }

            CopyLocalFileToModsFolder();
            FileStructureUtils.ClearUnzipFolder();
            UnzipToTempFolder();
            installedFiles = FileStructureUtils.CopyFolder(ProgramData.Paths.unzipFolder, ProgramData.Paths.gameFolder);
            ModManager.UpdateModDetails(this);
            FileStructureUtils.ClearUnzipFolder();
        }

        public void Uninstall() {
            List<string> foldersToDelete = new List<string>();

            foreach(string file in installedFiles) {
                if (File.Exists(file)) {
                    if (DoesFileHaveNamedParentFolder(file)) {
                        foldersToDelete.Add(Path.GetDirectoryName(file));
                    }

                    File.Delete(file);
                }
            }

            foldersToDelete = foldersToDelete.Distinct().ToList();
            foreach(string folder in foldersToDelete) {
                Directory.Delete(folder);
            }

            installedFiles.Clear();
        }

        public bool IsLocal() {
            return string.IsNullOrEmpty(link);
        }

        public bool HasConfigFile() {
            return !string.IsNullOrEmpty(configFileLocation);
        }

        // Private Functions

        private bool CheckForZipFile() {
            if(File.Exists(zipFileLocation)) {
                return true;
            }

            string error = $"Could not install mod '{name}' - Cannot find zip file";
            DebugUtils.SendDebugLine(error);
            DebugUtils.CrashIfDebug(error);

            GuiUtils.ShowErrorMessage("Couldn't Install Mod", $"Sorry, TML couldn't install this mod as it cannot find it at '{zipFileLocation}'.\n");
            if (GuiUtils.GetUserConfirmation("Manually Find File?", "Would you like to manually look for this file?")) {
                System.Windows.Forms.OpenFileDialog reader = new System.Windows.Forms.OpenFileDialog() {
                    Filter = "Zip Files (*.zip)|*.zip"
                };
                if (reader.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    zipFileLocation = reader.FileName;
                    return true;
                }
            }

            return false;
        }

        private bool CheckGameFolder() {
            if (!string.IsNullOrEmpty(ProgramData.Paths.gameFolder)) return true;

            string error = $"Couldn't install mod - ProgramData.Paths.gameFolder is null or empty";
            DebugUtils.SendDebugLine(error);
            DebugUtils.CrashIfDebug(error);

            GuiUtils.ShowErrorMessage("Couldn't Install Mod", "You need to set your game folder location in the settings.");
            return false;
        }

        private void CopyLocalFileToModsFolder() {
            if (!IsLocal()) return;
            if (zipFileLocation.Contains(ProgramData.Paths.modsFolder)) return;

            string source = Path.GetDirectoryName(zipFileLocation);
            string newPath = zipFileLocation.Replace(source, ProgramData.Paths.modsFolder);
            if (File.Exists(newPath)) {
                File.Delete(newPath);
            }

            File.Copy(zipFileLocation, newPath);
            zipFileLocation = newPath;
        }

        private void UnzipToTempFolder() {
            using (ZipArchive archive = ZipFile.OpenRead(zipFileLocation)) {
                foreach (ZipArchiveEntry entry in archive.Entries) {
                    string entryFilePath = Path.Combine(ProgramData.Paths.unzipFolder, entry.FullName);
                    Directory.CreateDirectory(Path.GetDirectoryName(entryFilePath));
                    if (!entryFilePath.EndsWith("/")) {
                        entry.ExtractToFile(entryFilePath, true);
                        if(entryFilePath.EndsWith(".cfg")) {
                            configFileLocation = entryFilePath.Replace(ProgramData.Paths.unzipFolder, ProgramData.Paths.gameFolder);
                        }
                    }
                }
            }
        }

        private bool DoesFileHaveNamedParentFolder(string file) {
            string parentFolder = Path.GetDirectoryName(file).Split('\\').Last();
            return parentFolder != "plugins" && parentFolder != "config";
        }
    }

    public struct ModVersion {
        public int major;
        public int minor;
        public int build;

        public static ModVersion Parse(string input) {
            try {
                string[] parts = input.Split('.');
                return new ModVersion() {
                    major = int.Parse(parts[0]),
                    minor = int.Parse(parts[1]),
                    build = int.Parse(parts[2]),
                };
            }
            catch (Exception e) {
                string error = $"Error occurred while parsing Version '{input}': {e.Message}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return new ModVersion() {
                    major = 0,
                    minor = 0,
                    build = 0
                };
            }
        }

        public override string ToString() {
            return $"{major}.{minor}.{build}";
        }
    }
}
