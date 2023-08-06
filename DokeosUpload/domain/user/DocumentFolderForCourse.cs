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

namespace lmsda.domain.user
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     This class represents a single folder for one single course on the platform.
    ///     It is used as part of a full DocumentFolder object, which combines same-name folders
    ///     read from different courses on the platform.
    /// </summary>
    class DocumentFolderForCourse
    {
        public Course course { get; set; }
        public String folderCode { get; set; }

        public DocumentFolderForCourse()
        {

        }

        public DocumentFolderForCourse(Course course, String folderCode)
        {
            this.course = course;
            this.folderCode = folderCode;
        }
    }
}
