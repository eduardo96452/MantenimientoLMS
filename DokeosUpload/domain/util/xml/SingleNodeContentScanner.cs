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
using System.Xml;

namespace lmsda.domain.util.xml
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    ///     Used for scanning a single node.
    /// </summary>
    class SingleNodeContentScanner : XmlScanner
    {
        private List<XmlNode> results;
        private String nodeName;
        private String searchString;
        private Boolean findAll;
        private ScanMode scanmode;
        private String truncation;

        #region Constructor

        /// <summary>
        ///     Finds a node or set of nodes based on the text content inside the tag
        /// </summary>
        /// <param name="nodeName">Name of the node to search for</param>
        /// <param name="findAll">find all occurences or only one</param>
        /// <param name="searchString">the content to search for</param>
        /// <param name="contains">false if the string has to be an exact match of the contents</param>
        public SingleNodeContentScanner(String nodeName, Boolean findAll, String searchString, Boolean contains)
        {
            this.scanmode = (contains ? ScanMode.CONTAINS : ScanMode.EQUALS);
            this.findAll = findAll;
            this.nodeName = nodeName;
            this.searchString = searchString;
            results = new List<XmlNode>();
        }


        /// <summary>
        ///     Finds a node or set of nodes based on the text content inside the tag.
        ///     If the scanmode is SAMETRUNCATEDSTART, the default truncation string
        ///     is the "…" character.
        /// </summary>
        /// <param name="nodeName">Name of the node to search for</param>
        /// <param name="findAll">find all occurences or only one</param>
        /// <param name="searchString">the content to search for</param>
        /// <param name="scanmode">Scan mode to use to find string</param>
        public SingleNodeContentScanner(String nodeName, Boolean findAll, String searchString, ScanMode scanmode)
            : this(nodeName, findAll, searchString, scanmode, "…")
        { }

        /// <summary>
        ///     Finds a node or set of nodes based on the text content inside the tag,
        ///     using the SAMETRUNCATEDSTART scan mode, with the specified truncation string.
        /// </summary>
        /// <param name="nodeName">Name of the node to search for</param>
        /// <param name="findAll">find all occurences or only one</param>
        /// <param name="searchString">the content to search for</param>
        /// <param name="truncation">String used to truncate node text.</param>
        public SingleNodeContentScanner(String nodeName, Boolean findAll, String searchString, String truncation)
            : this(nodeName, findAll, searchString, ScanMode.SAMETRUNCATEDSTART, truncation)
        { }

        /// <summary>
        ///     Finds a node or set of nodes based on the text content inside the tag
        /// </summary>
        /// <param name="nodeName">Name of the node to search for</param>
        /// <param name="findAll">find all occurences or only one</param>
        /// <param name="searchString">the content to search for</param>
        /// <param name="scanmode">Scan mode to use to find string</param>
        /// <param name="truncation">String used to truncate node text.</param>
        public SingleNodeContentScanner(String nodeName, Boolean findAll, String searchString, ScanMode scanmode, String truncation)
        {
            this.truncation = truncation;
            this.scanmode = scanmode;
            this.findAll = findAll;
            this.nodeName = nodeName;
            this.searchString = searchString;
            results = new List<XmlNode>();
        }


        #endregion

        #region Overrides

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals(this.nodeName) || this.nodeName == null)
            {
                String text = node.InnerXml;
                if (text != null)
                {
                    switch(scanmode)
                    {
                        case ScanMode.EQUALS: 
                            if (text.Equals(searchString))
                                addResult(node);
                            break;
                        case ScanMode.CONTAINS:
                            if (text.Contains(searchString))
                                addResult(node);
                            break;
                        case ScanMode.SAMETRUNCATEDSTART:
                            if (text.Equals(searchString) || Utility.isTruncatedVersionOf(text,searchString,truncation))
                                addResult(node);
                            break;
                    }
                }
            }
        }


        private void addResult(XmlNode node)
        {
            this.results.Add(node);
            cancelLoop = !findAll;
            ignoreChildren = true;
        }

        #endregion

        #region Getters and setters

        /// <summary>
        ///     Returns the node.
        /// </summary>
        /// <returns>the node.</returns>
        public XmlNode getNode()
        {
            if (this.results.Count > 0)
                return this.results[0];
            else return null;
        }

        /// <summary>
        ///     Return the list of nodes.
        /// </summary>
        /// <returns>The list of nodes.</returns>
        public List<XmlNode> getNodes()
        {
            return this.results;
        }

        #endregion
    }
}
