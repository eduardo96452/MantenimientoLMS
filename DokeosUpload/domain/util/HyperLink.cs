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

namespace lmsda.domain.util
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used for storing a hyperlink and its target.
    /// </summary>
    class HyperLink
    {
        private String target;
        private String address;

        public HyperLink(String target, String address)
        {
            this.setAddress(address);
            this.setTarget(target);
        }

        public void setTarget(String target)
        {
            this.target = target;
        }

        public void setAddress(String address)
        {
            this.address = address;
        }

        public String getTarget()
        {
            return this.target;
        }

        public String getAddress()
        {
            return this.address;
        }
    }
}
