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
using lmsda.domain.user;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    class CourseScanner : XmlScanner
    {
        private List<Course> courses;

        /// <summary>
        ///     Default constructor. Creates a new list of courses.
        /// </summary>
        public CourseScanner()
        {
            courses = new List<Course>();
        }

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals("li") && node.Attributes["class"]!=null && node.Attributes["class"].Value.Equals("courses"))
            {
                try
                {
                    XmlNode imgNode = node.SelectSingleNode("div/img");
                    if (imgNode.Attributes["title"] != null)
                    {
                        if (imgNode.Attributes["title"].Value.Equals("teachers.gif"))
                        {
                            try
                            {
                                XmlNode aNode = node.SelectSingleNode("a");
                                if (aNode.Attributes["href"] != null)
                                {
                                    String rawCourseCode = aNode.Attributes["href"].Value;
                                    String courseName = aNode.InnerText;
                                    rawCourseCode = rawCourseCode.Trim('/');
                                    int index = rawCourseCode.LastIndexOf('/') + 1;
                                    String courseCode = rawCourseCode.Substring(index);
                                    this.courses.Add(new Course(courseName,courseCode,courseCode));
                                }
                            }
                            catch { /* not the img node we need */}
                        }
                    }
                }
                catch { /* not the li node we need */}
            }
        }

        /// <summary>
        ///     returns the list with courses.
        /// </summary>
        /// <returns>The list with courses.</returns>
        public List<Course> getCourses()
        {
            return this.courses;
        }
    }
}
