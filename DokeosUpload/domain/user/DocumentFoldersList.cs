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
using System.Linq;

namespace lmsda.domain.user
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris.
    ///     This class is used for storing and manipulating document folders read from the platform.
    /// </summary>
    class DocumentFoldersList
    {
        private List<DocumentFolder> documentFolders;

        /// <summary>
        ///     Default constructor. Creates an empty list.
        /// </summary>
        public DocumentFoldersList()
        {
            this.documentFolders = new List<DocumentFolder>();
        }

        /// <summary>
        ///     Constructor. Creates a list based on the parameter.
        /// </summary>
        /// <param name="documentFolders">The document folders.</param>
        public DocumentFoldersList(List<DocumentFolder> documentFolders)
        {
            this.documentFolders = documentFolders;
        }

        /// <summary>
        ///     Adds a document folder to the list.
        /// </summary>
        /// <param name="folderCode">The folder code.</param>
        /// <param name="folderName">The folder name.</param>
        public void addDocumentFolder(Course course, String folderCode, String folderName)
        {
            this.addDocumentFolder(new DocumentFolder(course, folderCode, folderName));
        }

        /// <summary>
        ///     Adds a document folder to the list.
        /// </summary>
        /// <param name="documentFolder">An array with containing the folder code and folder name.</param>
        public void addDocumentFolder(DocumentFolder documentFolder)
        {
            DocumentFolder orig = getDocumentFolderFromFolderName(documentFolder.folderName);
            if (orig==null)
                this.documentFolders.Add(documentFolder);
            else
            {
                // make sure original objects aren't modified: don't merge into the originals
                // that are linked to the single courses
                documentFolders.Remove(orig);
                DocumentFolder temp = new DocumentFolder(new List<DocumentFolderForCourse>(orig.folders), orig.folderName);
                temp.mergeFolders(documentFolder);
                documentFolders.Add(temp);
            }
        }

        /// <summary>
        ///     Returns all document folders.
        /// </summary>
        /// <returns>A list of document folders.</returns>
        public List<DocumentFolder> getDocumentFolders()
        {
            return this.documentFolders;
        }

        /// <summary>
        ///     Returns all document folders as an array. 
        /// </summary>
        /// <returns>An array with all document folders.</returns>
        public String[] getDocumentFoldersForDropDown()
        {
            if (this.documentFolders.Count > 0)
            {
                String[] temp = new String[this.documentFolders.Count];

                for (int i = 0; i < this.documentFolders.Count; i++)
                {
                    temp[i] = this.documentFolders.ElementAt(i).folderName;
                }

                return temp;
            }
            else
                return null;
        }

        /// <summary>
        ///     Returns the document folder for a specified folder name.
        ///     This is treated as case insensitive, because the Windows file system is.
        /// </summary>
        /// <param name="folderName">The string from the dropdown list.</param>
        /// <returns>The DocumentFolder opject.</returns>
        public DocumentFolder getDocumentFolderFromFolderName(String folderName)
        {
            foreach (DocumentFolder df in this.documentFolders)
            {
                if (df.folderName.ToLower().Equals(folderName.ToLower())) 
                    return df;
            }
            return null;
        }

        /// <summary>
        ///     Returns the index of the given folder.
        /// </summary>
        /// <param name="internalFolder">The folder code.</param>
        /// <returns>The index of the folder.</returns>
        public int getDocumentIndexFromFolderName(String folderName)
        {
            for (int i = 0; i < this.documentFolders.Count; i++)
            { 
                if(this.documentFolders[i].folderName.Equals(folderName)) 
                    return i;
            }
            return -1;
        }

        /// <summary>
        ///     Truncates the document folders list.
        /// </summary>
        public void clear()
        {
            this.documentFolders.Clear();
        }

        /// <summary>
        ///     Returns whether the document folders list is empty.
        /// </summary>
        public Boolean isEmpty()
        {
            return this.documentFolders.Count == 0;
        }


    }
}
