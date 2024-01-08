using MyLogger;
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

namespace Techtonica_Mod_Loader.Controls
{
    /// <summary>
    /// Interaction logic for SVGButton.xaml
    /// </summary>
    public partial class SVGButton : UserControl
    {
        public SVGButton() {
            InitializeComponent();
        }

        // Properties

        #region Source Property

        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(string), typeof(SVGButton), new PropertyMetadata("", OnSourceChanged));

        public string Source {
            get => (string)GetValue(SourceProperty);
            set {
                SetValue(SourceProperty, value);
                OnSourceChanged(this, new DependencyPropertyChangedEventArgs(SourceProperty, value, value));
            }
        }

        private static void OnSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
            SVGButton thisSVGButton = obj as SVGButton;
            string path = $"{ProgramData.Paths.resourcesFolder}/{thisSVGButton.Source}.svg";
            if (!File.Exists(path)) {
                Log.Error($"Could not find SVG file: '{path}'");
                return;
            }
            else {
                thisSVGButton.svg.Source = new Uri(path);
            }
        }

        #endregion

        #region SVGMargin Property

        public static readonly DependencyProperty SVGMarginProperty = DependencyProperty.Register("SVGMargin", typeof(Thickness), typeof(SVGButton), new PropertyMetadata(new Thickness(0)));

        public Thickness SVGMargin {
            get => (Thickness)GetValue(SVGMarginProperty);
            set => SetValue(SVGMarginProperty, value);
        }

        #endregion

        #region Tip Property

        public static readonly DependencyProperty TipProperty = DependencyProperty.Register("Tip", typeof(string), typeof(SVGButton), new PropertyMetadata(""));

        public string Tip {
            get => (string)GetValue(TipProperty);
            set => SetValue(TipProperty, value);
        }

        #endregion

        // Custom Events

        public event EventHandler LeftClicked;
        public event EventHandler RightClicked;

        // Events

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            LeftClicked?.Invoke(this, EventArgs.Empty);
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            RightClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
