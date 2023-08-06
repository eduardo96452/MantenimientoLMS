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
using lmsda.domain.util.xml;
using System.Xml;
using lmsda.domain.util;
using lmsda.domain.score.data;
using lmsda.domain.user;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     As of 1.09
    ///     Last updated on 15/09/2010 by Gianni Van Hoecke
    /// </remarks>
    class GroupsScanner : XmlScanner
    {
        private Groups groups;
        private Course course;

        /// <summary>
        ///     Default constructor. Creates an empty list of groups.
        /// </summary>
        public GroupsScanner(Course course)
        {
            this.course = course;
            groups = new Groups(this.course);
        }

        public override void doScan(XmlNodeList nodes)
        {
            groups = new Groups(this.course);
            base.doScan(nodes);
        }

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals("table") && node.Attributes["class"] != null
                && (node.Attributes["class"].Value.Equals("data_table"))
                )
            {
                foreach (XmlNode child in node.ChildNodes)
                {
                    if (child.Name.Equals("tr"))
                    {
                        int i = 0;
                        foreach (XmlNode childTR in child.ChildNodes)
                        {                            
                            if (childTR.Name.Equals("td"))
                            {                               
                                foreach (XmlNode childTD in childTR.ChildNodes)
                                {
                                    if (childTD.Name.Equals("a") && childTD.Attributes["href"] != null
                                        && (childTD.Attributes["href"].Value.Contains("group_space.php")
                                        && childTD.Attributes["href"].Value.Contains("gidReq")))
                                    {
                                        groups.addGroup(Utility.findValueInURL(childTD.Attributes["href"].Value, "gidReq"), childTD.InnerText, child.ChildNodes[i + 1].InnerText);
                                    }                                    
                                }
                                i++;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the groups.
        /// </summary>
        /// <returns>The groups object.</returns>
        public Groups getGroups()
        {
            return this.groups;
        }
    }
}
