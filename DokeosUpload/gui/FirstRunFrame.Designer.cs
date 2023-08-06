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
    partial class FirstRunFrame
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
            this.cmbResources = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.chkCreateMySubjectsStats = new System.Windows.Forms.CheckBox();
            this.cmdOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmbResources
            // 
            this.cmbResources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResources.FormattingEnabled = true;
            this.cmbResources.Location = new System.Drawing.Point(15, 25);
            this.cmbResources.Name = "cmbResources";
            this.cmbResources.Size = new System.Drawing.Size(306, 21);
            this.cmbResources.TabIndex = 4;
            this.cmbResources.SelectedIndexChanged += new System.EventHandler(this.cmbResources_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(12, 9);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(92, 13);
            this.lblLanguage.TabIndex = 3;
            this.lblLanguage.Tag = "set_language_to :";
            this.lblLanguage.Text = "set_language_to :";
            // 
            // chkCreateMySubjectsStats
            // 
            this.chkCreateMySubjectsStats.Checked = true;
            this.chkCreateMySubjectsStats.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateMySubjectsStats.Location = new System.Drawing.Point(15, 71);
            this.chkCreateMySubjectsStats.Name = "chkCreateMySubjectsStats";
            this.chkCreateMySubjectsStats.Size = new System.Drawing.Size(306, 51);
            this.chkCreateMySubjectsStats.TabIndex = 6;
            this.chkCreateMySubjectsStats.Tag = "create_mysubjects_stats";
            this.chkCreateMySubjectsStats.Text = "create_mysubjects_stats";
            this.chkCreateMySubjectsStats.UseVisualStyleBackColor = true;
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdOk.Location = new System.Drawing.Point(131, 128);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(75, 23);
            this.cmdOk.TabIndex = 7;
            this.cmdOk.Tag = "ok";
            this.cmdOk.Text = "ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // FirstRunFrame
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdOk;
            this.ClientSize = new System.Drawing.Size(338, 168);
            this.Controls.Add(this.cmdOk);
            this.Controls.Add(this.chkCreateMySubjectsStats);
            this.Controls.Add(this.cmbResources);
            this.Controls.Add(this.lblLanguage);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FirstRunFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Tag = "welcome_to_lmsda";
            this.Text = "welcome_to_lmsda";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FirstRunFrame_FormClosing);
            this.Load += new System.EventHandler(this.FirstRunFrame_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbResources;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.CheckBox chkCreateMySubjectsStats;
        private System.Windows.Forms.Button cmdOk;
    }
}