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
    partial class ContainerFrame
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
            this.components = new System.ComponentModel.Container();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.panelMenuLateral = new System.Windows.Forms.Panel();
            this.panelSubMenuAyuda = new System.Windows.Forms.Panel();
            this.btnAcercaDeLMSDA = new System.Windows.Forms.Button();
            this.btnBuscarActualizaciones = new System.Windows.Forms.Button();
            this.btnAyuda2 = new System.Windows.Forms.Button();
            this.btnAyuda = new System.Windows.Forms.Button();
            this.panelSubMenuEditar = new System.Windows.Forms.Panel();
            this.btnAdministrarMaterias = new System.Windows.Forms.Button();
            this.btnPreferencias = new System.Windows.Forms.Button();
            this.btnEditar = new System.Windows.Forms.Button();
            this.panelSubMenuAcciones = new System.Windows.Forms.Panel();
            this.btnSalir = new System.Windows.Forms.Button();
            this.btnVerRegistro = new System.Windows.Forms.Button();
            this.btnSubirArchivos = new System.Windows.Forms.Button();
            this.btnCerrarSesion = new System.Windows.Forms.Button();
            this.loginToolStripMenuItem = new System.Windows.Forms.Button();
            this.btnAcciones = new System.Windows.Forms.Button();
            this.panelLogo = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grbDocument = new System.Windows.Forms.GroupBox();
            this.lblDocumentSelected = new System.Windows.Forms.Label();
            this.cmdChooseDocument = new System.Windows.Forms.Button();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabExercises = new System.Windows.Forms.TabPage();
            this.nrRandomQuestions = new System.Windows.Forms.NumericUpDown();
            this.chkRandomQuestions = new System.Windows.Forms.CheckBox();
            this.lblTreeEx04 = new System.Windows.Forms.Label();
            this.lblTreeEx03 = new System.Windows.Forms.Label();
            this.chkSetExerciseInvisible = new System.Windows.Forms.CheckBox();
            this.lblExerciseScanResultsDump = new System.Windows.Forms.Label();
            this.lblExerciseScanResults = new System.Windows.Forms.Label();
            this.lblExerciseErrors = new System.Windows.Forms.Label();
            this.lblTreeEx02 = new System.Windows.Forms.Label();
            this.chkOneQuestionPerPage = new System.Windows.Forms.CheckBox();
            this.lblTreeEx01 = new System.Windows.Forms.Label();
            this.chkNewDocWithExamples = new System.Windows.Forms.CheckBox();
            this.cmdOpenTemplate = new System.Windows.Forms.Button();
            this.cmdReviewExercises = new System.Windows.Forms.Button();
            this.cmdUploadExercises = new System.Windows.Forms.Button();
            this.txtExerciseDump = new System.Windows.Forms.TextBox();
            this.cmdScanDocument = new System.Windows.Forms.Button();
            this.cmdJumpToError = new System.Windows.Forms.Button();
            this.tabPDF = new System.Windows.Forms.TabPage();
            this.cmdsearchpath = new System.Windows.Forms.Button();
            this.txtpathsave = new System.Windows.Forms.TextBox();
            this.chkuploadpathsave = new System.Windows.Forms.CheckBox();
            this.pdfViewer = new PdfiumViewer.PdfViewer();
            this.chkConvertHyperlinksToJavascript = new System.Windows.Forms.CheckBox();
            this.lblTreePdf02 = new System.Windows.Forms.Label();
            this.lblTreePdf01 = new System.Windows.Forms.Label();
            this.rdbPerStyle = new System.Windows.Forms.RadioButton();
            this.rdbPerPage = new System.Windows.Forms.RadioButton();
            this.chkUpload = new System.Windows.Forms.CheckBox();
            this.txtSplit = new System.Windows.Forms.TextBox();
            this.chkSplit = new System.Windows.Forms.CheckBox();
            this.cmdConvertToPDF = new System.Windows.Forms.Button();
            this.documentsDropDownForPDF = new lmsda.gui.DocumentsDropDown();
            this.tabStatistics = new System.Windows.Forms.TabPage();
            this.chkGroups = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lstGroups = new lmsda.gui.listview.DragAndDropListView();
            this.label2 = new System.Windows.Forms.Label();
            this.lstColumns = new lmsda.gui.listview.DragAndDropListView();
            this.lblColumns = new System.Windows.Forms.Label();
            this.chkGenerateAllAttempts = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkOpenExcelFilesAfterConversion = new System.Windows.Forms.CheckBox();
            this.chkStatisticsCreateSubFolder = new System.Windows.Forms.CheckBox();
            this.chkCalculateExerciseStudentDetails = new System.Windows.Forms.CheckBox();
            this.chkCalculateResultsPerStudent = new System.Windows.Forms.CheckBox();
            this.lblTreeStat03 = new System.Windows.Forms.Label();
            this.lblTreeStat02 = new System.Windows.Forms.Label();
            this.chkCalculatePercentageMC = new System.Windows.Forms.CheckBox();
            this.chkCPMCShowQuestionTitles = new System.Windows.Forms.CheckBox();
            this.txtDoNotKnow = new System.Windows.Forms.TextBox();
            this.lblDoNotKnow = new System.Windows.Forms.Label();
            this.cmdDownloadStatistics = new System.Windows.Forms.Button();
            this.tabSynchronization = new System.Windows.Forms.TabPage();
            this.tableLayoutStartSync = new System.Windows.Forms.TableLayoutPanel();
            this.lblSynchronisationStatus = new System.Windows.Forms.Label();
            this.pnlSyncButtons = new System.Windows.Forms.Panel();
            this.cmdStartSynchronization = new System.Windows.Forms.Button();
            this.cmdStopSynchronization = new System.Windows.Forms.Button();
            this.progressBarSyncTotal = new System.Windows.Forms.ProgressBar();
            this.subjectFilesSettingsControl = new lmsda.gui.subject.SubjectFilesSettingsControl();
            this.grbLogin = new System.Windows.Forms.GroupBox();
            this.cmdLogin = new System.Windows.Forms.Button();
            this.cmbCourses = new System.Windows.Forms.ComboBox();
            this.lblInformation = new System.Windows.Forms.Label();
            this.cmdLogout = new System.Windows.Forms.Button();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.panelMenuLateral.SuspendLayout();
            this.panelSubMenuAyuda.SuspendLayout();
            this.panelSubMenuEditar.SuspendLayout();
            this.panelSubMenuAcciones.SuspendLayout();
            this.panel1.SuspendLayout();
            this.grbDocument.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabExercises.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nrRandomQuestions)).BeginInit();
            this.tabPDF.SuspendLayout();
            this.tabStatistics.SuspendLayout();
            this.tabSynchronization.SuspendLayout();
            this.tableLayoutStartSync.SuspendLayout();
            this.pnlSyncButtons.SuspendLayout();
            this.grbLogin.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "Word 2007 files|*.docx";
            // 
            // folderBrowserDialog
            // 
            this.folderBrowserDialog.HelpRequest += new System.EventHandler(this.folderBrowserDialog_HelpRequest);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // panelMenuLateral
            // 
            this.panelMenuLateral.AutoScroll = true;
            this.panelMenuLateral.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.panelMenuLateral.Controls.Add(this.panelSubMenuAyuda);
            this.panelMenuLateral.Controls.Add(this.btnAyuda);
            this.panelMenuLateral.Controls.Add(this.panelSubMenuEditar);
            this.panelMenuLateral.Controls.Add(this.btnEditar);
            this.panelMenuLateral.Controls.Add(this.panelSubMenuAcciones);
            this.panelMenuLateral.Controls.Add(this.btnAcciones);
            this.panelMenuLateral.Controls.Add(this.panelLogo);
            this.panelMenuLateral.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelMenuLateral.Location = new System.Drawing.Point(0, 0);
            this.panelMenuLateral.Margin = new System.Windows.Forms.Padding(5);
            this.panelMenuLateral.Name = "panelMenuLateral";
            this.panelMenuLateral.Size = new System.Drawing.Size(333, 686);
            this.panelMenuLateral.TabIndex = 19;
            // 
            // panelSubMenuAyuda
            // 
            this.panelSubMenuAyuda.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(198)))), ((int)(((byte)(218)))));
            this.panelSubMenuAyuda.Controls.Add(this.btnAcercaDeLMSDA);
            this.panelSubMenuAyuda.Controls.Add(this.btnBuscarActualizaciones);
            this.panelSubMenuAyuda.Controls.Add(this.btnAyuda2);
            this.panelSubMenuAyuda.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubMenuAyuda.Location = new System.Drawing.Point(0, 671);
            this.panelSubMenuAyuda.Margin = new System.Windows.Forms.Padding(4);
            this.panelSubMenuAyuda.Name = "panelSubMenuAyuda";
            this.panelSubMenuAyuda.Size = new System.Drawing.Size(316, 154);
            this.panelSubMenuAyuda.TabIndex = 6;
            // 
            // btnAcercaDeLMSDA
            // 
            this.btnAcercaDeLMSDA.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAcercaDeLMSDA.FlatAppearance.BorderSize = 0;
            this.btnAcercaDeLMSDA.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnAcercaDeLMSDA.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnAcercaDeLMSDA.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcercaDeLMSDA.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAcercaDeLMSDA.ForeColor = System.Drawing.Color.White;
            this.btnAcercaDeLMSDA.Image = global::lmsda.Properties.Resources.acerca_de;
            this.btnAcercaDeLMSDA.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAcercaDeLMSDA.Location = new System.Drawing.Point(0, 98);
            this.btnAcercaDeLMSDA.Margin = new System.Windows.Forms.Padding(4);
            this.btnAcercaDeLMSDA.Name = "btnAcercaDeLMSDA";
            this.btnAcercaDeLMSDA.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnAcercaDeLMSDA.Size = new System.Drawing.Size(316, 49);
            this.btnAcercaDeLMSDA.TabIndex = 2;
            this.btnAcercaDeLMSDA.Text = " Acerca de LMSDA";
            this.btnAcercaDeLMSDA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAcercaDeLMSDA.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAcercaDeLMSDA.UseVisualStyleBackColor = true;
            this.btnAcercaDeLMSDA.Click += new System.EventHandler(this.btnAcercaDeLMSDA_Click);
            // 
            // btnBuscarActualizaciones
            // 
            this.btnBuscarActualizaciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnBuscarActualizaciones.FlatAppearance.BorderSize = 0;
            this.btnBuscarActualizaciones.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnBuscarActualizaciones.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnBuscarActualizaciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBuscarActualizaciones.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarActualizaciones.ForeColor = System.Drawing.Color.White;
            this.btnBuscarActualizaciones.Image = global::lmsda.Properties.Resources.actualizar;
            this.btnBuscarActualizaciones.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscarActualizaciones.Location = new System.Drawing.Point(0, 49);
            this.btnBuscarActualizaciones.Margin = new System.Windows.Forms.Padding(4);
            this.btnBuscarActualizaciones.Name = "btnBuscarActualizaciones";
            this.btnBuscarActualizaciones.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnBuscarActualizaciones.Size = new System.Drawing.Size(316, 49);
            this.btnBuscarActualizaciones.TabIndex = 1;
            this.btnBuscarActualizaciones.Text = " Buscar Actualizaciones";
            this.btnBuscarActualizaciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnBuscarActualizaciones.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBuscarActualizaciones.UseVisualStyleBackColor = true;
            this.btnBuscarActualizaciones.Click += new System.EventHandler(this.btnBuscarActualizaciones_Click);
            // 
            // btnAyuda2
            // 
            this.btnAyuda2.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAyuda2.FlatAppearance.BorderSize = 0;
            this.btnAyuda2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnAyuda2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnAyuda2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAyuda2.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAyuda2.ForeColor = System.Drawing.Color.White;
            this.btnAyuda2.Image = global::lmsda.Properties.Resources.ayuda__1_;
            this.btnAyuda2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAyuda2.Location = new System.Drawing.Point(0, 0);
            this.btnAyuda2.Margin = new System.Windows.Forms.Padding(4);
            this.btnAyuda2.Name = "btnAyuda2";
            this.btnAyuda2.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnAyuda2.Size = new System.Drawing.Size(316, 49);
            this.btnAyuda2.TabIndex = 0;
            this.btnAyuda2.Text = " Ayuda";
            this.btnAyuda2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAyuda2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAyuda2.UseVisualStyleBackColor = true;
            this.btnAyuda2.Click += new System.EventHandler(this.btnAyuda2_Click);
            // 
            // btnAyuda
            // 
            this.btnAyuda.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAyuda.FlatAppearance.BorderSize = 0;
            this.btnAyuda.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAyuda.Font = new System.Drawing.Font("Gill Sans MT", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAyuda.ForeColor = System.Drawing.Color.White;
            this.btnAyuda.Image = global::lmsda.Properties.Resources.ayuda1;
            this.btnAyuda.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAyuda.Location = new System.Drawing.Point(0, 616);
            this.btnAyuda.Margin = new System.Windows.Forms.Padding(4);
            this.btnAyuda.Name = "btnAyuda";
            this.btnAyuda.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.btnAyuda.Size = new System.Drawing.Size(316, 55);
            this.btnAyuda.TabIndex = 5;
            this.btnAyuda.Text = " Ayuda";
            this.btnAyuda.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAyuda.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAyuda.UseVisualStyleBackColor = true;
            this.btnAyuda.Click += new System.EventHandler(this.btnAyuda_Click);
            // 
            // panelSubMenuEditar
            // 
            this.panelSubMenuEditar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(198)))), ((int)(((byte)(218)))));
            this.panelSubMenuEditar.Controls.Add(this.btnAdministrarMaterias);
            this.panelSubMenuEditar.Controls.Add(this.btnPreferencias);
            this.panelSubMenuEditar.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubMenuEditar.Location = new System.Drawing.Point(0, 511);
            this.panelSubMenuEditar.Margin = new System.Windows.Forms.Padding(4);
            this.panelSubMenuEditar.Name = "panelSubMenuEditar";
            this.panelSubMenuEditar.Size = new System.Drawing.Size(316, 105);
            this.panelSubMenuEditar.TabIndex = 4;
            // 
            // btnAdministrarMaterias
            // 
            this.btnAdministrarMaterias.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAdministrarMaterias.FlatAppearance.BorderSize = 0;
            this.btnAdministrarMaterias.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnAdministrarMaterias.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnAdministrarMaterias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdministrarMaterias.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdministrarMaterias.ForeColor = System.Drawing.Color.White;
            this.btnAdministrarMaterias.Image = global::lmsda.Properties.Resources.administracion;
            this.btnAdministrarMaterias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdministrarMaterias.Location = new System.Drawing.Point(0, 49);
            this.btnAdministrarMaterias.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdministrarMaterias.Name = "btnAdministrarMaterias";
            this.btnAdministrarMaterias.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnAdministrarMaterias.Size = new System.Drawing.Size(316, 49);
            this.btnAdministrarMaterias.TabIndex = 1;
            this.btnAdministrarMaterias.Text = " Administrar Materias";
            this.btnAdministrarMaterias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAdministrarMaterias.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAdministrarMaterias.UseVisualStyleBackColor = true;
            this.btnAdministrarMaterias.Click += new System.EventHandler(this.btnAdministrarMaterias_Click);
            // 
            // btnPreferencias
            // 
            this.btnPreferencias.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnPreferencias.FlatAppearance.BorderSize = 0;
            this.btnPreferencias.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnPreferencias.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnPreferencias.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreferencias.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreferencias.ForeColor = System.Drawing.Color.White;
            this.btnPreferencias.Image = global::lmsda.Properties.Resources.preferencia;
            this.btnPreferencias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreferencias.Location = new System.Drawing.Point(0, 0);
            this.btnPreferencias.Margin = new System.Windows.Forms.Padding(4);
            this.btnPreferencias.Name = "btnPreferencias";
            this.btnPreferencias.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnPreferencias.Size = new System.Drawing.Size(316, 49);
            this.btnPreferencias.TabIndex = 0;
            this.btnPreferencias.Text = " Preferencias";
            this.btnPreferencias.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnPreferencias.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPreferencias.UseVisualStyleBackColor = true;
            this.btnPreferencias.Click += new System.EventHandler(this.btnPreferencias_Click);
            // 
            // btnEditar
            // 
            this.btnEditar.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnEditar.FlatAppearance.BorderSize = 0;
            this.btnEditar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEditar.Font = new System.Drawing.Font("Gill Sans MT", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEditar.ForeColor = System.Drawing.Color.White;
            this.btnEditar.Image = global::lmsda.Properties.Resources.lapiz;
            this.btnEditar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditar.Location = new System.Drawing.Point(0, 456);
            this.btnEditar.Margin = new System.Windows.Forms.Padding(4);
            this.btnEditar.Name = "btnEditar";
            this.btnEditar.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.btnEditar.Size = new System.Drawing.Size(316, 55);
            this.btnEditar.TabIndex = 3;
            this.btnEditar.Text = " Editar";
            this.btnEditar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnEditar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditar.UseVisualStyleBackColor = true;
            this.btnEditar.Click += new System.EventHandler(this.btnEditar_Click);
            // 
            // panelSubMenuAcciones
            // 
            this.panelSubMenuAcciones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(198)))), ((int)(((byte)(218)))));
            this.panelSubMenuAcciones.Controls.Add(this.btnSalir);
            this.panelSubMenuAcciones.Controls.Add(this.btnVerRegistro);
            this.panelSubMenuAcciones.Controls.Add(this.btnSubirArchivos);
            this.panelSubMenuAcciones.Controls.Add(this.btnCerrarSesion);
            this.panelSubMenuAcciones.Controls.Add(this.loginToolStripMenuItem);
            this.panelSubMenuAcciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSubMenuAcciones.Location = new System.Drawing.Point(0, 204);
            this.panelSubMenuAcciones.Margin = new System.Windows.Forms.Padding(4);
            this.panelSubMenuAcciones.Name = "panelSubMenuAcciones";
            this.panelSubMenuAcciones.Size = new System.Drawing.Size(316, 252);
            this.panelSubMenuAcciones.TabIndex = 2;
            // 
            // btnSalir
            // 
            this.btnSalir.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSalir.FlatAppearance.BorderSize = 0;
            this.btnSalir.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnSalir.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.White;
            this.btnSalir.Image = global::lmsda.Properties.Resources.salir;
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Location = new System.Drawing.Point(0, 196);
            this.btnSalir.Margin = new System.Windows.Forms.Padding(4);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnSalir.Size = new System.Drawing.Size(316, 49);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = " Salir";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSalir.UseVisualStyleBackColor = true;
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            // 
            // btnVerRegistro
            // 
            this.btnVerRegistro.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnVerRegistro.FlatAppearance.BorderSize = 0;
            this.btnVerRegistro.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnVerRegistro.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnVerRegistro.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVerRegistro.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerRegistro.ForeColor = System.Drawing.Color.White;
            this.btnVerRegistro.Image = global::lmsda.Properties.Resources.documento;
            this.btnVerRegistro.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerRegistro.Location = new System.Drawing.Point(0, 147);
            this.btnVerRegistro.Margin = new System.Windows.Forms.Padding(4);
            this.btnVerRegistro.Name = "btnVerRegistro";
            this.btnVerRegistro.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnVerRegistro.Size = new System.Drawing.Size(316, 49);
            this.btnVerRegistro.TabIndex = 3;
            this.btnVerRegistro.Text = " Ver Registro";
            this.btnVerRegistro.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVerRegistro.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnVerRegistro.UseVisualStyleBackColor = true;
            this.btnVerRegistro.Click += new System.EventHandler(this.btnVerRegistro_Click);
            // 
            // btnSubirArchivos
            // 
            this.btnSubirArchivos.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnSubirArchivos.Enabled = false;
            this.btnSubirArchivos.FlatAppearance.BorderSize = 0;
            this.btnSubirArchivos.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnSubirArchivos.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnSubirArchivos.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubirArchivos.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubirArchivos.ForeColor = System.Drawing.Color.White;
            this.btnSubirArchivos.Image = global::lmsda.Properties.Resources.subir;
            this.btnSubirArchivos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSubirArchivos.Location = new System.Drawing.Point(0, 98);
            this.btnSubirArchivos.Margin = new System.Windows.Forms.Padding(4);
            this.btnSubirArchivos.Name = "btnSubirArchivos";
            this.btnSubirArchivos.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnSubirArchivos.Size = new System.Drawing.Size(316, 49);
            this.btnSubirArchivos.TabIndex = 2;
            this.btnSubirArchivos.Text = " Subir Archivos";
            this.btnSubirArchivos.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSubirArchivos.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSubirArchivos.UseVisualStyleBackColor = true;
            this.btnSubirArchivos.Click += new System.EventHandler(this.btnSubirArchivos_Click);
            // 
            // btnCerrarSesion
            // 
            this.btnCerrarSesion.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCerrarSesion.Enabled = false;
            this.btnCerrarSesion.FlatAppearance.BorderSize = 0;
            this.btnCerrarSesion.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.btnCerrarSesion.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.btnCerrarSesion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCerrarSesion.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCerrarSesion.ForeColor = System.Drawing.Color.White;
            this.btnCerrarSesion.Image = global::lmsda.Properties.Resources.cerrar_sesion;
            this.btnCerrarSesion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarSesion.Location = new System.Drawing.Point(0, 49);
            this.btnCerrarSesion.Margin = new System.Windows.Forms.Padding(4);
            this.btnCerrarSesion.Name = "btnCerrarSesion";
            this.btnCerrarSesion.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.btnCerrarSesion.Size = new System.Drawing.Size(316, 49);
            this.btnCerrarSesion.TabIndex = 1;
            this.btnCerrarSesion.Text = " Cerrar Sesion";
            this.btnCerrarSesion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCerrarSesion.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCerrarSesion.UseVisualStyleBackColor = true;
            this.btnCerrarSesion.Click += new System.EventHandler(this.btnCerrarSesion_Click);
            // 
            // loginToolStripMenuItem
            // 
            this.loginToolStripMenuItem.Dock = System.Windows.Forms.DockStyle.Top;
            this.loginToolStripMenuItem.FlatAppearance.BorderSize = 0;
            this.loginToolStripMenuItem.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(90)))), ((int)(((byte)(33)))));
            this.loginToolStripMenuItem.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(165)))), ((int)(((byte)(114)))), ((int)(((byte)(111)))));
            this.loginToolStripMenuItem.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.loginToolStripMenuItem.Font = new System.Drawing.Font("Gill Sans MT", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loginToolStripMenuItem.ForeColor = System.Drawing.Color.White;
            this.loginToolStripMenuItem.Image = global::lmsda.Properties.Resources.iniciar_sesion__1_;
            this.loginToolStripMenuItem.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loginToolStripMenuItem.Location = new System.Drawing.Point(0, 0);
            this.loginToolStripMenuItem.Margin = new System.Windows.Forms.Padding(4);
            this.loginToolStripMenuItem.Name = "loginToolStripMenuItem";
            this.loginToolStripMenuItem.Padding = new System.Windows.Forms.Padding(47, 0, 0, 0);
            this.loginToolStripMenuItem.Size = new System.Drawing.Size(316, 49);
            this.loginToolStripMenuItem.TabIndex = 0;
            this.loginToolStripMenuItem.Text = " Inicio de Sesion";
            this.loginToolStripMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.loginToolStripMenuItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.loginToolStripMenuItem.UseVisualStyleBackColor = true;
            this.loginToolStripMenuItem.Click += new System.EventHandler(this.btnInicioSesion_Click);
            // 
            // btnAcciones
            // 
            this.btnAcciones.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnAcciones.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnAcciones.FlatAppearance.BorderSize = 0;
            this.btnAcciones.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAcciones.Font = new System.Drawing.Font("Gill Sans MT", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAcciones.ForeColor = System.Drawing.Color.White;
            this.btnAcciones.Image = global::lmsda.Properties.Resources.tocar;
            this.btnAcciones.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAcciones.Location = new System.Drawing.Point(0, 149);
            this.btnAcciones.Margin = new System.Windows.Forms.Padding(4);
            this.btnAcciones.Name = "btnAcciones";
            this.btnAcciones.Padding = new System.Windows.Forms.Padding(13, 0, 0, 0);
            this.btnAcciones.Size = new System.Drawing.Size(316, 55);
            this.btnAcciones.TabIndex = 1;
            this.btnAcciones.Text = " Acciones";
            this.btnAcciones.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAcciones.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAcciones.UseVisualStyleBackColor = true;
            this.btnAcciones.Click += new System.EventHandler(this.btnAcciones_Click);
            // 
            // panelLogo
            // 
            this.panelLogo.BackgroundImage = global::lmsda.Properties.Resources.lms_logo;
            this.panelLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLogo.Location = new System.Drawing.Point(0, 0);
            this.panelLogo.Margin = new System.Windows.Forms.Padding(4);
            this.panelLogo.Name = "panelLogo";
            this.panelLogo.Size = new System.Drawing.Size(316, 149);
            this.panelLogo.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(198)))), ((int)(((byte)(218)))));
            this.panel1.Controls.Add(this.lblStatus);
            this.panel1.Controls.Add(this.grbDocument);
            this.panel1.Controls.Add(this.tabs);
            this.panel1.Controls.Add(this.grbLogin);
            this.panel1.Controls.Add(this.txtLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(333, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(833, 686);
            this.panel1.TabIndex = 20;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblStatus.Location = new System.Drawing.Point(17, 658);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(125, 25);
            this.lblStatus.TabIndex = 23;
            this.lblStatus.Tag = "ready";
            this.lblStatus.Text = "Gereed";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grbDocument
            // 
            this.grbDocument.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbDocument.Controls.Add(this.lblDocumentSelected);
            this.grbDocument.Controls.Add(this.cmdChooseDocument);
            this.grbDocument.Location = new System.Drawing.Point(12, 55);
            this.grbDocument.Margin = new System.Windows.Forms.Padding(4);
            this.grbDocument.Name = "grbDocument";
            this.grbDocument.Padding = new System.Windows.Forms.Padding(4);
            this.grbDocument.Size = new System.Drawing.Size(793, 50);
            this.grbDocument.TabIndex = 22;
            this.grbDocument.TabStop = false;
            // 
            // lblDocumentSelected
            // 
            this.lblDocumentSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDocumentSelected.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblDocumentSelected.Location = new System.Drawing.Point(176, 20);
            this.lblDocumentSelected.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDocumentSelected.Name = "lblDocumentSelected";
            this.lblDocumentSelected.Size = new System.Drawing.Size(608, 22);
            this.lblDocumentSelected.TabIndex = 18;
            this.lblDocumentSelected.Text = "No document selected";
            this.lblDocumentSelected.TextChanged += new System.EventHandler(this.lblDocumentSelected_TextChanged);
            // 
            // cmdChooseDocument
            // 
            this.cmdChooseDocument.BackColor = System.Drawing.Color.LightGray;
            this.cmdChooseDocument.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdChooseDocument.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdChooseDocument.Location = new System.Drawing.Point(7, 15);
            this.cmdChooseDocument.Margin = new System.Windows.Forms.Padding(4);
            this.cmdChooseDocument.Name = "cmdChooseDocument";
            this.cmdChooseDocument.Size = new System.Drawing.Size(161, 28);
            this.cmdChooseDocument.TabIndex = 3;
            this.cmdChooseDocument.Tag = "select_document";
            this.cmdChooseDocument.Text = "select_document";
            this.cmdChooseDocument.UseVisualStyleBackColor = false;
            this.cmdChooseDocument.Click += new System.EventHandler(this.cmdChooseDocument_Click_1);
            // 
            // tabs
            // 
            this.tabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabs.Controls.Add(this.tabExercises);
            this.tabs.Controls.Add(this.tabPDF);
            this.tabs.Controls.Add(this.tabStatistics);
            this.tabs.Controls.Add(this.tabSynchronization);
            this.tabs.Location = new System.Drawing.Point(12, 113);
            this.tabs.Margin = new System.Windows.Forms.Padding(4);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(792, 546);
            this.tabs.TabIndex = 19;
            // 
            // tabExercises
            // 
            this.tabExercises.Controls.Add(this.nrRandomQuestions);
            this.tabExercises.Controls.Add(this.chkRandomQuestions);
            this.tabExercises.Controls.Add(this.lblTreeEx04);
            this.tabExercises.Controls.Add(this.lblTreeEx03);
            this.tabExercises.Controls.Add(this.chkSetExerciseInvisible);
            this.tabExercises.Controls.Add(this.lblExerciseScanResultsDump);
            this.tabExercises.Controls.Add(this.lblExerciseScanResults);
            this.tabExercises.Controls.Add(this.lblExerciseErrors);
            this.tabExercises.Controls.Add(this.lblTreeEx02);
            this.tabExercises.Controls.Add(this.chkOneQuestionPerPage);
            this.tabExercises.Controls.Add(this.lblTreeEx01);
            this.tabExercises.Controls.Add(this.chkNewDocWithExamples);
            this.tabExercises.Controls.Add(this.cmdOpenTemplate);
            this.tabExercises.Controls.Add(this.cmdReviewExercises);
            this.tabExercises.Controls.Add(this.cmdUploadExercises);
            this.tabExercises.Controls.Add(this.txtExerciseDump);
            this.tabExercises.Controls.Add(this.cmdScanDocument);
            this.tabExercises.Controls.Add(this.cmdJumpToError);
            this.tabExercises.Location = new System.Drawing.Point(4, 25);
            this.tabExercises.Margin = new System.Windows.Forms.Padding(4);
            this.tabExercises.Name = "tabExercises";
            this.tabExercises.Padding = new System.Windows.Forms.Padding(4);
            this.tabExercises.Size = new System.Drawing.Size(784, 517);
            this.tabExercises.TabIndex = 0;
            this.tabExercises.Tag = "exercises";
            this.tabExercises.Text = "exercises";
            this.tabExercises.UseVisualStyleBackColor = true;
            // 
            // nrRandomQuestions
            // 
            this.nrRandomQuestions.Location = new System.Drawing.Point(101, 390);
            this.nrRandomQuestions.Margin = new System.Windows.Forms.Padding(4);
            this.nrRandomQuestions.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nrRandomQuestions.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nrRandomQuestions.Name = "nrRandomQuestions";
            this.nrRandomQuestions.Size = new System.Drawing.Size(160, 22);
            this.nrRandomQuestions.TabIndex = 45;
            this.nrRandomQuestions.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nrRandomQuestions.Visible = false;
            // 
            // chkRandomQuestions
            // 
            this.chkRandomQuestions.AutoSize = true;
            this.chkRandomQuestions.Location = new System.Drawing.Point(36, 362);
            this.chkRandomQuestions.Margin = new System.Windows.Forms.Padding(4);
            this.chkRandomQuestions.Name = "chkRandomQuestions";
            this.chkRandomQuestions.Size = new System.Drawing.Size(166, 20);
            this.chkRandomQuestions.TabIndex = 44;
            this.chkRandomQuestions.Tag = "use_random_questions";
            this.chkRandomQuestions.Text = "use_random_questions";
            this.chkRandomQuestions.UseVisualStyleBackColor = true;
            this.chkRandomQuestions.CheckedChanged += new System.EventHandler(this.chkRandomQuestions_CheckedChanged_1);
            // 
            // lblTreeEx04
            // 
            this.lblTreeEx04.AutoSize = true;
            this.lblTreeEx04.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeEx04.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeEx04.Location = new System.Drawing.Point(16, 363);
            this.lblTreeEx04.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeEx04.Name = "lblTreeEx04";
            this.lblTreeEx04.Size = new System.Drawing.Size(10, 16);
            this.lblTreeEx04.TabIndex = 41;
            this.lblTreeEx04.Text = " ";
            // 
            // lblTreeEx03
            // 
            this.lblTreeEx03.AutoSize = true;
            this.lblTreeEx03.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeEx03.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeEx03.Location = new System.Drawing.Point(16, 319);
            this.lblTreeEx03.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeEx03.Name = "lblTreeEx03";
            this.lblTreeEx03.Size = new System.Drawing.Size(10, 16);
            this.lblTreeEx03.TabIndex = 41;
            this.lblTreeEx03.Text = " ";
            // 
            // chkSetExerciseInvisible
            // 
            this.chkSetExerciseInvisible.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSetExerciseInvisible.Location = new System.Drawing.Point(36, 318);
            this.chkSetExerciseInvisible.Margin = new System.Windows.Forms.Padding(4);
            this.chkSetExerciseInvisible.Name = "chkSetExerciseInvisible";
            this.chkSetExerciseInvisible.Size = new System.Drawing.Size(245, 37);
            this.chkSetExerciseInvisible.TabIndex = 40;
            this.chkSetExerciseInvisible.Tag = "set_exercises_invisible";
            this.chkSetExerciseInvisible.Text = "set_exercises_invisible";
            this.chkSetExerciseInvisible.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.chkSetExerciseInvisible.UseVisualStyleBackColor = true;
            // 
            // lblExerciseScanResultsDump
            // 
            this.lblExerciseScanResultsDump.Location = new System.Drawing.Point(340, 39);
            this.lblExerciseScanResultsDump.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExerciseScanResultsDump.Name = "lblExerciseScanResultsDump";
            this.lblExerciseScanResultsDump.Size = new System.Drawing.Size(255, 71);
            this.lblExerciseScanResultsDump.TabIndex = 39;
            this.lblExerciseScanResultsDump.Text = "exercises: x\r\nquestions: x\r\nerrors: x:\r\nwarnings: x";
            // 
            // lblExerciseScanResults
            // 
            this.lblExerciseScanResults.AutoSize = true;
            this.lblExerciseScanResults.Location = new System.Drawing.Point(316, 16);
            this.lblExerciseScanResults.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExerciseScanResults.Name = "lblExerciseScanResults";
            this.lblExerciseScanResults.Size = new System.Drawing.Size(88, 16);
            this.lblExerciseScanResults.TabIndex = 38;
            this.lblExerciseScanResults.Tag = "scan_results :";
            this.lblExerciseScanResults.Text = "scan_results :";
            // 
            // lblExerciseErrors
            // 
            this.lblExerciseErrors.AutoSize = true;
            this.lblExerciseErrors.Location = new System.Drawing.Point(316, 111);
            this.lblExerciseErrors.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblExerciseErrors.Name = "lblExerciseErrors";
            this.lblExerciseErrors.Size = new System.Drawing.Size(138, 16);
            this.lblExerciseErrors.TabIndex = 37;
            this.lblExerciseErrors.Tag = "errors_and_warnings :";
            this.lblExerciseErrors.Text = "errors_and_warnings :";
            // 
            // lblTreeEx02
            // 
            this.lblTreeEx02.AutoSize = true;
            this.lblTreeEx02.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeEx02.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeEx02.Location = new System.Drawing.Point(16, 290);
            this.lblTreeEx02.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeEx02.Name = "lblTreeEx02";
            this.lblTreeEx02.Size = new System.Drawing.Size(10, 16);
            this.lblTreeEx02.TabIndex = 36;
            this.lblTreeEx02.Text = " ";
            // 
            // chkOneQuestionPerPage
            // 
            this.chkOneQuestionPerPage.AutoSize = true;
            this.chkOneQuestionPerPage.Location = new System.Drawing.Point(36, 289);
            this.chkOneQuestionPerPage.Margin = new System.Windows.Forms.Padding(4);
            this.chkOneQuestionPerPage.Name = "chkOneQuestionPerPage";
            this.chkOneQuestionPerPage.Size = new System.Drawing.Size(173, 20);
            this.chkOneQuestionPerPage.TabIndex = 11;
            this.chkOneQuestionPerPage.Tag = "one_question_per_page";
            this.chkOneQuestionPerPage.Text = "one_question_per_page";
            this.chkOneQuestionPerPage.UseVisualStyleBackColor = true;
            this.chkOneQuestionPerPage.CheckedChanged += new System.EventHandler(this.chkOneQuestionPerPage_CheckedChanged_1);
            // 
            // lblTreeEx01
            // 
            this.lblTreeEx01.AutoSize = true;
            this.lblTreeEx01.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeEx01.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeEx01.Location = new System.Drawing.Point(16, 47);
            this.lblTreeEx01.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeEx01.Name = "lblTreeEx01";
            this.lblTreeEx01.Size = new System.Drawing.Size(10, 16);
            this.lblTreeEx01.TabIndex = 35;
            this.lblTreeEx01.Text = " ";
            // 
            // chkNewDocWithExamples
            // 
            this.chkNewDocWithExamples.AutoSize = true;
            this.chkNewDocWithExamples.Location = new System.Drawing.Point(36, 46);
            this.chkNewDocWithExamples.Margin = new System.Windows.Forms.Padding(4);
            this.chkNewDocWithExamples.Name = "chkNewDocWithExamples";
            this.chkNewDocWithExamples.Size = new System.Drawing.Size(175, 20);
            this.chkNewDocWithExamples.TabIndex = 12;
            this.chkNewDocWithExamples.Tag = "new_doc_with_examples";
            this.chkNewDocWithExamples.Text = "new_doc_with_examples";
            this.chkNewDocWithExamples.UseVisualStyleBackColor = true;
            // 
            // cmdOpenTemplate
            // 
            this.cmdOpenTemplate.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.cmdOpenTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdOpenTemplate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdOpenTemplate.Location = new System.Drawing.Point(8, 10);
            this.cmdOpenTemplate.Margin = new System.Windows.Forms.Padding(4);
            this.cmdOpenTemplate.Name = "cmdOpenTemplate";
            this.cmdOpenTemplate.Size = new System.Drawing.Size(273, 28);
            this.cmdOpenTemplate.TabIndex = 11;
            this.cmdOpenTemplate.Tag = "new_from_template";
            this.cmdOpenTemplate.Text = "new_from_template";
            this.cmdOpenTemplate.UseVisualStyleBackColor = false;
            this.cmdOpenTemplate.Click += new System.EventHandler(this.cmdOpenTemplate_Click_1);
            // 
            // cmdReviewExercises
            // 
            this.cmdReviewExercises.BackColor = System.Drawing.Color.PaleTurquoise;
            this.cmdReviewExercises.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdReviewExercises.Location = new System.Drawing.Point(8, 182);
            this.cmdReviewExercises.Margin = new System.Windows.Forms.Padding(4);
            this.cmdReviewExercises.Name = "cmdReviewExercises";
            this.cmdReviewExercises.Size = new System.Drawing.Size(304, 28);
            this.cmdReviewExercises.TabIndex = 6;
            this.cmdReviewExercises.Tag = "review_exercises";
            this.cmdReviewExercises.Text = "review_exercises";
            this.cmdReviewExercises.UseVisualStyleBackColor = false;
            this.cmdReviewExercises.Click += new System.EventHandler(this.cmdReviewExercises_Click_1);
            // 
            // cmdUploadExercises
            // 
            this.cmdUploadExercises.BackColor = System.Drawing.Color.PaleTurquoise;
            this.cmdUploadExercises.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdUploadExercises.Location = new System.Drawing.Point(8, 254);
            this.cmdUploadExercises.Margin = new System.Windows.Forms.Padding(4);
            this.cmdUploadExercises.Name = "cmdUploadExercises";
            this.cmdUploadExercises.Size = new System.Drawing.Size(304, 28);
            this.cmdUploadExercises.TabIndex = 8;
            this.cmdUploadExercises.Tag = "upload_exercises";
            this.cmdUploadExercises.Text = "upload_exercises";
            this.cmdUploadExercises.UseVisualStyleBackColor = false;
            this.cmdUploadExercises.Click += new System.EventHandler(this.cmdUploadExercises_Click_1);
            // 
            // txtExerciseDump
            // 
            this.txtExerciseDump.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExerciseDump.BackColor = System.Drawing.Color.White;
            this.txtExerciseDump.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtExerciseDump.Location = new System.Drawing.Point(320, 146);
            this.txtExerciseDump.Margin = new System.Windows.Forms.Padding(4);
            this.txtExerciseDump.Multiline = true;
            this.txtExerciseDump.Name = "txtExerciseDump";
            this.txtExerciseDump.ReadOnly = true;
            this.txtExerciseDump.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtExerciseDump.Size = new System.Drawing.Size(452, 446);
            this.txtExerciseDump.TabIndex = 6;
            this.txtExerciseDump.TabStop = false;
            // 
            // cmdScanDocument
            // 
            this.cmdScanDocument.BackColor = System.Drawing.Color.PaleTurquoise;
            this.cmdScanDocument.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdScanDocument.Location = new System.Drawing.Point(8, 146);
            this.cmdScanDocument.Margin = new System.Windows.Forms.Padding(4);
            this.cmdScanDocument.Name = "cmdScanDocument";
            this.cmdScanDocument.Size = new System.Drawing.Size(304, 28);
            this.cmdScanDocument.TabIndex = 5;
            this.cmdScanDocument.Tag = "scan_document";
            this.cmdScanDocument.Text = "scan_document";
            this.cmdScanDocument.UseVisualStyleBackColor = false;
            this.cmdScanDocument.Click += new System.EventHandler(this.cmdScanDocument_Click_1);
            // 
            // cmdJumpToError
            // 
            this.cmdJumpToError.BackColor = System.Drawing.Color.PaleTurquoise;
            this.cmdJumpToError.Location = new System.Drawing.Point(8, 218);
            this.cmdJumpToError.Margin = new System.Windows.Forms.Padding(4);
            this.cmdJumpToError.Name = "cmdJumpToError";
            this.cmdJumpToError.Size = new System.Drawing.Size(304, 28);
            this.cmdJumpToError.TabIndex = 7;
            this.cmdJumpToError.Tag = "jump_to_error";
            this.cmdJumpToError.Text = "jump_to_error";
            this.cmdJumpToError.UseVisualStyleBackColor = false;
            this.cmdJumpToError.Click += new System.EventHandler(this.cmdJumpToError_Click_1);
            // 
            // tabPDF
            // 
            this.tabPDF.Controls.Add(this.cmdsearchpath);
            this.tabPDF.Controls.Add(this.txtpathsave);
            this.tabPDF.Controls.Add(this.chkuploadpathsave);
            this.tabPDF.Controls.Add(this.pdfViewer);
            this.tabPDF.Controls.Add(this.chkConvertHyperlinksToJavascript);
            this.tabPDF.Controls.Add(this.lblTreePdf02);
            this.tabPDF.Controls.Add(this.lblTreePdf01);
            this.tabPDF.Controls.Add(this.rdbPerStyle);
            this.tabPDF.Controls.Add(this.rdbPerPage);
            this.tabPDF.Controls.Add(this.chkUpload);
            this.tabPDF.Controls.Add(this.txtSplit);
            this.tabPDF.Controls.Add(this.chkSplit);
            this.tabPDF.Controls.Add(this.cmdConvertToPDF);
            this.tabPDF.Controls.Add(this.documentsDropDownForPDF);
            this.tabPDF.Location = new System.Drawing.Point(4, 25);
            this.tabPDF.Margin = new System.Windows.Forms.Padding(4);
            this.tabPDF.Name = "tabPDF";
            this.tabPDF.Padding = new System.Windows.Forms.Padding(4);
            this.tabPDF.Size = new System.Drawing.Size(784, 517);
            this.tabPDF.TabIndex = 1;
            this.tabPDF.Tag = "pdf_conversion";
            this.tabPDF.Text = "pdf_conversion";
            this.tabPDF.UseVisualStyleBackColor = true;
            // 
            // cmdsearchpath
            // 
            this.cmdsearchpath.BackColor = System.Drawing.Color.LightGray;
            this.cmdsearchpath.Enabled = false;
            this.cmdsearchpath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdsearchpath.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdsearchpath.Location = new System.Drawing.Point(613, 177);
            this.cmdsearchpath.Margin = new System.Windows.Forms.Padding(4);
            this.cmdsearchpath.Name = "cmdsearchpath";
            this.cmdsearchpath.Size = new System.Drawing.Size(161, 28);
            this.cmdsearchpath.TabIndex = 40;
            this.cmdsearchpath.Tag = "search_path";
            this.cmdsearchpath.Text = "search_path";
            this.cmdsearchpath.UseVisualStyleBackColor = false;
            this.cmdsearchpath.Click += new System.EventHandler(this.cmdsearchpath_Click);
            // 
            // txtpathsave
            // 
            this.txtpathsave.Enabled = false;
            this.txtpathsave.Location = new System.Drawing.Point(267, 180);
            this.txtpathsave.Margin = new System.Windows.Forms.Padding(4);
            this.txtpathsave.Multiline = true;
            this.txtpathsave.Name = "txtpathsave";
            this.txtpathsave.ReadOnly = true;
            this.txtpathsave.Size = new System.Drawing.Size(338, 22);
            this.txtpathsave.TabIndex = 39;
            // 
            // chkuploadpathsave
            // 
            this.chkuploadpathsave.AutoSize = true;
            this.chkuploadpathsave.Location = new System.Drawing.Point(20, 181);
            this.chkuploadpathsave.Margin = new System.Windows.Forms.Padding(4);
            this.chkuploadpathsave.Name = "chkuploadpathsave";
            this.chkuploadpathsave.Size = new System.Drawing.Size(138, 20);
            this.chkuploadpathsave.TabIndex = 38;
            this.chkuploadpathsave.Tag = "upload_path_save";
            this.chkuploadpathsave.Text = "upload_path_save";
            this.chkuploadpathsave.UseVisualStyleBackColor = true;
            this.chkuploadpathsave.CheckedChanged += new System.EventHandler(this.chkuploadpathsave_CheckedChanged);
            // 
            // pdfViewer
            // 
            this.pdfViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pdfViewer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pdfViewer.Location = new System.Drawing.Point(4, 263);
            this.pdfViewer.Margin = new System.Windows.Forms.Padding(4);
            this.pdfViewer.Name = "pdfViewer";
            this.pdfViewer.ShowToolbar = false;
            this.pdfViewer.Size = new System.Drawing.Size(776, 250);
            this.pdfViewer.TabIndex = 37;
            this.pdfViewer.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitWidth;
            // 
            // chkConvertHyperlinksToJavascript
            // 
            this.chkConvertHyperlinksToJavascript.AutoSize = true;
            this.chkConvertHyperlinksToJavascript.Location = new System.Drawing.Point(20, 150);
            this.chkConvertHyperlinksToJavascript.Margin = new System.Windows.Forms.Padding(4);
            this.chkConvertHyperlinksToJavascript.Name = "chkConvertHyperlinksToJavascript";
            this.chkConvertHyperlinksToJavascript.Size = new System.Drawing.Size(221, 20);
            this.chkConvertHyperlinksToJavascript.TabIndex = 36;
            this.chkConvertHyperlinksToJavascript.Tag = "convert_hyperlinks_to_javascript";
            this.chkConvertHyperlinksToJavascript.Text = "convert_hyperlinks_to_javascript";
            this.chkConvertHyperlinksToJavascript.UseVisualStyleBackColor = true;
            // 
            // lblTreePdf02
            // 
            this.lblTreePdf02.AutoSize = true;
            this.lblTreePdf02.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreePdf02.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreePdf02.Location = new System.Drawing.Point(45, 79);
            this.lblTreePdf02.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreePdf02.Name = "lblTreePdf02";
            this.lblTreePdf02.Size = new System.Drawing.Size(10, 16);
            this.lblTreePdf02.TabIndex = 35;
            this.lblTreePdf02.Text = " ";
            // 
            // lblTreePdf01
            // 
            this.lblTreePdf01.AutoSize = true;
            this.lblTreePdf01.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreePdf01.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreePdf01.Location = new System.Drawing.Point(45, 50);
            this.lblTreePdf01.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreePdf01.Name = "lblTreePdf01";
            this.lblTreePdf01.Size = new System.Drawing.Size(10, 16);
            this.lblTreePdf01.TabIndex = 34;
            this.lblTreePdf01.Text = " ";
            // 
            // rdbPerStyle
            // 
            this.rdbPerStyle.AutoSize = true;
            this.rdbPerStyle.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdbPerStyle.Location = new System.Drawing.Point(72, 76);
            this.rdbPerStyle.Margin = new System.Windows.Forms.Padding(4);
            this.rdbPerStyle.Name = "rdbPerStyle";
            this.rdbPerStyle.Size = new System.Drawing.Size(225, 20);
            this.rdbPerStyle.TabIndex = 9;
            this.rdbPerStyle.Tag = "split_per_style_restart_numbering";
            this.rdbPerStyle.Text = "split_per_style_restart_numbering";
            this.rdbPerStyle.UseVisualStyleBackColor = true;
            // 
            // rdbPerPage
            // 
            this.rdbPerPage.AutoSize = true;
            this.rdbPerPage.Checked = true;
            this.rdbPerPage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.rdbPerPage.Location = new System.Drawing.Point(72, 48);
            this.rdbPerPage.Margin = new System.Windows.Forms.Padding(4);
            this.rdbPerPage.Name = "rdbPerPage";
            this.rdbPerPage.Size = new System.Drawing.Size(115, 20);
            this.rdbPerPage.TabIndex = 8;
            this.rdbPerPage.TabStop = true;
            this.rdbPerPage.Tag = "split_per_page";
            this.rdbPerPage.Text = "split_per_page";
            this.rdbPerPage.UseVisualStyleBackColor = true;
            // 
            // chkUpload
            // 
            this.chkUpload.AutoSize = true;
            this.chkUpload.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkUpload.Location = new System.Drawing.Point(20, 115);
            this.chkUpload.Margin = new System.Windows.Forms.Padding(4);
            this.chkUpload.Name = "chkUpload";
            this.chkUpload.Size = new System.Drawing.Size(200, 20);
            this.chkUpload.TabIndex = 10;
            this.chkUpload.Tag = "upload_to_documents_folder";
            this.chkUpload.Text = "upload_to_documents_folder";
            this.chkUpload.UseVisualStyleBackColor = true;
            this.chkUpload.CheckedChanged += new System.EventHandler(this.chkUpload_CheckedChanged_1);
            // 
            // txtSplit
            // 
            this.txtSplit.Enabled = false;
            this.txtSplit.Location = new System.Drawing.Point(267, 17);
            this.txtSplit.Margin = new System.Windows.Forms.Padding(4);
            this.txtSplit.Name = "txtSplit";
            this.txtSplit.Size = new System.Drawing.Size(132, 22);
            this.txtSplit.TabIndex = 7;
            this.txtSplit.Text = "Header 1";
            // 
            // chkSplit
            // 
            this.chkSplit.AutoSize = true;
            this.chkSplit.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.chkSplit.Location = new System.Drawing.Point(20, 17);
            this.chkSplit.Margin = new System.Windows.Forms.Padding(4);
            this.chkSplit.Name = "chkSplit";
            this.chkSplit.Size = new System.Drawing.Size(107, 20);
            this.chkSplit.TabIndex = 6;
            this.chkSplit.Tag = "split_on_style";
            this.chkSplit.Text = "split_on_style";
            this.chkSplit.UseVisualStyleBackColor = true;
            this.chkSplit.CheckedChanged += new System.EventHandler(this.chkSplit_CheckedChanged_1);
            // 
            // cmdConvertToPDF
            // 
            this.cmdConvertToPDF.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(71)))));
            this.cmdConvertToPDF.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdConvertToPDF.Location = new System.Drawing.Point(20, 217);
            this.cmdConvertToPDF.Margin = new System.Windows.Forms.Padding(4);
            this.cmdConvertToPDF.Name = "cmdConvertToPDF";
            this.cmdConvertToPDF.Size = new System.Drawing.Size(213, 28);
            this.cmdConvertToPDF.TabIndex = 5;
            this.cmdConvertToPDF.Tag = "convert_to_pdf";
            this.cmdConvertToPDF.Text = "convert_to_pdf";
            this.cmdConvertToPDF.UseVisualStyleBackColor = false;
            this.cmdConvertToPDF.Click += new System.EventHandler(this.cmdConvertToPDF_Click_1);
            // 
            // documentsDropDownForPDF
            // 
            this.documentsDropDownForPDF.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.documentsDropDownForPDF.Location = new System.Drawing.Point(267, 104);
            this.documentsDropDownForPDF.Margin = new System.Windows.Forms.Padding(4);
            this.documentsDropDownForPDF.Name = "documentsDropDownForPDF";
            this.documentsDropDownForPDF.Size = new System.Drawing.Size(563, 34);
            this.documentsDropDownForPDF.TabIndex = 11;
            // 
            // tabStatistics
            // 
            this.tabStatistics.Controls.Add(this.chkGroups);
            this.tabStatistics.Controls.Add(this.label3);
            this.tabStatistics.Controls.Add(this.lstGroups);
            this.tabStatistics.Controls.Add(this.label2);
            this.tabStatistics.Controls.Add(this.lstColumns);
            this.tabStatistics.Controls.Add(this.lblColumns);
            this.tabStatistics.Controls.Add(this.chkGenerateAllAttempts);
            this.tabStatistics.Controls.Add(this.label1);
            this.tabStatistics.Controls.Add(this.chkOpenExcelFilesAfterConversion);
            this.tabStatistics.Controls.Add(this.chkStatisticsCreateSubFolder);
            this.tabStatistics.Controls.Add(this.chkCalculateExerciseStudentDetails);
            this.tabStatistics.Controls.Add(this.chkCalculateResultsPerStudent);
            this.tabStatistics.Controls.Add(this.lblTreeStat03);
            this.tabStatistics.Controls.Add(this.lblTreeStat02);
            this.tabStatistics.Controls.Add(this.chkCalculatePercentageMC);
            this.tabStatistics.Controls.Add(this.chkCPMCShowQuestionTitles);
            this.tabStatistics.Controls.Add(this.txtDoNotKnow);
            this.tabStatistics.Controls.Add(this.lblDoNotKnow);
            this.tabStatistics.Controls.Add(this.cmdDownloadStatistics);
            this.tabStatistics.Location = new System.Drawing.Point(4, 25);
            this.tabStatistics.Margin = new System.Windows.Forms.Padding(4);
            this.tabStatistics.Name = "tabStatistics";
            this.tabStatistics.Size = new System.Drawing.Size(784, 517);
            this.tabStatistics.TabIndex = 3;
            this.tabStatistics.Tag = "statistics";
            this.tabStatistics.Text = "statistics";
            this.tabStatistics.UseVisualStyleBackColor = true;
            // 
            // chkGroups
            // 
            this.chkGroups.AutoSize = true;
            this.chkGroups.Location = new System.Drawing.Point(367, 63);
            this.chkGroups.Margin = new System.Windows.Forms.Padding(4);
            this.chkGroups.Name = "chkGroups";
            this.chkGroups.Size = new System.Drawing.Size(68, 20);
            this.chkGroups.TabIndex = 69;
            this.chkGroups.Tag = "groups :";
            this.chkGroups.Text = "groups";
            this.chkGroups.UseVisualStyleBackColor = true;
            this.chkGroups.CheckedChanged += new System.EventHandler(this.chkGroups_CheckedChanged_1);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Image = global::lmsda.Properties.Resources.tree_corner;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(345, 64);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 16);
            this.label3.TabIndex = 68;
            this.label3.Text = " ";
            // 
            // lstGroups
            // 
            this.lstGroups.AllowDrop = true;
            this.lstGroups.AllowReorder = true;
            this.lstGroups.CheckBoxes = true;
            this.lstGroups.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstGroups.HideSelection = false;
            this.lstGroups.LineColor = System.Drawing.Color.Blue;
            this.lstGroups.Location = new System.Drawing.Point(367, 84);
            this.lstGroups.Margin = new System.Windows.Forms.Padding(4);
            this.lstGroups.Name = "lstGroups";
            this.lstGroups.Size = new System.Drawing.Size(289, 120);
            this.lstGroups.TabIndex = 66;
            this.lstGroups.UseCompatibleStateImageBehavior = false;
            this.lstGroups.View = System.Windows.Forms.View.Details;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Image = global::lmsda.Properties.Resources.tree_corner;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(36, 64);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 16);
            this.label2.TabIndex = 65;
            this.label2.Text = " ";
            // 
            // lstColumns
            // 
            this.lstColumns.AllowDrop = true;
            this.lstColumns.AllowReorder = true;
            this.lstColumns.CheckBoxes = true;
            this.lstColumns.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lstColumns.HideSelection = false;
            this.lstColumns.LineColor = System.Drawing.Color.Blue;
            this.lstColumns.Location = new System.Drawing.Point(61, 84);
            this.lstColumns.Margin = new System.Windows.Forms.Padding(4);
            this.lstColumns.Name = "lstColumns";
            this.lstColumns.Size = new System.Drawing.Size(256, 120);
            this.lstColumns.TabIndex = 64;
            this.lstColumns.UseCompatibleStateImageBehavior = false;
            this.lstColumns.View = System.Windows.Forms.View.Details;
            // 
            // lblColumns
            // 
            this.lblColumns.AutoSize = true;
            this.lblColumns.Location = new System.Drawing.Point(57, 64);
            this.lblColumns.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(107, 16);
            this.lblColumns.TabIndex = 52;
            this.lblColumns.Tag = "include_columns :";
            this.lblColumns.Text = "include_columns";
            // 
            // chkGenerateAllAttempts
            // 
            this.chkGenerateAllAttempts.AutoSize = true;
            this.chkGenerateAllAttempts.Location = new System.Drawing.Point(61, 37);
            this.chkGenerateAllAttempts.Margin = new System.Windows.Forms.Padding(4);
            this.chkGenerateAllAttempts.Name = "chkGenerateAllAttempts";
            this.chkGenerateAllAttempts.Size = new System.Drawing.Size(98, 20);
            this.chkGenerateAllAttempts.TabIndex = 50;
            this.chkGenerateAllAttempts.Tag = "all_attempts";
            this.chkGenerateAllAttempts.Text = "all_attempts";
            this.chkGenerateAllAttempts.UseVisualStyleBackColor = true;
            this.chkGenerateAllAttempts.CheckedChanged += new System.EventHandler(this.chkGenerateAllAttempts_CheckedChanged_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Image = global::lmsda.Properties.Resources.tree_corner;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(36, 38);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 16);
            this.label1.TabIndex = 49;
            this.label1.Text = " ";
            // 
            // chkOpenExcelFilesAfterConversion
            // 
            this.chkOpenExcelFilesAfterConversion.AutoSize = true;
            this.chkOpenExcelFilesAfterConversion.Location = new System.Drawing.Point(11, 379);
            this.chkOpenExcelFilesAfterConversion.Margin = new System.Windows.Forms.Padding(4);
            this.chkOpenExcelFilesAfterConversion.Name = "chkOpenExcelFilesAfterConversion";
            this.chkOpenExcelFilesAfterConversion.Size = new System.Drawing.Size(233, 20);
            this.chkOpenExcelFilesAfterConversion.TabIndex = 48;
            this.chkOpenExcelFilesAfterConversion.Tag = "open_excel_files_after_conversion";
            this.chkOpenExcelFilesAfterConversion.Text = "open_excel_files_after_conversion";
            this.chkOpenExcelFilesAfterConversion.UseVisualStyleBackColor = true;
            this.chkOpenExcelFilesAfterConversion.CheckedChanged += new System.EventHandler(this.chkOpenExcelFilesAfterConversion_CheckedChanged_1);
            // 
            // chkStatisticsCreateSubFolder
            // 
            this.chkStatisticsCreateSubFolder.AutoSize = true;
            this.chkStatisticsCreateSubFolder.Checked = true;
            this.chkStatisticsCreateSubFolder.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkStatisticsCreateSubFolder.Location = new System.Drawing.Point(11, 351);
            this.chkStatisticsCreateSubFolder.Margin = new System.Windows.Forms.Padding(4);
            this.chkStatisticsCreateSubFolder.Name = "chkStatisticsCreateSubFolder";
            this.chkStatisticsCreateSubFolder.Size = new System.Drawing.Size(134, 20);
            this.chkStatisticsCreateSubFolder.TabIndex = 6;
            this.chkStatisticsCreateSubFolder.Tag = "create_sub_folder";
            this.chkStatisticsCreateSubFolder.Text = "create_sub_folder";
            this.chkStatisticsCreateSubFolder.UseVisualStyleBackColor = true;
            this.chkStatisticsCreateSubFolder.CheckedChanged += new System.EventHandler(this.chkStatisticsCreateSubFolder_CheckedChanged_1);
            // 
            // chkCalculateExerciseStudentDetails
            // 
            this.chkCalculateExerciseStudentDetails.AutoSize = true;
            this.chkCalculateExerciseStudentDetails.Location = new System.Drawing.Point(11, 215);
            this.chkCalculateExerciseStudentDetails.Margin = new System.Windows.Forms.Padding(4);
            this.chkCalculateExerciseStudentDetails.Name = "chkCalculateExerciseStudentDetails";
            this.chkCalculateExerciseStudentDetails.Size = new System.Drawing.Size(235, 20);
            this.chkCalculateExerciseStudentDetails.TabIndex = 12;
            this.chkCalculateExerciseStudentDetails.Tag = "calculate_exercise_student_details";
            this.chkCalculateExerciseStudentDetails.Text = "calculate_exercise_student_details";
            this.chkCalculateExerciseStudentDetails.UseVisualStyleBackColor = true;
            this.chkCalculateExerciseStudentDetails.CheckedChanged += new System.EventHandler(this.chkCalculateExerciseStudentDetails_CheckedChanged_1);
            // 
            // chkCalculateResultsPerStudent
            // 
            this.chkCalculateResultsPerStudent.AutoSize = true;
            this.chkCalculateResultsPerStudent.Checked = true;
            this.chkCalculateResultsPerStudent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCalculateResultsPerStudent.Location = new System.Drawing.Point(11, 10);
            this.chkCalculateResultsPerStudent.Margin = new System.Windows.Forms.Padding(4);
            this.chkCalculateResultsPerStudent.Name = "chkCalculateResultsPerStudent";
            this.chkCalculateResultsPerStudent.Size = new System.Drawing.Size(203, 20);
            this.chkCalculateResultsPerStudent.TabIndex = 10;
            this.chkCalculateResultsPerStudent.Tag = "calculate_results_per_student";
            this.chkCalculateResultsPerStudent.Text = "calculate_results_per_student";
            this.chkCalculateResultsPerStudent.UseVisualStyleBackColor = true;
            this.chkCalculateResultsPerStudent.CheckedChanged += new System.EventHandler(this.chkCalculateResultsPerStudent_CheckedChanged_1);
            // 
            // lblTreeStat03
            // 
            this.lblTreeStat03.AutoSize = true;
            this.lblTreeStat03.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeStat03.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeStat03.Location = new System.Drawing.Point(36, 313);
            this.lblTreeStat03.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeStat03.Name = "lblTreeStat03";
            this.lblTreeStat03.Size = new System.Drawing.Size(10, 16);
            this.lblTreeStat03.TabIndex = 37;
            this.lblTreeStat03.Text = " ";
            // 
            // lblTreeStat02
            // 
            this.lblTreeStat02.AutoSize = true;
            this.lblTreeStat02.Image = global::lmsda.Properties.Resources.tree_corner;
            this.lblTreeStat02.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblTreeStat02.Location = new System.Drawing.Point(36, 284);
            this.lblTreeStat02.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTreeStat02.Name = "lblTreeStat02";
            this.lblTreeStat02.Size = new System.Drawing.Size(10, 16);
            this.lblTreeStat02.TabIndex = 36;
            this.lblTreeStat02.Text = " ";
            // 
            // chkCalculatePercentageMC
            // 
            this.chkCalculatePercentageMC.AutoSize = true;
            this.chkCalculatePercentageMC.BackColor = System.Drawing.Color.Transparent;
            this.chkCalculatePercentageMC.Location = new System.Drawing.Point(11, 255);
            this.chkCalculatePercentageMC.Margin = new System.Windows.Forms.Padding(4);
            this.chkCalculatePercentageMC.Name = "chkCalculatePercentageMC";
            this.chkCalculatePercentageMC.Size = new System.Drawing.Size(181, 20);
            this.chkCalculatePercentageMC.TabIndex = 7;
            this.chkCalculatePercentageMC.Tag = "calculate_percentage_mc";
            this.chkCalculatePercentageMC.Text = "calculate_percentage_mc";
            this.chkCalculatePercentageMC.UseVisualStyleBackColor = false;
            this.chkCalculatePercentageMC.CheckedChanged += new System.EventHandler(this.chkCalculatePercentageMC_CheckedChanged_1);
            // 
            // chkCPMCShowQuestionTitles
            // 
            this.chkCPMCShowQuestionTitles.AutoSize = true;
            this.chkCPMCShowQuestionTitles.Enabled = false;
            this.chkCPMCShowQuestionTitles.Location = new System.Drawing.Point(61, 283);
            this.chkCPMCShowQuestionTitles.Margin = new System.Windows.Forms.Padding(4);
            this.chkCPMCShowQuestionTitles.Name = "chkCPMCShowQuestionTitles";
            this.chkCPMCShowQuestionTitles.Size = new System.Drawing.Size(149, 20);
            this.chkCPMCShowQuestionTitles.TabIndex = 8;
            this.chkCPMCShowQuestionTitles.Tag = "show_question_titles";
            this.chkCPMCShowQuestionTitles.Text = "show_question_titles";
            this.chkCPMCShowQuestionTitles.UseVisualStyleBackColor = true;
            this.chkCPMCShowQuestionTitles.CheckedChanged += new System.EventHandler(this.chkCPMCShowQuestionTitles_CheckedChanged_1);
            // 
            // txtDoNotKnow
            // 
            this.txtDoNotKnow.Location = new System.Drawing.Point(300, 309);
            this.txtDoNotKnow.Margin = new System.Windows.Forms.Padding(4);
            this.txtDoNotKnow.Name = "txtDoNotKnow";
            this.txtDoNotKnow.Size = new System.Drawing.Size(265, 22);
            this.txtDoNotKnow.TabIndex = 9;
            this.txtDoNotKnow.Text = "Don\'t know";
            // 
            // lblDoNotKnow
            // 
            this.lblDoNotKnow.AutoSize = true;
            this.lblDoNotKnow.Location = new System.Drawing.Point(57, 313);
            this.lblDoNotKnow.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDoNotKnow.Name = "lblDoNotKnow";
            this.lblDoNotKnow.Size = new System.Drawing.Size(129, 16);
            this.lblDoNotKnow.TabIndex = 2;
            this.lblDoNotKnow.Tag = "do_not_know_label :";
            this.lblDoNotKnow.Text = "do_not_know_label :";
            // 
            // cmdDownloadStatistics
            // 
            this.cmdDownloadStatistics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(232)))), ((int)(((byte)(213)))));
            this.cmdDownloadStatistics.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdDownloadStatistics.Location = new System.Drawing.Point(11, 407);
            this.cmdDownloadStatistics.Margin = new System.Windows.Forms.Padding(4);
            this.cmdDownloadStatistics.Name = "cmdDownloadStatistics";
            this.cmdDownloadStatistics.Size = new System.Drawing.Size(211, 28);
            this.cmdDownloadStatistics.TabIndex = 5;
            this.cmdDownloadStatistics.Tag = "download_statistics";
            this.cmdDownloadStatistics.Text = "download_statistics";
            this.cmdDownloadStatistics.UseVisualStyleBackColor = false;
            this.cmdDownloadStatistics.Click += new System.EventHandler(this.cmdDownloadStatistics_Click_1);
            // 
            // tabSynchronization
            // 
            this.tabSynchronization.Controls.Add(this.tableLayoutStartSync);
            this.tabSynchronization.Controls.Add(this.progressBarSyncTotal);
            this.tabSynchronization.Controls.Add(this.subjectFilesSettingsControl);
            this.tabSynchronization.Location = new System.Drawing.Point(4, 25);
            this.tabSynchronization.Margin = new System.Windows.Forms.Padding(4);
            this.tabSynchronization.Name = "tabSynchronization";
            this.tabSynchronization.Size = new System.Drawing.Size(784, 517);
            this.tabSynchronization.TabIndex = 5;
            this.tabSynchronization.Tag = "synchronization";
            this.tabSynchronization.Text = "synchronization";
            this.tabSynchronization.UseVisualStyleBackColor = true;
            // 
            // tableLayoutStartSync
            // 
            this.tableLayoutStartSync.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutStartSync.ColumnCount = 2;
            this.tableLayoutStartSync.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 48F));
            this.tableLayoutStartSync.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 52F));
            this.tableLayoutStartSync.Controls.Add(this.lblSynchronisationStatus, 1, 0);
            this.tableLayoutStartSync.Controls.Add(this.pnlSyncButtons, 0, 0);
            this.tableLayoutStartSync.Location = new System.Drawing.Point(4, 364);
            this.tableLayoutStartSync.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutStartSync.Name = "tableLayoutStartSync";
            this.tableLayoutStartSync.RowCount = 1;
            this.tableLayoutStartSync.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutStartSync.Size = new System.Drawing.Size(688, 46);
            this.tableLayoutStartSync.TabIndex = 35;
            // 
            // lblSynchronisationStatus
            // 
            this.lblSynchronisationStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSynchronisationStatus.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSynchronisationStatus.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblSynchronisationStatus.Location = new System.Drawing.Point(334, 4);
            this.lblSynchronisationStatus.Margin = new System.Windows.Forms.Padding(4);
            this.lblSynchronisationStatus.Name = "lblSynchronisationStatus";
            this.lblSynchronisationStatus.Size = new System.Drawing.Size(350, 38);
            this.lblSynchronisationStatus.TabIndex = 31;
            this.lblSynchronisationStatus.Text = "sync_status";
            this.lblSynchronisationStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlSyncButtons
            // 
            this.pnlSyncButtons.Controls.Add(this.cmdStartSynchronization);
            this.pnlSyncButtons.Controls.Add(this.cmdStopSynchronization);
            this.pnlSyncButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSyncButtons.Location = new System.Drawing.Point(4, 4);
            this.pnlSyncButtons.Margin = new System.Windows.Forms.Padding(4);
            this.pnlSyncButtons.Name = "pnlSyncButtons";
            this.pnlSyncButtons.Padding = new System.Windows.Forms.Padding(7, 1, 7, 1);
            this.pnlSyncButtons.Size = new System.Drawing.Size(322, 38);
            this.pnlSyncButtons.TabIndex = 32;
            // 
            // cmdStartSynchronization
            // 
            this.cmdStartSynchronization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStartSynchronization.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(216)))), ((int)(((byte)(163)))), ((int)(((byte)(216)))));
            this.cmdStartSynchronization.Location = new System.Drawing.Point(8, 5);
            this.cmdStartSynchronization.Margin = new System.Windows.Forms.Padding(4);
            this.cmdStartSynchronization.Name = "cmdStartSynchronization";
            this.cmdStartSynchronization.Size = new System.Drawing.Size(308, 32);
            this.cmdStartSynchronization.TabIndex = 33;
            this.cmdStartSynchronization.Tag = "start_synchronization";
            this.cmdStartSynchronization.Text = "start_synchronization";
            this.cmdStartSynchronization.UseVisualStyleBackColor = false;
            // 
            // cmdStopSynchronization
            // 
            this.cmdStopSynchronization.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdStopSynchronization.Location = new System.Drawing.Point(7, 5);
            this.cmdStopSynchronization.Margin = new System.Windows.Forms.Padding(4);
            this.cmdStopSynchronization.Name = "cmdStopSynchronization";
            this.cmdStopSynchronization.Size = new System.Drawing.Size(309, 34);
            this.cmdStopSynchronization.TabIndex = 34;
            this.cmdStopSynchronization.Tag = "stop_synchronization";
            this.cmdStopSynchronization.Text = "stop_synchronization";
            this.cmdStopSynchronization.UseVisualStyleBackColor = true;
            this.cmdStopSynchronization.Visible = false;
            // 
            // progressBarSyncTotal
            // 
            this.progressBarSyncTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBarSyncTotal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progressBarSyncTotal.Location = new System.Drawing.Point(11, 414);
            this.progressBarSyncTotal.Margin = new System.Windows.Forms.Padding(4);
            this.progressBarSyncTotal.Name = "progressBarSyncTotal";
            this.progressBarSyncTotal.Size = new System.Drawing.Size(679, 12);
            this.progressBarSyncTotal.TabIndex = 32;
            // 
            // subjectFilesSettingsControl
            // 
            this.subjectFilesSettingsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.subjectFilesSettingsControl.BackColor = System.Drawing.Color.Transparent;
            this.subjectFilesSettingsControl.Location = new System.Drawing.Point(5, 5);
            this.subjectFilesSettingsControl.Margin = new System.Windows.Forms.Padding(0);
            this.subjectFilesSettingsControl.Name = "subjectFilesSettingsControl";
            this.subjectFilesSettingsControl.ParentContainer = null;
            this.subjectFilesSettingsControl.Size = new System.Drawing.Size(687, 364);
            this.subjectFilesSettingsControl.TabIndex = 0;
            this.subjectFilesSettingsControl.Load += new System.EventHandler(this.subjectFilesSettingsControl_Load);
            // 
            // grbLogin
            // 
            this.grbLogin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grbLogin.Controls.Add(this.cmdLogin);
            this.grbLogin.Controls.Add(this.cmbCourses);
            this.grbLogin.Controls.Add(this.lblInformation);
            this.grbLogin.Controls.Add(this.cmdLogout);
            this.grbLogin.Location = new System.Drawing.Point(12, 4);
            this.grbLogin.Margin = new System.Windows.Forms.Padding(4);
            this.grbLogin.Name = "grbLogin";
            this.grbLogin.Padding = new System.Windows.Forms.Padding(4);
            this.grbLogin.Size = new System.Drawing.Size(792, 50);
            this.grbLogin.TabIndex = 21;
            this.grbLogin.TabStop = false;
            // 
            // cmdLogin
            // 
            this.cmdLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(148)))), ((int)(((byte)(213)))));
            this.cmdLogin.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmdLogin.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdLogin.Location = new System.Drawing.Point(8, 14);
            this.cmdLogin.Margin = new System.Windows.Forms.Padding(4);
            this.cmdLogin.Name = "cmdLogin";
            this.cmdLogin.Size = new System.Drawing.Size(161, 28);
            this.cmdLogin.TabIndex = 1;
            this.cmdLogin.Tag = "login";
            this.cmdLogin.Text = "login";
            this.cmdLogin.UseVisualStyleBackColor = false;
            this.cmdLogin.Click += new System.EventHandler(this.cmdLogin_Click_1);
            // 
            // cmbCourses
            // 
            this.cmbCourses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCourses.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCourses.FormattingEnabled = true;
            this.cmbCourses.Location = new System.Drawing.Point(510, 16);
            this.cmbCourses.Margin = new System.Windows.Forms.Padding(4);
            this.cmbCourses.Name = "cmbCourses";
            this.cmbCourses.Size = new System.Drawing.Size(273, 24);
            this.cmbCourses.TabIndex = 2;
            // 
            // lblInformation
            // 
            this.lblInformation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInformation.AutoSize = true;
            this.lblInformation.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lblInformation.Location = new System.Drawing.Point(177, 20);
            this.lblInformation.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblInformation.Name = "lblInformation";
            this.lblInformation.Size = new System.Drawing.Size(178, 16);
            this.lblInformation.TabIndex = 0;
            this.lblInformation.Text = "login information comes here";
            // 
            // cmdLogout
            // 
            this.cmdLogout.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cmdLogout.Location = new System.Drawing.Point(8, 14);
            this.cmdLogout.Margin = new System.Windows.Forms.Padding(4);
            this.cmdLogout.Name = "cmdLogout";
            this.cmdLogout.Size = new System.Drawing.Size(160, 28);
            this.cmdLogout.TabIndex = 1;
            this.cmdLogout.Tag = "logout";
            this.cmdLogout.Text = "logout";
            this.cmdLogout.UseVisualStyleBackColor = true;
            this.cmdLogout.Visible = false;
            // 
            // txtLog
            // 
            this.txtLog.AcceptsReturn = true;
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.HideSelection = false;
            this.txtLog.Location = new System.Drawing.Point(150, 660);
            this.txtLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(655, 22);
            this.txtLog.TabIndex = 20;
            this.txtLog.TabStop = false;
            // 
            // ContainerFrame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1166, 686);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panelMenuLateral);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = global::lmsda.Properties.Resources.lmsda_icon;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1061, 693);
            this.Name = "ContainerFrame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ContainerFrame_FormClosing);
            this.Load += new System.EventHandler(this.ContainerFrame_Load);
            this.Shown += new System.EventHandler(this.ContainerFrame_Shown);
            this.panelMenuLateral.ResumeLayout(false);
            this.panelSubMenuAyuda.ResumeLayout(false);
            this.panelSubMenuEditar.ResumeLayout(false);
            this.panelSubMenuAcciones.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.grbDocument.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabExercises.ResumeLayout(false);
            this.tabExercises.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nrRandomQuestions)).EndInit();
            this.tabPDF.ResumeLayout(false);
            this.tabPDF.PerformLayout();
            this.tabStatistics.ResumeLayout(false);
            this.tabStatistics.PerformLayout();
            this.tabSynchronization.ResumeLayout(false);
            this.tableLayoutStartSync.ResumeLayout(false);
            this.pnlSyncButtons.ResumeLayout(false);
            this.grbLogin.ResumeLayout(false);
            this.grbLogin.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.Panel panelMenuLateral;
        private System.Windows.Forms.Panel panelSubMenuAyuda;
        private System.Windows.Forms.Button btnAcercaDeLMSDA;
        private System.Windows.Forms.Button btnBuscarActualizaciones;
        private System.Windows.Forms.Button btnAyuda2;
        private System.Windows.Forms.Button btnAyuda;
        private System.Windows.Forms.Panel panelSubMenuEditar;
        private System.Windows.Forms.Button btnAdministrarMaterias;
        private System.Windows.Forms.Button btnPreferencias;
        private System.Windows.Forms.Button btnEditar;
        private System.Windows.Forms.Panel panelSubMenuAcciones;
        private System.Windows.Forms.Button btnSalir;
        private System.Windows.Forms.Button btnVerRegistro;
        private System.Windows.Forms.Button btnSubirArchivos;
        private System.Windows.Forms.Button btnCerrarSesion;
        private System.Windows.Forms.Button loginToolStripMenuItem;
        private System.Windows.Forms.Button btnAcciones;
        private System.Windows.Forms.Panel panelLogo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.GroupBox grbDocument;
        private System.Windows.Forms.Label lblDocumentSelected;
        private System.Windows.Forms.Button cmdChooseDocument;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabExercises;
        private System.Windows.Forms.NumericUpDown nrRandomQuestions;
        private System.Windows.Forms.CheckBox chkRandomQuestions;
        private System.Windows.Forms.Label lblTreeEx04;
        private System.Windows.Forms.Label lblTreeEx03;
        private System.Windows.Forms.CheckBox chkSetExerciseInvisible;
        private System.Windows.Forms.Label lblExerciseScanResultsDump;
        private System.Windows.Forms.Label lblExerciseScanResults;
        private System.Windows.Forms.Label lblExerciseErrors;
        private System.Windows.Forms.Label lblTreeEx02;
        private System.Windows.Forms.CheckBox chkOneQuestionPerPage;
        private System.Windows.Forms.Label lblTreeEx01;
        private System.Windows.Forms.CheckBox chkNewDocWithExamples;
        private System.Windows.Forms.Button cmdOpenTemplate;
        private System.Windows.Forms.Button cmdReviewExercises;
        private System.Windows.Forms.Button cmdUploadExercises;
        private System.Windows.Forms.TextBox txtExerciseDump;
        private System.Windows.Forms.Button cmdScanDocument;
        private System.Windows.Forms.Button cmdJumpToError;
        private System.Windows.Forms.TabPage tabPDF;
        private System.Windows.Forms.CheckBox chkConvertHyperlinksToJavascript;
        private System.Windows.Forms.Label lblTreePdf02;
        private System.Windows.Forms.Label lblTreePdf01;
        private System.Windows.Forms.RadioButton rdbPerStyle;
        private System.Windows.Forms.RadioButton rdbPerPage;
        private System.Windows.Forms.CheckBox chkUpload;
        private System.Windows.Forms.TextBox txtSplit;
        private System.Windows.Forms.CheckBox chkSplit;
        private System.Windows.Forms.Button cmdConvertToPDF;
        private DocumentsDropDown documentsDropDownForPDF;
        private System.Windows.Forms.TabPage tabStatistics;
        private System.Windows.Forms.CheckBox chkGroups;
        private System.Windows.Forms.Label label3;
        private listview.DragAndDropListView lstGroups;
        private System.Windows.Forms.Label label2;
        private listview.DragAndDropListView lstColumns;
        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.CheckBox chkGenerateAllAttempts;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkOpenExcelFilesAfterConversion;
        private System.Windows.Forms.CheckBox chkStatisticsCreateSubFolder;
        private System.Windows.Forms.CheckBox chkCalculateExerciseStudentDetails;
        private System.Windows.Forms.CheckBox chkCalculateResultsPerStudent;
        private System.Windows.Forms.Label lblTreeStat03;
        private System.Windows.Forms.Label lblTreeStat02;
        private System.Windows.Forms.CheckBox chkCalculatePercentageMC;
        private System.Windows.Forms.CheckBox chkCPMCShowQuestionTitles;
        private System.Windows.Forms.TextBox txtDoNotKnow;
        private System.Windows.Forms.Label lblDoNotKnow;
        private System.Windows.Forms.Button cmdDownloadStatistics;
        private System.Windows.Forms.TabPage tabSynchronization;
        private System.Windows.Forms.TableLayoutPanel tableLayoutStartSync;
        private System.Windows.Forms.Label lblSynchronisationStatus;
        private System.Windows.Forms.Panel pnlSyncButtons;
        private System.Windows.Forms.Button cmdStartSynchronization;
        private System.Windows.Forms.Button cmdStopSynchronization;
        private System.Windows.Forms.ProgressBar progressBarSyncTotal;
        private subject.SubjectFilesSettingsControl subjectFilesSettingsControl;
        private System.Windows.Forms.GroupBox grbLogin;
        private System.Windows.Forms.Button cmdLogin;
        private System.Windows.Forms.ComboBox cmbCourses;
        private System.Windows.Forms.Label lblInformation;
        private System.Windows.Forms.Button cmdLogout;
        private System.Windows.Forms.TextBox txtLog;
        private PdfiumViewer.PdfViewer pdfViewer;
        private System.Windows.Forms.CheckBox chkuploadpathsave;
        private System.Windows.Forms.Button cmdsearchpath;
        private System.Windows.Forms.TextBox txtpathsave;
    }
}