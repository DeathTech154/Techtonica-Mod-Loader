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

        public OnlineModPanel(Mod modToShow) {
            InitializeComponent();
            showMod(modToShow);
        }

        // Objects & Variables

        public Mod mod;

        // Events

        private void OnDownloadClicked(object sender, EventArgs e) {
            ModManager.AddIfNew(mod);
            mod.FinishedDownloading += ModFinishedDownloading;
            mod.Download();
        }

        private void ModFinishedDownloading(object sender, EventArgs e) {
            StackPanel parent = Parent as StackPanel;
            parent.Children.Remove(this);
        }

        // Public Functions

        public void showMod(Mod modToShow) {
            mod = modToShow;
            modNameLabel.Text = modToShow.name;
            modTaglineLabel.Text = modToShow.tagLine;
            icon.Source = new BitmapImage(new Uri(modToShow.iconLink));
        }
    }
}
