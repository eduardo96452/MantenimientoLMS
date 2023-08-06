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
    partial class UploadFileFrame
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
            this.chkSetFileInvisible = new System.Windows.Forms.CheckBox();
            this.lblSourceFileForFileUpload = new System.Windows.Forms.TextBox();
            this.lblFileComment = new System.Windows.Forms.Label();
            this.txtUploadDescription = new System.Windows.Forms.TextBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.progressBarFileUpload = new System.Windows.Forms.ProgressBar();
            this.cmdBrowserForFileUpload = new System.Windows.Forms.Button();
            this.lblChooseSourceFile = new System.Windows.Forms.Label();
            this.lblChooseDestionation = new System.Windows.Forms.Label();
            this.cmdUploadFile = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.documentsDropDownForFileUpload = new lmsda.gui.DocumentsDropDown();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chkSetFileInvisible
            // 
            this.chkSetFileInvisible.AutoSize = true;
            this.chkSetFileInvisible.Location = new System.Drawing.Point(15, 80);
            this.chkSetFileInvisible.Name = "chkSetFileInvisible";
            this.chkSetFileInvisible.Size = new System.Drawing.Size(102, 17);
            this.chkSetFileInvisible.TabIndex = 64;
            this.chkSetFileInvisible.Tag = "set_file_invisible";
            this.chkSetFileInvisible.Text = "set_file_invisible";
            this.chkSetFileInvisible.UseVisualStyleBackColor = true;
            // 
            // lblSourceFileForFileUpload
            // 
            this.lblSourceFileForFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSourceFileForFileUpload.BackColor = System.Drawing.SystemColors.Control;
            this.lblSourceFileForFileUpload.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblSourceFileForFileUpload.Location = new System.Drawing.Point(169, 9);
            this.lblSourceFileForFileUpload.Name = "lblSourceFileForFileUpload";
            this.lblSourceFileForFileUpload.ReadOnly = true;
            this.lblSourceFileForFileUpload.Size = new System.Drawing.Size(409, 13);
            this.lblSourceFileForFileUpload.TabIndex = 63;
            this.lblSourceFileForFileUpload.TabStop = false;
            this.lblSourceFileForFileUpload.Tag = "no_source_selected";
            this.lblSourceFileForFileUpload.Text = "no_source_selected";
            // 
            // lblFileComment
            // 
            this.lblFileComment.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblFileComment.Location = new System.Drawing.Point(12, 114);
            this.lblFileComment.Name = "lblFileComment";
            this.lblFileComment.Size = new System.Drawing.Size(145, 19);
            this.lblFileComment.TabIndex = 62;
            this.lblFileComment.Tag = "file_upload_description :";
            this.lblFileComment.Text = "file_upload_description :";
            // 
            // txtUploadDescription
            // 
            this.txtUploadDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUploadDescription.Location = new System.Drawing.Point(172, 114);
            this.txtUploadDescription.Multiline = true;
            this.txtUploadDescription.Name = "txtUploadDescription";
            this.txtUploadDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtUploadDescription.Size = new System.Drawing.Size(572, 72);
            this.txtUploadDescription.TabIndex = 58;
            // 
            // lblProgress
            // 
            this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblProgress.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProgress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblProgress.Location = new System.Drawing.Point(12, 245);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Size = new System.Drawing.Size(732, 19);
            this.lblProgress.TabIndex = 61;
            this.lblProgress.Text = "%";
            this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblProgress.Visible = false;
            // 
            // progressBarFileUpload
            // 
            this.progressBarFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarFileUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarFileUpload.Location = new System.Drawing.Point(15, 221);
            this.progressBarFileUpload.Name = "progressBarFileUpload";
            this.progressBarFileUpload.Size = new System.Drawing.Size(729, 11);
            this.progressBarFileUpload.TabIndex = 60;
            this.progressBarFileUpload.Visible = false;
            // 
            // cmdBrowserForFileUpload
            // 
            this.cmdBrowserForFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowserForFileUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdBrowserForFileUpload.Location = new System.Drawing.Point(584, 4);
            this.cmdBrowserForFileUpload.Name = "cmdBrowserForFileUpload";
            this.cmdBrowserForFileUpload.Size = new System.Drawing.Size(162, 23);
            this.cmdBrowserForFileUpload.TabIndex = 56;
            this.cmdBrowserForFileUpload.Tag = "browse";
            this.cmdBrowserForFileUpload.Text = "browse";
            this.cmdBrowserForFileUpload.UseVisualStyleBackColor = true;
            this.cmdBrowserForFileUpload.Click += new System.EventHandler(this.cmdBrowserForFileUpload_Click);
            // 
            // lblChooseSourceFile
            // 
            this.lblChooseSourceFile.AutoSize = true;
            this.lblChooseSourceFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblChooseSourceFile.Location = new System.Drawing.Point(12, 9);
            this.lblChooseSourceFile.Name = "lblChooseSourceFile";
            this.lblChooseSourceFile.Size = new System.Drawing.Size(86, 13);
            this.lblChooseSourceFile.TabIndex = 55;
            this.lblChooseSourceFile.Tag = "choose_source :";
            this.lblChooseSourceFile.Text = "choose_source :";
            // 
            // lblChooseDestionation
            // 
            this.lblChooseDestionation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblChooseDestionation.Location = new System.Drawing.Point(12, 45);
            this.lblChooseDestionation.Name = "lblChooseDestionation";
            this.lblChooseDestionation.Size = new System.Drawing.Size(145, 19);
            this.lblChooseDestionation.TabIndex = 54;
            this.lblChooseDestionation.Tag = "choose_destination :";
            this.lblChooseDestionation.Text = "choose_destination :";
            // 
            // cmdUploadFile
            // 
            this.cmdUploadFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdUploadFile.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdUploadFile.Location = new System.Drawing.Point(272, 267);
            this.cmdUploadFile.Name = "cmdUploadFile";
            this.cmdUploadFile.Size = new System.Drawing.Size(212, 23);
            this.cmdUploadFile.TabIndex = 59;
            this.cmdUploadFile.Tag = "upload_file";
            this.cmdUploadFile.Text = "upload_file";
            this.cmdUploadFile.UseVisualStyleBackColor = true;
            this.cmdUploadFile.Click += new System.EventHandler(this.cmdUploadFile_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Word 2007 files|*.docx";
            // 
            // documentsDropDownForFileUpload
            // 
            this.documentsDropDownForFileUpload.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.documentsDropDownForFileUpload.Location = new System.Drawing.Point(169, 40);
            this.documentsDropDownForFileUpload.Name = "documentsDropDownForFileUpload";
            this.documentsDropDownForFileUpload.Size = new System.Drawing.Size(580, 28);
            this.documentsDropDownForFileUpload.TabIndex = 57;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(631, 266);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(110, 23);
            this.btnClose.TabIndex = 65;
            this.btnClose.Tag = "close";
            this.btnClose.Text = "close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // UploadFileFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(753, 301);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkSetFileInvisible);
            this.Controls.Add(this.lblSourceFileForFileUpload);
            this.Controls.Add(this.lblFileComment);
            this.Controls.Add(this.txtUploadDescription);
            this.Controls.Add(this.lblProgress);
            this.Controls.Add(this.progressBarFileUpload);
            this.Controls.Add(this.cmdBrowserForFileUpload);
            this.Controls.Add(this.lblChooseSourceFile);
            this.Controls.Add(this.lblChooseDestionation);
            this.Controls.Add(this.cmdUploadFile);
            this.Controls.Add(this.documentsDropDownForFileUpload);
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.Name = "UploadFileFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "upload_files";
            this.Text = "upload_files";
            this.Load += new System.EventHandler(this.UploadFileFrame_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkSetFileInvisible;
        private System.Windows.Forms.TextBox lblSourceFileForFileUpload;
        private System.Windows.Forms.Label lblFileComment;
        private System.Windows.Forms.TextBox txtUploadDescription;
        private System.Windows.Forms.Label lblProgress;
        private System.Windows.Forms.ProgressBar progressBarFileUpload;
        private System.Windows.Forms.Button cmdBrowserForFileUpload;
        private System.Windows.Forms.Label lblChooseSourceFile;
        private System.Windows.Forms.Label lblChooseDestionation;
        private System.Windows.Forms.Button cmdUploadFile;
        private DocumentsDropDown documentsDropDownForFileUpload;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnClose;
    }
}