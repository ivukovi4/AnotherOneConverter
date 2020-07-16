namespace AnotherOneConverter.WPF.Core
{
    public interface INotificationService
    {
        void ShowError(string title, string message);
        void ShowInfo(string title, string message);
        void ShowWarning(string title, string message);
        void Show(string title, string message);
    }
}
