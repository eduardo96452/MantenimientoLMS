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
using lmsda.domain.util;

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class represents a student and his/her results.
    /// </summary>
    /// <remarks>
    ///     Last updated on 15/09/2010 by Gianni Van Hoecke
    ///      -> Renamed "StudentResults" to "Student".
    /// </remarks>
    class Student
    {
        public String   username        { get; set; }
        public String   userFirstName   { get; set; }
        public String   userFamilyName  { get; set; }
        public String   email           { get; set; }
        public String   userSortName    { get; set; }
        
        public List<ExerciseResult> exerciseResults { get; set; }

        /// <summary>
        ///     Adds a student.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="userFirstName">The first name.</param>
        /// <param name="userFamilyName">The family name.</param>
        /// <param name="email">The e-mail address.</param>
        public Student(String username, String userFirstName, String userFamilyName, String email)
        {
            this.username = username;
            this.userFirstName = userFirstName;
            this.userFamilyName = userFamilyName;
            this.email = email;
            this.userSortName = Utility.removeSpecialCharacters(this.userFamilyName) + " " + Utility.removeSpecialCharacters(this.userFirstName);
            this.exerciseResults = new List<ExerciseResult>();
        }

        /// <summary>
        ///     Adds an exercise.
        /// </summary>
        /// <param name="exerciseResult">The exercise.</param>
        public void addExerciseResult(ExerciseResult exerciseResult)
        {
            this.exerciseResults.Add(exerciseResult);
        }

        /// <summary>
        ///     Returns an exercise.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The exercise.</returns>
        public ExerciseResult getExerciseResultAt(int index)
        {
            return this.exerciseResults.ElementAt(index);
        }

        /// <summary>
        ///     Read-only. Returns the number of exercises of this student.
        /// </summary>
        public int exerciseCount
        {
            get 
            {
                return this.exerciseResults.Count;
            }
        }

        /// <summary>
        ///     Returns an exercise, based on an exercise ID.
        /// </summary>
        /// <param name="id">The exercise ID.</param>
        /// <returns>The exercise.</returns>
        public ExerciseResult getExercise(int id)
        {
            foreach(ExerciseResult exercise in this.exerciseResults)
            {
                if(exercise.exerciseID == id)
                    return exercise;
            }

            return null;
        }

        /// <summary>
        ///     Sorts the exercises for this student.
        /// </summary>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 15/09/2010 by Gianni Van Hoecke
        /// </remarks>
        public void sortExercises()
        {
            this.exerciseResults.Sort(delegate(ExerciseResult ex1, ExerciseResult ex2) { return ex1.exerciseTitle.CompareTo(ex2.exerciseTitle); });
        }

        /// <summary>
        ///     Returns the readable representation of the internal objects.
        /// </summary>
        /// <returns>The readable string.</returns>
        public String toString()
        {
            String value = Environment.NewLine +                 
                "User name   = " + this.username + Environment.NewLine + 
                "First name  = " + this.userFirstName + Environment.NewLine + 
                "Family name = " + this.userFamilyName + Environment.NewLine + 
                "E-mail      = " + this.email + Environment.NewLine +
                Environment.NewLine +
                "# Exercises = " + exerciseCount;

            foreach(ExerciseResult exercise in this.exerciseResults)
            {
                value += exercise.toString();
            }

            return value;
        }
    }
}
