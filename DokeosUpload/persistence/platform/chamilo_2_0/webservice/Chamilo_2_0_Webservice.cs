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
using System.Text;
using lmsda.domain.user;
using lmsda.domain.exercise;
using lmsda.domain.score;
using lmsda.domain;
using lmsda.persistence.platform.chamilo_2_0;
using lmsda.domain.score.data;

namespace lmsda.persistence.platform.chamilo_2_0.webservice
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    ///     This class provides the functionality of the Chamilo 2.0 platform through web services.
    /// </summary>
    class Chamilo_2_0_Webservice : TargetPlatform
    {
        private Login login;
        private Encoding encoding;
        private TargetPlatformInfo platformInfo = new Chamilo_2_0_PlatformInfo();

        public Chamilo_2_0_Webservice(Login login)
        {
            this.login = login;
            String enc = DomainController.Instance().getSettings().getEncoding();
            this.encoding = Encoding.GetEncoding((enc == null || enc.Equals(String.Empty)) ? platformInfo.getPlatformEncoding() : enc);
        }

        public override String getPlatformName()
        {
            return platformInfo.getPlatformName();
        }

        public override Boolean tryLogin()
        {
            throw new NotImplementedException();
        }

        public override List<Course> readCourses()
        {
            throw new NotImplementedException();
        }

        public override Boolean readDocumentFolders(ref List<Course> courses, Boolean useLazyLoading)
        {
            throw new NotImplementedException();
        }

        public override String getFormattedFilenameString(String filenameToFormat)
        {
            return filenameToFormat.ToLower();
        }

        public override Boolean doUploadExercises(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions, Boolean onlyIfDoesntExist)
        {
            throw new NotImplementedException();
        }

        public override String[] doUploadFile(String uploadFilePath, String uploadedName, String description, DocumentFolder publishDestination, Subject subject, Boolean setInvisible)
        {
            throw new NotImplementedException();
        }

        public override DocumentFolder doCreateFolderInDocuments(DocumentFolder publishDestination, String foldername, Boolean setInvisible)
        {
            throw new NotImplementedException();
        }

        public override List<Scores> downloadStatistics(List<Course> courses, String saveTo, Boolean createSubFolder, Boolean withoutMissingData)
        {
            throw new NotImplementedException();
        }

        public override Boolean doUpdateExercisesInfo(List<Exercise> exercises, List<Course> publishDestinations, Subject subject, Boolean setExerciseInvisible, int randomQuestions)
        {
            throw new NotImplementedException();
        }

        public override Boolean doUpdateFile(String filename, DocumentFolder path, Subject subject, String description, Boolean visible)
        {
            throw new NotImplementedException();
        }

        public override domain.score.data.Groups downloadGroups(Course course)
        {
            throw new NotImplementedException();
        }

        public override List<domain.score.data.Groups> downloadGroups(List<Course> courses)
        {
            throw new NotImplementedException();
        }

        public override List<string> getUsersFromGroup(Course course, Group groupID)
        {
            throw new NotImplementedException();
        }
    }
}
