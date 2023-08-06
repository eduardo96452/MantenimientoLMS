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

namespace lmsda.domain.exercise
{
    /// <summary>
    ///     Author: Maarten Meuris, Gianni Van Hoecke
    ///     This class represents a single question, with a list of answers.
    /// </summary>
    class Question
    {
        private String questionTitle;
        private String questionText;
        private List<Answer> answers;
        private QuestionType questionType;

        /// <summary>
        ///     The basic constructor, which makes an empty Question object.
        /// </summary>
        public Question()
        {
            answers = new List<Answer>();
            questionType=QuestionType.NOT_SET;
        }

        /// <summary>
        ///     The general constructor, which sets the most common properties.
        /// </summary>
        /// <param name="title">The question title</param>
        /// <param name="text">the question text</param>
        /// <param name="type">The question type</param>
        public Question(String title, String text, QuestionType type) 
        {
            answers = new List<Answer>();
            this.questionTitle = title;
            this.questionText = text;
            this.questionType = type;
        }

        /// <summary>
        ///     The full constructor, which creates a full question with answers.
        /// </summary>
        /// <param name="title">De vraagtitel</param>
        /// <param name="text">Extra commentaar bij de vraag</param>
        /// <param name="type">Het vraagtype</param>
        /// <param name="answers">Een volledige collectie van Answer-objecten</param>
        public Question(String title, String text, QuestionType type, List<Answer> answers)
            : this(title,text,type)
        {
            this.answers = new List<Answer>(answers);
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
            String dump = this.toStringSimple(prefix);
            foreach (Answer ans in answers)
            {
                dump += Environment.NewLine + ans.toString(prefix + "    ");
            }
            return dump;
        }
        
        /// <summary>
        ///     Returns the full contents of the object as String, without the answers.
        /// </summary>
        /// <param name="prefix">text to put in front of each line</param>
        /// <returns>The readable version of the object.</returns>
        public String toStringSimple(String prefix)
        {
            String dump = prefix + "title" + " = " + (this.getQuestionTitle() != null  && !this.getQuestionTitle().Equals(String.Empty) ? this.getQuestionTitle() : "[[not set]]") + Environment.NewLine
                        + prefix + "text" + " = " + (this.getQuestionText() != null && !this.getQuestionText().Equals(String.Empty) ? this.getQuestionText() : "[[not set]]") + Environment.NewLine
                        + prefix + "type" + " = " + (this.getQuestionType() != QuestionType.NOT_SET ? this.getQuestionType().ToString() : "[[not set]]");
            return dump;
        }

        /// <summary>
        ///     Returns the full contents of the object as String, using translated strings.
        /// </summary>
        /// <param name="prefix">text to put in front of each line</param>
        /// <returns>The readable version of the object.</returns>
        public String toStringTranslated(String prefix)
        {
            String dump = this.toStringSimpleTranslated(prefix);
            foreach (Answer ans in answers)
            {
                dump += Environment.NewLine + ans.toStringTranslated(prefix + "    ");
            }
            return dump;
        }

        /// <summary>
        ///     Returns the full contents of the object as String, without the answers, using translated strings.
        /// </summary>
        /// <param name="prefix">text to put in front of each line</param>
        /// <returns>The readable version of the object.</returns>
        public String toStringSimpleTranslated(String prefix)
        {
            DomainController dc = DomainController.Instance();
            String dump = prefix + dc.getLanguageString("ed_title") + " = " + (this.getQuestionTitle() != null && !this.getQuestionTitle().Equals(String.Empty) ? this.getQuestionTitle() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                        + prefix + dc.getLanguageString("ed_text") + " = " + (this.getQuestionText() != null && !this.getQuestionText().Equals(String.Empty) ? this.getQuestionText() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                        + prefix + dc.getLanguageString("ed_type") + " = " + (this.getQuestionType() != QuestionType.NOT_SET ? this.getQuestionType().ToString() : dc.getLanguageString("ed_not_set"));
            return dump;
        }
        /// <summary>
        ///     Checks if this object is empty.
        /// </summary>
        /// <returns>True if all properties are empty.</returns>
        public Boolean isEmpty()
        {
            return (questionTitle == null || questionTitle.Equals(String.Empty))
                && (questionText  == null || questionText.Equals(String.Empty) )
                && (answers       == null || answers.Count == 0                )
                && (questionType  == QuestionType.NOT_SET                      );
        }

        /// <summary>
        ///     Returns an new object of an exercise, but with the same values.
        /// </summary>
        /// <returns>An new answer object, with the same values.</returns>
        public Question clone()
        {
            Question retn = new Question(this.questionTitle,this.questionText,this.questionType);
            foreach (Answer ans in answers)
            {
                retn.addAnswer(ans.clone());
            }
            return retn;
        }

        #region Getters and setters

        public void addAnswer(Answer answer)
        {
            answers.Add(answer);
        }

        public List<Answer> getAnswersAsList()
        {
            return answers;
        }

        public Answer[] getAnswersAsArray()
        {
            return answers.ToArray();
        }

        public Answer getAnswer(int i)
        {
            return getAnswersAsArray()[i];
        }

        public void setQuestionTitle(String text)
        {
            this.questionTitle = text;
        }

        public String getQuestionTitle()
        {
            return questionTitle;
        }

        public void setQuestionText(String comment)
        {
            this.questionText = comment;
        }

        public String getQuestionText()
        {
            return questionText;
        }

        public void setQuestionType(QuestionType type)
        {
            this.questionType = type;
        }

        public QuestionType getQuestionType()
        {
            return questionType;
        }

        #endregion

    }
}
