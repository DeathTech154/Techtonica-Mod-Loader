using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Techtonica_Mod_Loader.Classes
{
    public class Profile
    {
        public int id;
        public string name;
        public List<string> modIDs = new List<string>();

        public void AddMod(string id) {
            modIDs.Add(id);
        }
    }
}
