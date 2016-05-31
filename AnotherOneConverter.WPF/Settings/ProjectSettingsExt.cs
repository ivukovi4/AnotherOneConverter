using AnotherOneConverter.WPF.ViewModel;

namespace AnotherOneConverter.WPF.Settings {
    public class ProjectSettingsExt : ProjectSettings {
        public ProjectSettingsExt() { }

        public ProjectSettingsExt(ProjectViewModel project) : base(project) {
            IsDirty = project.IsDirty;
        }

        public bool IsDirty { get; set; }
    }
}
