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
    partial class DocumentsDropDown
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
            this.cmdMakeFolder = new System.Windows.Forms.Button();
            this.cmbSaveToSubDir = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmdMakeFolder
            // 
            this.cmdMakeFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdMakeFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdMakeFolder.Location = new System.Drawing.Point(288, 2);
            this.cmdMakeFolder.Name = "cmdMakeFolder";
            this.cmdMakeFolder.Size = new System.Drawing.Size(103, 23);
            this.cmdMakeFolder.TabIndex = 1;
            this.cmdMakeFolder.Tag = "create_documents_folder";
            this.cmdMakeFolder.Text = "create_documents_folder";
            this.cmdMakeFolder.UseVisualStyleBackColor = true;
            this.cmdMakeFolder.Click += new System.EventHandler(this.cmdMakeFolder_Click);
            // 
            // cmbSaveToSubDir
            // 
            this.cmbSaveToSubDir.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSaveToSubDir.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSaveToSubDir.FormattingEnabled = true;
            this.cmbSaveToSubDir.Location = new System.Drawing.Point(3, 3);
            this.cmbSaveToSubDir.Name = "cmbSaveToSubDir";
            this.cmbSaveToSubDir.Size = new System.Drawing.Size(279, 21);
            this.cmbSaveToSubDir.TabIndex = 0;
            // 
            // DocumentsDropDown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmdMakeFolder);
            this.Controls.Add(this.cmbSaveToSubDir);
            this.Name = "DocumentsDropDown";
            this.Size = new System.Drawing.Size(394, 28);
            this.Load += new System.EventHandler(this.DocumentsDropDown_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdMakeFolder;
        private System.Windows.Forms.ComboBox cmbSaveToSubDir;
    }
}
