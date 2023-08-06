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
using Microsoft.Office.Interop.Excel;
using System.IO;

namespace lmsda.persistence.document.microsoftoffice
{
    class MicrosoftExcelDocumentCommands : SupportedDocumentCommands
    {
        public override List<String> getAllActiveDocuments()
        {
            List<String> documentPaths = new List<string>();
            _Application app;
            Workbooks spreadsheets = null;
            Boolean addDoc = false;

            try
            {
                app = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                spreadsheets = app.Workbooks;
            }
            catch
            {
                //No documents opened...
            }

            if (spreadsheets != null)
            {
                foreach (Workbook spreadsheet in spreadsheets)
                {
                    try
                    {
                        String fileName = spreadsheet.FullName;
                        if (fileName.Contains('\\'))
                            fileName = fileName.Substring(spreadsheet.FullName.LastIndexOf('\\'));

                        if (fileName.Contains('.'))
                        {
                            addDoc = false;
                            foreach (String ext in this.getSupportedExtensions())
                                if (fileName.EndsWith("." + ext))
                                    addDoc = true;
                            if (addDoc)
                                documentPaths.Add(spreadsheet.FullName);
                        }
                        else
                        {
                            documentPaths.Add(spreadsheet.FullName);
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
            return new List<String>(new String[] { /* no extra types*/ }).Union(this.getSupportedExtensions()).ToList();
        }

        public override List<String> getSupportedExtensions()
        {
            return new List<String>(new String[] { "xls", "xlsx", "xlsm", "xlsb"});
        }

        public override SupportedDocument factory(String document)
        {
            return new MicrosoftExcelDocument(document);
        }

        public override DocumentType getDocumentType()
        {
            return DocumentType.SPREADSHEET_DOCUMENT;
        }

        public override void openDocument(String path, String changePathTo)
        {
            _Application app = new ApplicationClass();

            if (this.getSupportedExtensions().Contains(new FileInfo(path).Extension))
            {
                app.Workbooks.Open(path);
            }

            // if (changePathTo != null && !changePathTo.Equals(String.Empty))
            //     app.ChangeFileOpenDirectory(changePathTo);

            app.Visible = true;
            app = null;
        }

        public override bool isOfficeAppAvailable()
        {
            try
            {
                #pragma warning disable //disables "not used" message
                Microsoft.Office.Interop.Excel.XlCalculation test = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;
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
