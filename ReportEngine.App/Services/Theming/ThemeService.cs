using System.Windows;

namespace ReportEngine.App.Services.Theming;

public sealed class ThemeService : IThemeService
{
    private const string ResourceRoot =
        "/KIPARIS_PCM;component/Resources/Dictionaries/ColorThemes/";

    private ResourceDictionary? _currentThemeDictionary;

    public AppTheme CurrentTheme { get; private set; } = AppTheme.Light;

    public void ApplyTheme(AppTheme theme)
    {
        var application = Application.Current
            ?? throw new InvalidOperationException("WPF application is not initialized.");

        if (!application.Dispatcher.CheckAccess())
        {
            application.Dispatcher.Invoke(() => ApplyTheme(theme));
            return;
        }

        var dictionary = new ResourceDictionary
        {
            Source = new Uri(ResourceRoot + GetDictionaryName(theme), UriKind.RelativeOrAbsolute)
        };

        var dictionaries = application.Resources.MergedDictionaries;
        if (_currentThemeDictionary is null)
            dictionaries.Add(dictionary);
        else
        {
            var index = dictionaries.IndexOf(_currentThemeDictionary);
            if (index >= 0)
                dictionaries[index] = dictionary;
            else
                dictionaries.Add(dictionary);
        }

        _currentThemeDictionary = dictionary;
        CurrentTheme = theme;
    }

    private static string GetDictionaryName(AppTheme theme) => theme switch
    {
        AppTheme.Light => "LightTheme.xaml",
        AppTheme.Dark => "DarkTheme.xaml",
        AppTheme.MangoParadise => "MangoParadiseTheme.xaml",
        AppTheme.BubbleGum => "BubbleGumTheme.xaml",
        _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
    };
}
