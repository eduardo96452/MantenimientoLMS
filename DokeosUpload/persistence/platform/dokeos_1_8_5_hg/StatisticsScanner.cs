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
using lmsda.domain;
using Microsoft.Office.Interop.Excel;
using lmsda.domain.exercise;
using lmsda.domain.score.data;
using lmsda.domain.score;
using lmsda.domain.util;
using lmsda.domain.user;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class converts the downloaded raw data files into internal objects.
    /// </summary>
    class StatisticsScanner
    {
        private Course course;

        public StatisticsScanner(Course course)
        {
            this.course = course;
        }
        
        /// <summary>
        ///     Scans through the raw data and puts them into internal objects.
        /// </summary>
        /// <param name="files">The file names.</param>
        /// <returns>A new "Scores" object.</returns>
        /// <remarks>
        ///     Last updated on 16/08/2010 by Gianni Van Hoecke
        ///      -> one student can have multiple statistics.
        /// </remarks>
        public Scores scan(List<String> files)
        {
            DomainController.Instance().writeToLog("extracting_data", true, false, false);

            Application     app             = new Application();
            _Workbook       workbookUsers   = null;
            _Workbook       workbookScores  = null;
            _Worksheet      worksheetUsers  = null;
            _Worksheet      worksheetScores = null;

            Scores          scores          = new Scores(course);
            Student  studentResults;
            ExerciseResult  exerciseResult;
            QuestionResult  questionResult;

            String[,]       arrayUsers;
            String[,]       arrayScores;

            #region Excel to internal arrays...

            //Users...
            workbookUsers       = app.Workbooks.Open(files.ElementAt(0));
            worksheetUsers      = (_Worksheet)workbookUsers.Worksheets[1];
            Range rangeUsers    = (Range)worksheetUsers.Range["$B$1"].CurrentRegion;
            arrayUsers          = Utility.convertTwoDimensionalObjectArrayToStringArray((Object[,])rangeUsers.get_Value(Type.Missing), 1);

            //Data
            workbookScores      = app.Workbooks.Open(files.ElementAt(1));
            worksheetScores     = (_Worksheet)workbookScores.Worksheets[1];

            //Sort..
            this.sort(app, worksheetScores, "A2", "E2", "E");

            //Remove duplicates...
            //1 = user_id
            //3 = exercise_id
            //9 = question_id
            //worksheetScores.Range["$A$1"].CurrentRegion.RemoveDuplicates(new object[]{1, 3, 9}, XlYesNoGuess.xlYes); //Removed as of 1.08
            Range rangeScores   = (Range)worksheetScores.Range["$A$2"].CurrentRegion;
            arrayScores         = Utility.convertTwoDimensionalObjectArrayToStringArray((Object[,])rangeScores.get_Value(Type.Missing), 1);

            #endregion

            #region Clear memory...

            workbookScores.Save(); //Later nog te gebruiken.
            worksheetUsers  = null;
            worksheetScores = null;
            workbookUsers   = null;
            workbookScores  = null;
            app.Quit();
            app             = null;

            #endregion

            #region Read users...

            String          username;
            String          userFirstName;
            String          userFamilyName;
            String          email;

            for (int i = 1; i < arrayUsers.GetLength(0); i++) //Excel begint op 1.
            {
                username        = arrayUsers[i, 2];
                userFirstName   = arrayUsers[i, 4];
                userFamilyName  = arrayUsers[i, 3];
                email           = arrayUsers[i, 5];

                scores.addStudent(new Student(username, userFirstName, userFamilyName, email));
            }

            #endregion

            #region Read scores...

            int     exerciseID;
            String  exerciseTitle;
            String  dateSolved;
            int     resultExercise;
            int     weightExercise;

            for (int i = 2; i < arrayScores.GetLength(0); i++) //Excel begint op 1. + header negeren = 2!
            {
                username        = arrayScores[i, 1];
                studentResults  = scores.getStudent(username);

                if (studentResults != null)
                {
                    //Ophalen van de gegevens...
                    String tempValue = arrayScores[i, 3];

                    if (!tempValue.Equals(String.Empty))
                    {
                        exerciseID      = Convert.ToInt32(tempValue);
                        exerciseTitle   = arrayScores[i, 4];
                        dateSolved      = arrayScores[i, 5];
                        resultExercise  = Convert.ToInt32(arrayScores[i, 6]);
                        weightExercise  = Convert.ToInt32(arrayScores[i, 7]);

                        //Toevoegen van een nieuw ExerciseResult...
                        //   Enkel als deze nog niet bestaat!!
                        if (studentResults.getExercise(exerciseID) == null)
                            studentResults.addExerciseResult(new ExerciseResult(exerciseID, exerciseTitle, dateSolved, resultExercise, weightExercise));
                        else
                            studentResults.getExercise(exerciseID).addResult(resultExercise, weightExercise, dateSolved);

                        //En nu de vraag toevoegen...
                        //   Als de vraag al is toegevoegd, dan gewoon het antwoord bijvoegen!!
                        exerciseResult = studentResults.getExercise(exerciseID);
                        QuestionType    answerType;
                        int             questionID;
                        String          questionTitle;
                        int             order;
                        int             resultQuestion;
                        int             weightQuestion;

                        /*
                         * BEGIN NULLPOINTER OPVANGEN
                         */
                        String tempString = arrayScores[i, 8];
                        int qt;
                        qt = (tempString.Equals(String.Empty) ? 0 : Convert.ToInt32(tempString));
                        /*
                         * EINDE NULLPOINTER OPVANGEN
                         */
                        answerType = QuestionTypes.getQuestionType(qt);
                        
                        /*
                         * BEGIN NULLPOINTER OPVANGEN
                         */
                        tempString = arrayScores[i, 10];
                        qt = (tempString.Equals(String.Empty) ? 0 : Convert.ToInt32(tempString));
                        /*
                         * EINDE NULLPOINTER OPVANGEN
                         */
                        order = qt;

                        /*
                         * BEGIN NULLPOINTER OPVANGEN
                         */
                        tempString = arrayScores[i, 9];
                        qt = (tempString.Equals(String.Empty) ? -1 : Convert.ToInt32(tempString));
                        /*
                         * EINDE NULLPOINTER OPVANGEN
                         */
                        questionID = qt;
                        questionTitle = arrayScores[i, 11];

                        /*
                         * BEGIN NULLPOINTER OPVANGEN
                         */
                        tempString = arrayScores[i, 13];
                        qt = (tempString.Equals(String.Empty) ? 0 : Convert.ToInt32(tempString));
                        /*
                         * EINDE NULLPOINTER OPVANGEN
                         */
                        resultQuestion  = qt;

                        /*
                         * BEGIN NULLPOINTER OPVANGEN
                         */
                        tempString = arrayScores[i, 14];
                        qt = (tempString.Equals(String.Empty) ? 0 : Convert.ToInt32(tempString));
                        /*
                         * EINDE NULLPOINTER OPVANGEN
                         */
                        weightQuestion  = qt;

                        if(exerciseResult.getQuestion(questionID) == null //als de vraag nog niet is opgeslagen
                                && questionID != -1)                      //EN de huidige vraag MOET een ID hebben!
                            exerciseResult.addQuestionResult(new QuestionResult(answerType, questionID, questionTitle, order, resultQuestion, weightQuestion));

                        //Nu het antwoord toevoegen...
                        questionResult = exerciseResult.getQuestion(questionID);

                        String  answer;
                        Boolean correct = false;

                        answer   = arrayScores[i, 12];
                        if(resultQuestion > 0) //Als er een positieve score is toegekend, dan is het antwoord wellicht juist.
                            correct = true;

                        // /* //Restriction removed as of 1.08 -> students can have multiple answers now!!
                        
                        //Zit het antwoord reeds in de lijst? Zoja, negeren
                        //De student krijgt maar 1 kans op het juiste antwoord.
                        //Het eerste antwoord van de student telt; daarom sorteren we op datum.
                                               
                        Boolean add = true; 
                        
                        for (int z = 0; z < questionResult.answerCount; z++)
                            if (questionResult.getAnswerResultAt(z).answer.Equals(answer))
                                add = false;
                        if(add)
                            questionResult.addAnswer(new AnswerResult(answer, correct, true)); //True als het antwoord is gekozen door de student.
                        else
                            DomainController.Instance().writeToLog("duplicate_answer_ignored", true, true, false);
                        // */

                        //questionResult.addAnswer(new AnswerResult(answer, correct, true)); //True als het antwoord is gekozen door de student.
                    }
                    else
                    {
                        //Niet verder gaan, vraag werd verwijderd...
                        DomainController.Instance().writeToLog("deleted_exercise_ignored", true, true, false);
                    }
                }
                else
                {
                    DomainController.Instance().writeToLog("unknown_user_found", true, true, false);
                }
            }

            #endregion

            return scores;
        }

        #region Private methodes.

        /// <summary>
        ///     Sorts an Excel worksheet.
        /// </summary>
        /// <param name="app">Excel.</param>
        /// <param name="workSheet">The worksheet.</param>
        /// <param name="firstSelect">The first cell to be selected.</param>
        /// <param name="secondSelect">The second cell to be selected.</param>
        /// <param name="column">The sorting column.</param>
        private void sort(Application app, _Worksheet workSheet, String firstSelect, String secondSelect, String column)
        {
            //Sort the table on date, ascending.
            ((Range)workSheet.Range[firstSelect]).Select(); //["A2"]
            app.ActiveWindow.FreezePanes = true;
            ((Range)workSheet.Range[secondSelect]).Select(); //["E2"]
            workSheet.Sort.SortFields.Clear();
            workSheet.Sort.SortFields.Add((Range)workSheet.Columns[column],//["E"],
                                                    XlSortOn.xlSortOnValues,
                                                    XlSortOrder.xlAscending,
                                                    Type.Missing,
                                                    XlSortDataOption.xlSortNormal);
            workSheet.Sort.SetRange(workSheet.Range[firstSelect].CurrentRegion);
            workSheet.Sort.Header = XlYesNoGuess.xlYes;
            workSheet.Sort.MatchCase = false;
            workSheet.Sort.SortMethod = XlSortMethod.xlPinYin;
            workSheet.Sort.Apply();
        }

        #endregion

    }
}
