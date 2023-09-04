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
using Techtonica_Mod_Loader.Controls;
using Techtonica_Mod_Loader.Panels.ConfigOptionPanels;

namespace Techtonica_Mod_Loader.Panels
{
    /// <summary>
    /// Interaction logic for ModConfigPanel.xaml
    /// </summary>
    public partial class ModConfigPanel : UserControl
    {
        public ModConfigPanel()
        {
            InitializeComponent();
        }

        public ModConfigPanel(ModConfig config, string name) {
            InitializeComponent();
            modName = name;
            LoadModConfig(config);
        }

        // Objects & Variables

        private string modName;

        // Events

        private void OnBackToModsListClicked(object sender, EventArgs e) {
            MainWindow.current.LoadInstalledModList();
        }

        private void OnCategoryLabelClicked(object sender, EventArgs e) {
            TComboBoxOptionLabel label = sender as TComboBoxOptionLabel;
            string category = label.label.Text.Split(new string[] { " - " }, StringSplitOptions.None).First();
            LoadCategory(category);
        }

        // Private Functions

        private void LoadModConfig(ModConfig config) {
            foreach(string category in config.GetCategories()) {
                List<ConfigOption> options = config.GetOptionsInCategory(category);
                string labelText = $"{category} - ";
                labelText += options.Count == 1 ? "1 Option" : $"{options.Count} Options";
                TComboBoxOptionLabel label = new TComboBoxOptionLabel(labelText) {
                    Margin = new Thickness(2, 4, 2, 0)
                };
                label.LeftClicked += OnCategoryLabelClicked;
                categoriesPanel.Children.Add(label);
            }

            LoadCategory(config.GetCategories()[0]);
        }

        private void LoadCategory(string category) {
            optionsPanel.Children.Clear();
            categoryLabel.Content = $"{modName} - {category}";

            List<ConfigOption> options = ModConfig.activeConfig.GetOptionsInCategory(category);
            options = options.OrderBy(option => option.name).ToList();

            Thickness optionMargin = new Thickness(2, 4, 2, 0);
            foreach(ConfigOption option in options) {
                if(option is StringConfigOption stringOption) {
                    optionsPanel.Children.Add(new StringOptionPanel(stringOption) { Margin = optionMargin });
                }
                else if (option is IntConfigOption intOption) {
                    optionsPanel.Children.Add(new IntOptionPanel(intOption) { Margin = optionMargin });
                }
                else if (option is FloatConfigOption floatOption) {
                    optionsPanel.Children.Add(new FloatOptionPanel(floatOption) { Margin = optionMargin });
                }
                else if (option is DoubleConfigOption doubleOption) {
                    optionsPanel.Children.Add(new DoubleOptionPanel(doubleOption) { Margin = optionMargin });
                }
                else if (option is BooleanConfigOption booleanOption) {
                    optionsPanel.Children.Add(new BooleanOptionPanel(booleanOption) { Margin = optionMargin });
                }
                else {
                    string error = $"Cannot load OptionPanel for unknown option type";
                    DebugUtils.SendDebugLine(error);
                    DebugUtils.CrashIfDebug(error);
                }
            }
        }
    }
}
