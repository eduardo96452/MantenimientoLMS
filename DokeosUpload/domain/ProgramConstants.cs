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
using System.Linq;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using lmsda.persistence.platform;
using lmsda.domain.util;
using lmsda.persistence.resource;


namespace lmsda.domain
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     All the constant values needed by the program are stored here.
    /// </summary>
    static class ProgramConstants
    {
        public const String PROGRAM_NAME                = "LMS Desktop Assistant";
        public const String USERAGENT_LMSDA             = PROGRAM_NAME;
        public const String RESOURCE_DIRECTORY_NAME     = "language";
        public const String DEFAULT_LANGUAGE            = "English";
        public const String DEFAULT_PLATFORM_URL        = "https://chamilo.hogent.be";
        public const String SETTINGS_FILE_NAME          = "settings.xml";
        public const String SUBJECT_SETTINGS_FILE_NAME  = "subjects.xml";
        public const String USER_STORAGE_FILE_NAME      = "userfolders.txt";
        public const String RESOURCE_EXTENSION          = ".ini";
        public const String LOG_FILE_NAME               = "eventslog.txt";
        public const String TEMP_DIRECTORY              = PROGRAM_NAME + " Temp";
        public const String HTML_DUMP_NAME              = "document.html";
        public const String HTML_OUTPUT_NAME            = "documentscan.html";
        public const String LISTSEPARATOR               = "--------------";
        public const String SYNCHRONIZATION_FILE_NAME   = "lms_synchronization_settings.xml";
        public const String TEMPLATE_BLANK              = "_blank";
        public const String TEMPLATE_EXAMPLES           = "_full";
        public const String TEMPLATE_EXTENSION          = "dotx";
        public const String WEBSITE_LINK                = "http://lmsda.sourceforge.net/";
        public const String UPDATE_LINK                 = WEBSITE_LINK + "version.txt";
        public const String CHANGE_LOG                  = WEBSITE_LINK + "changelog.txt";
        public const String UPDATE_SOURCE               = "https://sourceforge.net/projects/lmsda/files/";
        public const String MANUAL_VERSION              = WEBSITE_LINK + "manualversion.txt";
        public const int    DEFAULT_SCORE_INIT          = 1;

        public const String PDF_MASK_ELEMENT_FILENAME  = "{file_name}";
        public const String PDF_MASK_ELEMENT_NR        = "{nr}";
        public const String PDF_MASK_ELEMENT_STYLETEXT = "{style_text}";

        public const String PDF_MASK_FILENAME_NR = PDF_MASK_ELEMENT_FILENAME + "_" + PDF_MASK_ELEMENT_NR;
        public const String PDF_MASK_NR_STYLETEXT = PDF_MASK_ELEMENT_NR + "_" + PDF_MASK_ELEMENT_STYLETEXT;
        public const String PDF_MASK_FILENAME_NR_STYLETEXT = PDF_MASK_ELEMENT_FILENAME + "_" + PDF_MASK_ELEMENT_NR + "_" + PDF_MASK_ELEMENT_STYLETEXT;

        public const String PDF_MASK_DEFAULT = PDF_MASK_FILENAME_NR;
        public static readonly String[] PDF_MASKS_LIST = new String[] 
            {
                PDF_MASK_FILENAME_NR,
                PDF_MASK_NR_STYLETEXT,
                PDF_MASK_FILENAME_NR_STYLETEXT
            };


        
        /// <summary>
        ///     Returns the folder where all language files are located, with trailing backslash.
        /// </summary>
        /// <returns>The folder that contains all language files.</returns>
        public static String getResourcePath()
        {
            return ProgramConstants.getProgramPath() + ProgramConstants.RESOURCE_DIRECTORY_NAME + @"\";
        }

        /// <summary>
        ///     Returns the actual program folder, with trailing backslash.
        /// </summary>
        /// <returns>The folder that contains all language files.</returns>
        public static String getProgramPath()
        {
            return Path.GetDirectoryName(Application.ExecutablePath).TrimEnd('\\') + @"\";
        }
        /// <summary>
        ///     Returns the current program version, in the format x.xx(.x)
        ///     - The last part is omitted if it is '.0'
        /// </summary>
        /// <returns>The current program version.</returns>
        public static String programVersion()
        {
            String version = Application.ProductVersion;
            String[] versionNumbers = version.Split('.');
            // ProductVersion is always "xx.xx.xx.xx"; a 4-element array after the split.
            version = versionNumbers[0] + "." + versionNumbers[1] + versionNumbers[2];
            if (!versionNumbers[3].Equals("0"))
                version += "." + versionNumbers[3];
            return version;
        }

        /// <summary>
        ///     Returns the path to the temporary folder, with trailing backslash.
        /// </summary>
        /// <returns>The path to the temporary folder.</returns>
        public static String getTempPath()
        {
            return Path.GetTempPath().TrimEnd('\\') + @"\" + ProgramConstants.TEMP_DIRECTORY + @"\";
        }

        /// <summary>
        ///     Returns the global program storage folder, with trailing backslash.
        /// </summary>
        /// <returns>The global program storage folder, with trailing backslash.</returns>
        public static String getProgramStoragePath()
        {
            if (!PortableIdentifier.Instance().isPortable)
                return getInstallProgramStoragePath();
            else
                return getProgramPath();
        }

        /// <summary>
        ///     Returns the global program storage folder of a full local install, with trailing backslash.
        /// </summary>
        /// <returns>The global program storage folder, with trailing backslash.</returns>
        public static String getInstallProgramStoragePath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData).TrimEnd('\\') + @"\" + ProgramConstants.PROGRAM_NAME + @"\";
        }

        /// <summary>
        ///     Returns the local program settings folder, with trailing backslash.
        /// </summary>
        /// <returns>The local program settings folder, with trailing backslash.</returns>
        public static String getProgramSettingsPath()
        {
            if (!PortableIdentifier.Instance().isPortable)
                return getInstallProgramSettingsPath();
            else
                return getProgramPath();
        }

        /// <summary>
        ///     Returns the local program settings folder of a full local install, with trailing backslash.
        /// </summary>
        /// <returns>The local program settings folder, with trailing backslash.</returns>
        private static String getInstallProgramSettingsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).TrimEnd('\\') + @"\" + ProgramConstants.PROGRAM_NAME + @"\";
        }

        /// <summary>
        ///     Returns the "My Documents" path
        /// </summary>
        /// <returns>The settings file.</returns>
        public static String getMyDocumentsPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).TrimEnd('\\') + @"\";
        }        

        /// <summary>
        ///     Returns the settings file.
        /// </summary>
        /// <returns>The settings file.</returns>
        public static String getSettingsPath()
        {
            return getProgramSettingsPath() + ProgramConstants.SETTINGS_FILE_NAME;
        }
        
        /// <summary>
        ///     Returns the settings file of a full local install
        /// </summary>
        /// <returns>The settings file.</returns>
        public static String getInstallSettingsPath()
        {
            return getInstallProgramSettingsPath() + ProgramConstants.SETTINGS_FILE_NAME;
        }

        /// <summary>
        ///     Returns the current Windows language set for the user if it is is available in the LMSDA languages list.
        ///     Otherwise it returns the default language set in the program constants.
        /// </summary>
        /// <returns></returns>
        public static String getDefaultOrSystemLanguage()
        {
            String[] langlist = ResourceList.getResources(getResourcePath(), RESOURCE_EXTENSION);

            // if no languages are installed, return empty string to default to code strings.
            if (langlist.Length == 0)
                return String.Empty;

            // first try UI language. NativeName returns the format "Language (Country)".
            String defaultLanguage = filterLanguage(langlist, System.Globalization.CultureInfo.CurrentUICulture.NativeName);

            // then try system language language
            if (defaultLanguage == null)
                defaultLanguage = filterLanguage(langlist, System.Globalization.CultureInfo.CurrentCulture.NativeName);

            // then try default language
            if (defaultLanguage == null)
            {
                // if default is in languages list (as it should), then take that.
                if (langlist.Contains(DEFAULT_LANGUAGE))
                    defaultLanguage = DEFAULT_LANGUAGE;
                else
                {
                    // default language is not installed; see if the languages have something resembling it
                    // e.g. "English (United States)"
                    foreach (String lang in langlist)
                        if (lang.Contains(DEFAULT_LANGUAGE))
                            defaultLanguage = DEFAULT_LANGUAGE;
                    // if that fails too, default to first language in installed languages list.
                    if (defaultLanguage == null)
                        defaultLanguage = langlist[1];
                }
            }
            return defaultLanguage;
        }

        private static String filterLanguage(String[] languageslist, String userLangAndCountry)
        {
            String userLang = userLangAndCountry;
            if (userLang.Contains('('))
                userLang = userLang.Substring(0, userLang.IndexOf('(')).TrimEnd();

            if (languageslist.Contains(userLangAndCountry))
                return userLangAndCountry;
            if (languageslist.Contains(userLang))
                return userLang;
            return null;
        }

        /// <summary>
        ///     Returns the settings file.
        /// </summary>
        /// <returns>The settings file.</returns>
        public static String getSubjectsFilePath()
        {
            return getProgramSettingsPath() + ProgramConstants.SUBJECT_SETTINGS_FILE_NAME;
        }

        /// <summary>
        ///     Returns the log file.
        /// </summary>
        /// <returns>the log file.</returns>
        public static String getLogFilePath()
        {
            return getProgramSettingsPath() + ProgramConstants.LOG_FILE_NAME;
        }

        /// <summary>
        ///     Returns a soft red color.
        /// </summary>
        /// <returns>A soft red color.</returns>
        public static Color getRed()
        {
            return Color.FromArgb(192, 0, 0);
        }

        /// <summary>
        ///     Returns a soft orange color.
        /// </summary>
        /// <returns>A soft orange color.</returns>
        public static Color getOrange()
        {
            return Color.FromArgb(255, 102, 0);
        }

        /// <summary>
        ///     Returns a soft green color.
        /// </summary>
        /// <returns>A soft green color.</returns>
        public static Color getGreen()
        {
            return Color.FromArgb(0, 192, 0);
        }

        /// <summary>
        ///     Returns all possible encoding options.
        /// </summary>
        /// <returns>A sorted array with all possible encoding options.</returns>
        public static String[] getAllEncodings()
        {
            List<String> encoding = new List<String>();

            foreach (EncodingInfo ei in Encoding.GetEncodings())
            { 
                encoding.Add(ei.Name);
            }

            String[] retVal = encoding.ToArray();

            Array.Sort(retVal);

            return retVal;
        }

    }
}