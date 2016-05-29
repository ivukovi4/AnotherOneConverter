namespace AnotherOneConverter {
    partial class MainWindow2 {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow2));
            this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this._toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._miOpen = new System.Windows.Forms.ToolStripMenuItem();
            this._miSaveAll = new System.Windows.Forms.ToolStripMenuItem();
            this._miSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this._miSaveAndSplit = new System.Windows.Forms.ToolStripMenuItem();
            this._miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._miNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this._miOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._miSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this._miSaveProjectAs = new System.Windows.Forms.ToolStripMenuItem();
            this._miCloseProject = new System.Windows.Forms.ToolStripMenuItem();
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this._toolStripContainer.ContentPanel.SuspendLayout();
            this._toolStripContainer.TopToolStripPanel.SuspendLayout();
            this._toolStripContainer.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this._menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStripContainer
            // 
            // 
            // _toolStripContainer.BottomToolStripPanel
            // 
            this._toolStripContainer.BottomToolStripPanel.Controls.Add(this._statusStrip);
            // 
            // _toolStripContainer.ContentPanel
            // 
            this._toolStripContainer.ContentPanel.Controls.Add(this._tabControl);
            resources.ApplyResources(this._toolStripContainer.ContentPanel, "_toolStripContainer.ContentPanel");
            resources.ApplyResources(this._toolStripContainer, "_toolStripContainer");
            this._toolStripContainer.Name = "_toolStripContainer";
            // 
            // _toolStripContainer.TopToolStripPanel
            // 
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._menuStrip);
            // 
            // _statusStrip
            // 
            resources.ApplyResources(this._statusStrip, "_statusStrip");
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripProgressBar,
            this._toolStripStatusLabel});
            this._statusStrip.Name = "_statusStrip";
            // 
            // _toolStripProgressBar
            // 
            this._toolStripProgressBar.Name = "_toolStripProgressBar";
            resources.ApplyResources(this._toolStripProgressBar, "_toolStripProgressBar");
            // 
            // _toolStripStatusLabel
            // 
            this._toolStripStatusLabel.Name = "_toolStripStatusLabel";
            resources.ApplyResources(this._toolStripStatusLabel, "_toolStripStatusLabel");
            // 
            // _tabControl
            // 
            resources.ApplyResources(this._tabControl, "_tabControl");
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            // 
            // _menuStrip
            // 
            resources.ApplyResources(this._menuStrip, "_menuStrip");
            this._menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.projectToolStripMenuItem});
            this._menuStrip.Name = "_menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miOpen,
            this._miSaveAll,
            this._miSaveAs,
            this._miSaveAndSplit,
            this._miExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // _miOpen
            // 
            this._miOpen.Name = "_miOpen";
            resources.ApplyResources(this._miOpen, "_miOpen");
            this._miOpen.Click += new System.EventHandler(this.On_miOpen_Click);
            // 
            // _miSaveAll
            // 
            this._miSaveAll.Name = "_miSaveAll";
            resources.ApplyResources(this._miSaveAll, "_miSaveAll");
            this._miSaveAll.Click += new System.EventHandler(this.On_miSaveAll_Click);
            // 
            // _miSaveAs
            // 
            this._miSaveAs.Name = "_miSaveAs";
            resources.ApplyResources(this._miSaveAs, "_miSaveAs");
            this._miSaveAs.Click += new System.EventHandler(this.On_miSaveAs_Click);
            // 
            // _miSaveAndSplit
            // 
            this._miSaveAndSplit.Name = "_miSaveAndSplit";
            resources.ApplyResources(this._miSaveAndSplit, "_miSaveAndSplit");
            this._miSaveAndSplit.Click += new System.EventHandler(this.On_miSaveAndSplit_Click);
            // 
            // _miExit
            // 
            this._miExit.Name = "_miExit";
            resources.ApplyResources(this._miExit, "_miExit");
            this._miExit.Click += new System.EventHandler(this.On_miExit_Click);
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._miNewProject,
            this._miOpenProject,
            this._toolStripSeparator1,
            this._miSaveProject,
            this._miSaveProjectAs,
            this._miCloseProject});
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            resources.ApplyResources(this.projectToolStripMenuItem, "projectToolStripMenuItem");
            // 
            // _miNewProject
            // 
            this._miNewProject.Name = "_miNewProject";
            resources.ApplyResources(this._miNewProject, "_miNewProject");
            this._miNewProject.Click += new System.EventHandler(this._miNewProject_Click);
            // 
            // _miOpenProject
            // 
            this._miOpenProject.Name = "_miOpenProject";
            resources.ApplyResources(this._miOpenProject, "_miOpenProject");
            // 
            // _toolStripSeparator1
            // 
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            resources.ApplyResources(this._toolStripSeparator1, "_toolStripSeparator1");
            // 
            // _miSaveProject
            // 
            this._miSaveProject.Name = "_miSaveProject";
            resources.ApplyResources(this._miSaveProject, "_miSaveProject");
            // 
            // _miSaveProjectAs
            // 
            this._miSaveProjectAs.Name = "_miSaveProjectAs";
            resources.ApplyResources(this._miSaveProjectAs, "_miSaveProjectAs");
            // 
            // _miCloseProject
            // 
            this._miCloseProject.Name = "_miCloseProject";
            resources.ApplyResources(this._miCloseProject, "_miCloseProject");
            // 
            // _openFileDialog
            // 
            resources.ApplyResources(this._openFileDialog, "_openFileDialog");
            this._openFileDialog.Multiselect = true;
            // 
            // MainWindow2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._toolStripContainer);
            this.MainMenuStrip = this._menuStrip;
            this.Name = "MainWindow2";
            this._toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.BottomToolStripPanel.PerformLayout();
            this._toolStripContainer.ContentPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.PerformLayout();
            this._toolStripContainer.ResumeLayout(false);
            this._toolStripContainer.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this._menuStrip.ResumeLayout(false);
            this._menuStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer _toolStripContainer;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.MenuStrip _menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _miOpen;
        private System.Windows.Forms.ToolStripMenuItem _miSaveAs;
        private System.Windows.Forms.ToolStripMenuItem _miSaveAll;
        private System.Windows.Forms.ToolStripMenuItem _miSaveAndSplit;
        private System.Windows.Forms.ToolStripMenuItem _miExit;
        private System.Windows.Forms.ToolStripProgressBar _toolStripProgressBar;
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.ToolStripStatusLabel _toolStripStatusLabel;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _miNewProject;
        private System.Windows.Forms.ToolStripMenuItem _miOpenProject;
        private System.Windows.Forms.ToolStripSeparator _toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem _miSaveProject;
        private System.Windows.Forms.ToolStripMenuItem _miSaveProjectAs;
        private System.Windows.Forms.ToolStripMenuItem _miCloseProject;
        private System.Windows.Forms.TabControl _tabControl;
    }
}