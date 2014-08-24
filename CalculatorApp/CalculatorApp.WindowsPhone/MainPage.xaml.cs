using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
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

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            
        }

        
        Sheet notebook ;//= new Sheet();
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            notebook = e.Parameter as Sheet;
            App.Model.OpenNotebook = notebook;
            await StatusBar.GetForCurrentView().HideAsync();
            
            DataContext = App.Model;

            root.Background = AppSettings.Themes[App.Model.Settings.ThemeIndex].Background;
            appbar.Background = AppSettings.Themes[App.Model.Settings.ThemeIndex].Background;
            appbar.Foreground = AppSettings.Themes[App.Model.Settings.ThemeIndex].RegularText;

            notebook.EvaluateAll();
        }

        int i = 0;
        private void addLine(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            //i++;
            //holder.Children.Add(new SyntaxLabel(i));
            //(holder.Children.Last() as SyntaxLabel).Focus(FocusState.Keyboard);
            App.Model.OpenNotebook.Lines.Add(new Line{ LineNumber =  App.Model.OpenNotebook.Lines.Count+1 , SheetID = notebook.ID});
        }

        private void ReorderChecked(object sender,RoutedEventArgs e)
        {
            App.Model.ReorderEnabled = (sender as AppBarToggleButton).IsChecked.Value ? ListViewReorderMode.Enabled : ListViewReorderMode.Disabled;
        }

        private void toSubstitutions(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SubstitutionsPage), App.Model);
        }

        private void toSettings(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        	Frame.Navigate(typeof(SettingsPage), App.Model);
        }


        SpeechRecognizer recoWithUI = new SpeechRecognizer();
        private  async void initSpeeach(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await recoWithUI.CompileConstraintsAsync();
            recoWithUI.UIOptions.AudiblePrompt = "What do you want to calculate?";
            recoWithUI.UIOptions.ExampleText = "salary equals 12 times 15";
            var result = await recoWithUI.RecognizeWithUIAsync();

            if (result.Text != "")
            {
                App.Model.OpenNotebook.Lines.Add(new Line { LineNumber = App.Model.OpenNotebook.Lines.Count + 1  , Expression = result.Text});
            }
        }
    }
}
