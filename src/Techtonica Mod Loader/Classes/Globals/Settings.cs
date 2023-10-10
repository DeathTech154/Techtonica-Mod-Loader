using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Techtonica_Mod_Loader
{
    public class Settings
    {
        // Objects & Variables
        public static Settings userSettings = new Settings();

        public string gameFolder = "";
        public ModListSortOption defaultSort = ModListSortOption.Popularity;
        public ModListSource defaultModList = ModListSource.Installed;
        public bool hideTools = true;
        public bool renderImages = true;
        public bool cacheImages = true;
        public bool logDebugMessages = false;

        // Hidden Settings - No Gui
        public List<string> seenMods = new List<string>();
        public double lastWidth = 1200;
        public double lastHeight = 675;

        // Data Functions

        public static void Save() {
            string json = JsonConvert.SerializeObject(userSettings, Formatting.Indented);
            File.WriteAllText(ProgramData.Paths.settingsFile, json);
        }

        public static void Load() {
            if (!File.Exists(ProgramData.Paths.settingsFile)) {
                CreateDefaultFile();
            }

            string json = File.ReadAllText(ProgramData.Paths.settingsFile);
            userSettings = JsonConvert.DeserializeObject<Settings>(json);
        }

        private static void CreateDefaultFile() {
            userSettings = new Settings();
            Save();
        }
    }
}
