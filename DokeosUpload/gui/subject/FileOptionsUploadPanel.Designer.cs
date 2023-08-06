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
    partial class FileOptionsUploadPanel
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
            this.chkSetFileInvisible = new System.Windows.Forms.CheckBox();
            this.lblFileComment = new System.Windows.Forms.Label();
            this.txtUploadDescription = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // chkSetFileInvisible
            // 
            this.chkSetFileInvisible.AutoSize = true;
            this.chkSetFileInvisible.Location = new System.Drawing.Point(3, 3);
            this.chkSetFileInvisible.Name = "chkSetFileInvisible";
            this.chkSetFileInvisible.Size = new System.Drawing.Size(102, 17);
            this.chkSetFileInvisible.TabIndex = 41;
            this.chkSetFileInvisible.Tag = "set_file_invisible";
            this.chkSetFileInvisible.Text = "set_file_invisible";
            this.chkSetFileInvisible.UseVisualStyleBackColor = true;
            this.chkSetFileInvisible.CheckedChanged += new System.EventHandler(this.chkSetFileInvisible_CheckedChanged);
            // 
            // lblFileComment
            // 
            this.lblFileComment.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFileComment.Location = new System.Drawing.Point(3, 23);
            this.lblFileComment.Name = "lblFileComment";
            this.lblFileComment.Size = new System.Drawing.Size(145, 19);
            this.lblFileComment.TabIndex = 43;
            this.lblFileComment.Tag = "file_upload_description :";
            this.lblFileComment.Text = "file_upload_description :";
            // 
            // txtUploadDescription
            // 
            this.txtUploadDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUploadDescription.Location = new System.Drawing.Point(6, 45);
            this.txtUploadDescription.Multiline = true;
            this.txtUploadDescription.Name = "txtUploadDescription";
            this.txtUploadDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUploadDescription.Size = new System.Drawing.Size(407, 72);
            this.txtUploadDescription.TabIndex = 44;
            this.txtUploadDescription.TextChanged += new System.EventHandler(this.txtUploadDescription_TextChanged);
            // 
            // FileOptionsUploadPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtUploadDescription);
            this.Controls.Add(this.lblFileComment);
            this.Controls.Add(this.chkSetFileInvisible);
            this.Name = "FileOptionsUploadPanel";
            this.Size = new System.Drawing.Size(420, 170);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSetFileInvisible;
        private System.Windows.Forms.Label lblFileComment;
        private System.Windows.Forms.TextBox txtUploadDescription;
    }
}
