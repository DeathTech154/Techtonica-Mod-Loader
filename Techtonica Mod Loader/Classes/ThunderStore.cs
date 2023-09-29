using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        public static async Task<Mod> GetMod(string id) {
            string endPoint = $"{baseURL}/package/{id}/";
            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            ThunderStoreMod thunderStoreMod = JsonConvert.DeserializeObject<ThunderStoreMod>(json);
            return new Mod(thunderStoreMod);
        }

        public static async Task<List<Mod>> GetAllMods() {
            string endPoint = $"{baseURL}/package/";
            string json = await GetApiData(endPoint);
            if (string.IsNullOrEmpty(json)) {
                return new List<Mod>();
            }

            List<ThunderStoreMod> thunderStoreMods = JsonConvert.DeserializeObject<List<ThunderStoreMod>>(json);
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
                    DebugUtils.SendDebugLine(error);
                    DebugUtils.CrashIfDebug(error);
                    return "";
                }
            }
        }
    }
}
