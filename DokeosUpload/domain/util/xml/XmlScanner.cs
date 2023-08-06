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
using System.Xml;

namespace lmsda.domain.util.xml
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    ///     Scanner class for XML nodes.
    /// </summary>
    abstract class XmlScanner
    {
        /// <summary>
        ///     During the scan, the methods will check the cancelLoop variable. 
        ///     If true, the scan will abort.
        /// </summary>
        protected Boolean cancelLoop = false;

        /// <summary>
        ///     If true, the scan will ignore its children nodes.
        /// </summary>
        protected Boolean ignoreChildren = false;

        /// <summary>
        ///     Recursively scans all XHTML nodes.
        ///     To stop a scan, set the cancelLoop flag to "true".
        ///     To ignore children nodes, set the ignoreChildren flas to "true".
        /// </summary>
        /// <param name="nodes">A nodes list.</param>
        protected void scanXML(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if(cancelLoop) return;
                this.examineNode(node);
                if(cancelLoop) return;
                if (!ignoreChildren)
                {
                    if (node.HasChildNodes)
                        this.scanXML(node.ChildNodes);
                }
                else
                { 
                    ignoreChildren=false;
                }

            }
        }
        
        /// <summary>
        ///     Recursively scans all XHTML nodes.
        ///     Virtual = subclasses can override this method, but they can just 'talk' to this one too.
        ///     Overrides always have to call scanXML.
        /// </summary>
        /// <param name="nodes">A nodes list.</param>
        public virtual void doScan(XmlNodeList nodes)
        {
            cancelLoop=false;
            this.scanXML(nodes);
        }

        /// <summary>
        ///     Inspects one single node. This function determines what to scan for,
        ///     and is defined in the implementation class.
        /// </summary>
        /// <param name="node">The node.</param>
        protected abstract void examineNode(XmlNode node);

        public XmlScanner()
        {
        }
    }
}
