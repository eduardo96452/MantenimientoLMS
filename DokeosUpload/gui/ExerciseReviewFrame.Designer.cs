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

namespace lmsda.gui
{
    partial class ExerciseReviewFrame
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExerciseReviewFrame));
            this.txtExercisesDump = new System.Windows.Forms.TextBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.webExercisesDump = new System.Windows.Forms.WebBrowser();
            this.cmdPlainText = new System.Windows.Forms.Button();
            this.cmdHtml = new System.Windows.Forms.Button();
            this.cmdJumpToError = new System.Windows.Forms.Button();
            this.cmdJumpToFirstError = new System.Windows.Forms.Button();
            this.cmdJumpToNextError = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtExercisesDump
            // 
            this.txtExercisesDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExercisesDump.BackColor = System.Drawing.Color.White;
            this.txtExercisesDump.HideSelection = false;
            this.txtExercisesDump.Location = new System.Drawing.Point(13, 48);
            this.txtExercisesDump.Multiline = true;
            this.txtExercisesDump.Name = "txtExercisesDump";
            this.txtExercisesDump.ReadOnly = true;
            this.txtExercisesDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExercisesDump.Size = new System.Drawing.Size(617, 370);
            this.txtExercisesDump.TabIndex = 4;
            this.txtExercisesDump.WordWrap = false;
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(429, 453);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(200, 23);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Tag = "close";
            this.cmdClose.Text = "close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // webExercisesDump
            // 
            this.webExercisesDump.AllowNavigation = false;
            this.webExercisesDump.AllowWebBrowserDrop = false;
            this.webExercisesDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.webExercisesDump.Location = new System.Drawing.Point(14, 49);
            this.webExercisesDump.MinimumSize = new System.Drawing.Size(20, 20);
            this.webExercisesDump.Name = "webExercisesDump";
            this.webExercisesDump.ScriptErrorsSuppressed = true;
            this.webExercisesDump.Size = new System.Drawing.Size(615, 368);
            this.webExercisesDump.TabIndex = 5;
            this.webExercisesDump.WebBrowserShortcutsEnabled = false;
            // 
            // cmdPlainText
            // 
            this.cmdPlainText.BackColor = System.Drawing.Color.White;
            this.cmdPlainText.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdPlainText.Location = new System.Drawing.Point(321, 20);
            this.cmdPlainText.Name = "cmdPlainText";
            this.cmdPlainText.Size = new System.Drawing.Size(309, 29);
            this.cmdPlainText.TabIndex = 3;
            this.cmdPlainText.Tag = "show_internal_data";
            this.cmdPlainText.Text = "show_internal_data";
            this.cmdPlainText.UseVisualStyleBackColor = false;
            this.cmdPlainText.Click += new System.EventHandler(this.cmdPlainText_Click);
            // 
            // cmdHtml
            // 
            this.cmdHtml.BackColor = System.Drawing.Color.White;
            this.cmdHtml.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmdHtml.Location = new System.Drawing.Point(13, 20);
            this.cmdHtml.Name = "cmdHtml";
            this.cmdHtml.Size = new System.Drawing.Size(309, 29);
            this.cmdHtml.TabIndex = 2;
            this.cmdHtml.Tag = "show_html";
            this.cmdHtml.Text = "show_html";
            this.cmdHtml.UseVisualStyleBackColor = false;
            this.cmdHtml.Click += new System.EventHandler(this.cmdHtml_Click);
            // 
            // cmdJumpToError
            // 
            this.cmdJumpToError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdJumpToError.Location = new System.Drawing.Point(429, 424);
            this.cmdJumpToError.Name = "cmdJumpToError";
            this.cmdJumpToError.Size = new System.Drawing.Size(200, 23);
            this.cmdJumpToError.TabIndex = 1;
            this.cmdJumpToError.Tag = "overview_jump_to_error";
            this.cmdJumpToError.Text = "overview_jump_to_error";
            this.cmdJumpToError.UseVisualStyleBackColor = true;
            this.cmdJumpToError.Visible = false;
            this.cmdJumpToError.Click += new System.EventHandler(this.cmdJumpToError_Click);
            // 
            // cmdJumpToFirstError
            // 
            this.cmdJumpToFirstError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdJumpToFirstError.Location = new System.Drawing.Point(12, 424);
            this.cmdJumpToFirstError.Name = "cmdJumpToFirstError";
            this.cmdJumpToFirstError.Size = new System.Drawing.Size(200, 23);
            this.cmdJumpToFirstError.TabIndex = 6;
            this.cmdJumpToFirstError.Tag = "overview_jump_to_first_error";
            this.cmdJumpToFirstError.Text = "overview_jump_to_first_error";
            this.cmdJumpToFirstError.UseVisualStyleBackColor = true;
            this.cmdJumpToFirstError.Click += new System.EventHandler(this.cmdJumpToFirstError_Click);
            // 
            // cmdJumpToNextError
            // 
            this.cmdJumpToNextError.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cmdJumpToNextError.Location = new System.Drawing.Point(12, 453);
            this.cmdJumpToNextError.Name = "cmdJumpToNextError";
            this.cmdJumpToNextError.Size = new System.Drawing.Size(200, 23);
            this.cmdJumpToNextError.TabIndex = 7;
            this.cmdJumpToNextError.Tag = "overview_jump_to_next_error";
            this.cmdJumpToNextError.Text = "overview_jump_to_next_error";
            this.cmdJumpToNextError.UseVisualStyleBackColor = true;
            this.cmdJumpToNextError.Click += new System.EventHandler(this.cmdJumpToNextError_Click);
            // 
            // ExerciseReviewFrame
            // 
            this.AcceptButton = this.cmdClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(642, 511);
            this.Controls.Add(this.cmdJumpToNextError);
            this.Controls.Add(this.cmdJumpToFirstError);
            this.Controls.Add(this.webExercisesDump);
            this.Controls.Add(this.cmdJumpToError);
            this.Controls.Add(this.txtExercisesDump);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdPlainText);
            this.Controls.Add(this.cmdHtml);
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(650, 500);
            this.Name = "ExerciseReviewFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "review_scanned_exercises";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.ExerciseReviewFrame_Load);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ExerciseReviewFrame_MouseMove);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExercisesDump;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.WebBrowser webExercisesDump;
        private System.Windows.Forms.Button cmdPlainText;
        private System.Windows.Forms.Button cmdHtml;
        private System.Windows.Forms.Button cmdJumpToError;
        private System.Windows.Forms.Button cmdJumpToFirstError;
        private System.Windows.Forms.Button cmdJumpToNextError;
    }
}