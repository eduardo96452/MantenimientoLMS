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
using Microsoft.Vbe.Interop;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Text;
using lmsda.domain.util;

namespace lmsda.domain.score.statistics
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used for calculating the results per student.
    /// </summary>
    class ResultsPerStudentCalculator
    {
        private String          date;
        private String          FILE_NAME           = "Results_Per_Student_{$0}{$1}.xlsx";
        private const int       ROUNDING            = 4;
        private const String    EXERCISE_ROW        = "3";
        private const String    STUDENT_NR_COLUMN   = "A";

        private Scores              scores;
        private String              saveToPath;
        private String              courseDescription;
        private String              courseCode;
        private Boolean             generateAllAttempts;
        private Boolean             useGroups;
        //private List<String[,]>     tables;
        private List<List<String[,]>> groupsTables;
        private List<List<ExerciseStats>> exerciseStats;
        private List<String> groupNames;

        #region Constructor and public methods.

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="scores">The scores.</param>
        /// <param name="path">The save to path.</param>
        /// <param name="courseCode">The course code.</param>
        /// <param name="courseDescription">Course description.</param>
        /// <param name="date">The current date.</param>
        /// <param name="generateAllAttempts">True to generate all exercise attempts by the student.</param>
        /// <param name="courseCode">The course code.</param>
        /// <param name="useGroups">Use groups or not.</param>
        public ResultsPerStudentCalculator(Scores scores, String courseCode, String date, String path, String courseDescription, Boolean generateAllAttempts, Boolean useGroups)
        {
            this.scores = scores;
            this.date = date;            
            this.saveToPath = path;
            this.courseDescription = courseDescription;
            this.courseCode = courseCode;
            this.generateAllAttempts = generateAllAttempts;
            this.useGroups = useGroups;
            //this.tables = new List<String[,]>();
            this.groupsTables = new List<List<String[,]>>();
            this.exerciseStats = new List<List<ExerciseStats>>();
            this.groupNames = new List<String>();
            this.FILE_NAME = this.FILE_NAME.Replace("{$1}", this.date).Replace("{$0}", this.courseCode);
        }
        
        /// <summary>
        ///     Gives the commands to start the calculation.
        /// </summary>
        /// <returns>A string that contains the path to the newly created Excel file</returns>
        public String calculate()
        {
            DomainController.Instance().writeToLog("analyzing_data", true, false, false);

            if (this.useGroups)
                this.createNewGroupsList();
            else
                this.groupsTables.Add(this.createNewList(new List<String[,]>(), this.scores.students, DomainController.Instance().getLanguageString("Results")));

            DomainController.Instance().writeToLog("calculating_results", true, false, false);
            this.exportToExcel();

            return this.saveToPath + FILE_NAME;
        }

        #endregion

        #region Calculations.

        /// <summary>
        ///     Creates and calculates a new groups list.
        /// </summary>
        /// <remarks>
        ///     As of 1.09
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        /// </remarks>
        private void createNewGroupsList() 
        {
            foreach(Group group in this.scores.groups.groups) //The groups are already sorted!
            {
                if(group.enabled)
                    this.groupsTables.Add(this.createNewList(new List<String[,]>(), group.students, group.name));
            }
        }

        /// <summary>
        ///     Creates and calculates a new list.
        /// </summary>
        /// <param name="tablesParamter">The table to save in (empty list).</param>
        /// <param name="students">A list of students.</param>
        /// <param name="groupName">The name for the tab.</param>
        /// <remarks>
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        ///      -> This method now returns the list of tables and needs a list of tables and a list of students.
        ///     
        ///     updated on 16-17/08/2010 by Gianni Van Hoecke
        ///      -> students can have multiple answers as of 1.08
        /// </remarks>
        private List<String[,]> createNewList(List<String[,]> tablesParamter, List<Student> students, String groupName)
        {
            int largestWidth = this.getLargestWidth(students);
            List<ExerciseStats> exerciseStat = new List<ExerciseStats>();

            foreach (Student student in students)
            {
                //Declaration.
                //+2 = column for user and mail.
                String[,] table = new String[this.getLargestHeight(student), largestWidth + 2];
                double exerciseTotal = 0.0;
                int row = 0;

                //Column user name.
                table[0, 0] = student.userFamilyName + " " + student.userFirstName;
                table[0, 1] = student.email;

                //All results for all exercises.
                foreach (ExerciseResult exercise in student.exerciseResults)
                {
                    if(!this.inList(exerciseStat, exercise.exerciseID))
                        exerciseStat.Add(new ExerciseStats(exercise));

                    int exerciseIndex = this.getIndexOfExercise(exerciseStat, exercise.exerciseID) + 2; //+2 omdat de eerste kolom de studentennaam is; tweede kolom = e-mailadres.

                    for (int i = 0; i < exercise.resultExercise.Count; i++)
                    {
                        exerciseTotal = (Convert.ToDouble(exercise.resultExercise.ElementAt(i)) / Convert.ToDouble(exercise.weightExercise.ElementAt(i))) * 100;
                        table[i, exerciseIndex] = Convert.ToString(exerciseTotal);
                    }
                    row++;
                }

                //Average calculation are removed as of 1.08.
                //Excel will now handle this.

                //Save table
                tablesParamter.Add(table);
            }

            this.exerciseStats.Add(exerciseStat);
            this.groupNames.Add(groupName);
            return tablesParamter;
        }

        #endregion

        #region Excel.

        /// <summary>
        ///     Exports the data to Excel.
        /// </summary>
        /// <remarks>
        ///     Last updated on 14-15-16/09/2010 by Gianni Van Hoecke
        ///     
        ///     updated on 16-17/08/2010 by Gianni Van Hoecke
        ///      -> students can have multiple answers as of 1.08
        /// </remarks>
        private void exportToExcel()
        {
            #region VBProject testing

            //Check if we are allowed to use the VBProject.
            //=============================================
            Boolean execMacro = true;
            String version = new ApplicationClass().Version;
            if (Convert.ToInt32(Utility.getRegistryKey(@"Software\Microsoft\Office\" + version + @"\Excel\Security", "AccessVBOM")) == 0)
            {
                if (DomainController.Instance().fireMessageBoxQuestion(DomainController.Instance().getLanguageString("can_not_use_macro_set_now"), false))
                {
                    Utility.setRegistryKey(@"Software\Microsoft\Office\" + version + @"\Excel\Security", "AccessVBOM", 1);                  
                }
                else
                {
                    execMacro = false;
                    DomainController.Instance().fireMessageBox(DomainController.Instance().getLanguageString("change_setting_manually"), System.Windows.Forms.MessageBoxIcon.Exclamation);
                }
            }

            #endregion //VBProject testing

            #region Excel initialization

            Microsoft.Office.Interop.Excel.Application app = new ApplicationClass();            
            _Workbook workBook = app.Workbooks.Add(1);
            _Worksheet workSheet;
            //Not really necessary, but just in case...
            app.AutomationSecurity = Microsoft.Office.Core.MsoAutomationSecurity.msoAutomationSecurityLow;

            app.ScreenUpdating = false; //As of 1.09 -> faster.

            while(workBook.Worksheets.Count > 1)
                ((_Worksheet)workBook.Worksheets[1]).Delete();

            workSheet = (_Worksheet)workBook.Sheets[1];

            #endregion //Excel initialization

            for(int groupsCounter = 0; groupsCounter < this.groupsTables.Count; groupsCounter++)
            {
                List<String[,]> tables = this.groupsTables[groupsCounter];
                List<ExerciseStats> exerciseStat = this.exerciseStats[groupsCounter];

                #region Select workSheet

                if (groupsCounter != 0)
                {
                    //Not the first tab.
                    workSheet = (_Worksheet)workBook.Sheets.Add(Type.Missing, workSheet, Type.Missing, Type.Missing);
                }

                workSheet.Name = this.groupNames[groupsCounter];

                workSheet.Select();

                #endregion //Select workSheet

                #region Header

                int row = 1;

                //Write the course first...
                workSheet.Cells[row, 1] = DomainController.Instance().getLanguageString("course") + ": " + this.courseDescription;
                ((Range)workSheet.Cells[row, 1]).Font.Bold = true;

                row++;
                row++; //An empty row.

                //Write the header
                //As of 1.09 dynamic!
                int i;

                List<String> columns = new List<String>();
                int columnSequenceNumber = 1;
                DomainController dc = DomainController.Instance();
                for (int ii = 0; ii < 5; ii++) //Go through all 5 defined columns (settings)
                {
                    if (dc.getSettings().getStatsColumnNumberID() == ii)
                    {
                        if (dc.getSettings().getStatsColumnNumber())
                        {
                            columns.Add(dc.getLanguageString("number"));
                            columnSequenceNumber = ii + 1;
                        }
                    }
                    else if (dc.getSettings().getStatsColumnNameID() == ii)
                    {
                        if (dc.getSettings().getStatsColumnName())
                            columns.Add(dc.getLanguageString("name"));
                    }
                    else if (dc.getSettings().getStatsColumnStudentNumberID() == ii)
                    {
                        if (dc.getSettings().getStatsColumnStudentNumber())
                            columns.Add(dc.getLanguageString("student_number"));
                    }
                    else if (dc.getSettings().getStatsColumnEmailID() == ii)
                    {
                        if (dc.getSettings().getStatsColumnEmail())
                            columns.Add(dc.getLanguageString("email"));
                    }
                    else if (dc.getSettings().getStatsColumnGroupID() == ii)
                    {
                        if (dc.getSettings().getStatsColumnGroup())
                            columns.Add(dc.getLanguageString("group"));
                    }
                }

                for (i = 1; i <= columns.Count; i++)
                {
                    workSheet.Cells[row, i] = columns[i - 1]; //-1 = index starts at 0.
                    ((Range)workSheet.Cells[row, i]).Font.Bold = true;
                }

                //Write attempt header.
                workSheet.Cells[row, i] = dc.getLanguageString("attempt");
                ((Range)workSheet.Cells[row, i]).Font.Bold = true;
                i++; //Next column!

                for (int ii = 0; ii < exerciseStat.Count; ii++)
                {
                    workSheet.Cells[row, i] = exerciseStat.ElementAt(ii).exerciseTitle;
                    i++; //Next column!
                }

                workSheet.Cells[row, i] = dc.getLanguageString("average_exercises");
                ((Range)workSheet.Cells[row, i]).Font.Bold = true;
                this.setLeftBorder((Range)workSheet.Cells[row, i]);
                i++; //Next column!
                workSheet.Cells[row, i] = dc.getLanguageString("average_all_exercises");
                ((Range)workSheet.Cells[row, i]).Font.Bold = true;

                //Set text vertical (header).
                Range rangeTitles = (Range)workSheet.Rows[row + ":" + row];
                rangeTitles.HorizontalAlignment = Constants.xlCenter;
                rangeTitles.VerticalAlignment = Constants.xlBottom;
                rangeTitles.WrapText = false;
                rangeTitles.Orientation = 90;
                rangeTitles.AddIndent = false;
                rangeTitles.IndentLevel = 0;
                rangeTitles.ShrinkToFit = false;
                rangeTitles.ReadingOrder = (int)Constants.xlContext;
                rangeTitles.MergeCells = false;

                int exerciseColumnStart = columns.Count + 2; //+2 because we have a "attempt" field + index start at 1.

                #endregion //Header

                #region Data

                row++; //Next row!            

                //The results...
                for (int j = 0; j < tables.Count; j++)
                {
                    i = 1; //Go back to the first column!

                    String[,] table = tables.ElementAt(j);
                    int length0i = table.GetLength(0);
                    int length1z = table.GetLength(1);

                    //Identify the student we're working with.
                    Student student = scores.getStudentByName(table[0, 0]);

                    if (student == null)
                        continue; //Fatal error; go to next table!

                    for (int ii = 0; ii < columns.Count; ii++)
                    {
                        if (columns[ii].Equals(dc.getLanguageString("number")))
                        {
                            workSheet.Cells[row, i] = (j + 1); //j+1 = We start counting from 1.
                            ((Range)workSheet.Cells[row, i]).NumberFormat = "0\".\"";
                            i++;
                        }
                        else if (columns[ii].Equals(dc.getLanguageString("student_number")))
                        {
                            workSheet.Hyperlinks.Add(workSheet.Cells[row, i], "mailto:" + table[0, 1], Type.Missing, Type.Missing, student.username);
                            i++;
                        }
                        else if (columns[ii].Equals(dc.getLanguageString("name")))
                        {
                            workSheet.Hyperlinks.Add(workSheet.Cells[row, i], "mailto:" + table[0, 1], Type.Missing, Type.Missing, table[0, 0]); //table[0, 0] = the full name.
                            i++;
                        }
                        else if (columns[ii].Equals(dc.getLanguageString("email")))
                        {
                            workSheet.Hyperlinks.Add(workSheet.Cells[row, i], "mailto:" + table[0, 1], Type.Missing, Type.Missing, table[0, 1]); //table[0, 1] = the e-mail address.
                            i++;
                        }
                        else if (columns[ii].Equals(dc.getLanguageString("group")))
                        {
                            workSheet.Cells[row, i] = scores.getGroupsWhereStudentIsSubscribedTo(student);
                            i++;
                        }
                    }

                    for (int ii = 0; ii < table.GetLength(0); ii++) //number of rows in table. (One row is one attempt!)
                    {
                        i = exerciseColumnStart - 1; //-1 = we want also to write the attempt.

                        //attempt (always write attempt #)
                        workSheet.Cells[row, i] = ii + 1; //ii + 1 = We start from 1 instead of 0.
                        i++; //next column!

                        //Number of columns in table. //as in exercises.
                        for (int z = 0; z < table.GetLength(1) - 2; z++)
                        {
                            workSheet.Cells[row, i] = table[ii, z + 2];

                            if (this.valueHasError(table[ii, z + 2])) //Remove text...
                                workSheet.Cells[row, i] = String.Empty;

                            //Deprecated as of 1.08
                            //this.setInterior((Range)workSheet.Cells[row, i], table[i, z + 2]);

                            //Left border...
                            if (z == table.GetLength(1) - 3) //We draw a line between the results and averages.
                                this.setLeftBorder((Range)workSheet.Cells[row, i + 1]);

                            i++; //next column!
                        }

                        row++; //Next row!

                        if (!this.generateAllAttempts) //If the user wants the first attempt only, do not continue.
                            break;
                    }
                }

                #endregion //data

                #region Average calculations

                //Now let Excel calculate the average. (As of 1.08)
                //=================================================
                //FormulaR1C1 won't work on not English versions...
                int exerciseCount=0;
                if (tables.Count > 0)
                {
                    exerciseCount = tables.ElementAt(0).GetLength(1) - 2;
                }

                try
                {
                    if (execMacro) //Don't execute the macro if the user pressed "No".
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("Sub WriteFormula()" + "\n");
                        sb.Append("   Range(Cells(4, (" + exerciseCount + " + " + exerciseColumnStart + ")), Cells(" + (row - 1) + ", (" + exerciseCount + " + " + exerciseColumnStart + "))).FormulaR1C1 = \"=IF(NOT(ISERROR(AVERAGE(RC[-" + exerciseCount + "]:RC[-1]))),AVERAGE(RC[-" + exerciseCount + "]:RC[-1]),\"\"\"\")\"" + "\n");
                        sb.Append("   Range(Cells(4, (" + exerciseCount + " + " + (exerciseColumnStart + 1) + ")), Cells(" + (row - 1) + ", (" + exerciseCount + " + " + (exerciseColumnStart + 1) + "))).FormulaR1C1 = \"=IF(COUNTA(RC[-" + (exerciseCount + 1) + "]:RC[-2])<>0,SUM(RC[-" + (exerciseCount + 1) + "]:RC[-2])/(COUNTBLANK(RC[-" + (exerciseCount + 1) + "]:RC[-2])+COUNTA(RC[-" + (exerciseCount + 1) + "]:RC[-2])),\"\"\"\")\"" + "\n");
                        sb.Append("End Sub");
                        VBComponent module = workBook.VBProject.VBComponents.Add(vbext_ComponentType.vbext_ct_StdModule);
                        module.CodeModule.AddFromString(sb.ToString());
                        workBook.Application.Run("WriteFormula");
                        workBook.VBProject.VBComponents.Remove(module);
                    }
                }
                catch (Exception)
                {
                    String message = DomainController.Instance().getLanguageString("try_macro_failed") + "\n\n" + DomainController.Instance().getLanguageString("change_setting_manually");
                    DomainController.Instance().fireMessageBox(message, System.Windows.Forms.MessageBoxIcon.Exclamation);
                }

                #endregion //Average calculations

                #region Colors and formatting

                //Colors and formatting
                //=====================
                String rangeS = ExcelUtility.getColumnNameFromIndex(exerciseColumnStart) + "4:" + ExcelUtility.getColumnNameFromIndex(exerciseCount + exerciseColumnStart + 1) + (row - 1);
                Range range = workSheet.Range[rangeS];
                range.NumberFormat = "0,00";
                FormatConditions formatConditions = range.FormatConditions;
                formatConditions.Delete();
                //value equals ""
                formatConditions.Add(XlFormatConditionType.xlCellValue,
                                            XlFormatConditionOperator.xlEqual,
                                            "=\"\"");
                ((FormatCondition)formatConditions[1]).Interior.PatternColorIndex = XlPattern.xlPatternAutomatic;
                ((FormatCondition)formatConditions[1]).Interior.ThemeColor = XlThemeColor.xlThemeColorDark1;
                ((FormatCondition)formatConditions[1]).Interior.TintAndShade = -0.14996795556505;
                ((FormatCondition)formatConditions[1]).StopIfTrue = true;
                ((FormatCondition)formatConditions[1]).SetFirstPriority();
                //value < 50
                formatConditions.Add(XlFormatConditionType.xlCellValue,
                                            XlFormatConditionOperator.xlLess,
                                            "=50");
                ((FormatCondition)formatConditions[2]).Font.ThemeColor = XlThemeColor.xlThemeColorDark1;
                ((FormatCondition)formatConditions[2]).Font.TintAndShade = 0;
                ((FormatCondition)formatConditions[2]).Interior.PatternColorIndex = XlPattern.xlPatternAutomatic;
                ((FormatCondition)formatConditions[2]).Interior.Color = 255;
                ((FormatCondition)formatConditions[2]).Interior.TintAndShade = 0;
                ((FormatCondition)formatConditions[2]).StopIfTrue = false;
                //value >= 50 && value <= 100
                formatConditions.Add(XlFormatConditionType.xlCellValue,
                                            XlFormatConditionOperator.xlBetween,
                                            "=50",
                                            "=100");
                ((FormatCondition)formatConditions[3]).Interior.PatternColorIndex = XlPattern.xlPatternAutomatic;
                ((FormatCondition)formatConditions[3]).Interior.Color = 65280;
                ((FormatCondition)formatConditions[3]).Interior.TintAndShade = 0;
                ((FormatCondition)formatConditions[3]).StopIfTrue = false;

                //Adjust column widths
                //====================
                workSheet.Cells.EntireColumn.AutoFit();
                int rowCounter = 0;

                String columnSequenceNumberName = ExcelUtility.getColumnNameFromIndex((columnSequenceNumber > 0) ? columnSequenceNumber : 1);
                do
                {
                    rowCounter++;
                    ((Range)workSheet.Range[columnSequenceNumberName + (row - rowCounter)]).Select();
                }
                while (((Range)workSheet.Range[columnSequenceNumberName + (row - rowCounter)]).Value == null);
               
                ((Range)app.Selection).Columns.AutoFit();
                ((Range)workSheet.Rows[EXERCISE_ROW + ":" + EXERCISE_ROW]).RowHeight = 200;

                //Adjust font
                //===========
                Range r = (Range)workSheet.Range[STUDENT_NR_COLUMN + EXERCISE_ROW].CurrentRegion;
                r.Font.Name = "Trebuchet MS";
                r.Font.Size = 9;

                //Print setup TODO
                //===========
                workSheet.PageSetup.PrintTitleRows = "$" + EXERCISE_ROW + ":$" + EXERCISE_ROW;
                workSheet.PageSetup.PrintTitleColumns = "$A:$" + ExcelUtility.getColumnNameFromIndex(exerciseColumnStart - 1);
                workSheet.PageSetup.CenterFooter = DomainController.Instance().getLanguageString("page") + " &P " + DomainController.Instance().getLanguageString("of") + " &N";

                //Freeze title panes
                //==================
                /* //This doesn't work properly! Only column C is freezed, but NOT row 4!
                ((Range)workSheet.Range["$C$4"]).Select();
                app.ActiveWindow.FreezePanes = true;
                */
                //app.Visible = true;
                ((Range)workSheet.Range["A1"]).Select();                
                Range cell = (Range)workSheet.Cells[4, exerciseColumnStart];
                cell.Activate();
                cell.Select();
                app.ActiveWindow.FreezePanes = true;

                //SORT columns of Excercisenames
                //==============================
                //Patrick: added as a quick solution for the sorting problem in the ResultsPerStudent worksheet
                String rangeSortExercises = ExcelUtility.getColumnNameFromIndex(exerciseColumnStart) + "3:" + ExcelUtility.getColumnNameFromIndex(exerciseCount + exerciseColumnStart - 1) + (row - 1);
                Range rngSortExercises = workSheet.Range[rangeSortExercises];
                //app.ScreenUpdating = true;
                rngSortExercises.Select();
                rngSortExercises.Sort(rngSortExercises.Rows[1, Type.Missing], XlSortOrder.xlAscending,
                    Type.Missing, Type.Missing, XlSortOrder.xlAscending,
                    Type.Missing, XlSortOrder.xlAscending,
                    XlYesNoGuess.xlYes, Type.Missing,
                    Type.Missing, XlSortOrientation.xlSortRows,
                    XlSortMethod.xlPinYin,
                    XlSortDataOption.xlSortNormal, XlSortDataOption.xlSortNormal, XlSortDataOption.xlSortNormal);

                //Select cell 1,1.
                //================
                ((Range)workSheet.Range["A1"]).Select();

                //Delete the attempt column if only the first attempt is shown.        
                //=============================================================
                if (!this.generateAllAttempts)
                {
                    String c = ExcelUtility.getColumnNameFromIndex(exerciseColumnStart - 1); //-1 = we want to remove column attempt.
                    ((Range)workSheet.Columns[c + ":" + c]).Delete(XlDirection.xlToLeft);
                }

                #endregion //Colors and formatting
            }

            #region Save and quit

            ((_Worksheet)workBook.Sheets[1]).Select();

            app.ScreenUpdating = true;

            //Save!
            //=====
            workBook.SaveAs(this.saveToPath + FILE_NAME, 
                            XlFileFormat.xlOpenXMLWorkbook,
                            Type.Missing, 
                            Type.Missing,
                            false, 
                            false, 
                            XlSaveAsAccessMode.xlNoChange,
                            Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            
            //Quit Excel...
            //=============
            workSheet = null;
            workBook = null;
            app.Quit();
            app = null;

            #endregion //Save and quit
        }

        [Obsolete("Deprecated as of 1.08. Conditional formatting is now used.")]
        /// <summary>
        ///     Sets the interior of a range.
        /// </summary>
        /// <param name="range">The range.</param>
        /// <param name="value">The value.</param>
        private void setInterior(Range range, String value)
        {
            range.Interior.PatternColorIndex = XlColorIndex.xlColorIndexAutomatic;
            range.Interior.TintAndShade = 0;

            if (this.valueHasError(value))
            {
                //range.Interior.Color = Color.FromArgb(191, 191, 191).ToArgb();
                range.Interior.Color = Color.FromArgb(216, 216, 216).ToArgb();
                range.NumberFormat = "@";
            }
            else
            {
                try
                {
                    double valueDouble = Convert.ToDouble(value);
                    range.NumberFormat = "0,00"; //Geen '.' maar ',' !
                    if (valueDouble >= 50)
                    {
                        range.Interior.Color = Color.FromArgb(0, 255, 0).ToArgb();
                    }
                    else
                    {
                        range.Interior.Color = Color.FromArgb(0, 0, 255).ToArgb();
                        range.Font.ThemeColor = XlThemeColor.xlThemeColorDark1;
                    }
                    //RGB is apparently BGR...
                }
                catch
                {
                    //Ignore, no value!
                }
            }
        }

        /// <summary>
        ///     Creates a left border on a range.
        /// </summary>
        /// <param name="range">The range.</param>
        private void setLeftBorder(Range range)
        {
            range.Borders[XlBordersIndex.xlEdgeLeft].LineStyle = XlLineStyle.xlContinuous;
            range.Borders[XlBordersIndex.xlEdgeLeft].Weight = XlBorderWeight.xlMedium;
        }

        #endregion

        #region Private methods.

        /// <summary>
        ///     Checks if the value is valid.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>True if the value is valid.</returns>
        private Boolean valueHasError(String value)
        {
            if(value == null || value.Equals(String.Empty))
                return true;

            try
            {
                double x = Convert.ToDouble(value);
            }
            catch
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Checks if an exercise already exists in the list.
        /// </summary>
        /// <param name="exStats">The exercise list.</param>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>True if the list already contains the exercise.</returns>
        private Boolean inList(List<ExerciseStats> exStats, int exerciseID)
        {
            foreach (ExerciseStats e in exStats)
            {
                if (e.exerciseID == exerciseID)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Returns the index of an exercise.
        /// </summary>
        /// <param name="exerciseID">The exercise ID.</param>
        /// <returns>The index.</returns>
        private int getIndexOfExercise(List<ExerciseStats> exerciseStat, int exerciseID)
        {
            int index = -1;
            
            for (int i = 0; i < exerciseStat.Count; i++)
            {
                if (exerciseStat.ElementAt(i).exerciseID == exerciseID)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     Returns the largest width of the exercises.
        /// </summary>
        /// <param name="students">The list of students.</param>
        /// <returns>The largest width.</returns>
        /// <remarks>
        ///     Last updated on 16/09/2010 by Gianni Van Hoecke
        ///      -> Added students list.
        /// </remarks>
        private int getLargestWidth(List<Student> students)
        {
            List<ExerciseStats> exStats = new List<ExerciseStats>();

            foreach (Student student in students)
            {
                foreach(ExerciseResult res in student.exerciseResults)
                {
                    if(!this.inList(exStats, res.exerciseID))
                        exStats.Add(new ExerciseStats(res));
                }
            }

            return exStats.Count;
        }

        /// <summary>
        ///     Gets the largest number of attempts by a student.
        /// </summary>
        /// <param name="student">The student results.</param>
        /// <returns>The largest number of attempts.</returns>
        /// <remarks>
        ///     As of 1.08
        ///     Last updated on 16/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private int getLargestHeight(Student student)
        {
            int heighest = 1;

            foreach (ExerciseResult exercise in student.exerciseResults)
            {
                if (exercise.resultExercise.Count > heighest)
                    heighest = exercise.resultExercise.Count;
            }

            return heighest;
        }

        #endregion
    }
}
