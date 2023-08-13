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
using lmsda.persistence.document.microsoftoffice;
using lmsda.persistence.document.openoffice;
using lmsda.domain.exercise;
using lmsda.domain;

namespace lmsda.persistence.document
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     The abstract superclass of documents containing exercises.
    /// </summary>
    abstract class SupportedExercisesDocument : SupportedDocument
    {
        public Boolean PlainText { get; set; }

        /// <summary>
        ///     Returns the number of warnings encountered during the scan.
        /// </summary>
        /// <returns>The number of warnings.</returns>
        public abstract int getDocumentScanWarnings();

        /// <summary>
        ///     Returns a list of exercises.
        /// </summary>
        /// <returns>A list of exercises.</returns>
        public abstract List<Exercise> getExercises();

        /// <summary>
        ///     Scans all exercises from the loaded document.
        /// </summary>
        public abstract void extractExercises();

        /// <summary>
        ///     Converts the loaded document to PDF.
        /// </summary>
        /// <param name="destinationPath">Target folder.</param>

        /// <param name="replaceSpacesByUndescores">True if the final filename should contain underscores instead of spaces.</param>
        /// <param name="convertHyperlinksToJavascript">True if this method has to convert the hyperlinks.</param>
        /// <param name="error">True if error occurred.</param>
        /// <returns>The list with the converted documents.</returns>
        public abstract List<String> convertToPDF(String destinationPath, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error);

        /// <summary>
        ///     Converts the loaded document to multiple PDF files. Plitting is based on style name.
        /// </summary>
        /// <param name="destinationPath">Target folder. Null for temporary folder.</param>
        /// <param name="splitAt">Split at the given paragraph name.</param>
        /// <param name="splitOnPage">True if this method has to split per page.</param>
        /// <param name="replaceSpacesByUndescores">True if the final filenames should contain underscores instead of spaces.</param>
        /// <param name="convertHyperlinksToJavascript">True if this method has to convert the hyperlinks.</param>
        /// <param name="namePattern"></param>
        /// <param name="error">True if error occurred.</param>
        /// <returns>The list with the converted documents.</returns>
        public abstract List<String> convertToPDFWithSplit(String destinationPath, String splitAt, String namePattern, Boolean splitOnPage, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error);

        /// <summary>
        ///     Returns a list with MD5 hashes for all exercises.
        /// </summary>
        /// <returns>A list with MD5 hashes for all exercises</returns>
        public abstract List<String> getExerciseMD5s();

        /// <summary>
        ///     Opens the document in the target application and jumps to a specified section containing an error.
        /// </summary>
        /// <param name="exerciseNumber">Exercise number.</param>
        /// <param name="questionNumber">Question number (-1 jumps to exercise).</param>
        /// <param name="answerNumber">Answer number (-1 jumps to question).</param>
        public abstract void jumpToError(int exerciseNumber, int questionNumber, int answerNumber);

        /// <summary>
        ///     Indicates if this document type supports Jump to Error.
        /// </summary>
        public abstract Boolean supportsJumpToSection();

        /// <summary>
        ///     Returns the readable form of the exercises, questions and answers.
        /// </summary>
        /// <returns>A readable string of exercises.</returns>
        public String exercisesToString()
        {
            List<Exercise> exercises = this.getExercises();
            DomainController dc = DomainController.Instance();
            String dump = "";
            if (exercises != null)
            {
                for (int i_ex = 0; i_ex < exercises.Count; i_ex++)
                {
                    if (i_ex != 0) dump += Environment.NewLine + Environment.NewLine;

                    // "ed" = "Exercise Dump"
                    dump += dc.getLanguageString("ed_exercise") + " " + (i_ex + 1) + ":" + Environment.NewLine
                        + dc.getLanguageString("ed_title") + " = "
                        + (exercises[i_ex].getName() != null && !exercises[i_ex].getName().Equals(String.Empty) ?
                            exercises[i_ex].getName() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                        + dc.getLanguageString("ed_description") + " = "
                        + (exercises[i_ex].getDescription() != null && !exercises[i_ex].getDescription().Equals(String.Empty) ?
                            exercises[i_ex].getDescription() : dc.getLanguageString("ed_not_set")) + Environment.NewLine;

                    List<Question> questions = exercises[i_ex].getQuestionsAsList();
                    for (int i_ques = 0; i_ques < questions.Count; i_ques++)
                    {
                        dump += Environment.NewLine
                            + "      " + dc.getLanguageString("ed_question") + " " + (i_ques + 1) + ":" + Environment.NewLine
                            + "      " + dc.getLanguageString("ed_title") + " = "
                            + (questions[i_ques].getQuestionTitle() != null  && !questions[i_ques].getQuestionTitle().Equals(String.Empty) ?
                                questions[i_ques].getQuestionTitle() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                            + "      " + dc.getLanguageString("ed_text") + " = "
                            + (questions[i_ques].getQuestionText() != null  && !questions[i_ques].getQuestionText().Equals(String.Empty) ? 
                                questions[i_ques].getQuestionText() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                            + "      " + dc.getLanguageString("ed_type") + " = "
                            + dc.getLanguageString("ed_"+questions[i_ques].getQuestionType().ToString().ToLower()) + Environment.NewLine;

                        List<Answer> answers = questions[i_ques].getAnswersAsList();
                        for (int i_ans=0; i_ans < answers.Count; i_ans++)
                        {
                            dump += Environment.NewLine
                                 + "                  " + dc.getLanguageString("ed_answer")+ " " + (i_ans+1) + ":"+ Environment.NewLine;

                            if (questions[i_ques].getQuestionType() != QuestionType.MATCHING
                                || ((answers[i_ans].getMatchAnswer() == null  || answers[i_ans].getMatchAnswer().Equals(String.Empty))
                                    || (answers[i_ans].getAnswer() != null  && !answers[i_ans].getAnswer().Equals(String.Empty)))
                                )
                            {
                                dump += "                  " + dc.getLanguageString("ed_answer")   + " = "
                                     + (answers[i_ans].getAnswer() != null  && !answers[i_ans].getAnswer().Equals(String.Empty) ?
                                        answers[i_ans].getAnswer() : dc.getLanguageString("ed_not_set")) + Environment.NewLine;
                            }
                            if (questions[i_ques].getQuestionType() == QuestionType.MATCHING
                                && (answers[i_ans].getMatchAnswer() != null && !answers[i_ans].getMatchAnswer().Equals(String.Empty)))
                            {
                                dump +=  "                  " + dc.getLanguageString("ed_matchanswer")   + " = "
                                     + (answers[i_ans].getMatchAnswer() != null  && !answers[i_ans].getMatchAnswer().Equals(String.Empty) ?
                                        answers[i_ans].getMatchAnswer() : dc.getLanguageString("ed_not_set")) + Environment.NewLine;
                            }
                            
                            dump += "                  " + dc.getLanguageString("ed_weight")   + " = "
                                 + (answers[i_ans].getWeight() != int.MinValue ?
                                    answers[i_ans].getWeight().ToString() : dc.getLanguageString("ed_not_set")) + Environment.NewLine
                                 + "                  " + dc.getLanguageString("ed_feedback") + " = "
                                 + (answers[i_ans].getFeedback() != null && !answers[i_ans].getFeedback().Equals(String.Empty) ?
                                    answers[i_ans].getFeedback() : dc.getLanguageString("ed_not_set")) + Environment.NewLine;
                        }
                    }
                }
            }
            return dump;
        }

    }
}