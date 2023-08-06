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
using lmsda.domain;
using System.IO;

namespace lmsda.persistence.logger
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///     This static class is used for logging events.
    /// </summary>
    static class Logger
    {   
        public const long MAX_SIZE_IN_BYTES = 20480;
        private static String COMMENT_PREFIX = "#";
        private static String DEBUG_PREFIX = ";";


        /// <summary>
        ///     Returns the contents of the log.
        ///     Lines starting with ";", "#" and " " are ignored.
        /// </summary>
        /// <returns>The contents of the log.</returns>
        public static String getLog()
        {
            List<String> log = getLogAsList(false);

            log.Reverse();
            
            return String.Join(Environment.NewLine, log.ToArray());
        }

        /// <summary>
        ///     Returns the contents of the log in a list.
        /// </summary>
        /// <param name="advancedView">True if the debug lines have to be included.</param>
        /// <returns>The contents of the log in a list.</returns>
        /// <remarks>
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        ///      -> Added advanced view option. (true = all lines)
        /// </remarks>
        public static List<String> getLogAsList(Boolean advancedView)
        {
            List<String> log = new List<String>();
            try
            {
                String path = ProgramConstants.getLogFilePath();
                String line;
                StreamReader streamReader = File.OpenText(path);
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (!line.StartsWith(DEBUG_PREFIX) && !line.StartsWith(COMMENT_PREFIX) && !line.StartsWith(" "))
                        log.Add(line);
                    else
                        if (advancedView)
                            log.Add(line);
                }
                streamReader.Close();
            }
            catch (Exception) { }
            return log;
        }

        /// <summary>
        ///     Returns the last event of the log.
        ///     Lines starting with ";", "#" and " " are ignored.
        /// </summary>
        /// <returns>The last event.</returns>
        public static String getLastEvent()
        {
            String temp = String.Join(Environment.NewLine, getLogAsList(false).ToArray());
            int lio = temp.LastIndexOf(Environment.NewLine);
            if (temp.Equals(String.Empty))
            {
                return String.Empty;
            }
            temp = temp.Substring(lio + 2); //+2: "\r\n" are 2 characters.
            return temp;
        }

        /// <summary>
        ///     Writes an event to the log.
        /// </summary>
        /// <param name="value">The value that has to be written.</param>
        /// <param name="debug">True is the value is for debug purposes.</param>
        public static void write(String value, Boolean debug)
        {

            String path = ProgramConstants.getLogFilePath();
            StreamWriter streamWriter = null;
            try
            {
                FileInfo logfile = new FileInfo(path);
                Boolean newfile = !logfile.Exists;
                if (newfile)
                    File.CreateText(path).Close();
                else if (logfile.Length <= 5)
                {
                    StreamReader streamReader = File.OpenText(path);
                    newfile = streamReader.ReadToEnd().Equals(String.Empty);
                    streamReader.Close();
                }
                
                streamWriter = File.AppendText(path);

                if (newfile)
                {
                    // DomainController is usually not yet started at this point, so we can't use language strings. For that reason,
                    // the start-of-file comments indications will simply always be written in english.
                    streamWriter.Write(COMMENT_PREFIX);
                    streamWriter.Write(Environment.NewLine + COMMENT_PREFIX + " " + "Lines preceded by '" + DEBUG_PREFIX + "' are debug information.");
                    streamWriter.Write(Environment.NewLine + COMMENT_PREFIX + " " + "Lines preceded by '" + COMMENT_PREFIX + "' are comments.");
                    streamWriter.Write(Environment.NewLine + COMMENT_PREFIX);
                }

                if (debug)
                    streamWriter.Write(Environment.NewLine + DEBUG_PREFIX + String.Format("[{0:G}] {1}", DateTime.Now, value));
                else
                    streamWriter.Write(Environment.NewLine + String.Format("[{0:G}] {1}", DateTime.Now, value.Replace(Environment.NewLine, " ")));
            }
            catch { }
            finally
            {
                if (streamWriter!=null)
                    streamWriter.Close();
            }
        }

        /// <summary>
        ///     Writes an event to the log.
        /// </summary>
        /// <param name="value">The value that has to be written.</param>
        public static void write(String value)
        {
            Logger.write(value, false);
        }

        /// <summary>
        ///     Deletes the log file.
        /// </summary>
        public static void clear()
        {
            try { File.Delete(ProgramConstants.getLogFilePath()); }
            catch { }
        }

        /// <summary>
        ///     Truncates the log if the maximum size has been reached.
        /// </summary>
        /// <returns>True if the log file has been truncated.</returns>
        public static Boolean emptyLogIfSizeExceeded()
        {
            return emptyLogIfSizeExceeded(MAX_SIZE_IN_BYTES);
        }

        /// <summary>
        ///     Truncates the log if the maximum size has been reached.
        /// </summary>
        /// <param name="size">A user defined maximum size.</param>
        /// <returns>True if the log file has been truncated.</returns>
        public static Boolean emptyLogIfSizeExceeded(long size)
        {
            Boolean deleted = false;
            try
            {
                FileInfo fi = new FileInfo(ProgramConstants.getLogFilePath());
                if(fi.Length >= size)
                    clear();
                deleted = true;
            }
            catch { }
            return deleted;
        }
    }
}