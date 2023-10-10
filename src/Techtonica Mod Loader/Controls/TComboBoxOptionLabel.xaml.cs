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
    /// Interaction logic for TComboBoxOptionLabel.xaml
    /// </summary>
    public partial class TComboBoxOptionLabel : UserControl
    {
        public TComboBoxOptionLabel()
        {
            InitializeComponent();
        }

        public TComboBoxOptionLabel(string item) {
            InitializeComponent();
            ShowItem(item);
        }

        // Custom Events

        public event EventHandler LeftClicked;

        // Public Functions

        public string GetItem() {
            return label.Text.ToString();
        }

        public void ShowItem(string item) {
            label.Text = item;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            LeftClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
