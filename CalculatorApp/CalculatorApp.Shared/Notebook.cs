using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;

namespace CalculatorApp
{
    public class Notebook
    {
        public Solver Solver = new Solver();

        public Dictionary<string, Brush> variableColors = new Dictionary<string, Brush>();

        public ObservableCollection<Line> _lines = new ObservableCollection<Line>();
        public ObservableCollection<Line> Lines
        {
            get { return _lines; }
        }

        public ObservableCollection<Substitution> _subs = new ObservableCollection<Substitution>
        {
            new Substitution("pi","π"),
            new Substitution("lambda" , "λ"),
            new Substitution("/" , "÷"),
            new Substitution("*" , "x"),
            new Substitution("delta","Δ")/*,
            new Substitution("(" , " ( "),
            new Substitution(")" , " ) ")*/
        };

        public ObservableCollection<Substitution> Substitutions
        {
            get { return _subs; }
        }

        public void EvaluateAll()
        {
            Solver.VariableTable.Clear();
            foreach (Line l in Lines)
            {
                l.ShouldUpdate = !l.ShouldUpdate;
            }
        }
    }

    public class Line : INotifyPropertyChanged
    {
        public string _expression = "";
        public string Expression { get { return _expression; } set { _expression = value; OnPropertyChanged(); } }

        public bool _shouldUpdate = false;
        public bool ShouldUpdate { get { return _shouldUpdate; } set { _shouldUpdate = value; OnPropertyChanged(); } }

        public int _lineNumber = 1;
        public int LineNumber { get { return _lineNumber; } set { _lineNumber = value; OnPropertyChanged(); } }


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

    public class Substitution : INotifyPropertyChanged
    {
        string _key ="";
        public string Key { get { return _key; } set { _key = value; OnPropertyChanged(); } }

        string _value = "";
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged(); } }


        public Substitution(string a, string b) { Key = a; Value = b; }

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
