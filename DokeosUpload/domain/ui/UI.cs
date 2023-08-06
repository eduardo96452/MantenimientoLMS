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
using System.Windows.Forms;

namespace lmsda.domain.ui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     MVC pattern.
    ///     All observers must implement this class and its methods.
    /// </summary>
    /// <remarks>
    ///     Last updated on 10/08/2010 by Gianni Van Hoecke
    ///      -> Since 1.08 this class was made a superclass for UI's.
    /// </remarks>
    interface UI
    {
        /// <summary>
        ///     Sends a message to the UI.
        /// </summary>
        /// <param name="message">The value which has to be displayed.</param>
        /// <param name="icon">The icon to display.</param>
        void showMessage(String message, MessageBoxIcon icon);

        /// <summary>
        ///     A method free to use.
        /// </summary>
        void update(Boolean refreshUI);
    }
}
