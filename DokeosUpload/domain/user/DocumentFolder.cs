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
    ///     This class represents a folder for a Subject as it is used by the program.
    ///     Each DocumentFolder is a group of same-name folders read from different
    ///     courses on the platform.
    /// </summary>
    class DocumentFolder
    {
        public String folderName { get; set; }
        public List<DocumentFolderForCourse> folders { get; set; }

        public DocumentFolder()
        {
            folders=new List<DocumentFolderForCourse>();
        }

        public DocumentFolder(Course course, String folderCode, String folderName)
        {
            folders=new List<DocumentFolderForCourse>();
            folders.Add(new DocumentFolderForCourse(course,folderCode));
            this.folderName = folderName;
        }
        
        public DocumentFolder(List<DocumentFolderForCourse> folders, String folderName)
        {
            this.folders = folders;
            this.folderName = folderName;
        }

        /// <summary>
        ///     Merges same-name DocumentFolders from different courses into this one.
        /// </summary>
        /// <param name="docfolders"></param>
        public void mergeFolders(DocumentFolder docfolders)
        {
            if (docfolders.folderName.Equals(this.folderName))
            {
                foreach (DocumentFolderForCourse folder in docfolders.folders)
                {
                    if (!this.foldersContain(folder))
                    {
                        this.folders.Add(folder);
                    }
                }
            }
        }

        private Boolean foldersContain(DocumentFolderForCourse documentFolderForCourse)
        {
            foreach (DocumentFolderForCourse dffc in folders)
            {
                if(dffc.course.Equals(documentFolderForCourse.course))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Gets this folder's folder ID for a specific course
        /// </summary>
        /// <param name="course">the course to get the folder ID for</param>
        /// <returns></returns>
        public String getIdForCourse(Course course)
        {
            foreach (DocumentFolderForCourse dffc in folders)
            {
                if (dffc.course.Equals(course))
                    return dffc.folderCode;
            }
            return null;
        }

    }
}
