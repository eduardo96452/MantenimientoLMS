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
using System.Xml;
using lmsda.domain.util.xml;

namespace lmsda.persistence.platform.chamilo_2_0.post
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    class UserScanner : XmlScanner
    {
        public String lastName  { get; set; }
        public String firstName { get; set; }

        /// <summary>
        ///     Default constructor. Creates a new list of courses.
        /// </summary>
        public UserScanner()
        {
            lastName = String.Empty;
            firstName = String.Empty;
        }

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals("input") && node.Attributes["name"]!=null && node.Attributes["name"].Value.Equals("lastname"))
            {
                //Store last name.
                lastName = node.Attributes["value"].Value;
            }
            else if (node.Name.Equals("input") && node.Attributes["name"]!=null && node.Attributes["name"].Value.Equals("firstname"))
            {
                //Store first name.
                firstName = node.Attributes["value"].Value;
            }
        }
    }
}
