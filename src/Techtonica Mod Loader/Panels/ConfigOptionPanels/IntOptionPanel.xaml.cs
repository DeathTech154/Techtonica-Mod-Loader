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
    /// Interaction logic for IntOptionPanel.xaml
    /// </summary>
    public partial class IntOptionPanel : UserControl
    {
        public IntOptionPanel()
        {
            InitializeComponent();
        }

        public IntOptionPanel(IntConfigOption option) {
            InitializeComponent();
            ShowConfigOption(option);
        }

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(int), typeof(IntOptionPanel), new PropertyMetadata(0, onValueChanged));

        public int Value {
            get => (int)GetValue(ValueProperty);
            set {
                SetValue(ValueProperty, value);
                onValueChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, value, value));
            }
        }

        private static void onValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            IntOptionPanel thisIntOptionPanel = obj as IntOptionPanel;
            if(thisIntOptionPanel.min != int.MinValue && thisIntOptionPanel.Value < thisIntOptionPanel.min) {
                thisIntOptionPanel.Value = thisIntOptionPanel.min;
            }
            else if (thisIntOptionPanel.max != int.MaxValue && thisIntOptionPanel.Value > thisIntOptionPanel.max) {
                thisIntOptionPanel.Value = thisIntOptionPanel.max;
            }
        }

        #endregion

        // Objects & Variables

        private int min;
        private int max;

        // Events

        private void OnValueBoxChangesConfirmed(object sender, EventArgs e) {
            ModConfig.activeConfig.UpdateSetting(nameLabel.Content.ToString(), Value);
        }

        // Private Functions

        private void ShowConfigOption(IntConfigOption option) {
            nameLabel.Content = option.name;
            min = option.min;
            max = option.max;
            Value = option.value;

            if(min != int.MinValue && max != int.MaxValue) {
                AddInfoLabel($"Range: {min} to {max}");
            }

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
