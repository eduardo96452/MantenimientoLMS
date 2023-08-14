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
using System.IO;
using lmsda.domain;
using lmsda.domain.util.xml;
using lmsda.domain.exercise;
using Microsoft.Office.Interop.Word;
using System.Xml;
using lmsda.domain.util;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace lmsda.persistence.document.microsoftoffice
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     This class represents a MS Word 2003/2007 document.
    /// </summary>
    class MicrosoftWordDocument : SupportedExercisesDocument
    {
        private _Document wordDocument; //The word object.
        private String documentFileName;
        private String lastmodifiedDate;
        private int documentScanWarnings;
        private String tempPath;
        private String documentLanguage;

        private List<Exercise> exercises; //The collection of Exercises

        #region Constructor

        /// <summary>
        ///     Opens the document.
        /// </summary>
        /// <param name="document">The full file name of the document.</param>
        /// <param name="plainText">With (false) or without (true) markup.</param>
        public MicrosoftWordDocument(String document)
        {
            this.documentFileName = document;
            this.PlainText = false;
            this.tempPath = DomainController.Instance().getTempPath();
            Directory.CreateDirectory(this.tempPath);
        }

        #endregion

        #region Methods to convert exercises in documents into internal objects

        public override void extractExercises()
        {
            //*
            try
            {//*/
                DomainController.Instance().writeToLog("opening_word_document", true, false);
                if (this.convertToHTML())
                    this.scanDocument();
                else if (this.documentLanguage == null)
                    DomainController.Instance().writeToLog("no_exercise_styles_found", true, false);
                else
                    DomainController.Instance().writeToLog("conversion_to_xhtml_failed", true, false);
            //*
            }
            catch (Exception e)
            {
                DomainController.Instance().writeToLog(e.Message, false, false);
                DomainController.Instance().writeToLog(e.StackTrace, false, true);
            }
            //*/
        }

        /// <summary>
        ///     Converts the document to a filtered HTML file.
        /// </summary>
        /// <returns>True if conversion succeeds.</returns>
        private Boolean convertToHTML()
        {
            _Application app = null;
            try
            {
                if (!(new FileInfo(this.documentFileName).Exists)) return false;
                app = new ApplicationClass(); //Contains the Word application object (Microsoft.Office.Interop.Word.Application)
                try
                {
                    _Document opendoc = getActiveWordDocument(this.documentFileName);
                    if (!opendoc.Saved)
                    {
                        Boolean saveDocument = DomainController.Instance().fireMessageBoxQuestion(DomainController.Instance().getLanguageString("document_is_unsaved"),true);
                        if (saveDocument)
                            opendoc.Save();
                        else return false;
                    }
                }
                catch
                {
                    //Document is not open
                }
                Boolean readOnly = true;
                this.wordDocument = app.Documents.Open(this.documentFileName, Type.Missing, readOnly, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                this.documentLanguage = this.findDocumentLanguage();
                // optimizing: only convert if language styles are found
                if (this.documentLanguage != null)
                {
                    Directory.CreateDirectory(tempPath);
                    DomainController.Instance().writeToLog("converting_to_html", true, true);
                    this.wordDocument.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
                    this.wordDocument.WebOptions.AllowPNG = true;
                    // Most on the following options only affect normal HTML save, since filtered HTML
                    // has its own specific set of save options
                    this.wordDocument.WebOptions.OrganizeInFolder = true;
                    this.wordDocument.WebOptions.RelyOnVML = true;  // MUST be enabled to allow the PNG image switching for the CRC checks
                    this.wordDocument.WebOptions.RelyOnCSS = true;
                    this.wordDocument.WebOptions.UseLongFileNames = true;
                    this.wordDocument.WebOptions.OptimizeForBrowser = true;
                    
                    String htmlname2 = ProgramConstants.HTML_DUMP_NAME.Substring(0, ProgramConstants.HTML_DUMP_NAME.LastIndexOf('.')).Trim('.')
                                 + "2." + ProgramConstants.HTML_DUMP_NAME.Substring(ProgramConstants.HTML_DUMP_NAME.LastIndexOf('.')).Trim('.');

                    // This is saved to prevent irregularities in CRCs caused by saving images at jpg.
                    // This save will dump the original PNGs to check. If the size was modified inside the Word doc,
                    // the img tag width and height should reflect this anyway.
                    this.wordDocument.SaveAs(tempPath + htmlname2, WdSaveFormat.wdFormatHTML, false, "", false, "",
                                    false, false, false, false, false, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing);
                    // the actual document to be scanned for exercises
                    this.wordDocument.SaveAs(tempPath + ProgramConstants.HTML_DUMP_NAME, WdSaveFormat.wdFormatFilteredHTML, false, "", false, "",
                                    false, false, false, false, false, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing);
                }
                this.wordDocument.Close(false, Type.Missing, Type.Missing);
                this.lastmodifiedDate = new FileInfo(this.documentFileName).LastWriteTime.ToString();
                app.Quit(Type.Missing, Type.Missing, Type.Missing);
                app = null;
                return new FileInfo(tempPath + ProgramConstants.HTML_DUMP_NAME).Exists;
            }
            catch (Exception e)
            {
                DomainController.Instance().writeToLog(e.Message, false, false);
                DomainController.Instance().writeToLog(e.StackTrace, false, true);
                app = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to identify the language of the document by counting the number of matching
        ///     style names in each language. If no correct styles are found for any language, the
        ///     documentLanguage variable will be Null and the actual exercises scan will not be done.
        /// </summary>
        private String findDocumentLanguage()
        {
            //List<int> nrofstylesfound = new List<int>();
            List<String> validstyles;
            // get language list
            List<String> languages = new List<string>(DomainController.Instance().getLanguages());
            
            // Legacy support: Adding the styles from the original program as language '*OLD*'.
            // Any '*' in the language name makes the DigexStyles class revert to the old styles,
            // since filenames can't contain a '*'.
            languages.Insert(0,"*OLD*");

            int[] nrofstylesfound = new int[languages.Count];

            DigexStyles digexStyles;

            // speeds up the process by reducing interop calls; the entire list of styles is only fetched once.
            List<String> stylenames = new List<String>();
            foreach (Style style in this.wordDocument.Styles)
            {
                stylenames.Add(style.NameLocal);
            }
            stylenames.Sort();

            for (int langId = 0; langId < languages.Count; langId++)
            {
                digexStyles = new DigexStyles(languages[langId]);

                int amount_found = 0;
                foreach (String stylename in stylenames)
                {
                    // increment if a valid style is found
                    validstyles = digexStyles.getAllValidStyles();
                    if (validstyles.Contains(stylename))
                       amount_found++;
                    // speed up by aborting if all styles are found
                    if (amount_found == validstyles.Count)
                        break;
                }
                // save amount of correct styles found for this language
                nrofstylesfound[langId] = amount_found;
            }

            // this code makes sure that if several languages use exactly the same exercise styles, the function
            // prefers to take A) the selected UI language or B) the default language from those. This is handy if
            // the detected document language would be shown on the UI.

            // Create a list of the languages with the same maximum amount of styles found
            List<int> toplanguages = new List<int>();
            int max = nrofstylesfound.Max();
            if (max == 0)
                return null;
            for(int lan=0; lan< nrofstylesfound.Length; lan++ )
            {
                if (nrofstylesfound[lan] == max)
                    toplanguages.Add(lan);
            }
            if (toplanguages.Count > 1)
            {
                // Amongst the top matches, take the currently set language as first preference
                String currentLanguage = DomainController.Instance().getSettings().getLanguage();
                foreach (int toplang in toplanguages)
                    if (languages[toplang].Equals(currentLanguage))
                        return currentLanguage;
                // ...and the program's default language as second preference
                foreach (int toplang in toplanguages)
                    if (languages[toplang].Equals(ProgramConstants.DEFAULT_LANGUAGE))
                        return ProgramConstants.DEFAULT_LANGUAGE;
            }
            // if code reaches this point, just take the first occurence.
            return languages[nrofstylesfound.ToList().IndexOf(max)];
        }

        private void replaceExercisesImages()
        {
            if (exercises == null || exercises.Count == 0)
                return;
            String imagepath = null;
            String alternateImagesPath = null;
            Regex r = new Regex(@"<img(.*?)>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(this.exercisesToString());
            while (matcher.Success)
            {
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value,"src=\"(.*?)\"");
                String image_link = matcher.Groups[0].Value;
                if (matcher.Success)
                {
                    imagepath = urlmatcher.Groups[1].Value.Replace('/', '\\');
                    imagepath = imagepath.Substring(0, imagepath.LastIndexOf('\\')).Trim('\\');
                }
                if (imagepath == null)
                    matcher = matcher.NextMatch();
                else
                    break;
            }
            if (imagepath == null)
                return; // no images in doc
            
            String htmlname = ProgramConstants.HTML_DUMP_NAME.Substring(0, ProgramConstants.HTML_DUMP_NAME.IndexOf('.')).Trim('.');
            String htmlext = ProgramConstants.HTML_DUMP_NAME.Substring(ProgramConstants.HTML_DUMP_NAME.IndexOf('.')).Trim('.');
            if (imagepath!=null && imagepath.StartsWith(htmlname))
            {
                alternateImagesPath = htmlname + "2" + imagepath.Substring(htmlname.Length);
            }
            foreach (Exercise ex in this.exercises)
            {
                ex.setDescription(replaceImages(ex.getDescription(), alternateImagesPath, "png"));

                foreach (Question q in ex.getQuestionsAsList())
                {
                    q.setQuestionText(replaceImages(q.getQuestionText(), alternateImagesPath, "png"));
                    foreach (Answer a in q.getAnswersAsList())
                    {
                        a.setAnswer(replaceImages(a.getAnswer(), alternateImagesPath, "png"));
                        a.setFeedback(replaceImages(a.getFeedback(), alternateImagesPath, "png"));
                    }
                }
            }
        }

        private String replaceImages(String input, String alternateImagesPath, String alternateImagesExtension)
        {
            if(input == null)
                return null;

            Regex r = new Regex(@"<img(.*?)>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(input);
            while (matcher.Success)
            {
                // <img src="path/image.ext"> --> <img src="alt_path/image.alt_ext">
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value,"src=\"(.*?)\"");
                String image_link = matcher.Groups[0].Value;
                if (matcher.Success)
                {
                    // src="path/image.ext" --> src="alt_path/image.alt_ext"
                    String relImagePath = urlmatcher.Groups[1].Value.Replace('/', '\\');
                    String relImagePath2 = relImagePath;
                    relImagePath2 = alternateImagesPath.Trim('\\') + '\\' + relImagePath2.Substring(relImagePath2.LastIndexOf('\\')).Trim('\\');
                    relImagePath2 = relImagePath2.Substring(0, relImagePath2.LastIndexOf('.')).TrimEnd('.') + '.' + alternateImagesExtension.Trim('.');
                    
                    String imagepath=null;
                    if (Utility.doesFileExist(DomainController.Instance().getTempPath() + relImagePath2))
                        imagepath=relImagePath2;

                    if (imagepath != null)
                    {
                        // src="path/image.ext" --> src="alt_path/image.alt_ext"
                        image_link = image_link.Replace(urlmatcher.Groups[0].Value, "src=\"" + imagepath + "\"");
                        // <img src="path/image.ext"> --> <img src="alt_path/image.alt_ext">
                        input = input.Replace(matcher.Groups[0].Value, image_link);
                    }
                }
                matcher = matcher.NextMatch();
            }
            return input;
        }

        /// <summary>
        ///     Converts the document to a valid XML document and extract the exercises.
        /// </summary>
        private void scanDocument()
        {
            try
            {
                DomainController.Instance().writeToLog("converting_to_xhtml", true, true);
                StreamReader reader = File.OpenText(tempPath + ProgramConstants.HTML_DUMP_NAME);

                String htmlfile = reader.ReadToEnd();
                reader.Close();
                // fix for whitespace between tags disappearing when converting to xml
                htmlfile = Regex.Replace(htmlfile, @">[ \t]+<", ">&nbsp;<");
                
                //Creates a valid XML-document based on the converted HTML document.
                XmlDocument xmlExercise = SGMLReaderHelper.htmlToXmlDocument(htmlfile);
                
                DomainController.Instance().writeToLog("scanning_document", true, false);

                MicrosoftWordDocumentScanner scanner = new MicrosoftWordDocumentScanner(documentLanguage, this.PlainText);
                scanner.doScan(xmlExercise.ChildNodes);

                this.exercises = scanner.getExercises();
                this.documentScanWarnings = scanner.getNumberOfWarnings();
                
                //Doesn't work because you can't remotely enable image cropping
                //this.replaceExercisesImages();

                DomainController.Instance().writeToLog("scanning_completed", true, false);
            }
            catch (Exception e)
            {
                DomainController.Instance().processError(e, false);
            }
        }

        public override void jumpToError(int exerciseNumber, int questionNumber, int answerNumber)
        {
            try
            {
                int exNumber = exerciseNumber;
                int quesNumber = questionNumber;
                int ansNumber = answerNumber;
                String questionPar = String.Empty;
                String lastStyle = String.Empty;
                if (exerciseNumber == -1) return;
                DomainController dc = DomainController.Instance();
                FileInfo docinfo = new FileInfo(this.documentFileName);
                const String CHANGED_ERROR = "document_changed_since_scan";
                if (!docinfo.Exists || !docinfo.LastWriteTime.ToString().Equals(this.lastmodifiedDate))
                {
                    dc.writeToLog(CHANGED_ERROR, true, true, true);
                    return;
                }
                this.wordDocument = null;
                try
                {
                    this.wordDocument = getActiveWordDocument(this.documentFileName);
                    if (!this.wordDocument.Saved)
                    {
                        dc.writeToLog(CHANGED_ERROR, true, true, true);
                        return;
                    }
                }
                catch
                {
                    //Document is not open
                }

                // load style names in the current document's detected language
                DigexStyles styles = new DigexStyles(documentLanguage);

                QuestionType styleType = QuestionType.NOT_SET;
                if (questionNumber >= 0)
                    styleType = this.exercises[exerciseNumber].getQuestionsAsList()[questionNumber].getQuestionType();

                _Application app = null;

                if (this.wordDocument == null)
                {
                    app = new ApplicationClass(); //Contains the Word application object (Microsoft.Office.Interop.Word.Application)
                    this.wordDocument = app.Documents.Open(this.documentFileName, Type.Missing, false, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
                }
                else
                {
                    app = (Microsoft.Office.Interop.Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
                }
                
                // This section will decrease the Exercise, Question ans Answer numbers until they are -1.
                // When all three numbers reach -1, the correct spot is found.

                Paragraph sectionToSelect = null;
                foreach (Paragraph par in this.wordDocument.Paragraphs)
                {
                    Style st = (Style)par.get_Style();
                    //if (((Style)par.get_Style()).NameLocal.Equals(""))
                    if (st.NameLocal.Equals(styles.getDigexExercise()) && exerciseNumber >= 0)
                    {
                        if (exerciseNumber == 0 && questionNumber == -1 && answerNumber == -1)
                        {
                            sectionToSelect = par;
                            break;
                        }
                        exerciseNumber--;
                    }
                    else if (exerciseNumber == -1 && st.NameLocal.StartsWith(styles.getDigexQuestionType()) && questionNumber >= 0)
                    {
                        if (questionNumber == 0 && answerNumber == -1)
                        {
                            sectionToSelect = par;
                            break;
                        }
                        questionNumber--;
                        // question paragraph name is saved to improve accuracy of answer filtering
                        questionPar=st.NameLocal;
                    }
                    else if (exerciseNumber == -1 && questionNumber == -1 && st.NameLocal.Equals(styles.getDigexAnswer()) && answerNumber >= 0)
                    {
                        Boolean readThis = false;

                        if (questionPar.Equals(styles.getDigexQuestionTypeMatch()))
                        {
                            if (paragraphIsInTable(par) && par.Range.Columns.First.Index == 1)
                            {
                                Paragraph parnext = par.Next();
                                if (parnext==null || !paragraphIsInTable(parnext) || parnext.Range.Columns.First.Index != 1
                                    || par.Range.Rows.First.Index != parnext.Range.Rows.First.Index)
                                    readThis = true;
                            }
                        }
                        else
                        {
                            // if the next paragraphs with a valid style is the same style as the current,
                            // look for score to see if next paragraph has to be treated as next answer.

                            // first look for next valid paragraph
                            Paragraph parsearch = par.Next();
                            String parsearchStyle = (parsearch!=null ? ((Style)parsearch.get_Style()).NameLocal : String.Empty);

                            while (parsearch != null &&
                                (!styles.getAllValidStyles().Contains(parsearchStyle)))
                            {
                                parsearch = parsearch.Next();
                                parsearchStyle = (parsearch!=null ? ((Style)parsearch.get_Style()).NameLocal : String.Empty);
                            }
                            // After this, either the search paragraph is null (EOF), or a valid style has been found.

                            // Evaluate next paragraph: only look for score if it is also an Answer,
                            // and if the question is not a Gaps or Matching question, which don't have normal scores.
                            if (parsearch != null && parsearchStyle.Equals(styles.getDigexAnswer())
                                && !questionPar.Equals(styles.getDigexQuestionTypeGaps())
                                && !questionPar.Equals(styles.getDigexQuestionTypeMatch()))
                            {
                                // check for score
                                String text = par.Range.Text.TrimEnd(new Char[] { '\r', '\n'}).Replace('\t', ' ');
                                readThis = scoreFound(text);
                            }
                            else
                                // next paragraph is not an Answer, so
                                readThis = true;
                        }

                        if (readThis)
                        {
                            if (exerciseNumber == -1 && questionNumber == -1 && answerNumber == 0)
                            {
                                sectionToSelect = par;
                                break;
                            }
                            answerNumber--;
                        }
                    }
                    if (styles.getAllValidStyles().Contains(st.NameLocal))
                        lastStyle = st.NameLocal;
                }
                object jumpPosition;
                if (styleType == QuestionType.FILL_IN_THE_GAPS || styleType == QuestionType.FILL_IN_THE_GAPS_DROPDOWN || // always jump to start in gaps exercise
                    ((styleType == QuestionType.MULTIPLE_CHOICE_SINGLE || styleType == QuestionType.MULTIPLE_CHOICE_SEVERAL || styleType == QuestionType.OPEN_QUESTION)
                        && ansNumber != -1 && this.exercises[exNumber].getQuestion(quesNumber).getAnswer(ansNumber).getWeight() != int.MinValue)) // not a "weight not set" error
                {
                    jumpPosition = sectionToSelect.Range.Start;
                }
                else if (styleType == QuestionType.MATCHING && exNumber > -1 && quesNumber > -1 && ansNumber > -1)
                {
                    Answer answer = this.exercises[exNumber].getQuestion(quesNumber).getAnswer(ansNumber);
                    Paragraph nextSection = sectionToSelect.Next();
                    if (nextSection !=null && answer.getMatchAnswer().Equals(String.Empty) && !answer.getAnswer().Equals(String.Empty))
                    {
                        jumpPosition = nextSection.Range.Start;
                    }
                    else
                    {
                        jumpPosition = sectionToSelect.Range.Start;
                    }
                }
                else
                {
                    jumpPosition = sectionToSelect.Range.End - 1;
                }
                Range rng = this.wordDocument.Range(ref jumpPosition, ref jumpPosition);


                rng.Select();
                app.Visible = true;
                app.Activate();

                this.wordDocument = null;
                app = null;
            }
            catch(Exception e)
            {
                DomainController.Instance().processError(e,false);
            }
        }

        private Boolean paragraphIsInTable(Paragraph parsearch)
        {                    
            // test if current paragraph is in a table
            Boolean isTable = false;
            try
            {
                isTable=parsearch.Range.Columns.Count>0;
            }
            catch{}

            return isTable;
        }

        /// <summary>
        ///     Looks if a score is available in an answer.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>True if a score has been found.</returns>
        private Boolean scoreFound(String text)
        {
            String score = Utility.findWeightInString(text, false);
            if (!score.Equals(String.Empty))
            {
                try
                {
                    Convert.ToInt32(score);
                    return true;
                }
                catch { }
            }
            return false;
        }

        public override Boolean supportsJumpToSection()
        {
            return true;
        }

        public override List<String> getExerciseMD5s()
        {
            if (exercises == null || exercises.Count == 0)
                return new List<String>();
            List<String> md5s = new List<String>();
            String imagepath = null;
            String alternateImagesPath = null;
            Regex r = new Regex(@"<img(.*?)>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(this.exercisesToString());
            while (matcher.Success)
            {
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value,"src=\"(.*?)\"");
                String image_link = matcher.Groups[0].Value;
                if (matcher.Success)
                {
                    imagepath = urlmatcher.Groups[1].Value.Replace('/', '\\');
                    imagepath = imagepath.Substring(0, imagepath.LastIndexOf('\\')).Trim('\\');
                }
                if (imagepath == null)
                    matcher = matcher.NextMatch();
                else
                    break;
            }
            String htmlname = ProgramConstants.HTML_DUMP_NAME.Substring(0, ProgramConstants.HTML_DUMP_NAME.IndexOf('.')).Trim('.');
            String htmlext = ProgramConstants.HTML_DUMP_NAME.Substring(ProgramConstants.HTML_DUMP_NAME.IndexOf('.')).Trim('.');
            if (imagepath!=null && imagepath.StartsWith(htmlname))
            {
                alternateImagesPath = htmlname + "2" + imagepath.Substring(htmlname.Length);
            }
            foreach (Exercise ex in this.exercises)
            {
                md5s.Add(ex.getMD5LegacySupport(alternateImagesPath, (alternateImagesPath == null? null : "png")));
            }

            return md5s;
        }

        /// <summary>
        ///     Returns the active word document.
        /// </summary>
        /// <param name="fullPath">The document to compare.</param>
        /// <returns>The active word document.</returns>
        private _Document getActiveWordDocument(String fullPath)
        {
            _Application app;
            Documents documents = null;

            try
            {
                app = (Microsoft.Office.Interop.Word.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Word.Application");
                documents = app.Documents;
            }
            catch { }

            if (documents != null)
            {
                foreach (Document document in documents)
                {
                    if (document.FullName.Equals(fullPath))
                    {
                        app = null;
                        return document;
                    }
                }
            }
            app = null;
            return null;
        }


        #endregion

        #region Conversion to PDF

        public override List<String> convertToPDF(String destinationPath, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error)
        {
            List<String> retValue = new List<String>();
            Directory.CreateDirectory(destinationPath);

            _Application app = null;

            String fileName = Path.GetFileNameWithoutExtension(this.documentFileName);

            try
            {
                app = new ApplicationClass(); //Contains the Word application object (Microsoft.Office.Interop.Word.Application)
                Boolean readOnly = true;
                if (!new FileInfo(this.documentFileName).Exists) 
                    return retValue;
                this.wordDocument = app.Documents.Open(this.documentFileName, Type.Missing, readOnly, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                this.wordDocument.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;
                
                String saveToFileName = Utility.getFileNameWithoutBadCharacters(fileName) + ".pdf";
                
                // for compatibility with the platforms
                if (replaceSpacesByUndescores)
                    saveToFileName = saveToFileName.Replace(" ", "_");

                String pdfPath = destinationPath.Trim('\\') + @"\" + saveToFileName;
                this.wordDocument.ExportAsFixedFormat(pdfPath, 
                                                        WdExportFormat.wdExportFormatPDF, 
                                                        false, 
                                                        WdExportOptimizeFor.wdExportOptimizeForPrint, 
                                                        WdExportRange.wdExportAllDocument, 
                                                        1, 
                                                        1, 
                                                        WdExportItem.wdExportDocumentContent, 
                                                        true, 
                                                        true, 
                                                        WdExportCreateBookmarks.wdExportCreateNoBookmarks, 
                                                        true, 
                                                        true, 
                                                        false, 
                                                        Type.Missing);

                //MessageBox.Show(pdfPath);
                //Convert hyperlinks
                if (convertHyperlinksToJavascript)
                {
                    foreach (Hyperlink h in this.wordDocument.Hyperlinks)
                    {
                        try
                        {
                            if (h.Target.ToLower().Equals("_blank"))
                                PDFTools.convertHyperlinks(pdfPath, new HyperLink(h.Target, h.Address));
                        }
                        catch
                        {
                            //Ignore
                        }
                    }
                }

                if(new FileInfo(pdfPath).Exists)
                    retValue.Add(pdfPath);

                this.wordDocument.Close(false, Type.Missing, Type.Missing);
                app.Quit(Type.Missing, Type.Missing, Type.Missing);
                app = null;
            }
            catch (Exception e)
            {
                error = true;
                DomainController.Instance().processError(e, false);
                app = null;
            }
            
            return retValue;
        }

        


        /// <summary>
        ///     Converts a document to PDF, splitting it at the given styles, and using the given filename template.
        /// </summary>
        /// <param name="destinationPath">Path to save the resulting PDF files in.</param>
        /// <param name="splitAt">Semicolon-separated list of styles to split at.</param>
        /// <param name="namePattern">Name pattern to use for the resulting filenames, using the parameters {nr}, {file_name} and {style_text}.</param>
        /// <param name="splitOnPage">Determines whether to split on the actual position of the style, or to split on the existing pages.</param>
        /// <param name="convertHyperlinksToJavascript">Determines whether to do additional post-processing to fix hyperlinks set to open in a new page so they work in browser embedding.</param>
        /// <returns>A list with the full paths to all of the generated files</returns>
        public override List<String> convertToPDFWithSplit(String destinationPath, String splitAt, String namePattern, Boolean splitOnPage, Boolean replaceSpacesByUndescores, Boolean convertHyperlinksToJavascript, ref Boolean error)
        {
            List<String> retValue = new List<String>();
            Directory.CreateDirectory(destinationPath);

            _Application app = null;
            int PDFCounter = 1;

            if(namePattern == null || namePattern.Equals(String.Empty))
                namePattern = ProgramConstants.PDF_MASK_DEFAULT;

            //Multiple split support.
            String[] splitStrings = splitAt.Split(new char[]{';'}, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < splitStrings.Length; i++)
            {
                splitStrings[i] = splitStrings[i].Trim();
            }

            try
            {
                //Open instance van Word...
                app = new ApplicationClass(); //Contains the Word application object (Microsoft.Office.Interop.Word.Application)
                if (!(new FileInfo(this.documentFileName).Exists)) 
                    return retValue;

                this.wordDocument = app.Documents.Open(this.documentFileName, Type.Missing, true, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing,
                                    Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

                this.wordDocument.WebOptions.Encoding = Microsoft.Office.Core.MsoEncoding.msoEncodingUTF8;

                Boolean stylesFound=false;
                for (int j = 0; j < splitStrings.Length; j++)
                {
                    Style st;
                    try
                    {
                        st = this.wordDocument.Styles[splitStrings[j]];
                        Range searchRange = this.wordDocument.Content;
                        searchRange.Find.ClearFormatting();
                        searchRange.Find.set_Style(st);
                        searchRange.Find.Execute();
                        if (searchRange.Find.Found)
                        {
                            stylesFound = true;
                            break;
                        }
                        else
                        {
                            splitStrings[j] = String.Empty;
                        }
                    }
                    catch
                    {
                        splitStrings[j] = String.Empty;
                    }
                }
                //Initialize the paragraphs...
                Paragraphs paragraphs = this.wordDocument.Paragraphs;
                Paragraph paragraph;
                Range range;
                int startPDFpart;
                int endPDFpart;
                int startPagePDF;
                int endPagePDF;

                String saveToFileName = "";

                //Run through all paragraphs...
                int i = 1;

                while (i <= paragraphs.Count)
                {
                    try
                    {
                        paragraph = this.wordDocument.Paragraphs[i];
                    }
                    catch
                    {
                        break;
                    }

                    //Multiple split support.
                    Style st = (Style)paragraph.get_Style();
                    Boolean styleFound=false;
                    if (st!=null)
                        styleFound=splitStrings.Contains(st.NameLocal);

                    // Only start getting a range if either it's the start of the file,
                    // or the paragraph style is a valid split string.
                    if (i == 1 || styleFound)
                    {
                        // If start of the file is NOT a valid split string, start file counter on 0.
                        if (i == 1 && DomainController.Instance().getSettings().getPDFSplitZeroBeforeFirst() && !styleFound)
                        {
                            PDFCounter = 0;
                        }

                        // prevents double replacement by adding a "bad" character (newline) in the pattern replace string.
                        // Otherwise, if the file name would accidentally contain the string "{style_text}", the next replace
                        // would replace that as if it were a pattern item. Probably paranoia, but it's possible ;)
                        saveToFileName = namePattern
                            .Replace("{nr}", "{nr\n}")
                            .Replace("{file_name}", "{file_name\n}")
                            .Replace("{style_text}", "{style_text\n}");

                        saveToFileName = saveToFileName
                            .Replace("{nr\n}", String.Format("{0:d2}", PDFCounter))
                            .Replace("{file_name\n}", Utility.getFileNameWithoutBadCharacters(Path.GetFileNameWithoutExtension(this.wordDocument.FullName)))
                            .Replace("{style_text\n}", Utility.getFileNameWithoutBadCharacters(paragraph.Range.Text)) + ".pdf";

                        // for compatibility with the platforms
                        if (replaceSpacesByUndescores)
                            saveToFileName = saveToFileName.Replace(" ", "_");

                        DomainController.Instance().writeToLog("converting_chunk_x_pdf", new String[] { saveToFileName }, true, false, false);
                        saveToFileName = destinationPath.Trim('\\') + @"\" + saveToFileName;
                        startPDFpart = paragraph.Range.Start;

                        //Takes a whole page.
                        startPagePDF = Convert.ToInt32(paragraph.Range.Information[WdInformation.wdActiveEndPageNumber]);

                        //Search for the next paragraph that contains a split style.
                        i++;
                        try
                        {
                            paragraph = this.wordDocument.Paragraphs[i];
                        }
                        catch
                        {
                            if (PDFCounter == 0)
                            {
                                DomainController.Instance().writeToLog("document_publishing_error",
                                    new String[] { DomainController.Instance().getLanguageString("publishing_error_empty_file") },
                                    true, false, !DomainController.Instance().isSynchronization);

                                error = true;
                            }
                            break;
                        }

                        if (stylesFound)
                        {
                            while ((i <= paragraphs.Count) && (!splitStrings.Contains(((Style)paragraph.get_Style()).NameLocal)))
                            {
                                i++;
                                try
                                {
                                    paragraph = paragraphs[i];
                                }
                                catch
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            try
                            {
                                paragraph = this.wordDocument.Paragraphs[paragraphs.Count];
                                i=paragraphs.Count+1;
                            }
                            catch
                            {
                                break;
                            }
                        }

                        endPagePDF = Convert.ToInt32(paragraph.Range.Information[WdInformation.wdActiveEndPageNumber]) - 1;

                        //The last paragraph should also be included if we are at the end of the document.
                        if (i >= this.wordDocument.Paragraphs.Count)
                        {
                            endPDFpart = paragraph.Range.End;
                            endPagePDF = endPagePDF + 1;
                        }
                        else
                        {
                            endPDFpart = paragraph.Range.Start;
                        }

                        if (splitOnPage)
                        {
                            // Write PDF
                            this.exportPagesToPDF(startPagePDF, endPagePDF, saveToFileName, ref error);
                        }
                        else
                        {
                            this.exportSelectionToPDF(startPDFpart, endPDFpart, saveToFileName, true, ref error);
                        }
                        //Convert hyperlinks
                        if (convertHyperlinksToJavascript)
                        {
                            if (splitOnPage)
                            {
                                foreach (Hyperlink h in this.wordDocument.Hyperlinks)
                                {
                                    try
                                    {
                                        if (h.Target.ToLower().Equals("_blank"))
                                            PDFTools.convertHyperlinks(saveToFileName, new HyperLink(h.Target, h.Address));
                                    }
                                    catch
                                    {
                                        //Ignore
                                    }
                                }
                            }
                            else
                            {
                                range = this.wordDocument.Range(startPDFpart,endPDFpart);
                                foreach (Hyperlink h in range.Hyperlinks)
                                {
                                    try
                                    {
                                        if (h.Target.ToLower().Equals("_blank"))
                                            PDFTools.convertHyperlinks(saveToFileName, new HyperLink(h.Target, h.Address));
                                    }
                                    catch
                                    {
                                        //Ignore
                                    }
                                }
                            }
                        }

                        if (new FileInfo(saveToFileName).Exists)
                            retValue.Add(saveToFileName);

                        PDFCounter++;
                    }
                    else
                    {
                        i++;
                    }
                } // end of paragraphs loop
            } // end of try
            catch (Exception e)
            {
                error = true;
                DomainController.Instance().processError(e, false);
                app = null;
            }
            finally
            {
                try
                {
                    this.wordDocument.Close(false, Type.Missing, Type.Missing);
                    app.Quit(Type.Missing, Type.Missing, Type.Missing);
                    app = null;
                }
                catch { }
            }

            return retValue;
        }

        /// <summary>
        ///     Creates a PDF file based on a range in a word document.
        /// </summary>
        /// <param name="r">The "range" in a Word-document.</param>
        /// <param name="PDFTeller">Het  file number.</param>
        /// <param name="startofRange">PDF start offset.</param>
        /// <param name="endofRange">PDF end offset.</param>
        /// <param name="pathWithFile">The target location.</param>
        /// <param name="trimEnd"></param>
        /// <param name="error">True if error occurred.</param>
        /// <remarks>
        ///     Last updated on 12/08/2010 by Gianni Van Hoecke
        ///      -> Fixed: Word resets the numbering of certain fields when exporting a <b>selection</b> to PDF.
        /// </remarks>
        private void exportSelectionToPDF(int startofRange, int endofRange, String pathWithFile, Boolean trimEnd, ref Boolean error)
        {
            Range range = this.wordDocument.Range(startofRange, endofRange);
            // remove new line and end of page characters at the end of the selection
            if (trimEnd)
            {
                while (range.End - 1 > range.Start && range.Text != null && (range.Text.EndsWith("\r") || range.Text.EndsWith("\f")))
                    range.End--;
            }

            range.Fields.Update(); //Workaround for bug. (12/08/2010)

            //range.Select();
            //if (startofRange!=0)
            //    System.Threading.Thread.Sleep(2000);
            try
            {
                //range.ExportFragment(pathWithFile,WdSaveFormat.wdFormatPDF);
                //*
                range.ExportAsFixedFormat(pathWithFile,                                             // String OutputFileName,
                                            WdExportFormat.wdExportFormatPDF,                       // WdExportFormat ExportFormat,
                                            false,                                                  // Boolean OpenAfterExport,
                                            WdExportOptimizeFor.wdExportOptimizeForPrint,           // WdExportOptimizeFor OptimizeFor,
                                            false,                                                  // Boolean ExportCurrentPage,
                                            WdExportItem.wdExportDocumentContent,                   // WdExportItem Item,
                                            true,                                                   // Boolean IncludeDocProps,
                                            true,                                                   // Boolean KeepIRM,
                                            WdExportCreateBookmarks.wdExportCreateNoBookmarks,      // WdExportCreateBookmarks CreateBookmarks,
                                            true,                                                   // Boolean DocStructureTags,
                                            true,                                                   // Boolean BitmapMissingFonts,
                                            false,                                                  // Boolean UseISO19005_1,
                                            Type.Missing);                                          // Object& FixedFormatExtClassPtr
                //*/
            }
            catch(Exception e)
            {
                error = true;
                DomainController.Instance().processError(e, false);
            }
        }

        /// <summary>
        ///     Creates a PDF file based on the selected pages in a word document.
        /// </summary>
        /// <param name="PDFTeller">The file number.</param>
        /// <param name="startPagePDF">PDF start page.</param>
        /// <param name="endPagePDF">PDF end page.</param>
        /// <param name="pathWithFile">The target location.</param>
        private void exportPagesToPDF(int startPagePDF, int endPagePDF, String pathWithFile, ref Boolean error)
        {
            try
            {
                if (startPagePDF > endPagePDF)
                    endPagePDF = startPagePDF;

                this.wordDocument.ExportAsFixedFormat(pathWithFile,
                                            WdExportFormat.wdExportFormatPDF,
                                            false,
                                            WdExportOptimizeFor.wdExportOptimizeForPrint,
                                            WdExportRange.wdExportFromTo,
                                            startPagePDF,
                                            endPagePDF,
                                            WdExportItem.wdExportDocumentContent,
                                            true,
                                            true,
                                            WdExportCreateBookmarks.wdExportCreateNoBookmarks,
                                            true,
                                            true,
                                            false,
                                            Type.Missing);
            }
            catch
            {
                error = true;
            }
        }

        #endregion

        #region Getters for converted exercises and other getters

        public override List<Exercise> getExercises()
        {
            return this.exercises;
        }

        public override String getDocumentPathWithFilename()
        {
            return this.documentFileName;
        }

        public override int getDocumentScanWarnings()
        {
            return this.documentScanWarnings;
        }

        #endregion

    }
}