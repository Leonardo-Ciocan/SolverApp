using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace CalculatorApp
{
    public sealed partial class AppSettingsUI : SettingsFlyout
    {
        public AppSettingsUI()
        {
            this.InitializeComponent();
            this.Loaded += AppSettingsUI_Loaded;
        }

        void AppSettingsUI_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = App.Model.Settings;
            themeBox.ItemsSource = AppSettings.Themes;
        }
    }
}
