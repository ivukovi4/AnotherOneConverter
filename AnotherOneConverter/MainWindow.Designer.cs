namespace AnotherOneConverter {
    partial class MainWindow {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this._openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this._folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this._toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this._fileIcons32 = new System.Windows.Forms.ImageList(this.components);
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._openFiles = new System.Windows.Forms.ToolStripButton();
            this._exportToPdf = new System.Windows.Forms.ToolStripButton();
            this._remove = new System.Windows.Forms.ToolStripButton();
            this._saveToSameDirectory = new System.Windows.Forms.ToolStripButton();
            this._listView = new System.Windows.Forms.ListView();
            this._fileIcons16 = new System.Windows.Forms.ImageList(this.components);
            this._toolStripContainer.BottomToolStripPanel.SuspendLayout();
            this._toolStripContainer.ContentPanel.SuspendLayout();
            this._toolStripContainer.TopToolStripPanel.SuspendLayout();
            this._toolStripContainer.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _openFileDialog
            // 
            this._openFileDialog.Filter = "All|*.doc;*.docx;*.xls;*.xlsx|Word 2003|*.doc|Word 2007|*.docx|Excel 2003|*.xls|E" +
    "xcel 2007|*.xlsx";
            this._openFileDialog.Multiselect = true;
            // 
            // _notifyIcon
            // 
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Visible = true;
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
            this._toolStripContainer.ContentPanel.Controls.Add(this._listView);
            this._toolStripContainer.ContentPanel.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this._toolStripContainer.ContentPanel.Size = new System.Drawing.Size(624, 394);
            this._toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._toolStripContainer.Location = new System.Drawing.Point(0, 0);
            this._toolStripContainer.Name = "_toolStripContainer";
            this._toolStripContainer.Size = new System.Drawing.Size(624, 441);
            this._toolStripContainer.TabIndex = 1;
            this._toolStripContainer.Text = "toolStripContainer";
            // 
            // _toolStripContainer.TopToolStripPanel
            // 
            this._toolStripContainer.TopToolStripPanel.Controls.Add(this._toolStrip);
            // 
            // _statusStrip
            // 
            this._statusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._progressBar});
            this._statusStrip.Location = new System.Drawing.Point(0, 0);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(624, 22);
            this._statusStrip.TabIndex = 0;
            // 
            // _progressBar
            // 
            this._progressBar.Name = "_progressBar";
            this._progressBar.Size = new System.Drawing.Size(100, 16);
            this._progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this._progressBar.Visible = false;
            // 
            // _fileIcons32
            // 
            this._fileIcons32.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_fileIcons32.ImageStream")));
            this._fileIcons32.TransparentColor = System.Drawing.Color.Transparent;
            this._fileIcons32.Images.SetKeyName(0, "_blank.png");
            this._fileIcons32.Images.SetKeyName(1, "_page.png");
            this._fileIcons32.Images.SetKeyName(2, "aac.png");
            this._fileIcons32.Images.SetKeyName(3, "ai.png");
            this._fileIcons32.Images.SetKeyName(4, "aiff.png");
            this._fileIcons32.Images.SetKeyName(5, "avi.png");
            this._fileIcons32.Images.SetKeyName(6, "bmp.png");
            this._fileIcons32.Images.SetKeyName(7, "c.png");
            this._fileIcons32.Images.SetKeyName(8, "cpp.png");
            this._fileIcons32.Images.SetKeyName(9, "css.png");
            this._fileIcons32.Images.SetKeyName(10, "dat.png");
            this._fileIcons32.Images.SetKeyName(11, "dmg.png");
            this._fileIcons32.Images.SetKeyName(12, "doc.png");
            this._fileIcons32.Images.SetKeyName(13, "dotx.png");
            this._fileIcons32.Images.SetKeyName(14, "dwg.png");
            this._fileIcons32.Images.SetKeyName(15, "dxf.png");
            this._fileIcons32.Images.SetKeyName(16, "eps.png");
            this._fileIcons32.Images.SetKeyName(17, "exe.png");
            this._fileIcons32.Images.SetKeyName(18, "flv.png");
            this._fileIcons32.Images.SetKeyName(19, "gif.png");
            this._fileIcons32.Images.SetKeyName(20, "h.png");
            this._fileIcons32.Images.SetKeyName(21, "hpp.png");
            this._fileIcons32.Images.SetKeyName(22, "html.png");
            this._fileIcons32.Images.SetKeyName(23, "ics.png");
            this._fileIcons32.Images.SetKeyName(24, "iso.png");
            this._fileIcons32.Images.SetKeyName(25, "java.png");
            this._fileIcons32.Images.SetKeyName(26, "jpg.png");
            this._fileIcons32.Images.SetKeyName(27, "js.png");
            this._fileIcons32.Images.SetKeyName(28, "key.png");
            this._fileIcons32.Images.SetKeyName(29, "less.png");
            this._fileIcons32.Images.SetKeyName(30, "mid.png");
            this._fileIcons32.Images.SetKeyName(31, "mp3.png");
            this._fileIcons32.Images.SetKeyName(32, "mp4.png");
            this._fileIcons32.Images.SetKeyName(33, "mpg.png");
            this._fileIcons32.Images.SetKeyName(34, "odf.png");
            this._fileIcons32.Images.SetKeyName(35, "ods.png");
            this._fileIcons32.Images.SetKeyName(36, "odt.png");
            this._fileIcons32.Images.SetKeyName(37, "otp.png");
            this._fileIcons32.Images.SetKeyName(38, "ots.png");
            this._fileIcons32.Images.SetKeyName(39, "ott.png");
            this._fileIcons32.Images.SetKeyName(40, "pdf.png");
            this._fileIcons32.Images.SetKeyName(41, "php.png");
            this._fileIcons32.Images.SetKeyName(42, "png.png");
            this._fileIcons32.Images.SetKeyName(43, "ppt.png");
            this._fileIcons32.Images.SetKeyName(44, "psd.png");
            this._fileIcons32.Images.SetKeyName(45, "py.png");
            this._fileIcons32.Images.SetKeyName(46, "qt.png");
            this._fileIcons32.Images.SetKeyName(47, "rar.png");
            this._fileIcons32.Images.SetKeyName(48, "rb.png");
            this._fileIcons32.Images.SetKeyName(49, "rtf.png");
            this._fileIcons32.Images.SetKeyName(50, "sass.png");
            this._fileIcons32.Images.SetKeyName(51, "scss.png");
            this._fileIcons32.Images.SetKeyName(52, "sql.png");
            this._fileIcons32.Images.SetKeyName(53, "tga.png");
            this._fileIcons32.Images.SetKeyName(54, "tgz.png");
            this._fileIcons32.Images.SetKeyName(55, "tiff.png");
            this._fileIcons32.Images.SetKeyName(56, "txt.png");
            this._fileIcons32.Images.SetKeyName(57, "wav.png");
            this._fileIcons32.Images.SetKeyName(58, "xls.png");
            this._fileIcons32.Images.SetKeyName(59, "xlsx.png");
            this._fileIcons32.Images.SetKeyName(60, "xml.png");
            this._fileIcons32.Images.SetKeyName(61, "yml.png");
            this._fileIcons32.Images.SetKeyName(62, "zip.png");
            // 
            // _toolStrip
            // 
            this._toolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._openFiles,
            this._exportToPdf,
            this._remove,
            this._saveToSameDirectory});
            this._toolStrip.Location = new System.Drawing.Point(3, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(504, 25);
            this._toolStrip.TabIndex = 0;
            // 
            // _openFiles
            // 
            this._openFiles.Image = global::AnotherOneConverter.Properties.Resources.blue_folder_horizontal_open;
            this._openFiles.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._openFiles.Name = "_openFiles";
            this._openFiles.Size = new System.Drawing.Size(115, 22);
            this._openFiles.Text = "Открыть файлы";
            this._openFiles.ToolTipText = "Открыть файлы";
            this._openFiles.Click += new System.EventHandler(this.OnOpenFilesClick);
            // 
            // _exportToPdf
            // 
            this._exportToPdf.Image = global::AnotherOneConverter.Properties.Resources.disk;
            this._exportToPdf.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._exportToPdf.Name = "_exportToPdf";
            this._exportToPdf.Size = new System.Drawing.Size(123, 22);
            this._exportToPdf.Text = "Сохранить в ПДФ";
            this._exportToPdf.ToolTipText = "Сохранить в ПДФ";
            this._exportToPdf.Click += new System.EventHandler(this.OnExportClick);
            // 
            // _remove
            // 
            this._remove.Enabled = false;
            this._remove.Image = global::AnotherOneConverter.Properties.Resources.minus;
            this._remove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._remove.Name = "_remove";
            this._remove.Size = new System.Drawing.Size(71, 22);
            this._remove.Text = "Удалить";
            this._remove.Click += new System.EventHandler(this.OnRemoveClick);
            // 
            // _saveToSameDirectory
            // 
            this._saveToSameDirectory.Checked = true;
            this._saveToSameDirectory.CheckOnClick = true;
            this._saveToSameDirectory.CheckState = System.Windows.Forms.CheckState.Checked;
            this._saveToSameDirectory.Image = global::AnotherOneConverter.Properties.Resources.blue_folder_horizontal;
            this._saveToSameDirectory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._saveToSameDirectory.Name = "_saveToSameDirectory";
            this._saveToSameDirectory.Size = new System.Drawing.Size(183, 22);
            this._saveToSameDirectory.Text = "Сохранять в исходной папке";
            // 
            // _listView
            // 
            this._listView.AllowDrop = true;
            this._listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._listView.FullRowSelect = true;
            this._listView.GridLines = true;
            this._listView.LargeImageList = this._fileIcons32;
            this._listView.Location = new System.Drawing.Point(3, 0);
            this._listView.Name = "_listView";
            this._listView.Size = new System.Drawing.Size(618, 394);
            this._listView.SmallImageList = this._fileIcons16;
            this._listView.TabIndex = 1;
            this._listView.UseCompatibleStateImageBehavior = false;
            this._listView.View = System.Windows.Forms.View.List;
            this._listView.SelectedIndexChanged += new System.EventHandler(this.OnSelectedIndexChanged);
            this._listView.DragDrop += new System.Windows.Forms.DragEventHandler(this.OnDragDrop);
            this._listView.DragEnter += new System.Windows.Forms.DragEventHandler(this.OnDragEnter);
            this._listView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnKeyDown);
            // 
            // _fileIcons16
            // 
            this._fileIcons16.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("_fileIcons16.ImageStream")));
            this._fileIcons16.TransparentColor = System.Drawing.Color.Transparent;
            this._fileIcons16.Images.SetKeyName(0, "_blank.png");
            this._fileIcons16.Images.SetKeyName(1, "_page.png");
            this._fileIcons16.Images.SetKeyName(2, "aac.png");
            this._fileIcons16.Images.SetKeyName(3, "ai.png");
            this._fileIcons16.Images.SetKeyName(4, "aiff.png");
            this._fileIcons16.Images.SetKeyName(5, "avi.png");
            this._fileIcons16.Images.SetKeyName(6, "bmp.png");
            this._fileIcons16.Images.SetKeyName(7, "c.png");
            this._fileIcons16.Images.SetKeyName(8, "cpp.png");
            this._fileIcons16.Images.SetKeyName(9, "css.png");
            this._fileIcons16.Images.SetKeyName(10, "dat.png");
            this._fileIcons16.Images.SetKeyName(11, "dmg.png");
            this._fileIcons16.Images.SetKeyName(12, "doc.png");
            this._fileIcons16.Images.SetKeyName(13, "dotx.png");
            this._fileIcons16.Images.SetKeyName(14, "dwg.png");
            this._fileIcons16.Images.SetKeyName(15, "dxf.png");
            this._fileIcons16.Images.SetKeyName(16, "eps.png");
            this._fileIcons16.Images.SetKeyName(17, "exe.png");
            this._fileIcons16.Images.SetKeyName(18, "flv.png");
            this._fileIcons16.Images.SetKeyName(19, "gif.png");
            this._fileIcons16.Images.SetKeyName(20, "h.png");
            this._fileIcons16.Images.SetKeyName(21, "hpp.png");
            this._fileIcons16.Images.SetKeyName(22, "html.png");
            this._fileIcons16.Images.SetKeyName(23, "ics.png");
            this._fileIcons16.Images.SetKeyName(24, "iso.png");
            this._fileIcons16.Images.SetKeyName(25, "java.png");
            this._fileIcons16.Images.SetKeyName(26, "jpg.png");
            this._fileIcons16.Images.SetKeyName(27, "js.png");
            this._fileIcons16.Images.SetKeyName(28, "key.png");
            this._fileIcons16.Images.SetKeyName(29, "less.png");
            this._fileIcons16.Images.SetKeyName(30, "mid.png");
            this._fileIcons16.Images.SetKeyName(31, "mp3.png");
            this._fileIcons16.Images.SetKeyName(32, "mp4.png");
            this._fileIcons16.Images.SetKeyName(33, "mpg.png");
            this._fileIcons16.Images.SetKeyName(34, "odf.png");
            this._fileIcons16.Images.SetKeyName(35, "ods.png");
            this._fileIcons16.Images.SetKeyName(36, "odt.png");
            this._fileIcons16.Images.SetKeyName(37, "otp.png");
            this._fileIcons16.Images.SetKeyName(38, "ots.png");
            this._fileIcons16.Images.SetKeyName(39, "ott.png");
            this._fileIcons16.Images.SetKeyName(40, "pdf.png");
            this._fileIcons16.Images.SetKeyName(41, "php.png");
            this._fileIcons16.Images.SetKeyName(42, "png.png");
            this._fileIcons16.Images.SetKeyName(43, "ppt.png");
            this._fileIcons16.Images.SetKeyName(44, "psd.png");
            this._fileIcons16.Images.SetKeyName(45, "py.png");
            this._fileIcons16.Images.SetKeyName(46, "qt.png");
            this._fileIcons16.Images.SetKeyName(47, "rar.png");
            this._fileIcons16.Images.SetKeyName(48, "rb.png");
            this._fileIcons16.Images.SetKeyName(49, "rtf.png");
            this._fileIcons16.Images.SetKeyName(50, "sass.png");
            this._fileIcons16.Images.SetKeyName(51, "scss.png");
            this._fileIcons16.Images.SetKeyName(52, "sql.png");
            this._fileIcons16.Images.SetKeyName(53, "tga.png");
            this._fileIcons16.Images.SetKeyName(54, "tgz.png");
            this._fileIcons16.Images.SetKeyName(55, "tiff.png");
            this._fileIcons16.Images.SetKeyName(56, "txt.png");
            this._fileIcons16.Images.SetKeyName(57, "wav.png");
            this._fileIcons16.Images.SetKeyName(58, "xls.png");
            this._fileIcons16.Images.SetKeyName(59, "xlsx.png");
            this._fileIcons16.Images.SetKeyName(60, "xml.png");
            this._fileIcons16.Images.SetKeyName(61, "yml.png");
            this._fileIcons16.Images.SetKeyName(62, "zip.png");
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this._toolStripContainer);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(520, 320);
            this.Name = "MainWindow";
            this.Text = "PDF Конвертер";
            this._toolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.BottomToolStripPanel.PerformLayout();
            this._toolStripContainer.ContentPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this._toolStripContainer.TopToolStripPanel.PerformLayout();
            this._toolStripContainer.ResumeLayout(false);
            this._toolStripContainer.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.OpenFileDialog _openFileDialog;
        private System.Windows.Forms.FolderBrowserDialog _folderBrowserDialog;
        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.ToolStripContainer _toolStripContainer;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripButton _openFiles;
        private System.Windows.Forms.ToolStripButton _exportToPdf;
        private System.Windows.Forms.ToolStripProgressBar _progressBar;
        private System.Windows.Forms.ToolStripButton _saveToSameDirectory;
        private System.Windows.Forms.ImageList _fileIcons32;
        private System.Windows.Forms.ToolStripButton _remove;
        private System.Windows.Forms.ListView _listView;
        private System.Windows.Forms.ImageList _fileIcons16;
    }
}

