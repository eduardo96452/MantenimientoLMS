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
using System.IO;

namespace lmsda.domain.user
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used to store information for a file upload.
    /// </summary>
    class FileForUpload
    {
        private FileInfo selectedFileForUpload;
        private long bytesProcessed;

        #region Constructor

        /// <summary>
        ///     Constructor based on a string.
        /// </summary>
        /// <param name="fileName">Full file name as string.</param>
        public FileForUpload(String fileName)
            : this(new FileInfo(fileName))
        {
 
        }

        /// <summary>
        ///     Constructor based on a FileInfo.
        /// </summary>
        /// <param name="fileName">The file.</param>
        public FileForUpload(FileInfo fileName)
        {
            this.setSelectedFileForUpload(fileName);
        }

        #endregion

        #region Getters and setters

        public void setSelectedFileForUpload(FileInfo fileName)
        {
            this.selectedFileForUpload = fileName;
        }

        public FileInfo getSelectedFileForUpload()
        {
            return this.selectedFileForUpload;
        }

        public void setBytesProcessed(long bytes)
        {
            this.bytesProcessed = bytes;
        }

        public long getBytesProcessed()
        {
            return this.bytesProcessed;
        }

        #endregion

    }
}
