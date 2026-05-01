using Serilog.Sinks.RichTextBox.Themes;

namespace ReportEngine.App.Services.Logger;

public static class RichTextBoxLoggerTheme
{
    public static RichTextBoxConsoleTheme Create()
    {
        return new RichTextBoxConsoleTheme(
            new Dictionary<RichTextBoxThemeStyle, RichTextBoxConsoleThemeStyle>
            {
                [RichTextBoxThemeStyle.Text] = new() { Foreground = "Black" },
                [RichTextBoxThemeStyle.SecondaryText] = new() { Foreground = "Black" },
                [RichTextBoxThemeStyle.TertiaryText] = new() { Foreground = "Black" },

                [RichTextBoxThemeStyle.Invalid] = new() { Foreground = "#FF5555" },

                [RichTextBoxThemeStyle.Null] = new() { Foreground = "#808080" },
                [RichTextBoxThemeStyle.Name] = new() { Foreground = "#9CDCFE" },
                [RichTextBoxThemeStyle.String] = new() { Foreground = "#CE9178" },
                [RichTextBoxThemeStyle.Number] = new() { Foreground = "#B5CEA8" },
                [RichTextBoxThemeStyle.Boolean] = new() { Foreground = "#569CD6" },
                [RichTextBoxThemeStyle.Scalar] = new() { Foreground = "#DCDCAA" },

                [RichTextBoxThemeStyle.LevelVerbose] = new() { Foreground = "#808080" },
                [RichTextBoxThemeStyle.LevelDebug] = new() { Foreground = "#9CDCFE" },
                [RichTextBoxThemeStyle.LevelInformation] = new() { Foreground = "Blue" },
                [RichTextBoxThemeStyle.LevelWarning] = new() { Foreground = "Yellow" },
                [RichTextBoxThemeStyle.LevelError] = new() { Foreground = "Red" },
                [RichTextBoxThemeStyle.LevelFatal] = new() { Foreground = "Red" }
            });
    }
}
