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
    ///     Author: Gianni Van Hoecke.
    ///     This class is used for storing the login information of the user.
    /// </summary>
    class Login
    {
        private String username;
        private String password;
        private String platformUrl;
        private List<Course> courses;

        #region Constructor

        /// <summary>
        ///     Default constructor. Creates an empty object, based on platform.
        /// </summary>
        /// <param name="platformUrl">The platform URL.</param>
        public Login(String platformUrl)
            : this(platformUrl, "", "")
        { }

        /// <summary>
        ///     Constructor with parameters.
        /// </summary>
        /// <param name="platformUrl">The platform URL.</param>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        public Login(String platformUrl, String username, String password)
        {
            this.username = username;
            this.password = password;
            this.platformUrl = platformUrl;
            this.courses = new List<Course>();
        }

        #endregion

        #region Getters and setters

        public void setUsername(String username)
        {
            this.username = username;
        }

        public String getUsername()
        {
            return this.username;
        }

        public void setPassword(String password)
        {
            this.password = password;
        }

        public String getPassword()
        {
            return this.password;
        }

        public void setPlatformUrl(String platformUrl)
        {
            this.platformUrl = platformUrl;
        }

        public String getPlatformUrl()
        {
            return this.platformUrl;
        }

        public void setCourses(List<Course> coursesList)
        {
            this.courses = coursesList;
        }

        public List<Course> getCourses()
        {
            return this.courses;
        }

        #endregion

        #region User actions based on login

        /// <summary>
        ///     Returns the course based on a String from the drop down list. (Display name)
        /// </summary>
        /// <param name="dropdownString">The string that defines a course.</param>
        /// <returns>The course that represents the string; null if not found.</returns>
        public Course getCourseFromDropDownString(String dropdownString)
        {
            foreach (Course cr in courses)
                if(cr.getDisplayName().Equals(dropdownString)) return cr;
            return null;
        }

        /// <summary>
        ///     Returns the course based on a String. (Course name)
        /// </summary>
        /// <param name="courseName">The course name as string.</param>
        /// <returns>The course that represents the string; null if not found.</returns>
        public Course getCourseFromName(String courseName)
        {
            foreach (Course cr in courses)
                if(cr.getCourseName().Equals(courseName)) return cr;
            return null;
        }

        /// <summary>
        ///     Return the course based on the course code.
        /// </summary>
        /// <param name="courseCode">The course code.</param>
        /// <returns>The course that represents the course code.</returns>
        public Course getCourseFromCode(String courseCode)
        {
            foreach (Course cr in courses)
                if(cr.getCourseId().Equals(courseCode)) return cr;
            return null;
        }

        /// <summary>
        ///     Clears all courses.
        /// </summary>
        public void clearCourses()
        {
            this.courses = new List<Course>();
        }

        #endregion

    }
}
