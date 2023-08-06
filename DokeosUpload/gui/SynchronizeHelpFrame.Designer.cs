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
    partial class SynchronizeHelpFrame
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
            this.lblIconAddedFile = new System.Windows.Forms.Label();
            this.lblIconDisabledFolder = new System.Windows.Forms.Label();
            this.lblTextDisabledFolder = new System.Windows.Forms.Label();
            this.lblIconSynchronized = new System.Windows.Forms.Label();
            this.lblIconChanged = new System.Windows.Forms.Label();
            this.lblIconDataError = new System.Windows.Forms.Label();
            this.lblIconFolderUpload = new System.Windows.Forms.Label();
            this.lblTextFolderUpload = new System.Windows.Forms.Label();
            this.lblTextDataError = new System.Windows.Forms.Label();
            this.lblTextChanged = new System.Windows.Forms.Label();
            this.lblTextSynchronized = new System.Windows.Forms.Label();
            this.lblTextAddedFile = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblIconDisabledFile = new System.Windows.Forms.Label();
            this.lblTextDisabledFile = new System.Windows.Forms.Label();
            this.lblTextEdited = new System.Windows.Forms.Label();
            this.lblIconEdited = new System.Windows.Forms.Label();
            this.lblTextCourseAdded = new System.Windows.Forms.Label();
            this.lblIconCourseAddedSynced = new System.Windows.Forms.Label();
            this.lblIconCourseAddedModified = new System.Windows.Forms.Label();
            this.lblAccoladeCourseAdded = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblIconAddedFile
            // 
            this.lblIconAddedFile.Image = global::lmsda.Properties.Resources.sync_added;
            this.lblIconAddedFile.Location = new System.Drawing.Point(12, 124);
            this.lblIconAddedFile.Name = "lblIconAddedFile";
            this.lblIconAddedFile.Size = new System.Drawing.Size(16, 16);
            this.lblIconAddedFile.TabIndex = 0;
            // 
            // lblIconDisabledFolder
            // 
            this.lblIconDisabledFolder.Image = global::lmsda.Properties.Resources.sync_excludedfolder;
            this.lblIconDisabledFolder.Location = new System.Drawing.Point(12, 9);
            this.lblIconDisabledFolder.Name = "lblIconDisabledFolder";
            this.lblIconDisabledFolder.Size = new System.Drawing.Size(16, 16);
            this.lblIconDisabledFolder.TabIndex = 1;
            // 
            // lblTextDisabledFolder
            // 
            this.lblTextDisabledFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextDisabledFolder.Location = new System.Drawing.Point(35, 9);
            this.lblTextDisabledFolder.Name = "lblTextDisabledFolder";
            this.lblTextDisabledFolder.Size = new System.Drawing.Size(545, 30);
            this.lblTextDisabledFolder.TabIndex = 2;
            this.lblTextDisabledFolder.Tag = "syncinfo_excludedfolder";
            this.lblTextDisabledFolder.Text = "syncinfo_excludedfolder";
            // 
            // lblIconSynchronized
            // 
            this.lblIconSynchronized.Image = global::lmsda.Properties.Resources.sync_done;
            this.lblIconSynchronized.Location = new System.Drawing.Point(12, 154);
            this.lblIconSynchronized.Name = "lblIconSynchronized";
            this.lblIconSynchronized.Size = new System.Drawing.Size(16, 16);
            this.lblIconSynchronized.TabIndex = 3;
            // 
            // lblIconChanged
            // 
            this.lblIconChanged.Image = global::lmsda.Properties.Resources.sync_changed;
            this.lblIconChanged.Location = new System.Drawing.Point(12, 184);
            this.lblIconChanged.Name = "lblIconChanged";
            this.lblIconChanged.Size = new System.Drawing.Size(16, 16);
            this.lblIconChanged.TabIndex = 4;
            // 
            // lblIconDataError
            // 
            this.lblIconDataError.Image = global::lmsda.Properties.Resources.sync_data_error;
            this.lblIconDataError.Location = new System.Drawing.Point(12, 214);
            this.lblIconDataError.Name = "lblIconDataError";
            this.lblIconDataError.Size = new System.Drawing.Size(16, 16);
            this.lblIconDataError.TabIndex = 5;
            // 
            // lblIconFolderUpload
            // 
            this.lblIconFolderUpload.Image = global::lmsda.Properties.Resources.sync_upload;
            this.lblIconFolderUpload.Location = new System.Drawing.Point(12, 39);
            this.lblIconFolderUpload.Name = "lblIconFolderUpload";
            this.lblIconFolderUpload.Size = new System.Drawing.Size(16, 16);
            this.lblIconFolderUpload.TabIndex = 7;
            // 
            // lblTextFolderUpload
            // 
            this.lblTextFolderUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextFolderUpload.Location = new System.Drawing.Point(35, 39);
            this.lblTextFolderUpload.Name = "lblTextFolderUpload";
            this.lblTextFolderUpload.Size = new System.Drawing.Size(545, 30);
            this.lblTextFolderUpload.TabIndex = 8;
            this.lblTextFolderUpload.Tag = "syncinfo_folder_upload";
            this.lblTextFolderUpload.Text = "syncinfo_folder_upload";
            // 
            // lblTextDataError
            // 
            this.lblTextDataError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextDataError.Location = new System.Drawing.Point(35, 214);
            this.lblTextDataError.Name = "lblTextDataError";
            this.lblTextDataError.Size = new System.Drawing.Size(545, 30);
            this.lblTextDataError.TabIndex = 9;
            this.lblTextDataError.Tag = "syncinfo_file_error";
            this.lblTextDataError.Text = "syncinfo_file_error";
            // 
            // lblTextChanged
            // 
            this.lblTextChanged.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextChanged.Location = new System.Drawing.Point(35, 184);
            this.lblTextChanged.Name = "lblTextChanged";
            this.lblTextChanged.Size = new System.Drawing.Size(545, 30);
            this.lblTextChanged.TabIndex = 10;
            this.lblTextChanged.Tag = "syncinfo_file_changed";
            this.lblTextChanged.Text = "syncinfo_file_changed";
            // 
            // lblTextSynchronized
            // 
            this.lblTextSynchronized.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextSynchronized.Location = new System.Drawing.Point(35, 154);
            this.lblTextSynchronized.Name = "lblTextSynchronized";
            this.lblTextSynchronized.Size = new System.Drawing.Size(545, 30);
            this.lblTextSynchronized.TabIndex = 11;
            this.lblTextSynchronized.Tag = "syncinfo_file_synchronized";
            this.lblTextSynchronized.Text = "syncinfo_file_synchronized";
            // 
            // lblTextAddedFile
            // 
            this.lblTextAddedFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextAddedFile.Location = new System.Drawing.Point(35, 124);
            this.lblTextAddedFile.Name = "lblTextAddedFile";
            this.lblTextAddedFile.Size = new System.Drawing.Size(545, 30);
            this.lblTextAddedFile.TabIndex = 12;
            this.lblTextAddedFile.Tag = "syncinfo_file_added";
            this.lblTextAddedFile.Text = "syncinfo_file_added";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(259, 351);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 14;
            this.btnClose.Tag = "close";
            this.btnClose.Text = "close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblIconDisabledFile
            // 
            this.lblIconDisabledFile.Image = global::lmsda.Properties.Resources.sync_excludedfile;
            this.lblIconDisabledFile.Location = new System.Drawing.Point(12, 89);
            this.lblIconDisabledFile.Name = "lblIconDisabledFile";
            this.lblIconDisabledFile.Size = new System.Drawing.Size(16, 16);
            this.lblIconDisabledFile.TabIndex = 1;
            // 
            // lblTextDisabledFile
            // 
            this.lblTextDisabledFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextDisabledFile.Location = new System.Drawing.Point(35, 89);
            this.lblTextDisabledFile.Name = "lblTextDisabledFile";
            this.lblTextDisabledFile.Size = new System.Drawing.Size(545, 30);
            this.lblTextDisabledFile.TabIndex = 2;
            this.lblTextDisabledFile.Tag = "syncinfo_excludedfile";
            this.lblTextDisabledFile.Text = "syncinfo_excludedfile";
            // 
            // lblTextEdited
            // 
            this.lblTextEdited.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextEdited.Location = new System.Drawing.Point(35, 298);
            this.lblTextEdited.Name = "lblTextEdited";
            this.lblTextEdited.Size = new System.Drawing.Size(545, 50);
            this.lblTextEdited.TabIndex = 16;
            this.lblTextEdited.Tag = "syncinfo_edited";
            this.lblTextEdited.Text = "syncinfo_edited";
            // 
            // lblIconEdited
            // 
            this.lblIconEdited.Image = global::lmsda.Properties.Resources.sync_edited;
            this.lblIconEdited.Location = new System.Drawing.Point(12, 298);
            this.lblIconEdited.Name = "lblIconEdited";
            this.lblIconEdited.Size = new System.Drawing.Size(16, 16);
            this.lblIconEdited.TabIndex = 15;
            // 
            // lblTextCourseAdded
            // 
            this.lblTextCourseAdded.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTextCourseAdded.Location = new System.Drawing.Point(42, 248);
            this.lblTextCourseAdded.Name = "lblTextCourseAdded";
            this.lblTextCourseAdded.Size = new System.Drawing.Size(538, 42);
            this.lblTextCourseAdded.TabIndex = 17;
            this.lblTextCourseAdded.Tag = "syncinfo_file_course_added";
            this.lblTextCourseAdded.Text = "syncinfo_file_course_added";
            this.lblTextCourseAdded.Visible = false;
            // 
            // lblIconCourseAddedSynced
            // 
            this.lblIconCourseAddedSynced.Image = global::lmsda.Properties.Resources.sync_added_small;
            this.lblIconCourseAddedSynced.Location = new System.Drawing.Point(12, 244);
            this.lblIconCourseAddedSynced.Name = "lblIconCourseAddedSynced";
            this.lblIconCourseAddedSynced.Size = new System.Drawing.Size(16, 16);
            this.lblIconCourseAddedSynced.TabIndex = 18;
            this.lblIconCourseAddedSynced.Visible = false;
            // 
            // lblIconCourseAddedModified
            // 
            this.lblIconCourseAddedModified.Image = global::lmsda.Properties.Resources.sync_added_small;
            this.lblIconCourseAddedModified.Location = new System.Drawing.Point(12, 260);
            this.lblIconCourseAddedModified.Name = "lblIconCourseAddedModified";
            this.lblIconCourseAddedModified.Size = new System.Drawing.Size(16, 16);
            this.lblIconCourseAddedModified.TabIndex = 18;
            this.lblIconCourseAddedModified.Visible = false;
            // 
            // lblAccoladeCourseAdded
            // 
            this.lblAccoladeCourseAdded.AutoSize = true;
            this.lblAccoladeCourseAdded.BackColor = System.Drawing.SystemColors.Control;
            this.lblAccoladeCourseAdded.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAccoladeCourseAdded.Location = new System.Drawing.Point(25, 242);
            this.lblAccoladeCourseAdded.Margin = new System.Windows.Forms.Padding(0);
            this.lblAccoladeCourseAdded.Name = "lblAccoladeCourseAdded";
            this.lblAccoladeCourseAdded.Size = new System.Drawing.Size(23, 31);
            this.lblAccoladeCourseAdded.TabIndex = 19;
            this.lblAccoladeCourseAdded.Text = "}";
            this.lblAccoladeCourseAdded.Visible = false;
            // 
            // SynchronizeHelpFrame
            // 
            this.AcceptButton = this.btnClose;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(592, 386);
            this.Controls.Add(this.lblIconCourseAddedModified);
            this.Controls.Add(this.lblIconCourseAddedSynced);
            this.Controls.Add(this.lblTextCourseAdded);
            this.Controls.Add(this.lblTextEdited);
            this.Controls.Add(this.lblIconEdited);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTextAddedFile);
            this.Controls.Add(this.lblTextSynchronized);
            this.Controls.Add(this.lblTextChanged);
            this.Controls.Add(this.lblTextDataError);
            this.Controls.Add(this.lblTextFolderUpload);
            this.Controls.Add(this.lblIconFolderUpload);
            this.Controls.Add(this.lblIconDataError);
            this.Controls.Add(this.lblIconChanged);
            this.Controls.Add(this.lblIconSynchronized);
            this.Controls.Add(this.lblTextDisabledFile);
            this.Controls.Add(this.lblTextDisabledFolder);
            this.Controls.Add(this.lblIconDisabledFile);
            this.Controls.Add(this.lblIconDisabledFolder);
            this.Controls.Add(this.lblIconAddedFile);
            this.Controls.Add(this.lblAccoladeCourseAdded);
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MaximumSize = new System.Drawing.Size(10000, 420);
            this.MinimumSize = new System.Drawing.Size(600, 420);
            this.Name = "SynchronizeHelpFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "legenda";
            this.Text = "legenda";
            this.Load += new System.EventHandler(this.SynchronizeHelpFrame_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIconAddedFile;
        private System.Windows.Forms.Label lblIconDisabledFolder;
        private System.Windows.Forms.Label lblTextDisabledFolder;
        private System.Windows.Forms.Label lblIconSynchronized;
        private System.Windows.Forms.Label lblIconChanged;
        private System.Windows.Forms.Label lblIconDataError;
        private System.Windows.Forms.Label lblIconFolderUpload;
        private System.Windows.Forms.Label lblTextFolderUpload;
        private System.Windows.Forms.Label lblTextDataError;
        private System.Windows.Forms.Label lblTextChanged;
        private System.Windows.Forms.Label lblTextSynchronized;
        private System.Windows.Forms.Label lblTextAddedFile;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblIconDisabledFile;
        private System.Windows.Forms.Label lblTextDisabledFile;
        private System.Windows.Forms.Label lblTextEdited;
        private System.Windows.Forms.Label lblIconEdited;
        private System.Windows.Forms.Label lblTextCourseAdded;
        private System.Windows.Forms.Label lblIconCourseAddedSynced;
        private System.Windows.Forms.Label lblIconCourseAddedModified;
        private System.Windows.Forms.Label lblAccoladeCourseAdded;
    }
}