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

using lmsda.domain.exercise;

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     All question types are saved in this class.
    /// </summary>
    static class QuestionTypes
    {
        // the unique identifiers for each question type used on the platform
        public const int MULTIPLE_CHOICE_SINGLE     = 1;
        public const int MULTIPLE_CHOICE_SEVERAL    = 2;
        public const int FILL_IN_THE_GAPS           = 3;
        public const int MATCHING                   = 4;
        public const int OPEN_QUESTION              = 5;

        /// <summary>
        ///     returns a question type based on an index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The question type representing the index.</returns>
        public static QuestionType getQuestionType(int index)
        {
            switch (index)
            {
                case 0: default:
                    return QuestionType.NOT_SET;
                case MULTIPLE_CHOICE_SINGLE:
                    return QuestionType.MULTIPLE_CHOICE_SINGLE;
                case MULTIPLE_CHOICE_SEVERAL:
                    return QuestionType.MULTIPLE_CHOICE_SEVERAL;
                case FILL_IN_THE_GAPS:
                    return QuestionType.FILL_IN_THE_GAPS;
                case MATCHING:
                    return QuestionType.MATCHING;
                case OPEN_QUESTION:
                    return QuestionType.OPEN_QUESTION;
            }
        }
    }
}
