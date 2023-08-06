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
using System.Collections;
using lmsda.domain.util;

namespace lmsda.domain.user.synchronization
{
    /// <summary>
    ///     Author: Gianni Van Hoecke, Maarten Meuris<br />
    ///     This class contains the settings of one file to synchronize.
    /// </summary>
    class FileSettings : IComparable<FileSettings>
    {
        private String path;

        public  String              relativeFileName        { get { return this.path; } }

        //Standard
        public SynchronizationType  synchronizationType     { get; set; }
        public String               lastHash                { get; set; }
        public Boolean              isDirectory             { get; set; }
        public Boolean              isExpanded              { get; set; } //As of 1.08
        public Boolean              hasError                { get; set; }

        /// <summary>
        ///     Use to indicate option changes. If the option changes also require
        ///     a full reupload, you should clear the lastHash when setting this flag.
        /// </summary>
        public Boolean              optionsChanged          { get; set; }

        //Normal upload
        public Boolean              setFileInvisible        { get; set; }
        public String               fileDescription         { get; set; }

        //Conversion to PDF
        public Boolean              splitOnStyle            { get; set; }
        public String               splitString             { get; set; }
        public Boolean              splitPerPage            { get; set; }
        public String               pdfNameTemplate         { get; set; }
        public String               source                  { get; set; }
        public Boolean              convertToJavascript     { get; set; }
        public Boolean              setPDFInvisible         { get; set; }

        //Conversion to exercise
        public Boolean              oneQuestionPerPage      { get; set; }
        public String               resultHashes            { get; set; }
        public int                  randomQuestions         { get; set; }
        public Boolean              setExerciseInvisible    { get; set; }

        //Conversion from PowerPoint to PDF (as of 1.08)
        public Boolean              frameSlides             { get; set; }
        public Boolean              horizontal              { get; set; }
        public PresentationPublishTypes publishMethod         { get; set; }
        public int                  slidesPerPage           { get; set; }
        public Boolean              includeHiddenSlides     { get; set; }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="relativeFileName">The relative path to the file or directory. (E.g.: "\file.docx")</param>
        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added isExpanded parameter for directories.
        ///     
        ///     updated on 12/08/2010 by Gianni Van Hoecke
        ///      -> Removed PDFSplitStyle from settings.
        /// 
        ///     updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> Added PowerPoint support.
        /// </remarks>
        public FileSettings(String relativeFileName, Boolean isDir)
        {
            this.path                   = relativeFileName;

            this.synchronizationType    = SynchronizationType.UPLOAD;
            this.lastHash               = String.Empty;
            this.isDirectory            = isDir;
            this.isExpanded             = true; //As of 1.08
            this.hasError               = false;
            this.optionsChanged         = false;

            this.setFileInvisible       = false;
            this.fileDescription        = String.Empty;

            this.splitOnStyle           = false;
            this.splitString            = String.Empty; //DomainController.Instance().getSettings().getPDFSplitStyle();
            this.splitPerPage           = false;
            this.source                 = String.Empty;
            this.convertToJavascript    = false;
            this.pdfNameTemplate        = String.Empty;
            this.setPDFInvisible        = false;

            this.oneQuestionPerPage     = false;
            this.resultHashes           = String.Empty;
            this.randomQuestions        = 0;
            this.setExerciseInvisible   = false;

            this.frameSlides            = true;
            this.horizontal             = true;
            this.publishMethod          = PresentationPublishTypes.SLIDES;
            this.slidesPerPage          = 6;
            this.includeHiddenSlides    = false;
        }

        /// <summary>
        ///     Constructor for a file.
        /// </summary>
        /// <param name="relativeFileName">The relative path to the file or directory. (E.g.: "\file.docx")</param>
        public FileSettings(String relativeFileName)
            : this(relativeFileName, false)
        { }

        #region IComparable<FileSettings> Members

        /// <summary>
        ///     Allows Sort() methods to sort the files by name in the same way they are shown in the tree view.
        /// </summary>
        /// <param name="other">The second FileSettings object to compare with</param>
        /// <returns></returns>
        public int CompareTo(FileSettings other)
        {
            char ds = System.IO.Path.DirectorySeparatorChar;
            String compare1 = this.relativeFileName;
            String compare2 = other.relativeFileName;
            if (compare1.Equals(compare2))
                return 0;
            // Directory location of each file item.
            // added backslashes are NECESSARY, otherwise "\directory11" starts with "\directory1"
            String compare1directory = (this.isDirectory ? compare1 : compare1.Substring(0, compare1.LastIndexOf(ds)).TrimEnd(ds)) + ds;
            String compare2directory = (other.isDirectory ? compare2 : compare2.Substring(0, compare2.LastIndexOf(ds)).TrimEnd(ds)) + ds;

            // COMPARE RULES:
            // If the current item should be higher on the list than the other item, its ranking value is lower, so we use -1.
            // If the current item should be lower on the list than the other item, its ranking value is higher, so we use 1.

            // two directorys
            if (this.isDirectory && other.isDirectory)
            {
                if (compare2directory.StartsWith(compare1directory))
                    return -1; // other directory starts with this directory: this is higher on list
                else if (compare1directory.StartsWith(compare2directory))
                    return 1; // this directory starts with other directory: this is lower on list
                else
                    return compare1directory.CompareTo(compare2directory); // no similarities: compare alphabetically, sort A-Z
            }
            // two files
            else if (!this.isDirectory && !other.isDirectory)
            {
                if (compare2directory.Equals(compare1directory))
                    return compare1.CompareTo(compare2); // same directory: compare alphabetically, sort A-Z
                else if (compare2directory.StartsWith(compare1directory))
                    return 1; // other directory starts with this directory: this is lower on list
                else if (compare1directory.StartsWith(compare2directory))
                    return -1; // this directory starts with other directory: this is higher on list
                else
                    return compare1directory.CompareTo(compare2directory); // no similarities: compare alphabetically, sort A-Z
            }
            //one file, one directory
            else if (this.isDirectory && !other.isDirectory)
            {
                if (compare2directory.Equals(compare1directory))
                    return -1; // other file is in this directory: this is higher on list
                else if (compare2directory.StartsWith(compare1directory))
                    return -1; // other file is in this directory: this is higher on list
                else if (compare1directory.StartsWith(compare2directory))
                    return -1; // this is a subdirectory of the directory in which other file is. Subdirectories are shown first: this is higher on list
                else
                    return compare1.CompareTo(compare2); // no similarities: compare alphabetically, sort A-Z
            }
            // one directory, one file
            else // if (!this.isDirectory && fls2.isDirectory) // check is unneeded; is only remaining possibility
            {
                if (compare2directory.Equals(compare1directory))
                    return 1; // this file is in the other directory. Directories are shown first: this is lower on list
                else if (compare2directory.StartsWith(compare1directory))
                    return 1; // other directory is subdirectory of this file's directory. Subdirectories are shown first: this is lower on list
                else if (compare1directory.StartsWith(compare2directory))
                    return 1; // other directory is root directory of this file's directory: this is lower on list
                else
                    return compare1.CompareTo(compare2); // no similarities: compare alphabetically, sort A-Z
            }
        }

        #endregion
    }
}
