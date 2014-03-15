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
        private const string split_pattern = "[+]|[-]|[/]|[*]|[(]|[)]|[=]";
        private const string split_pattern_inc = "([+]|[-]|[/]|[*]|[(]|[)]|[=])";

        public string Expression { get; set; }
        public List<Member> Members { get; set; }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder(40);
            int members_count = Members.Count;
            Members = Members.OrderByDescending(x => x.MaxPower).ToList();
            if (members_count > 0)
            {
                result.AppendFormat("{0} ", Members[0].ToShortString());
                if (members_count > 1)
                {
                    for (int i = 1; i < members_count; i++)
                    {
                        if (Members[i] == null)
                            continue;
                        result.AppendFormat("{0} ", Members[i].ToString());
                    }
                }
                result.Append("= 0");
            }
            return result.ToString();
        }

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
                token = token.Trim();
                if (string.IsNullOrEmpty(token))
                    continue;

                ExpressionToken expr_token = ExpressionToken.FromStringExpression(token);
                if (expr_token.Type == TokenType.NONE)
                    throw new ArgumentException("Equation member has incorrect format");
                _parsed_tokens.Add(expr_token);
            }
            //we've got preparsed tokens

            var obrackets = _parsed_tokens.Where(x => x.Type == TokenType.O_BRACKET).ToList();
            var cbrackets = _parsed_tokens.Where(x => x.Type == TokenType.C_BRACKET).ToList();
            if (obrackets.Count != cbrackets.Count)
                throw new ArgumentException("Equation is incorrect. Some brackets is not closed/opened", "Expression");
            if(_parsed_tokens.Where(x=>x.Type == TokenType.MATH_OPERATION && x.Operation == MathOperation.EQUAL).Count() != 1)
                throw new ArgumentException("Equation is incorrect. There should be one equal sign", "Expression");
        }

        private Member GetMember(ExpressionToken token, ref int index)
        {
            Member cur_member = null;
            switch (token.Type)
            {
                case TokenType.MATH_OPERATION:
                    if(token.Operation == MathOperation.EQUAL)
                        MoveEqualSign(ref index);
                    break;
                case TokenType.MEMBER:
                    cur_member = Member.FromStringExpression(token.Text, _variable_aliases);
                    if (index - 1 > 0)
                        if (_parsed_tokens[index - 1].Type == TokenType.MATH_OPERATION && _parsed_tokens[index - 1].Operation != MathOperation.EQUAL)
                            cur_member.Operation = _parsed_tokens[index - 1].Operation;
                    break;
                case TokenType.NUMERIC:
                    cur_member = Member.FromStringExpression(token.Text, _variable_aliases);
                    if (_parsed_tokens[index - 1].Type == TokenType.MATH_OPERATION && _parsed_tokens[index - 1].Operation != MathOperation.EQUAL)
                        cur_member.Operation = _parsed_tokens[index - 1].Operation;
                    break;
                case TokenType.O_BRACKET:
                    index = OpenBracket(index);
                    break;
            }
            return cur_member;
        }

        private void MoveEqualSign(ref int index)
        {
            for (index = index+1; index < _parsed_tokens.Count; index++)
            {
                var mem = GetMember(_parsed_tokens[index],ref index);
                if (mem == null)
                    continue;
                if (mem.Operation == MathOperation.PLUS)
                    mem.Operation = MathOperation.MINUS;
                else if (mem.Operation == MathOperation.MINUS)
                    mem.Operation = MathOperation.PLUS;
                Members.Add(mem);
            }
        }

        public void Evaluate()
        {
            GetTokens();
            for(int i=0;i<_parsed_tokens.Count;i++)
            {
                var token = _parsed_tokens[i];
                var cur_member = GetMember(token, ref i);
                if (cur_member == null && (token.Type == TokenType.MEMBER || token.Type == TokenType.NUMERIC))
                    throw new InvalidCastException(string.Format("Incorrect member \"{0}\" in equation", token.Text));
                Members.Add(cur_member);
            }
            ReductionOfSimilar();
        }

        private void ReductionOfSimilar()
        {
            List<Member> Reducted = new List<Member>();

            List<Member> SemiReducted = new List<Member>();
            var multiply = Members.Where(x => x != null && x.Operation == MathOperation.MULTIPLY).ToList();
            foreach (var item in multiply)
            {
                var index = Members.IndexOf(item);
                var sibling = (Member)Members[index - 1].Clone();
                sibling *= item;
                SemiReducted.Add(sibling);
                Members.RemoveAt(index);
                Members.RemoveAt(index - 1);
            }

            var divide = Members.Where(x => x != null && x.Operation == MathOperation.DIV).ToList();
            foreach (var item in divide)
            {
                var index = Members.IndexOf(item);
                var sibling = (Member)Members[index - 1].Clone();
                sibling /= item;
                SemiReducted.Add(sibling);
                Members.Remove(item);
                Members.Remove(sibling);
            }
            var plus_minus = PerformPlusMinusOperations(Members);
            SemiReducted.AddRange(plus_minus);
            Members = PerformPlusMinusOperations(SemiReducted);
        }

        private List<Member> PerformPlusMinusOperations(List<Member> in_members)
        {
            List<Member> Reducted = new List<Member>();
            var plus_min = in_members.Where(x => x != null && (x.Operation == MathOperation.PLUS || x.Operation == MathOperation.MINUS)).ToList();
            List<Member> handled = new List<Member>();
            foreach (var item in plus_min)
            {
                if (handled.Contains(item))
                    continue;
                var similar = in_members.Where(x => x != null && x.CompareVariables(item.Variables) && item != x).ToList();
                Member result = (Member)item.Clone();
                foreach (var summand in similar)
                {
                    switch (summand.Operation)
                    {
                        case MathOperation.PLUS:
                            result += summand;
                            break;
                        case MathOperation.MINUS:
                            result -= summand;
                            break;
                    }
                    handled.Add(summand);
                }
                Reducted.Add(result);
            }
            return Reducted;
        }

        /// <summary>
        /// Perform required operations to members inside brackets: 
        /// change sign,
        /// multiply on numeric constant,
        /// multiply on members
        /// </summary>
        /// <param name="current_index">index of "(" token</param>
        /// <param name="ParsedTokens">recently parsed tokens of the expression</param>
        /// <param name="tokens">entiry array of expression's tokens</param>
        /// <returns>index on closing bracket token</returns>
        public int OpenBracket(int current_index)
        {
            if(current_index + 1 > _parsed_tokens.Count)
                throw new MissingMemberException(string.Format("There should be closing parenthesis after opened one. Position: {0}", current_index));
            List<Member> members_in_brackets = new List<Member>();
            if (current_index - 1 > 0)
            {
                var token_bef_br = _parsed_tokens[current_index - 1];
                int index = current_index+1;
                var cur_token = _parsed_tokens[index];
                while (cur_token.Type != TokenType.C_BRACKET)
                {
                    var cur_member = GetMember(cur_token, ref index);
                    if (cur_member == null && cur_token.Type != TokenType.MEMBER && cur_token.Type != TokenType.NUMERIC)
                    {
                        index++;
                        cur_token = _parsed_tokens[index];
                        continue;
                    }
                    members_in_brackets.Add(cur_member);
                    index++;
                    cur_token = _parsed_tokens[index];
                }

                if (token_bef_br.Type == TokenType.MATH_OPERATION)
                {
                    if (token_bef_br.Operation == MathOperation.MINUS)
                    {
                        foreach (var item in members_in_brackets)
                        {
                            if (item.Operation == MathOperation.PLUS)
                                item.Operation = MathOperation.MINUS;
                            if (item.Operation == MathOperation.MINUS)
                                item.Operation = MathOperation.PLUS;
                        }
                    }

                    var member_bef_operation = Members[current_index - 2];
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index - 2));
                    performBracketOperation(current_index - 2, token_bef_br.Operation,
                        members_in_brackets, new List<Member>() { member_bef_operation }, true);

                    current_index = index;
                }//end if token before bracket is operation
                else if (token_bef_br.Type == TokenType.NUMERIC || token_bef_br.Type == TokenType.MEMBER)
                {
                    var member_bef_operation = Members[current_index - 1];
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index - 1));
                    performBracketOperation(current_index - 1, MathOperation.MULTIPLY,
                        members_in_brackets, new List<Member>() { member_bef_operation }, true);
                    current_index = index;
                }
            }//end if there is something before brackets
            //todo handle case if there is brackets after brackets
            return current_index;
        }

        /// <summary>
        /// Performs operation with brackets (multiply, divide)
        /// </summary>
        /// <param name="current_index">current index of member before operation in tokens/members</param>
        /// <param name="operation">Mathematic operation required to perform</param>
        /// <param name="m_in_brackets">members which are in brackets</param>
        /// <param name="m_out_brackets">members which are in another brackets or out of brackets</param>
        /// <param name="replace_m_before">True - it is required to replace member located before brackets, ex. 5*(a+b)</param>
        private void performBracketOperation(int current_index, MathOperation operation,
            List<Member> m_in_brackets, List<Member> m_out_brackets, bool replace_m_before)
        {
            if (current_index < 0)
                throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index));
            if (Members.Count < current_index)
                throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index));
            if (m_out_brackets == null || m_out_brackets.Count == 0)
                throw new MissingMemberException(string.Format("Missing member before operation.  Position: {0}", current_index));
            for (int k = 0; k < m_out_brackets.Count; k++)
            {
                for (int i = 0; i < m_in_brackets.Count; i++)
                {
                    switch (operation)
                    {
                        case MathOperation.MULTIPLY:
                            m_in_brackets[i] *= m_out_brackets[k];
                            break;
                        case MathOperation.DIV:
                            m_in_brackets[i] /= m_out_brackets[k];
                            break;
                        case MathOperation.MINUS:
                            //m_in_brackets[i] *= m_out_brackets[k];
                            break;
                        case MathOperation.PLUS:
                            //m_in_brackets[i] *= m_out_brackets[k];
                            break;
                    }
                }
            }
            int index = 0;
            if (replace_m_before)
            {
                Members[current_index] = m_in_brackets[0];
                index++;
            }
            for (; index < m_in_brackets.Count; index++)
                Members.Add(m_in_brackets[index]);
        }


    }
}
