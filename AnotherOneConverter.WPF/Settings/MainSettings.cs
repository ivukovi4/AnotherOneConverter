using AnotherOneConverter.WPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AnotherOneConverter.WPF.Settings {
    public class MainSettings {
        public MainSettings() { }

        public MainSettings(MainViewModel model) {
            if (model.ActiveProject != null) {
                ActiveProjectId = model.ActiveProject.Id;
            }

            Projects = new List<ProjectSettingsExt>(from p in model.Projects
                                                    select new ProjectSettingsExt(p));
        }

        public Guid? ActiveProjectId { get; set; }

        public IList<ProjectSettingsExt> Projects { get; set; }
    }
}
