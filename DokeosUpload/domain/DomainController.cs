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
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using lmsda.domain.exercise;
using lmsda.domain.score;
using lmsda.domain.score.statistics;
using lmsda.domain.ui;
using lmsda.domain.user;
using lmsda.domain.util;
using lmsda.persistence.document;
using lmsda.persistence.logger;
using lmsda.persistence.platform;
using lmsda.persistence.platform.service;
using lmsda.persistence.resource;
using lmsda.persistence.settings;
using lmsda.domain.user.synchronization;
using lmsda.persistence.download;
using System.Text.RegularExpressions;
using lmsda.persistence.httpcommunication;
using lmsda.domain.score.data;

namespace lmsda.domain
{
    /// <summary>
    ///     Auteur: Gianni Van Hoecke
    ///             Maarten Meuris          
    ///     This class offers all methods needed by the program. Through the singleton pattern, these methods can be called from anywhere in the program.
    /// </summary>
    class DomainController
    {
        private static DomainController _instance;
        private static object syncLock = new object();

        private List<UI> observers;

        private ResourceLoader resourceLoader;
        private Settings settings;
        private SupportedExercisesDocument document;
        private UserInfo userInfo;
        private TargetPlatform platform;
        private FileForUpload fileForUpload;
        private SynchronizationOperations synchronizationOperations;
        List<int[]> exerciseSimpleErrorList;

        private StringBuilder loggerSession;

        private Boolean firstRun;
        private String resourcePath;
        private String tempPath;
        public DocumentFolder currentFolder { get; set; }
        public Boolean isSynchronization { get; set; }

        public string latestVersion; //As of 1.08

        #region Constructor and singleton pattern

        /// <summary>
        ///     Default constructor.
        ///     Loads the settings and the language.
        /// </summary>
        protected DomainController()
        {
            // WARNING! This method can NOT contain any methods that use the Singleton pattern
            // to call Domaincontroller methods!
            firstRun = false;
            // can't be stand alone, since stand alone is only triggered by the setting
            // in the settings file.
            if (!Utility.doesFileExist(ProgramConstants.getSettingsPath()))
            {
                firstRun = true;
                String oldsettingspath = ProgramConstants.getProgramPath() + ProgramConstants.SETTINGS_FILE_NAME;
                if (Utility.doesFileExist(oldsettingspath))
                {
                    // read the settings from the 'old' config file
                    this.settings = new Settings(oldsettingspath, false);
                    // save settings in their new location
                    this.settings.setPath(ProgramConstants.getSettingsPath());
                }
                else
                {
                    this.settings = new Settings(ProgramConstants.getSettingsPath(), true);
                }
            }
            else
            {
                this.settings = new Settings(ProgramConstants.getSettingsPath(), true);
            }
            if (this.settings.getSubjectsFolder().Equals(String.Empty) || this.settings.getStatsFolder().Equals(String.Empty))
            {
                firstRun = true;
                if (!PortableIdentifier.Instance().isPortable)
                {
                    this.settings.setSubjectsFolder(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                    this.settings.setStatsFolder(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                }
                else
                {
                    this.settings.setSubjectsFolder("\\");
                    this.settings.setStatsFolder("\\");
                }
            }

            this.userInfo = new UserInfo(settings.getCoursecode());
            this.observers = new List<UI>();
            this.isSynchronization = false;

            this.resourcePath = ProgramConstants.getResourcePath();
            this.tempPath = ProgramConstants.getTempPath();

            //False : language choice shouldn't be written back to the settings when loading it the first time.
            this.selectLanguage(settings.getLanguage(), false);

            Logger.emptyLogIfSizeExceeded();
            this.writeToLog("starting_and_loading", true, true);
        }

        /// <summary>
        ///     Singleton Pattern.
        ///     Double Checked Locking Pattern. (support for multithreaded programs.)
        ///     Returns the object of this class.
        /// </summary>
        /// <returns>The object of the DomainController class.</returns>
        public static DomainController Instance()
        {
            if (_instance == null)
            {
                lock (syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new DomainController();
                    }
                }
            }
            return _instance;
        }

        /// <summary>
        ///     Allows conditional use of the singleton pattern (only use it if it's already there)
        /// </summary>
        /// <returns>whether Domaincontroller has been initialized.</returns>
        public static Boolean hasInstance()
        {
            return (_instance != null);
        }

        #endregion

        #region All methods which use HTTP sessions

        /// <summary>
        ///     Logs a user in by trying a login of the user on the configured platform.
        ///     The system doesn't maintain the login on the platform, but instead stores the (validated) login
        ///     information so a login can be performed when the user actually performs actions on the platform.
        /// </summary>
        /// <param name="username">The user name.</param>
        /// <param name="password">The password.</param>
        /// <returns>True if the login succeeded.</returns>
        public Boolean validateUser(String username, String password)
        {
            this.clearStatus();
            Login login = new Login(settings.getUrl());
            login.setUsername(username);
            login.setPassword(password);
            String platform = this.settings.getPlatform();
            Service service = (Service)Enum.Parse(typeof(Service), this.getSettings().getService());
            this.platform = TargetPlatforms.factory(platform, service, login);
            Boolean isLoggedin = this.platform.tryLogin();
            if (!isLoggedin)
            {
                this.logOut();
                this.writeToLog(this.getLanguageString("error") + ": " + this.getLanguageString("cannot_login"));
            }
            else
            {
                this.writeToLog("logged_in_as_x_on_y", new String[] { username, this.platform.getPlatformName() }, true, false, false);
                this.userInfo.setLogin(login);
                this.settings.setLoginname(username);
                this.loadCourses();
                this.userInfo.getSubjects().linkSubjectsToCourses(login);
                this.userInfo.reselectSavedCourse();
                this.loadDocumentFolders(false);
                this.downloadGroups();
                this.writeToLog("logged_in_as_x_on_y", new String[] { username, this.platform.getPlatformName() }, true, false, false);
            }
            return isLoggedin;
        }

        /// <summary>
        ///     Checks if the user is logged in.
        /// </summary>
        /// <returns>True if the user is logged in.</returns>
        public Boolean isLoggedIn()
        {
            return this.userInfo.getLogin() != null;
        }

        /// <summary>
        ///     Logs the user out.
        /// </summary>
        public void logOut()
        {
            this.clearStatus();
            this.userInfo.logOut();
            this.platform = null;
        }

        /// <summary>
        ///     Fetches all available courses of which the user is 'Teacher', and the saved categories.
        /// </summary>
        public void loadCourses()
        {
            if (platform != null && isLoggedIn())
            {
                List<Course> courses = this.platform.readCourses();
                this.userInfo.getLogin().setCourses(courses);
            }
            else
            {
                if (this.userInfo.getLogin() != null)
                    this.userInfo.getLogin().clearCourses();
            }
        }

        /// <summary>
        ///     Reads all document folders of the selected course from the platform, and stores them in the local documentFoldersList object.
        /// </summary>
        public void loadDocumentFolders(Boolean lazy)
        {
            List<Course> courses = null;
            if (this.isLoggedIn())
                courses = this.userInfo.getSelectedItemCourses();
            if (courses != null)
                platform.readDocumentFolders(ref courses, lazy);
        }


        /// <summary>
        ///     Uploads a file to the chosen document folder.
        /// </summary>
        /// <param name="filename">The full path of the document to upload.</param>
        /// <param name="documentFolders">Target destination on the server.</param>
        /// <param name="description">Description for the file on the platform.</param>
        /// <param name="setInvisible">Set file invisible on the platform.</param> 
        public String[] uploadFile(String filename, DocumentFolder documentFolders, String description, Boolean setInvisible)
        {
            return uploadFile(filename, documentFolders, description, setInvisible, true);
        }    
        /// <summary>
        ///     Uploads a file to the chosen document folder.
        /// </summary>
        /// <param name="filename">The full path of the document to upload.</param>
        /// <param name="documentFolders">Target destination on the server.</param>
        /// <param name="description">Description for the file on the platform.</param>
        /// <param name="setInvisible">Set file invisible on the platform.</param> 
        public String[] uploadFile(String filename, DocumentFolder documentFolders, String description, Boolean setInvisible, Boolean showMessageBox)
        {
            this.writeToLog("uploading_file", new String[] { Path.GetFileName(filename) }, true, false, false);
            this.fileForUpload = new FileForUpload(filename);
            String[] retValue = this.platform.doUploadFile(filename, null, description, documentFolders, userInfo.getSelectedSubject(), setInvisible);
            if (retValue.Length > 0)
                this.writeToLog("upload_completed", true, false, showMessageBox && !this.isSynchronization);
            else
                this.writeToLog("upload_failed", true, false, showMessageBox && !this.isSynchronization);
            this.fileForUpload = null;
            return retValue;
        }

        /// <summary>
        ///     Updates the information of a file on the platform.
        /// </summary>
        /// <param name="filename">The name of the file on the platform. NOT the full path.</param>
        /// <param name="path">Path of the file on the platform.</param>
        /// <param name="description">New description for the file on the platform.</param>
        /// <param name="visible">The new visibility status of the file on the platform.</param> 
        /// <returns>True if the update succeeded for all courses.</returns>
        public Boolean updateFile(String filename, DocumentFolder path, String description, Boolean visible)
        {
            this.writeToLog("updating_file", new String[] { Path.GetFileName(filename) }, true, false, !this.isSynchronization);
            Boolean updated = this.platform.doUpdateFile(filename, path, userInfo.getSelectedSubject(), description, visible);
            if (updated)
                this.writeToLog("update_completed", true, false, !this.isSynchronization);
            else
                this.writeToLog("update_failed", true, false, !this.isSynchronization);
            return updated;
        }

        /// <summary>
        ///     Creates a new folder on the server.
        /// </summary>
        /// <param name="publishDestination">The path on the server in which the folder has to be created.</param>
        /// <param name="newFolderName">The name of the folder.</param>
        /// <param name="setInvisible">Set folder invisible after creating it.</param>
        public DocumentFolder createNewDocumentsFolder(DocumentFolder publishDestination, String newFolderName, Boolean setInvisible)
        {
            return this.platform.doCreateFolderInDocuments(publishDestination, newFolderName, setInvisible);
        }

        #endregion

        #region MVC related methods

        /// <summary>
        ///     Adds an object (an observer) to the list of Observers.
        /// </summary>
        /// <param name="containerFrame">The observable to add.</param>
        public void addObserver(UI observer)
        {
            this.observers.Add(observer);
            this.fireLogUpdate();
        }

        /// <summary>
        ///     Removes an observer.
        /// </summary>
        /// <param name="observer">The observable to remove.</param>
        public void removeObserver(UI observer)
        {
            this.observers.Remove(observer);
        }

        /// <summary>
        ///     Updates the log on all main observers.
        /// </summary>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireLogUpdate()
        {
            foreach (UI observer in this.observers)
                if(observer is MainUI)
                    ((MainUI)observer).updateLog(Logger.getLastEvent());
        }


        /// <summary>
        ///     Updates the log on all main observers.
        /// </summary>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void clearStatus()
        {
            foreach (UI observer in this.observers)
                if (observer is MainUI)
                    ((MainUI)observer).updateLog(String.Empty);
        }

        /// <summary>
        ///     Shows a message on all observers.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireMessageBox(String message, MessageBoxIcon icon)
        {
            foreach (UI observer in this.observers)
            {
                observer.showMessage(message, icon);
                break; //A message BOX will only be showed once.
            }
        }

        /// <summary>
        ///     Asks a yes/no question on all main observers.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <returns>False if any of the observers answered negative</returns>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public Boolean fireMessageBoxQuestion(String message, Boolean okcancel)
        {
            Boolean retn = true;
            foreach (UI observer in this.observers)
                if(observer is MainUI)
                    if (!((MainUI)observer).showQuestionMessage(message, okcancel))
                        retn = false;
            return retn;
        }

        /// <summary>
        ///     Shows text on all main observers.
        /// </summary>
        /// <param name="text">The text to show.</param>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireDumpOnUI(String text)
        {
            foreach (UI observer in this.observers)
                if (observer is MainUI)
                    ((MainUI)observer).dumpText(text);
        }

        /// <summary>
        ///     Performs the update method on the UIs.
        /// </summary>
        public void fireUpdate(Boolean refreshUI)
        {
            foreach (UI observer in this.observers)
                observer.update(refreshUI);
        }

        /// <summary>
        ///     Updates a progress bar on the UIs.
        /// </summary>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireProgressBarUpdate(long bytesProcessed)
        {
            if (this.fileForUpload != null)
            {
                this.fileForUpload.setBytesProcessed(bytesProcessed);
                long total = this.fileForUpload.getSelectedFileForUpload().Length;

                double bytesD = Convert.ToDouble(bytesProcessed);
                double totalD = Convert.ToDouble(total);

                double value = Math.Floor((bytesD / totalD) * 100.00);

                foreach (UI observer in this.observers)
                    if (observer is FileUploadUI)
                        ((FileUploadUI)observer).updateProgressBar(Convert.ToInt32(value));
            }
        }

        /// <summary>
        ///     Sets the state of the UIs.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireState(State state)
        {
            foreach (UI observer in this.observers)
                if (observer is MainUI)
                    ((MainUI)observer).setState(state);
        }

        /// <summary>
        ///     Updates the synchronization UI's.
        /// </summary>
        /// <param name="currentFile">The current file.</param>
        /// <param name="totalFiles">The number of files.</param>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireSynchronizationUpdate(int currentFile, int totalFiles)
        {
            String text = String.Empty;
            int value = 0;

            if (currentFile >= totalFiles)
            {
                value = 100;
                text = this.getLanguageString("done_synchronizing");
            }
            else
            {
                text = this.getLanguageString("synchronizing_file_x_of_y", new String[] { Convert.ToString(currentFile + 1), Convert.ToString(totalFiles) });
                Double result = Convert.ToDouble(currentFile) / Convert.ToDouble(totalFiles);
                value = Convert.ToInt32(Math.Ceiling(result * 100.00));
            }

            foreach (UI observer in this.observers)
                if (observer is MainUI)
                    ((MainUI)observer).updateSynchronizationStatus(value, text);
        }

        /// <summary>
        ///     Updates the exercise results on the UI.
        /// </summary>
        /// <remarks>
        ///     Last updated on 10/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void fireExerciseResultsUpdate()
        {
            foreach (UI observer in this.observers)
                if (observer is MainUI)
                    ((MainUI)observer).updateExerciseScanResults(this.getExerciseScanResults());
        }

        #endregion

        #region All methods using documents

        /// <summary>
        ///     Initialises the document that will be worked with.
        /// </summary>
        /// <param name="link">The full path of the document.</param>
        public void setDocument(String link)
        {
            this.exerciseSimpleErrorList = null;
            FileInfo fileInfo = new FileInfo(link);
            if (fileInfo.Exists)
            {
                this.document = SupportedOfficeFiles.ExercisesFactory(link);
                this.writeToLog("selected_document_is_x", new String[] { fileInfo.Name }, true, false, false);
            }
            else
            {
                this.writeToLog("selected_document_x_not_valid", new String[] { fileInfo.Name }, true, false, false);
            }
        }

        /// <summary>
        ///     Removes all temporary files and resets the document object.
        /// </summary>
        public void clearDocument()
        {
            if (this.document != null)
            {
                this.clearTempFolder();
                this.document = null;
                this.exerciseSimpleErrorList = null;
            }
        }

        public Boolean exercisesAreScanned()
        {
            if (this.document == null) return false;
            return (this.document.getExercises() != null && this.document.getExercises().Count > 0);
        }

        public Boolean exercisesHaveErrors()
        {
            if (this.document == null) return true;
            if (this.exerciseSimpleErrorList == null) return true;
            return exerciseSimpleErrorList.Count > 0;
        }

        public String getExerciseScanResults()
        {
            if (this.exercisesAreScanned())
            {
                int questions = 0;
                foreach (Exercise ex in this.document.getExercises())
                {
                    questions += ex.getQuestionsAsList().Count;
                }
                return this.getLanguageString("exercises") + ": " + this.document.getExercises().Count
                        + Environment.NewLine + this.getLanguageString("questions") + ": " + questions
                        + Environment.NewLine + this.getLanguageString("errors") + ": " + exerciseSimpleErrorList.Count.ToString()
                        + Environment.NewLine + this.getLanguageString("warnings") + ": " + document.getDocumentScanWarnings().ToString();
            }
            else if (this.document != null && this.document.getExercises() != null && this.document.getExercises().Count == 0)
                return this.getLanguageString("no_exercises_found");
            else
                return this.getLanguageString("no_document_scanned");
        }


        public void jumpToError()
        {
            if (this.document != null && this.document.supportsJumpToSection() && this.exerciseSimpleErrorList.Count > 0)
                this.document.jumpToError(exerciseSimpleErrorList[0][0], exerciseSimpleErrorList[0][1], exerciseSimpleErrorList[0][2]);
        }

        public Boolean canJumpToError()
        {
            if (this.document == null) return false;
            return this.document.supportsJumpToSection();
        }

        /// <summary>
        ///     Returns a text representation of the exercises.
        /// </summary>
        /// <returns>The text representation of the exercises.</returns>
        public String getExercisesToString()
        {
            return this.document.exercisesToString();
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document.
        /// </summary>
        /// <returns>True if the document contains exercises.</returns>
        private Boolean convertDocument(SupportedExercisesDocument abstractDocument, Boolean nologging)
        {
            Boolean newLogSession = !this.hasActiveLoggerSession() && !nologging;
            Boolean isValidDocument = false;
            if (abstractDocument != null)
            {
                //Start by removing all existing documents...
                this.clearTempFolder();

                //Read exercises...
                if (newLogSession) this.startLoggerSession();
                abstractDocument.extractExercises();

                //Check if document contains exercises...
                if (abstractDocument.getExercises() != null && abstractDocument.getExercises().Count > 0)
                {
                    isValidDocument = true;
                }
                else
                {
                    isValidDocument = false;
                    this.writeToLog("no_exercises_found", true, false, !this.isSynchronization);
                }
            }
            else
            {
                isValidDocument = false;
                this.writeToLog("no_document_selected", true, false, !this.isSynchronization);
            }
            if (newLogSession) this.endLoggerSession();
            return isValidDocument;
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document and validates them.
        /// </summary>
        /// <param name="simplecheck">Perform a simple check which only returns if the exeerrcises are valid, without returning a list of errors</param>
        /// <param name="showPositiveFeedback">Show positive feedback message on UI. Disable when calling this for a single-command check and upload</param>
        /// <returns>True if the document contains 100% valid exercises.</returns>
        public Boolean checkDocument(Boolean simplecheck, Boolean showPositiveFeedback)
        {
            return this.checkDocument(this.document, simplecheck, showPositiveFeedback);
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document and validates them.
        /// </summary>
        /// <param name="abstractDocument">The document.</param>
        /// <param name="simplecheck">Perform a simple check which only returns if the exeerrcises are valid, without returning a list of errors</param>
        /// <param name="showPositiveFeedback">Show positive feedback message on UI. Disable when calling this for a single-command check and upload</param>
        /// <returns>True if the document contains 100% valid exercises.</returns>
        public Boolean checkDocument(SupportedExercisesDocument abstractDocument, Boolean simplecheck, Boolean showPositiveFeedback)
        {
            Boolean newLogSession = !this.hasActiveLoggerSession() && !simplecheck;
            if (newLogSession) this.startLoggerSession();
            if (convertDocument(abstractDocument, simplecheck)) // geeft al een message box als er geen oefeningen gevonden zijn
            {
                this.writeToLog("checking_exercises", true, false, false);
                List<String> errorsList = new List<String>();
                this.exerciseSimpleErrorList = new List<int[]>();
                String htmlPath = this.tempPath + ProgramConstants.HTML_OUTPUT_NAME;
                this.trimEmptyEndingsFromExercises(abstractDocument.getExercises());
                Boolean correctDocument = ExercisesValidator.checkExercises(abstractDocument.getExercises().ToArray(), ref errorsList, ref this.exerciseSimpleErrorList, htmlPath);
                if (!simplecheck)
                {
                    String errors = String.Empty;
                    if (errorsList.Count > 0)
                        this.writeToLoggerSession(String.Empty);
                    foreach (String err in errorsList)
                    {
                        if (!errors.Equals(String.Empty))
                            errors += Environment.NewLine;
                        errors += err;
                        this.writeToLoggerSession(err);
                    }
                    this.writeToLog("check_completed_errors_x_warnings_y",
                            new String[] { exerciseSimpleErrorList.Count.ToString(), abstractDocument.getDocumentScanWarnings().ToString() },
                            true, false, !this.isSynchronization && (!correctDocument || (showPositiveFeedback && correctDocument)));
                    if (newLogSession) this.endLoggerSession();
                }
                else
                {
                    if (correctDocument)
                        this.writeToLog("check_completed", true, false, !this.isSynchronization && showPositiveFeedback);
                    else
                        this.writeToLog("errors_in_exercises", true, false, !this.isSynchronization);
                }
                return correctDocument;
            }
            else
            {
                if (newLogSession) this.endLoggerSession();
                return false;
            }
        }

        private void trimEmptyEndingsFromExercises(List<Exercise> exercises)
        {
            // filter out empty end-answers
            foreach (Exercise ex in exercises)
            {
                foreach (Question qu in ex.getQuestionsAsList())
                {
                    // clean up all empty feedback tags
                    foreach (Answer ans in qu.getAnswersAsList())
                    {
                        if (ans.getFeedback() == null
                            || Regex.IsMatch(ans.getFeedback(), Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                            ans.setFeedback(String.Empty);
                    }

                    Answer lastAns = null;
                    if (qu.getAnswersAsList().Count > 0)
                        lastAns = qu.getAnswersAsList()[qu.getAnswersAsList().Count - 1];

                    while (lastAns != null
                           && (lastAns.getAnswer() == null
                               || Regex.IsMatch(lastAns.getAnswer(), Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                           && lastAns.getWeight() == int.MinValue
                           && lastAns.getFeedback().Equals(String.Empty)
                          )
                    {
                        qu.getAnswersAsList().RemoveAt(qu.getAnswersAsList().Count - 1);
                        if (qu.getAnswersAsList().Count == 0)
                            lastAns = null;
                        else
                            lastAns = qu.getAnswersAsList()[qu.getAnswersAsList().Count - 1];
                    }
                }
            }

            // filter out empty end-questions
            foreach (Exercise ex in exercises)
            {
                Question lastQues = null;
                if (ex.getQuestionsAsList().Count > 0)
                    lastQues = ex.getQuestionsAsList()[ex.getQuestionsAsList().Count - 1];

                while (lastQues != null
                       && (lastQues.getQuestionTitle() == null || lastQues.getQuestionTitle().Equals(String.Empty))
                       && (lastQues.getQuestionText() == null ||
                           Regex.IsMatch(lastQues.getQuestionText(), Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                       && lastQues.getAnswersAsList().Count == 0
                      )
                {
                    ex.getQuestionsAsList().RemoveAt(ex.getQuestionsAsList().Count - 1);
                    if (ex.getQuestionsAsList().Count == 0)
                        lastQues = null;
                    else
                        lastQues = ex.getQuestionsAsList()[ex.getQuestionsAsList().Count - 1];
                }
            }

            // filter out empty end-answers
            Exercise lastEx = null;
            if (exercises.Count > 0)
                lastEx = exercises[exercises.Count - 1];

            while (lastEx != null
                   && (lastEx.getName() == null || lastEx.getName().Equals(String.Empty))
                   && (lastEx.getDescription() == null ||
                       Regex.IsMatch(lastEx.getDescription(), Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                   && lastEx.getQuestionsAsList().Count == 0
                  )
            {
                exercises.RemoveAt(exercises.Count - 1);
                if (exercises.Count == 0)
                    lastEx = null;
                else
                    lastEx = exercises[exercises.Count - 1];
            }
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document, validates them and uploads them to the platform if they're valid.
        /// </summary>
        public void convertAndUploadExercisesDocument()
        {
            convertAndUploadExercisesDocument(false, false, false, 0);
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document, validates them and uploads them to the platform if they're valid.
        /// </summary>
        /// <param name="onePerPage">Enable "one exercise page" status on all exercises.</param>
        public void convertAndUploadExercisesDocument(Boolean onePerPage, Boolean setExerciseInvisible, int randomQuestions)
        {
            convertAndUploadExercisesDocument(true, onePerPage, setExerciseInvisible, randomQuestions);
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document, validates them and uploads them to the platform if they're valid.
        /// </summary>
        /// <param name="changePageStatus">Determines whether the "one exercise page" status in the exercises is changed.</param>
        /// <param name="onePerPage">Enable "one exercise page" status on all exercises.</param>
        public void convertAndUploadExercisesDocument(Boolean changePageStatus, Boolean onePerPage, Boolean setExerciseInvisible, int randomQuestions)
        {
            this.convertAndUploadExercisesDocument(this.document, changePageStatus, onePerPage, setExerciseInvisible, randomQuestions);
        }

        /// <summary>
        ///     Extracts the exercises from the loaded document, validates them and uploads them to the platform if they're valid.
        /// </summary>
        /// <param name="abstractDocument">The document.</param>
        /// <param name="changePageStatus">Determines whether the "one exercise page" status in the exercises is changed.</param>
        /// <param name="onePerPage">Enable "one exercise page" status on all exercises.</param>
        /// <param name="isInvisible">Indicates whether the exercise must be visible or not.</param>
        /// <returns>True if conversion has succeeded.</returns>
        public Boolean convertAndUploadExercisesDocument(SupportedExercisesDocument abstractDocument, Boolean changePageStatus, Boolean onePerPage, Boolean isInvisible, int randomQuestions)
        {
            Boolean retValue = false;

            if (abstractDocument != null)
            {
                if (this.isLoggedIn())
                {
                    Boolean newLogSession = !this.hasActiveLoggerSession();
                    if (newLogSession) this.startLoggerSession();
                    if (this.checkDocument(abstractDocument, false, false))
                    {
                        if (changePageStatus)
                            foreach (Exercise ex in abstractDocument.getExercises())
                                ex.setMultipage(onePerPage);

                        this.fireExerciseResultsUpdate();
                        retValue = platform.doUploadExercises(abstractDocument.getExercises(), this.userInfo.getSelectedItemCourses(), this.userInfo.getSelectedSubject(), isInvisible, randomQuestions, false);
                    }
                    if (newLogSession) this.endLoggerSession();
                }
                else
                {
                    this.writeToLog("please_login_first", true, false, !this.isSynchronization);
                }
            }
            else
            {
                this.fireDumpOnUI(String.Empty);
                this.writeToLog("no_document_selected", true, false, !this.isSynchronization);

            }
            return retValue;
        }

        /// <summary>
        ///     Uploads the currently extracted exercises to the platform, if they're valid.
        /// </summary>
        public void uploadExercises()
        {
            uploadExercises(false, false, false, 0);
        }

        /// <summary>
        ///     Uploads the currently extracted exercises to the platform, if they're valid.
        /// </summary>
        /// <param name="onePerPage">Enable "one exercise page" status on all exercises.</param>
        public void uploadExercises(Boolean onePerPage, int randomQuestions)
        {
            uploadExercises(true, onePerPage, false, randomQuestions);
        }

        /// <summary>
        ///     Uploads the currently extracted exercises to the platform, if they're valid.
        /// </summary>
        /// <param name="changePageStatus">Determines whether the "one exercise page" status in the exercises is changed.</param>
        /// <param name="onePerPage">Enable "one exercise page" status on all exercises.</param>
        /// <param name="isInvisible">Indicates if the exercise has to be invisible.</param>
        /// <remarks>
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        ///      -> Upload anonymous statistics.
        /// </remarks>
        public void uploadExercises(Boolean changePageStatus, Boolean onePerPage, Boolean setInvisible, int randomQuestions)
        {
            if (this.exerciseSimpleErrorList != null && this.exerciseSimpleErrorList.Count == 0) // check is done
            {
                if (changePageStatus)
                    foreach (Exercise ex in document.getExercises())
                        ex.setMultipage(onePerPage);
                platform.doUploadExercises(document.getExercises(), this.userInfo.getSelectedItemCourses(), this.userInfo.getSelectedSubject(), setInvisible, randomQuestions, false);
                //As of 1.09
                AnonymousStatistics.sendStatistics(document.getExercises());
            }
        }

        /// <summary>
        ///     Converts the selected document to PDF, using the PDF split filename pattern from the program settings,
        ///     and uploads the resulting file(s) to the specified documentFolder on the platform.
        /// </summary>
        /// <param name="split">Split the document in chapters</param>
        /// <param name="splitText">The style name to use when splitting the document.</param>
        /// <param name="splitOnPage">Indicates if the split should only occur on page ends, or also in the middle of a page.</param>
        /// <param name="publishDestinations">Location(s) on the platform to publish the resulting file(s). Null for no upload.</param>
        /// <param name="convertHyperlinksToJavascript">Convert the hyperlinks in the PFD document(s) to javascript to allow browser-embedded documents to open links in a new browser page.</param>
        /// <returns>the full paths of the converted files</returns>
        public Boolean convertToPDF(Boolean split, String splitText, Boolean splitOnPage, DocumentFolder publishDestinations, Boolean convertHyperlinksToJavascript, Boolean setInvisible, ref Boolean uploadSucceeded)
        {
            Boolean error = false;
            this.convertToPDF(this.document, split, settings.getPDFFileName(), splitText, splitOnPage, publishDestinations, convertHyperlinksToJavascript, setInvisible, ref uploadSucceeded, ref error);
            return error;
        }

        /// <summary>
        ///     Converts a document to PDF without upload to the platform.
        /// </summary>
        /// <param name="convertDocument">The codument to convert</param>
        /// <param name="split">Split the document in chapters</param>
        /// <param name="namePattern">The name pattern to use for the split PDF parts.</param>
        /// <param name="splitText">The style name to use when splitting the document.</param>
        /// <param name="splitOnPage">Indicates if the split should only occur on page ends, or also in the middle of a page.</param>
        /// <param name="convertHyperlinksToJavascript">Convert the hyperlinks in the PFD document(s) to javascript to allow browser-embedded documents to open links in a new browser page.</param>
        /// <param name="setInvisible"></param>
        /// <param name="error">True if error occurred.</param>
        /// <returns>the full paths of the converted files</returns>
        /// <remarks>
        ///     Last updated on 13/08/2010 by Gianni Van Hoecke
        ///      -> Added error support
        /// </remarks>
        public List<String> convertToPDF(SupportedExercisesDocument convertDocument, Boolean split, String namePattern, String splitText, Boolean splitOnPage, Boolean convertHyperlinksToJavascript, Boolean setInvisible, ref Boolean error)
        {
            Boolean uploadSucceeded = false;
            return this.convertToPDF(convertDocument, split, namePattern, splitText, splitOnPage, null, convertHyperlinksToJavascript, setInvisible, ref uploadSucceeded, ref error);
        }

        /// <summary>
        ///     Converts a document to PDF, and uploads the resulting file(s)
        ///     to the specified documentFolder on the platform.
        /// </summary>
        /// <param name="convertDocument">The codument to convert</param>
        /// <param name="split">Split the document in chapters</param>
        /// <param name="namePattern">The name pattern to use for the split PDF parts.</param>
        /// <param name="splitText">The style name to use when splitting the document.</param>
        /// <param name="splitOnPage">Indicates if the split should only occur on page ends, or also in the middle of a page.</param>
        /// <param name="publishDestinations">Location(s) on the platform to publish the resulting file(s). Null for no upload.</param>
        /// <param name="convertHyperlinksToJavascript">Convert the hyperlinks in the PFD document(s) to javascript to allow browser-embedded documents to open links in a new browser page.</param>
        /// <param name="uploadSucceeded">Returns the status of the upload process.</param>
        /// <param name="setInvisible"></param>
        /// <param name="error">True if error occurred.</param>
        /// <returns>the full paths of the converted files</returns>
        public List<String> convertToPDF(SupportedExercisesDocument convertDocument, Boolean split, String namePattern, String splitText, Boolean splitOnPage, DocumentFolder publishDestinations, Boolean convertHyperlinksToJavascript, Boolean setInvisible, ref Boolean uploadSucceeded, ref Boolean error)
        {
            List<String> retValue = new List<String>();
            if (convertDocument != null)
            {
                if (publishDestinations != null && !this.isLoggedIn())
                {
                    this.writeToLog("please_login_first", true, false, !this.isSynchronization);
                    return retValue;
                }
                DomainController.Instance().writeToLog("publishing_document", new String[] { Path.GetFileName(convertDocument.getDocumentPathWithFilename()) }, true, false, false);

                Boolean uploadReturnValue = false;    //The path returned by the upload function.
                String localSavePath = Path.GetDirectoryName(convertDocument.getDocumentPathWithFilename());
                //Convert
                Boolean underscores = DomainController.Instance().getSettings().getPDFReplaceSpacesByUndescores();
                if (split)
                    retValue = convertDocument.convertToPDFWithSplit(localSavePath, splitText, namePattern, splitOnPage, underscores , convertHyperlinksToJavascript, ref error);
                else
                    retValue = convertDocument.convertToPDF(localSavePath, underscores, convertHyperlinksToJavascript, ref error);

                if (error || retValue.Count == 0)
                {
                    DomainController.Instance().writeToLog("converting_to_pdf_failed", new String[] { Path.GetFileName(convertDocument.getDocumentPathWithFilename()) }, true, false, !this.isSynchronization);
                }
                else if (publishDestinations != null)
                {
                    DomainController.Instance().writeToLog("converting_to_pdf_completed", true, false, false);
                    uploadReturnValue = uploadPDFs(retValue, publishDestinations, setInvisible, true);

                    if (!uploadReturnValue)
                        DomainController.Instance().writeToLog("converting_to_pdf_completed_but_upload_failed", new String[] { convertDocument.getDocumentPathWithFilename() }, true, false, !this.isSynchronization);
                }
                else
                {
                    DomainController.Instance().writeToLog("converting_to_pdf_completed", true, false, !this.isSynchronization);
                }
            }
            else
            {
                this.writeToLog("no_document_selected", true, false, !this.isSynchronization);
            }

            return retValue;
        }

        public Boolean uploadPDFs(List<String> convertedPDFs, DocumentFolder publishDestinations, Boolean setInvisible)
        {
            return uploadPDFs(convertedPDFs, publishDestinations, setInvisible, false);
        }

        public Boolean uploadPDFs(List<String> convertedPDFs, DocumentFolder publishDestinations, Boolean setInvisible, Boolean showMessageBox)
        {

            this.writeToLog("uploading_pdf", true, false);
            Boolean uploadReturnValue = true;
            foreach (String file in convertedPDFs)
            {
                String[] uploadReturn = this.uploadFile(file, publishDestinations, "", setInvisible, false);
                Boolean allfilesUploaded = uploadReturn.Length > 0;
                foreach (String st in uploadReturn)
                    if (st.Equals(String.Empty))
                        allfilesUploaded = false;
                uploadReturnValue &= allfilesUploaded;
            }
            this.loadDocumentFolders(false);
            this.fireUpdate(!this.isSynchronization);
            this.writeToLog("uploading_pdf_completed", true, false, showMessageBox && !this.isSynchronization);

            return uploadReturnValue;
        }

        /// <summary>
        ///     Opens the template for the currently set language.
        /// </summary>
        /// <remarks>
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke.
        ///      -> This method now uses a MS Word object so that we can manipulate the default values.
        /// </remarks>
        /// <param name="useExamplesTemplate">Determines whether to use a blank template; or a template with examples</param>
        public void openTemplateFile(Boolean useExamplesTemplate)
        {
            //OLD CODE
            /*
            String template;
            if (useExamplesTemplate)
                template = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + ProgramConstants.RESOURCE_DIRECTORY_NAME + "\\" + this.settings.getLanguage() + ProgramConstants.TEMPLATE_EXAMPLES + "." + ProgramConstants.TEMPLATE_EXTENSION;
            else
                template = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + ProgramConstants.RESOURCE_DIRECTORY_NAME + "\\" + this.settings.getLanguage() + ProgramConstants.TEMPLATE_BLANK + "." + ProgramConstants.TEMPLATE_EXTENSION;
            if (Utility.doesFileExist(template))
                Process.Start(template);
            else
                this.writeToLog("template_file_not_found", true, false, true);
            */
            //END OLD CODE
            String template = Path.GetDirectoryName(Application.ExecutablePath) + "\\" + ProgramConstants.RESOURCE_DIRECTORY_NAME + "\\" + this.settings.getLanguage() + (useExamplesTemplate ? ProgramConstants.TEMPLATE_EXAMPLES : ProgramConstants.TEMPLATE_BLANK) + "." + ProgramConstants.TEMPLATE_EXTENSION;
            String changeToFolder = this.userInfo.getSelectedItemSubjectFolder();
            SupportedOfficeFiles.openDocument(template, changeToFolder);
        }

        #endregion

        #region All methods concerning presentation and spreadsheet files (As of 1.08)

        /// <summary>
        ///     Converts a presentation file to a PDF file.
        /// </summary>
        /// <param name="file">The presentation File.</param>
        /// <param name="frameSlides">True if the slides have to be framed.</param>
        /// <param name="horizontal">True if the hand-outs are printed horizontally.</param>
        /// <param name="publishMethod">The publish method.</param>
        /// <param name="slidesPerPage">Indicates how many slides per page.</param>
        /// <param name="includeHiddenSlides">True if hidden slides must be processed.</param>
        /// <returns>A list containing the converted files.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 11/03/2010 by Maarten Meuris
        ///      -> Added log messages after conversion
        /// 
        ///     updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> Initial code.
        /// </remarks>
        public List<String> convertPresentationToPDF(String file, String destination, Boolean frameSlides, Boolean horizontal, PresentationPublishTypes publishMethod, int slidesPerPage, Boolean includeHiddenSlides, ref Boolean error)
        {
            this.writeToLog("publishing_presentation", new String[] { Path.GetFileName(file) }, true, false, false);
            List<String> resultfiles = new List<String>();
            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.PRESENTATION_DOCUMENT)
            {
                SupportedPresentationDocument presentation = SupportedOfficeFiles.PresentationFactory(file);
                resultfiles = presentation.convertToPDF(destination, frameSlides, horizontal, publishMethod, slidesPerPage, includeHiddenSlides, ref error);
            }
            else
            {
                error = true;
            }

            if (error || resultfiles.Count == 0)
                DomainController.Instance().writeToLog("converting_to_pdf_failed", new String[] { Path.GetFileName(file) }, true, false, !this.isSynchronization);
            else
                DomainController.Instance().writeToLog("converting_to_pdf_completed", true, false, !this.isSynchronization);


            return resultfiles;
        }

        /// <summary>
        ///     Converts the Excel worksheet into a PDF file.
        ///     <b>Note</b>: no parameters are currently supported. The whole workbook will be converted!
        /// </summary>
        /// <param name="file">The Excel File.</param>
        /// <returns>A list containing the converted files.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 11/03/2010 by Maarten Meuris
        ///      -> Added log messages after conversion
        /// 
        ///     updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> Initial code.
        /// </remarks>
        public List<String> convertExcelToPDF(String file, String destination, ref Boolean error)
        {
            this.writeToLog("publishing_workbook", new String[] { Path.GetFileName(file) }, true, false,false);
            List<String> resultfiles = new List<String>();
            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.SPREADSHEET_DOCUMENT)
            {
                SupportedSpreadsheetDocument presentation = SupportedOfficeFiles.SpreadsheetFactory(file);
                resultfiles = presentation.convertToPDF(destination, ref error);
            }
            else
            {
                error = true;
            }

            if (error || resultfiles.Count == 0)
                DomainController.Instance().writeToLog("converting_to_pdf_failed", new String[] { Path.GetFileName(file) }, true, false, !this.isSynchronization);
            else
                DomainController.Instance().writeToLog("converting_to_pdf_completed", true, false, !this.isSynchronization);

            return resultfiles;
        }

        #endregion

        #region All methods related to statistics

        /// <summary>
        ///     Exports the statistics.
        /// </summary>
        /// <param name="saveToFolder">The output folder.</param>
        /// <param name="calculateMC">True = calculate MC percentage.</param>
        /// <param name="calculateResultsPerStudent">True = calculate results per student.</param>
        /// <param name="calculateExerciseStudentDetails">True = calculate details per student.</param>
        /// <param name="mcShowQuestionTitles">Show question titles at MC export.</param>
        /// <param name="doNotKnowString">The "Do not know" answer. (used to generate the chart.)</param>
        /// <param name="createSubFolder">True = create sub folder.</param>
        /// <param name="showEmailInsteadOfName">True = e-mail address will replace the students name.</param>
        /// <param name="createSubjectFolder">True to create a new subfolder.</param>
        /// <param name="generateAllAttempts">True to generate all exercise attempts by the student.</param>
        /// <remarks>
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        ///      -> Added groups.
        ///     
        ///     updated on 17-18/08/2010 by Gianni Van Hoecke
        ///      -> multiple attempts can now be generated while calculating calculateResultsPerStudent.
        /// </remarks>
        public void exportStatistics(String saveToFolder, Boolean createSubFolder, Boolean createSubjectFolder, Boolean calculateMC, Boolean calculateResultsPerStudent, Boolean calculateExerciseStudentDetails, Boolean mcShowQuestionTitles, String doNotKnowString, Boolean generateAllAttempts, Boolean useGroups)
        {
            List<Course> courses = this.userInfo.getSelectedItemCourses();
            this.clearTempFolder();
            String folder = String.Empty;

            folder = saveToFolder.TrimEnd('\\');

            //If the course is (in) a subject, add a subfolder (only if the user wants this!)
            if (this.userInfo.selectedItemIsSubject() && createSubjectFolder)
            {
                folder += @"\" + this.userInfo.getSelectedSubject().getSubjectName();
            }

            Boolean withoutMissingData = calculateResultsPerStudent && !calculateMC && !calculateExerciseStudentDetails;
            List<Scores> scoresList;

            try
            {
                scoresList = this.getPlatform().downloadStatistics(courses, folder, createSubFolder, withoutMissingData);
            }
            catch (NotImplementedException ex)
            {
                DomainController.Instance().writeToLog(ex.Message, null , true, false, true);
                return;
            }

            String dateStringNow = "_" + DateTime.Now.Year + String.Format("{0:d2}", DateTime.Now.Month) + String.Format("{0:d2}", DateTime.Now.Day) + "_" + String.Format("{0:d2}", DateTime.Now.Hour) + String.Format("{0:d2}", DateTime.Now.Minute);
            foreach (Scores scores in scoresList)
            {
                scores.sortStudents();
                scores.sortExercises();
                scores.sortGroups();

                //Added by Patrick Lauwaerts on 2010-11-23:Utility.getFileNameWithoutBadCharacters
                String name = @"\" + Utility.getFileNameWithoutBadCharacters(scores.course.getCourseName() + "_" + scores.course.getCourseCode());
                String oldName = name + "_TEMP";
                String newName = name + dateStringNow;

                oldName = folder + oldName.Replace(' ', '_');
                newName = folder + newName.Replace(' ', '_');

                //Rename temp folder.
                try
                {
                    Directory.Move(oldName, newName);
                }
                catch { /* ignore */}

                Calculator calculator = new Calculator(scores, scores.course.getDisplayName(), scores.course.getCourseCode(), dateStringNow, newName, calculateMC, calculateResultsPerStudent, calculateExerciseStudentDetails, mcShowQuestionTitles, doNotKnowString, generateAllAttempts, useGroups);
                calculator.run();
            }

            this.writeToLog("creating_statistics_completed", true, false, true);

            try
            {
                //Start with tree view!
                if (!this.getSettings().getStatsOpenExcelAfterConversion())
                {
                    Utility.openFolderInExplorer(folder);
                }
            }
            catch (Exception ex)
            {
                List<String[]> list = new List<String[]>();
                list.Add(new String[]{ "File", folder });
                this.processError(ex, false, ex.Message, false, list);
            }
        }

        #endregion

        #region All methods related to groups (As of 1.09)

        /// <summary>
        ///     Downloads the groups of the selected cours(es).
        /// </summary>
        public void downloadGroups()
        {
            if(isLoggedIn())
                this.userInfo.setGroups(this.platform.downloadGroups(this.userInfo.getSelectedItemCourses()));
        }

        #endregion

        #region All language-related methods

        /// <summary>
        ///     Provides the ability to change the language.
        ///     Changes are also saved to the settings file.
        /// </summary>
        /// <param name="lang">The selected language.</param>
        public void selectLanguage(String lang)
        {
            this.selectLanguage(lang, true);
        }

        /// <summary>
        ///     Provides the ability to change the language.
        /// </summary>
        /// <param name="lang">The selected language.</param>
        /// <param name="save">Save to settings file.</param>
        public void selectLanguage(String lang, Boolean save)
        {
            this.resourceLoader = new ResourceLoader(resourcePath, lang, ProgramConstants.RESOURCE_EXTENSION);
            if (save)
                this.settings.setLanguage(lang);
        }

        /// <summary>
        ///     Returns a list of all available languages.
        /// </summary>
        /// <returns>An array with all available languages.</returns>
        public String[] getLanguages()
        {
            return ResourceList.getResources(resourcePath, ProgramConstants.RESOURCE_EXTENSION);
        }

        /// <summary>
        ///     Returns a string, based on a code string, in the selected language.
        /// </summary>
        /// <param name="str">De code string.</param>
        /// <returns>The translated string.</returns>
        public String getLanguageString(String str)
        {
            return resourceLoader.getString(str);
        }

        /// <summary>
        ///     Returns a string, based on a code string, in the selected language.
        /// </summary>
        /// <param name="str">The code string.</param>
        /// <param name="args">Extra arguments to be inserted into the string by the language system.</param>
        /// <returns>The translated String.</returns>
        public String getLanguageString(String str, String[] args)
        {
            return resourceLoader.getString(str, args);
        }

        #endregion

        #region All logging methods

        /// <summary>
        ///     Opens the log file in the default text editor.
        ///     If no default editor is set, the program will try to open notepad.
        /// </summary>
        public void openLogInNotePad()
        {
            try
            {
                Process.Start(ProgramConstants.getLogFilePath());
            }
            catch (Exception e)
            {
                try
                {
                    Process.Start("notepad.exe", ProgramConstants.getLogFilePath());
                }
                catch (Exception ex)
                {
                    this.processError(ex, true, e.Message, false, null);
                }
            }
        }

        /// <summary>
        ///     Gets the contents of the log file in a single string.
        /// </summary>
        /// <param name="advancedView">True if debug parameters have to included.</param>
        /// <returns>The log contents.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last Updated on 14/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public String getLogFileContent(Boolean advancedView)
        {
            return String.Join(Environment.NewLine, Logger.getLogAsList(advancedView).ToArray());
        }

        /// <summary>
        ///     Writes an event to the log file. The UI's will be updated.
        /// </summary>
        /// <param name="value">The event that has to be written to the log file.</param>
        public void writeToLog(String value)
        {
            this.writeToLog(value, false, false);
        }

        /// <summary>
        ///     Writes an event to the log file. The UI's will be updated.
        /// </summary>
        /// <param name="isStringID">Is the string a code string?</param>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="debug">Indicates whether the string is a debug value.</param>
        public void writeToLog(String value, Boolean isStringID, Boolean debug)
        {
            this.writeToLog(value, null, isStringID, debug, false);
        }

        /// <summary>
        ///     Writes an event to the log file. The UI's will be updated.
        /// </summary>
        /// <param name="isStringID">Is the string a code string?</param>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="debug">Indicates whether the string is a debug value.</param>
        /// <param name="showMessageBox">Indicates if a messagebox has to be fired.</param>
        public void writeToLog(String value, Boolean isStringID, Boolean debug, Boolean showMessageBox)
        {
            this.writeToLog(value, null, isStringID, debug, showMessageBox);
        }

        /// <summary>
        ///     Writes an event to the log file. The UI's will be updated.
        /// </summary>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="args">Extra arguments to be inserted into the string by the language system.</param>
        /// <param name="isStringID">Indicates whether the value has to be converted using the language system.</param>
        /// <param name="debug">Indicates whether the string is a debug value.</param>
        public void writeToLog(String value, String[] args, Boolean isStringID, Boolean debug, Boolean showMessageBox)
        {
            String tempString = this.getLogString(value, args, isStringID);
            Logger.write(tempString, debug);
            this.fireLogUpdate();
            if (showMessageBox)
                this.fireMessageBox(tempString, MessageBoxIcon.Information);
        }

        /// <summary>
        ///     Writes a string to the current logger session
        /// </summary>
        /// <param name="value">The string to write to the logger session</param>
        public void writeToLoggerSession(String value)
        {
            writeToLoggerSession(value, null, false);
        }

        /// <summary>
        ///     Writes a string to the current logger session
        /// </summary>
        /// <param name="value">The string to write to the logger session</param>
        /// <param name="isStringID">Indicates whether the value has to be converted using the language system</param>
        public void writeToLoggerSession(String value, Boolean isStringID)
        {
            writeToLoggerSession(value, null, isStringID);
        }

        /// <summary>
        ///     Writes a string to the current logger session
        /// </summary>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="args">Extra arguments to be inserted into the string by the language system.</param>
        public void writeToLoggerSession(String value, String[] args)
        {
            writeToLoggerSession(value, args, true);
        }

        /// <summary>
        ///     Writes a string to the current logger session
        /// </summary>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="args">Extra arguments to be inserted into the string by the language system.</param>
        /// <param name="isStringID">Indicates whether the value has to be converted using the language system.</param>
        public void writeToLoggerSession(String value, String[] args, Boolean isStringID)
        {
            if (this.loggerSession != null)
            {
                this.loggerSession.Append(this.getLogString(value, args, isStringID) + Environment.NewLine);
                if (!value.Equals(String.Empty)) writeToLog(value, args, isStringID, true, false);
                this.fireDumpOnUI(loggerSession.ToString());
            }
            else writeToLog(value, args, isStringID, false, false);
        }

        /// <summary>
        ///     Starts a logger session.
        /// </summary>        
        public void startLoggerSession()
        {
            this.loggerSession = new StringBuilder();
        }

        /// <summary>
        ///     returns if there is currently an active logger session.
        /// </summary>
        /// <returns>True if there is currently an active logger session.</returns>
        public Boolean hasActiveLoggerSession()
        {
            return this.loggerSession != null;
        }

        /// <summary>
        ///     Gets the current logger session StringBuilder.
        /// </summary>
        /// <returns>the current logger session.</returns>
        public StringBuilder getLoggerSession()
        {
            return this.loggerSession;
        }

        /// <summary>
        ///     Destroys the current logger session
        /// </summary>
        public void endLoggerSession()
        {
            this.loggerSession = null;
        }

        /// <summary>
        ///     Prepares a string for the logger function
        /// </summary>
        /// <param name="value">The event that has to be written to the log file.</param>
        /// <param name="args">Extra arguments to be inserted into the string by the language system.</param>
        /// <param name="isStringID">Indicates whether the value has to be converted using the language system.</param>
        /// <returns>The formatted string</returns>
        private String getLogString(String value, String[] args, Boolean isStringID)
        {
            if (!isStringID)
            {
                return value;
            }
            else
            {
                return this.getLanguageString(value, args);
            }
        }

        /// <summary>
        ///     Processes an error. Writes the error to the log file and shows a message box if needed.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="showMessageBox">Show message box.</param>
        public void processError(Exception e, Boolean showMessageBox)
        {
            this.writeToLog(e.Message, false, false);
            this.writeToLog(e.StackTrace, false, true);
            if (showMessageBox)
                this.fireMessageBox(this.getLanguageString("error") + ": " + e.Message, MessageBoxIcon.Error);
        }

        /// <summary>
        ///     Processes an error with a custom message. 
        ///     Writes the error to the log file and shows a message with the custom message box if needed.
        /// </summary>
        /// <param name="e">The exception.</param>
        /// <param name="showMessageBox">Show message box.</param>
        /// <param name="customMessage">The custom message string.</param>
        /// <param name="customMessageIsLanguageCode">True if the custom message string has to be translated.</param>
        /// <param name="extraInfo">Optional parameter (can be null or empty). Shows extra information on the message box.</param>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 13/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void processError(Exception e, Boolean showMessageBox, String customMessage, Boolean customMessageIsLanguageCode, List<String[]> extraInfo)
        {
            StringBuilder message = new StringBuilder();
            if (customMessageIsLanguageCode)
                customMessage = this.getLanguageString(customMessage);
            
            message.Append(customMessage);
            message.AppendLine();

            if(extraInfo != null)
                foreach(String[] s in extraInfo)
                    message.AppendLine(s[0] + ": " + s[1]);

            if (showMessageBox)
                this.fireMessageBox(message.ToString(), MessageBoxIcon.Error);

            this.writeToLog(message.ToString(), false, false);
            this.writeToLog(e.Message, false, true);
            this.writeToLog(e.StackTrace, false, true);
        }

        #endregion

        #region File processing

        /// <summary>
        ///    Removes the temporary folder.
        /// </summary>
        public void clearTempFolder()
        {
            Utility.deleteFolderAndContents(ProgramConstants.getTempPath());
        }

        public void createUserFolders()
        {
            String myDocs;
            if (!PortableIdentifier.Instance().isPortable)
                myDocs = ProgramConstants.getMyDocumentsPath();
            else
                myDocs = ProgramConstants.getProgramPath();
            String folderMySubjects = myDocs + this.getLanguageString("folder_mysubjects");
            String folderStatistics = folderMySubjects + @"\" + this.getLanguageString("folder_statistics");
            try
            {
                new DirectoryInfo(folderMySubjects).Create();
                new DirectoryInfo(folderStatistics).Create();

                this.settings.setSubjectsFolder(folderMySubjects);
                this.settings.setStatsFolder(folderStatistics);
            }
            catch (Exception ex)
            {
                this.processError(ex, true);
            }
        }

        #endregion

        #region Help and update methods

        /// <summary>
        ///     Downloads and opens the help file for THIS version.
        /// </summary>
        /// <remarks>
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        ///      -> Instead of downloading the latest version of the help file, 
        ///         we'll download the help file for this version.
        /// 
        ///     updated on 18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void openHelpFile()
        {
            Boolean start = true;
            String file_name = this.getLanguageString("help_file_name");

            String thisVersion = ProgramConstants.programVersion();

            // saved in "All Users appdata\LMS Desktop Assistant"
            String destination = ProgramConstants.getProgramStoragePath().TrimEnd('\\')
                                    + @"\"
                                    + file_name
                                    + thisVersion
                                    + ".pdf";

            if (!new FileInfo(destination).Exists)
                if (!Downloader.downloadFile(ProgramConstants.WEBSITE_LINK + file_name + thisVersion + ".pdf", destination))
                    start = false;

            if (start)
            {
                Utility.StartWithShell(destination);
            }
            else
            {
                this.fireMessageBox(this.getLanguageString("cannot_open_help"), MessageBoxIcon.Error);
            }
        }

        /// <summary>
        ///     Checks if there's a newer version of this program available.
        /// </summary>
        /// <returns>True if a new version is available.</returns>
        /// <remarks>
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        ///      -> added global variable.
        /// 
        ///     updated on 11/08/2010 by Gianni Van Hoecke
        ///      -> renamed from 'checkForUpdates'.
        ///      -> this function now returns the boolean.
        /// </remarks>
        public Boolean newUpdateAvailable()
        {
            this.latestVersion = Downloader.getLatestProgramVersion();
            if (this.latestVersion.Equals(String.Empty))
                throw new ApplicationException(this.getLanguageString("cannot_contact_server"));
            return Utility.isHigherNewVersion(ProgramConstants.programVersion(), this.latestVersion);
        }

        /// <summary>
        ///     Downloads and returns the change log of the latest version.
        /// </summary>
        /// <returns>The change log.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 20/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public String getChangeLogOfLatestVersion()
        {
            String destination = ProgramConstants.getProgramStoragePath().TrimEnd('\\') + @"\changelog.txt";
            if (Downloader.downloadFile(ProgramConstants.CHANGE_LOG, destination))
            {
                return ReadFile.getContentsOfFile(destination, true);
            }

            return this.getLanguageString("cannot_load_change_log");
        }

        /// <summary>
        ///     Gets the applictation version info (used for the updates).
        /// </summary>
        /// <returns>The info.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public String getAppVersionInfo()
        {
            return this.getLanguageString("app_name_new_version_info", new String[] { ProgramConstants.PROGRAM_NAME, ProgramConstants.programVersion(), this.latestVersion });
        }

        /// <summary>
        ///     This method will take the user to the update site.
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void goToUpdateSource()
        {
            Utility.StartWithShell(ProgramConstants.UPDATE_SOURCE);
        }

        #endregion

        #region All methods that return an object of a class

        /// <summary>
        ///     Returns the FileForUpload object.
        /// </summary>
        /// <returns>The object.</returns>
        public FileForUpload getFileForUpload()
        {
            return this.fileForUpload;
        }

        /// <summary>
        ///     Returns the program title.
        /// </summary>
        /// <returns>The title.</returns>
        public String getProgramTitle()
        {
            String versionString = ProgramConstants.PROGRAM_NAME + " v" + ProgramConstants.programVersion();
            if (PortableIdentifier.Instance().isPortable)
            {
                versionString+=" - " + getLanguageString("portable_version");
            }
            return versionString;
        }

        /// <summary>
        ///     Returns the Login object.
        /// </summary>
        /// <returns>The object.</returns>
        public UserInfo getUserInfo()
        {
            return this.userInfo;
        }

        /// <summary>
        ///     Returns the AbstractTargetPlatform object.
        /// </summary>
        /// <returns>The object.</returns>
        public TargetPlatform getPlatform()
        {
            return this.platform;
        }

        /// <summary>
        ///     Returns the Settings object.
        /// </summary>
        /// <returns>The object.</returns>
        public Settings getSettings()
        {
            return this.settings;
        }

        /// <summary>
        ///     Returns the Subject Settings object.
        /// </summary>
        /// <returns>The object.</returns>
        public SubjectSettings getSubjects()
        {
            return this.userInfo.getSubjects();
        }

        /// <summary>
        ///     Returns the DocumentFoldersList object.
        /// </summary>
        /// <returns>The object.</returns>
        public DocumentFoldersList getDocumentFoldersList()
        {
            return this.userInfo.getSelectedItemDocumentFolders();
        }

        /// <summary>
        ///     Returns the path to the temporary folder, with trailing backslash.
        /// </summary>
        /// <returns>The path to the temporary folder.</returns>
        public String getTempPath()
        {
            return this.tempPath;
        }

        /// <summary>
        ///     Returns whether this is the first time the program is ran.
        /// </summary>
        /// <returns>True if this is the first time the program is ran.</returns>
        public Boolean isfirstRun()
        {
            return this.firstRun;
        }
        /// <summary>
        ///     Initialize a synchronization.
        /// </summary>
        /// <param name="path">Path to folder.</param>
        public void initSynchronizationOperations(String path)
        {
            this.synchronizationOperations = new SynchronizationOperations(path);
        }

        /// <summary>
        ///     When called, the synchronization will stop after the current file has finished synchronizing.
        /// </summary>
        public void cancelSynchronization()
        {
            if (this.synchronizationOperations != null)
                this.synchronizationOperations.cancelSynchronisation = true;
        }

        /// <summary>
        ///     Returns the current loaded synchronization.
        /// </summary>
        /// <returns>The current loaded synchronization.</returns>
        public SynchronizationOperations getSynchronizationOperations()
        {
            return this.synchronizationOperations;
        }

        #endregion

        #region Getters and setters

        /// <summary>
        ///     Returns all courses for a dropdown menu.
        /// </summary>
        /// <returns>An array of courses.</returns>
        public String[] getCoursesListForDropDown()
        {
            return this.userInfo.getCoursesDropDownList();
        }

        /// <summary>
        ///     Returns all document folders of the selected course.
        /// </summary>
        /// <returns>An array of document folders.</returns>
        public String[] getDocumentFoldersForDropDown()
        {
            return this.userInfo.getSelectedItemDocumentFolders().getDocumentFoldersForDropDown();
        }

        /// <summary>
        ///     Returns the index of the selected course.
        /// </summary>
        /// <returns>The index of the selected course.</returns>
        public int getSelectedCourseDropdownIndex()
        {
            return this.userInfo.getSelectedItemIndex();
        }

        /// <summary>
        ///     Returns all opened documents.
        /// </summary>
        /// <returns>An array of all opened documents.</returns>
        public String[] getAllOpenDocuments()
        {
            return SupportedOfficeFiles.getAllActiveDocuments(DocumentType.PROCESSEDTEXT_DOCUMENT);
        }

        /// <summary>
        ///     Checks if a document is loaded.
        /// </summary>
        /// <returns>True if a document is loaded.</returns>
        public Boolean isDocumentSet()
        {
            return this.document != null;
        }

        /// <summary>
        ///     Returns the path to the loaded document.
        /// </summary>
        /// <returns>The path to the loaded document.</returns>
        public String getDocumentFullPath()
        {
            if (this.document != null)
                return this.document.getDocumentPathWithFilename();
            else
                return String.Empty;
        }

        /// <summary>
        ///     Returns the upload progress in a formatted string.
        /// </summary>
        /// <returns>The formatted progress.</returns>
        public String getUploadProgessOfFileUpload()
        {
            String progress = String.Empty;

            String totalBytes = String.Format("{0, -9}", Utility.getFormattedBytes(this.fileForUpload.getSelectedFileForUpload().Length));
            String processedBytes = String.Format("{0, -9}", Utility.getFormattedBytes(this.fileForUpload.getBytesProcessed()));
            progress = this.getLanguageString("sent_x_bytes_of_y", new String[] { processedBytes, totalBytes });

            return progress;
        }

        #endregion

    }
}