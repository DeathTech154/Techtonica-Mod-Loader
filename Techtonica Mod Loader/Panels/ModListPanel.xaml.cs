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
using Techtonica_Mod_Loader.Classes;

namespace Techtonica_Mod_Loader.Panels
{
    /// <summary>
    /// Interaction logic for ModListPanel.xaml
    /// </summary>
    public partial class ModListPanel : UserControl
    {
        public ModListPanel()
        {
            InitializeComponent();
        }

        // Public Functions

        public void LoadInstalledModList() {
            modsPanel.Children.Clear();
            Profile profile = ProfileManager.GetActiveProfile();
            profile.SortMods(ModListSortOption.Alphabetical);
            foreach (string modID in profile.modIDs) { // ToDo: Sort from MainWindow setting
                AddInstalledModToModList(modID);
            }
        }

        public void LoadOnlineModList() {
            // ToDo: Elliot - Get modlist from api
            List<Mod> mods = new List<Mod>();
            foreach (Mod mod in mods) {
                AddOnlineModToModList(mod);
            }
        }

        // Private Functions

        private void AddInstalledModToModList(string modID) {
            modsPanel.Children.Add(new InstalledModPanel(modID) { Margin = new Thickness(4, 4, 4, 0) });
        }

        private void AddOnlineModToModList(Mod mod) {
            modsPanel.Children.Add(new OnlineModPanel(mod) { Margin = new Thickness(4, 4, 4, 0) });
        }
    }
}
