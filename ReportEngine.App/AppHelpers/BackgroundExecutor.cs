using System.Diagnostics;

public static class BackgroundExecutor
{
    public static async Task ExecuteAsync(Action backgroundWork)
    {
        try
        {
            await Task.Run(backgroundWork);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            throw;
        }
    }
}