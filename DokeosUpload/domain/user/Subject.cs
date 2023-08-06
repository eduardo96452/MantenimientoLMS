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

namespace lmsda.domain.user
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     This class represents a Subject in the local program.
    ///     A Subject is used to group together different courses from the platform.
    ///     A Subject contains a list of SubjectLogin objects, which represent the 
    ///     Courses on the platform for a specific login which are linked to this Subject.
    /// </summary>
    class Subject
    {
        private String subjectName;
        private String subjectFolder;
        private List<SubjectLogin> logins;

        #region Constructor

        public Subject(String subjectName, String subjectFolder)
        {
            this.subjectName = subjectName;
            this.setSubjectFolder(subjectFolder);
            this.logins=new List<SubjectLogin>();
        }

        #endregion

        #region Getters and setters

        public String getSubjectName()
        {
            return this.subjectName;
        }

        public void setSubjectName(String name)
        {
            this.subjectName = name;
        }

        public String getSubjectFolderToSave()
        {
            return this.subjectFolder;
        }

        public String getSubjectFolder()
        {
            if (this.subjectFolder.StartsWith("\\"))
                return ProgramConstants.getProgramPath().TrimEnd('\\') + this.subjectFolder;
            return this.subjectFolder;
        }

        public void setSubjectFolder(String folder)
        {
            if (PortableIdentifier.Instance().isPortable && folder.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                folder = "\\" + folder.Substring(ProgramConstants.getProgramPath().TrimEnd('\\').Length).Trim('\\');
            this.subjectFolder = folder;
        }

        public List<SubjectLogin> getLogins()
        {
            return this.logins;
        }

        public void setLogins(List<SubjectLogin> logins)
        {
            this.logins = logins;
            foreach (SubjectLogin login in this.logins)
            {
                login.setSubjectForLogin(this);
            }
        }

        public void clearLogins()
        {
            foreach (SubjectLogin sjl in this.logins)
                sjl.setSubjectForLogin(null);
            this.logins=new List<SubjectLogin>();
        }

        public void addLogin(SubjectLogin login)
        {
            if (!this.logins.Contains(login))
            {
                login.setSubjectForLogin(this);
                this.logins.Add(login);
            }
        }
        
        public void deleteLogin(SubjectLogin login)
        {
            if (this.logins.Contains(login))
            {
                login.setSubjectForLogin(null);
                this.logins.Remove(login);
            }
        }

        public SubjectLogin getLogin(String username, String platformUrl)
        {
            foreach (SubjectLogin login in logins)
            {
                if (login.getLoginname().Equals(username)
                    && login.getPlatformUrl().Equals(platformUrl))
                {
                    return login;
                }
            }
            return null;
        }

        public List<Course> getCoursesForLogin(Login login)
        {
            if (login==null) return new List<Course>();
            SubjectLogin sjl = this.getLogin(login.getUsername(), login.getPlatformUrl());
            if (sjl==null) return new List<Course>();
            return sjl.getCourses();
        }
        
        public DocumentFoldersList getDocumentFolders(Login login)
        {
            DocumentFoldersList dflFolders = new DocumentFoldersList();

            // make full list of all folders
            foreach (Course course in this.getCoursesForLogin(login))
            {
                foreach (DocumentFolder df in course.getDocumentFolders().getDocumentFolders())
                    dflFolders.addDocumentFolder(df);
            }
            // remove all folders that are missing from some of the courses from the full list
            DocumentFoldersList dflFoldersFinal = new DocumentFoldersList(new List<DocumentFolder>(dflFolders.getDocumentFolders()));
            foreach (Course course in this.getCoursesForLogin(login))
            {
                foreach (DocumentFolder df in dflFolders.getDocumentFolders())
                {
                    if (course.getDocumentFolders().getDocumentFolderFromFolderName(df.folderName) == null)
                    {
                        dflFoldersFinal.getDocumentFolders().Remove(df);
                    }
                }
            }
            return dflFoldersFinal;
        }

        #endregion

    }
}
