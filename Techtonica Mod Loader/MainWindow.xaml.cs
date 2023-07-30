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

        // Objects & Variables

        public static MainWindow current => (MainWindow)Application.Current.MainWindow;

        // ToDo: Move To ProgramData
        public string GameLocation; 
        public Brush LaunchVanillaDisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public string SelfLoc = Directory.GetParent(AppContext.BaseDirectory).FullName;
        //public UpdateInfoEventArgs UpdateArgs = new UpdateInfoEventArgs();

        // Events

        private void OnProgramLoaded(object sender, RoutedEventArgs e) {
            DebugUtils.SendDebugLine("Logtest!");
            ProgramData.Paths.createFolderStructure();
            loadData();
            initialiseGUI();

            //string[] SplitSelfLoc = SelfLoc.Split("/");

            DebugUtils.SendDebugLine(SelfLoc);
            string NewLoc = RemoveFromBack(SelfLoc, 1, "\\");
            AutoUpdater.InstallationPath = NewLoc;
            DebugUtils.SendDebugLine(AutoUpdater.InstallationPath);
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
            //AutoUpdater.ReportErrors = true;
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
            this.Title = "Techtonica Mod Loader v"+ version; // ToDo: Move to label
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
            //AutoUpdater.RemindLaterAt = 0;
            //AutoUpdater.RemindLaterTimeSpan = 0;
            //AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
            //UpdateArgs.ChangelogURL = AutoUpdater.AppCastURL;
            //UpdateArgs.InstalledVersion = AutoUpdater.InstalledVersion;
            //UpdateArgs.DownloadURL = AutoUpdater.DownloadPath;
            //UpdateArgs.ExecutablePath = AutoUpdater.ExecutablePath;
            //UpdateArgs.Mandatory = new Mandatory();
            //UpdateArgs.Mandatory.Value = AutoUpdater.Mandatory;
            //UpdateArgs.Mandatory.MinimumVersion = "0.0.0.1";
            //UpdateArgs.Mandatory.UpdateMode = Mode.Normal;
            //UpdateArgs.CurrentVersion = version;
            //AutoUpdater.ShowUpdateForm(Args);
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
