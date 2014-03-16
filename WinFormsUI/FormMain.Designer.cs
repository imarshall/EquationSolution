namespace WinFormsUI
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxEquation = new System.Windows.Forms.TextBox();
            this.labelEqText = new System.Windows.Forms.Label();
            this.buttonRun = new System.Windows.Forms.Button();
            this.groupBoxVariables = new System.Windows.Forms.GroupBox();
            this.textBoxVariable = new System.Windows.Forms.TextBox();
            this.buttonAddVar = new System.Windows.Forms.Button();
            this.listBoxVariables = new System.Windows.Forms.ListBox();
            this.buttonVarDel = new System.Windows.Forms.Button();
            this.textBoxResult = new System.Windows.Forms.TextBox();
            this.groupBoxVariables.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxEquation
            // 
            this.textBoxEquation.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxEquation.Location = new System.Drawing.Point(12, 25);
            this.textBoxEquation.Name = "textBoxEquation";
            this.textBoxEquation.Size = new System.Drawing.Size(314, 26);
            this.textBoxEquation.TabIndex = 1;
            // 
            // labelEqText
            // 
            this.labelEqText.AutoSize = true;
            this.labelEqText.Location = new System.Drawing.Point(12, 9);
            this.labelEqText.Name = "labelEqText";
            this.labelEqText.Size = new System.Drawing.Size(79, 13);
            this.labelEqText.TabIndex = 2;
            this.labelEqText.Text = "Write equation:";
            // 
            // buttonRun
            // 
            this.buttonRun.Location = new System.Drawing.Point(12, 57);
            this.buttonRun.Name = "buttonRun";
            this.buttonRun.Size = new System.Drawing.Size(75, 23);
            this.buttonRun.TabIndex = 3;
            this.buttonRun.Text = "Run";
            this.buttonRun.UseVisualStyleBackColor = true;
            this.buttonRun.Click += new System.EventHandler(this.buttonRun_Click);
            // 
            // groupBoxVariables
            // 
            this.groupBoxVariables.Controls.Add(this.buttonVarDel);
            this.groupBoxVariables.Controls.Add(this.textBoxVariable);
            this.groupBoxVariables.Controls.Add(this.buttonAddVar);
            this.groupBoxVariables.Controls.Add(this.listBoxVariables);
            this.groupBoxVariables.Location = new System.Drawing.Point(341, 9);
            this.groupBoxVariables.Name = "groupBoxVariables";
            this.groupBoxVariables.Size = new System.Drawing.Size(228, 109);
            this.groupBoxVariables.TabIndex = 8;
            this.groupBoxVariables.TabStop = false;
            this.groupBoxVariables.Text = "Variables";
            // 
            // textBoxVariable
            // 
            this.textBoxVariable.Location = new System.Drawing.Point(132, 19);
            this.textBoxVariable.Name = "textBoxVariable";
            this.textBoxVariable.Size = new System.Drawing.Size(75, 20);
            this.textBoxVariable.TabIndex = 10;
            // 
            // buttonAddVar
            // 
            this.buttonAddVar.Location = new System.Drawing.Point(132, 48);
            this.buttonAddVar.Name = "buttonAddVar";
            this.buttonAddVar.Size = new System.Drawing.Size(75, 23);
            this.buttonAddVar.TabIndex = 9;
            this.buttonAddVar.Text = "Add";
            this.buttonAddVar.UseVisualStyleBackColor = true;
            this.buttonAddVar.Click += new System.EventHandler(this.buttonAddVar_Click);
            // 
            // listBoxVariables
            // 
            this.listBoxVariables.FormattingEnabled = true;
            this.listBoxVariables.Location = new System.Drawing.Point(6, 19);
            this.listBoxVariables.Name = "listBoxVariables";
            this.listBoxVariables.Size = new System.Drawing.Size(120, 82);
            this.listBoxVariables.TabIndex = 8;
            // 
            // buttonVarDel
            // 
            this.buttonVarDel.Location = new System.Drawing.Point(132, 77);
            this.buttonVarDel.Name = "buttonVarDel";
            this.buttonVarDel.Size = new System.Drawing.Size(75, 23);
            this.buttonVarDel.TabIndex = 11;
            this.buttonVarDel.Text = "Del";
            this.buttonVarDel.UseVisualStyleBackColor = true;
            this.buttonVarDel.Click += new System.EventHandler(this.buttonVarDel_Click);
            // 
            // textBoxResult
            // 
            this.textBoxResult.AcceptsReturn = true;
            this.textBoxResult.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.textBoxResult.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxResult.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxResult.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.textBoxResult.Location = new System.Drawing.Point(12, 134);
            this.textBoxResult.Multiline = true;
            this.textBoxResult.Name = "textBoxResult";
            this.textBoxResult.ReadOnly = true;
            this.textBoxResult.Size = new System.Drawing.Size(557, 84);
            this.textBoxResult.TabIndex = 9;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(577, 230);
            this.Controls.Add(this.textBoxResult);
            this.Controls.Add(this.groupBoxVariables);
            this.Controls.Add(this.buttonRun);
            this.Controls.Add(this.labelEqText);
            this.Controls.Add(this.textBoxEquation);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FormMain";
            this.Text = "Equation Solution Example";
            this.groupBoxVariables.ResumeLayout(false);
            this.groupBoxVariables.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxEquation;
        private System.Windows.Forms.Label labelEqText;
        private System.Windows.Forms.Button buttonRun;
        private System.Windows.Forms.GroupBox groupBoxVariables;
        private System.Windows.Forms.Button buttonVarDel;
        private System.Windows.Forms.TextBox textBoxVariable;
        private System.Windows.Forms.Button buttonAddVar;
        private System.Windows.Forms.ListBox listBoxVariables;
        private System.Windows.Forms.TextBox textBoxResult;

    }
}

