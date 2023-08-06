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
using lmsda.domain.user;
using System.Xml;
using lmsda.domain.util.xml;
using lmsda.domain.util;

namespace lmsda.persistence.platform.chamilo_2_0.post
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
            this.courses = new List<Course>();
        }

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals("li") && node.Attributes["style"] != null && node.Attributes["style"].Value.Contains("logo/16.png"))
            {
                try
                {
                    XmlNode aNode = node.SelectSingleNode("a");
                    if (aNode.Attributes["href"] != null)
                    {
                        String courseName = aNode.InnerText;
                        String courseId = Utility.findValueInURL(aNode.Attributes["href"].Value, "course");
                        String courseCode = aNode.ParentNode.InnerText;
                        int cutoff = courseCode.IndexOf(courseName) + courseName.Length;
                        courseCode = courseCode.Substring(cutoff, courseCode.IndexOf(" - ") - cutoff);
                        courseName = courseName.Replace('\n', ' ').Trim();
                        courseCode = courseCode.Replace('\n', ' ').Trim();
                        //Store course.
                        if (courseId!=String.Empty)
                            this.courses.Add(new Course(courseName, courseCode, courseId));
                    }
                }
                catch
                { }
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
