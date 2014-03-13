using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EquationSolution
{
    class Equation
    {
        private List<string> _variable_aliases;
        private List<ExpressionToken> _parsed_tokens;
        private const string split_pattern = "+|-|/|*|[(]|[)]|=";
        private const string split_pattern_inc = "(+|-|/|*|[(]|[)]|=)";

        public string Expression { get; set; }
        public List<Member> Members { get; set; }

        public Equation(List<string> variables)
        {
            Members = new List<Member>();
            _variable_aliases = variables;
        }

        public Equation(string _expr, List<string> variables)
        {
            Members = new List<Member>();
            Expression = _expr;
            _variable_aliases = variables;
        }

        public void GetTokens()
        {
            if (string.IsNullOrEmpty(Expression))
                throw new ArgumentNullException("Expression", "Class parameter Expression cannot be null or empty");

            var tokens = Regex.Split(Expression, split_pattern_inc);
            _parsed_tokens = new List<ExpressionToken>();
            for (int i = 0; i < tokens.Length; i++)
            {
                //compiler smarter than me, it will optimize this
                var token = tokens[i];

                //if (token == "(")
                //{
                //    OpenBracket(i, tokens, ref ParsedTokens);
                //}

                ExpressionToken expr_token = ExpressionToken.FromStringExpression(token);
                _parsed_tokens.Add(expr_token);
                _parsed_tokens.Add(new ExpressionToken(token, TokenType.NONE));
            }
            //we've got preparsed tokens

            var obrackets = _parsed_tokens.Where(x => x.Type == TokenType.O_BRACKET).ToList();
            var cbrackets = _parsed_tokens.Where(x => x.Type == TokenType.C_BRACKET).ToList();
            if (obrackets.Count != cbrackets.Count)
                throw new ArgumentException("Equation is incorrect. Some brackets is not closed/opened", "Expression");
        }

        /// <summary>
        /// Perform required operations to members inside brakets: 
        /// change sign,
        /// multiply on numeric constant,
        /// multiply on expression with members
        /// </summary>
        /// <param name="current_index">index of "(" token</param>
        /// <param name="ParsedTokens">recently parsed tokens of the expression</param>
        /// <param name="tokens">entiry array of expression's tokens</param>
        /// <returns>index on next element after closing bracket in tokens</returns>
        public int OpenBracket(List<ExpressionToken> ParsedTokens)
        {
            var prev_token = ParsedTokens.LastOrDefault();
            if(prev_token == null)
                prev_token = new ExpressionToken("+", TokenType.MATH_OPERATION, MathOperation.PLUS);
            if(prev_token.Type == TokenType.MATH_OPERATION)
            {
                switch(prev_token.Operation)
                {
                    case MathOperation.PLUS:
                        break;
                    case MathOperation.MINUS:
                        break;
                    case MathOperation.DIV:
                        break;
                    case MathOperation.MULTIPLY:
                        break;
                }
            }
            else
            {
                //if it is not operation then it is member
                var token = tokens[current_index];
                if(prev_token.Type == TokenType.MEMBER || prev_token.Type == TokenType.NUMERIC)
                {
                    var member = Members.LastOrDefault();
                }
            }
            throw new NotImplementedException();
        }


    }
}
