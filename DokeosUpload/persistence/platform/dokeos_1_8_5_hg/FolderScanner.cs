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
using lmsda.domain.util.xml;
using System.Xml;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    class FolderScanner : XmlScanner
    {
        private List<String> folders;

        /// <summary>
        ///     Default constructor. Creates an empty list of folders.
        /// </summary>
        public FolderScanner()
        { 
            folders = new List<String>();
        }

        public override void doScan(XmlNodeList nodes)
        {
            folders = new List<String>();
            base.doScan(nodes);
        }

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals("select") && node.Attributes["name"] != null
                && (node.Attributes["name"].Value.Equals("curdirpath"))
                )
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.Equals("option") && child.Attributes["value"] != null)
                    { 
                        if(!folders.Contains(child.Attributes["value"].Value))
                            folders.Add(child.Attributes["value"].Value);
                    }
                }
                cancelLoop=true;
            }
        }

        /// <summary>
        ///     Returns the list with folders.
        /// </summary>
        /// <returns>The list with folders.</returns>
        public List<String> getFolders()
        {
            return this.folders;
        }

    }
}
