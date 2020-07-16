using AnotherOneConverter.WPF.Properties;
using Hardcodet.Wpf.TaskbarNotification;

namespace AnotherOneConverter.WPF.Core
{
    public class WpfNotificationService : INotificationService
    {
        public TaskbarIcon TaskbarIcon { get; set; }

        public void Show(string title, string message)
        {
            TaskbarIcon.ShowBalloonTip(title, message, Resources.LogoIcon, true);
        }

        public void ShowError(string title, string message)
        {
            TaskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Error);
        }

        public void ShowInfo(string title, string message)
        {
            TaskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Info);
        }

        public void ShowWarning(string title, string message)
        {
            TaskbarIcon.ShowBalloonTip(title, message, BalloonIcon.Warning);
        }
    }
}
