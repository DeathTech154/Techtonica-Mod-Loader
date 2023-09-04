using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Techtonica_Mod_Loader.Classes;

namespace Techtonica_Mod_Loader
{
    public static class DefaultMods
    {
        public static Mod BepInEx = new Mod() { // ToDo: Set BepInEx missing / wrong details
            id = "bepinex",
            name = "BepInEx",
            version = ModVersion.Parse("5.4.21"),
            tagline = "Enables use of mods in Unity games.",
            description = "",
            link = "https://docs.bepinex.dev/articles/user_guide/installation/index.html",
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
            version = ModVersion.Parse("1.0.0"),
            tagline = "",
            description = "",
            link = "https://github.com/sinai-dev/UnityExplorer",
            iconLink = "https://gcdn.thunderstore.io/live/repository/icons/sinai-dev-UnityExplorer-4.8.2.png.128x128_q95.jpg",
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
