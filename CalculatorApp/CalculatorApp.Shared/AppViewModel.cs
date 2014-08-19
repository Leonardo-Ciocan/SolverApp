using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using System.Runtime.CompilerServices;

namespace CalculatorApp
{
    public class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Sheet> Notebooks = new ObservableCollection<Sheet>();
        public Sheet OpenNotebook { get; set; }

#if WINDOWS_PHONE_APP

        public ListViewReorderMode _reorderEnabled = ListViewReorderMode.Disabled;
        public ListViewReorderMode ReorderEnabled{
            get{return _reorderEnabled;}
            set { _reorderEnabled = value; OnPropertyChanged(); }
        }
#endif


        public AppSettings Settings { get; set; }


        public AppViewModel()
        {
            Settings = new AppSettings();
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
}
