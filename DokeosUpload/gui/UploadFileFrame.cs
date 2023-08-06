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
using System.Threading;
using lmsda.domain.user;
using lmsda.domain.ui;

namespace lmsda.gui
{
    /// <remarks>
    ///     As of 1.08
    ///     Last updated on 10/08/2010 by Gianni Van Hoecke
    /// </remarks>
    public partial class UploadFileFrame : Form, FileUploadUI
    {
        private DomainController domainController;
        public delegate void invoke_delegate_with_arg(object value);
        public delegate void invoke_delegate_single_parameter(Control c, object value);

        public UploadFileFrame()
        {
            this.domainController = DomainController.Instance();
            InitializeComponent();
            this.domainController.addObserver(this);
            this.documentsDropDownForFileUpload.setDataSource(this.domainController.getDocumentFoldersForDropDown());
        }

        private void UploadFileFrame_Load(object sender, EventArgs e)
        {
            this.resetStrings();
        }

        private void resetStrings()
        {
            //Set in user language
            StringsResetter.resetStaticStrings(this);
        }

        private void cmdBrowserForFileUpload_Click(object sender, EventArgs e)
        {
            this.openFileDialog.Filter = "*.*|*.*";
            this.openFileDialog.FileName = String.Empty;
            this.openFileDialog.ShowDialog();

            if (!this.openFileDialog.FileName.Equals(String.Empty))
            {
                this.lblSourceFileForFileUpload.Text = this.openFileDialog.FileName;
            }
        }
        
        private void cmdUploadFile_Click(object sender, EventArgs e)
        {
            Boolean exists = false;
            Boolean setInvisible = this.chkSetFileInvisible.Checked;
            String localPath = this.lblSourceFileForFileUpload.Text;
            String description = this.txtUploadDescription.Text;
            try
            {
                exists = new FileInfo(localPath).Exists;
            }
            catch { }

            if (exists && this.documentsDropDownForFileUpload.getComboBox().Items.Count > 0)
            {
                this.progressBarFileUpload.Value = 0;
                this.progressBarFileUpload.Visible = true;
                this.lblProgress.Text = "...";
                this.lblProgress.Visible = true;
                Thread thread = new Thread(new ParameterizedThreadStart(doUploadFile));
                thread.SetApartmentState(ApartmentState.STA);
                DocumentFolder saveToSubDir =
                    this.domainController.getUserInfo().getSelectedItemDocumentFolders().getDocumentFolderFromFolderName(
                                        this.documentsDropDownForFileUpload.getSelectedItem());
                thread.Start(new Object[] { localPath, description, saveToSubDir, setInvisible });
            }
            else
            {
                this.domainController.writeToLog("no_source_selected", true, false, true);
            }
        }

        private void doUploadFile(Object objects)
        {
            try
            {
                Object[] parameters = (Object[])objects;
                //localPath, description, saveToSubDir, setInvisible
                String filename = (String)parameters[0];
                String description = (String)parameters[1];
                DocumentFolder documentFolders = (DocumentFolder)parameters[2];
                Boolean setInvisible = (Boolean)parameters[3];
                this.enableComponents(false);
                this.domainController.uploadFile(filename, documentFolders, description, setInvisible);
            }
            catch (Exception ex)
            {
                this.domainController.processError(ex, true);
            }
            finally
            {
                this.enableComponents(true);
            }
        }

        private void enableComponents(Boolean b)
        {
            this.Invoke(new invoke_delegate_with_arg(setStatusText), b ? State.READY : State.BUSY);

            foreach(Control control in this.Controls)
                control.Invoke(new invoke_delegate_single_parameter(setControlEnabled), new object[] { control, b });

            //Override feedback components
            this.progressBarFileUpload.Invoke(new invoke_delegate_single_parameter(setControlVisible), new object[] { progressBarFileUpload, !b });
            this.lblProgress.Invoke(new invoke_delegate_single_parameter(setControlVisible), new object[] { lblProgress, !b });
        }

        private void setStatusText(Object state)
        {
            this.domainController.fireState((State)state);
        }

        private void setControlEnabled(Control c, object b)
        {
            if (b is Boolean)
            {
                c.Enabled = (Boolean)b;
                if (c is ListBox)
                {
                    try
                    {
                        ListBox lb = (ListBox)c;
                        if (!lb.Enabled)
                            lb.BackColor = SystemColors.Control;
                        else
                            lb.BackColor = SystemColors.Window;
                    }
                    catch { }
                }
            }
        }

        private void setControlVisible(Control c, object b)
        {
            c.Visible = Convert.ToBoolean(b);
        }

        public void updateProgressBar(int value)
        {
            if (value > 100)
                value = 100;
            this.progressBarFileUpload.Invoke(new invoke_delegate_with_arg(setProgressBarValue), value);
        }

        private void setProgressBarValue(object value)
        {
            //Update the progress bar
            this.progressBarFileUpload.Value = Convert.ToInt32(value);

            //Update also the label
            try
            {
                this.lblProgress.Text = this.domainController.getUploadProgessOfFileUpload();
            }
            catch
            {
                this.lblProgress.Text = String.Empty;
            }
        }

        public void showMessage(string message, MessageBoxIcon icon)
        {
            this.Invoke(new invoke_delegate_with_arg(showMessageBox), (object)new object[] { message, icon });
        }

        private void showMessageBox(object o)
        {
            String message = Convert.ToString(((object[])o)[0]);
            MessageBoxIcon icon = (MessageBoxIcon)((object[])o)[1];
            MessageBox.Show(this, Convert.ToString(message), this.Text, MessageBoxButtons.OK, icon);
        }

        public void update(Boolean refreshUI)
        {
            //Not used.
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

    }
}
