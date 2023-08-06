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
using lmsda.persistence.platform;
using lmsda.domain.util;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

namespace lmsda.gui
{
    /// <summary>
    ///     Author: Maarten Meuris
    ///             Gianni Van Hoecke
    /// </summary>
    /// <remarks>
    ///     Last updated on 12/08/2010 by Gianni Van Hoecke
    ///      -> Added default score variable.
    /// </remarks>
    partial class SettingsFrame : Form
    {
        private String language;

        private DomainController domainController;

        public SettingsFrame()
        {
            this.domainController = DomainController.Instance();
            InitializeComponent();
        }

        private void SettingsFrame_Load(object sender, EventArgs e)
        {
            cmbPlatform.DataSource = TargetPlatforms.getTargetPlatforms();
            String[] languages = this.domainController.getLanguages();
            cmbResources.DataSource = languages;
            cmbServices.DataSource = Utility.serviceAsStringArray();
            cmbEncodings.DataSource = ProgramConstants.getAllEncodings();

            language = this.domainController.getSettings().getLanguage();
            if(!language.Equals(String.Empty) && languages.Contains(language))
                cmbResources.SelectedIndex = cmbResources.Items.IndexOf(language);
            else if (this.domainController.getLanguages().Length > 0)
                cmbResources.SelectedIndex = 0;

            String platform = this.domainController.getSettings().getPlatform();
            if(!platform.Equals(String.Empty))
                cmbPlatform.SelectedIndex = cmbPlatform.Items.IndexOf(platform);
            else
                cmbPlatform.SelectedIndex = 0;

            String encoding = this.domainController.getSettings().getEncoding();
            if(encoding == null || encoding.Equals(String.Empty))
                selectStandardEncodingBasedOnPlatform();
            else
                cmbEncodings.SelectedItem = encoding;

            this.txtBaseURL.Text = this.domainController.getSettings().getUrl();

            this.txtPDFCombo.Text = this.domainController.getSettings().getPDFFileName();
            this.chkPdfSplitZeroBeforeFirstMatch.Checked = this.domainController.getSettings().getPDFSplitZeroBeforeFirst();
            this.chkPdfReplaceSpacesByUndescores.Checked = this.domainController.getSettings().getPDFReplaceSpacesByUndescores();
            this.lblFileName.Text = ProgramConstants.PDF_MASK_ELEMENT_FILENAME;
            this.lblNr.Text = ProgramConstants.PDF_MASK_ELEMENT_NR;
            this.lblStyleText.Text = ProgramConstants.PDF_MASK_ELEMENT_STYLETEXT;

            this.lblExample1.Text = ProgramConstants.PDF_MASK_FILENAME_NR;
            this.lblExample2.Text = ProgramConstants.PDF_MASK_NR_STYLETEXT;
            this.lblExample3.Text = ProgramConstants.PDF_MASK_FILENAME_NR_STYLETEXT;


            this.lblSubjectsFolderShow.Text = this.domainController.getSettings().getSubjectsFolder();
            if (lblSubjectsFolderShow.Text.Equals(String.Empty) || !Utility.doesFolderExist(lblSubjectsFolderShow.Text))
                lblSubjectsFolderShow.Text = this.domainController.getLanguageString("no_destination_selected");
            String service = this.domainController.getSettings().getService();
            if(!service.Equals(String.Empty))
                cmbServices.SelectedIndex = cmbServices.Items.IndexOf(service);
            else
                cmbServices.SelectedIndex = 0;

            lblStatisticsSavePath.Text = this.domainController.getSettings().getStatsFolder();
            if (lblStatisticsSavePath.Text.Equals(String.Empty) || !Utility.doesFolderExist(lblStatisticsSavePath.Text))
                lblStatisticsSavePath.Text = this.domainController.getLanguageString("no_destination_selected");

            this.chkDeleteRawDataAfterConversion.Checked = this.domainController.getSettings().getStatsDeleteRawDataAfterConversion();
            this.chkMakeCategoryFolderForSingleCourse.Checked = this.domainController.getSettings().getStatsMakeSubjectFolderForSingleCourse();

            this.txtDefaultScoreGaps.Text = Convert.ToString(this.domainController.getSettings().getDefaultScoreGaps()); //As of 1.08
            this.txtDefaultScoreMatching.Text = Convert.ToString(this.domainController.getSettings().getDefaultScoreMatching()); //As of 1.08

            this.setStrings();
        }

        /// <summary>
        ///     Method to adapt the language of the entire UI
        /// </summary>
        private void setStrings()
        {
            StringsResetter.resetStaticStrings(this);
            if(!new DirectoryInfo(this.lblSubjectsFolderShow.Text).Exists)
                this.lblSubjectsFolderShow.Text = this.domainController.getLanguageString("no_destination_selected");
        }

        private void cmbResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.domainController.selectLanguage(this.cmbResources.SelectedItem.ToString(), false);
            this.setStrings();
        }

        private void cmdCancel_Click(object sender, EventArgs e)
        {
            this.domainController.selectLanguage(language, false);
            this.Dispose();
        }

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (this.validatePDFCombo())
            {
                if (Utility.isNumeric(this.txtDefaultScoreGaps.Text))
                {
                    if (Utility.isNumeric(this.txtDefaultScoreMatching.Text))
                    {
                        this.save();
                    }
                    else 
                    {
                        this.tabControl1.SelectedTab = this.tabCommon;
                        this.txtDefaultScoreMatching.Focus();
                        MessageBox.Show(this, this.domainController.getLanguageString("validate_default_score_matching_fail"), this.domainController.getLanguageString("validation"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    this.tabControl1.SelectedTab = this.tabCommon;
                    this.txtDefaultScoreGaps.Focus();
                    MessageBox.Show(this, this.domainController.getLanguageString("validate_default_score_gaps_fail"), this.domainController.getLanguageString("validation"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                this.tabControl1.SelectedTab = this.tabPDF;
                this.txtPDFCombo.Focus();
                MessageBox.Show(this, this.domainController.getLanguageString("validate_pdf_fail"), this.domainController.getLanguageString("validation"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdOpenSettingsFolder_Click(object sender, EventArgs e)
        {
            Utility.openFolderInExplorer(ProgramConstants.getProgramSettingsPath());
        }

        private void save()
        {
            txtBaseURL.Text = txtBaseURL.Text.Trim('/');
            this.domainController.getSettings().setPlatform(this.cmbPlatform.SelectedItem.ToString());
            this.domainController.getSettings().setService(this.cmbServices.SelectedItem.ToString());
            this.domainController.getSettings().setUrl(this.txtBaseURL.Text);
            this.domainController.getSettings().setEncoding(this.cmbEncodings.SelectedItem.ToString());
            String subjectsFolder = this.lblSubjectsFolderShow.Text;
            if(subjectsFolder.Equals(this.domainController.getLanguageString("no_destination_selected")))
                subjectsFolder = String.Empty;
            this.domainController.getSettings().setSubjectsFolder(subjectsFolder);
            if (this.cmbResources.Items.Count > 0)
                this.domainController.getSettings().setLanguage(this.cmbResources.SelectedItem.ToString());
            this.domainController.getSettings().setStatsFolder(this.lblStatisticsSavePath.Text);
            this.domainController.getSettings().setStatsDeleteRawDataAfterConversion(this.chkDeleteRawDataAfterConversion.Checked);
            this.domainController.getSettings().setStatsMakeSubjectFolderForSingleCourse(this.chkMakeCategoryFolderForSingleCourse.Checked);
            this.domainController.getSettings().setPDFFileName(this.txtPDFCombo.Text);
            this.domainController.getSettings().setPDFSplitZeroBeforeFirst(this.chkPdfSplitZeroBeforeFirstMatch.Checked);
            this.domainController.getSettings().setPDFReplaceSpacesByUndescores(this.chkPdfReplaceSpacesByUndescores.Checked);
            this.domainController.getSettings().setDefaultScoreGaps(Convert.ToInt32(this.txtDefaultScoreGaps.Text));
            this.domainController.getSettings().setDefaultScoreMatching(Convert.ToInt32(this.txtDefaultScoreMatching.Text));
            /*
            if (!this.language.Equals(this.domainController.getSettings().getLanguage()))
            {
                this.domainController.getSettings().setStatsIDontKnowString(this.domainController.getLanguageString("do_not_know_string"));
                this.domainController.getSettings().setPDFSplitStyle(this.domainController.getLanguageString("pdf_split_default"));
            }
            */

            this.Close();
        }

        private void cmdStatisticsBrowse_Click(object sender, EventArgs e)
        {
            String defaultFolder;
            if (Utility.doesFolderExist(this.lblStatisticsSavePath.Text))
                defaultFolder = this.lblStatisticsSavePath.Text;
            else if (Utility.doesFolderExist(this.domainController.getSettings().getStatsFolder()))
                defaultFolder = this.domainController.getSettings().getStatsFolder();
            else if (!PortableIdentifier.Instance().isPortable)
                defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            else
                defaultFolder = ProgramConstants.getProgramPath();

            String path = this.openFolderBrowser(defaultFolder);
            if (!path.Equals(String.Empty))
            {
                if (PortableIdentifier.Instance().isPortable && !path.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                    this.domainController.fireMessageBox(this.domainController.getLanguageString("standalone_needs_local_folder"), MessageBoxIcon.Exclamation);
                else
                    this.lblStatisticsSavePath.Text = path;
            }
        }

        private void cmdBrowseForSubjectsFolder_Click(object sender, EventArgs e)
        {
            String defaultFolder;
            if (Utility.doesFolderExist(this.lblSubjectsFolderShow.Text))
                defaultFolder = this.lblSubjectsFolderShow.Text;
            else if (Utility.doesFolderExist(this.domainController.getSettings().getSubjectsFolder()))
                defaultFolder = this.domainController.getSettings().getSubjectsFolder();
            else if (!PortableIdentifier.Instance().isPortable)
                defaultFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            else
                defaultFolder = ProgramConstants.getProgramPath();

            String path = this.openFolderBrowser(defaultFolder);
            if (!path.Equals(String.Empty))
            {
                if (PortableIdentifier.Instance().isPortable && !path.StartsWith(ProgramConstants.getProgramPath().TrimEnd('\\')))
                    this.domainController.fireMessageBox(this.domainController.getLanguageString("standalone_needs_local_folder"), MessageBoxIcon.Exclamation);
                else
                    this.lblSubjectsFolderShow.Text = path;
            }
        }

        /// <summary>
        ///     Opent een folder browser.
        /// </summary>
        /// <returns>Het pad naar de geselecteerde folder.</returns>
        private String openFolderBrowser(String path)
        {
            this.folderBrowserDialog.SelectedPath = (path != null ? path : String.Empty);
            this.folderBrowserDialog.ShowDialog();
            return this.folderBrowserDialog.SelectedPath;
        }

        private void cmbPlatform_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.selectStandardEncodingBasedOnPlatform();
        }

        private void selectStandardEncodingBasedOnPlatform()
        {
            try
            {
                List<String> platforms = new List<String>(TargetPlatforms.getTargetPlatforms());
                String platformname = cmbPlatform.SelectedItem.ToString();
                this.cmbEncodings.SelectedItem = TargetPlatforms.getPlatformInfo(platformname).getPlatformEncoding();
            }
            catch { }
        }

        #region PDF

        private void cmdPDFValidate_Click(object sender, EventArgs e)
        {
            if(this.validatePDFCombo())
                MessageBox.Show(this, this.domainController.getLanguageString("validate_pdf_ok"), this.domainController.getLanguageString("validation"), MessageBoxButtons.OK, MessageBoxIcon.None);
            else
                MessageBox.Show(this, this.domainController.getLanguageString("validate_pdf_fail"), this.domainController.getLanguageString("validation"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        private Boolean validatePDFCombo()
        {
            String text = this.txtPDFCombo.Text;
            String c1 = "{nr}";
            String c2 = "{file_name}";
            String c3 = "{style_text}";

            //Number or style_text needed (split)
            if(!text.Contains(c1) && !text.Contains(c3))
                return false;

            //check tags
            Boolean correctTags = true;
            Regex regex = new Regex(@"\{(.*?)\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);            
            MatchCollection mc = regex.Matches(text);
            IEnumerator en = mc.GetEnumerator();
            while (en.MoveNext())
            {
                Match match = (Match)en.Current;
                String theLink = match.Value;
                if(!theLink.Equals(c1) && !theLink.Equals(c2) && !theLink.Equals(c3))
                    correctTags = false;
            }
            if (!correctTags)
                return false;

            this.txtPDFCombo.Text = Utility.getFileNameWithoutBadCharacters(text);

            return true;
        }

        private void lblNr_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text += "{nr}";
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        private void lblFileName_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text += "{file_name}";
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        private void lblStyleText_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text += "{style_text}";
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        private void lblExample1_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text = this.lblExample1.Text;
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        private void lblExample2_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text = this.lblExample2.Text;
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        private void lblExample3_Click(object sender, EventArgs e)
        {
            this.txtPDFCombo.Text = this.lblExample3.Text;
            this.txtPDFCombo.Focus();
            this.txtPDFCombo.Select(this.txtPDFCombo.Text.Length, 0);
        }

        #endregion

    }
}
