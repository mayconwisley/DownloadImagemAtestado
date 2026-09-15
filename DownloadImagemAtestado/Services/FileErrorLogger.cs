using System.Globalization;

namespace DownloadImagemAtestado.Services;

/// <summary>
/// Registra erros em um arquivo de log ao lado do executavel, ja que o processo e chamado
/// pelo ExecProg da Senior e sua saida padrao normalmente nao e capturada pelo chamador.
/// </summary>
internal static class FileErrorLogger
{
    private const string LogFileName = "DownloadImagemAtestado.log";
    private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    private static readonly TimeSpan RetentionPeriod = TimeSpan.FromDays(3);

    public static void LogError(string message) => Log("ERRO", message);

    public static void LogInfo(string message) => Log("INFO", message);

    private static void Log(string level, string message)
    {
        string logFilePath = Path.Combine(AppContext.BaseDirectory, LogFileName);
        DateTime now = DateTime.Now;
        string line = $"{now.ToString(TimestampFormat, CultureInfo.InvariantCulture)} [{level}] {message}";

        try
        {
            IEnumerable<string> existingLines = File.Exists(logFilePath)
                ? File.ReadAllLines(logFilePath)
                : [];

            IEnumerable<string> retainedLines = existingLines.Where(existingLine => IsWithinRetentionPeriod(existingLine, now));

            File.WriteAllLines(logFilePath, retainedLines.Append(line));
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static bool IsWithinRetentionPeriod(string logLine, DateTime now)
    {
        if (logLine.Length < TimestampFormat.Length)
        {
            return false;
        }

        string timestampPart = logLine[..TimestampFormat.Length];
        if (!DateTime.TryParseExact(timestampPart, TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timestamp))
        {
            return false;
        }

        return now - timestamp <= RetentionPeriod;
    }
}
