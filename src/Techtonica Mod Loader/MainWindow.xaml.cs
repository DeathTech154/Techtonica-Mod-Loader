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
using Techtonica_Mod_Loader.Classes;
using Techtonica_Mod_Loader.Panels;
using System.Windows.Automation;
using MyLogger;

namespace Techtonica_Mod_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        // Objects & Variables

        public static MainWindow current => (MainWindow)Application.Current.MainWindow;

        // Events

        private async void OnProgramLoaded(object sender, RoutedEventArgs e) {
            InitialiseLogger();
            FileStructureUtils.CreateFolderStructure();
            FileStructureUtils.GenerateSVGFiles();

            await LoadData();
            if (string.IsNullOrEmpty(ProgramData.Paths.gameFolder)) {
                if (!FileStructureUtils.FindGameFolder()) {
                    GuiUtils.ShowWarningMessage("Couldn't Find Game Folder", "Please go to the settings and set your game folder before installing mods or launching the game.");
                }
            }

            string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            Title = "Techtonica Mod Loader v" + version;

            loader.Visibility = Visibility.Visible;
            mainGrid.Visibility = Visibility.Hidden;

            double widthDiff = Settings.userSettings.lastWidth - Width;
            Left -= widthDiff / 2.0;

            double topDiff = Settings.userSettings.lastHeight - Height;
            Height -= topDiff / 2.0;
            
            Width = Settings.userSettings.lastWidth;
            Height = Settings.userSettings.lastHeight;
            
            await ModManager.CheckForUpdates();
            InitialiseGUI();
            CheckForUpdates();

            if (!ProgramData.skipLoadingScreenDelay && ProgramData.isDebugBuild) {
                await Task.Delay(3000); // Let users bask in the glory of the loading screen
            }

            loader.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;
        }

        private void OnProgramClosing(object sender, CancelEventArgs e) {
            SaveData();
        }

        private void OnLaunchGameClicked(object sender, EventArgs e) {
            Process.Start($"{ProgramData.Paths.gameFolder}/Techtonica.exe");
        }

        private void OnInstallFromFileClicked(object sender, EventArgs e) {
            System.Windows.Forms.OpenFileDialog reader = new System.Windows.Forms.OpenFileDialog() {
                Filter = "Zip Files (*.zip)|*.zip"
            };
            if(reader.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string selectedZip = reader.FileName;
                string name = System.IO.Path.GetFileName(selectedZip).Replace(".zip", "");
                Mod mod = new Mod() {
                    id = $"com.localfile.{name}",
                    name = name,
                    tagLine = "Unknown mod installed from local file",
                    iconLink = "",
                    zipFileLocation = selectedZip
                };
                
                Profile profile = ProfileManager.GetActiveProfile();
                if (profile.HasMod(mod)) {
                    GuiUtils.ShowWarningMessage($"Already Added To Profile: {profile.name}", "You've already installed this mod and added it to this profile.");
                    return;
                }

                ModManager.AddIfNew(mod);
                mod.Install();

                GuiUtils.ShowWarningMessage("Check Mod Description", "Be sure to read the mod description when importing mods manually, as some contain dependencies or BepInEx settings changes in order to work properly, and may need to be manually configured");

                RefreshModList();
            }
        }

        private void OnSettingsClicked(object sender, EventArgs e) {
            mainBorder.Child = new SettingsPanel();
        }

        private void OnSelectedProfileChanged(object sender, EventArgs e) {
            Profile chosenProfile = ProfileManager.GetProfileByName(profilesBox.SelectedItem);
            ProfileManager.LoadProfile(chosenProfile);
            RefreshModList();
        }

        private void OnModsToShowChanged(object sender, EventArgs e) {
            RefreshModList();
        }

        private void OnSortOptionChanged(object sender, EventArgs e) {
            ProgramData.currentSortOption = StringUtils.GetModListSortOptionFromName(sortBox.SelectedItem);
            RefreshModList();
        }

        private void OnCheckForUpdatesClicked(object sender, RoutedEventArgs e) {
            CallUpdateWindow();
        }

        // Public Functions

        public void LoadDefaultModList() {
            ModListPanel panel = new ModListPanel();
            mainBorder.Child = panel;
            showingBox.SetSelectedItem(StringUtils.GetModListSourceName(Settings.userSettings.defaultModList));
            RefreshModList();
        }

        public void RefreshModList() {
            if (mainBorder.Child is ModListPanel panel) {
                ModListSource source = StringUtils.GetModListSourceFromName(showingBox.SelectedItem);
                switch (source) {
                    case ModListSource.Installed: panel.LoadInstalledModList(); break;
                    case ModListSource.NewMods: panel.LoadNewModsList(); break;
                    case ModListSource.Online: panel.LoadOnlineModList(); break;
                }
            }
        }

        // Private Functions

        private void InitialiseLogger() {
            Log.logPath = ProgramData.Paths.logFile;
            Log.logDebugToFile = Settings.userSettings.logDebugMessages || ProgramData.isDebugBuild;
        }

        private void CheckForUpdates() {
            string programDirectory = FileStructureUtils.GetProgramDirectory();
            Log.Debug($"Program Directory: {programDirectory}");

            string installPath = StringUtils.RemoveFromBack(programDirectory, 1, "\\");
            AutoUpdater.InstallationPath = installPath;
            Log.Debug($"Installation Path: '{installPath}'");
            
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
        }

        private void CallUpdateWindow() {
            AutoUpdater.Mandatory = true;
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
        }

        private void SaveData() {
            Settings.userSettings.lastWidth = ActualWidth;
            Settings.userSettings.lastHeight = ActualHeight;
            Settings.Save();

            ModManager.Save();
            ProfileManager.Save();
            Settings.Save();
            ImageCache.Save();
        }

        private async Task<string> LoadData() {
            Settings.Load();
            ModManager.Load();
            await ProfileManager.Load();
            ImageCache.Load();
            return "";
        }

        private void InitialiseGUI() {
            profilesBox.SetItems(ProfileManager.GetProfileNames());
            
            showingBox.SetItems(StringUtils.GetAllModListSourceNames());
            showingBox.SetSelectedItem(StringUtils.GetModListSourceName(Settings.userSettings.defaultModList));
            
            sortBox.SetItems(StringUtils.GetAllModListSortOptionNames());
            sortBox.SetSelectedItem(StringUtils.GetModListSortOptionName(Settings.userSettings.defaultSort));

            LoadDefaultModList();
        }
    }
}
