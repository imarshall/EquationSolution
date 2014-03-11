using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationResolver
{
    public enum TokenType { NONE, MEMBER, NUMERIC, MATH_OPERATION };

    class ExpressionToken
    {
        public string Text { get; set; }
        public TokenType Type { get; set; }
        public MathOperation Operation { get; set; }

        public ExpressionToken() { }

        public ExpressionToken(string _text, TokenType _type, MathOperation _operation = MathOperation.NONE)
        {
            Text = _text;
            Type = _type;
            Operation = _operation;
        }
    }
}
