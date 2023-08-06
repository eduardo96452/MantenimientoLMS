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
using System.Xml;
using System.IO;
using lmsda.domain.user.synchronization;
using lmsda.domain;
using lmsda.domain.util;

namespace lmsda.persistence.settings
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    class SynchronizationSettings
    {
        private const String    ROOTNODE        = "synchronisationsettings";
        private const String    GROUPNODE       = "configuration";
        private const String    SYNCHRONIZED    = "synchronizedcourses";
        private const String    FILE            = "file";
        private const String    COURSE          = "course";
        private const String    NEWLINE         = "<~§~>";

        private String          path;
        private XmlDocument     xmlDocument;

        private List<String>       synchronized         = null;
        public  List<String>       synchronizedCourses  { get { return this.synchronized; } }
        public  List<FileSettings> data                 = null;

        private String userName;
        private String platformurl;

        /// <summary>
        ///     Default constructor. It loads the settings of a subject.
        /// </summary>
        /// <param name="pathToLoad">The path of the settings file.</param>
        public SynchronizationSettings(String pathToLoad)
        {
            this.path = pathToLoad;
            this.data = new List<FileSettings>();
            this.synchronized = new List<String>();
            this.loadUser();
            this.readSynchronizationSettings();
        }

        /// <summary>
        ///     This will save the settings file.
        /// </summary>
        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added isExpanded parameter.
        ///     
        ///     updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> Added PowerPoint support.
        /// </remarks>
        public void saveSynchronizationSettings()
        {
            DomainController.Instance().writeToLog("saving_synchronization_settings", true, true, false);
            XmlNode workingNode = null;
            
            //Get the user node.
            foreach (XmlNode node in this.xmlDocument.SelectNodes("/" + ROOTNODE + "/" + GROUPNODE))
            {
                if (this.userName != null &&
                    this.platformurl != null &&
                    node.Attributes["login"] != null &&
                    node.Attributes["platformurl"] != null &&
                    node.Attributes["login"].Value.Equals(this.userName) &&
                    node.Attributes["platformurl"].Value.Equals(this.platformurl))
                {
                    workingNode = node;
                    break;
                }
            }

            //If no user node has been found, create one.
            if (workingNode == null)
            { 
                XmlElement xmlConfig = this.xmlDocument.CreateElement(GROUPNODE);
                xmlConfig.SetAttribute("login", this.userName);
                xmlConfig.SetAttribute("platformurl", this.platformurl);
                workingNode = this.xmlDocument.SelectSingleNode("/" + ROOTNODE).AppendChild(xmlConfig);
            }

            //Delete all child nodes.
            XmlNodeList nodeList = workingNode.ChildNodes;
            for(int i = nodeList.Count - 1; i >= 0; i--)
                workingNode.RemoveChild(nodeList[i]);

            //Save the synchronized courses
            XmlElement xmlCourses = this.xmlDocument.CreateElement(SYNCHRONIZED);
            XmlNode coursesNode = workingNode.AppendChild(xmlCourses);
            foreach (String courseID in this.synchronizedCourses)
            {
                XmlElement xmlCourse = this.xmlDocument.CreateElement(COURSE);
                xmlCourse.SetAttribute("id", courseID);
                coursesNode.AppendChild(xmlCourse);
            }

            // Sort the files so they correspond to the tree view;
            // see the CompareTo(...) function in FileSettings class
            this.data.Sort();

            //Save the files
            XmlElement xmlFile = null;
            foreach (FileSettings fileSettings in this.data)
            {
                xmlFile = this.xmlDocument.CreateElement(FILE);

                xmlFile.SetAttribute("name", fileSettings.relativeFileName);
                xmlFile.SetAttribute("synchronizationtype", Enum.GetName(typeof(SynchronizationType), fileSettings.synchronizationType));

                if (fileSettings.isDirectory)
                {
                    xmlFile.SetAttribute("isdirectory", Convert.ToString(fileSettings.isDirectory));
                    xmlFile.SetAttribute("isexpanded", Convert.ToString(fileSettings.isExpanded)); //As of 1.08
                }
                else
                {
                    xmlFile.SetAttribute("lastcrc", fileSettings.lastHash);

                    if (fileSettings.hasError)
                        xmlFile.SetAttribute("haserror", Convert.ToString(fileSettings.hasError));

                    if (fileSettings.synchronizationType != SynchronizationType.EXCLUDE)
                    {
                        xmlFile.SetAttribute("optionschanged", Convert.ToString(fileSettings.optionsChanged));
                    }

                    //Only save the information we need; ignore the rest
                    switch (fileSettings.synchronizationType)
                    {
                        case SynchronizationType.CONVERT_TO_EXERCISE:
                            xmlFile.SetAttribute("onequestionperpage", Convert.ToString(fileSettings.oneQuestionPerPage));
                            xmlFile.SetAttribute("exercisemd5s", Convert.ToString(fileSettings.resultHashes));
                            xmlFile.SetAttribute("randomquestions", Convert.ToString(fileSettings.randomQuestions));
                            xmlFile.SetAttribute("setexerciseinvisible", Convert.ToString(fileSettings.setExerciseInvisible));
                            break;
                        case SynchronizationType.CONVERT_WORD_TO_PDF:
                            if (!fileSettings.splitOnStyle)
                            {
                                fileSettings.splitPerPage = false;
                                fileSettings.splitString = String.Empty;
                                fileSettings.pdfNameTemplate = String.Empty;
                            }
                            xmlFile.SetAttribute("splitonstyle", Convert.ToString(fileSettings.splitOnStyle));
                            xmlFile.SetAttribute("splitstring", fileSettings.splitString);
                            xmlFile.SetAttribute("splitperpage", Convert.ToString(fileSettings.splitPerPage));
                            xmlFile.SetAttribute("converttojavascript", Convert.ToString(fileSettings.convertToJavascript));
                            xmlFile.SetAttribute("nametemplate", fileSettings.pdfNameTemplate);
                            xmlFile.SetAttribute("setpdfinvisible", Convert.ToString(fileSettings.setPDFInvisible));
                            break;
                        case SynchronizationType.CONVERT_POWERPOINT_TO_PDF:
                            //As of 1.08
                            xmlFile.SetAttribute("frameslides", Convert.ToString(fileSettings.frameSlides));
                            xmlFile.SetAttribute("horizontal", Convert.ToString(fileSettings.horizontal));
                            xmlFile.SetAttribute("publishmethod", Enum.GetName(typeof(PresentationPublishTypes), fileSettings.publishMethod));
                            xmlFile.SetAttribute("slidesperpage", Convert.ToString(fileSettings.slidesPerPage));
                            xmlFile.SetAttribute("includehiddenslides", Convert.ToString(fileSettings.includeHiddenSlides));
                            break;

                        case SynchronizationType.CONVERT_EXCEL_TO_PDF:
                            //As of 1.08
                            //Excel has at this moment no settings. The whole workbook will be converted.
                            break;
                        case SynchronizationType.UPLOAD:
                            // can not be saved for folders, since folders are
                            // only created when they're needed for a file upload
                            xmlFile.SetAttribute("setfileinvisible", Convert.ToString(fileSettings.setFileInvisible));
                            xmlFile.SetAttribute("filedescription", fileSettings.fileDescription.Replace(Environment.NewLine, NEWLINE));
                            break;
                    }

                    if (fileSettings.source != null && !fileSettings.source.Equals(String.Empty))
                        xmlFile.SetAttribute("source", fileSettings.source);
                }
                workingNode.AppendChild(xmlFile);
            }

            if (Utility.doesFileOrFolderExist(this.path, false))
            {
                try
                {
                    new FileInfo(this.path).Attributes = FileAttributes.Normal;
                    this.xmlDocument.Save(this.path);
                    new FileInfo(this.path).Attributes = FileAttributes.Hidden;
                }
                catch
                {
                    //do what when XMS document save fails?
                }
                DomainController.Instance().writeToLog("synchronization_settings_saved", true, true, false);
            }
            else
            {
                DomainController.Instance().writeToLog("synchronization_save_failed", true, true, false);
            }

        }

        /// <summary>
        ///     Adds a file description to the list.
        ///     Changes are not saved until saveSynchronizationSettings() is called.
        /// </summary>
        /// <param name="fileSettings">The file settings description object.</param>
        public void addFile(FileSettings fileSettings)
        {
            if(fileSettings != null)
                this.data.Add(fileSettings);
        }

        /// <summary>
        ///     Adds a course to the synchronized courses list. If the course is already in the list, the method will exit.
        ///     Changes are not saved until saveSynchronizationSettings() is called.
        /// </summary>
        /// <param name="courseID">The course ID.</param>
        public void addSynchronizedCourse(String courseID)
        {
            if(courseID != null && !courseID.Equals(String.Empty))
                if(!this.synchronized.Contains(courseID))
                    this.synchronized.Add(courseID);
        }

        /// <summary>
        ///     Adds a course to the synchronized courses list. If the course is already in the list, the method will exit.
        ///     Changes are not saved until saveSynchronizationSettings() is called.
        /// </summary>
        /// <param name="courseID">The course ID.</param>
        public void clearSynchronizedCourses()
        {
            this.synchronized = new List<String>();
        }

        #region Private methods

        private void createSynchronizationFile()
        { 
            XmlDocument createDocument = new XmlDocument();

            XmlNode xmlDeclaration = createDocument.CreateNode(XmlNodeType.XmlDeclaration, null, null);
            createDocument.AppendChild(xmlDeclaration);

            //Root node
            XmlElement xmlData = createDocument.CreateElement(ROOTNODE);
            createDocument.AppendChild(xmlData);

            //config node (user info)
            XmlElement xmlConfig = createDocument.CreateElement(GROUPNODE);
            xmlConfig.SetAttribute("login", this.userName);
            xmlConfig.SetAttribute("platformurl", this.platformurl);
            xmlData.AppendChild(xmlConfig);

            //synchronized courses node
            XmlElement xmlCourses = createDocument.CreateElement(SYNCHRONIZED);
            xmlConfig.AppendChild(xmlCourses);

            createDocument.Save(this.path);
        }

        private void readSynchronizationSettings()
        {
            if(!new FileInfo(this.path).Exists)
                this.createSynchronizationFile();

            this.xmlDocument = new XmlDocument();
            this.xmlDocument.Load(this.path);

            try
            {
                XmlNodeList nodeList = this.xmlDocument.SelectNodes("/" + ROOTNODE + "/" + GROUPNODE);
                foreach (XmlNode node in nodeList)
                {
                    if (this.userName != null && 
                        this.platformurl != null && 
                        node.Attributes["login"] != null &&
                        node.Attributes["platformurl"] != null &&
                        node.Attributes["login"].Value.Equals(this.userName) &&
                        node.Attributes["platformurl"].Value.Equals(this.platformurl))
                    { 
                        //Use this node
                        this.loadFileList(node.ChildNodes);

                        //Now that we have the correct user, break the search
                        break;
                    }
                }
            }
            catch { }
        }

        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added isExpanded parameter.
        ///      
        ///     updated on 10/08/2010 by Gianni Van Hoecke
        ///      -> Added PowerPoint support.
        /// </remarks>
        private void loadFileList(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (node.Name.Equals(SYNCHRONIZED))
                {
                    //Add all synchronized courses...
                    foreach (XmlNode syncNode in node.ChildNodes)
                    {
                        if (syncNode.Name.Equals(COURSE) && syncNode.Attributes["id"] != null)
                        {
                            this.synchronized.Add(syncNode.Attributes["id"].Value);
                        }
                    }
                }
                else if (node.Name.Equals(FILE))
                {
                    //List all files...
                    FileSettings fs = new FileSettings(node.Attributes["name"].Value);
                    this.data.Add(fs);
                    foreach (XmlAttribute attribute in node.Attributes)
                    {
                        if(attribute.Name.Equals("isdirectory"))
                            fs.isDirectory = Convert.ToBoolean(attribute.Value);
                        else if (attribute.Name.Equals("isexpanded")) //As of 1.08
                            fs.isExpanded = Convert.ToBoolean(attribute.Value);
                        else if(attribute.Name.Equals("lastcrc"))
                            fs.lastHash = attribute.Value;
                        else if(attribute.Name.Equals("synchronizationtype"))
                            fs.synchronizationType = (SynchronizationType)Enum.Parse(typeof(SynchronizationType), attribute.Value);
                        else if(attribute.Name.Equals("haserror"))
                            fs.hasError = Convert.ToBoolean(attribute.Value);

                        else if (attribute.Name.Equals("setfileinvisible"))
                            fs.setFileInvisible = Convert.ToBoolean(attribute.Value);
                        else if (attribute.Name.Equals("filedescription"))
                            fs.fileDescription = attribute.Value.Replace(NEWLINE, Environment.NewLine);
                        else if(attribute.Name.Equals("splitonstyle"))
                            fs.splitOnStyle = Convert.ToBoolean(attribute.Value);
                        else if(attribute.Name.Equals("splitstring"))
                            fs.splitString = attribute.Value;
                        else if(attribute.Name.Equals("splitperpage"))
                            fs.splitPerPage = Convert.ToBoolean(attribute.Value);
                        else if(attribute.Name.Equals("converttojavascript"))
                            fs.convertToJavascript = Convert.ToBoolean(attribute.Value);
                        else if(attribute.Name.Equals("nametemplate"))
                            fs.pdfNameTemplate = attribute.Value;
                        else if(attribute.Name.Equals("source"))
                            fs.source = attribute.Value;
                        else if (attribute.Name.Equals("setpdfinvisible"))
                            fs.setPDFInvisible = Convert.ToBoolean(attribute.Value);

                        else if(attribute.Name.Equals("onequestionperpage"))
                            fs.oneQuestionPerPage = Convert.ToBoolean(attribute.Value);
                        else if (attribute.Name.Equals("setexerciseinvisible"))
                            fs.setExerciseInvisible = Convert.ToBoolean(attribute.Value);
                        else if(attribute.Name.Equals("randomquestions"))
                            fs.randomQuestions = Convert.ToInt32(attribute.Value);
                        else if(attribute.Name.Equals("exercisemd5s"))
                            fs.resultHashes = attribute.Value;

                        else if(attribute.Name.Equals("optionschanged"))
                            fs.optionsChanged = Convert.ToBoolean(attribute.Value);

                        //As of 1.08
                        else if (attribute.Name.Equals("frameslides"))
                            fs.frameSlides = Convert.ToBoolean(attribute.Value);
                        else if (attribute.Name.Equals("horizontal"))
                            fs.horizontal = Convert.ToBoolean(attribute.Value);
                        else if (attribute.Name.Equals("publishmethod"))
                            fs.publishMethod = (PresentationPublishTypes)Enum.Parse(typeof(PresentationPublishTypes), attribute.Value);
                        else if (attribute.Name.Equals("slidesperpage"))
                            fs.slidesPerPage = Convert.ToInt32(attribute.Value);
                        else if (attribute.Name.Equals("includehiddenslides"))
                            fs.includeHiddenSlides = Convert.ToBoolean(attribute.Value);
                    }
                }
            }
        }

        private void loadUser()
        {
            this.userName = null;
            this.platformurl = null;

            try
            { 
                this.userName = DomainController.Instance().getUserInfo().getLogin().getUsername();
            }
            catch { }

            try
            { 
                this.platformurl = DomainController.Instance().getSettings().getUrl(); 
            }
            catch { }
        }

        #endregion

    }
}
