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

        public static void LoadProfile(int id) {
            if (DoesProfileExist(id)) {
                LoadProfile(profiles[id]);
            }
        }

        public static void LoadProfile(Profile profile) {
            activeProfile = profile.id;
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

        public static void Load() {
            if (File.Exists(ProgramData.Paths.profilesFile)) {
                string json = File.ReadAllText(ProgramData.Paths.profilesFile);
                List<Profile> profilesFromFile = JsonConvert.DeserializeObject<List<Profile>>(json);
                foreach(Profile profile in profilesFromFile) {
                    AddProfile(profile);
                }
            }
            else {
                CreateDefaultProfiles();
            }
        }

        public static void CreateDefaultProfiles() {
            Profile modded = new Profile() { name = "Modded" };
            modded.modIDs.Add(Classes.Globals.DefaultMods.BepInEx.id);

            Profile development = new Profile() { name = "Development" };
            development.modIDs.Add(Classes.Globals.DefaultMods.BepInEx.id);
            development.modIDs.Add(Classes.Globals.DefaultMods.UnityExplorer.id);

            AddProfile(modded);
            AddProfile(development);
            AddProfile(new Profile() { name = "Vanilla" });
        }
    }
}
