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
    partial class ManageSubjectsFrame
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
            this.components = new System.ComponentModel.Container();
            this.cmdMoveSubjectDown = new System.Windows.Forms.Button();
            this.cmdMoveSubjectUp = new System.Windows.Forms.Button();
            this.cmdOpenSubjectFolder = new System.Windows.Forms.Button();
            this.lblAvailableCourses = new System.Windows.Forms.Label();
            this.lblSelectedCourses = new System.Windows.Forms.Label();
            this.lblSubjects = new System.Windows.Forms.Label();
            this.cmdEditSubject = new System.Windows.Forms.Button();
            this.txtSubjectFolder = new System.Windows.Forms.TextBox();
            this.lblCategoryFolderLabel = new System.Windows.Forms.Label();
            this.cmdRemoveSubject = new System.Windows.Forms.Button();
            this.cmdCreateSubject = new System.Windows.Forms.Button();
            this.cmdRemoveCourse = new System.Windows.Forms.Button();
            this.cmdAddCourse = new System.Windows.Forms.Button();
            this.lstAvailableCourses = new System.Windows.Forms.ListBox();
            this.lstSubject = new System.Windows.Forms.ListBox();
            this.lstSelectedCourses = new System.Windows.Forms.ListBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // cmdMoveSubjectDown
            // 
            this.cmdMoveSubjectDown.Image = global::lmsda.Properties.Resources.btn_down;
            this.cmdMoveSubjectDown.Location = new System.Drawing.Point(96, 158);
            this.cmdMoveSubjectDown.Name = "cmdMoveSubjectDown";
            this.cmdMoveSubjectDown.Size = new System.Drawing.Size(78, 23);
            this.cmdMoveSubjectDown.TabIndex = 60;
            this.cmdMoveSubjectDown.UseVisualStyleBackColor = true;
            this.cmdMoveSubjectDown.Click += new System.EventHandler(this.cmdMoveSubjectDown_Click);
            // 
            // cmdMoveSubjectUp
            // 
            this.cmdMoveSubjectUp.Image = global::lmsda.Properties.Resources.btn_up;
            this.cmdMoveSubjectUp.Location = new System.Drawing.Point(12, 158);
            this.cmdMoveSubjectUp.Name = "cmdMoveSubjectUp";
            this.cmdMoveSubjectUp.Size = new System.Drawing.Size(78, 23);
            this.cmdMoveSubjectUp.TabIndex = 59;
            this.cmdMoveSubjectUp.Tag = "";
            this.cmdMoveSubjectUp.UseVisualStyleBackColor = true;
            this.cmdMoveSubjectUp.Click += new System.EventHandler(this.cmdMoveSubjectUp_Click);
            // 
            // cmdOpenSubjectFolder
            // 
            this.cmdOpenSubjectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOpenSubjectFolder.Location = new System.Drawing.Point(499, 288);
            this.cmdOpenSubjectFolder.Name = "cmdOpenSubjectFolder";
            this.cmdOpenSubjectFolder.Size = new System.Drawing.Size(66, 23);
            this.cmdOpenSubjectFolder.TabIndex = 57;
            this.cmdOpenSubjectFolder.Tag = "open";
            this.cmdOpenSubjectFolder.Text = "open";
            this.cmdOpenSubjectFolder.UseVisualStyleBackColor = true;
            this.cmdOpenSubjectFolder.Click += new System.EventHandler(this.cmdOpenSubjectFolder_Click);
            // 
            // lblAvailableCourses
            // 
            this.lblAvailableCourses.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvailableCourses.Location = new System.Drawing.Point(410, 12);
            this.lblAvailableCourses.Name = "lblAvailableCourses";
            this.lblAvailableCourses.Size = new System.Drawing.Size(162, 16);
            this.lblAvailableCourses.TabIndex = 70;
            this.lblAvailableCourses.Tag = "available_courses";
            this.lblAvailableCourses.Text = "available_courses";
            this.lblAvailableCourses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSelectedCourses
            // 
            this.lblSelectedCourses.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectedCourses.Location = new System.Drawing.Point(197, 12);
            this.lblSelectedCourses.Name = "lblSelectedCourses";
            this.lblSelectedCourses.Size = new System.Drawing.Size(162, 16);
            this.lblSelectedCourses.TabIndex = 69;
            this.lblSelectedCourses.Tag = "courses_in_subject";
            this.lblSelectedCourses.Text = "courses_in_subject";
            this.lblSelectedCourses.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubjects
            // 
            this.lblSubjects.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubjects.Location = new System.Drawing.Point(12, 12);
            this.lblSubjects.Name = "lblSubjects";
            this.lblSubjects.Size = new System.Drawing.Size(162, 16);
            this.lblSubjects.TabIndex = 68;
            this.lblSubjects.Tag = "subjects";
            this.lblSubjects.Text = "subjects";
            this.lblSubjects.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdEditSubject
            // 
            this.cmdEditSubject.Location = new System.Drawing.Point(12, 216);
            this.cmdEditSubject.Name = "cmdEditSubject";
            this.cmdEditSubject.Size = new System.Drawing.Size(162, 23);
            this.cmdEditSubject.TabIndex = 62;
            this.cmdEditSubject.Tag = "edit_subject";
            this.cmdEditSubject.Text = "edit_subject";
            this.cmdEditSubject.UseVisualStyleBackColor = true;
            this.cmdEditSubject.Click += new System.EventHandler(this.cmdEditSubject_Click);
            // 
            // txtSubjectFolder
            // 
            this.txtSubjectFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSubjectFolder.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.txtSubjectFolder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSubjectFolder.Location = new System.Drawing.Point(15, 293);
            this.txtSubjectFolder.Name = "txtSubjectFolder";
            this.txtSubjectFolder.ReadOnly = true;
            this.txtSubjectFolder.Size = new System.Drawing.Size(478, 13);
            this.txtSubjectFolder.TabIndex = 56;
            this.txtSubjectFolder.TabStop = false;
            // 
            // lblCategoryFolderLabel
            // 
            this.lblCategoryFolderLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblCategoryFolderLabel.Location = new System.Drawing.Point(12, 272);
            this.lblCategoryFolderLabel.Name = "lblCategoryFolderLabel";
            this.lblCategoryFolderLabel.Size = new System.Drawing.Size(162, 18);
            this.lblCategoryFolderLabel.TabIndex = 58;
            this.lblCategoryFolderLabel.Tag = "subject_folder :";
            this.lblCategoryFolderLabel.Text = "subject_folder :";
            // 
            // cmdRemoveSubject
            // 
            this.cmdRemoveSubject.Location = new System.Drawing.Point(12, 245);
            this.cmdRemoveSubject.Name = "cmdRemoveSubject";
            this.cmdRemoveSubject.Size = new System.Drawing.Size(162, 23);
            this.cmdRemoveSubject.TabIndex = 61;
            this.cmdRemoveSubject.Tag = "remove_subject";
            this.cmdRemoveSubject.Text = "remove_subject";
            this.cmdRemoveSubject.UseVisualStyleBackColor = true;
            this.cmdRemoveSubject.Click += new System.EventHandler(this.cmdRemoveSubject_Click);
            // 
            // cmdCreateSubject
            // 
            this.cmdCreateSubject.Location = new System.Drawing.Point(12, 187);
            this.cmdCreateSubject.Name = "cmdCreateSubject";
            this.cmdCreateSubject.Size = new System.Drawing.Size(162, 23);
            this.cmdCreateSubject.TabIndex = 55;
            this.cmdCreateSubject.Tag = "create_subject";
            this.cmdCreateSubject.Text = "create_subject";
            this.cmdCreateSubject.UseVisualStyleBackColor = true;
            this.cmdCreateSubject.Click += new System.EventHandler(this.cmdCreateSubject_Click);
            // 
            // cmdRemoveCourse
            // 
            this.cmdRemoveCourse.Enabled = false;
            this.cmdRemoveCourse.Location = new System.Drawing.Point(365, 61);
            this.cmdRemoveCourse.Name = "cmdRemoveCourse";
            this.cmdRemoveCourse.Size = new System.Drawing.Size(39, 23);
            this.cmdRemoveCourse.TabIndex = 66;
            this.cmdRemoveCourse.Text = ">";
            this.cmdRemoveCourse.UseVisualStyleBackColor = true;
            this.cmdRemoveCourse.Click += new System.EventHandler(this.cmdRemoveCourse_Click);
            // 
            // cmdAddCourse
            // 
            this.cmdAddCourse.Enabled = false;
            this.cmdAddCourse.Location = new System.Drawing.Point(365, 31);
            this.cmdAddCourse.Name = "cmdAddCourse";
            this.cmdAddCourse.Size = new System.Drawing.Size(39, 23);
            this.cmdAddCourse.TabIndex = 65;
            this.cmdAddCourse.Text = "<";
            this.cmdAddCourse.UseVisualStyleBackColor = true;
            this.cmdAddCourse.Click += new System.EventHandler(this.cmdAddCourse_Click);
            // 
            // lstAvailableCourses
            // 
            this.lstAvailableCourses.Enabled = false;
            this.lstAvailableCourses.FormattingEnabled = true;
            this.lstAvailableCourses.HorizontalScrollbar = true;
            this.lstAvailableCourses.Location = new System.Drawing.Point(410, 31);
            this.lstAvailableCourses.Name = "lstAvailableCourses";
            this.lstAvailableCourses.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAvailableCourses.Size = new System.Drawing.Size(162, 238);
            this.lstAvailableCourses.TabIndex = 67;
            // 
            // lstSubject
            // 
            this.lstSubject.FormattingEnabled = true;
            this.lstSubject.HorizontalScrollbar = true;
            this.lstSubject.Location = new System.Drawing.Point(12, 31);
            this.lstSubject.Name = "lstSubject";
            this.lstSubject.Size = new System.Drawing.Size(162, 121);
            this.lstSubject.TabIndex = 63;
            this.lstSubject.SelectedIndexChanged += new System.EventHandler(this.lstSubject_SelectedIndexChanged);
            this.lstSubject.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSubject_MouseDoubleClick);
            // 
            // lstSelectedCourses
            // 
            this.lstSelectedCourses.Enabled = false;
            this.lstSelectedCourses.FormattingEnabled = true;
            this.lstSelectedCourses.HorizontalScrollbar = true;
            this.lstSelectedCourses.Location = new System.Drawing.Point(197, 31);
            this.lstSelectedCourses.Name = "lstSelectedCourses";
            this.lstSelectedCourses.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstSelectedCourses.Size = new System.Drawing.Size(162, 238);
            this.lstSelectedCourses.TabIndex = 64;
            // 
            // cmdClose
            // 
            this.cmdClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdClose.Location = new System.Drawing.Point(215, 327);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(162, 23);
            this.cmdClose.TabIndex = 71;
            this.cmdClose.Tag = "close_management";
            this.cmdClose.Text = "close_management";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // ManageSubjectsFrame
            // 
            this.AcceptButton = this.cmdOpenSubjectFolder;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdClose;
            this.ClientSize = new System.Drawing.Size(592, 361);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.cmdMoveSubjectDown);
            this.Controls.Add(this.cmdMoveSubjectUp);
            this.Controls.Add(this.cmdOpenSubjectFolder);
            this.Controls.Add(this.lblAvailableCourses);
            this.Controls.Add(this.lblSelectedCourses);
            this.Controls.Add(this.lblSubjects);
            this.Controls.Add(this.cmdEditSubject);
            this.Controls.Add(this.txtSubjectFolder);
            this.Controls.Add(this.lblCategoryFolderLabel);
            this.Controls.Add(this.cmdRemoveSubject);
            this.Controls.Add(this.cmdCreateSubject);
            this.Controls.Add(this.cmdRemoveCourse);
            this.Controls.Add(this.cmdAddCourse);
            this.Controls.Add(this.lstAvailableCourses);
            this.Controls.Add(this.lstSubject);
            this.Controls.Add(this.lstSelectedCourses);
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MinimumSize = new System.Drawing.Size(600, 395);
            this.Name = "ManageSubjectsFrame";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "subjects_management";
            this.Text = "subjects_management";
            this.Load += new System.EventHandler(this.ManageSubjectsFrame_Load);
            this.Resize += new System.EventHandler(this.ManageSubjectsFrame_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cmdMoveSubjectDown;
        private System.Windows.Forms.Button cmdMoveSubjectUp;
        private System.Windows.Forms.Button cmdOpenSubjectFolder;
        private System.Windows.Forms.Label lblAvailableCourses;
        private System.Windows.Forms.Label lblSelectedCourses;
        private System.Windows.Forms.Label lblSubjects;
        private System.Windows.Forms.Button cmdEditSubject;
        private System.Windows.Forms.TextBox txtSubjectFolder;
        private System.Windows.Forms.Label lblCategoryFolderLabel;
        private System.Windows.Forms.Button cmdRemoveSubject;
        private System.Windows.Forms.Button cmdCreateSubject;
        private System.Windows.Forms.Button cmdRemoveCourse;
        private System.Windows.Forms.Button cmdAddCourse;
        private System.Windows.Forms.ListBox lstAvailableCourses;
        private System.Windows.Forms.ListBox lstSubject;
        private System.Windows.Forms.ListBox lstSelectedCourses;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.ToolTip toolTip;

    }
}