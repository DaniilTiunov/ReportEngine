using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.AppHelpers
{
    public static class Guard
    {
        public static bool ExitIfNull(string message, INotificationService notify, params object?[] values)
        {

            foreach(var value in values)
            {
                if (value is null)
                {
                    notify.ShowError(message);
                    return true;
                }
            }
            
            return false;
        }
    }
}
