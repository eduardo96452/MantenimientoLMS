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
using Microsoft.Office.Interop.PowerPoint;
using Microsoft.Office.Core;
using lmsda.domain.util;
using lmsda.domain;
using System.Runtime.InteropServices;

namespace lmsda.persistence.document.microsoftoffice
{
    class MicrosoftPowerpointDocument : SupportedPresentationDocument
    {
        
        private String documentFileName;

        /// <summary>
        ///     Opens the document.
        /// </summary>
        /// <param name="document">The full file name of the document.</param>
        public MicrosoftPowerpointDocument(String document)
        {
            this.documentFileName = document;
        }

        public override string getDocumentPathWithFilename()
        {
            return documentFileName;
        }

        public override List<String> convertToPDF(String destinationPath, Boolean frameSlides, Boolean horizontal, PresentationPublishTypes publishMethod, int slidesPerPage, Boolean includeHiddenSlides, ref Boolean error)
        {
            Directory.CreateDirectory(destinationPath);
            FileInfo file = new FileInfo(documentFileName);
            List<String> list = new List<String>();
            Microsoft.Office.Interop.PowerPoint.Application app = null;
            Presentation presentation = null;

            try
            {
                PpPrintOutputType printType;

                if (publishMethod == PresentationPublishTypes.SLIDES)
                    printType = PpPrintOutputType.ppPrintOutputSlides;
                else if (publishMethod == PresentationPublishTypes.NOTES_PAGES)
                    printType = PpPrintOutputType.ppPrintOutputNotesPages;
                else
                {
                    switch (slidesPerPage)
                    {
                        case 1:
                        default:
                            printType = PpPrintOutputType.ppPrintOutputOneSlideHandouts;
                            break;
                        case 2:
                            printType = PpPrintOutputType.ppPrintOutputTwoSlideHandouts;
                            break;
                        case 3:
                            printType = PpPrintOutputType.ppPrintOutputThreeSlideHandouts;
                            break;
                        case 4:
                            printType = PpPrintOutputType.ppPrintOutputFourSlideHandouts;
                            break;
                        case 6:
                            printType = PpPrintOutputType.ppPrintOutputSixSlideHandouts;
                            break;
                        case 9:
                            printType = PpPrintOutputType.ppPrintOutputNineSlideHandouts;
                            break;
                    }
                }

                app = new Microsoft.Office.Interop.PowerPoint.ApplicationClass();

                presentation = app.Presentations.Open(file.FullName,
                                                                    MsoTriState.msoFalse,
                                                                    MsoTriState.msoFalse,
                                                                    MsoTriState.msoFalse);

                // exporting an empty workbook produces no file and gives a "range no longer available"
                // error. This code makes sure the error never appears by aborting in advance if the
                // file is confirmed to be empty, allowing correct catching of more serious errors.
                if (presentation.Slides.Count == 0)
                {
                    DomainController.Instance().writeToLog("presentation_publishing_error", 
                        new String[] { DomainController.Instance().getLanguageString("publishing_error_empty_file") },
                        true, false, false);
                    error = true;
                }
                else
                {
                    String output = destinationPath.TrimEnd('\\') + @"\" + Path.GetFileNameWithoutExtension(file.FullName) + ".pdf";

                    presentation.ExportAsFixedFormat(output,
                                                        PpFixedFormatType.ppFixedFormatTypePDF,
                                                        PpFixedFormatIntent.ppFixedFormatIntentPrint,
                                                        frameSlides ? MsoTriState.msoTrue : MsoTriState.msoTrue,
                                                        horizontal ? PpPrintHandoutOrder.ppPrintHandoutHorizontalFirst : PpPrintHandoutOrder.ppPrintHandoutVerticalFirst,
                                                        printType,
                                                        includeHiddenSlides ? MsoTriState.msoTrue : MsoTriState.msoFalse,
                                                        null,
                                                        PpPrintRangeType.ppPrintAll,
                                                        String.Empty,
                                                        false,
                                                        true,
                                                        true,
                                                        true,
                                                        false,
                                                        Type.Missing);

                    //Maybe later we can add support for PDF-splitting? Therefore we'll implement basic compatibility (the list).
                    list.Add(output);
                }
            }
            catch (Exception e)
            {
                if (DomainController.hasInstance())
                {
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
                    presentation.Close();
                    app.Quit();
                    Marshal.ReleaseComObject(presentation);
                    Marshal.ReleaseComObject(app);
                }
                catch { /* Ignore! */ }

                presentation = null;
                app = null;
            }

            return list;
        }
    }
}
