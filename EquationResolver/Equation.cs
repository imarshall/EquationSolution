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
            _parsed_tokens = new List<ExpressionToken>();
        }

        public Equation(string _expr, List<string> variables)
        {
            Members = new List<Member>();
            Expression = _expr;
            _variable_aliases = variables;
            _parsed_tokens = new List<ExpressionToken>();
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
                    if (index - 1 > 0)
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
            Members.Clear();
            _parsed_tokens.Clear();
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
            var multiply = Members.Where(x => x != null && (x.Operation == MathOperation.MULTIPLY || x.Operation == MathOperation.DIV)).ToList();
            foreach (var item in multiply)
            {
                var index = Members.IndexOf(item);
                var sibling = (Member)Members[index - 1].Clone();
                switch (item.Operation)
                {
                    case MathOperation.MULTIPLY:
                        sibling *= item;
                        break;
                    case MathOperation.DIV:
                        sibling /= item;
                        break;
                }
                
                SemiReducted.Add(sibling);
                Members.RemoveAt(index);
                Members.RemoveAt(index - 1);
            }

            var plus_minus = performPlusMinusOperations(Members);
            SemiReducted.AddRange(plus_minus);
            Members = performPlusMinusOperations(SemiReducted);
        }

        private List<Member> performPlusMinusOperations(List<Member> in_members)
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

        public int OpenBracket(int o_br_index)
        {
            List<Member> tmp = new List<Member>();
            return OpenBracket(o_br_index, ref tmp);
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
        public int OpenBracket(int o_br_index, ref List<Member> near_bracket)
        {
            if (o_br_index + 1 > _parsed_tokens.Count)
                throw new MissingMemberException(string.Format("There should be closing parenthesis after opened one. Position: {0}", o_br_index));

            List<Member> members_in_brackets = new List<Member>();
            int current_index = o_br_index + 1;
            int replace_index = -1;
            var cur_token = _parsed_tokens[current_index];

            while (cur_token.Type != TokenType.C_BRACKET)
            {
                var cur_member = GetMember(cur_token, ref current_index);
                if (cur_member == null && cur_token.Type != TokenType.MEMBER && cur_token.Type != TokenType.NUMERIC)
                {
                    current_index++;
                    cur_token = _parsed_tokens[current_index];
                    continue;
                }
                members_in_brackets.Add(cur_member);
                current_index++;
                cur_token = _parsed_tokens[current_index];
            }

            //if it is recursive
            if (near_bracket.Count != 0)
            {
                performBracketOperation(current_index, MathOperation.MULTIPLY, ref members_in_brackets, near_bracket);
            }

            //if there is smth before brackets
            if (o_br_index - 1 > 0) 
            {
                var token_bef_br = _parsed_tokens[o_br_index - 1];

                if (token_bef_br.Type == TokenType.MATH_OPERATION)
                {
                    if (token_bef_br.Operation == MathOperation.MINUS)
                    {
                        foreach (var item in members_in_brackets)
                        {
                            if (item.Operation == MathOperation.PLUS)
                                item.Operation = MathOperation.MINUS;
                            else if (item.Operation == MathOperation.MINUS)
                                item.Operation = MathOperation.PLUS;
                        }
                    }

                    var member_bef_operation = Members[o_br_index - 2];
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", o_br_index - 2));
                    asdqwdmw(o_br_index - 2, token_bef_br.Operation, ref members_in_brackets, new List<Member>() { member_bef_operation });
                    replace_index = o_br_index - 2;
                }//end if token before bracket is operation
                else if (token_bef_br.Type == TokenType.NUMERIC || token_bef_br.Type == TokenType.MEMBER)
                {
                    var member_bef_operation = Members[o_br_index - 1];
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", o_br_index - 1));
                    asdqwdmw(o_br_index - 1, MathOperation.MULTIPLY, ref members_in_brackets, new List<Member>() { member_bef_operation });
                    replace_index = o_br_index - 1;
                }
            }//end if there is something before brackets

            //if there is smth after brackets
            if (current_index < _parsed_tokens.Count) 
            {
                var token_aft_br = _parsed_tokens[current_index + 1];
                if (token_aft_br.Type == TokenType.MATH_OPERATION && token_aft_br.Operation != MathOperation.EQUAL)
                {
                    var member_bef_operation = GetMember(_parsed_tokens[current_index + 2], ref current_index);
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index + 2));
                    asdqwdmw(current_index + 2, token_aft_br.Operation, ref members_in_brackets, new List<Member>() { member_bef_operation });
                }
                else if (token_aft_br.Type == TokenType.NUMERIC || token_aft_br.Type == TokenType.MEMBER)
                {
                    var member_bef_operation = GetMember(_parsed_tokens[current_index + 1], ref current_index);//Members[current_index + 1];
                    if (member_bef_operation == null)
                        throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index + 1));
                    asdqwdmw(current_index + 1, MathOperation.MULTIPLY, ref members_in_brackets, new List<Member>() { member_bef_operation });
                }
                else if (token_aft_br.Type == TokenType.O_BRACKET)
                {
                    current_index++;
                    current_index = OpenBracket(current_index, ref members_in_brackets);
                }
            }

            if (near_bracket.Count == 0)
            {
                int index = 0;
                if (replace_index > 0)
                {
                    Members[replace_index] = members_in_brackets[0];
                    index++;
                }
                for (; index < members_in_brackets.Count; index++)
                    Members.Add(members_in_brackets[index]);
            }
            else
            {
                near_bracket = members_in_brackets;
            }

            return current_index;
        }

        private void asdqwdmw(int index, MathOperation operation, ref List<Member> members, List<Member> m_out_br)
        {
            if (m_out_br == null || m_out_br.Count == 0)
                throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", index));
            performBracketOperation(index, operation,
                ref members, m_out_br);
            //replaceMemberWithMembers(index, members);
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
            ref List<Member> m_in_brackets, List<Member> m_out_brackets)
        {
            if (current_index < 0)
                throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index));
            //if (Members.Count < current_index)
            //    throw new MissingMemberException(string.Format("Missing member before operation. Position: {0}", current_index));
            if (m_out_brackets == null || m_out_brackets.Count == 0)
                throw new MissingMemberException(string.Format("Missing member before operation.  Position: {0}", current_index));
            var initial_values = new List<Member>();
            foreach (var item in m_in_brackets)
                initial_values.Add((Member)item.Clone());
            m_in_brackets.Clear();
            int expected_amount = -1;
            if(operation == MathOperation.MULTIPLY || operation == MathOperation.DIV)
                expected_amount = initial_values.Count * m_out_brackets.Count;
            else if (operation == MathOperation.MINUS || operation == MathOperation.PLUS)
                expected_amount = initial_values.Count;
            for (int i = 0; i < expected_amount; i++)
                m_in_brackets.Add(new Member());
            int out_count = 0;
            for (int k = 0; k < m_out_brackets.Count; k++)
            {
                for (int i = 0; i < initial_values.Count; i++)
                {
                    switch (operation)
                    {
                        case MathOperation.MULTIPLY:
                            m_in_brackets[out_count] = initial_values[i] * m_out_brackets[k];
                            break;
                        case MathOperation.DIV:
                            m_in_brackets[out_count] = initial_values[i] / m_out_brackets[k];
                            break;
                        case MathOperation.MINUS:
                            m_in_brackets[out_count] = initial_values[i];
                            break;
                        case MathOperation.PLUS:
                            m_in_brackets[out_count] = initial_values[i];
                            break;
                    }
                    out_count++;
                }
            }
        }

        private void replaceMemberWithMembers(int replace_index, List<Member> members)
        {
            int index = 0;
            Members[replace_index] = members[0];
            index++;
            for (; index < members.Count; index++)
                Members.Add(members[index]);
        }


    }
}
