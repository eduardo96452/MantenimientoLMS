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
using Microsoft.Office.Interop.PowerPoint;
using System.IO;
using Microsoft.Office.Core;

namespace lmsda.persistence.document.microsoftoffice
{
    class MicrosoftPowerpointDocumentCommands : SupportedDocumentCommands
    {
        public override List<String> getAllActiveDocuments()
        {
            List<String> documentPaths = new List<string>();
            _Application app;
            Presentations presentations = null;
            Boolean addDoc = false;

            try
            {
                app = (Microsoft.Office.Interop.PowerPoint.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("PowerPoint.Application");
                presentations = app.Presentations;
            }
            catch
            {
                //No documents opened...
            }

            if (presentations != null)
            {
                foreach (Presentation presentation in presentations)
                {
                    try
                    {
                        String fileName = presentation.FullName;
                        if (fileName.Contains('\\'))
                            fileName = fileName.Substring(presentation.FullName.LastIndexOf('\\'));

                        if (fileName.Contains('.'))
                        {
                            addDoc = false;
                            foreach (String ext in this.getSupportedExtensions())
                                if (fileName.EndsWith("." + ext))
                                    addDoc = true;
                            if (addDoc)
                                documentPaths.Add(presentation.FullName);
                        }
                        else
                        {
                            documentPaths.Add(presentation.FullName);
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
            return new List<String>(new String[] { "pot", "potx", "potm"}).Union(this.getSupportedExtensions()).ToList();
        }

        public override List<String> getSupportedExtensions()
        {
            return new List<String>(new String[] { "ppt", "pps", "pptx", "ppsx", "pptm", "ppsm" });
        }

        public override SupportedDocument factory(String document)
        {
            return new MicrosoftPowerpointDocument(document);
        }

        public override DocumentType getDocumentType()
        {
            return DocumentType.PRESENTATION_DOCUMENT;
        }
        
        public override void openDocument(String path, String changePathTo)
        {
            _Application app = new ApplicationClass();

            if (this.getSupportedExtensions().Contains(new FileInfo(path).Extension))
            {
                app.Presentations.Open(path, MsoTriState.msoFalse,
                                             MsoTriState.msoFalse,
                                             MsoTriState.msoFalse);

            }

            // if (changePathTo != null && !changePathTo.Equals(String.Empty))
            //     app.ChangeFileOpenDirectory(changePathTo);

            app.Visible = MsoTriState.msoTrue;
            app.Activate();
            app = null;
        }


        public override bool isOfficeAppAvailable()
        {
            try
            {
                #pragma warning disable //disables "not used" message
                Microsoft.Office.Interop.PowerPoint.PpAutoSize test = Microsoft.Office.Interop.PowerPoint.PpAutoSize.ppAutoSizeNone;
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
