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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lmsda.domain;
using System.IO;

namespace lmsda.gui
{
    public partial class FirstRunFrame : Form
    {
        private DomainController dc;

        public FirstRunFrame()
        {
            dc = DomainController.Instance();
            InitializeComponent();
            if (PortableIdentifier.Instance().isPortable)
                chkCreateMySubjectsStats.Tag = "create_mysubjects_stats_standalone";
        }

        private void FirstRunFrame_Load(object sender, EventArgs e)
        {
            String[] languages = dc.getLanguages();

            cmbResources.DataSource = languages;
            String language = dc.getSettings().getLanguage();
            if (!language.Equals(String.Empty) && languages.Contains(language))
                cmbResources.SelectedIndex = cmbResources.Items.IndexOf(language);
            else if (dc.getLanguages().Length > 0)
                cmbResources.SelectedIndex = 0;
            else 
                cmbResources.SelectedIndex = -1;

            setStrings();
        }

        private void cmbResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            DomainController.Instance().selectLanguage(this.cmbResources.SelectedItem.ToString(),false);
            setStrings();
        }

        /// <summary>
        ///     Method to adapt the language of the entire UI
        /// </summary>
        private void setStrings()
        {
            StringsResetter.resetStaticStrings(this);
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (chkCreateMySubjectsStats.Checked)
            {
                dc.createUserFolders();
            }
            this.Dispose();
        }

        private void FirstRunFrame_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
