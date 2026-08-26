namespace DownloadImagemAtestado.Services;

/// <summary>
/// Registra erros em um arquivo de log ao lado do executavel, ja que o processo e chamado
/// pelo ExecProg da Senior e sua saida padrao normalmente nao e capturada pelo chamador.
/// </summary>
internal static class FileErrorLogger
{
    private const string LogFileName = "DownloadImagemAtestado.log";

    public static void LogError(string message)
    {
        string logFilePath = Path.Combine(AppContext.BaseDirectory, LogFileName);
        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [ERRO] {message}{Environment.NewLine}";

        try
        {
            File.AppendAllText(logFilePath, line);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
