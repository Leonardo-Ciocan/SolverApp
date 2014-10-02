using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.UI;
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

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            Loaded += MainPage_Loaded;
            this.Tapped += (a, b) => changes = true;
        }
        bool changes;
        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            btnPin.IsChecked = SecondaryTile.Exists(notebook.ID.Substring(0, 10));
            tile.DataContext = notebook;
            tileSmall.DataContext = notebook;
            DispatcherTimer dt = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
            dt.Tick += (a, b) =>
            {

                if (changes)
                {
                        //Core.sa
                       if(SecondaryTile.Exists(notebook.ID.Substring(0,10)))
                           TileManager.SaveAndPin(tile, tileSmall, notebook.ID);
                }
                changes = false;
            };
            dt.Start();
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

                double ans = App.Model.OpenNotebook.Solver.EvaluateNested(result.Text);
                var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();

                // Generate the audio stream from plain text.
                SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync("The answer is " + ans.ToString());

                // Send the stream to the media object.
                mediaElement.SetSource(stream, stream.ContentType);
                mediaElement.Play();
            }
        }

        private async void review(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=1e5827bc-9152-4726-9b40-cf3303ec272d"));
            //Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=27721LeonardoCiocan.Karma_55ag5hpdedhr2"));
            //var mailto = new Uri("mailto:?to=leonardo.ciocan@outlook.com&subject=Karma feedback");
            //await Windows.System.Launcher.LaunchUriAsync(mailto);
        }

        Random r = new Random();
        private void randomize(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (string s in App.Model.OpenNotebook.variableColors.Keys.ToArray())
            {
                App.Model.OpenNotebook.variableColors[s] = Color.FromArgb(200, (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
            }
            App.Model.OpenNotebook.EvaluateAll();
        }

        private async void pin(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (!btnPin.IsChecked.Value)
            {
                await (await SecondaryTile.FindAllAsync()).Where(x => x.TileId == notebook.ID.Substring(0, 10)).FirstOrDefault().RequestDeleteAsync();
            }
            else
            {
                TileManager.SaveAndPin(tile, tileSmall, notebook.ID);
            }
        }
    }
}
