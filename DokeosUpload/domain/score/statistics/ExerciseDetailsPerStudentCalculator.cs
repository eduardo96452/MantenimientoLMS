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
using Microsoft.Office.Interop.Excel;
using System.Drawing;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used for calculating details per student.
    /// </summary>
    class ExerciseDetailsPerStudentCalculator
    {
        private String date;
        private String          FILE_NAME           = "Exercise_Details_Per_Student_{$0}{$1}.xlsx";
        private const int       ROUNDING            = 4;
        private const String    NO_VALUE            = "Er is momenteel geen antwoord.";
        private const String    EXERCISE_TITLE      = "{{EXERCISE_TITLE}}";
        private const String    QUESTION_TITLE      = "{{QUESTION_TITLE}}";

        private Scores              scores;
        private String              saveToPath;
        private String              courseCode;
        private List<List<ExerciseStats>> exerciseStatsPerStudent;

        #region Constructor and public methods.

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <param name="path">Save to path.</param>
        public ExerciseDetailsPerStudentCalculator(Scores scores, String courseCode, String date, String path)
        {
            this.scores = scores;
            this.courseCode = courseCode;
            this.date = date;
            this.saveToPath = path;
            this.exerciseStatsPerStudent = new List<List<ExerciseStats>>();
            this.FILE_NAME = this.FILE_NAME.Replace("{$1}", this.date).Replace("{$0}", this.courseCode);
        }

        /// <summary>
        ///     Gives the command to begin the calculation.
        /// </summary>
        /// <returns>A string that contains the path to the newly created Excel file.</returns>
        public String calculate()
        {
            DomainController.Instance().writeToLog("analyzing_data", true, false, false);
            this.createNewList();

            DomainController.Instance().writeToLog("calculating_results", true, false, false);
            this.exportToExcel();

            return this.saveToPath + FILE_NAME;
        }

        #endregion

        #region Calculations.

        /// <summary>
        ///     Creates and calculates the data.
        /// </summary>
        private void createNewList()
        {
            Boolean no_answer_added;

            foreach (Student student in scores.students)
            {
                List<ExerciseStats> exerciseStats = new List<ExerciseStats>();

                foreach (ExerciseResult exercise in student.exerciseResults)
                {
                    if(!this.inList(exerciseStats, exercise.exerciseID))
                        exerciseStats.Add(new ExerciseStats(exercise));

                    int exerciseIndex = this.getIndexOfExercise(exerciseStats, exercise.exerciseID);

                    foreach (QuestionResult question in exercise.questionResults)
                    {
                        //if (question.answerType == QuestionType.MULTIPLE_CHOICE_SINGLE || question.answerType == QuestionType.MULTIPLE_CHOICE_SEVERAL)
                        //{
                            no_answer_added = false;

                            exerciseStats.ElementAt(exerciseIndex).addQuestion(question);

                            int questionIndex = exerciseStats.ElementAt(exerciseIndex).getIndexOfQuestion(question.questionID);

                            foreach (AnswerResult answer in question.answers)
                            {
                                exerciseStats.ElementAt(exerciseIndex).getQuestionStat(questionIndex).addAnswer(answer);
                                if(answer.answer.Equals(NO_VALUE))
                                    no_answer_added = true;
                            }

                            if(!no_answer_added)
                                exerciseStats.ElementAt(exerciseIndex).getQuestionStat(questionIndex).addAnswer(new AnswerResult(NO_VALUE, false, false));
                    }
                }
                this.exerciseStatsPerStudent.Add(exerciseStats);
            }
        }

        #endregion

        #region Excel.

        /// <summary>
        ///     Exports the data to Excel.
        /// </summary>
        private void exportToExcel()
        {
            Application app = new ApplicationClass();
            _Workbook workBook = app.Workbooks.Add(1);
            _Worksheet workSheet;

            int row = 1;

            while(workBook.Worksheets.Count > 1)
                ((_Worksheet)workBook.Worksheets[1]).Delete();

            List<String> studentNames = new List<String>();

            for(int i = 0; i < scores.studentsCount; i++)
            {
                row = 1;
                Student student = scores.getStudentAt(i);
                List<ExerciseStats> exerciseStats = this.exerciseStatsPerStudent.ElementAt(i);

                //Add worksheet...
                if(i == 0)
                    workSheet = (_Worksheet)workBook.Sheets[i + 1];
                else
                    workSheet = (_Worksheet)workBook.Sheets.Add(Type.Missing, workBook.Sheets[i]);

                String name0 = student.userFamilyName + " " + student.userFirstName;
                
                // bug fix: excel tab pages can't contain the ' character
                name0 = name0.Replace("\'", "");

                // for duplicate student names
                String name = name0;
                int extracount=1;
                if (name.Length > 31)
                    name = name.Substring(0, 27) + "...";

                while (studentNames.Contains(name))
                {
                    name = name0;
                    if (name.Length + 1 + extracount.ToString().Length > 31)
                        name = name.Substring(0, 27 - 1 - extracount.ToString().Length) + "...";
                    name = name + " " + extracount.ToString();
                }

                studentNames.Add(name);

                workSheet.Name = name;

                //If the student has no exercises, show this...
                if (exerciseStats.Count == 0)
                    workSheet.Cells[row, 1] = DomainController.Instance().getLanguageString("no_data_for_this_student", new String[]{ name });
                else
                    workSheet.Cells[row, 1] = DomainController.Instance().getLanguageString("results_for_student_x", new String[]{ name });

                this.setInterior((Range)workSheet.Cells[row, 1], EXERCISE_TITLE, false, false);

                row++; //After the name of the student, we want an empty row.

                //For each exercise of this student...
                for (int j = 0; j < exerciseStats.Count; j++)
                {
                    row++; //An empty row between the exercises

                    ExerciseStats exercise = exerciseStats.ElementAt(j);

                    //Write the exercise info...
                    workSheet.Cells[row, 1] = exercise.exerciseTitle;
                    this.setInterior((Range)workSheet.Cells[row, 1], EXERCISE_TITLE, false, false);

                    row++;

                    //Write the question info and answer info...
                    for(int k = 0; k < exercise.questionStats.Count; k++)
                    {
                        QuestionStats question = exercise.getQuestionStat(k);

                        workSheet.Cells[row, 1] = Convert.ToString(k + 1) + " " + question.questionTitle;
                        this.setInterior((Range)workSheet.Cells[row, 1], QUESTION_TITLE, false, false);

                        row++;

                        foreach (AnswerStats answer in question.answerStats)
                        { 
                            workSheet.Cells[row, 1] = "'" + answer.answer;
                            this.setInterior((Range)workSheet.Cells[row, 1], answer.answer, answer.correct, (answer.timesChosen > 0) ? true : false);

                            row++;
                        }
                    }
                }
            }

            //Select first work sheet
            Worksheet sheet = (Worksheet)workBook.Sheets[1];
            sheet.Select(Type.Missing);

            //Select cel 1,1.
            ((Range)sheet.Range["A1"]).Select();

            //Save!
            workBook.SaveAs(this.saveToPath + FILE_NAME, 
                            XlFileFormat.xlOpenXMLWorkbook,
                            Type.Missing, 
                            Type.Missing,
                            false, 
                            false, 
                            XlSaveAsAccessMode.xlNoChange,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            //Quit Excel...
            workSheet = null;
            workBook = null;
            app.Quit();
            app = null;
        }

        /// <summary>
        ///     Sets the interior of a range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The range value.</param>
        /// <param name="correct">Indicates whether the value is correct.</param>
        /// <param name="isChosen">Indicates if the value has been chosen by the student.</param>
        private void setInterior(Range range, String value, Boolean correct, Boolean isChosen)
        {
            range.HorizontalAlignment = XlHAlign.xlHAlignLeft;
            range.VerticalAlignment = XlVAlign.xlVAlignBottom;
            range.WrapText = false;
            range.MergeCells = false;

            if (value.Equals(EXERCISE_TITLE))
            {
                range.Font.Bold = true;
                range.Font.Size = 13;
            }
            else if(value.Equals(QUESTION_TITLE))
            {
                range.Font.Bold = true;
                range.Font.Size = 12;
            }
            else
            {
                if(correct)
                    range.Font.Underline = true;

                if (isChosen)
                {
                    if (correct)
                        range.Font.Color = Color.FromArgb(0, 255, 0).ToArgb();
                    else
                        range.Font.Color = Color.FromArgb(0, 0, 255).ToArgb();
                }
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        ///     Checks if an exercise already exists in the list.
        /// </summary>
        /// <param name="exStats">The list with exercises.</param>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>True if the list already contains the exercise.</returns>
        private Boolean inList(List<ExerciseStats> exStats, int exerciseID)
        {
            Boolean inList = false;

            foreach (ExerciseStats e in exStats)
            {
                if (e.exerciseID == exerciseID)
                {
                    inList = true;
                    break;
                }
            }

            return inList;
        }

        /// <summary>
        ///     Returns the index of an exercise.
        /// </summary>
        /// <param name="exStats">The list with exercises.</param>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>The index.</returns>
        private int getIndexOfExercise(List<ExerciseStats> exStats, int exerciseID)
        {
            int index = -1;
            
            for (int i = 0; i < exStats.Count; i++)
            {
                if (exStats.ElementAt(i).exerciseID == exerciseID)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        #endregion
    }
}
