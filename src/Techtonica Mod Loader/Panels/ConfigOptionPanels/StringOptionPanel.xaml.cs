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

namespace Techtonica_Mod_Loader.Panels.ConfigOptionPanels
{
    public partial class StringOptionPanel : UserControl
    {
        public StringOptionPanel()
        {
            InitializeComponent();
        }

        public StringOptionPanel(StringConfigOption option) { 
            InitializeComponent();
            ShowConfigOption(option);
        }

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(string), typeof(StringOptionPanel), new PropertyMetadata(""));

        public string Value {
            get => (string)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        #region IsTMLSetting Property

        public static readonly DependencyProperty IsTMLSettingProperty = DependencyProperty.Register("IsTMLSetting", typeof(bool), typeof(StringOptionPanel), new PropertyMetadata(false));

        public bool IsTMLSetting {
            get => (bool)GetValue(IsTMLSettingProperty);
            set => SetValue(IsTMLSettingProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler ChangesConfirmed;

        // Events

        private void OnValueBoxChangesConfirmed(object sender, EventArgs e) {
            if (!IsTMLSetting) {
                ModConfig.activeConfig.UpdateSetting(nameLabel.Content.ToString(), Value);
            }

            ChangesConfirmed?.Invoke(this, EventArgs.Empty);
        }

        // Public Functions

        public void ShowConfigOption(StringConfigOption option) {
            nameLabel.Content = option.name;
            Value = option.value;

            if (!string.IsNullOrEmpty(option.description)) {
                AddInfoLabel(option.description);
            }
        }

        // Private Functions

        private void AddInfoLabel(string info) {
            mainPanel.Children.Insert(1, new TextBlock() {
                Text = info,
                Foreground = Brushes.White,
                FontSize = 14,
                Margin = new Thickness(5, 5, 5, 0)
            });
        }
    }
}
