using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsUI
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private void buttonAddVar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxVariable.Text))
            {
                double tmp = 0;
                if (!double.TryParse(textBoxVariable.Text, out tmp))
                {
                    listBoxVariables.Items.Add(textBoxVariable.Text);
                    textBoxVariable.SelectAll();
                    textBoxVariable.Focus();
                }
                else
                    MessageBox.Show("Variable cannot be written as a number", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show("Variable cannot be an empty string", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void buttonVarDel_Click(object sender, EventArgs e)
        {
            var selected = listBoxVariables.SelectedIndex;
            if (selected != -1)
                listBoxVariables.Items.RemoveAt(selected);
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxEquation.Text))
            {
                MessageBox.Show("Equation shouldn't be empty string", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (listBoxVariables.Items.Count == 0)
            {
                MessageBox.Show("Please specify variables that are in equation", "Caution", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxVariable.Focus();
                return;
            }
            List<string> vars = new List<string>();
            foreach (var item in listBoxVariables.Items)
                vars.Add(item.ToString());
            EquationSolution.Equation eq = new EquationSolution.Equation(textBoxEquation.Text, vars);
            try
            {
                eq.Evaluate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            textBoxResult.Text = eq.ToString();
        }
    }
}
