using System.Windows;
using System.Windows.Threading;

public static class BackgroundExecutor
{
    public static async Task ExecuteAsync(
        Func<Task> backgroundWork,      
        Action uiUpdate = null,          
        int initialDelay = 10,           
        DispatcherPriority priority = DispatcherPriority.Background,
        CancellationToken cancellationToken = default)
    {
        if (initialDelay > 0)
            await Task.Delay(initialDelay, cancellationToken);

        await Task.Run(backgroundWork, cancellationToken);
  
        if (uiUpdate != null)
        {
            await Application.Current.Dispatcher.InvokeAsync(uiUpdate, priority);
        }
    }
    
    public static async Task<T> ExecuteAsync<T>(
        Func<T> backgroundWork,
        Action<T> uiUpdate = null,
        int initialDelay = 10,
        DispatcherPriority priority = DispatcherPriority.Background,
        CancellationToken cancellationToken = default)
    {
        if (initialDelay > 0)
            await Task.Delay(initialDelay, cancellationToken);
        
        var result = await Task.Run(backgroundWork, cancellationToken);
        
        if (uiUpdate != null)
        {
            await Application.Current.Dispatcher.InvokeAsync(() => uiUpdate(result), priority);
        }
        
        return result;
    }
}