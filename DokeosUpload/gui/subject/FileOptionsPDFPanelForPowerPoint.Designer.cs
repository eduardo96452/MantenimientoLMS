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
    partial class FileOptionsPDFPanelForPowerPoint
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmbPublishMethod = new System.Windows.Forms.ComboBox();
            this.chkDiasInFrame = new System.Windows.Forms.CheckBox();
            this.chkInclusiveHiddenDias = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDiasPerPage = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.rdbHorizontal = new System.Windows.Forms.RadioButton();
            this.rdbVertical = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 13);
            this.label1.TabIndex = 0;
            this.label1.Tag = "what_do_you_want_to_publish :";
            this.label1.Text = "what_do_you_want_to_publish";
            // 
            // cmbPublishMethod
            // 
            this.cmbPublishMethod.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPublishMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPublishMethod.FormattingEnabled = true;
            this.cmbPublishMethod.Location = new System.Drawing.Point(190, 1);
            this.cmbPublishMethod.Name = "cmbPublishMethod";
            this.cmbPublishMethod.Size = new System.Drawing.Size(171, 21);
            this.cmbPublishMethod.TabIndex = 1;
            this.cmbPublishMethod.SelectedIndexChanged += new System.EventHandler(this.cmbPublishMethod_SelectedIndexChanged);
            // 
            // chkDiasInFrame
            // 
            this.chkDiasInFrame.AutoSize = true;
            this.chkDiasInFrame.Location = new System.Drawing.Point(7, 20);
            this.chkDiasInFrame.Name = "chkDiasInFrame";
            this.chkDiasInFrame.Size = new System.Drawing.Size(91, 17);
            this.chkDiasInFrame.TabIndex = 2;
            this.chkDiasInFrame.Tag = "dias_in_frame";
            this.chkDiasInFrame.Text = "dias_in_frame";
            this.chkDiasInFrame.UseVisualStyleBackColor = true;
            this.chkDiasInFrame.CheckedChanged += new System.EventHandler(this.chkDiasInFrame_CheckedChanged);
            // 
            // chkInclusiveHiddenDias
            // 
            this.chkInclusiveHiddenDias.AutoSize = true;
            this.chkInclusiveHiddenDias.Location = new System.Drawing.Point(7, 43);
            this.chkInclusiveHiddenDias.Name = "chkInclusiveHiddenDias";
            this.chkInclusiveHiddenDias.Size = new System.Drawing.Size(130, 17);
            this.chkInclusiveHiddenDias.TabIndex = 3;
            this.chkInclusiveHiddenDias.Tag = "inclusive_hidden_dias";
            this.chkInclusiveHiddenDias.Text = "inclusive_hidden_dias";
            this.chkInclusiveHiddenDias.UseVisualStyleBackColor = true;
            this.chkInclusiveHiddenDias.CheckedChanged += new System.EventHandler(this.chkInclusiveHiddenDias_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(7, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 4;
            this.label2.Tag = "dias_per_page :";
            this.label2.Text = "dias_per_page";
            // 
            // cmbDiasPerPage
            // 
            this.cmbDiasPerPage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDiasPerPage.Enabled = false;
            this.cmbDiasPerPage.FormattingEnabled = true;
            this.cmbDiasPerPage.Location = new System.Drawing.Point(110, 64);
            this.cmbDiasPerPage.Name = "cmbDiasPerPage";
            this.cmbDiasPerPage.Size = new System.Drawing.Size(58, 21);
            this.cmbDiasPerPage.TabIndex = 5;
            this.cmbDiasPerPage.SelectedIndexChanged += new System.EventHandler(this.cmbDiasPerPage_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(7, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 6;
            this.label3.Tag = "sequence :";
            this.label3.Text = "sequence";
            // 
            // rdbHorizontal
            // 
            this.rdbHorizontal.AutoSize = true;
            this.rdbHorizontal.Checked = true;
            this.rdbHorizontal.Enabled = false;
            this.rdbHorizontal.Location = new System.Drawing.Point(110, 92);
            this.rdbHorizontal.Name = "rdbHorizontal";
            this.rdbHorizontal.Size = new System.Drawing.Size(70, 17);
            this.rdbHorizontal.TabIndex = 7;
            this.rdbHorizontal.TabStop = true;
            this.rdbHorizontal.Tag = "horizontal";
            this.rdbHorizontal.Text = "horizontal";
            this.rdbHorizontal.UseVisualStyleBackColor = true;
            this.rdbHorizontal.CheckedChanged += new System.EventHandler(this.rdbHorizontal_CheckedChanged);
            // 
            // rdbVertical
            // 
            this.rdbVertical.AutoSize = true;
            this.rdbVertical.Enabled = false;
            this.rdbVertical.Location = new System.Drawing.Point(110, 115);
            this.rdbVertical.Name = "rdbVertical";
            this.rdbVertical.Size = new System.Drawing.Size(59, 17);
            this.rdbVertical.TabIndex = 8;
            this.rdbVertical.TabStop = true;
            this.rdbVertical.Tag = "vertical";
            this.rdbVertical.Text = "vertical";
            this.rdbVertical.UseVisualStyleBackColor = true;
            this.rdbVertical.CheckedChanged += new System.EventHandler(this.rdbVertical_CheckedChanged);
            // 
            // FileOptionsPDFPanelForPowerPoint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rdbVertical);
            this.Controls.Add(this.rdbHorizontal);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbDiasPerPage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkInclusiveHiddenDias);
            this.Controls.Add(this.chkDiasInFrame);
            this.Controls.Add(this.cmbPublishMethod);
            this.Controls.Add(this.label1);
            this.Name = "FileOptionsPDFPanelForPowerPoint";
            this.Size = new System.Drawing.Size(364, 229);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbPublishMethod;
        private System.Windows.Forms.CheckBox chkDiasInFrame;
        private System.Windows.Forms.CheckBox chkInclusiveHiddenDias;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDiasPerPage;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdbHorizontal;
        private System.Windows.Forms.RadioButton rdbVertical;
    }
}
