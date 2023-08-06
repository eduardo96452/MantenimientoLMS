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

namespace lmsda.gui.treeview
{
    partial class TreeViewExplorerPanel
    {
        /// <summary> 
        ///     Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeViewExplorer = new lmsda.gui.treeview.TreeViewExplorer();
            this.rightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SuspendLayout();
            // 
            // treeViewExplorer
            // 
            this.treeViewExplorer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewExplorer.HideSelection = false;
            this.treeViewExplorer.Location = new System.Drawing.Point(0, 0);
            this.treeViewExplorer.Name = "treeViewExplorer";
            this.treeViewExplorer.Size = new System.Drawing.Size(100, 100);
            this.treeViewExplorer.TabIndex = 0;
            this.treeViewExplorer.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewExplorer_AfterCollapse);
            this.treeViewExplorer.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewExplorer_AfterExpand);
            this.treeViewExplorer.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewExplorer_AfterSelect);
            this.treeViewExplorer.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewExplorer_MouseDoubleClick);
            this.treeViewExplorer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewExplorer_MouseDown);
            // 
            // rightClickMenu
            // 
            this.rightClickMenu.Name = "contextMenuStrip1";
            this.rightClickMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // TreeViewExplorerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeViewExplorer);
            this.Name = "TreeViewExplorerPanel";
            this.Size = new System.Drawing.Size(100, 100);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip rightClickMenu;
    }
}
