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

        public static void addProfile(Profile profile) {
            profile.id = getNewProfileID();
            profiles.Add(profile.id, profile);
        }

        public static Profile getProfile(int id) {
            if (doesProfileExist(id)) {
                return profiles[id];
            }
            else {
                string error = $"Cannot retrieve profile with id ({id}), does not exist.";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return new Profile();
            }
        }

        public static Profile getActiveProfile() {
            if (doesProfileExist(activeProfile)) {
                return profiles[activeProfile];
            }
            else {
                string error = $"Cannot retrieve active profile (id: {activeProfile}), does not exist.";
                DebugUtils.SendDebugLine(error);
                DebugUtils.CrashIfDebug(error);
                return new Profile();
            }
        }

        public static Profile getProfileByName(string name) {
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

        public static List<string> getProfileNames() {
            List<string> names = new List<string>();
            foreach(Profile profile in profiles.Values) {
                names.Add(profile.name);
            }

            return names;
        }

        public static void loadProfile(int id) {
            if (doesProfileExist(id)) {
                loadProfile(profiles[id]);
            }
        }

        public static void loadProfile(Profile profile) {
            activeProfile = profile.id;

        }

        // Private Functions

        private static bool doesProfileExist(int id) {
            return profiles.ContainsKey(id);
        }

        private static int getNewProfileID() {
            if (profiles.Count == 0) return 0;
            else return profiles.Keys.Max() + 1;
        }

        // Data Functions

        public static void save() {
            string json = JsonConvert.SerializeObject(profiles.Values.ToList());
            File.WriteAllText(ProgramData.Paths.profilesFile, json);
        }

        public static void load() {
            if (File.Exists(ProgramData.Paths.profilesFile)) {
                string json = File.ReadAllText(ProgramData.Paths.profilesFile);
                List<Profile> profilesFromFile = JsonConvert.DeserializeObject<List<Profile>>(json);
                foreach(Profile profile in profilesFromFile) {
                    addProfile(profile);
                }
            }
            else {
                createDefaultProfiles();
            }
        }

        public static void createDefaultProfiles() {
            Profile modded = new Profile() { name = "Modded" };
            modded.modIDs.Add(Classes.Globals.DefaultMods.BepInEx.id);

            Profile development = new Profile() { name = "Development" };
            development.modIDs.Add(Classes.Globals.DefaultMods.BepInEx.id);
            development.modIDs.Add(Classes.Globals.DefaultMods.UnityExporer.id);

            addProfile(modded);
            addProfile(development);
            addProfile(new Profile() { name = "Vanilla" });
        }
    }
}
