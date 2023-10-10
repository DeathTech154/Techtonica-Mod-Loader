using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
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
    /// Interaction logic for FloatOptionPanel.xaml
    /// </summary>
    public partial class FloatOptionPanel : UserControl
    {
        public FloatOptionPanel()
        {
            InitializeComponent();
        }

        public FloatOptionPanel(FloatConfigOption option) {
            InitializeComponent();
            ShowConfigOption(option);
        }

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(float), typeof(FloatOptionPanel), new PropertyMetadata(0f, onValueChanged));

        public float Value {
            get => (float)GetValue(ValueProperty);
            set {
                SetValue(ValueProperty, value);
                onValueChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, value, value));
            }
        }

        private static void onValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            FloatOptionPanel thisFloatOptionPanel = obj as FloatOptionPanel;
            if (thisFloatOptionPanel.inputBox.Input.EndsWith(".")) return;

            if(thisFloatOptionPanel.min != float.MinValue && thisFloatOptionPanel.Value < thisFloatOptionPanel.min) {
                thisFloatOptionPanel.Value = thisFloatOptionPanel.min;
            }
            else if (thisFloatOptionPanel.max != float.MaxValue && thisFloatOptionPanel.Value > thisFloatOptionPanel.max) {
                thisFloatOptionPanel.Value = thisFloatOptionPanel.max;
            }
        }

        #endregion

        // Objects & Variables

        private float min;
        private float max;

        // Events

        private void OnValueBoxChangesConfirmed(object sender, EventArgs e) {
            ModConfig.activeConfig.UpdateSetting(nameLabel.Content.ToString(), Value);
        }

        // Private Functions

        private void ShowConfigOption(FloatConfigOption option) {
            nameLabel.Content = option.name;
            min = option.min;
            max = option.max;
            Value = option.value;

            if (min != float.MinValue && max != float.MaxValue) {
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
