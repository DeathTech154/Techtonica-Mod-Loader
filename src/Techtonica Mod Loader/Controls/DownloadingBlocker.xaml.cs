using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Techtonica_Mod_Loader.Classes;

namespace Techtonica_Mod_Loader.Controls
{
    /// <summary>
    /// Interaction logic for DownloadingBlocker.xaml
    /// </summary>
    public partial class DownloadingBlocker : UserControl
    {
        public DownloadingBlocker()
        {
            InitializeComponent();
        }

        // Objects & Variables
        string modName;

        // Public Functions

        public void ShowForMod(Mod mod) {
            modName = mod.name;
            titleLabel.Content = $"Downloading Mod: {mod.name}";
        }

        public void SetToInstalling() {
            titleLabel.Content = $"Installing Mod: {modName}";
        }
    }
}
