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
using lmsda.domain.util.xml;
using lmsda.domain.util;

namespace lmsda.persistence.platform.chamilo_2_0.post
{
    /// <summary>
    ///     Author: Gianni Van Hoecke, Maarten Meuris
    /// </summary>
    class FolderScanner : XmlScanner
    {
        private const string CATEGORY_CLASS = "category";
        private List<String[]> folders;

        /// <summary>
        ///     Default constructor. Creates an empty list of folders.
        /// </summary>
        public FolderScanner()
        { 
            folders = new List<String[]>();
        }

        public override void doScan(XmlNodeList nodes)
        {
            folders = new List<String[]>();
            base.doScan(nodes);
        }

        protected override void examineNode(XmlNode node)
        {
            if(node.Name.Equals("ul") && node.Attributes["class"] != null && node.Attributes["class"].Value.Equals("tree-menu"))
            {
            	//We have the tree. Now examine it.
            	node = node.SelectSingleNode("li");

            	//Get the root folder first...
            	String folderName = this.extractNode(node, String.Empty, true);

            	//Now get the sub folders...
            	if(this.containsNodeName(node.ChildNodes, "ul"))
            		this.examineul(node.ChildNodes, folderName);
                
                // folders are scanned; abort.
                cancelLoop = true;
            }
        }

        /// <summary>
        ///     Checks if the specified node is present in the given XmlNodeList.
        /// </summary>
        /// <param name="nodes">The XmlNodeList.</param>
        /// <param name="nodeName">The node name.</param>
        /// <returns>True if the XmlNodeList contains the specified node name.</returns>
        private Boolean containsNodeName(XmlNodeList nodes, String nodeName)
        {
            foreach (XmlNode node in nodes)
            {
                if(node.Name.Equals(nodeName))
                    return true;
            }
            return false;
        }

        /// <summary>
        ///     Iterates through all nodes. For all 'ul' nodes, this method will call 
        ///     the 'examineli' method.
        /// </summary>
        /// <param name="nodes">The XmlNodeList.</param>
        /// <param name="currentFolder">The current folder.</param>
        private void examineul(XmlNodeList nodes, String currentFolder)
        {	
        	foreach(XmlNode node in nodes)
        	{
        		if(node.Name.Equals("ul"))
        		{
        			this.examineli(node.ChildNodes, currentFolder);
        		}
        	}
        }

        /// <summary>
        ///     Iterates through all nodes. For all 'li' nodes, this method will call 
        ///     the 'extractNodeName' method. Afterwards, if the 'li' node contains one or more 
        ///     'ul' node(s), the 'examineul' method will be called.
        /// </summary>
        /// <param name="nodes">The XmlNodeList.</param>
        /// <param name="currentFolder">The current folder.</param>
        private void examineli(XmlNodeList nodes, String currentFolder)
        {
        	foreach(XmlNode node in nodes)
        	{
        		if(node.Name.Equals("li"))
        		{
        			String folder = this.extractNode(node, currentFolder, false);
        			if(this.containsNodeName(node.ChildNodes, "ul"))
        				this.examineul(node.ChildNodes, folder);
        		}
        	}
        }

        /// <summary>
        ///     This method will examine an 'li' node. It selects the first 'div/a' node and extracts 
        ///     the necessary information from it.
        /// </summary>
        /// <param name="node">The 'li' node.</param>
        /// <param name="currentFolder">The current folder.</param>
        /// <param name="isRootNode">Indicates whether the given node is a root node.</param>
        /// <returns>The current folder plus the name of this node (separated with a slash).</returns>
        private String extractNode(XmlNode node, String currentFolder, Boolean isRootNode)
        {
        	node = node.SelectSingleNode("div/a");

        	if(node.Attributes["class"] != null && node.Attributes["class"].Value.Equals(CATEGORY_CLASS))
        	{
        		String folderName = node.InnerText;
                // filters slashes from the read directory name; a slash inside a single folder name
                // is not supported in our system since it is used as folder separator.
                folderName = folderName.Replace("/", "~!~");
		        currentFolder += "/" + (isRootNode ? String.Empty : folderName);
                String currentId = node.Attributes["id"].Value;
                // failsafe in case the (apparently useless) "id" element is ever removed.
                if (currentId == null || currentId.Equals(String.Empty))
                {
                    String link = node.Attributes["href"].Value;
                    if (link != null)
                        currentId = Utility.findValueInURL(node.Attributes["href"].Value, "category_id");
                }
                folders.Add(new String[]{ "/" + currentFolder.TrimStart('/'), node.Attributes["id"].Value });
        	}
        	
        	return currentFolder;
        }

        /// <summary>
        ///     Returns the list with folders, in the format String[folderName,folderId].
        /// </summary>
        /// <returns>The list with folders.</returns>
        public List<String[]> getFolders()
        {
            return this.folders;
        }
    }
}
