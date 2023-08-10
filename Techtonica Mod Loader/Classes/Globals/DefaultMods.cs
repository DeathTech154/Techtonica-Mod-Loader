using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Techtonica_Mod_Loader.Classes.Globals
{
    public static class DefaultMods
    {
        public static Mod BepInEx = new Mod() { // ToDo: Set BepInEx missing / wrong details
            id = "bepinex",
            name = "BepInEx",
            version = "",
            tagline = "Enables use of mods in Unity games.",
            description = "",
            iconLink = "https://avatars2.githubusercontent.com/u/39589027?s=256",
            bannerLink = "",
            screenshot1Link = "",
            screenshot2Link = "",
            screenshot3Link = "",
            screenshot4Link = "",
            zipFileLocation = $"{ProgramData.Paths.modsFolder}\\BepInEx.zip", // set actual zip name
            enabled = true,
            canBeToggled = false
        };

        public static Mod UnityExplorer = new Mod() { // ToDo: Set Unity Explorer missing / wrong details
            id = "unityExplorer",
            name = "Unity Explorer",
            version = "",
            tagline = "",
            description = "",
            iconLink = "",
            bannerLink = "",
            screenshot1Link = "",
            screenshot2Link = "",
            screenshot3Link = "",
            screenshot4Link = "",
            zipFileLocation = $"{ProgramData.Paths.modsFolder}\\BepInEx.zip", // set actual zip name
            enabled = false // Installed for 'Development' profile, but off by default
        };
    }
}
