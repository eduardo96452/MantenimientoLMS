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
using lmsda.domain.util;

namespace lmsda.gui.subject
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     As of 1.08
    /// </remarks>
    partial class FileOptionsPDFPanelForPowerPoint : UserControl
    {
        private Boolean loading = false;
        private String relativeFileName = String.Empty;
        private SubjectFilesSettingsControl parent;

        #region Load / unload

        public FileOptionsPDFPanelForPowerPoint()
        {
            InitializeComponent();
            this.loading = true;
            this.resetStrings();
            this.loading = false;
        }

        public void setParent(SubjectFilesSettingsControl parent)
        {
            this.parent = parent;
        }

        public void resetStrings()
        {
            this.loading = true;
            int publishmethod = this.cmbPublishMethod.SelectedIndex;
            String[] ds1 = new String[] { DomainController.Instance().getLanguageString("dias"), DomainController.Instance().getLanguageString("hand_outs"), DomainController.Instance().getLanguageString("notes_pages") };
            this.cmbPublishMethod.DataSource = ds1;
            this.cmbPublishMethod.SelectedIndex = publishmethod;

            int slidesperpage = this.cmbDiasPerPage.SelectedIndex;
            String[] ds2 = new String[] { "1", "2", "3", "4", "6", "9" };
            this.cmbDiasPerPage.DataSource = ds2;
            this.cmbDiasPerPage.SelectedIndex = slidesperpage;
            this.loading = false;
        }

        public void update(FileSettings selectedFile)
        {
            this.relativeFileName = selectedFile.relativeFileName;
            this.loading = true;

            switch (selectedFile.publishMethod)
            {
                case PresentationPublishTypes.SLIDES:
                default:
                    this.cmbPublishMethod.SelectedIndex = 0;
                    break;
                case PresentationPublishTypes.HAND_OUTS:
                    this.cmbPublishMethod.SelectedIndex = 1;
                    break;
                case PresentationPublishTypes.NOTES_PAGES:
                    this.cmbPublishMethod.SelectedIndex = 2;
                    break;
            }
            this.chkDiasInFrame.Checked = selectedFile.frameSlides;
            this.chkInclusiveHiddenDias.Checked = selectedFile.includeHiddenSlides;
            this.cmbDiasPerPage.SelectedItem = Convert.ToString(selectedFile.slidesPerPage);
            this.rdbHorizontal.Checked = selectedFile.horizontal;
            this.rdbVertical.Checked = !selectedFile.horizontal;

            //this.cmbPublishMethod_SelectedIndexChanged(null, null);

            this.loading = false;
        }

        #endregion

        #region User clicks

        private void cmbPublishMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            Boolean handOuts = this.cmbPublishMethod.SelectedItem != null && this.cmbPublishMethod.SelectedItem.Equals(DomainController.Instance().getLanguageString("hand_outs"));
            this.cmbDiasPerPage.Enabled = handOuts;
            this.rdbHorizontal.Enabled = handOuts;
            this.rdbVertical.Enabled = handOuts;
            this.label2.Enabled = handOuts;
            this.label3.Enabled = handOuts;

            //Save
            this.somethingHasChanged();
        }

        private void chkDiasInFrame_CheckedChanged(object sender, EventArgs e)
        {
            //Save
            this.somethingHasChanged();
        }

        private void chkInclusiveHiddenDias_CheckedChanged(object sender, EventArgs e)
        {
            //Save
            this.somethingHasChanged();
        }

        private void cmbDiasPerPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Save
            this.somethingHasChanged();
        }

        private void rdbHorizontal_CheckedChanged(object sender, EventArgs e)
        {
            //Save
            this.somethingHasChanged();
        }

        private void rdbVertical_CheckedChanged(object sender, EventArgs e)
        {
            //Save
            this.somethingHasChanged();
        }

        #endregion

        #region Private methods

        private void somethingHasChanged()
        {
            if (!loading)
            {
                FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);

                fs.frameSlides = this.chkDiasInFrame.Checked;
                fs.horizontal = this.rdbHorizontal.Checked;
                fs.slidesPerPage = Convert.ToInt32(this.cmbDiasPerPage.SelectedItem);
                fs.includeHiddenSlides = this.chkInclusiveHiddenDias.Checked;
                if (this.cmbPublishMethod.SelectedItem.Equals(DomainController.Instance().getLanguageString("hand_outs")))
                    fs.publishMethod = PresentationPublishTypes.HAND_OUTS;
                else if (this.cmbPublishMethod.SelectedItem.Equals(DomainController.Instance().getLanguageString("notes_pages")))
                    fs.publishMethod = PresentationPublishTypes.NOTES_PAGES;
                else
                    fs.publishMethod = PresentationPublishTypes.SLIDES;

                if (fs.lastHash != String.Empty)
                    fs.optionsChanged = true;
                fs.lastHash = String.Empty;
                this.parent.updateNodeImage(fs);
            }
        }

        #endregion

    }
}
