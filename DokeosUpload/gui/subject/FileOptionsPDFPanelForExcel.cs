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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lmsda.domain.user.synchronization;

namespace lmsda.gui.subject
{
    partial class FileOptionsPDFPanelForExcel : UserControl
    {
        #pragma warning disable //disables "not used" message
        private Boolean loading = false;
        #pragma warning restore
        private String relativeFileName = String.Empty;
        private SubjectFilesSettingsControl parent;

        public FileOptionsPDFPanelForExcel()
        {
            InitializeComponent();
        }

        private void FileOptionsPDFPanelForExcel_Load(object sender, EventArgs e)
        {
            resetStrings();
        }

        public void setParent(SubjectFilesSettingsControl parent)
        {
            this.parent = parent;
        }

        public void resetStrings()
        {
            //not used.
        }

        public void update(FileSettings selectedFile)
        {
            this.relativeFileName = selectedFile.relativeFileName;
            this.loading = true;

            //No options to read... This can be expanded later.

            this.loading = false;
        }
    }
}
