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
        //private const string member_novar_pattern = @"^((?:[1-9]\d*|0)?(?:\.\d+))?$";
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

        public static Member operator *(Member mem1, Member mem2)
        {
            Member result = new Member();
            result.Factor = mem1.Factor * mem2.Factor;
            List<Variable> variable_list = new List<Variable>();
            foreach (var item in mem1.Variables)
                variable_list.Add(item.Clone());
            foreach (var item in mem2.Variables)
            {
                if (variable_list.Where(x => x.Alias == item.Alias).Count() == 1)
                {
                    var tmp = variable_list.Find(x => x.Alias == item.Alias);
                    tmp *= item;
                    var in_list = variable_list.Find(x => x.Alias == item.Alias);
                    in_list.Power = tmp.Power;
                    in_list.Factor = tmp.Factor;
                }
                else
                {
                    variable_list.Add(item);
                }
            }
            result.Variables = variable_list;
            return result;
        }

        public override string ToString()
        {
            string _factor = "";
            //(this.Factor == 1) ? "" : this.Factor.ToString();
            if (Variables.Count == 0)
                _factor = this.Factor.ToString();
            else if (this.Factor != 1)
                _factor = this.Factor.ToString();
            StringBuilder variables = new StringBuilder(10);
            string sign = "";
            foreach (var item in this.Variables)
                variables.Append(item.ToString());
            switch (this.Operation)
            {
                case MathOperation.PLUS:
                    sign = "+";
                    break;
                case MathOperation.MINUS:
                    sign = "-";
                    break;
                case MathOperation.MULTIPLY:
                    sign = "*";
                    break;
                case MathOperation.DIV:
                    sign = "/";
                    break;
            }
            return string.Format("{2} {0}{1}", _factor, variables.ToString(), sign);
        }

        public string ToString(string format)
        {
            string _factor = (this.Factor == 1) ? "" : this.Factor.ToString();
            StringBuilder variables = new StringBuilder(10);
            string sign = "";
            foreach (var item in this.Variables)
                variables.Append(item.ToString());
            throw new NotImplementedException();
        }

        public static Member operator /(Member mem1, Member mem2)
        {
            Member result = new Member();
            result.Factor = mem1.Factor / mem2.Factor;
            List<Variable> variable_list = new List<Variable>();
            foreach (var item in mem1.Variables)
                variable_list.Add(item.Clone());
            foreach (var item in mem2.Variables)
            {
                if (variable_list.Where(x => x.Alias == item.Alias).Count() == 1)
                {
                    var tmp = variable_list.Find(x => x.Alias == item.Alias);
                    tmp /= item;
                    var in_list = variable_list.Find(x => x.Alias == item.Alias);
                    in_list.Power = tmp.Power;
                    in_list.Factor = tmp.Factor;
                }
                else
                {
                    var clone = item.Clone();
                    clone.Power *= -1;
                    variable_list.Add(clone);
                }
            }
            result.Variables = variable_list;
            return result;
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
                //if(string.IsNullOrEmpty(match.Groups[1].Value))
                if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                    mem._factor = 1.0;
                input_str = input_str.Replace(mem._factor.ToString(CultureInfo.InvariantCulture), "");
                mem.Variables = Variable.FromStringExpression(input_str, _variable_aliases);
                //todo get operation if there is one
            }
            else
            {
                //match = Regex.Match(input_str, member_novar_pattern);
                //if (match.Success)
                //{
                    //it is float numeric
                if (!double.TryParse(input_str, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                    mem._factor = 1.0;
                    //todo get operation if there is one
                //}
            }
            return mem;
        }
    }
}
