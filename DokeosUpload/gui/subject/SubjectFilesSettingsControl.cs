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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using lmsda.domain;
using lmsda.gui.treeview;
using System.IO;
using lmsda.persistence.document;
using lmsda.domain.user.synchronization;
using lmsda.domain.util;
using lmsda.domain.ui;

namespace lmsda.gui.subject
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    partial class SubjectFilesSettingsControl : UserControl, TreeMVC
    {
        public ContainerFrame ParentContainer {get; set;}
        private TooltipChecker tooltipChecker;
        private String path;
        private String relativeFileName;
        private Boolean loading = false;
        DocumentType documentType = DocumentType.UNKNOWN;

        public delegate void invoke_delegate();
        public delegate void invoke_delegate_with_args(object o);

        #region Constructor, loads and unloads

        public SubjectFilesSettingsControl()
        {            
            InitializeComponent();
            this.treeViewExplorerPanel.init(this);
            this.fileOptionsExercisePanel.setParent(this);
            this.fileOptionsPDFPanel.setParent(this);
            this.fileOptionsUploadPanel.setParent(this);
            this.fileOptionsPDFPanelForExcel.setParent(this);
            this.fileOptionsPDFPanelForPowerPoint.setParent(this);
            this.tooltipChecker = new TooltipChecker(this.toolTip); //Why? See the tooltipchecker class
        }

        private void SubjectFilesSettingsControl_Load(object sender, EventArgs e)
        {
            this.treeViewExplorerPanel.addObserver(this);
            this.resetComponents();
            this.tooltipChecker.addControl(this.cmdSave, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdReload, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdOpenFolder, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdHelp, "not_logged_in");
            Control p = this.cmdHelp.Parent;
        }
        
        /// <summary>
        ///     Loads the specified subject folder.
        /// </summary>
        /// <param name="subjectFolderPath">The path to the subject folder.</param>
        public void loadSubjectFolder(String subjectFolderPath, Boolean reselect)
        {
            this.Invoke(new invoke_delegate(disableFileOptions));
            if(subjectFolderPath != null && new DirectoryInfo(subjectFolderPath).Exists)
                this.Invoke(new invoke_delegate_with_args(loadSubjectFolderSafe), (object)new object[]{subjectFolderPath, reselect});
        }

        /// <summary>
        ///     Loads the specified subject folder.
        /// </summary>
        /// <param name="subjectFolderPath">The path to the subject folder.</param>
        public void reloadSubjectFolderTree()
        {
            if(this.path != null)
                this.Invoke(new invoke_delegate_with_args(loadSubjectFolderSafe), (object)new object[]{true, true});
        }

        /// <summary>
        ///     Loads the specified subject folder. Must be called from a delegate!
        /// </summary>
        /// <param name="o">The path to the subject folder as String, or a boolean indicating a reload.</param>
        /// <remarks>
        ///     Updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> A TreeView node will be expanded based on the saved setting isExpanded.
        /// </remarks>
        private void loadSubjectFolderSafe(object o)
        {
            this.treeViewExplorerPanel.treeViewExplorer.BeginUpdate();
            object[] pars;
            Boolean reload = false;
            Boolean reselect = false;
            try
            {
                pars = (object[])o;
                if (pars.Length != 2) return;
                if (pars[0] is Boolean)
                {
                    reload = (Boolean)pars[0];
                }
                else if (pars[0] is String)
                    this.path = (String)pars[0];
                else
                    return;
                reselect = (Boolean)pars[1];
            }
            catch
            {
                return;
            }
            String selectedNode=null;
            int elements = this.treeViewExplorerPanel.treeViewExplorer.VisibleCount;
                        
            if (reselect)
                try { 
                    selectedNode = this.treeViewExplorerPanel.treeViewExplorer.SelectedNode.Name;
                } catch { }

            this.treeViewExplorerPanel.clear();

            if (new DirectoryInfo(this.path).Exists)
            {
                if (!reload)
                    DomainController.Instance().initSynchronizationOperations(this.path);
                this.treeViewExplorerPanel.loadTreeViewExplorer(this.path);
            }

            TreeNode toSelect = this.treeViewExplorerPanel.treeViewExplorer.GetNodeAt(0, 0);

            if (selectedNode != null)
            {
                TreeNode[] found = this.treeViewExplorerPanel.treeViewExplorer.Nodes.Find(selectedNode, true);
                if (found.Length == 1)
                    toSelect = found[0];
            }
            
            // end update here, so the reselect scrolls down to selected node.
            this.treeViewExplorerPanel.treeViewExplorer.EndUpdate();

            try
            {
                this.treeViewExplorerPanel.treeViewExplorer.SelectedNode = toSelect;
            }
            catch { /* Ignore */}

            this.update();

        }

        /// <summary>
        ///     Unloads the subject.
        /// </summary>
        public void unload()
        {
            try
            {
                this.treeViewExplorerPanel.clear();
                this.resetComponents();
                this.disableAllButtons();
            }
            catch { /* When nothing is loaded, the method will fail! */}
        }

        #endregion

        #region User clicks

        private void rdbConvertToExercises_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbConvertToExercises.Checked)
            {
                this.checkDocumentOptionsEnabled();
                if (!this.loading)
                {
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                    if (fs.synchronizationType != SynchronizationType.CONVERT_TO_EXERCISE)
                    {
                        fs.synchronizationType = SynchronizationType.CONVERT_TO_EXERCISE;
                        fs.optionsChanged = false;
                        fs.lastHash = String.Empty;
                        fs.hasError = false;
                        this.updateNodeImage(fs);
                    }
                }
            }
        }

        private void rdbConvertToPDF_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbConvertToPDF.Checked)
            {
                this.checkDocumentOptionsEnabled();
                
                Boolean optionsChanged = false;

                if (!this.loading)
                {
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                    //DocumentType documentType = SupportedOfficeFiles.identifyDocumentType(this.path + fs.relativeFileName);
                    if (fs.synchronizationType != SynchronizationType.CONVERT_WORD_TO_PDF && documentType == DocumentType.PROCESSEDTEXT_DOCUMENT)
                    {
                        fs.synchronizationType = SynchronizationType.CONVERT_WORD_TO_PDF;
                        optionsChanged = true;
                    }
                    else if (fs.synchronizationType != SynchronizationType.CONVERT_EXCEL_TO_PDF && documentType == DocumentType.SPREADSHEET_DOCUMENT)
                    {
                        fs.synchronizationType = SynchronizationType.CONVERT_EXCEL_TO_PDF;
                        optionsChanged = true;
                    }
                    else if (fs.synchronizationType != SynchronizationType.CONVERT_POWERPOINT_TO_PDF && documentType == DocumentType.PRESENTATION_DOCUMENT)
                    {
                        fs.synchronizationType = SynchronizationType.CONVERT_POWERPOINT_TO_PDF;
                        optionsChanged = true;
                    }

                    if (optionsChanged)
                    {
                        fs.optionsChanged = false;
                        fs.lastHash = String.Empty;
                        fs.hasError = false;
                        this.updateNodeImage(fs);
                    }
                }
            }
        }

        private void rdbUploadFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbUploadFile.Checked)
            {
                this.checkDocumentOptionsEnabled();
                if (!this.loading)
                {
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                    if (fs.synchronizationType != SynchronizationType.UPLOAD)
                    {
                        fs.synchronizationType = SynchronizationType.UPLOAD;
                        fs.optionsChanged = false;
                        fs.lastHash = String.Empty;
                        fs.hasError = false;
                        this.updateNodeImage(fs);
                    }
                }
            }
        }

        private void rdbExcludeFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rdbExcludeFile.Checked)
            {
                this.checkDocumentOptionsEnabled();
                if (!this.loading)
                {
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);
                    if (fs.synchronizationType != SynchronizationType.EXCLUDE)
                    {
                        fs.synchronizationType = SynchronizationType.EXCLUDE;
                        fs.optionsChanged = false;
                        fs.lastHash = String.Empty;
                        fs.hasError = false;
                        this.updateNodeImage(fs);
                    }
                }
            }
        }
        
        #endregion

        #region enabled / disabled

        private void disableFileOptions()
        {
            this.rdbExcludeFile.Enabled = false;
            this.rdbUploadFile.Enabled = false;
            this.rdbConvertToPDF.Enabled = false;
            this.rdbConvertToExercises.Enabled = false;
            this.documentType = DocumentType.UNKNOWN;
            checkDocumentOptionsEnabled();
        }

        private void setDocumentOptionsEnabled()
        {
            this.rdbExcludeFile.Enabled = true;
            this.rdbUploadFile.Enabled = true;
            this.rdbConvertToPDF.Enabled = 
                   documentType == DocumentType.PROCESSEDTEXT_DOCUMENT
                || documentType == DocumentType.PRESENTATION_DOCUMENT
                || documentType == DocumentType.SPREADSHEET_DOCUMENT;
            this.rdbConvertToExercises.Enabled = 
                documentType == DocumentType.PROCESSEDTEXT_DOCUMENT;
            this.checkDocumentOptionsEnabled();
        }

        private void disableAllButtons()
        {
            this.rdbExcludeFile.Enabled = false;
            this.rdbUploadFile.Enabled = false;
            this.rdbConvertToPDF.Enabled = false;
            this.rdbConvertToExercises.Enabled = false;
            this.loading = true;
            this.rdbExcludeFile.Checked = false;
            this.rdbUploadFile.Checked = false;
            this.rdbConvertToPDF.Checked = false;
            this.rdbConvertToExercises.Checked = false;
            this.loading = false;
            checkDocumentOptionsEnabled();
        }

        private void checkDocumentOptionsEnabled()
        {
            this.fileOptionsExercisePanel.Visible = rdbConvertToExercises.Checked;
            this.fileOptionsPDFPanel.Visible = rdbConvertToPDF.Checked && this.documentType == DocumentType.PROCESSEDTEXT_DOCUMENT;
            this.fileOptionsPDFPanelForExcel.Visible = rdbConvertToPDF.Checked && this.documentType == DocumentType.SPREADSHEET_DOCUMENT;
            this.fileOptionsPDFPanelForPowerPoint.Visible = rdbConvertToPDF.Checked && this.documentType == DocumentType.PRESENTATION_DOCUMENT;
            this.fileOptionsUploadPanel.Visible = rdbUploadFile.Checked;
            this.cmdApplyToAllFiles.Enabled = rdbConvertToExercises.Checked 
                                               || rdbConvertToPDF.Checked
                                               || rdbExcludeFile.Checked
                                               || rdbUploadFile.Checked;
        }

        private void resetComponents()
        {
            this.loading = true;
            this.rdbUploadFile.Checked = false;
            this.loading = false;
        }

        #endregion

        #region Load GUI

        /// <summary>
        ///     MVC-like pattern
        /// </summary>
        /// <remarks>
        ///     Last updated on 14/03/2011 by Maarten Meuris
        ///      -> Repaired and optimized node reselection
        /// </remarks>
        public void update()
        {
            if (this.treeViewExplorerPanel.treeViewExplorer.Nodes.Count == 0)
            {
                disableFileOptions();
                return;
            }

            this.loading = true;
            
            String clickedFile = this.treeViewExplorerPanel.treeViewExplorer.SelectedNode.Name;
            if (clickedFile == null) // in case selection is somehow gone?
                this.relativeFileName = Path.DirectorySeparatorChar.ToString();
            else
                this.relativeFileName = Path.DirectorySeparatorChar + clickedFile.Replace(this.path, "").Trim(Path.DirectorySeparatorChar); 

            FileSettings selectedFile = DomainController.Instance().getSynchronizationOperations().getFileSettings(relativeFileName);

            switch (selectedFile.synchronizationType)
            {
                case SynchronizationType.UPLOAD:
                    this.rdbUploadFile.Checked = true;
                    break;
                case SynchronizationType.EXCLUDE:
                    this.rdbExcludeFile.Checked = true;
                    break;
                case SynchronizationType.CONVERT_TO_EXERCISE:
                    this.rdbConvertToExercises.Checked = true;
                    break;
                case SynchronizationType.CONVERT_WORD_TO_PDF:
                    this.rdbConvertToPDF.Checked = true;
                    break;
                case SynchronizationType.CONVERT_EXCEL_TO_PDF:
                    this.rdbConvertToPDF.Checked = true;
                    break;
                case SynchronizationType.CONVERT_POWERPOINT_TO_PDF:
                    this.rdbConvertToPDF.Checked = true;
                    break;
            }

            this.fileOptionsUploadPanel.update(selectedFile);
            this.fileOptionsExercisePanel.update(selectedFile);
            this.fileOptionsPDFPanel.update(selectedFile);
            this.fileOptionsPDFPanelForExcel.update(selectedFile);
            this.fileOptionsPDFPanelForPowerPoint.update(selectedFile);

            this.documentType = SupportedOfficeFiles.identifyDocumentType(clickedFile);

            this.setDocumentOptionsEnabled();
            
            loading = false;
            this.cmdApplyToAllFiles.Enabled = !selectedFile.isDirectory;
        }

        /// <summary>
        ///     fix for the bizarre error that the 'select' listener doesn't fire on startup login
        ///     when the tab is not selected.
        /// </summary>
        public void tryReload()
        {
            if (this.treeViewExplorerPanel.treeViewExplorer.SelectedNode != null
                && !this.rdbUploadFile.Checked
                && !this.rdbExcludeFile.Checked
                && !this.rdbConvertToExercises.Checked
                && !this.rdbConvertToPDF.Checked)
            {
                this.update();
            }
        }

        public void updateNodeImage(FileSettings fs)
        {
            String fullname = this.path + fs.relativeFileName.TrimEnd('\\');
            TreeNode[] found = this.treeViewExplorerPanel.treeViewExplorer.Nodes.Find(fullname, true);
            if (found.Length == 1)
            {
                SynchronizationStatus sc = DomainController.Instance().getSynchronizationOperations().getFileStatus(fs);
                int index = this.treeViewExplorerPanel.treeViewExplorer.getIconIndex(fullname, sc, fs.isDirectory);
                found[0].ImageIndex = index;
                found[0].SelectedImageIndex = index;
            }
        }

        #endregion

        #region Buttons

        private void cmdHelp_Click(object sender, EventArgs e)
        {
            new SynchronizeHelpFrame().ShowDialog();
        }

        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added isExpanded parameter.
        /// </remarks>
        public void cmdSave_Click(object sender, EventArgs e)
        {
            this.save(true);
        }

        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added isExpanded parameter.
        /// </remarks>
        public void save(Boolean reload)
        {
            //Update isExpanded parameter. (As of 1.08)
            this.treeViewExplorerPanel.treeViewExplorer.updateExpandedSettings(this.treeViewExplorerPanel.treeViewExplorer.Nodes);

            String folder = DomainController.Instance().getUserInfo().getSelectedItemSubjectFolderForSubject();

            if (folder != null && Utility.doesFolderExist(folder))
            {
                try
                {
                    DomainController.Instance().getSynchronizationOperations().saveSettings();
                    if (reload)
                        this.reload(true);
                }
                catch (Exception ex)
                {
                    DomainController.Instance().processError(ex, false);
                    MessageBox.Show(this, DomainController.Instance().getLanguageString("synchronization_save_failed"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, DomainController.Instance().getLanguageString("synchronization_folder_missing"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.unload();
            }
        }

        private void cmdReload_Click(object sender, EventArgs e)
        {
            String folder = DomainController.Instance().getUserInfo().getSelectedItemSubjectFolderForSubject();

            if (folder!=null && Utility.doesFolderExist(folder))
            {
                this.reload(true);
            }
            else
            {
                MessageBox.Show(this, DomainController.Instance().getLanguageString("synchronization_folder_missing"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.unload();
            }
        }

        public void reload(Boolean hardReload)
        {
            try
            {
                if (DomainController.Instance().isLoggedIn() && DomainController.Instance().getUserInfo().selectedItemIsSubject())
                {
                    if (hardReload)
                        this.loadSubjectFolder(DomainController.Instance().getUserInfo().getSelectedItemSubjectFolderForSubject(), true);
                    else
                        this.reloadSubjectFolderTree();
                }
            }
            catch (Exception ex)
            { 
                DomainController.Instance().processError(ex, false);
                MessageBox.Show(this, DomainController.Instance().getLanguageString("synchronization_load_failed"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmdOpenFolder_Click(object sender, EventArgs e)
        {
            String folder = DomainController.Instance().getUserInfo().getSelectedItemSubjectFolderForSubject();

            if (folder != null && Utility.doesFolderExist(folder))
            {
                Utility.openFolderInExplorer(folder);
            }
            else
            {
                MessageBox.Show(this, DomainController.Instance().getLanguageString("synchronization_folder_missing"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.unload();
            }
        }

        ///<remarks>
        ///     Last updated on 11/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void cmdApplyToAllFiles_Click(object sender, EventArgs e)
        {
            try
            {
                SynchronizationOperations syn = DomainController.Instance().getSynchronizationOperations();
                FileSettings fsOrig = syn.getFileSettings(relativeFileName);
                syn.applyToAllFolderItems(fsOrig);
                String relativeFolderName = fsOrig.relativeFileName.Substring(0, fsOrig.relativeFileName.LastIndexOf('\\')) + @"\";
                foreach (FileSettings fs in syn.getData())
                {
                    if (fs.relativeFileName.StartsWith(relativeFolderName) && !fs.relativeFileName.Equals(relativeFolderName)
                        && !fs.relativeFileName.Equals(fsOrig.relativeFileName)
                        && !fs.isDirectory
                        && !fs.relativeFileName.Substring(relativeFolderName.Length).Contains('\\'))
                    {
                        //Since 1.08: 11/08/2010 by Gianni Van Hoecke
                        //            Inserted if before updateNodeImage(fs):
                        //            when clicking on apply to all, all files in that folder would get the edited icon, this
                        //            is now solved with the if command.
                        if(fs.optionsChanged && fsOrig.synchronizationType == fs.synchronizationType)
                            updateNodeImage(fs);
                    }
                }
            }
            catch (Exception ex)
            { 
                DomainController.Instance().processError(ex, false);
                MessageBox.Show(this, DomainController.Instance().getLanguageString("apply_to_all_failed"), DomainController.Instance().getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Language

        public void resetStrings()
        {
            toolTip.SetToolTip(this.cmdApplyToAllFiles, DomainController.Instance().getLanguageString("apply_to_all_sub_items_tooltip"));
            toolTip.SetToolTip(this.cmdSave,            DomainController.Instance().getLanguageString("save_synchronyzation_settings"));
            toolTip.SetToolTip(this.cmdReload,          DomainController.Instance().getLanguageString("reload_synchronization_settings"));
            toolTip.SetToolTip(this.cmdOpenFolder,      DomainController.Instance().getLanguageString("open_subject_folder"));
            toolTip.SetToolTip(this.cmdHelp,            DomainController.Instance().getLanguageString("legenda"));
            fileOptionsPDFPanel.resetStrings();
            fileOptionsPDFPanelForPowerPoint.resetStrings();
        }

        #endregion

        #region Tooltips (As of 1.08)

        private void tableLayoutFilesSettings_MouseMove(object sender, MouseEventArgs e)
        {
            // Temp disabled. 
            //this.tooltipChecker.updateTooltips(tableLayoutFilesSettings.GetChildAtPoint(e.Location));
        }

        #endregion

        #region enable/disable panel (As of 1.08)

        public void enablePanel(Boolean b)
        {
            this.treeViewExplorerPanel.Enabled = b;
            panelOptions.Enabled = b;
            // Disabled because it's unnecessary, and messed up the update done before it.
            // Any control which is disabled shows its sub-controls as disabled anyway.
            //foreach (Control c in this.panelOptions.Controls)
            //    c.Enabled = b;
            this.cmdHelp.Enabled = b;
            this.cmdOpenFolder.Enabled = b;
            this.cmdReload.Enabled = b;
            this.cmdSave.Enabled = b;
        }

        #endregion

    }
}
