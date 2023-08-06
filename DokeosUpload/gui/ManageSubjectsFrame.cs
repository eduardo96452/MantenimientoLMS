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
using System.Drawing;
using System.Windows.Forms;
using lmsda.domain;
using lmsda.domain.util;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    /// <remarks>
    ///     Last updated on 09/08/2010 by Gianni Van Hoecke
    ///      -> You can select multiple items in each list.
    /// </remarks>
    public partial class ManageSubjectsFrame : Form
    {
        private DomainController domainController;

        private Double upbuttonEnd = 0;
        private Double downButtonStart = 0;
        private int    upDownYFromBottom = 0;
        private int    createYFromBottom = 0;
        private int    editYFromBottom = 0;
        private int    removeYFromBottom = 0;
        private Double subjectsEnd = 0;
        private int    subjectsBottom = 0;
        private Double selectedCoursesStart = 0;
        private Double selectedCoursesEnd = 0;
        private int    selectedCoursesBottom = 0;
        private Double buttonsStart = 0;
        private Double buttonsEnd = 0;
        private Double availableCoursesStart = 0;
        private int    availableCoursesLeft = 0;
        private int    availableCoursesBottom = 0;
        
        public ManageSubjectsFrame()
        {
            InitializeComponent();
            this.domainController = DomainController.Instance();
        }

        private void ManageSubjectsFrame_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.lmsda_icon;
            StringsResetter.resetStaticStrings(this);
            this.saveSubjectsTabOffsets();
            toolTip.SetToolTip(cmdAddCourse, this.domainController.getLanguageString("add_course_to_subject"));
            toolTip.SetToolTip(cmdRemoveCourse, this.domainController.getLanguageString("remove_course_from_subject"));
            this.loadSubjects();
            if(this.lstSubject.Items.Count > 0)
                this.lstSubject.SelectedIndex = 0;
        }

        private void loadSubjects()
        {
            //Clear the list
            this.lstSubject.Items.Clear();

            //Fill the list
            List<String> subjects = this.domainController.getUserInfo().getSubjects().getSubjectNames();
            foreach(String sj in subjects)
                this.lstSubject.Items.Add(sj);
            // refresh other lists
            refreshSubjectTabCoursesLists();
        }

        /// <remarks>
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        ///      -> Courses can be linked to multiple subjects
        /// </remarks>
        private void refreshSubjectTabCoursesLists()
        {
            String selectedSubject = Convert.ToString(this.lstSubject.SelectedItem);

            if (!this.domainController.isLoggedIn())
            {
                this.lstSelectedCourses.Items.Clear();
                this.lstSelectedCourses.SelectedIndex = -1;

                this.lstAvailableCourses.Items.Clear();
                this.lstAvailableCourses.Items.Add(domainController.getLanguageString("no_courses_available1"));
                this.lstAvailableCourses.Items.Add(domainController.getLanguageString("no_courses_available2"));
                this.lstAvailableCourses.SelectedIndex = -1;
            }
            else
            {
                Object selectedItem = lstSelectedCourses.SelectedItem;
                String[] coursesForSubject = this.domainController.getUserInfo().getCoursesForSubject(selectedSubject);
                this.lstSelectedCourses.DataSource = coursesForSubject;

                /*if (selectedItem != null && this.lstSelectedCourses.Items.Contains(selectedItem))
                    this.lstSelectedCourses.SelectedItem = selectedItem;
                else */
                if (this.lstSelectedCourses.Items.Count > 0)
                    this.lstSelectedCourses.SelectedIndex = 0;

                selectedItem = lstAvailableCourses.SelectedItem;
                
                String[] courses = this.domainController.getUserInfo().getCoursesNotInSubject(selectedSubject);
                //String[] courses = this.domainController.getUserInfo().getCoursesNotInSubjects();
                
                this.lstAvailableCourses.DataSource = courses;

                /*if (selectedItem != null && this.lstAvailableCourses.Items.Contains(selectedItem))
                    this.lstAvailableCourses.SelectedItem = selectedItem;
                else */
                if (this.lstAvailableCourses.Items.Count > 0)
                    this.lstAvailableCourses.SelectedIndex = 0;
            }

            this.setTabComponentsEnabledSubjects(domainController.isLoggedIn());
        }

        private void setTabComponentsEnabledSubjects(Boolean loggedin)
        { 
            Boolean subjectSelected     = listHasItemsSelected(lstSubject);
            Boolean allowRemove         = listHasItemsSelected(lstSelectedCourses);
            Boolean allowAdd            = listHasItemsSelected(lstAvailableCourses);
            Boolean allowMoveUp         = (lstSubject.Items.Count > 0) && (lstSubject.SelectedIndex > 0);
            Boolean allowMoveDown       = (lstSubject.Items.Count > 0) && (lstSubject.SelectedIndex >= 0)
                                            && (lstSubject.SelectedIndex < lstSubject.Items.Count-1);

            //Categories tabPage
            this.lstSelectedCourses.Enabled =   loggedin;
            this.lblSelectedCourses.Enabled =   loggedin;
            this.lstAvailableCourses.Enabled =  loggedin;
            this.lblAvailableCourses.Enabled =  loggedin;
            this.cmdRemoveSubject.Enabled =     subjectSelected;
            this.cmdEditSubject.Enabled =       subjectSelected;
            this.cmdAddCourse.Enabled =         loggedin && subjectSelected && allowAdd;
            this.cmdRemoveCourse.Enabled =      loggedin && subjectSelected && allowRemove;
            this.cmdOpenSubjectFolder.Enabled = !txtSubjectFolder.Text.Equals(String.Empty);
            this.cmdMoveSubjectUp.Enabled =     subjectSelected && allowMoveUp;
            this.cmdMoveSubjectDown.Enabled =   subjectSelected && allowMoveDown;
        }


        private Boolean listHasItemsSelected(ListBox listBox)
        {
            return ((listBox.Items.Count > 0) && (listBox.SelectedIndices.Count > 0));
        }

        private void ManageSubjectsFrame_Resize(object sender, EventArgs e)
        {
            this.loadSubjectsTabOffsets();
            this.Refresh();
        }


        /// <summary>
        ///     Saving of relative and absolute offsets to allow dynamic resize of all elements on the Subjects tab.
        /// </summary>
        private void saveSubjectsTabOffsets()
        {

            upbuttonEnd = (double)(cmdMoveSubjectUp.Location.X + cmdMoveSubjectUp.Width) / (double)this.Width;
            downButtonStart = (double)cmdMoveSubjectDown.Location.X / (double)this.Width;
            upDownYFromBottom = this.Height - cmdMoveSubjectDown.Location.Y;
            createYFromBottom = this.Height - cmdCreateSubject.Location.Y;
            editYFromBottom   = this.Height - cmdEditSubject.Location.Y;
            removeYFromBottom = this.Height - cmdRemoveSubject.Location.Y;
            subjectsEnd = (double)(lstSubject.Location.X + lstSubject.Width) / (double)this.Width;
            subjectsBottom = this.Height - (lstSubject.Location.Y + lstSubject.Height);
            selectedCoursesStart = (double)lstSelectedCourses.Location.X / (double)this.Width;
            selectedCoursesEnd = (double)(lstSelectedCourses.Location.X + lstSelectedCourses.Width) / (double)this.Width;
            selectedCoursesBottom = this.Height - (lstSelectedCourses.Location.Y + lstSelectedCourses.Height);
            buttonsStart = (double)cmdAddCourse.Location.X / (double)this.Width;
            buttonsEnd = (double)(cmdAddCourse.Location.X + cmdAddCourse.Width) / (double)this.Width;
            availableCoursesStart = (double)lstAvailableCourses.Location.X / (double)this.Width;
            availableCoursesLeft = this.Width - (lstAvailableCourses.Location.X + lstAvailableCourses.Width);
            availableCoursesBottom = this.Height - (lstAvailableCourses.Location.Y + lstAvailableCourses.Height);
        }

        /// <summary>
        ///     Dynamic resize of all elements on the Subjects tab.
        /// </summary>
        private void loadSubjectsTabOffsets()
        {
            Point location;
            int xCoord;

            location = cmdMoveSubjectUp.Location;
            location.Y = this.Height - upDownYFromBottom;
            cmdMoveSubjectUp.Location = location;
            cmdMoveSubjectUp.Width =  Convert.ToInt32(upbuttonEnd * this.Width) - cmdMoveSubjectUp.Location.X;
            
            location = cmdMoveSubjectDown.Location;
            xCoord = Convert.ToInt32(downButtonStart * this.Width);
            location.X = xCoord;
            location.Y = this.Height - upDownYFromBottom;
            cmdMoveSubjectDown.Location = location;
            cmdMoveSubjectDown.Width =  Convert.ToInt32(subjectsEnd * this.Width) - cmdMoveSubjectDown.Location.X;

            lstSubject.Width    = Convert.ToInt32(subjectsEnd * this.Width) - lstSubject.Location.X;
            lstSubject.Height   = this.Height - lstSubject.Location.Y - subjectsBottom;
            lblSubjects.Width = lstSubject.Width;
            
            
            location = cmdCreateSubject.Location;
            location.Y = this.Height - createYFromBottom;
            cmdCreateSubject.Location = location;
            cmdCreateSubject.Width = lstSubject.Width;

            location = cmdEditSubject.Location;
            location.Y = this.Height - editYFromBottom;
            cmdEditSubject.Location = location;
            cmdEditSubject.Width = lstSubject.Width;

            location = cmdRemoveSubject.Location;
            location.Y = this.Height - removeYFromBottom;
            cmdRemoveSubject.Location = location;
            cmdRemoveSubject.Width = lstSubject.Width;
            
            
            location = lstSelectedCourses.Location;
            xCoord = Convert.ToInt32(selectedCoursesStart * this.Width);
            location.X = xCoord;
            lstSelectedCourses.Location = location;
            lstSelectedCourses.Width    = Convert.ToInt32(selectedCoursesEnd * this.Width) - xCoord;
            lstSelectedCourses.Height   = this.Height - lstSelectedCourses.Location.Y - selectedCoursesBottom;

            location = lblSelectedCourses.Location;
            location.X = xCoord;
            lblSelectedCourses.Location = location;
            lblSelectedCourses.Width = lstSelectedCourses.Width;

            location = cmdAddCourse.Location;
            xCoord = Convert.ToInt32(buttonsStart * this.Width);
            location.X = xCoord;
            cmdAddCourse.Location = location;
            cmdAddCourse.Width    = Convert.ToInt32(buttonsEnd * this.Width) - xCoord;
            
            location = cmdRemoveCourse.Location;
            location.X = xCoord;
            cmdRemoveCourse.Location = location;
            cmdRemoveCourse.Width = cmdAddCourse.Width;

            location = lstAvailableCourses.Location;
            xCoord = Convert.ToInt32(availableCoursesStart * this.Width);
            location.X = xCoord;
            lstAvailableCourses.Location = location;
            lstAvailableCourses.Width    = this.Width - xCoord - availableCoursesLeft;
            lstAvailableCourses.Height   = this.Height - lstAvailableCourses.Location.Y - availableCoursesBottom;

            location = lblAvailableCourses.Location;
            location.X = xCoord;
            lblAvailableCourses.Location = location;
            lblAvailableCourses.Width = lstAvailableCourses.Width;
        }

        private void cmdCreateSubject_Click(object sender, EventArgs e)
        {
            String selected = null;
            if(this.lstSubject.Items.Count > 0)
                selected = Convert.ToString(this.lstSubject.SelectedItem);
            new EditSubjectFrame().ShowDialog();
            this.loadSubjects();
            if (selected == null && this.lstSubject.Items.Count > 0)
                this.lstSubject.SelectedIndex = 0;
            else if (selected != null)
                this.lstSubject.SelectedItem = selected;
        }

        private void cmdMoveSubjectUp_Click(object sender, EventArgs e)
        {
            cmdMoveSubjectUp.Enabled = false;
            if (this.lstSubject.SelectedItems.Count > 0)
            {
                String selected = Convert.ToString(this.lstSubject.SelectedItem);
                this.domainController.getSubjects().moveSubjectUpInList(selected);
                this.loadSubjects();
                this.lstSubject.SelectedItem = selected;
            }
            cmdMoveSubjectUp.Enabled = true;
        }

        private void cmdMoveSubjectDown_Click(object sender, EventArgs e)
        {
            cmdMoveSubjectDown.Enabled = false;
            if (this.lstSubject.SelectedItems.Count > 0)
            {
                String selected = Convert.ToString(this.lstSubject.SelectedItem);
                this.domainController.getSubjects().moveSubjectDownInList(selected);
                this.loadSubjects();
                this.lstSubject.SelectedItem = selected;
            }
            cmdMoveSubjectDown.Enabled = true;
        }

        private void cmdRemoveSubject_Click(object sender, EventArgs e)
        {
            if (this.lstSubject.SelectedItems.Count > 0)
            {
                Boolean result = domainController.fireMessageBoxQuestion(this.domainController.getLanguageString("delete_subject_confirmation"), false);
                if (result)
                {
                    Boolean removeSettings = true;
                    // maybe make a new confirmation frame which has a checkbox for the sync settings removal?
                    // domainController.fireMessageBoxQuestion(this.domainController.getLanguageString("delete_subject_sync_confirmation"), false);
                    this.domainController.getSubjects().deleteSubject(Convert.ToString(this.lstSubject.SelectedItem), removeSettings);
                    if (this.domainController.getUserInfo().getSelectedSubject() != null
                        && this.domainController.getUserInfo().getSelectedSubject().getSubjectName().Equals(this.lstSubject.SelectedItem))
                        this.domainController.getUserInfo().clearSelectedItem();
                    this.loadSubjects();
                }
            }
        }

        private void cmdEditSubject_Click(object sender, EventArgs e)
        {
            String subject = Convert.ToString(lstSubject.SelectedItem);
            if (domainController.getUserInfo().getSubjects().subjectExists(subject))
            {
                new EditSubjectFrame(Convert.ToString(lstSubject.SelectedItem)).ShowDialog();
                this.loadSubjects();
                lstSubject.SelectedItem = subject;
            }
        }

        private void cmdOpenSubjectFolder_Click(object sender, EventArgs e)
        {
            if (!txtSubjectFolder.Text.Equals(String.Empty))
                Utility.openFolderInExplorer(txtSubjectFolder.Text);
        }

        private void cmdAddCourse_Click(object sender, EventArgs e)
        {
            Boolean changed=false;
            foreach(int i in this.lstAvailableCourses.SelectedIndices)
            {
                //add course
                changed|=this.domainController.getUserInfo()
                            .addCourseToSubject(Convert.ToString(this.lstAvailableCourses.Items[i]),
                                                Convert.ToString(this.lstSubject.SelectedItem));
            }
            if (changed)
            {
                this.refreshSubjectTabCoursesLists();
            }
        }

        private void cmdRemoveCourse_Click(object sender, EventArgs e)
        {
            Boolean changed=false;
            foreach(int i in this.lstSelectedCourses.SelectedIndices)
            {
                changed|=this.domainController.getUserInfo()
                            .removeCourseFromSubject(Convert.ToString(this.lstSelectedCourses.Items[i]),
                                                     Convert.ToString(this.lstSubject.SelectedItem));
            }
            if (changed)
            {
                this.refreshSubjectTabCoursesLists();
            }
        }

        private void lstSubject_SelectedIndexChanged(object sender, EventArgs e)
        {
            String selected = Convert.ToString(this.lstSubject.SelectedItem);
            if (selected != null && !selected.Equals(String.Empty))
            {
                this.txtSubjectFolder.Text = this.domainController.getUserInfo().getSubjects().getSubject(selected).getSubjectFolder();
            }
            else
            {
                this.txtSubjectFolder.Text = String.Empty;
            }
            this.refreshSubjectTabCoursesLists();
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void lstSubject_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.cmdEditSubject_Click(sender, e);
        }

    }
}
