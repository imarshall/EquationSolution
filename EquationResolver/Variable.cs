using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace EquationResolver
{
    class Variable
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
    }
}
