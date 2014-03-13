using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationResolver
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> variables = new List<string>() { "modx", "x" };
            var test = Member.FromStringExpression("1.53xmodx^2", variables);
        }
    }
}
