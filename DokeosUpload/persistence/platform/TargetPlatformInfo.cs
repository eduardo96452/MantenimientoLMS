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
using lmsda.persistence.platform.service;
using lmsda.domain.user;

namespace lmsda.persistence.platform
{
    /// <summary>
    ///     This class contains the abstracts for the identifying properties and the factory
    ///     needed for a platform.
    ///     The actual platformInfo classes made from this are used to make a single array
    ///     that can be used for all actions related to platform choices.
    /// </summary>
    abstract class TargetPlatformInfo
    {
        /// <summary>
        ///     Determines the platform name.
        /// </summary>
        /// <returns>The platform name.</returns>
        abstract public String getPlatformName();

        /// <summary>
        ///     Determines the standard text encoding normally used for communicating with this platform.
        /// </summary>
        /// <returns>The text encoding as string</returns>
        abstract public String getPlatformEncoding();

        /// <summary>
        ///     Factory: instantiates a new object of this target platform.
        /// </summary>
        /// <param name="service">Indicates which service to use.</param>
        /// <param name="login">The login information.</param>
        /// <returns>A new object of this platform, based on the given information.</returns>
        abstract public TargetPlatform factory(Service service, Login login);
    }
}
