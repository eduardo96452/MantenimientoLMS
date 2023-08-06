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
using lmsda.persistence.platform.chamilo_2_0.post;
using lmsda.persistence.platform.chamilo_2_0.webservice;

namespace lmsda.persistence.platform.chamilo_2_0
{
    /// <summary>
    ///     The platform information class for the Chamilo 2.0 platform.
    ///     This serves for both the Post and Webservice implementation of the platform.
    /// </summary>
    class Chamilo_2_0_PlatformInfo : TargetPlatformInfo
    {
        public override String getPlatformName()
        {
            return "Chamilo 2.0";
        }

        public override String getPlatformEncoding()
        {
            return "utf-8";
        }

        public override TargetPlatform factory(Service service, Login login)
        {
            return new Chamilo_2_0_Post(login);
            /*
            switch (service)
            {
                case Service.WEBSERVICE:
                    return new Chamilo_2_0_Webservice(login);
                default:
                    return new Chamilo_2_0_Post(login);
            }
            //*/
        }
    }
}
