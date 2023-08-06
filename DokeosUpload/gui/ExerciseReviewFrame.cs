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
using System.Drawing;
using System.Windows.Forms;
using lmsda.domain;
using System.Threading;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Maarten Meuris
    /// </summary>
    public partial class ExerciseReviewFrame : Form
    {
        private TooltipChecker tooltipChecker;
        private int errornumber;

        public delegate void invoke_delegate_single_parameter(Control c, object value);
        public delegate void invoke_delegate();

        public ExerciseReviewFrame()
        {
            InitializeComponent();
            this.tooltipChecker = new TooltipChecker();
            tooltipChecker.addControl(cmdJumpToError, "no_errors_in_exercises");
            tooltipChecker.addControl(cmdJumpToFirstError, "no_errors_in_exercises");
            tooltipChecker.addControl(cmdJumpToNextError, "no_errors_in_exercises");
            errornumber=-1;
        }

        private void ExerciseReviewFrame_Load(object sender, EventArgs e)
        {
            this.resetStrings();
            //this.cmdJumpToError.Visible = DomainController.Instance().canJumpToError();
            this.cmdJumpToError.Enabled = this.cmdJumpToError.Visible && DomainController.Instance().exercisesHaveErrors();
            this.cmdJumpToFirstError.Enabled = DomainController.Instance().exercisesHaveErrors();
            this.cmdJumpToNextError.Enabled = DomainController.Instance().exercisesHaveErrors();
            this.txtExercisesDump.Text = DomainController.Instance().getExercisesToString();
            this.txtExercisesDump.SelectionLength=0;
            this.txtExercisesDump.SelectionStart=0;
            this.webExercisesDump.Navigate(DomainController.Instance().getTempPath() + ProgramConstants.HTML_OUTPUT_NAME);
            cmdHtml_Click(sender, e);
        }

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void resetStrings()
        {
            StringsResetter.resetStaticStrings(this);
        }

        private void setComponentsEnabled(Boolean enable)
        {
            foreach(Control control in this.Controls)
                control.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { control, enable });
            if (enable)
            {
                if (webExercisesDump.Visible)
                    this.Invoke(new invoke_delegate(doShowHtml));
                else
                    this.Invoke(new invoke_delegate(doShowPlainText));
            }
        }

        private void cmdPlainText_Click(object sender, EventArgs e)
        {
            this.doShowPlainText();
            txtExercisesDump.Select();
        }

        private void cmdHtml_Click(object sender, EventArgs e)
        {
            this.doShowHtml();
            webExercisesDump.Select();
        }

        private void doShowPlainText()
        {
            txtExercisesDump.Enabled = true;
            webExercisesDump.Visible = false;
            cmdPlainText.Enabled = false;
            cmdHtml.Enabled = true;
            cmdHtml.BackColor = Control.DefaultBackColor;
            cmdPlainText.BackColor = Color.White;
        }
        
        private void doShowHtml()
        {
            txtExercisesDump.Enabled = false;
            webExercisesDump.Visible = true;
            cmdPlainText.Enabled = true;
            cmdHtml.Enabled = false;
            cmdHtml.BackColor = Color.White;
            cmdPlainText.BackColor = Control.DefaultBackColor;
        }

        private void cmdJumpToError_Click(object sender, EventArgs e)
        {
            if (DomainController.Instance().canJumpToError())
            {
                Thread thread = new Thread(new ThreadStart(doJumpToError));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void doJumpToError()
        {
            tooltipChecker.addControl(cmdJumpToError, "jumping_to_location_please_wait");
            setComponentsEnabled(false);
            DomainController.Instance().jumpToError();
            setComponentsEnabled(true);
            tooltipChecker.addControl(cmdJumpToError, "no_errors_in_exercises");
        }

        private void ExerciseReviewFrame_MouseMove(object sender, MouseEventArgs e)
        {
            this.tooltipChecker.updateTooltips(this.GetChildAtPoint(e.Location));
        }

        private void setControlEnabled(Control c, object b)
        {
            if (b is Boolean)
            {
                c.Enabled = (Boolean)b;
            }
        }

        private void cmdJumpToFirstError_Click(object sender, EventArgs e)
        {
            errornumber=0;
            jumpToAnchor("error_"+errornumber);
        }

        private void cmdJumpToNextError_Click(object sender, EventArgs e)
        {
            errornumber++;
            jumpToAnchor("error_"+errornumber);
        }

        private void jumpToAnchor(String anchor)
        {
        HtmlElementCollection elements = this.webExercisesDump.Document.Body.All;
        foreach (HtmlElement element in elements)
        {
            String nameAttribute = element.GetAttribute("Name");
            if (!String.IsNullOrEmpty(nameAttribute) && nameAttribute == anchor)
            {
                element.ScrollIntoView(true);
                break;
            }
        }

        }

    }
}
