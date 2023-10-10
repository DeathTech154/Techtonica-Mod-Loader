using Newtonsoft.Json.Linq;
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
using System.Windows.Shell;
using Techtonica_Mod_Loader.Classes;

namespace Techtonica_Mod_Loader.Panels.ConfigOptionPanels
{
    /// <summary>
    /// Interaction logic for ButtonOptionPanel.xaml
    /// </summary>
    public partial class ButtonOptionPanel : UserControl
    {
        public ButtonOptionPanel() {
            InitializeComponent();
        }

        // Objects & Variables

        #region SettingName Property

        public static readonly DependencyProperty SettingNameProperty = DependencyProperty.Register("SettingName", typeof(string), typeof(ButtonOptionPanel), new PropertyMetadata("Name"));

        public string SettingName {
            get => (string)GetValue(SettingNameProperty);
            set => SetValue(SettingNameProperty, value);
        }

        #endregion

        #region SettingDescription Property

        public static readonly DependencyProperty SettingDescriptionProperty = DependencyProperty.Register("SettingDescription", typeof(string), typeof(ButtonOptionPanel), new PropertyMetadata("Description"));

        public string SettingDescription {
            get => (string)GetValue(SettingDescriptionProperty);
            set => SetValue(SettingDescriptionProperty, value);
        }

        #endregion

        #region ButtonText Property

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(ButtonOptionPanel), new PropertyMetadata("Button Text"));

        public string ButtonText {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        #endregion

        // Custom Events
        public event EventHandler Clicked;

        // Events

        private void OnSettingButtonClicked(object sender, EventArgs e) {
            Clicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
