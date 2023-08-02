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
    public partial class WarningWindow : Window
    {
        public WarningWindow(WarningType type, string title, string description, string closeButtonText)
        {
            GuiUtils.ShowShader();
            Owner = Application.Current.MainWindow;
            InitializeComponent();

            switch (type) {
                case WarningType.info:
                    titleLabel.Foreground = Brushes.White;
                    icon1.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Info.svg");
                    icon2.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Info.svg");
                    break;
                case WarningType.warning:
                    titleLabel.Foreground = Brushes.Yellow;
                    icon1.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Warning.svg");
                    icon2.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Warning.svg");
                    break;
                case WarningType.error:
                    titleLabel.Foreground = Brushes.Red;
                    icon1.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Error.svg");
                    icon2.Source = new Uri($"{ProgramData.Paths.resourcesFolder}\\Error.svg");
                    break;
            }

            titleLabel.Content = title;
            descriptionBlock.Text = description;
            closeButton.ButtonText = closeButtonText;
        }

        // Events

        private void OnCloseClicked(object sender, EventArgs e) {
            GuiUtils.HideShader();
            Close();
        }

        // Return Functions

        private string result;
        private string GetResult() { return result; }

        public static string ShowInfo(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.info, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }

        public static string ShowWarning(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.warning, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }

        public static string ShowError(string title, string description, string closeButtonText) {
            WarningWindow window = new WarningWindow(WarningType.error, title, description, closeButtonText);
            window.ShowDialog();
            return window.GetResult();
        }
    }

    public enum WarningType {
        info,
        warning,
        error
    }
}
