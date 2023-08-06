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
    partial class LogViewerFrame
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
            this.txtLog = new System.Windows.Forms.TextBox();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.chkAdvancedView = new System.Windows.Forms.CheckBox();
            this.cmdOpenLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.White;
            this.txtLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(12, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(586, 392);
            this.txtLog.TabIndex = 0;
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(473, 410);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(125, 23);
            this.cmdCancel.TabIndex = 1;
            this.cmdCancel.Tag = "cancel";
            this.cmdCancel.Text = "cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // chkAdvancedView
            // 
            this.chkAdvancedView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkAdvancedView.AutoSize = true;
            this.chkAdvancedView.Location = new System.Drawing.Point(12, 411);
            this.chkAdvancedView.Name = "chkAdvancedView";
            this.chkAdvancedView.Size = new System.Drawing.Size(102, 17);
            this.chkAdvancedView.TabIndex = 2;
            this.chkAdvancedView.Tag = "advanced_view";
            this.chkAdvancedView.Text = "advanced_view";
            this.chkAdvancedView.UseVisualStyleBackColor = true;
            this.chkAdvancedView.CheckedChanged += new System.EventHandler(this.chkAdvancedView_CheckedChanged);
            // 
            // cmdOpenLog
            // 
            this.cmdOpenLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOpenLog.Location = new System.Drawing.Point(342, 410);
            this.cmdOpenLog.Name = "cmdOpenLog";
            this.cmdOpenLog.Size = new System.Drawing.Size(125, 23);
            this.cmdOpenLog.TabIndex = 3;
            this.cmdOpenLog.Tag = "open_log";
            this.cmdOpenLog.Text = "open_log";
            this.cmdOpenLog.UseVisualStyleBackColor = true;
            this.cmdOpenLog.Click += new System.EventHandler(this.cmdOpenLog_Click);
            // 
            // LogViewerFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(610, 445);
            this.Controls.Add(this.cmdOpenLog);
            this.Controls.Add(this.chkAdvancedView);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.txtLog);
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.Name = "LogViewerFrame";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "log_viewer";
            this.Text = "log_viewer";
            this.Shown += new System.EventHandler(this.LogViewerFrame_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.CheckBox chkAdvancedView;
        private System.Windows.Forms.Button cmdOpenLog;
    }
}