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

namespace lmsda.domain.user.synchronization
{
    /// <summary>
    ///     Author: Maarten Meuris
    /// </summary>
    enum SynchronizationStatus
    {
        UNKNOWN,
        FOLDER_UPLOAD,
        FOLDER_EXCLUDE,
        FILE_ADDED,
        FILE_CHANGED,
        FILE_EXCLUDED,
        FILE_SYNCHRONYZED,
        FILE_CONVERTEDPDF,
        FILE_ERROR,
    }
}
