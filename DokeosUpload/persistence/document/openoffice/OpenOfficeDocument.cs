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
using lmsda.domain.exercise;

namespace lmsda.persistence.document.openoffice
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     This class represents an OpenOffice.org document.
    /// </summary>
    class OpenOfficeDocument : SupportedExercisesDocument
    {
        private String document;

        /// <summary>
        ///     Opens the document.
        /// </summary>
        /// <param name="document">The full file name of the document.</param>
        public OpenOfficeDocument(String document)
        {
            this.PlainText = false;
            this.document = document;
        }

        public override List<Exercise> getExercises()
        {
            throw new NotImplementedException();
        }

        public override void extractExercises()
        {
            throw new NotImplementedException();
        }

        public override String getDocumentPathWithFilename()
        {
            throw new NotImplementedException();
        }

        public static List<String> getAllActiveOpenOfficeDocuments()
        {
            throw new NotImplementedException();
        }

        public override void jumpToError(int exerciseNumber, int questionNumber, int answerNumber)
        {
            throw new NotImplementedException();
        }

        public override Boolean supportsJumpToSection()
        {
            return false;
        }

        public override int getDocumentScanWarnings()
        {
            throw new NotImplementedException();
        }

        public override List<String> getExerciseMD5s()
        {
            throw new NotImplementedException();
        }

        public override List<string> convertToPDF(string destinationPath, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error)
        {
            throw new NotImplementedException();
        }

        public override List<string> convertToPDFWithSplit(string destinationPath, string splitAt, string namePattern, Boolean splitOnPage, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error)
        {
            throw new NotImplementedException();
        }
    }
}
