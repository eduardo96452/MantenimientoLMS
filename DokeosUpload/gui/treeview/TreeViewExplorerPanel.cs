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
using System.Windows.Forms;
using lmsda.domain;
using lmsda.domain.user.synchronization;
using lmsda.domain.util;
using System.IO;
using lmsda.domain.ui;
using lmsda.gui.subject;

namespace lmsda.gui.treeview
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    partial class TreeViewExplorerPanel : UserControl
    {
        public TreeViewExplorer treeViewExplorer;
        private Boolean loading;
        private SubjectFilesSettingsControl subjectFilesSettingsControl;

        private String path;
        private FileSettings selectedFileSettings;

        private List<TreeMVC> observers;

        public TreeViewExplorerPanel() 
        {
            loading = true;
            InitializeComponent();
            loading = false;
        }

        public Boolean isLoading()
        {
            return this.loading;
        }

        public void setLoading(Boolean loading)
        {
            this.loading = loading;
        }

        public void init(SubjectFilesSettingsControl subjectFilesSettingsControl)
        {
            this.subjectFilesSettingsControl = subjectFilesSettingsControl;
            this.loading = true;
            this.observers = new List<TreeMVC>();
            this.loading = false;
        }

        /// <summary>
        ///     Loads the tree view explorer, root node = path.
        /// </summary>
        /// <param name="path">The root node</param>
        public void loadTreeViewExplorer(String path)
        {
            this.loading = true;
            this.path = path;
            this.treeViewExplorer.loadTree(path);
            this.loading = false;
        }

        private void treeViewExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.updateObservers();
        }

        /// <summary>
        ///     Adds an observer to the list.
        /// </summary>
        /// <param name="observer">The observer</param>
        public void addObserver(TreeMVC observer)
        {
            this.observers.Add(observer);
        }

        /// <summary>
        ///     Updates all registered observers.
        /// </summary>
        private void updateObservers()
        {
            foreach(TreeMVC observer in this.observers)
                observer.update();
        }

        public void clear()
        {
            this.treeViewExplorer.Nodes.Clear();
        }

        private void treeViewExplorer_MouseDown(object sender, MouseEventArgs e)
        {
            this.selectedFileSettings = null;
            TreeNode tn = null;
            try
            {
                tn = this.treeViewExplorer.GetNodeAt(e.X, e.Y);
                if (tn !=null)
                    this.treeViewExplorer.SelectedNode = this.treeViewExplorer.GetNodeAt(e.X, e.Y);
                String relName = @"\" + this.treeViewExplorer.SelectedNode.Name.Replace(this.path, "").Trim('\\');
                this.selectedFileSettings = DomainController.Instance().getSynchronizationOperations().getFileSettings(relName);
            }
            catch { /* Ignore */ }

            if (e.Button == System.Windows.Forms.MouseButtons.Right
                && treeViewExplorer != null
                && tn !=null // only make menu if click was really on an item
                && selectedFileSettings != null)
            {
                this.populateRightClickMenu();
                this.rightClickMenu.Show(MousePosition);
            }
        }

        private void populateRightClickMenu()
        {
            //Init
            this.rightClickMenu = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItem;

            List<String> excludedFolders = DomainController.Instance().getSynchronizationOperations().getExcludedFolders();
            Boolean inExcludedFolder = Utility.isInFolderList(
                selectedFileSettings.relativeFileName + (selectedFileSettings.isDirectory? "\\": String.Empty),
                excludedFolders, '\\', true);
            //Open file
            if (!this.selectedFileSettings.isDirectory)
            {
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("open_file"));
                toolStripMenuItem.Click += new System.EventHandler(this.openFile_ItemClicked);
                toolStripMenuItem.Font = new System.Drawing.Font(toolStripMenuItem.Font, System.Drawing.FontStyle.Bold);
                this.rightClickMenu.Items.Add(toolStripMenuItem);

                // open folder of file
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("open_folder_of_file"));
                toolStripMenuItem.Click += new System.EventHandler(this.openFolderOfFile_ItemClicked);
                this.rightClickMenu.Items.Add(toolStripMenuItem);

                SynchronizationStatus ss = DomainController.Instance().getSynchronizationOperations().getFileStatus(selectedFileSettings);

                // set file synchronized
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("set_file_synced"));
                toolStripMenuItem.Click += new System.EventHandler(this.setsynced_ItemClicked);
                toolStripMenuItem.Enabled = ss != SynchronizationStatus.FILE_SYNCHRONYZED
                    && ss != SynchronizationStatus.FILE_EXCLUDED && !inExcludedFolder;
                this.rightClickMenu.Items.Add(toolStripMenuItem);

                // set file not synchronized
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("set_file_new"));
                toolStripMenuItem.Click += new System.EventHandler(this.reset_ItemClicked);
                toolStripMenuItem.Enabled = ss != SynchronizationStatus.FILE_ADDED
                    && ss != SynchronizationStatus.FILE_EXCLUDED && !inExcludedFolder;
                this.rightClickMenu.Items.Add(toolStripMenuItem);
            }
            else
            {
                // Open folder
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("open_folder"));
                toolStripMenuItem.Click += new System.EventHandler(this.openFolder_ItemClicked);
                this.rightClickMenu.Items.Add(toolStripMenuItem);

                // Set folder contents synced
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("set_folder_synced"));
                toolStripMenuItem.Click += new System.EventHandler(this.setsynced_ItemClicked);
                this.rightClickMenu.Items.Add(toolStripMenuItem);
                toolStripMenuItem.Enabled = !inExcludedFolder;

                // Set folder contents unsynced
                toolStripMenuItem = new ToolStripMenuItem(DomainController.Instance().getLanguageString("set_folder_new"));
                toolStripMenuItem.Click += new System.EventHandler(this.reset_ItemClicked);
                this.rightClickMenu.Items.Add(toolStripMenuItem);
                 toolStripMenuItem.Enabled = !inExcludedFolder; // not needed?

            }
        }

        private void openFolder_ItemClicked(object sender, EventArgs e)
        {
            Utility.openFolderInExplorer(this.path + this.selectedFileSettings.relativeFileName);
        }

        private void openFolderOfFile_ItemClicked(object sender, EventArgs e)
        {
            FileInfo fi = new FileInfo(this.path + this.selectedFileSettings.relativeFileName);
            Utility.openFolderInExplorer(fi.Directory.FullName);
        }

        private void openFile_ItemClicked(object sender, EventArgs e)
        {
            Utility.StartWithShell(this.path + this.selectedFileSettings.relativeFileName);
        }

        private void reset_ItemClicked(object sender, EventArgs e)
        {
            List<FileSettings> markUnsyncedList = 
                DomainController.Instance().getSynchronizationOperations().getSubFilesList(selectedFileSettings.relativeFileName);
            foreach (FileSettings fs in markUnsyncedList)
            {
                FileSettings fsSave = DomainController.Instance().getSynchronizationOperations().getFileSettings(fs.relativeFileName);
                fsSave.optionsChanged = false;
                fsSave.hasError = false;
                fsSave.lastHash = String.Empty;
                fsSave.resultHashes = String.Empty;
                foreach (TreeMVC observer in this.observers)
                    observer.updateNodeImage(fsSave);
            }

        }

        private void setsynced_ItemClicked(object sender, EventArgs e)
        {
            this.subjectFilesSettingsControl.ParentContainer.startSynchronization(this.selectedFileSettings.relativeFileName);
        }

        private void treeViewExplorer_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (treeViewExplorer != null)
            {
                if (!this.selectedFileSettings.isDirectory)
                    this.openFile_ItemClicked(sender, e);
            }
        }

        private void treeViewExplorer_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (!this.loading)
                this.subjectFilesSettingsControl.save(false);
                
        }

        private void treeViewExplorer_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            if(!this.loading)
                this.subjectFilesSettingsControl.save(false);
        }
    }
}
