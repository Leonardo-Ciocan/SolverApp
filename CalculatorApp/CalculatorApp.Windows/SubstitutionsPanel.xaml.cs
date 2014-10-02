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
    public sealed partial class SubstitutionsPanel : SettingsFlyout
    {
        public SubstitutionsPanel()
        {
            this.InitializeComponent();
        }
        private void addSub(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var context = DataContext;
            App.Model.OpenNotebook.Substitutions.Add(new Substitution("", ""));
        }
    }
}
