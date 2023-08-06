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
using System.IO;
using lmsda.domain;
using lmsda.domain.user;
using System.Xml;
using lmsda.domain.util;

namespace lmsda.persistence.settings
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     This class contains all methods regarding the division of courses in subjects.
    /// </summary>
    class SubjectSettings
    {
        private List<Subject> subjects;

        public SubjectSettings()
        {
            scanSubjects();
        }

        public void scanSubjects()
        {
            this.subjects = new List<Subject>();
            String filePath = ProgramConstants.getSubjectsFilePath();
            XmlDocument xmlDocument = new XmlDocument();
            if (!new FileInfo(filePath).Exists) return;
            try
            {
                xmlDocument.Load(filePath);
            }
            catch
            {
                return;
            }
            // XML document is open
            
            // Get all subjects
            XmlNodeList xmlSubjects = null;
            try
            {
                xmlSubjects = xmlDocument.SelectNodes("/subjects/subject");
            }
            catch {}
            if (xmlSubjects != null)
            {
                foreach (XmlNode categoryNode in xmlSubjects)
                {
                    if (categoryNode.Attributes["name"] != null && categoryNode.Attributes["folder"] != null)
                    {
                        Subject subject = new Subject(
                            categoryNode.Attributes["name"].Value,
                            categoryNode.Attributes["folder"].Value);

                        if (Utility.doesFolderExist(subject.getSubjectFolder()))
                        {
                            foreach (XmlNode loginNode in categoryNode.ChildNodes)
                            {
                                if (loginNode.Name.Equals("login")
                                    && loginNode.Attributes["name"] != null && loginNode.Attributes["platform"] != null)
                                {
                                    SubjectLogin login = new SubjectLogin(loginNode.Attributes["name"].Value,
                                                                        loginNode.Attributes["platform"].Value);

                                    foreach (XmlNode courseNode in loginNode.ChildNodes)
                                    {
                                        if (courseNode.Name.Equals("course") && courseNode.Attributes["id"] != null)
                                        {
                                            Course course = new Course("", "", courseNode.Attributes["id"].Value);
                                            login.addCourse(course);
                                        }
                                    }
                                    if (login.getCourses().Count > 0)
                                        subject.addLogin(login);
                                }
                            }
                            this.subjects.Add(subject);
                        }
                    }
                }
            }
            saveSubjects();
        }

        public void linkSubjectsToCourses(Login login)
        {
            Boolean rewrite=false;
            Boolean processed = false;
            if (login==null) return;
            foreach (Subject subject in subjects)
            {
                SubjectLogin sjl = subject.getLogin(login.getUsername(), login.getPlatformUrl());
                if (sjl != null)
                {
                    List<Course> foreachcourses = new List<Course>(sjl.getCourses());
                    List<Course> courses = sjl.getCourses();
                    foreach (Course dummyCourse in foreachcourses)
                    {
                        processed = false;
                        foreach (Course realCourse in login.getCourses())
                        {
                            if (dummyCourse.getCourseId().Equals(realCourse.getCourseId()))
                            {
                                courses[courses.IndexOf(dummyCourse)] = realCourse;
                                processed=true;
                            }
                        }
                        if (!processed)
                        {
                            courses.Remove(dummyCourse);
                            rewrite=true;
                        }
                    }
                }
            }
            if (rewrite)
                saveSubjects();
        }

        public void saveSubjects()
        {
            XmlDocument xmlDocument = new XmlDocument(); 
            xmlDocument = new XmlDocument(); 
            
            XmlNode xmlDeclaration = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, null, null);
            xmlDocument.AppendChild(xmlDeclaration);

            XmlElement xmlSubjects = xmlDocument.CreateElement("subjects");
            xmlDocument.AppendChild(xmlSubjects);

            foreach (Subject subject in this.subjects)
            {
                XmlElement xmlSubject = xmlDocument.CreateElement("subject");
                xmlSubject.SetAttribute("name",subject.getSubjectName());
                xmlSubject.SetAttribute("folder", subject.getSubjectFolderToSave());
                xmlSubjects.AppendChild(xmlSubject);

                foreach (SubjectLogin login in subject.getLogins())
                {
                    if (login.getCourses().Count > 0)
                    {
                        XmlElement xmlLogin = xmlDocument.CreateElement("login");
                        xmlLogin.SetAttribute("name", login.getLoginname());
                        xmlLogin.SetAttribute("platform", login.getPlatformUrl());
                        xmlSubject.AppendChild(xmlLogin);

                        foreach (Course course in login.getCourses())
                        {
                            XmlElement xmlCourse = xmlDocument.CreateElement("course");
                            xmlCourse.SetAttribute("id", course.getCourseId());
                            xmlLogin.AppendChild(xmlCourse);
                        }
                    }
                }
            }
            xmlDocument.Save(ProgramConstants.getSubjectsFilePath());
        }

        public List<Subject> getSubjects()
        {
            return this.subjects;
        }

        public List<String> getSubjectNames()
        {
            List<String> retn = new List<String>();
            foreach (Subject sj in this.subjects)
                retn.Add(sj.getSubjectName());
            return retn;

        }

        public void setSubjects(List<Subject> subjects)
        {
            this.subjects = subjects;
        }

        public void addSubject(Subject subject)
        {
            if (this.subjects != null && this.getSubject(subject.getSubjectName()) == null)
            {
                this.subjects.Add(subject);
                this.saveSubjects();
            }
        }

        public void addSubject(String subjectName, String subjectFolder)
        {
            if (this.subjects != null && this.getSubject(subjectName) == null)
            {
                this.subjects.Add(new Subject(subjectName, subjectFolder));
                this.saveSubjects();
            }
        }

        public Subject getSubject(String subjectName)
        {
            if (this.subjects != null)
            {
                foreach (Subject subject in subjects)
                {
                    if (subject.getSubjectName().Equals(subjectName))
                        return subject;
                }
            }
            return null;
        }

        public Boolean subjectExists(String subjectName)
        {
            return this.getSubject(subjectName) != null;
        }

        public Subject getSubjectForFolder(String subjectFolder)
        {
            if (subjectFolder == null) return null;
            subjectFolder = subjectFolder.TrimEnd('/');
            if (this.subjects != null)
            {
                foreach (Subject subject in subjects)
                {
                    if (subject.getSubjectFolder().TrimEnd('/').Equals(subjectFolder))
                        return subject;
                }
            }
            return null;
        }

        public Boolean folderIsUsed(String subjectFolder)
        {
            if (subjectFolder == null) return false;
            subjectFolder = subjectFolder.TrimEnd('/');
            if (this.subjects != null)
            {
                foreach (Subject subject in subjects)
                {
                    if (subject.getSubjectFolder().TrimEnd('/').Equals(subjectFolder))
                        return true;
                }
            }
            return false;
        }

        public void deleteSubject(Subject subject, Boolean removeSettingsFile)
        {
            if (subject != null && subjects != null && subjects.Contains(subject))
            {
                foreach (SubjectLogin login in subject.getLogins())
                {
                    login.setSubjectForLogin(null);
                }
                String subjectPath = subject.getSubjectFolder();
                // remove synchronization settings since folder is no longer a subject
                if (removeSettingsFile)
                    Utility.tryDeleteFile(subject.getSubjectFolder().TrimEnd(new Char[]{'\\'}) + '\\' + ProgramConstants.SYNCHRONIZATION_FILE_NAME);
                this.subjects.Remove(subject);
                this.saveSubjects();
            }
        }

        public void deleteSubject(String subjectName, Boolean removeSettingsFile)
        {
            deleteSubject(getSubject(subjectName), removeSettingsFile);
        }

        public void clearSubjects()
        {
            this.subjects = new List<Subject>();
        }

        public void moveSubjectUpInList(String subjectName)
        {
            this.moveSubjectUpInList(this.getSubject(subjectName));
        }

        public void moveSubjectUpInList(Subject subject)
        {
            if (subject == null || subjects == null) return;
            for (int i=1; i < subjects.Count; i++)
            {
                if (subjects[i].Equals(subject))
                {
                    Subject cc1 = subjects[i-1];
                    subjects[i-1] = subject;
                    subjects[i] = cc1;
                    this.saveSubjects();
                    break;
                }
            }
        }

        public void moveSubjectDownInList(String subjectName)
        {
            this.moveSubjectDownInList(this.getSubject(subjectName));
        }
        public void moveSubjectDownInList(Subject subject)
        {
            if (subject == null || subjects == null) return;
            for (int i=0; i < subjects.Count-1; i++)
            {
                if (subjects[i].Equals(subject))
                {
                    Subject cc1 = subjects[i+1];
                    subjects[i+1] = subject;
                    subjects[i] = cc1;
                    this.saveSubjects();
                    break;
                }
            }
        }


    }
}
