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
    ///     This class represents the frequency in percentage of an answer.
    /// </summary>
    class AnswerPercentage
    {
        public String answer     { get; set; }
        public double percentage { get; set; }
        public Boolean isCorrect { get; set; }

        /// <summary>
        ///     Instantiates an object.
        /// </summary>
        /// <param name="answer">The answer text.</param>
        /// <param name="percentage">The frequency of this answer, in percentage.</param>
        /// <param name="isCorrect">True if this is a correct answer.</param>
        public AnswerPercentage(String answer, double percentage, Boolean isCorrect)
        {
            this.answer = answer;
            this.percentage = percentage;
            this.isCorrect = isCorrect;
        }
    }
}
