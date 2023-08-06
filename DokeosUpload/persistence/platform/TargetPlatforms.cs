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
using lmsda.persistence.platform.dokeos_1_8_5_hg;
using lmsda.persistence.platform.chamilo_2_0;
using lmsda.persistence.platform.service;
using lmsda.domain.user;

namespace lmsda.persistence.platform
{
    static class TargetPlatforms
    {

        #region Definition of the platforms list

        /// <summary>
        ///     Defines all target platforms supported by the application. A new platforms can
        ///     be added by defining a new TargetPlatformInfo class for it and putting it in
        ///     this array.
        /// </summary>
        /// <returns>An array containing the AbstractTargetPlatformInfo objects for all supported document types.</returns>
        public static TargetPlatformInfo[] getSupportedPlatforms()
        {
            return new TargetPlatformInfo[]
            {
                // The first platform listed here will be used as default when first launching the program
                new Chamilo_2_0_PlatformInfo(),
                //new Chamilo_2_0_Test_PlatformInfo(),
                new Dokeos_1_8_5_hg_PlatformInfo()
            };
        }

        #endregion

        #region Methods related to target platform choice

        /// <summary>
        ///     Factory: instantiates a new platform.
        /// </summary>
        /// <param name="platform">The name of the platform.</param>
        /// <param name="service">Indicates which service to use.</param>
        /// <param name="login">The login information.</param>
        /// <returns>A new abstractTargetPlatform object, based on the given information.</returns>
        public static TargetPlatform factory(String platform, Service service, Login login)
        {
            TargetPlatformInfo platforminfo = getPlatformInfo(platform);
            if (platforminfo != null)
                return platforminfo.factory(service, login);
            else
                // if the platform name didn't match any existing platforms
                throw new NotSupportedException();
        }

        public static TargetPlatformInfo getPlatformInfo(String platformName)
        {
            foreach (TargetPlatformInfo platforminfo in TargetPlatforms.getSupportedPlatforms())
            {
                if (platformName.Equals(platforminfo.getPlatformName()))
                    return platforminfo;
            }
            return null;
        }

        #endregion

        #region Methods related to requesting platform information

        /// <summary>
        ///     Returns the supported platforms.
        /// </summary>
        /// <returns>A String array with all supported platforms.</returns>
        public static String[] getTargetPlatforms()
        {
            List<String> platforms = new List<String>();

            foreach (TargetPlatformInfo platform in TargetPlatforms.getSupportedPlatforms())
            {
                platforms.Add(platform.getPlatformName());
            }
            return platforms.ToArray();
        }

        /// <summary>
        ///     Returns the default (first) platform.
        /// </summary>
        /// <returns>The name of the default platform.</returns>
        public static TargetPlatformInfo getDefaultPlatformInfo()
        {
            if (TargetPlatforms.getSupportedPlatforms().Length > 0)
            {
                return TargetPlatforms.getSupportedPlatforms()[0];
            }
            return null;
        }

        #endregion

    }
}
