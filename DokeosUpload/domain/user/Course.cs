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
    ///     This class represents a course on the platform.
    /// </summary>
    class Course
    {
        private String courseName;
        private String courseId;
        private String courseCode;
        private DocumentFoldersList documentFolders;

        public Course(String courseName, String courseCode, String courseId)
        {
            this.courseName = courseName;
            this.courseCode = courseCode;
            this.courseId = courseId;
            documentFolders = new DocumentFoldersList();
        }
        
        public String getCourseName()
        {
            return this.courseName;
        }

        /// <summary>
        ///     Returns the name to show on the UI.
        /// </summary>
        /// <returns>the name to show on the UI</returns>
        public String getDisplayName()
        {
            return this.courseName + " (" + this.courseCode + ")";
        }

        public String getCourseId()
        {
            return this.courseId;
        }

        public String getCourseCode()
        {
            return this.courseCode;
        }

        public DocumentFoldersList getDocumentFolders()
        {
            return this.documentFolders;
        }

        public void setDocumentFolders(DocumentFoldersList documentFolders)
        {
            this.documentFolders = documentFolders;
        }
    }
}
