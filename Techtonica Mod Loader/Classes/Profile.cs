using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Techtonica_Mod_Loader.Classes
{
    public class Profile
    {
        // Objects & Variables
        public int id;
        public string name;
        public List<string> modIDs = new List<string>();

        // Public Functions

        public void AddMod(string id) {
            modIDs.Add(id);
        }

        public bool HasMod(string id) {
            return modIDs.Contains(id);
        }

        public void SortMods(ModListSortOption sortOption) {
            switch (sortOption) {
                case ModListSortOption.Alphabetical:
                    List<Mod> mods = GetMods();
                    mods = mods.OrderBy(mod => mod.name).ToList();
                    modIDs.Clear();
                    foreach(Mod mod in mods) {
                        modIDs.Add(mod.id);
                    }

                    break;
            }

            if (HasMod(DefaultMods.BepInEx)) {
                int bepIndex = modIDs.IndexOf(DefaultMods.BepInEx.id);
                modIDs.RemoveAt(bepIndex);
                modIDs.Insert(0, DefaultMods.BepInEx.id);
            }
            
            // ToDo: More Mod Sort Options
        }

        public List<Mod> GetMods() {
            List<Mod> mods = new List<Mod>();
            foreach(string id in modIDs) {
                mods.Add(ModManager.GetMod(id));
            }

            return mods;
        }

        #region Overloads

        public void AddMod(Mod mod) {
            AddMod(mod.id);
        }

        public bool HasMod(Mod mod) {
            return HasMod(mod.id);
        }

        #endregion
    }
}
