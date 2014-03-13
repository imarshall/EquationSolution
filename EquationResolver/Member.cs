using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace EquationSolution
{
    public enum MathOperation { NONE, PLUS, MINUS, MULTIPLY, DIV, POW, EQUAL }

    class Member
    {
        //1.23234w1^3
        //123.123a^2
        private const string member_var_pattern = @"^((?:[1-9]\d*|0)?(?:\.\d+)?)(([a-zA-Z]{1,}[0-9]*(\^[0-9])?){1,})$";
        //1.23234
        //123.123
        private const string member_novar_pattern = @"^(?:[1-9]\d*|0)?(?:\.\d+)?$";
        private List<string> _variable_aliases;
        private double _factor = 0.0f;
        public double Factor {
            get { return _factor; }
            set { _factor = value; }
        }
        public List<Variable> Variables { get; set; }
        public MathOperation Operation { get; set; }

        public Member()
        {
            Variables = new List<Variable>();
            Operation = MathOperation.PLUS;
        }

        public static Member FromStringExpression(string input_str, List<string> _variable_aliases)
        {
            Member mem = new Member();
            var match = Regex.Match(input_str, member_var_pattern);
            if (match.Success)
            {
                //it is member with variables
                //1 group - float numeric
                //2 group - variables with powers
                //3 group - useless last variable
                //4 group - useless last power
                if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                    throw new Exception("incorrect member");
                input_str = input_str.Replace(mem._factor.ToString(CultureInfo.InvariantCulture), "");
                mem.Variables = Variable.FromStringExpression(input_str, _variable_aliases);
                //todo get operation if there is one
            }
            else
            {
                match = Regex.Match(input_str, member_novar_pattern);
                if (match.Success)
                {
                    //it is float numeric
                    if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                        throw new Exception("incorrect member");
                    //todo get operation if there is one
                }
            }
            return mem;
        }
    }
}
