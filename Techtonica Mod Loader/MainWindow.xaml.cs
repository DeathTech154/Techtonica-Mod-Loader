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

        // DeathTech: TODO: Push to seperate file.
        // LocalPath = "C:/Filename.ext"
        public static ProgramData.EnumDownloadStatus DownloadFile(string RemoteURL, string LocalPath)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.Headers.Add("Accept: text/html, application/xhtml+xml, */*");
                webClient.Headers.Add("User-Agent: Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
                webClient.DownloadFileAsync(new Uri(RemoteURL), LocalPath);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Dependencies.Dependency.DownloadFileCallback);
            }
            catch
            {
                return ProgramData.EnumDownloadStatus.FAIL;
            }
            return ProgramData.EnumDownloadStatus.DOWNLOADING;
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
            ProgramData.Paths.createFolderStructure();
            loadData();
            initialiseGUI();

            if (!ProgramData.skipLoadingScreenDelay) {
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
            }
            else {
                DebugUtils.SendDebugLine("Error: Failed to obtain steam path. Disabling launch.");
                Button_Launch_Vanilla.IsEnabled = false;
            }
        }

        private void OnProgramClosing(object sender, CancelEventArgs e) {
            saveData();
        }

        private void OnSelectedProfileChanged(object sender, EventArgs e) {
            Profile chosenProfile = ProfileManager.getProfileByName(profilesBox.SelectedItem);
            ProfileManager.loadProfile(chosenProfile);
            loadInstalledModList();
        }

        private void OnModsToShowChanged(object sender, EventArgs e) {
            switch (showingBox.SelectedItem) {
                case "Downloaded": loadInstalledModList(); break;
                case "Online": loadOnlineModList(); break;
                default:
                    string error = $"Cannot show mod set '{showingBox.SelectedItem}'";
                    DebugUtils.SendDebugLine($"Error: {error}");
                    DebugUtils.CrashIfDebug(error);
                    break;
            }
        }

        private void OnSortOptionChanged(object sender, EventArgs e) {
            // ToDo: Elliot - Sort Mods List
        }

        private void Button_Check_For_Updates_Click(object sender, RoutedEventArgs e) {
            CallUpdateWindow();
        }

        private void Button_Launch_Vanilla_Click(object sender, RoutedEventArgs e) {
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

        // Private Functions

        private void CallUpdateWindow() {
            AutoUpdater.Mandatory = true;
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.ShowRemindLaterButton = true;
            AutoUpdater.ShowSkipButton = true;
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
        }

        private void saveData() {
            ModManager.save();
            ProfileManager.save();
        }

        private void loadData() {
            ModManager.load();
            ProfileManager.load();
        }

        private void initialiseGUI() {
            profilesBox.setItems(ProfileManager.getProfileNames());
            showingBox.setItems(new List<string>() { "Downloaded", "Online" });
            sortBox.setItems(new List<string>() { "Last Updated", "Alphabetical", "Downloads", "Popularity" });
        }

        private void loadInstalledModList() {
            modsPanel.Children.Clear();
            Profile profile = ProfileManager.getActiveProfile();
            foreach(string modID in profile.modIDs) {
                addInstalledModToModList(modID);
            }
        }

        private void loadOnlineModList() {
            // ToDo: Elliot - Get modlist from api
            List<Mod> mods = new List<Mod>();
            foreach(Mod mod in mods) {
                addOnlineModToModList(mod);
            }
        }

        private void addInstalledModToModList(string modID) {
            modsPanel.Children.Add(new InstalledModPanel(modID));
        }

        private void addOnlineModToModList(Mod mod) {
            modsPanel.Children.Add(new OnlineModPanel(mod));
        }
    }
}
