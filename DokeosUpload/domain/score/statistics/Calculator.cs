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
using Microsoft.Office.Interop.Excel;
using lmsda.domain.score.data;
using lmsda.domain.util.xml;
using lmsda.domain.util;
using System.Diagnostics;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     Use this class to calculate and save the statistics.
    /// </summary>
    class Calculator
    {
        private Scores  scores;
        private String  date;
        private String  saveToFolder;
        private String  courseDescription;
        private String  courseCode;
        private Boolean calculateMC;
        private Boolean calculateResultsPerStudent;
        private Boolean calculateExerciseStudentDetails;
        private Boolean mcShowQuestionTitles;
        private String  doNotKnowString;
        private Boolean generateAllAttempts;
        private Boolean useGroups;

        private List<String> createdFiles;

        #region Constructor and public methodes.

        /// <summary>
        ///     Instantiates the calculator. Default constructor.
        /// </summary>
        /// <param name="scores">The scores object that contains all data.</param>
        /// <param name="date">The date when this report is calculated.</param>
        /// <param name="courseCode">The course code of the currently selected course or subject.</param>
        /// <param name="courseDescription">The course name.</param>
        /// <remarks>
        ///     Last updated on 17-18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public Calculator(Scores scores, String courseDescription, String courseCode, String date)
            : this(scores, courseDescription, courseCode, date, ProgramConstants.getTempPath(), true, true, true, true, "Weet het niet", false, false)
        {
        }

        /// <summary>
        ///     Instantiates the calculator.
        /// </summary>
        /// <param name="scores">The scores object that contains all data.</param>
        /// <param name="date">The date when this report is calculated.</param>
        /// <param name="courseDescription">The course code of the currently selected course or subject.</param>
        /// <param name="saveToFolder">The output folder.</param>
        /// <param name="calculateMC">True if the percentage of MC questions has to be calculated.</param>
        /// <param name="calculateResultsPerStudent">True if the results per students has to be calculated.</param>
        /// <param name="calculateExerciseStudentDetails">True if the details per exercise per student has to be calculated.</param>
        /// <param name="mcShowQuestionTitles">Show the question titles instead of the default numbering.</param>
        /// <param name="doNotKnowString">The "I don't know" string. Used for creating the charts.</param>        
        /// <param name="generateAllAttempts">True to generate all exercise attempts by the student.</param>
        /// <param name="courseCode">The course code.</param>
        /// <param name="useGroups">Indicates whether or not to use groups.</param>
        /// <remarks>
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        ///      -> Added groups.
        ///     
        ///     updated on 17-18/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public Calculator(Scores scores, String courseDescription, String courseCode, String date, String saveToFolder, Boolean calculateMC, Boolean calculateResultsPerStudent, Boolean calculateExerciseStudentDetails, Boolean mcShowQuestionTitles, String doNotKnowString, Boolean generateAllAttempts, Boolean useGroups)
        {
            this.scores = scores;
            this.date = date;
            this.saveToFolder = saveToFolder.TrimEnd('\\') + @"\";
            this.courseDescription = courseDescription;
            this.courseCode = courseCode;
            this.calculateMC = calculateMC;
            this.calculateResultsPerStudent = calculateResultsPerStudent;
            this.calculateExerciseStudentDetails = calculateExerciseStudentDetails;
            this.mcShowQuestionTitles = mcShowQuestionTitles;
            this.doNotKnowString = doNotKnowString;
            this.generateAllAttempts = generateAllAttempts;
            this.createdFiles = new List<String>();
            this.useGroups = useGroups;
        }

        /// <summary>
        ///     Starts the calculations, based on the user settings.
        /// </summary>
        public void run()
        {
            this.removeAllHTML();

            if(this.calculateMC)
                this.doCalculateMultipleChoicePercentage();

            if(this.calculateResultsPerStudent)
                this.doCalculateResultsPerStudent();

            if(this.calculateExerciseStudentDetails)
                this.doCalculateExerciseStudentDetails();

            if (DomainController.Instance().getSettings().getStatsOpenExcelAfterConversion())
            {
                /* (1)
                ApplicationClass app = new ApplicationClass();
                Workbook workBook;
                */

                foreach (String file in this.createdFiles)
                {
                    Process.Start(file);
                    /* (1)
                    workBook = app.Workbooks.Open(file, 
                                                    Type.Missing, 
                                                    false, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing,  
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing, 
                                                    Type.Missing);
                    */
                }

                /* (1)
                app.Visible = true;
                */

                /* (1)
                 * ===
                 * 
                 * If we open Excel through automation, some settings (like PageSetup) are not loaded.
                 * 
                 */
            }
        }

        #endregion

        #region Private methodes.

        /// <summary>
        ///     Starts the calculation for MultipleChoicePercentageCalculator.
        /// </summary>
        /// <remarks>
        ///     Last updated on 17/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void doCalculateMultipleChoicePercentage()
        {
            MultipleChoicePercentageCalculator calc = new MultipleChoicePercentageCalculator(this.scores, this.courseCode, this.date, this.saveToFolder, this.mcShowQuestionTitles, this.doNotKnowString);
            this.createdFiles.Add(calc.calculate());
        }

        /// <summary>
        ///     Starts the calculation for ResultsPerStudentCalculator.
        /// </summary>
        /// <remarks>
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        ///     
        ///     updated on 17/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void doCalculateResultsPerStudent()
        {
            ResultsPerStudentCalculator calc = new ResultsPerStudentCalculator(this.scores, this.courseCode, this.date, this.saveToFolder, this.courseDescription, this.generateAllAttempts, this.useGroups);
            this.createdFiles.Add(calc.calculate());
        }

        /// <summary>
        ///     Starts the calculation for ExerciseDetailsPerStudentCalculator.
        /// </summary>
        /// <remarks>
        ///     Last updated on 17/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void doCalculateExerciseStudentDetails()
        {
            ExerciseDetailsPerStudentCalculator calc = new ExerciseDetailsPerStudentCalculator(this.scores, this.courseCode, this.date, this.saveToFolder);
            this.createdFiles.Add(calc.calculate());
        }

        /// <summary>
        ///     Removes all HTML tags from all objects.
        /// </summary>
        private void removeAllHTML()
        {
            foreach (Student sr in this.scores.students)
            {
                foreach (ExerciseResult er in sr.exerciseResults)
                {
                    er.exerciseTitle = Utility.stripHTML(er.exerciseTitle);
                    foreach (QuestionResult qr in er.questionResults)
                    {
                        qr.questionTitle = Utility.stripHTML(qr.questionTitle);
                        foreach (AnswerResult ar in qr.answers)
                        {
                            ar.answer = Utility.stripHTML(ar.answer);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
