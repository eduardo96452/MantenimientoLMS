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
using lmsda.persistence.httpcommunication;
using lmsda.domain;
using lmsda.domain.util.xml;
using System.Xml;
using System.IO;
using lmsda.domain.util;
using lmsda.domain.exercise;
using System.Text.RegularExpressions;
using lmsda.domain.score.data;
using lmsda.domain.score;
using System.Threading;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///             Patrick Lauwaerts
    ///     This class contains all necessary information regarding the communication with the Dokeos 1.8 platform. 
    /// </summary>
    class Dokeos_1_8_5_hg : TargetPlatform
    {
        private const String PORTAL = "/user_portal.php";
        private const String LOGOUT = "/index.php?logout=true";
        private const String UPLOAD_FOLDER = "lms_upload";
        private const String AUTO_UPLOAD_STRING = "Uploaded with LMS Desktop Assistant";
        private Encoding encoding;
        private Login login;
        private TargetPlatformInfo platformInfo = new Dokeos_1_8_5_hg_PlatformInfo();

        private List<Course> courses;

        #region Constructor & platform information
        
        /// <summary>
        ///     Instantiates a new Dokeos object.
        /// </summary>
        /// <param name="login">The login information.</param>
        public Dokeos_1_8_5_hg(Login login)
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
                this.doLogout(httpSession);
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
                String page = httpSession.getResponseFromServer();
                XmlDocument xmlCoursesPage = SGMLReaderHelper.htmlToXmlDocument(page);
                
                CourseScanner courseScanner = new CourseScanner();
                courseScanner.doScan(xmlCoursesPage.ChildNodes);
                this.courses = courseScanner.getCourses();
                
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
            return filenameToFormat.Replace(' ', '_');
        }

        #endregion

        #region Sending information

        public override String[] doUploadFile(String uploadFilePath, String uploadedName, String description, DocumentFolder publishDestination, Subject subject, Boolean setInvisible)
        {
            HttpSession httpSession = new HttpSession(this.encoding);

            if (this.doLogin(httpSession))
            {
                List<String> retn = new List<String>();
                foreach (DocumentFolderForCourse dffc in publishDestination.folders)
                    retn.Add(uploadSingleFile(httpSession, uploadFilePath, uploadedName, description, dffc.course, dffc.folderCode, true, false, setInvisible));
                this.doLogout(httpSession);
                return retn.ToArray();
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            return null;
        }
        
        /// <summary>
        ///     Uploads a file to the flatform.
        /// </summary>
        /// <param name="httpSession"></param>
        /// <param name="uploadFilePath">The full file name of the file that has to be uploaded.</param>
        /// <param name="uploadedName">Name of the uploaded file item on the platform. Set to null to use the filename.</param>
        /// <param name="description">The description of the file.</param>
        /// <param name="course">The course.</param>
        /// <param name="uploadDestination">Destination on the platform.</param>
        /// <param name="TryCreateFolder">True if this method has to create a new folder.</param>
        /// <param name="giveDownloadlink">True if this method has to generate a download link.</param>
        /// <returns>The path of the uploaded file.</returns>
        private String uploadSingleFile(HttpSession httpSession, String uploadFilePath, String uploadedName, String description, Course course, String uploadDestination, Boolean TryCreateFolder, Boolean giveDownloadlink, Boolean setInvisible)
        {
            String dokeosUrl=this.login.getPlatformUrl();
            String response;
            FileInfo uploadfile = new FileInfo(uploadFilePath);
            if(uploadedName==null || uploadedName.Equals(String.Empty))
                uploadedName = uploadfile.Name;
            uploadDestination= '/' + uploadDestination.Trim('/');

            DocumentFolder df;
            if (TryCreateFolder)
            {
                df = this.createFolderInDocumentsRecursive(httpSession, course, uploadDestination, "/",
                    uploadDestination.Trim('/').Equals(UPLOAD_FOLDER) // maak onzichtbaar als het de documents folder is
                            );
            }

            httpSession.setRequestUrl(dokeosUrl + "/main/document/upload.php?cidReq=" + course.getCourseId()
                                                 + "&path=" + System.Web.HttpUtility.UrlEncode(uploadDestination));
            
            httpSession.addNameValuePair("if_exists", "overwrite");
            httpSession.addNameValuePair("title",uploadedName);
            httpSession.addNameValuePair("comment",description);
            httpSession.addNameValuePair("submitDocument", "OK");
            httpSession.addNameValuePair("_qf__upload", "");
            httpSession.addNameValuePair("curdirpath", uploadDestination);
            httpSession.addNameValuePair("MAX_FILE_SIZE", "524288000");
            
            httpSession.sendPOSTrequestFromFormForUpload("user_upload", uploadedName.Replace(' ', '_'), uploadFilePath,
                                                          MIMEDictionary.getMimeType(uploadfile.Extension.Trim('.')));
            response  = httpSession.getResponseFromServer();
            
            if (response.Contains("class=\"confirmation-message\""))
            {
                if (setInvisible)
                    this.setFileVisibility(httpSession, course, uploadDestination, uploadedName, false);

                if (!giveDownloadlink)
                    return dokeosUrl + "/courses/" + course.getCourseId() + '/' + "document" + uploadDestination + '/' + uploadedName;
                else
                    return dokeosUrl + "/main/document/document.php?action=download&id=" + uploadDestination + '/' + uploadedName;
            }
            else if (response.Contains("class=\"error-message\""))
            {
                // dokeos error
                return String.Empty;
            }
            else
            {
                return String.Empty;
            }
        }

        public override DocumentFolder doCreateFolderInDocuments(DocumentFolder publishDestination, String foldername, Boolean setInvisible)
        {
            DocumentFolder df = null;
            HttpSession httpSession = new HttpSession(this.encoding);
            if (this.doLogin(httpSession))
            {
                foreach (DocumentFolderForCourse folder in publishDestination.folders)
                {
                    if (df == null)
                        df = createFolderInDocumentsRecursive(httpSession, folder.course, foldername, publishDestination.folderName, setInvisible);
                    else
                    {
                        // multiple courses in documentfolder; create additional ones and merge them.
                        DocumentFolder dfl = createFolderInDocumentsRecursive(httpSession, folder.course, foldername, publishDestination.folderName, setInvisible);
                        if (dfl != null) df.mergeFolders(dfl);
                    }
                }
                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }
            return df;
        }

        private DocumentFolder createFolderInDocumentsRecursive(HttpSession httpSession, Course course, String foldername, String destination, Boolean makeInvisible)
        {
            DocumentFoldersList dfl = new DocumentFoldersList(this.readDocumentFolders(httpSession, course));
            DocumentFolder df = dfl.getDocumentFolderFromFolderName(this.getFormattedFilenameString(destination));
            if (df == null)
            {
                foldername = destination.TrimEnd('/') + '/' + foldername;
                df = dfl.getDocumentFolderFromFolderName("/");
            }
            String[] tree = foldername.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (tree.Length > 0)
            {
                foreach (String item in tree)
                {
                    destination = df.folderName;
                    df = dfl.getDocumentFolderFromFolderName(this.getFormattedFilenameString(destination.TrimEnd('/') + "/" + item));
                    if (df == null)
                    {
                        df = this.createFolderInDocuments(httpSession, course, item, destination, makeInvisible);
                    }
                }
            }
            if (df != null)
                return df;
            else return null;
        }

        /// <summary>
        ///     Creates a new folder in the documents folder of one course on the platform.
        /// </summary>
        /// <param name="course">The course code.</param>
        /// <param name="foldername">The new folder name.</param>
        /// <param name="destination">Folder code of the folder in which the subfolder has to be created.</param>
        /// <param name="makeInvisible">True if the new folder has to be invisible for students.</param>
        /// <returns>True if the folder has been created.</returns>
        private DocumentFolder createFolderInDocuments(HttpSession httpSession, Course course, String foldername, String destination, Boolean makeInvisible)
        {
            String dokeosUrl=this.login.getPlatformUrl();
            String response;
            destination = '/' + destination.Trim('/');
            foldername = foldername.Trim('/');

            if (foldername.Contains('/'))
            {
                destination = destination + '/' + foldername;
                destination = '/' + destination.Trim('/');
                foldername = destination.Substring(destination.LastIndexOf('/')).Trim('/');
                destination = '/' + destination.Substring(0, destination.LastIndexOf('/')).Trim('/');

            }

            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                                + "&curdirpath=" + System.Web.HttpUtility.UrlEncode(destination)
                                                + "&createdir=1");
            
            httpSession.addNameValuePair("curdirpath", System.Web.HttpUtility.UrlEncode(destination));
            httpSession.addNameValuePair("dirname",foldername);
            httpSession.addNameValuePair("create_dir","OK");
            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
            Boolean correctResponse = response.Contains("class=\"confirmation-message\"");
            
            if (correctResponse)
            {
                if(makeInvisible)
                    this.setFileVisibility(httpSession, course, destination, foldername, false);

                destination = destination + '/' + foldername;
                destination = getFormattedFilenameString('/' + destination.Trim('/'));
                DocumentFolder df = new DocumentFolder(course,destination,destination);
                course.getDocumentFolders().addDocumentFolder(df);
                return df;
            }
            return null;
        }

        private Boolean folderExists(HttpSession httpSession, Course course, String foldername)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                    + "&curdirpath=" + System.Web.HttpUtility.UrlEncode(foldername));
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();

            SingleNodeScanner scanner = new SingleNodeScanner("select");
            scanner.addNameValuePair("name", "curdirpath");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode node = scanner.getNode();
            scanner = new SingleNodeScanner("option");
            scanner.addNameValuePair("selected", "selected");
            scanner.doScan(node.ChildNodes);
            if (scanner.getNodes().Count == 1)
                return true;
            else
            return false;
        }

        public override Boolean doUploadExercises(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions, Boolean onlyIfDoesntExist)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            try
            {
                if (this.doLogin(httpSession))
                {
                    foreach (Course course in publishDestinations)
                    {
                        int exerciseNumber = 0;
                        foreach (Exercise exercise in exercises)
                        {
                            exerciseNumber++;
                            this.uploadSingleExercise(httpSession, exercise, course, exerciseNumber, setExerciseInvisible, randomQuestions, onlyIfDoesntExist);
                        }
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
            catch
            {
                DomainController.Instance().writeToLog("exercises_upload_failed", true, false, !DomainController.Instance().isSynchronization);
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
                    Boolean returnvalue = true;
                    foreach (Course course in publishDestinations)
                    {
                        int exerciseNumber = 0;
                        foreach (Exercise exercise in exercises)
                        {
                            exerciseNumber++;
                            if (!this.updateSingleExerciseInfo(httpSession, exercise, course, exerciseNumber, randomQuestions, setExerciseInvisible))
                                returnvalue = false;
                        }
                    }
                    this.doLogout(httpSession);
                    DomainController.Instance().writeToLog("exercises_update_completed", true, false, !DomainController.Instance().isSynchronization);
                    return returnvalue;
                }
                else
                {
                    DomainController.Instance().writeToLog("cannot_login", true, false);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public override Boolean doUpdateFile(String filename, DocumentFolder path, Subject subject, String description, Boolean visible)
        {
            HttpSession httpSession = new HttpSession(this.encoding);
            try
            {
                if (this.doLogin(httpSession))
                {
                    Boolean returnvalue = true;
                    foreach (DocumentFolderForCourse dffc in path.folders)
                    {
                        returnvalue &= this.setFileVisibility(httpSession, dffc.course, dffc.folderCode, filename, visible);
                        returnvalue &= this.updateFileDescription(httpSession, dffc.course, dffc.folderCode, filename, description);
                    }
                    this.doLogout(httpSession);
                    DomainController.Instance().writeToLog("visibility_changed", true, false, !DomainController.Instance().isSynchronization);
                    return returnvalue;
                }
                else
                {
                    DomainController.Instance().writeToLog("cannot_login", true, false);
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        #endregion

        #region Statistics

        public override List<Scores> downloadStatistics(List<Course> courses, String saveTo, Boolean createSubFolder, Boolean withoutMissingData)
        {
            List<Scores> retn = new List<Scores>();
            foreach (Course targetCourse in courses)
            {
                //Added by Patrick Lauwaerts on 2010-11-23:Utility.getFileNameWithoutBadCharacters
                String folderName = Path.DirectorySeparatorChar + Utility.getFileNameWithoutBadCharacters(targetCourse.getCourseName() + "_" + targetCourse.getCourseCode()) + "_TEMP";
                String save = saveTo + folderName.Replace(' ', '_');

                //Clear temp folder
                if(Directory.Exists(save))
                    Directory.Delete(save, true);

                retn.Add(this.downloadStatistics(targetCourse, save, withoutMissingData));
            }
            return retn;
        }

        public Scores downloadStatistics(Course course, String saveTo, Boolean withoutMissingData)
        {
            DomainController.Instance().writeToLog("downloading_statistics", true, false);
            HttpSession httpSession = new HttpSession(this.encoding);
            StatisticsScanner scanner = new StatisticsScanner(course);
            Scores scores = null;

            if (this.doLogin(httpSession))
            {
                List<String> rawData = this.downloadUserListAndResults(httpSession, course, saveTo);
                scores = scanner.scan(rawData);
                
                if (DomainController.Instance().getSettings().getStatsDeleteRawDataAfterConversion())
                {
                    //Delete raw data...
                    //Wait until the resources are cleared...
                    Thread.Sleep(2000);
                    foreach(String path in rawData)
                        Utility.tryDeleteFile(path);
                }

                if (!withoutMissingData)
                {
                    DomainController.Instance().writeToLog("downloading_statistics_missing_data", true, false);

                    List<QuestionResult> questions = new List<QuestionResult>();
                    List<AnswerResult> answers = new List<AnswerResult>();

                    //For all students...
                    for (int i = 0; i < scores.studentsCount; i++)
                    {
                        Student student = scores.getStudentAt(i);
                        //For all exercises...
                        for (int j = 0; j < student.exerciseCount; j++)
                        {
                            ExerciseResult exercise = student.getExerciseResultAt(j);

                            //For all questions...
                            for (int k = 0; k < exercise.questionCount; k++)
                            {
                                QuestionResult question = exercise.getQuestionResultAt(k);
                                //Question already in list?
                                if (question.answerType == QuestionType.MULTIPLE_CHOICE_SINGLE || question.answerType == QuestionType.MULTIPLE_CHOICE_SEVERAL) //Enkel voor MC enkel en meerdere
                                {
                                    Boolean inList = false;
                                    foreach (QuestionResult q in questions)
                                    {
                                        if (q.questionID == question.questionID)
                                        {
                                            //Already in list, do not download again (saves time)
                                            inList = true;
                                            answers = q.getAnswerResults();
                                            break;
                                        }
                                    }
                                    if (!inList)
                                    {
                                        //Not yet in list, download question and its answers.
                                        answers = this.getAnswersFromQuestion(httpSession, course, question.questionID, question.answerType);
                                        questions.Add(question);
                                    }

                                    //Add the missing answers - if any - to the question.
                                    foreach (AnswerResult answer in answers)
                                    {
                                        Boolean chosen = false;
                                        int index = -1;

                                        for (int z = 0; z < question.answerCount; z++)
                                        {
                                            if (question.getAnswerResultAt(z).answer.Equals(answer.answer))
                                            {
                                                chosen = true;
                                                index = z;
                                                break;
                                            }
                                        }

                                        if (chosen)
                                            question.getAnswerResults().RemoveAt(index);

                                        question.addAnswer(new AnswerResult(answer.answer, answer.correct, chosen));
                                    }
                                }
                            }
                        }
                    }
                }
                this.doLogout(httpSession);

                scores.linkStudentsToGroups();
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }            
            return scores;
        }

        /// <summary>
        ///     Downloads the files that are needed to generate the statistics.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <returns>A list with the downloaded files.</returns>
        private List<String> downloadUserListAndResults(HttpSession httpSession, Course course, String saveTo)
        {
            List<String> downloadedFileNames = new List<String>();

            //Create the output folder if it's nonexistent.
            Directory.CreateDirectory(saveTo);
            //if(!new DirectoryInfo(saveTo).Exists)
            //    new DirectoryInfo(saveTo).Create();

            //Download users list...
            httpSession.setRequestUrl(this.login.getPlatformUrl() + "/main/user/user.php?action=export&type=csv&cidReq=" + course.getCourseId());
            httpSession.sendPOSTrequestFromFormForDownload(saveTo);
            downloadedFileNames.Add(httpSession.getDownloadFilePath());

            //Download results...
            httpSession.setRequestUrl(this.login.getPlatformUrl() + "/main/exercice/exercice.php?show=result&export_id=all&cidReq=" + course.getCourseId());
            httpSession.sendPOSTrequestFromFormForDownload(saveTo);
            downloadedFileNames.Add(httpSession.getDownloadFilePath());
            
            return downloadedFileNames;
        }

        /// <summary>
        ///     Returns the answers of the given question.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="courseCode">The course code.</param>
        /// <param name="questionID">The working question ID.</param>
        /// <param name="type">The question type.</param>
        /// <returns>A list of answers.</returns>
        public List<AnswerResult> getAnswersFromQuestion(HttpSession httpSession, Course course, int questionID, QuestionType type)
        {
            //Surf to "edit question" (ID) and "course code" (ID).
            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/exercice/admin.php?myid=1&editQuestion=" + questionID + "&cidReq=" + course.getCourseId());
            httpSession.sendPOSTrequestSimple();
            String response = httpSession.getResponseFromServer();
            XmlDocument xmlResponse = SGMLReaderHelper.htmlToXmlDocument(response);

            //We've retrieved the page with the question and its answers.
            //Now we have to extract those answers...
            SingleNodeScanner sns;
            List<AnswerResult> answers = new List<AnswerResult>();
            int answerCount = 0;

            XmlNodeList nodeList = xmlResponse.ChildNodes;

            sns = new SingleNodeScanner("input");

            sns.addNameValuePair("name", "nb_answers");
            sns.doScan(nodeList);

            if(sns.getNode() != null && sns.getNode().Attributes["value"] != null)
                answerCount = Convert.ToInt32(sns.getNode().Attributes["value"].Value);

            //Add all answers...
            for (int i = 1; i <= answerCount; i++)
            {
                sns = new SingleNodeScanner("input");
                
                if (type == QuestionType.MULTIPLE_CHOICE_SINGLE)
                {
                    sns.addNameValuePair("name", "correct");
                    sns.addNameValuePair("value", Convert.ToString(i));
                    sns.addNameValuePair("type", "radio");
                }
                else
                {
                    sns.addNameValuePair("name", "correct[" + i + "]");
                    sns.addNameValuePair("type", "checkbox");
                }
                
                sns.doScan(nodeList);
                
                if (sns.getNode() != null)
                {
                    Boolean correct = sns.getNode().Attributes["checked"] != null && sns.getNode().Attributes["checked"].Value.Equals("checked");
                    String answerText = String.Empty;

                    sns = new SingleNodeScanner("textarea");
                    sns.addNameValuePair("name", "answer[" + i + "]");
                    
                    sns.doScan(nodeList);
                    if (sns.getNode() != null)
                        answerText = sns.getNode().InnerText;

                    answers.Add(new AnswerResult(System.Web.HttpUtility.HtmlDecode(answerText), correct, false));
                }
            }

            return answers;
        }
        
        #endregion

        #region Groups (As of 1.09)

        public override Groups downloadGroups(Course course)
        {
            DomainController.Instance().writeToLog("retrieve_groups_for_course_x", new String[] { course.getCourseCode() }, true, false, false);
            Groups groups = new Groups(course);

            HttpSession httpSession = new HttpSession(this.encoding);

            if (this.doLogin(httpSession))
            {
                //Retrieve groups...
                String dokeosUrl = this.login.getPlatformUrl();
                httpSession.setRequestUrl(dokeosUrl
                                        + "/main/group/group.php?cidReq="
                                        + course.getCourseCode()
                                        + "&show_all=1");

                httpSession.sendPOSTrequestSimple();
                String page = httpSession.getResponseFromServer();

                XmlDocument xmlDocument = SGMLReaderHelper.htmlToXmlDocument(page);

                GroupsScanner groupsScanner = new GroupsScanner(course);
                groupsScanner.doScan(xmlDocument.ChildNodes);
                groups = groupsScanner.getGroups();

                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }

            return groups;
        }

        public override List<Groups> downloadGroups(List<Course> courses)
        {
            List<Groups> groups = new List<Groups>();

            foreach(Course course in courses)
                groups.Add(this.downloadGroups(course));

            return groups;
        }

        public override List<String> getUsersFromGroup(Course course, lmsda.domain.score.data.Group group)
        {
            // returns a list with email addresses
            List<String> addresses = new List<String>();
            
            HttpSession httpSession = new HttpSession(this.encoding);

            if (this.doLogin(httpSession))
            {
                String dokeosUrl = this.login.getPlatformUrl();
                httpSession.setRequestUrl(dokeosUrl
                                        + "/main/group/group_space.php?cidReq="
                                        + course.getCourseCode()
                                        + "&gidReq="
                                        + group.id
                                        + "&group_users_per_page="
                                        + group.studentCount);

                httpSession.sendPOSTrequestSimple();
                String page = httpSession.getResponseFromServer();

                XmlDocument xmlDocument = SGMLReaderHelper.htmlToXmlDocument(page);

                StudentsInGroupScanner scanner = new StudentsInGroupScanner();
                scanner.doScan(xmlDocument.ChildNodes);
                addresses = scanner.getAddresses();

                this.doLogout(httpSession);
            }
            else
            {
                DomainController.Instance().writeToLog("cannot_login", true, false);
            }

            return addresses;
        }

        #endregion

        #region Private methods

        private List<DocumentFolder> readDocumentFolders(HttpSession httpSession, Course course)
        {
            List<DocumentFolder> folders = new List<DocumentFolder>();

            String dokeosUrl=this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                    + "&curdirpath=/"
                                    + "&tablename_per_page=1000000");

            httpSession.sendPOSTrequestSimple();
            String page = httpSession.getResponseFromServer();
            XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(page);
            
            FolderScanner folderScanner = new FolderScanner();
            folderScanner.doScan(xmlFoldersPage.ChildNodes);

            foreach (String s in folderScanner.getFolders())
            {
                folders.Add(new DocumentFolder(course, s, s));
            }
            return folders;
        }

        
        /// <summary>
        ///     Uploads or updates a single exercise to the platform.
        /// </summary>
        /// <param name="exercise">The exercise.</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <param name="noUpdate">Only create new ones; don't update existing ones</param>
        private void uploadSingleExercise(HttpSession httpSession, Exercise exercise, Course course, int exerciseNumber, Boolean setExerciseInvisible, int randomQuestions, Boolean noUpdate)
        {
            if (exercise.getQuestionsAsList().Count == 0) 
                return;
            Exercise exercise_clone = exercise.clone();
            int exerciseID = this.createOrUpdateExercise(httpSession, exercise_clone, course, exerciseNumber, randomQuestions, noUpdate);

            if (exerciseID != -1) // -1 = cancelled by noUpdate parameter.
            {
                if (setExerciseInvisible)
                    httpSession.setRequestUrl(DomainController.Instance().getSettings().getUrl() + "/main/exercice/exercice.php?choice=disable&page=&exerciseId=" + exerciseID);
                else
                    httpSession.setRequestUrl(DomainController.Instance().getSettings().getUrl() + "/main/exercice/exercice.php?choice=enable&page=&exerciseId=" + exerciseID);

                httpSession.sendPOSTrequestSimple();

                this.uploadExerciseImages(httpSession, exercise_clone, exerciseID, course, exerciseNumber);
                // necessary if exercise description contains images
                this.updateExerciseInfo(httpSession, exerciseID, exercise_clone, course, randomQuestions);
                DomainController.Instance().writeToLog("uploading_questions_exercise_x_course_y", new String[] { exerciseNumber.ToString(), course.getCourseCode() }, true, false, false);
                this.addQuestionsToExercise(httpSession, exerciseID, exercise_clone, course);
            }
        }

        /// <summary>
        ///     Updates the information of a single exercise on the platform, without touching the questions or descriptions.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="exercise">The exercise. Only used to get the exercise name to look up on the platform.</param>
        /// <param name="course">The course.</param>
        /// <param name="exerciseNumber">Number of the exercise in the full list of exercises that is being processed.</param>
        /// <param name="randomQuestions">Number of random questions to set for this exercise.</param>
        /// <param name="setExerciseInvisible">Determines whethr the exercise should be set invisible or visible.</param>
        /// <returns></returns>
        private Boolean updateSingleExerciseInfo(HttpSession httpSession, Exercise exercise, Course course, int exerciseNumber, int randomQuestions, Boolean setExerciseInvisible)
        {
            Exercise exercise_clone = exercise.clone();
            int exerciseID=findExerciseOnPlatform(httpSession, exercise_clone.getName(), course);
            
            if (exerciseID == -1)
                return false;
            DomainController.Instance().writeToLog("updating_exercise_x_course_y", new String[]{exerciseNumber.ToString(), course.getCourseCode()}, true, false, false);
            
            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/exercice/exercise_admin.php?cidReq=" + course.getCourseId()
                                    + "&modifyExercise=yes"
                                    + "&exerciseId=" + exerciseID
                         );
            httpSession.sendPOSTrequestSimple();

            String response = httpSession.getResponseFromServer();
            SingleNodeScanner scanner = new SingleNodeScanner("textarea");
            scanner.addNameValuePair("name", "exerciseDescription");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode node = scanner.getNode();
            if (node == null)
                return false;
            exercise_clone.setDescription(node.InnerText);

            updateExerciseInfo(httpSession, exerciseID, exercise_clone, course, randomQuestions);
            if (setExerciseInvisible)
                httpSession.setRequestUrl(DomainController.Instance().getSettings().getUrl() + "/main/exercice/exercice.php?choice=disable&page=&exerciseId=" + exerciseID);
            else
                httpSession.setRequestUrl(DomainController.Instance().getSettings().getUrl() + "/main/exercice/exercice.php?choice=enable&page=&exerciseId=" + exerciseID);
            httpSession.sendPOSTrequestSimple();
            return true;

        }

        /// <summary>
        ///     Creates a new exercise, or updates an existing one and deletes its questions. The result of this is ALWAYS an exercise without any questions in it.
        /// </summary>
        /// <param name="exercise">The exercise.</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <param name="exerciseNumber">Number of the exercise in the full list of exercises that is being processed.</param>
        /// <param name="randomQuestions">Number of random questions to set for this exercise.</param>
        /// <param name="noUpdate">If the exercise already exists, perform no ations and don't return ID (to prevent overwrite).</param>
        /// <returns>The exercise ID.</returns>
        private int createOrUpdateExercise(HttpSession httpSession, Exercise exercise, Course course, int exerciseNumber, int randomQuestions, Boolean noUpdate)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            int exerciseID=findExerciseOnPlatform(httpSession, exercise.getName(), course);

            if (exerciseID != -1 && noUpdate)
                return -1;
            if (exerciseID == -1)
            {
                // Create new exercise
                DomainController.Instance().writeToLog("creating_exercise_x_course_y", new String[]{exerciseNumber.ToString(), course.getCourseCode()}, true, false, false);
                String response;

                httpSession.setRequestUrl(dokeosUrl + "/main/exercice/exercise_admin.php?cidReq=" + course.getCourseId());

                httpSession.addNameValuePair("exerciseTitle", exercise.getName());
                httpSession.addNameValuePair("exerciseDescription", exercise.getDescription());
                //1 for one page, 2 for multiple pages. This is the Boolean "multipage" + 1.
                httpSession.addNameValuePair("exerciseType", (Convert.ToInt32(exercise.isMultipage()) + 1).ToString());
                httpSession.addNameValuePair("randomQuestions", Convert.ToString(randomQuestions));
                httpSession.addNameValuePair("submitExercise", "OK");
                httpSession.addNameValuePair("_qf__exercise_admin", "");
                httpSession.addNameValuePair("edit", "false");
                httpSession.sendPOSTrequestFromForm();

                response = httpSession.getResponseFromServer();

                if (response.Contains("class=\"confirmation-message\""))
                {
                    try
                    {
                        exerciseID = Convert.ToInt32(Utility.findValueInURL(httpSession.getResponseUrl(), "exerciseId"));
                    }
                    catch
                    {
                        throw new Exception("Unknown error: can't retrieve exercise ID");
                    }
                }
                else if (response.Contains("class=\"error-message\""))
                {
                    throw new Exception("Dokeos error: Creation of exercise failed");
                }
                else
                {
                    throw new Exception("Unknown error");
                }
            }
            else
            {
                // Update existing exercise
                DomainController.Instance().writeToLog("updating_exercise_x_course_y", new String[]{exerciseNumber.ToString(), course.getCourseCode()}, true, false, false);
                List<int> currentQuestions = getQuestionIDsFromExercise(httpSession, exerciseID, course);
                foreach (int quesID in currentQuestions)
                {
                    // delete from exercise
                    httpSession.setRequestUrl(dokeosUrl + "/main/exercice/admin.php?cidReq=" + course.getCourseId()
                                                        + "&exerciseID=" + exerciseID
                                                        + "&deleteQuestion=" + quesID);
                    httpSession.sendPOSTrequestSimple();
                    // delete from database
                    httpSession.setRequestUrl(dokeosUrl + "/main/exercice/question_pool.php?cidReq=" + course.getCourseId()
                                                        + "&delete=" + quesID);
                    httpSession.sendPOSTrequestSimple();
                }
                updateExerciseInfo(httpSession, exerciseID, exercise, course, randomQuestions);
            }
            return exerciseID;
        }

        /// <summary>
        ///     Adds a question to a exercise.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <param name="exercise">the exercise object.</param>
        /// <param name="courseCode">The course.</param>
        /// <param name="httpSession">The working session.</param>
        private void addQuestionsToExercise(HttpSession httpSession, int exerciseID, Exercise exercise, Course course)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            List<Question> questions = exercise.getQuestionsAsList();

            foreach (Question question in questions)
            {
                int numberOfAnswers = question.getAnswersAsList().Count;

                if (numberOfAnswers < 1) 
                    break;

                int questionType = 0;

                if      (question.getQuestionType() == QuestionType.MULTIPLE_CHOICE_SINGLE) 
                    questionType = QuestionTypes.MULTIPLE_CHOICE_SINGLE;
                else if (question.getQuestionType() == QuestionType.MULTIPLE_CHOICE_SEVERAL) 
                    questionType = QuestionTypes.MULTIPLE_CHOICE_SEVERAL;
                else if (question.getQuestionType() == QuestionType.FILL_IN_THE_GAPS
                      || question.getQuestionType() == QuestionType.FILL_IN_THE_GAPS_DROPDOWN)
                    questionType = QuestionTypes.FILL_IN_THE_GAPS;
                else if (question.getQuestionType() == QuestionType.MATCHING) 
                    questionType = QuestionTypes.MATCHING;
                else if (question.getQuestionType() == QuestionType.OPEN_QUESTION) 
                    questionType = QuestionTypes.OPEN_QUESTION;

                //Needed to reset the internal session variables.
                //Otherwise, Dokeos will use the ID of the last opened exercise instead of the URL ID.
                httpSession.setRequestUrl(dokeosUrl + "/main/exercice/exercice.php?cidReq=" + course.getCourseId());
                httpSession.sendPOSTrequestSimple();

                httpSession.setRequestUrl(dokeosUrl + "/main/exercice/admin.php?cidReq=" + course.getCourseId()
                                                    + "&exerciseId=" + exerciseID
                                                    + "&newQuestion=yes"
                                                    + "&answerType=" + questionType.ToString()
                                         );
                String questionTitle = question.getQuestionTitle().Trim();
                if (questionTitle == null || questionTitle.Equals(String.Empty))
                    questionTitle = "_";
                // Trim weight from string (weight is not used in Chamilo)
                questionTitle = Utility.trimWeightFromString(questionTitle, true);

                httpSession.addNameValuePair("Type", questionType.ToString());
                httpSession.addNameValuePair("answerType", questionType.ToString());
                httpSession.addNameValuePair("questionName",questionTitle);
                httpSession.addNameValuePair("questionDescription",question.getQuestionText());

                switch (questionType)
                {
                    case QuestionTypes.MULTIPLE_CHOICE_SINGLE:
                        this.addAnswersMCSingle(httpSession, question);
                        break;
                    case QuestionTypes.MULTIPLE_CHOICE_SEVERAL:
                        this.addAnswersMCMultiple(httpSession, question);
                        break;
                    case QuestionTypes.FILL_IN_THE_GAPS:
                        this.addAnswersGaps(httpSession, question);
                        break;
                    case QuestionTypes.MATCHING:
                        this.addAnswersMatching(httpSession, question);
                        break;
                    case QuestionTypes.OPEN_QUESTION:
                        this.addOpenQuestionWeight(httpSession, question);
                        break;
                }
                httpSession.addNameValuePair("submitQuestion", "OK");
                httpSession.addNameValuePair("_qf__question_admin_form", "");
                httpSession.addNameValuePair("myid", "1");

                httpSession.sendPOSTrequestFromForm();
            }
        }

        /// <summary>
        ///     Adds a weight to an open question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="httpSession">The working session.</param>
        private void addOpenQuestionWeight(HttpSession httpSession, Question question)
        {
            int weight=question.getAnswersAsArray()[0].getWeight();
            httpSession.addNameValuePair("weighting", weight.ToString());
        }

        /// <summary>
        ///     Adds the answers of a MC with 1 correct answer to a question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="httpSession">The working session.</param>
        private void addAnswersMCSingle(HttpSession httpSession, Question question)
        {
            Answer[] answers = question.getAnswersAsArray();
            int correctAnswer = 1;
            int maxweight = int.MinValue;
            for (int i = 0; i < answers.Length; i++)
            {
                httpSession.addNameValuePair("answer["+ (i+1) +"]",answers[i].getAnswer());
                httpSession.addNameValuePair("comment["+ (i+1) +"]",answers[i].getFeedback());
                httpSession.addNameValuePair("weighting["+ (i+1) +"]",answers[i].getWeight().ToString());
                if(answers[i].getWeight() > maxweight)
                {
                    maxweight = answers[i].getWeight();
                    correctAnswer = i+1; // on the form, answers begins at index base 1.
                }
            }
            httpSession.addNameValuePair("correct", correctAnswer.ToString());
            httpSession.addNameValuePair("nb_answers", answers.Length.ToString());
        }

        /// <summary>
        ///     Adds the answers of a MC with multiple correct answers to a question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="httpSession">The working session.</param>
        private void addAnswersMCMultiple(HttpSession httpSession, Question question)
        {
            Answer[] answers = question.getAnswersAsArray();
            for (int i = 0; i < answers.Length; i++)
            {
                httpSession.addNameValuePair("answer["+ (i+1) +"]", answers[i].getAnswer());
                httpSession.addNameValuePair("comment["+ (i+1) +"]", answers[i].getFeedback());
                httpSession.addNameValuePair("weighting["+ (i+1) +"]", answers[i].getWeight().ToString());
                httpSession.addNameValuePair("correct["+ (i+1) +"]", Convert.ToInt16(answers[i].getWeight() > 0).ToString());
            }
            httpSession.addNameValuePair("nb_answers", answers.Length.ToString());
        }

        /// <summary>
        ///     Adds gaps answers to a question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="httpSession">The working session.</param>
        private void addAnswersGaps(HttpSession httpSession, Question question)
        {
            const String regexStart = "{regex(=[0-9]+)?}";
            const String regexEnd = "{/regex}";
            const String regexContains = "^" + regexStart + ".*?" + regexEnd;
            
            Answer[] answers = question.getAnswersAsArray();
            String answer = answers[0].getAnswer();

            Regex r = new Regex(@"\[([^\[\]]+)\]", RegexOptions.IgnoreCase);
            Match matcher = r.Match(answer);

            String plainText = answer;
            List<int> answerWeights = new List<int>();
            List<String> feedbacks = new List<String>();
            int indexcorrection = 0;
            String repl = String.Empty;
            String orig = String.Empty;
            while (matcher.Success)
            {
                orig = matcher.Groups[1].Value;
                repl = filterWeightAndFeedbackFromGapsTag(orig, ref answerWeights, ref feedbacks);
                answer = answer.Substring(0, matcher.Groups[1].Index + indexcorrection) + repl + answer.Substring(matcher.Groups[1].Index + matcher.Groups[1].Length + indexcorrection);
                indexcorrection += repl.Length - orig.Length;
                matcher = matcher.NextMatch();
            }

            int answernumber = -1;
            matcher = r.Match(answer);
            indexcorrection=0;
            while (matcher.Success)
            {
                repl=String.Empty;
                answernumber++;
                if (answernumber < answerWeights.Count)
                {
                    String feedback = feedbacks[answernumber];
                    if (feedback==null) feedback=String.Empty;
                    String answerText = matcher.Groups[1].Value;
                    Boolean regexFound = Regex.IsMatch(answerText, regexContains);
                    if (feedback != String.Empty || answerText.Contains('|') || regexFound)
                    {
                        if (!regexFound)
                        {
                            String[] gappossibilities = answerText.Split('|');
                            foreach (String gappossibility in gappossibilities)
                            {
                                repl += "^" + gappossibility + "$|";
                            }
                            repl = feedback + "{regex}" + repl.TrimEnd('|') + "{/regex}";
                        }
                        else
                        {
                            // backwards compatibility for new regex tags with size field "{regex=16}"
                            answerText = Regex.Replace(answerText, regexStart, "{regex}");
                            const String multipleRegex = "{/regex}|{regex}";
                            if (answerText.IndexOf(multipleRegex) != -1)
                            {
                                answerText = answerText.Replace("{/regex}|{regex}", ")|(");
                                answerText = answerText.Replace("{/regex}", "){/regex}");
                                answerText = answerText.Replace("{regex}", "{regex}(");

                            }
                            // fix for Dokeos problem where \ characters are escaped incorrectly, causing
                            // the platform to remove non-doubled \ characters.
                            repl = feedback + Regex.Replace(answerText, @"\\", @"\\");
                            //repl = feedback + answerText;
                        }
                    }
                    else
                        repl = answerText;
                    answer = answer.Substring(0,matcher.Groups[1].Index + indexcorrection) + repl + answer.Substring(matcher.Groups[1].Index + matcher.Groups[1].Length + indexcorrection);
                    indexcorrection += repl.Length - matcher.Groups[1].Length;
                }
                matcher = matcher.NextMatch();
            }
            
            httpSession.addNameValuePair("answer", answer);
            for (int i = 0; i < answerWeights.Count; i++)
                //Index base 0 on form. (weighting[x])
                httpSession.addNameValuePair("weighting["+ (i) +"]", answerWeights[i].ToString());
        }


        /// <summary>
        ///     Filters the weight and feedback from a gaps answer, and returns the answer in one of three formats:
        ///     "anwers", "anwer1|anwer2|anwer3" or "{regex}regex statement{/regex}"
        /// </summary>
        /// <param name="gapText">The text taken from inside the brackets.</param>
        /// <param name="answerWeights">The list to add answer weights to.</param>
        /// <param name="feedbacks">The list to add answer feedbacks to.</param>
        /// <param name="trimwhitespace">Determines whether whitespace is trimmed around the answers.</param>
        /// <returns></returns>
        private String filterWeightAndFeedbackFromGapsTag(String gapText, ref List<int> answerWeights, ref List<String> feedbacks)
        {
            const String regexStartRegex = "{regex(=[0-9]+)?}";
            const String regexEnd = "{/regex}";
            const String regexContainsRegex = regexStartRegex + ".*?" + regexEnd;
            const String regexStartRegexOr = @"\|{regex(=[0-9]+)?}";
            const string feedbackSeparator = "||";

            Regex regexStart = new Regex(regexStartRegex);
            Regex regexStartOr = new Regex(regexStartRegexOr);
            Regex regexContains = new Regex(regexContainsRegex);

            gapText = filterWeightFromGapsTag(gapText, ref answerWeights);
            gapText = gapText.TrimEnd('|').TrimEnd();
            Match matcher = regexContains.Match(gapText);
            int regexPos = matcher.Index;
            int feedbackIndex = gapText.IndexOf(feedbackSeparator);
            int feedbackSepEnd = feedbackIndex + feedbackSeparator.Length;

            if (feedbackIndex != -1 && feedbackIndex == gapText.IndexOf('|'))
            {
                if (matcher.Success && regexPos < feedbackIndex)
                {
                    feedbackIndex = regexPos;
                    feedbackSepEnd = regexPos;
                }
                feedbacks.Add(gapText.Substring(0, feedbackIndex));
                return gapText.Substring(feedbackSepEnd);
            }
            else if (matcher.Success && regexPos > 0 && gapText[regexPos - 1] != '|')
            {
                feedbacks.Add(gapText.Substring(0, regexPos));
                return gapText.Substring(regexPos);
            }
            else if (gapText.Contains('|'))
            {
                String[] gappossibilities;
                if (!regexContains.IsMatch(gapText))
                {
                    gappossibilities = gapText.Split('|');
                }
                else
                {
                    int index = 0;
                    List<String> gapposlist = new List<String>();
                    while (index != -1)
                    {
                        String curText = gapText.Substring(index);
                        int indexRegEnd = curText.IndexOf(regexEnd);
                        int indexSep = curText.IndexOf("|");
                        int indexEnd;
                        if (regexStart.IsMatch(curText) && indexRegEnd != -1)
                            indexEnd = indexRegEnd;
                        else
                            indexEnd = indexSep;

                        gapposlist.Add(curText.Substring(0, indexEnd));
                        index = indexEnd;
                    }
                    gappossibilities = gapposlist.ToArray();
                }

                String feedback = String.Empty;
                foreach (String str in gappossibilities)
                    feedback += " / " + str;
                if (feedback.Length > 3) feedback = feedback.Substring(3);
                feedbacks.Add(feedback);
                return gapText;
            }
            else
            {
                feedbacks.Add(String.Empty);
                return gapText;
            }
        }


        /// <summary>
        ///     Filters the score out of the internal text from a gaps tag, and returns the trimmed string.
        ///     The score is added to the provided integers list.
        /// </summary>
        /// <param name="gapsTag">The string inside a gaps exercise tag</param>
        /// <param name="answerWeights">The list to add the weight to.</param>
        /// <returns>The input string, with the weight trimmed off</returns>
        private String filterWeightFromGapsTag(String gapsTag, ref List<int> answerWeights)
        {
            String weightRegex = @"(?: |\|\|)([+-][0-9]+)$";
            Regex weight = new Regex(weightRegex);
            Match match = weight.Match(gapsTag);
            int answerWeight = 0;
            if (!match.Success)
            {
                answerWeight = DomainController.Instance().getSettings().getDefaultScoreGaps();
            }
            else
            {
                String score = match.Groups[1].Value;
                answerWeight = Convert.ToInt16(score);
                gapsTag = gapsTag.Substring(0, match.Index);
            }
            if (answerWeights != null)
                answerWeights.Add(answerWeight);
            return gapsTag;
        }

        /// <summary>
        ///     Adds the matching answers to a question.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="httpSession">The working session.</param>
        private void addAnswersMatching(HttpSession httpSession, Question question)
        {
            // list of match "questions" - the statements to be matched with something
            List<Answer> matchQuestions = new List<Answer>(question.getAnswersAsList());
            // list of all possible match "answers" - the possible statements to match with
            List<String> matchAnswers = new List<String>();
                        
            // cut off at answer 'Z'
            if (matchQuestions.Count > 26)
                matchQuestions.RemoveRange(26, matchQuestions.Count-26);
            
            Answer[] forlooplist = new Answer[Math.Min(matchQuestions.Count,26)];
            matchQuestions.CopyTo(0,forlooplist,0,Math.Min(matchQuestions.Count,26));
            
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
                if (answ.getAnswer().Equals(String.Empty))
                    matchQuestions.Remove(answ);
            }

            // Add name-value pairs for match "questions" to session
            for (int i = 0; i < matchQuestions.Count; i++)
            {
                httpSession.addNameValuePair("answer["+ (i+1) +"]", Utility.stripHTMLParagraphs(matchQuestions[i].getAnswer()));
                
                int matchResult = 0;
                for (int match = 0; match < matchAnswers.Count; match++)
                {
                    if (matchQuestions[i].getMatchAnswer().Equals(matchAnswers[match]))
                    {
                        matchResult= match+1;
                        break;
                    }
                }
                httpSession.addNameValuePair("matches["+ (i+1) +"]", matchResult + "");
                httpSession.addNameValuePair("weighting["+ (i+1) +"]", matchQuestions[i].getWeight().ToString());
            }

            // Add name-value pairs for match "answers" to session
            for (int i = 0; i < matchAnswers.Count; i++)
            {
                httpSession.addNameValuePair("option["+ (i+1) +"]", Utility.stripHTMLParagraphs(matchAnswers[i]));
            }
            httpSession.addNameValuePair("nb_options", matchAnswers.Count.ToString());
            httpSession.addNameValuePair("nb_matches", matchQuestions.Count.ToString());
        }

        /// <summary>
        ///     Updates the basic information of an existing exercise on the platform.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <param name="exercise">The exercise object.</param>
        /// <param name="courseCode">The course.</param>
        /// <param name="httpSession">The working session.</param>
        private void updateExerciseInfo(HttpSession httpSession, int exerciseID, Exercise exercise, Course course, int randomQuestions)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/exercice/exercise_admin.php?cidReq=" + course.getCourseId()
                                                + "&modifyExercise=yes"
                                                + "&exerciseId=" + exerciseID
                                     );
            httpSession.addNameValuePair("exerciseTitle", exercise.getName());
            httpSession.addNameValuePair("exerciseDescription", exercise.getDescription());
            //1 for one page, 2 for multiple pages. This is the Boolean "multipage" + 1
            httpSession.addNameValuePair("exerciseType", (Convert.ToInt32(exercise.isMultipage()) + 1).ToString());
            httpSession.addNameValuePair("randomQuestions", Convert.ToString(randomQuestions));
            httpSession.addNameValuePair("submitExercise", "OK");
            httpSession.addNameValuePair("_qf__exercise_admin", "");
            httpSession.addNameValuePair("edit", "true");
            httpSession.sendPOSTrequestFromForm();
        }

        /// <summary>
        ///     Searches an exercise on the platform.
        /// </summary>
        /// <param name="exerciseName">The exercise name.</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <returns>The exercise ID.</returns>
        private int findExerciseOnPlatform(HttpSession httpSession, String exerciseName, Course course)
        {
            List<ExerciseResult> exercises = findExercisesOnPlatform(httpSession, course);
            ExerciseResult exercise;
            try
            {
                exercise = exercises.Find(delegate(ExerciseResult ex) { return ex.exerciseTitle.Equals(exerciseName); });
                return exercise.exerciseID;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Searches for exercises on the platform.
        /// </summary>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <returns>A list of exercises</returns>
        private List<ExerciseResult> findExercisesOnPlatform(HttpSession httpSession, Course course)
        {
            // This function uses ExerciseResult objects from the score/statistics namespace, because
            // that class can store the exercise ID.
            List<ExerciseResult> exercises = new List<ExerciseResult>();
            int excount = -1;
            int pageNumber = 0;
            while (excount < exercises.Count())
            {
                excount = exercises.Count();
                XmlDocument xmlExercisesPage = getExercisesPage(httpSession, pageNumber, course);
                SingleNodeScanner evenScanner = new SingleNodeScanner("tr",true,false);
                evenScanner.addNameValuePair("class","row_even");
                evenScanner.doScan(xmlExercisesPage.ChildNodes);
                SingleNodeScanner oddScanner = new SingleNodeScanner("tr", true, false);
                oddScanner.addNameValuePair("class","row_odd");
                oddScanner.doScan(xmlExercisesPage.ChildNodes);
                
                getExercisesFromPageScan(ref exercises, evenScanner.getNodes());
                getExercisesFromPageScan(ref exercises, oddScanner.getNodes());
                pageNumber++;
            }
            exercises.Sort(delegate(ExerciseResult ex1, ExerciseResult ex2) { return ex1.exerciseID.CompareTo(ex2.exerciseID); });
            return exercises;
        }

        /// <summary>
        ///     Extracts an exercise from a downloaded page.
        /// </summary>
        /// <param name="exercisesList">The exercise result list.</param>
        /// <param name="nodesList">The node list.</param>
        private void getExercisesFromPageScan(ref List<ExerciseResult> exercisesList, List<XmlNode> nodesList)
        {
            foreach (XmlNode n in nodesList)
            {
                XmlNode contentNode = null;
                try
                {
                    contentNode = n.SelectSingleNode("td/table/tr/td/a");
                }
                catch { }
                if (contentNode != null && contentNode.Attributes["href"] != null && contentNode.Attributes["href"].Value.Contains("exerciseId"))
                {
                    int exId = -1;
                    try
                    {
                        exId = Convert.ToInt32(Utility.findValueInURL(contentNode.Attributes["href"].Value, "exerciseId"));
                    }
                    catch { }
                    if (exId != -1)
                    {
                        exercisesList.Add(new ExerciseResult(exId, contentNode.InnerXml, null, 0, 0));
                    }
                }
            }
        }

        /// <summary>
        ///     Returns a valid XML document, based on the downloaded page.
        /// </summary>
        /// <param name="pagenumber">The page number.</param>
        /// <param name="courseCode">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <returns>A valid XML document.</returns>
        private XmlDocument getExercisesPage(HttpSession httpSession, int pagenumber, Course course)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/exercice/exercice.php?cidReq=" + course.getCourseId()
                                                + "&page=" + pagenumber );
            httpSession.sendPOSTrequestSimple();
            return SGMLReaderHelper.htmlToXmlDocument(httpSession.getResponseFromServer());
        }

        /// <summary>
        ///     Returns all question ID's from an exercise.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <returns>A list with all question ID's.</returns>
        private List<int> getQuestionIDsFromExercise(HttpSession httpSession, int exerciseID, Course course)
        {
            List<int> questionIDList = new List<int>();

            String dokeosUrl = this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/exercice/admin.php?cidReq=" + course.getCourseId()
                                                + "&exerciseId=" + exerciseID );
            httpSession.sendPOSTrequestSimple();
            XmlDocument xmlExercisesPage = SGMLReaderHelper.htmlToXmlDocument(httpSession.getResponseFromServer());
            SingleNodeScanner evenScanner = new SingleNodeScanner("tr", true, false);
            evenScanner.addNameValuePair("class","row_even");
            evenScanner.doScan(xmlExercisesPage.ChildNodes);
            SingleNodeScanner oddScanner = new SingleNodeScanner("tr", true, false);
            oddScanner.addNameValuePair("class","row_odd");
            oddScanner.doScan(xmlExercisesPage.ChildNodes);
            getQuestionsFromPageScan(ref questionIDList, evenScanner.getNodes());
            getQuestionsFromPageScan(ref questionIDList, oddScanner.getNodes());
            return questionIDList;
        }

        /// <summary>
        ///     Gets the questions from a page scan.
        /// </summary>
        /// <param name="questionIDList">The list with question ID's</param>
        /// <param name="nodesList">The nodes.</param>
        private void getQuestionsFromPageScan(ref List<int> questionIDList, List<XmlNode> nodesList)
        {
            foreach (XmlNode n in nodesList)
            {
                XmlNode contentNode = null;
                try
                {
                    contentNode = n.SelectSingleNode("td/a");
                }
                catch { }
                if (contentNode != null && contentNode.Attributes["href"] != null && contentNode.Attributes["href"].Value.Contains("questionId"))
                {
                    int quesId = -1;
                    try
                    {
                        quesId = Convert.ToInt32(Utility.findValueInURL(contentNode.Attributes["href"].Value, "questionId"));
                    }
                    catch { }
                    if (quesId != -1)
                    {
                        questionIDList.Add(quesId);
                    }
                }
            }
        }
        
        /// <summary>
        ///     Deletes the images of an exercise.
        /// </summary>
        /// <param name="exerciseID">The exercide ID.</param>
        /// <param name="filenamePrefix">Extra prefix of the filename. Leave null or empty for none</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        private void deleteImagesForExercise(HttpSession httpSession, String filenamePrefix, int exerciseID, Course course)
        {
            String startString = '/' + UPLOAD_FOLDER + '/' + exerciseID.ToString() + '_';
            if (filenamePrefix != null && !filenamePrefix.Equals(String.Empty))
                startString += filenamePrefix + '_';
            String dokeosUrl=this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                    + "&curdirpath=" + System.Web.HttpUtility.UrlEncode('/' + UPLOAD_FOLDER)
                                    + "&tablename_per_page=1000000");
            httpSession.sendPOSTrequestSimple();
            XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(httpSession.getResponseFromServer());
            SingleNodeScanner fileScanner = new SingleNodeScanner("input", true, false);
            fileScanner.addNameValuePair("name","path[]");
            fileScanner.addNameValuePair("type","checkbox");
            fileScanner.addNameValuePair("value",null);
            fileScanner.doScan(xmlFoldersPage.ChildNodes);
            foreach (XmlNode node in fileScanner.getNodes())
            {
                if (node.Attributes["value"] != null && node.Attributes["value"].Value.StartsWith(startString))
                {
                    httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                                        + "&curdirpath=/" + UPLOAD_FOLDER
                                                        + "&delete=" + node.Attributes["value"].Value);
                    httpSession.sendPOSTrequestSimple();
                }
            }
        }

        /// <summary>
        ///     Uploads the images from an exercise, and changes the exercise content to contain platform image URLs.
        /// </summary>
        /// <param name="exercise">The exercise.</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        private void uploadExerciseImages(HttpSession httpSession, Exercise exercise, int exerciseID, Course course, int exerciseNumber)
        {
            deleteImagesForExercise(httpSession, null, exerciseID, course);
            
            // separate images for exercise info
            Dictionary<String, String> uploadedFiles_desc = new Dictionary<String,String>();

            Dictionary<String, String> uploadedFiles = new Dictionary<String, String>();
            //exercise.setName(this.uploadImagesFromDescription(httpSession, exercise.getName(), EXINFO_PREFIX, course, exerciseID, uploadedFiles_desc, exerciseNumber));
            exercise.setDescription(this.uploadImagesFromDescription(httpSession, exercise.getDescription(), null, course, exerciseID, uploadedFiles_desc, exerciseNumber));
            List<Question> questions = exercise.getQuestionsAsList();
            foreach (Question question in questions)
            {
                //question.setQuestionTitle(this.uploadImagesFromDescription(httpSession, question.getQuestionTitle(), course, exerciseID, uploadedFiles, exerciseNumber));
                question.setQuestionText(this.uploadImagesFromDescription(httpSession, question.getQuestionText(), null, course, exerciseID, uploadedFiles, exerciseNumber));
                List<Answer> answers = question.getAnswersAsList();
                foreach (Answer answer in answers)
                {
                    answer.setAnswer(this.uploadImagesFromDescription(httpSession, answer.getAnswer(), null, course, exerciseID, uploadedFiles, exerciseNumber));
                    answer.setMatchAnswer(this.uploadImagesFromDescription(httpSession, answer.getMatchAnswer(), null, course, exerciseID, uploadedFiles, exerciseNumber));
                    answer.setFeedback(this.uploadImagesFromDescription(httpSession, answer.getFeedback(), null, course, exerciseID, uploadedFiles, exerciseNumber));
                }
            }
            this.setFileVisibility(httpSession, course, '/' + UPLOAD_FOLDER, String.Empty, false);
        }

        /// <summary>
        ///     Uploads the images from a string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="filenamePrefix">Extra prefix of the filename. Leave null or empty for none</param>
        /// <param name="course">The course.</param>
        /// <param name="httpSession">The working session.</param>
        /// <returns>The new img-tag.</returns>
        /// <param name="exerciseID"></param>
        /// <param name="uploadedList">Reference list for uploaded images</param>
        /// <param name="exerciseNumber"></param>
        private String uploadImagesFromDescription(HttpSession httpSession, String input, String filenamePrefix, Course course, int exerciseID, Dictionary<String, String> uploadedList, int exerciseNumber)
        {
            if(input == null) 
                return null;

            Regex r = new Regex(@"<img(.*?)>\s*", RegexOptions.IgnoreCase);
            Match matcher = r.Match(input);
            while (matcher.Success)
            {
                String orig_link = matcher.Groups[0].Value;
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value,"src=\"(.*?)\"");

                String uploadpath = DomainController.Instance().getTempPath() + urlmatcher.Groups[1].Value.Replace('/', Path.DirectorySeparatorChar);
                String new_link;
                if (!uploadedList.ContainsKey(uploadpath))
                {
                    if(uploadedList.Count == 0) DomainController.Instance().writeToLog("uploading_images_exercise_x_course_y",new String[]{exerciseNumber.ToString(),course.getCourseId()}, true, false, false);
                    FileInfo uploadFile = new FileInfo(uploadpath);
                    long randvalue = Math.Abs(DateTime.Now.ToBinary());
                    Random random = new Random((Int32)randvalue);
                    // make sure random value is actually random so students can't see the intended order from the filename
                    randvalue += random.Next(Int32.MaxValue / 2, Int32.MaxValue) * 10000000; //(push into the seconds range)

                    // filename on platform: exercise ID + filenameprefix +  obfuscated datetime as unique string + original extension
                    String uploadFileName = exerciseID.ToString() + "_" 
                        + (filenamePrefix == null || filenamePrefix.Equals(String.Empty)? String.Empty : filenamePrefix + "_")
                        + randvalue.ToString() + "_" + uploadFile.Extension;
                    
                    // uploaded file is not set invisible: it ends up in an invisible folder anyway.
                    new_link = uploadSingleFile(httpSession, uploadFile.FullName, uploadFileName, AUTO_UPLOAD_STRING, course, UPLOAD_FOLDER, true, true, false);
                    if (!new_link.Equals(String.Empty))
                        //new_link = "<img src=\"" + new_link + "\" />";
                        new_link = orig_link.Replace(urlmatcher.Groups[0].Value, "src=\"" + new_link + "\"").Trim();
                    uploadedList.Add(uploadpath,new_link);
                }
                else 
                {
                    uploadedList.TryGetValue(uploadpath,out new_link);
                }
                input = input.Replace(orig_link, new_link);
                matcher = matcher.NextMatch();
            }

            return input;
        }

        /// <summary>
        ///     Makes a folder invisible
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="course">The course.</param>
        /// <param name="path">The path of the file or folder.</param>
        /// <param name="name">The name of the file or folder.</param>
        /// <param name="visible">the new visibility status for the file</param>
        private Boolean setFileVisibility(HttpSession httpSession, Course course, String path, String name, Boolean visible)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            int fileId = getFileID(httpSession, course, path, name);
            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                            + "&curdirpath=" + System.Web.HttpUtility.UrlEncode(path)
                                            + "&set_" + (visible ? String.Empty : "in") + "visible=" + fileId);
            httpSession.sendPOSTrequestSimple();
            if (httpSession.getResponseFromServer().Contains("class=\"confirmation-message\""))
                return true;
            else return false;
        }

        /// <summary>
        ///     Updates the file information of a file on the platform
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="course">The course.</param>
        /// <param name="path">The path of the file or folder.</param>
        /// <param name="name">The name of the file or folder.</param>
        /// <param name="description">the new description for the file</param>
        private Boolean updateFileDescription(HttpSession httpSession, Course course, String path, String name, String description)
        {
            String dokeosUrl = this.login.getPlatformUrl();
            
            // get original filename title
            String filename = System.Web.HttpUtility.UrlEncode(path.TrimEnd('/') + '/' + name);
            String pathname = System.Web.HttpUtility.UrlEncode(path.TrimEnd('/'));
            httpSession.setRequestUrl(dokeosUrl + "/main/document/edit_document.php?cidReq=" + course.getCourseId()
                                            + "&curdirpath=" + pathname
                                            + "&file=" + filename);
            httpSession.sendPOSTrequestSimple();
            
            String form_newTitle = String.Empty;
            String form_filename = String.Empty;
            String form_extension = String.Empty;
            String form_file_path = String.Empty;
            String form_commentPath = String.Empty;

            String response = httpSession.getResponseFromServer();

            SingleNodeScanner scanner = new SingleNodeScanner("form");
            scanner.addNameValuePair("name", "formEdit");
            scanner.doScan(SGMLReaderHelper.htmlToXmlDocument(response).ChildNodes);
            XmlNode mainNode = scanner.getNode();
            if (mainNode == null)
                return false;

            /*
                <input name="newTitle" type="text" value="Handleiding_voor_eindgebruikers_00.pdf" />
		         <textarea rows="3" style="width:300px;" name="newComment">ee</textarea>
                <input name="filename" type="hidden" value="Handleiding_voor_eindgebruikers_00" />
                <input name="extension" type="hidden" value="pdf" />
                <input name="file_path" type="hidden" value="/handleiding/Handleiding_voor_eindgebruikers_00.pdf" />
                <input name="commentPath" type="hidden" value="/handleiding/Handleiding_voor_eindgebruikers_00.pdf" />
            */

            scanner = new SingleNodeScanner("input");
            scanner.addNameValuePair("name", "newTitle");
            scanner.doScan(mainNode.ChildNodes);
            XmlNode node = scanner.getNode();
            if (node != null && node.Attributes["value"] != null)
                form_newTitle = node.Attributes["value"].Value;
            else return false;

            scanner = new SingleNodeScanner("input");
            scanner.addNameValuePair("name", "filename");
            scanner.doScan(mainNode.ChildNodes);
            node = scanner.getNode();
            if (node != null && node.Attributes["value"] != null)
                form_filename = node.Attributes["value"].Value;
            else return false;

            scanner = new SingleNodeScanner("input");
            scanner.addNameValuePair("name", "extension");
            scanner.doScan(mainNode.ChildNodes);
            node = scanner.getNode();
            if (node != null && node.Attributes["value"] != null)
                form_extension = node.Attributes["value"].Value;
            else return false;

            scanner = new SingleNodeScanner("input");
            scanner.addNameValuePair("name", "file_path");
            scanner.doScan(mainNode.ChildNodes);
            node = scanner.getNode();
            if (node != null && node.Attributes["value"] != null)
                form_file_path = node.Attributes["value"].Value;
            else return false;

            scanner = new SingleNodeScanner("input");
            scanner.addNameValuePair("name", "commentPath");
            scanner.doScan(mainNode.ChildNodes);
            node = scanner.getNode();
            if (node != null && node.Attributes["value"] != null)
                form_commentPath = node.Attributes["value"].Value;
            else return false;

            // Send form with new description
            httpSession.setRequestUrl(dokeosUrl + "/main/document/edit_document.php?cidReq=" + course.getCourseId()
                                            + "&curdirpath=" + pathname
                                            + "file=" + filename);

            httpSession.addNameValuePair("newTitle", form_newTitle);
            httpSession.addNameValuePair("newComment", description);
            httpSession.addNameValuePair("filename", form_filename);
            httpSession.addNameValuePair("extension", form_extension);
            httpSession.addNameValuePair("file_path", form_file_path);
            httpSession.addNameValuePair("commentPath", form_commentPath);
            
            httpSession.addNameValuePair("_qf__formEdit", "");
            httpSession.addNameValuePair("showedit", String.Empty);
            httpSession.addNameValuePair("formSent", "1");
            httpSession.addNameValuePair("submit", "OK");

            httpSession.sendPOSTrequestFromForm();

            response = httpSession.getResponseFromServer();
            if (response.Contains("class=\"normal-message\"")
                && !response.Contains("class=\"error-message\""))
                return true;
            else return false;
        }

        /// <summary>
        ///     Returns the internal ID of a file or folder from the document page.
        ///     This ID is used to adjust the visibility of the folder.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <param name="course">The course.</param>
        /// <param name="path">The path of the file or folder.</param>
        /// <param name="name">The name of the file.</param>
        /// <returns>The ID.</returns>
        private int getFileID(HttpSession httpSession, Course course, String path, String name)
        {
            String dokeosUrl=this.login.getPlatformUrl();
            httpSession.setRequestUrl(dokeosUrl + "/main/document/document.php?cidReq=" + course.getCourseId()
                                    + "&curdirpath=" + System.Web.HttpUtility.UrlEncode('/' + path.Trim('/'))
                                    + "&tablename_per_page=1000000");

            httpSession.sendPOSTrequestSimple();
            String page = httpSession.getResponseFromServer();
            XmlDocument xmlFoldersPage = SGMLReaderHelper.htmlToXmlDocument(page);
            
            FileIDScanner idScanner = new FileIDScanner('/' + (path.Trim('/') + '/' + name).Trim('/'));
            idScanner.doScan(xmlFoldersPage.ChildNodes);

            return idScanner.getId();
        }

        /// <summary>
        ///     Login the user.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        /// <returns>True if the login succeeds.</returns>
        private Boolean doLogin(HttpSession httpSession)
        {
            httpSession.setRequestUrl(this.login.getPlatformUrl());
            httpSession.addNameValuePair("login", login.getUsername());
            httpSession.addNameValuePair("password", login.getPassword());
            httpSession.addNameValuePair("submitAuth", "OK");
            httpSession.addNameValuePair("_qf__formLogin", String.Empty);
            httpSession.sendPOSTrequestFromForm();
            String page = httpSession.getResponseFromServer();
            if (page.Contains("(" + this.login.getUsername() + ")"))
                return true;
            else
                return false;
        }

        /// <summary>
        ///     Logout the user.
        /// </summary>
        /// <param name="httpSession">The working session.</param>
        private void doLogout(HttpSession httpSession)
        {
            httpSession.setRequestUrl(this.login.getPlatformUrl() + LOGOUT);
        }

        #endregion

    }
}
