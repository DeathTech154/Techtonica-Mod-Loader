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

namespace Techtonica_Mod_Loader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public void SendDebugLine(string Str)
        {
            Console.WriteLine(Str);
            Debug.WriteLine(Str);
        }

        public static MainWindow current => (MainWindow)Application.Current.MainWindow;

        public string GameLocation;
        public Brush LaunchVanillaDisabledBrush = new SolidColorBrush(Colors.DarkRed);
        public string version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        public string SelfLoc = Directory.GetParent(AppContext.BaseDirectory).FullName;
        //public UpdateInfoEventArgs UpdateArgs = new UpdateInfoEventArgs();

        public string RemoveFromBack(string Original, int Entries, string Seperator)
        {
            string[] Splited = Original.Split(Seperator);
            int SplitLength = Splited.Length;
            string NewString = "";
            int count = 0;
            foreach (string Split in Splited)
            {
                if (count == 0) {
                    NewString = NewString + Split;
                    count = count + 1;
                }
                else if(count < (SplitLength - Entries))
                {
                    NewString = NewString + Seperator + Split;
                    count = count + 1;
                }
            }
            return NewString;
        }

        public MainWindow()
        {
            SendDebugLine("Logtest!");
            InitializeComponent();
            ProgramData.Paths.createFolderStructure();
            //string[] SplitSelfLoc = SelfLoc.Split("/");

            SendDebugLine(SelfLoc);
            string NewLoc = RemoveFromBack(SelfLoc, 1, "\\");
            AutoUpdater.InstallationPath = NewLoc;
            SendDebugLine(AutoUpdater.InstallationPath);
            AutoUpdater.Start("https://www.DeeTeeNetwork.com/TechtonicaML_AutoUpdate.xml");
            //AutoUpdater.ReportErrors = true;
            AutoUpdater.UpdateFormSize = new System.Drawing.Size(800, 600);
            this.Title = "Techtonica Mod Loader v"+ version; // ToDo: Move to label
            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Valve\Steam"); // Gets steam folder location from registry.
            if (key != null) { 
                string SteamPath = (string)key.GetValue("SteamPath");
                key.Close();
                SendDebugLine(SteamPath);
                GameLocation = SteamPath + @"/steamapps/common/Techtonica";
                SendDebugLine(GameLocation);
            } else
            {
                SendDebugLine("Error: Failed to obtain steam path. Disabling launch.");
                Button_Launch_Vanilla.IsEnabled = false;
            }
        }

        private void CallUpdateWindow()
        {
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

        private void Button_Check_For_Updates_Click(object sender, RoutedEventArgs e)
        {
            CallUpdateWindow();
        }

        private void Button_Launch_Vanilla_Click(object sender, RoutedEventArgs e)
        {
            ProcessStartInfo start = new ProcessStartInfo();
            start.Arguments = e.ToString();
            start.FileName = "Techtonica.exe";
            start.WorkingDirectory = GameLocation;
            start.UseShellExecute = true;
            SendDebugLine("Launching from directory: "+ start.WorkingDirectory);
            SendDebugLine("Target: " + start.FileName);
            start.WindowStyle = ProcessWindowStyle.Normal;
            start.CreateNoWindow = false;
            int exitCode;
            SendDebugLine("Launching game in Vanilla mode!");
            try { 
                using (Process proc = Process.Start(start))
                {
                    proc.WaitForExit();

                    // Retrieve the app's exit code
                    exitCode = proc.ExitCode;
                }
            }
            catch (Win32Exception ex)
            {

                SendDebugLine(ex.Message);
                Button_Launch_Vanilla.IsEnabled = true;
                Button_Launch_Vanilla.Content = "Game not found!";
                Button_Launch_Vanilla.Background = LaunchVanillaDisabledBrush;
            }
        }
    }
}
