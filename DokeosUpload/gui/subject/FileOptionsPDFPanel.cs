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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lmsda.domain.user.synchronization;
using lmsda.domain;
using lmsda.gui.treeview;

namespace lmsda.gui.subject
{
    partial class FileOptionsPDFPanel : UserControl
    {
        private Boolean loading = false;
        private String relativeFileName = String.Empty;
        private SubjectFilesSettingsControl parent;

        public FileOptionsPDFPanel()
        {
            InitializeComponent();
        }

        private void FileOptionsPDFPanel_Load(object sender, EventArgs e)
        {
            resetStrings();
        }

        public void setParent(SubjectFilesSettingsControl parent)
        {
            this.parent = parent;
        }

        private void chkSplit_CheckedChanged(object sender, EventArgs e)
        {
            this.txtSplit.Enabled = this.chkSplit.Checked;
            this.cmbSplitTemplates.Enabled = this.chkSplit.Checked;
            this.chkSplitPerPage.Enabled = this.chkSplit.Checked;

            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                if (fs != null)
                {
                    fs.splitOnStyle = this.chkSplit.Checked;

                    if (this.chkSplit.Checked)
                    {
                        if (fs.splitString.Equals(String.Empty))
                        {
                            loading = true;
                            this.txtSplit.Text = DomainController.Instance().getLanguageString("pdf_split_default");
                            fs.splitString = this.txtSplit.Text;
                            loading = false;
                        }

                        if (fs.pdfNameTemplate.Equals(String.Empty) && this.cmbSplitTemplates.Items.Count > 0)
                        {
                            fs.pdfNameTemplate = DomainController.Instance().getSettings().getPDFFileName();
                            this.cmbSplitTemplates.SelectedIndex = 0;
                            fs.pdfNameTemplate = this.cmbSplitTemplates.Text;
                        }
                    }

                    if (fs.lastHash != String.Empty)
                        fs.optionsChanged = true;
                    fs.lastHash = String.Empty;
                    this.parent.updateNodeImage(fs);
                }
            }

        }

        private void txtSplit_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.splitString = this.txtSplit.Text;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                fs.lastHash = String.Empty;
                this.parent.updateNodeImage(fs);
            }
        }

        private void cmbSplitTemplates_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.pdfNameTemplate = this.cmbSplitTemplates.Text;
                if (!fs.lastHash.Equals(String.Empty))
                    fs.optionsChanged = true;
                fs.lastHash = String.Empty;
                this.parent.updateNodeImage(fs);
            }
        }

        private void chkSplitPerPage_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.splitPerPage = this.chkSplitPerPage.Checked;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                fs.lastHash = String.Empty;
                this.parent.updateNodeImage(fs);
            }
        }

        private void chkConvertHyperlinksToJavascript_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.convertToJavascript = this.chkConvertHyperlinksToJavascript.Checked;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                fs.lastHash = String.Empty;
                this.parent.updateNodeImage(fs);
            }
        }

        private void chkSetUploadedInvisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.setPDFInvisible = this.chkSetUploadedInvisible.Checked;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        public void resetStrings()
        {
            toolTip.SetToolTip(this.chkSplit, DomainController.Instance().getLanguageString("split_info"));
            toolTip.SetToolTip(this.txtSplit, DomainController.Instance().getLanguageString("split_info"));
            toolTip.SetToolTip(this.chkConvertHyperlinksToJavascript,
                                                        DomainController.Instance().getLanguageString("convert_hyperlinks_to_javascript"));
        }

        public void update(FileSettings selectedFile)
        {
            this.relativeFileName = selectedFile.relativeFileName;
            this.loading = true;
            chkSplit.Checked = selectedFile.splitOnStyle;
            this.chkSplitPerPage.Checked = selectedFile.splitPerPage;
            this.chkConvertHyperlinksToJavascript.Checked = selectedFile.convertToJavascript;
            this.chkSetUploadedInvisible.Checked = selectedFile.setPDFInvisible;
            this.txtSplit.Text = selectedFile.splitString;
            List<String> pdfMaskChoices = new List<String>(ProgramConstants.PDF_MASKS_LIST);
            int index = 0;
            if (!selectedFile.pdfNameTemplate.Equals(String.Empty))
            {
                if (pdfMaskChoices.Contains(selectedFile.pdfNameTemplate))
                    index = pdfMaskChoices.IndexOf(selectedFile.pdfNameTemplate);
                else
                    pdfMaskChoices.Insert(0, selectedFile.pdfNameTemplate);
            }
            this.cmbSplitTemplates.DataSource = pdfMaskChoices;
            this.cmbSplitTemplates.SelectedIndex = index;

            if (!selectedFile.splitString.Equals(String.Empty))
                this.txtSplit.Text = selectedFile.splitString;
            else
                this.txtSplit.Text = DomainController.Instance().getLanguageString("pdf_split_default");
            
            this.loading = false;
        }
    }
}
