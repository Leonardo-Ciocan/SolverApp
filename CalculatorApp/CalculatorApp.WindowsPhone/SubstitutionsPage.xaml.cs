using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CalculatorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SubstitutionsPage : Page
    {
        public SubstitutionsPage()
        {
            this.InitializeComponent();
        }

        
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = e.Parameter;

            await StatusBar.GetForCurrentView().HideAsync();
        }

        private void add(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.Model.OpenNotebook.Substitutions.Add(new Substitution ( "",  "" ));
        }
    }
}
