using MyLogger;
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
using System.Transactions;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shell;
using Techtonica_Mod_Loader.Classes.ThunderStoreResponses;
using Techtonica_Mod_Loader.Panels;

namespace Techtonica_Mod_Loader.Classes
{
    public class Mod
    {
        public string id;
        public string name;
        public string author;
        public ModVersion version;
        public string tagLine;
        public string description;
        public DateTime dateUpdated;
        public int ratingScore;
        public int downloads;
        public bool isDeprecated;
        public bool hasDonated;
        public bool updateAvailable;
        public List<string> categories = new List<string>();
        public List<string> dependencies = new List<string>();

        public string link;
        public string iconLink;
        public string donationLink;
        public string zipFileDownloadLink;

        public string zipFileLocation;
        public string configFileLocation;
        public string markdownFileLocation;
        public List<string> installedFiles = new List<string>();

        public bool canBeToggled = true;

        // Constructors

        public Mod(){}
        public Mod(ThunderStoreMod thunderStoreMod) {
            id = thunderStoreMod.uuid4;
            name = thunderStoreMod.name;
            author = thunderStoreMod.owner;
            dateUpdated = DateTime.Parse(thunderStoreMod.date_updated);
            ratingScore = thunderStoreMod.rating_score;
            isDeprecated = thunderStoreMod.is_deprecated;
            categories = thunderStoreMod.categories;

            link = thunderStoreMod.package_url;
            donationLink = thunderStoreMod.donation_link;

            if (thunderStoreMod.versions.Count > 0) {
                ThunderStoreVerion versionInfo = thunderStoreMod.versions[0];
                version = ModVersion.Parse(versionInfo.version_number);
                tagLine = versionInfo.description;
                dependencies = versionInfo.dependencies;
                
                iconLink = versionInfo.icon;
                zipFileDownloadLink = versionInfo.download_url;
            }

            downloads = 0;
            foreach(ThunderStoreVerion versionInfo in thunderStoreMod.versions) {
                downloads += versionInfo.downloads;
            }
        }

        // Custom Events

        public event EventHandler FinishedDownloading;

        // Events

        private void OnDownloadFinished(object sender, AsyncCompletedEventArgs e) {
            Install();
            ProgramData.isDownloading = false;
            FinishedDownloading?.Invoke(this, EventArgs.Empty);
        }

        private void OnTempDownloadFinished(object sender, AsyncCompletedEventArgs e) {
            FileStructureUtils.ClearUnzipFolder();
            zipFileLocation = ProgramData.Paths.tempZipFile;
            UnzipToTempFolder();

            ProgramData.isDownloading = false;
            FinishedDownloading?.Invoke(this, EventArgs.Empty);
        }

        // Public Functions

        public async Task<string> Download() {
            ProgramData.isDownloading = true;
            if (dependencies.Count != 0) {
                foreach(string dependency in dependencies) {
                    Mod mod = await ThunderStore.SearchForMod(dependency);
                    if(mod == null) {
                        GuiUtils.ShowErrorMessage("Couldn't Download", "Failed to find a dependency for this mod. Aborting install.");
                        return "";
                    }

                    ModManager.AddIfNew(mod);
                    ModManager.Save();
                    await mod.Download();
                }
            }

            try {
                zipFileLocation = $"{ProgramData.Paths.modsFolder}\\{name}.zip";

                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileCompleted += OnDownloadFinished;
                webClient.DownloadFileAsync(new Uri(zipFileDownloadLink), zipFileLocation);

                while (ProgramData.isDownloading) {
                    await Task.Delay(10);
                }
            }
            catch (Exception e){
                string error = $"Error occurred while downloading file: {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }

            return "";
        }

        public async void DownloadAsTemp() {
            while (ProgramData.isDownloading) {
                await Task.Delay(10);
            }

            try {
                ProgramData.isDownloading = true;
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileCompleted += OnTempDownloadFinished;
                webClient.DownloadFileAsync(new Uri(zipFileDownloadLink), ProgramData.Paths.tempZipFile);

                while (ProgramData.isDownloading) {
                    await Task.Delay(10);
                }
            }
            catch (Exception e) {
                string error = $"Error occurred while downloading file: {e.Message}";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                ProgramData.isDownloading = false;
            }
        }

        public void Install() {
            if (!CheckForZipFile()) {
                GuiUtils.ShowErrorMessage("ZIP Fle Not Found", "Could not locate the .zip file for this mod.");
                GuiUtils.HideInstallingGui();
                return;
            }

            if (!CheckGameFolder()) {
                GuiUtils.ShowErrorMessage("Game Folder Incorrect", "Could not locate Techtonica.exe inside your game folder. Please check it is correct in the settings.");
                GuiUtils.HideInstallingGui();
                return;
            }

            if (id != ProgramData.bepInExID) {
                GuiUtils.ShowInstallingGui();
            }

            CopyLocalFileToModsFolder();
            FileStructureUtils.ClearUnzipFolder();
            UnzipToTempFolder();
            
            if(id != ProgramData.bepInExID) {
                if(FileStructureUtils.SearchForConfigFile(ProgramData.Paths.unzipFolder, out configFileLocation)) {
                    string newPath = configFileLocation.Replace(Path.GetDirectoryName(configFileLocation), ProgramData.Paths.bepInExConfigFolder);
                    if (File.Exists(newPath)) {
                        File.Delete(newPath);
                    }

                    File.Copy(configFileLocation, newPath);
                    configFileLocation = newPath;
                    installedFiles.Add(configFileLocation);
                }

                if (FileStructureUtils.SearchForMarkdownFile(ProgramData.Paths.unzipFolder, out markdownFileLocation)) {
                    string newPath = markdownFileLocation.Replace(Path.GetDirectoryName(markdownFileLocation), ProgramData.Paths.markdownFiles);
                    newPath = newPath.Replace("README", name);
                    if (File.Exists(newPath)) {
                        File.Delete(newPath);
                    }

                    File.Copy(markdownFileLocation, newPath);
                    markdownFileLocation = newPath;
                    installedFiles.Add(markdownFileLocation);
                }

                string pluginsFolder = $"{ProgramData.Paths.unzipFolder}/plugins";
                string patchersFolder = $"{ProgramData.Paths.unzipFolder}/patchers";
                bool hasPluginFiles = Directory.Exists(pluginsFolder);
                bool hasPatcherFiles = Directory.Exists(patchersFolder);

                if (hasPluginFiles) {
                    string targetFolder = $"{ProgramData.Paths.bepInExPluginsFolder}\\{name}";
                    Directory.CreateDirectory(targetFolder);
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(pluginsFolder);
                    InstallFiles(dllFiles, targetFolder);
                }

                if (hasPatcherFiles) {
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(patchersFolder);
                    InstallFiles(dllFiles, ProgramData.Paths.bepInExPatchersFolder);
                }

                if (!hasPluginFiles && !hasPatcherFiles){
                    List<string> dllFiles = FileStructureUtils.SearchForDllFiles(ProgramData.Paths.unzipFolder);
                    InstallFiles(dllFiles, ProgramData.Paths.bepInExPluginsFolder);
                }

                FileStructureUtils.ClearUnzipFolder();
            }
            else {
                canBeToggled = false;
                installedFiles = FileStructureUtils.CopyFolder($"{ProgramData.Paths.unzipFolder}/BepInExPack", ProgramData.Paths.gameFolder);
                FileStructureUtils.ClearUnzipFolder();
            }

            ModManager.UpdateModDetails(this);
            Profile profile = ProfileManager.GetActiveProfile();
            if (!profile.HasMod(id)) profile.AddMod(id);
            ProfileManager.UpdateProfile(profile);
            MainWindow.current.RefreshModList();
            GuiUtils.HideInstallingGui();
        }

        public void Uninstall() {
            if (name == "BepInExPack") return; 
            List<string> foldersToDelete = new List<string>();

            foreach(string file in installedFiles) {
                if (file.EndsWith(".md")) continue;
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
            return !string.IsNullOrEmpty(configFileLocation) && 
                    configFileLocation != "Not Found" &&
                    name != "BepInExPack";
        }

        public bool HasMarkdownFile() {
            return !string.IsNullOrEmpty(markdownFileLocation) &&
                    markdownFileLocation != "Not Found";
        }

        // Private Functions

        private bool CheckForZipFile() {
            if(File.Exists(zipFileLocation)) {
                return true;
            }

            string error = $"Could not install mod '{name}' - Cannot find zip file";
            Log.Error(error);
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
            if (FileStructureUtils.FindGameFolder()) return true;

            string error = $"Couldn't install mod - ProgramData.Paths.gameFolder is null or empty";
            Log.Error(error);
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
            List<string> validParents = new List<string>() {
                "patchers",
                "plugins",
                "config",
                "MarkdownFiles"
            };
            return !validParents.Contains(parentFolder);
        }

        private void InstallFiles(List<string> files, string targetFolder) {
            foreach (string file in files) {
                string newPath = file.Replace("/", "\\").Replace(Path.GetDirectoryName(file), targetFolder);
                if (File.Exists(newPath)) {
                    File.Delete(newPath);
                }

                File.Copy(file, newPath);
                installedFiles.Add(newPath);
            }
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
                Log.Error(error);
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

        public static bool operator >(ModVersion v1, ModVersion v2) {
            if (v1.major > v2.major) return true;
            if (v1.major < v2.major) return false;

            if (v1.minor > v2.minor) return true;
            if (v1.minor < v2.minor) return false;

            return v1.build > v2.build;
        }

        public static bool operator <(ModVersion v1, ModVersion v2) {
            if (v1.major < v2.major) return true;
            if (v1.major > v2.major) return false;

            if (v1.minor < v2.minor) return true;
            if (v1.minor > v2.minor) return false;

            return v1.build < v2.build;
        }
    }
}
