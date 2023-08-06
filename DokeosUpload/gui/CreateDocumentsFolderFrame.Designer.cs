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
    partial class CreateDocumentsFolderFrame
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
            this.lblChooseFolder = new System.Windows.Forms.Label();
            this.cmbSaveToSubDir = new System.Windows.Forms.ComboBox();
            this.lblNewFolderName = new System.Windows.Forms.Label();
            this.txtNewFolderName = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.cmdOk = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // lblChooseFolder
            // 
            this.lblChooseFolder.AutoSize = true;
            this.lblChooseFolder.Location = new System.Drawing.Point(12, 13);
            this.lblChooseFolder.Name = "lblChooseFolder";
            this.lblChooseFolder.Size = new System.Drawing.Size(74, 13);
            this.lblChooseFolder.TabIndex = 0;
            this.lblChooseFolder.Tag = "choose_folder";
            this.lblChooseFolder.Text = "choose_folder";
            // 
            // cmbSaveToSubDir
            // 
            this.cmbSaveToSubDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSaveToSubDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSaveToSubDir.FormattingEnabled = true;
            this.cmbSaveToSubDir.Location = new System.Drawing.Point(157, 10);
            this.cmbSaveToSubDir.Name = "cmbSaveToSubDir";
            this.cmbSaveToSubDir.Size = new System.Drawing.Size(250, 21);
            this.cmbSaveToSubDir.TabIndex = 30;
            // 
            // lblNewFolderName
            // 
            this.lblNewFolderName.AutoSize = true;
            this.lblNewFolderName.Location = new System.Drawing.Point(12, 47);
            this.lblNewFolderName.Name = "lblNewFolderName";
            this.lblNewFolderName.Size = new System.Drawing.Size(91, 13);
            this.lblNewFolderName.TabIndex = 31;
            this.lblNewFolderName.Tag = "new_folder_name";
            this.lblNewFolderName.Text = "new_folder_name";
            // 
            // txtNewFolderName
            // 
            this.txtNewFolderName.Location = new System.Drawing.Point(157, 44);
            this.txtNewFolderName.Name = "txtNewFolderName";
            this.txtNewFolderName.Size = new System.Drawing.Size(250, 20);
            this.txtNewFolderName.TabIndex = 32;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(332, 70);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 34;
            this.cmdCancel.Tag = "cancel";
            this.cmdCancel.Text = "cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // cmdOk
            // 
            this.cmdOk.Location = new System.Drawing.Point(251, 70);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 33;
            this.cmdOk.Tag = "ok";
            this.cmdOk.Text = "ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 77);
            this.progressBar.MarqueeAnimationSpeed = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(230, 10);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.TabIndex = 35;
            this.progressBar.Visible = false;
            // 
            // CreateDocumentsFolderFrame
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(419, 105);
            this.ControlBox = false;
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.txtNewFolderName);
            this.Controls.Add(this.lblNewFolderName);
            this.Controls.Add(this.cmbSaveToSubDir);
            this.Controls.Add(this.lblChooseFolder);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateDocumentsFolderFrame";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "create_documents_folder";
            this.Text = "Create documents folder";
            this.Load += new System.EventHandler(this.CreateDocumentsFolderFrame_Load);
            this.Shown += new System.EventHandler(this.CreateDocumentsFolderFrame_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblChooseFolder;
        private System.Windows.Forms.ComboBox cmbSaveToSubDir;
        private System.Windows.Forms.Label lblNewFolderName;
        private System.Windows.Forms.TextBox txtNewFolderName;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.ProgressBar progressBar;
    }
}