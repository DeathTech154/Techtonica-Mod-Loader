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
    /// Interaction logic for TCheckBox.xaml
    /// </summary>
    public partial class TCheckBox : UserControl
    {
        public TCheckBox()
        {
            InitializeComponent();
        }

        #region IsChecked Property

        public static readonly DependencyProperty IsCheckedProperty = DependencyProperty.Register("IsChecked", typeof(bool), typeof(TCheckBox), new PropertyMetadata(false));

        public bool IsChecked {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }

        #endregion

        #region IsEditable Property

        public static readonly DependencyProperty IsEditableProperty = DependencyProperty.Register("IsEditable", typeof(bool), typeof(TCheckBox), new PropertyMetadata(true));

        public bool IsEditable {
            get => (bool)GetValue(IsEditableProperty);
            set => SetValue(IsEditableProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler IsCheckedChanged;

        // Events

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (IsEditable) {
                IsChecked = !IsChecked;
                IsCheckedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
