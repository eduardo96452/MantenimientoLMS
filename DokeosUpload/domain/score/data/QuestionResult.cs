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

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class represents a question and its answers.
    /// </summary>
    class QuestionResult
    {
        public QuestionType answerType      { get; set; }
        public int          questionID      { get; set; }
        public String       questionTitle   { get; set; }
        public int          order           { get; set; }
        public int          resultQuestion  { get; set; }
        public int          weightQuestion  { get; set; }
        
        public List<AnswerResult> answers { get; set; }

        /// <summary>
        ///     Adds a question.
        /// </summary>
        /// <param name="answerType">The type of question: 1 = MC single; 2 = MC multiple; ...</param>
        /// <param name="questionID">The question ID.</param>
        /// <param name="questionTitle">The question title.</param>
        /// <param name="order">The order in the exercise.</param>
        /// <param name="resultQuestion">Score gained on this question.</param>
        /// <param name="weightQuestion">The maximum possible score on this question.</param>
        public QuestionResult(QuestionType answerType, int questionID, String questionTitle, int order, int resultQuestion, int weightQuestion)
        {
            this.answerType = answerType;
            this.questionID = questionID;
            this.questionTitle = questionTitle;
            this.order = order;
            this.resultQuestion = resultQuestion;
            this.weightQuestion = weightQuestion;
            this.answers = new List<AnswerResult>();
        }

        /// <summary>
        ///     Adds an answer.
        /// </summary>
        /// <param name="exerciseResult">The answer.</param>
        public void addAnswer(AnswerResult answer)
        {
            this.answers.Add(answer);
        }

        /// <summary>
        ///     Returns an answer.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The question.</returns>
        public AnswerResult getAnswerResultAt(int index)
        {
            return this.answers.ElementAt(index);
        }

        /// <summary>
        ///     Returns alls answers.
        /// </summary>
        /// <returns>The list of answers.</returns>
        public List<AnswerResult> getAnswerResults()
        {
            return this.answers;
        }

        /// <summary>
        ///     Read-only. The number of answers of this question.
        /// </summary>
        public int answerCount
        {
            get 
            {
                return this.answers.Count;
            }
        }

        /// <summary>
        ///     Returns the readable representation of the internal objects.
        /// </summary>
        /// <returns>The readable string.</returns>
        public String toString()
        {
            String value = Environment.NewLine + 
                "            Answer type    = " + this.answerType + Environment.NewLine + 
                "            Question ID    = " + this.questionID + Environment.NewLine + 
                "            Question Title = " + this.questionTitle + Environment.NewLine + 
                "            Order          = " + this.order + Environment.NewLine + 
                "            Result         = " + this.resultQuestion + Environment.NewLine + 
                "            Weight         = " + this.weightQuestion + Environment.NewLine +
                Environment.NewLine +
                "            # Answers      = " + answerCount;

            foreach(AnswerResult answer in this.answers)
            {
                value += answer.toString();
            }

            return value;
        }
    }
}
