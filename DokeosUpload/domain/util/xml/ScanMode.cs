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

namespace lmsda.domain.util.xml
{
    enum ScanMode
    {
        /// <summary>
        /// The node text is exactly the same as the search string
        /// </summary>
        EQUALS,
        /// <summary>
        /// The node text contains the search string
        /// </summary>
        CONTAINS,
        /// <summary>
        /// Strings are either equal, or after stripping the truncation
        /// from the end, the node text equals the start of the search text
        /// </summary>
        SAMETRUNCATEDSTART
    }
}
