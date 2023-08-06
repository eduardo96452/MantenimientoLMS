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

namespace lmsda.persistence.platform.dokeos_1_8_5_hg
{
    /// <summary>
    ///     The platform information class for the Chamilo 2.0 platform.
    /// </summary>
    class Dokeos_1_8_5_hg_PlatformInfo : TargetPlatformInfo
    {
        public override String getPlatformName()
        {
            return "Dokeos 1.8.5 HoGent";
        }

        public override String getPlatformEncoding()
        {
            return "iso-8859-15";
        }

        public override TargetPlatform factory(Service service, Login login)
        {
            return new Dokeos_1_8_5_hg(login);
        }
    }
}
