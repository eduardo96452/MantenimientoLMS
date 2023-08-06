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
using System.Linq;
using Microsoft.Office.Interop.Excel;

namespace lmsda.domain.util
{
    /// <summary>
    ///     Author: Gianni Van Hoecke,
    ///             Maarten Meuris
    /// </summary>
    static class ExcelUtility
    {
        /// <summary>
        ///     Returns an Excel cell based on indices.
        /// </summary>
        /// <param name="column">The column number.</param>
        /// <param name="row">The row number.</param>
        /// <param name="fix">True if this method has to return fixed cells ($).</param>
        /// <returns>The Excel cell.</returns>
        public static String getCellFromColumnIndex(int column, int row, Boolean fix)
        {
            String cell = String.Empty;

            if (fix)
                cell = "$" + getColumnNameFromIndex(column) + "$" + Convert.ToString(row);
            else
                cell = getColumnNameFromIndex(column) + Convert.ToString(row);

            return cell;
        }

        /// <summary>
        ///     Gets the column name by index.
        /// </summary>
        /// <param name="columnNumber">The column number (index).</param>
        /// <returns>The column name.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 11/03/2011 by Maarten Meuris
        /// </remarks>
        public static String getColumnNameFromIndex(int column)
        {
            column--;
            String col = Convert.ToString((char)('A' + (column % 26)));
            while (column >= 26)
            {
                column = (column / 26) -1;
                col = Convert.ToString((char)('A' + (column % 26))) + col;
            }
            return col;
        }

        /// <summary>
        ///     Searches for a specific range name in a sheet.
        /// </summary>
        /// <param name="sheet">The Excel worksheet.</param>
        /// <param name="rangename">The range name to search for.</param>
        /// <returns>True if the sheet contains the range name.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public static Boolean sheetContainsRangeName(_Worksheet sheet, String rangename)
        {
            foreach (Name s in sheet.Names)
            {
                if (s.Name.EndsWith("!" + rangename))
                    return true;
            }
            return false;
        }
    }
}
