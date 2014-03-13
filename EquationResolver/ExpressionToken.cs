using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace EquationSolution
{
    public enum TokenType { NONE, MEMBER, NUMERIC, MATH_OPERATION, O_BRACKET, C_BRACKET };

    class ExpressionToken
    {
        public string Text { get; set; }
        public TokenType Type { get; set; }
        public MathOperation Operation { get; set; }
        private static Dictionary<string, ExpressionToken> syntax = new Dictionary<string, ExpressionToken>()
        {
            {"+", new ExpressionToken("+", TokenType.MATH_OPERATION, MathOperation.PLUS)},
            {"-", new ExpressionToken("-", TokenType.MATH_OPERATION, MathOperation.MINUS)},
            {"/", new ExpressionToken("/", TokenType.MATH_OPERATION, MathOperation.DIV)},
            {"=", new ExpressionToken("=", TokenType.MATH_OPERATION, MathOperation.EQUAL)},
            {"*", new ExpressionToken("*", TokenType.MATH_OPERATION, MathOperation.MULTIPLY)},
            {"(", new ExpressionToken("(", TokenType.O_BRACKET, MathOperation.NONE)},
            {")", new ExpressionToken(")", TokenType.C_BRACKET, MathOperation.NONE)}
        };

        private ExpressionToken() {}

        public ExpressionToken(string _text, TokenType _type, MathOperation _operation = MathOperation.NONE)
        {
            Text = _text;
            Type = _type;
            Operation = _operation;
        }

        public static ExpressionToken FromStringExpression(string input_str)
        {
            ExpressionToken expr_token = syntax.ContainsKey(input_str) ? syntax[input_str] : null;
            if (expr_token == null)
            {
                expr_token = new ExpressionToken();
                expr_token.Text = input_str;
                double tmp;
                if (double.TryParse(input_str, NumberStyles.Any, CultureInfo.InvariantCulture, out tmp))
                    expr_token.Type = TokenType.NUMERIC;
                else
                    expr_token.Type = TokenType.MEMBER;
            }
            return expr_token;
        }
    }
}
