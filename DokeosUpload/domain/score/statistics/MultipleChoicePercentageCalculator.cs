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
using lmsda.domain.util;
using Microsoft.Office.Interop.Excel;
using System.Drawing;
using lmsda.domain.exercise;
using lmsda.domain.score.data;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used for calculating the MC percentage answers.
    /// </summary>
    class MultipleChoicePercentageCalculator
    {
        private String date;
        private String       FILE_NAME              = "Multiple_Choice_Statistics_{$0}{$1}.xlsx";
        private const String NO_VALUE               = "Er is momenteel geen antwoord.";
        private char[] CORRECT_CHAR                 = new char[]{ '[', ']'};
        private char[] DO_NOT_KNOW_CHAR             = new char[]{ '{', '}'};
        private const int PLACEDONOTKNOW            = 2;
        private const int PLACECORRECT              = 3;
        private const int ROUNDING                  = 4;

        private const int CHART_START_LEFT          = 10;
        private const int CHART_MULTIPLIER_HEIGHT   = 30;
        private const int CHART_WIDTH               = 400;
        private const int CHART_HEIGHT              = 300;
        private const int CHART_PLACE_BETWEEN       = 40;

        private Scores scores;
        private String saveToPath;
        private String courseCode;
        private Boolean writeQuestionTitles;
        private String do_not_know_string;
        private List<ExerciseStats> exerciseStats;
        private List<String[,]> tables;

        #region Constructor and public methods.

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <param name="saveToPath">The save to path.</param>
        /// <param name="writeQuestionTitles">Indicates if the question titles should be exported.</param>
        /// <param name="doNotKnowString">The "Do not know" string.</param>
        /// <param name="courseCode">The course code.</param>
        /// <param name="date">The current date.</param>
        public MultipleChoicePercentageCalculator(Scores scores, String courseCode, String date, String saveToPath, Boolean writeQuestionTitles, String doNotKnowString)
        {
            this.scores = scores;
            this.date = date;
            this.courseCode = courseCode;
            this.saveToPath = saveToPath;
            this.writeQuestionTitles = writeQuestionTitles;
            this.do_not_know_string = doNotKnowString;
            this.exerciseStats = new List<ExerciseStats>();
            this.tables = new List<String[,]>();
            this.FILE_NAME = this.FILE_NAME.Replace("{$1}", this.date).Replace("{$0}", this.courseCode);
        }

        /// <summary>
        ///     Gives the commands to start the calculation.
        /// </summary>
        /// <returns>A string that contains the path to the newly created Excel file.</returns>
        public String calculate()
        {
            DomainController.Instance().writeToLog("analyzing_data", true, false, false);
            this.createNewList();

            //PATRICK Quick fix for the sorting problem.
            this.exerciseStats.Sort(delegate(ExerciseStats ex1, ExerciseStats ex2) { return ex1.exerciseTitle.CompareTo(ex2.exerciseTitle); });            

            DomainController.Instance().writeToLog("calculating_percentage", true, false, false);
            this.calculatePercentage();

            this.exportToExcel(this.tables);

            return this.saveToPath + FILE_NAME;
        }

        #endregion

        #region Calculations.

        /// <summary>
        ///     Creates a new list.
        /// </summary>
        private void createNewList()
        {
            Boolean no_answer_added;

            foreach (Student student in scores.students)
            {
                foreach (ExerciseResult exercise in student.exerciseResults)
                {
                    if(!this.inList(exercise.exerciseID))
                        this.exerciseStats.Add(new ExerciseStats(exercise));

                    int exerciseIndex = this.getIndexOfExercise(exercise.exerciseID);

                    foreach (QuestionResult question in exercise.questionResults)
                    {
                        if (question.answerType == QuestionType.MULTIPLE_CHOICE_SINGLE || question.answerType == QuestionType.MULTIPLE_CHOICE_SEVERAL)
                        {
                            no_answer_added = false;

                            this.exerciseStats.ElementAt(exerciseIndex).addQuestion(question);

                            int questionIndex = this.exerciseStats.ElementAt(exerciseIndex).getIndexOfQuestion(question.questionID);

                            foreach (AnswerResult answer in question.answers)
                            {
                                this.exerciseStats.ElementAt(exerciseIndex).getQuestionStat(questionIndex).addAnswer(answer);
                                if(answer.answer.Equals(NO_VALUE))
                                    no_answer_added = true;
                            }

                            if(!no_answer_added)
                                this.exerciseStats.ElementAt(exerciseIndex).getQuestionStat(questionIndex).addAnswer(new AnswerResult(NO_VALUE, false, false));
                        }
                        else
                        {
                            //Negeren, want we kunnen enkel percentages berekenen van
                            // Multiple Choice vragen.
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Calculates the percentage.
        /// </summary>
        private void calculatePercentage()
        {
            List<AnswerPercentage[]> questionResults;
            List<String[]> questionTitles;
            AnswerPercentage[] answerResults;
            int totalVotes;

            foreach (ExerciseStats exercise in this.exerciseStats)
            {
                questionResults = new List<AnswerPercentage[]>();
                questionTitles = new List<String[]>();
                /*
                 *  Voor elke oefening maken we een een 'tabel' aan.
                 *  Per vraag tonen we alle mogelijke antwoorden.
                 *     We duiden aan welk antwoord juist is --> [antwoord]
                 *     We gaan voor elke antwoord het percentage berekenen --> (aantal stemmen op dit antwoord / totaal aantal stemmen voor de vraag) * 100 
                 */
                for (int i = 0; i < exercise.questionStats.Count; i++)
                {
                    totalVotes = 0;
                    QuestionStats question = exercise.questionStats.ElementAt(i);

                    answerResults = new AnswerPercentage[question.answerStats.Count];
                    //eerst het totaal aantal stemmen berekenen...
                    foreach(AnswerStats answer in question.answerStats)
                        totalVotes += answer.timesChosen;

                    //Nu we het totaal hebben,
                    //Per antwoord berekenen...
                    for(int j = 0; j < question.answerStats.Count; j++)
                    {
                        AnswerStats answer = question.answerStats.ElementAt(j);
                        double divider = Convert.ToDouble(totalVotes);
                        double votes = Convert.ToDouble(answer.timesChosen);
                        double result = (votes / divider);// * 100; //Niet maal 100, we willen de range tussen 0 en 1.
                        answerResults[j] = new AnswerPercentage(answer.answer, result, answer.correct);
                    }
                    questionTitles.Add(new String[]{Convert.ToString(question.questionID), question.questionTitle});
                    questionResults.Add(answerResults);
                }

                int answerCount = this.getLargestWidth(questionResults);
                String[,] oneTable = new String[questionResults.Count + 2, answerCount + 1];

                oneTable[0, 0] = Convert.ToString(exercise.exerciseID);
                oneTable[0, 1] = exercise.exerciseTitle;

                //Altijd een kolom voorzien voor vragen waar geen antwoord voor is aangeduid!
                oneTable[1, 1] = DomainController.Instance().getLanguageString("statistics_no_answer");

                for (int z = 1; z < answerCount; z++) // NIET '<=' omdat we reeds een antwoord hebben toegevoegd.
                    oneTable[1, z + 1] = DomainController.Instance().getLanguageString("answer") + " " + (z);

                for(int z = 0; z < questionResults.Count; z++)
                {
                    AnswerPercentage[] a = questionResults.ElementAt(z);
                    String[] b = questionTitles.ElementAt(z);

                    if(writeQuestionTitles)
                        oneTable[(z + 2), 0] = Convert.ToString(z + 1) + ". " + b[1]; //vraag_title
                    else
                        oneTable[(z + 2), 0] = DomainController.Instance().getLanguageString("question") + " " + Convert.ToString(z + 1); //geen vraag_title, enkel "Vraag 1", "Vraag 2", ...

                    int column = 0;
                    int counter = 2;

                    for (int i = 0; i < a.Length; i++)
                    {
                        column = counter;

                        if (a[i].answer.Equals(NO_VALUE))
                            column = 1; //Een vraag welke niet werd opgelost, komt in de eerste kolom.
                        else
                            counter++; //Anders de eerst volgende kolom.

                        if(a[i].isCorrect) //Aanduiden dat dit antwoord correct is.
                            oneTable[(z + 2), column] = Convert.ToString(CORRECT_CHAR[0]) + Convert.ToString(Math.Round(a[i].percentage, ROUNDING)) + Convert.ToString(CORRECT_CHAR[1]);
                        else if(a[i].answer.Equals(this.do_not_know_string))
                            oneTable[(z + 2), column] = Convert.ToString(DO_NOT_KNOW_CHAR[0]) + Convert.ToString(Math.Round(a[i].percentage, ROUNDING)) + Convert.ToString(DO_NOT_KNOW_CHAR[1]);
                        else //Niet correct, de waarde gewoon wegschrijven.
                            oneTable[(z + 2), column] = Convert.ToString(Math.Round(a[i].percentage, ROUNDING));
                    }
                }

                this.tables.Add(oneTable);
            }
        }

        #endregion

        #region Excel.

        /// <summary>
        ///     Export the data to Excel.
        /// </summary>
        /// <param name="table">The list of tables.</param>
        private void exportToExcel(List<String[,]> table)
        {
            Application app = new ApplicationClass();
            _Workbook workBook = app.Workbooks.Add(1);
            _Worksheet workSheet;

            while(workBook.Worksheets.Count > 1)
                ((_Worksheet)workBook.Worksheets[1]).Delete();

            for (int z = 0; z < table.Count; z++) //Voor elke oefening...
            {
                if(z == 0)
                    workSheet = (_Worksheet)workBook.Sheets[z + 1];
                else
                    workSheet = (_Worksheet)workBook.Sheets.Add(Type.Missing, workBook.Sheets[z]);
                
                String[,] oneTable = table.ElementAt(z);
                int height = oneTable.GetLength(0);
                int width  = oneTable.GetLength(1);

                workSheet.Name = DomainController.Instance().getLanguageString("ed_exercise") + " " + (z + 1);

                for (int i = 1; i < height; i++) //Start with 1 -> don't add the header...
                {
                    for (int j = 0; j < width; j++)
                    {
                        if(i > 1 && j > 0)
                            this.setInterior((Range)workSheet.Cells[i + 1, j + 1], String.Empty);
                        try
                        {
                            workSheet.Cells[i + 1, j + 1] = this.stripChars(oneTable[i, j]); //j+1 = j begins at 0!
                            this.writeCorrectOrNotKnownValue(i, width, oneTable[i, j], workSheet);
                            if(j+1 > 1)
                                ((Range)workSheet.Cells[i + 1, j + 1]).HorizontalAlignment = XlHAlign.xlHAlignRight;
                            if(i > 1 && j > 0)
                                this.setInterior((Range)workSheet.Cells[i + 1, j + 1], oneTable[i, j]);
                        }
                        catch { } //No data.
                        if(j == 1 && i > 1)
                        {
                            Range rangeBorder = (Range)workSheet.Cells[i + 1, j + 1];
                            Border border = (Border)rangeBorder.Borders[XlBordersIndex.xlEdgeRight];
                            border.LineStyle = XlLineStyle.xlContinuous;
                            border.ColorIndex = Constants.xlAutomatic;
                            border.TintAndShade = 0;
                            border.Weight = XlBorderWeight.xlMedium;
                        }
                    }
                }

                /*
                 * Adjust the colors.
                 */
                Range colorRange = (Range)workSheet.Range["$B$3"].CurrentRegion;
                ColorScale cs = (ColorScale)colorRange.FormatConditions.AddColorScale(3);
                
                cs.ColorScaleCriteria[1].Type = XlConditionValueTypes.xlConditionValueNumber;//.xlConditionValueLowestValue;
                cs.ColorScaleCriteria[1].Value = 0;
                cs.ColorScaleCriteria[1].FormatColor.Color = 7039480;
                cs.ColorScaleCriteria[1].FormatColor.TintAndShade = 0;
                cs.ColorScaleCriteria[2].Type = XlConditionValueTypes.xlConditionValueNumber; //xlConditionValuePercentile;
                cs.ColorScaleCriteria[2].Value = 0.5;
                cs.ColorScaleCriteria[2].FormatColor.Color = 8711167;
                cs.ColorScaleCriteria[2].FormatColor.TintAndShade = 0;
                cs.ColorScaleCriteria[3].Type = XlConditionValueTypes.xlConditionValueNumber;
                cs.ColorScaleCriteria[3].Value = 1;
                cs.ColorScaleCriteria[3].FormatColor.Color = 8109667;
                cs.ColorScaleCriteria[3].FormatColor.TintAndShade = 0;

                //Adjust column widths.
                workSheet.Cells.EntireColumn.AutoFit();
  
                //Write info about the exercise...
                workSheet.Cells[1, 1] = DomainController.Instance().getLanguageString("ed_exercise") + ": ";
                workSheet.Cells[1, 2] = oneTable[0, 1] + " (ID: " + oneTable[0, 0] + ")";

                //Charts...
                int widthChart = width + PLACEDONOTKNOW;
                String rangeString1 = ExcelUtility.getCellFromColumnIndex(widthChart, 2, true);
                String rangeString2 = ExcelUtility.getCellFromColumnIndex(widthChart, height - 1, true);
                Range rangeChart = workSheet.Range[rangeString1 + ":" + rangeString2];
                Boolean createCharts = true;
                if(rangeString1.Equals(rangeString2))
                    if(rangeChart.Text.Equals(""))
                        createCharts = false;

                if(height-1 >= 2 && createCharts) //If there are any values...
                {
                    //Chart of "I don't know"...
                    workSheet.Shapes.AddChart(XlChartType.xlColumnClustered, CHART_START_LEFT, height * CHART_MULTIPLIER_HEIGHT, CHART_WIDTH, CHART_HEIGHT).Select();
                    //workBook.ActiveChart.Name = DO_NOT_KNOW;
                    workBook.ActiveChart.ClearToMatchStyle();
                    workBook.ActiveChart.ChartStyle = 28;
                    workBook.ActiveChart.SetSourceData(rangeChart);
                    Series serie = (Series)workBook.ActiveChart.SeriesCollection(1);
                    String xvalues = "='" + ((_Worksheet)workBook.ActiveSheet).Name + "'!" + ExcelUtility.getCellFromColumnIndex(1, 3, true) + ":" + ExcelUtility.getCellFromColumnIndex(1, height, true);
                    serie.XValues = xvalues;
                    ((Axis)workBook.ActiveChart.Axes(XlAxisType.xlValue)).MinimumScale = 0;
                    ((Axis)workBook.ActiveChart.Axes(XlAxisType.xlValue)).MaximumScale = 1;
                    workBook.ActiveChart.HasTitle = true;
                    workBook.ActiveChart.ChartTitle.Text = this.do_not_know_string;
                    workBook.ActiveChart.Legend.Delete();
                }

                widthChart = width + PLACECORRECT;
                rangeString1 = ExcelUtility.getCellFromColumnIndex(widthChart, 2, true);
                rangeString2 = ExcelUtility.getCellFromColumnIndex(widthChart, height - 1, true);
                rangeChart = workSheet.Range[rangeString1 + ":" + rangeString2];
                createCharts = true;
                if(rangeString1.Equals(rangeString2))
                    if(rangeChart.Text.Equals(""))
                        createCharts = false;
                
                if(height-1 >= 2 && createCharts) //If there are any values...
                {
                    //Chart of "Correct"...
                    workSheet.Shapes.AddChart(XlChartType.xlColumnClustered, CHART_START_LEFT + CHART_WIDTH + CHART_PLACE_BETWEEN, height * CHART_MULTIPLIER_HEIGHT, CHART_WIDTH, CHART_HEIGHT).Select();
                    //workBook.ActiveChart.Name = "Correct";
                    workBook.ActiveChart.ClearToMatchStyle();
                    workBook.ActiveChart.ChartStyle = 29;
                    workBook.ActiveChart.SetSourceData(rangeChart);
                    Series serie = (Series)workBook.ActiveChart.SeriesCollection(1);
                    String xvalues = "='" + ((_Worksheet)workBook.ActiveSheet).Name + "'!" + ExcelUtility.getCellFromColumnIndex(1, 3, true) + ":" + ExcelUtility.getCellFromColumnIndex(1, height, true);
                    serie.XValues = xvalues;
                    ((Axis)workBook.ActiveChart.Axes(XlAxisType.xlValue)).MinimumScale = 0;
                    ((Axis)workBook.ActiveChart.Axes(XlAxisType.xlValue)).MaximumScale = 1;
                    workBook.ActiveChart.HasTitle = true;
                    workBook.ActiveChart.ChartTitle.Text = DomainController.Instance().getLanguageString("correct");
                    workBook.ActiveChart.Legend.Delete();
                }

                //Select cell 1,1
                workSheet.Range["A1"].Select();

                //Page = landscape and width fit to one page.
                workSheet.PageSetup.Orientation = XlPageOrientation.xlLandscape;
                workSheet.PageSetup.Zoom = false;
                workSheet.PageSetup.FitToPagesWide = 1;
            }

            //Select first sheet.
            Worksheet sheet = (Worksheet)workBook.Sheets[1];
            sheet.Select(Type.Missing);

            //Select cell 1,1.
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
        /// <param name="value">The value</param>
        private void setInterior(Range range, String value)
        {
            range.Interior.PatternColorIndex = XlColorIndex.xlColorIndexAutomatic;
            range.Interior.TintAndShade = 0;

            if (value.Equals(String.Empty))
            {
                range.Interior.Color = Color.FromArgb(216, 216, 216).ToArgb();
            }
            else
            {
                //If the answer is correct -> cell is bold and italic.
                if (value.StartsWith("["))
                {
                    range.Font.Bold = true;
                    range.Font.Italic = true;
                    range.Font.Size = 12;
                    range.NumberFormat = "\"[\"0,00%\"]\""; //no '.' but ',' !
                }
                else
                {
                    range.NumberFormat = "0,00%"; //no '.' but ',' !
                }
            }
        }

        /// <summary>
        ///     Writes an 'invisible' value to a cell. (Used for the generation of charts.)
        /// </summary>
        /// <param name="height">The row.</param>
        /// <param name="width">The column.</param>
        /// <param name="value">The value.</param>
        /// <param name="workSheet">The work sheet.</param>
        private void writeCorrectOrNotKnownValue(int height, int width, String value, _Worksheet workSheet)
        {
            Boolean write = false;

            if (value.StartsWith("["))
            {
                width += PLACECORRECT;
                write = true;
            }
            else if(value.StartsWith("{"))
            {
                width   += PLACEDONOTKNOW;
                write = true;
            }

            if (write)
            {
                Range range = (Range)workSheet.Cells[height, width];
                workSheet.Cells[height, width] = this.stripChars(value);
                range.Select();
                range.Font.ThemeColor = XlThemeColor.xlThemeColorDark1;
                range.Font.TintAndShade = 0;
                range.NumberFormat = "0,00%";
            }
        }

        #endregion

        #region Private methods.

        /// <summary>
        ///     Checks if an exercise already exists in the list.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>True if the list already contains the exercise.</returns>
        private Boolean inList(int exerciseID)
        {
            Boolean inList = false;

            foreach (ExerciseStats e in this.exerciseStats)
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
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>The index.</returns>
        private int getIndexOfExercise(int exerciseID)
        {
            int index = -1;
            
            for (int i = 0; i < this.exerciseStats.Count; i++)
            {
                if (this.exerciseStats.ElementAt(i).exerciseID == exerciseID)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     Returns the largest width of the questions.
        /// </summary>
        /// <param name="questionResults">The list of questions.</param>
        /// <returns>The largest width.</returns>
        private int getLargestWidth(List<AnswerPercentage[]> questionResults)
        {
            int largest = 1;

            for (int i = 0; i < questionResults.Count; i++)
            {
                AnswerPercentage[] a = questionResults.ElementAt(i);
                if(a.Length > largest)
                    largest = a.Length;
            }

            return largest;
        }

        /// <summary>
        ///     Removes all tags from a value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The filtered value.</returns>
        private String stripChars(String value)
        {
            for (int i = 0; i < CORRECT_CHAR.Length; i++)
            {
                value = value.Trim(CORRECT_CHAR[i]);
                value = value.Trim(DO_NOT_KNOW_CHAR[i]);
            }
            return value;
        }

        #endregion

    }
}
