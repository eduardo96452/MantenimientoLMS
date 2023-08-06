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
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Maarten Meuris
    /// </summary>
    public partial class SynchronizeHelpFrame : Form
    {
        public SynchronizeHelpFrame()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.lmsda_icon;
            StringsResetter.resetStaticStrings(this);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void SynchronizeHelpFrame_Load(object sender, EventArgs e)
        {
            Boolean showAddedCoursesInfo = false;

            if (showAddedCoursesInfo)
            {
                lblAccoladeCourseAdded.Visible = true;
                lblTextCourseAdded.Visible = true;
                Rectangle rect = new Rectangle(0, 0, 16, 16);
                Bitmap icon = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
                Graphics g = Graphics.FromImage(icon);
                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_done, rect);
                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added_small, rect);
                g.Flush();
                lblIconCourseAddedSynced.Image = icon;
                lblIconCourseAddedSynced.Visible = true;

                rect = new Rectangle(0, 0, 16, 16);
                icon = new Bitmap(16, 16, PixelFormat.Format32bppArgb);
                g = Graphics.FromImage(icon);
                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_changed, rect);
                g.DrawImageUnscaledAndClipped(Properties.Resources.sync_added_small, rect);
                g.Flush();
                lblIconCourseAddedModified.Image = icon;
                lblIconCourseAddedModified.Visible = true;
            }
        }
    }
}
