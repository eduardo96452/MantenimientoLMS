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
using Microsoft.Office.Interop.Word;
using System.IO;

namespace lmsda.persistence.document.microsoftoffice
{
    class MicrosoftWordDocumentCommands : SupportedDocumentCommands
    {

        public override List<String> getAllActiveDocuments()
        {
            List<String> documentPaths = new List<string>();
            _Application app;
            Documents documents = null;
            Boolean addDoc=false;

            try
            {
                app = (Microsoft.Office.Interop.Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
                documents = app.Documents;
            }
            catch
            {
                //No documents opened...
            }

            if (documents != null)
            {
                foreach (Document document in documents)
                {
                    try
                    {
                        String fileName = document.FullName;
                        if (fileName.Contains('\\'))
                            fileName = fileName.Substring(document.FullName.LastIndexOf('\\'));

                        if (fileName.Contains('.'))
                        {
                            addDoc=false;
                            foreach (String ext in this.getSupportedExtensions())
                                if (fileName.EndsWith("." + ext))
                                    addDoc = true;
                            if (addDoc)
                                documentPaths.Add(document.FullName);
                        }
                        else
                        {
                            documentPaths.Add(document.FullName);
                        }
                    }
                    catch { }
                }
            }

            app = null;
            
            return documentPaths;
        }

        public override List<String> getAllSupportedExtensions()
        {
            return new List<String>(new String[]{"dot", "dotx"}).Union(this.getSupportedExtensions()).ToList();
        }

        public override List<String> getSupportedExtensions()
        {
            return new List<String>(new String[]{"doc", "docx", "pptx", "jpg", "png", "xlsx"});
        }

        public override SupportedDocument factory(String document)
        {
            return new MicrosoftWordDocument(document);
        }

        public override DocumentType getDocumentType()
        {
            return DocumentType.PROCESSEDTEXT_DOCUMENT;
        }

        public override void openDocument(String path, String changePathTo)
        {
            _Application app = new ApplicationClass();

            if (this.getSupportedExtensions().Contains(new FileInfo(path).Extension))
            {
                app.Documents.Open(path, Type.Missing, false, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            }
            else
            {
                app.Documents.Add(path, Type.Missing, Type.Missing, Type.Missing);
            }

            if (changePathTo != null && !changePathTo.Equals(String.Empty))
                app.ChangeFileOpenDirectory(changePathTo);

            app.Visible = true;
            app.Activate();
            app = null;
        }

        public override bool isOfficeAppAvailable()
        {
            try
            {
                #pragma warning disable //disables "not used" message
                Microsoft.Office.Interop.Word.WdBreakType test = Microsoft.Office.Interop.Word.WdBreakType.wdPageBreak;
                #pragma warning restore
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
