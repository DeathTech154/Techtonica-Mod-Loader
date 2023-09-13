using SharpVectors.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Techtonica_Mod_Loader.Controls
{
    /// <summary>
    /// Interaction logic for HyperlinkImage.xaml
    /// </summary>
    public partial class HyperlinkImage : UserControl
    {
        public HyperlinkImage()
        {
            InitializeComponent();
        }

        public HyperlinkImage(string link) {
            InitializeComponent();
            url = link;
        }

        // Objects & Variables
        private string url;

        // Events

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            GuiUtils.OpenURL(url);
        }

        // Public Functions

        public async Task<string> ShowImage(string imageLink) {
            if (imageLink.EndsWith(".png") || imageLink.EndsWith(".jpg")) {
                Image newImage = await GuiUtils.GetImageFromURL(imageLink);
                if (newImage != null) {
                    mainGrid.Children.Add(newImage);
                    Width = newImage.Width;
                    Height = newImage.Height;
                }
            }
            else {
                SvgViewbox svg = await GuiUtils.GetSVGViewboxFromURL(imageLink);
                if (svg != null) {
                    mainGrid.Children.Add(svg);
                    Width = svg.Width;
                    Height = svg.Height;
                }
            }

            return "";
        }
    }
}
