using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CalculatorApp
{
    public class Solver
    {
        public Dictionary<string, double> VariableTable = new Dictionary<string, double>();


        public double EvaluateAssignment(string expr)
        {
            //expr = expr.Replace(" ", "");
            string[] parts = expr.Split('=');
            VariableTable[parts[0].Trim()] = EvaluateNested(Sanitize(parts[1]));
            return VariableTable[parts[0].Trim()];
        }


        Dictionary<string, int> Precedence = new Dictionary<string, int>
        {
            {"+",0},{"-",0}
           ,{"*",1},{"/",1},
           {"^",1}
        };

        public List<string> Operators = new List<string>
        {
            "+" , "-" , "*" , "/" , "^"
        };

        public List<string> Tokenize(string expression)
        {
            List<String> tokens = new List<string>();
            expression = expression.Replace(" ", "");
            string current = "";

            for (int x = 0; x < expression.Length; x++)
            {
                
                if (
                    (   Operators.Contains(expression[x].ToString())
                        && x != 0
                        && !Operators.Contains(expression[x-1].ToString())
                    )
                     ||
                    expression[x] == '(' ||
                    expression[x] == ')')
                {
                    if(current != "") tokens.Add(current);

                    tokens.Add(expression[x].ToString());
                    current = "";
                }
                else
                {
                    current += expression[x];
                }
            }
            tokens.Add(current);
            return tokens;
        }


        public string ConvertSyntaxSugar(string expr)
        {
            expr.Replace(" ", "");
            List<string> tokens = Tokenize(expr);
            for (int x = 0; x < tokens.Count; x++)
            {
                if (tokens[x].Contains("%"))
                {
                    for (int y = x-1; y >= 0; y--)
                    {
                        if (!Operators.Contains(tokens[y]) && tokens[y] != "(" && tokens[y] != ")")
                        {
                            tokens[x] = tokens[y] + "*" + tokens[x].Replace("%", "") +"/100";
                            break;
                        }
                    }
                }
            }
            return tokens.Aggregate((a, b) => a + b);
        }


      

        public double Evaluate(string expression)
        {
            List<string> tokens = Tokenize(expression);

            int head = 0;
            while (tokens.Count > 1)
            {
                

                if ((Operators.Contains(tokens[head+1]) && Precedence[tokens[head+1]] == 1)
                    
                    || ((tokens.Count>3) && Precedence[tokens[head+3]] == 0)
                    || (tokens.Count == 3)
                    )
                {
                    double i1;
                    if (!double.TryParse(tokens[head], out i1))
                    {
                        bool negative = tokens[head].StartsWith("-");
                        string val = (negative)? tokens[head].Replace("-","") :tokens[head];
                        if (VariableTable.ContainsKey(val))
                        {
                            i1 = VariableTable[val];
                        }

                        if (negative) i1 *= -1;
                    }

                    double i2;
                    if (!double.TryParse(tokens[head+2], out i2))
                    {
                        bool negative = tokens[head+2].StartsWith("-");
                        string val = (negative)? tokens[head+2].Replace("-","") :tokens[head+2];
                        if (VariableTable.ContainsKey((val)))
                        {
                            i2 = VariableTable[val];
                        }
                        if (negative) i2 *= -1;
                    }

                    tokens[0] = EvaluateSign(tokens[head+1] , i1 , i2).ToString();
                    tokens.RemoveAt(head + 1);
                    tokens.RemoveAt(head + 1);
                }
                else
                {
                    double i1;
                    if (!double.TryParse(tokens[head + 2], out i1))
                    {
                        bool negative = tokens[head + 2].StartsWith("-");

                        string val = (negative) ? tokens[head + 2].Replace("-", "") : tokens[head + 2];
                        if (VariableTable.ContainsKey(val))
                        {
                            i1 = VariableTable[tokens[head + 2]];
                        }
                        if (negative) i1 *= -1;
                    }

                    double i2;
                    if (!double.TryParse(tokens[head + 4], out i2))
                    {
                        bool negative = tokens[head + 4].StartsWith("-");
                        string val = (negative) ? tokens[head + 4].Replace("-", "") : tokens[head + 4];
                        if (VariableTable.ContainsKey(val))
                        {
                            i2 = VariableTable[tokens[head + 4]];
                        }
                        if (negative) i2 *= -1;
                    }

                    tokens[2] = EvaluateSign(tokens[3], i1, i2).ToString();
                    tokens.RemoveAt(3);
                    tokens.RemoveAt(3);
                }
            }

            double ret;
            if (double.TryParse(tokens[0], out ret))
            {
                return ret;
            }
            else
            {
                return 0.0d;
            }
        }


        public double EvaluateSign(string op, double v1, double v2)
        {
            double v1d = v1;
            double v2d = v2;
            if (op == "+") return v1d + v2d;
            if (op == "-") return v1d - v2d;
            if (op == "/") return v1d / v2d;
            if (op == "*") return v1d * v2d;

            if (op == "^") return Math.Pow(v1,v2);
            return 0.0;
        }

        bool Degrees { get{
            return App.Model.Settings.Degrees; 
        } }


        //public static Regex CurrencyOperation = new Regex(@"\d+");
        public static Regex currencyConversionNotation = new Regex(@"(\d+|\d+\.\d+)\W?(USD|JPY|BGN|CZK|DKK|GBP|HUF|LTN|PLN|RON|SEK|CHF|NOK|HRK|RUB)\W?(to|into|as)\W?(USD|JPY|BGN|CZK|DKK|GBP|HUF|LTN|PLN|RON|SEK|CHF|NOK|HRK|RUB)" , RegexOptions.IgnoreCase);
        public static Regex kNotation = new Regex(@"(\d+|\d+.\d+)k");
        public static Regex FunctionNotation = new Regex(@"(sqrt|sin|cos|exp|tan|ln)(\d+|\d+.\d+)");

        public static Regex PercentageNotation = new Regex(@"(-|\+)\s?(\d+|\d+.\d+)\s?%");
        public double EvaluateNested(string expr)
        {
            expr = currencyConversionNotation.Replace(expr, (match) =>
            {
                string[] data = match.Value.Split(' ');
                double value = double.Parse(data[0]);
                string from = data[1].ToUpper();
                string to = data[3].ToUpper();

                /*if (!Core.data.ContainsKey(from))
                {
                    var alias = (Core.Aliases.Where((a, b) => a.Key.Contains(from)).FirstOrDefault()).Key;
                    if (alias != null) from = (string)alias;
                }

                if (!Core.data.ContainsKey(to))
                {
                    var alias = (Core.Aliases.Where((a, b) => a.Key.Contains(to)).FirstOrDefault()).Key;
                    if (alias != null) to = (string)alias;
                }*/


                double answer = Core.data[to]/Core.data[from] * value;
                return answer.ToString();
            });

            expr = PercentageNotation.Replace(expr, (match) =>
            {
                string v = match.Value.Replace(" ","");
                v = v.Replace("%", "");
                double i = double.Parse(v);
                i /= 100;
                if (i > 0)
                {
                    v = "* " + (i+1).ToString();
                }
                else
                {
                    v = v.Replace("-", ""); 
                    v = "* " + (1 - i).ToString();
                }
                return v;
            });

            expr = kNotation.Replace(expr , (match) =>
            {
                string v = match.Value;
                v = v.Replace("k", "");
                return (double.Parse(v) * 1000).ToString();
            });

            expr = FunctionNotation.Replace(expr, (match) =>
            {
                string v = match.Value;
                v = v.Replace(")", "");
                string[] parts = new string[2];
                int index = 0;
                foreach (char i in v)
                {
                    if (char.IsNumber(i))
                    {
                        parts[1] += i;
                    }
                    else
                    {
                        parts[0] += i;
                    }
                }
                double number = double.Parse(parts[1]);
                switch (parts[0])
                {
                    case "sin":
                        number = Math.Sin(number * (Degrees ? (Math.PI / 180) : 1));
                        break;
                    case "cos":
                        number = Math.Cos(number * (Degrees ? (Math.PI / 180) : 1));
                        break;
                    case "tan":
                        number = Math.Tan(number * (Degrees ? (Math.PI / 180) : 1));
                        break;
                    case "ln":
                        number = Math.Log(number);
                        break;
                    case "exp":
                        number = Math.Exp(number);
                        break;
                    case "sqrt":
                        number = Math.Sqrt(number);
                        break;
                }
                return number.ToString();
            });
            
            

            if (expr.Contains("="))
            {
                return EvaluateAssignment(expr);
            }
            
            //if (expr.Contains("%")) expr = ConvertSyntaxSugar(expr);

            expr = Sanitize(expr);
            int start = 0, end = 0;
            if (!expr.Contains("(") && !expr.Contains(")")) return Evaluate(expr);
            List<string> tokens = Tokenize(expr);

            for (int x = 0; x < tokens.Count; x++)
            {
                if (tokens[x] == "(") start = x;
                else if (tokens[x] == ")")
                {
                    end = x;
                    break;
                }
            }

            string nestedExpr = "";
            for(int x = start +1;x<=end-1;x++){
                nestedExpr += tokens[x];
            }
            double nestedResult = Evaluate(nestedExpr);
            tokens.RemoveRange(start, end - start +1);
            tokens.Insert(start, nestedResult.ToString());



            return EvaluateNested(tokens.Aggregate((a, b) => a + b)); ;
        }



        public string Sanitize(string expr)
        {
            expr = expr.Replace("÷", "/");
            expr = expr.Replace("x", "*");

            foreach (string s in Operators)
            {
                expr = expr.Replace(s, " " + s + " ");
            }

            expr = expr.Replace("(", " ( ");
            expr = expr.Replace(")", " ) ");

            List<string> tokens = expr.Split(' ').ToList();
            int removed = 0;
            int count = tokens.Count;
            for (int x = 0; x < count; x++)
            {
                string value = tokens[x - removed].Replace(" ", "");
                double val;
                if (!double.TryParse(value , out val)
                    && !VariableTable.ContainsKey(value)
                    && !Operators.Contains(value)
                    && !value.Contains("(") 
                    && !value.Contains(")")
                    && !value.Contains("%"))
                {
                    tokens.RemoveAt(x - removed);
                    removed++;
                }
            }
            if (tokens.Count == 0) return "";
            return tokens.Aggregate((a, b) => a + b);
        }
    }
}
