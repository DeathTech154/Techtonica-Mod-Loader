using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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
        private bool isExpanded;
        private bool hasMarkdownLoaded;

        // Events

        private async void OnPanelMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (downloadButton.IsMouseOver) return;

            isExpanded = !isExpanded;
            string animKey = isExpanded ? "growAnim" : "shrinkAnim";
            BeginStoryboard((Storyboard)Resources[animKey]);

            // Don't use 'mod' to avoid multiple callbacks attached to FinishedDownloading
            if (!hasMarkdownLoaded) {
                Mod temp = await ThunderStore.GetMod(mod.id);
                temp.FinishedDownloading += TempModFinishedDownloading;
                temp.DownloadAsTemp();
            }
        }

        private void TempModFinishedDownloading(object sender, EventArgs e) {
            hasMarkdownLoaded = true;
            if (File.Exists(ProgramData.Paths.tempMarkdownFile)) {
                markdownViewer.ViewMarkdownFromFile(ProgramData.Paths.tempMarkdownFile);
            }
            else {
                markdownViewer.ViewMarkdown("# No description available");
            }
        }

        private async void OnDownloadClicked(object sender, EventArgs e) {
            GuiUtils.ShowDownloadingGui(mod);
            ModManager.AddIfNew(mod);
            await mod.Download();
        }

        // Public Functions

        public void showMod(Mod modToShow) {
            mod = modToShow;
            modNameLabel.Text = modToShow.name;
            modTaglineLabel.Text = modToShow.tagLine;
            icon.Source = new BitmapImage(new Uri(modToShow.iconLink));
            markdownViewer.ViewMarkdown("Loading, please wait...");
        }
    }
}
