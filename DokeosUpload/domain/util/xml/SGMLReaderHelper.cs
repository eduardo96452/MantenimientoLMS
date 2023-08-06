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
using Sgml;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace lmsda.domain.util.xml
{ 
    /// <summary>
    ///     Author: Maarten Meuris <br />
    ///     Source of DLL: http://developer.mindtouch.com/SgmlReader <br />
    ///     Helper class to allow string processing using SGMLReader/Parser
    /// </summary> 
    public static class SGMLReaderHelper
    {
        /// <summary>
        ///     Converts a HTML string to a valid XmlDocument.
        /// </summary>
        /// <param name="strInputHtml">The HTML string.</param>
        /// <returns>A new valid XmlDocument.</returns>
        public static XmlDocument htmlToXmlDocument(String strInputHtml)
        {
            if (strInputHtml.IndexOf("<html",StringComparison.InvariantCultureIgnoreCase) > 0)
                strInputHtml = strInputHtml.Substring(strInputHtml.IndexOf("<html",StringComparison.InvariantCultureIgnoreCase));
            strInputHtml = cleanupCdata(strInputHtml);
            SgmlReader reader = new SgmlReader();
            reader.DocType = "HTML";
            StringReader sr = new System.IO.StringReader(strInputHtml);
            reader.InputStream = sr;
            XmlDocument xm = new XmlDocument();
            reader.Read();
            xm.Load(reader);
            return xm;
        }

        /// <summary>
        ///     Strips all HTML from a string.
        /// </summary>
        /// <param name="strInputHtml">A string that contains HTML tags.</param>
        /// <returns>A string without any HTML tags.</returns>
        public static String stripHtmlContent(String strInputHtml)
        {
            XmlDocument xm = htmlToXmlDocument("<html>" + strInputHtml + "</html>");
            return xm.InnerText;
        }

        /// <summary>
        ///     Converts a HTML string to a valid XHTML String.
        /// </summary>
        /// <param name="strInputHtml">The HTML string.</param>
        /// <returns>A new valid XHTML string.</returns>
        public static String htmlToXhtmlString(String strInputHtml)
		{
            strInputHtml = cleanupCdata(strInputHtml);
    		String strOutputXhtml = String.Empty;
    		SgmlReader reader = new SgmlReader();		
    		reader.DocType ="HTML";
    		StringReader sr = new System.IO.StringReader(strInputHtml);	 
    		reader.InputStream = sr;
    		StringWriter sw = new StringWriter();
    		XmlTextWriter w =new XmlTextWriter( sw);
    		reader.Read();
    		while(!reader.EOF)
    		{
    			w.WriteNode(reader,true);
    		} 		
    		w.Flush();
    		w.Close();	
    		return sw.ToString();
       }

        /// <summary>
        ///     Removes all CDATA from the input string.
        /// </summary>
        /// <param name="strInputHtml">The input string.</param>
        /// <returns>The inputstring, without any CDATA.</returns>
        private static String cleanupCdata(String strInputHtml)
        {
            // remove escaped cdata
            strInputHtml = Regex.Replace(strInputHtml, Regex.Escape("/*<![CDATA[*/") + ".*?" + Regex.Escape("/*]]>*/"), "");
            strInputHtml = strInputHtml.Replace("<![CDATA[","<!--");
            strInputHtml = strInputHtml.Replace("]]>","-->");
            return strInputHtml;
        }
    }
}