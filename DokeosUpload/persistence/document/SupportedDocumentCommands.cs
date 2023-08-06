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

namespace lmsda.persistence.document
{
    /// <summary>
    ///     This class contains the abstracts for the identifying properties needed for a document type.
    ///     This class should still be extended to document types which contain the factory.
    ///     
    ///     The actual documentCommands classes made from the extensions of this class are used to make
    ///     a single array that can be used for all actions related to document type choices.
    /// </summary>
    abstract class SupportedDocumentCommands
    {

        /// <summary>
        ///     Returns an array of filenames for all documents currently opened in the
        ///     editor program that normally handles this type. Returns a NotSupportedException
        ///     if the current document type doesn't support listing of opened documents.
        /// </summary>
        /// <returns>An array of filenames of open documents of this type</returns>
        public abstract List<String> getAllActiveDocuments();

        /// <summary>
        ///     Returns a list of all supported extensions (without leading dots) for this document type.
        ///     Do not use this method for factory.
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        /// <returns>A list of all possible filename extensions for this document type</returns>
        public abstract List<String> getAllSupportedExtensions();

        /// <summary>
        ///     Returns a list of extensions (without leading dots) that identify a document of this type.
        /// </summary>
        /// <returns>A list of the filename extensions for this document type that should be recognized as normal documents to open</returns>
        public abstract List<String> getSupportedExtensions();

        /// <summary>
        ///     Factory: instantiates a document of this document type.
        /// </summary>
        /// <param name="document">The full file name to the document.</param>
        /// <returns>A new document object of this document type</returns>
        public abstract SupportedDocument factory(String document);

        /// <summary>
        ///     Identified the document type
        /// </summary>
        /// <returns>A new document object of this document type</returns>
        public abstract DocumentType getDocumentType();
        
        /// <summary>
        ///     Opens the document and changes the working directory to a user defined directory.
        ///     (This is the directory the user sees when clicking on 'Save' or 'Open'.)
        /// </summary>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 09/08/2010 by Gianni Van Hoecke
        /// </remarks>
        /// <param name="path">The full file name to the document. (the file to open)</param>
        /// <param name="changePathTo">Optional parameter. Changes the working directory. Use 'null' or 'String.Empty' to ignore this parameter.</param>
        public abstract void openDocument(String path, String changePathTo);

        /// <summary>
        ///     Does a check to see if the office application for this type is available.
        /// </summary>
        /// <returns></returns>
        public abstract Boolean isOfficeAppAvailable();

    }
}
