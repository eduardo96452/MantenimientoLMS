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
using System.Windows.Forms;
using lmsda.domain;
using lmsda.persistence.document;
using System.Threading;
using lmsda.domain.user;
using System.IO;
using lmsda.domain.util;
using lmsda.domain.ui;
using System.Diagnostics;
using System.Linq;
using lmsda.gui.subject;
using lmsda.domain.score.data;
using System.Collections.Generic;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    partial class ContainerFrame : Form, MainUI
    {
        private DomainController domainController;
        private TooltipChecker tooltipChecker;
        private Boolean componentsEnabled = true;
        private double syncStatusLabelStart;
        private int syncStatusLabelEndDiff;
        private int syncStartButtonDiff;
        private String lastSelectedDocumentFolder;

        private System.Windows.Forms.Timer timer;
        private int ticks;
        private String statusBusyText = String.Empty;

        #region Delegates

        public delegate void invoke_delegate();
        public delegate void invoke_delegate_with_arg(object value);
        public delegate void invoke_delegate_single_parameter(Object c, object value);
        public delegate void invoke_delegate_set_datasource(DocumentsDropDown c, String[] ds);
        public delegate void invoke_delegate_control_command(Control c);
        public delegate Boolean invoke_delegate_for_questionbox(String message, Boolean okcancel);
        public delegate Boolean invoke_delegate_for_listHasItemsSelected(ListControl lb);

        #endregion

        #region Start and stop methods

        public ContainerFrame()
        {            
            this.domainController = DomainController.Instance();
            this.ticks = 1;
            this.timer = new System.Windows.Forms.Timer();
            this.timer.Interval = 400;
            this.timer.Tick += new EventHandler(busyTick);
            InitializeComponent();
            PersonalizarDiseño();
            this.subjectFilesSettingsControl.ParentContainer = this;
            this.tooltipChecker = new TooltipChecker(this.toolTip); //Why? See the tooltipchecker class
            this.domainController.addObserver(this);

            syncStatusLabelStart = (double)lblSynchronisationStatus.Location.X / (double)tabSynchronization.Width;
            syncStatusLabelEndDiff = tabSynchronization.Width - (lblSynchronisationStatus.Location.X +  lblSynchronisationStatus.Width);
            syncStartButtonDiff = lblSynchronisationStatus.Location.X - (cmdStartSynchronization.Location.X + cmdStartSynchronization.Width);

        }
        private void PersonalizarDiseño()
        {
            panelSubMenuAcciones.Visible = false;
            panelSubMenuEditar.Visible = false;
            panelSubMenuAyuda.Visible = false;
        }
        private void OcultarSubMenu()
        {
            if (panelSubMenuAcciones.Visible == true)
                panelSubMenuAcciones.Visible = false;
            if (panelSubMenuEditar.Visible == true)
                panelSubMenuEditar.Visible = false;
            if (panelSubMenuAyuda.Visible == true)
                panelSubMenuAyuda.Visible = false;
        }
        private void MostrarSubMenu(Panel submenu)
        {
            if (submenu.Visible == false)
            {
                OcultarSubMenu();
                submenu.Visible = true;
            }
            else
                submenu.Visible = false;
        }

        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void ContainerFrame_Load(object sender, EventArgs e)
        {
            this.Text = this.domainController.getProgramTitle();
            this.lblSynchronisationStatus.Text = string.Empty;
            this.lblExerciseScanResultsDump.Text = this.domainController.getExerciseScanResults();
            this.cmbCourses.Enabled = false;
            this.cmbCourses.DataSource = this.domainController.getCoursesListForDropDown();
            this.cmbCourses.SelectedIndex = this.domainController.getUserInfo().getSelectedItemIndex();
            this.cmbCourses.Enabled = true;
            // do all cmbCourses_SelectedIndexChanged actions without the refresh.
            String[] ds = this.domainController.getDocumentFoldersForDropDown();
            this.documentsDropDownForPDF.Invoke(new invoke_delegate_set_datasource(setDocumentsDropDownDataSource), new object[]{ documentsDropDownForPDF, ds});
            this.setLastSelectedFolder(this.domainController.getUserInfo().getSelectedItemSubjectFolder());
            
            // make sure settings don't rewrite when the checkboxes are set on startup by disabling the components;
            // the listeners are set only to write when the component is enabled.
            Boolean status = this.chkStatisticsCreateSubFolder.Enabled;
            this.chkStatisticsCreateSubFolder.Enabled = false;
            this.chkStatisticsCreateSubFolder.Checked = this.domainController.getSettings().getStatsCreateSubFolder();
            this.chkStatisticsCreateSubFolder.Enabled = status;
            
            status = this.chkCalculatePercentageMC.Enabled;
            this.chkCalculatePercentageMC.Enabled = false;
            this.chkCalculatePercentageMC.Checked = this.domainController.getSettings().getStatsCalculateMCPercentage();
            this.chkCalculatePercentageMC.Enabled = status;

            status = this.chkCPMCShowQuestionTitles.Enabled;
            this.chkCPMCShowQuestionTitles.Enabled = false;
            this.chkCPMCShowQuestionTitles.Checked = this.domainController.getSettings().getStatsShowQuestionTitles();
            this.chkCPMCShowQuestionTitles.Enabled = status;

            status = this.chkGenerateAllAttempts.Enabled;
            this.chkGenerateAllAttempts.Enabled = false;
            this.chkGenerateAllAttempts.Checked = this.domainController.getSettings().getStatsShowAllAttempts();
            this.chkGenerateAllAttempts.Enabled = status;

            status = this.chkCalculateResultsPerStudent.Enabled;
            this.chkCalculateResultsPerStudent.Enabled = false;
            this.chkCalculateResultsPerStudent.Checked = this.domainController.getSettings().getStatsCalculateResultsPerStudent();
            this.chkCalculateResultsPerStudent.Enabled = status;

            status = this.chkCalculateExerciseStudentDetails.Enabled;
            this.chkCalculateExerciseStudentDetails.Enabled = false;
            this.chkCalculateExerciseStudentDetails.Checked = this.domainController.getSettings().getStatsCalculateExerciseDetailsPerStudent();
            this.chkCalculateExerciseStudentDetails.Enabled = status;
            
            status = this.chkOneQuestionPerPage.Enabled;
            this.chkOneQuestionPerPage.Enabled = false;
            this.chkOneQuestionPerPage.Checked = this.domainController.getSettings().getExercisesMultiPage();
            this.chkOneQuestionPerPage.Enabled = status;

            status = this.chkOpenExcelFilesAfterConversion.Enabled;
            this.chkOpenExcelFilesAfterConversion.Enabled = false;
            this.chkOpenExcelFilesAfterConversion.Checked = this.domainController.getSettings().getStatsOpenExcelAfterConversion();
            this.chkOpenExcelFilesAfterConversion.Enabled = status;

            this.resetStrings();
        }

        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void ContainerFrame_Shown(object sender, EventArgs e)
        {
            this.tooltipChecker.addControl(this.cmdScanDocument, "no_document_selected");
            this.tooltipChecker.addControl(this.cmdReviewExercises, "no_document_scanned");
            this.tooltipChecker.addControl(this.cmdUploadExercises, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdJumpToError, "no_document_scanned");
            this.tooltipChecker.addControl(this.cmdConvertToPDF, "no_document_selected");
            this.tooltipChecker.addControl(this.chkSplit, "no_document_selected");
            this.tooltipChecker.addControl(this.chkUpload, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdDownloadStatistics, "not_logged_in");
            this.tooltipChecker.addControl(this.cmdStartSynchronization, "not_logged_in");

            this.setTabComponentsEnabled();
            this.cmdLogin.Focus();
            this.domainController.writeToLog("program_started", true, false);
            if (this.domainController.isfirstRun())
            {
                new FirstRunFrame().ShowDialog();
                this.resetStrings();
            }
        }

        private void ContainerFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.componentsEnabled)
            {
                if (!showQuestionMessage(this.domainController.getLanguageString("program_busy_close_confirmation"), false))
                {
                    e.Cancel = true;
                    return;
                }
            }
            this.domainController.writeToLog("cleaning_up_and_closing", true, true);
            this.domainController.clearTempFolder();
            Application.ExitThread(); //Als er andere threads lopen, deze ook sluiten.
            Environment.Exit(0);
        }

        #endregion

        #region Mouse events

        private void tabExercises_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(tabExercises.GetChildAtPoint(e.Location));
        }

        private void tabPDF_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(tabPDF.GetChildAtPoint(e.Location));
        }

        private void tabStatistics_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(tabStatistics.GetChildAtPoint(e.Location));
        }

        private void tabSynchronization_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(tabSynchronization.GetChildAtPoint(e.Location));
        }

        private void pnlSyncButtons_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(pnlSyncButtons.GetChildAtPoint(e.Location));
        }

        #endregion

        #region GUI methods

        #region Logger

        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> instead of opening the log in the default editor, open a frame.
        /// </remarks>
        private void txtLog_DoubleClick(object sender, EventArgs e)
        {
            //this.domainController.openLogInNotePad(); //Removed as of 1.08
            new LogViewerFrame().ShowDialog();
        }

        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> instead of opening the log in the default editor, open a frame.
        /// </remarks>
        private void lblStatus_DoubleClick(object sender, EventArgs e)
        {
            //this.domainController.openLogInNotePad(); //Removed as of 1.08
            new LogViewerFrame().ShowDialog();
        }

        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void viewlogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new LogViewerFrame().ShowDialog();
        }

        #endregion

        #region Login, logout

        private void cmdLogin_Click(object sender, EventArgs e)
        {
            if(!this.platformIsSet())
            {
                MessageBox.Show(this.domainController.getLanguageString("configure_platform"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.setTabComponentsEnabled();
            }
            else
            {
                new LoginFrame().ShowDialog();
                this.load_frame(); // does a setTabComponentsEnabled()
            }
        }

        private void cmdLogout_Click(object sender, EventArgs e)
        {
            this.domainController.logOut();
            this.load_frame();
        }

        #endregion 

        #region General

        private void cmbCourses_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if(cmbCourses.Enabled)
                {
                    // special case for list separator
                    if (this.cmbCourses.SelectedItem != null && this.cmbCourses.SelectedItem.ToString().Equals(ProgramConstants.LISTSEPARATOR))
                    {
                        this.cmbCourses.SelectedIndex = this.domainController.getSelectedCourseDropdownIndex();
                    }
                    else
                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(doUpdateCourses));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start(this.cmbCourses.SelectedItem);
                    }
                }
            }
            catch (Exception)
            { //Ignore
            }
        }

        private Point getFullCoordinates(Control control, int addedX, int addedY)
        { 
            Point labelOrigin = new Point(0, 0); // this is referencing the control
            Point screenOrigin = control.PointToScreen(labelOrigin); // this references
            screenOrigin.X +=addedX;
            screenOrigin.Y +=addedY;
            return screenOrigin;
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            // needed to prevent the bug where the options of the file are cleared if the control has not been drawn before login.
            if (tabs.SelectedTab == tabSynchronization)
                this.subjectFilesSettingsControl.tryReload();
            this.txtLog.Text = String.Empty;
        }

        #endregion

        #region Tab exercises

        private void cmdOpenTemplate_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(doOpenTemplate));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(this.chkNewDocWithExamples.Checked);
            this.chkNewDocWithExamples.Checked = false;
        }

    	private void cmdScanDocument_Click(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                txtExerciseDump.Text = String.Empty;
                Thread thread = new Thread(new ThreadStart(doScanDocument));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }

        private void cmdReviewExercises_Click(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet() && this.domainController.exercisesAreScanned())
            {
                new ExerciseReviewFrame().ShowDialog();
            }
        }

        private void cmdUploadExercises_Click(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                Boolean onePerPage = this.chkOneQuestionPerPage.Checked;
                Boolean setInvisible = this.chkSetExerciseInvisible.Checked;
                int randomquestions = (chkRandomQuestions.Checked ? Convert.ToInt32(nrRandomQuestions.Value) : 0);

                Thread thread = new Thread(new ParameterizedThreadStart(doUploadExercises));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(new Object[] {onePerPage, setInvisible, randomquestions} );
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }
        
        private void cmdJumpToError_Click(object sender, EventArgs e)
        {
            if (domainController.canJumpToError())
            {
                Thread thread = new Thread(new ThreadStart(doJumpToError));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void chkOneQuestionPerPage_CheckedChanged(object sender, EventArgs e)
        {
            if (chkOneQuestionPerPage.Enabled)
                this.domainController.getSettings().setExercisesMultiPage(chkOneQuestionPerPage.Checked);
        }

        private void chkRandomQuestions_CheckedChanged(object sender, EventArgs e)
        {
            this.nrRandomQuestions.Visible = this.chkRandomQuestions.Checked;
        }

        private void tabs_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(!componentsEnabled)
                e.Cancel = true;
        }

    	#endregion
        
        #region Menu bar

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void preferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String currentPlatform = this.domainController.getSettings().getPlatform();
            String currentURL = this.domainController.getSettings().getUrl();
            SettingsFrame settingsFrame = new SettingsFrame();
            settingsFrame.ShowDialog();
            resetStrings();
            String newPlatform = this.domainController.getSettings().getPlatform();
            String newURL = this.domainController.getSettings().getUrl();
            if ((!currentPlatform.Equals(newPlatform) || !currentURL.Equals(newURL)) && this.domainController.isLoggedIn())
            {
                this.cmdLogout_Click(sender, e);
                this.domainController.writeToLog("platform_changed_logged_out", true, false, true);
            }
            else
            {
                this.setTabComponentsEnabled();
            }
        }

        private void manageSubjectsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new ManageSubjectsFrame().ShowDialog();
            
            //Load the courses list...
            String[] list = this.domainController.getCoursesListForDropDown();
            Object selectedItem = cmbCourses.SelectedItem;

            Subject subject = domainController.getUserInfo().getSelectedSubject();
            Boolean cmbCoursesEnabled = cmbCourses.Enabled;

            cmbCourses.Enabled = false;
            this.cmbCourses.DataSource = list;
            this.cmbCourses.SelectedIndex=-1;
            cmbCourses.Enabled = cmbCoursesEnabled;
            if (this.cmbCourses.DataSource != null)
            {
                // setting cmbCourses SelectedItem or SelectedIndex triggers cmbCourses_SelectedIndexChanged,
                // which handles all the rest.
                if (subject!=null && this.cmbCourses.Items.Contains(subject.getSubjectName()))
                    this.cmbCourses.SelectedItem = subject.getSubjectName();
                else if (selectedItem != null && this.cmbCourses.Items.Contains(selectedItem))
                    this.cmbCourses.SelectedItem = selectedItem;
                else
                {
                    // Index is -1: manually trigger courses list update
                    Thread thread = new Thread(new ParameterizedThreadStart(doUpdateCourses));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start(this.cmbCourses.SelectedItem);
                }

            }
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread downloadThread = new Thread(new ThreadStart(showHelp));
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.Start();
        }

        private void showHelp()
        {
            try
            {
                this.enableComponents(false);
                this.domainController.openHelpFile();
            }
            catch (Exception e)
            {
                this.domainController.processError(e, false);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutFrame aboutFrame = new AboutFrame();
            aboutFrame.ShowDialog();
        }

        private void loginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.cmdLogin_Click(sender, e);
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.cmdLogout_Click(sender, e);
        }

        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void uploadfilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new UploadFileFrame().ShowDialog();
        }

        private void checkforupdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread downloadThread = new Thread(new ThreadStart(checkForUpdates));
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.Start();
        }

        /// <remarks>
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        ///      -> Added release notes frame.
        /// </remarks>
        private void checkForUpdates()
        {
            try
            {
                this.enableComponents(false);

                if (this.domainController.newUpdateAvailable())
                { 
                    //Show frame.
                    this.Invoke(new invoke_delegate(showUpdateFrame));
                }
                else
                {
                    DomainController.Instance().fireMessageBox(DomainController.Instance().getLanguageString("program_is_up_to_date"), MessageBoxIcon.Information);
                }
            }
            catch (Exception e)
            {
                this.domainController.processError(e, true);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        /// <remarks>
        ///     As of 1.08
        /// </remarks>
        private void showUpdateFrame()
        {
            new UpdateFrame().ShowDialog();
        }

        #endregion

        #region Tab PDF / file upload

        private void cmdConvertToPDF_Click(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                Thread thread = new Thread(new ParameterizedThreadStart(doConvertToPDF));
                thread.SetApartmentState(ApartmentState.STA);
                String saveToSubDir = String.Empty;

                if (this.documentsDropDownForPDF.getComboBox().Items.Count > 0)
                    saveToSubDir = this.documentsDropDownForPDF.getSelectedItem();

                thread.Start(new String[]{saveToSubDir});
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }

        private void chkSplit_CheckedChanged(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
        }

        private void chkUpload_CheckedChanged(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
        }

        #endregion

        #region Documents

        private void openDocument_ItemClicked(object sender, EventArgs e)
        {            
            String selected = ((ToolStripMenuItem)sender).Text;
            if (selected.Equals(this.domainController.getLanguageString("browse")))
            {
                String[] filter_array = SupportedOfficeFiles.getSupportedExercisesDocumentExtensions();
                String filters = "";

                foreach (String s in filter_array)
                    filters += "*." + s + ";";
                    //filters += " " + s + " ";

                filters = filters.Substring(0, filters.Length-1);

                this.openFileDialog.Filter = "Office files (" + filters.Replace(";", ", ") + ")|" + filters;
                this.openFileDialog.FileName = String.Empty;
                this.openFileDialog.InitialDirectory = this.lastSelectedDocumentFolder;
                this.openFileDialog.ShowDialog();

                if (!this.openFileDialog.FileName.Equals(String.Empty))
                {
                    this.domainController.setDocument(this.openFileDialog.FileName);
                    this.setLastSelectedFolder(this.openFileDialog.FileName.Substring(0,this.openFileDialog.FileName.LastIndexOf('\\')).TrimEnd('\\'));
                    //this.tooltipChecker.changeStringForControl(this.cmdConvertToPDF, "save_location_needed");
                }
            }
            else if (Utility.doesFileExist(selected))
            {
                this.domainController.setDocument(selected);
            }
            else
            {
                MessageBox.Show(this.domainController.getLanguageString("cannot_use_unsaved_document"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            this.updateDocumentLabel();
            this.setTabComponentsEnabled();
        }

        private void cmdChooseDocument_MouseDown(object sender, MouseEventArgs e)
        {
            this.checkForOpenDocuments();
            //this.contextMenuStrip.Show(Control.MousePosition);
            this.contextMenuStrip.Show(getFullCoordinates(cmdChooseDocument,0,cmdChooseDocument.Height));
        }

        private void cmdChooseDocument_Click(object sender, EventArgs e)
        {
            cmdChooseDocument_MouseDown(sender, null);
        }

        #endregion

        #region Tab statistics

        private void cmdDownloadStatistics_Click(object sender, EventArgs e)
        {
            Boolean exists = false;

            String localPath = String.Empty;
            
            if(this.chkStatisticsCreateSubFolder.Checked)
                localPath = this.domainController.getSettings().getStatsFolder();
            else
                localPath = this.openFolderBrowser(this.domainController.getSettings().getStatsFolder());
            
            if(this.chkCalculatePercentageMC.Checked)
                this.domainController.getSettings().setStatsIDontKnowString(this.txtDoNotKnow.Text);

            //As of 1.09
            //Save the order and checked columns.
            for (int i = 0; i < this.lstColumns.Items.Count; i++)
            {
                ListViewItem item = this.lstColumns.Items[i];
                if (item.Tag.Equals("number"))
                {
                    domainController.getSettings().setStatsColumnNumber(item.Checked);
                    domainController.getSettings().setStatsColumnNumberID(i);
                }
                else if (item.Tag.Equals("name"))
                {
                    domainController.getSettings().setStatsColumnName(item.Checked);
                    domainController.getSettings().setStatsColumnNameID(i);
                }
                else if (item.Tag.Equals("email"))
                {
                    domainController.getSettings().setStatsColumnEmail(item.Checked);
                    domainController.getSettings().setStatsColumnEmailID(i);
                }
                else if (item.Tag.Equals("student_number"))
                {
                    domainController.getSettings().setStatsColumnStudentNumber(item.Checked);
                    domainController.getSettings().setStatsColumnStudentNumberID(i);
                }
                else if (item.Tag.Equals("group"))
                {
                    domainController.getSettings().setStatsColumnGroup(item.Checked);
                    domainController.getSettings().setStatsColumnGroupID(i);
                }
            }

            //There's at least one required column; username OR name OR e-mail address
            if (!domainController.getSettings().getStatsColumnStudentNumber() &&
                !domainController.getSettings().getStatsColumnName() &&
                !domainController.getSettings().getStatsColumnEmail()
                && this.chkCalculateResultsPerStudent.Checked)
            {
                domainController.fireMessageBox(domainController.getLanguageString("required_column_missing"), MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    exists = new DirectoryInfo(localPath).Exists;
                }
                catch { }

                if (exists)
                {
                    //Prepare groups (as of 1.09)                    
                    if (this.chkGroups.Checked && this.chkCalculateResultsPerStudent.Checked)
                    {
                        for (int i = 0; i < this.lstGroups.Items.Count; i++)
                        {
                            ListViewItem item = this.lstGroups.Items[i];
                            foreach (Groups groups in domainController.getUserInfo().getGroups())
                            {
                                Group group = groups.getGroup(Convert.ToString(item.Tag));
                                group.groupNumber = i;
                                if (group != null)
                                    group.enabled = item.Checked;
                            }
                        }
                    }

                    Thread thread = new Thread(new ParameterizedThreadStart(exportStatistics));
                    thread.SetApartmentState(ApartmentState.STA);
                    object objects = new object[]   {   
                                                        localPath, 
                                                        this.chkCalculatePercentageMC.Checked, 
                                                        this.chkCalculateResultsPerStudent.Checked, 
                                                        this.chkCalculateExerciseStudentDetails.Checked,
                                                        this.chkCPMCShowQuestionTitles.Checked,
                                                        this.txtDoNotKnow.Text,
                                                        this.chkStatisticsCreateSubFolder.Checked,
                                                        this.chkGenerateAllAttempts.Checked,
                                                        this.chkGroups.Checked
                                                    };
                    thread.Start(objects);
                }
                else
                {
                    this.domainController.writeToLog("no_destination_selected", true, false, true);
                }
            }
        }

        private void exportStatistics(object o)
        {
            object[]    objects                         = (object[])o;
            String      path                            = Convert.ToString(objects[0]);
            Boolean     calculateMC                     = Convert.ToBoolean(objects[1]);
            Boolean     calculateResultsPerStudent      = Convert.ToBoolean(objects[2]);
            Boolean     calculateExerciseStudentDetails = Convert.ToBoolean(objects[3]);
            Boolean     mcShowQuestionTitles            = Convert.ToBoolean(objects[4]);
            String      doNotKnowString                 = Convert.ToString(objects[5]);
            Boolean     createSubFolder                 = Convert.ToBoolean(objects[6]);
            Boolean     generateAllAttempts             = Convert.ToBoolean(objects[7]);
            Boolean     useGroups                       = Convert.ToBoolean(objects[8]);
            Boolean     createSubjectFolder             = this.domainController.getSettings().getStatsMakeSubjectFolderForSingleCourse();

            this.enableComponents(false);
            this.domainController.exportStatistics(path, createSubFolder, createSubjectFolder, calculateMC, calculateResultsPerStudent, calculateExerciseStudentDetails, mcShowQuestionTitles, doNotKnowString, generateAllAttempts, useGroups);
            this.enableComponents(true);
        }

        private void chkCalculatePercentageMC_CheckedChanged(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (chkCalculatePercentageMC.Enabled)
                this.domainController.getSettings().setStatsCalculateMCPercentage(this.chkCalculatePercentageMC.Checked);
        }

        private void chkCalculateResultsPerStudent_CheckedChanged(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (this.chkCalculateResultsPerStudent.Enabled)
                this.domainController.getSettings().setStatsCalculateResultsPerStudent(this.chkCalculateResultsPerStudent.Checked);
        }

        private void chkCalculateExerciseStudentDetails_CheckedChanged(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (chkCalculateExerciseStudentDetails.Enabled)
                this.domainController.getSettings().setStatsCalculateExerciseDetailsPerStudent(this.chkCalculateExerciseStudentDetails.Checked);
        }

        private void chkStatisticsCreateSubFolder_CheckedChanged(object sender, EventArgs e)
        {
            if (chkStatisticsCreateSubFolder.Enabled)
                this.domainController.getSettings().setStatsCreateSubFolder(this.chkStatisticsCreateSubFolder.Checked);
        }

        private void chkCPMCShowQuestionTitles_CheckedChanged(object sender, EventArgs e)
        {
            if(chkCPMCShowQuestionTitles.Enabled)
                this.domainController.getSettings().setStatsShowQuestionTitles(this.chkCPMCShowQuestionTitles.Checked);
        }

        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void chkGenerateAllAttempts_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGenerateAllAttempts.Enabled)
                this.domainController.getSettings().setStatsShowAllAttempts(this.chkGenerateAllAttempts.Checked);
        }

        /// <remarks>
        ///     Last updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void chkOpenExcelFilesAfterConversion_CheckedChanged(object sender, EventArgs e)
        {
            if(chkOpenExcelFilesAfterConversion.Enabled)
                this.domainController.getSettings().setStatsOpenExcelAfterConversion(this.chkOpenExcelFilesAfterConversion.Checked);
        }

        #endregion

        #region Synchronization

        private void cmdStartSynchronization_Click(object sender, EventArgs e)
        {
            startSynchronization();
        }

        public void startSynchronization()
        {
            startSynchronization(null);
        }

        public void startSynchronization(String fileToDummySync)
        {
            if (this.domainController.getUserInfo().getCoursesForSubject(this.domainController.getUserInfo().getSelectedSubject().getSubjectName()).Length > 0)
            {
                DialogResult result;
                if (fileToDummySync!=null)
                    result = MessageBox.Show(this, this.domainController.getLanguageString("set_as_synced_confirmation_message"), this.domainController.getLanguageString("confirmation"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                else
                    result = MessageBox.Show(this, this.domainController.getLanguageString("start_synchronization_confirmation_message"), this.domainController.getLanguageString("confirmation"), MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (result != DialogResult.OK)
                    return;

                try
                {
                    String folder = this.domainController.getUserInfo().getSelectedItemSubjectFolderForSubject();
                    if (folder != null && Utility.doesFolderExist(folder))
                    {
                        //Save the file.
                        this.subjectFilesSettingsControl.cmdSave_Click(null, null);

                        //Now sync.
                        Thread thread = new Thread(new ParameterizedThreadStart(startActualSynchronization));
                        thread.SetApartmentState(ApartmentState.STA);
                        thread.Start(fileToDummySync);
                    }
                    else
                    {
                        MessageBox.Show(this, this.domainController.getLanguageString("synchronization_folder_missing"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    this.domainController.processError(ex, false);
                    MessageBox.Show(this, this.domainController.getLanguageString("synchronization_failed_check_settings"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show(this, this.domainController.getLanguageString("selected_subject_has_no_courses"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void startActualSynchronization(object o)
        {
            String fileToDummySync = (String)o;

            this.enableComponents(false);
            
            //Override the button stop.
            this.Invoke(new invoke_delegate_with_arg(setStartSynchronizingButtons), (object)true);

            if (fileToDummySync == null)
                this.domainController.getSynchronizationOperations().synchronizeSubject();
            else
                this.domainController.getSynchronizationOperations().markAsSynchronized(fileToDummySync);

            this.Invoke(new invoke_delegate_with_arg(setStartSynchronizingButtons), (object)false);

            this.enableComponents(true);

            this.subjectFilesSettingsControl.reload(true);
        }

        private void setStartSynchronizingButtons(object o)
        {
            Boolean value = Convert.ToBoolean(o);
            this.tableLayoutStartSync.Enabled = true;
            this.cmdStopSynchronization.Visible = value;
            this.cmdStopSynchronization.Enabled = value;
            this.cmdStartSynchronization.Visible = !value;
            if (!value)
            {
                this.lblSynchronisationStatus.Text = String.Empty;
                this.progressBarSyncTotal.Value = 0;
            }
        }

        private void loadSynchronizationTab()
        {
            if(this.domainController.isLoggedIn() && this.domainController.getUserInfo().getSelectedSubject() != null)
                this.subjectFilesSettingsControl.loadSubjectFolder(this.domainController.getUserInfo().getSelectedItemSubjectFolderForSubject(),false);
            else
                this.subjectFilesSettingsControl.unload();
        }

        private void cmdStopSynchronization_Click(object sender, EventArgs e)
        {
            this.domainController.cancelSynchronization();
            this.cmdStopSynchronization.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[]{ cmdStopSynchronization, false});
            this.lblSynchronisationStatus.Text = this.domainController.getLanguageString("stopping_synchronization");
        }

        #endregion

        #endregion

        #region Components enable/disable

        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void enableComponents(Boolean b)
        {
            this.componentsEnabled = b;
            
            if(b)
                this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), State.READY);
            else
                this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), State.BUSY);

            this.tooltipChecker.showTooltips = b;
            this.grbDocument.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[]{ grbDocument, b});
            this.grbLogin.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[]{ grbLogin, b});
            foreach (TabPage tabPage in this.tabs.TabPages)
            {
                // unused because it's unnecessary: any control which is disabled shows its sub-controls as disabled anyway.
                //this.setControlsEnabledRecursive(tabPage.Controls, b);

                foreach(Control control in tabPage.Controls)
                    control.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { control, b });
            }
            this.txtExerciseDump.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { txtExerciseDump, true });
            //this.menuBar.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[]{ menuBar, b});
            setTabComponentsEnabled();
        }

        private void setControlsEnabledRecursive(Control.ControlCollection controls, Boolean enabled)
        {
            foreach (Control control in controls)
            {
                control.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { control, enabled });
                setControlsEnabledRecursive(control.Controls, enabled);
            }
        }

        private void setTabComponentsEnabled()
        {
            
            if (!this.componentsEnabled) return;

            Boolean loggedin            = this.domainController.isLoggedIn();
            Boolean documentselected    = !this.domainController.getDocumentFullPath().Equals(String.Empty);
            Boolean onlineDestSelected  = loggedin && this.domainController.getUserInfo().hasSelectedItem();
            Boolean subjectSelected     = loggedin && this.domainController.getUserInfo().selectedItemIsSubject();
            //Controls login
            //this.cmbCourses.Invoke(new invoke_delegate_set_enabled(setControlEnabled), new object[] { cmbCourses,
            //    loggedin });

            this.setTabComponentsEnabledExercises(loggedin, documentselected, onlineDestSelected);
            this.setTabComponentsEnabledPdf(loggedin, documentselected, onlineDestSelected);
            this.setTabComponentsEnabledUpload(loggedin, documentselected, onlineDestSelected);
            this.setTabComponentsEnabledStats(onlineDestSelected, subjectSelected);
            this.setTabComponentsEnabledSynchronization(loggedin, subjectSelected);
        }

        private void setTabComponentsEnabledExercises(Boolean loggedin, Boolean documentselected, Boolean onlineDestSelected)
        {
            //exercises tabPage
            Boolean documentScanned     = this.domainController.exercisesAreScanned();
            Boolean documentHasErrors   = this.domainController.exercisesHaveErrors();
            Boolean canJumpToError      = this.domainController.canJumpToError();

            if (!loggedin)
                this.tooltipChecker.changeStringForControl(cmdUploadExercises, "not_logged_in");
            else if (!onlineDestSelected)
                this.tooltipChecker.changeStringForControl(cmdUploadExercises, "no_course_selected");
            else if (!documentselected)
                this.tooltipChecker.changeStringForControl(cmdUploadExercises, "no_document_selected");

            if (!documentScanned)
                this.tooltipChecker.changeStringForControl(cmdJumpToError, "no_document_scanned");
            else if (!documentHasErrors)
                this.tooltipChecker.changeStringForControl(cmdJumpToError, "no_errors_in_exercises");
            else if (!canJumpToError)
                this.tooltipChecker.changeStringForControl(cmdJumpToError, "not_supported_for_document_type");

            this.cmdScanDocument.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdScanDocument,
                documentselected });
            this.cmdReviewExercises.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdReviewExercises,
                documentselected && documentScanned });
            this.cmdUploadExercises.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdUploadExercises,
                loggedin && onlineDestSelected && documentselected});
            this.cmdJumpToError.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdJumpToError,
                documentselected && documentScanned && documentHasErrors && canJumpToError});
            
            this.chkOneQuestionPerPage.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkOneQuestionPerPage,
                loggedin && onlineDestSelected && documentselected});
            this.chkSetExerciseInvisible.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkSetExerciseInvisible,
                loggedin && onlineDestSelected && documentselected});
            this.chkRandomQuestions.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkRandomQuestions,
                loggedin && onlineDestSelected && documentselected});
            this.nrRandomQuestions.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { nrRandomQuestions,
                loggedin && onlineDestSelected && documentselected});
        }

        private void setTabComponentsEnabledPdf(Boolean loggedin, Boolean documentselected, Boolean onlineDestSelected)
        {
            //pdf tabPage
            if (!loggedin)
            {
                this.tooltipChecker.changeStringForControl(chkUpload, "not_logged_in");
            }
            else if (!onlineDestSelected)
            {
                this.tooltipChecker.changeStringForControl(chkUpload, "no_course_selected");
            }
            this.chkSplit.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkSplit,
                documentselected });
            this.txtSplit.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { txtSplit,
                documentselected && this.chkSplit.Checked });
            this.rdbPerPage.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { rdbPerPage,
                this.txtSplit.Enabled });
            this.rdbPerStyle.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { rdbPerStyle,
                this.rdbPerPage.Enabled });
            this.chkUpload.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkUpload,
                onlineDestSelected && documentselected });
            this.chkConvertHyperlinksToJavascript.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkConvertHyperlinksToJavascript,
                documentselected });
            this.documentsDropDownForPDF.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { documentsDropDownForPDF,
                onlineDestSelected && documentselected && this.chkUpload.Checked });
            this.cmdConvertToPDF.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdConvertToPDF,
                documentselected });
        }

        private void setTabComponentsEnabledUpload(Boolean loggedin, Boolean documentselected, Boolean courseSelected)
        {
            if (loggedin)
            {
                if (courseSelected)
                {
                    //this.uploadfilesToolStripMenuItem.ToolTipText = "";
                }
                else
                {
                    //this.uploadfilesToolStripMenuItem.ToolTipText = this.domainController.getLanguageString("no_course_selected");
                }
            }
            else
            {
                //this.uploadfilesToolStripMenuItem.ToolTipText = this.domainController.getLanguageString("not_logged_in");
            }

            //this.Invoke(new invoke_delegate_single_parameter(setToolStripItemEnabled), new object[] { this.uploadfilesToolStripMenuItem,loggedin && courseSelected });
        }

        private void setTabComponentsEnabledStats(Boolean onlineDestSelected, Boolean subjectSelected)
        {
            //Statistics tabPage

            /*
            if (subjectSelected)
            {
                //We won't allow calculation of statistics for subjects
                //override loggedin parameter!
                loggedin = false;
            }
            //*/
            Boolean statsButtonEnabled =onlineDestSelected && (this.chkCalculateExerciseStudentDetails.Checked || this.chkCalculatePercentageMC.Checked || this.chkCalculateResultsPerStudent.Checked);
            if (onlineDestSelected)
                this.tooltipChecker.changeStringForControl(cmdDownloadStatistics, "no_options_selected");
            else if (!onlineDestSelected)
                this.tooltipChecker.changeStringForControl(cmdDownloadStatistics, "no_course_selected");
            else
                this.tooltipChecker.changeStringForControl(cmdDownloadStatistics, "not_logged_in");
            this.cmdDownloadStatistics.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { cmdDownloadStatistics,
                statsButtonEnabled });
            this.chkCalculatePercentageMC.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkCalculatePercentageMC,
                onlineDestSelected});
            this.chkCalculateResultsPerStudent.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkCalculateResultsPerStudent,
                onlineDestSelected});
            this.chkCalculateExerciseStudentDetails.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkCalculateExerciseStudentDetails,
                onlineDestSelected});
            this.lblDoNotKnow.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { lblDoNotKnow,
                onlineDestSelected && this.chkCalculatePercentageMC.Checked });
            this.chkCPMCShowQuestionTitles.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkCPMCShowQuestionTitles,
                onlineDestSelected && this.chkCalculatePercentageMC.Checked });
            this.txtDoNotKnow.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { txtDoNotKnow,
                onlineDestSelected && this.chkCalculatePercentageMC.Checked });
            this.chkStatisticsCreateSubFolder.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkStatisticsCreateSubFolder,
                onlineDestSelected});
            this.lblColumns.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { lblColumns,
                onlineDestSelected && this.chkCalculateResultsPerStudent.Checked });
            this.lstColumns.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { lstColumns,
                onlineDestSelected && this.chkCalculateResultsPerStudent.Checked });
            this.chkGroups.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkGroups,
                onlineDestSelected && this.chkCalculateResultsPerStudent.Checked });
            this.lstGroups.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { lstGroups,
                onlineDestSelected && this.chkCalculateResultsPerStudent.Checked && this.chkGroups.Checked });
            this.chkGenerateAllAttempts.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkGenerateAllAttempts,
                onlineDestSelected && this.chkCalculateResultsPerStudent.Checked });
            this.chkOpenExcelFilesAfterConversion.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { chkOpenExcelFilesAfterConversion,
                onlineDestSelected});
            /*
            this.lblStatsError.Invoke(new invoke_delegate_single_parameter(setControlVisible), new object[] { lblStatsError,
                !onlineDestSelected});
            //*/
        }

        private void setTabComponentsEnabledSynchronization(Boolean loggedin, Boolean subjectSelected)
        {            
            this.Invoke(new invoke_delegate_single_parameter(setControlEnabledOnPanel), new object[] { this.subjectFilesSettingsControl, 
                subjectSelected });
            this.cmdStartSynchronization.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { this.cmdStartSynchronization, 
                subjectSelected });
            this.lblSynchronisationStatus.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { this.lblSynchronisationStatus, 
                subjectSelected });
            this.progressBarSyncTotal.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { this.progressBarSyncTotal, 
                subjectSelected });

            if(!loggedin)
                this.tooltipChecker.changeStringForControl(cmdStartSynchronization, "no_subject_selected");
            else if (!subjectSelected)
                this.tooltipChecker.changeStringForControl(cmdStartSynchronization, "not_logged_in");
        }        

        #endregion

        #region Private methods

        private void load_frame2(object refreshUI)
        {
            if (refreshUI is Boolean)
            {
                this.load_frame((Boolean)refreshUI);
            }
        }

        /// <summary>
        ///     Executed on login & logout
        /// </summary>
        private void load_frame()
        {
            this.load_frame(true);
        }

        /// <summary>
        ///     Executed on login & logout
        /// </summary>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void load_frame(Boolean refreshUI)
        {
            if (refreshUI)
                this.resetStrings();
            this.cmbCourses.Enabled = false;
            //Load the courses list...
            String[] list = this.domainController.getCoursesListForDropDown();
            
            this.cmbCourses.DataSource = list;
            if (this.cmbCourses.DataSource != null)
            {
                this.cmbCourses.SelectedIndex = this.domainController.getSelectedCourseDropdownIndex();
                this.documentsDropDownForPDF.setDataSource(this.domainController.getDocumentFoldersForDropDown());
                try
                {
                    this.loadGroupsForStatistics();
                }
                catch { this.lstGroups.Clear(); }
            }
            this.cmbCourses.Enabled = true;
            Boolean loggedIn = this.domainController.isLoggedIn();
            this.cmdLogin.Visible = !loggedIn;
            //this.loginToolStripMenuItem1.Enabled = !loggedIn;
            this.cmdLogout.Visible = loggedIn;
            //this.logoutToolStripMenuItem.Enabled = loggedIn;
            this.setTabComponentsEnabled();
            if (refreshUI)
                this.loadSynchronizationTab();
            // fix for the feature to reselect a previously selected course when logging in.
            // This obviously only works if no subject has been selected since then.
            if (loggedIn && cmbCourses.SelectedIndex == -1 && refreshUI)
            {
                cmbCourses.SelectedIndex = this.domainController.getUserInfo().reselectSavedCourse();
            }
            if (!loggedIn)
            {
                this.lstGroups.Clear();
            }
        }

        /// <remarks>
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        ///     
        ///     Updated on 10/08/2010 by Gianni Van Hoecke 
        /// </remarks>
        private void resetStrings()
        {
            //Menu bar language
            //StringsResetter.resetStaticStrings(menuBar.Items);
            
            //language for all other tagged items
            StringsResetter.resetStaticStrings(this.Controls);

            //Login
            if (this.domainController.isLoggedIn())
            {
                this.lblInformation.Text = domainController.getLanguageString("logged_in_as_x_on_y", new String[] { this.domainController.getUserInfo().getLogin().getUsername(), this.domainController.getPlatform().getPlatformName() });
                //As of 1.09
                this.lblInformation.ForeColor = ProgramConstants.getGreen();
                this.lblInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }
            else
            {
                this.lblInformation.Text = domainController.getLanguageString("not_logged_in");
                //As of 1.09
                this.lblInformation.ForeColor = Color.Black;
                this.lblInformation.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            }

            //Document
            this.updateDocumentLabel();

            //Exercises scan result
            this.lblExerciseScanResultsDump.Text = this.domainController.getExerciseScanResults();

            //Statistics do not know string
            this.txtDoNotKnow.Text = this.domainController.getLanguageString("do_not_know_string"); ;

            this.txtSplit.Text = this.domainController.getLanguageString("pdf_split_default");

            toolTip.SetToolTip(chkCalculatePercentageMC, this.domainController.getLanguageString("calculate_mp_percentage_tooltip"));
            toolTip.SetToolTip(chkCalculateResultsPerStudent, this.domainController.getLanguageString("calculate_results_per_student_tooltip"));
            toolTip.SetToolTip(chkCalculateExerciseStudentDetails, this.domainController.getLanguageString("calculate_exercise_details_per_student_tooltip"));
            toolTip.SetToolTip(chkSplit, this.domainController.getLanguageString("split_info"));
            toolTip.SetToolTip(txtSplit, this.domainController.getLanguageString("split_info"));
            this.subjectFilesSettingsControl.resetStrings();

            //statistics column names
            lstColumns.Items.Clear();
            lstColumns.Columns.Clear();
            lstColumns.CheckBoxes = true;

            ListViewItem itemNumber = new ListViewItem(domainController.getLanguageString("number"));
            itemNumber.Tag = "number";
            itemNumber.Checked = domainController.getSettings().getStatsColumnNumber();
            ListViewItem itemName = new ListViewItem(domainController.getLanguageString("name"));
            itemName.Tag = "name";
            itemName.Checked = domainController.getSettings().getStatsColumnName();
            ListViewItem itemEmail = new ListViewItem(domainController.getLanguageString("email"));
            itemEmail.Tag = "email";
            itemEmail.Checked = domainController.getSettings().getStatsColumnEmail();
            ListViewItem itemSN = new ListViewItem(domainController.getLanguageString("student_number"));
            itemSN.Tag = "student_number";
            itemSN.Checked = domainController.getSettings().getStatsColumnStudentNumber();
            ListViewItem itemGroup = new ListViewItem(domainController.getLanguageString("group"));
            itemGroup.Tag = "group";
            itemGroup.Checked = domainController.getSettings().getStatsColumnGroup();

            for (int i = 0; i < 5; i++)
            {
                if(domainController.getSettings().getStatsColumnNameID() == i)
                    lstColumns.Items.Add(itemName);
                else if (domainController.getSettings().getStatsColumnNumberID() == i)
                    lstColumns.Items.Add(itemNumber);
                else if (domainController.getSettings().getStatsColumnEmailID() == i)
                    lstColumns.Items.Add(itemEmail);
                else if (domainController.getSettings().getStatsColumnStudentNumberID() == i)
                    lstColumns.Items.Add(itemSN);
                else if (domainController.getSettings().getStatsColumnGroupID() == i)
                    lstColumns.Items.Add(itemGroup);
            }

            //We need to specify a column header (and set it invisible) if we want to scroll vertically.
            //If we don't do this, we're only able to scroll horizontally!
            lstColumns.Columns.Add("", -1);
            lstColumns.HeaderStyle = ColumnHeaderStyle.None;

            foreach (ColumnHeader column in this.lstGroups.Columns)
                column.Text = domainController.getLanguageString(Convert.ToString(column.Tag));
        }

        private void updateDocumentLabel()
        {
            if (domainController.getDocumentFullPath().Equals(String.Empty))
                this.lblDocumentSelected.Text = domainController.getLanguageString("no_document_selected");
            else
            {
                String name = new FileInfo(this.domainController.getDocumentFullPath()).Name;
                this.lblDocumentSelected.Text = domainController.getLanguageString("selected_document_is_x", new String[] {name});
                //Reset previous results...
                this.lblExerciseScanResultsDump.Text = this.domainController.getExerciseScanResults();
                this.txtExerciseDump.Text = String.Empty;
            }
        }

        private void checkForOpenDocuments()
        {
            this.contextMenuStrip = new ContextMenuStrip();
            ToolStripMenuItem toolStripMenuItem;

            //Voeg een optie om naar een document te bladeren toe...
            toolStripMenuItem = new ToolStripMenuItem(this.domainController.getLanguageString("browse"));
            toolStripMenuItem.Click += new System.EventHandler(this.openDocument_ItemClicked);
            this.contextMenuStrip.Items.Add(toolStripMenuItem);

            //Haalt de open documenten op...
            String[] documents = this.domainController.getAllOpenDocuments();

            //Voeg een separator toe...
            if(documents.Length > 0)
                this.contextMenuStrip.Items.Add(new ToolStripSeparator());

            //Voeg alle open documenten toe aan de lijst...
            foreach (String path in documents)
            {
                toolStripMenuItem = new ToolStripMenuItem(path);
                toolStripMenuItem.Click += new System.EventHandler(this.openDocument_ItemClicked);
                this.contextMenuStrip.Items.Add(toolStripMenuItem);
            }
        }

        private Boolean platformIsSet() 
        { 
            return !domainController.getSettings().getPlatform().Equals(String.Empty)
                 && !domainController.getSettings().getUrl().Equals(String.Empty);
        }

        private void doScanDocument()
        {
            
            try
            {
                this.enableComponents(false);
                this.domainController.checkDocument(false, true);
                this.tabs.Invoke(new invoke_delegate_with_arg(switchTab),new Object[]{0});
                this.updateExerciseScanResults(this.domainController.getExerciseScanResults());
                this.enableComponents(true);
                this.txtLog.Invoke(new invoke_delegate_control_command(clearTextBoxBaseSelection), new object[]{ txtLog });
                this.cmdChooseDocument.Invoke(new invoke_delegate_control_command(selectControl), new object[]{ cmdChooseDocument });
            }
            catch (Exception ex)
            {
                this.domainController.processError(ex, true);
                this.enableComponents(true);
            }
        }

        private void doUploadExercises(Object objects)
        {
            try
            {
                this.enableComponents(false);
                Object[] parameters = (Object[])objects;
                //onePerPage, setInvisible, randomquestions
                Boolean onePerPage = (Boolean)parameters[0];
                Boolean setInvisible = (Boolean)parameters[1];
                int randomquestions = (int)parameters[2];
                
                if (this.domainController.exercisesAreScanned() && !this.domainController.exercisesHaveErrors())
                    this.domainController.uploadExercises
                        (true, onePerPage, setInvisible, randomquestions);
                else
                    this.domainController.convertAndUploadExercisesDocument
                        (onePerPage, setInvisible, randomquestions);
            }
            catch (Exception ex)
            {
                this.domainController.processError(ex, true);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void doUploadFile(Object objects)
        {
            try
            {
                Object[] parameters = (Object[])objects;
                //localPath, description, saveToSubDir, setInvisible
                String filename = (String)parameters[0];
                String description = (String)parameters[1];
                DocumentFolder documentFolders = (DocumentFolder)parameters[2];
                Boolean setInvisible = (Boolean)parameters[3];
                this.enableComponents(false);
                this.domainController.uploadFile(filename, documentFolders, description, setInvisible);
            }
            catch (Exception ex)
            {
                this.domainController.processError(ex, true);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void doOpenTemplate(Object useExamplesTemplate)
        {
            if (useExamplesTemplate is Boolean)
            {
                this.enableComponents(false);
                this.domainController.openTemplateFile((Boolean)useExamplesTemplate);
                this.enableComponents(true);

            }
        }
        
        private void doJumpToError()
        {
            this.enableComponents(false);
            this.domainController.jumpToError();
            this.enableComponents(true);
        }

        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void doUpdateCourses(Object selectedItem)
        {
            try
            {
                UserInfo info = this.domainController.getUserInfo();
                this.enableComponents(false);
                
                if (selectedItem != null)
                    info.setSelectedItemFromDropdownString(selectedItem.ToString());
                else
                    info.clearSelectedItem();

                this.domainController.getSettings().setCoursecode(this.domainController.getUserInfo().getSelectedItemSaveName());
                this.setLastSelectedFolder(info.getSelectedItemSubjectFolder());
                this.domainController.loadDocumentFolders(true);
                this.domainController.downloadGroups();
                String[] ds = this.domainController.getDocumentFoldersForDropDown();
                this.documentsDropDownForPDF.Invoke(new invoke_delegate_set_datasource(setDocumentsDropDownDataSource), new object[]{ documentsDropDownForPDF, ds});
                this.Invoke(new invoke_delegate(loadSynchronizationTab));
                if(domainController.isLoggedIn())
                    this.Invoke(new invoke_delegate(loadGroupsForStatistics));
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void loadGroupsForStatistics()
        {
            lstGroups.Clear();

            lstGroups.Columns.Add(domainController.getLanguageString("name"), -1);
            lstGroups.Columns.Add(domainController.getLanguageString("id"), -2);
            lstGroups.Columns.Add(domainController.getLanguageString("students"), -2);

            lstGroups.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lstGroups.Columns[1].TextAlign = HorizontalAlignment.Right;
            lstGroups.Columns[2].TextAlign = HorizontalAlignment.Right;

            foreach(Groups groups in domainController.getUserInfo().getGroups())
            {
                foreach(Group group in groups.groups)
                {
                    ListViewItem item = new ListViewItem(new String[]{ group.name, group.id, Convert.ToString(group.studentCount) });
                    item.Checked = true;
                    item.Tag = group.uniqueName; //We use this for reffering!
                    lstGroups.Items.Add(item);
                }
            }

            lstGroups.Columns[0].Width = -1;
            lstGroups.Columns[0].Tag = "name";
            lstGroups.Columns[1].Width = -2;
            lstGroups.Columns[1].Tag = "id";
            lstGroups.Columns[2].Width = -2;
            lstGroups.Columns[2].Tag = "students";
        }

        private void setLastSelectedFolder(String path)
        {
            if (path != null && !path.Equals(String.Empty))
                this.lastSelectedDocumentFolder = path;
            else
                this.lastSelectedDocumentFolder = this.domainController.getSettings().getSubjectsFolder();
        }

        private void setStatusText(object s)
        {
            this.statusBusyText = this.domainController.getLanguageString("busy");
            State state = (State)s;
            if (state == State.READY)
            {
                timer.Stop();
                lblStatus.Text = this.domainController.getLanguageString("ready");
                lblStatus.ForeColor = ProgramConstants.getGreen();
            }
            else
            {
                lblStatus.Text = this.statusBusyText + "   ";
                lblStatus.ForeColor = ProgramConstants.getRed();
                this.ticks = 1;
                timer.Start();
            }
        }

        public void busyTick(object sender,EventArgs eArgs)
        {
            if(sender == this.timer)
            {
                this.ticks++;
                this.lblStatus.ForeColor = ProgramConstants.getRed();

                if(ticks == 1)
                    lblStatus.Text = this.statusBusyText + "   ";
                else if(ticks == 2)
                    lblStatus.Text = this.statusBusyText + ".  ";
                else if (ticks == 3)
                    lblStatus.Text = this.statusBusyText + ".. ";
                else
                {
                    lblStatus.Text = this.statusBusyText + "...";
                    this.ticks = 0;
                }
            }
        }

        private Boolean listHasItemsSelected(ListControl listcontrol)
        {
            if (listcontrol is ComboBox)
            {
                ComboBox cb = (ComboBox)listcontrol;
                return ((cb.Items.Count > 0) && (cb.SelectedIndex > -1));
            }
            else if (listcontrol is ListBox)
            {
                ListBox lb = (ListBox)listcontrol;
                return ((lb.Items.Count > 0) && (lb.SelectedIndices.Count > 0));
            }
            else return false;
        }
        
        private void doConvertToPDF(Object objects)
        {
            try
            {
                String[] parameters = (String[])objects;
                String splitText = txtSplit.Text;
                DocumentFolder documentUploadFolder = null;
                Boolean split = chkSplit.Checked;
                Boolean upload = chkUpload.Checked && chkUpload.Enabled;
                // adapt UI to support PDF invisibility?
                // disabled for now.
                Boolean setInvisible = false;
                Boolean splitOnPage = false;
                Boolean convertHyperlinksToJavascript = this.chkConvertHyperlinksToJavascript.Checked;

                if(this.rdbPerPage.Checked)
                        splitOnPage = true;
                
                if (upload)
                    documentUploadFolder = 
                        this.domainController.getDocumentFoldersList().getDocumentFolderFromFolderName(parameters[0]);

                this.enableComponents(false);
                Boolean uploadSucceeded = false;
                Boolean error = this.domainController.convertToPDF(split, splitText, splitOnPage, documentUploadFolder, convertHyperlinksToJavascript, setInvisible, ref uploadSucceeded);
                if (upload && !uploadSucceeded) { } // no use for this atm, since convertToPDF function throws message box

                error = this.documentsDropDownForPDF.InvokeRequired;

                this.documentsDropDownForPDF.Invoke(new invoke_delegate_single_parameter(setDocumentsDropDownValue), new object[] { documentsDropDownForPDF, parameters[0] });
            }
            catch (Exception ex)
            {
                this.domainController.processError(ex, true);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void scrollToLogBottom()
        {
            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.ScrollToCaret();
        }

        /// <remarks>
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void setTextValue(Object c, object value)
        {
            if (!(c is Control))
                return;

            ((Control)c).Text = Convert.ToString(value);
        }

        private void setDocumentsDropDownValue(Object c, object value)
        {
            if (!(c is DocumentsDropDown))
                return;

            ((DocumentsDropDown)c).setSelectedItem(Convert.ToString(value));
        }

        private void openLoginFrame()
        {
            Application.Run(new LoginFrame(true));
        }

        private void setDump(object value)
        {
            txtLog.Text = Convert.ToString(value);
            txtLog.SelectionStart = 0;
            txtLog.SelectionLength = 0;
        }

        private void switchTab(Object index)
        { 
            this.tabs.SelectedIndex = (int)index;
        }

        private void setDocumentsDropDownDataSource(DocumentsDropDown ddr, String[] ds)
        {
             ddr.setDataSource(ds);
        }

        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void setToolStripItemEnabled(Object c, object b)
        {
            if (!(c is ToolStripItem))
                return;

            if (b is Boolean)
                ((ToolStripItem)c).Enabled = (Boolean)b;
        }

        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void setControlEnabled(Object obj, object b)
        {
            if (!(obj is Control))
                return;

            Control c = (Control)obj;

            if (b is Boolean)
            {
                c.Enabled = (Boolean)b;
                if (c is ListBox)
                {
                    try
                    {
                        ListBox lb = (ListBox)c;
                        if (!lb.Enabled)
                            lb.BackColor = SystemColors.Control;
                        else
                            lb.BackColor = SystemColors.Window;
                    }
                    catch { }
                }
            }
        }

        private void setControlEnabledOnPanel(Object obj, object b)
        {
            if (!(obj is SubjectFilesSettingsControl))
                return;

            SubjectFilesSettingsControl c = (SubjectFilesSettingsControl)obj;

            if (b is Boolean)
            {
                c.enablePanel((Boolean)b);
            }
        }

        private void setControlVisible(Object obj, object b)
        {
            if (!(obj is Control))
                return;

            Control c = (Control)obj;

            c.Visible = Convert.ToBoolean(b);
        }

        private void selectControl(Control c)
        {
            c.Select();
        }

        private void clearTextBoxBaseSelection(Control c)
        {
            TextBoxBase tbb = (TextBoxBase)c;
            tbb.SelectionStart = 0;
            tbb.SelectionLength = 0;
        }

        private void showMessageBox(object o)
        {
            String message = Convert.ToString(((object[])o)[0]);
            MessageBoxIcon icon = (MessageBoxIcon)((object[])o)[1];
            MessageBox.Show(this, Convert.ToString(message), this.Text, MessageBoxButtons.OK, icon);
        }

        private Boolean showQuestionMessageBox(String message, Boolean okcancel)
        {
            DialogResult diag = MessageBox.Show(this, message, this.Text, (okcancel ? MessageBoxButtons.OKCancel : MessageBoxButtons.YesNo), MessageBoxIcon.Question);
            if(diag == DialogResult.OK || diag == DialogResult.Yes)
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Opent een folder browser.
        /// </summary>
        /// <returns>Het pad naar de geselecteerde folder.</returns>
        private String openFolderBrowser(String path)
        {
            this.folderBrowserDialog.SelectedPath = (path!=null ? path : String.Empty);

            if(this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
                return this.folderBrowserDialog.SelectedPath;
            else
                return String.Empty;
        }

        private void chkGroups_CheckedChanged(object sender, EventArgs e)
        {
            this.lstGroups.Enabled = this.chkGroups.Checked;
        }

        #endregion

        #region MVC

        public void dumpText(String text)
        {
            this.txtExerciseDump.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[] { txtExerciseDump, text });
        }

        public void showMessage(String message, MessageBoxIcon icon)
        {
            this.Invoke(new invoke_delegate_with_arg(showMessageBox), (object)new object[]{message, icon});
        }

        public Boolean showQuestionMessage(String message, Boolean okcancel)
        {
            return Convert.ToBoolean(this.Invoke(new invoke_delegate_for_questionbox(showQuestionMessageBox), message, okcancel));
        }

        public void updateLog(String value)
        {
            try
            {
                this.txtLog.Invoke(new invoke_delegate_with_arg(setDump), value);
            }
            catch(Exception){}
        }

        public void update(Boolean refreshUI)
        {
            this.Invoke(new invoke_delegate_with_arg(this.load_frame2), refreshUI);
        }

        public void setState(State state)
        {
            this.lblStatus.Invoke(new invoke_delegate_with_arg(setStatusText), state);
        }

        public void updateSynchronizationStatus(int valueSingleFile, String status)
        {
            object o = new object[]{ valueSingleFile, status };
            this.Invoke(new invoke_delegate_with_arg(setSynchronizationStatus), o);
        }

        private void setSynchronizationStatus(object o)
        {
            int progress = Convert.ToInt32(((object[])o)[0]);
            String status = Convert.ToString(((object[])o)[1]);

            this.progressBarSyncTotal.Value = (progress > 100) ? 100 : progress;
            this.lblSynchronisationStatus.Text = status;
        }

        public void updateExerciseScanResults(String value)
        { 
            this.lblExerciseScanResultsDump.Invoke(new invoke_delegate_single_parameter(setTextValue), new object[]{ lblExerciseScanResultsDump, value });
        }

        #endregion

        private void menuBar_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnInicioSesion_Click(object sender, EventArgs e)
        {
            if (!this.platformIsSet())
            {
                MessageBox.Show(this.domainController.getLanguageString("configure_platform"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.setTabComponentsEnabled();
            }
            else
            {
                new LoginFrame().ShowDialog();
                this.load_frame(); // does a setTabComponentsEnabled()
            }
            OcultarSubMenu();
        }

        private void btnAcciones_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(panelSubMenuAcciones);
        }

        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            this.cmdLogout_Click(sender, e);
            OcultarSubMenu();
        }

        private void btnSubirArchivos_Click(object sender, EventArgs e)
        {
            new UploadFileFrame().ShowDialog();
            OcultarSubMenu();
        }

        private void btnVerRegistro_Click(object sender, EventArgs e)
        {
            new LogViewerFrame().ShowDialog();
            OcultarSubMenu();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            this.Close();
            OcultarSubMenu();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(panelSubMenuEditar);
        }

        private void btnPreferencias_Click(object sender, EventArgs e)
        {
            String currentPlatform = this.domainController.getSettings().getPlatform();
            String currentURL = this.domainController.getSettings().getUrl();
            SettingsFrame settingsFrame = new SettingsFrame();
            settingsFrame.ShowDialog();
            resetStrings();
            String newPlatform = this.domainController.getSettings().getPlatform();
            String newURL = this.domainController.getSettings().getUrl();
            if ((!currentPlatform.Equals(newPlatform) || !currentURL.Equals(newURL)) && this.domainController.isLoggedIn())
            {
                this.cmdLogout_Click(sender, e);
                this.domainController.writeToLog("platform_changed_logged_out", true, false, true);
            }
            else
            {
                this.setTabComponentsEnabled();
            }
            OcultarSubMenu();
        }

        private void btnAdministrarMaterias_Click(object sender, EventArgs e)
        {
            new ManageSubjectsFrame().ShowDialog();

            //Load the courses list...
            String[] list = this.domainController.getCoursesListForDropDown();
            Object selectedItem = cmbCourses.SelectedItem;

            Subject subject = domainController.getUserInfo().getSelectedSubject();
            Boolean cmbCoursesEnabled = cmbCourses.Enabled;

            cmbCourses.Enabled = false;
            this.cmbCourses.DataSource = list;
            this.cmbCourses.SelectedIndex = -1;
            cmbCourses.Enabled = cmbCoursesEnabled;
            if (this.cmbCourses.DataSource != null)
            {
                // setting cmbCourses SelectedItem or SelectedIndex triggers cmbCourses_SelectedIndexChanged,
                // which handles all the rest.
                if (subject != null && this.cmbCourses.Items.Contains(subject.getSubjectName()))
                    this.cmbCourses.SelectedItem = subject.getSubjectName();
                else if (selectedItem != null && this.cmbCourses.Items.Contains(selectedItem))
                    this.cmbCourses.SelectedItem = selectedItem;
                else
                {
                    // Index is -1: manually trigger courses list update
                    Thread thread = new Thread(new ParameterizedThreadStart(doUpdateCourses));
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start(this.cmbCourses.SelectedItem);
                }

            }
            OcultarSubMenu();
        }

        private void btnAyuda2_Click(object sender, EventArgs e)
        {
            Thread downloadThread = new Thread(new ThreadStart(showHelp));
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.Start();
            OcultarSubMenu();
        }

        private void btnBuscarActualizaciones_Click(object sender, EventArgs e)
        {
            Thread downloadThread = new Thread(new ThreadStart(checkForUpdates));
            downloadThread.SetApartmentState(ApartmentState.STA);
            downloadThread.Start();
            OcultarSubMenu();
        }

        private void btnAcercaDeLMSDA_Click(object sender, EventArgs e)
        {
            AboutFrame aboutFrame = new AboutFrame();
            aboutFrame.ShowDialog();
            OcultarSubMenu();
        }

        private void btnAyuda_Click(object sender, EventArgs e)
        {
            MostrarSubMenu(panelSubMenuAyuda);
        }

        private void cmdLogin_Click_1(object sender, EventArgs e)
        {
            if (!this.platformIsSet())
            {
                MessageBox.Show(this.domainController.getLanguageString("configure_platform"), this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.setTabComponentsEnabled();
            }
            else
            {
                new LoginFrame().ShowDialog();
                this.load_frame(); // does a setTabComponentsEnabled()
            }
        }

        private void cmdChooseDocument_Click_1(object sender, EventArgs e)
        {
            cmdChooseDocument_MouseDown(sender, null);
        }

        private void cmdOpenTemplate_Click_1(object sender, EventArgs e)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(doOpenTemplate));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(this.chkNewDocWithExamples.Checked);
            this.chkNewDocWithExamples.Checked = false;
        }

        private void cmdScanDocument_Click_1(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                txtExerciseDump.Text = String.Empty;
                Thread thread = new Thread(new ThreadStart(doScanDocument));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }

        private void cmdReviewExercises_Click_1(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet() && this.domainController.exercisesAreScanned())
            {
                new ExerciseReviewFrame().ShowDialog();
            }
        }

        private void cmdJumpToError_Click_1(object sender, EventArgs e)
        {
            if (domainController.canJumpToError())
            {
                Thread thread = new Thread(new ThreadStart(doJumpToError));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void cmdUploadExercises_Click_1(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                Boolean onePerPage = this.chkOneQuestionPerPage.Checked;
                Boolean setInvisible = this.chkSetExerciseInvisible.Checked;
                int randomquestions = (chkRandomQuestions.Checked ? Convert.ToInt32(nrRandomQuestions.Value) : 0);

                Thread thread = new Thread(new ParameterizedThreadStart(doUploadExercises));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(new Object[] { onePerPage, setInvisible, randomquestions });
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }

        private void chkOneQuestionPerPage_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkOneQuestionPerPage.Enabled)
                this.domainController.getSettings().setExercisesMultiPage(chkOneQuestionPerPage.Checked);
        }

        private void chkRandomQuestions_CheckedChanged_1(object sender, EventArgs e)
        {
            this.nrRandomQuestions.Visible = this.chkRandomQuestions.Checked;
        }

        private void chkSplit_CheckedChanged_1(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
        }

        private void chkUpload_CheckedChanged_1(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
        }

        private void cmdConvertToPDF_Click_1(object sender, EventArgs e)
        {
            if (this.domainController.isDocumentSet())
            {
                Thread thread = new Thread(new ParameterizedThreadStart(doConvertToPDF));
                thread.SetApartmentState(ApartmentState.STA);
                String saveToSubDir = String.Empty;

                if (this.documentsDropDownForPDF.getComboBox().Items.Count > 0)
                    saveToSubDir = this.documentsDropDownForPDF.getSelectedItem();

                thread.Start(new String[] { saveToSubDir });
            }
            else
            {
                this.domainController.writeToLog("no_document_selected", true, false, true);
            }
        }

        private void chkCalculateResultsPerStudent_CheckedChanged_1(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (this.chkCalculateResultsPerStudent.Enabled)
                this.domainController.getSettings().setStatsCalculateResultsPerStudent(this.chkCalculateResultsPerStudent.Checked);
        }

        private void chkGenerateAllAttempts_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkGenerateAllAttempts.Enabled)
                this.domainController.getSettings().setStatsShowAllAttempts(this.chkGenerateAllAttempts.Checked);
        }

        private void chkGroups_CheckedChanged_1(object sender, EventArgs e)
        {
            this.lstGroups.Enabled = this.chkGroups.Checked;
        }

        private void chkCalculateExerciseStudentDetails_CheckedChanged_1(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (chkCalculateExerciseStudentDetails.Enabled)
                this.domainController.getSettings().setStatsCalculateExerciseDetailsPerStudent(this.chkCalculateExerciseStudentDetails.Checked);
        }

        private void chkCalculatePercentageMC_CheckedChanged_1(object sender, EventArgs e)
        {
            this.setTabComponentsEnabled();
            if (chkCalculatePercentageMC.Enabled)
                this.domainController.getSettings().setStatsCalculateMCPercentage(this.chkCalculatePercentageMC.Checked);
        }

        private void chkCPMCShowQuestionTitles_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkCPMCShowQuestionTitles.Enabled)
                this.domainController.getSettings().setStatsShowQuestionTitles(this.chkCPMCShowQuestionTitles.Checked);
        }

        private void chkStatisticsCreateSubFolder_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkStatisticsCreateSubFolder.Enabled)
                this.domainController.getSettings().setStatsCreateSubFolder(this.chkStatisticsCreateSubFolder.Checked);
        }

        private void chkOpenExcelFilesAfterConversion_CheckedChanged_1(object sender, EventArgs e)
        {
            if (chkOpenExcelFilesAfterConversion.Enabled)
                this.domainController.getSettings().setStatsOpenExcelAfterConversion(this.chkOpenExcelFilesAfterConversion.Checked);
        }

        private void cmdDownloadStatistics_Click_1(object sender, EventArgs e)
        {
            Boolean exists = false;

            String localPath = String.Empty;

            if (this.chkStatisticsCreateSubFolder.Checked)
                localPath = this.domainController.getSettings().getStatsFolder();
            else
                localPath = this.openFolderBrowser(this.domainController.getSettings().getStatsFolder());

            if (this.chkCalculatePercentageMC.Checked)
                this.domainController.getSettings().setStatsIDontKnowString(this.txtDoNotKnow.Text);

            //As of 1.09
            //Save the order and checked columns.
            for (int i = 0; i < this.lstColumns.Items.Count; i++)
            {
                ListViewItem item = this.lstColumns.Items[i];
                if (item.Tag.Equals("number"))
                {
                    domainController.getSettings().setStatsColumnNumber(item.Checked);
                    domainController.getSettings().setStatsColumnNumberID(i);
                }
                else if (item.Tag.Equals("name"))
                {
                    domainController.getSettings().setStatsColumnName(item.Checked);
                    domainController.getSettings().setStatsColumnNameID(i);
                }
                else if (item.Tag.Equals("email"))
                {
                    domainController.getSettings().setStatsColumnEmail(item.Checked);
                    domainController.getSettings().setStatsColumnEmailID(i);
                }
                else if (item.Tag.Equals("student_number"))
                {
                    domainController.getSettings().setStatsColumnStudentNumber(item.Checked);
                    domainController.getSettings().setStatsColumnStudentNumberID(i);
                }
                else if (item.Tag.Equals("group"))
                {
                    domainController.getSettings().setStatsColumnGroup(item.Checked);
                    domainController.getSettings().setStatsColumnGroupID(i);
                }
            }

            //There's at least one required column; username OR name OR e-mail address
            if (!domainController.getSettings().getStatsColumnStudentNumber() &&
                !domainController.getSettings().getStatsColumnName() &&
                !domainController.getSettings().getStatsColumnEmail()
                && this.chkCalculateResultsPerStudent.Checked)
            {
                domainController.fireMessageBox(domainController.getLanguageString("required_column_missing"), MessageBoxIcon.Exclamation);
            }
            else
            {
                try
                {
                    exists = new DirectoryInfo(localPath).Exists;
                }
                catch { }

                if (exists)
                {
                    //Prepare groups (as of 1.09)                    
                    if (this.chkGroups.Checked && this.chkCalculateResultsPerStudent.Checked)
                    {
                        for (int i = 0; i < this.lstGroups.Items.Count; i++)
                        {
                            ListViewItem item = this.lstGroups.Items[i];
                            foreach (Groups groups in domainController.getUserInfo().getGroups())
                            {
                                Group group = groups.getGroup(Convert.ToString(item.Tag));
                                group.groupNumber = i;
                                if (group != null)
                                    group.enabled = item.Checked;
                            }
                        }
                    }

                    Thread thread = new Thread(new ParameterizedThreadStart(exportStatistics));
                    thread.SetApartmentState(ApartmentState.STA);
                    object objects = new object[]   {
                                                        localPath,
                                                        this.chkCalculatePercentageMC.Checked,
                                                        this.chkCalculateResultsPerStudent.Checked,
                                                        this.chkCalculateExerciseStudentDetails.Checked,
                                                        this.chkCPMCShowQuestionTitles.Checked,
                                                        this.txtDoNotKnow.Text,
                                                        this.chkStatisticsCreateSubFolder.Checked,
                                                        this.chkGenerateAllAttempts.Checked,
                                                        this.chkGroups.Checked
                                                    };
                    thread.Start(objects);
                }
                else
                {
                    this.domainController.writeToLog("no_destination_selected", true, false, true);
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
