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
    public sealed partial class SheetListPage : Page
    {
        public SheetListPage()
        {
            this.InitializeComponent();
            Loaded += SheetListPage_Loaded;
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Required;
            Core.LoadAll();
        }

        async void SheetListPage_Loaded(object sender, RoutedEventArgs e)
        {
            //var u = UnitsNet.UnitSystem.Parse<UnitsNet.Speed>("m/s^2");

            //Core.DownloadCurrencies();
            list.Footer = new Grid { Height = this.BottomAppBar.ActualHeight *1.2 };
            Core.GetCurrencies();
            list.ItemsSource = Core.Sheets;
            list.IsItemClickEnabled = true;
            list.ItemClick += (a, b) =>
            {
                Frame.Navigate(typeof(MainPage), b.ClickedItem);
            };
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await StatusBar.GetForCurrentView().HideAsync();
            if (e.NavigationMode == NavigationMode.New)
            {
                var storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Voice.xml"));
                await Windows.Media.SpeechRecognition.VoiceCommandManager.InstallCommandSetsFromStorageFileAsync(storageFile);
            }

            
        }
        private void add(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Core.Sheets.Add(new Sheet { });
        }

        private void gototutorial(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Tutorial), "inapp");
        }

        private void int_s(int k = 0)
        {

        }
    }
}
