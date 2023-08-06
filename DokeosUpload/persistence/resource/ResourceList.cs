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
using System.Text.RegularExpressions;

namespace lmsda.persistence.resource
{
    /// <summary>
    ///     Author: Gianni Van Hoecke.
    ///     This class is used to load all available language files.
    /// </summary>
    class ResourceList
    {
        /// <summary>
        ///     Returns a list with all available languages.
        /// </summary>
        /// <param name="basePath">The path to folder that contains the language files.</param>
        /// <param name="extension">The extension of the language files.</param>
        /// <returns>An array of all available language files.</returns>
        public static String[] getResources(String basePath, String extension)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(basePath);

                FileInfo[] fileInfo = directoryInfo.GetFiles("*" + extension);

                String[] resources = new String[fileInfo.Length];

                for (int i = 0; i < fileInfo.Length; i++)
                {
                    resources[i] = Regex.Split(fileInfo[i].Name, extension)[0];
                }

                return resources;
            }
            catch (Exception)
            {
                return new String[]{};
            }
        }
    }
}
