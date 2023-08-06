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

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     As of 1.09
    ///     Last updated on 15-16/09/2010 by Gianni Van Hoecke
    /// </remarks>
    class Group
    {
        private String groupID;
        private String groupName;
        private int numberOfStudents;
        private List<Student> subscribedStudents;

        #region Properties

        /// <summary>
        ///     If set to false, this group will not be included when exporting statistics.
        /// </summary>
        public Boolean enabled { get; set; }

        /// <summary>
        ///     Sets or gets the sequence of this group.
        /// </summary>
        public int groupNumber { get; set; }

        #endregion

        #region Read-only properties

        public String id { get { return this.groupID; } }
        public String name { get { return this.groupName; } }
        public int studentCount { get { return this.numberOfStudents; } }
        public List<Student> students { get { return this.subscribedStudents; } }
        public String uniqueName { get { return this.groupName + " [id=" + this.groupID + "] (" + this.numberOfStudents + ")"; } }

        #endregion

        #region Constructor

        /// <summary>
        ///     Default constructor. Creates a group.
        /// </summary>
        /// <param name="id">The ID of this group.</param>
        /// <param name="name">The name of this group.</param>
        /// <param name="numberOf">The number of students in this group.</param>
        /// <param name="sequence">The sequence of this group.</param>
        public Group(String id, String name, String numberOf, int sequence)
        {
            this.groupID = id;
            this.groupName = name;
            this.numberOfStudents = Convert.ToInt32(numberOf);
            this.subscribedStudents = new List<Student>();
            this.enabled = true;
            this.groupNumber = sequence;
        }

        #endregion

        #region Getters, setter and operations

        /// <summary>
        ///     Adds a student to the group.
        /// </summary>
        /// <param name="student">The student to add.</param>
        public void addStudent(Student student)
        {
            this.subscribedStudents.Add(student);
        }

        /// <summary>
        ///     Removes a student from the group.
        /// </summary>
        /// <param name="student">The student to remove.</param>
        public void removeStudent(Student student)
        {
            this.subscribedStudents.Remove(student);
        }

        /// <summary>
        ///     Removes a student from the group.
        /// </summary>
        /// <param name="student">The index where the student is located.</param>
        public void removeStudent(int index)
        {
            this.subscribedStudents.RemoveAt(index);
        }

        /// <summary>
        ///     Removes all students from this group.
        /// </summary>
        public void clearStudents()
        {
            this.subscribedStudents = new List<Student>();
        }

        #endregion
    }
}