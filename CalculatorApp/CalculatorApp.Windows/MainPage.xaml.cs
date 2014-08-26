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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CalculatorApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            Loaded += SheetListPage_Loaded;
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            Core.LoadAll();
        }

        Sheet notebook;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            DataContext = App.Model;
            
        }

        private void addLine(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            //i++;
            //holder.Children.Add(new SyntaxLabel(i));
            //(holder.Children.Last() as SyntaxLabel).Focus(FocusState.Keyboard);
            App.Model.OpenNotebook.Lines.Add(new Line { LineNumber = App.Model.OpenNotebook.Lines.Count + 1, SheetID = notebook.ID });
        }

        

        async void SheetListPage_Loaded(object sender, RoutedEventArgs e)
        {
            //root.Background = AppSettings.Themes[App.Model.Settings.ThemeIndex].Background;
            list.ItemsSource = Core.Sheets;
            list.IsItemClickEnabled = true;
            list.ItemClick += (a, b) =>
            {
                notebook = b.ClickedItem as Sheet;
                App.Model.OpenNotebook = b.ClickedItem as Sheet;
                actualList.DataContext = App.Model.OpenNotebook;
            };
        }

        
        private void add(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Core.Sheets.Add(new Sheet { });
        }
    }
}
