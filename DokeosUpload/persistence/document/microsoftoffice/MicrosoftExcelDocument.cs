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
using System.Text;
using System.IO;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using lmsda.domain.util;
using lmsda.domain;
using System.Runtime.InteropServices;

namespace lmsda.persistence.document.microsoftoffice
{
    class MicrosoftExcelDocument : SupportedSpreadsheetDocument
    {
        private String documentFileName;

        /// <summary>
        ///     Opens the document.
        /// </summary>
        /// <param name="document">The full file name of the document.</param>
        public MicrosoftExcelDocument(String document)
        {
            this.documentFileName = document;
        }

        /// <summary>
        ///     Converts the Excel worksheet into a PDF file.
        ///     <b>Note</b>: no parameters are currently supported. The whole workbook will be converted!
        /// </summary>
        /// <param name="file">The Excel File.</param>
        /// <returns>A list containing the converted files.</returns>
        public override List<String> convertToPDF(String destinationPath, ref Boolean error)
        {
            Directory.CreateDirectory(destinationPath);
            FileInfo file = new FileInfo(documentFileName);
            List<String> list = new List<String>();
            Microsoft.Office.Interop.Excel.Application app = null;
            _Workbook workbook = null;

            try
            {
                app = new Microsoft.Office.Interop.Excel.ApplicationClass();

                app.DisplayAlerts = false;

                workbook = app.Workbooks.Open(file.FullName);

                // exporting an empty workbook produces no file and gives an error code which can't be
                // identified because it seems to be thrown for other interop exceptions too. This code
                // makes sure the error never appears by aborting in advance if the file is confirmed
                // to be empty, allowing correct catching of more serious errors.
                if (IsWorkbookEmpty(workbook))
                {
                    DomainController.Instance().writeToLog("spreadsheet_publishing_error",
                        new String[] { DomainController.Instance().getLanguageString("publishing_error_empty_file") },
                        true, false, false);
                    error = true;
                }
                else
                {
                    String output = file.DirectoryName.Trim('\\') + @"\" + Path.GetFileNameWithoutExtension(file.FullName) + ".pdf";
                
                    //Programmatically ExportAsFixedFormat seems to ignore the print area, 
                    //even if I put the false flag on IgnorePrintAreas parameter. Hereunder the workaround.
                    foreach (_Worksheet sheet in workbook.Worksheets) //Ignore chart pages because they don't have print areas.
                    {
                        //String range = sheet.PageSetup.PrintArea; <- this is null; -> Bug Microsoft!
                        //that's maybe why the IgnorePrintAreas parameter is ignored in the ExportAsFixedFormat method.

                        if (ExcelUtility.sheetContainsRangeName(sheet, "Print_Area"))
                        {
                            String range = ((Range)sheet.Range["Print_Area"]).Address;

                            //If the range is empty, then there is no print area.
                            if (range != null && !range.Equals(String.Empty))
                            {
                                int aboveRowNumber = ((Range)sheet.Range[range].Cells[1, 1]).Row - 1;
                                int leftColumnNumber = ((Range)sheet.Range[range].Cells[1, 1]).Column - 1;
                                int belowRowNumber = aboveRowNumber + 1 + sheet.Range[range].Rows.Count;
                                int rightColumnNumber = leftColumnNumber + 1 + sheet.Range[range].Columns.Count;
                                String indices;

                                //Hide the rows (ABOVE) to ignore.
                                if (aboveRowNumber > 0)
                                {
                                    indices = "1:" + aboveRowNumber;
                                    ((Range)sheet.Rows[indices]).Hidden = true;
                                }

                                //Hide the columns (LEFT) to ignore.
                                if (leftColumnNumber > 0)
                                {
                                    indices = "A:" + ExcelUtility.getColumnNameFromIndex(leftColumnNumber);
                                    ((Range)sheet.Columns[indices]).Hidden = true;
                                }

                                //Hide the rows (BELOW) to ignore.
                                indices = belowRowNumber + ":" + (sheet.Rows.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing).Row + 1);
                                ((Range)sheet.Rows[indices]).Hidden = true;

                                //Hide the columns (RIGHT) to ignore.
                                indices = ExcelUtility.getColumnNameFromIndex(rightColumnNumber) + ":" + ExcelUtility.getColumnNameFromIndex((sheet.Columns.SpecialCells(XlCellType.xlCellTypeLastCell, Type.Missing).Column + 1));
                                ((Range)sheet.Columns[indices]).Hidden = true;
                            }
                        }
                    }

                    workbook.ExportAsFixedFormat(XlFixedFormatType.xlTypePDF,
                                                 output,
                                                 XlFixedFormatQuality.xlQualityStandard,
                                                 true,
                                                 false,
                                                 Type.Missing,
                                                 Type.Missing,
                                                 false,
                                                 Type.Missing);

                    list.Add(output);
                }
            }
            catch (Exception e)
            {
                if (DomainController.hasInstance())
                {
                    // In case of error "HRESULT: 0x800A17B4" the program does not have permission
                    // to communicate with Excel. See http://support.microsoft.com/kb/282830/
                    // note: error 0x800A03EC (probably) means the excel file is completely empty.

                    List<String[]> pairs = new List<String[]>();
                    pairs.Add(new String[] { "File", file.FullName });
                    DomainController.Instance().processError(
                        e, !DomainController.Instance().isSynchronization,
                        "presentation_publishing_error", true, pairs);
                }
                error = true;
            }
            finally
            {
                try
                {
                    //Do not save the workbook.
                    workbook.Close(false);
                    workbook = null;
                    app.Quit();
                    Marshal.ReleaseComObject(workbook);
                    Marshal.ReleaseComObject(app);
                }
                catch { /* Ignore! */ }

                workbook = null;
                app = null;
            }
            return list;
        }


        
        /// <summary>
        ///     Checks if an excel workbook is completely empty.
        /// </summary>
        /// <param name="excelBook">excel workbook to check</param>
        /// <returns>true if the workbook is completely empty</returns>
        /// <remarks>
        ///     code written by Thomas ST at
        ///     http://www.codeproject.com/Forums/1649/Csharp.aspx?fid=1649&df=90&mpp=10&sort=Position&select=3374236&tid=3374236
        /// </remarks>
        private bool IsWorkbookEmpty(_Workbook excelBook)
        {
            try
            {
                if (excelBook.Sheets.Count <= 0)
                {
                    return true;
                }
                else
                {
                    foreach (Worksheet sheet in excelBook.Sheets)
                    {
                        Range excelRange = sheet.UsedRange;
                        int test1 = excelRange.Columns.Count;
                        int test2 = excelRange.Rows.Count;
                        int test3 = excelRange.Count;

                        if (test1 > 1 || test2 > 1 || test3 > 1)
                        {
                            return false;
                        }
                        else //look for content..
                        {
                            foreach(Range cell in excelRange)
                            {
                                if (cell.Value2 != null)
                                {
                                    string cellValue = cell.Value2.ToString();
                                    if (cellValue.Trim().Length > 0)
                                        return false;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public override string getDocumentPathWithFilename()
        {
            return documentFileName;
        }
    }
}
