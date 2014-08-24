using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml.Media;
using Newtonsoft.Json;
using Windows.UI;

namespace CalculatorApp
{
    public class Sheet : INotifyPropertyChanged
    {

        public Sheet()
        {
            Lines = new ObservableCollection<Line>();
            Substitutions = new ObservableCollection<Substitution>
        {
            new Substitution("pi","π"),
            new Substitution("lambda" , "λ"),
            new Substitution("/" , "÷"),
            new Substitution("*" , "x"),
            new Substitution("delta","Δ"),
            new Substitution("times" , "*"),
            new Substitution("divided" , "/"),
            new Substitution("plus" , "+"),
            new Substitution("minus" , "-"),
            new Substitution("equals" , "="),
            new Substitution("percent" , "%")
        };

            variableColors = new Dictionary<string, Color>();
        }

        public string _name = "";
        public string Name { get { return _name; } set { _name = value; OnPropertyChanged(); } }

        public string _id = Guid.NewGuid().ToString();
        public string ID { get { return _id; } set{_id = value;} }

        [Newtonsoft.Json.JsonIgnore]
        public Solver Solver = new Solver
        {

        };


        public Dictionary<string, Color> variableColors { get; set; }



        public ObservableCollection<Line> Lines
        {
            get;
            set;
        }


       /* [Newtonsoft.Json.JsonIgnore]
        private ObservableCollection<Substitution> _subs = new ObservableCollection<Substitution>
        {
            new Substitution("pi","π"),
            new Substitution("lambda" , "λ"),
            new Substitution("/" , "÷"),
            new Substitution("*" , "x"),
            new Substitution("delta","Δ"),
            new Substitution("times" , "*"),
            new Substitution("divided" , "/"),
            new Substitution("plus" , "+"),
            new Substitution("minus" , "-"),
            new Substitution("equals" , "="),
            new Substitution("percent" , "%")
        };*/

        public ObservableCollection<Substitution> Substitutions
        {
            get;
            set;
        }

        public void EvaluateAll()
        {
            Solver.VariableTable.Clear();
            foreach (Line l in Lines)
            {
                l.ShouldUpdate = !l.ShouldUpdate;
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

        public string SheetID { get; set; }
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
