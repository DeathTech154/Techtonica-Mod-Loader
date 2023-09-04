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
    /// Interaction logic for DoubleOptionPanel.xaml
    /// </summary>
    public partial class DoubleOptionPanel : UserControl
    {
        public DoubleOptionPanel()
        {
            InitializeComponent();
        }

        public DoubleOptionPanel(DoubleConfigOption option) {
            InitializeComponent();
            ShowConfigOption(option);
        }

        // Objects & Variables

        private double min;
        private double max;

        // Properties

        #region Value Property

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(double), typeof(DoubleOptionPanel), new PropertyMetadata(0d, OnValueChanged));

        public double Value {
            get => (double)GetValue(ValueProperty);
            set {
                SetValue(ValueProperty, value);
                OnValueChanged(this, new DependencyPropertyChangedEventArgs(ValueProperty, value, value));
            }
        }

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            DoubleOptionPanel thisDoubleOptionPanel = obj as DoubleOptionPanel;
            if (thisDoubleOptionPanel.inputBox.Input.EndsWith(".")) return;

            if (thisDoubleOptionPanel.min != double.MinValue && thisDoubleOptionPanel.Value < thisDoubleOptionPanel.min) {
                thisDoubleOptionPanel.Value = thisDoubleOptionPanel.min;
            }
            else if (thisDoubleOptionPanel.max != double.MaxValue && thisDoubleOptionPanel.Value > thisDoubleOptionPanel.max) {
                thisDoubleOptionPanel.Value = thisDoubleOptionPanel.max;
            }
        }

        #endregion

        // Events

        private void OnValueBoxChangesConfirmed(object sender, EventArgs e) {
            ModConfig.activeConfig.UpdateSetting(nameLabel.Content.ToString(), Value);
        }

        // Private Functions

        private void ShowConfigOption(DoubleConfigOption option) {
            nameLabel.Content = option.name;
            min = option.min;
            max = option.max;
            Value = option.value;

            if (min != double.MinValue && max != double.MaxValue) {
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
