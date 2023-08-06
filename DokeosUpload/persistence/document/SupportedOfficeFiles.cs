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
using lmsda.persistence.document.microsoftoffice;
using lmsda.persistence.document.openoffice;

namespace lmsda.persistence.document
{
    /// <summary>
    ///     This class defines all supported office document implementations, and allows opening them.
    /// </summary>
    static class SupportedOfficeFiles
    {

        #region Definition of all supported document type implementations

        /// <summary>
        ///     Defines the exercises document types supported by the application. A new exercises document type can
        ///     be added by defining a new SupportedExercisesDocumentCommands class for it and putting it in this array.
        /// </summary>
        /// <returns>An array containing the SupportedExercisesDocumentCommands objects for all supported document types from which exercises can be extracted.</returns>
        public static SupportedDocumentCommands[] getSupportedExercisesDocumentTypes()
        {
            return new SupportedDocumentCommands[]
            {
                new MicrosoftWordDocumentCommands()
                // , new OpenOfficeDocumentCommands()
            };
        }

        /// <summary>
        ///     Defines the presentation document types supported by the application. A new presentation document type can
        ///     be added by defining a new SupportedPresentationDocumentCommands class for it and putting it in this array.
        /// </summary>
        /// <returns>An array containing the SupportedPresentationDocumentCommands objects for all supported presentation document types.</returns>
        public static SupportedDocumentCommands[] getSupportedPresentationDocumentTypes()
        {
            return new SupportedDocumentCommands[]
            {
                new MicrosoftPowerpointDocumentCommands()
            };
        }

        /// <summary>
        ///     Defines the spreadsheet document types supported by the application. A new spreadsheet document type can
        ///     be added by defining a new SupportedSpreadsheetDocumentCommands class for it and putting it in this array.
        /// </summary>
        /// <returns>An array containing the SupportedSpreadsheetDocumentCommands objects for all supported spreadsheet document types.</returns>
        public static SupportedDocumentCommands[] getSupportedSpreadsheetDocumentTypes()
        {
            return new SupportedDocumentCommands[]
            {
                new MicrosoftExcelDocumentCommands()
            };
        }

        /// <summary>
        ///     Defines all document types supported by the application. A new supported document type can
        ///     be added by defining a new SupportedDocumentCommands class for it and adding it to the list here.
        ///     
        ///     All of these document implementations should contain some kind of PDF conversion function, but
        ///     the difference in conversion parameters makes it impossible to have one uniform abstract function
        ///     definition for that, so manual casts will be needed for that.
        /// </summary>
        /// <returns>An array containing the SupportedDocumentCommands objects for all supported document types.</returns>
        public static SupportedDocumentCommands[] getSupportedDocumentTypes()
        {
            return new SupportedDocumentCommands[0]
                .Union(getSupportedExercisesDocumentTypes()).ToArray()
                .Union(getSupportedPresentationDocumentTypes()).ToArray()
                .Union(getSupportedSpreadsheetDocumentTypes()).ToArray();
        }
        #endregion

        #region Exercises documents

        /// <summary>
        ///     Factory: instantiates an new object of SupportedExercisesDocument, based on the extension.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <returns>A new SupportedExercisesDocument object.</returns>
        public static SupportedExercisesDocument ExercisesFactory(String document)
        {
            return ExercisesFactory(document, false);
        }

        /// <summary>
        ///     Factory: instantiates an new object of SupportedExercisesDocument, based on the extension.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <param name="plainText">True if the output has to be plain text.</param>
        /// <returns>A new SupportedExercisesDocument object.</returns>
        public static SupportedExercisesDocument ExercisesFactory(String document, Boolean plainText)
        {
            foreach (SupportedDocumentCommands doctype in getSupportedExercisesDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    if (document.EndsWith("." + ext))
                        return (SupportedExercisesDocument)doctype.factory(document);
            }
            // if the extension didn't match any existing document type
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Returns the extensions of all supported exercises document types.
        /// </summary>
        /// <returns>An array of extensions for all supported document types from which exercises can be extracted.</returns>
        public static String[] getSupportedExercisesDocumentExtensions()
        {
            List<String> extensions = new List<String>();

            foreach (SupportedDocumentCommands doctype in getSupportedExercisesDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    extensions.Add(ext);
            }
            return extensions.ToArray();
        }

        #endregion

        #region Presentation documents

        /// <summary>
        ///     Factory: instantiates an new object of SupportedPresentationDocument, based on the extension.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <param name="plainText">True if the output has to be plain text.</param>
        /// <returns>A new SupportedPresentationDocument object.</returns>
        public static SupportedPresentationDocument PresentationFactory(String document)
        {
            foreach (SupportedDocumentCommands doctype in getSupportedPresentationDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    if (document.EndsWith("." + ext))
                        return (SupportedPresentationDocument) doctype.factory(document);
            }
            // if the extension didn't match any existing document type
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Returns the extensions of all supported document types.
        /// </summary>
        /// <returns>An array of extensions for all supported document types.</returns>
        public static String[] getSupportedPresentationDocumentExtensions()
        {
            List<String> extensions = new List<String>();

            foreach (SupportedDocumentCommands doctype in getSupportedPresentationDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    extensions.Add(ext);
            }
            return extensions.ToArray();
        }

        #endregion
        
        #region Spreadsheet documents

        /// <summary>
        ///     Factory: instantiates an new object of SupportedSpreadsheetDocument, based on the extension.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <param name="plainText">True if the output has to be plain text.</param>
        /// <returns>A new SupportedSpreadsheetDocument object.</returns>
        public static SupportedSpreadsheetDocument SpreadsheetFactory(String document)
        {
            foreach (SupportedDocumentCommands doctype in getSupportedSpreadsheetDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    if (document.EndsWith("." + ext))
                        return (SupportedSpreadsheetDocument)doctype.factory(document);
            }
            // if the extension didn't match any existing document type
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Returns the extensions of all supported document types.
        /// </summary>
        /// <returns>An array of extensions for all supported document types.</returns>
        public static String[] getSupportedSpreadsheetDocumentExtensions()
        {
            List<String> extensions = new List<String>();

            foreach (SupportedDocumentCommands doctype in getSupportedSpreadsheetDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    extensions.Add(ext);
            }
            return extensions.ToArray();
        }

        #endregion

        #region All files

        /// <summary>
        ///     Factory: instantiates an new object of SupportedDocument, based on the extension.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <param name="plainText">True if the output has to be plain text.</param>
        /// <returns>A new SupportedExercisesDocument object.</returns>
        public static SupportedDocument factory(String document)
        {
            if (!document.Contains('.'))
                throw new ArgumentException();
            String docExt = document.Substring(document.LastIndexOf('.') + 1);

            foreach (SupportedDocumentCommands doctype in getSupportedDocumentTypes())
            {
                foreach (String ext in doctype.getSupportedExtensions())
                    if (docExt.Equals(ext))
                        return doctype.factory(document);
            }
            // if the extension didn't match any existing document type
            throw new NotSupportedException();
        }

        public static DocumentType identifyDocumentType(String document)
        {
            if (!document.Contains('.'))
                return DocumentType.UNKNOWN;
            String docExt = document.Substring(document.LastIndexOf('.') + 1);
            foreach (SupportedDocumentCommands doctype in getSupportedDocumentTypes())
            {
                if (doctype.getAllSupportedExtensions().Contains(docExt))
                    return doctype.getDocumentType();
            }
            return DocumentType.UNKNOWN;

        }        
        
        /// <summary>
        ///     Opens the document and changes the default directory to a user defined directory.
        ///     (This is the directory the user sees when he/she clicks on 'Save' or 'Open'.)
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        /// <param name="document">The full file name to the document. (the file to open)</param>
        /// <param name="changeDirectoryTo">Optional parameter. Changes the default directory. Use 'null' or 'String.Empty' to ignore this parameter.</param>
        public static void openDocument(String document, String changeDirectoryTo)
        {
            if (!document.Contains('.'))
                return;
            String docExt = document.Substring(document.LastIndexOf('.') + 1);
            foreach (SupportedDocumentCommands doctype in getSupportedDocumentTypes())
            {
                try
                {
                    foreach (String ext in doctype.getAllSupportedExtensions())
                        if (docExt.Equals(ext))
                        {
                            doctype.openDocument(document, changeDirectoryTo);
                            return;
                        }
                }
                catch (NotSupportedException)
                {
                    //Method not supported for this document type.
                }
                catch (Exception)
                {
                    // error with document type
                }
            }
        }

        /// <summary>
        ///     Returns all opened exercises documents.
        /// </summary>
        /// <returns>An Array of all opened exercises documents.</returns>
        public static String[] getAllActiveDocuments(DocumentType documenttype)
        {
            SupportedDocumentCommands[] doctypes = null;
            switch (documenttype)
            {
                case DocumentType.PROCESSEDTEXT_DOCUMENT:
                    doctypes = getSupportedExercisesDocumentTypes();
                    break;
                case DocumentType.PRESENTATION_DOCUMENT:
                    doctypes = getSupportedPresentationDocumentTypes();
                    break;
                case DocumentType.SPREADSHEET_DOCUMENT:
                    doctypes = getSupportedSpreadsheetDocumentTypes();
                    break;
                case DocumentType.ALL:
                    doctypes = getSupportedDocumentTypes();
                    break;
            }

            List<String> documents = new List<String>();

            if (doctypes != null)
            {
                foreach (SupportedDocumentCommands doctype in doctypes)
                {
                    try
                    {
                        foreach (String doc in doctype.getAllActiveDocuments())
                            documents.Add(doc);
                    }
                    catch (NotSupportedException)
                    {
                        //Method not supported for this document type.
                    }
                    catch (Exception)
                    {
                        // error with document type
                    }
                }
                documents.Sort();
            }
            return documents.ToArray();
        }

        #endregion
    }
}
