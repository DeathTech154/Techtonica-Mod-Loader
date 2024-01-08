using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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

        // Objects & Variables
        public List<Mod> modsOnDisplay = new List<Mod>();
        private string currentList = "";

        // Public Functions

        public void LoadInstalledModList() {
            modsPanel.Children.Clear();
            Profile profile = ProfileManager.GetActiveProfile();
            profile.SortMods(ProgramData.currentSortOption);
            foreach (string modID in profile.modIDs) {
                AddInstalledModToModList(modID);
                modsOnDisplay.Add(ModManager.GetMod(modID));
            }

            currentList = "Installed";
        }

        public async void LoadNewModsList() {
            modsPanel.Children.Clear();
            Profile profile = ProfileManager.GetActiveProfile();    
            List<Mod> mods = await ThunderStore.GetAllMods();
            mods = ModManager.SortModList(mods, ProgramData.currentSortOption);
            modsOnDisplay = mods;
            foreach (Mod mod in mods) {
                if (!Settings.userSettings.seenMods.Contains(mod.id) && !profile.HasMod(mod)) {
                    AddOnlineModToModList(mod);
                }
            }

            currentList = "Online";
        }

        public async void LoadOnlineModList() {
            modsPanel.Children.Clear();
            Profile profile = ProfileManager.GetActiveProfile();
            List<Mod> mods = await ThunderStore.GetAllMods();
            mods = ModManager.SortModList(mods, ProgramData.currentSortOption);
            modsOnDisplay = mods;
            foreach (Mod mod in mods) {
                if (!profile.HasMod(mod.id)){
                    AddOnlineModToModList(mod);

                    if (!Settings.userSettings.seenMods.Contains(mod.id)) {
                        Settings.userSettings.seenMods.Add(mod.id);
                        Settings.Save();
                    }
                }
            }

            currentList = "Online";
        }

        public void SearchModsList(string searchTerm) {
            modsPanel.Children.Clear();
            List<Mod> results = modsOnDisplay.Where(mod => mod.AppearsInSearch(searchTerm)).ToList();
            if(currentList == "Installed") {
                foreach(Mod mod in results) {
                    AddInstalledModToModList(mod.id);
                }
            }
            else {
                foreach(Mod mod in results) {
                    AddOnlineModToModList(mod);
                }
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
