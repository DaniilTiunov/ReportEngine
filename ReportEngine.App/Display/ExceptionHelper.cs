using ReportEngine.App.Display;

namespace ReportEngine.Shared.Helpers
{
    public static class ExceptionHelper
    {
        public static void SafeExecute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}");
            }
        }
        public static async Task SafeExecuteAsync(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}");
            }
        }
    }
}
