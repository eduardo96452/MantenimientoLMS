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

using lmsda.gui.treeview;

namespace lmsda.gui.subject
{
    partial class SubjectFilesSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cmdApplyToAllFiles = new System.Windows.Forms.Button();
            this.tableLayoutFilesSettings = new System.Windows.Forms.TableLayoutPanel();
            this.treeViewExplorerPanel = new lmsda.gui.treeview.TreeViewExplorerPanel();
            this.cmdHelp = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.cmdOpenFolder = new System.Windows.Forms.Button();
            this.cmdReload = new System.Windows.Forms.Button();
            this.panelOptions = new System.Windows.Forms.Panel();
            this.rdbExcludeFile = new System.Windows.Forms.RadioButton();
            this.rdbUploadFile = new System.Windows.Forms.RadioButton();
            this.rdbConvertToExercises = new System.Windows.Forms.RadioButton();
            this.rdbConvertToPDF = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fileOptionsPDFPanel = new lmsda.gui.subject.FileOptionsPDFPanel();
            this.fileOptionsPDFPanelForExcel = new lmsda.gui.subject.FileOptionsPDFPanelForExcel();
            this.fileOptionsPDFPanelForPowerPoint = new lmsda.gui.subject.FileOptionsPDFPanelForPowerPoint();
            this.fileOptionsExercisePanel = new lmsda.gui.subject.FileOptionsExercisePanel();
            this.fileOptionsUploadPanel = new lmsda.gui.subject.FileOptionsUploadPanel();
            this.tableLayoutFilesSettings.SuspendLayout();
            this.panelOptions.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cmdApplyToAllFiles
            // 
            this.cmdApplyToAllFiles.Location = new System.Drawing.Point(3, 266);
            this.cmdApplyToAllFiles.Name = "cmdApplyToAllFiles";
            this.cmdApplyToAllFiles.Size = new System.Drawing.Size(190, 23);
            this.cmdApplyToAllFiles.TabIndex = 41;
            this.cmdApplyToAllFiles.Tag = "apply_to_all_folder_items";
            this.cmdApplyToAllFiles.Text = "apply_to_all_folder_items";
            this.toolTip.SetToolTip(this.cmdApplyToAllFiles, "Click here to apply the current settings of this file to all files in the folder");
            this.cmdApplyToAllFiles.UseVisualStyleBackColor = true;
            this.cmdApplyToAllFiles.Click += new System.EventHandler(this.cmdApplyToAllFiles_Click);
            // 
            // tableLayoutFilesSettings
            // 
            this.tableLayoutFilesSettings.ColumnCount = 5;
            this.tableLayoutFilesSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutFilesSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutFilesSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutFilesSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12F));
            this.tableLayoutFilesSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.tableLayoutFilesSettings.Controls.Add(this.treeViewExplorerPanel, 0, 0);
            this.tableLayoutFilesSettings.Controls.Add(this.cmdHelp, 3, 1);
            this.tableLayoutFilesSettings.Controls.Add(this.cmdSave, 0, 1);
            this.tableLayoutFilesSettings.Controls.Add(this.cmdOpenFolder, 2, 1);
            this.tableLayoutFilesSettings.Controls.Add(this.cmdReload, 1, 1);
            this.tableLayoutFilesSettings.Controls.Add(this.panelOptions, 4, 0);
            this.tableLayoutFilesSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutFilesSettings.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutFilesSettings.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.tableLayoutFilesSettings.Name = "tableLayoutFilesSettings";
            this.tableLayoutFilesSettings.RowCount = 2;
            this.tableLayoutFilesSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutFilesSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 53F));
            this.tableLayoutFilesSettings.Size = new System.Drawing.Size(750, 298);
            this.tableLayoutFilesSettings.TabIndex = 1;
            this.tableLayoutFilesSettings.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tableLayoutFilesSettings_MouseMove);
            // 
            // treeViewExplorerPanel
            // 
            this.tableLayoutFilesSettings.SetColumnSpan(this.treeViewExplorerPanel, 4);
            this.treeViewExplorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewExplorerPanel.Location = new System.Drawing.Point(3, 3);
            this.treeViewExplorerPanel.Name = "treeViewExplorerPanel";
            this.treeViewExplorerPanel.Size = new System.Drawing.Size(354, 239);
            this.treeViewExplorerPanel.TabIndex = 20;
            // 
            // cmdHelp
            // 
            this.cmdHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdHelp.Image = global::lmsda.Properties.Resources.btn_info;
            this.cmdHelp.Location = new System.Drawing.Point(273, 248);
            this.cmdHelp.Name = "cmdHelp";
            this.cmdHelp.Padding = new System.Windows.Forms.Padding(5);
            this.cmdHelp.Size = new System.Drawing.Size(84, 47);
            this.cmdHelp.TabIndex = 24;
            this.cmdHelp.Tag = "";
            this.cmdHelp.UseVisualStyleBackColor = true;
            this.cmdHelp.Click += new System.EventHandler(this.cmdHelp_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdSave.Image = global::lmsda.Properties.Resources.btn_save;
            this.cmdSave.Location = new System.Drawing.Point(3, 248);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Padding = new System.Windows.Forms.Padding(5);
            this.cmdSave.Size = new System.Drawing.Size(84, 47);
            this.cmdSave.TabIndex = 21;
            this.cmdSave.Tag = "";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // cmdOpenFolder
            // 
            this.cmdOpenFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdOpenFolder.Image = global::lmsda.Properties.Resources.btn_open;
            this.cmdOpenFolder.Location = new System.Drawing.Point(183, 248);
            this.cmdOpenFolder.Name = "cmdOpenFolder";
            this.cmdOpenFolder.Padding = new System.Windows.Forms.Padding(5);
            this.cmdOpenFolder.Size = new System.Drawing.Size(84, 47);
            this.cmdOpenFolder.TabIndex = 23;
            this.cmdOpenFolder.Tag = "";
            this.cmdOpenFolder.UseVisualStyleBackColor = true;
            this.cmdOpenFolder.Click += new System.EventHandler(this.cmdOpenFolder_Click);
            // 
            // cmdReload
            // 
            this.cmdReload.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmdReload.Image = global::lmsda.Properties.Resources.btn_reload;
            this.cmdReload.Location = new System.Drawing.Point(93, 248);
            this.cmdReload.Name = "cmdReload";
            this.cmdReload.Padding = new System.Windows.Forms.Padding(5);
            this.cmdReload.Size = new System.Drawing.Size(84, 47);
            this.cmdReload.TabIndex = 22;
            this.cmdReload.Tag = "";
            this.cmdReload.UseVisualStyleBackColor = true;
            this.cmdReload.Click += new System.EventHandler(this.cmdReload_Click);
            // 
            // panelOptions
            // 
            this.panelOptions.Controls.Add(this.rdbExcludeFile);
            this.panelOptions.Controls.Add(this.rdbUploadFile);
            this.panelOptions.Controls.Add(this.rdbConvertToExercises);
            this.panelOptions.Controls.Add(this.cmdApplyToAllFiles);
            this.panelOptions.Controls.Add(this.rdbConvertToPDF);
            this.panelOptions.Controls.Add(this.groupBox1);
            this.panelOptions.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelOptions.Location = new System.Drawing.Point(363, 3);
            this.panelOptions.Name = "panelOptions";
            this.tableLayoutFilesSettings.SetRowSpan(this.panelOptions, 2);
            this.panelOptions.Size = new System.Drawing.Size(384, 292);
            this.panelOptions.TabIndex = 25;
            // 
            // rdbExcludeFile
            // 
            this.rdbExcludeFile.AutoSize = true;
            this.rdbExcludeFile.Location = new System.Drawing.Point(3, 3);
            this.rdbExcludeFile.Name = "rdbExcludeFile";
            this.rdbExcludeFile.Size = new System.Drawing.Size(121, 17);
            this.rdbExcludeFile.TabIndex = 26;
            this.rdbExcludeFile.Tag = "do_not_upload_item";
            this.rdbExcludeFile.Text = "do_not_upload_item";
            this.rdbExcludeFile.UseVisualStyleBackColor = true;
            this.rdbExcludeFile.CheckedChanged += new System.EventHandler(this.rdbExcludeFile_CheckedChanged);
            // 
            // rdbUploadFile
            // 
            this.rdbUploadFile.AutoSize = true;
            this.rdbUploadFile.Location = new System.Drawing.Point(3, 24);
            this.rdbUploadFile.Name = "rdbUploadFile";
            this.rdbUploadFile.Size = new System.Drawing.Size(104, 17);
            this.rdbUploadFile.TabIndex = 27;
            this.rdbUploadFile.Tag = "just_upload_item";
            this.rdbUploadFile.Text = "just_upload_item";
            this.rdbUploadFile.UseVisualStyleBackColor = true;
            this.rdbUploadFile.CheckedChanged += new System.EventHandler(this.rdbUploadFile_CheckedChanged);
            // 
            // rdbConvertToExercises
            // 
            this.rdbConvertToExercises.AutoSize = true;
            this.rdbConvertToExercises.Location = new System.Drawing.Point(3, 45);
            this.rdbConvertToExercises.Name = "rdbConvertToExercises";
            this.rdbConvertToExercises.Size = new System.Drawing.Size(177, 17);
            this.rdbConvertToExercises.TabIndex = 28;
            this.rdbConvertToExercises.Tag = "upload_document_as_exercises";
            this.rdbConvertToExercises.Text = "upload_document_as_exercises";
            this.rdbConvertToExercises.UseVisualStyleBackColor = true;
            this.rdbConvertToExercises.CheckedChanged += new System.EventHandler(this.rdbConvertToExercises_CheckedChanged);
            // 
            // rdbConvertToPDF
            // 
            this.rdbConvertToPDF.AutoSize = true;
            this.rdbConvertToPDF.Location = new System.Drawing.Point(3, 66);
            this.rdbConvertToPDF.Name = "rdbConvertToPDF";
            this.rdbConvertToPDF.Size = new System.Drawing.Size(148, 17);
            this.rdbConvertToPDF.TabIndex = 29;
            this.rdbConvertToPDF.Tag = "upload_document_as_pdf";
            this.rdbConvertToPDF.Text = "upload_document_as_pdf";
            this.rdbConvertToPDF.UseVisualStyleBackColor = true;
            this.rdbConvertToPDF.CheckedChanged += new System.EventHandler(this.rdbConvertToPDF_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.fileOptionsPDFPanel);
            this.groupBox1.Controls.Add(this.fileOptionsPDFPanelForExcel);
            this.groupBox1.Controls.Add(this.fileOptionsPDFPanelForPowerPoint);
            this.groupBox1.Controls.Add(this.fileOptionsExercisePanel);
            this.groupBox1.Controls.Add(this.fileOptionsUploadPanel);
            this.groupBox1.Location = new System.Drawing.Point(9, 89);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(370, 171);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Tag = "synchronize_options";
            this.groupBox1.Text = "synchronize_options";
            // 
            // fileOptionsPDFPanel
            // 
            this.fileOptionsPDFPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileOptionsPDFPanel.Location = new System.Drawing.Point(3, 16);
            this.fileOptionsPDFPanel.Name = "fileOptionsPDFPanel";
            this.fileOptionsPDFPanel.Size = new System.Drawing.Size(364, 152);
            this.fileOptionsPDFPanel.TabIndex = 40;
            this.fileOptionsPDFPanel.Visible = false;
            // 
            // fileOptionsPDFPanelForExcel
            // 
            this.fileOptionsPDFPanelForExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileOptionsPDFPanelForExcel.Location = new System.Drawing.Point(3, 16);
            this.fileOptionsPDFPanelForExcel.Name = "fileOptionsPDFPanelForExcel";
            this.fileOptionsPDFPanelForExcel.Size = new System.Drawing.Size(364, 152);
            this.fileOptionsPDFPanelForExcel.TabIndex = 40;
            this.fileOptionsPDFPanelForExcel.Visible = false;
            // 
            // fileOptionsPDFPanelForPowerPoint
            // 
            this.fileOptionsPDFPanelForPowerPoint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileOptionsPDFPanelForPowerPoint.Location = new System.Drawing.Point(3, 16);
            this.fileOptionsPDFPanelForPowerPoint.Name = "fileOptionsPDFPanelForPowerPoint";
            this.fileOptionsPDFPanelForPowerPoint.Size = new System.Drawing.Size(364, 152);
            this.fileOptionsPDFPanelForPowerPoint.TabIndex = 40;
            this.fileOptionsPDFPanelForPowerPoint.Visible = false;
            // 
            // fileOptionsExercisePanel
            // 
            this.fileOptionsExercisePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileOptionsExercisePanel.Location = new System.Drawing.Point(3, 16);
            this.fileOptionsExercisePanel.Name = "fileOptionsExercisePanel";
            this.fileOptionsExercisePanel.Size = new System.Drawing.Size(364, 152);
            this.fileOptionsExercisePanel.TabIndex = 40;
            this.fileOptionsExercisePanel.Visible = false;
            // 
            // fileOptionsUploadPanel
            // 
            this.fileOptionsUploadPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileOptionsUploadPanel.Location = new System.Drawing.Point(3, 16);
            this.fileOptionsUploadPanel.Name = "fileOptionsUploadPanel";
            this.fileOptionsUploadPanel.Size = new System.Drawing.Size(364, 152);
            this.fileOptionsUploadPanel.TabIndex = 40;
            this.fileOptionsUploadPanel.Visible = false;
            // 
            // SubjectFilesSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutFilesSettings);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SubjectFilesSettingsControl";
            this.Size = new System.Drawing.Size(750, 298);
            this.Load += new System.EventHandler(this.SubjectFilesSettingsControl_Load);
            this.tableLayoutFilesSettings.ResumeLayout(false);
            this.panelOptions.ResumeLayout(false);
            this.panelOptions.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private treeview.TreeViewExplorerPanel treeViewExplorerPanel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.TableLayoutPanel tableLayoutFilesSettings;
        private System.Windows.Forms.Panel panelOptions;
        private System.Windows.Forms.RadioButton rdbExcludeFile;
        private System.Windows.Forms.RadioButton rdbUploadFile;
        private System.Windows.Forms.RadioButton rdbConvertToExercises;
        private System.Windows.Forms.Button cmdApplyToAllFiles;
        private System.Windows.Forms.RadioButton rdbConvertToPDF;
        private FileOptionsUploadPanel fileOptionsUploadPanel;
        private FileOptionsExercisePanel fileOptionsExercisePanel;
        private FileOptionsPDFPanel fileOptionsPDFPanel;
        private FileOptionsPDFPanelForExcel fileOptionsPDFPanelForExcel;
        private FileOptionsPDFPanelForPowerPoint fileOptionsPDFPanelForPowerPoint;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button cmdHelp;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.Button cmdOpenFolder;
        private System.Windows.Forms.Button cmdReload;
    }
}
