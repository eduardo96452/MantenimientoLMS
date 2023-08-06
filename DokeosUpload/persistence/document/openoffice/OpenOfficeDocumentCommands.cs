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

namespace lmsda.persistence.document.openoffice
{
    class OpenOfficeDocumentCommands : SupportedDocumentCommands
    {
        public override List<String> getAllActiveDocuments()
        {
            throw new NotSupportedException();
        }

        public override List<string> getAllSupportedExtensions()
        {
            return this.getSupportedExtensions();
        }

        public override List<String> getSupportedExtensions()
        {
            return new List<String>(new String[]{"odt"});
        }

        public override SupportedDocument factory(String document)
        {
            return new OpenOfficeDocument(document);
        }

        public override DocumentType getDocumentType()
        {
            return DocumentType.PROCESSEDTEXT_DOCUMENT;
        }

        public override void openDocument(string path, string changePathTo)
        {
            throw new NotImplementedException();
        }


        public override bool isOfficeAppAvailable()
        {
            // todo when implementing
            return false;
        }
    }
}
