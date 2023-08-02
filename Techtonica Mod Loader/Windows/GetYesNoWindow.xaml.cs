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
using System.Windows.Shapes;

namespace Techtonica_Mod_Loader.Windows
{
    public partial class GetYesNoWindow : Window
    {
        public GetYesNoWindow(string title, string description)
        {
            GuiUtils.ShowShader();
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            titleLabel.Content = title;
            descriptionBlock.Text = description;
        }

        // Events

        private void OnYesClicked(object sender, EventArgs e) {
            result = true;
            GuiUtils.HideShader();
            Close();
        }

        private void OnNoClicked(object sender, EventArgs e) {
            result = false;
            GuiUtils.HideShader();
            Close();
        }

        // Return Functions

        private bool result;
        private bool GetResult() { return result; }
        public static bool GetYesNo(string title, string description) {
            GetYesNoWindow window = new GetYesNoWindow(title, description);
            window.ShowDialog();
            return window.GetResult();
        }
    }
}
