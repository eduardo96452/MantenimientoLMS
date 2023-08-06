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
using System.Threading;
using lmsda.domain.util;

namespace lmsda.gui
{
    partial class UpdateFrame : Form
    {
        public delegate void invoke_delegate_set_text(String text);
        //As of 1.09
        public delegate void invoke_delegate();

        public UpdateFrame()
        {
            InitializeComponent();
            this.resetStrings();
            this.updateChangeLog();
        }

        private void resetStrings()
        {
            //Set in user language
            StringsResetter.resetStaticStrings(this);
            this.lblInfo.Text = DomainController.Instance().getAppVersionInfo();
        }

        private void updateChangeLog()
        {
            this.txtChangeLog.Text = DomainController.Instance().getLanguageString("loading");
            Thread thread = new Thread(new ThreadStart(getChangeLog));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void getChangeLog()
        {
            String text = DomainController.Instance().getChangeLogOfLatestVersion();
            this.txtChangeLog.Invoke(new invoke_delegate_set_text(setChangeLogText), text);
            //As of 1.09
            this.Invoke(new invoke_delegate(processPrintButton));
        }

        private void setChangeLogText(String text)
        {
            this.txtChangeLog.Text = text;
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmdUpdate_Click(object sender, EventArgs e)
        {
            DomainController.Instance().goToUpdateSource();
            this.Dispose();
        }

        private void cmdPrint_Click(object sender, EventArgs e)
        {
            PrintDialog printDlg = new PrintDialog();
            printDlg.AllowSelection = true;
            printDlg.AllowSomePages = true;
            printDlg.Document = printer;
            printer.DocumentName = "LMS Desktop Assistan Changelog";
            if (printDlg.ShowDialog() == DialogResult.OK)
                printer.Print();
        }

        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        /// </remarks>
        private void processPrintButton()
        {
            if (this.txtChangeLog.Text.Equals(DomainController.Instance().getLanguageString("cannot_load_change_log")))
            {
                this.cmdPrint.Enabled = false;
            }
            else
            {
                this.cmdPrint.Enabled = true;
            }
        }

        /// <remarks>
        ///	    As of 1.09
        ///     Last updated on 13/09/2010 by Gianni Van Hoecke
        /// </remarks>
        private void printer_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float printHeight = printer.DefaultPageSettings.PaperSize.Height - printer.DefaultPageSettings.Margins.Top - printer.DefaultPageSettings.Margins.Bottom;
            float printWidth = printer.DefaultPageSettings.PaperSize.Width - printer.DefaultPageSettings.Margins.Left - printer.DefaultPageSettings.Margins.Right;
            float leftMargin = printer.DefaultPageSettings.Margins.Left;  //X
            float rightMargin = printer.DefaultPageSettings.Margins.Top;  //Y
            RectangleF printArea = new RectangleF(leftMargin, rightMargin, printWidth, printHeight);

            Graphics g = e.Graphics;
            SolidBrush Brush = new SolidBrush(Color.Black);
            g.DrawString(this.txtChangeLog.Text, new Font("Courier New", 10), Brush, printArea);
        }
    }
}
