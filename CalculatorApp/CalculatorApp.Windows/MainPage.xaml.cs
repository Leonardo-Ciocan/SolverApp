using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
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
            this.Loaded += PageLoaded;
            this.Unloaded += PageUnloaded;
            if (e.Parameter != null)
            {
                btnSubs.IsEnabled = true;
                notebook = e.Parameter as Sheet;
                App.Model.OpenNotebook = e.Parameter as Sheet;
                actualList.DataContext = App.Model.OpenNotebook;
                actualList.Visibility = Visibility.Visible;
                
            }
        }

        private void PageUnloaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged -= Window_SizeChanged;
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            Window.Current.SizeChanged += Window_SizeChanged;
        }

        private void Window_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            //if (e.Size.Width <= 500)
            //{
            //    //VisualStateManager.GoToState(this, state.State, transitions);
            //}
            //else if (e.Size.Height > e.Size.Width)
            //{
            //    //VisualStateManager.GoToState(this, state.State, transitions);
            //}
            //else
            //{
            //    //VisualStateManager.GoToState(this, state.State, transitions);
            //}

            var state = string.Empty;
            var applicationView = ApplicationView.GetForCurrentView();
            var size = Window.Current.Bounds;

            if (applicationView.IsFullScreen)
            {
                state = applicationView.Orientation == ApplicationViewOrientation.Landscape ? "Filled" : "Snapped";
            }
            else
            {
               
                state = size.Width <= 700 ? "Snapped" : "Filled";
            }

            root.Margin = state == "Snapped" ? new Thickness(0) : new Thickness(423,0,0,0);

            System.Diagnostics.Debug.WriteLine("Width: {0}, New VisulState: {1}",
                size.Width, state);

            VisualStateManager.GoToState(this, state, true);
        }

        private void addLine(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            
            //i++;  
            //holder.Children.Add(new SyntaxLabel(i));
            //(holder.Children.Last() as SyntaxLabel).Focus(FocusState.Keyboard);
        }

        

        async void SheetListPage_Loaded(object sender, RoutedEventArgs e)
        {
            tile.DataContext = App.Model.OpenNotebook;
            tileSmall.DataContext = App.Model.OpenNotebook;
            btnPin.Click += pin;
            root.Background = AppSettings.Themes[App.Model.Settings.ThemeIndex].Background;
            list.ItemsSource = Core.Sheets;
            list.IsItemClickEnabled = true;
            list.ItemClick += (a, b) =>
            {

                App.Model.OpenNotebook = b.ClickedItem as Sheet;
                tile.DataContext = App.Model.OpenNotebook;
                tileSmall.DataContext = App.Model.OpenNotebook;
                btnSubs.IsEnabled = true;
                notebook = b.ClickedItem as Sheet;
                actualList.DataContext = App.Model.OpenNotebook;
                actualList.Visibility = Visibility.Visible;
            };
        }

        /*void populateWithSheet(Sheet s)
        {
            actualList.Items.Clear();
            for (int x = 0; x < s.Lines.Count; x++)
            {
                var label = new SyntaxLabel(x);
                label.DataContext = App.Model.OpenNotebook.Lines[x];
                
                label.Tag = x;
                label.KeyDown += (a, b) =>
                {
                    if (b.Key == VirtualKey.Enter)
                    {
                        (actualList.Items[(int) (a as SyntaxLabel).Tag + 1] as SyntaxLabel).Focus(FocusState.Keyboard);
                    }
                    else if (b.Key == VirtualKey.Back)
                    {
                        
                    }
                };
                label.init();
                actualList.Items.Add(label);
            }
        }*/

        

        

        private void new_sheet_clicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Core.Sheets.Add(new Sheet { });
        }

        private void add_new_line(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            App.Model.OpenNotebook.Lines.Add(new Line { LineNumber = App.Model.OpenNotebook.Lines.Count + 1, SheetID = notebook.ID });
        }

        private void show_settings(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	var settings = new AppSettingsUI();
            settings.Show();
        }

        private void show_subs(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	var panel = new SubstitutionsPanel();
            panel.DataContext = App.Model.OpenNotebook;
            panel.Show();
        }

        private async void pin(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!btnPin.IsChecked.Value)
            {
                var tilex = (await SecondaryTile.FindAllAsync()).
                FirstOrDefault(x => x.TileId == notebook.ID.Substring(0, 10));
                if(tilex!=null) await (tilex).RequestDeleteAsync();
            }
            else
            {
                TileManager.SaveAndPin(tile, tileSmall, notebook.ID);
            }
        }
    }
}
