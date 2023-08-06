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
using lmsda.domain.score.data;
using lmsda.domain.user;
using lmsda.domain.util;

namespace lmsda.domain.score
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class stores a collection of students and their results.
    /// </summary>
    /// <remarks>
    ///	    As of 1.09
    ///     Last updated on 15/09/2010 by Gianni Van Hoecke
    /// </remarks>
    class Scores
    {
        public List<Student> students { get; set; }
        public Course course { get; set; }
        public Groups groups { get; set; }

        /// <summary>
        ///     Default constructor. Creates an empty students list.
        /// </summary>
        public Scores(Course course)
        {
            this.students = new List<Student>();
            this.course = course;
            this.groups = new Groups(this.course);
            //Get the groups associated with the course.
            foreach (Groups theGroups in DomainController.Instance().getUserInfo().getGroups())
            {
                if (theGroups.course.Equals(this.course))
                {
                    this.groups = theGroups;
                    break; //we have the group we wanted, so break the operation.
                }
            }
        }

        /// <summary>
        ///     Adds a student to the list.
        /// </summary>
        /// <param name="exerciseResult">The student.</param>
        public void addStudent(Student studentResult)
        {
            this.students.Add(studentResult);
        }

        /// <summary>
        ///     Returns a student.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The student.</returns>
        public Student getStudentAt(int index)
        {
            return this.students.ElementAt(index);
        }

        /// <summary>
        ///     Read-only. Gets the number of students in the list.
        /// </summary>
        public int studentsCount
        {
            get 
            {
                return this.students.Count;
            }
        }

        /// <summary>
        ///     Returns a student, based on the user name.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <returns>The student object.</returns>
        public Student getStudent(String username)
        {
            foreach(Student student in this.students)
            {
                if(student.username.Equals(username))
                    return student;
            }

            return null;
        }

        /// <summary>
        ///     Returns a student, based on the full name.
        /// </summary>
        /// <param name="name">The full name.</param>
        /// <returns>The student object.</returns>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 15/09/2010 by Gianni Van Hoecke
        /// </remarks>
        public Student getStudentByName(String name)
        {
            foreach (Student student in this.students)
            {
                String fullname = student.userFamilyName + " " + student.userFirstName;
                if (fullname.Equals(name))
                    return student;
            }

            return null;
        }

        /// <summary>
        ///     Returns a student, based on the e-mail address.
        /// </summary>
        /// <param name="address">The e-mail address.</param>
        /// <returns>The student object.</returns>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        /// </remarks>
        public Student getStudentByEmailAddress(String address)
        {
            foreach (Student student in this.students)
            {
                if (student.email.Equals(address))
                    return student;
            }

            return null;
        }

        /// <summary>
        ///     Sorts the studentslist.
        /// </summary>
        public void sortStudents()
        {
            //Create sort array
            String[] sortArray = new String[this.students.Count];
            int[] sortArrayIndex = new int[this.students.Count];

            //Copy data to sort array
            for (int i = 0; i < this.students.Count; i++)
            {
                sortArrayIndex[i] = i;
                sortArray[i] = this.students.ElementAt(i).userSortName;
                this.students.ElementAt(i).userSortName = i.ToString() + "|" + this.students.ElementAt(i).userSortName;
            }

            //Sort the array
            Array.Sort(sortArray, sortArrayIndex);

            //Sort the studentslist
            List<Student> studentsSorted = new List<Student>();
            for (int i = 0; i < sortArrayIndex.Length; i++)
            {
                foreach (Student sr in this.students)
                {
                    if (sr.userSortName.StartsWith(sortArrayIndex[i].ToString() + "|"))
                    {
                        sr.userSortName = sr.userSortName.Split('|')[1];
                        studentsSorted.Add(sr);
                        break;
                    }
                }
            }

            //Save the sorted students list
            this.students = studentsSorted;
        }

        /// <summary>
        ///     Sorts the exercises of all students.
        /// </summary>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 15/09/2010 by Gianni Van Hoecke
        /// </remarks>
        public void sortExercises()
        {
            foreach (Student student in this.students)
            {
                student.sortExercises();
            }
        }

        /// <summary>
        ///     Sorts the groups.
        /// </summary>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        /// </remarks>
        public void sortGroups()
        {
            this.groups.sortGroups();
        }

        /// <summary>
        ///     Returns in a string all groups where the student has subscribed to.
        /// </summary>
        /// <param name="stdnt">The student object.</param>
        /// <returns>A string of groups.</returns>
        public String getGroupsWhereStudentIsSubscribedTo(Student stdnt)
        {
            String temp = "";

            foreach (Group group in this.groups.getGroupsWhereStudentIsSubscribedTo(stdnt))
                temp += group.name + ", ";

            return (!temp.Equals(String.Empty)) ? temp.Substring(0, temp.Length - 2) : temp; //remove last ", "
        }

        /// <summary>
        ///     Links all students to their groups. This function uses requests to the platform and
        ///     can therefore take a while to complete.
        /// </summary>
        public void linkStudentsToGroups()
        {
            List<String> addresses;
            foreach(Group theGroup in this.groups.groups)
            {
                theGroup.clearStudents();
                addresses = DomainController.Instance().getPlatform().getUsersFromGroup(this.course, theGroup);
                foreach (String address in addresses)
                {
                    Student stdnt = this.getStudentByEmailAddress(address);
                    if (stdnt != null)
                    {
                        //We have a match!
                        theGroup.addStudent(stdnt);
                    }
                }
            }            
        }

        /// <summary>
        ///     Returns the readable representation of the internal objects.
        /// </summary>
        /// <returns>The readable string.</returns>
        public String toString()
        {
            String value = "# Students = " + this.studentsCount 
                            + Environment.NewLine;

            foreach (Student student in this.students)
            {
                value += student.toString();
            }

            return value;
        }
    }
}
