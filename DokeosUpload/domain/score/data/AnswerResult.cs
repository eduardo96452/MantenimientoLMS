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

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class represents an answer.
    /// </summary>
    class AnswerResult
    {
        public String  answer       { get; set; }
        public Boolean correct      { get; set; }
        public Boolean chosen       { get; set; } //True if the answer was chosen by the student.

        /// <summary>
        ///     Default constructor, adds an answer.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="correct">True if the answer is correct.</param>
        /// <param name="chosen">True if the student has chosen the answer.</param>
        public AnswerResult(String answer, Boolean correct, Boolean chosen)
        {
            this.answer = answer;
            this.correct = correct;
            this.chosen = chosen;
        }

        /// <summary>
        ///     Returns the readable representation of the internal objects.
        /// </summary>
        /// <returns>The readable string.</returns>
        public String toString()
        {
            String value = Environment.NewLine + 
                "                  Answer  = " + this.answer + Environment.NewLine + 
                "                  Correct = " + this.correct + Environment.NewLine + 
                "                  Chosen  = " + this.chosen + Environment.NewLine;

            return value;
        }
    }
}
