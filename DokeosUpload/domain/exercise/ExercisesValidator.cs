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
using System.Text;
using System.IO;

namespace lmsda.domain.exercise
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///     This static class validates an array of Exercise objects, and provides output in various ways.
    /// </summary>
    static class ExercisesValidator
    {
        private const String EX_HTML_1 = "<tr><td colspan=\"2\">";
        private const String EX_HTML_2 = "</td><td colspan=\"3\">";
        private const String EX_HTML_3 = "</td></tr>";
        private const String QU_HTML_1 = "<tr><td width=\"50\">&nbsp;</td><td colspan=\"2\">";
        private const String QU_HTML_2 = "</td><td colspan=\"2\">";
        private const String QU_HTML_3 = "</td></tr>";
        private const String AN_HTML_1 = "<tr><td width=\"50\">&nbsp;</td><td width=\"50\">&nbsp;</td><td colspan=\"2\">";
        private const String AN_HTML_2 = "</td><td>";
        private const String AN_HTML_3 = "</td></tr>";
        private const String HTML_RED1 = "<span style=\"color:red; font-weight=bold; font-size=2em;\"><a name=\"error_";
        private const String HTML_RED2 = "\"></a>";
        private const String HTML_RED3 = "</span>";
        private const String HTML_BORDER1 = "<div style=\"border-width:thin; border-style:solid; font-family:'arial';\">";
        private const String HTML_BORDER2 = "</div>";

        /// <summary>
        ///     This function tests an array of exercises to see if they contain errors, stores the errors in both a full text list and a simple position indication list, and generates a HTML page of the contents of the full exercises array, with indication of the errors.
        /// </summary>
        /// <param name="exercises">The array of exercises that should be checked.</param>
        /// <param name="errorsList">A String list in which to store errors returned by the scanner.</param>
        /// <param name="simpleErrorList">A list in which each error will be stored as an array of integers, of the format {exerciseId, QuestionId, AnswerId}, with the last two values put to -1 if they're not needed.</param>
        /// <param name="htmlOutputPath">Path and filename of the HTML file to generate.</param>
        /// <returns>A boolean which tells if the exercises list contains fatal errors.</returns>
        public static Boolean checkExercises(Exercise[] exercises, ref List<String> errorsList,ref List<int[]> simpleErrorList, String htmlOutputPath)
        {
            Boolean exercisesAreValid = true;
            int errorNumber = 0;
            String htmlDump;
            if (htmlOutputPath!=null)
                htmlDump = "<html style=\"margin:1px;padding:1px\"><head><style type=\"text/css\"><!-- p {margin: 0px 0px 0px 0px;} --></style></head><body>"
                    + "<table width=\"100%\" border=\"0\" cellpadding=\"3\" cellspacing=\"0\""
                    + " style=\"font-family:'courier new';font-size:10pt\">";
            else htmlDump=null;
            // Loop check through all exercises
            for(int i_ex=0; i_ex<exercises.Length; i_ex++)
            {
                if (i_ex > 0) addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "", "&nbsp;",-1);
                addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "ed_exercise", (i_ex + 1).ToString(),-1);
                
                if (exercises[i_ex].getName().Equals(String.Empty))
                {
                    errorsList.Add(getErrorString(i_ex, ref simpleErrorList) + getExeciseNameError());
                    addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "ed_title", getExeciseNameError(), errorNumber++);
                    //addTextString(ref textDump, getExeciseNameError(), true);
                    exercisesAreValid = false;
                }
                else
                {
                    addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "ed_title", exercises[i_ex].getName(),-1);
                    //addTextString(ref textDump,exercises[i_ex].getName(),false);
                }
                addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "ed_description", exercises[i_ex].getDescription(), -1,true);

                Question[] questions = exercises[i_ex].getQuestionsAsArray();
                // Preliminary checks on questions
                if (questions.Length==0)
                {
                    errorsList.Add(getErrorString(i_ex, ref simpleErrorList) + getExeciseNoQuestionsError());
                    addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "ed_error", getExeciseNoQuestionsError(), errorNumber++);
                    exercisesAreValid = false;
                }
                // Loop check through all questions
                for(int i_ques=0; i_ques<questions.Length; i_ques++)
                {
                    addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "", "&nbsp;", -1);
                    addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_question", (i_ques + 1).ToString(), -1);

                    addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_type", DomainController.Instance().getLanguageString("ed_" + questions[i_ques].getQuestionType().ToString()), -1);
                    
                    /* removed because question without title is allowed now
                    if (questions[i_ques].getQuestionTitle().Equals(String.Empty))
                    {
                        errorsList.Add(getErrorString(i_ex, i_ques) + getQuestionNoTitleError());
                        addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_title", getQuestionNoTitleError(), true);
                    }
                    else
                    {//*/
                    addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_title", questions[i_ques].getQuestionTitle(), -1);
                    // }
                    addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_text", questions[i_ques].getQuestionText(), -1,true);
                    
                    
                    Answer[] answers = questions[i_ques].getAnswersAsArray();
                    // Preliminary checks on answers
                    if (answers.Length == 0)
                    {
                        errorsList.Add(getErrorString(i_ex, i_ques, ref simpleErrorList) + getQuestionNoAnswersError());
                        addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_error", getQuestionNoAnswersError(), errorNumber++);
                        exercisesAreValid = false;
                    }
                    else
                    {
                        if (answers.Length > 1 && questions[i_ques].getQuestionType() == QuestionType.OPEN_QUESTION)
                        {
                            errorsList.Add(getErrorString(i_ex, i_ques, ref simpleErrorList) + getQuestionOpenMultipleAnswersError());
                            addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_error", getQuestionOpenMultipleAnswersError(), -1);
                        }
                        else if (answers.Length == 1 && questions[i_ques].getQuestionType() == QuestionType.MULTIPLE_CHOICE_SINGLE)
                        {
                            errorsList.Add(getErrorString(i_ex, i_ques, ref simpleErrorList) + getQuestionMCOnlyOneAnswerError());
                            addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_error", getQuestionMCOnlyOneAnswerError(), errorNumber++);
                            exercisesAreValid = false;
                        }
                        if (questions[i_ques].getQuestionType() == QuestionType.FILL_IN_THE_GAPS
                         || questions[i_ques].getQuestionType() == QuestionType.FILL_IN_THE_GAPS_DROPDOWN)
                        {
                            if (answers[0].getAnswer() == null || answers[0].getAnswer().Equals(String.Empty))
                            {
                                addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_question", getAnswerNoTextError(), errorNumber++);
                                errorsList.Add(getErrorString(i_ex, i_ques, ref simpleErrorList) + getAnswerNoTextError());
                                exercisesAreValid = false;
                            }
                            else
                            {
                                addHTMLString(QU_HTML_1, QU_HTML_2, QU_HTML_3, ref htmlDump, "ed_question", answers[0].getAnswer(), -1 , true);
                            }
                        }
                    }
                    // Loop check through all answers
                    for (int i_ans = 0; i_ans < answers.Length; i_ans++)
                    {
                        if (!((questions[i_ques].getQuestionType() == QuestionType.FILL_IN_THE_GAPS
                            || questions[i_ques].getQuestionType() == QuestionType.FILL_IN_THE_GAPS_DROPDOWN)
                            && answers[i_ans].getWeight() == int.MinValue))
                        {
                            addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "", "&nbsp;", -1);
                            addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_answer", (i_ans + 1).ToString(), -1);

                            if (questions[i_ques].getQuestionType() != QuestionType.FILL_IN_THE_GAPS
                                && questions[i_ques].getQuestionType() != QuestionType.FILL_IN_THE_GAPS_DROPDOWN)
                            {
                                if ((answers[i_ans].getAnswer() == null || answers[i_ans].getAnswer().Equals(String.Empty))
                                    && questions[i_ques].getQuestionType() != QuestionType.OPEN_QUESTION)
                                {
                                    if (!(questions[i_ques].getQuestionType() == QuestionType.MATCHING
                                      && answers[i_ans].getAnswer().Equals(String.Empty)
                                      && !answers[i_ans].getMatchAnswer().Equals(String.Empty)))
                                    {
                                        errorsList.Add(getErrorString(i_ex, i_ques, i_ans, ref simpleErrorList) + getAnswerNoTextError());
                                        addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_text", getAnswerNoTextError(), errorNumber++);
                                        exercisesAreValid = false;
                                    }
                                }
                                else
                                {
                                    addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_text", answers[i_ans].getAnswer(), -1, true);
                                }
                            }

                            if (questions[i_ques].getQuestionType() == QuestionType.MATCHING)
                            {
                                if (answers[i_ans].getMatchAnswer() == null || answers[i_ans].getMatchAnswer().Equals(String.Empty))
                                {
                                    errorsList.Add(getErrorString(i_ex, i_ques, i_ans, ref simpleErrorList) + getAnswerNoMatchError());
                                    addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_matchanswer", getAnswerNoMatchError(), errorNumber++);
                                    exercisesAreValid = false;
                                }
                                else
                                {
                                    addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_matchanswer", answers[i_ans].getMatchAnswer(), -1, true);
                                }

                            }

                            if (answers[i_ans].getWeight() == int.MinValue)
                            {
                                // A matching question's answer can be unweighted, if it only contains a match answer.
                                // On empty answers for the matching type, the weight error is ignored
                                if (!(questions[i_ques].getQuestionType() == QuestionType.MATCHING
                                      && answers[i_ans].getAnswer().Equals(String.Empty)))
                                {
                                    errorsList.Add(getErrorString(i_ex, i_ques, i_ans, ref simpleErrorList) + getAnswerNoWeightError());
                                    addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_weight", getAnswerNoWeightError(), errorNumber++);
                                    exercisesAreValid = false;
                                }
                            }
                            else
                            {
                                addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_weight", answers[i_ans].getWeightString(), -1);
                            }
                            addHTMLString(AN_HTML_1, AN_HTML_2, AN_HTML_3, ref htmlDump, "ed_feedback",
                                answers[i_ans].getFeedback(), -1,
                                questions[i_ques].getQuestionType() != QuestionType.FILL_IN_THE_GAPS
                                  && questions[i_ques].getQuestionType() != QuestionType.FILL_IN_THE_GAPS_DROPDOWN);
                        }
                    }
                }
            }
            addHTMLString(EX_HTML_1, EX_HTML_2, EX_HTML_3, ref htmlDump, "", "&nbsp;",-1);
            if (htmlDump != null) htmlDump+="</table></body></html>";

            if (htmlOutputPath != null)
            {
                Directory.CreateDirectory(htmlOutputPath.Substring(0,htmlOutputPath.LastIndexOf('\\')));
                FileStream fs = new FileStream(htmlOutputPath, FileMode.Create);
                byte[] writebuffer = Encoding.Default.GetBytes(htmlDump);
                fs.Write(writebuffer, 0, writebuffer.Length);
                fs.Close();
            }
            return exercisesAreValid;
        }

        #region Exercise errors

        private static String getExeciseNameError()
        { 
            return DomainController.Instance().getLanguageString("error_title_not_set");
        }

        private static String getExeciseNoQuestionsError()
        { 
            return DomainController.Instance().getLanguageString("error_no_questions");
        }

        #endregion

        #region Question errors

        private static String getQuestionNoTitleError()
        { 
            return DomainController.Instance().getLanguageString("error_title_not_set");
        }

        private static String getQuestionNoAnswersError()
        { 
            return DomainController.Instance().getLanguageString("error_no_answers");
        }

        private static String getQuestionMCOnlyOneAnswerError()
        { 
            return DomainController.Instance().getLanguageString("error_mc_only_one_answer");
        }

        private static String getQuestionOpenMultipleAnswersError()
        { 
            return DomainController.Instance().getLanguageString("error_open_multiple_answers");
        }

        private static String getQuestionFillNoGapsError()
        {
            return DomainController.Instance().getLanguageString("error_no_gaps");
        }

        #endregion

        #region Answer errors

        private static String getAnswerNoWeightError()
        { 
            return DomainController.Instance().getLanguageString("error_weight_not_set");
        }

        private static String getAnswerNoTextError()
        { 
            return DomainController.Instance().getLanguageString("error_text_not_set");
        }

        private static String getAnswerNoMatchError()
        { 
            return DomainController.Instance().getLanguageString("error_match_not_set");
        }

        #endregion

        /// <summary>
        ///     Returns a formatted "error in exercise 1" type message, and adds the error to the Simple Errors List
        /// </summary>
        /// <param name="exerciseId">Internal exercise ID</param>
        /// <param name="simpleErrorList">List in which to add the position of the error</param>
        /// <returns>The formatted error message.</returns>
        private static String getErrorString(int exerciseId, ref List<int[]> simpleErrorList)
        { 
            simpleErrorList.Add(new int[]{exerciseId, -1, -1});
            return DomainController.Instance().getLanguageString("error_in_exercise_x", new String[]{(exerciseId+1).ToString()});
        }

        /// <summary>
        ///     Returns a formatted "error in exercise 1, question 2" type message, and adds the error to the Simple Errors List
        /// </summary>
        /// <param name="exerciseId">Internal exercise ID</param>
        /// <param name="questionId">Internal question ID</param>
        /// <param name="simpleErrorList">List in which to add the position of the error</param>
        /// <returns>The formatted error message.</returns>
        private static String getErrorString(int exerciseId, int questionId, ref List<int[]> simpleErrorList)
        {
            simpleErrorList.Add(new int[]{exerciseId, questionId, -1});
            return DomainController.Instance().getLanguageString("error_in_exercise_x_question_y", new String[]{(exerciseId+1).ToString(),(questionId+1).ToString()});
        }

        /// <summary>
        ///     Returns a formatted "error in exercise 1, question 2, answer 3" type message, and adds the error to the Simple Errors List
        /// </summary>
        /// <param name="exerciseId">Internal exercise ID</param>
        /// <param name="questionId">Internal question ID</param>
        /// <param name="answerId">Internal answer ID</param>
        /// <param name="simpleErrorList">List in which to add the position of the error</param>
        /// <returns>The formatted error message.</returns>
        private static String getErrorString(int exerciseId, int questionId, int answerId, ref List<int[]> simpleErrorList)
        { 
            simpleErrorList.Add(new int[]{exerciseId, questionId, answerId});
            return DomainController.Instance().getLanguageString("error_in_exercise_x_question_y_answer_z", new String[]{(exerciseId+1).ToString(),(questionId+1).ToString(),(answerId+1).ToString()});
        }

        /// <summary>
        ///     Adds a HTML block to the HTML dump. The dump is created from two parts: the (string ID of a) title, and the contents.
        ///     They are put together as HTML-start + title + HTML-middle + contents + html-end.
        ///     This overload does not add a border around the contents.
        /// </summary>
        /// <param name="htmlStart" >HTML element start</param>
        /// <param name="htmlMiddle">HTML element middle; separates title section and content section</param>
        /// <param name="htmlEnd"   >HTML element end</param>
        /// <param name="htmlDump"  >the String to add the new text to</param>
        /// <param name="titleID"   >String ID of the title name</param>
        /// <param name="contents"  >String to put in the content section</param>
        /// <param name="isError"   >Indicates if the content message should be formatted to show it's an error message.</param>
        private static void addHTMLString(String html1, String html2, String html3, ref String htmlDump, String nameID, String value, int errorNumber)
        {
            addHTMLString(html1, html2, html3, ref htmlDump, nameID, value, errorNumber, false);
        }

        /// <summary>
        ///     Adds a HTML block to the HTML dump. The dump is created from two parts: the (string ID of a) title, and the contents.
        ///     They are put together as HTML-start + title + HTML-middle + contents + html-end.
        /// </summary>
        /// <param name="htmlStart" >HTML element start</param>
        /// <param name="htmlMiddle">HTML element middle; separates title section and content section</param>
        /// <param name="htmlEnd"   >HTML element end</param>
        /// <param name="htmlDump"  >the String to add the new text to</param>
        /// <param name="titleID"   >String ID of the title name</param>
        /// <param name="contents"  >String to put in the content section</param>
        /// <param name="isError"   >Indicates if the content message should be formatted to show it's an error message.</param>
        /// <param name="addBorder" >Indicates if a border should be drawn around the content section.</param>
        private static void addHTMLString(String htmlStart, String htmlMiddle, String htmlEnd, ref String htmlDump, String titleID, String contents, int errorNumber, Boolean addBorder)
        {
            if (htmlDump != null)
            {
                htmlDump += htmlStart
                    + (titleID != null && !titleID.Equals(String.Empty) ? DomainController.Instance().getLanguageString(titleID) + ":" : "&nbsp;")
                    + htmlMiddle
                    + (addBorder && contents != null && !contents.Trim().Equals(String.Empty) ? HTML_BORDER1 : String.Empty)
                    + (errorNumber >= 0 ? HTML_RED1 + errorNumber + HTML_RED2 : String.Empty)
                    + (contents != null && !contents.Equals(String.Empty) ? contents : "<i>" + DomainController.Instance().getLanguageString("empty") + "</i>")
                    + (errorNumber >= 0 ? HTML_RED3 : String.Empty)
                    + (addBorder && contents != null && !contents.Trim().Equals(String.Empty) ? HTML_BORDER2 : String.Empty)
                    + htmlEnd;
            }
        }
    }
}
