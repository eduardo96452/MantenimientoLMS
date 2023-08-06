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
using lmsda.domain.exercise;
using lmsda.domain.score;
using lmsda.domain.score.data;

namespace lmsda.persistence.platform
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     The abstract superclass of platforms.
    /// </summary>
    abstract class TargetPlatform
    {

        #region Abstract methods

        /// <summary>
        ///     Tries to login.
        /// </summary>
        /// <returns>True if the login succeeds.</returns>
        abstract public Boolean tryLogin();

        /// <summary>
        ///     Returns all courses of which the user is administrator.
        /// </summary>
        /// <returns>A new object of CourseList with all the available courses.</returns>
        abstract public List<Course> readCourses();

        /// <summary>
        ///     Returns the document folders of the given course.
        /// </summary>
        /// <param name="courseCode">The course code.</param>
        /// <returns>A new object of DocumentFoldersList with all available document folders.</returns>
        abstract public Boolean readDocumentFolders(ref List<Course> courses, Boolean useLazyLoading);

        /// <summary>
        ///     Uploads the exercises to the selected platform.
        /// </summary>
        /// <param name="exercises">The list with exercises and their questions and answers.</param>
        /// <param name="publishDestinations">The courses in which the exercises should be published.</param>
        /// <param name="Subject">The Subject the exercises are uploaded for</param>
        /// <param name="setExerciseInvisible">Indicates if the exercise has to be invisible.</param>
        /// <param name="randomQuestions">Number of random questions to set (0 for none)</param>
        /// <param name="onlyIfDoesntExist">Abort if an exercise with the same name already exists on the platform</param>
        /// <returns>True if the upload of the exercises succeeded.</returns>
        abstract public Boolean doUploadExercises(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions, Boolean onlyIfDoesntExist);
        
        /// <summary>
        ///     Updates the information of an existing exercise on the selected platform.
        /// </summary>
        /// <param name="exercises">The list with exercises and their questions and answers.</param>
        /// <param name="publishDestination">The courses in which the exercises should be published.</param>
        /// <param name="setExerciseInvisible">Indicates if the exercise has to be invisible.</param>
        /// <returns>True if the upload of the exercises succeeded.</returns>
        abstract public Boolean doUpdateExercisesInfo(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions);

        /// <summary>
        ///     Uploads a file to the selected platform.
        /// </summary>
        /// <param name="uploadFilePath">The full file name of the file that has to be uploaded.</param>
        /// <param name="uploadedName">Name to give to the file item on the platform. Null uses the filename</param>
        /// <param name="description">The description of the file.</param>
        /// <param name="publishDestination">The courses folder in which the file should be published.</param>
        /// <returns>The paths of the uploaded files for each course.</returns>
        abstract public String[] doUploadFile(String uploadFilePath, String uploadedName, String description, DocumentFolder publishDestination, Subject subject, Boolean setInvisible);


        /// <summary>
        ///     Updates the information of a file on the platform.
        /// </summary>
        /// <param name="filename">The name of the file on the platform.</param>
        /// <param name="path">Path of the file on the platform.</param>
        /// <param name="description">New description for the file on the platform.</param>
        /// <param name="visible">The new visibility status of the file on the platform.</param> 
        /// <returns>True if the update succeeded for all courses.</returns>
        abstract public Boolean doUpdateFile(String filename, DocumentFolder path, Subject subject, String description, Boolean visible);

        /// <summary>
        ///     Returns the name of the working platform.
        /// </summary>
        /// <returns>The name of the working platform.</returns>
        abstract public String getPlatformName();

        /// <summary>
        ///     Returns the folder path as it would be returned as folder string from the platform.
        /// </summary>
        /// <param name="filenameToFormat">The folder string to format</param>
        /// <returns>The correctly formatted folder string.</returns>
        abstract public String getFormattedFilenameString(String filenameToFormat);

        /// <summary>
        ///     Creates a new folder in the given document folder of the platform.
        /// </summary>
        /// <param name="courseCode">The course or courses category.</param>
        /// <param name="foldername">The new folder name.</param>
        /// <param name="destination">The destination path on the platform.</param>
        abstract public DocumentFolder doCreateFolderInDocuments(DocumentFolder publishDestination, String foldername, Boolean setInvisible);

        /// <summary>
        ///     Downloads the statistics and converts them into inernal objects.
        /// </summary>
        /// <param name="courses">The course(s) to download exercise statistics for</param>
        /// <param name="saveTo">The output folder to save any downloaded raw file into</param>
        /// <param name="createSubFolder">Create subfolder for course</param>
        /// <param name="subjectFolder">Subject folder to put in between</param>
        /// <param name="withoutMissingData">Download extra missing data</param>
        /// <returns></returns>
        abstract public List<Scores> downloadStatistics(List<Course> courses, String saveTo, Boolean createSubFolder, Boolean withoutMissingData);

        /// <summary>
        ///     Downloads the groups of <b>a single course</b>.
        /// </summary>
        /// <param name="course">The course (code).</param>
        /// <returns>A new object of Groups.</returns>
        abstract public Groups downloadGroups(Course course);

        /// <summary>
        ///     Downloads the groups of the given courses.
        /// </summary>
        /// <param name="courses">The courses. (codes)</param>
        /// <returns>A list of Groups objects. Each item in this list is a collection of groups of a single course.</returns>
        abstract public List<Groups> downloadGroups(List<Course> courses);

        /// <summary>
        ///     Gets all users from the given group.
        /// </summary>
        /// <param name="course">The course where the group is located.</param>
        /// <param name="groupID">The group object.</param>
        /// <returns>
        ///     A list of all e-mail addresses (unique!) of the students that are subscribed to the given group.
        ///     If the group is empty (no subscribed students) then it will return an empty list.    
        /// </returns>
        abstract public List<String> getUsersFromGroup(Course course, Group groupID);

        #endregion

    }
}
