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
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using lmsda.domain.util;
using lmsda.domain;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Gianni Van Hoecke
    ///             Maarten Meuris
    /// </summary>
    partial class AboutFrame : Form
    {
        public AboutFrame()
        {
            InitializeComponent();
            this.Text = String.Format("About {0}", AssemblyTitle());
            this.labelProductName.Text = AssemblyTitle();
            this.labelVersion.Text = String.Format("Version {0}", Application.ProductVersion);
            this.labelCopyright.Text = AssemblyCopyright();
            this.labelCompanyName.Text = AssemblyCompany();
            this.labelURL.Text = lmsda.domain.ProgramConstants.WEBSITE_LINK;
            this.textBoxDescription.Text = AssemblyDescription();
        }

        #region Assembly Attribute Accessors

        private String AssemblyTitle()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != "")
                {
                    return titleAttribute.Title;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        }

        private String AssemblyVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        private String AssemblyDescription()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        }

        private String AssemblyProduct()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }

        private String AssemblyCopyright()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }

        private String AssemblyCompany()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void labelURL_Click(object sender, EventArgs e)
        {
            Utility.StartWithShell(ProgramConstants.WEBSITE_LINK);
        }

        private void labelURL_MouseEnter(object sender, EventArgs e)
        {
            this.labelURL.Font = new Font(this.labelURL.Font, FontStyle.Underline);
            
        }

        private void labelURL_MouseLeave(object sender, EventArgs e)
        {
            this.labelURL.Font = new Font(this.labelURL.Font, FontStyle.Regular);
        }
    }
}
