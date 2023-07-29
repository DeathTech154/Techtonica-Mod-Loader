using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for TComboBox.xaml
    /// </summary>
    public partial class TComboBox : UserControl
    {
        public TComboBox()
        {
            InitializeComponent();
        }

        // Properties

        #region SelectedItem Property

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(string), typeof(TComboBox), new PropertyMetadata(""));

        public string SelectedItem {
            get => (string)GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        #endregion

        #region Items Property

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register("Items", typeof(List<string>), typeof(TComboBox), new PropertyMetadata(new List<string>()));

        public List<string> Items {
            get => (List<string>)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler SelectedItemChanged;

        // Events

        private void comboBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if (!itemsPopup.IsMouseOver && !itemsPopup.IsOpen) {
                openOptions();
            }
            else if(itemsPopup.IsOpen) {
                closeOptions();
            }
        }

        private void comboBoxMouseLeave(object sender, MouseEventArgs e) {
            if(!thisControl.IsMouseOver && !itemsPopup.IsMouseOver) {
                closeOptions();
            }
        }

        private void optionLabelClicked(object sender, EventArgs e) {
            TComboBoxOptionLabel clickedLabel = sender as TComboBoxOptionLabel;
            setSelectedItem(clickedLabel.getItem());
            closeOptions();
        }

        // Public Functions

        public void setSelectedItem(string item) {
            SelectedItem = item;
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            if (!Items.Contains(item)) {
                DebugUtils.CrashIfDebug($"{item} is not a member of Items: {string.Join(" ,", Items)}");
            }
        }

        public void setSelectedItem(int index) {
            if(index < Items.Count) {
                SelectedItem = Items[index];
            }
            else {
                SelectedItem = Items.Last();
                DebugUtils.CrashIfDebug($"Index {index} is out of range ({Items.Count})");
            }

            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        public void setItems(List<string> items) {
            Items = items;
            populateItems();
            setSelectedItem(0);
        }

        public void setItems(List<int> items) {
            List<string> itemsAsString = new List<string>();
            foreach(int item in items) {
                itemsAsString.Add(item.ToString());
            }

            setItems(items);
        }

        // Private Functions

        private void populateItems() {
            itemsPanel.Children.Clear();
            foreach(string item in Items) {
                addItemLabelToOptions(item);
            }
        }

        private void addItemLabelToOptions(string item) {
            TComboBoxOptionLabel label = new TComboBoxOptionLabel(item) {
                Height = 40
            };
            label.LeftClicked += optionLabelClicked;
            itemsPanel.Children.Add(label);
        }

        private void openOptions() {
            itemsPopup.IsOpen = true;
            rotator.Angle = 180;
            arrow.Margin = new Thickness(0, 2, 0, -2);
        }

        private void closeOptions() {
            itemsPopup.IsOpen = false;
            rotator.Angle = 0;
            arrow.Margin = new Thickness(0, -2, 0, 2);
        }
    }
}
