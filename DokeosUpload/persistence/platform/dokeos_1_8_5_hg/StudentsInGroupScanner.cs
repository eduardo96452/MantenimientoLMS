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
using System.Web;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     As of 1.09
    ///     Last updated on 16/09/2010 by Gianni Van Hoecke
    /// </remarks>
    class StudentsInGroupScanner : XmlScanner
    {
        private List<String> addresses;

        /// <summary>
        ///     Default constructor. Creates an empty list of addresses.
        /// </summary>
        public StudentsInGroupScanner()
        {
            addresses = new List<String>();
        }

        public override void doScan(XmlNodeList nodes)
        {
            addresses = new List<String>();
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
                                    if (childTD.Name.Equals("a") && childTD.Attributes["name"] != null
                                        && (childTD.Attributes["name"].Value.Equals("clickable_email_link")))
                                    {
                                        addresses.Add(HttpUtility.HtmlDecode(childTD.InnerText));                                        
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
        ///     Returns the addresses.
        /// </summary>
        /// <returns>A list of addresses.</returns>
        public List<String> getAddresses()
        {
            return this.addresses;
        }
    }
}
