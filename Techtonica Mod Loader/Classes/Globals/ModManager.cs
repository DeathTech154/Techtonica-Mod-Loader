﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyLogger;
using Newtonsoft.Json;
using Techtonica_Mod_Loader.Classes;
using Techtonica_Mod_Loader.Classes.ThunderStoreResponses;

namespace Techtonica_Mod_Loader
{
    public static class ModManager
    {
        // Objects & Variables

        private static Dictionary<string, Mod> mods = new Dictionary<string, Mod>();

        // Public Functions

        public static void AddMod(Mod mod) {
            if (!DoesModExist(mod.id)){
                mods.Add(mod.id, mod);
                Save();
            }
            else {
                string error = $"Cannot add mod ({mod.name}), id: ({mod.id}), already exists.";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static void UpdateModDetails(Mod mod) {
            if (DoesModExist(mod.id)) {
                mods[mod.id] = mod;
                Save();
            }
            else {
                string error = $"Could not update details of mod ({mod.id}: {mod.name}), does not exist";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static void AddIfNew(Mod mod) {
            if (!DoesModExist(mod)) {
                AddMod(mod);
            }
        }

        public static Mod GetMod(string id) {
            if (DoesModExist(id)) {
                return mods[id];
            }
            else {
                string error = $"Cannot get mod with id ({id}), does not exist.";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new Mod();
            }
        }

        public static bool DoesModExist(string id) {
            return mods.ContainsKey(id);
        }

        public static void DeleteMod(string id) {
            if (DoesModExist(id)) {
                mods.Remove(id);
            }
            else {
                string error = $"Cannot delete mod with id '{id}' - Does not exist";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static List<Mod> SortModList(List<Mod> modsToSort, ModListSortOption sortOption) {
            switch(sortOption) {
                case ModListSortOption.Alphabetical: return modsToSort.OrderBy(mod => mod.name).ToList();
                case ModListSortOption.LastUpdated: return modsToSort.OrderByDescending(mod => mod.dateUpdated).ToList();
                case ModListSortOption.Downloads: return modsToSort.OrderByDescending(mod => mod.downloads).ToList();
                case ModListSortOption.Popularity: return modsToSort.OrderByDescending(mod => mod.ratingScore).ToList();
            }

            string error = $"Could not sort mods with unknown sort option: '{StringUtils.GetModListSortOptionName(sortOption)}'";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return modsToSort;
        }

        public static async void CheckForUpdates() {
            foreach(Mod mod in mods.Values) {
                if (mod.IsLocal() || mod.id == ProgramData.bepInExID) continue;

                ThunderStoreMod thunderStoreMod = await ThunderStore.GetThunderStoreMod(mod.id);
                if (thunderStoreMod == null) continue;
                if (thunderStoreMod.versions.Count < 2) continue;

                ModVersion latestVersion = ModVersion.Parse(thunderStoreMod.versions[0].version_number);
                mod.updateAvailable = latestVersion > mod.version;
            }
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(mods.Values.ToList());
            File.WriteAllText(ProgramData.Paths.modsFile, json);
        }

        public static void Load() {
            if (File.Exists(ProgramData.Paths.modsFile)) {
                string json = File.ReadAllText(ProgramData.Paths.modsFile);
                List<Mod> modsFromFile = JsonConvert.DeserializeObject<List<Mod>>(json);
                foreach(Mod mod in modsFromFile) {
                    AddMod(mod);
                }
            }
        }

        #region Overloads

        // Public Functions

        public static bool DoesModExist(Mod mod) {
            return DoesModExist(mod.id);
        }

        public static void DeleteMod(Mod mod) {
            DeleteMod(mod.id);
        }

        // Private Functions

        #endregion
    }
}
