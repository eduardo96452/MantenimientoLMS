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
using lmsda.domain;
using System.IO;
using lmsda.domain.util;

namespace lmsda.persistence.document
{
    /// <summary>
    ///     Author: Gianni Van Hoecke, Maarten Meuris
    ///             Based on the Visual Basic code written by Patrick LAUWAERTS.
    ///     This class is used for PDF purposes.
    /// </summary>
    static class PDFTools
    {
        /// <summary>
        ///     Converts all hyperlinks in word document so the links will be opened in a new window.
        /// </summary>
        /// <param name="pdfFullName">Path to the PDF file.</param>
        /// <param name="links">The list of hyperlinks.</param>
        public static void convertHyperlinks(String pdfFullName, HyperLink link)
        {
            try
            {
                if (link != null && link.getAddress()!=null && !link.getAddress().Equals(String.Empty))
                {
                    String sourcePDFFile = pdfFullName;
                    String targetPDFFile = pdfFullName + ".tmp"; //A temp file.
                    Utility.tryDeleteFile(targetPDFFile);
                    FileStream input = new FileStream(sourcePDFFile, FileMode.Open);
                    FileStream output = new FileStream(targetPDFFile, FileMode.Create);
                    Int64 inputBytePosition;
                    Boolean correctMatch;

                    int byteValue = input.ReadByte();

                    String searchURL = "/S/URI/URI(" + link.getAddress() + ")";
                    Int64 lengthSearchURL = (searchURL).Length;
                    String replaceURL = "/S/JavaScript/JS(app.launchURL\\(\"" + link.getAddress() + "\",true\\) )";
                    Int64 lengthReplaceURL = (replaceURL).Length;

                    while (input.Position < input.Length)
                    {
                        output.WriteByte(Convert.ToByte(byteValue));

                        inputBytePosition = input.Position;
                        byteValue = input.ReadByte();

                        // only try scanning after a match of the first character
                        if (byteValue.Equals(Convert.ToInt32(searchURL[0])) && input.Position + lengthSearchURL <= input.Length)
                        {
                            correctMatch = true;
                            for (int z = 0; z < lengthSearchURL; z++)
                            {
                                // aborts scanning from the moment the last read character no longer
                                // matches the one in the search string
                                if (byteValue != -1 && byteValue.Equals(Convert.ToInt32(searchURL[z])))
                                {
                                    byteValue = input.ReadByte();
                                }
                                else
                                {
                                    correctMatch = false;
                                    break;
                                }
                            }
                            input.Position = inputBytePosition;

                            if (correctMatch)
                            {
                                // manually positions the read offset to the end of the found string
                                input.Position += lengthSearchURL;

                                // writes the entire replaced string to the output
                                for (int z = 0; z < lengthReplaceURL; z++)
                                {
                                    output.WriteByte(Convert.ToByte(replaceURL[z]));
                                }
                            }
                            // read new byte for the loop; if previous loop succeeded this is a new byte,
                            // otherwise it's the byte at the originally checked offset.
                            byteValue = input.ReadByte();
                        }
                    }

                    input.Close();
                    output.Close();

                    Utility.tryDeleteFile(sourcePDFFile);
                    Utility.tryRenameFile(targetPDFFile, sourcePDFFile);
                }
                else
                {
                    DomainController.Instance().writeToLog("no_links_in_document_found", true, true, false);
                }
            }
            catch(Exception e)
            {
                DomainController.Instance().processError(e, false);
            }
        }
    }
}
