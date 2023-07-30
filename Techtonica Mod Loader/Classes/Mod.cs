using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Techtonica_Mod_Loader.Classes
{
    public class Mod
    {
        public string id;
        public string name;
        public string version; // ToDo: Make this a struct with major, minor, subversion to compare versions
        public string tagline;
        public string description;

        public string link;
        public string iconLink;
        public string bannerLink;
        public string screenshot1Link;
        public string screenshot2Link;
        public string screenshot3Link;
        public string screenshot4Link;

        public string zipFileLocation;
        public List<string> installedFiles = new List<string>();
        public bool enabled;
        public bool canBeToggled;

        // Public Functions

        public void install() {
            // ToDo: DeathTech - Install Mod
        }

        public void uninstall() {
            // ToDo: DeathTech - Uninstall Mod
        }
    }
}
