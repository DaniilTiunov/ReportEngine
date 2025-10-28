using ReportEngine.App.Display;
using ReportEngine.Shared.Config.DebugConsol;
using System.Diagnostics;

namespace ReportEngine.App.AppHelpers;

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
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
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
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}");
        }
    }
}
