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
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using lmsda.domain;
using lmsda.domain.user.synchronization;
using System.Drawing.Imaging;
using lmsda.domain.util;

namespace lmsda.gui.treeview
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    class TreeViewExplorer : TreeView
    {
        private ImageList imageList = new ImageList();
        private Hashtable systemIcons = new Hashtable();
        public static readonly int Folder = 0;
        private TreeNode rootNode;
        private String startPath;
        private DomainController dc;
        private Boolean containsNewCourse;
        private Boolean containsSyncedCourses;

        /// <summary>
        ///     Default constructor. 
        /// </summary>
        public TreeViewExplorer()
        {
            this.HideSelection = false;
        }

        /// <summary>
        ///     Loads the tree, starting at a specified path.
        /// </summary>
        /// <param name="startPath">The path as root node.</param>
        public void loadTree(String startPath)
        {
            if (!new DirectoryInfo(startPath).Exists) return;
            dc = DomainController.Instance();
            this.containsNewCourse = dc.getSynchronizationOperations().containsNewCourse();
            this.containsSyncedCourses = dc.getSynchronizationOperations().containsSyncedCourses();
            this.startPath = startPath.TrimEnd('\\');
            this.ImageList = imageList;
            this.rootNode = new TreeNode();
            SynchronizationStatus sync = dc.getSynchronizationOperations().getFileStatus("\\");
            this.setNodeValue(this.rootNode, new FileInfo(this.startPath), sync, true);
            this.Nodes.Add(this.rootNode);
            this.populateTree(new DirectoryInfo(this.rootNode.Name), this.rootNode);
            this.expandTree(this.Nodes);
        }

        /// <summary>
        ///     Expands the TreeView based on settings.
        /// </summary>
        /// <param name="theNodes">The root node to expand. (recursive)</param>
        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        /// </remarks>
        private void expandTree(TreeNodeCollection theNodes)
        {
            foreach (TreeNode node in theNodes)
            {
                if (node.Nodes.Count > 0)
                {
                    //This node is a directory because it has child nodes.
                    String fullPath = "\\" + node.FullPath.Substring(this.Nodes[0].FullPath.Length).TrimStart('\\');
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(fullPath);
                    if (fs.isExpanded)
                        node.Expand();
                    else
                        node.Collapse();
                    this.expandTree(node.Nodes);
                }
            }
        }

        /// <summary>
        ///     Updates the FileSettings objects.
        /// </summary>
        /// <param name="theNodes">The root node. (recursive)</param>
        /// <remarks>
        ///     As of 1.08
        ///     
        ///     Last updated on 14/08/2010 by Gianni Van Hoecke
        /// </remarks>
        public void updateExpandedSettings(TreeNodeCollection theNodes)
        {
            foreach (TreeNode node in theNodes)
            {
                if (node.Nodes.Count > 0)
                {
                    //This node is a directory because it has child nodes.
                    String fullPath = "\\" + node.FullPath.Substring(this.Nodes[0].FullPath.Length).TrimStart('\\');
                    FileSettings fs = DomainController.Instance().getSynchronizationOperations().getFileSettings(fullPath);
                    fs.isExpanded = node.IsExpanded;
                    this.updateExpandedSettings(node.Nodes);
                }
            }
        }

        /// <summary>
        ///     Recursive method. Fills the tree with files and folders.
        /// </summary>
        /// <param name="dir">The directory to examine.</param>
        /// <param name="parentNode">Add the contents of the dir to this node.</param>
        private void populateTree(DirectoryInfo dir, TreeNode parentNode)
        {
            //List all directories...
            DirectoryInfo[] subDirs = dir.GetDirectories();
            int startpathlength = this.startPath.Length;

            foreach (DirectoryInfo di in subDirs)
            {
                //Add node
                TreeNode dirNode = new TreeNode();
                SynchronizationStatus sync = dc.getSynchronizationOperations().getFileStatus(di.FullName.Substring(startpathlength));
                this.setNodeValue(dirNode, new FileInfo(di.FullName), sync, true);
                parentNode.Nodes.Add(dirNode);

                //Call this again...
                this.populateTree(di, dirNode);
            }

            //List all files...
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                SynchronizationStatus sync = dc.getSynchronizationOperations().getFileStatus(file.FullName.Substring(startpathlength));
                //Add node
                if((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden // bit compare
                    && !file.FullName.Contains(ProgramConstants.SYNCHRONIZATION_FILE_NAME)
                    && sync != SynchronizationStatus.FILE_CONVERTEDPDF)
                {
                    TreeNode fileNode = new TreeNode();
                    this.setNodeValue(fileNode, file, sync, false);
                    parentNode.Nodes.Add(fileNode);
                }
            }
        }

        /// <summary>
        ///     Sets the value of a node.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="file">The file.</param>
        /// <param name="sync">The synchronization status.</param>
        /// <param name="isFolder">True if the file is actually a folder.</param>
        private void setNodeValue(TreeNode node, FileInfo file, SynchronizationStatus sync, Boolean isFolder)
        {
            node.Name = file.FullName;
            node.Text = file.Name;
            node.ImageIndex = this.getIconImageIndex(node.Name, sync, isFolder, false);
            node.SelectedImageIndex = node.ImageIndex;
        }

        /// <summary>
        ///     Source (icons lazy load): http://www.codingforums.com/showthread.php?t=98374
        ///     Source (image to greyscale): http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        ///     Adapted for status icons.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The icon.</returns>
        public int getIconIndex(String path, SynchronizationStatus sync, Boolean isFolder)
        {
            return getIconImageIndex(path, sync, isFolder, true);
        }

        /// <summary>    
        ///     Source (icons lazy load): http://www.codingforums.com/showthread.php?t=98374
        ///     Source (image to greyscale): http://www.switchonthecode.com/tutorials/csharp-tutorial-convert-a-color-image-to-grayscale
        ///     Adapted for status icons
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The icon.</returns>
        private int getIconImageIndex(String path, SynchronizationStatus sync, Boolean isFolder, Boolean externalcall)
        {
            String extension = (isFolder? "*folder*" : Path.GetExtension(path)+"*");
            
            if  ((sync == SynchronizationStatus.FILE_SYNCHRONYZED || sync == SynchronizationStatus.FILE_CHANGED)
                    && !this.containsSyncedCourses)
                extension+="_" + (int)(SynchronizationStatus.FILE_ADDED);
            else
                extension+="_" + (int)sync;
            if  ((sync == SynchronizationStatus.FILE_SYNCHRONYZED || sync == SynchronizationStatus.FILE_CHANGED)
                    && this.containsNewCourse)
                extension+="_*";

            if (externalcall)
                extension="new_" + extension;

            if (!systemIcons.ContainsKey(extension))
            {
                Rectangle rect = new Rectangle(0, 0, 16, 16);
                Bitmap icon = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(icon);
                if (sync != SynchronizationStatus.FILE_EXCLUDED && sync != SynchronizationStatus.FOLDER_EXCLUDE)
                    g.DrawIconUnstretched(ShellIcon.GetSmallIcon(path), rect);
                else
                {
                    Icon shellicon = ShellIcon.GetSmallIcon(path);
                    g.DrawImage(shellicon.ToBitmap(), rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, Utility.getDarkGreyMask());
                }
                switch (sync)
                {
                    case SynchronizationStatus.FOLDER_UPLOAD:
                        g.DrawImageUnscaledAndClipped(Properties.Resources.sync_upload, rect);
                        break;
                    case SynchronizationStatus.FILE_ADDED:
                        g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added, rect);
                        break;
                    case SynchronizationStatus.FILE_CHANGED:
                        if (!DomainController.Instance().getSynchronizationOperations().containsSyncedCourses())
                        {
                            g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added, rect);
                        }
                        else
                        {
                            g.DrawImageUnscaledAndClipped(Properties.Resources.sync_changed, rect);
                            if (DomainController.Instance().getSynchronizationOperations().containsNewCourse())
                            {
                                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added_small, rect);
                            }
                        }

                        break;
                    case SynchronizationStatus.FILE_SYNCHRONYZED:
                        if (!DomainController.Instance().getSynchronizationOperations().containsSyncedCourses())
                        {
                            g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added, rect);
                        }
                        else
                        {
                            g.DrawImageUnscaledAndClipped(Properties.Resources.sync_done, rect);
                            if (DomainController.Instance().getSynchronizationOperations().containsNewCourse())
                            {
                                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added_small, rect);
                            }
                        }
                        break;
                    case SynchronizationStatus.FILE_ERROR:
                        g.DrawImageUnscaledAndClipped(Properties.Resources.sync_data_error, rect);
                        break;
                }
                if (externalcall)
                    g.DrawImageUnscaledAndClipped(Properties.Resources.sync_edited, rect);

                g.Flush();

                imageList.Images.Add(icon);
                systemIcons.Add(extension, imageList.Images.Count - 1);
            }

            return (int)systemIcons[extension];
        }
    }
}
