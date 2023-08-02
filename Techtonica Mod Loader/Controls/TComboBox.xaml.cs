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

        private void OnComboBoxMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if (!itemsPopup.IsMouseOver && !itemsPopup.IsOpen) {
                OpenOptions();
            }
            else if(itemsPopup.IsOpen) {
                CloseOptions();
            }
        }

        private void OnComboBoxMouseLeave(object sender, MouseEventArgs e) {
            if(!thisControl.IsMouseOver && !itemsPopup.IsMouseOver) {
                CloseOptions();
            }
        }

        private void OnOptionLabelClicked(object sender, EventArgs e) {
            TComboBoxOptionLabel clickedLabel = sender as TComboBoxOptionLabel;
            SetSelectedItem(clickedLabel.GetItem());
            CloseOptions();
        }

        // Public Functions

        public void SetSelectedItem(string item) {
            SelectedItem = item;
            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
            if (!Items.Contains(item)) {
                DebugUtils.CrashIfDebug($"{item} is not a member of Items: {string.Join(" ,", Items)}");
            }
        }

        public void SetSelectedItem(int index) {
            if(index < Items.Count) {
                SelectedItem = Items[index];
            }
            else {
                SelectedItem = Items.Last();
                DebugUtils.CrashIfDebug($"Index {index} is out of range ({Items.Count})");
            }

            SelectedItemChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SetItems(List<string> items) {
            Items = items;
            PopulateItems();
            SetSelectedItem(0);
        }

        public void SetItems(List<int> items) {
            List<string> itemsAsString = new List<string>();
            foreach(int item in items) {
                itemsAsString.Add(item.ToString());
            }

            SetItems(items);
        }

        // Private Functions

        private void PopulateItems() {
            itemsPanel.Children.Clear();
            foreach(string item in Items) {
                AddItemLabelToOptions(item);
            }
        }

        private void AddItemLabelToOptions(string item) {
            TComboBoxOptionLabel label = new TComboBoxOptionLabel(item) {
                Height = 40
            };
            label.LeftClicked += OnOptionLabelClicked;
            itemsPanel.Children.Add(label);
        }

        private void OpenOptions() {
            itemsPopup.IsOpen = true;
            rotator.Angle = 180;
            arrow.Margin = new Thickness(0, 2, 0, -2);
        }

        private void CloseOptions() {
            itemsPopup.IsOpen = false;
            rotator.Angle = 0;
            arrow.Margin = new Thickness(0, -2, 0, 2);
        }
    }
}
