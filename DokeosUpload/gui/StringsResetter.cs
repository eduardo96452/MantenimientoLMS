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
using System.Linq;
using System.Windows.Forms;
using lmsda.domain;

namespace lmsda.gui
{

    /// <summary>
    ///     Author: Maarten Meuris
    ///     
    ///     This static class examines all UI elements en replaces the Text property by the translation
    ///     of the Tag property. Because language code strings never contains any space character, we've
    ///     added the ability to add extra text after the translated text.
    ///     A string that has to contain fixed text in the front, is to be translated manually.
    /// </summary>
    static class StringsResetter
    {
        /// <summary>
        ///     Replaces all Text properties by the tag property, translated to the specified language.
        /// </summary>
        /// <param name="controls">The ControlCollection that has to be translated.</param>
        public static void resetStaticStrings(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                resetString(ctrl);

                if (ctrl.HasChildren)
                {
                    resetStaticStrings(ctrl.Controls);
                }
            }
        }

        /// <summary>
        ///     Replaces all Text properties by the tag property, translated to the specified language.
        /// </summary>
        /// <param name="items">The ToolStripItemCollection that has to be translated.</param>
        public static void resetStaticStrings(ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                try
                {
                    ToolStripMenuItem menuItem = (ToolStripMenuItem)item;
                    resetString(menuItem);

                    if (menuItem.DropDownItems.Count > 0)
                    {
                        resetStaticStrings(menuItem.DropDownItems);
                    }
                }
                catch { }
            }
        }

        /// <summary>
        ///     Replaces all Text properties by the Tag property, translated to the currently selected language.
        /// </summary>
        /// <param name="form">The form object that has to be translated.</param>
        public static void resetStaticStrings(Form form)
        {
            resetString(form);
            resetStaticStrings(form.Controls);
        }
        
        private static void resetString(Control control)
        {
            if (control.Tag != null && !control.Tag.Equals(String.Empty))
            {
                String tag = Convert.ToString(control.Tag);
                String added = String.Empty;
                if (tag.Contains(' '))
                {
                    added = tag.Substring(tag.IndexOf(' ') + 1);
                    tag = tag.Substring(0, tag.IndexOf(' '));
                }
                control.Text = DomainController.Instance().getLanguageString(tag) + added;
            }
        }

        /// <summary>
        ///     ToolStripMenuItem and Control doesn't implement from the same classes.
        ///     (Tag and Text properties)
        ///     That's why we have to copy the whole method.
        /// </summary>
        /// <param name="control"></param>
        private static void resetString(ToolStripMenuItem control)
        {
            if (control.Tag != null && !control.Tag.Equals(String.Empty))
            {
                String tag = Convert.ToString(control.Tag);
                String added = String.Empty;
                if (tag.Contains(' '))
                {
                    added = tag.Substring(tag.IndexOf(' ') + 1);
                    tag = tag.Substring(0, tag.IndexOf(' '));
                }
                control.Text = DomainController.Instance().getLanguageString(tag) + added;
            }
        }
    }
}
