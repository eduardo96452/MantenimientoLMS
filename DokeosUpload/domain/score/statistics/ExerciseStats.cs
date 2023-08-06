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
using lmsda.domain.score.data;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    /// </summary>
    class ExerciseStats
    {
        public int     exerciseID      { get; set; }
        public String  exerciseTitle   { get; set; }
        public List<String>  dateSolved      { get; set; }

        public List<QuestionStats> questionStats;

        /// <summary>
        ///     Default constructor.
        /// </summary>
        /// <param name="exerciseResult">The ExerciseResult object.</param>
        public ExerciseStats(ExerciseResult exerciseResult)
        {
            this.exerciseID = exerciseResult.exerciseID;
            this.exerciseTitle = exerciseResult.exerciseTitle;
            this.dateSolved = exerciseResult.dateSolved;
            this.questionStats = new List<QuestionStats>();
        }

        /// <summary>
        ///     Adds a question result.
        /// </summary>
        /// <param name="questionResult">The QuestionResult object.</param>
        /// <returns>True if the object has been added to the list.</returns>
        public Boolean addQuestion(QuestionResult questionResult)
        {
            Boolean add = true;

            foreach (QuestionStats q in questionStats)
            {
                if (q.questionID == questionResult.questionID)
                {
                    add = false;
                    break;
                }
            }

            if(add)
                questionStats.Add(new QuestionStats(questionResult.questionID, questionResult.questionTitle, questionResult.answerType));

            return add;
        }

        /// <summary>
        ///     Returns the index of a question.
        /// </summary>
        /// <param name="questionID">The question ID.</param>
        /// <returns>The index.</returns>
        public int getIndexOfQuestion(int questionID)
        {
            int index = -1;
            
            for (int i = 0; i < this.questionStats.Count; i++)
            {
                if (this.questionStats.ElementAt(i).questionID == questionID)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     Returns a QuestionStats object.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The QuestionStats object at that index.</returns>
        public QuestionStats getQuestionStat(int index)
        {
            return this.questionStats.ElementAt(index);
        }
    }
}
