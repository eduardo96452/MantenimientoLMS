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
using System.Collections.Generic;
using lmsda.persistence.settings;
using lmsda.domain.score.data;

namespace lmsda.domain.user
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     
    ///     This class stores all settings related to the user login, subjects and courses.
    ///     The "SelectedItem" represents a course or subject selected on the UI.
    /// </summary>
    /// <remarks>
    ///     Last updated on 15/09/2010 by Gianni Van Hoecke
    ///      -> Added groups.
    /// </remarks>
    class UserInfo
    {
        private SubjectSettings subjects;
        private Login login;
        private Subject selectedSubject;
        private Course selectedCourse;
        private List<Groups> groups;

        #region Constructor

        public UserInfo(String selectedSubject)
        {
            this.subjects = new SubjectSettings();
            this.login = null;
            this.setSelectedItemFromSavedString(selectedSubject);           
        }

        #endregion

        #region Getters and setters

        public void setGroups(List<Groups> groups)
        {
            this.groups = groups;
        }

        public List<Groups> getGroups()
        {
            return this.groups;
        }

        public void setLogin(Login login)
        {
            if (this.subjects == null)
                this.subjects = new SubjectSettings();
            this.login = login;
        }

        public Login getLogin()
        {
            return this.login;
        }

        public void logOut()
        {
            if (this.login == null)
                return;
            this.subjects.clearSubjects();
            this.selectedSubject = null;
            this.subjects = null;
            this.login.clearCourses();
            this.selectedCourse = null;
            this.login = null;
        }

        public SubjectSettings getSubjects()
        {
            return this.subjects;
        }

        public void setSelectedItemFromSavedString(String savedString)
        {
            Boolean set=false;
            if (savedString != null)
            {
                foreach (Subject subject in subjects.getSubjects())
                {
                    if (savedString.Equals(subject.getSubjectName()))
                    {
                        setSelectedSubject(subject);
                        set = true;
                        break;
                    }
                }
                if (set == false && login != null)
                {
                    foreach (Course course in login.getCourses())
                    {
                        if (savedString.Equals(course.getCourseId()))
                        {
                            setSelectedCourse(course);
                            set = true;
                            break;
                        }
                    }
                }
            }
            if (set == false)
                clearSelectedItem();
        }

        public void setSelectedItemFromDropdownString(String dropdownString)
        {
            if (dropdownString == null)
                clearSelectedItem();
            if (dropdownString.Equals(ProgramConstants.LISTSEPARATOR)) return;
            Boolean set=false;
            foreach (Subject subject in subjects.getSubjects())
            {
                if (dropdownString.Equals(subject.getSubjectName()))
                {
                    setSelectedSubject(subject);
                    set=true;
                }
            }
            if (set == false)
            {
                foreach (Course course in login.getCourses())
                {
                    if (dropdownString.Equals(course.getDisplayName()))
                    {
                        setSelectedCourse(course);
                        set = true;
                    }
                }
            }
            if (set == false)
                clearSelectedItem();
        }

        public void setSelectedItemFromDropdownIndex(int dropdownIndex)
        {
            String[] dropdown = getCoursesDropDownList();
            if (dropdownIndex >=0 && dropdownIndex < (dropdown.Length))
                setSelectedItemFromDropdownString(dropdown[dropdownIndex]);
            else
                clearSelectedItem();
        }

        private void setSelectedSubject(Subject subject)
        {
            this.selectedCourse = null;
            this.selectedSubject = subject;
        }

        private void setSelectedCourse(Course course)
        {
            this.selectedCourse = course;
            this.selectedSubject = null;
        }

        /// <summary>
        ///     Sets the selected course to the last selected course saved in the settings file.
        /// </summary>
        /// <returns>The list index of the course.</returns>
        public int reselectSavedCourse()
        {
            List<String> list = new List<String>(this.getCoursesDropDownList());

            if (this.isLoggedIn())
            {
                if (this.selectedSubject == null && this.selectedCourse == null)
                {
                    String savedcode = DomainController.Instance().getSettings().getCoursecode();
                    Course savedCourse = this.login.getCourseFromCode(savedcode);
                    if (savedCourse != null && list.Contains(savedCourse.getDisplayName()))
                    {
                        setSelectedCourse(savedCourse);
                        return list.IndexOf(selectedCourse.getDisplayName());
                    }
                    else
                    {
                        Subject savedSubject = DomainController.Instance().getSubjects().getSubject(savedcode);
                        if (savedSubject != null && list.Contains(savedSubject.getSubjectName()))
                        {
                            setSelectedSubject(savedSubject);
                            return list.IndexOf(selectedSubject.getSubjectName());
                        }
                    }
                }
                else return getSelectedItemIndex();
            }
            return -1;
        }

        public void clearSelectedItem()
        {
            this.selectedCourse = null;
            this.selectedSubject = null;
        }

        public String getSelectedItemName()
        {
            if (this.selectedCourse != null)
                return this.selectedCourse.getDisplayName();
            else if (this.selectedSubject !=null)
                return this.selectedSubject.getSubjectName();
            else return String.Empty;
        }

        public String getSelectedItemSubjectFolderForSubject()
        {
            if (this.selectedCourse != null)
                return null;
            else if (this.selectedSubject !=null)
                return this.selectedSubject.getSubjectFolder();
            else return null;
        }

        public String getSelectedItemSubjectFolder()
        {
            if (this.selectedSubject !=null)
                return this.selectedSubject.getSubjectFolder();
            else
                return DomainController.Instance().getSettings().getSubjectsFolder();
        }

        public String getSelectedItemSaveName()
        {
            if (this.selectedCourse != null)
                return this.selectedCourse.getCourseId();
            else if (this.selectedSubject !=null)
                return this.selectedSubject.getSubjectName();
            else return String.Empty;
        }
        
        public int getSelectedItemIndex()
        {
            String item = getSelectedItemName();
            if (item == null) return -1;
            String[] dropdown = getCoursesDropDownList();
            for (int index = 0; index < dropdown.Length; index++)
            {
                if (dropdown[index].Equals(item))
                    return index;
            }
            return -1;
        }

        public Course getSelectedCourse()
        {
            return this.selectedCourse;
        }

        public Subject getSelectedSubject()
        {
            return this.selectedSubject;
        }
        
        public Boolean selectedItemIsCourse()
        {
            return (this.selectedCourse!=null);
        }

        public Boolean selectedItemIsSubject()
        {
            return (this.selectedSubject!=null);
        }

        public DocumentFoldersList getSelectedItemDocumentFolders()
        {
            if (this.selectedCourse != null)
                return this.selectedCourse.getDocumentFolders();
            else if (this.selectedSubject !=null)
                return this.selectedSubject.getDocumentFolders(login);
            else return new DocumentFoldersList();
        }

        public List<Course> getSelectedItemCourses()
        {
            if (this.selectedCourse != null)
                return new List<Course>(new Course[]{this.selectedCourse});
            else if (this.selectedSubject !=null)
                return this.selectedSubject.getCoursesForLogin(this.login);
            return new List<Course>();
        }

        [Obsolete("This function is no longer used, since the restriction to only link unlinked courses to a subject was removed.")]
        /// <summary>
        ///     Returns a list of all courses that are currently not inside a Subject.
        /// </summary>
        /// <remarks>
        ///     Deprecated since 1.08
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        /// <returns>the list of all courses that are not inside a Subject.</returns>
        public String[] getCoursesNotInSubjects()
        {
            if (!isLoggedIn()) return new String[0];
            List<String> retn = new List<String>();
            List<Course> returncourses = new List<Course>(this.login.getCourses());
            foreach (Subject subject in this.subjects.getSubjects())
                foreach (Course c in subject.getCoursesForLogin(this.login))
                    if (returncourses.Contains(c))
                        returncourses.Remove(c);
            foreach (Course c in returncourses)
                retn.Add(c.getDisplayName());
            return retn.ToArray();
        }

        /// <summary>
        ///     Returns a list of all courses that are currently not inside the specified Subject
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        /// </remarks>
        /// <returns>the list of all courses that are not inside a Subject.</returns>
        public String[] getCoursesNotInSubject(String subjectname)
        {
            List<String> retn = new List<String>();
            if (this.isLoggedIn())
            {
                Subject sj = this.subjects.getSubject(subjectname);
                if (sj != null)
                {
                    List<Course> allcourses = this.login.getCourses();
                    List<Course> linkedCourses = sj.getCoursesForLogin(this.login);
                    foreach (Course c in allcourses)
                        if (!linkedCourses.Contains(c))
                            retn.Add(c.getDisplayName());
                }
            }
            return retn.ToArray();
        }

        /// <summary>
        ///     Returns all courses and categories as list for the dropdown menu.
        /// </summary>
        /// <returns>A String array of all categories and courses, with a separator in between them.</returns>
        public String[] getCoursesDropDownList()
        {
            List<String> retn = new List<String>();
            if (!this.isLoggedIn()) return retn.ToArray();
            foreach (Subject subject in this.subjects.getSubjects())
            {
                retn.Add(subject.getSubjectName());
            }
            List<Course> courses = login.getCourses();
            if (retn.Count > 0 && courses.Count > 0) retn.Add(ProgramConstants.LISTSEPARATOR);
            foreach (Course cr in courses)
            {
                retn.Add(cr.getDisplayName());
            }
            return retn.ToArray();
        }

        public String[] getCoursesForSubject(String subjectname)
        {
            List<String> retn = new List<String>();
            if (this.isLoggedIn())
            {
                Subject sj = this.subjects.getSubject(subjectname);
                if (sj != null)
                {
                    SubjectLogin sjl = sj.getLogin(this.login.getUsername(), this.login.getPlatformUrl());
                    if (sjl != null)
                    {
                        foreach (Course course in sjl.getCourses())
                        {
                            retn.Add(course.getDisplayName());
                        }
                    }
                }
            }
            return retn.ToArray();
        }

        #endregion

        #region User actions

        public void clearSubjects()
        {
            this.subjects.clearSubjects();
        }

        public Boolean hasSelectedItem()
        {
            return (this.selectedSubject!=null || this.selectedCourse!=null);
        }

        /// <summary>
        ///     Checks if the user is logged in.
        /// </summary>
        /// <returns>True if the user is logged in.</returns>
        public Boolean isLoggedIn()
        {
            return this.login != null;
        }

        public Boolean addCourseToSubject(String courseDropdownName, String subjectname)
        {
            //add course
            if (!this.isLoggedIn()) return false;
            Course cr = this.login.getCourseFromDropDownString(courseDropdownName);
            Subject sj = this.subjects.getSubject(subjectname);
            String username=this.login.getUsername();
            String platform=this.login.getPlatformUrl();
            if (sj!=null)
            {
                SubjectLogin sjl = sj.getLogin(username, platform);
                if(sjl==null)
                {
                    sjl = new SubjectLogin(username, platform);
                    sj.addLogin(sjl);
                }
                sjl.addCourse(cr);
                subjects.saveSubjects();
                return true;
            }
            return false;
        }

        public Boolean removeCourseFromSubject(String courseDropdownName, String subjectname)
        {
            //add course
            if (!this.isLoggedIn()) return false;
            Course cr = this.login.getCourseFromDropDownString(courseDropdownName);
            Subject sj = this.subjects.getSubject(subjectname);
            if (sj==null) return false;
            SubjectLogin sjl = sj.getLogin(this.login.getUsername(), this.login.getPlatformUrl());
            if(sjl==null) return false;
            sjl.deleteCourse(cr);
            if (sjl.getCourses().Count == 0)
                sj.deleteLogin(sjl);
            subjects.saveSubjects();
            return true;
        }

        #endregion
    }
}
