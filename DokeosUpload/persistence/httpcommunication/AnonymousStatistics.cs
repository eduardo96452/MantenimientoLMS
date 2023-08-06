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
using lmsda.domain;
using lmsda.domain.exercise;

namespace lmsda.persistence.httpcommunication
{
    /// <summary>
    ///     Sends anonymous statistics to the LMSDA server!
    /// </summary>
    /// <remarks>
    ///	    As of 1.09
    ///     Last updated on 14/04/2011 by Maarten Meuris
    /// </remarks>
    class AnonymousStatistics
    {
        /// <summary>
        ///     Sends anonymous statistics to the LMSDA server.
        /// </summary>
        /// <param name="question_count">The number of questions.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public static Boolean sendStatistics(int question_count)
        {
            HttpSession session = new HttpSession(Encoding.UTF8, ProgramConstants.USERAGENT_LMSDA);
            session.setRequestUrl("http://lmsda.sourceforge.net/statistics/add_statistic.php");
            session.addNameValuePair("version", ProgramConstants.programVersion());
            session.addNameValuePair("count", Convert.ToString(question_count));
            session.sendPOSTrequestFromForm();

            return session.getResponseFromServer().Equals("OK");
        }

        /// <summary>
        ///     Sends anonymous statistics to the LMSDA server, based on a list of exercises.
        /// </summary>
        /// <param name="exercises">The list of exercises.</param>
        /// <returns>True if succeeded, false otherwise.</returns>
        public static Boolean sendStatistics(List<Exercise> exercises)
        {
            int question_count = 0;

            foreach (Exercise ex in exercises)
                question_count += ex.getQuestionsAsList().Count;

            return AnonymousStatistics.sendStatistics(question_count);
        }
    }
}
