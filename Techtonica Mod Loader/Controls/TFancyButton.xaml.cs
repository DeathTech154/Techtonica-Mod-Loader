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

namespace Techtonica_Mod_Loader.Controls
{
    /// <summary>
    /// Interaction logic for TFancyButton.xaml
    /// </summary>
    public partial class TFancyButton : UserControl
    {
        public TFancyButton()
        {
            InitializeComponent();
        }

        // Properties

        #region ButtonText Property

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register("ButtonText", typeof(string), typeof(TFancyButton), new PropertyMetadata(""));

        public string ButtonText {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler LeftClicked;
        public event EventHandler RightClicked;

        // Events

        private void mouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            LeftClicked?.Invoke(this, EventArgs.Empty);
        }

        private void mouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
