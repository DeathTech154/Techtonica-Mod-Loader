using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Techtonica_Mod_Loader.Classes.Globals
{
    public static class ModManager
    {
        // Objects & Variables

        private static Dictionary<string, Mod> mods = new Dictionary<string, Mod>();

        // Public Functions

        public static void addMod(Mod mod) {
            if (!doesModExist(mod.id)){
                mods.Add(mod.id, mod);
            }
            else {
                string error = $"Cannot add mod ({mod.name}), id: ({mod.id}), already exists.";
                DebugUtils.SendDebugLine($"Error: {error}");
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static void updateModDetails(Mod mod) {
            if (doesModExist(mod.id)) {
                mods[mod.id] = mod;
            }
            else {
                string error = $"Could not update details of mod ({mod.id}: {mod.name}), does not exist";
                DebugUtils.SendDebugLine($"Error: {error}");
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static Mod getMod(string id) {
            if (doesModExist(id)) {
                return mods[id];
            }
            else {
                string error = $"Cannot get mod with id ({id}), does not exist.";
                DebugUtils.SendDebugLine($"Error: {error}");
                DebugUtils.CrashIfDebug(error);
                return new Mod();
            }
        }

        public static bool doesModExist(string id) {
            return mods.ContainsKey(id);
        }

        // Private Functions

        // Data Functions

        public static void save() {
            string json = JsonConvert.SerializeObject(mods.Values.ToList());
            File.WriteAllText(ProgramData.Paths.modsFile, json);
        }

        public static void load() {
            if (File.Exists(ProgramData.Paths.modsFile)) {
                string json = File.ReadAllText(ProgramData.Paths.modsFile);
                List<Mod> modsFromFile = JsonConvert.DeserializeObject<List<Mod>>(json);
                foreach(Mod mod in modsFromFile) {
                    addMod(mod);
                }
            }
            else {
                addMod(DefaultMods.BepInEx);
                addMod(DefaultMods.UnityExporer);
            }
        }
    }
}
