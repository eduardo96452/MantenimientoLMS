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
using System.Xml;
using System.IO;
using System.Linq;
using lmsda.domain;
using lmsda.persistence.platform.service;
using lmsda.persistence.platform;
using lmsda.domain.util;

namespace lmsda.persistence.settings
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     This class is used for settings.
    /// </summary>
    /// <remarks>
    ///     When editing this class, make sure not to introduce calls to Programconstants
    ///     that use the StandAloneIdentifier instance. This class should be completely
    ///     independent from hardcoded folder settings so stand-alone mode can work.
    /// </remarks>
    class Settings
    {
        private String path;

        private XmlDocument xmlDocument;

        private String language;
        private Boolean isStandAlone;
        private String platform;
        private String url;
        private String loginname;
        private String coursecode;
        private String encoding;
        private Boolean exercisesMultiPage;
        private String pdfFileName;
        private Boolean pdfSplitZeroBeforeFirst;
        private Boolean pdfReplaceSpacesByUndescores; // as of 1.11
        private String statsFolder;
        private Boolean statsCreateSubFolder;
        private Boolean statsCalculateMCPercentage;
        private Boolean statsShowQuestionTitles;
        private String statsIDontKnowString;
        private Boolean statsCalculateResultsPerStudent;
        private Boolean statsCalculateExerciseDetailsPerStudent;
        private Boolean statsOpenExcelAfterConversion;
        private Boolean statsDeleteRawDataAfterConversion;
        private Boolean statsMakeSubjectFolderForSingleCourse;
        private Boolean statsShowAllAttempts; //As of 1.08
        private Boolean statsColumnNumber; //As of 1.09
        private Boolean statsColumnName; //As of 1.09
        private Boolean statsColumnEmail; //As of 1.09
        private Boolean statsColumnStudentNumber; //As of 1.09
        private Boolean statsColumnGroup; //As of 1.09
        private int statsColumnNumberID = 0; //As of 1.09
        private int statsColumnStudentNumberID = 1; //As of 1.09
        private int statsColumnNameID = 2; //As of 1.09
        private int statsColumnEmailID = 3; //As of 1.09
        private int statsColumnGroupID = 4; //As of 1.09
        private String service;
        private String subjectsFolder;
        private int defaultScoreGaps; //As of 1.08
        private int defaultScoreMatching; //As of 1.08

        private const String STR_CONFIGURATION = "configuration";

        private const String STR_LANGUAGE = "language";
        private const String STR_STANDALONE = "standalone";
        private const String STR_SUBJECTSFOLDER = "subjectsfolder";

        private const String STR_PLATFORM = "platform";

        private const String STR_PLATFORM_NAME = "name";
        private const String STR_PLATFORM_ENCODING = "encoding";
        private const String STR_PLATFORM_LOGINNAME = "loginname";
        private const String STR_PLATFORM_COURSECODE = "coursecode";
        private const String STR_PLATFORM_URL = "url";
        private const String STR_PLATFORM_SERVICE = "service";

        private const String STR_STATISTICS = "statistics";

        private const String STR_STATISTICS_FOLDER = "folder";
        private const String STR_STATISTICS_IDONTKNOWSTRING = "idontknowstring";
        private const String STR_STATISTICS_CREATESUBFOLDER = "createsubfolder";
        private const String STR_STATISTICS_CALCULATEMCPERCENTAGE = "calculatemcpercentage";
        private const String STR_STATISTICS_SHOWQUESTIONTITLES = "showquestiontitles";
        private const String STR_STATISTICS_CALCULATERESULTSPERSTUDENT = "calculateresultsperstudent";
        private const String STR_STATISTICS_OPENEXCELAFTERCONVERSION = "openexcelafterconversion";
        private const String STR_STATISTICS_DELETERAWDATAAFTERCONVERSION = "deleterawdataafterconversion";
        private const String STR_STATISTICS_CALCULATEEXERCISEDETAILSPERSTUDENT = "calculateexercisedetailsperstudent";
        private const String STR_STATISTICS_MAKESUBJECTFOLDERFORSINGLECOURSE = "makesubjectfolderforsinglecourse";
        private const String STR_STATISTICS_SHOWALLATTEMPTS = "showallattempts";
        private const String STR_STATISTICS_COLUMNS = "columns";
        private const String STR_STATISTICS_COLUMN_NUMBER = "number";
        private const String STR_STATISTICS_COLUMN_STUDENTNUMBER = "studentnumber";
        private const String STR_STATISTICS_COLUMN_NAME = "name";
        private const String STR_STATISTICS_COLUMN_EMAIL = "email";
        private const String STR_STATISTICS_COLUMN_GROUP = "group";
        private const String STR_STATISTICS_COLUMNS_ID = "id";

        private const String STR_PDF = "pdf";

        private const String STR_PDF_FILENAME = "filename";
        private const String STR_PDF_SPLITZEROBEFOREFIRST = "pdfsplitzerobeforefirst";
        private const String STR_PDF_REPLACESPACESBYUNDESCORES = "pdfreplacespacesbyundescores";

        private const String STR_EXERCISES = "exercises";

        private const String STR_EXERCISES_MULTIPAGE = "multipage";
        private const String STR_EXERCISES_DEFAULTSCOREGAPS = "defaultscoregaps";
        private const String STR_EXERCISES_DEFAULTSCOREMATCHING = "defaultscorematching";

        /// <summary>
        ///     Constructor. Reads all settings.
        /// </summary>
        /// <param name="path">The full path to the settings file.</param>
        /// <param name="rewrite">Determines if the settings are only read, or also rewritten with missing info filled in.</param>
        public Settings(String path, Boolean rewrite)
        {
            this.path = path;
            this.loadSettings(rewrite);
        }

        /// <summary>
        ///     Changes the path of the settings file, and rewrites it at the new location.
        /// </summary>
        /// <param name="path">The full path to the settings file.</param>
        public void setPath(String path)
        {
            this.path = path;
            this.loadSettings(false);
            this.makeSettingsFile();
        }

        /// <summary>
        ///     Reads all settings. This function will (re)write the XML file if elements are missing.
        /// </summary>
        /// <param name="rewriteFile">If data is missing, rewrite the file after loading it.</param>
        private void loadSettings(Boolean rewriteFile)
        {
            Boolean rewrite = false;
            if (!Utility.doesFileExist(this.path))
            {
                rewrite = true;
            }
            else
            {
                try
                {
                    this.xmlDocument = new XmlDocument();
                    this.xmlDocument.Load(this.path);
                }
                catch
                {
                    rewrite = true;
                }
            }

            TargetPlatformInfo defaultPlatform = TargetPlatforms.getDefaultPlatformInfo();

            // General settings

            Boolean deflanguage = false;
            this.language =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_LANGUAGE, ref deflanguage,
                ProgramConstants.DEFAULT_LANGUAGE);
            if (deflanguage)
            {
                rewrite = true;
                this.language = ProgramConstants.getDefaultOrSystemLanguage();
            }
            this.isStandAlone =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STANDALONE, ref rewrite,
                false);
            this.subjectsFolder =
                this.getValue("/" + STR_CONFIGURATION + "/"+ STR_SUBJECTSFOLDER, ref rewrite,
                String.Empty);

            // platform settings

            this.platform =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_NAME, ref rewrite,
                defaultPlatform.getPlatformName());
            this.encoding =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_ENCODING, ref rewrite,
                (defaultPlatform != null ? defaultPlatform.getPlatformEncoding() : String.Empty));
            if (!TargetPlatforms.getTargetPlatforms().Contains(platform))
            {
                this.platform = (defaultPlatform.getPlatformName());
                this.encoding = (defaultPlatform.getPlatformEncoding());
            }

            this.loginname =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_LOGINNAME, ref rewrite,
                String.Empty);
            this.coursecode =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_COURSECODE, ref rewrite,
                String.Empty);
            this.url =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_URL, ref rewrite,
                ProgramConstants.DEFAULT_PLATFORM_URL);
            this.service =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_SERVICE, ref rewrite,
                Enum.GetName(typeof(Service), Service.POST));

            // statistics settings

            this.statsFolder =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_FOLDER, ref rewrite,
                String.Empty);
            this.statsIDontKnowString =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_IDONTKNOWSTRING, ref rewrite,
                String.Empty);
            this.statsCreateSubFolder =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CREATESUBFOLDER, ref rewrite,
                true);
            this.statsCalculateMCPercentage =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATEMCPERCENTAGE, ref rewrite,
                false);
            this.statsShowQuestionTitles =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_SHOWQUESTIONTITLES, ref rewrite,
                false);
            this.statsCalculateResultsPerStudent =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATERESULTSPERSTUDENT, ref rewrite,
                true);
            this.statsOpenExcelAfterConversion =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_OPENEXCELAFTERCONVERSION, ref rewrite,
                false);
            this.statsDeleteRawDataAfterConversion =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_DELETERAWDATAAFTERCONVERSION, ref rewrite,
                true);
            this.statsCalculateExerciseDetailsPerStudent =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATEEXERCISEDETAILSPERSTUDENT, ref rewrite,
                false);
            this.statsMakeSubjectFolderForSingleCourse =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_MAKESUBJECTFOLDERFORSINGLECOURSE, ref rewrite,
                true);

            /*
             START COLUMNS
             */
            //Column values
            this.statsColumnNumber =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NUMBER, ref rewrite,
                true);
            this.statsColumnStudentNumber =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_STUDENTNUMBER, ref rewrite,
                true);
            this.statsColumnName =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NAME, ref rewrite,
                true);
            this.statsColumnEmail =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_EMAIL, ref rewrite,
                true);
            this.statsColumnGroup =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_GROUP, ref rewrite,
                true);
            //Column id's
            this.statsColumnNumberID =
                Convert.ToInt32(this.getAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NUMBER, STR_STATISTICS_COLUMNS_ID));
            this.statsColumnStudentNumberID =
                Convert.ToInt32(this.getAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_STUDENTNUMBER, STR_STATISTICS_COLUMNS_ID));
            this.statsColumnNameID =
                Convert.ToInt32(this.getAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NAME, STR_STATISTICS_COLUMNS_ID));
            this.statsColumnEmailID =
                Convert.ToInt32(this.getAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_EMAIL, STR_STATISTICS_COLUMNS_ID));
            this.statsColumnGroupID =
                Convert.ToInt32(this.getAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_GROUP, STR_STATISTICS_COLUMNS_ID));
            /*
             END COLUMNS
             */
            this.pdfFileName =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_FILENAME, ref rewrite,
                ProgramConstants.PDF_MASK_DEFAULT);
            this.pdfSplitZeroBeforeFirst =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_SPLITZEROBEFOREFIRST, ref rewrite,
                true);
            this.pdfReplaceSpacesByUndescores =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_REPLACESPACESBYUNDESCORES, ref rewrite,
                true);
            this.exercisesMultiPage =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_MULTIPAGE, ref rewrite,
                false);
            this.defaultScoreGaps =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_DEFAULTSCOREGAPS, ref rewrite,
                ProgramConstants.DEFAULT_SCORE_INIT);
            this.defaultScoreMatching =
                this.getValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_DEFAULTSCOREMATCHING, ref rewrite,
                ProgramConstants.DEFAULT_SCORE_INIT);

            if (rewrite && rewriteFile)
                this.makeSettingsFile();
        }

        /// <summary>
        ///     Returns the value from the given node.
        /// </summary>
        /// <param name="nodePath">The node.</param>
        /// <param name="readFailed">Is put to True if the read failed, left untouched otherwise.</param>
        /// <param name="defaultValue">The default value to return if the read failed.</param>
        /// <returns>The text read from the node.</returns>
        private String getValue(String nodePath, ref Boolean readFailed, String defaultValue)
        {
            try
            {
                String temp = this.xmlDocument.SelectSingleNode(nodePath).InnerText;
                if (temp.Equals(String.Empty))
                {
                    readFailed = true;
                    return defaultValue;
                }
                return temp;
            }
            catch
            {
                readFailed = true;
                return defaultValue;
            }
        }

        /// <summary>
        ///     Gets an attribute of the given node path.
        /// </summary>
        /// <param name="nodePath">The node that contains the attribute.</param>
        /// <param name="attributeName">The attribute to search for.</param>
        /// <returns>The value of the given attribute, null on error.</returns>
        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 14/09/2010 by Gianni Van Hoecke
        /// </remarks>
        private String getAttribute(String nodePath, String attributeName)
        {
            String returnValue = null;

            try
            {
                String temp = this.xmlDocument.SelectSingleNode(nodePath).Attributes[attributeName].Value;
                if (temp.Equals(String.Empty))
                {
                    returnValue = null;
                }
                return temp;
            }
            catch
            {
                returnValue = null;
            }

            return returnValue;
        }

        /// <summary>
        ///     Returns the value from the given node.
        /// </summary>
        /// <param name="nodePath">The node.</param>
        /// <param name="readFailed">Is put to True if the read failed, left untouched otherwise.</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The boolean read from the node.</returns>
        private Boolean getValue(String nodePath, ref Boolean readFailed, Boolean defaultValue)
        {
            bool failed=false;
            String temp = getValue(nodePath, ref failed, defaultValue.ToString());
            if (failed)
            {
                readFailed = true;
                return defaultValue;
            }
            else
                return Convert.ToBoolean(temp);
        }

        /// <summary>
        ///     Returns the value from the given node.
        /// </summary>
        /// <param name="nodePath">The node.</param>
        /// <param name="readFailed">Is put to True if the read failed, left untouched otherwise.</param>
        /// <param name="defaultValue">The default value</param>
        /// <returns>The integer read from the node.</returns>
        private int getValue(String nodePath, ref Boolean readFailed, int defaultValue)
        {
            bool failed = false;
            String temp = getValue(nodePath, ref readFailed, defaultValue.ToString());
            if (failed)
            {
                readFailed = true;
                return defaultValue;
            }
            else
                return Convert.ToInt32(temp);
        }

        /// <summary>
        ///     Saves all settings.
        /// </summary>
        private void saveSettings()
        {
            this.saveSettings(false);
        }

        /// <summary>
        ///     Saves all settings.
        /// </summary>
        private void saveSettings(Boolean isFirstRun)
        {
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_LANGUAGE,
                this.language);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STANDALONE,
                Convert.ToString(this.isStandAlone));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_SUBJECTSFOLDER,
                this.subjectsFolder);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_NAME,
                this.platform);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_URL,
                this.url);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_ENCODING,
                this.encoding);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_SERVICE,
                this.service);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_LOGINNAME,
                this.loginname);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PLATFORM + "/" + STR_PLATFORM_COURSECODE,
                this.coursecode);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_FILENAME,
                this.pdfFileName);

            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_SPLITZEROBEFOREFIRST,
                Convert.ToString(this.pdfSplitZeroBeforeFirst));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_PDF + "/" + STR_PDF_REPLACESPACESBYUNDESCORES,
                Convert.ToString(this.pdfReplaceSpacesByUndescores));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_FOLDER,
                this.statsFolder);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CREATESUBFOLDER,
                Convert.ToString(this.statsCreateSubFolder));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATEMCPERCENTAGE,
                Convert.ToString(this.statsCalculateMCPercentage));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_SHOWQUESTIONTITLES,
                Convert.ToString(this.statsShowQuestionTitles));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_IDONTKNOWSTRING,
                this.statsIDontKnowString);
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATERESULTSPERSTUDENT,
                Convert.ToString(this.statsCalculateResultsPerStudent));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_CALCULATEEXERCISEDETAILSPERSTUDENT,
                Convert.ToString(this.statsCalculateExerciseDetailsPerStudent));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_MAKESUBJECTFOLDERFORSINGLECOURSE,
                Convert.ToString(this.statsMakeSubjectFolderForSingleCourse));
            /*
             write the column settings in the correct order.
             */
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NUMBER,
                Convert.ToString(this.statsColumnNumber));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NAME,
                Convert.ToString(this.statsColumnName));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_EMAIL,
                Convert.ToString(this.statsColumnEmail));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_STUDENTNUMBER,
                Convert.ToString(this.statsColumnStudentNumber));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_GROUP,
                Convert.ToString(this.statsColumnGroup));
            this.writeAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NUMBER, STR_STATISTICS_COLUMNS_ID,
                Convert.ToString(this.statsColumnNumberID));
            this.writeAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_NAME, STR_STATISTICS_COLUMNS_ID,
                Convert.ToString(this.statsColumnNameID));
            this.writeAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_EMAIL, STR_STATISTICS_COLUMNS_ID,
                Convert.ToString(this.statsColumnEmailID));
            this.writeAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_STUDENTNUMBER, STR_STATISTICS_COLUMNS_ID,
                Convert.ToString(this.statsColumnStudentNumberID));
            this.writeAttribute("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_COLUMNS + "/" + STR_STATISTICS_COLUMN_GROUP, STR_STATISTICS_COLUMNS_ID,
                    Convert.ToString(this.statsColumnGroupID));
            /*
             END write the column settings in the correct order.
             */
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_OPENEXCELAFTERCONVERSION,
                Convert.ToString(this.statsOpenExcelAfterConversion));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_STATISTICS + "/" + STR_STATISTICS_DELETERAWDATAAFTERCONVERSION,
                Convert.ToString(this.statsDeleteRawDataAfterConversion));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_MULTIPAGE,
                Convert.ToString(this.exercisesMultiPage));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_DEFAULTSCOREGAPS,
                Convert.ToString(this.defaultScoreGaps));
            this.writeValue("/" + STR_CONFIGURATION + "/" + STR_EXERCISES + "/" + STR_EXERCISES_DEFAULTSCOREMATCHING,
                Convert.ToString(this.defaultScoreMatching));

            this.saveSettingsFile();
        }

        /// <summary>
        ///     Writes a value to the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="value">The value.</param>
        private Boolean writeValue(String node, String value)
        {
            try
            {
                this.xmlDocument.SelectSingleNode(node).InnerText = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Writes an attribute to the given node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="attribute">The attribute to write to.</param>
        /// <param name="value">The value for the attribute.</param>
        private Boolean writeAttribute(String node, String attribute, String value)
        {
            try
            {
                this.xmlDocument.SelectSingleNode(node).Attributes[attribute].Value = value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Creates a new settings file, and then calls the function to save it.
        /// </summary>
        private void makeSettingsFile()
        {
            this.xmlDocument = new XmlDocument();

            XmlNode xmlDeclaration = this.xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, null, null);
            this.xmlDocument.AppendChild(xmlDeclaration);

            XmlElement xmlConfiguration = this.xmlDocument.CreateElement(STR_CONFIGURATION);
            this.xmlDocument.AppendChild(xmlConfiguration);

            XmlElement xmlLanguage = this.xmlDocument.CreateElement(STR_LANGUAGE);
            xmlLanguage.InnerText = this.language;
            xmlConfiguration.AppendChild(xmlLanguage);

            XmlElement xmlStandalone = this.xmlDocument.CreateElement(STR_STANDALONE);
            xmlStandalone.InnerText = Convert.ToString(this.isStandAlone);
            xmlConfiguration.AppendChild(xmlStandalone);

            XmlElement xmlSubjectsFolder = this.xmlDocument.CreateElement(STR_SUBJECTSFOLDER);
            xmlSubjectsFolder.InnerText = this.subjectsFolder;
            xmlConfiguration.AppendChild(xmlSubjectsFolder);

            XmlElement xmlPlatform = this.xmlDocument.CreateElement(STR_PLATFORM);
            xmlConfiguration.AppendChild(xmlPlatform);

            XmlElement xmlPlatformName = this.xmlDocument.CreateElement(STR_PLATFORM_NAME);
            xmlPlatformName.InnerText = this.platform;
            xmlPlatform.AppendChild(xmlPlatformName);

            XmlElement xmlPlatformUrl = this.xmlDocument.CreateElement(STR_PLATFORM_URL);
            xmlPlatformUrl.InnerText = this.url;
            xmlPlatform.AppendChild(xmlPlatformUrl);

            XmlElement xmlPlatformEncoding = this.xmlDocument.CreateElement(STR_PLATFORM_ENCODING);
            xmlPlatformEncoding.InnerText = this.encoding;
            xmlPlatform.AppendChild(xmlPlatformEncoding);

            XmlElement xmlPlatformService = this.xmlDocument.CreateElement(STR_PLATFORM_SERVICE);
            xmlPlatformService.InnerText = this.service;
            xmlPlatform.AppendChild(xmlPlatformService);

            XmlElement xmlPlatformLoginname = this.xmlDocument.CreateElement(STR_PLATFORM_LOGINNAME);
            xmlPlatformLoginname.InnerText = this.loginname;
            xmlPlatform.AppendChild(xmlPlatformLoginname);

            XmlElement xmlPlatformCoursecode = this.xmlDocument.CreateElement(STR_PLATFORM_COURSECODE);
            xmlPlatformCoursecode.InnerText = this.coursecode;
            xmlPlatform.AppendChild(xmlPlatformCoursecode);

            XmlElement xmlPDF = this.xmlDocument.CreateElement(STR_PDF);
            xmlConfiguration.AppendChild(xmlPDF);

            XmlElement xmlPdfFileName = this.xmlDocument.CreateElement(STR_PDF_FILENAME);
            xmlPdfFileName.InnerText = this.pdfFileName;
            xmlPDF.AppendChild(xmlPdfFileName);

            XmlElement xmlPdfSplitZeroStatus = this.xmlDocument.CreateElement(STR_PDF_SPLITZEROBEFOREFIRST);
            xmlPdfSplitZeroStatus.InnerText = Convert.ToString(this.pdfSplitZeroBeforeFirst);
            xmlPDF.AppendChild(xmlPdfSplitZeroStatus);

            XmlElement xmlPdfReplaceSpacesByUndescores = this.xmlDocument.CreateElement(STR_PDF_REPLACESPACESBYUNDESCORES);
            xmlPdfReplaceSpacesByUndescores.InnerText = Convert.ToString(this.pdfReplaceSpacesByUndescores);
            xmlPDF.AppendChild(xmlPdfReplaceSpacesByUndescores);

            XmlElement xmlStats = this.xmlDocument.CreateElement(STR_STATISTICS);
            xmlConfiguration.AppendChild(xmlStats);

            XmlElement xmlStatsFolder = this.xmlDocument.CreateElement(STR_STATISTICS_FOLDER);
            xmlStatsFolder.InnerText = this.statsFolder;
            xmlStats.AppendChild(xmlStatsFolder);

            XmlElement xmlStatsCSF = this.xmlDocument.CreateElement(STR_STATISTICS_CREATESUBFOLDER);
            xmlStatsCSF.InnerText = Convert.ToString(this.statsCreateSubFolder);
            xmlStats.AppendChild(xmlStatsCSF);

            XmlElement xmlStatsCMCP = this.xmlDocument.CreateElement(STR_STATISTICS_CALCULATEMCPERCENTAGE);
            xmlStatsCMCP.InnerText = Convert.ToString(this.statsCalculateMCPercentage);
            xmlStats.AppendChild(xmlStatsCMCP);

            XmlElement xmlStatsSQT = this.xmlDocument.CreateElement(STR_STATISTICS_SHOWQUESTIONTITLES);
            xmlStatsSQT.InnerText = Convert.ToString(this.statsShowQuestionTitles);
            xmlStats.AppendChild(xmlStatsSQT);

            XmlElement xmlStatsIDKS = this.xmlDocument.CreateElement(STR_STATISTICS_IDONTKNOWSTRING);
            xmlStatsIDKS.InnerText = this.statsIDontKnowString;
            xmlStats.AppendChild(xmlStatsIDKS);

            XmlElement xmlStatsCRPS = this.xmlDocument.CreateElement(STR_STATISTICS_CALCULATERESULTSPERSTUDENT);
            xmlStatsCRPS.InnerText = Convert.ToString(this.statsCalculateResultsPerStudent);
            xmlStats.AppendChild(xmlStatsCRPS);

            XmlElement xmlStatsCEDPS = this.xmlDocument.CreateElement(STR_STATISTICS_CALCULATEEXERCISEDETAILSPERSTUDENT);
            xmlStatsCEDPS.InnerText = Convert.ToString(this.statsCalculateExerciseDetailsPerStudent);
            xmlStats.AppendChild(xmlStatsCEDPS);

            XmlElement xmlStatsOEAC = this.xmlDocument.CreateElement(STR_STATISTICS_OPENEXCELAFTERCONVERSION);
            xmlStatsOEAC.InnerText = Convert.ToString(this.statsOpenExcelAfterConversion);
            xmlStats.AppendChild(xmlStatsOEAC);

            XmlElement xmlStatsDRDAC = this.xmlDocument.CreateElement(STR_STATISTICS_DELETERAWDATAAFTERCONVERSION);
            xmlStatsDRDAC.InnerText = Convert.ToString(this.statsDeleteRawDataAfterConversion);
            xmlStats.AppendChild(xmlStatsDRDAC);

            XmlElement xmlStatsMCFFSC = this.xmlDocument.CreateElement(STR_STATISTICS_MAKESUBJECTFOLDERFORSINGLECOURSE);
            xmlStatsMCFFSC.InnerText = Convert.ToString(this.statsMakeSubjectFolderForSingleCourse);
            xmlStats.AppendChild(xmlStatsMCFFSC);
            
            XmlElement xmlStatsSAA = this.xmlDocument.CreateElement(STR_STATISTICS_SHOWALLATTEMPTS);
            xmlStatsSAA.InnerText = Convert.ToString(this.statsShowAllAttempts);
            xmlStats.AppendChild(xmlStatsSAA);

            /*
             START COLUMNS
             */
            XmlElement xmlStatsColumns = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMNS);
            xmlStats.AppendChild(xmlStatsColumns);

            XmlElement xmlStatsColumnsNumber = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMN_NUMBER);
            xmlStatsColumnsNumber.InnerText = Convert.ToString(this.statsColumnNumber);
            this.statsColumnNumberID = 0;
            xmlStatsColumnsNumber.SetAttribute(STR_STATISTICS_COLUMNS_ID, Convert.ToString(this.statsColumnNumberID));
            xmlStatsColumns.AppendChild(xmlStatsColumnsNumber);

            XmlElement xmlStatsColumnsSN = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMN_STUDENTNUMBER);
            xmlStatsColumnsSN.InnerText = Convert.ToString(this.statsColumnStudentNumber);
            this.statsColumnStudentNumberID = 1;
            xmlStatsColumnsSN.SetAttribute(STR_STATISTICS_COLUMNS_ID, Convert.ToString(this.statsColumnStudentNumberID));
            xmlStatsColumns.AppendChild(xmlStatsColumnsSN);

            XmlElement xmlStatsColumnsName = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMN_NAME);
            xmlStatsColumnsName.InnerText = Convert.ToString(this.statsColumnName);
            this.statsColumnNameID = 2;
            xmlStatsColumnsName.SetAttribute(STR_STATISTICS_COLUMNS_ID, Convert.ToString(this.statsColumnNameID));
            xmlStatsColumns.AppendChild(xmlStatsColumnsName);

            XmlElement xmlStatsColumnsEmail = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMN_EMAIL);
            xmlStatsColumnsEmail.InnerText = Convert.ToString(this.statsColumnEmail);
            this.statsColumnEmailID = 3;
            xmlStatsColumnsEmail.SetAttribute(STR_STATISTICS_COLUMNS_ID, Convert.ToString(this.statsColumnEmailID));
            xmlStatsColumns.AppendChild(xmlStatsColumnsEmail);

            XmlElement xmlStatsColumnsGroup = this.xmlDocument.CreateElement(STR_STATISTICS_COLUMN_GROUP);
            xmlStatsColumnsGroup.InnerText = Convert.ToString(this.statsColumnGroup);
            this.statsColumnGroupID = 4;
            xmlStatsColumnsGroup.SetAttribute(STR_STATISTICS_COLUMNS_ID, Convert.ToString(this.statsColumnGroupID));
            xmlStatsColumns.AppendChild(xmlStatsColumnsGroup);
            /*
             END COLUMNS
             */

            XmlElement xmlExercises = this.xmlDocument.CreateElement(STR_EXERCISES);
            xmlConfiguration.AppendChild(xmlExercises);

            XmlElement xmlExercisesMultiPage = this.xmlDocument.CreateElement(STR_EXERCISES_MULTIPAGE);
            xmlExercisesMultiPage.InnerText = Convert.ToString(this.exercisesMultiPage);
            xmlExercises.AppendChild(xmlExercisesMultiPage);

            XmlElement xmlDefaultScoreGaps = this.xmlDocument.CreateElement(STR_EXERCISES_DEFAULTSCOREGAPS);
            xmlDefaultScoreGaps.InnerText = Convert.ToString(this.defaultScoreGaps);
            xmlExercises.AppendChild(xmlDefaultScoreGaps);

            XmlElement xmlDefaultScoreMatching = this.xmlDocument.CreateElement(STR_EXERCISES_DEFAULTSCOREMATCHING);
            xmlDefaultScoreMatching.InnerText = Convert.ToString(this.defaultScoreMatching);
            xmlExercises.AppendChild(xmlDefaultScoreMatching);

            this.saveSettingsFile();
        }

        /// <summary>
        ///     Saves the settings file
        /// </summary>
        private void saveSettingsFile()
        {
            String settingsFolder = Path.GetDirectoryName(this.path);
            if (!Utility.doesFolderExist(settingsFolder))
            {
                try
                {
                    new DirectoryInfo(settingsFolder).Create();
                }
                catch (Exception e)
                {
                    if (DomainController.hasInstance())
                        DomainController.Instance().processError(e, true);
                }
            }
            else
            {
                try
                {
                    this.xmlDocument.Save(this.path);
                }
                catch (Exception e)
                {
                    if (DomainController.hasInstance())
                        DomainController.Instance().processError(e, true);
                }
            }
        }

        /*
         * Getters & setters
         */
        private void saveAndLog(String option)
        {
            saveAndLog(option, null);
        }

        private void saveAndLog(String option, String section)
        {
            this.saveSettings();
            if (DomainController.hasInstance())
            {
                if (section == null)
                    DomainController.Instance().writeToLog("setting_saved", new String[] { option }, true, true, false);
                else
                    DomainController.Instance().writeToLog("setting_saved_section", new String[] { option, section }, true, true, false);
            }
        }

        public void setLanguage(String resource)
        {
            this.language = resource;
            saveAndLog(STR_LANGUAGE);
        }
        
        public String getLanguage()
        {
            return this.language;
        }

        public void setStandAlone(Boolean isStandAlone)
        {
            this.isStandAlone = isStandAlone;
            saveAndLog(STR_STANDALONE);
        }

        public Boolean getStandAlone()
        {
            return this.isStandAlone;
        }
        
        public void setPlatform(String platform)
        {
            if (!this.platform.Equals(platform))
            {
                this.platform = platform;
                this.loginname = String.Empty;
                this.coursecode = String.Empty;
            }
            saveAndLog(STR_PLATFORM_NAME, STR_PLATFORM);
        }

        public String getPlatform()
        {
            return this.platform;
        }

        public void setUrl(String url)
        {
            if (!this.url.Equals(url))
            {
                this.url = url;
                this.loginname = String.Empty;
                this.coursecode = String.Empty;
            }
            saveAndLog(STR_PLATFORM_URL, STR_PLATFORM);
        }

        public String getUrl()
        {
            return this.url;
        }

        public void setEncoding(String encoding)
        {
            this.encoding = encoding;
            saveAndLog(STR_PLATFORM_ENCODING, STR_PLATFORM);
        }

        public String getEncoding()
        {
            return this.encoding;
        }

        public void setSubjectsFolder(String folder)
        {
            if (PortableIdentifier.Instance().isPortable && folder.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                folder = "\\" + folder.Substring(ProgramConstants.getProgramPath().TrimEnd('\\').Length).Trim('\\');

            this.subjectsFolder = folder;
            saveAndLog(STR_SUBJECTSFOLDER);
        }

        public String getSubjectsFolder()
        {
            if (this.subjectsFolder.StartsWith("\\"))
                return ProgramConstants.getProgramPath().TrimEnd('\\') + this.subjectsFolder;
            return this.subjectsFolder;
        }

        public void setService(String service)
        {
            this.service = service;
            saveAndLog(STR_PLATFORM_SERVICE, STR_PLATFORM);
        }

        public String getService()
        {
            return this.service;
        }

        public void setLoginname(String loginname)
        {
            this.loginname = loginname;
            saveAndLog(STR_PLATFORM_LOGINNAME, STR_PLATFORM);
        }

        public String getLoginname()
        {
            return this.loginname;
        }

        public void setCoursecode(String coursecode)
        {
            if (coursecode != null && !coursecode.Equals(String.Empty))
            {
                this.coursecode = coursecode;
                saveAndLog(STR_PLATFORM_COURSECODE, STR_PLATFORM);
            }
        }

        public String getCoursecode()
        {
            return this.coursecode;
        }

        public void setPDFFileName(String pdfFileName)
        {
            this.pdfFileName = pdfFileName;
            saveAndLog(STR_PDF_FILENAME, STR_PDF);
        }

        public String getPDFFileName()
        {
            return this.pdfFileName;
        }
        
        public void setPDFSplitZeroBeforeFirst(Boolean pdfSplitZeroBeforeFirst)
        {
            this.pdfSplitZeroBeforeFirst = pdfSplitZeroBeforeFirst;
            saveAndLog(STR_PDF_SPLITZEROBEFOREFIRST, STR_PDF);
        }

        public Boolean getPDFSplitZeroBeforeFirst()
        {
            return this.pdfSplitZeroBeforeFirst;
        }

        public void setPDFReplaceSpacesByUndescores(Boolean pdfReplaceSpacesByUndescores)
        {
            this.pdfReplaceSpacesByUndescores = pdfReplaceSpacesByUndescores;
            saveAndLog(STR_PDF_REPLACESPACESBYUNDESCORES, STR_PDF);
        }

        public Boolean getPDFReplaceSpacesByUndescores()
        {
            return this.pdfReplaceSpacesByUndescores;
        }

        public void setStatsFolder(String folder)
        {
            if (PortableIdentifier.Instance().isPortable && folder.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                folder = "\\" + folder.Substring(ProgramConstants.getProgramPath().TrimEnd('\\').Length).Trim('\\');

            this.statsFolder = folder;
            saveAndLog(STR_STATISTICS_FOLDER, STR_STATISTICS);
        }

        public String getStatsFolder()
        {
            if (this.statsFolder.StartsWith("\\"))
                return ProgramConstants.getProgramPath().TrimEnd('\\') + this.statsFolder;
            return this.statsFolder;
        }

        public void setStatsCreateSubFolder(Boolean statsCreateSubFolder)
        {
            this.statsCreateSubFolder = statsCreateSubFolder;
            saveAndLog(STR_STATISTICS_CREATESUBFOLDER, STR_STATISTICS);
        }

        public Boolean getStatsCreateSubFolder()
        {
            return this.statsCreateSubFolder;
        }

        public void setStatsCalculateMCPercentage(Boolean statsCalculateMCPercentage)
        {
            this.statsCalculateMCPercentage = statsCalculateMCPercentage;
            saveAndLog(STR_STATISTICS_CALCULATEMCPERCENTAGE, STR_STATISTICS);
        }

        public Boolean getStatsCalculateMCPercentage()
        {
            return this.statsCalculateMCPercentage;
        }

        public void setStatsShowQuestionTitles(Boolean statsShowQuestionTitles)
        {
            this.statsShowQuestionTitles = statsShowQuestionTitles;
            saveAndLog(STR_STATISTICS_SHOWQUESTIONTITLES, STR_STATISTICS);
        }

        public Boolean getStatsShowQuestionTitles()
        {
            return this.statsShowQuestionTitles;
        }

        public void setStatsIDontKnowString(String statsIDontKnowString)
        {
            this.statsIDontKnowString = statsIDontKnowString;
            saveAndLog(STR_STATISTICS_IDONTKNOWSTRING, STR_STATISTICS);
        }

        public String getStatsIDontKnowString()
        {
            return this.statsIDontKnowString;
        }

        public void setStatsCalculateResultsPerStudent(Boolean statsCalculateResultsPerStudent)
        {
            this.statsCalculateResultsPerStudent = statsCalculateResultsPerStudent;
            saveAndLog(STR_STATISTICS_CALCULATERESULTSPERSTUDENT, STR_STATISTICS);
        }

        public Boolean getStatsCalculateResultsPerStudent()
        {
            return this.statsCalculateResultsPerStudent;
        }

        public void setStatsCalculateExerciseDetailsPerStudent(Boolean statsCalculateExerciseDetailsPerStudent)
        {
            this.statsCalculateExerciseDetailsPerStudent = statsCalculateExerciseDetailsPerStudent;
            saveAndLog(STR_STATISTICS_CALCULATEEXERCISEDETAILSPERSTUDENT, STR_STATISTICS);
        }

        public Boolean getStatsCalculateExerciseDetailsPerStudent()
        {
            return this.statsCalculateExerciseDetailsPerStudent;
        }

        public void setStatsMakeSubjectFolderForSingleCourse(Boolean statsMakeSubjectFolderForSingleCourse)
        {
            this.statsMakeSubjectFolderForSingleCourse = statsMakeSubjectFolderForSingleCourse;
            saveAndLog(STR_STATISTICS_MAKESUBJECTFOLDERFORSINGLECOURSE, STR_STATISTICS);
        }

        public Boolean getStatsMakeSubjectFolderForSingleCourse()
        {
            return this.statsMakeSubjectFolderForSingleCourse;
        }
        
        public void setStatsShowAllAttempts(Boolean showAllAttempts)
        {
            this.statsShowAllAttempts = showAllAttempts;
            saveAndLog(STR_STATISTICS_SHOWALLATTEMPTS, STR_STATISTICS);
        }

        public Boolean getStatsShowAllAttempts()
        {
            return this.statsShowAllAttempts;
        }

        public void setStatsOpenExcelAfterConversion(Boolean statsOpenExcelAfterConversion)
        {
            this.statsOpenExcelAfterConversion = statsOpenExcelAfterConversion;
            saveAndLog(STR_STATISTICS_OPENEXCELAFTERCONVERSION, STR_STATISTICS);
        }

        public Boolean getStatsOpenExcelAfterConversion()
        {
            return this.statsOpenExcelAfterConversion;
        }

        public void setStatsDeleteRawDataAfterConversion(Boolean statsDeleteRawDataAfterConversion)
        {
            this.statsDeleteRawDataAfterConversion = statsDeleteRawDataAfterConversion;
            saveAndLog(STR_STATISTICS_DELETERAWDATAAFTERCONVERSION, STR_STATISTICS);
        }

        public Boolean getStatsDeleteRawDataAfterConversion()
        {
            return this.statsDeleteRawDataAfterConversion;
        }

        /*
         START COLUMNS
         */
        public void setStatsColumnNumber(Boolean number)
        {
            this.statsColumnNumber = number;
            saveAndLog(STR_STATISTICS_COLUMN_NUMBER, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public Boolean getStatsColumnNumber()
        {
            return this.statsColumnNumber;
        }

        public void setStatsColumnNumberID(int id)
        {
            this.statsColumnNumberID = id;
            saveAndLog(STR_STATISTICS_COLUMN_NUMBER + "-" + STR_STATISTICS_COLUMNS_ID, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public int getStatsColumnNumberID()
        {
            return this.statsColumnNumberID;
        }

        public void setStatsColumnStudentNumber(Boolean number)
        {
            this.statsColumnStudentNumber = number;
            saveAndLog(STR_STATISTICS_COLUMN_STUDENTNUMBER, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public Boolean getStatsColumnStudentNumber()
        {
            return this.statsColumnStudentNumber;
        }

        public void setStatsColumnStudentNumberID(int id)
        {
            this.statsColumnStudentNumberID = id;
            saveAndLog(STR_STATISTICS_COLUMN_STUDENTNUMBER + "-" + STR_STATISTICS_COLUMNS_ID, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public int getStatsColumnStudentNumberID()
        {
            return this.statsColumnStudentNumberID;
        }

        public void setStatsColumnName(Boolean name)
        {
            this.statsColumnName = name;
            saveAndLog(STR_STATISTICS_COLUMN_NAME, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public Boolean getStatsColumnName()
        {
            return this.statsColumnName;
        }

        public void setStatsColumnNameID(int id)
        {
            this.statsColumnNameID = id;
            saveAndLog(STR_STATISTICS_COLUMN_NAME + "-" + STR_STATISTICS_COLUMNS_ID, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public int getStatsColumnNameID()
        {
            return this.statsColumnNameID;
        }

        public void setStatsColumnEmail(Boolean email)
        {
            this.statsColumnEmail = email;
            saveAndLog(STR_STATISTICS_COLUMN_EMAIL, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public Boolean getStatsColumnEmail()
        {
            return this.statsColumnEmail;
        }

        public void setStatsColumnEmailID(int id)
        {
            this.statsColumnEmailID = id;
            saveAndLog(STR_STATISTICS_COLUMN_EMAIL + "-" + STR_STATISTICS_COLUMNS_ID, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public int getStatsColumnEmailID()
        {
            return this.statsColumnEmailID;
        }

        public void setStatsColumnGroup(Boolean group)
        {
            this.statsColumnGroup = group;
            saveAndLog(STR_STATISTICS_COLUMN_GROUP, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public Boolean getStatsColumnGroup()
        {
            return this.statsColumnGroup;
        }

        public void setStatsColumnGroupID(int id)
        {
            this.statsColumnGroupID = id;
            saveAndLog(STR_STATISTICS_COLUMN_GROUP + "-" + STR_STATISTICS_COLUMNS_ID, STR_STATISTICS + "-" + STR_STATISTICS_COLUMNS);
        }

        public int getStatsColumnGroupID()
        {
            return this.statsColumnGroupID;
        }
        /*
         END COLUMNS
         */

        public void setExercisesMultiPage(Boolean exercisesMultiPage)
        {
            this.exercisesMultiPage = exercisesMultiPage;
            saveAndLog(STR_EXERCISES_MULTIPAGE, STR_EXERCISES);
        }

        public Boolean getExercisesMultiPage()
        {
            return this.exercisesMultiPage;
        }

        public void setDefaultScoreGaps(int score)
        {
            this.defaultScoreGaps = score;
            saveAndLog(STR_EXERCISES_DEFAULTSCOREGAPS, STR_EXERCISES);
        }

        public int getDefaultScoreGaps()
        {
            return this.defaultScoreGaps;
        }

        public void setDefaultScoreMatching(int score)
        {
            this.defaultScoreMatching = score;
            saveAndLog(STR_EXERCISES_DEFAULTSCOREMATCHING, STR_EXERCISES);
        }

        public int getDefaultScoreMatching()
        {
            return this.defaultScoreMatching;
        }
    }
}
