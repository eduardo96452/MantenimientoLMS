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

namespace lmsda.domain.score.data
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class represents an exercise and its questions.
    /// </summary>
    class ExerciseResult
    {
        public int          exerciseID      { get; set; }
        public String       exerciseTitle   { get; set; }
        public List<String> dateSolved      { get; set; }
        public List<int>    resultExercise  { get; set; }
        public List<int>    weightExercise  { get; set; }

        public List<QuestionResult> questionResults { get; set; }

        /// <summary>
        ///     Adds an exercise.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <param name="exerciseTitle">The exercise title.</param>
        /// <param name="dateSolved">The date when this exercise was solved.</param>
        /// <param name="resultExercise">The score gained.</param>
        /// <param name="weightExercise">The maximum possible score.</param>
        /// <remarks>
        ///     Last updated on 16/08/2010 by Gianni Van Hoecke
        ///      -> An exercise can be solved multiple times (as of 1.08)
        /// </remarks>
        public ExerciseResult(int exerciseID, String exerciseTitle, String dateSolved, int resultExercise, int weightExercise)
        {
            this.exerciseID = exerciseID;
            this.exerciseTitle = exerciseTitle;
            this.dateSolved = new List<String>();
            this.dateSolved.Add(dateSolved);
            this.resultExercise = new List<int>();
            this.weightExercise = new List<int>();
            this.resultExercise.Add(resultExercise);
            this.weightExercise.Add(weightExercise);
            this.questionResults = new List<QuestionResult>();
        }

        /// <summary>
        ///     As of 1.08, an exercise can be solved multiple times. Calling this method will add another score 
        ///     for <b>this</b> exercise.
        /// </summary>
        /// <param name="resultExercise">The score gained.</param>
        /// <param name="weightExercise">The maximum possible score.</param>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 16/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void addResult(int resultExercise, int weightExercise, String dateSolved)
        {
            if (!this.dateSolved.Contains(dateSolved))
            {
                this.dateSolved.Add(dateSolved);
                this.resultExercise.Add(resultExercise);
                this.weightExercise.Add(weightExercise);
            }
        }

        /// <summary>
        ///     Adds a question.
        /// </summary>
        /// <param name="questionResult">The question.</param>
        public void addQuestionResult(QuestionResult questionResult)
        {
            this.questionResults.Add(questionResult);
        }

        /// <summary>
        ///     Returns a question.
        /// </summary>
        /// <param name="index">The index of the question.</param>
        /// <returns>The question.</returns>
        public QuestionResult getQuestionResultAt(int index)
        {
            return this.questionResults.ElementAt(index);
        }

        /// <summary>
        ///     Read-only. Returns the number of questions of this exercise.
        /// </summary>
        public int questionCount
        {
            get 
            {
                return this.questionResults.Count;
            }
        }

        /// <summary>
        ///     Returns a question, based on the question ID.
        /// </summary>
        /// <param name="id">The question ID.</param>
        /// <returns>The question.</returns>
        public QuestionResult getQuestion(int id)
        {
            foreach(QuestionResult question in this.questionResults)
            {
                if(question.questionID == id)
                    return question;
            }

            return null;
        }

        /// <summary>
        ///     Returns the readable representation of the internal objects.
        /// </summary>
        /// <returns>The readable string.</returns>
        public String toString()
        {
            String value = Environment.NewLine + 
                "      Exercise ID    = " + this.exerciseID + Environment.NewLine + 
                "      Exercise title = " + this.exerciseTitle + Environment.NewLine + 
                "      Date solved    = " + this.dateSolved + Environment.NewLine + 
                "      Result         = " + this.resultExercise + Environment.NewLine + 
                "      Weight         = " + this.weightExercise + Environment.NewLine + 
                Environment.NewLine +
                "      # Questions    = " + questionCount;

            foreach(QuestionResult question in this.questionResults)
            {
                value += question.toString();
            }

            return value;
        }
    }
}
