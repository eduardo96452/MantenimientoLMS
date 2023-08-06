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

namespace lmsda.gui
{
    partial class SettingsFrame
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmbEncodings = new System.Windows.Forms.ComboBox();
            this.lblEncoding = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbServices = new System.Windows.Forms.ComboBox();
            this.lblService = new System.Windows.Forms.Label();
            this.cmbPlatform = new System.Windows.Forms.ComboBox();
            this.lblPlatform = new System.Windows.Forms.Label();
            this.txtBaseURL = new System.Windows.Forms.TextBox();
            this.lblPlatformBaseURL = new System.Windows.Forms.Label();
            this.cmbResources = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.cmdOk = new System.Windows.Forms.Button();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.chkMakeCategoryFolderForSingleCourse = new System.Windows.Forms.CheckBox();
            this.chkDeleteRawDataAfterConversion = new System.Windows.Forms.CheckBox();
            this.lblStatisticsSavePath = new System.Windows.Forms.TextBox();
            this.lblStatisticsOutputFolder = new System.Windows.Forms.Label();
            this.cmdStatisticsBrowse = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabCommon = new System.Windows.Forms.TabPage();
            this.txtDefaultScoreMatching = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDefaultScoreGaps = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmdBrowseForSubjectsFolder = new System.Windows.Forms.Button();
            this.lblSubjectsFolderShow = new System.Windows.Forms.TextBox();
            this.lblSubjectsFolder = new System.Windows.Forms.Label();
            this.tabPlatform = new System.Windows.Forms.TabPage();
            this.tabPDF = new System.Windows.Forms.TabPage();
            this.chkPdfReplaceSpacesByUndescores = new System.Windows.Forms.CheckBox();
            this.chkPdfSplitZeroBeforeFirstMatch = new System.Windows.Forms.CheckBox();
            this.cmdPDFValidate = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.lblExample3 = new System.Windows.Forms.Label();
            this.lblExample2 = new System.Windows.Forms.Label();
            this.lblExample1 = new System.Windows.Forms.Label();
            this.txtPDFCombo = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblStyleTextInfo = new System.Windows.Forms.Label();
            this.lblFileNameInfo = new System.Windows.Forms.Label();
            this.lblNrInfo = new System.Windows.Forms.Label();
            this.lblStyleText = new System.Windows.Forms.Label();
            this.lblFileName = new System.Windows.Forms.Label();
            this.lblNr = new System.Windows.Forms.Label();
            this.lblPDFInfo = new System.Windows.Forms.Label();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.cmdOpenSettingsFolder = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabCommon.SuspendLayout();
            this.tabPlatform.SuspendLayout();
            this.tabPDF.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmbEncodings
            // 
            this.cmbEncodings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbEncodings.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEncodings.FormattingEnabled = true;
            this.cmbEncodings.Location = new System.Drawing.Point(162, 130);
            this.cmbEncodings.Name = "cmbEncodings";
            this.cmbEncodings.Size = new System.Drawing.Size(313, 21);
            this.cmbEncodings.TabIndex = 52;
            // 
            // lblEncoding
            // 
            this.lblEncoding.AutoSize = true;
            this.lblEncoding.Location = new System.Drawing.Point(14, 133);
            this.lblEncoding.Name = "lblEncoding";
            this.lblEncoding.Size = new System.Drawing.Size(57, 13);
            this.lblEncoding.TabIndex = 51;
            this.lblEncoding.Tag = "encoding :";
            this.lblEncoding.Text = "encoding :";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(162, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(314, 33);
            this.label2.TabIndex = 12;
            this.label2.Tag = "webservice_info";
            this.label2.Text = "webservice_info";
            // 
            // cmbServices
            // 
            this.cmbServices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbServices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbServices.FormattingEnabled = true;
            this.cmbServices.Location = new System.Drawing.Point(162, 70);
            this.cmbServices.Name = "cmbServices";
            this.cmbServices.Size = new System.Drawing.Size(314, 21);
            this.cmbServices.TabIndex = 6;
            // 
            // lblService
            // 
            this.lblService.AutoSize = true;
            this.lblService.Location = new System.Drawing.Point(14, 73);
            this.lblService.Name = "lblService";
            this.lblService.Size = new System.Drawing.Size(54, 13);
            this.lblService.TabIndex = 10;
            this.lblService.Tag = "service : *";
            this.lblService.Text = "service : *";
            // 
            // cmbPlatform
            // 
            this.cmbPlatform.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPlatform.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPlatform.FormattingEnabled = true;
            this.cmbPlatform.Location = new System.Drawing.Point(162, 40);
            this.cmbPlatform.Name = "cmbPlatform";
            this.cmbPlatform.Size = new System.Drawing.Size(314, 21);
            this.cmbPlatform.TabIndex = 5;
            this.cmbPlatform.SelectedIndexChanged += new System.EventHandler(this.cmbPlatform_SelectedIndexChanged);
            // 
            // lblPlatform
            // 
            this.lblPlatform.AutoSize = true;
            this.lblPlatform.Location = new System.Drawing.Point(14, 43);
            this.lblPlatform.Name = "lblPlatform";
            this.lblPlatform.Size = new System.Drawing.Size(50, 13);
            this.lblPlatform.TabIndex = 9;
            this.lblPlatform.Tag = "platform :";
            this.lblPlatform.Text = "platform :";
            // 
            // txtBaseURL
            // 
            this.txtBaseURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBaseURL.Location = new System.Drawing.Point(162, 10);
            this.txtBaseURL.Name = "txtBaseURL";
            this.txtBaseURL.Size = new System.Drawing.Size(314, 20);
            this.txtBaseURL.TabIndex = 4;
            // 
            // lblPlatformBaseURL
            // 
            this.lblPlatformBaseURL.AutoSize = true;
            this.lblPlatformBaseURL.Location = new System.Drawing.Point(14, 13);
            this.lblPlatformBaseURL.Name = "lblPlatformBaseURL";
            this.lblPlatformBaseURL.Size = new System.Drawing.Size(96, 13);
            this.lblPlatformBaseURL.TabIndex = 0;
            this.lblPlatformBaseURL.Tag = "platform_base_url :";
            this.lblPlatformBaseURL.Text = "platform_base_url :";
            // 
            // cmbResources
            // 
            this.cmbResources.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbResources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResources.FormattingEnabled = true;
            this.cmbResources.Location = new System.Drawing.Point(143, 10);
            this.cmbResources.Name = "cmbResources";
            this.cmbResources.Size = new System.Drawing.Size(338, 21);
            this.cmbResources.TabIndex = 2;
            this.cmbResources.SelectedIndexChanged += new System.EventHandler(this.cmbResources_SelectedIndexChanged);
            // 
            // lblLanguage
            // 
            this.lblLanguage.AutoSize = true;
            this.lblLanguage.Location = new System.Drawing.Point(14, 13);
            this.lblLanguage.Name = "lblLanguage";
            this.lblLanguage.Size = new System.Drawing.Size(92, 13);
            this.lblLanguage.TabIndex = 0;
            this.lblLanguage.Tag = "set_language_to :";
            this.lblLanguage.Text = "set_language_to :";
            // 
            // cmdOk
            // 
            this.cmdOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdOk.Location = new System.Drawing.Point(312, 273);
            this.cmdOk.Name = "cmdOk";
            this.cmdOk.Size = new System.Drawing.Size(96, 23);
            this.cmdOk.TabIndex = 90;
            this.cmdOk.Tag = "ok";
            this.cmdOk.Text = "ok";
            this.cmdOk.UseVisualStyleBackColor = true;
            this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
            // 
            // cmdCancel
            // 
            this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(414, 273);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(96, 23);
            this.cmdCancel.TabIndex = 91;
            this.cmdCancel.Tag = "cancel";
            this.cmdCancel.Text = "cancel";
            this.cmdCancel.UseVisualStyleBackColor = true;
            this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // chkMakeCategoryFolderForSingleCourse
            // 
            this.chkMakeCategoryFolderForSingleCourse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMakeCategoryFolderForSingleCourse.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkMakeCategoryFolderForSingleCourse.Location = new System.Drawing.Point(17, 120);
            this.chkMakeCategoryFolderForSingleCourse.Name = "chkMakeCategoryFolderForSingleCourse";
            this.chkMakeCategoryFolderForSingleCourse.Size = new System.Drawing.Size(431, 43);
            this.chkMakeCategoryFolderForSingleCourse.TabIndex = 12;
            this.chkMakeCategoryFolderForSingleCourse.Tag = "make_subject_folder_for_single_course";
            this.chkMakeCategoryFolderForSingleCourse.Text = "make_subject_folder_for_single_course";
            this.chkMakeCategoryFolderForSingleCourse.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkMakeCategoryFolderForSingleCourse.UseVisualStyleBackColor = true;
            // 
            // chkDeleteRawDataAfterConversion
            // 
            this.chkDeleteRawDataAfterConversion.AutoSize = true;
            this.chkDeleteRawDataAfterConversion.Location = new System.Drawing.Point(17, 93);
            this.chkDeleteRawDataAfterConversion.Name = "chkDeleteRawDataAfterConversion";
            this.chkDeleteRawDataAfterConversion.Size = new System.Drawing.Size(190, 17);
            this.chkDeleteRawDataAfterConversion.TabIndex = 11;
            this.chkDeleteRawDataAfterConversion.Tag = "delete_raw_data_after_conversion";
            this.chkDeleteRawDataAfterConversion.Text = "delete_raw_data_after_conversion";
            this.chkDeleteRawDataAfterConversion.UseVisualStyleBackColor = true;
            // 
            // lblStatisticsSavePath
            // 
            this.lblStatisticsSavePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatisticsSavePath.BackColor = System.Drawing.SystemColors.Control;
            this.lblStatisticsSavePath.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblStatisticsSavePath.Location = new System.Drawing.Point(17, 37);
            this.lblStatisticsSavePath.Name = "lblStatisticsSavePath";
            this.lblStatisticsSavePath.ReadOnly = true;
            this.lblStatisticsSavePath.Size = new System.Drawing.Size(431, 13);
            this.lblStatisticsSavePath.TabIndex = 48;
            this.lblStatisticsSavePath.TabStop = false;
            this.lblStatisticsSavePath.Text = "No folder selected";
            // 
            // lblStatisticsOutputFolder
            // 
            this.lblStatisticsOutputFolder.AutoSize = true;
            this.lblStatisticsOutputFolder.Location = new System.Drawing.Point(14, 12);
            this.lblStatisticsOutputFolder.Name = "lblStatisticsOutputFolder";
            this.lblStatisticsOutputFolder.Size = new System.Drawing.Size(75, 13);
            this.lblStatisticsOutputFolder.TabIndex = 47;
            this.lblStatisticsOutputFolder.Tag = "output_folder :";
            this.lblStatisticsOutputFolder.Text = "output_folder :";
            // 
            // cmdStatisticsBrowse
            // 
            this.cmdStatisticsBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStatisticsBrowse.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdStatisticsBrowse.Location = new System.Drawing.Point(348, 56);
            this.cmdStatisticsBrowse.Name = "cmdStatisticsBrowse";
            this.cmdStatisticsBrowse.Size = new System.Drawing.Size(100, 23);
            this.cmdStatisticsBrowse.TabIndex = 9;
            this.cmdStatisticsBrowse.Tag = "browse";
            this.cmdStatisticsBrowse.Text = "browse";
            this.cmdStatisticsBrowse.UseVisualStyleBackColor = true;
            this.cmdStatisticsBrowse.Click += new System.EventHandler(this.cmdStatisticsBrowse_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabCommon);
            this.tabControl1.Controls.Add(this.tabPlatform);
            this.tabControl1.Controls.Add(this.tabPDF);
            this.tabControl1.Controls.Add(this.tabStatistics);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(502, 255);
            this.tabControl1.TabIndex = 92;
            // 
            // tabCommon
            // 
            this.tabCommon.Controls.Add(this.txtDefaultScoreMatching);
            this.tabCommon.Controls.Add(this.label3);
            this.tabCommon.Controls.Add(this.txtDefaultScoreGaps);
            this.tabCommon.Controls.Add(this.label1);
            this.tabCommon.Controls.Add(this.cmdBrowseForSubjectsFolder);
            this.tabCommon.Controls.Add(this.lblSubjectsFolderShow);
            this.tabCommon.Controls.Add(this.lblSubjectsFolder);
            this.tabCommon.Controls.Add(this.cmbResources);
            this.tabCommon.Controls.Add(this.lblLanguage);
            this.tabCommon.Location = new System.Drawing.Point(4, 22);
            this.tabCommon.Name = "tabCommon";
            this.tabCommon.Padding = new System.Windows.Forms.Padding(3);
            this.tabCommon.Size = new System.Drawing.Size(494, 229);
            this.tabCommon.TabIndex = 0;
            this.tabCommon.Tag = "common";
            this.tabCommon.Text = "common";
            this.tabCommon.UseVisualStyleBackColor = true;
            // 
            // txtDefaultScoreMatching
            // 
            this.txtDefaultScoreMatching.Location = new System.Drawing.Point(423, 145);
            this.txtDefaultScoreMatching.MaxLength = 4;
            this.txtDefaultScoreMatching.Name = "txtDefaultScoreMatching";
            this.txtDefaultScoreMatching.Size = new System.Drawing.Size(58, 20);
            this.txtDefaultScoreMatching.TabIndex = 56;
            this.txtDefaultScoreMatching.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 148);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(120, 13);
            this.label3.TabIndex = 55;
            this.label3.Tag = "default_score_matching :";
            this.label3.Text = "default_score_matching";
            // 
            // txtDefaultScoreGaps
            // 
            this.txtDefaultScoreGaps.Location = new System.Drawing.Point(423, 118);
            this.txtDefaultScoreGaps.MaxLength = 4;
            this.txtDefaultScoreGaps.Name = "txtDefaultScoreGaps";
            this.txtDefaultScoreGaps.Size = new System.Drawing.Size(58, 20);
            this.txtDefaultScoreGaps.TabIndex = 54;
            this.txtDefaultScoreGaps.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 121);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 53;
            this.label1.Tag = "default_score_gaps :";
            this.label1.Text = "default_score_gaps";
            // 
            // cmdBrowseForSubjectsFolder
            // 
            this.cmdBrowseForSubjectsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdBrowseForSubjectsFolder.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdBrowseForSubjectsFolder.Location = new System.Drawing.Point(381, 86);
            this.cmdBrowseForSubjectsFolder.Name = "cmdBrowseForSubjectsFolder";
            this.cmdBrowseForSubjectsFolder.Size = new System.Drawing.Size(100, 23);
            this.cmdBrowseForSubjectsFolder.TabIndex = 50;
            this.cmdBrowseForSubjectsFolder.Tag = "browse";
            this.cmdBrowseForSubjectsFolder.Text = "browse";
            this.cmdBrowseForSubjectsFolder.UseVisualStyleBackColor = true;
            this.cmdBrowseForSubjectsFolder.Click += new System.EventHandler(this.cmdBrowseForSubjectsFolder_Click);
            // 
            // lblSubjectsFolderShow
            // 
            this.lblSubjectsFolderShow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSubjectsFolderShow.BackColor = System.Drawing.SystemColors.Control;
            this.lblSubjectsFolderShow.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblSubjectsFolderShow.Location = new System.Drawing.Point(17, 67);
            this.lblSubjectsFolderShow.Name = "lblSubjectsFolderShow";
            this.lblSubjectsFolderShow.ReadOnly = true;
            this.lblSubjectsFolderShow.Size = new System.Drawing.Size(464, 13);
            this.lblSubjectsFolderShow.TabIndex = 52;
            this.lblSubjectsFolderShow.TabStop = false;
            this.lblSubjectsFolderShow.Tag = "";
            this.lblSubjectsFolderShow.Text = "No folder selected";
            // 
            // lblSubjectsFolder
            // 
            this.lblSubjectsFolder.AutoSize = true;
            this.lblSubjectsFolder.Location = new System.Drawing.Point(14, 42);
            this.lblSubjectsFolder.Name = "lblSubjectsFolder";
            this.lblSubjectsFolder.Size = new System.Drawing.Size(84, 13);
            this.lblSubjectsFolder.TabIndex = 51;
            this.lblSubjectsFolder.Tag = "subjects_folder :";
            this.lblSubjectsFolder.Text = "subjects_folder :";
            // 
            // tabPlatform
            // 
            this.tabPlatform.Controls.Add(this.cmbEncodings);
            this.tabPlatform.Controls.Add(this.lblPlatformBaseURL);
            this.tabPlatform.Controls.Add(this.lblEncoding);
            this.tabPlatform.Controls.Add(this.txtBaseURL);
            this.tabPlatform.Controls.Add(this.lblPlatform);
            this.tabPlatform.Controls.Add(this.cmbPlatform);
            this.tabPlatform.Controls.Add(this.lblService);
            this.tabPlatform.Controls.Add(this.label2);
            this.tabPlatform.Controls.Add(this.cmbServices);
            this.tabPlatform.Location = new System.Drawing.Point(4, 22);
            this.tabPlatform.Name = "tabPlatform";
            this.tabPlatform.Padding = new System.Windows.Forms.Padding(3);
            this.tabPlatform.Size = new System.Drawing.Size(494, 229);
            this.tabPlatform.TabIndex = 1;
            this.tabPlatform.Tag = "platform_settings";
            this.tabPlatform.Text = "platform_settings";
            this.tabPlatform.UseVisualStyleBackColor = true;
            // 
            // tabPDF
            // 
            this.tabPDF.Controls.Add(this.chkPdfReplaceSpacesByUndescores);
            this.tabPDF.Controls.Add(this.chkPdfSplitZeroBeforeFirstMatch);
            this.tabPDF.Controls.Add(this.cmdPDFValidate);
            this.tabPDF.Controls.Add(this.groupBox2);
            this.tabPDF.Controls.Add(this.txtPDFCombo);
            this.tabPDF.Controls.Add(this.groupBox1);
            this.tabPDF.Controls.Add(this.lblPDFInfo);
            this.tabPDF.Location = new System.Drawing.Point(4, 22);
            this.tabPDF.Name = "tabPDF";
            this.tabPDF.Size = new System.Drawing.Size(462, 229);
            this.tabPDF.TabIndex = 3;
            this.tabPDF.Tag = "pdf";
            this.tabPDF.Text = "pdf";
            this.tabPDF.UseVisualStyleBackColor = true;
            // 
            // chkPdfReplaceSpacesByUndescores
            // 
            this.chkPdfReplaceSpacesByUndescores.AutoSize = true;
            this.chkPdfReplaceSpacesByUndescores.Location = new System.Drawing.Point(14, 202);
            this.chkPdfReplaceSpacesByUndescores.Name = "chkPdfReplaceSpacesByUndescores";
            this.chkPdfReplaceSpacesByUndescores.Size = new System.Drawing.Size(202, 17);
            this.chkPdfReplaceSpacesByUndescores.TabIndex = 14;
            this.chkPdfReplaceSpacesByUndescores.Tag = "pdf_convert_spaces_to_underscores";
            this.chkPdfReplaceSpacesByUndescores.Text = "pdf_convert_spaces_to_underscores";
            this.chkPdfReplaceSpacesByUndescores.UseVisualStyleBackColor = true;
            // 
            // chkPdfSplitZeroBeforeFirstMatch
            // 
            this.chkPdfSplitZeroBeforeFirstMatch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPdfSplitZeroBeforeFirstMatch.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkPdfSplitZeroBeforeFirstMatch.Location = new System.Drawing.Point(14, 164);
            this.chkPdfSplitZeroBeforeFirstMatch.Name = "chkPdfSplitZeroBeforeFirstMatch";
            this.chkPdfSplitZeroBeforeFirstMatch.Size = new System.Drawing.Size(431, 31);
            this.chkPdfSplitZeroBeforeFirstMatch.TabIndex = 13;
            this.chkPdfSplitZeroBeforeFirstMatch.Tag = "pdf_split_zero_before_first";
            this.chkPdfSplitZeroBeforeFirstMatch.Text = "pdf_split_zero_before_first\r\npdf_split_zero_before_first";
            this.chkPdfSplitZeroBeforeFirstMatch.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkPdfSplitZeroBeforeFirstMatch.UseVisualStyleBackColor = true;
            // 
            // cmdPDFValidate
            // 
            this.cmdPDFValidate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdPDFValidate.Location = new System.Drawing.Point(368, 135);
            this.cmdPDFValidate.Name = "cmdPDFValidate";
            this.cmdPDFValidate.Size = new System.Drawing.Size(81, 23);
            this.cmdPDFValidate.TabIndex = 4;
            this.cmdPDFValidate.Tag = "validate";
            this.cmdPDFValidate.Text = "validate";
            this.cmdPDFValidate.UseVisualStyleBackColor = true;
            this.cmdPDFValidate.Click += new System.EventHandler(this.cmdPDFValidate_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.lblExample3);
            this.groupBox2.Controls.Add(this.lblExample2);
            this.groupBox2.Controls.Add(this.lblExample1);
            this.groupBox2.Location = new System.Drawing.Point(283, 45);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(166, 86);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Tag = "examples";
            this.groupBox2.Text = "examples";
            // 
            // lblExample3
            // 
            this.lblExample3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblExample3.Location = new System.Drawing.Point(7, 63);
            this.lblExample3.Name = "lblExample3";
            this.lblExample3.Size = new System.Drawing.Size(141, 13);
            this.lblExample3.TabIndex = 2;
            this.lblExample3.Text = "{file_name}_{nr}_{style_text}";
            this.lblExample3.Click += new System.EventHandler(this.lblExample3_Click);
            // 
            // lblExample2
            // 
            this.lblExample2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblExample2.Location = new System.Drawing.Point(7, 41);
            this.lblExample2.Name = "lblExample2";
            this.lblExample2.Size = new System.Drawing.Size(141, 13);
            this.lblExample2.TabIndex = 1;
            this.lblExample2.Text = "{nr}_{style_text}";
            this.lblExample2.Click += new System.EventHandler(this.lblExample2_Click);
            // 
            // lblExample1
            // 
            this.lblExample1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblExample1.Location = new System.Drawing.Point(7, 20);
            this.lblExample1.Name = "lblExample1";
            this.lblExample1.Size = new System.Drawing.Size(141, 21);
            this.lblExample1.TabIndex = 0;
            this.lblExample1.Text = "{file_name}_{nr}";
            this.lblExample1.Click += new System.EventHandler(this.lblExample1_Click);
            // 
            // txtPDFCombo
            // 
            this.txtPDFCombo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPDFCombo.Location = new System.Drawing.Point(17, 137);
            this.txtPDFCombo.Name = "txtPDFCombo";
            this.txtPDFCombo.Size = new System.Drawing.Size(345, 20);
            this.txtPDFCombo.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblStyleTextInfo);
            this.groupBox1.Controls.Add(this.lblFileNameInfo);
            this.groupBox1.Controls.Add(this.lblNrInfo);
            this.groupBox1.Controls.Add(this.lblStyleText);
            this.groupBox1.Controls.Add(this.lblFileName);
            this.groupBox1.Controls.Add(this.lblNr);
            this.groupBox1.Location = new System.Drawing.Point(17, 44);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 87);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "legenda";
            this.groupBox1.Text = "legenda";
            // 
            // lblStyleTextInfo
            // 
            this.lblStyleTextInfo.AutoSize = true;
            this.lblStyleTextInfo.Location = new System.Drawing.Point(81, 64);
            this.lblStyleTextInfo.Name = "lblStyleTextInfo";
            this.lblStyleTextInfo.Size = new System.Drawing.Size(74, 13);
            this.lblStyleTextInfo.TabIndex = 5;
            this.lblStyleTextInfo.Tag = "style_text_info";
            this.lblStyleTextInfo.Text = "style_text_info";
            // 
            // lblFileNameInfo
            // 
            this.lblFileNameInfo.AutoSize = true;
            this.lblFileNameInfo.Location = new System.Drawing.Point(80, 20);
            this.lblFileNameInfo.Name = "lblFileNameInfo";
            this.lblFileNameInfo.Size = new System.Drawing.Size(75, 13);
            this.lblFileNameInfo.TabIndex = 4;
            this.lblFileNameInfo.Tag = "file_name_info";
            this.lblFileNameInfo.Text = "file_name_info";
            // 
            // lblNrInfo
            // 
            this.lblNrInfo.AutoSize = true;
            this.lblNrInfo.Location = new System.Drawing.Point(81, 42);
            this.lblNrInfo.Name = "lblNrInfo";
            this.lblNrInfo.Size = new System.Drawing.Size(39, 13);
            this.lblNrInfo.TabIndex = 3;
            this.lblNrInfo.Tag = "nr_info";
            this.lblNrInfo.Text = "nr_info";
            // 
            // lblStyleText
            // 
            this.lblStyleText.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStyleText.Location = new System.Drawing.Point(7, 64);
            this.lblStyleText.Name = "lblStyleText";
            this.lblStyleText.Size = new System.Drawing.Size(67, 13);
            this.lblStyleText.TabIndex = 2;
            this.lblStyleText.Text = "{style_text}";
            this.lblStyleText.Click += new System.EventHandler(this.lblStyleText_Click);
            // 
            // lblFileName
            // 
            this.lblFileName.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblFileName.Location = new System.Drawing.Point(7, 21);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(68, 13);
            this.lblFileName.TabIndex = 1;
            this.lblFileName.Text = "{file_name}";
            this.lblFileName.Click += new System.EventHandler(this.lblFileName_Click);
            // 
            // lblNr
            // 
            this.lblNr.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblNr.Location = new System.Drawing.Point(8, 42);
            this.lblNr.Name = "lblNr";
            this.lblNr.Size = new System.Drawing.Size(67, 13);
            this.lblNr.TabIndex = 0;
            this.lblNr.Text = "{nr}";
            this.lblNr.Click += new System.EventHandler(this.lblNr_Click);
            // 
            // lblPDFInfo
            // 
            this.lblPDFInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPDFInfo.Location = new System.Drawing.Point(14, 10);
            this.lblPDFInfo.Name = "lblPDFInfo";
            this.lblPDFInfo.Size = new System.Drawing.Size(435, 31);
            this.lblPDFInfo.TabIndex = 0;
            this.lblPDFInfo.Tag = "pdf_info";
            this.lblPDFInfo.Text = "pdf_info";
            // 
            // tabStatistics
            // 
            this.tabStatistics.Controls.Add(this.chkMakeCategoryFolderForSingleCourse);
            this.tabStatistics.Controls.Add(this.lblStatisticsOutputFolder);
            this.tabStatistics.Controls.Add(this.chkDeleteRawDataAfterConversion);
            this.tabStatistics.Controls.Add(this.cmdStatisticsBrowse);
            this.tabStatistics.Controls.Add(this.lblStatisticsSavePath);
            this.tabStatistics.Location = new System.Drawing.Point(4, 22);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Size = new System.Drawing.Size(462, 229);
            this.tabStatistics.TabIndex = 2;
            this.tabStatistics.Tag = "statistics";
            this.tabStatistics.Text = "statistics";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // cmdOpenSettingsFolder
            // 
            this.cmdOpenSettingsFolder.Location = new System.Drawing.Point(12, 273);
            this.cmdOpenSettingsFolder.Name = "cmdOpenSettingsFolder";
            this.cmdOpenSettingsFolder.Size = new System.Drawing.Size(153, 23);
            this.cmdOpenSettingsFolder.TabIndex = 93;
            this.cmdOpenSettingsFolder.Tag = "open_settings_folder";
            this.cmdOpenSettingsFolder.Text = "open_settings_folder";
            this.cmdOpenSettingsFolder.UseVisualStyleBackColor = true;
            this.cmdOpenSettingsFolder.Click += new System.EventHandler(this.cmdOpenSettingsFolder_Click);
            // 
            // SettingsFrame
            // 
            this.AcceptButton = this.cmdOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(526, 308);
            this.ControlBox = false;
            this.Controls.Add(this.cmdOpenSettingsFolder);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.cmdOk);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsFrame";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Tag = "preferences";
            this.Text = "preferences";
            this.Load += new System.EventHandler(this.SettingsFrame_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabCommon.ResumeLayout(false);
            this.tabCommon.PerformLayout();
            this.tabPlatform.ResumeLayout(false);
            this.tabPlatform.PerformLayout();
            this.tabPDF.ResumeLayout(false);
            this.tabPDF.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabStatistics.ResumeLayout(false);
            this.tabStatistics.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtBaseURL;
        private System.Windows.Forms.Label lblPlatformBaseURL;
        private System.Windows.Forms.ComboBox cmbPlatform;
        private System.Windows.Forms.Label lblPlatform;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.ComboBox cmbResources;
        private System.Windows.Forms.Button cmdOk;
        private System.Windows.Forms.Button cmdCancel;
        private System.Windows.Forms.Button cmdStatisticsBrowse;
        private System.Windows.Forms.Label lblStatisticsOutputFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.TextBox lblStatisticsSavePath;
        private System.Windows.Forms.CheckBox chkDeleteRawDataAfterConversion;
        private System.Windows.Forms.ComboBox cmbServices;
        private System.Windows.Forms.Label lblService;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkMakeCategoryFolderForSingleCourse;
        private System.Windows.Forms.Label lblEncoding;
        private System.Windows.Forms.ComboBox cmbEncodings;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabCommon;
        private System.Windows.Forms.TabPage tabPlatform;
        private System.Windows.Forms.TabPage tabPDF;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.Label lblPDFInfo;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblNr;
        private System.Windows.Forms.Label lblStyleText;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.TextBox txtPDFCombo;
        private System.Windows.Forms.Label lblStyleTextInfo;
        private System.Windows.Forms.Label lblFileNameInfo;
        private System.Windows.Forms.Label lblNrInfo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblExample3;
        private System.Windows.Forms.Label lblExample2;
        private System.Windows.Forms.Label lblExample1;
        private System.Windows.Forms.Button cmdPDFValidate;
        private System.Windows.Forms.Button cmdBrowseForSubjectsFolder;
        private System.Windows.Forms.TextBox lblSubjectsFolderShow;
        private System.Windows.Forms.Label lblSubjectsFolder;
        private System.Windows.Forms.CheckBox chkPdfSplitZeroBeforeFirstMatch;
        private System.Windows.Forms.Button cmdOpenSettingsFolder;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDefaultScoreGaps;
        private System.Windows.Forms.TextBox txtDefaultScoreMatching;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkPdfReplaceSpacesByUndescores;
    }
}