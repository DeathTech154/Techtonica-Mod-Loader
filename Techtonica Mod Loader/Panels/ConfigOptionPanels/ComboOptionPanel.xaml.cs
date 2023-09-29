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

namespace Techtonica_Mod_Loader.Panels.ConfigOptionPanels
{
    /// <summary>
    /// Interaction logic for ComboOptionPanel.xaml
    /// </summary>
    public partial class ComboOptionPanel : UserControl
    {
        public ComboOptionPanel() {
            InitializeComponent();
        }

        // Properties

        #region SettingName Property

        public static readonly DependencyProperty SettingNameProperty = DependencyProperty.Register("SettingName", typeof(string), typeof(ComboOptionPanel), new PropertyMetadata("Name"));

        public string SettingName {
            get => (string)GetValue(SettingNameProperty);
            set => SetValue(SettingNameProperty, value);
        }

        #endregion

        #region SettingDescription Property

        public static readonly DependencyProperty SettingDescriptionProperty = DependencyProperty.Register("SettingDescription", typeof(string), typeof(ComboOptionPanel), new PropertyMetadata("Description"));

        public string SettingDescription {
            get => (string)GetValue(SettingDescriptionProperty);
            set => SetValue(SettingDescriptionProperty, value);
        }

        #endregion

        // Objects & Variables
        public string selectedOption;

        // Custom Events
        public event EventHandler SelectedOptionChanged;

        // Events

        private void OnOptionsComboSelectedOptionChanged(object sender, EventArgs e) {
            selectedOption = optionsCombo.SelectedItem;
            SelectedOptionChanged?.Invoke(this, EventArgs.Empty);
        }

        // Public Functions

        public void SetOption(string option) {
            selectedOption = option;
            optionsCombo.SetSelectedItem(option);
        }

        public void SetOptions(List<string> options) {
            optionsCombo.SetItems(options);
            if (!options.Contains(optionsCombo.SelectedItem)) {
                optionsCombo.SetSelectedItem(0);
            }
        }
    }
}
