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

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class represents the frequency of an answer.
    /// </summary>
    class AnswerStats
    {
        public String   answer      { get; set; }
        public Boolean  correct     { get; set; }
        public int      timesChosen { get; set; }

        /// <summary>
        ///     Instantiates an new object.
        /// </summary>
        /// <param name="answer">The answer text.</param>
        /// <param name="correct">True if this is a correct answer.</param>
        /// <param name="choosen">True if this answer was choosen.</param>
        public AnswerStats(String answer, Boolean correct, Boolean choosen)
        {
            this.answer = answer;
            this.correct = correct;
            if(choosen)
                this.timesChosen = 1;
            else
                this.timesChosen = 0;
        }

        /// <summary>
        ///     Adds 1 (one) to the total of choosen.
        /// </summary>
        public void addNewChosen()
        {
            this.timesChosen++;
        }
    }
}
