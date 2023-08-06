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
using lmsda.domain.user;
using lmsda.domain.util;

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     As of 1.09
    ///     Last updated on 15-16/09/2010 by Gianni Van Hoecke
    /// </remarks>
    class Groups
    {
        public Course course { get; set; }
        private List<Group> theGroups;

        #region Read-only properties

        public List<Group> groups { get { return this.theGroups; } }
        public int count { get { return this.theGroups.Count; } }

        #endregion

        #region Constructors

        /// <summary>
        ///     Constructor that creates an empty collection of groups with course information.
        /// </summary>
        /// <param name="course"></param>
        public Groups(Course course)
        {
            this.theGroups = new List<Group>();
            this.course = course;
        }

        #endregion

        #region Getters, setters and operations

        /// <summary>
        ///     Creates a new group.
        /// </summary>
        /// <param name="groupID">The ID of the group.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="numberOf">The number of students in the group.</param>
        public void addGroup(String groupID, String groupName, String numberOf)
        {
            this.theGroups.Add(new Group(groupID, groupName, numberOf, this.count));
        }

        /// <summary>
        ///     Removes a group.
        /// </summary>
        /// <param name="gr">The group to remove.</param>
        public void removeGroup(Group gr)
        {
            this.theGroups.Remove(gr);
        }

        /// <summary>
        ///     Removes a group.
        /// </summary>
        /// <param name="index">The index where the group is located.</param>
        public void removeGroup(int index)
        {
            this.theGroups.RemoveAt(index);
        }

        /// <summary>
        ///     Gets the collection of groups where a single student is subscribed to.
        /// </summary>
        /// <param name="student">The student.</param>
        /// <returns>
        ///     A List of groups where the student is  subscribed to. 
        ///     An empty list is returned if the student isn't subscribed to any group.
        /// </returns>
        public List<Group> getGroupsWhereStudentIsSubscribedTo(Student student)
        {
            List<Group> groupOfStudent = new List<Group>();
            
            foreach (Group group in this.theGroups)
            {
                if (group.students.Contains(student))
                    groupOfStudent.Add(group);
            } 

            return groupOfStudent;
        }

        /// <summary>
        ///     Returns the group based on the unique name.
        /// </summary>
        /// <param name="uniqueName">The unique name.</param>
        /// <returns>The group.</returns>
        public Group getGroup(String uniqueName)
        {
            foreach (Group group in this.groups)
            {
                if (group.uniqueName.Equals(uniqueName))
                    return group;
            }

            return null;
        }

        /// <summary>
        ///     Sorts the groups according to the sequence.
        /// </summary>
        public void sortGroups()
        {
            List<Group> tempGroups = new List<Group>();

            while (tempGroups.Count < this.groups.Count)
            {
                foreach (Group group in this.groups)
                {
                    if (group.groupNumber == tempGroups.Count)
                    {
                        //This is the next group
                        tempGroups.Add(group);
                    }
                }
            }

            this.theGroups = tempGroups;
        }

        #endregion
    }
}
