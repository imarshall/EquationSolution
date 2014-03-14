using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace EquationSolution
{
    class Variable : ICloneable
    {
        private const string variable_pattern = @"((?:[1-9]\d*|0)?(?:\.\d+)?)({0}(\^[0-9])?)";
        public string Alias { get; set; }
        public double Factor { get; set; }
        public double Power { get; set; }

        public Variable() { }
        public Variable(string _alias, double _factor, double _power)
        {
            Alias = _alias;
            Factor = _factor;
            Power = _power;
        }

        public static Variable operator *(Variable var1, Variable var2)
        {
            if (var1.Alias != var2.Alias)
                throw new InvalidCastException(string.Format("Cannot cast an \"{0}\" into \"{1}\"", var2.Alias, var1.Alias));
            Variable result = new Variable(var1.Alias, 1.0, 1.0);
            if (var1.Factor == 0 || var2.Factor == 0)
            {
                result.Factor = 0;
            }
            else
            {
                result.Factor = var1.Factor * var2.Factor;
                result.Power = var1.Power + var2.Power;
            }
            return result;
        }

        public override string ToString()
        {
            string _factor = (this.Factor == 1) ? "" : this.Factor.ToString();
            string power = (this.Power == 1) ? "" : "^" + this.Power.ToString();
            string alias = this.Alias;
            if (this.Factor == 0)
            {
                power = "";
                alias = "";
            }
            return string.Format("{0}{1}{2}", _factor, alias, power);
        }

        public static Variable operator /(Variable var1, Variable var2)
        {
            if (var1.Alias != var2.Alias)
                throw new InvalidCastException(string.Format("Cannot cast an \"{0}\" into \"{1}\"", var2.Alias, var1.Alias));
            Variable result = new Variable(var1.Alias, 1.0, 1.0);
            if (var2.Factor == 0)
                throw new InvalidOperationException("Cannot divide by zero");
            if (var1.Factor == 0)
            {
                result.Factor = 0;
            }
            else
            {
                result.Factor = var1.Factor / var2.Factor;
                result.Power = var1.Power - var2.Power;
            }
            return result;
        }

        public static List<Variable> FromStringExpression(string input_str, List<string> variable_aliases)
        {
            List<Variable> vars = new List<Variable>();
            variable_aliases = variable_aliases.OrderByDescending(x => x.Length).ToList();
            var tmp = input_str;
            foreach(var item in variable_aliases)
            {
                var pattern = string.Format(variable_pattern, item);
                var matches = Regex.Matches(tmp, pattern);
                
                foreach (Match match in matches)
                {
                    //to prevent double variables, let's remove what we've found
                    tmp = tmp.Replace(match.Groups[0].Value, "");
                    var factor = 1.0;
                    if(!string.IsNullOrEmpty(match.Groups[1].Value))
                        factor = Double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
                    var power_val_str = new string(match.Groups[3].Value.Where(x => x != '^').ToArray());
                    var pow = 1.0;
                    if(!string.IsNullOrEmpty(power_val_str))
                        pow = Double.Parse(power_val_str);
                    vars.Add(new Variable(item, factor, pow));
                }
            }
            return vars;
        }

        object ICloneable.Clone()
        {
            Variable _copy = new Variable();
            _copy.Alias = (string)this.Alias.Clone();
            _copy.Factor = this.Factor;
            _copy.Power = this.Power;
            return _copy;
        }

        public Variable Clone()
        {
            return (Variable)(((ICloneable)this).Clone());
        }
    }
}
