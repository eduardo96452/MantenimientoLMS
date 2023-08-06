using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using lmsda.domain;

namespace lmsda.gui
{
    /// <remarks>
    ///     As of 1.08
    ///     
    ///     Last updated on 14/08/2010 by Gianni Van Hoecke
    /// </remarks>
    public partial class LogViewerFrame : Form
    {
        public LogViewerFrame()
        {
            InitializeComponent();
            this.resetStrings();
        }

        private void resetStrings()
        {
            //Set in user language
            StringsResetter.resetStaticStrings(this);
        }

        private void loadLog(Boolean advanced)
        {
            this.txtLog.Text = DomainController.Instance().getLogFileContent(advanced);
            this.txtLog.SelectionStart = this.txtLog.Text.Length;
            this.txtLog.ScrollToCaret();
        }

        #region User clicks

        private void chkAdvancedView_CheckedChanged(object sender, EventArgs e)
        {
            this.loadLog(this.chkAdvancedView.Checked);
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void cmdOpenLog_Click(object sender, EventArgs e)
        {
            DomainController.Instance().openLogInNotePad();
        }

        #endregion

        private void LogViewerFrame_Shown(object sender, EventArgs e)
        {
            this.loadLog(false);
        }
    }
}
