using System.Windows.Threading;
using ReportEngine.App.Views.Windows.Dialog;

public static class SplashManager
{
    private static Thread? _thread;
    private static SplashWindow? _splash;
    private static readonly ManualResetEventSlim _ready = new();

    public static void Start()
    {
        _thread = new Thread(() =>
        {
            _splash = new SplashWindow();

            _splash.Show();

            _ready.Set();
            
            Dispatcher.Run();
        });

        _thread.SetApartmentState(ApartmentState.STA);
        _thread.IsBackground = true;
        _thread.Start();

        _ready.Wait();
    }

    public static void SetStatus(string text)
    {
        var splash = _splash;

        if (splash == null)
            return;

        splash.Dispatcher.BeginInvoke(() =>
        {
            splash.SetStatusText(text);
        });
    }

    public static void Close()
    {
        var splash = _splash;

        if (splash == null)
            return;

        splash.Dispatcher.BeginInvoke(() =>
        {
            splash.Close();
            
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        });

        _thread?.Join();
    }
}