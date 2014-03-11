using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationResolver
{
    public enum MathOperation { NONE, PLUS, MINUS, MULTIPLY, DIV, POW }

    class Member
    {
        public double Factor { get; set; }
        public List<Variable> Variables { get; set; }
        public MathOperation Operation { get; set; }

        public Member()
        {
            Variables = new List<Variable>();
            Operation = MathOperation.PLUS;
        }
    }
}
