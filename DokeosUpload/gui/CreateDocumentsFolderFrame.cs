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
using System.Windows.Forms;
using lmsda.domain;
using System.Threading;
using lmsda.domain.user;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    partial class CreateDocumentsFolderFrame : Form
    {
        private DomainController domainController;

        public delegate void invoke_delegate();
        public delegate void invoke_delegate_set_control_value(Control c, object value);

        #region Start methodes

        public CreateDocumentsFolderFrame()
        {
            this.domainController = DomainController.Instance();
            InitializeComponent();
        }

        private void CreateDocumentsFolderFrame_Load(object sender, EventArgs e)
        {
            StringsResetter.resetStaticStrings(this);
        }

        private void CreateDocumentsFolderFrame_Shown(object sender, EventArgs e)
        {
            this.cmbSaveToSubDir.DataSource = this.domainController.getDocumentFoldersForDropDown();
            if(this.cmbSaveToSubDir.Items.Count != 0)
                this.cmbSaveToSubDir.SelectedItem = this.domainController.currentFolder.folderName;
            else
                this.cmdOk.Enabled = false;
        }

        #endregion

        #region Control methods

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (this.txtNewFolderName.Text.Equals(String.Empty))
            {
                this.domainController.writeToLog(this.domainController.getLanguageString("error") + ": " + this.domainController.getLanguageString("please_fill_out_the_form_correctly"));
                MessageBox.Show(this.domainController.getLanguageString("please_fill_out_the_form_correctly"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.txtNewFolderName.Focus();
            }
            else
            {
                this.progressBar.Visible = true;
                this.progressBar.MarqueeAnimationSpeed = 100;
                this.setComponentsEnabled(false);
                DocumentFolder destination = this.domainController.getUserInfo().getSelectedItemDocumentFolders().getDocumentFolderFromFolderName(
                    this.cmbSaveToSubDir.SelectedItem.ToString());

                String newFullName = destination.folderName + "/" + this.txtNewFolderName.Text;
                newFullName= this.domainController.getPlatform().getFormattedFilenameString("/" + newFullName.Trim('/'));
                // only create if it doesn't already exist
                DocumentFolder df2 = domainController.getUserInfo().getSelectedItemDocumentFolders().
                    getDocumentFolderFromFolderName(newFullName);
                if (df2 != null)
                {
                    this.progressBar.MarqueeAnimationSpeed = 0;
                    this.domainController.currentFolder = df2;
                    this.domainController.writeToLog("create_folder_x_failed_already_exists", new String[] { this.txtNewFolderName.Text }, true, false, true);
                    this.Close();
                }
                else
                {
                    Object[] para = new Object[]{destination, this.txtNewFolderName.Text, false};
                    Thread thread = new Thread(new ParameterizedThreadStart(doCreateNewFolder));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start(para);
                }
            }
        }

        #endregion

        #region Action methods

        private void doCreateNewFolder(object o)
        {
            Object[] parameters = (Object[])o;
            DocumentFolder target = (DocumentFolder)parameters[0];
            String newName = (String)parameters[1];
            Boolean setInvisible = (Boolean)parameters[2];
            DocumentFolder df = this.domainController.createNewDocumentsFolder(target, newName, setInvisible);
            String targetName;
            if (df!=null)
                targetName=df.folderName;
            else
                // if the error is just that the folder couldn't be created because it was created on the platform
                // after the folder list was fetched, ignore error and return success.
                targetName =  this.domainController.getPlatform().getFormattedFilenameString(target.folderName + "/" + newName);

            // Makes sure the currentFolder is set correctly after a partial creation: if the folder already
            // existed in certain courses of a coursesCategory, the new DocumentFolder's folders list will only
            // contain the DocumentFolderForCourse objects for courses where the new folders were created.
            // This refreshes it from the freshly reloaded full document folders list.

            domainController.loadDocumentFolders(false);
            DocumentFolder df2 = this.domainController.getUserInfo().getSelectedItemDocumentFolders().
                getDocumentFolderFromFolderName(targetName);

            if (df2 != null)
            {
                this.domainController.currentFolder = (df2);
                this.domainController.writeToLog("create_folder_x_success", new String[] { newName }, true, false, false);
            }
            else
            {
                this.progressBar.Invoke(new invoke_delegate(this.stopAnimation));
                this.domainController.writeToLog("create_folder_x_failed", new String[] { newName }, true, false, true);
            }

            this.Invoke(new invoke_delegate(this.Close));
        }

        private void setComponentsEnabled(Boolean b)
        {
            this.txtNewFolderName.Enabled = b;
            this.cmbSaveToSubDir.Enabled = b;
            this.cmdOk.Enabled = b;
            this.cmdCancel.Enabled = b;
        }

        private void stopAnimation()
        {
            this.progressBar.MarqueeAnimationSpeed = 0;
        }
    
        #endregion

    }
}
