using MyLogger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Techtonica_Mod_Loader.Classes.ThunderStoreResponses;

namespace Techtonica_Mod_Loader.Classes
{
    public static class ThunderStore
    {
        // Objects & Variables
        private static string baseURL = "https://thunderstore.io/c/techtonica/api/v1";

        // Public Functions

        public static async Task<ThunderStoreMod> GetThunderStoreMod(string id) {
            string endPoint = $"{baseURL}/package/{id}/";
            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            return JsonConvert.DeserializeObject<ThunderStoreMod>(json);
        }

        public static async Task<Mod> SearchForMod(string fullName) {
            List<ThunderStoreMod> mods = await GetAllThunderStoreMods();
            mods = mods.Where(mod => mod.versions[0].full_name == fullName).ToList();
            if (mods.Count == 1) {
                return new Mod(mods[0]);
            }

            string error = $"Could not find a mod with full_name= '{fullName}'";
            Log.Error(error);
            DebugUtils.CrashIfDebug(error);
            return null;
        }

        public static async Task<Mod> GetMod(string id) {
            return new Mod(await GetThunderStoreMod(id));
        }

        public static async Task<List<Mod>> GetAllMods() {
            List<ThunderStoreMod> thunderStoreMods = await GetAllThunderStoreMods();
            List<Mod> mods = new List<Mod>();
            foreach (ThunderStoreMod thunderStoreMod in thunderStoreMods) {
                if(Settings.userSettings.hideTools && thunderStoreMod.categories.Contains("Tools")) continue;
                mods.Add(new Mod(thunderStoreMod));
            }

            return mods;
        } 

        // Private Functions

        private static async Task<string> GetApiData(string apiUrl) {
            using (HttpClient client = new HttpClient()) {
                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode) {
                    return await response.Content.ReadAsStringAsync();
                }
                else {
                    string error = $"HTTP request failed with status code {response.StatusCode}";
                    Log.Error(error);
                    DebugUtils.CrashIfDebug(error);
                    return "";
                }
            }
        }

        private static async Task<List<ThunderStoreMod>> GetAllThunderStoreMods() {
            string endPoint = $"{baseURL}/package/";
            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                string error = $"API returned empty JSON";
                Log.Error(error);
                DebugUtils.CrashIfDebug(error);
                return new List<ThunderStoreMod>();
            }

            return JsonConvert.DeserializeObject<List<ThunderStoreMod>>(json);
        }
    }
}
