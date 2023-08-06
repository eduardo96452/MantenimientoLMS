/*
*  
*  Copyright (C) 2010-2011 University College Ghent.
*  
*  For a full list of contributors, see "credits.txt".
*  
*  This file is part of LMS Desktop Assistant.
*  
*  LMS Desktop Assistant is free software: you can redistribute it and/or modify
*  it under the terms of the GNU General Public License as published by
*  the Free Software Foundation, either version 3 of the License, or
*  (at your option) any later version.
*  
*  LMS Desktop Assistant is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*  GNU General Public License for more details.
*  
*  You should have received a copy of the GNU General Public License
*  along with LMS Desktop Assistant. If not, see <http://www.gnu.org/licenses/>.
*  
*/

namespace lmsda.gui.subject
{
    partial class FileOptionsExercisePanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.chkOneQuestionPerPage = new System.Windows.Forms.CheckBox();
            this.chkSetExerciseInvisible = new System.Windows.Forms.CheckBox();
            this.chkRandomQuestions = new System.Windows.Forms.CheckBox();
            this.nrRandomQuestions = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nrRandomQuestions)).BeginInit();
            this.SuspendLayout();
            // 
            // chkOneQuestionPerPage
            // 
            this.chkOneQuestionPerPage.AutoSize = true;
            this.chkOneQuestionPerPage.Location = new System.Drawing.Point(3, 26);
            this.chkOneQuestionPerPage.Name = "chkOneQuestionPerPage";
            this.chkOneQuestionPerPage.Size = new System.Drawing.Size(141, 17);
            this.chkOneQuestionPerPage.TabIndex = 29;
            this.chkOneQuestionPerPage.Tag = "one_question_per_page";
            this.chkOneQuestionPerPage.Text = "one_question_per_page";
            this.chkOneQuestionPerPage.UseVisualStyleBackColor = true;
            this.chkOneQuestionPerPage.CheckedChanged += new System.EventHandler(this.chkOneQuestionPerPage_CheckedChanged);
            // 
            // chkSetExerciseInvisible
            // 
            this.chkSetExerciseInvisible.AutoSize = true;
            this.chkSetExerciseInvisible.Location = new System.Drawing.Point(3, 3);
            this.chkSetExerciseInvisible.Name = "chkSetExerciseInvisible";
            this.chkSetExerciseInvisible.Size = new System.Drawing.Size(133, 17);
            this.chkSetExerciseInvisible.TabIndex = 41;
            this.chkSetExerciseInvisible.Tag = "set_exercises_invisible";
            this.chkSetExerciseInvisible.Text = "set_exercises_invisible";
            this.chkSetExerciseInvisible.UseVisualStyleBackColor = true;
            this.chkSetExerciseInvisible.CheckedChanged += new System.EventHandler(this.chkSetExerciseInvisible_CheckedChanged);
            // 
            // chkRandomQuestions
            // 
            this.chkRandomQuestions.AutoSize = true;
            this.chkRandomQuestions.Location = new System.Drawing.Point(4, 50);
            this.chkRandomQuestions.Name = "chkRandomQuestions";
            this.chkRandomQuestions.Size = new System.Drawing.Size(135, 17);
            this.chkRandomQuestions.TabIndex = 42;
            this.chkRandomQuestions.Tag = "use_random_questions";
            this.chkRandomQuestions.Text = "use_random_questions";
            this.chkRandomQuestions.UseVisualStyleBackColor = true;
            this.chkRandomQuestions.CheckedChanged += new System.EventHandler(this.chkRandomQuestions_CheckedChanged);
            // 
            // nrRandomQuestions
            // 
            this.nrRandomQuestions.Location = new System.Drawing.Point(219, 49);
            this.nrRandomQuestions.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nrRandomQuestions.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nrRandomQuestions.Name = "nrRandomQuestions";
            this.nrRandomQuestions.Size = new System.Drawing.Size(120, 20);
            this.nrRandomQuestions.TabIndex = 43;
            this.nrRandomQuestions.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.nrRandomQuestions.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nrRandomQuestions.Visible = false;
            this.nrRandomQuestions.ValueChanged += new System.EventHandler(this.nrRandomQuestions_ValueChanged);
            // 
            // FileOptionsExercisePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.nrRandomQuestions);
            this.Controls.Add(this.chkRandomQuestions);
            this.Controls.Add(this.chkSetExerciseInvisible);
            this.Controls.Add(this.chkOneQuestionPerPage);
            this.Name = "FileOptionsExercisePanel";
            this.Size = new System.Drawing.Size(420, 170);
            ((System.ComponentModel.ISupportInitialize)(this.nrRandomQuestions)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkOneQuestionPerPage;
        private System.Windows.Forms.CheckBox chkSetExerciseInvisible;
        private System.Windows.Forms.CheckBox chkRandomQuestions;
        private System.Windows.Forms.NumericUpDown nrRandomQuestions;

    }
}
