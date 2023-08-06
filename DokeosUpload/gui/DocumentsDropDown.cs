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

namespace lmsda.gui
{
    /// <summary>
    ///     Auteur: Gianni Van Hoecke.
    /// </summary>
    partial class DocumentsDropDown : UserControl
    {
        private DomainController domainController;

        #region Constructor en form/control methods

        public DocumentsDropDown()
        {
            this.domainController = DomainController.Instance();
            InitializeComponent();
        }

        private void DocumentsDropDown_Load(object sender, EventArgs e)
        {

        }

        #endregion

        #region GUI events

        private void cmdMakeFolder_Click(object sender, EventArgs e)
        {
            this.domainController.currentFolder = 
                this.domainController.getUserInfo().getSelectedItemDocumentFolders().getDocumentFolderFromFolderName(
                this.cmbSaveToSubDir.SelectedItem.ToString());
            new CreateDocumentsFolderFrame().ShowDialog();
            this.setDataSource(this.domainController.getDocumentFoldersForDropDown());
            this.setSelectedItem(this.domainController.currentFolder.folderName);
        }

        #endregion

        #region Getters en setters

        public void setCmbDocumentsEnabled(Boolean b)
        {
            this.cmbSaveToSubDir.Enabled = b;
        }

        public void setCmdMakeFolderEnabled(Boolean b)
        {
            this.cmdMakeFolder.Enabled = b;
        }

        public void setDataSource(String[] dataSource)
        {
            this.cmbSaveToSubDir.DataSource = dataSource;
        }

        public String getSelectedItem()
        {
            try
            {
                return this.cmbSaveToSubDir.SelectedItem.ToString();
            }
            catch
            {
                return String.Empty;
            }
        }

        public void setSelectedItem(String item)
        {
            this.cmbSaveToSubDir.SelectedItem = item;
        }

        public ComboBox getComboBox()
        {
            return this.cmbSaveToSubDir;
        }

        public Button getButton()
        {
            return this.cmdMakeFolder;
        }

        #endregion
    }
}
