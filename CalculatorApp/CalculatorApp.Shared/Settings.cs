using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Windows.Storage;
using Windows.UI;

namespace CalculatorApp 
{
    public class AppSettings :INotifyPropertyChanged
    {

      public static List<Theme> Themes = new List<Theme>
        {
            new Theme{
                    Background = new SolidColorBrush(Color.FromArgb(255, 43, 43, 43)),
                    NumberText   = new SolidColorBrush(Color.FromArgb(255, 0, 86, 207)),
                    RegularText = new SolidColorBrush(Colors.White),
                    OperatorText = new SolidColorBrush(Color.FromArgb(255,0,181,6))
              },
            new Theme{
                    Background = new SolidColorBrush(Color.FromArgb(255, 238, 232, 213)),
                    NumberText   = new SolidColorBrush(Color.FromArgb(255, 203, 75, 22)),
                    RegularText = new SolidColorBrush(Colors.DarkGray),
                    OperatorText = new SolidColorBrush(Color.FromArgb(255,153,0,68))
              },
            
              new Theme{
                    Background = new SolidColorBrush(Colors.Black),
                    NumberText   = new SolidColorBrush(Color.FromArgb(255, 73, 186, 0)),
                    RegularText = new SolidColorBrush(Colors.White),
                    OperatorText = new SolidColorBrush(Color.FromArgb(255,95,218,242))
              },
              new Theme{
                    Background = new SolidColorBrush(Colors.White),
                    NumberText   = new SolidColorBrush(Color.FromArgb(255, 0, 86, 207)),
                    RegularText = new SolidColorBrush(Colors.Black),
                    OperatorText = new SolidColorBrush(Color.FromArgb(255,0,181,6))
              }
        };

        public int SignificantFigures
        {
            get
            {
                object k;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("SignificantFigures", out k))
                {
                    ApplicationData.Current.RoamingSettings.Values["SignificantFigures"] = 3;
                    return 3;
                }
                else
                {
                    return (int)k;
                }
            }
            set
            {
                
                ApplicationData.Current.RoamingSettings.Values["SignificantFigures"] = value;
                OnPropertyChanged();
            }
        }

        public double FontSize
        {
            get
            {
                object k;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("FontSize", out k))
                {
                    ApplicationData.Current.RoamingSettings.Values["FontSize"] = 20.0;
                    return 20.0;
                }
                else
                {
                    return (double)k;
                }
            }
            set
            {
                ApplicationData.Current.RoamingSettings.Values["FontSize"] = value;
                OnPropertyChanged();
            }
        }

        public int ThemeIndex
        {
            get
            {
                object k;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("ThemeIndex", out k))
                {
                    ApplicationData.Current.RoamingSettings.Values["ThemeIndex"] = 0;
                    return 0;
                }
                else
                {
                    return (int)k;
                }
            }
            set
            {

                ApplicationData.Current.RoamingSettings.Values["ThemeIndex"] = value;
                OnPropertyChanged();
            }
        }

        public bool Degrees
        {
            get
            {
                object k;
                if (!ApplicationData.Current.RoamingSettings.Values.TryGetValue("Degrees", out k))
                {
                    ApplicationData.Current.RoamingSettings.Values["Degrees"] = true;
                    return true;
                }
                else
                {
                    return (bool)k;
                }
            }
            set
            {
                ApplicationData.Current.RoamingSettings.Values["Degrees"] = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string caller = null)
        {
            // make sure only to call this if the value actually changes

            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }
    }

    public class Theme
    {
        public Brush Background { get; set; }
        public Brush RegularText { get; set; }
        public Brush OperatorText { get; set; }
        public Brush NumberText { get; set; }
        public Brush PercentageText { get; set; }
    }
}
