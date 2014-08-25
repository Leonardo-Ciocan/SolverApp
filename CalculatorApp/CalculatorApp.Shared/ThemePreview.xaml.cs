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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CalculatorApp
{
    public sealed partial class ThemePreview : UserControl
    {
        public ThemePreview()
        {
            this.InitializeComponent();
            DataContextChanged += ThemePreview_DataContextChanged;
        }

        void ThemePreview_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            //DataContext = AppSettings.Themes[(int)DataContext];
            int k;
        }
    }
}
