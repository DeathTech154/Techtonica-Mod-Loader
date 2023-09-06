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
using Techtonica_Mod_Loader.Classes.Globals;
using Techtonica_Mod_Loader.Classes;
using Techtonica_Mod_Loader.Panels;
using System.Windows.Automation;

namespace Techtonica_Mod_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Objects & Variables

        public static MainWindow current => (MainWindow)Application.Current.MainWindow;

        // Events

        private async void OnProgramLoaded(object sender, RoutedEventArgs e) {
            string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            Title = "Techtonica Mod Loader v" + version;

            loader.Visibility = Visibility.Visible;
            mainGrid.Visibility = Visibility.Hidden;

            FileStructureUtils.FindGameLocation();
            FileStructureUtils.CreateFolderStructure();
            await LoadData();
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
                    enabled = true,
                    iconLink = $"{ProgramData.Paths.resourcesFolder}/UnknownModIcon.png",
                    zipFileLocation = selectedZip
                };
                
                Profile profile = ProfileManager.GetActiveProfile();
                if (profile.HasMod(mod)) {
                    GuiUtils.ShowWarningMessage($"Already Added To Profile: {profile.name}", "You've already installed this mod and added it to this profile.");
                    return;
                }

                ModManager.AddIfNew(mod);
                mod.Install();

                RefreshModList();
            }
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
            // ToDo: Elliot - Sort Mods List
        }

        private void OnCheckForUpdatesClicked(object sender, RoutedEventArgs e) {
            CallUpdateWindow();
        }

        // Public Functions

        public void LoadInstalledModList() {
            ModListPanel panel = new ModListPanel();
            panel.LoadInstalledModList();
            mainBorder.Child = panel;
        }

        // Private Functions

        private void CheckForUpdates() {
            string programDirectory = FileStructureUtils.GetProgramDirectory();
            DebugUtils.SendDebugLine(programDirectory);
            string installPath = StringUtils.RemoveFromBack(programDirectory, 1, "\\");
            AutoUpdater.InstallationPath = installPath;
            DebugUtils.SendDebugLine(AutoUpdater.InstallationPath);
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
            ModManager.Save();
            ProfileManager.Save();
        }

        private async Task<string> LoadData() {
            ModManager.Load();
            await ProfileManager.Load();
            return "";
        }

        private void InitialiseGUI() {
            profilesBox.SetItems(ProfileManager.GetProfileNames());
            showingBox.SetItems(StringUtils.GetAllModListSourceNames());
            sortBox.SetItems(StringUtils.GetAllModListSortOptionNames());

            LoadInstalledModList();
        }

        private void RefreshModList() {
            if(mainBorder.Child is ModListPanel panel) {
                ModListSource source = StringUtils.GetModListSourceFromName(showingBox.SelectedItem);
                switch (source) {
                    case ModListSource.Installed: panel.LoadInstalledModList(); break;
                    case ModListSource.Online: panel.LoadOnlineModList(); break;
                }
            }
        }
    }
}
