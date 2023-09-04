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
    /// <summary>
    /// Interaction logic for BooleanOptionPanel.xaml
    /// </summary>
    public partial class BooleanOptionPanel : UserControl
    {
        public BooleanOptionPanel()
        {
            InitializeComponent();
        }

        public BooleanOptionPanel(BooleanConfigOption option) {
            InitializeComponent();
            ShowConfigOption(option);
        }

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(bool), typeof(BooleanOptionPanel), new PropertyMetadata(false));

        public bool Value {
            get => (bool)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        // Events

        private void OnCheckBoxIsCheckedChanged(object sender, EventArgs e) {
            ModConfig.activeConfig.UpdateSetting(nameLabel.Content.ToString(), Value);
        }

        // Private Functions

        private void ShowConfigOption(BooleanConfigOption option) {
            nameLabel.Content = option.name;
            Value = option.value;

            if (!string.IsNullOrEmpty(option.description)) {
                AddInfoLabel(option.description);
            }
        }

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
