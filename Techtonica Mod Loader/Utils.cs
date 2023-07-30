using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Techtonica_Mod_Loader.Windows;

namespace Techtonica_Mod_Loader
{
    public static class DebugUtils
    {
        public static void CrashIfDebug(string errorMessage) {
            if (ProgramData.isDebugBuild) {
                throw new Exception(errorMessage);
            }
        }

        public static void SendDebugLine(string Str) {
            Console.WriteLine(Str);
            Debug.WriteLine(Str);
        }
    }

    public static class GuiUtils 
    {
        public static void showShader() {
            MainWindow.current.shader.Visibility = System.Windows.Visibility.Visible;
        }

        public static void hideShader() {
            MainWindow.current.shader.Visibility = System.Windows.Visibility.Hidden;
        }

        public static void showInfoMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.showInfo(title, description, closeButtonText);
        }

        public static void showWarningMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.showWarning(title, description, closeButtonText);
        }

        public static void showErrorMessage(string title, string description, string closeButtonText = "Close") {
            WarningWindow.showError(title, description, closeButtonText);
        }

        public static bool getUserConfirmation(string title, string description) {
            return GetYesNoWindow.getYesNo(title, description);
        }
    }
}
