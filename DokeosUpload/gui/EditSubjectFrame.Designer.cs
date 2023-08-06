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
    partial class EditSubjectFrame
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
            this.txtSubjectName = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.lblSubjectName = new System.Windows.Forms.Label();
            this.txtSubjectFolder = new System.Windows.Forms.TextBox();
            this.lblSubjectFolder = new System.Windows.Forms.Label();
            this.cmdSubjectFolderBrowse = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtSubjectName
            // 
            this.txtSubjectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubjectName.Location = new System.Drawing.Point(23, 29);
            this.txtSubjectName.Name = "txtSubjectName";
            this.txtSubjectName.Size = new System.Drawing.Size(446, 20);
            this.txtSubjectName.TabIndex = 3;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(394, 99);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 29;
            this.cmdCancel.Tag = "cancel";
            this.cmdCancel.Text = "cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.Location = new System.Drawing.Point(313, 99);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 30;
            this.cmdOk.Tag = "ok";
            this.cmdOk.Text = "ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // lblSubjectName
            // 
            this.lblSubjectName.AutoSize = true;
            this.lblSubjectName.Location = new System.Drawing.Point(20, 13);
            this.lblSubjectName.Name = "lblSubjectName";
            this.lblSubjectName.Size = new System.Drawing.Size(79, 13);
            this.lblSubjectName.TabIndex = 31;
            this.lblSubjectName.Tag = "subject_name :";
            this.lblSubjectName.Text = "subject_name :";
            // 
            // txtSubjectFolder
            // 
            this.txtSubjectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubjectFolder.Location = new System.Drawing.Point(23, 68);
            this.txtSubjectFolder.Name = "txtSubjectFolder";
            this.txtSubjectFolder.ReadOnly = true;
            this.txtSubjectFolder.Size = new System.Drawing.Size(330, 20);
            this.txtSubjectFolder.TabIndex = 32;
            // 
            // lblSubjectFolder
            // 
            this.lblSubjectFolder.AutoSize = true;
            this.lblSubjectFolder.Location = new System.Drawing.Point(20, 52);
            this.lblSubjectFolder.Name = "lblSubjectFolder";
            this.lblSubjectFolder.Size = new System.Drawing.Size(79, 13);
            this.lblSubjectFolder.TabIndex = 31;
            this.lblSubjectFolder.Tag = "subject_folder :";
            this.lblSubjectFolder.Text = "subject_folder :";
            // 
            // cmdSubjectFolderBrowse
            // 
            this.cmdSubjectFolderBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdSubjectFolderBrowse.Location = new System.Drawing.Point(359, 66);
            this.cmdSubjectFolderBrowse.Name = "cmdSubjectFolderBrowse";
            this.cmdSubjectFolderBrowse.Size = new System.Drawing.Size(110, 23);
            this.cmdSubjectFolderBrowse.TabIndex = 33;
            this.cmdSubjectFolderBrowse.Tag = "browse";
            this.cmdSubjectFolderBrowse.Text = "browse";
            this.cmdSubjectFolderBrowse.UseVisualStyleBackColor = true;
            this.cmdSubjectFolderBrowse.Click += new System.EventHandler(this.cmdSubjectFolderBrowse_Click);
            // 
            // EditSubjectFrame
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(492, 134);
            this.Controls.Add(this.cmdSubjectFolderBrowse);
            this.Controls.Add(this.txtSubjectFolder);
            this.Controls.Add(this.lblSubjectFolder);
            this.Controls.Add(this.lblSubjectName);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtSubjectName);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(10000, 168);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 168);
            this.Name = "EditSubjectFrame";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "";
            this.Text = "create_subject";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSubjectName;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Label lblSubjectName;
        private System.Windows.Forms.TextBox txtSubjectFolder;
        private System.Windows.Forms.Label lblSubjectFolder;
        private System.Windows.Forms.Button cmdSubjectFolderBrowse;

    }
}