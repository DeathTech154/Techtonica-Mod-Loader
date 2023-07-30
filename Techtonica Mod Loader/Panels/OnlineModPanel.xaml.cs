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

namespace Techtonica_Mod_Loader.Panels
{
    /// <summary>
    /// Interaction logic for OnlineModPanel.xaml
    /// </summary>
    public partial class OnlineModPanel : UserControl
    {
        public OnlineModPanel()
        {
            InitializeComponent();
        }

        public OnlineModPanel(string modID) {
            InitializeComponent();
            showMod(modID);
        }

        public OnlineModPanel(Mod mod) {
            InitializeComponent();
            showMod(modID);
        }

        // Objects & Variables

        public string modID;

        // Custom Events

        // Events

        private void OnDownloadClicked(object sender, EventArgs e) {
            // ToDo: Elliot - Download and install mod
        }

        // Public Functions

        public void showMod(string id) {
            // ToDo: Elliot - Get mod from API
        }

        public void showMod(Mod mod) {
            modID = mod.id;
            modNameLabel.Text = mod.name;
            modTaglineLabel.Text = mod.tagline;
        }
    }
}
