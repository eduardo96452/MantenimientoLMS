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

namespace lmsda.gui.subject
{
    partial class FileOptionsExercisePanel : UserControl
    {
        private Boolean loading = false;
        private String relativeFileName = String.Empty;
        private SubjectFilesSettingsControl parent;

        public void setParent(SubjectFilesSettingsControl parent)
        {
            this.parent = parent;
        }
        
        public FileOptionsExercisePanel()
        {
            InitializeComponent();
        }

        private void chkOneQuestionPerPage_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.oneQuestionPerPage = this.chkOneQuestionPerPage.Checked;
                if (!fs.hasError && fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        private void chkSetExerciseInvisible_CheckedChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.setExerciseInvisible = this.chkSetExerciseInvisible.Checked;
                if (!fs.hasError && fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }


        private void chkRandomQuestions_CheckedChanged(object sender, EventArgs e)
        {
            this.nrRandomQuestions.Visible = this.chkRandomQuestions.Checked;

            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                if (!this.chkRandomQuestions.Checked)
                    fs.randomQuestions = 0;
                else
                    fs.randomQuestions = Convert.ToInt32(this.nrRandomQuestions.Value);
                if (!fs.hasError && fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        private void nrRandomQuestions_ValueChanged(object sender, EventArgs e)
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                fs.randomQuestions = Convert.ToInt32(nrRandomQuestions.Value);
                if (!fs.hasError && fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                this.parent.updateNodeImage(fs);
            }
        }

        public void update(FileSettings selectedFile)
        {
            this.relativeFileName = selectedFile.relativeFileName;
            this.loading = true;
            this.chkOneQuestionPerPage.Checked = selectedFile.oneQuestionPerPage;
            this.chkSetExerciseInvisible.Checked = selectedFile.setExerciseInvisible;
            this.chkRandomQuestions.Checked = selectedFile.randomQuestions > 0;
            if (selectedFile.randomQuestions > 0)
                nrRandomQuestions.Value = selectedFile.randomQuestions;
            else
                nrRandomQuestions.Value = 1;
            this.loading = false;
        }
        
    }
}
