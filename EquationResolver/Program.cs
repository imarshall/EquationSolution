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
            /*var test = ExpressionToken.FromStringExpression("(");
            Member first = Member.FromStringExpression("x", variables);
            Member second = Member.FromStringExpression("2x^1", variables);
            Member third = Member.FromStringExpression("y", variables);
            var res = first * second / third;
            Console.WriteLine(first.ToString() + " * " + second.ToString() + " / " + third + " = " + res.ToString());
            Console.ReadKey();*/

            Equation eq = new Equation("x - y + 2x*(x + 2) = x*2.4", variables);
            eq.Evaluate();
            Console.WriteLine("x - y + 2x*(x + 2) = x*2.4");
            Console.WriteLine(eq.ToString());
            Console.ReadKey();
        }
    }
}
