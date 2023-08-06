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
    class SingleNodeScanner : XmlScanner
    {
        private List<XmlNode> results;
        private String nodeName;
        private List<String[]> nameValuePairs;
        private Boolean findAll;
        private Boolean contains;

        #region Constructor

        /// <summary>
        ///     Initialize a scanner to look for one node
        /// </summary>
        /// <param name="nodeName">The node name.</param>
        public SingleNodeScanner(String nodeName)
        {
            this.nodeName = nodeName;
            this.nameValuePairs = new List<String[]>();
            results = new List<XmlNode>();
        }

        /// <summary>
        ///     Initialize a scanner to look for one or more nodes, with extra options
        /// </summary>
        /// <param name="nodeName">The node name.</param>
        /// <param name="findAll">True if the scan has to find all attributes.</param>
        /// <param name="contains">false if the string has to be an exact match of the contents</param>
        public SingleNodeScanner(String nodeName, Boolean findAll, Boolean contains)
            :this(nodeName)
        {
            this.findAll = findAll;
            this.contains = contains;
        }

        #endregion

        #region User actions

        /// <summary>
        ///     Adds a new pair to the list.
        /// </summary>
        /// <param name="attribute">The attribute name.</param>
        /// <param name="value">The value of the attribute.</param>
        public void addNameValuePair(String attribute, String value)
        {
            if(attribute!= null || value!= null)
                this.nameValuePairs.Add(new String[]{attribute, value});
        }

        #endregion

        #region Overrides

        protected override void examineNode(XmlNode node)
        {
            if (node.Name.Equals(this.nodeName) || this.nodeName == null)
            {
                int nodeCriteriaFound = 0;

                foreach (String[] nameValuePair in this.nameValuePairs)
                {
                    if (nameValuePair[0] != null && node.Attributes[nameValuePair[0]] != null)
                    {
                        // node with correct attribute found
                        if (nameValuePair[1] == null)
                        {
                            nodeCriteriaFound++;
                        }
                        else if (node.Attributes[nameValuePair[0]].Value.Equals(nameValuePair[1]) ||
                                 (contains && node.Attributes[nameValuePair[0]].Value.Contains(nameValuePair[1])))
                        {
                            nodeCriteriaFound++;
                        }
                    }
                }
                if (nodeCriteriaFound == this.nameValuePairs.Count)
                {
                    this.results.Add(node);
                    cancelLoop = !findAll;
                    ignoreChildren=true;
                }
            }
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
