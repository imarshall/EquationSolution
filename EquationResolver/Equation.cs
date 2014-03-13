﻿using System;
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
        private const string split_pattern = "[+]|[-]|[/]|[*]|[(]|[)]|[=]";
        private const string split_pattern_inc = "([+]|[-]|[/]|[*]|[(]|[)]|[=])";

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

        private void GetTokens()
        {
            if (string.IsNullOrEmpty(Expression))
                throw new ArgumentNullException("Expression", "Class parameter Expression cannot be null or empty");

            var tokens = Regex.Split(Expression, split_pattern_inc);
            _parsed_tokens = new List<ExpressionToken>();
            for (int i = 0; i < tokens.Length; i++)
            {
                //compiler smarter than me, it will optimize this
                var token = tokens[i];

                ExpressionToken expr_token = ExpressionToken.FromStringExpression(token);
                _parsed_tokens.Add(expr_token);
                //_parsed_tokens.Add(new ExpressionToken(token, TokenType.NONE));
            }
            //we've got preparsed tokens

            var obrackets = _parsed_tokens.Where(x => x.Type == TokenType.O_BRACKET).ToList();
            var cbrackets = _parsed_tokens.Where(x => x.Type == TokenType.C_BRACKET).ToList();
            if (obrackets.Count != cbrackets.Count)
                throw new ArgumentException("Equation is incorrect. Some brackets is not closed/opened", "Expression");
        }

        private Member GetMember(ExpressionToken token, int index)
        {
            Member cur_member = null;
            switch (token.Type)
            {
                case TokenType.MATH_OPERATION:
                    if(token.Operation == MathOperation.EQUAL)
                    {

                    }
                    break;
                case TokenType.MEMBER:
                    cur_member = Member.FromStringExpression(token.Text, _variable_aliases);
                    if (index - 1 > 0)
                        if (_parsed_tokens[index - 1].Type == TokenType.MATH_OPERATION)
                            cur_member.Operation = _parsed_tokens[index - 1].Operation;
                    break;
                case TokenType.NUMERIC:
                    cur_member = Member.FromStringExpression(token.Text, _variable_aliases);
                    break;
                case TokenType.C_BRACKET:
                    index = OpenBracket(index);
                    break;
            }
            return cur_member;
        }

        public void Evaluate()
        {
            GetTokens();
            for(int i=0;i<_parsed_tokens.Count;i++)
            {
                var token = _parsed_tokens[i];
                var cur_member = GetMember(token, i);
                if (cur_member == null && (token.Type == TokenType.MEMBER || token.Type == TokenType.NUMERIC))
                    throw new InvalidCastException(string.Format("Incorrect member \"{0}\" in equation", token.Text));
                Members.Add(cur_member);
            }
        }

        /// <summary>
        /// Perform required operations to members inside brakets: 
        /// change sign,
        /// multiply on numeric constant,
        /// multiply on members
        /// </summary>
        /// <param name="current_index">index of "(" token</param>
        /// <param name="ParsedTokens">recently parsed tokens of the expression</param>
        /// <param name="tokens">entiry array of expression's tokens</param>
        /// <returns>index on next element after closing bracket in tokens</returns>
        public int OpenBracket(int current_index)
        {
            if(current_index + 1 > _parsed_tokens.Count)
                throw new MissingMemberException(string.Format("There should be at least closing parenthesis after opened one. Position: {0}", current_index));
            List<Member> members_in_brackets = new List<Member>();
            if (current_index - 1 > 0)
            {
                var token_bef_br = _parsed_tokens[current_index - 1];
                if (token_bef_br.Type == TokenType.MATH_OPERATION)
                {
                    int index = current_index+1;
                    var cur_token = _parsed_tokens[index];
                    while (cur_token.Type != TokenType.C_BRACKET)
                    {
                        var cur_member = GetMember(cur_token, index);
                        if (cur_member == null)
                            throw new InvalidCastException(string.Format("Incorrect member \"{0}\" in equation", cur_token.Text));
                        members_in_brackets.Add(cur_member);
                        index++;
                    }

                    if (token_bef_br.Operation == MathOperation.MINUS)
                    {
                        foreach (var item in members_in_brackets)
                        {
                            if (item.Operation == MathOperation.PLUS)
                                item.Operation = MathOperation.MINUS;
                            if(item.Operation == MathOperation.MINUS)
                                item.Operation = MathOperation.PLUS;
                        }
                    }

                    if (token_bef_br.Operation == MathOperation.MULTIPLY)
                    {
                        if(current_index - 2 < 0)
                            throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index - 2));
                        var token_bef_operation = Members[current_index - 2];
                        if(token_bef_br.Type != TokenType.MEMBER || token_bef_br.Type != TokenType.NUMERIC)
                            throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index - 2));
                        for(int i=0;i<members_in_brackets.Count;i++)
                        {
                            members_in_brackets[i] *= token_bef_operation;
                        }
                    }
                }//end if token before bracket is operation
                else if (token_bef_br.Type == TokenType.NUMERIC || token_bef_br.Type == TokenType.MEMBER)
                {

                }
            }

            throw new NotImplementedException();
        }


    }
}
