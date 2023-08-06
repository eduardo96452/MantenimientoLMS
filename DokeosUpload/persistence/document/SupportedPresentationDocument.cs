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
using lmsda.domain.util;

namespace lmsda.persistence.document
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     The abstract superclass of documents containing exercises.
    /// </summary>
    abstract class SupportedPresentationDocument : SupportedDocument
    {

        /// <summary>
        ///     Converts the loaded document to PDF.
        /// </summary>
        /// <param name="destinationPath">Target folder.</param>
        /// <param name="convertHyperlinksToJavascript">True if this method has to convert the hyperlinks.</param>
        /// <param name="error">True if error occurred.</param>
        /// <returns>The list with the converted documents.</returns>
        public abstract List<String> convertToPDF(String destinationPath, Boolean frameSlides, Boolean horizontal, PresentationPublishTypes publishMethod, int slidesPerPage, Boolean includeHiddenSlides, ref Boolean error);

    }
}