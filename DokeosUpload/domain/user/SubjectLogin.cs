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
    ///     This class represents the Courses on a platform for a specific login which are linked to one Subject.
    /// </summary>
    class SubjectLogin
    {
        private String loginName;
        private String platformUrl;
        private List<Course> courses;
        private Subject subjectForLogin;

        #region Constructor

        public SubjectLogin(String loginname, String platformUrl)
        {
            this.loginName=loginname;
            this.platformUrl=platformUrl;
            this.courses = new List<Course>();
        }

        #endregion

        #region Getters and setters

        public Subject getSubjectForLogin()
        {
            return this.subjectForLogin;
        }

        /// <summary>
        ///     This function allows storage of the containing element in this object and its contained courses.
        /// </summary>
        /// <param name="subjectForLogin"></param>
        public void setSubjectForLogin(Subject subjectForLogin)
        {
            this.subjectForLogin = subjectForLogin;
        }

        public String getLoginname()
        {
            return this.loginName;
        }

        public void setLoginname(String loginname)
        {
            this.loginName = loginname;
        }

        public String getPlatformUrl()
        {
            return this.platformUrl;
        }

        public void setPlatformUrl(String platformUrl)
        {
            this.platformUrl = platformUrl;
        }

        public List<Course> getCourses()
        {
            return this.courses;
        }

        public void setCourses(List<Course> courses)
        {
            this.courses = courses;
        }

        public void addCourse(Course course)
        {
            if (!this.courses.Contains(course))
            {
                this.courses.Add(course);
            }
        }

        public void deleteCourse(Course course)
        {
            if (this.courses.Contains(course))
            {
                this.courses.Remove(course);
            }
        }

        public void clearCourses()
        {
            this.courses = new List<Course>();
        }

        #endregion
    }
}
