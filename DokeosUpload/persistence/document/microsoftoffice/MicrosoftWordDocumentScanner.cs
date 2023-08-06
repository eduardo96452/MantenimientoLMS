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
using lmsda.domain.util.xml;
using lmsda.domain;
using System.Xml;
using lmsda.domain.exercise;
using lmsda.domain.util;
using System.Text.RegularExpressions;

namespace lmsda.persistence.document.microsoftoffice
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    class MicrosoftWordDocumentScanner : XmlScanner
    {
        private Boolean plainText = false;

        private Boolean styleFound;
        private String styleName;

        private List<Exercise> exercises;   // the collection of Exercises.
        private Exercise exercise;          // the current Exercise.
        private Question question;          // the current Question.
        private Answer answer;              // the current Answer.
        private Boolean canAddExerciseDescription=false;
        private Boolean canAddQuestion=false;
        private Boolean canAddQuestionDescription=false;
        private Boolean canAddAnswer=false;
        private Boolean canAddAnswerFeedback=false;
        private int numberOfWarnings=0;
        private String digex_prefix;
        private String digex_exercise;
        private String digex_exercise_desc;
        private String digex_question_desc;
        private String digex_question_type;
        private String digex_question_type_mcs;
        private String digex_question_type_mcm;
        private String digex_question_type_gaps;
        private String digex_question_type_gaps_dropdown;
        private String digex_question_type_match;
        private String digex_question_type_open;
        private String digex_answer;
        private String digex_answer_feedback;

        #region Constructor

        public MicrosoftWordDocumentScanner(String documentLanguage)
            :this(documentLanguage, false) { }

        public MicrosoftWordDocumentScanner(String documentLanguage, Boolean plainText)
        {
            this.plainText = plainText;
            DigexStyles digexStyles = new DigexStyles(documentLanguage);
            digex_prefix=               digexStyles.getDigexPrefix().Replace(" ", String.Empty);
            digex_exercise=             digexStyles.getDigexExercise().Replace(" ", String.Empty);
            digex_exercise_desc=        digexStyles.getDigexExerciseDesc().Replace(" ", String.Empty);
            digex_question_type=        digexStyles.getDigexQuestionType().Replace(" ", String.Empty);
            digex_question_type_mcs=    digexStyles.getDigexQuestionTypeMcs().Replace(" ", String.Empty);
            digex_question_type_mcm=    digexStyles.getDigexQuestionTypeMcm().Replace(" ", String.Empty);
            digex_question_type_gaps=   digexStyles.getDigexQuestionTypeGaps().Replace(" ", String.Empty);
            digex_question_type_gaps_dropdown = digexStyles.getDigexQuestionTypeGapsDropdown().Replace(" ", String.Empty);
            digex_question_type_match=  digexStyles.getDigexQuestionTypeMatch().Replace(" ", String.Empty);
            digex_question_type_open=   digexStyles.getDigexQuestionTypeOpen().Replace(" ", String.Empty);
            digex_question_desc=        digexStyles.getDigexQuestionDesc().Replace(" ", String.Empty);
            digex_answer=               digexStyles.getDigexAnswer().Replace(" ", String.Empty);
            digex_answer_feedback=      digexStyles.getDigexAnswerFeedback().Replace(" ", String.Empty);
        }

        #endregion

        #region Overrides

        public override void doScan(XmlNodeList nodes)
        {
            this.exercises = new List<Exercise>();
            base.scanXML(nodes);
            if(this.answer != null && this.question != null)
                this.question.addAnswer(this.answer);
            if(this.question != null && this.exercise != null)
                this.exercise.addQuestion(this.question);
            if(this.exercise != null)
                this.exercises.Add(this.exercise);
        }

        /// <summary>
        ///     This function is called in each iteration of the the main scan loop in 
        ///     XmlScanner's scanXML function.
        /// </summary>
        /// <param name="node">the node to scan</param>
        protected override void examineNode(XmlNode node)
        {
            this.styleFound=false;
            
            // basic paragraph
            if (node.Name.Equals("p"))
            {
                getStylefromNode(node);
            }
            // Special case: table. Checks for the first style found inside the table.
            else if (node.Name.Equals("table"))
            {
                if(node.HasChildNodes)
                    FindStyleInTable(node.ChildNodes);
            }

            if (this.styleFound)
            {
                // The checkNodeContents() function will process everything inside the
                // node. This means the main scan no longer has to look at them.
                ignoreChildren=true;
                // checks if the newly found style marks the end of the currently built
                // exercise/question/answer object, and if so, stores the previous object
                // permanently and creates the new object to fill.
                storeCurrentElement();
                // checks the current node's contents and adds them to the current
                // exercise/question/answer object.
                checkNodeContents(node);
            }
        }

        #endregion

        #region Private methods


        /// <summary>
        ///     Looks if a recognized style can be found in a table.
        /// </summary>
        /// <param name="nodes">The nodes.</param>
        private void FindStyleInTable(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if(this.styleFound) return;
                if (node.Name.Equals("p"))
                {
                    getStylefromNode(node);
                    if (this.styleFound)
                        return;
                }
                if (node.HasChildNodes)
                    this.FindStyleInTable(node.ChildNodes);
            }
        }

        /// <summary>
        ///     Sets the local Stylename string to the value taken from the node, if it is a valid name.
        /// </summary>
        /// <param name="node">Node to get stylename from</param>
        private void getStylefromNode(XmlNode node)
        {
            this.styleName = node.Attributes["class"].Value;
            if (this.styleName != null && this.styleName.StartsWith(this.digex_prefix))
            {
                // filters out First, Middle and Last paragraph substyles which are the result of MS Word's 
                // option to suppress spacing between paragraphs of the same style.
                if (this.styleName.Contains("CxSp"))
                { 
                    this.styleName = this.styleName.Substring(0,this.styleName.IndexOf("CxSp"));
                }
                // Fill empty node with no-break space.
                if (node.InnerXml.Length == 0 || node.InnerXml.TrimStart(new char[]{'\u00A0', ' '}).Length == 0)
                    node.InnerXml = "&nbsp;";
                this.styleFound = true;
            }
        }

        /// <summary>
        ///     Checks if the newly found style marks the end of the currently built
        ///     exercise/question/answer object, and if so, stores the previous object
        ///     permanently and creates the new object to fill.
        /// </summary>
        private void storeCurrentElement()
        {
            if (this.styleName.Equals(this.digex_exercise))
                {
                    if (this.exercise == null)
                    {
                        this.exercise = new Exercise();
                    }
                    else if(!this.exercise.isEmpty())
                    {
                        if (this.answer != null && !this.answer.isEmpty() && this.question != null)
                            this.storeAnswer();
                        if (this.question != null && !this.question.isEmpty())
                            this.storeQuestion();
                        // joining of exercise title paragraphs is no longer allowed
                        //if (this.exercise.getQuestionsAsList().Count > 0)
                        //{
                        this.storeExercise();
                        this.question = null;
                        this.answer = null;
                        this.exercise = new Exercise();
                        //}
                    }
                }
            else if (this.styleName.StartsWith(this.digex_question_type) && canAddQuestion)
            {
                if (this.question == null)
                {
                    this.question = new Question();
                }
                else if(!this.question.isEmpty())
                {
                    if (this.answer != null && !this.answer.isEmpty())
                        this.storeAnswer();
                    
                    // joining of question title paragraphs is no longer allowed
                    //if (this.question.getAnswersAsList().Count > 0)
                    //{
                    this.storeQuestion();
                    this.answer = null;
                    this.question = new Question();
                    //}
                }
            }
            else if (this.styleName.Equals(this.digex_answer) && canAddAnswer)
            {
                if (this.answer == null)
                {
                    this.answer = new Answer();
                }
                else if(this.answer.getWeight() != int.MinValue || (this.answer.getFeedback() !=null && !this.answer.getFeedback().Equals(String.Empty)))
                {
                    this.storeAnswer();
                    this.answer = new Answer();
                }
            }
        }

        /// <summary>
        ///     Stores the current exercise object after replacing its paragraphs by &lt;br /&gt; tags
        /// </summary>
        private void storeExercise()
        {
            if (this.exercise.getDescription() !=null)
                this.exercise.setDescription(Utility.fillEmptyHTMLParagraphs(this.exercise.getDescription()));
            this.exercises.Add(this.exercise);
        }

        /// <summary>
        ///     Stores the current exercise object after replacing its paragraphs by &lt;br /&gt; tags
        /// </summary>
        private void storeQuestion()
        {
            if (this.question.getQuestionText() !=null)
                this.question.setQuestionText(Utility.fillEmptyHTMLParagraphs(this.question.getQuestionText()));
            this.exercise.addQuestion(this.question);
        }

        private void storeAnswer()
        {
            if (this.answer.getAnswer() !=null)
                this.answer.setAnswer(Utility.fillEmptyHTMLParagraphs(this.answer.getAnswer()));
            if (this.answer.getMatchAnswer() !=null)
                this.answer.setMatchAnswer(Utility.fillEmptyHTMLParagraphs(this.answer.getMatchAnswer()));
            if (this.answer.getFeedback() !=null)
                this.answer.setFeedback(Utility.fillEmptyHTMLParagraphs(this.answer.getFeedback()));
            this.question.addAnswer(this.answer);
        }

        /// <summary>
        ///     Converts the content of the node to the proper class.
        /// </summary>
        /// <param name="node">The node.</param>
        private void checkNodeContents(XmlNode node)
        {
            if (this.styleName.Equals(this.digex_exercise))
            {
                if (this.exercise.getName() != null)
                    this.exercise.setName(this.exercise.getName() + " " + currenNodeText(node, true));
                else this.exercise.setName(currenNodeText(node, true));

                canAddExerciseDescription = true;   // there is an exercise to add a description to
                canAddQuestion = true;              // exercise created: can add questions
                canAddQuestionDescription = false;  // no question yet
                canAddAnswer = false;               // no question yet
                canAddAnswerFeedback = false;       // no answer yet
            }
            else if (this.styleName.Equals(this.digex_exercise_desc))
            {
                if (canAddExerciseDescription)
                {
                    if (this.exercise.getDescription() != null)
                        this.exercise.setDescription(this.exercise.getDescription() + getLineChar() + currenNodeText(node, false));
                    else this.exercise.setDescription(currenNodeText(node, false));

                    canAddExerciseDescription = true;   // allow joining together of exercise description paragraphs
                    canAddQuestionDescription = false;  // no question yet
                    canAddAnswer = false;               // no question yet
                    canAddAnswerFeedback = false;       // no answer yet
                }
                else
                {
                    writeIgnoreMessage(node,"ignored_no_exercise");
                }
            }
            else if (this.styleName.StartsWith(this.digex_question_type))
            {
                if (canAddQuestion)
                {
                    if ((this.question.getQuestionTitle() != null)
                        && (this.question.getQuestionType() == GetCurrentQuestionType()))
                        this.question.setQuestionTitle(this.question.getQuestionTitle() + " " + currenNodeText(node, true));
                    else
                    {
                        this.question.setQuestionTitle(currenNodeText(node, true));
                        this.question.setQuestionType(GetCurrentQuestionType());
                    }

                    canAddExerciseDescription = false;  // there is no new exercise
                    canAddQuestionDescription = true;   // next has to be a question description or an answer
                    canAddAnswer = true;                // question exists; answers can be added
                    canAddAnswerFeedback = false;       // no answer yet
                }
                else
                {
                    writeIgnoreMessage(node,"ignored_no_exercise");
                }
            }
            else if (this.styleName.Equals(this.digex_question_desc))
            {
                if (canAddQuestionDescription)
                {
                    if (this.question.getQuestionText() != null)
                        this.question.setQuestionText(this.question.getQuestionText() + getLineChar() + currenNodeText(node, false));
                    else this.question.setQuestionText(currenNodeText(node, false));
                    
                    canAddExerciseDescription = false;  // there is no new exercise
                    canAddQuestionDescription = true;   // allow joining together of question description paragraphs
                    canAddAnswer = true;                // question exists; answers can be added
                    canAddAnswerFeedback = false;       // no answer yet
                }
                else
                {
                    if (!canAddQuestion) writeIgnoreMessage(node,"ignored_no_exercise");
                    else                 writeIgnoreMessage(node,"ignored_no_question");
                }
            }
            else if (this.styleName.Equals(this.digex_answer))
            {
                if (canAddAnswer)
                {
                    setAnswerTextAndWeight(node);

                    canAddExerciseDescription = false;  // there is no new exercise
                    canAddQuestionDescription = false;  // there is no new question
                    canAddAnswer = true;                // allow joining together of answer paragraphs
                    canAddAnswerFeedback = this.question.getQuestionType() != QuestionType.FILL_IN_THE_GAPS
                                        && this.question.getQuestionType() != QuestionType.FILL_IN_THE_GAPS_DROPDOWN
                                        && this.question.getQuestionType() != QuestionType.MATCHING;
                                                // no feedback section allowed for matching: it's inside the [ ]
                }
                else
                {
                    if (!canAddQuestion) writeIgnoreMessage(node,"ignored_no_exercise");
                    else                 writeIgnoreMessage(node,"ignored_no_question");
                }
            }
            else if (this.styleName.Equals(this.digex_answer_feedback))
            {
                if (canAddAnswerFeedback)
                {
                    if (this.answer.getFeedback() != null)
                        this.answer.setFeedback(this.answer.getFeedback() + getLineChar() + currenNodeText(node, false));
                    else
                        this.answer.setFeedback(currenNodeText(node, false));
                    
                    canAddExerciseDescription = false;  // there is no new exercise
                    canAddQuestionDescription = false;  // there is no new question
                    canAddAnswer = true;                // Answer finished; can add new one
                    canAddAnswerFeedback = true;        // allow joining together of answer feedback paragraphs
                }
                else
                {
                    if (!canAddQuestion)    writeIgnoreMessage(node,"ignored_no_exercise");
                    else if (!canAddAnswer) writeIgnoreMessage(node, "ignored_no_question");
                    else if (question!=null && (question.getQuestionType() == QuestionType.FILL_IN_THE_GAPS
                                             || question.getQuestionType() == QuestionType.FILL_IN_THE_GAPS_DROPDOWN
                                             || question.getQuestionType() == QuestionType.MATCHING))
                        writeIgnoreMessage(node, "ignored_not_supported_for_this_question_type");
                    else writeIgnoreMessage(node, "ignored_no_answer");

                }
            }
        }

        private void writeIgnoreMessage(XmlNode node,String reason)
        {
            numberOfWarnings++;
            String contents = currenNodeText(node,true);
            if(contents.Equals(String.Empty))
                contents = currenNodeTextXml(node, true);
            DomainController.Instance().writeToLoggerSession("section_ignored_x_y", new String[] { this.styleName, DomainController.Instance().getLanguageString(reason), contents});

        }

        private void setAnswerTextAndWeight(XmlNode node)
        {
            switch (this.question.getQuestionType())
            {
                case QuestionType.MULTIPLE_CHOICE_SINGLE:
                case QuestionType.MULTIPLE_CHOICE_SEVERAL:
                    setMCAnswerTextAndWeight(node);
                    break;
                case QuestionType.FILL_IN_THE_GAPS:
                case QuestionType.FILL_IN_THE_GAPS_DROPDOWN:
                    setGapsAnswers(node);
                    break;
                case QuestionType.MATCHING:
                    setMatchAnswers(node);
                    break;
                case QuestionType.OPEN_QUESTION:
                    setOpenAnswerWeight(node);
                    break;
            }
        }

        /// <summary>
        ///     Filters the answer of an MC question and split it into answer text and answer weight.
        /// </summary>
        /// <param name="node">The node.</param>
        private void setMCAnswerTextAndWeight(XmlNode node)
        {
            setAnswerTextAndWeight(node,true);
        }

        /// <summary>
        ///     Filters the answer of an MC question and split it into answer text and answer weight.
        /// </summary>
        /// <param name="node">The node.</param>
        private void setOpenAnswerWeight(XmlNode node)
        {
            setAnswerTextAndWeight(node,false);
        }
        
        /// <summary>
        ///     Filters the answer of an MC question and split it into answer text and answer weight.
        /// </summary>
        /// <param name="node">The node.</param>
        private void setAnswerTextAndWeight(XmlNode node, Boolean saveText)
        {
            String rawText = this.currenNodeText(node,false);
            String plainText = this.currenNodeText(node,true);
            String answer_text = "";
            String weight = "";

            // find weight
            if (!node.Name.Equals("table"))  // weight can never be in a table
            {
                weight = Utility.findWeightInString(plainText, false);

                if (!weight.Equals(String.Empty))
                {
                    if (saveText)
                    {
                        int lio = rawText.LastIndexOf(weight);
                        answer_text = rawText.Substring(0, lio).TrimEnd(new char[]{'\u00A0', ' '}) + rawText.Substring(lio + weight.Length);
                    }
                    this.answer.setWeight(Convert.ToInt16(weight));
                }
                else if (saveText)
                {
                    answer_text = rawText;
                }
            }
            else if (saveText)
            {
                answer_text = rawText;
            }
            
            if (saveText)
            {
                answer_text = answer_text.Trim();
                answer_text = answer_text.Replace(new String(new char[]{'\u00A0'}), "&nbsp;");

                if (currenNodeTextXml(node, true).Trim().Equals(weight)) 
                    answer_text = String.Empty;

                if (this.answer.getAnswer() != null)
                    this.answer.setAnswer(this.answer.getAnswer() + getLineChar() + answer_text);
                else
                    this.answer.setAnswer(answer_text);
            }
        }

        /// <summary>
        ///     Filters the answers out of a gaps exercise
        /// </summary>
        /// <param name="node">The node.</param>
        private void setGapsAnswers(XmlNode node)
        {
            const String regexBrackets = @"\[([^\[\]]+)\]";
            String rawText = this.currenNodeText(node,false);
            String answerText = rawText + String.Empty;
            String plainText = this.currenNodeText(node,true);
            List<int> answerWeights = new List<int>();
            List<String> feedbacks = new List<String>();
            Regex r1 = new Regex(regexBrackets, RegexOptions.IgnoreCase);
            Match matcherPlain = r1.Match(plainText);
            Match matcherRaw   = r1.Match(rawText);
            int indexcorrection=0;
            while (matcherPlain.Success && matcherRaw.Success)
            {
                while(insideXmlTag(rawText.Substring(matcherRaw.Groups[0].Index+matcherRaw.Groups[0].Length)))
                {
                    matcherRaw = matcherRaw.NextMatch();
                }
                String orig = matcherRaw.Groups[1].Value;
                String repl = matcherPlain.Groups[1].Value;
              //String repl = filterWeightAndFeedbackFromGapsTag(orig, ref answerWeights, ref feedbacks);
                // removes all XML from the content of the [ ] tags
                answerText = answerText.Substring(0,matcherRaw.Groups[1].Index + indexcorrection) + repl + answerText.Substring(matcherRaw.Groups[1].Index + matcherRaw.Groups[1].Length + indexcorrection);
                indexcorrection += repl.Length - orig.Length;
                matcherPlain = matcherPlain.NextMatch();
                matcherRaw = matcherRaw.NextMatch();
            }
            // there is still text from a previous P-block without answers in it
            if (this.answer.getAnswer() != null)
                answerText=this.answer.getAnswer() + answerText;
            
            // Adjust all previous answers
            if (this.question.getAnswersAsList().Count > 0)
            {
                answerText = question.getAnswer(0).getAnswer() + answerText;
                foreach (Answer ans in question.getAnswersAsList())
                {
                    ans.setAnswer(answerText);
                }
            }
            // only for first line without answers in it.
            if (answerWeights.Count == 0 && this.question.getAnswersAsList().Count == 0)
            {
                this.answer.setAnswer(answerText);
            }
            else
            {
                for (int i = 0; i < answerWeights.Count; i++)
                {
                    this.answer.setAnswer(answerText);
                    this.answer.setWeight(answerWeights[i]);
                    this.answer.setFeedback(feedbacks[i]);
                    this.question.addAnswer(this.answer);
                    this.answer = new Answer();
                }
                this.answer = null;
            }
        }
        
        /// <summary>
        ///     Filters the answers out of a gaps exercise
        /// </summary>
        /// <param name="node">The node.</param>
        private void setMatchAnswers(XmlNode node)
        {
            if (!node.Name.Equals("table"))
                return;
            XmlNodeList trnodelist = node.SelectNodes("tr");
            if (trnodelist.Count > 0 && this.answer!=null && !this.answer.isEmpty())
                this.question.addAnswer(this.answer);
            foreach (XmlNode trnode in trnodelist)
            {
                this.answer = new Answer();
                XmlNodeList tdnodelist = trnode.SelectNodes("td");
                if (tdnodelist.Count<2)
                    return;
                String answerText = Utility.stripHTMLParagraphs(this.currenNodeTextXml(tdnodelist[0], true));
                String matchText = Utility.stripHTMLParagraphs(this.currenNodeTextXml(tdnodelist[1], true));
                int answerweight = Int32.MinValue;

                if (Regex.IsMatch(matchText, Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                {
                    matchText=string.Empty;
                }

                if (Regex.IsMatch(answerText, Utility.ONLY_EMPTY_PARAGRAPH_REGEX))
                {
                    answerText=string.Empty;
                }
                else
                {
                    if (tdnodelist.Count > 2)
                    {
                        // get score string from third column
                        try
                        {
                            answerweight = Convert.ToInt32(
                                Utility.findWeightInString(tdnodelist[2].InnerText.Trim(), false)
                                );
                        }
                        catch
                        {
                            //answerweight=1;
                            answerweight = DomainController.Instance().getSettings().getDefaultScoreMatching(); //As of 1.08
                        }
                    }
                    else
                    {
                        //answerweight=1;
                        answerweight = DomainController.Instance().getSettings().getDefaultScoreMatching(); //As of 1.08
                    }
                }
                this.answer.setAnswer(answerText);
                this.answer.setMatchAnswer(matchText);
                this.answer.setWeight(answerweight);
                this.question.addAnswer(this.answer);
                this.answer = null;
            }
        }

        /// <summary>
        ///     This method tests if a string is the content of an xml tag.
        ///     The text just after the string is given as the source and the method looks if an "&gt;" is found when there is an "&lt;".
        /// </summary>
        /// <param name="search">The source text.</param>
        /// <returns>True if the string has been found.</returns>
        private Boolean insideXmlTag(String search)
        {
            if(!search.Contains('>')) return false;
            else
                if(!search.Contains('<'))
                    return true;
                else
                    if(search.IndexOf('<') > search.IndexOf('>'))
                        return true;
                    else
                        return false;
        }

        /// <summary>
        ///     Filters the score out of the internal text from a gaps tag, and returns the trimmed string.
        ///     The score is added to the provided integers list.
        /// </summary>
        /// <param name="gapsTag">The string inside a gaps exercise tag</param>
        /// <param name="answerWeights">The list to add the weight to.</param>
        /// <returns>The input string, with the weight trimmed off</returns>
        private String filterWeightFromGapsTag(String gapsTag, ref List<int> answerWeights)
        {
            String weightRegex = @"(?: |\|\|)([+-][0-9]+)$";
            Regex weight = new Regex(weightRegex);
            Match match = weight.Match(gapsTag);
            int answerWeight = 0;
            if (!match.Success)
            {
                answerWeight = DomainController.Instance().getSettings().getDefaultScoreGaps();
            }
            else
            {
                String score = match.Groups[1].Value;
                answerWeight = Convert.ToInt16(score);
                gapsTag = gapsTag.Substring(0, match.Index);
            }
            if (answerWeights != null)
                answerWeights.Add(answerWeight);
            return gapsTag;
        }

        /// <summary>
        ///     Filters the weight and feedback from a gaps answer, and returns the answer in one of three formats:
        ///     "anwers", "anwer1|anwer2|anwer3" or "{regex}regex statement{/regex}"
        /// </summary>
        /// <param name="gapText">The text taken from inside the brackets.</param>
        /// <param name="answerWeights">The list to add answer weights to.</param>
        /// <param name="feedbacks">The list to add answer feedbacks to.</param>
        /// <param name="trimwhitespace">Determines whether whitespace is trimmed around the answers.</param>
        /// <returns></returns>
        private String filterWeightAndFeedbackFromGapsTag(String gapText, ref List<int> answerWeights, ref List<String> feedbacks)
        {
            const String regexStartRegex = "{regex(=[0-9]+)?}";
            const String regexEnd = "{/regex}";
            const String regexContainsRegex = regexStartRegex + ".*?" + regexEnd;
            const String regexStartRegexOr = @"\|{regex(=[0-9]+)?}";
            const string feedbackSeparator = "||";

            Regex regexStart = new Regex(regexStartRegex);
            Regex regexStartOr = new Regex(regexStartRegexOr);
            Regex regexContains = new Regex(regexContainsRegex);

            gapText = filterWeightFromGapsTag(gapText, ref answerWeights);
            gapText = gapText.TrimEnd('|').TrimEnd();
            Match matcher = regexContains.Match(gapText);
            int regexPos = matcher.Index;
            int feedbackIndex = gapText.IndexOf(feedbackSeparator);
            int feedbackSepEnd = feedbackIndex + feedbackSeparator.Length;

            if (feedbackIndex != -1 && feedbackIndex == gapText.IndexOf('|'))
            {
                if (matcher.Success && regexPos < feedbackIndex)
                {
                    feedbackIndex = regexPos;
                    feedbackSepEnd = regexPos;
                }
                feedbacks.Add(gapText.Substring(0, feedbackIndex));
                return gapText.Substring(feedbackSepEnd);
            }
            else if (matcher.Success && regexPos > 0 && gapText[regexPos-1] != '|')
            {
                feedbacks.Add(gapText.Substring(0, regexPos));
                return gapText.Substring(regexPos);
            }
            else if (gapText.Contains('|'))
            {
                String[] gappossibilities;
                if (!regexContains.IsMatch(gapText))
                {
                    gappossibilities = gapText.Split('|');
                }
                else
                {
                    int index = 0;
                    List<String> gapposlist = new List<String>();
                    while (index != -1)
                    {
                        String curText = gapText.Substring(index);
                        int indexRegEnd = curText.IndexOf(regexEnd);
                        int indexSep = curText.IndexOf("|");
                        int indexEnd;
                        if (regexStart.IsMatch(curText) && indexRegEnd != -1)
                            indexEnd = indexRegEnd;
                        else
                            indexEnd = indexSep;

                        gapposlist.Add(curText.Substring(0, indexEnd));
                        index = indexEnd;
                    }
                    gappossibilities = gapposlist.ToArray();
                }

                String feedback = String.Empty;
                foreach (String str in gappossibilities)
                    feedback += " / " + str;
                if (feedback.Length > 3) feedback = feedback.Substring(3);
                feedbacks.Add(feedback);
                return gapText;
            }
            else
            {
                feedbacks.Add(String.Empty);
                return gapText;
            }
        }

        /// <summary>
        ///     Returns the new line character, based on the plaintext option.
        /// </summary>
        /// <returns>The "New Line Character".</returns>
        private String getLineChar()
        {
            if (plainText) 
                return Environment.NewLine;
            else return String.Empty; // "<br />" is no longer used because we keep the <p> blocks
        }

        /// <summary>
        ///     Filters the node on "\r" and "\n". Deletes all unnecessary HTML information.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="plainText">True if the output has to be plain text.</param>
        /// <param name="innerXml">True if only the inner text has to be used.</param>
        /// <returns>The filtered string.</returns>
        private String currenNodeText(XmlNode node, Boolean plainText)
        {
            if(plainText)
                return currenNodeTextPlain(node);
            else
                return currenNodeTextXml(node, false);
        }

        /// <summary>
        ///     Filters the node on "\r" and "\n".
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>The filtered string.</returns>
        private String currenNodeTextPlain(XmlNode node)
        {
            return characterCleanup(node.InnerText);
        }
        
        /// <summary>
        ///     Filters the node on "\r" and "\n". Deletes all unnecessary HTML information.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="innerXml">True if only the inner text has to be used.</param>
        /// <returns>The filtered string.</returns>
        private String currenNodeTextXml(XmlNode node, Boolean innerXml)
        {
            String retn;
            if (innerXml)
                retn = node.InnerXml;
            else            
                retn = node.OuterXml;
            retn = characterCleanup(retn);
            
            // filter <a name="_Toc...">...</a> tags from the MS Word 'Table of Contents' system
            Regex r = new Regex("<a name=\"_Toc.*?\">(.*?)</a>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(retn);
            while (matcher.Success)
            {
                // only execute if no nested tags
                if (!matcher.Groups[1].Value.Contains("<a"))
                {
                    retn = retn.Replace(matcher.Groups[0].Value, matcher.Groups[1].Value);
                }
                matcher = matcher.NextMatch();
            }
            // filter <span lang="xxx">...</span> tags from the contents
            r = new Regex("<span lang=\".*?\"(.*?)>(.*?)</span>", RegexOptions.IgnoreCase);
            matcher = r.Match(retn);
            while (matcher.Success)
            {
                // only execute if no nested tags
                if (!matcher.Groups[2].Value.Contains("<span"))
                {
                    // tag contains only language: remove it completely
                    if (matcher.Groups[1].Value.Trim().Equals(String.Empty))
                        retn = retn.Replace(matcher.Groups[0].Value, matcher.Groups[2].Value);
                    // tag also contains other information: only remove language part
                    else 
                        retn = retn.Replace(matcher.Groups[0].Value,
                                        "<span " + matcher.Groups[1].Value.Trim() + ">" + matcher.Groups[2].Value + "</span>");
                }
                matcher = matcher.NextMatch();
            }

            // fix underline colour of span tags
            r = new Regex("<u><span(.*?)>(.*?)</span></u>", RegexOptions.IgnoreCase);
            matcher = r.Match(retn);
            while (matcher.Success)
            {
                // only execute if no nested tags
                if (!matcher.Groups[2].Value.Contains("<span"))
                {
                    retn = retn.Replace(matcher.Groups[0].Value, 
                            "<span" + matcher.Groups[1].Value + "><u>" + matcher.Groups[2].Value + "</u></span>");
                }
                matcher = matcher.NextMatch();
            }
            // filter the class tags of this program out of the <p> tags
            r = new Regex("<p(.*?) class=\"" + this.digex_prefix + "\\S*?\"(.*?)>", RegexOptions.IgnoreCase);
            matcher = r.Match(retn);
            while (matcher.Success)
            {
                if (!matcher.Groups[1].Value.Contains(">"))
                {
                    String orig = matcher.Groups[0].Value;

                    String repl = matcher.Groups[1].Value.Trim() + " " + matcher.Groups[2].Value.Trim();
                    repl = repl.Trim();
                    repl = "<p" + (repl.Equals(String.Empty) ? String.Empty : " ") + repl + ">";
                    retn = retn.Replace(orig, repl);
                }
                matcher = matcher.NextMatch();
            }
            retn = retn.Trim();
            return retn;
        }

        private String characterCleanup(String str)
        { 
            String retn;
            // remove line ends
            retn = Regex.Replace(str,@"\s*[\r\n]+\s*"," ");
            retn = Regex.Replace(retn,@"\s*<br />\s*","<br />");
            //retn = str.Replace("\r\n ", " ").Replace(" \r\n", " ").Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
            // Convert MS Word quotes to normal ones to avoid problems with character conversion
            retn = retn.Replace('´','\'').Replace('`','\'').Replace('`','\'').Replace('‛','\'').Replace('’','\'').Replace('‘','\'');
            retn = retn.Replace('˝','\"').Replace('˝','\"').Replace('“','\"').Replace('”','\"').Replace('„','\"');
            // fix "..." character, because URLEncode replaces it by a normal "." for some bizarre reason.
            retn = retn.Replace("…", "...");
            retn = retn.Replace('–', '-');
            // Remove tabs
            retn = retn.Replace(Convert.ToChar(0xA0), ' ');
            return retn.Trim();
        }

        /// <summary>
        ///     Returns the question type.
        /// </summary>
        /// <returns>The question type.</returns>
        private QuestionType GetCurrentQuestionType()
        {
            if (this.styleName.Equals(this.digex_question_type_mcs))
                return QuestionType.MULTIPLE_CHOICE_SINGLE;
            else if (this.styleName.Equals(this.digex_question_type_mcm))
                return QuestionType.MULTIPLE_CHOICE_SEVERAL;
            else if (this.styleName.Equals(this.digex_question_type_gaps))
                return QuestionType.FILL_IN_THE_GAPS;
            else if (this.styleName.Equals(this.digex_question_type_gaps_dropdown))
                return QuestionType.FILL_IN_THE_GAPS_DROPDOWN;
            else if (this.styleName.Equals(this.digex_question_type_match))
                return QuestionType.MATCHING;
            else if (this.styleName.Equals(this.digex_question_type_open))
                return QuestionType.OPEN_QUESTION;
            else 
                return QuestionType.MULTIPLE_CHOICE_SINGLE;
        }

        #endregion

        #region Getters and setters

        public List<Exercise> getExercises()
        {
            return this.exercises;
        }
        
        public int getNumberOfWarnings()
        {
            return this.numberOfWarnings;
        }

        #endregion
    }
}
