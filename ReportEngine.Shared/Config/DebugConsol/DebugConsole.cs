using Serilog;

namespace ReportEngine.Shared.Config.DebugConsol
{
    public static class DebugConsole
    {
        public static void WriteLine(object message, ConsoleColor color = ConsoleColor.White)
        {
            var oldColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"[DEBUG] {message}");
            Console.ForegroundColor = oldColor;

            // Дублируем в Serilog (если нужно)
            Log.Debug(message.ToString());
        }
    }
}
