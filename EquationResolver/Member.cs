using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace EquationSolution
{
    public enum MathOperation { NONE, PLUS, MINUS, MULTIPLY, DIV, POW, EQUAL }

    class Member : ICloneable
    {
        private const string member_var_pattern = @"^((?:[1-9]\d*|0)?(?:\.\d+)?)(([a-zA-Z]{1,}[0-9]*(\^[0-9])?){1,})$";
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

        /// <summary>
        /// Maximum power within variables
        /// </summary>
        public Double MaxPower
        {
            get 
            {
                double max = 0;
                foreach (var item in Variables)
                {
                    if (item.Power > max)
                        max = item.Power;
                }
                return max;
            }
        }

        public static Member operator +(Member mem1, Member mem2)
        {
            Member result = (Member)mem1.Clone();
            if (!mem1.CompareVariables(mem2.Variables))
                throw new InvalidOperationException("Cannot add not similar members");
            if(result.Operation == mem2.Operation)
                result._factor += mem2._factor;
            else
                result._factor -= mem2._factor;
            validateMember(ref result);
            return result;
        }

        public static Member operator -(Member mem1, Member mem2)
        {
            Member result = (Member)mem1.Clone();
            if (!mem1.CompareVariables(mem2.Variables))
                throw new InvalidOperationException("Cannot add not similar members");
            if (result.Operation == mem2.Operation)
                result._factor += mem2._factor;
            else
                result._factor -= mem2._factor;
            validateMember(ref result);
            return result;
        }

        /// <summary>
        /// Do some compares to determine an operation to be set
        /// </summary>
        /// <param name="op1"></param>
        /// <param name="op2"></param>
        /// <returns></returns>
        private static MathOperation getOperationMultiplyOrDivide(MathOperation op1, MathOperation op2)
        {
            MathOperation result = MathOperation.PLUS;
            if (op1 == MathOperation.MULTIPLY || op1 == MathOperation.DIV)
                if (op2 == MathOperation.MINUS)
                    result = MathOperation.MINUS;
            if (op2 == MathOperation.MULTIPLY || op2 == MathOperation.DIV)
                if (op1 == MathOperation.MINUS)
                    result = MathOperation.MINUS;

            if (op1 == MathOperation.PLUS && op2 == MathOperation.MINUS)
                result = MathOperation.MINUS;
            if (op2 == MathOperation.PLUS && op1 == MathOperation.MINUS)
                result = MathOperation.MINUS;
            return result;
        }

        /// <summary>
        /// Checks what sign does factor has and sets Operation if it is required
        /// </summary>
        /// <param name="mem"></param>
        private static void validateMember(ref Member mem)
        {
            if (mem.Factor < 0 && mem.Operation == MathOperation.PLUS)
            {
                mem.Factor = -mem.Factor;
                mem.Operation = MathOperation.MINUS;
            }
            else if (mem.Factor < 0 && mem.Operation == MathOperation.MINUS)
            {
                mem.Factor = -mem.Factor;
                mem.Operation = MathOperation.PLUS;
            }
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
            result.Operation = getOperationMultiplyOrDivide(mem1.Operation, mem2.Operation);
            result.Variables = variable_list;

            validateMember(ref result);
            return result;
        }
        
        public override string ToString()
        {
            string _factor = "";
            Variables = Variables.OrderBy(x => x.Alias).ToList();
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
            string variables_str = variables.ToString();
            if (this.Factor == 0)
            {
                _factor = "";
                variables_str = "";
                sign = "";
            }
            return string.Format("{2} {0}{1}", _factor, variables_str, sign);
        }

        /// <summary>
        /// Do not write sign if it is "+"
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            string _factor = "";
            if (Variables.Count == 0)
                _factor = this.Factor.ToString();
            else if (this.Factor != 1)
                _factor = this.Factor.ToString();
            string sign = "";
            switch (this.Operation)
            {
                case MathOperation.PLUS:
                    sign = "";
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
            StringBuilder variables = new StringBuilder(10);
            foreach (var item in this.Variables)
                variables.Append(item.ToString());
            return string.Format("{2}{0}{1}", _factor, variables.ToString(), sign);
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
            result.Operation = getOperationMultiplyOrDivide(mem1.Operation, mem2.Operation);
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
                if (!double.TryParse(match.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                    mem._factor = 1.0;
                input_str = input_str.Replace(mem._factor.ToString(CultureInfo.InvariantCulture), "");
                mem.Variables = Variable.FromStringExpression(input_str, _variable_aliases);
                //todo get operation if there is one
            }
            else
            {
                //it is float numeric
                if (!double.TryParse(input_str, NumberStyles.Any, CultureInfo.InvariantCulture, out mem._factor))
                    mem._factor = 1.0;
                //todo get operation if there is one
            }
            return mem;
        }

        public object Clone()
        {
            Member _copy = new Member();
            _copy.Operation = this.Operation;
            _copy.Factor = this.Factor;
            foreach (var item in this.Variables)
                _copy.Variables.Add(item.Clone());
            return _copy;
        }

        /// <summary>
        /// Compares two lists of variables between each other
        /// </summary>
        /// <param name="in_variables">another list of variables</param>
        /// <returns>True if equal</returns>
        public bool CompareVariables(List<Variable> in_variables)
        {
            if (this.Variables.Count != in_variables.Count)
                return false;
            foreach (var my_var in this.Variables)
            {
                var match = in_variables.Where(x => x.Alias == my_var.Alias && x.Power == my_var.Power).FirstOrDefault();
                if (match == null)
                    return false;
            }
            return true;
        }
    }
}
