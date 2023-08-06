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
using System.Text;
using System.Text.RegularExpressions;
using lmsda.domain.util;

namespace lmsda.domain.exercise
{
    /// <summary>
    ///     Author: Gianni Van Hoecke, Maarten Meuris
    ///     This class acts as a container of questions.
    /// </summary>
    class Exercise
    {
        private String name;
        private Boolean multipage;
        private String description;
        private List<Question> questions;

        /// <summary>
        ///     Simple constructor, that just intializes the list of questions.
        /// </summary>
        public Exercise()
        {
            this.questions = new List<Question>();
        }

        /// <summary>
        ///     The constructor with 2 parameters. This method is used if the multipage option isn't needed.
        /// </summary>
        /// <param name="name">The name of the exercise.</param>
        /// <param name="description">The description of the exercise.</param>
        public Exercise(String name, String description)
        {
            this.setName(name);
            this.setDescription(description);
            this.questions = new List<Question>();
        }

        /// <summary>
        ///     The constructor with 2 parameters. This method is used if the multipage option is needed.
        /// </summary>
        /// <param name="name">The name of the exercise.</param>
        /// <param name="multipage">True if the exercise should be solved one question at the time.</param>
        /// <param name="description">The description of the exercise.</param>
        public Exercise(String name, Boolean multipage, String description)
            : this(name, description)
        {
            this.setMultipage(multipage);
        }

        /// <summary>
        ///     Adds a question to the exercise.
        /// </summary>
        /// <param name="question">The Question object.</param>
        public void addQuestion(Question question)
        {
            this.questions.Add(question);
        }

        /// <summary>
        ///     Returns the list of questions, as List.
        /// </summary>
        /// <returns>A List of Question objects.</returns>
        public List<Question> getQuestionsAsList()
        {
            return this.questions;
        }

        /// <summary>
        ///     Returns the list of questions, as Array.
        /// </summary>
        /// <returns>An array of Question objects.</returns>
        public Question[] getQuestionsAsArray()
        {
            return this.getQuestionsAsList().ToArray();
        }

        /// <summary>
        ///     Returns the question at the specified index.
        /// </summary>
        /// <param name="i">The index.</param>
        /// <returns>The question object at that index.</returns>
        public Question getQuestion(int i)
        {
            return this.getQuestionsAsArray()[i];
        }

        /// <summary>
        ///     Returns an MD5 of the exercise text dump, ignoring filenames of HTML-linked images but taking their MD5 into account instead.
        /// </summary>
        /// <returns>An MD5 string</returns>
        public String getMD5()
        {
            return getMD5(null,null);
        }

        /// <summary>
        ///     Returns two MD5s of the exercise text dump, ignoring filenames of HTML-linked images but taking their MD5 into account instead.
        ///     The first MD5 is generated using static strings, the second using translated strings.
        /// </summary>
        /// <param name="alternateImagesExtension"></param>
        /// <param name="alternateImagesPath"></param>
        /// <returns>An MD5 string</returns>
        public String getMD5LegacySupport(String alternateImagesPath, String alternateImagesExtension)
        {
            return getMD5(alternateImagesPath, alternateImagesExtension, false) + " " + getMD5(alternateImagesPath, alternateImagesExtension, true);
        }

        /// <summary>
        ///     Returns an MD5 of the exercise text dump, ignoring filenames of HTML-linked images but taking their MD5 into account instead.
        /// </summary>
        /// <param name="alternateImagesExtension">Alternate extension to use for generating image MD5s</param>
        /// <param name="alternateImagesPath">Alternate path to use for generating image MD5s</param>
        /// <returns>An MD5 string</returns>
        public String getMD5(String alternateImagesPath, String alternateImagesExtension)
        {
            return getMD5(alternateImagesPath, alternateImagesExtension, false);
        }

        /// <summary>
        ///     Returns an MD5 of the exercise text dump, ignoring filenames of HTML-linked images but taking their MD5 into account instead.
        /// </summary>
        /// <param name="alternateImagesExtension">Alternate extension to use for generating image MD5s</param>
        /// <param name="alternateImagesPath">Alternate path to use for generating image MD5s</param>
        /// <param name="useTranslatedToString">Take MD5 from the ToString method which translates string to the currently set language</param>
        /// <returns>An MD5 string</returns>
        public String getMD5(String alternateImagesPath, String alternateImagesExtension, Boolean useTranslatedToString)
        {
            if (alternateImagesPath != null && alternateImagesPath.Equals(String.Empty))
                alternateImagesPath = null;
            if (alternateImagesExtension != null && alternateImagesExtension.Equals(String.Empty))
                alternateImagesExtension = null;
            Exercise md5ex = this.clone();
            Dictionary<String, String> processedList = new Dictionary<String,String>();

            md5ex.setDescription(replaceImageMD5s(md5ex.getDescription(), ref processedList, alternateImagesPath, alternateImagesExtension));

            foreach (Question q in md5ex.getQuestionsAsList())
            {
                q.setQuestionText(replaceImageMD5s(q.getQuestionText(), ref processedList, alternateImagesPath, alternateImagesExtension));
                foreach (Answer a in q.getAnswersAsList())
                {
                    a.setAnswer(replaceImageMD5s(a.getAnswer(), ref processedList, alternateImagesPath, alternateImagesExtension));
                    a.setFeedback(replaceImageMD5s(a.getFeedback(), ref processedList, alternateImagesPath, alternateImagesExtension));
                }
            }
            if (useTranslatedToString)
                return Utility.calculateMD5ForString(md5ex.toStringTranslated());
            else
                return Utility.calculateMD5ForString(md5ex.toString());
        }

        /// <summary>
        ///     Replaces the image URL in all img tags by the MD5 of the image file.
        /// </summary>
        /// <param name="input">The input to process</param>
        /// <param name="processedList">list of already-processed images</param>
        /// <param name="alternateImagesPath">Alternate path to use for generating image MD5s</param>
        /// <param name="alternateImagesExtension">Alternate extension to use for generating image MD5s</param>
        /// <returns>the string with images replaced by their MD5 value</returns>
        private String replaceImageMD5s(String input, ref Dictionary<String,String> processedList, String alternateImagesPath, String alternateImagesExtension)
        {
            if(input == null)
                return null;

            Regex r = new Regex(@"<img(.*?)>", RegexOptions.IgnoreCase);
            Match matcher = r.Match(input);
            while (matcher.Success)
            {
                // <img src="path/image.ext"> --> <img src="MD5HASH">
                Match urlmatcher = Regex.Match(matcher.Groups[1].Value,"src=\"(.*?)\"");
                String image_link = matcher.Groups[0].Value;
                if (matcher.Success)
                {
                    // src="path/image.ext" --> src="MD5HASH"
                    String relImagePath = urlmatcher.Groups[1].Value.Replace('/', '\\');
                    String magePath2 = relImagePath;
                    if (alternateImagesPath != null)
                    {
                        magePath2 = alternateImagesPath.TrimEnd('\\') + '\\' + magePath2.Substring(magePath2.LastIndexOf('\\')).Trim('\\');
                    }
                    if (alternateImagesExtension != null)
                    {
                        magePath2 = magePath2.Substring(0, magePath2.LastIndexOf('.')).TrimEnd('.') + '.' + alternateImagesExtension;
                    }

                    String imagepath = DomainController.Instance().getTempPath();
                    
                    if (Utility.doesFileExist(imagepath + magePath2))
                        imagepath+=magePath2;
                    else
                        imagepath+=relImagePath;
                    
                    String imageMD5;
                    if (!processedList.TryGetValue(imagepath, out imageMD5))
                    {
                        imageMD5 = Utility.calculateMD5ForFile(imagepath);
                        processedList.Add(imagepath, imageMD5);
                    }
                    // src="path/image.ext" --> src="MD5HASH"
                    image_link = image_link.Replace(urlmatcher.Groups[0].Value,"src=\"" + imageMD5 + "\"");
                    // <img src="path/image.ext"> --> <img src="MD5HASH">
                    input = input.Replace(matcher.Groups[0].Value, image_link);
                }
                matcher = matcher.NextMatch();
            }
            return input;
        }

        /// <summary>
        ///     Returns the full contents of the object as String.
        /// </summary>
        /// <returns>The readable version of the object.</returns>
        public String toString()
        {
            String value = toStringSimple(false) + Environment.NewLine;
            for (int i = 0; i < questions.Count; i++)
            {
                value += Environment.NewLine +
                    "    " + "question" + " " + (i + 1) + ":" + Environment.NewLine +
                    questions[i].toString("    ");
            }
            return value;
        }

        /// <summary>
        ///     Returns the contents of the object as String, without the contents of the Question list.
        /// </summary>
        /// <param name="showMultipageOption">Also show the option that determines of the exercise will be made on multiple pages</param>
        /// <returns>The readable version of the object, without the questions.</returns>
        public String toStringSimple(Boolean showMultipageOption)
        {
            return "title" + " = " + (this.getName() != null  && !this.getName().Equals(String.Empty) ? this.getName() : "[[not set]]") + Environment.NewLine +
                   "description" + " = " + (this.getDescription() != null  && !this.getDescription().Equals(String.Empty) ? this.getDescription() : "[[not set]]") + Environment.NewLine +
                   (showMultipageOption ? ("multipage" + " = " + (this.isMultipage() ? "yes" : "no")) : String.Empty);
        }

        /// <summary>
        ///     Returns the full contents of the object as String, using translated strings.
        /// </summary>
        /// <returns>The readable version of the object.</returns>
        public String toStringTranslated()
        {
            String value = toStringSimpleTranslated(false) + Environment.NewLine;
            DomainController dc = DomainController.Instance();
            for (int i = 0; i < questions.Count; i++)
            {
                value += Environment.NewLine +
                    "    " + dc.getLanguageString("ed_question") + " " + (i + 1) + ":" + Environment.NewLine +
                    questions[i].toStringTranslated("    ");
            }
            return value;
        }

        /// <summary>
        ///     Returns the contents of the object as String, without the contents of the Question list, using translated strings.
        /// </summary>
        /// <param name="showMultipageOption">Also show the option that determines of the exercise will be made on multiple pages</param>
        /// <returns>The readable version of the object, without the questions.</returns>
        public String toStringSimpleTranslated(Boolean showMultipageOption)
        {
            DomainController dc = DomainController.Instance();
            return dc.getLanguageString("ed_title") + " = " + (this.getName() != null && !this.getName().Equals(String.Empty) ? this.getName() : dc.getLanguageString("ed_not_set")) + Environment.NewLine +
                dc.getLanguageString("ed_description") + " = " + (this.getDescription() != null && !this.getDescription().Equals(String.Empty) ? this.getDescription() : dc.getLanguageString("ed_not_set")) + Environment.NewLine +
                (showMultipageOption ? (dc.getLanguageString("ed_multipage") + " = " + (this.isMultipage() ? dc.getLanguageString("yes") : dc.getLanguageString("no"))) : String.Empty);
        }

        /// <summary>
        ///     Checks if this object is empty.
        /// </summary>
        /// <returns>True if all properties are empty.</returns>
        public Boolean isEmpty()
        {
            return (name        == null || name.Equals(String.Empty)       )
                && (description == null || description.Equals(String.Empty))
                && (questions   == null || questions.Count == 0            );
        }
        
        /// <summary>
        ///     Returns an new object of an exercise, but with the same values.
        /// </summary>
        /// <returns>An new answer object, with the same values.</returns>
        public Exercise clone()
        {
            Exercise retn = new Exercise(this.name,this.multipage,this.description);
            foreach (Question q in questions)
            {
                retn.addQuestion(q.clone());
            }
            return retn;
        }

        #region Getters and setters

        public void setName(String name)
        {
            this.name = name;
        }

        public String getName()
        {
            return this.name;
        }

        public void setMultipage(Boolean multipage)
        {
            this.multipage = multipage;
        }

        public Boolean isMultipage()
        {
            return this.multipage;
        }

        public void setDescription(String description)
        {
            this.description = description;
        }

        public String getDescription()
        {
            return this.description;
        }

        #endregion

    }
}
