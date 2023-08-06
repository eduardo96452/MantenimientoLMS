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

namespace lmsda.domain.exercise
{
    /// <summary>
    ///     Author: Gianni Van Hoecke, Maarten Meuris
    ///     This class represents one single answer.
    /// </summary>
    class Answer
    {
        private String answer;
        private String matchAnswer;
        private int weight;
        private String feedback;

        /// <summary>
        ///     Default constructor. Only instantiates an empty object.
        /// </summary>
        public Answer()
        {
            this.weight = int.MinValue;
        }

        /// <summary>
        ///     Instantiates an object with an answer and its score and feedback.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="weight">The score (positive or negative) of this answser.</param>
        /// <param name="feedback">The feedback of this answer.</param>
        public Answer(String answer, int weight, String feedback)
        {
            this.setAnswer(answer);
            this.setMatchAnswer(null);
            this.setWeight(weight);
            this.setFeedback(feedback);
        }

        /// <summary>
        ///     Instantiates an object with an answer and its score and feedback.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="weight">The score (positive or negative) of this answser.</param>
        /// <param name="feedback">The feedback of this answer.</param>
        public Answer(String answer, String matchanswer, int weight, String feedback)
        {
            this.setAnswer(answer);
            this.setMatchAnswer(matchanswer);
            this.setWeight(weight);
            this.setFeedback(feedback);
        }

        /// <summary>
        ///     Instantiates an object with an answer and its score.
        /// </summary>
        /// <param name="answer">The answer.</param>
        /// <param name="weight">The score (positive or negative) of this answer.</param>
        public Answer(String answer, int weight)
        {
            this.setAnswer(answer);
            this.setWeight(weight);
        }

        /// <summary>
        ///     Returns the full contents of the object as String.
        /// </summary>
        /// <returns>The readable version of the object.</returns>
        public String toString()
        {
            return this.toStringTranslated(String.Empty);
        }

        /// <summary>
        ///     Returns the full contents of the object as String.
        /// </summary>
        /// <param name="prefix">text to put in front of each line</param>
        /// <returns>The readable version of the object.</returns>
        public String toString(String prefix)
        {
            return prefix + "answer"      + " = " + (this.getAnswer() != null && !this.getAnswer().Equals(String.Empty) ? this.getAnswer() : "[[not set]]") + Environment.NewLine
                 + prefix + "matchanswer" + " = " + (this.getMatchAnswer() != null && !this.getMatchAnswer().Equals(String.Empty) ? this.getMatchAnswer() : "[[not set]]") + Environment.NewLine
                 + prefix + "weight"      + " = " + (this.getWeight() != int.MinValue ? this.getWeight().ToString() : "[[not set]]") + Environment.NewLine
                 + prefix + "feedback"    + " = " + (this.getFeedback() != null && !this.getFeedback().Equals(String.Empty) ? this.getFeedback() : "]]not set]]");
        }

        /// <summary>
        ///     Returns the full contents of the object as String, using translated strings.
        /// </summary>
        /// <param name="prefix">text to put in front of each line</param>
        /// <returns>The readable version of the object.</returns>
        public String toStringTranslated(String prefix)
        {
            DomainController dc = DomainController.Instance();
            return prefix + dc.getLanguageString("ed_answer") + " = " + (this.getAnswer() != null && !this.getAnswer().Equals(String.Empty) ? this.getAnswer() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                 + prefix + dc.getLanguageString("ed_matchanswer") + " = " + (this.getMatchAnswer() != null && !this.getMatchAnswer().Equals(String.Empty) ? this.getMatchAnswer() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                 + prefix + dc.getLanguageString("ed_weight") + " = " + (this.getWeight() != int.MinValue ? this.getWeight().ToString() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                 + prefix + dc.getLanguageString("ed_feedback") + " = " + (this.getFeedback() != null && !this.getFeedback().Equals(String.Empty) ? this.getFeedback() : dc.getLanguageString("ed_not_set"));
        }

        /// <summary>
        ///     Checks if this object is empty.
        /// </summary>
        /// <returns>True if all properties are empty.</returns>
        public Boolean isEmpty()
        {
            return (answer == null   || answer.Equals(String.Empty))
                && (matchAnswer == null || matchAnswer.Equals(String.Empty))
                && (feedback == null || feedback.Equals(String.Empty))
                && (weight == int.MinValue);
        }

        /// <summary>
        ///     Returns an new object of an answer, but with the same values.
        /// </summary>
        /// <returns>An new answer object, with the same values.</returns>
        public Answer clone()
        {
            return new Answer(this.answer, this.matchAnswer, this.weight, this.feedback);
        }

        #region Getters and setters

        public void setAnswer(String answer)
        {
            this.answer = answer;
        }

        public String getAnswer()
        {
            return this.answer;
        }

        public void setMatchAnswer(String matchAnswer)
        {
            this.matchAnswer = matchAnswer;
        }

        public String getMatchAnswer()
        {
            return this.matchAnswer;
        }

        public void setFeedback(String feedback)
        {
            this.feedback = feedback;
        }

        public String getFeedback()
        {
            return this.feedback;
        }

        public void setWeight(int weight)
        {
            this.weight = weight;
        }

        public int getWeight()
        {
            return this.weight;
        }

        public String getWeightString()
        {
            String retn = this.weight.ToString();
            if (this.weight > 0)
                retn= '+' + retn;
            return retn;
        }
        #endregion

    }
}
