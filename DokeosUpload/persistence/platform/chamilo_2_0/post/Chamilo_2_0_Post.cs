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
using lmsda.domain.exercise;
using lmsda.persistence.httpcommunication;
using lmsda.domain;
using lmsda.domain.score;
using System.Xml;
using lmsda.domain.util.xml;
using lmsda.domain.util;
using System.IO;
using lmsda.persistence.platform.chamilo_2_0;
using lmsda.domain.score.data;
using System.Text.RegularExpressions;

namespace lmsda.persistence.platform.chamilo_2_0.post
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    ///     This class provides the functionality of the Chamilo 2.0 platform through post methods.
    /// </summary>
    class Chamilo_2_0_Post : TargetPlatform
    {
        protected const String INDEX          = "/index.php?application=user&go=login";
        protected const String PORTAL         = "/index.php?application=home";
        protected const String LOGOUT         = "/index.php?application=user&go=logout";
        protected const String ACCOUNT        = "/index.php?application=user&go=account";
        protected const String COURSES        = "/index.php?application=weblcms";
        protected const String DOWNLOAD_LINK  = "/index.php?go=document_downloader&display=1&application=repository&object=";
        protected const String AUTO_UPLOAD_STRING = "Uploaded with LMS Desktop Assistant";
        protected const String OBJID_ANY                  = "disabled_0";
        protected const String OBJID_FILE                 = "document";
        protected const String OBJID_EXERCISE             = "assessment";

        protected const String OBJID_QUESTION_BLANKS      = "fill_in_blanks_question";
        protected const String OBJID_QUESTION_HOTPOTATOES = "hotpotatoes";
        protected const String OBJID_QUESTION_HOTSPOT     = "hotspot_question";
        protected const String OBJID_QUESTION_MATCH       = "match_question";
        protected const String OBJID_QUESTION_MATCHING    = "assessment_matching_question";
        protected const String OBJID_QUESTION_MATCHTEXT   = "assessment_match_text_question";
        protected const String OBJID_QUESTION_MATRIC      = "assessment_matrix_question";
        protected const String OBJID_QUESTION_MULTIPLE    = "assessment_multiple_choice_question";
        protected const String OBJID_QUESTION_MATCHNUM    = "assessment_match_numeric_question";
        protected const String OBJID_QUESTION_OPEN        = "assessment_open_question";
        protected const String OBJID_QUESTION_ORDERING    = "ordering_question";
        protected const String OBJID_QUESTION_RATING      = "assessment_rating_question";
        protected const String OBJID_QUESTION_SELECT      = "assessment_select_question";

        protected const String ASSESSMENTS_CATEGORY_NAME = "Assessments";

        protected const String EXERCISE_IMAGE_PREFIX = "exerpic";
        protected const String LMS_AUTO_SCAN = "lms-({0})-";
        protected const String TRUNCATE_STRING = "…";
        protected Course repositoryDummy = new Course("Repository", "Repository", "-1");
        protected Encoding encoding;
        protected Login login;
        protected String firstName;
        protected String lastName;
        
        /// <summary>
        ///     List of all repository folders, in the format String[folderName,folderId]
        /// </summary>
        protected List<String[]> repositoryFolders;
        protected List<Course> courses;
        private TargetPlatformInfo platformInfo = new Chamilo_2_0_PlatformInfo();

        #region Constructor & platform information

        /// <summary>
        ///     Instantiates a new Chamilo 2.0 (POST) object.
        /// </summary>
        /// <param name="login">The login information.</param>
        public Chamilo_2_0_Post(Login login)
        {
            this.login = login;
            String enc = DomainController.Instance().getSettings().getEncoding();
            this.encoding = Encoding.GetEncoding((enc == null || enc.Equals(String.Empty)) ? platformInfo.getPlatformEncoding() : enc);
        }

        public override String getPlatformName()
        {
            return platformInfo.getPlatformName();
        }

        #endregion

        #region Public login

        public override Boolean tryLogin()
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            Boolean loggedIn = this.doLogin(httpSession);

            if (loggedIn)
            {
                this.readUserFullName(httpSession);
                this.scanRepositoryFolders(httpSession);
                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            
            return loggedIn;
        }

        #endregion

        #region Retrieving information

        public override List<Course> readCourses()
        {
            this.courses = new List<Course>();

            HttpSession httpSession = new HttpSession(this.encoding);

            if (this.doLogin(httpSession))
            {
                String page;

                httpSession.setRequestUrl(this.login.getPlatformUrl() + COURSES);
                httpSession.sendPOSTrequestSimple();

                page = httpSession.getResponseFromServer();

                XmlDocument xmlCoursesPage = SGMLReaderHelper.htmlToXmlDocument(page);
                String xhtmlPage = SGMLReaderHelper.htmlToXhtmlString(page);

                CourseScanner courseScanner = new CourseScanner();
                courseScanner.doScan(xmlCoursesPage.ChildNodes);
                this.courses = courseScanner.getCourses();
                this.courses.Insert(0,repositoryDummy);
                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }

            return this.courses;
        }

        public override Boolean readDocumentFolders(ref List<Course> courses, Boolean useLazyLoading)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            try
            {
                if (this.doLogin(httpSession))
                {
                    foreach (Course targetCourse in courses)
                    {
                        if (!useLazyLoading || targetCourse.getDocumentFolders().getDocumentFolders().Count == 0)
                        {
                            DomainController.Instance().writeToLog("fetching_folders_list_for_x", new String[] { targetCourse.getCourseCode() }, true, false, false);
                            targetCourse.setDocumentFolders(new DocumentFoldersList(readDocumentFolders(httpSession, targetCourse)));
                            DomainController.Instance().writeToLog("folders_list_fetched", new String[] { targetCourse.getCourseCode() }, true, false, false);
                        }
                    }
                    this.doLogout(httpSession);
                    return true;
                }
                else
                {
                    foreach (Course targetCourse in courses)
                    {
                        targetCourse.setDocumentFolders(new DocumentFoldersList()); ;
                    }
                    DomainController.Instance().writeToLog("cannot_login", true, false);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public override String getFormattedFilenameString(String filenameToFormat)
        {
            return filenameToFormat.Trim('/');
        }

        #endregion

        #region Sending information

        public override String[] doUploadFile(String uploadFilePath, String uploadedName, String description, DocumentFolder publishDestination, Subject subject, Boolean setInvisible)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            List<String> retn = new List<String>();
            String fileId = String.Empty;
            Boolean isNew = false;
            if (this.doLogin(httpSession))
            {
                String platformDestinationId = this.getPlatformDestinationId(httpSession, publishDestination, subject, true);

                // special case for files: cut off path
                String searchString = uploadFilePath;
                if (uploadFilePath.Contains(Path.DirectorySeparatorChar.ToString()))
                    try { searchString = new FileInfo(uploadFilePath).Name; }
                    catch { }


                //If the file already exists, edit it instead of uploading a new file.
                fileId = this.getIdFromObjectInCategory(httpSession, searchString, platformDestinationId, OBJID_FILE);

                if (fileId.Equals(String.Empty))
                {
                    //Upload file...
                    fileId = this.uploadSingleFile(httpSession, uploadFilePath, uploadedName, description, platformDestinationId);
                    isNew = true;
                }
                else
                {
                    //EDIT!
                    //this.unlinkObject(httpSession, fileId);
                    if (!this.doUpdateFile(httpSession, fileId, platformDestinationId, uploadFilePath, uploadedName, description))
                        fileId=String.Empty; // upload failed
                }

                //Do something with the modified date. Save it to the file?
                //Temporarily disabled to avoid extra request.
                //String modifiedDate = this.getModifiedDate(httpSession, fileId, platformDestinationId);

                if (!fileId.Equals(String.Empty))
                {
                    //Publish to courses...
                    updatePublications(httpSession, publishDestination, fileId, isNew, !setInvisible, OBJID_FILE);
                    
                    foreach (DocumentFolderForCourse dffc in publishDestination.folders)
                        retn.Add(this.login.getPlatformUrl() + DOWNLOAD_LINK + fileId);
                }

                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            if (fileId == null)
                return null;
            else
                return retn.ToArray();
        }

        public override DocumentFolder doCreateFolderInDocuments(DocumentFolder publishDestination, String foldername, Boolean setInvisible)
        { 
            DocumentFolder df = null;
            HttpSession httpSession = new HttpSession(this.encoding);
            if (this.doLogin(httpSession))
            {
                foreach (DocumentFolderForCourse folder in publishDestination.folders)
                {
                    DocumentFolder dfl = createFolderInDocumentsRecursive(httpSession, folder.course, foldername, folder.folderCode, publishDestination.folderName, df == null);

                    if (df == null)
                        df = dfl;
                    else      // multiple courses in documentfolder; merge them.
                        df.mergeFolders(dfl);
                }
                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            return df;
        }

        public override Boolean doUpdateFile(String filename, DocumentFolder publishDestinations, Subject subject, String description, Boolean visible)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            if (this.doLogin(httpSession))
            {

                // get the category id
                String platformDestinationId = this.getPlatformDestinationId(httpSession, publishDestinations, subject, false);

                if (platformDestinationId == null || platformDestinationId.Equals(String.Empty))
                    return false;

                //get the file from that category
                String fileId = this.getIdFromObjectInCategory(httpSession, filename, platformDestinationId, OBJID_FILE);

                if (fileId == null || fileId.Equals(String.Empty))
                    return false;

                // unlink and republish, to avoid extra requests to look up the link ID.
                // Side effect: it's shown as new publication. Maybe look up?
                //this.unlinkObject(httpSession, fileId);

                // update object itself
                Boolean updatesuccess = this.doUpdateFile(httpSession, fileId, platformDestinationId, null, filename, description);

                this.updatePublications(httpSession, publishDestinations, fileId, false, visible, OBJID_FILE);
                // Do something with the modified date. Save it to the file?
                // Temporarily disabled to avoid extra request.
                //String modifiedDate = this.getModifiedDate(httpSession, fileId, platformDestinationId);

                this.doLogout(httpSession);

                return updatesuccess;
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            return false;

        }

        public override Boolean doUploadExercises(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions, Boolean onlyIfDoesntExist)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            try
            {
                if (this.doLogin(httpSession))
                {
                    int exerciseNumber = 0;
                    List<String[]> exerciseIds = new List<String[]>();
                    
                    String categoryId = getPlatformDestinationId(httpSession, publishDestinations, ASSESSMENTS_CATEGORY_NAME, subject);

                    foreach (Exercise exercise in exercises)
                    {
                        exerciseNumber++;
                        String[] exerciseResult=this.uploadSingleExercise(httpSession, exercise, exerciseNumber, categoryId, randomQuestions, onlyIfDoesntExist);
                        String exerciseId= exerciseResult[0];
                        if (!exerciseId.Equals(String.Empty))
                            exerciseIds.Add(exerciseResult);
                    }
                    foreach (String[] exerciseResult in exerciseIds)
                    {
                        Boolean isNew = Convert.ToBoolean(Convert.ToInt32(exerciseResult[1]));
                        this.updatePublications(httpSession, publishDestinations, exerciseResult[0], isNew, !setExerciseInvisible, OBJID_EXERCISE);
                    }

                    this.doLogout(httpSession);
                    DomainController.Instance().writeToLog("exercises_upload_completed", true, false, !DomainController.Instance().isSynchronization);
                    return true;
                }
                else
                {
                    DomainController.Instance().writeToLog("cannot_login", true, false);
                    return false;
                }
            }
            catch(Exception e)
            {
                DomainController.Instance().writeToLog("exercises_upload_failed", true, false, !DomainController.Instance().isSynchronization);
                DomainController.Instance().writeToLog(e.Message, false, true, false);
                return false;
            }
        }

        public override Boolean doUpdateExercisesInfo(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            try
            {
                if (this.doLogin(httpSession))
                {
                    int exerciseNumber = 0;
                    List<String> exerciseIds = new List<String>();
                    String categoryId = getPlatformDestinationId(httpSession, publishDestinations, ASSESSMENTS_CATEGORY_NAME, subject);


                    Boolean updateSucceeded = true;

                    foreach (Exercise exercise in exercises)
                    {
                        exerciseNumber++;
                        DomainController.Instance().writeToLog("updating_exercise_x_course_y", new String[] { exerciseNumber.ToString(), "repository" }, true, false, false);
                        String exerciseId = getIdFromObjectInCategory(httpSession, exercise.getName(), categoryId, OBJID_EXERCISE);
                        if (exerciseId.Equals(String.Empty))
                            updateSucceeded = false;
                        else
                        {
                            exerciseIds.Add(exerciseId);
                            this.updateExerciseInfo(httpSession, exerciseId, exercise, categoryId, randomQuestions);
                        }
                    }
                    foreach (String exerciseId in exerciseIds)
                        this.updatePublications(httpSession, publishDestinations, exerciseId, false, !setExerciseInvisible,OBJID_EXERCISE);

                    this.doLogout(httpSession);
                    DomainController.Instance().writeToLog("exercises_update_completed", true, false, !DomainController.Instance().isSynchronization);
                    return updateSucceeded;
                }
                else
                {
                    DomainController.Instance().writeToLog("cannot_login", true, false);
                    return false;
                }
            }
            catch
            {
                // add error here?
                DomainController.Instance().writeToLog("exercises_upload_failed", true, false, !DomainController.Instance().isSynchronization);
                return false;
            }
        }

        #endregion

        #region Statistics

        /*
         * Statistics are not yet implemented in Chamilo 2.0!
         */

        public override List<Scores> downloadStatistics(List<Course> courses, String saveTo, Boolean createSubFolder, Boolean withoutMissingData)
        {
            throw new NotImplementedException("statistics_available_on_the_platform");
        }

        #endregion

        #region private methods

        /// <summary>
        ///     Fetches the DocumentFolderList for a course from a logged in httpsession
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="course">The fourse from which to fetch the folders.</param>
        /// <returns>A List of DocumentFolder objects containing the folders of the course</returns>
        protected List<DocumentFolder> readDocumentFolders(HttpSession httpSession, Course course)
        {
            if (isRepository(course))
            {
                scanRepositoryFolders(httpSession);
                return getRepositoryCategoriesAsDocumentfolders();
            }
            List<DocumentFolder> folders = new List<DocumentFolder>();
            String chamiloUrl = this.login.getPlatformUrl();
            String docUrl = "/index.php?application=weblcms&course=" + course.getCourseId() + "&go=course_viewer&tool=document&tool_action=category_manager&browser=table&category_id=0";
            httpSession.setRequestUrl(chamiloUrl + docUrl);
            httpSession.sendPOSTrequestSimple();

            String page = httpSession.getResponseFromServer();
            XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(page);
            String foldersPage = SGMLReaderHelper.htmlToXhtmlString(page);

            FolderScanner folderScanner = new FolderScanner();
            folderScanner.doScan(xmlFoldersPage.ChildNodes);

            foreach (String[] s in folderScanner.getFolders())
                folders.Add(new DocumentFolder(course, s[1], s[0]));

            return folders;
        }

        protected List<DocumentFolder> getRepositoryCategoriesAsDocumentfolders()
        {
            List<DocumentFolder> folders = new List<DocumentFolder>();
            foreach (String[] folder in repositoryFolders)
                folders.Add(new DocumentFolder(repositoryDummy, folder[1], folder[0]));
            return folders;

        }

        /// <summary>
        ///     Uploads or updates a single exercise in the repository of the platform.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="exercise">The exercise.</param>
        /// <param name="exerciseNumber">Number of the exercise in the full list of exercises that is being processed.</param>
        /// <param name="categoryId">the category to upload it to.</param>
        /// <param name="randomQuestions">amount of random questions to show when making the exercise. 0 for none.</param>
        /// <param name="noUpdate">Only create new ones; don't update existing ones</param>
        protected String[] uploadSingleExercise(HttpSession httpSession, Exercise exercise, int exerciseNumber, String categoryId, int randomQuestions, Boolean noUpdate)
        {
            if (exercise.getQuestionsAsList().Count == 0)
                return new String[] {String.Empty, "0"};
            Exercise exercise_clone = exercise.clone();
            String[] exerciseResult = this.createOrUpdateExercise(httpSession, exercise_clone, exerciseNumber, categoryId, randomQuestions, noUpdate);
            String exerciseId = exerciseResult[0];
            Boolean isNew = Convert.ToBoolean(Convert.ToInt32(exerciseResult[1]));

            if (!exerciseId[0].Equals(String.Empty)) // empty = cancelled by noUpdate parameter.
            {
                this.uploadExerciseImages(httpSession, EXERCISE_IMAGE_PREFIX, exercise_clone, exerciseId, categoryId, exerciseNumber);
                // necessary if exercise description contains images
                this.updateExerciseInfo(httpSession, exerciseId, exercise_clone, categoryId, randomQuestions);
                DomainController.Instance().writeToLog("uploading_questions_exercise_x_course_y", new String[] { exerciseNumber.ToString(), "repository" }, true, false, false);
                this.addQuestionsToExercise(httpSession, exerciseId, categoryId, exercise_clone);
            }
            return exerciseResult;
        }

        /// <summary>
        ///     Creates a new exercise, or updates an existing one and deletes its questions. The result of this is ALWAYS an exercise without any questions in it.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="exercise">The exercise.</param>
        /// <param name="categoryId">the category to upload it to.</param>
        /// <param name="exerciseNumber">Number of the exercise in the full list of exercises that is being processed.</param>
        /// <param name="randomQuestions">Number of random questions to set for this exercise.</param>
        /// <param name="noUpdate">If the exercise already exists, perform no ations and don't return ID (to prevent overwrite).</param>
        /// <returns>The exercise ID.</returns>
        protected String[] createOrUpdateExercise(HttpSession httpSession, Exercise exercise, int exerciseNumber, String categoryId, int randomQuestions, Boolean noUpdate)
        {
            String platformUrl = this.login.getPlatformUrl();
            String exerciseId = getIdFromObjectInCategory(httpSession, exercise.getName(), categoryId, OBJID_EXERCISE);
            Boolean isNew = exerciseId.Equals(String.Empty);

            if (!isNew && noUpdate)
                return new String[]{String.Empty, "0"};
            if (isNew)
            {
                // Create new exercise
                DomainController.Instance().writeToLog("creating_exercise_x_course_y", new String[] { exerciseNumber.ToString(), "repository" }, true, false, false);
                String exercisebuilder = "/index.php?category=" + categoryId +
                                       "&renderer=table&application=repository&go=creator&content_object_type=assessment";
                httpSession.setRequestUrl(platformUrl + exercisebuilder );

                httpSession.addNameValuePair("title", exercise.getName());
                httpSession.addNameValuePair("parent_id", categoryId);
                String description = exercise.getDescription();
                if (description == null || description.Equals(String.Empty))
                    description = "<br />";
                httpSession.addNameValuePair("description", description);
                
                httpSession.addNameValuePair("all_questions", Convert.ToInt32(exercise.isMultipage()).ToString());
                httpSession.addNameValuePair("questions_per_page", exercise.isMultipage() ? "1" : String.Empty); // no options for # of  questions per page in LMSDA

                httpSession.addNameValuePair("unlimited_attempts", "0"); // Option 0: Unlimited attempts
                httpSession.addNameValuePair("max_attempts", "0");       // number of attempts: 0

                httpSession.addNameValuePair("unlimited_time", "0"); // No options for this in LMSDA
                httpSession.addNameValuePair("max_time", String.Empty); // 

                httpSession.addNameValuePair("random", randomQuestions>0 ? "0":"1");
                httpSession.addNameValuePair("random_questions", Convert.ToString(randomQuestions));
                
                httpSession.addNameValuePair("submit", "Create");
                httpSession.addNameValuePair("_qf__create", "");

                httpSession.sendPOSTrequestFromForm();

                String responseUrl = httpSession.getResponseUrl();
                exerciseId = Utility.findValueInURL(responseUrl, "object");

                if (exerciseId.Equals(String.Empty))
                {
                    throw new Exception("Creation of exercise failed");
                }
            }
            else
            {
                // Update existing exercise
                DomainController.Instance().writeToLog("updating_exercise_x_course_y", new String[] { exerciseNumber.ToString(), "repository" }, true, false, false);
                List<String[]> currentQuestions = getQuestionIdsFromExercise(httpSession, exerciseId);
                List<String> quesIds = new List<String>();
                foreach (String[] quesId in currentQuestions)
                {
                    unlinkComplexObject(httpSession, exerciseId, quesId[1]); // quesId[1] = realId
                    // unlink in one bulk request.
                    //deleteObjectIfNotUsed(httpSession, quesId[0]); // quesId[0] = linkId
                    quesIds.Add(quesId[0]);
                }
                if (quesIds.Count > 0)
                    deleteObjectsIfNotUsed(httpSession,quesIds.ToArray());

                // update basic text of exercise
                updateExerciseInfo(httpSession, exerciseId, exercise, categoryId, randomQuestions);
            }
            return new String[]{ exerciseId, Convert.ToInt32(isNew).ToString()};
        }

        protected void updateExerciseInfo(HttpSession httpSession, String exerciseId, Exercise exercise, String categoryId, int randomQuestions)
        {
            String platformUrl = this.login.getPlatformUrl();
            String exercisebuilder = "/index.php?renderer=table&application=repository&go=editor&object=" + exerciseId;
            httpSession.setRequestUrl(platformUrl + exercisebuilder );

            httpSession.addNameValuePair("title", exercise.getName());
            httpSession.addNameValuePair("parent_id", categoryId);
            String description = exercise.getDescription();
            if (description == null || description.Equals(String.Empty))
                description = "<br />";
            httpSession.addNameValuePair("description", description);
            httpSession.addNameValuePair("unlimited_attempts", "0"); // Option 0: Unlimited attempts
            httpSession.addNameValuePair("max_attempts", "0");       // Number of attempts: 0

            httpSession.addNameValuePair("all_questions", Convert.ToInt32(exercise.isMultipage()).ToString());
            httpSession.addNameValuePair("questions_per_page", exercise.isMultipage() ? "1" : String.Empty); // no options for # of  questions per page in LMSDA

            httpSession.addNameValuePair("unlimited_time", "0"); // No options for this in LMSDA
            httpSession.addNameValuePair("max_time", String.Empty); // 

            httpSession.addNameValuePair("random", randomQuestions>0 ? "0":"1");
            httpSession.addNameValuePair("random_questions", Convert.ToString(randomQuestions)); // 
                
            httpSession.addNameValuePair("submit", "Update");
            httpSession.addNameValuePair("_qf__edit", "");
            httpSession.addNameValuePair("id", exerciseId);

            httpSession.sendPOSTrequestFromForm();

            String responseUrl = httpSession.getResponseUrl();
            
            //if (Utility.findValueInURL(responseUrl, "message").Equals(String.Empty))
            //{
            //    String errormessage = Utility.findValueInURL(responseUrl, "error_message").Replace('+',' ');
            //    throw new Exception("Creation of exercise failed: \"" + errormessage +"\"");
            //}
        }


        /// <summary>
        ///     Uploads the images from an exercise, and changes the exercise content to contain platform image URLs.
        /// </summary>
        /// 
        /// <param name="httpSession">The working session.</param>
        /// <param name="filenamePrefix">Prefix string to put before all images.</param>
        /// <param name="exercise">The exercise.</param>
        /// <param name="exerciseId">The ID of the exercise on the platform.</param>
        /// <param name="categoryId">the ID of the category to upload the images in.</param>
        /// <param name="exerciseNumber">The number of the exercise in the series of uploaded exercises.</param>
        protected void uploadExerciseImages(HttpSession httpSession, String filenamePrefix, Exercise exercise, String exerciseId, String categoryId, int exerciseNumber)
        {
            deleteImagesForExercise(httpSession, filenamePrefix, exerciseId, categoryId);

            Dictionary<String, String> uploadedFiles = new Dictionary<String, String>();
            exercise.setDescription(this.uploadImagesFromDescription(httpSession, exercise.getDescription(), filenamePrefix, exerciseId, categoryId, uploadedFiles, exerciseNumber));
            List<Question> questions = exercise.getQuestionsAsList();
            foreach (Question question in questions)
            {
                question.setQuestionText(this.uploadImagesFromDescription(httpSession, question.getQuestionText(), filenamePrefix, exerciseId, categoryId, uploadedFiles, exerciseNumber));
                List<Answer> answers = question.getAnswersAsList();
                foreach (Answer answer in answers)
                {
                    answer.setAnswer(this.uploadImagesFromDescription(httpSession, answer.getAnswer(), filenamePrefix, exerciseId, categoryId, uploadedFiles, exerciseNumber));
                    answer.setMatchAnswer(this.uploadImagesFromDescription(httpSession, answer.getMatchAnswer(), filenamePrefix, exerciseId, categoryId, uploadedFiles, exerciseNumber));
                    answer.setFeedback(this.uploadImagesFromDescription(httpSession, answer.getFeedback(), filenamePrefix, exerciseId, categoryId, uploadedFiles, exerciseNumber));
                }
            }
        }

        /// <summary>
        ///     Uploads the images from a string.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="input">The input string.</param>
        /// <param name="filenamePrefix">Extra prefix of the filename. Leave null or empty for none</param>
        /// <param name="exerciseId">The ID of the exercise on the platform.</param>
        /// <param name="categoryId">the ID of the category to upload the images in.</param>
        /// <param name="uploadedList">Reference list of uploaded images, to avoid duplicates.</param>
        /// <param name="exerciseNumber">The number of the exercise in the series of uploaded exercises.</param>
        /// <returns>The description with all image links replaced with links to the images uploaded on the platform.</returns>
        protected String uploadImagesFromDescription(HttpSession httpSession, String input, String filenamePrefix, String exerciseId, String categoryId, Dictionary<String, String> uploadedList, int exerciseNumber)
        {
            if (input == null)
                return null;

            Regex r = new Regex(@"<img(.*?)>\s*", RegexOptions.IgnoreCase);
            Match matcher = r.Match(input);
            while (matcher.Success)
            {
                // retrying the upload doesn't fix the upload problem, so I'm leaving it 0 for now.
                int retry = 0;

                String orig_link = matcher.Groups[0].Value;
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value, "src=\"(.*?)\"");

                String uploadpath = DomainController.Instance().getTempPath() + urlmatcher.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar);
                String new_link;
                if (!uploadedList.ContainsKey(uploadpath))
                {
                    if (uploadedList.Count == 0)
                        DomainController.Instance().writeToLog("uploading_images_exercise_x_course_y", new String[] { exerciseNumber.ToString(), "repository" }, true, false, false);
                    FileInfo uploadFile = new FileInfo(uploadpath);
                    long randvalue = Math.Abs(DateTime.Now.ToBinary());
                    Random random = new Random((int)randvalue);
                    // make sure random value is actually random so students can't see the intended order from the filename
                    randvalue += random.Next(Int32.MaxValue / 2, Int32.MaxValue) * 10000000; //(push into the seconds range)

                    // filename on platform: exercise ID + filenameprefix +  obfuscated datetime as unique string + original extension
                    String uploadFileName = 
                        (filenamePrefix == null || filenamePrefix.Equals(String.Empty) ? String.Empty : filenamePrefix + "_")
                        + exerciseId + '_'
                        + randvalue.ToString() + uploadFile.Extension;

                    new_link = uploadSingleFile(httpSession, uploadFile.FullName, uploadFileName, AUTO_UPLOAD_STRING, categoryId, retry);
                    string resourcetag;
                       if (!new_link.Equals(String.Empty))
                    {
                        string height = Utility.findPropertyInXmlNodeText(orig_link, "height");
                         string width = Utility.findPropertyInXmlNodeText(orig_link, "width");
                        resourcetag = "<resource width='" + width + "' height='" + height + "' source='" + new_link +
                                      "'  type='document'></resource>";
                        new_link = resourcetag;
                    }
                    //if (!new_link.Equals(String.Empty))
                    //{

                    //    // convert ID to download url
                    //    new_link = this.login.getPlatformUrl() + DOWNLOAD_LINK + new_link;
                    //    //new_link = "<img src=\"" + new_link + "\" />";
                    //    new_link = orig_link.Replace(urlmatcher.Groups[0].Value, "src=\"" + new_link + "\"").Trim();
                    //    // remove ALT text
                    //    string img_alt = Utility.findPropertyInXmlNodeText(orig_link, "alt");
                    //    if (!img_alt.Equals(String.Empty))
                    //        new_link = new_link.Replace("alt=\"" + img_alt + "\"", "alt=\"x\"");
                    //    // remove title
                    //    string img_title = Utility.findPropertyInXmlNodeText(orig_link, "title");
                    //    if (!img_title.Equals(String.Empty))
                    //        new_link = new_link.Replace("title=\"" + img_title + "\"", "title=\"\"");
                    //    // remove CSS style information
                    //    string img_id = Utility.findPropertyInXmlNodeText(orig_link, "id");
                    //    if (!img_id.Equals(String.Empty))
                    //        new_link = new_link.Replace("id=\"" + img_id + "\" ", String.Empty) // remove extra space
                    //                            .Replace("id=\"" + img_id + "\"", String.Empty);
                    //}
                    else
                    {
                        new_link = "[" + uploadFileName + " failed to upload]";
                    }
                    uploadedList.Add(uploadpath, new_link);
                }
                else
                {
                    uploadedList.TryGetValue(uploadpath, out new_link);
                }

                input = input.Replace(orig_link, new_link);
                matcher = matcher.NextMatch();
            }

            return input;
        }


        /// <summary>
        ///     Deletes the images of an exercise.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="filenamePrefix">Extra prefix of the filename. Leave null or empty for none</param>
        /// <param name="exerciseId">The exercide ID.</param>
        /// <param name="categoryId">The category ID.</param>
        protected void deleteImagesForExercise(HttpSession httpSession, String filenamePrefix, String exerciseId, String categoryId)
        {
            List<String[]> images = getAllObjectsInCategory(httpSession, categoryId, OBJID_FILE);
            String startString = exerciseId + '_';
            if (filenamePrefix != null && !filenamePrefix.Equals(String.Empty))
                startString = filenamePrefix + '_' + startString;

            foreach (String[] image in images)
            {
                if (image[1].StartsWith(startString))
                {
                    deleteObjectFromId(httpSession,image[0]);
                }
            }
        }


        /// <summary>
        ///     Adds a question to a exercise.
        /// </summary>
        /// <param name="exerciseId">The exercise ID.</param>
        /// <param name="exercise">the exercise object.</param>
        /// <param name="httpSession">The working session.</param>
        protected void addQuestionsToExercise(HttpSession httpSession, String exerciseId, String categoryId, Exercise exercise)
        {
            List<Question> questions = exercise.getQuestionsAsList();
            String platformUrl = this.login.getPlatformUrl();
            String questionType = String.Empty;
            // excludedLinkIDs list is used to prevent extra requests to already processed link IDs.
            foreach (Question question in questions)
            {
                if (question.getAnswersAsList().Count < 1)
                    break;
                String scantitle = String.Format(LMS_AUTO_SCAN, new Random().Next(1000000));
                // set question type
                switch (question.getQuestionType())
                {
                    case QuestionType.MULTIPLE_CHOICE_SINGLE:
                    case QuestionType.MULTIPLE_CHOICE_SEVERAL:
                        questionType = OBJID_QUESTION_MULTIPLE; break;
                    case QuestionType.FILL_IN_THE_GAPS:
                    case QuestionType.FILL_IN_THE_GAPS_DROPDOWN:
                        questionType = OBJID_QUESTION_BLANKS; break;
                    case QuestionType.MATCHING:
                        questionType = OBJID_QUESTION_MATCHING; break;
                    case QuestionType.OPEN_QUESTION:
                        questionType = OBJID_QUESTION_OPEN; break;
                }

                // refresh exercise page. Not sure why, but this seems to solve some problems.
                // Might be related to setting the starting number of options in the session.
                String exerciseRefreshUrl = "/index.php?renderer=table&application=repository&object=" + exerciseId
                                      + "&go=builder&builder_action=creator&type=" + questionType;
                httpSession.setRequestUrl(platformUrl + exerciseRefreshUrl);
                httpSession.sendPOSTrequestSimple();

                String exerciseCreateUrl = "/index.php?renderer=table&application=repository&object=" + exerciseId
                                   + "&go=builder&builder_action=creator&repoviewer_action=creator&type=" + questionType;
                httpSession.setRequestUrl(platformUrl + exerciseCreateUrl);

                String questionTitle = question.getQuestionTitle().Trim();
                if (questionTitle == null || questionTitle.Equals(String.Empty))
                    questionTitle = "_";
                
                // new: scan weight from title. end title on " +#" to set weight (with # a number)
                String weight = Utility.findWeightInString(questionTitle, true);
                if (!weight.Equals(String.Empty))
                    questionTitle = questionTitle.Substring(0, questionTitle.Length - weight.Length).TrimEnd();
                // start of name-value pairs buildup
                httpSession.addNameValuePair("parent_id", categoryId);
                String description = question.getQuestionText();
                if (description == null || description.Equals(String.Empty))
                    description = "<br />";
                httpSession.addNameValuePair("description", description);
                
                int questionWeight=0;
                switch (question.getQuestionType())
                {
                    case QuestionType.MULTIPLE_CHOICE_SINGLE:
                    case QuestionType.MULTIPLE_CHOICE_SEVERAL:
                        questionWeight = this.addAnswersMC(httpSession, question);
                        break;
                    case QuestionType.FILL_IN_THE_GAPS:
                    case QuestionType.FILL_IN_THE_GAPS_DROPDOWN:
                        questionWeight = this.addAnswersGaps(httpSession, question);
                        break;
                    case QuestionType.MATCHING:
                        questionWeight = this.addAnswersMatching(httpSession, question);
                        break;
                    case QuestionType.OPEN_QUESTION:
                        questionWeight = this.addOpenQuestion(httpSession, question);
                        break;
                }
                if (weight.Equals(String.Empty))
                    weight = questionWeight.ToString();
                httpSession.addNameValuePair("weight", weight);

                // make backup of completely created exercise
                // without LMS_AUTO_SCAN title or CREATE method.
                List<String[]> questionData = httpSession.getNameValuePairs();

                // scan title
                httpSession.addNameValuePair("title", scantitle + questionTitle);
                // submit parameters
                httpSession.addNameValuePair("submit", "Create");
                httpSession.addNameValuePair("_qf__create", "");
                // Primary submit for creation
                httpSession.sendPOSTrequestFromForm();

                // find the question that was just created, to edit it.
                List<String[]> storedQuestions = getQuestionIdsFromExercise(httpSession, exerciseId);
                foreach (String[] ques in storedQuestions)
                {
                    // ques[2] = question name
                    if (ques[2].StartsWith(scantitle) ||
                        Utility.isTruncatedVersionOf(ques[2], scantitle, TRUNCATE_STRING))
                    {
                        // final inserts with weight and adjusted number of options (ques[1] = link ID)
                        String exerciseEditUrl = "/index.php?renderer=table&application=repository&object=" + exerciseId
                                        + "&go=builder&builder_action=updater&selected_cloi=" + ques[1];

                        httpSession.setRequestUrl(platformUrl + exerciseEditUrl);
                        // restore exercise data
                        httpSession.setNameValuePairs(questionData);
                        // add correct title
                        httpSession.addNameValuePair("title", questionTitle);
                        // exercise Id to edit
                        httpSession.addNameValuePair("id", ques[0]); // question ID
                        // versioning comment; not used
                        httpSession.addNameValuePair("comment", String.Empty);
                        // submit as edit
                        httpSession.addNameValuePair("_qf__edit", "");

                        // at this point, the edit data is complete, to potentially be
                        // used in multiple requests by saving the name-value pairs.

                        switch (question.getQuestionType())
                        {
                            case QuestionType.MULTIPLE_CHOICE_SINGLE:
                            case QuestionType.MULTIPLE_CHOICE_SEVERAL:
                                this.submitMCQuestion(httpSession, question, exerciseId, categoryId, ques[0], ques[1]);
                                break;
                            case QuestionType.MATCHING:
                                submitMatchingQuestion(httpSession, question, exerciseId, categoryId, ques[0], ques[1]);
                                break;
                            case QuestionType.FILL_IN_THE_GAPS:
                            case QuestionType.FILL_IN_THE_GAPS_DROPDOWN:
                            case QuestionType.OPEN_QUESTION:
                                this.submitSimpleQuestion(httpSession);
                                break;
                        }
                        break;
                    }
                }
            }
        }

        protected void submitMCQuestion(HttpSession httpSession, Question question, String exerciseId, String categoryId, String questionId, String linkId)
        {
            List<String[]> questionData = httpSession.getNameValuePairs();
            String requestUrl = httpSession.getRequestUrl();
            // extra request needed to switch multiple choice to checkboxes mode
            if (question.getQuestionType() == QuestionType.MULTIPLE_CHOICE_SEVERAL)
            {
                // don't need to set pairs & url since it's always the first action
                httpSession.addNameValuePair("change_answer_type", "SwitchToRadioButtons");
                httpSession.sendPOSTrequestFromForm(); // switch to radio buttons
            }

            int addQuestions = question.getAnswersAsList().Count - 3;
            for (int add = 0; add < addQuestions; add++)
            {
                httpSession.setRequestUrl(requestUrl);
                httpSession.setNameValuePairs(questionData);
                httpSession.addNameValuePair("add[]", "Add an option");
                httpSession.sendPOSTrequestFromForm();
            }
            httpSession.setRequestUrl(requestUrl);
            httpSession.setNameValuePairs(questionData);
            submitSimpleQuestion(httpSession);
        }

        protected void submitMatchingQuestion(HttpSession httpSession, Question question, String exerciseId, String categoryId, String questionId, String linkId)
        {
            List<String[]> questionData = httpSession.getNameValuePairs();
            String requestUrl = httpSession.getRequestUrl();

            // EXTRA REQUEST:
            // remove option: 
            // remove_option[i].x  (no idea about the values)
            // remove_option[i].y
            // add option:
            // httpSession.addNameValuePair("add_option[]","Add an option");
            // Remove match;
            // remove_match[i]
            // add match: 
            // httpSession.addNameValuePair("add_match[]","Add a match");

            int answers = -3;
            int matches = -3;
            List<Answer> answersRaw = new List<Answer> (question.getAnswersAsList());

            // cut off at Z
            if (answersRaw.Count > 26)
                answersRaw.RemoveRange(26, answersRaw.Count - 26);

            foreach (Answer ans in answersRaw)
            {
                if (ans.getAnswer() != null && !ans.getAnswer().Equals(String.Empty))
                    answers++;
                if (ans.getMatchAnswer() != null && !ans.getMatchAnswer().Equals(String.Empty))
                    matches++;
            }

            // adjust answers
            for (int count = 0; count < answers; count++)
            {
                httpSession.setRequestUrl(requestUrl);
                httpSession.setNameValuePairs(questionData);
                httpSession.addNameValuePair("add_option[]", "Add an option");
                httpSession.sendPOSTrequestFromForm();
            }
            // adjust matches
            for (int count = 0; count < matches; count++)
            {
                httpSession.setRequestUrl(requestUrl);
                httpSession.setNameValuePairs(questionData);
                httpSession.addNameValuePair("add_match[]", "Add a match");
                httpSession.sendPOSTrequestFromForm();
            }                
            httpSession.setRequestUrl(requestUrl);
            httpSession.setNameValuePairs(questionData);
            submitSimpleQuestion(httpSession);
        }

        protected void submitSimpleQuestion(HttpSession httpSession)
        {
            // just send. No extra options.
            httpSession.addNameValuePair("submit", "Update");
            httpSession.sendPOSTrequestFromForm();
        }
        
        /// <summary>
        ///     Adds the answers of a MC with 1 correct answer to a question.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="question">The question.</param>
        /// <returns>The question's weight</returns>
        protected int addAnswersMC(HttpSession httpSession, Question question)
        {
            Boolean multipleAnswers = question.getQuestionType() == QuestionType.MULTIPLE_CHOICE_SEVERAL;
            Answer[] answers = question.getAnswersAsArray();
            int totalweight = 0;
            int correctAnswer = 1;
            int maxweight = int.MinValue;
            for (int i = 0; i < answers.Length; i++)
            {
                /*
                 radio:
                  <input class="value" id="correct[3]" name="correct" value="3" type="radio" checked="checked" />
                  <textarea class="html_editor" name="value[3]"></textarea>
                  <textarea class="html_editor" name="feedback[3]"></textarea>
                  <input size="2" class="input_numeric" name="score[3]" type="text" value="1" />
                 
                 checkboxes:
                  <input class="value" id="correct[3]" name="correct[3]" type="checkbox" value="1" />
                  <textarea class="html_editor" name="value[3]"></textarea>
                  <textarea class="html_editor" name="feedback[3]"></textarea>
                  <input size="2" class="input_numeric" name="score[3]" type="text" value="1" />
                  */
                int weight = answers[i].getWeight();
                if (multipleAnswers)
                {
                    if (weight > 0)
                    {
                        httpSession.addNameValuePair("correct[" + i + "]", "1");
                        totalweight += weight;
                    }
                }
                httpSession.addNameValuePair("value[" + i + "]", answers[i].getAnswer());
                httpSession.addNameValuePair("feedback[" + i + "]", answers[i].getFeedback());
                httpSession.addNameValuePair("score[" + i + "]", weight.ToString());

                if (!multipleAnswers && weight > maxweight)
                {
                    maxweight = weight;
                    correctAnswer = i;
                }
            }
            if (!multipleAnswers)
            {
                httpSession.addNameValuePair("correct", correctAnswer.ToString());
                totalweight = (maxweight == int.MinValue ? 1 : maxweight);
            }
            httpSession.addNameValuePair("hint", "<br />");
            httpSession.addNameValuePair("mc_answer_type", (1 + Convert.ToInt32(multipleAnswers)).ToString()); // 1 for single, 2 for multi
            httpSession.addNameValuePair("mc_number_of_options", answers.Length.ToString());

            return totalweight;
        }

        /// <summary>
        ///     Adds gaps answers to a question.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="question">The question.</param>
        /// <returns>The question's weight</returns>
        protected virtual int addAnswersGaps(HttpSession httpSession, Question question)
        {
            const String gapsRegex = @"\[([^\[\]]+)\]";
            const String regexStartRegex = "{regex(=[0-9]+)?}";
            const String regexEnd = "{/regex}";
            const String regexContainsRegex = regexStartRegex + ".*?" + regexEnd;
            // $1: Answer text
            // $2: Size given by regex tag
            // $3: Regex text
            const String regexChamiloAnswer1 = @"^(\{regex(?:=([0-9]+))?\}(.+?)\{\/regex\}|(?!\{regex(?:=(?:[0-9]+))?\}).+?(?!\{\/regex\}))";
            // $4: Feedback
            // $5: score
            const String regexChamiloAnswerSuffix = @"(?:\((.+?)\))?(?:=([+-]?[0-9]+))?(?:\||$)";
            const String regexChamiloChoice = regexChamiloAnswer1 + regexChamiloAnswerSuffix;
            int defaultScore = DomainController.Instance().getSettings().getDefaultScoreGaps();

            // separator on platform
            const char separator = '|';
            int totalweight = 0;
            Answer[] answers = question.getAnswersAsArray();
            String answer = answers[0].getAnswer();
            Regex gaps = new Regex(gapsRegex);
            Regex regexContains = new Regex(regexContainsRegex);
            Regex regexSeparator = new Regex(@"\|");
            Regex regexContainsStart = new Regex(regexStartRegex);
            Regex regexContainsEnd = new Regex(regexEnd);
            Match matcher = gaps.Match(answer);
            String repl;
            int indexcorrection = 0;
            while (matcher.Success)
            {
                //
                // CODE TO HANDLE ONE SINGLE GAP IN THE ANSWER TEXT
                //

                // Types: (besides normal [ch|oi|ce])
                // 1. [feedback||ch|oi|ce] (careful for regex case [feedback{regex}...{/regex}])
                // 2. [ch|oi|ce +2]
                // 3. [ch|oi|ce||+2]
                // 4. [choice||+2] -> CAREFUL that this isn't seen as feedback! Filter out FIRST!
                // 5. [feedback||ch|oi|ce||+2]
                // 6. [feedback||ch|oi|ce +2]
                // 7. [choice(feedback)=2|choice(feedback)=2|choice(feedback)=2]
                Boolean oldformat = false;
                String answerText = matcher.Groups[1].Value;
                String oldFeedback=String.Empty;
                int oldWeight=int.MinValue;
                answerText = filterWeightFromGapsTag(answerText, ref oldWeight);
                if (oldWeight != int.MinValue)
                    oldformat = true;

                int regContainsFull = regexContains.Matches(answerText).Count;
                Boolean containsValidRegex = regContainsFull > 0;
                if (containsValidRegex)
                    containsValidRegex = regContainsFull == regexContainsStart.Matches(answerText).Count
                                        && regContainsFull == regexContainsEnd.Matches(answerText).Count;


                int feedbackIndex = answerText.IndexOf(separator + String.Empty + separator);
                if (feedbackIndex > 0 && feedbackIndex == answerText.IndexOf(separator))
                {
                    oldformat = true;
                    oldFeedback = answerText.Substring(0, feedbackIndex);
                    answerText = answerText.Substring(feedbackIndex+2);
                }
                
                List<String> gappossibilities = new List<String>();
                if (!answerText.Contains(separator))
                    gappossibilities.Add(answerText);
                else
                {
                    String[] splits = answerText.Split(new Char[] { separator }, StringSplitOptions.None);
                    // Split into valid choices
                    if (!containsValidRegex)
                        gappossibilities.AddRange(splits);
                    else
                    {
                        String currentString = null;
                        foreach (String split in splits)
                        {
                            if (currentString != null)
                            {
                                currentString += '|' + split;
                                if (split.Contains(regexEnd))
                                {
                                    gappossibilities.Add(currentString);
                                    currentString = null;
                                }
                            }
                            else
                            {
                                Match containsStartMatch = regexContainsStart.Match(split);
                                if (!regexContains.IsMatch(split) && containsStartMatch.Success)
                                {
                                    if (containsStartMatch.Index > 0)
                                    {
                                        oldFeedback = split.Substring(0, containsStartMatch.Index);
                                        oldformat = true;
                                        currentString = split.Substring(containsStartMatch.Index);
                                    }
                                    else
                                        currentString = split;
                                }
                                else
                                    gappossibilities.Add(split);
                            }
                        }
                    }
                }

                int currentWeight = defaultScore;
                if (oldformat)
                {
                    repl = String.Empty;
                    if (oldWeight != int.MinValue)
                        currentWeight = oldWeight;
                    foreach (String gappossib in gappossibilities)
                    {
                        repl += gappossib;
                        if (!oldFeedback.Equals(String.Empty))
                            repl += "(" + oldFeedback + ")";
                        if (oldWeight != int.MinValue && oldWeight != defaultScore)
                            repl += "=" + oldWeight;
                        repl += separator;
                    }
                    if (repl.Length > 0)
                        repl = repl.Substring(0,repl.Length-1);
                }
                else 
                {
                    // only used to get maximum score; nothing gets replaced since format is already OK
                    repl = null;
                    foreach (String gappossib in gappossibilities)
                    {
                        int score = defaultScore;
                        Match isMatch = new Regex(regexChamiloChoice).Match(gappossib);
                        if (isMatch.Success && !isMatch.Groups[5].Value.Equals(String.Empty))
                        {
                            try   { score = Int32.Parse(isMatch.Groups[5].Value); }
                            catch { score = defaultScore; }
                        }
                        currentWeight = Math.Max(currentWeight, score);
                    }
                }
                totalweight += currentWeight;
                if (repl!=null)
                {
                    answer = answer.Substring(0, matcher.Groups[1].Index + indexcorrection) + repl + answer.Substring(matcher.Groups[1].Index + matcher.Groups[1].Length + indexcorrection);
                    indexcorrection += repl.Length - matcher.Groups[1].Length;
                }
                matcher = matcher.NextMatch();
            }
            String questionType;
            if (question.getQuestionType() == QuestionType.FILL_IN_THE_GAPS_DROPDOWN)
                questionType = "2";
            else
                questionType = "0";

            httpSession.addNameValuePair("question_type", questionType); // gaps; not dropdown
            httpSession.addNameValuePair("default_positive_score", defaultScore.ToString());
            httpSession.addNameValuePair("default_negative_score", "0");
            httpSession.addNameValuePair("field_option_variation", "3"); // randomly add between 0 and this number to each gap's size
            httpSession.addNameValuePair("uniform_input_type", "0"); // use fixed size fields (off)
            httpSession.addNameValuePair("field_option_size", "15"); // not used since previous option is off
            httpSession.addNameValuePair("case_sensitive", "1"); 
            httpSession.addNameValuePair("show_inline", "1"); // show gap boxes inline
            httpSession.addNameValuePair("answer_text", answer);

            // "answer" should be deleted. I left it for compatibility reasons though;
            // sending the question text twice beats getting corrupted platform objects.
            httpSession.addNameValuePair("answer", answer);
            return totalweight;
        }

        /// <summary>
        ///     Filters the score out of the internal text from a gaps tag, and returns the trimmed string.
        ///     The score is added to the provided integers list.
        /// </summary>
        /// <param name="gapsTag">The string inside a gaps exercise tag</param>
        /// <param name="answerWeights">The list to add the weight to.</param>
        /// <returns>The input string, with the weight trimmed off</returns>
        private String filterWeightFromGapsTag(String gapsTag, ref int answerWeight)
        {
            String weightRegex = @"(?: |\|\|)([+-][0-9]+)$";
            Regex weight = new Regex(weightRegex);
            Match match = weight.Match(gapsTag);
            if (match.Success)
            {
                String score = match.Groups[1].Value;
                try
                {
                    answerWeight = Convert.ToInt32(score);
                    gapsTag = gapsTag.Substring(0, match.Index);
                }
                catch (FormatException e1) { }
                catch (OverflowException e2) { }
            }
            return gapsTag;
        }

        /// <summary>
        ///     Adds the matching answers to a question.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="question">The question.</param>
        /// <returns>The question's weight</returns>
        protected int addAnswersMatching(HttpSession httpSession, Question question)
        {
            // list of match "questions" - the statements to be matched with something
            List<Answer> matchOptions = new List<Answer>(question.getAnswersAsList());
            // list of all possible match "answers" - the possible statements to match with
            List<String> matchAnswers = new List<String>();
            int totalweight = 0;

            // cut off at answer 'Z'
            if (matchOptions.Count > 26)
                matchOptions.RemoveRange(26, matchOptions.Count - 26);

            Answer[] forlooplist = new Answer[Math.Min(matchOptions.Count, 26)];
            matchOptions.CopyTo(0, forlooplist, 0, Math.Min(matchOptions.Count, 26));

            // randomize match "answers" list, and remove matches without linked answer from match "questions" list.
            Random rand = new Random();
            foreach (Answer answ in forlooplist)
            {
                if (!matchAnswers.Contains(answ.getMatchAnswer()))
                {
                    // upper boundary of rand.Next is exclusive, so maximum for (0,Count) would be Count-1.
                    // upper boundary needs to be Count + 1 to allow any answer to be inserted AFTER the last index,
                    // otherwise the first processed string is always last in the randomized list.
                    matchAnswers.Insert(rand.Next(0, matchAnswers.Count + 1), answ.getMatchAnswer());
                }
                if (answ.getAnswer() == null || answ.getAnswer().Equals(String.Empty))
                    matchOptions.Remove(answ);
            }

            // Options ("Questions")
            // <textarea class="html_editor" name="value[1]">   </textarea>
            // <select name="matches_to[1]"><option value="0">A</option><option value="1">B</option><option value="2">C</option></select>
            // <textarea class="html_editor" name="feedback[1]"></textarea>
            // <input size="2" class="input_numeric" name="score[1]" type="text" value="1" />

            // Matches
            // <textarea class="html_editor" name="match[0]"></textarea>

            // Add name-value pairs for match Options to session
            for (int i = 0; i < matchOptions.Count; i++)
            {
                httpSession.addNameValuePair("value[" + i + "]", Utility.stripHTMLParagraphs(matchOptions[i].getAnswer()));

                int matchResult = 0;
                for (int match = 0; match < matchAnswers.Count; match++)
                {
                    if (matchOptions[i].getMatchAnswer().Equals(matchAnswers[match]))
                    {
                        matchResult = match;
                        break;
                    }
                }
                httpSession.addNameValuePair("matches_to[" + i + "]", matchResult.ToString());
                httpSession.addNameValuePair("feedback[" + i + "]", matchOptions[i].getFeedback().Equals(String.Empty)? "<br />" : matchOptions[i].getFeedback());
                httpSession.addNameValuePair("score[" + i + "]", matchOptions[i].getWeight().ToString());
                totalweight += matchOptions[i].getWeight();
            }

            // Add name-value pairs for match "answers" to session
            for (int i = 0; i < matchAnswers.Count; i++)
            {
                httpSession.addNameValuePair("match_label[" + i + "]", Convert.ToChar(Convert.ToInt32('A') + i).ToString());
                httpSession.addNameValuePair("match[" + i + "]", Utility.stripHTMLParagraphs(matchAnswers[i]));
            }
            httpSession.addNameValuePair("mq_number_of_options", matchOptions.Count.ToString());
            httpSession.addNameValuePair("mq_number_of_matches", matchAnswers.Count.ToString());

            return totalweight;
        }

        /// <summary>
        ///     Adds the options of an open question.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="question">The question.</param>
        /// <returns>The question's weight</returns>
        protected int addOpenQuestion(HttpSession httpSession, Question question)
        {
            httpSession.addNameValuePair("hint", "<br />");
            httpSession.addNameValuePair("feedback", "<br />");
            httpSession.addNameValuePair("question_type", "1");
            return question.getAnswersAsArray()[0].getWeight();
        }

        /// <summary>
        ///     Sets the repository view to a certain type for this session.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectType">The object type to filter on</param>
        protected void setRepositoryViewObjectType(HttpSession httpSession, String objectType, Boolean listAll)
        {
            setRepositoryViewObjectType(httpSession, null, objectType, listAll);
        }

        /// <summary>
        ///     Sets the repository view to a certain type for this session, and returns the page of a requested category
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="platformDestinationId">Category to fetch list page for. Use Null for none.</param>
        /// <param name="objectType">The object type to filter on</param>
        /// <returns>The resulting page</returns>
        protected String setRepositoryViewObjectType(HttpSession httpSession, String platformDestinationId, String objectType, Boolean listAll)
        {
            String category;
            if (platformDestinationId == null)
                category = String.Empty;
            else
                category = platformDestinationId;
            
            String chamiloUrl = this.login.getPlatformUrl();
            String repo = "/index.php?renderer=table&application=repository&go=browser&category=" + platformDestinationId + "&go=browser";
            if(listAll)
                repo+="&repository_browser_table_per_page=all";
            else
                repo += "&repository_browser_table_per_page=20";
            // if fetching a category list, disable page view.
            if (platformDestinationId != null)
                repo += "all";
            // else minimize page size to fetch
            else
                repo += "1";

            httpSession.setRequestUrl(chamiloUrl + repo);

            httpSession.addNameValuePair("filter_type", objectType);
            httpSession.addNameValuePair("_qf__repository_filter_form", "");
            httpSession.sendPOSTrequestFromForm();

            return httpSession.getResponseFromServer();
        }

        /// <summary>
        ///     Retrieves the category code of an object ID.
        /// </summary>
        /// <param name="httpSession">The working session</param>
        /// <param name="objectId">The object ID</param>
        /// <returns></returns>
        protected String getCategoryFromObjectID(HttpSession httpSession, String objectId)
        {
            return getCategoryFromObjectId(httpSession, objectId, null);
        }

        /// <summary>
        ///     Retrieves the category code of an object ID.
        /// </summary>
        /// <param name="httpSession">The working session</param>
        /// <param name="objectId">The object ID</param>
        /// <returns></returns>
        protected String getCategoryFromObjectId(HttpSession httpSession, String objectId, String linkedId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String editView = "/index.php?renderer=table&application=repository&object=" + objectId;
            if (linkedId == null)
                editView += "&go=editor";
            else
                editView += "&go=builder&builder_action=updater&selected_cloi=" + linkedId;
            httpSession.setRequestUrl(chamiloUrl + editView);
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();

            SingleNodeScanner scanner = new SingleNodeScanner("select");
            scanner.addNameValuePair("name", "parent_id");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode selectNode = scanner.getNode();
            if (selectNode != null)
            {
                scanner = new SingleNodeScanner("option");
                scanner.addNameValuePair("selected", "selected");
                scanner.doScan(selectNode.ChildNodes);
                XmlNode optionNode = scanner.getNode();
                if (optionNode.Attributes["value"] != null)
                {
                    return optionNode.Attributes["value"].Value;
                }
            }
            return null;
        }

        /// <summary>
        ///     Returns a ID from a platform object. If the object doesn't exists, it'll return an empty string.
        ///     This function does a request for every repository category until an object with the name is found.
        ///     If possible, avoid this and use getIdFromObjectInCategory(...) instead.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectname">The name of the object.</param>
        /// <param name="platformDestinationId">The category in which the object should be located</param>
        /// <param name="objectType">The object type to look for</param>
        /// <returns>The repository object ID.</returns>
        protected String getIdFromObject(HttpSession httpSession, String objectname, String objectType)
        {
            String id = String.Empty;
            foreach (String[] repofolder in repositoryFolders)
            {
                id = getIdFromObjectInCategory(httpSession, objectname, repofolder[1], objectType);
                if (!id.Equals(String.Empty))
                    break;
            }
            return id;
        }

        /// <summary>
        ///     Returns a ID from an object in a specified category. If the object doesn't exists, it'll return an empty string.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectname">The name of the object.</param>
        /// <param name="platformDestinationId">The category in which the object should be located</param>
        /// <returns>The file ID.</returns>
        protected String getIdFromObjectInCategory(HttpSession httpSession, String objectname, String platformDestinationId, String objectType)
        {
            String response = setRepositoryViewObjectType(httpSession, platformDestinationId, objectType,true);

            SingleNodeContentScanner scanner = new SingleNodeContentScanner("a", false, objectname, TRUNCATE_STRING);
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode node = scanner.getNode();
            List<XmlNode> nodes = scanner.getNodes();

            if (node != null && node.InnerText.ToLower().Equals(objectname.ToLower()) && node.Attributes["href"] != null)
                return Utility.findValueInURL(node.Attributes["href"].Value, "object");
            else if (nodes.Count > 0)
            {
                foreach (XmlNode n in nodes)
                {
                    String hrefValue = null;
                    if (n.Attributes["href"] != null)
                        hrefValue = n.Attributes["href"].Value;
                    if (hrefValue != null && Utility.findValueInURL(hrefValue, "go").Equals("viewer")
                                          && Utility.findValueInURL(hrefValue, "application").Equals("repository"))
                    {
                        String name = n.InnerText;
                        if (name.Equals(objectname) || Utility.isTruncatedVersionOf(name, objectname, TRUNCATE_STRING))
                        {
                            String id = Utility.findValueInURL(hrefValue, "object");
                            String chamiloUrl = this.login.getPlatformUrl();
                            String objectview = "/index.php?application=repository&go=viewer&object=" + id;
                            httpSession.setRequestUrl(chamiloUrl + objectview);
                            httpSession.sendPOSTrequestSimple();
                            response = httpSession.getResponseFromServer();

                            SingleNodeScanner sc = new SingleNodeScanner("div");
                            sc.addNameValuePair("class", "title");
                            sc.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
                            List<XmlNode> divnodes = sc.getNodes();
                            foreach (XmlNode divnode in divnodes)
                                if (divnode.InnerText.ToLower().Equals(objectname.ToLower()))
                                    return id;
                        }
                    }
                }
            }
            return String.Empty;
        }

        /// <summary>
        ///     Returns a ID from a file. If the file doesn't exists, it'll return an empty string.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectname">The name of the object.</param>
        /// <param name="platformDestinationId">The category in which the object should be located</param>
        /// <param name="objectType">The object type to look for</param>
        /// <returns>The repository object ID.</returns>
        protected List<String[]> getAllObjects(HttpSession httpSession, String objectType)
        {
            List<String[]> objects = new List<String[]>();
            foreach (String[] repofolder in repositoryFolders)
            {
                objects.AddRange(getAllObjectsInCategory(httpSession, repofolder[1], objectType));
            }
            return objects;
        }

        /// <summary>
        ///     Returns a list of all objects in the repository, as a 2-element string array of [ID, name].
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectname">The name of the object.</param>
        /// <param name="platformDestinationId">The category in which the object should be located</param>
        /// <returns>The file ID.</returns>
        protected List<String[]> getAllObjectsInCategory(HttpSession httpSession, String platformDestinationId, String objectType)
        {
            String response = setRepositoryViewObjectType(httpSession, platformDestinationId, objectType, true);
            List<String[]> objects = new List<String[]>();
            // special case for files: cut off path

            SingleNodeScanner scanner = new SingleNodeScanner("table");
            scanner.addNameValuePair("class","data_table");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode tablenode = scanner.getNode();

            SingleNodeContentScanner scanner2 = new SingleNodeContentScanner("td", true, "go=viewer", true);
            scanner2.doScan(tablenode.ChildNodes);
            List<XmlNode> tdnodes = scanner2.getNodes();

            XmlNode viewnode = null;
            foreach (XmlNode node in tdnodes)
            {
                try
                {
                    viewnode = node.SelectSingleNode("a");
                    if (viewnode != null && viewnode.Attributes["href"] != null)
                    {
                        objects.Add(
                            new String[] {
                                Utility.findValueInURL(viewnode.Attributes["href"].Value, "object"),
                                viewnode.InnerText});
                    }
                } catch{ }
            }
            return objects;
        }


        /// <summary>
        ///     Returns a list of all linked objects in an exercise, as a 3-element string array of [realID, linkID, name].
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="exerciseId">The name of the exercise.</param>
        /// <returns>list of all linked objects in an exercise, as a 3-element string array of [realID, linkID, name].</returns>
        //protected List<String[]> getQuestionIdsFromExercise(HttpSession httpSession, String exerciseId)
        //{
        //    List<String[]> objects = new List<String[]>();
        //    String chamiloUrl = this.login.getPlatformUrl();
            
        //    String exerciseContents = "/index.php?application=repository&go=viewer&link_browser_table_3_per_page=all&object=" + exerciseId;
        //    exerciseContents =
        //        "/index.php?renderer=table&tab=category&application=repository&object=" + exerciseId + "&go=builder";

        //    httpSession.setRequestUrl(chamiloUrl + exerciseContents);
        //    httpSession.sendPOSTrequestSimple();
        //    String response = httpSession.getResponseFromServer();
        //    XmlNodeList resp = SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes;
        //    SingleNodeScanner table3scanner = new SingleNodeScanner("table", false, false);
        //    table3scanner.addNameValuePair("class","data_table");
        //    table3scanner.addNameValuePair("id", "complex_browser_table");
        //    table3scanner.doScan(resp);
        //    XmlNode tableNode = table3scanner.getNode();
        //    XmlNodeList trnodes=null;
        //    try
        //    {
        //        trnodes= tableNode.SelectSingleNode("tbody").SelectNodes("tr");
        //    }
        //    catch{}

        //    if (trnodes != null)
        //    {
        //        SingleNodeScanner scanner;
        //        foreach (XmlNode trnode in trnodes)
        //        {
        //            if (trnode != null && trnode.HasChildNodes)
        //            {
        //                scanner = new SingleNodeScanner("a", true, false);
        //                scanner.addNameValuePair("href", null);
        //                scanner.doScan(trnode.ChildNodes);
        //                String name = null;
        //                String realId = null;
        //                String linkId = null;
        //                List<XmlNode> anodes = scanner.getNodes();
        //                foreach (XmlNode anode in anodes)
        //                {
        //                    String href = anode.Attributes["href"].Value;
        //                    if (href != null)
        //                    {
        //                        String id = Utility.findValueInURL(href, "object");
        //                        if (id != exerciseId && href.Contains("go=viewer"))
        //                        {
        //                            if (realId == null && name == null)
        //                            {
        //                                realId = id;
        //                                name = anode.InnerText;
        //                            }
        //                        }
        //                        else if (href.Contains("go=link_deleter"))
        //                        {
        //                            if (linkId == null)
        //                            {
        //                                linkId = Utility.findValueInURL(href, "link_id");
        //                                if (linkId.Equals(String.Empty))
        //                                    linkId = null;
        //                            }
        //                        }
        //                    }
        //                    if (realId != null && name != null && linkId != null)
        //                    {
        //                        objects.Add(new String[] { realId, linkId, name });
        //                        break;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return objects;
        //}
        protected List<String[]> getQuestionIdsFromExercise(HttpSession httpSession, String exerciseId)
        {
            List<String[]> objects = new List<String[]>();
            String chamiloUrl = this.login.getPlatformUrl();
            String exerciseContents = "/index.php?application=repository&go=viewer&link_browser_table_3_per_page=all&object=" + exerciseId;

            httpSession.setRequestUrl(chamiloUrl + exerciseContents);
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();
            XmlNodeList resp = SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes;
            SingleNodeScanner table3scanner = new SingleNodeScanner("table", false, false);
            table3scanner.addNameValuePair("class", "data_table");
            table3scanner.addNameValuePair("id", "link_browser_table_3");
            table3scanner.doScan(resp);
            XmlNode tableNode = table3scanner.getNode();
            XmlNodeList trnodes = null;
            try
            {
                trnodes = tableNode.SelectSingleNode("tbody").SelectNodes("tr");
            }
            catch { }

            if (trnodes != null)
            {
                SingleNodeScanner scanner;
                foreach (XmlNode trnode in trnodes)
                {
                    if (trnode != null && trnode.HasChildNodes)
                    {
                        scanner = new SingleNodeScanner("a", true, false);
                        scanner.addNameValuePair("href", null);
                        scanner.doScan(trnode.ChildNodes);
                        String name = null;
                        String realId = null;
                        String linkId = null;
                        List<XmlNode> anodes = scanner.getNodes();
                        foreach (XmlNode anode in anodes)
                        {
                            String href = anode.Attributes["href"].Value;
                            if (href != null)
                            {
                                String id = Utility.findValueInURL(href, "object");
                                if (id != exerciseId && href.Contains("go=viewer"))
                                {
                                    if (realId == null && name == null)
                                    {
                                        realId = id;
                                        name = anode.InnerText;
                                    }
                                }
                                else if (href.Contains("go=link_deleter"))
                                {
                                    if (linkId == null)
                                    {
                                        linkId = Utility.findValueInURL(href, "link_id");
                                        if (linkId.Equals(String.Empty))
                                            linkId = null;
                                    }
                                }
                            }
                            if (realId != null && name != null && linkId != null)
                            {
                                objects.Add(new String[] { realId, linkId, name });
                                break;
                            }
                        }
                    }
                }
            }
            return objects;
        }
        /// <summary>
        ///     Returns a list of all linked objects in an exercise, as a 3-element string array of [realID, linkID, name].
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="exerciseId">The name of the exercise.</param>
        /// <returns>list of all linked objects in an exercise, as a 3-element string array of [realID, linkID, name].</returns>
        protected List<String[]> getQuestionIdsFromExercise_old(HttpSession httpSession, String exerciseId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String exerciseView = "/index.php?renderer=table&application=repository&go=builder&object=" + exerciseId
                    + "&complex_browser_table_per_page=all";
            httpSession.setRequestUrl(chamiloUrl + exerciseView);
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();
            List<String[]> linkobjects = new List<String[]>();
            List<String[]> objects = new List<String[]>();

            SingleNodeScanner scanner = new SingleNodeScanner("table");
            scanner.addNameValuePair("class", "data_table");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode tablenode = scanner.getNode();
            if (tablenode == null)
                return objects;

            SingleNodeContentScanner scanner2 = new SingleNodeContentScanner("td", true, "builder_action=viewer", true);
            scanner2.doScan(tablenode.ChildNodes);
            List<XmlNode> tdnodes = scanner2.getNodes();

            XmlNode viewnode = null;
            foreach (XmlNode node in tdnodes)
            {
                try
                {
                    viewnode = node.SelectSingleNode("a");
                    if (viewnode != null && viewnode.Attributes["href"] != null)
                    {
                        linkobjects.Add(new String[]{
                            Utility.findValueInURL(viewnode.Attributes["href"].Value, "selected_cloi"),
                            viewnode.InnerText}
                            );
                    }
                }
                catch { }
            }
            // only execute if there ARE linked exercises
            if (linkobjects.Count > 0)
            {
                String exerciseContents = "/index.php?application=repository&go=viewer&link_browser_table_3_per_page=all&object=" + exerciseId;

                httpSession.setRequestUrl(chamiloUrl + exerciseContents);
                httpSession.sendPOSTrequestSimple();
                response = httpSession.getResponseFromServer();
                XmlNodeList resp = SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes;
                SingleNodeContentScanner cscanner;
                foreach (String[] link in linkobjects)
                {
                    cscanner = new SingleNodeContentScanner("tr", false, "link_id=" + link[0], ScanMode.CONTAINS);
                    cscanner.doScan(resp);
                    XmlNode trnode = cscanner.getNode();
                    if (trnode != null && trnode.HasChildNodes)
                    {
                        scanner = new SingleNodeScanner("a", true, false);
                        scanner.addNameValuePair("href", null);
                        scanner.doScan(trnode.ChildNodes);
                        List<XmlNode> anodes = scanner.getNodes();
                        foreach (XmlNode anode in anodes)
                        {
                            String href = anode.Attributes["href"].Value;
                            if (href != null && href.Contains("go=viewer"))
                            {
                                String realId = Utility.findValueInURL(href, "object");
                                if (realId != exerciseId)
                                {
                                    objects.Add(new String[] { realId, link[0], link[1] });
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            return objects;
        }

        /// <summary>
        ///     Deletes a publication of an object to the Courses application.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectId">The object ID.</param>
        /// <param name="publicationId">The publication ID.</param>
        protected void deletePublication(HttpSession httpSession, String objectId, String publicationId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            // adjust this address
            String publicationRemoveUrl = "/index.php?renderer=table&application=repository&object=" + objectId
                                        + "&go=link_deleter&link_type=1&link_id=weblcms|" + publicationId;
            httpSession.setRequestUrl(chamiloUrl + publicationRemoveUrl);
            httpSession.sendPOSTrequestSimple();
        }

        protected void unlinkComplexObject(HttpSession httpSession, String exerciseId, String linkId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String unlink = "/index.php?renderer=table&application=repository"
                            + "&go=link_deleter&link_type=3" // I think "3" it means "link_id is child"
                            + "&object=" + exerciseId
                            + "&link_id=" + linkId;

            httpSession.setRequestUrl(chamiloUrl + unlink);
            httpSession.sendPOSTrequestSimple();
        }

        
        //hagafluub // try permanent_deleter
        protected void unlinkObjects(HttpSession httpSession, String[] objectIDs, String linkId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String unlink = "/index.php?renderer=table&application=repository&go=browser";
            httpSession.setRequestUrl(chamiloUrl + unlink);
            foreach (String id in objectIDs)
                httpSession.addNameValuePair("repository_browser_table_id[]", id);
            httpSession.addNameValuePair("repository_browser_table_action_value", "recycler");

            httpSession.sendPOSTrequestFromForm();
        }


        

        /// <summary>
        ///     Unlinks a file.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectID">The file ID.</param>
        protected void unlinkObject(HttpSession httpSession, String objectId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String unlink = "/index.php?renderer=table&go=unlinker"
                                + "&application=repository"
                                + "&object=" + objectId;

            httpSession.setRequestUrl(chamiloUrl + unlink);
            httpSession.sendPOSTrequestSimple();
        }

        /// <summary>
        ///     Unlinks and deletes a file (if present).
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="uploadFilePath">The name of the file</param>
        /// <param name="platformDestinationId">the destination ID.</param>
        protected void deleteObjectIfExists(HttpSession httpSession, String uploadFilePath, String platformDestinationId)
        {
            // special case for files: cut off path
            String searchString = uploadFilePath;
            if (uploadFilePath.Contains(Path.DirectorySeparatorChar.ToString()))
                try { searchString = new FileInfo(uploadFilePath).Name; }
                catch { }

            String objectId = getIdFromObjectInCategory(httpSession, searchString, platformDestinationId, OBJID_FILE);
            deleteObjectFromId(httpSession, objectId);
        }

        /// <summary>
        ///     Unlinks and deletes a file (if present).
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="uploadFilePath">The name of the file (inc. path).</param>
        /// <param name="platformDestinationId">the destination ID.</param>
        protected void deleteObjectFromId(HttpSession httpSession, String objectId)
        {
            if (objectId != null && !objectId.Equals(String.Empty))
            {
                //unlink it...
                this.unlinkObject(httpSession, objectId);
                // delete it
                this.deleteObjectIfNotUsed(httpSession, objectId);
            }
        }

        /// <summary>
        ///     Deletes an object, if it's not linked to anything.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="uploadFilePath">The name of the file (inc. path).</param>
        /// <param name="platformDestinationId">the destination ID.</param>
        protected void deleteObjectIfNotUsed(HttpSession httpSession, String objectId)
        {
            if (objectId != null && !objectId.Equals(String.Empty))
            {
                String chamiloUrl = this.login.getPlatformUrl();

                String delete = "/index.php?go=deleter"
                                + "&application=repository"
                                + "&object=" + objectId
                                + "&delete_permanently=1";

                httpSession.setRequestUrl(chamiloUrl + delete);
                httpSession.sendPOSTrequestSimple();
            }
        }

        /// <summary>
        ///     Deletes a series of objects with one request. Note that this uses the
        ///     rather dirty hack of calling an option normally only available in the Recycler.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectIDs">List of object IDs to delete</param>
        protected void deleteObjectsIfNotUsed(HttpSession httpSession, String[] objectIDs)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String unlink= "/index.php?renderer=table&application=repository&go=browser";
            httpSession.setRequestUrl(chamiloUrl + unlink);
            foreach (String id in objectIDs)
                httpSession.addNameValuePair("repository_browser_table_id[]", id, true);

            httpSession.addNameValuePair("repository_browser_table_action_value", "permanent_deleter");

            httpSession.addNameValuePair("repository_browser_table_action_name", "go");
            httpSession.addNameValuePair("repository_browser_table_namespace","repository");
            httpSession.addNameValuePair("table_name","repository_browser_table");

            httpSession.sendPOSTrequestFromForm();

            String page = httpSession.getResponseFromServer();
        }

        /// <summary>
        ///     Deletes a series of objects with one request. Note that this uses the
        ///     rather dirty hack of calling an option normally only available in the Recycler.
        /// </summary>
        /// <param name="httpSession">The session. Login is assumed.</param>
        /// <param name="objectIDs">List of object IDs to delete</param>
        protected void deleteObjectsIfNotUsedRecycl(HttpSession httpSession, String[] objectIDs)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String unlink = "/index.php?renderer=table&application=repository&go=recycle_bin_browser";
            httpSession.setRequestUrl(chamiloUrl + unlink);
            foreach (String id in objectIDs)
                httpSession.addNameValuePair("recycle_bin_browser_table_id[]", id, true);

            httpSession.addNameValuePair("recycle_bin_browser_table_action_value", "permanent_deleter");

            httpSession.addNameValuePair("recycle_bin_browser_table_action_name", "go");
            httpSession.addNameValuePair("recycle_bin_browser_table_namespace", "repository");
            httpSession.addNameValuePair("table_name", "recycle_bin_browser_table");

            httpSession.sendPOSTrequestFromForm();

            String page = httpSession.getResponseFromServer();
        }


        /// <summary>
        ///     Returns the publications of an object in the format [linkId, courseId]
        /// </summary>
        /// <param name="httpSession">The working session</param>
        /// <param name="publishDestination">the folder in which the object should have been published</param>
        /// <param name="objectId">the object ID in the repository</param>
        /// <returns>the publications of an object in the format [linkId, courseId]</returns>
        protected List<String[]> getPublications(HttpSession httpSession, String objectId)
        {
            List<String[]> publications = new List<String[]>();
            String chamiloUrl = this.login.getPlatformUrl();
            String objectView = "/index.php?renderer=table&application=repository&go=viewer&object=" + objectId
                    + "&link_browser_table_1_per_page=all";
            httpSession.setRequestUrl(chamiloUrl + objectView);
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();
            List<String[]> objects = new List<String[]>();

            SingleNodeScanner scanner = new SingleNodeScanner("div");
            scanner.addNameValuePair("class", "admin_tab");
            scanner.addNameValuePair("id", "repository_manager_viewer_component_1"); // component 1 = publications
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode divnode = scanner.getNode();
            if (divnode == null)
                return publications;

            scanner = new SingleNodeScanner("table");
            scanner.addNameValuePair("class", "data_table");
            scanner.doScan(divnode.ChildNodes);
            XmlNode tablenode = scanner.getNode();
            if (tablenode == null)
                return publications;
            scanner = new SingleNodeScanner("tr", true, false);
            scanner.addNameValuePair("id","row_Weblcms");
            scanner.doScan(tablenode.ChildNodes);
            List<XmlNode> trnodes = scanner.getNodes();
            
            foreach (XmlNode trnode in trnodes)
            {
                try
                {
                    String coursename = trnode.SelectNodes("td")[1].InnerText.Replace(" &gt; ", " > ");
                    if (coursename.Contains(" > "))
                    {
                        coursename = coursename.Substring(0, coursename.IndexOf(" > ")).Trim();
                        String courseId = String.Empty;
                        foreach (Course course in this.courses)
                        {
                            if (!isRepository(course) && coursename.ToLower().Equals(course.getCourseName().ToLower()))
                            {
                                courseId = course.getCourseId();
                                break;
                            }
                        }
                        if (!courseId.Equals(String.Empty))
                        {
                            scanner = new SingleNodeScanner("a", true, true);
                            scanner.addNameValuePair("href", "go=link_deleter");
                            scanner.addNameValuePair("href", "link_id=weblcms");
                            scanner.addNameValuePair("href", "object=" + objectId);
                            scanner.doScan(trnode.ChildNodes);
                            XmlNode tdnode = scanner.getNode();
                            String linkId = Utility.findValueInURL(tdnode.Attributes["href"].Value, "link_id");
                            String[] link = linkId.Replace("%7C", "|").Split('|');
                            if (link[0].Equals("weblcms"))
                            {
                                publications.Add(new String[] { link[1], courseId });
                            }
                        }
                    }
                }
                catch { /* something went wrong; just ignore */}
            }
            return publications;
        }

        /// <summary>
        ///     Determines the folder in which to put an object in the repository,
        ///     and creates the necessary folders.
        /// </summary>
        /// <param name="publishDestination">The publish destination used to determine the platform destination.</param>
        /// <returns>The folder ID of the platform destination.</returns>
        protected String getPlatformDestinationId(HttpSession httpSession, DocumentFolder publishDestination, Subject subject, Boolean create)
        {
            String destination;

            if (publishDestination.folders.Count > 0)
            {
                if (subject != null)
                {
                    List<Course> courses = subject.getCoursesForLogin(this.login);
                    if (courses.Count == 1 && isRepository(courses[0]))
                        destination = String.Empty;
                    else
                        destination = "/" + this.getFormattedFilenameString(subject.getSubjectName());
                }
                else if (!isRepository(publishDestination.folders[0].course))
                    destination = "/" + this.getFormattedFilenameString(publishDestination.folders[0].course.getCourseName());
                else
                    destination = String.Empty;
            }
            else
                destination = String.Empty;

            return this.retrievePlatformDestination(httpSession, destination + publishDestination.folderName, create);
        }

        /// <summary>
        ///     Creates and returns a specifically-named category in the repository,
        ///     named after the course or subject it is for.
        /// </summary>
        /// <param name="httpSession">The working session</param>
        /// <param name="publishDestination">the folder in which the object should have been published</param>
        /// <param name="foldername">Name of the subfolder in this subject or course's folder</param>
        /// <param name="subject">Subject</param>
        /// <returns></returns>
        protected String getPlatformDestinationId(HttpSession httpSession, List<Course> publishDestinations, String foldername, Subject subject)
        {
            String destination;
            if (subject != null)
            {
                List<Course> courses = subject.getCoursesForLogin(this.login);
                if (courses.Count == 1 && isRepository(courses[0]))
                    destination = String.Empty;
                else
                    destination = "/" + this.getFormattedFilenameString(subject.getSubjectName());
            }
            else if (publishDestinations.Count == 1 && !isRepository(publishDestinations[0]))
                destination = "/" + this.getFormattedFilenameString(publishDestinations[0].getCourseName());
            else
                destination = String.Empty;

            return retrievePlatformDestination(httpSession, destination + "/" + foldername, true);
        }

        /// <summary>
        ///     This method will create the full path of the given string in the repository.
        /// </summary>
        /// <param name="platformDestination">The full path.</param>
        /// <returns>The ID of the created folder.</returns>
        protected String retrievePlatformDestination(HttpSession httpSession, String platformDestination, Boolean create)
        {
            String returnCode = this.getRepositoryFolderCode(platformDestination);
            if (returnCode == null)
            {
                //Destination not present.
                String[] tree = platformDestination.Split(new char[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
                if (tree.Length > 0)
                {
                    String build = String.Empty;
                    String[] lastReturn = new String[]{ "/", "0" }; //foldername, foldercode: start in root.
                    foreach (String item in tree)
                    {
                        build += "/" + item;
                        returnCode = this.getRepositoryFolderCode(build);
                        if (returnCode == null)
                        {
                            lastReturn = this.createFolderInRepository(httpSession, item, lastReturn[0], lastReturn[1]);
                            returnCode = (lastReturn != null? lastReturn[1] : null);
                        }
                        else
                        {
                            lastReturn = new String[] { build, returnCode };
                        }
                    }
                }
            }
            return returnCode;
        }

        /// <summary>
        ///     Returns the repository code of a folder name.
        /// </summary>
        /// <param name="folderName">The folder name.</param>
        /// <returns>The folder code.</returns>
        protected String getRepositoryFolderCode(String folderName)
        {
            folderName = "/" + this.getFormattedFilenameString(folderName);
            var subfolder = folderName.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);

            foreach(String[] repofolder in this.repositoryFolders)
            //    if (repofolder[0].ToLower().Equals(folderName.ToLower()))
                if (repofolder[0].ToLower().Equals("/" + subfolder[0].ToLower()))
                    return repofolder[1];

            return null;
        }

        /// <summary>
        ///     This method will publish an already uploaded file into a courses.
        /// </summary>
        /// <param name="httpSession">The session.</param>
        /// <param name="fileId">The file ID.</param>
        /// <param name="course">The course.</param>
        /// <param name="folderCode">The folder code.</param>
        /// <param name="isVisible">If true the published file will be visible.</param>
        protected void publishFileToCourse(HttpSession httpSession, String fileId, Course course, String folderCode, Boolean isVisible)
        {
            if (isRepository(course))
                return;
            String chamiloUrl = this.login.getPlatformUrl();
            String publishUrl = "/index.php?&browser=table&application=weblcms&go=course_viewer&course=" + course.getCourseId()
                              + "&tool=document&tool_action=publisher&repoviewer_action=publisher&publication_category=" + folderCode
                              + "&repo_object=" + fileId;
            String response;

            httpSession.setRequestUrl(chamiloUrl + publishUrl);

            /*
                _qf__content_object_publication_form=""
                category_id="0"
                forever="1"
                from_date[F]="6"
                from_date[H]="9"
                from_date[Y]="2011"
                from_date[d]="9"
                from_date[i]="50"
                inherit="0"
                right_option="0"
                submit=Publish"
                targets_active_hidden=""
                targets_element_types="-1"
                targets_search=""
                to_date[F]="6"
                to_date[H]="9"
                to_date[Y]="2011"
                to_date[d]="9"
                to_date[i]="50"
             */

            httpSession.addNameValuePair("inherit", "0"); // rights
            httpSession.addNameValuePair("category", folderCode);
            httpSession.addNameValuePair("forever", "1");
            if(!isVisible)
                httpSession.addNameValuePair("hidden", "1");
            httpSession.addNameValuePair("submit", "Publish");
            httpSession.addNameValuePair("_qf__content_object_publication_form", "");

            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
        }

        protected void updateFilePublication(HttpSession httpSession, DocumentFolderForCourse documentFolderForCourse, String publicationId, Boolean visible)
        {
            if (isRepository(documentFolderForCourse.course))
                return;
            String chamiloUrl = this.login.getPlatformUrl();
            //http://chamilo.hogent.be/index.php?application=weblcms&course=969&go=course_viewer&tool=document&tool_action=publication_updater&browser=table&publication=237887
            String publicationEditorUrl = "/index.php?application=weblcms&go=course_viewer&tool=document"
                                        + "&tool_action=publication_updater&pub_type=2&course=" + documentFolderForCourse.course.getCourseId()
                                        + "&browser=table&publication=" + publicationId;
            String response;
            httpSession.setRequestUrl(chamiloUrl + publicationEditorUrl);

            httpSession.addNameValuePair("category", documentFolderForCourse.folderCode);
            httpSession.addNameValuePair("forever", "1");
            httpSession.addNameValuePair("inherit", "0"); // rights
            if (!visible)
                httpSession.addNameValuePair("hidden", "1");
            httpSession.addNameValuePair("submit", "Publish");
            httpSession.addNameValuePair("publication", publicationId);
            httpSession.addNameValuePair("action", "edit");
            httpSession.addNameValuePair("_qf__content_object_publication_form", "");

            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
        }

        protected void updatePublications(HttpSession httpSession, List<Course> publishDestinations, String objectId, Boolean isNew, Boolean visible, String objectType)
        {
            List<DocumentFolderForCourse> ldffc = new List<DocumentFolderForCourse>();
            foreach (Course course in publishDestinations)
                if (!isRepository(course))
                    ldffc.Add(new DocumentFolderForCourse(course,"0"));
            DocumentFolder df = new DocumentFolder(ldffc,"/");
            this.updatePublications(httpSession, df, objectId, isNew, visible, objectType);
        }

        /// <summary>
        ///     Updates publications of an object, and adds new ones if needed.
        /// </summary>
        /// <param name="httpSession"></param>
        /// <param name="publishDestinations"></param>
        /// <param name="objectId"></param>
        /// <param name="isNew"></param>
        /// <param name="visible"></param>
        /// <param name="objectType"></param>
        protected void updatePublications(HttpSession httpSession, DocumentFolder publishDestinations, String objectId, Boolean isNew, Boolean visible, String objectType)
        {
            // get all publications
            List<String[]> allPublications;
            if (!isNew)
                allPublications = this.getPublications(httpSession, objectId);
            else
                allPublications = new List<String[]>();

            // filter out relevant publications
            // delete others?
            List<String[]> relevantPublications = new List<String[]>();
            List<String[]> otherPublications = new List<String[]>(allPublications);

            foreach (String[] publication in allPublications)
            {
                foreach (DocumentFolderForCourse dffc in publishDestinations.folders)
                {
                    if (!isRepository(dffc.course) && dffc.course.getCourseId().Equals(publication[1]))
                    {
                        relevantPublications.Add(publication);
                        otherPublications.Remove(publication);
                    }
                }
            }

            // update publications, add new ones where necessary.
            foreach (DocumentFolderForCourse dffc in publishDestinations.folders)
            {
                if (!isRepository(dffc.course))
                {
                    Boolean contains = false;
                    foreach (String[] publication in relevantPublications)
                    {
                        if (dffc.course.getCourseId().Equals(publication[1]))
                        {
                            //[linkId, courseId]
                            if (objectType.Equals(OBJID_EXERCISE))
                                this.updateExercisePublication(httpSession, dffc.course, publication[0], visible);
                            else if (objectType.Equals(OBJID_FILE))
                                this.updateFilePublication(httpSession, dffc, publication[0], visible);
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        if (objectType.Equals(OBJID_EXERCISE))
                            this.publishExerciseToCourse(httpSession, objectId, dffc.course, visible);
                        else if (objectType.Equals(OBJID_FILE))
                            this.publishFileToCourse(httpSession, objectId, dffc.course, dffc.folderCode, visible);
                    }
                }
            }

            // remove publications in other courses
            foreach (String[] publication in otherPublications)
            {
                //publication: [linkId, courseId]
                this.deletePublication(httpSession, objectId, publication[0]);
            }

        }

        protected void updateExercisePublication(HttpSession httpSession, Course course, String publicationId, Boolean visible)
        {
            if (isRepository(course))
                return;
            String chamiloUrl = this.login.getPlatformUrl();
            //http://chamilo.hogent.be/index.php?application=weblcms&course=969&go=course_viewer&tool=document&tool_action=publication_updater&browser=table&publication=237887
            String publicationEditorUrl = "/index.php?application=weblcms&go=course_viewer&tool=assessment"
                                        + "&tool_action=publication_updater&pub_type=2&course=" + course.getCourseId()
                                        + "&browser=table&pub_type=2&publication=" + publicationId;
            String response;
            httpSession.setRequestUrl(chamiloUrl + publicationEditorUrl);
            httpSession.addNameValuePair("category", "0");
            httpSession.addNameValuePair("forever", "1");
            httpSession.addNameValuePair("inherit", "0"); // rights
            if (!visible)
                httpSession.addNameValuePair("hidden", "1");
            httpSession.addNameValuePair("submit", "Publish");
            httpSession.addNameValuePair("publication", publicationId);
            httpSession.addNameValuePair("action", "edit");
            httpSession.addNameValuePair("_qf__content_object_publication_form", "");

            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
        }

        /// <summary>
        ///     This method will publish an already uploaded exercise into a courses.
        /// </summary>
        /// <param name="httpSession">The session.</param>
        /// <param name="assessmentId">The ID of the exercise object.</param>
        /// <param name="course">The course.</param>
        /// <param name="isVisible">If true the published exercise will be visible.</param>
        protected void publishExerciseToCourse(HttpSession httpSession, String assessmentId, Course course, Boolean isVisible)
        {
            if (isRepository(course))
                return;
            String chamiloUrl = this.login.getPlatformUrl();
            String publishUrl = "/index.php?browser=table&application=weblcms&go=course_viewer&course=" + course.getCourseId()
                              + "&repoviewer_action=publisher&tool=assessment&tool_action=publisher&repo_object=" + assessmentId;

            String response;
            
            httpSession.setRequestUrl(chamiloUrl + publishUrl);

            httpSession.addNameValuePair("inherit", "0"); // rights
            httpSession.addNameValuePair("right_option", "0");
            httpSession.addNameValuePair("forever", "1");
            if (!isVisible)
                httpSession.addNameValuePair("hidden", "1");
            httpSession.addNameValuePair("collaborate", "1");
            httpSession.addNameValuePair("submit", "Publish");
            httpSession.addNameValuePair("_qf__content_object_publication_form", "");

            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
        }
        /// <summary>
        ///     Returns the modified date of a file in the repository. Returns an empty string if the file has not
        ///     been found.
        /// </summary>
        /// <param name="httpSession">The session.</param>
        /// <param name="fileId">The fileID.</param>
        /// <param name="uploadDestinationId">The destination ID.</param>
        /// <returns>The modified date.</returns>
        protected String getModifiedDate(HttpSession httpSession, String fileId, String uploadDestinationId)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String table = "/index.php?renderer=table"
                            +"&application=repository&go=browser"
                            + "&category=" + uploadDestinationId;
            String response;

            httpSession.setRequestUrl(chamiloUrl + table);
            httpSession.sendPOSTrequestSimple();

            response = httpSession.getResponseFromServer();

            String modifiedDate = String.Empty;

            SingleNodeScanner scanner = new SingleNodeScanner("tr");
            scanner.addNameValuePair("id", "row_" + fileId);
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode node = scanner.getNode();

            if (node != null)
            {
                XmlNode dateNode = node.ChildNodes[4];
                modifiedDate = dateNode.InnerText;
            }
            
            return modifiedDate;
        }

        /// <summary>
        ///     Edits or reuploads a file in the repository. The ID remains the same.
        /// </summary>
        /// <param name="httpSession">The session.</param>
        /// <param name="fileId">The file ID (object ID).</param>
        /// <param name="uploadDestinationId">The destination ID.</param>
        /// <param name="uploadFilePath">The full file name. (Can be null)</param>
        /// <param name="uploadedName">The file name. (Can be null if full path is given)</param>
        /// <param name="description">The description.</param>
        protected Boolean doUpdateFile(HttpSession httpSession, String fileId, String uploadDestinationId, String uploadFilePath, String uploadedName, String description)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String edit = "/index.php?go=editor"
                            + "&application=repository"
                            + "&object=" + fileId;
            
            String fileextension=null;
            String platformFileName = uploadedName;


            if (uploadFilePath != null)
            {
                FileInfo uploadfile;
                uploadfile = new FileInfo(uploadFilePath);
                platformFileName = uploadfile.Name.Replace(' ', '_');
                if (uploadedName == null || uploadedName.Equals(String.Empty))
                    uploadedName = uploadfile.Name;
                fileextension = uploadfile.Extension.Trim('.');
            }
            if (description == null || description.Equals(String.Empty))
                description = "<br />";


            httpSession.setRequestUrl(chamiloUrl + edit);
            
            httpSession.addNameValuePair("title", uploadedName);
            httpSession.addNameValuePair("parent_id", uploadDestinationId);
            httpSession.addNameValuePair("description", description.Replace("\n","<br />"));
            httpSession.addNameValuePair("comment", String.Empty);
            httpSession.addNameValuePair("submit", "Update");
            httpSession.addNameValuePair("_qf__edit", "");
            httpSession.addNameValuePair("id", fileId);
            httpSession.addNameValuePair("MAX_FILE_SIZE", "104857600");
            if (uploadFilePath != null)
                httpSession.sendPOSTrequestFromFormForUpload("file", platformFileName, uploadFilePath,
                                                          MIMEDictionary.getMimeType(fileextension));
            else
                httpSession.sendPOSTrequestFromForm();

            String responseUrl = httpSession.getResponseUrl();
            Boolean correctResponse = !Utility.findValueInURL(responseUrl, "message").Equals(String.Empty)
                                       && Utility.findValueInURL(responseUrl, "error_message").Equals(String.Empty);
            return correctResponse;
        }

        /// <summary>
        ///     Uploads a file to the platform, and returns the ID.
        /// </summary>
        /// <param name="httpSession">The HTTP session to use</param>
        /// <param name="uploadFilePath">The full file name of the file that has to be uploaded.</param>
        /// <param name="uploadedName">Name of the uploaded file item on the platform. Set to null to use the filename.</param>
        /// <param name="description">The description of the file.</param>
        /// <param name="uploadDestinationId">Destination on the platform.</param>
        /// <param name="TryCreateFolder">True if this method has to create a new folder.</param>
        /// <returns>the object ID of the file in the repository</returns>
        protected String uploadSingleFile(HttpSession httpSession, String uploadFilePath, String uploadedName, String description, String uploadDestinationId)
        {
            return uploadSingleFile(httpSession, uploadFilePath, uploadedName, description, uploadDestinationId, 0);
        }
        /// <summary>
        ///     Uploads a file to the platform, and returns the ID.
        /// </summary>
        /// <param name="httpSession">The HTTP session to use</param>
        /// <param name="uploadFilePath">The full file name of the file that has to be uploaded.</param>
        /// <param name="uploadedName">Name of the uploaded file item on the platform. Set to null to use the filename.</param>
        /// <param name="description">The description of the file.</param>
        /// <param name="uploadDestinationId">Destination on the platform.</param>
        /// <param name="TryCreateFolder">True if this method has to create a new folder.</param>
        /// <returns>the object ID of the file in the repository</returns>
        protected String uploadSingleFile(HttpSession httpSession, String uploadFilePath, String uploadedName, String description, String uploadDestinationId, int retries)
        {
            String fileId = String.Empty;

            String chamiloUrl = this.login.getPlatformUrl();
            String uploadUrl = "/index.php?renderer=table&application=repository&go=creator&content_object_type=" + OBJID_FILE+ "&category=" + uploadDestinationId;
            String response;

            FileInfo uploadfile = new FileInfo(uploadFilePath);
            String platformFileName = uploadfile.Name;
            // Removed for now, though I doubt it makes a difference; the platform itself does the same replace.
            //.Replace(' ', '_');
            if(uploadedName == null || uploadedName.Equals(String.Empty))
                uploadedName = uploadfile.Name;

            if (description == null || description.Equals(String.Empty))
                description = "<br />";

            while (fileId.Equals(String.Empty) && retries >= 0)
            {
                retries--;
                httpSession.setRequestUrl(chamiloUrl + uploadUrl);
                httpSession.sendPOSTrequestSimple();

                httpSession.setRequestUrl(chamiloUrl + uploadUrl);
                httpSession.addNameValuePair("title", uploadedName);
                httpSession.addNameValuePair("parent_id", uploadDestinationId);
                httpSession.addNameValuePair("description", description.Replace("\n","<br />"));
                httpSession.addNameValuePair("html_content", "");
                httpSession.addNameValuePair("choice", "0"); // upload file rather than create as text right there
                httpSession.addNameValuePair("submit", "Create");
                httpSession.addNameValuePair("_qf__create", "");
                // scan for this? Is <input name="MAX_FILE_SIZE" type="hidden" value="?????" />
                httpSession.addNameValuePair("MAX_FILE_SIZE", "104857600");

                httpSession.sendPOSTrequestFromFormForUpload("file", platformFileName, uploadFilePath,
                                                              MIMEDictionary.getMimeType(uploadfile.Extension.Trim('.')));

                response = httpSession.getResponseFromServer();

                XmlNodeList scanpage = SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes;

                if (Utility.findValueInURL(httpSession.getResponseUrl(), "error_message").Equals(String.Empty)
                    && !response.Contains("form_error")
                    && !response.Contains("error-message"))
                {
                    fileId = getIdFromObjectInCategory(httpSession, uploadedName, uploadDestinationId, OBJID_FILE);
                }
                else
                {
                    //Catch errors here.
                    SingleNodeScanner scanner = new SingleNodeScanner("span");
                    scanner.addNameValuePair("class","form_error");
                    scanner.doScan(scanpage);
                    XmlNode node = scanner.getNode();
                    if (node==null)
                    {
                        scanner = new SingleNodeScanner("div");
                        scanner.addNameValuePair("class", "error-message");
                        scanner.doScan(scanpage);
                        node = scanner.getNode();
                    }
                    String message = String.Empty;
                    if (node != null)
                        message = node.InnerText;
                    message = message.Replace("\n", "");
                    DomainController.Instance().writeToLog("upload_failed", new String[] { "\"" + message + "\"" }, true, false, false);
                    fileId = String.Empty;
                }
            }
            return fileId;
        }

        /// <summary>
        ///     Creates a new folder in the repository.
        /// </summary>
        /// <param name="foldername">The new folder name.</param>
        /// <param name="destinationName">Destination name.</param>
        /// <param name="destinationId">Destination ID.</param>
        /// <returns>A String array with the newly created folder name and id.</returns>
        protected String[] createFolderInRepository(HttpSession httpSession, String foldername, String destinationName, String destinationId)
        {
            DocumentFolder df = this.createFolderInDocuments(httpSession, repositoryDummy, foldername, destinationId, destinationName, false);
            String[] ret = null;
            if (df != null)
            {
                ret = new String[] { df.folderName, df.folders[0].folderCode };
                if (this.getRepositoryFolderCode(df.folderName) == null)
                    this.repositoryFolders.Add(ret);
            }
            return ret;
        }

        /// <summary>
        ///     Creates a new folder in the document folder in a certain course on the platform,
        ///     and adds it to the folders list of the course.
        /// </summary>
        /// <param name="httpSession">The HTTP session to use</param>
        /// <param name="course">The course. Leave null to make a folder in the repository.</param>
        /// <param name="foldername">The new folder name.</param>
        /// <param name="destinationId">ID of the destination folder</param>
        /// <param name="destinationName">Name of the destination folder</param>
        /// <param name="makeInvisible">True if the new folder has to be invisible for students.</param>
        /// <returns>the DocumentFolder object.</returns>
        protected DocumentFolder createFolderInDocuments(HttpSession httpSession, Course course, String foldername, String destinationId, String destinationName, Boolean makeInvisible)
        {
            String chamiloURL = this.login.getPlatformUrl();
            DocumentFolder retvalue = null;
            String response = String.Empty;
            String destination = "/" + this.getFormattedFilenameString(destinationName + '/' + foldername);

            //Create the new map.
            foldername = foldername.Trim('/');

            String createLink = String.Empty;

            if (isRepository(course))
            {
                createLink = "/index.php?renderer=table&application=repository&go=category_manager"
                    +"&category_action=create_category&category_id=" + destinationId;
            }
            else
            {
                createLink = "/index.php?application=weblcms&course=" + course.getCourseId()
                           + "&go=course_viewer&tool=document&tool_action=category_manager&browser=table"
                           + "&category_action=create_category&category_id=" + destinationId;
            }

            httpSession.setRequestUrl(chamiloURL + createLink);

            httpSession.addNameValuePair("name0", foldername);
            httpSession.addNameValuePair("_qf__category_form", ""); //Without this line, the form won't submit!
            httpSession.addNameValuePair("create", "Create");
            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
            String responseUrl = httpSession.getResponseUrl();
            Boolean correctResponse = !Utility.findValueInURL(responseUrl, "message").Equals(String.Empty)
                                       && Utility.findValueInURL(responseUrl, "error_message").Equals(String.Empty);


            if (correctResponse)
            {
                XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(response);
                FolderScanner folderScanner = new FolderScanner();
                folderScanner.doScan(xmlFoldersPage.ChildNodes);
                foreach (String[] s in folderScanner.getFolders())
                {
                    if (s[0].ToLower().Equals(destination.ToLower()))
                    {
                        DocumentFolder newdf = new DocumentFolder(course, s[1], destination);
                        if (!isRepository(course))
                            course.getDocumentFolders().addDocumentFolder(newdf);
                        else
                        {
                            repositoryFolders.Add(s);
                            this.repositoryDummy.setDocumentFolders(
                                new DocumentFoldersList(getRepositoryCategoriesAsDocumentfolders()));
                        }
                        retvalue = newdf;
                        break;
                    }
                }
            }
            else
            {
                // force full folder rescan
                if (isRepository(course))
                {
                    scanRepositoryFolders(httpSession);
                    String foldercode = getRepositoryFolderCode(destination);
                    if (foldercode != null)
                        retvalue = new DocumentFolder(repositoryDummy, foldercode, destination);
                }
                else
                {
                    DocumentFoldersList dfl = new DocumentFoldersList(readDocumentFolders(httpSession, course));
                    if (!dfl.isEmpty())
                    {
                        course.setDocumentFolders(dfl);
                        retvalue = dfl.getDocumentFolderFromFolderName(destination);
                    }
                }
            }

            if (makeInvisible && retvalue != null)
            {
                //make folder invisible (chamilo)
                //Cannot make folder invisible in Chamilo 2.0 for the moment.
            }

            return retvalue;
        }

        protected DocumentFolder createFolderInDocumentsRecursive(HttpSession httpSession, Course course, String foldername, String destinationId, String destinationName, Boolean makeInvisible)
        {
            DocumentFoldersList dfl = new DocumentFoldersList(this.readDocumentFolders(httpSession, course));
            DocumentFolder df = dfl.getDocumentFolderFromFolderName(destinationName);
            if (df == null)
            {
                foldername = destinationName.TrimEnd('/') + '/' + foldername;
                df = dfl.getDocumentFolderFromFolderName("/");
            }
            String[] tree = foldername.Split(new char[]{'/'},StringSplitOptions.RemoveEmptyEntries);
            if (tree.Length > 0)
            {
                foreach (String item in tree)
                {
                    if (df == null)
                        throw new Exception(DomainController.Instance().getLanguageString("error_creating_category") + ": \"" + foldername + "\" (\"" + item + "\") ");
                    destinationId = df.getIdForCourse(course);
                    destinationName = df.folderName;
                    df = dfl.getDocumentFolderFromFolderName(destinationName.TrimEnd('/') + "/" + item);
                    if (df == null)
                    {
                        df = this.createFolderInDocuments(httpSession, course, item, destinationId, destinationName, makeInvisible);
                    }
                }
            }
            return df;
        }

        /// <summary>
        ///     Sets the users full name.
        /// </summary>
        /// <param name="httpSession">The session.</param>
        protected void readUserFullName(HttpSession httpSession)
        {
            httpSession.setRequestUrl(this.login.getPlatformUrl() + ACCOUNT);
            httpSession.sendPOSTrequestSimple();

            String page = httpSession.getResponseFromServer();

            XmlDocument xmlUserPage = SGMLReaderHelper.htmlToXmlDocument(page);

            UserScanner userScanner = new UserScanner();
            userScanner.doScan(xmlUserPage.ChildNodes);

            this.firstName = userScanner.firstName;
            this.lastName = userScanner.lastName;
        }

        /// <summary>
        ///     Scans the repository folders, and stores them into a list.
        /// </summary>
        protected void scanRepositoryFolders(HttpSession httpSession)
        {
            String chamiloUrl = this.login.getPlatformUrl();
            String docUrl = "/index.php?renderer=table&application=repository&go=category_manager&category_id=0";

            httpSession.setRequestUrl(chamiloUrl + docUrl);
            httpSession.sendPOSTrequestSimple();

            String page = httpSession.getResponseFromServer();
            XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(page);

            FolderScanner folderScanner = new FolderScanner();
            folderScanner.doScan(xmlFoldersPage.ChildNodes);

            this.repositoryFolders = folderScanner.getFolders();
        }

        /// <summary>
        ///     checks if the currently handled course is actually the repository.
        /// </summary>
        /// <param name="course">course to check</param>
        /// <returns></returns>
        protected Boolean isRepository(Course course)
        {
            if (course == null || course.Equals(repositoryDummy)) return true;
            else return false;
        }

        /// <summary>
        ///     Log the user in.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <returns>True if the login succeeds.</returns>
        protected Boolean doLogin(HttpSession httpSession)
        {
            httpSession.setRequestUrl(this.login.getPlatformUrl() + INDEX);
            httpSession.addNameValuePair("login", login.getUsername());
            httpSession.addNameValuePair("password", login.getPassword());
            httpSession.addNameValuePair("submitAuth", "Login");
            httpSession.sendPOSTrequestFromForm();
            String content = httpSession.getResponseFromServer();
            String url = httpSession.getResponseUrl();
            if (url.Contains("loginFailed") || (content.Equals(String.Empty) || url.Equals(String.Empty) || httpSession.getErrorStatus()))
                return false;
            else
            {
                SingleNodeScanner scanner = new SingleNodeScanner("a",false, true);
                scanner.addNameValuePair("href", "application=user");
                scanner.addNameValuePair("href", "go=logout");
                scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(content).ChildNodes);
                return scanner.getNode() !=null;
            }
        }

        /// <summary>
        ///     Log the user out.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        protected void doLogout(HttpSession httpSession)
        {
            httpSession.setRequestUrl(this.login.getPlatformUrl() + LOGOUT);
        }

        #endregion

        public override Groups downloadGroups(Course course)
        {
            // TODO Not implemented
            return new Groups(course);
        }

        public override List<Groups> downloadGroups(List<Course> courses)
        {
            // TODO Not implemented
            List<Groups> groups = new List<Groups>();
            foreach (Course course in courses)
                groups.Add(new Groups(course));
            return groups;
        }

        public override List<String> getUsersFromGroup(Course course, lmsda.domain.score.data.Group groupId)
        {
            // TODO Not implemented
            return new List<String>();
        }
    }
}
