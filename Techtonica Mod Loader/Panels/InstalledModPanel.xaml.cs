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

        // Events

        private void EnabledToggled(object sender, EventArgs e) {
            Mod mod = ModManager.GetMod(modID);
            mod.enabled = enabledBox.IsChecked;
            ModManager.UpdateModDetails(mod);
            
            if (mod.enabled) {
                mod.Install();
            }
            else {
                mod.Uninstall();
            }
        }

        private void OnConfigureClicked(object sender, EventArgs e) {
            // ToDo: Elliot - Configure mod
        }

        private void ViewModPageClicked(object sender, EventArgs e) {
            Process.Start(ModManager.GetMod(modID).link);
        }

        private void DeleteModClicked(object sender, EventArgs e) {
            // ToDo: Elliot - Get Confirmation
            Mod mod = ModManager.GetMod(modID);
            if (mod.enabled) {
                mod.Uninstall();
            }

            File.Delete(mod.zipFileLocation);
        }

        // Public Functions

        public void ShowMod(Mod mod) {
            modID = mod.id;
            enabledBox.IsChecked = mod.enabled;
            enabledBox.IsEditable = mod.canBeToggled;
            modNameLabel.Text = mod.name;
            modTaglineLabel.Text = mod.tagline;
            icon.Source = new BitmapImage(new Uri(mod.iconLink));
        }

        // Private Functions

        private void HideConfigureColumn() {
            mainGrid.Children.Remove(configureButton);
            configureColumn.Width = new GridLength(0);
        }
    }
}
