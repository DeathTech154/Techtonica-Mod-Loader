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

        // ToDo: Move To ProgramData
        public string GameLocation; 
        public Brush LaunchVanillaDisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public string SelfLoc = Directory.GetParent(AppContext.BaseDirectory).FullName;
        //public UpdateInfoEventArgs UpdateArgs = new UpdateInfoEventArgs();

        // Events

        private async void OnProgramLoaded(object sender, RoutedEventArgs e) {
            loader.Visibility = Visibility.Visible;
            mainGrid.Visibility = Visibility.Hidden;

            DebugUtils.SendDebugLine("Logtest!");
            FileStructureUtils.CreateFolderStructure();
            LoadData();
            InitialiseGUI();

            if (!ProgramData.skipLoadingScreenDelay && ProgramData.isDebugBuild) {
                await Task.Delay(3000); // Let users bask in the glory of the loading screen
            }

            loader.Visibility = Visibility.Hidden;
            mainGrid.Visibility = Visibility.Visible;

            //string[] SplitSelfLoc = SelfLoc.Split("/");

            DebugUtils.SendDebugLine(SelfLoc);
            string NewLoc = RemoveFromBack(SelfLoc, 1, "\\");
            AutoUpdater.InstallationPath = NewLoc;
            DebugUtils.SendDebugLine(AutoUpdater.InstallationPath);
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
            ProgramData.DependancyStatus = Dependencies.Dependency.CheckDependencies();
            Dependencies.Dependency.HandleDependencies(ProgramData.DependancyStatus);
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
            this.Title = "Techtonica Mod Loader v"+ version;
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"); // Gets steam folder location from registry.
            if (key != null) {
                string SteamPath = (string)key.GetValue("SteamPath");
                key.Close();
                DebugUtils.SendDebugLine(SteamPath);
                GameLocation = SteamPath + @"/steamapps/common/Techtonica";
                DebugUtils.SendDebugLine(GameLocation);
                ProgramData.Paths.gameFolder = GameLocation;
            }
            else {
                DebugUtils.SendDebugLine("Error: Failed to obtain steam path. Disabling launch.");
                Button_Launch_Vanilla.IsEnabled = false;
            }
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
                    tagline = "Unknown mod installed from local file",
                    enabled = true,
                    iconLink = $"{ProgramData.Paths.resourcesFolder}/UnknownModIcon.png",
                    zipFileLocation = selectedZip
                };
                
                Profile profile = ProfileManager.GetActiveProfile();
                if (profile.HasMod(mod)) {
                    GuiUtils.ShowWarningMessage($"Already Added To Profile: {profile.name}", "You've already installed this mod and added it to this profile.");
                    return;
                }

                ModManager.AddOrUpdateMod(mod);
                profile.AddMod(mod);
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

        // ToDo: Elliot - Move elsewhere or delete, launching vanilla handled by profile
        private void OnButtonLaunchVanillaClicked(object sender, RoutedEventArgs e) {
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = e.ToString();
            start.FileName = "Techtonica.exe";
            start.WorkingDirectory = GameLocation;
            start.UseShellExecute = true;
            DebugUtils.SendDebugLine("Launching from directory: "+ start.WorkingDirectory);
            DebugUtils.SendDebugLine("Target: " + start.FileName);
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = false;
            int exitCode;
            DebugUtils.SendDebugLine("Launching game in Vanilla mode!");
            try {
                using (Process proc = Process.Start(start)) {
                    proc.WaitForExit();

                    // Retrieve the app's exit code
                    exitCode = proc.ExitCode;
                }
            }
            catch (Win32Exception ex) {

                DebugUtils.SendDebugLine(ex.Message);
                Button_Launch_Vanilla.IsEnabled = true;
                Button_Launch_Vanilla.Content = "Game not found!";
                Button_Launch_Vanilla.Background = LaunchVanillaDisabledBrush;
            }
        }

        // Public Functions

        // ToDo: Move To Utils.StringUtils
        public string RemoveFromBack(string Original, int Entries, string Seperator) {
            string[] Splited = Original.Split(Seperator);
            int SplitLength = Splited.Length;
            string NewString = "";
            int count = 0;
            foreach (string Split in Splited) {
                if (count == 0) {
                    NewString = NewString + Split;
                    count = count + 1;
                }
                else if (count < (SplitLength - Entries)) {
                    NewString = NewString + Seperator + Split;
                    count = count + 1;
                }
            }
            return NewString;
        }

        // Public Functions

        public void LoadInstalledModList() {
            ModListPanel panel = new ModListPanel();
            panel.LoadInstalledModList();
            mainBorder.Child = panel;
        }

        // Private Functions

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

        private void LoadData() {
            ModManager.Load();
            ProfileManager.Load();
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
