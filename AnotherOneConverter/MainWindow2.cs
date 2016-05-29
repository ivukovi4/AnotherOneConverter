using AnotherOneConverter.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnotherOneConverter {
    public partial class MainWindow2 : Form {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(MainWindow2));

        private readonly IDocumentFactory _documentFactory = new DocumentFactory();
        private readonly IList<DocumentProject> _projects = new List<DocumentProject>();

        public MainWindow2() {
            InitializeComponent();

            /*_dataGridView.AutoGenerateColumns = false;

            _dataGridView.Columns.Add(new DataGridViewImageColumn {
                Name = nameof(DocumentInfo.Icon),
                HeaderText = "#",
                DataPropertyName = nameof(DocumentInfo.Icon),
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 24,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            _dataGridView.Columns.Add();

            _dataGridView.Columns.Add();

            _dataGridView.DataSource = Documents = new BindingList<DocumentInfo>();*/
        }

        //public IList<DocumentInfo> Documents { get; private set; }

        private void On_miOpen_Click(object sender, EventArgs e) {
            if (_openFileDialog.ShowDialog() != DialogResult.OK)
                return;

            for (int i = 0; i < _openFileDialog.FileNames.Length; i++) {
                //Documents.Add(_documentsFactory.Create(_openFileDialog.FileNames[i]));
            }
        }

        private async void On_miSaveAs_Click(object sender, EventArgs e) {
            if (_folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;

            await SaveAsAsync(_folderBrowserDialog.SelectedPath);
        }

        private Task<IList<string>> SaveAsAsync(string targetDirectory) {
            return Task.Run(() => {
                IList<string> result = new List<string>();

                var project = _projects[_tabControl.SelectedIndex];

                BeginInvoke(() => {
                    _toolStripProgressBar.Visible = true;
                    _toolStripProgressBar.Maximum = project.Documents.Count;
                    _toolStripProgressBar.Value = 0;

                    _toolStripStatusLabel.Visible = true;
                });

                for (int i = 0; i < project.Documents.Count; i++) {
                    BeginInvoke(() => _toolStripStatusLabel.Text = project.Documents[i].FileName);

                    try {
                        result.Add(project.Documents[i].ConvertToPdf(targetDirectory));
                    }
                    catch (Exception ex) {
                        Log.Error(string.Format("Can't convert document '{0}'", project.Documents[i].FileName), ex);
                        continue;
                    }

                    BeginInvoke(() => _toolStripProgressBar.Value = Math.Min(i + 1, _toolStripProgressBar.Maximum));
                }

                BeginInvoke(() => {
                    _toolStripProgressBar.Visible = false;
                    _toolStripStatusLabel.Visible = false;
                });

                return result;
            });
        }

        private async void On_miSaveAll_Click(object sender, EventArgs e) {
            await SaveAsAsync(null);
        }

        private void On_miSaveAndSplit_Click(object sender, EventArgs e) {
        }

        private void On_miExit_Click(object sender, EventArgs e) {
            Close();
        }

        private IAsyncResult BeginInvoke(Action action) {
            return BeginInvoke((Delegate)action);
        }

        private void _miNewProject_Click(object sender, EventArgs e) {
            var project = new DocumentProject(_documentFactory);
            _projects.Add(project);
            CreateProjectPage(project);
        }

        private void CreateProjectPage(DocumentProject project) {
            var tabPage = new TabPage();
            tabPage.DataBindings.Add(new Binding("Text", project, nameof(DocumentProject.DisplayName)));

            var dataGridView = new DataGridView {
                BorderStyle = BorderStyle.None,
                AutoGenerateColumns = false,
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                AllowUserToOrderColumns = false,
                AllowUserToResizeColumns = false,
                AllowUserToResizeRows = false,
                RowHeadersVisible = false,
                DataSource = project.Documents
            };

            dataGridView.Columns.Add(new DataGridViewImageColumn {
                Name = nameof(DocumentInfo.Icon),
                HeaderText = string.Empty,
                DataPropertyName = nameof(DocumentInfo.Icon),
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                MinimumWidth = 48,
                Width = 48,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                Name = nameof(DocumentInfo.FileName),
                HeaderText = "File Name",
                DataPropertyName = nameof(DocumentInfo.FileName),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                MinimumWidth = 200
            });

            dataGridView.Columns.Add(new DataGridViewTextBoxColumn {
                Name = nameof(DocumentInfo.LastWriteTime),
                HeaderText = "Changed",
                DataPropertyName = nameof(DocumentInfo.LastWriteTime),
                AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                SortMode = DataGridViewColumnSortMode.Programmatic,
                DefaultCellStyle = new DataGridViewCellStyle {
                    Format = "g"
                }
            });

            tabPage.Controls.Add(dataGridView);
            _tabControl.TabPages.Add(tabPage);
        }
    }
}
