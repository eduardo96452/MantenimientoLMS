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
using lmsda.domain.exercise;
using lmsda.domain.score.data;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    class QuestionStats
    {
        public QuestionType answerType      { get; set; }
        public int          questionID      { get; set; }
        public String       questionTitle   { get; set; }

        public List<AnswerStats> answerStats;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="questionID">The question ID.</param>
        /// <param name="questionTitle">The question title.</param>
        /// <param name="answerType">The answer type of the question.</param>
        public QuestionStats(int questionID, String questionTitle, QuestionType answerType)
        {
            this.questionID = questionID;
            this.questionTitle = questionTitle;
            this.answerType = answerType;
            this.answerStats = new List<AnswerStats>();
        }

        /// <summary>
        ///     Adds an answer to the list.
        /// </summary>
        /// <param name="answerResult">The AnswerResult object.</param>
        /// <returns>True if the answer has been added.</returns>
        public Boolean addAnswer(AnswerResult answerResult)
        {
            Boolean add = true;
            int index = -1; 

            for(int i = 0; i < answerStats.Count; i++)
            {
                if(answerStats.ElementAt(i).answer.Equals(answerResult.answer))
                {
                    add = false;
                    index = i;
                    break;
                }
            }

            if(add)
                answerStats.Add(new AnswerStats(answerResult.answer, answerResult.correct, answerResult.chosen));
            else if(answerResult.chosen)
                answerStats.ElementAt(index).addNewChosen();

            return add;
        }
    }
}
