using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Techtonica_Mod_Loader.Classes;

namespace Techtonica_Mod_Loader
{
    public static class ProfileManager
    {
        // Objects & Variables

        private static int activeProfile;
        private static Dictionary<int, Profile> profiles = new Dictionary<int, Profile>();

        // Public Functions

        public static void AddProfile(Profile profile) {
            profile.id = GetNewProfileID();
            profiles.Add(profile.id, profile);
            Save();
        }

        public static void UpdateProfile(Profile profile) {
            if (DoesProfileExist(profile)) {
                profiles[profile.id] = profile; 
                Save();
            }
            else {
                string error = $"Cannot update profile '{profile.id}|{profile.name}' - does not exist";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
            }
        }

        public static Profile GetProfile(int id) {
            if (DoesProfileExist(id)) {
                return profiles[id];
            }
            else {
                string error = $"Cannot retrieve profile with id ({id}), does not exist.";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return new Profile();
            }
        }

        public static Profile GetActiveProfile() {
            if (DoesProfileExist(activeProfile)) {
                return profiles[activeProfile];
            }
            else {
                string error = $"Cannot retrieve active profile (id: {activeProfile}), does not exist.";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return new Profile();
            }
        }

        public static Profile GetProfileByName(string name) {
            foreach (Profile profile in profiles.Values) {
                if (profile.name == name) {
                    return profile;
                }
            }

            string error = $"Couldn't find profile with name ({name}).";
            DebugUtils.SendDebugLine(error);
            DebugUtils.CrashIfDebug(error);
            return new Profile();
        }

        public static List<string> GetProfileNames() {
            List<string> names = new List<string>();
            foreach(Profile profile in profiles.Values) {
                names.Add(profile.name);
            }

            return names;
        }

        public static void LoadProfile(Profile profile) {
            Profile oldProfile = GetActiveProfile();
            foreach(string modID in oldProfile.modIDs) {
                if (oldProfile.IsModEnabled(modID)) {
                    Mod mod = ModManager.GetMod(modID);
                    mod.Uninstall();
                }
            }

            activeProfile = profile.id;
            Profile newProfile = GetActiveProfile();
            foreach(string modID in newProfile.modIDs) {
                if (newProfile.IsModEnabled(modID)) {
                    Mod mod = ModManager.GetMod(modID);
                    mod.Install();
                }
            }
        }

        // Private Functions

        private static bool DoesProfileExist(int id) {
            return profiles.ContainsKey(id);
        }

        private static int GetNewProfileID() {
            if (profiles.Count == 0) return 0;
            else return profiles.Keys.Max() + 1;
        }

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(profiles.Values.ToList());
            File.WriteAllText(ProgramData.Paths.profilesFile, json);
        }

        public static async Task<string> Load() {
            if (File.Exists(ProgramData.Paths.profilesFile)) {
                string json = File.ReadAllText(ProgramData.Paths.profilesFile);
                List<Profile> profilesFromFile = JsonConvert.DeserializeObject<List<Profile>>(json);
                foreach(Profile profile in profilesFromFile) {
                    AddProfile(profile);
                }
            }
            else {
                await CreateDefaultProfiles();
            }

            return "";
        }

        public static async Task<string> CreateDefaultProfiles() {
            Mod bepInEx = await ThunderStore.GetMod(ProgramData.bepInExID);
            ModManager.AddIfNew(bepInEx);

            Profile modded = new Profile() { name = "Modded" };
            Profile development = new Profile() { name = "Development", modIDs = new List<string>() { ProgramData.bepInExID } };

            AddProfile(modded);
            AddProfile(development);
            AddProfile(new Profile() { name = "Vanilla" });

            LoadProfile(modded);
            bepInEx.Download();

            while (!GetActiveProfile().HasMod(ProgramData.bepInExID)) {
                await Task.Delay(10); // Wait for BepInEx to download before showing mod list
            }

            return "";
        }

        #region Overloads

        // Public Functions

        public static void LoadProfile(int id) {
            if (DoesProfileExist(id)) {
                LoadProfile(profiles[id]);
            }
        }

        // Private Functions

        private static bool DoesProfileExist(Profile profile) {
            return DoesProfileExist(profile.id);
        }

        #endregion
    }
}
