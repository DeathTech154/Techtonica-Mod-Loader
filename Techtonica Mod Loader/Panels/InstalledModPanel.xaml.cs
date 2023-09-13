using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Techtonica_Mod_Loader.Classes;
using Techtonica_Mod_Loader.Classes.Globals;

namespace Techtonica_Mod_Loader.Panels
{
    /// <summary>
    /// Interaction logic for InstalledModPanel.xaml
    /// </summary>
    public partial class InstalledModPanel : UserControl
    {
        public InstalledModPanel()
        {
            InitializeComponent();
        }

        public InstalledModPanel(string modID) {
            InitializeComponent();
            ShowMod(ModManager.GetMod(modID));
        }

        public InstalledModPanel(Mod mod) {
            InitializeComponent();
            ShowMod(mod);
        }

        // Objects & Variables

        public string modID;
        private bool isExpanded;

        // Events

        private void OnPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (enabledBox.IsMouseOver || 
                configureButton.IsMouseOver ||
                viewModPageButton.IsMouseOver ||
                deleteButton.IsMouseOver) {
                return;
            }

            isExpanded = !isExpanded;
            string animKey = isExpanded ? "growAnim" : "shrinkAnim";
            BeginStoryboard((Storyboard)Resources[animKey]);
        }

        private void EnabledToggled(object sender, EventArgs e) {
            Profile profile = ProfileManager.GetActiveProfile();
            profile.SetEnabledState(modID, enabledBox.IsChecked);
            ProfileManager.UpdateProfile(profile);

            Mod mod = ModManager.GetMod(modID);
            if (enabledBox.IsChecked) {
                mod.Install();
            }
            else {
                mod.Uninstall();
            }
        }

        private void OnConfigureClicked(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            ModConfig config = ModConfig.FromFile(mod.configFileLocation);
            ModConfig.activeConfig = config;
            MainWindow.current.mainBorder.Child = new ModConfigPanel(config, mod.name);
        }

        private void ViewModPageClicked(object sender, EventArgs e) {
            ProcessStartInfo info = new ProcessStartInfo() {
                FileName = ModManager.GetMod(modID).link,
                UseShellExecute = true
            };
            Process.Start(info);
        }

        private void DeleteModClicked(object sender, EventArgs e) {
            if(GuiUtils.GetUserConfirmation("Delete Mod?", "Are you sure you want to delete this mod?")) {
                Profile profile = ProfileManager.GetActiveProfile();
                Mod mod = ModManager.GetMod(modID);
                if (profile.IsModEnabled(modID)) {
                    mod.Uninstall();
                }

                File.Delete(mod.zipFileLocation);
                ModManager.DeleteMod(mod);
            }
        }

        // Public Functions

        public void ShowMod(Mod mod) {
            modID = mod.id;
            enabledBox.IsChecked = ProfileManager.GetActiveProfile().IsModEnabled(modID);
            enabledBox.IsEditable = mod.canBeToggled;
            modNameLabel.Text = mod.name;
            modTaglineLabel.Text = mod.tagLine;
            icon.Source = new BitmapImage(new Uri(mod.iconLink));

            if (mod.HasMarkdownFile()) {
                markdownViewer.ViewMarkdownFromFile(mod.markdownFileLocation);
            }
            else {
                markdownViewer.ViewMarkdown("# No description available");
            }

            if (!mod.HasConfigFile()) {
                HideConfigureColumn();
            }

            if (mod.IsLocal()) {
                HideViewModPageColumn();
            }
        }

        // Private Functions

        private void HideConfigureColumn() {
            mainGrid.Children.Remove(configureButton);
            configureColumn.Width = new GridLength(0);
        }

        private void HideViewModPageColumn() {
            mainGrid.Children.Remove(viewModPageButton);
            viewModPageColumn.Width = new GridLength(0);
        }
    }
}
