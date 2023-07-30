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
    /// Interaction logic for TTextBox.xaml
    /// </summary>
    public partial class TTextBox : UserControl
    {
        public TTextBox()
        {
            InitializeComponent();
        }

        // Properties

        #region Input Property

        public static readonly DependencyProperty InputProperty = DependencyProperty.Register("Input", typeof(string), typeof(TTextBox), new PropertyMetadata(""));

        public string Input {
            get => (string)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }

        #endregion

        #region Hint Property

        public static readonly DependencyProperty HintProperty = DependencyProperty.Register("Hint", typeof(string), typeof(TTextBox), new PropertyMetadata("Hint..."));

        public string Hint {
            get => (string)GetValue(HintProperty);
            set => SetValue(HintProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler EnterPressed;
        public event EventHandler EscapePressed;
        public event EventHandler KeyPressed;
        public event EventHandler ChangesConfirmed;

        // Events

        private void inputBoxPreviewKeyUp(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.Enter: 
                    EnterPressed?.Invoke(this, EventArgs.Empty); 
                    ChangesConfirmed?.Invoke(this, EventArgs.Empty);
                    break;

                case Key.Escape:
                    EscapePressed?.Invoke(this, EventArgs.Empty);
                    ChangesConfirmed?.Invoke(this, EventArgs.Empty);
                    Keyboard.ClearFocus();
                    break;

                default:
                    KeyPressed?.Invoke(this, EventArgs.Empty);
                    break;
            }
        }

        private void inputBoxLostFocus(object sender, RoutedEventArgs e) {
            ChangesConfirmed?.Invoke(this, EventArgs.Empty);
        }
    }
}
