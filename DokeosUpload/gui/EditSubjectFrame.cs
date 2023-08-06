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
using System.Linq;
using System.Windows.Forms;
using lmsda.domain;
using lmsda.persistence.settings;
using lmsda.domain.user;
using System.IO;
using lmsda.domain.util;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    public partial class EditSubjectFrame : Form
    {
        private DomainController dc;
        private SubjectSettings subjects;
        private Subject editSubject;

        public EditSubjectFrame()
        {   
            InitializeComponent();
            dc = DomainController.Instance();
            this.Tag = "create_subject";
            subjects = dc.getUserInfo().getSubjects();
            editSubject = null;
            StringsResetter.resetStaticStrings(this);
        }

        public EditSubjectFrame(String subjectName)
        {   
            InitializeComponent();
            dc = DomainController.Instance();
            this.Tag = "edit_subject";
            subjects = dc.getUserInfo().getSubjects();
            editSubject = subjects.getSubject(subjectName);
            this.txtSubjectName.Text = editSubject.getSubjectName();
            this.txtSubjectFolder.Text = editSubject.getSubjectFolder();
            StringsResetter.resetStaticStrings(this);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }


        private void cmdOk_Click(object sender, EventArgs e)
        {
            String subjectName   = this.txtSubjectName.Text;
            String subjectFolder = this.txtSubjectFolder.Text;

            if (subjectName.Equals(String.Empty))
            {
                dc.fireMessageBox(dc.getLanguageString("subject_name_empty"), MessageBoxIcon.Exclamation);
                return;
            }
            if (editSubject == null) // add subject part
            {
                if (subjects.getSubject(subjectName) != null)
                {
                    dc.fireMessageBox(dc.getLanguageString("subject_name_already_exists"), MessageBoxIcon.Exclamation);
                    return;
                }
                if (subjectFolder.Equals(String.Empty))
                {
                    DialogResult result = MessageBox.Show(this, this.dc.getLanguageString("create_folder_based_on_subject_name"), this.dc.getLanguageString(Convert.ToString(this.Tag)), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        try
                        {
                            subjectFolder = Directory.CreateDirectory(this.dc.getSettings().getSubjectsFolder().TrimEnd('\\') + @"\" + subjectName).FullName;
                        }
                        catch
                        {
                            MessageBox.Show(this, this.dc.getLanguageString("creation_directory_failed"), this.dc.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                subjects.addSubject(subjectName, subjectFolder);
                this.Dispose();
            }
            else // edit subject part
            {
                if (!editSubject.getSubjectName().Equals(subjectName) && subjects.getSubject(subjectName) != null)
                {
                    dc.fireMessageBox(dc.getLanguageString("subject_name_already_exists"), MessageBoxIcon.Exclamation);
                    return;
                }
                editSubject.setSubjectName(subjectName);
                editSubject.setSubjectFolder(subjectFolder);
                subjects.saveSubjects();
                this.Dispose();
            }
        }

        private void cmdSubjectFolderBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            // start path in both cases (new subject / editing subject)
            folderBrowserDialog.SelectedPath = txtSubjectFolder.Text;
            if (!Utility.doesFolderExist(txtSubjectFolder.Text))
                folderBrowserDialog.SelectedPath = (editSubject==null ? dc.getSettings().getSubjectsFolder() : editSubject.getSubjectFolder());
            DialogResult dr = folderBrowserDialog.ShowDialog();

            if (!folderBrowserDialog.SelectedPath.Equals(String.Empty))
            {
                if (PortableIdentifier.Instance().isPortable && !folderBrowserDialog.SelectedPath.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                {
                    dc.fireMessageBox(dc.getLanguageString("portable_needs_local_folder"), MessageBoxIcon.Exclamation);
                    return;
                }

                // check if subject folder is already in use
                Subject usedSubject = subjects.getSubjectForFolder(folderBrowserDialog.SelectedPath);
                String usedSubjectName = (usedSubject == null || usedSubject.Equals(editSubject) ? null : usedSubject.getSubjectName());
                // if subject is already in use, retry until it isn't.
                while (dr != DialogResult.Cancel && usedSubjectName != null)
                {
                    dc.fireMessageBox(dc.getLanguageString("subject_folder_already_used", new String[] { usedSubjectName }), MessageBoxIcon.Exclamation);
                    dr = folderBrowserDialog.ShowDialog();
                    usedSubject = subjects.getSubjectForFolder(folderBrowserDialog.SelectedPath);
                    usedSubjectName = (usedSubject == null || usedSubject.Equals(editSubject) ? null : usedSubject.getSubjectName());
                }
                if (dr != DialogResult.Cancel)
                {
                    txtSubjectFolder.Text = folderBrowserDialog.SelectedPath.TrimEnd('\\');
                    if (txtSubjectName.Text.Equals(String.Empty) && txtSubjectFolder.Text.Contains('\\'))
                    {
                        txtSubjectName.Text = txtSubjectFolder.Text.Substring(txtSubjectFolder.Text.LastIndexOf('\\') + 1);
                    }
                }
            }
        }
    }
}
