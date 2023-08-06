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
using lmsda.domain;
using lmsda.persistence.resource;

namespace lmsda.persistence.document
{
    class DigexStyles
    {
        // language identifiers of digital exercise style strings.
        // The code combines these parts to the actual full strings.
        private const String LANG_DIGEX_PREFIX=                "digex_prefix";
        private const String LANG_DIGEX_EXERCISE=              "digex_exercise";
        private const String LANG_DIGEX_EXERCISE_DESC=         "digex_exercise_description";
        private const String LANG_DIGEX_QUESTION_TYPE=         "digex_question_type";
        private const String LANG_DIGEX_QUESTION_TYPE_MCS=     "digex_question_type_mcs";
        private const String LANG_DIGEX_QUESTION_TYPE_MCM=     "digex_question_type_mcm";
        private const String LANG_DIGEX_QUESTION_TYPE_GAPS=    "digex_question_type_gaps";
        private const String LANG_DIGEX_QUESTION_TYPE_GAPS_DROPDOWN =
                                                               "digex_question_type_gaps_dropdown";
        private const String LANG_DIGEX_QUESTION_TYPE_MATCH=   "digex_question_type_match";
        private const String LANG_DIGEX_QUESTION_TYPE_OPEN=    "digex_question_type_open";
        private const String LANG_DIGEX_QUESTION_DESC=         "digex_question_description";
        private const String LANG_DIGEX_ANSWER=                "digex_answer";
        private const String LANG_DIGEX_ANSWER_FEEDBACK=       "digex_answer_feedback";

        // old styles, for supporting original DokeosUpload documents. Don't change these.
        // Unlike the language ID strings, these are already the full strings.
        private const String OLD_DIGEX_PREFIX=                "DO";
        private const String OLD_DIGEX_EXERCISE=              "DO Oefening";
        private const String OLD_DIGEX_EXERCISE_DESC=         "DO Oefening - Beschrijving";
        private const String OLD_DIGEX_QUESTION_TYPE=         "DO Vraag - Type";
        private const String OLD_DIGEX_QUESTION_TYPE_MCS=     "DO Vraag - Type meerkeuze";
        private const String OLD_DIGEX_QUESTION_TYPE_MCM=     "DO Vraag - Type meerkeuze meerdere";
        private const String OLD_DIGEX_QUESTION_TYPE_GAPS=    "DO Vraag - Type invuloefening";
        private const String OLD_DIGEX_QUESTION_TYPE_GAPS_DROPDOWN =
                                                              "DO Vraag - Type invuloefening meerkeuze";
        private const String OLD_DIGEX_QUESTION_TYPE_MATCH=   "DO Vraag - Type matching";
        private const String OLD_DIGEX_QUESTION_TYPE_OPEN=    "DO Vraag - Type open vraag";
        private const String OLD_DIGEX_QUESTION_DESC=         "DO Vraag - Beschrijving";
        private const String OLD_DIGEX_ANSWER=                "DO Vraag - Antwoord";
        private const String OLD_DIGEX_ANSWER_FEEDBACK=       "DO Vraag - Feedback";

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
        private List<String> stylesList;
        
        /// <summary>
        ///     Creates a DigexStyles object based on the given language
        /// </summary>
        /// <param name="language">The language to use. A language with * in it will trigger legacy support.</param>
        public DigexStyles(String language)
        {
            // Any language string containing '*' can't refer to a filename, and will load
            // the style names used by the documents of the original old DokeosUpload program.
            if (language.Contains('*'))
                loadOldStyles();
            else
                loadLanguage(language);
        }

        /// <summary>
        ///     Loads the style names set for the specified language.
        /// </summary>
        /// <param name="language"></param>
        private void loadLanguage(String language)
        {
            ResourceLoader stringsList = new ResourceLoader(ProgramConstants.getResourcePath(), language, ProgramConstants.RESOURCE_EXTENSION);

            digex_prefix=               stringsList.getString(LANG_DIGEX_PREFIX);
            digex_exercise=             digex_prefix + " " + stringsList.getString(LANG_DIGEX_EXERCISE);
            digex_exercise_desc=        digex_prefix + " " + stringsList.getString(LANG_DIGEX_EXERCISE_DESC);
            digex_question_type=        digex_prefix + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE);
            digex_question_type_mcs=    digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE);
            digex_question_type_mcm=    digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE_MCM);
            digex_question_type_gaps=   digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE_GAPS);
            digex_question_type_gaps_dropdown=
                                        digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE_GAPS_DROPDOWN);
            digex_question_type_match=  digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE_MATCH);
            digex_question_type_open=   digex_question_type + " " + stringsList.getString(LANG_DIGEX_QUESTION_TYPE_OPEN);
            digex_question_desc=        digex_prefix + " " + stringsList.getString(LANG_DIGEX_QUESTION_DESC);
            digex_answer=               digex_prefix + " " + stringsList.getString(LANG_DIGEX_ANSWER);
            digex_answer_feedback=      digex_prefix + " " + stringsList.getString(LANG_DIGEX_ANSWER_FEEDBACK);
            makeList();
        }

        /// <summary>
        ///     Creates a DigexStyles object based on the language currently configured in the program.
        /// </summary>
        public DigexStyles()
        {
            DomainController dc = DomainController.Instance();

            digex_prefix=               dc.getLanguageString(LANG_DIGEX_PREFIX);
            digex_exercise=             digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_EXERCISE);
            digex_exercise_desc=        digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_EXERCISE_DESC);
            digex_question_type=        digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE);
            digex_question_type_mcs=    digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_MCS);
            digex_question_type_mcm=    digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_MCM);
            digex_question_type_gaps=   digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_GAPS);
            digex_question_type_gaps_dropdown=
                                        digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_GAPS_DROPDOWN);
            digex_question_type_match=  digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_MATCH);
            digex_question_type_open=   digex_question_type + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_TYPE_OPEN);
            digex_question_desc=        digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_QUESTION_DESC);
            digex_answer=               digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_ANSWER);
            digex_answer_feedback=      digex_prefix + " " + dc.getLanguageString(LANG_DIGEX_ANSWER_FEEDBACK);
            makeList();
        }

        /// <summary>
        ///     Loads the style names used by the original first DokeosUpload program.
        /// </summary>
        private void loadOldStyles()
        {
            digex_prefix=               OLD_DIGEX_PREFIX;
            digex_exercise=             OLD_DIGEX_EXERCISE;
            digex_exercise_desc=        OLD_DIGEX_EXERCISE_DESC;
            digex_question_type=        OLD_DIGEX_QUESTION_TYPE;
            digex_question_type_mcs=    OLD_DIGEX_QUESTION_TYPE;
            digex_question_type_mcm=    OLD_DIGEX_QUESTION_TYPE_MCM;
            digex_question_type_gaps=   OLD_DIGEX_QUESTION_TYPE_GAPS;
            digex_question_type_gaps_dropdown=
                                        OLD_DIGEX_QUESTION_TYPE_GAPS_DROPDOWN;
            digex_question_type_match=  OLD_DIGEX_QUESTION_TYPE_MATCH;
            digex_question_type_open=   OLD_DIGEX_QUESTION_TYPE_OPEN;
            digex_question_desc=        OLD_DIGEX_QUESTION_DESC;
            digex_answer=               OLD_DIGEX_ANSWER;
            digex_answer_feedback=      OLD_DIGEX_ANSWER_FEEDBACK;
            makeList();
        }


        private void makeList()
        {
            this.stylesList = new List<String>(
                new String[]
                    {
                        digex_exercise,
                        digex_exercise_desc,
                        digex_question_desc,
                        digex_question_type_mcs,
                        digex_question_type_mcm,
                        digex_question_type_gaps,
                        digex_question_type_gaps_dropdown,
                        digex_question_type_match,
                        digex_question_type_open,
                        digex_answer,
                        digex_answer_feedback
                    }
            );
        }

        public String getDigexPrefix()
        {
            return this.digex_prefix;
        }
        public String getDigexExercise()
        {
            return this.digex_exercise;
        }
        public String getDigexExerciseDesc()
        {
            return this.digex_exercise_desc;
        }
        public String getDigexQuestionType()
        {
            return this.digex_question_type;
        }
        public String getDigexQuestionTypeMcs()
        {
            return this.digex_question_type_mcs;
        }
        public String getDigexQuestionTypeMcm()
        {
            return this.digex_question_type_mcm;
        }
        public String getDigexQuestionTypeGaps()
        {
            return this.digex_question_type_gaps;
        }
        public String getDigexQuestionTypeGapsDropdown()
        {
            return this.digex_question_type_gaps_dropdown;
        }
        public String getDigexQuestionTypeMatch()
        {
            return this.digex_question_type_match;
        }
        public String getDigexQuestionTypeOpen()
        {
            return this.digex_question_type_open;
        }
        public String getDigexQuestionDesc()
        {
            return this.digex_question_desc;
        }
        public String getDigexAnswer()
        {
            return this.digex_answer;
        }
        public String getDigexAnswerFeedback()
        {
            return this.digex_answer_feedback;
        }

        public List<String> getAllValidStyles()
        {
            return this.stylesList;
        }
    }
}
