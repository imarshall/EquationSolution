using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> variables = new List<string>() { "y", "x" };
            Equation eq = new Equation("x - y - 2x*(x + 2)(2 + x)(2) = x*5.4 + 1", variables);
            eq.Evaluate();
            Console.WriteLine("x - y - 2x*(x + 2)(2 + x)(2) = x*5.4 + 1");
            Console.WriteLine(eq.ToString());
            Console.ReadKey();

            string eq_str = "x + 1-(ab + b + c)(ab + b + c)(ab + b + c) = 1";
            Equation eq1 = new Equation(eq_str, new List<string>() { "ab", "b", "c" });
            eq1.Evaluate();
            Console.WriteLine(eq_str);
            Console.WriteLine(eq1.ToString());
            Console.ReadKey();
        }
    }
}
