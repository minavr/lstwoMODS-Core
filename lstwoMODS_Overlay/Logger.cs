namespace lstwoMODS_Overlay;

/// <summary>
/// The plugin starts this process with stdout and stderr redirected so it can forward our output
/// into BepInEx's log. Wine's Mono cannot write to a redirected console handle: the very first
/// <c>Console.WriteLine</c> throws <c>IOException("Invalid handle to path ...\[Unknown]")</c> and,
/// because that happens on the startup path, it kills the overlay before it ever draws a frame —
/// which under Proton looks exactly like "the menu never appears".
///
/// Every console access is therefore guarded. Once one fails we stop trying and append to
/// overlay.log next to the exe instead, so the output still goes somewhere diagnosable.
/// </summary>
public static class Logger
{
    private static bool _consoleUsable = true;
    private static readonly object FileLock = new();

    private static readonly string FallbackPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "overlay.log");

    public static void LogError(object message) => Write(message, ConsoleColor.Red);

    public static void Log(object message) => Write(message, ConsoleColor.White);

    private static void Write(object message, ConsoleColor color)
    {
        if (_consoleUsable)
        {
            try
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
                return;
            }
            catch
            {
                // Setting the colour can throw just as readily as the write itself.
                _consoleUsable = false;
            }
        }

        try
        {
            lock (FileLock)
                File.AppendAllText(FallbackPath,
                    $"{DateTime.Now:HH:mm:ss.fff} {message}{Environment.NewLine}");
        }
        catch
        {
            // Logging must never be the reason the overlay dies.
        }
    }
}
