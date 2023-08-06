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
using System.IO;

namespace lmsda.persistence.resource
{
    /// <summary>
    ///     Author: Maarten Meuris.
    ///     This class loads the selected language.
    /// </summary>
    class ResourceLoader
    {
        private String basePath;
        private String resourceName;
        private String resourceExtension;
        private Dictionary<String, String> resourceStrings;

        /// <summary>
        ///     Default constructor. Loads all strings of the selected language.
        /// </summary>
        /// <param name="basePath">The path to the folder that contains the language file.</param>
        /// <param name="resourceName">The name of the language (without extension).</param>
        /// <param name="resourceExtension">The extension of the language file.</param>
        public ResourceLoader(String basePath, String resourceName, String resourceExtension)
        {
            this.basePath = basePath;
            this.resourceName = resourceName;
            this.resourceExtension = resourceExtension;
            fillDictionary();
        }

        /// <summary>
        ///     Returns a string that represents the translation in the selected language of the given code string.
        /// </summary>
        /// <param name="identifier">The code string.</param>
        /// <returns>The string representing the translation.</returns>
        public String getString(String identifier)
        {
            if(identifier == null) return "No ID given";
            if (resourceStrings == null) return "[==" + identifier + "==]";
            else
            {
                String output = null;
                if (resourceStrings.TryGetValue(identifier.ToLower(), out output))
                    return output;
                else return "[==" + identifier + "==]";
            }
        }

        /// <summary>
        ///     Returns a string that represents the translation in the selected language of the given code string, with the ability of arguments.
        ///     Strings "$0", "$1", etc. are replaced by the given arguments.
        /// </summary>
        /// <param name="identifier">The code.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>The string representing the translation.</returns>
        /// <remarks>
        ///     Last updated on 11/03/2011 by Maarten Meuris
        ///      -> If there are no parameters in a string, arguments are now simply pasted behind it, separated by spaces.
        /// </remarks>
        public String getString(String identifier, String[] args)
        {
            String returnstring = getString(identifier);
            if (args != null)
            {
                for (int i = args.Length-1; i >= 0; i--)
                {
                    String paramvalue = "$" + i;
                    int paramindex = returnstring.IndexOf(paramvalue);
                    if (paramindex >= 0)
                    {
                        returnstring = returnstring.Substring(0, paramindex)
                                        + args[i]
                                        + returnstring.Substring(paramindex + paramvalue.Length);
                    }
                    else returnstring = returnstring + " " + args[i];

                }
            }
            return returnstring;
        }

        /// <summary>
        ///     Loads the language file.
        /// </summary>
        private void fillDictionary()
        {
            try
            {
                resourceStrings = new Dictionary<String, String>();
                StreamReader reader = File.OpenText(this.basePath + this.resourceName + this.resourceExtension);
                String input = null;
                String key;
                String value;

                while ((input = reader.ReadLine()) != null)
                {
                    if (input.IndexOf('=') != -1 && input[0] != ';')
                    {
                        key = input.Substring(0, input.IndexOf('=')).ToLower();
                        value = input.Substring(input.IndexOf('=') + 1).Replace("\\n",Environment.NewLine);
                        try
                        {
                            resourceStrings.Add(key, value);
                        }
                        catch (Exception)
                        { 
                             // geeft problemen bij startup omdat ResourceLoader wordt aangeroepen vanuit de constructor
                             // van DomainController
                            //DomainController.Instance().writeToLog("Error: duplicate key in resource file ("+key+")",false,true);
                        }
                    }
                }
                reader.Close();
            }
            catch (Exception)
            { 
                //DomainController.Instance().writeToLog("Loading of resources failed",true,true);
            }
        }
    }
}
