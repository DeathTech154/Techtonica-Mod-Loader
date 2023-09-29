using System;
using System.Collections.Generic;
using System.IO;
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

namespace Techtonica_Mod_Loader.Panels
{
    /// <summary>
    /// Interaction logic for SettingsPanel.xaml
    /// </summary>
    public partial class SettingsPanel : UserControl
    {
        public SettingsPanel() {
            InitializeComponent();
        }

        // Events

        private void OnSettingsPanelLoaded(object sender, RoutedEventArgs e) {
            gameFolderOption.ShowConfigOption(new StringConfigOption() {
                name = "Game Folder",
                description = "The location of Techtonica's installation",
                value = Settings.userSettings.gameFolder,
            });

            defaultSortOptionSetting.SetOptions(StringUtils.GetAllModListSortOptionNames());
            defaultSortOptionSetting.SetOption(StringUtils.GetModListSortOptionName(Settings.userSettings.defaultSort));

            defaultModListSetting.SetOptions(StringUtils.GetAllModListSourceNames());
            defaultModListSetting.SetOption(StringUtils.GetModListSourceName(Settings.userSettings.defaultModList));

            hideToolsOption.ShowConfigOption(new BooleanConfigOption() {
                name = "Hide Tools From Online Mods",
                description = "When enabled, the online mods list will no longer show packages marked as 'Tools'.",
                value = Settings.userSettings.hideTools
            });

            renderImagesOption.ShowConfigOption(new BooleanConfigOption() {
                name = "Render Images",
                description = "Speed up description load times by skipping images.",
                value = Settings.userSettings.renderImages
            });

            cacheImagesOption.ShowConfigOption(new BooleanConfigOption() {
                name = "Cache Images",
                description = "Save images so that they load faster next time they are viewed.",
                value = Settings.userSettings.cacheImages
            });

            logDebugMessagesOption.ShowConfigOption(new BooleanConfigOption() {
                name = "Log Debug Messages",
                description = "When enabled, TML will log debug messages. Could create a large log file.",
                value = Settings.userSettings.logDebugMessages
            });
        }

        private void OnBackToModsClicked(object sender, EventArgs e) {
            MainWindow.current.LoadDefaultModList();
        }

        private void OnFindGameFolderClicked(object sender, EventArgs e) {
            if (FileStructureUtils.FindSteamGameFolder()) {
                gameFolderOption.Value = ProgramData.Paths.gameFolder;
                return;
            }

            GuiUtils.ShowWarningMessage("Couldn't Find Game Folder", 
                                        "TML was unable to locate your Techtonica installation folder.\n" +
                                        "Please browse for it or enter it manually with the options below.");
        }

        private void OnBrowseForGameFolderClicked(object sender, EventArgs e) {
            System.Windows.Forms.FolderBrowserDialog browser = new System.Windows.Forms.FolderBrowserDialog();
            if(browser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                string techtonicaPath = $"{browser.SelectedPath}/Techtonica.exe";
                if (File.Exists(techtonicaPath)) {
                    Settings.userSettings.gameFolder = browser.SelectedPath;
                    gameFolderOption.Value = browser.SelectedPath;
                }
                else {
                    GuiUtils.ShowWarningMessage("Invalid Location", "This folder does not contain the file 'Techtonica.exe', please try again.");
                    gameFolderOption.Value = "";
                    Settings.userSettings.gameFolder = "";
                }

                Settings.Save();
            }
        }

        private void OnGameFolderChangesConfirmed(object sender, EventArgs e) {
            string newPath = gameFolderOption.Value;
            string techtonicaPath = $"{newPath}\\Techtonica.exe";
            if (File.Exists(techtonicaPath)) {
                Settings.userSettings.gameFolder = techtonicaPath;
            }
            else {
                GuiUtils.ShowWarningMessage("Invalid Location", "This folder does not contain the file 'Techtonica.exe', please try again.");
                gameFolderOption.Value = "";
                Settings.userSettings.gameFolder = "";
            }

            Settings.Save();
        }

        private void OnDefaultSortOptionSelectedOptionChanged(object sender, EventArgs e) {
            Settings.userSettings.defaultSort = StringUtils.GetModListSortOptionFromName(defaultSortOptionSetting.selectedOption);
            Settings.Save();
        }

        private void OnDefaultModListSelectedOptionChanged(object sender, EventArgs e) {
            Settings.userSettings.defaultModList = StringUtils.GetModListSourceFromName(defaultModListSetting.selectedOption);
            Settings.Save();
        }

        private void OnHideToolsValueChanged(object sender, EventArgs e) {
            Settings.userSettings.hideTools = hideToolsOption.Value;
            Settings.Save();
        }

        private void OnRenderImagesValueChanged(object sender, EventArgs e) {
            Settings.userSettings.renderImages = renderImagesOption.Value;
            Settings.Save();
        }

        private void OnCacheImagesValueChanged(object sender, EventArgs e) {
            Settings.userSettings.cacheImages = cacheImagesOption.Value;
            Settings.Save();
        }

        private void OnClearImageCacheClicked(object sender, EventArgs e) {
            ImageCache.ClearCache();
            GuiUtils.ShowInfoMessage("Cache Cleared", "Image cache has been cleared");
        }

        private void OnLogDebugMessagesValueChanged(object sender, EventArgs e) {
            Settings.userSettings.logDebugMessages = logDebugMessagesOption.Value;
            Settings.Save();
        }
    }
}
