using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace CalculatorApp
{
    public sealed partial class SyntaxLabel : UserControl
    {

        public bool IsStaticLabel { get; set; }
        /*string _text ="";
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                hiddenText.Text = _text;
                DisplayText(Text);
            }
        }*/



        Line line;
        Sheet notebook;
        FontFamily consolas = new FontFamily("ms-appx:///Assets/consolas.ttf#Consolas");

        int i = 0;
        public SyntaxLabel(int i)
        {
            this.i = i;
            this.InitializeComponent();
            Loaded += SyntaxLabel_Loaded;
        }

        public SyntaxLabel()
        {
            DataContextChanged += (AlignmentX, k) =>
            {
                //redm
                


                //DisplayText(line.Expression);
            }; 

            this.InitializeComponent();
            Loaded += SyntaxLabel_Loaded;
        }

        void SyntaxLabel_Loaded(object sender, RoutedEventArgs e)
        {
            if(!IsStaticLabel) Focus(FocusState.Keyboard);


            line = (DataContext as Line);
            if (line == null) return;
            if (IsStaticLabel)
            {
                notebook = Core.Sheets.Where((x, b) => x.ID == line.SheetID).FirstOrDefault<Sheet>();
            }


            line.PropertyChanged += (a, b) =>
            {
                if (b.PropertyName == "ShouldUpdate")
                {
                    DisplayText(line.Expression);
                }
                else
                {
                    notebook.EvaluateAll();
                    //DisplayText(line.Expression);
                }
            };

            notebook = App.Model.OpenNotebook;
            
           // num.Text = i.ToString();

            txt.FontFamily = consolas;
            txt.FontSize = 20;

            num.FontFamily = consolas;
            num.FontSize = 20;

            hiddenText.FontFamily = consolas;
            hiddenText.FontSize = 20;

            resultText.FontFamily = consolas;
            resultText.FontSize = 20;

            hiddenText.TextChanged += (a, b) =>
            {
                if (IsStaticLabel) return;
                int s  = hiddenText.SelectionStart, l = hiddenText.SelectionLength;
                foreach (Substitution pair in notebook.Substitutions)
                {
                    if (hiddenText.Text.Contains(" " + pair.Key) || hiddenText.Text.Contains(pair.Key + " ") || hiddenText.Text == pair.Key)
                    {
                        hiddenText.Text = hiddenText.Text.Replace(" " + pair.Key, " " + pair.Value);
                        hiddenText.Text = hiddenText.Text.Replace( pair.Key + " " ,  pair.Value+ " " );
                    }
                }
                hiddenText.SelectionStart = s;
                hiddenText.SelectionLength = l;
            };

            

            var over = new SolidColorBrush(Color.FromArgb(20,0,0,0));
            if(!IsStaticLabel) root.Background = over;
            hiddenText.GotFocus += (a, b) =>
            {
                root.Background = over;
            };
            hiddenText.LostFocus += async (a, b) =>
            {
                root.Background = null;
                Core.Save(notebook);
            };

            if (!IsStaticLabel)
            {
                int s1 = hiddenText.SelectionStart, l1 = hiddenText.SelectionLength;
                foreach (Substitution pair in notebook.Substitutions)
                {
                    if (hiddenText.Text.Contains(" " + pair.Key) || hiddenText.Text.Contains(pair.Key + " ") || hiddenText.Text == pair.Key)
                    {
                        hiddenText.Text = hiddenText.Text.Replace(" " + pair.Key, " " + pair.Value);
                        hiddenText.Text = hiddenText.Text.Replace(pair.Key + " ", pair.Value + " ");
                    }
                }
                hiddenText.SelectionStart = s1;
                hiddenText.SelectionLength = l1;
            }

            DisplayText(line.Expression);

            /*this.Tapped += (Action, b) =>
            {
                hiddenText.Visibility = Visibility.Visible;
                hiddenText.Focus(FocusState.Keyboard);
            };

            hiddenText.LostFocus += (a, b) =>
            {
                _text = hiddenText.Text;
                DisplayText(hiddenText.Text);
                hiddenText.Visibility = Visibility.Visible;
            };

            hiddenText.KeyDown += (a, b) =>
            {
                if (b.Key == Windows.System.VirtualKey.Enter)
                {
                    hiddenText.IsEnabled = false;
                    txt.Focus(FocusState.Keyboard);
                    hiddenText.IsEnabled = true;
                    hiddenText.Visibility = Visibility.Collapsed;
                }
            };*/
        }



        Brush NumberBrush = new SolidColorBrush(Color.FromArgb(255, 0, 86, 207));
        Brush NormalBrush = new SolidColorBrush(Colors.Black);
        Brush OperatorBrush = new SolidColorBrush(Color.FromArgb(255, 0, 181, 6));
        Brush VariableBrush = new SolidColorBrush(Colors.DodgerBlue);

        Random r = new Random();
        public void DisplayText(string expr)
        {
            NumberBrush = AppSettings.Themes[App.Model.Settings.ThemeIndex].NumberText;
            NormalBrush = AppSettings.Themes[App.Model.Settings.ThemeIndex].RegularText;
            OperatorBrush = AppSettings.Themes[App.Model.Settings.ThemeIndex].OperatorText;

            if (notebook == null) notebook = new Sheet();
            double answer = notebook.Solver.EvaluateNested(expr);
            answer = RoundToSignificantDigits(answer, App.Model.Settings.SignificantFigures);
            resultText.Text = answer.ToString();

            hiddenText.FontSize = App.Model.Settings.FontSize;
            num.FontSize = App.Model.Settings.FontSize;
            resultText.FontSize = App.Model.Settings.FontSize;
            //expr = Core.Solver.Sanitize(expr);
            //expr = Core.Solver.Tokenize(
            var ls = expr.Split(' ');
            //Paragraph p = new Paragraph();
            //txt.Blocks.Add(p);
            txt.Inlines.Clear();
            for(int x =0; x < ls.Count();x++){
                string s = ls[x];
                double d;
                bool isNum = double.TryParse(s, out d) ||  (s.Contains("%") && double.TryParse(s.Replace("%",""),out d)) || Solver.kNotation.IsMatch(s);
                bool var = notebook.Solver.VariableTable.ContainsKey(s);
                bool op = s == "+" || s == "-" || s == "÷" || s == "x";//notebook.Solver.Operators.Contains(s) || notebook.Substitutions.Where((key,val) => notebook.Solver.Operators.Contains(key.Key)).Count() > 0;

                Run item = new Run
                {
                    FontSize = App.Model.Settings.FontSize,
                    FontFamily = consolas,
                    Text = s + " ",
                    Foreground = NormalBrush
                };

                /*if (x != 0)
                {
                    if (ls[x - 1] == "^")
                    {

                    }
                }*/

                
                if (isNum) item.Foreground = NumberBrush;
                if (op) item.Foreground = OperatorBrush;
                if (var)
                {
                    if(!notebook.variableColors.ContainsKey(s)){

                        notebook.variableColors[s] = Color.FromArgb(200, (byte)r.Next(0, 255), (byte)r.Next(0, 255), (byte)r.Next(0, 255));
                    }
                    item.Foreground =new SolidColorBrush( notebook.variableColors[s]);
                } 
                txt.Inlines.Add(item);
            }
        }

        bool isPrice(string expr)
        {
            if (expr.Contains("$"))
            {
                return true;
            }
            return false;
        }
        static double RoundToSignificantDigits(double d, int digits)
        {
            if (d == 0)
                return 0;

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(d))) + 1);
            return scale * Math.Round(d / scale, digits);
        }
    }
}
