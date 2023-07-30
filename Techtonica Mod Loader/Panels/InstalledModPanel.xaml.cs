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
            showMod(ModManager.getMod(modID));
        }

        public InstalledModPanel(Mod mod) {
            InitializeComponent();
            showMod(mod);
        }

        // Objects & Variables

        public string modID;

        // Events

        private void EnabledToggled(object sender, EventArgs e) {
            Mod mod = ModManager.getMod(modID);
            mod.enabled = enabledBox.IsChecked;
            ModManager.updateModDetails(mod);
            
            if (mod.enabled) {
                mod.install();
            }
            else {
                mod.uninstall();
            }
        }

        private void DeleteModClicked(object sender, EventArgs e) {
            // ToDo: Elliot - Get Confirmation
            Mod mod = ModManager.getMod(modID);
            if (mod.enabled) {
                mod.uninstall();
            }

            File.Delete(mod.zipFileLocation);
        }

        private void ViewModPageClicked(object sender, EventArgs e) {
            Process.Start(ModManager.getMod(modID).link);
        }

        // Public Functions

        public void showMod(Mod mod) {
            modID = mod.id;
            enabledBox.IsChecked = mod.enabled;
            icon.Source = new BitmapImage(new Uri(mod.iconLink));
            modNameLabel.Text = mod.name;
            modTaglineLabel.Text = mod.tagline;
        }
    }
}
