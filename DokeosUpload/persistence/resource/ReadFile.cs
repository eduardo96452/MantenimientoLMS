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
using System.Text;
using System.IO;
using lmsda.domain.util;

namespace lmsda.persistence.resource
{
    /// <remarks>
    ///     As of 1.08
    ///     Last updated on 20/08/2010 by Gianni Van Hoecke
    /// </remarks>
    static class ReadFile
    {
        /// <summary>
        ///     Loads the contents of a file.
        /// </summary>
        /// <param name="path">The location of the file.</param>
        /// <param name="deleteFile">If true, the file will be deleted.</param>
        /// <returns>The contents.</returns>
        public static String getContentsOfFile(String path, Boolean deleteFile)
        {
            StreamReader reader = File.OpenText(path);
            String input = null;
            StringBuilder sb = new StringBuilder();

            while ((input = reader.ReadLine()) != null)
            {
                sb.Append(input);
                sb.AppendLine();
            }

            reader.Close();
            input = null;
            reader = null;

            if(deleteFile)
                Utility.tryDeleteFile(path);

            return sb.ToString();
        }

        /// <summary>
        ///     Loads the contents of a file.
        /// </summary>
        /// <param name="path">The location of the file.</param>
        /// <returns>The contents.</returns>
        public static String getContentsOfFile(String path)
        {
            return getContentsOfFile(path, false);
        }
    }
}
