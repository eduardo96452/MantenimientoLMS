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
using lmsda.persistence.settings;
using System.IO;
using System.Security.Cryptography;
using lmsda.persistence.document;
using lmsda.domain.util;
using lmsda.domain.exercise;
using lmsda.persistence.httpcommunication;

namespace lmsda.domain.user.synchronization
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class handles the operations for the synchronization.
    /// </summary>
    class SynchronizationOperations
    {
        private String path;
        private SynchronizationSettings settings;
        public  Boolean cancelSynchronisation { get; set; }

        private DomainController dc;

        #region Constructor

        /// <summary>
        ///     Default constructor. Loads the settings file, if present. Reads the new files and new
        ///     folders inside the given path and adds them to the settingsfile.
        /// </summary>
        /// <param name="path">The full path to the synchronization folder.</param>
        public SynchronizationOperations(String path)
        {
            this.cancelSynchronisation = false;
            this.dc = DomainController.Instance();
            this.path = path.TrimEnd('\\');
            this.settings = new SynchronizationSettings(this.path + @"\" + ProgramConstants.SYNCHRONIZATION_FILE_NAME);
            this.readAllFiles(this.path);
            this.saveSettings();
        }

        #endregion

        #region Save and load

        /// <summary>
        ///     Saves the settings to the disk. This method will remove nonexistent files and folders.
        /// </summary>
        public void saveSettings()
        {
            //Remove deleted files...
            List<FileSettings> toRemove = new List<FileSettings>();
            foreach (FileSettings fs in this.settings.data)
            {
                Boolean remove = false;
                if (fs.isDirectory)
                {
                    if (!new DirectoryInfo(this.path + fs.relativeFileName).Exists)
                        remove = true;
                }
                else
                {
                    if (!new FileInfo(this.path + fs.relativeFileName).Exists)
                        remove = true;
                }
                if(remove)
                    toRemove.Add(fs);
            }
            foreach(FileSettings fsRemove in toRemove)
                this.settings.data.Remove(fsRemove);

            //Save
            this.settings.saveSynchronizationSettings();
        }

        /// <summary>
        ///     Reads all files in the folder, and adds them to the settings file if it is a new file.
        /// </summary>
        /// <param name="dirPath"></param>
        private Boolean readAllFiles(String dirPath)
        {
            Boolean folderExists = Utility.doesFileOrFolderExist(dirPath,true);
            if (!folderExists)
                return false;

            String relativeDirName = @"\" + new DirectoryInfo(dirPath).FullName.Replace(this.path, "").Trim('\\');

            if(this.getFileSettings(relativeDirName) == null)
                this.settings.addFile(new FileSettings(relativeDirName, true));

            DirectoryInfo[] dirs = new DirectoryInfo(dirPath).GetDirectories();

            foreach (DirectoryInfo dir in dirs)
            {
                this.readAllFiles(dir.FullName);
            }

            FileInfo[] files = new DirectoryInfo(dirPath).GetFiles();

            foreach (FileInfo file in files)
            {
                String relativeFileName = @"\" + file.FullName.Replace(this.path, "").Trim('\\');

                if(this.getFileSettings(relativeFileName) == null
                    && (file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden // bit compare
                    && !file.Name.Contains(ProgramConstants.SYNCHRONIZATION_FILE_NAME))
                    this.settings.addFile(new FileSettings(relativeFileName, false));
            }
            return true;
        }

        #endregion 

        #region Synchronization

        public void synchronizeSubject()
        {
            this.dc.writeToLog("starting_synchronization", true, false, false);
            this.synchronize(this.getData(),false);
        }

        public void markAsSynchronized(String relativeFileName)
        {
            this.dc.writeToLog("starting_synchronization_imitation", new String[] { relativeFileName }, true, false, false);
            List<FileSettings> subFilesList = getSubFilesList(relativeFileName);
            this.synchronize(subFilesList, true);
        }

        public void markAsUnsynchronized(String relativeFileName)
        {
            List<FileSettings> subFilesList = getSubFilesList(relativeFileName);
            foreach (FileSettings fs in subFilesList)
            {
                FileSettings fsSave = this.getFileSettings(fs.relativeFileName);
                fsSave.optionsChanged = false;
                fsSave.hasError = false;
                fsSave.lastHash = String.Empty;
                fsSave.resultHashes = String.Empty;
            }

        }

        /// <summary>
        ///     Starts the synchronization. New and modified files will be synchronized.
        /// </summary>
        /// <remarks>
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        ///      -> Added anonymous statistics.
        ///     
        ///     updated on 13/08/2010 by Gianni Van Hoecke
        ///      -> Files with previous sync errors are now processed anyway.
        ///     
        ///     updated on 11/08/2010 by Gianni Van Hoecke
        ///      -> added Excel to PDF conversion support.
        /// 
        ///     Updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> added PowerPoint to PDF conversion support.
        ///      
        ///     Updated on 09/03/2011 by Maarten Meuris
        ///      -> completely refactored the synchronization types, removed double checks on which files to process
        /// </remarks>
        /// <param name="synclist_process">list of file settings to synchronize</param>
        /// <param name="imitateUpload">if true, process the files and mark them as synchronized without uploading anything to the platform</param>
        private void synchronize(List<FileSettings> synclist, Boolean imitateUpload)
        {
            // make backup, to make sure the excluded files in the original data list
            // definitely don't get removed
            List<FileSettings> synclist_process = new List<FileSettings>(synclist);

            if (!Utility.doesFileOrFolderExist(this.path, true))
                return;
            List<FileSettings> addedFiles = new List<FileSettings>();
            DocumentFolder destinations;
            // newly added courses: empty by default
            List<Course> addedCoursesToSync;
            DocumentFolder addedDestinations;
            Boolean uploadError = false;
            Boolean uploadErrors = false;
            Boolean convertError = false;
            Boolean convertErrors = false;
            Boolean hasNewCourses = this.containsNewCourse();
            Subject subjectTosync = DomainController.Instance().getUserInfo().getSelectedSubject();

            int fileCounter = 0;
            int totalFiles = 0;

            //
            // Create list of excluded folders. These don't care about the given files list and always use the full settings.data list.
            //
            List<String> excludedFolders = getExcludedFolders();

            //
            // Filter out which files need updating
            //
            List<FileSettings> checksynclist = new List<FileSettings>(synclist_process);
            foreach (FileSettings fileSettings in checksynclist)
            {
                if (fileSettings.isDirectory                                  // don't process if... * is directory
                    || fileSettings.synchronizationType == SynchronizationType.EXCLUDE            // * is set not to upload
                    || (fileSettings.source != null && !fileSettings.source.Equals(String.Empty)) // * is pdf generated from a conversion; everything that changed is reconverted anyway
                    || Utility.isInFolderList(fileSettings.relativeFileName                       // * is in excluded folder
                                                   + (fileSettings.isDirectory? "\\": String.Empty), excludedFolders, '\\', true)
                    || (fileSettings.lastHash.Equals(this.calculateHashForFile(
                                                                fileSettings.relativeFileName))   // * the file's md5 hash is still the same
                         && !hasNewCourses               // ! don't exclude if hash is the same but there are new courses
                         && !fileSettings.optionsChanged // ! don't exclude if hash is the same but the file options changed
                         && !fileSettings.hasError)      // ! don't exclude if hash is the same but there were errors converting it last time
                                                         //   (As of 1.08. All files with errors will be processed again)
                    )
                {
                    synclist_process.Remove(fileSettings);
                }
            }
            totalFiles = synclist_process.Count;

            //
            // Separate courses and upload destinations to correctly determine where to upload/update content
            //
            
            // all courses of current Subject
            List<Course> coursesToSync = new List<Course>(this.dc.getUserInfo().getSelectedItemCourses());
            destinations = this.dc.getUserInfo().getSelectedItemDocumentFolders().getDocumentFolderFromFolderName("/");

            // The already-synchronized courses (for updating)
            List<Course> oldCoursesToSync;
            DocumentFolder oldDestinations;
            
            if (!hasNewCourses)
            {
                // define 'Added' lists as empty
                addedCoursesToSync = new List<Course>();
                addedDestinations = new DocumentFolder();
                //define 'Old' lists as full
                oldCoursesToSync = new List<Course>(coursesToSync);
                oldDestinations = new DocumentFolder(new List<DocumentFolderForCourse>(destinations.folders), destinations.folderName);
            }
            else
            {
                // fill 'Added' lists with all content
                addedCoursesToSync = new List<Course>(coursesToSync);
                addedDestinations = new DocumentFolder(new List<DocumentFolderForCourse>(destinations.folders), destinations.folderName);
                // Make 'Old' courses list empty
                oldCoursesToSync = new List<Course>();
                // Make 'Old' destinations list full
                oldDestinations = new DocumentFolder(new List<DocumentFolderForCourse>(destinations.folders), destinations.folderName);

                // loop through all previously-synced courses
                foreach (String coursecode in this.settings.synchronizedCourses)
                {
                    // get course for code
                    Course course = this.dc.getUserInfo().getLogin().getCourseFromCode(coursecode);
                    // if the course exists and is still linked to the Subject:
                    if (course != null && coursesToSync.Contains(course))
                    {
                        // Remove old courses from Added list so only new ones remain
                        addedCoursesToSync.Remove(course);
                        // Add course to Old list
                        oldCoursesToSync.Add(course);
                    }
                }

                // loop through all DocumentFolderForCourse objects in the full root folder object
                foreach (DocumentFolderForCourse df in destinations.folders)
                {
                    // remove folders of Old courses from Added list
                    if (oldCoursesToSync.Contains(df.course))
                        addedDestinations.folders.Remove(df);
                    // remove folders of Added courses from Old list
                    else
                        oldDestinations.folders.Remove(df);
                }
            }

            //
            // The actual synchronization loop
            //

            foreach (FileSettings fileSettings in synclist_process)
            {
                //Stop the synchronization if the user pressed 'cancel'.
                if (this.cancelSynchronisation)
                    break;
                uploadError = false;
                convertError = false;
                //Process
                String newMd5 = this.calculateHashForFile(fileSettings.relativeFileName);
                Boolean sameCRC = fileSettings.lastHash.Equals(newMd5);
                
                fileCounter++;
                this.dc.fireSynchronizationUpdate(fileCounter - 1, totalFiles);

                String convertedFileName = fileSettings.relativeFileName.Replace('\\', '/');
                //If the conversion and/or upload fails, the process will continue with the other files.
                try
                {
                    //
                    // actual file synchronization
                    //

                    switch (fileSettings.synchronizationType)
                    {
                        case SynchronizationType.CONVERT_TO_EXERCISE:
                            this.synchronize_ConvertToExercise(fileSettings,sameCRC,coursesToSync,oldCoursesToSync, addedCoursesToSync, subjectTosync, imitateUpload, ref convertError, ref uploadError); break;
                        case SynchronizationType.CONVERT_WORD_TO_PDF:
                            this.synchronize_ConvertDocumentToPDF(fileSettings,sameCRC,destinations,addedCoursesToSync, addedDestinations, subjectTosync, imitateUpload, ref convertError, ref uploadError, ref addedFiles); break;
                        case SynchronizationType.CONVERT_POWERPOINT_TO_PDF:
                            this.synchronize_ConvertPresentationToPDF(fileSettings,sameCRC,destinations,addedCoursesToSync, addedDestinations, subjectTosync, imitateUpload, ref convertError, ref uploadError, ref addedFiles); break;
                        case SynchronizationType.CONVERT_EXCEL_TO_PDF:
                            this.synchronize_ConvertSpreadsheetToPDF(fileSettings,sameCRC,destinations,addedCoursesToSync, addedDestinations, subjectTosync, imitateUpload, ref convertError, ref uploadError, ref addedFiles); break;
                        case SynchronizationType.UPLOAD:
                            this.synchronize_Upload(fileSettings, sameCRC, destinations, oldDestinations, addedCoursesToSync, addedDestinations, subjectTosync, imitateUpload, ref convertError, ref uploadError); break;
                    }
                    //Stop the synchronization if the user pressed 'cancel'.
                }
                catch (Exception e)
                {
                    this.dc.processError(e, false);
                    uploadError = true;
                }
                finally
                {
                    //
                    // Save this file's new sync data
                    //

                    // make sure the original synchronization data object is edited;
                    // apparently making a clone list can result in getting cloned list items too.
                    FileSettings fsSave = this.getFileSettings(fileSettings.relativeFileName);
                    // 'file not found' safeguard. Should normally never happen, but it's added just to be safe.
                    if (fsSave == null)
                    {
                        fsSave = fileSettings;
                        this.settings.addFile(fsSave);
                    }
                    // update to current md5 hash!
                    if (!uploadError) fsSave.lastHash = newMd5;
                    // include upload errors? Help says this should be enabled
                    // for all failed files, but is it useful for upload errors?
                    fsSave.hasError = convertError || uploadError;
                    fsSave.optionsChanged &= uploadError;
                    convertErrors|=convertError;
                    uploadErrors|=uploadError;
                }
            }
            
            // Update synchronized courses.
            // This is sort of incorrect with the "set synchronized" function, so it's not executed in case of dummy sync

            //if (!imitateUpload) {
            this.settings.clearSynchronizedCourses();
            foreach (Course course in DomainController.Instance().getUserInfo().getSelectedItemCourses())
                this.settings.addSynchronizedCourse(course.getCourseId());
            // }

            // add new files (like those created by PDF conversions) to actual data list in sync settings
            foreach (FileSettings newFileSettings in addedFiles)
            {
                this.settings.addFile(newFileSettings);
            }

            // Now that all files are processed, save.
            this.saveSettings();

            // Make sure UI shows final file count
            this.dc.fireSynchronizationUpdate(fileCounter + 1, totalFiles);

            //
            // Send finishing message to UI
            //

            if (uploadErrors)
            {
                this.dc.writeToLog("done_synchronizing_upload_errors", true, false, false);
                this.dc.fireMessageBox(this.dc.getLanguageString("done_synchronizing_upload_errors"), System.Windows.Forms.MessageBoxIcon.Error);
            }
            else if (convertErrors)
            {
                this.dc.writeToLog("done_synchronizing_errors_short", true, false, false);
                this.dc.fireMessageBox(this.dc.getLanguageString("done_synchronizing_errors"), System.Windows.Forms.MessageBoxIcon.Error);
            }
            else
            {
                this.dc.writeToLog("done_synchronizing", true, false, !imitateUpload);
            }
        }

        private void synchronize_ConvertToExercise(FileSettings fileSettings, Boolean sameCRC, List<Course> coursesToSync, List<Course> oldCoursesToSync, List<Course> addedCoursesToSync, Subject subjectTosync, Boolean imitateUpload, ref Boolean convertError, ref Boolean uploadError)
        {
            this.dc.isSynchronization = true;
            SupportedExercisesDocument exercisesdocument = SupportedOfficeFiles.ExercisesFactory(this.path + fileSettings.relativeFileName);
            if (this.dc.checkDocument(exercisesdocument, true, false))
            {
                //Stop the synchronization if the user pressed 'cancel'.
                if (this.cancelSynchronisation)
                    return;

                List<Exercise> uploadExercises = exercisesdocument.getExercises();
                List<Exercise> uploadExercisesModified = new List<Exercise>(exercisesdocument.getExercises());
                List<Exercise> uploadExercisesUnModified = new List<Exercise>();

                // temporary var for the received exercise MD5s
                List<String> exerciseMD5sGet = exercisesdocument.getExerciseMD5s();
                // normally processed list of MD5s
                List<String> exerciseMD5sNew;
                // extra processed list of legacy MD5s; is null if no extra list is given.
                // To remove this legacy support, change the getExerciseMD5s function in the MicrosoftWordDocument class
                // to call "getMD5" instead of "getMD5LegacySupport".
                List<String> exerciseMD5sNewLegacy;

                // checks for legacy support: md5 string of an exercise containing two md5s split by a space;
                // the first being the new untranslated type, and the second the legacy one with translated strings in it.
                if (exerciseMD5sGet.Count > 0 && !exerciseMD5sGet[0].Contains(' '))
                {
                    exerciseMD5sNew = new List<String>(exerciseMD5sGet);
                    exerciseMD5sNewLegacy = null;
                }
                else
                {
                    exerciseMD5sNew = new List<String>();
                    exerciseMD5sNewLegacy = new List<String>();
                    foreach (String s in exerciseMD5sGet)
                    {
                        String[] split = s.Split(new char[] { ' ' });
                        if (split.Length >= 2)
                        {
                            // split into new and legacy-new md5s
                            exerciseMD5sNew.Add(split[0]);
                            exerciseMD5sNewLegacy.Add(split[1]);
                        }
                    }
                }

                List<String> exerciseMD5sOld = new List<String>(fileSettings.resultHashes.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                exerciseMD5sOld.Sort();

                for (int i = 0; i < uploadExercises.Count; i++)
                {
                    uploadExercises[i].setMultipage(fileSettings.oneQuestionPerPage);
                    // check both new and legacy-new md5
                    if (exerciseMD5sOld.Contains(exerciseMD5sNew[i]) || (exerciseMD5sNewLegacy != null && exerciseMD5sOld.Contains(exerciseMD5sNewLegacy[i])))
                    {
                        uploadExercisesModified.Remove(uploadExercises[i]);
                        uploadExercisesUnModified.Add(uploadExercises[i]);
                    }
                }
                exerciseMD5sNew.Sort();
                String exerciseMD5String = String.Empty;
                foreach (String md5 in exerciseMD5sNew)
                    exerciseMD5String += md5 + " ";
                exerciseMD5String = exerciseMD5String.TrimEnd();

                String exerciseMD5LegacyString = String.Empty;
                if (exerciseMD5sNewLegacy != null)
                {
                    exerciseMD5sNewLegacy.Sort();
                    foreach (String md5 in exerciseMD5sNewLegacy)
                        exerciseMD5LegacyString += md5 + " ";
                    exerciseMD5LegacyString = exerciseMD5LegacyString.TrimEnd();
                }

                // sameCRC = true if either the new or the legacy-new hashes match.
                if (exerciseMD5String.Equals(fileSettings.resultHashes) || (exerciseMD5sNewLegacy != null && exerciseMD5LegacyString.Equals(fileSettings.resultHashes)))
                    sameCRC = true;


                //
                // if not dummy operation, upload exercises
                //

                if (!imitateUpload)
                {
                    // some exercises are modified
                    if (!sameCRC)
                    {
                        // upload UNMODIFIED exercises to any ADDED courses
                        if (addedCoursesToSync.Count > 0 && uploadExercisesUnModified.Count > 0)
                        {
                            if (!this.dc.getPlatform().doUploadExercises(uploadExercisesUnModified,
                                        addedCoursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                        fileSettings.randomQuestions, false))
                                uploadError = true;
                            else
                                AnonymousStatistics.sendStatistics(uploadExercisesUnModified);
                        }
                        // upload MODIFIED exercises to ALL courses
                        if (uploadExercisesModified.Count > 0)
                        {
                            if (!this.dc.getPlatform().doUploadExercises(uploadExercisesModified,
                                            coursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                            fileSettings.randomQuestions, false))
                                uploadError = true;
                            else
                                AnonymousStatistics.sendStatistics(uploadExercisesModified);
                        }
                        // If exercise options changed, update UNMODIFIED
                        if (fileSettings.optionsChanged)
                        {
                            if (!this.dc.getPlatform().doUpdateExercisesInfo(uploadExercisesModified,
                                            oldCoursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                            fileSettings.randomQuestions))
                                uploadError = true;
                            else
                                AnonymousStatistics.sendStatistics(uploadExercisesModified);
                        }
                    }
                    // exercises are unmodified: upload ALL exercises to any ADDED courses.
                    else
                    {
                        if (addedCoursesToSync.Count > 0)
                        {
                            if (!this.dc.getPlatform().doUploadExercises(exercisesdocument.getExercises(),
                                        addedCoursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                        fileSettings.randomQuestions, false))
                                uploadError = true;
                            else
                                AnonymousStatistics.sendStatistics(exercisesdocument.getExercises());
                        }
                        if (fileSettings.optionsChanged)
                        {
                            // Update all exercise settings on the courses that already have them.
                            if (!this.dc.getPlatform().doUpdateExercisesInfo(exercisesdocument.getExercises(),
                                        oldCoursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                        fileSettings.randomQuestions)
                                // if any of the updates failed, try uploading, but without overwrite
                                && !this.dc.getPlatform().doUploadExercises(exercisesdocument.getExercises(),
                                        oldCoursesToSync, subjectTosync, fileSettings.setExerciseInvisible,
                                        fileSettings.randomQuestions, true))
                                uploadError = true;
                            else
                                AnonymousStatistics.sendStatistics(exercisesdocument.getExercises());
                        }
                    }
                }

                if (!uploadError)
                {
                    fileSettings.resultHashes = exerciseMD5String;
                    fileSettings.optionsChanged = false;
                }
            }
            else
            {
                DomainController.Instance().writeToLog("document_x_contains_errors", new String[] { Path.GetFileName(this.path + fileSettings.relativeFileName) }, true, false, false);
                convertError = true;
                fileSettings.resultHashes = String.Empty;
            }
            this.dc.isSynchronization = false;
        }

        private void synchronize_ConvertDocumentToPDF(FileSettings fileSettings, Boolean sameCRC, DocumentFolder destinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Subject subjectTosync, Boolean imitateUpload, ref Boolean convertError, ref Boolean uploadError, ref List<FileSettings> addedFiles)
        {
            this.dc.isSynchronization = true;
            String convertedFileName = fileSettings.relativeFileName.Replace('\\', '/');
            String newFolderName = convertedFileName.Substring(0, convertedFileName.LastIndexOf('/')).Trim('/');
            List<String> uploadParts;

            // set split pattern.

            if (fileSettings.splitOnStyle
                && (fileSettings.pdfNameTemplate == null || fileSettings.pdfNameTemplate.Equals(string.Empty)))
                fileSettings.pdfNameTemplate = this.dc.getSettings().getPDFFileName();

            // only convert, without upload

            uploadParts = this.dc.convertToPDF(SupportedOfficeFiles.ExercisesFactory(this.path + fileSettings.relativeFileName), fileSettings.splitOnStyle, fileSettings.pdfNameTemplate, fileSettings.splitString, fileSettings.splitPerPage, fileSettings.convertToJavascript, fileSettings.setPDFInvisible, ref convertError);

            //Stop the synchronization if the user pressed 'cancel'.
            if (this.cancelSynchronisation)
                return;

            // make MD5s list
            if (!convertError)
                uploadError = uploadError || this.uploadPDF(uploadParts, fileSettings, sameCRC, newFolderName, destinations, addedCoursesToSync, addedDestinations, imitateUpload, ref addedFiles);

            this.dc.isSynchronization = false;
        }

        private void synchronize_ConvertPresentationToPDF(FileSettings fileSettings, Boolean sameCRC, DocumentFolder destinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Subject subjectTosync, Boolean imitateUpload, ref Boolean convertError, ref Boolean uploadError, ref List<FileSettings> addedFiles)
        {
            this.dc.isSynchronization = true;
            String convertedFileName = fileSettings.relativeFileName.Replace('\\', '/');
            String newFolderName = convertedFileName.Substring(0, convertedFileName.LastIndexOf('/')).Trim('/');
            String filepath = this.path + fileSettings.relativeFileName;
            List<String> uploadParts;

            // only convert, without upload
            uploadParts = this.dc.convertPresentationToPDF(filepath, Path.GetDirectoryName(filepath), fileSettings.frameSlides, fileSettings.horizontal, fileSettings.publishMethod, fileSettings.slidesPerPage, fileSettings.includeHiddenSlides, ref convertError);

            //Stop the synchronization if the user pressed 'cancel'.
            if (this.cancelSynchronisation)
                return;

            // Uploads the files, and adds them to the added files list, with md5 hash
            if (!convertError)
            {
                Boolean uploaderr = this.uploadPDF(uploadParts, fileSettings, sameCRC, newFolderName, destinations, addedCoursesToSync, addedDestinations, imitateUpload, ref addedFiles);
                uploadError = uploadError || uploaderr;
            }

            this.dc.isSynchronization = false;
        }

        private void synchronize_ConvertSpreadsheetToPDF(FileSettings fileSettings, Boolean sameCRC, DocumentFolder destinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Subject subjectTosync, Boolean imitateUpload, ref Boolean convertError, ref Boolean uploadError, ref List<FileSettings> addedFiles)
        {
            this.dc.isSynchronization = true;
            String convertedFileName = fileSettings.relativeFileName.Replace('\\', '/');
            String newFolderName = convertedFileName.Substring(0, convertedFileName.LastIndexOf('/')).Trim('/');
            String filepath = this.path + fileSettings.relativeFileName;
            List<String> uploadParts;

            // only convert, without upload
            uploadParts = this.dc.convertExcelToPDF(filepath, Path.GetDirectoryName(filepath), ref convertError);

            //Stop the synchronization if the user pressed 'cancel'.
            if (this.cancelSynchronisation)
                return;

            // Uploads the files, and adds them to the added files list, with md5 hash
            if (!convertError)
            {
                Boolean uploaderr = this.uploadPDF(uploadParts, fileSettings, sameCRC, newFolderName, destinations, addedCoursesToSync, addedDestinations, imitateUpload, ref addedFiles);
                uploadError = uploadError || uploaderr;
            }
            this.dc.isSynchronization = false;
        }

        private void removePreviousResultFiles(FileSettings fileSettings, DocumentFolder destinations, DocumentFolder oldDestinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Subject subjectTosync, Boolean imitateUpload)
        {
            // deleting old converted files from platform.
            // Should be possible by going over all files and check on source property,
            // and delete those that are not in the freshly converted files list.

            // not yet implemented or used, mainly because Domaincontroller doesn't
            // have a function for deletion on platform. Might not be worth the work.
        }

        private void synchronize_Upload(FileSettings fileSettings, Boolean sameCRC, DocumentFolder destinations, DocumentFolder oldDestinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Subject subjectTosync, Boolean imitateUpload, ref Boolean convertError, ref Boolean uploadError)
        {
            if (imitateUpload)
                return; // this is nothing but upload; simply abort.
            this.dc.isSynchronization = true;
            String convertedFileName = fileSettings.relativeFileName.Replace('\\', '/');
            String folderName = convertedFileName.Substring(0, convertedFileName.LastIndexOf('/')).Trim('/');
            String fullFileName = this.path + fileSettings.relativeFileName;
            String fileName = convertedFileName.Substring(convertedFileName.LastIndexOf('/')).Trim('/');
            DocumentFolder publishDestination;

            // file is modified: upload file to ALL courses.
            if (!sameCRC)
            {
                if (!folderName.Equals(String.Empty)) // file  will be uploaded to a subfolder
                    publishDestination = this.dc.createNewDocumentsFolder(destinations, folderName, false);
                else
                    publishDestination = destinations;
                String[] uploaded = this.dc.uploadFile(fullFileName, publishDestination, fileSettings.fileDescription, fileSettings.setFileInvisible);
                if (uploaded == null || uploaded.Count() == 0)
                    uploadError = true;
            }
            // File unmodified
            else
            {
                // courses ADDED: upload file to ADDED courses
                if (addedCoursesToSync.Count() > 0)
                {
                    if (!folderName.Equals(String.Empty)) // file  will be uploaded to a subfolder
                        publishDestination = this.dc.createNewDocumentsFolder(addedDestinations, folderName, false);
                    else
                        publishDestination = addedDestinations;
                    String[] uploaded1 = this.dc.uploadFile(fullFileName, publishDestination, fileSettings.fileDescription, fileSettings.setFileInvisible);
                    if (uploaded1 == null || uploaded1.Count() == 0)
                        uploadError = true;
                }

                // file settings are MODIFIED: update file on OLD courses
                if (fileSettings.optionsChanged)
                {
                    // update file in OLD courses
                    if (!folderName.Equals(String.Empty)) // file  will be uploaded to a subfolder
                        publishDestination = this.dc.createNewDocumentsFolder(oldDestinations, folderName, fileSettings.setFileInvisible);
                    else
                        publishDestination = oldDestinations;
                    Boolean updated = this.dc.updateFile(fileName, publishDestination, fileSettings.fileDescription, !fileSettings.setFileInvisible);

                    // if update failed, try reupload
                    if (!updated)
                    {
                        String[] uploaded1 = this.dc.uploadFile(fullFileName, publishDestination, fileSettings.fileDescription, fileSettings.setFileInvisible);
                        if (uploaded1 == null || uploaded1.Count() == 0)
                            uploadError = true;
                    }
                }
            }
            this.dc.isSynchronization = false;
        }

        /// <summary>
        ///     Uploads already converted PDF's to the platform.
        /// </summary>
        /// <param name="uploadParts">A list containing the converted files.</param>
        /// <param name="fileSettings">The FileSettings object.</param>
        /// <param name="sameCRC">Indicates whether it has the same CRC.</param>
        /// <param name="newFolderName">The new folder name.</param>
        /// <returns>True if an upload error occurred.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 11/08/2010 by Gianni Van Hoecke
        ///      -> Moved from inline code when converting a Word document to a private method so Excel and PowerPoint
        ///         can benefit from the same actions.
        /// </remarks>
        private Boolean uploadPDF(List<String> uploadParts, FileSettings fileSettings, Boolean sameCRC, String newFolderName, DocumentFolder destinations, List<Course> addedCoursesToSync, DocumentFolder addedDestinations, Boolean imitateUpload, ref List<FileSettings> addedFiles)
        {
            Boolean uploadError = false;

            if (uploadParts.Count > 0)
            {
                List<String> uploadPartsModified = new List<String>(uploadParts);
                List<String> uploadPartsUnModified = new List<String>();
                List<String> pdfMD5s = new List<string>();
                List<String> pdfMD5sOld = new List<String>();

                foreach (String doc in uploadParts)
                {
                    Boolean isNewFileSettings = false;
                    String newRelFileName = @"\" + doc.Replace(this.path, "").Trim('\\');
                    // calculates hash for PDF that ignores generated stamps
                    String newHash = this.calculateHashForPDFFile(newRelFileName);
                    FileSettings newFileSettings = this.getFileSettings(newRelFileName);
                    if (newFileSettings == null)
                    {
                        newFileSettings = new FileSettings(newRelFileName);
                        isNewFileSettings = true;
                    }
                    else
                    {
                        pdfMD5sOld.Add(newFileSettings.lastHash);
                    }
                    newFileSettings.source = fileSettings.relativeFileName;
                    newFileSettings.lastHash = newHash;
                    newFileSettings.synchronizationType = SynchronizationType.EXCLUDE;
                    pdfMD5s.Add(newHash);
                    if (isNewFileSettings)
                        addedFiles.Add(newFileSettings);
                }

                // do not filter out unmodified documents if file is set as "newly added".
                if (!fileSettings.lastHash.Equals(String.Empty) && pdfMD5sOld.Count > 0)
                {
                    for (int i = 0; i < uploadParts.Count; i++)
                    {
                        if (pdfMD5sOld.Contains(pdfMD5s[i]))
                        {
                            uploadPartsModified.Remove(uploadParts[i]);
                            uploadPartsUnModified.Add(uploadParts[i]);
                        }
                    }
                }


                if (uploadPartsUnModified.Count == uploadParts.Count)
                    sameCRC = true;
                DocumentFolder publishDestination;

                //
                // if not dummy operation, upload the files.
                //

                if (!imitateUpload)
                {
                    // some PDF parts are modified
                    if (!sameCRC)
                    {
                        // upload UNMODIFIED PDF parts to any ADDED courses.
                        if (addedCoursesToSync.Count() > 0 && uploadPartsUnModified.Count > 0)
                        {
                            if (!newFolderName.Equals(String.Empty)) // PDF will be uploaded to subfolder
                                publishDestination = this.dc.createNewDocumentsFolder(addedDestinations, newFolderName, false);
                            else
                                publishDestination = addedDestinations;
                            if (!this.dc.uploadPDFs(uploadPartsUnModified, publishDestination, fileSettings.setPDFInvisible))
                                uploadError = true;
                        }
                        // (re)upload MODIFIED PDF parts to ALL courses.
                        if (!newFolderName.Equals(String.Empty)) // PDF will be uploaded to subfolder
                            publishDestination = this.dc.createNewDocumentsFolder(destinations, newFolderName, false);
                        else
                            publishDestination = destinations;
                        if (!this.dc.uploadPDFs(uploadPartsModified, publishDestination, fileSettings.setPDFInvisible))
                            uploadError = true;
                    }
                    // No PDF parts are modified
                    else
                    {
                        // upload ALL PDF parts to ADDED courses
                        if (addedCoursesToSync.Count() > 0)
                        {
                            if (!newFolderName.Equals(String.Empty)) // PDF will be uploaded to subfolder
                                publishDestination = dc.createNewDocumentsFolder(addedDestinations, newFolderName, false);
                            else
                                publishDestination = addedDestinations;
                            if (!dc.uploadPDFs(uploadParts, publishDestination, fileSettings.setPDFInvisible))
                                uploadError = true;
                        }
                    }
                }
            }
            else
            {
                // convertError spawns an error about Exercises, so this is disabled for now.
                //convertError = true;
                fileSettings.resultHashes = String.Empty;
            }
            return uploadError;
        }


        public List<String> getExcludedFolders()
        {
            List<String> excludedFolders = new List<String>();
            foreach (FileSettings fileSettings in this.settings.data)
            {
                if (fileSettings.isDirectory && fileSettings.synchronizationType == SynchronizationType.EXCLUDE)
                    excludedFolders.Add(fileSettings.relativeFileName);
            }
            return excludedFolders;
        }


        /// <summary>
        ///     Gets a list of FileSettings objects based on one relative filename.
        ///     If the relative filename is a file, the returned list will contain
        ///     the FileSettings objects of that file, if it is not excluded.
        ///     If the relative filename is a folder, the returned list will contain
        ///     the FileSettings objects of all non-excluded files in that folder.
        /// </summary>
        /// <param name="relativeFileName">Relative path to get a FileSettings list for</param>
        /// <returns></returns>
        public List<FileSettings> getSubFilesList(String relativeFileName)
        {
            FileSettings selectedfs = this.getFileSettings(relativeFileName);
            List<String> excludedFolders = this.getExcludedFolders();
            List<FileSettings> data;
            List<FileSettings> subFilesList = new List<FileSettings>();
            List<String> folders;
            if (!selectedfs.isDirectory)
            {
                folders = new List<String>() { selectedfs.relativeFileName
                                    .Substring(0, selectedfs.relativeFileName.LastIndexOf('\\'))};
                data = new List<FileSettings>() { selectedfs };
            }
            else
            {
                folders = new List<String>() { selectedfs.relativeFileName };
                data = this.getData();
            }
            foreach (FileSettings file in data)
                if (!file.isDirectory &&
                    !Utility.isInFolderList(file.relativeFileName, excludedFolders, '\\', true) &&
                    Utility.isInFolderList(file.relativeFileName, folders, '\\', true))
                    subFilesList.Add(file);

            return subFilesList;
        }


        /// <summary>
        ///     Gets all items to be changed based on one relative path;
        ///     if the path is a file it just returns the  FileSettings object of it,
        ///     if it is a folder it returns all files inside the folder that need
        ///     to be synchronized.
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <returns></returns>
        public List<FileSettings> getSyncList(String relativeFileName)
        {
            List<FileSettings> markSyncedList = new List<FileSettings>();
            FileSettings fs = this.getFileSettings(relativeFileName);
            if (!fs.isDirectory)
                markSyncedList.Add(fs);
            else
            {
                List<String> folders = new List<String>() { fs.relativeFileName };
                List<FileSettings> data = this.getData();
                List<String> excludedFolders = this.getExcludedFolders();
                foreach (FileSettings file in data)
                    if (file.synchronizationType != SynchronizationType.EXCLUDE &&
                        !(file.source != null && !file.source.Equals(String.Empty)) &&
                        !file.isDirectory &&
                        !Utility.isInFolderList(file.relativeFileName, excludedFolders, '\\', true) &&
                        Utility.isInFolderList(file.relativeFileName, folders, '\\', true))
                        markSyncedList.Add(file);
            }
            return markSyncedList;
        }



        /// <summary>
        ///     Calculates and returns the MD5 hash value of a file.
        /// </summary>
        /// <param name="relativeFileName">The relative path.</param>
        /// <returns>The MD5 hash.</returns>
        private String calculateHashForFile(String relativeFileName)
        {
            String path = this.path + relativeFileName;
            return Utility.calculateMD5ForFile(path);
        }

        /// <summary>
        ///     Calculates and returns the MD5 hash value of a PDF file (with time stamps filtered out).
        /// </summary>
        /// <param name="relativeFileName">The relative path.</param>
        /// <returns>The MD5 hash.</returns>
        private String calculateHashForPDFFile(String relativeFileName)
        {
            String path = this.path + relativeFileName;
            return Utility.calculateMD5ForPDFFile(path);
        }

        #endregion

        #region Getters and setters
      
        /// <summary>
        ///     Returns the synchronization status of the given file.
        ///     Author: Maarten Meuris
        /// </summary>
        /// <param name="name">The relative name of the file.</param>
        /// <returns>The status of that file.</returns>
        public SynchronizationStatus getFileStatus(String file)
        {
            FileSettings fs = this.getFileSettings(file);
            return getFileStatus(fs);
        }

        /// <summary>
        ///     Returns the synchronization status of the given file.
        ///     Author: Maarten Meuris
        /// </summary>
        /// <param name="name">FileSettings object.</param>
        /// <returns>The status of that file.</returns>
        public SynchronizationStatus getFileStatus(FileSettings fs)
        {
            String currentcrc;
            if (fs == null)
                return SynchronizationStatus.UNKNOWN;
            if (fs.isDirectory)
            {
                if (fs.synchronizationType == SynchronizationType.EXCLUDE)
                    return SynchronizationStatus.FOLDER_EXCLUDE;
                else
                    return SynchronizationStatus.FOLDER_UPLOAD;
            }
            else
            {
                // Converted files are also set to EXCLUDE, so this check HAS to happen first.
                if (fs.source != null && !fs.source.Equals(String.Empty))
                    return SynchronizationStatus.FILE_CONVERTEDPDF;
                else if (fs.synchronizationType == SynchronizationType.EXCLUDE)
                    return SynchronizationStatus.FILE_EXCLUDED;
                // The "optionsChanged" check makes PDFs with a different split pattern
                // appear as "modified" even though their md5 is cleared.
                // To the user, this should appear as "modified", but in reality
                // they produce completely new file names so they are processed as New.

                else if (fs.lastHash.Equals(String.Empty) && !fs.optionsChanged)
                {
                    if (fs.hasError)
                        return SynchronizationStatus.FILE_ERROR;
                    else
                        return SynchronizationStatus.FILE_ADDED;
                }
                else
                {
                    currentcrc = this.calculateHashForFile(fs.relativeFileName);
                    if (fs.lastHash.Equals(currentcrc))
                    {
                        if (fs.hasError)
                            return SynchronizationStatus.FILE_ERROR;
                        else if (fs.optionsChanged)
                            return SynchronizationStatus.FILE_CHANGED;
                        else 
                            return SynchronizationStatus.FILE_SYNCHRONYZED;
                    }
                    else
                    {
                        return SynchronizationStatus.FILE_CHANGED;
                    }
                }
            }
        }

        /// <summary>
        ///     Returns the settings object of the given file. Returns null if not found.
        /// </summary>
        /// <param name="name">The relative name of the file.</param>
        /// <returns>The settings of that file.</returns>
        public FileSettings getFileSettings(String relativeFileName)
        {
            foreach (FileSettings fs in this.settings.data)
            {
                if (fs.relativeFileName.Equals(relativeFileName))
                    return fs;
            }
            return null;
        }

        /// <summary>
        ///     Returns the FileSettings data collection.
        /// </summary>
        /// <returns>The FileSettings data collection in a list.</returns>
        public List<FileSettings> getData()
        {
            return this.settings.data;
        }

        /// <summary>
        ///     This method checks if a new course was added to the subject.
        /// </summary>
        /// <returns>True if a new course is added.</returns>
        public Boolean containsNewCourse()
        {
            Boolean containsNewCourse = false;

            foreach (Course course in DomainController.Instance().getUserInfo().getSelectedItemCourses())
            {
                if(!this.settings.synchronizedCourses.Contains(course.getCourseId()))
                    containsNewCourse = true;
            }

            return containsNewCourse;
        }
        /// <summary>
        ///     This method checks if the subject still contains any of the courses it last synchronized to
        /// </summary>
        /// <returns>True if there are still synchronized course in the subject.</returns>
        public Boolean containsSyncedCourses()
        {
            Boolean containsSyncedCourses = false;

            foreach (Course course in DomainController.Instance().getUserInfo().getSelectedItemCourses())
            {
                if(this.settings.synchronizedCourses.Contains(course.getCourseId()))
                    containsSyncedCourses = true;
            }

            return containsSyncedCourses;
        }
        
        #endregion

        #region public file settings operations

        /// <summary>
        ///     Applies the settings of a file to all files in the same folder. Files in subfolders are unaffected.
        /// </summary>
        /// <param name="fileSettings">The settings of the file.</param>
        /// <remarks>
        ///     Last updated on 11/08/2010 by Gianni Van Hoecke
        ///      -> Changed the "!=" to "!Equals" when comparing strings.
        ///      -> Added PowerPoint to PDF and Excel to PDF support.
        ///      -> Added "fs.optionsChanged = true;" when "fs.synchronizationType != fileSettings.synchronizationType"
        /// </remarks>
        public void applyToAllFolderItems(FileSettings fileSettings)
        {
            String relativeFolderName = fileSettings.relativeFileName.Substring(0, fileSettings.relativeFileName.LastIndexOf('\\')) + @"\";
            String file;

            foreach (FileSettings fs in this.settings.data)
            {
                if (fs.relativeFileName.StartsWith(relativeFolderName) && !fs.relativeFileName.Equals(relativeFolderName)
                    && !fs.relativeFileName.Equals(fileSettings.relativeFileName)
                    && !fs.isDirectory
                    && !fs.relativeFileName.Substring(relativeFolderName.Length).Contains('\\'))
                {
                    file = this.path + fs.relativeFileName;
                    switch (fileSettings.synchronizationType)
                    {
                        case SynchronizationType.CONVERT_TO_EXERCISE:
                            //Only set if it is a supported document.
                            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.PROCESSEDTEXT_DOCUMENT)
                            {
                                if (fs.synchronizationType != fileSettings.synchronizationType)
                                {
                                    fs.synchronizationType = fileSettings.synchronizationType;
                                    fs.lastHash = String.Empty;

                                    fs.synchronizationType = fileSettings.synchronizationType;
                                    fs.oneQuestionPerPage = fileSettings.oneQuestionPerPage;
                                    fs.randomQuestions = fileSettings.randomQuestions;
                                    fs.setExerciseInvisible = fileSettings.setExerciseInvisible;
                                    fs.optionsChanged = true;
                                }
                                else
                                {
                                    if (fs.oneQuestionPerPage != fileSettings.oneQuestionPerPage)
                                    {
                                        fs.oneQuestionPerPage = fileSettings.oneQuestionPerPage;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.randomQuestions != fileSettings.randomQuestions)
                                    {
                                        fs.randomQuestions = fileSettings.randomQuestions;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.setExerciseInvisible != fileSettings.setExerciseInvisible)
                                    {
                                        fs.setExerciseInvisible = fileSettings.setExerciseInvisible;
                                        fs.optionsChanged = true;
                                    }
                                }
                            }
                            //else: leave file as it is
                            break;
                        case SynchronizationType.CONVERT_WORD_TO_PDF:
                            
                            //Only set if it is a supported document.
                            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.PROCESSEDTEXT_DOCUMENT)
                            {
                                if (fs.synchronizationType != fileSettings.synchronizationType)
                                {
                                    fs.synchronizationType = fileSettings.synchronizationType;
                                    fs.lastHash = String.Empty;

                                    fs.splitOnStyle = fileSettings.splitOnStyle;
                                    fs.splitString = fileSettings.splitString;
                                    fs.splitPerPage = fileSettings.splitPerPage;
                                    fs.pdfNameTemplate = fileSettings.pdfNameTemplate;
                                    fs.convertToJavascript = fileSettings.convertToJavascript;
                                    fs.setPDFInvisible = fileSettings.setPDFInvisible;
                                    fs.optionsChanged = true;
                                }
                                else
                                {
                                    if (fs.splitOnStyle != fileSettings.splitOnStyle)
                                    {
                                        fs.splitOnStyle = fileSettings.splitOnStyle;
                                        fs.optionsChanged = true;
                                    }
                                    if (!fs.splitString.Equals(fileSettings.splitString))
                                    {
                                        fs.splitString = fileSettings.splitString;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.splitPerPage != fileSettings.splitPerPage)
                                    {
                                        fs.splitPerPage = fileSettings.splitPerPage;
                                        fs.optionsChanged = true;
                                    }
                                    if (!fs.pdfNameTemplate.Equals(fileSettings.pdfNameTemplate))
                                    {
                                        fs.pdfNameTemplate = fileSettings.pdfNameTemplate;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.convertToJavascript != fileSettings.convertToJavascript)
                                    {
                                        fs.convertToJavascript = fileSettings.convertToJavascript;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.setPDFInvisible != fileSettings.setPDFInvisible)
                                    {
                                        fs.setPDFInvisible = fileSettings.setPDFInvisible;
                                        fs.optionsChanged = true;
                                    }
                                }
                            }
                            //else: leave file as it is
                            break;
                        case SynchronizationType.CONVERT_POWERPOINT_TO_PDF:
                            //As of 1.08 by Gianni Van Hoecke
                            //Only set if it is a supported document.
                            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.PRESENTATION_DOCUMENT)
                            {
                                if (fs.synchronizationType != fileSettings.synchronizationType)
                                {
                                    fs.synchronizationType = fileSettings.synchronizationType;
                                    fs.lastHash = String.Empty;

                                    fs.frameSlides = fileSettings.frameSlides;
                                    fs.horizontal = fileSettings.horizontal;
                                    fs.includeHiddenSlides = fileSettings.includeHiddenSlides;
                                    fs.publishMethod = fileSettings.publishMethod;
                                    fs.slidesPerPage = fileSettings.slidesPerPage;
                                    fs.optionsChanged = true;
                                }
                                else
                                {
                                    if (fs.frameSlides != fileSettings.frameSlides)
                                    {
                                        fs.frameSlides = fileSettings.frameSlides;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.horizontal != fileSettings.horizontal)
                                    {
                                        fs.horizontal = fileSettings.horizontal;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.includeHiddenSlides != fileSettings.includeHiddenSlides)
                                    {
                                        fs.includeHiddenSlides = fileSettings.includeHiddenSlides;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.publishMethod != fileSettings.publishMethod)
                                    {
                                        fs.publishMethod = fileSettings.publishMethod;
                                        fs.optionsChanged = true;
                                    }
                                    if (fs.slidesPerPage != fileSettings.slidesPerPage)
                                    {
                                        fs.slidesPerPage = fileSettings.slidesPerPage;
                                        fs.optionsChanged = true;
                                    }
                                }
                            }
                            //else: leave file as it is
                            break;
                        case SynchronizationType.CONVERT_EXCEL_TO_PDF:
                            //As of 1.08 by Gianni Van Hoecke
                            
                            //Only set if it is a supported document.
                            if (SupportedOfficeFiles.identifyDocumentType(file) == DocumentType.SPREADSHEET_DOCUMENT)
                            {
                                if (fs.synchronizationType != fileSettings.synchronizationType)
                                {
                                    fs.synchronizationType = fileSettings.synchronizationType;
                                    fs.lastHash = String.Empty;
                                    fs.optionsChanged = true;
                                    //No other parameters available.
                                }
                            }
                            //else: leave file as it is
                            break;
                        case SynchronizationType.UPLOAD:
                            if (fs.synchronizationType != fileSettings.synchronizationType)
                            {
                                fs.synchronizationType = fileSettings.synchronizationType;
                                fs.lastHash = String.Empty;

                                fs.fileDescription = fileSettings.fileDescription;
                                fs.setFileInvisible = fileSettings.setFileInvisible;
                                fs.optionsChanged = true;
                            }
                            else
                            {
                                if (!fs.fileDescription.Equals(fileSettings.fileDescription))
                                {
                                    fs.fileDescription = fileSettings.fileDescription;
                                    fs.optionsChanged = true;
                                }
                                if (fs.setFileInvisible != fileSettings.setFileInvisible)
                                {
                                    fs.setFileInvisible = fileSettings.setFileInvisible;
                                    fs.optionsChanged = true;
                                }
                            }
                            break;
                        default:
                            if (fs.synchronizationType != fileSettings.synchronizationType)
                            {
                                fs.synchronizationType = fileSettings.synchronizationType;
                                fs.lastHash = String.Empty;
                                fs.optionsChanged = true;
                            }
                            break;
                    }
                }
            }
        }

        #endregion

    }
}
