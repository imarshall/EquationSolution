using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationResolver
{
    class Variable
    {
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

        public static List<Variable> FromStringExpression(string input_str)
        {
            throw new NotImplementedException();
        }
    }
}
