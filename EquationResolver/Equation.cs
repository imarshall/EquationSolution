using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EquationResolver
{
    class Equation
    {
        private const string split_pattern = "+|-|/|*|[(]|[)]";
        private const string split_pattern_inc = "(+|-|/|*|[(]|[)])";
        public string Expression { get; set; }
        public List<Member> Members { get; set; }

        public Equation()
        {
            Members = new List<Member>();
        }

        public Equation(string _expr)
        {
            Members = new List<Member>();
            Expression = _expr;
        }

        private string 

        public void Parse()
        {
            if (string.IsNullOrEmpty(Expression))
                throw new ArgumentNullException("Expression", "Class parameter Expression cannot be null or empty");

            var tokens = Regex.Split(Expression, split_pattern_inc);
            List<ExpressionToken> ParsedTokens = new List<ExpressionToken>();
            for (int i = 0; i < tokens.Length; i++)
            {
                //compiler smarter than me, it will optimize this
                var token = tokens[i];

                if (token == "(")
                {
                    OpenBracket(i, tokens, ref ParsedTokens);
                }

                ParsedTokens.Add(new ExpressionToken(token, TokenType.NONE);
            }
        }

        /// <summary>
        /// Does required operations to members inside brakets: 
        /// change sign,
        /// multiply on numeric constant,
        /// multiply on expression with members
        /// </summary>
        /// <param name="current_index">index of "(" token</param>
        /// <param name="ParsedTokens">recently parsed tokens of the expression</param>
        /// <param name="tokens">entiry array of expression's tokens</param>
        /// <returns>index on next element after closing bracket in tokens</returns>
        public int OpenBracket(int current_index, string[] tokens, ref List<ExpressionToken> ParsedTokens)
        {
            var prev_token = ParsedTokens.LastOrDefault();
            
            throw new NotImplementedException();
        }
    }
}
