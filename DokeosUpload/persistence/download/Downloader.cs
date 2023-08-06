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
using System.Net;
using lmsda.domain;
using System.IO;

namespace lmsda.persistence.download
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This class is used to download and process files.
    /// </summary>
    static class Downloader
    {
        /// <summary>
        ///     Gets the latest program version.
        /// </summary>
        /// <returns>The latest program version.</returns>
        public static String getLatestProgramVersion()
        {
            String returnValue = String.Empty;
            StreamReader stream = null;

            try
            {
                WebClient webClient = new WebClient();
                stream = new StreamReader(webClient.OpenRead(ProgramConstants.UPDATE_LINK));
                String line;
                do
                {
                    line = stream.ReadLine();
                    try
                    {
                        returnValue += line;
                    }
                    catch { } //first line was null, do not add...
                }
                while (line != null);
                
            }
            catch (Exception e)
            {
                DomainController.Instance().processError(e, false);
            }
            finally
            {
                if (stream!=null)
                    stream.Close();
            }

            return returnValue;
        }

        /// <summary>
        ///     Gets the latest program version.
        /// </summary>
        /// <returns>The latest program version.</returns>
        public static String getLatestManualVersion()
        {
            String returnValue = String.Empty;
            StreamReader stream = null;

            try
            {
                WebClient webClient = new WebClient();
                stream = new StreamReader(webClient.OpenRead(ProgramConstants.MANUAL_VERSION));
                String line;
                do
                {
                    line = stream.ReadLine();
                    try
                    {
                        returnValue += line;
                    }
                    catch { } //first line was null, do not add...
                }
                while (line != null);
                
            }
            catch (Exception e)
            {
                DomainController.Instance().processError(e, false);
            }
            finally
            {
                stream.Close();
            }

            return returnValue;
        }

        /// <summary>
        ///     Downloads the given source to a local folder.
        /// </summary>
        /// <param name="sourceURL">The URL where the data can be found.</param>
        /// <param name="destination">The destination: full file name inc. extension.</param>
        /// <returns>True if the download was a succes.</returns>
        public static Boolean downloadFile(String sourceURL, String destination)
        {
            try
            {
                WebClient webClient = new WebClient();
                webClient.DownloadFile(sourceURL, destination);
            }
            catch (Exception e)
            {
                DomainController.Instance().processError(e, true);
                return false;
            }
            return true;
        }
    }
}
