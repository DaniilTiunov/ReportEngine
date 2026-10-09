namespace ReportEngine.App.Services.Theming;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }

    void ApplyTheme(AppTheme theme);
}
