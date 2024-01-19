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
using System.Windows.Threading;

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
        private DispatcherTimer processCheckTimer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };

        // Events

        private async void OnProgramLoaded(object sender, RoutedEventArgs e) {
            loader.Visibility = Visibility.Visible;
            mainGrid.Visibility = Visibility.Hidden;

            InitialiseLogger();
            FileStructureUtils.CreateFolderStructure();
            FileStructureUtils.GenerateSVGFiles();

            bool foundGameFolder = true;
            await LoadData();
            if (string.IsNullOrEmpty(ProgramData.Paths.gameFolder)) {
                if (!FileStructureUtils.FindGameFolder()) {
                    GuiUtils.ShowWarningMessage("Couldn't Find Game Folder", "Please go to the settings and set your game folder before installing mods or launching the game.");
                }
                else {
                    foundGameFolder = true;
                }
            }
            else {
                foundGameFolder = true;
            }

            string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            Title = "Techtonica Mod Loader v" + version;

            double widthDiff = Settings.userSettings.lastWidth - Width;
            Left -= widthDiff / 2.0;

            double topDiff = Settings.userSettings.lastHeight - Height;
            Height -= topDiff / 2.0;
            
            Width = Settings.userSettings.lastWidth;
            Height = Settings.userSettings.lastHeight;

            await ModManager.CheckForUpdates();
            InitialiseGUI();

            if (!foundGameFolder) {
                OpenSettingsWindow(); 
            }
            
            CheckForUpdates();

            processCheckTimer.Tick += OnProcessCheckTimerTick;
            processCheckTimer.Start();

            if (!ProgramData.skipLoadingScreenDelay && ProgramData.isDebugBuild) {
                await Task.Delay(3000); // Let users bask in the glory of the loading screen
            }

            loader.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;
        }

        private void OnProgramClosing(object sender, CancelEventArgs e) {
            SaveData();
        }

        private void OnProcessCheckTimerTick(object sender, EventArgs e) {
            Process[] techtonicaProcess = Process.GetProcessesByName("Techtonica");
            if (techtonicaProcess.Length != 0) {
                launchGameButton.IsEnabled = false;
                launchGameButton.ButtonText = "Game Running";
            }
            else {
                launchGameButton.IsEnabled = true;
                launchGameButton.ButtonText = "Launch Game";
            }
        }

        private void OnLaunchGameClicked(object sender, EventArgs e) {
            SetHideGameManagerObject();
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
            OpenSettingsWindow();
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

        private void OnSearchBarKeyPressed(object sender, EventArgs e) {
            if(mainBorder.Child is ModListPanel panel) {
                panel.SearchModsList(searchBar.Input);
            }
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

        private void OpenSettingsWindow() {
            mainBorder.Child = new SettingsPanel();
            mainBorder.SetValue(Grid.RowProperty, 0);
            mainBorder.SetValue(Grid.RowSpanProperty, 2);
        }

        private void SetHideGameManagerObject() {
            string bepinexConfigPath = $"{ProgramData.Paths.bepInExConfigFolder}/BepInEx.cfg";
            if(!File.Exists(bepinexConfigPath)) {
                Log.Error("Could not find BepInEx config file.");
                return;
            }

            string text = File.ReadAllText(bepinexConfigPath);
            text = text.Replace("HideManagerGameObject = false", "HideManagerGameObject = true");
            File.WriteAllText(bepinexConfigPath, text);
        }
    }
}
