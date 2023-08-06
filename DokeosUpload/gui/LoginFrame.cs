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
using lmsda.domain;
using System.Threading;
using lmsda.domain.ui;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    partial class LoginFrame : Form
    {
        private DomainController domainController;
        private Boolean forceSelectUsername = false;

        public delegate void invoke_delegate();
        public delegate void invoke_delegate_set_control_value(Control c, object value);

        #region Start methodes

        public LoginFrame()
            : this(false)
        { }

        public LoginFrame(Boolean forceSelectUsername)
        {
            this.domainController = DomainController.Instance();
            this.forceSelectUsername = forceSelectUsername;
            InitializeComponent();
        }

        private void LoginFrame_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.lmsda_icon;
            txtUsername.Text = this.domainController.getSettings().getLoginname();
            this.resetStrings();
        }

        /// <summary>
        ///     Deze methode wordt opgeroepen wanneer alle componenten zijn geladen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginFrame_Shown(object sender, EventArgs e)
        {
            if(!txtUsername.Text.Equals(String.Empty) && !this.forceSelectUsername)
                txtPassword.Focus();
            else
                txtUsername.Focus();
        }

        #endregion

        #region Control methodes
        
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length < 1 || txtPassword.Text.Length < 1)
            {
                if (txtUsername.Text.Length > 0 && txtUsername.Focused && txtPassword.Text.Length < 1)
                    txtPassword.Focus();
                else
                {
                    this.domainController.writeToLog(this.domainController.getLanguageString("error") + ": " + this.domainController.getLanguageString("please_fill_out_the_form_correctly"));
                    MessageBox.Show(this.domainController.getLanguageString("please_fill_out_the_form_correctly"), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (txtUsername.Text.Length < 1)
                        txtUsername.Focus();
                    else
                        txtPassword.Focus();
                }
            }
            else
            {
                this.progressBar.Visible = true;
                this.progressBar.MarqueeAnimationSpeed = 100;
                this.setComponentsEnabled(false);
                String[] para = new String[]{txtUsername.Text, txtPassword.Text};
                Thread thread = new Thread(new ParameterizedThreadStart(doLogin));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start(para);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        #endregion

        #region Eigen methodes

        private void doLogin(object o)
        {
            this.domainController.fireState(State.BUSY);
            String[] parameters = (String[])o;

            try
            {
                if (this.domainController.validateUser(parameters[0], parameters[1]))
                    this.Invoke(new invoke_delegate(this.Close));
                else
                    this.Invoke(new invoke_delegate(showLoginError));
            }
            catch (Exception e)
            {
                domainController.processError(e, true);
            }
            this.domainController.fireState(State.READY);
        }

        private void showLoginError()
        {
            showError("cannot_login");
        }

        private void showFoldersError()
        {
            showError("categories_folder_not_set");
        }

        private void showError(String message)
        {
            this.progressBar.MarqueeAnimationSpeed = 0;
            MessageBox.Show(this.domainController.getLanguageString(message), this.domainController.getLanguageString("error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.progressBar.Visible = false;
            this.setComponentsEnabled(true);
            this.txtPassword.Text = String.Empty;
            this.txtPassword.Focus();
        }

        private void showSettingsFrame()
        {
            new SettingsFrame().ShowDialog();
        }

        private void setComponentsEnabled(Boolean b)
        {
            this.txtUsername.Enabled = b;
            this.txtPassword.Enabled = b;
            this.btnOk.Enabled = b;
            this.btnCancel.Enabled = b;
        }

        private void resetStrings()
        {
            //Set in user language
            StringsResetter.resetStaticStrings(this);
            /*
            this.Text = this.domainController.getLanguageString("login");
            this.grbInformation.Text = this.domainController.getLanguageString("information");
            this.lblInformation.Text = this.domainController.getLanguageString("login_information");
            this.lblUsername.Text = this.domainController.getLanguageString("username") + ":";
            this.lblPassword.Text = this.domainController.getLanguageString("password") + ":";
            this.btnOk.Text = this.domainController.getLanguageString("ok");
            this.btnCancel.Text = this.domainController.getLanguageString("cancel");
            //*/
        }

        #endregion

    }
}