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
using lmsda.domain.util.xml;
using System.Xml;
using lmsda.domain.util;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    class FileIDScanner : XmlScanner
    {
        private static String ID_STRING1 = "set_invisible";
        private static String ID_STRING2 = "set_visible";
        
        private int fileId = int.MinValue;

        private String filepath;

        /// <summary>
        ///     Instantiates a new object.
        /// </summary>
        /// <param name="filepath">The path to the file.</param>
        public FileIDScanner(String filepath)
        { 
            this.filepath = filepath;
        }

        protected override void examineNode(XmlNode node)
        {
            Boolean isTRNode = node.Name.Equals("tr");
            Boolean hasClass = node.Attributes != null && node.Attributes["class"] != null;
            Boolean isContentRow=false;
            if (hasClass)
                isContentRow=node.Attributes["class"].Value.Equals("row_odd")
                          || node.Attributes["class"].Value.Equals("row_even");

            if (isTRNode && hasClass && isContentRow)
            {
                try
                {
                    XmlNode checkbox = node.SelectSingleNode("td/input");
                    Boolean isCheckbox = checkbox.Attributes["type"] != null && checkbox.Attributes["type"].Value.Equals("checkbox");
                    Boolean hasPath = checkbox.Attributes["name"] != null && checkbox.Attributes["name"].Value.Equals("path[]");
                    Boolean isSearchedFolder = checkbox.Attributes["value"] != null && checkbox.Attributes["value"].Value.Equals(filepath);

                    if (isCheckbox && hasPath && isSearchedFolder)
                        findObjectId(node);
                    if(fileId != int.MinValue) cancelLoop = true;
                }
                catch
                { 
                    // no checkbox found
                }

            }
        }

        /// <summary>
        ///     Finds an object ID.
        /// </summary>
        /// <param name="node">The node.</param>
        private void findObjectId(XmlNode node)
        {
            XmlNodeList children = node.SelectNodes("td/a");
            foreach (XmlNode child in children)
            {
                if (child.Attributes!=null && child.Attributes["href"] != null)
                {
                    if (child.Attributes["href"].Value.Contains(ID_STRING1))
                        this.fileId = Convert.ToInt32(Utility.findValueInURL(child.Attributes["href"].Value, ID_STRING1));
                    else if (child.Attributes["href"].Value.Contains(ID_STRING2))
                        this.fileId = Convert.ToInt32(Utility.findValueInURL(child.Attributes["href"].Value, ID_STRING2));
                }
            }
        }

        /// <summary>
        ///     Returns the file ID.
        /// </summary>
        /// <returns>The file ID.</returns>
        public int getId()
        { 
            if(fileId == int.MinValue) 
                return -1;
            else 
                return fileId;
        }

    }
}
