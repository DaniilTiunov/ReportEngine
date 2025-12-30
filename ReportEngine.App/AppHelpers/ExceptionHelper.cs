using System.Diagnostics;
using ReportEngine.App.Display;
using ReportEngine.Shared.Config.DebugConsol;
using Serilog;

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

    public static void SafeExecute(Action action, string message)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}\n{message}");
        }
    }

    public static void SafeExecute(Action action, string message, string? methodName = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                Log.Information("Вызван метод: {MethodName}", methodName);
            }
            action();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Произошла ошибка в методе {MethodName}", methodName ?? "Unknown");
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}\n{message}");
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

    public static async Task SafeExecuteAsync(Func<Task> action, string message)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}\n{message}");
        }
    }

    public static async Task SafeExecuteAsync(Func<Task> action, string message, string? methodName = null)
    {
        try
        {
            if (!string.IsNullOrEmpty(methodName))
            {
                Log.Information("Вызван метод: {MethodName}", methodName);
            }
            await action();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Произошла ошибка в методе {MethodName}", methodName ?? "Unknown");
            DebugConsole.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            Debug.WriteLine($"Произшла ошибка: {ex.Message}", ConsoleColor.Red);
            MessageBoxHelper.ShowError($"Произошла ошибка: {ex.Message}\n{message}");
        }
    }
}
