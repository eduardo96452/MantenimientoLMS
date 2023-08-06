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
using lmsda.gui.treeview;
using lmsda.domain.user.synchronization;
using lmsda.domain;

namespace lmsda.gui.subject
{
    partial class FileOptionsUploadPanel : UserControl
    {
        private Boolean loading = false;
        private String relativeFileName = String.Empty;
        private SubjectFilesSettingsControl parent;

        public void setParent(SubjectFilesSettingsControl parent)
        {
            this.parent = parent;
        }

        public FileOptionsUploadPanel()
        {
            InitializeComponent();
        }

        private void chkSetFileInvisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.setFileInvisible = this.chkSetFileInvisible.Checked;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        private void txtUploadDescription_TextChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.fileDescription = this.txtUploadDescription.Text;
                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        public void update(FileSettings selectedFile)
        {
            this.relativeFileName = selectedFile.relativeFileName;
            this.loading = true;
            this.chkSetFileInvisible.Checked = selectedFile.setFileInvisible;
            this.chkSetFileInvisible.Enabled = !selectedFile.isDirectory;
            this.txtUploadDescription.Text = selectedFile.fileDescription;
            this.txtUploadDescription.Enabled = !selectedFile.isDirectory;
            this.loading = false;
        }

    }
}
