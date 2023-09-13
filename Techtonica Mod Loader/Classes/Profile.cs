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
        public List<bool> enabledStates = new List<bool>();

        // Public Functions

        public void AddMod(string modID) {
            if (!modIDs.Contains(modID)) {
                modIDs.Add(modID);
                enabledStates.Add(true);
            }
            else {
                string error = $"Cannot add mod '{modID}' - Already included in profile";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public bool HasMod(string modID) {
            return modIDs.Contains(modID);
        }

        public bool IsModEnabled(string modID) {
            int index = modIDs.IndexOf(modID);
            if (index != -1) {
                return enabledStates[index];
            }
            else {
                string error = $"Tried to get enabled state of mod that is not in profile ({modID}|{name}) - {modID}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return false;
            }
        }

        public void SetEnabledState(string modID, bool state) {
            int index = modIDs.IndexOf(modID);
            if (index != -1) {
                enabledStates[index] = state;
            }
            else {
                string error = $"Tried to set enabled state of mod that is not in profile ({modID}|{name}) - {modID}";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public void SortMods(ModListSortOption sortOption) {
            Dictionary<string, bool> tempEnabledStates = new Dictionary<string, bool>();
            for(int i = 0; i < modIDs.Count; i++) {
                tempEnabledStates.Add(modIDs[i], enabledStates[i]);
            }

            switch (sortOption) {
                case ModListSortOption.Alphabetical:
                    List<Mod> mods = GetMods();
                    mods = mods.OrderBy(mod => mod.name).ToList();
                    
                    modIDs.Clear();
                    enabledStates.Clear();
                    
                    foreach(Mod mod in mods) {
                        modIDs.Add(mod.id);
                        enabledStates.Add(tempEnabledStates[mod.id]);
                    }

                    break;
            }

            if (HasMod(ProgramData.bepInExID)) {
                int bepIndex = modIDs.IndexOf(ProgramData.bepInExID);
                modIDs.RemoveAt(bepIndex);
                modIDs.Insert(0, ProgramData.bepInExID);
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
