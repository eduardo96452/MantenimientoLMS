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
    partial class FileOptionsPDFPanel
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
            this.components = new System.ComponentModel.Container();
            this.txtSplit = new System.Windows.Forms.TextBox();
            this.chkConvertHyperlinksToJavascript = new System.Windows.Forms.CheckBox();
            this.chkSplitPerPage = new System.Windows.Forms.CheckBox();
            this.chkSplit = new System.Windows.Forms.CheckBox();
            this.lblChildImage = new System.Windows.Forms.Label();
            this.chkSetUploadedInvisible = new System.Windows.Forms.CheckBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSplitTemplates = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtSplit
            // 
            this.txtSplit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSplit.Enabled = false;
            this.txtSplit.Location = new System.Drawing.Point(173, 1);
            this.txtSplit.Name = "txtSplit";
            this.txtSplit.Size = new System.Drawing.Size(244, 20);
            this.txtSplit.TabIndex = 36;
            this.txtSplit.Text = "Header 1";
            this.txtSplit.TextChanged += new System.EventHandler(this.txtSplit_TextChanged);
            // 
            // chkConvertHyperlinksToJavascript
            // 
            this.chkConvertHyperlinksToJavascript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkConvertHyperlinksToJavascript.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConvertHyperlinksToJavascript.Location = new System.Drawing.Point(3, 89);
            this.chkConvertHyperlinksToJavascript.Name = "chkConvertHyperlinksToJavascript";
            this.chkConvertHyperlinksToJavascript.Size = new System.Drawing.Size(414, 32);
            this.chkConvertHyperlinksToJavascript.TabIndex = 39;
            this.chkConvertHyperlinksToJavascript.Tag = "convert_hyperlinks_to_javascript";
            this.chkConvertHyperlinksToJavascript.Text = "convert_hyperlinks_to_javascript\r\n...";
            this.chkConvertHyperlinksToJavascript.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkConvertHyperlinksToJavascript.UseVisualStyleBackColor = true;
            this.chkConvertHyperlinksToJavascript.CheckedChanged += new System.EventHandler(this.chkConvertHyperlinksToJavascript_CheckedChanged);
            // 
            // chkSplitPerPage
            // 
            this.chkSplitPerPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSplitPerPage.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSplitPerPage.Enabled = false;
            this.chkSplitPerPage.Location = new System.Drawing.Point(41, 51);
            this.chkSplitPerPage.Name = "chkSplitPerPage";
            this.chkSplitPerPage.Size = new System.Drawing.Size(376, 32);
            this.chkSplitPerPage.TabIndex = 38;
            this.chkSplitPerPage.Tag = "split_per_page";
            this.chkSplitPerPage.Text = "split_per_page";
            this.chkSplitPerPage.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSplitPerPage.UseVisualStyleBackColor = true;
            this.chkSplitPerPage.CheckedChanged += new System.EventHandler(this.chkSplitPerPage_CheckedChanged);
            // 
            // chkSplit
            // 
            this.chkSplit.AutoSize = true;
            this.chkSplit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkSplit.Location = new System.Drawing.Point(3, 3);
            this.chkSplit.Name = "chkSplit";
            this.chkSplit.Size = new System.Drawing.Size(89, 17);
            this.chkSplit.TabIndex = 35;
            this.chkSplit.Tag = "split_on_style";
            this.chkSplit.Text = "split_on_style";
            this.chkSplit.UseVisualStyleBackColor = true;
            this.chkSplit.CheckedChanged += new System.EventHandler(this.chkSplit_CheckedChanged);
            // 
            // lblChildImage
            // 
            this.lblChildImage.AutoSize = true;
            this.lblChildImage.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblChildImage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblChildImage.Location = new System.Drawing.Point(22, 25);
            this.lblChildImage.Name = "lblChildImage";
            this.lblChildImage.Size = new System.Drawing.Size(10, 13);
            this.lblChildImage.TabIndex = 37;
            this.lblChildImage.Text = " ";
            // 
            // chkSetUploadedInvisible
            // 
            this.chkSetUploadedInvisible.AutoSize = true;
            this.chkSetUploadedInvisible.Location = new System.Drawing.Point(3, 127);
            this.chkSetUploadedInvisible.Name = "chkSetUploadedInvisible";
            this.chkSetUploadedInvisible.Size = new System.Drawing.Size(109, 17);
            this.chkSetUploadedInvisible.TabIndex = 40;
            this.chkSetUploadedInvisible.Tag = "set_pdfs_invisible";
            this.chkSetUploadedInvisible.Text = "set_pdfs_invisible";
            this.chkSetUploadedInvisible.UseVisualStyleBackColor = true;
            this.chkSetUploadedInvisible.CheckedChanged += new System.EventHandler(this.chkSetUploadedInvisible_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Image = global::lmsda.Properties.Resources.tree_corner;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(22, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 42;
            this.label1.Text = " ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 13);
            this.label2.TabIndex = 44;
            this.label2.Tag = "pdf_split_name_template :";
            this.label2.Text = "pdf_split_name_template :";
            // 
            // cmbSplitTemplates
            // 
            this.cmbSplitTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSplitTemplates.Enabled = false;
            this.cmbSplitTemplates.FormattingEnabled = true;
            this.cmbSplitTemplates.Location = new System.Drawing.Point(173, 22);
            this.cmbSplitTemplates.Name = "cmbSplitTemplates";
            this.cmbSplitTemplates.Size = new System.Drawing.Size(244, 21);
            this.cmbSplitTemplates.TabIndex = 37;
            this.cmbSplitTemplates.TextChanged += new System.EventHandler(this.cmbSplitTemplates_TextChanged);
            // 
            // FileOptionsPDFPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmbSplitTemplates);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.chkSetUploadedInvisible);
            this.Controls.Add(this.txtSplit);
            this.Controls.Add(this.chkConvertHyperlinksToJavascript);
            this.Controls.Add(this.chkSplitPerPage);
            this.Controls.Add(this.chkSplit);
            this.Controls.Add(this.lblChildImage);
            this.Name = "FileOptionsPDFPanel";
            this.Size = new System.Drawing.Size(420, 170);
            this.Load += new System.EventHandler(this.FileOptionsPDFPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSplit;
        private System.Windows.Forms.CheckBox chkConvertHyperlinksToJavascript;
        private System.Windows.Forms.CheckBox chkSplitPerPage;
        private System.Windows.Forms.CheckBox chkSplit;
        private System.Windows.Forms.Label lblChildImage;
        private System.Windows.Forms.CheckBox chkSetUploadedInvisible;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSplitTemplates;
    }
}
