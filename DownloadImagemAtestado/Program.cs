using DownloadImagemAtestado.Cli;
using DownloadImagemAtestado.Services;

ParseResult parseResult = CommandLineParser.Parse(args);
bool quiet = parseResult.Quiet;

if (parseResult.Status is ParseStatus.HelpRequested)
{
    if (!quiet)
    {
        HelpText.Print();
    }
    return ExitCode.Success;
}

if (parseResult.Status is ParseStatus.Error)
{
    if (!quiet)
    {
        Console.Error.WriteLine(parseResult.ErrorMessage);
        HelpText.Print();
    }
    FileErrorLogger.LogError($"Parametros invalidos ({string.Join(' ', args)}): {parseResult.ErrorMessage}");
    return ExitCode.UsageError;
}

CommandLineOptions options = parseResult.Options!;
string context = $"numemp={options.NumEmp} tipcol={options.TipCol} numcad={options.NumCad} link={options.Link}";

string destinationDirectory;
try
{
    destinationDirectory = DestinationFolderBuilder.CreateFolder(options.Local, options.NumEmp, options.TipCol, options.NumCad);
}
catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException or PathTooLongException)
{
    string message = $"Erro ao criar a pasta de destino ({context}): {ex.Message}";
    if (!quiet)
    {
        Console.Error.WriteLine(message);
    }
    FileErrorLogger.LogError(message);
    return ExitCode.IoError;
}

using var downloader = new AttachmentDownloader();

try
{
    string destinationFile = await downloader.DownloadAsync(options.Link, destinationDirectory, options.NomDoc);
    if (!quiet)
    {
        Console.WriteLine($"Download concluido com sucesso: {destinationFile}");
    }
    return ExitCode.Success;
}
catch (Exception ex)
{
    string message = $"Falha ao baixar o arquivo ({context}): {ex.Message}";
    if (!quiet)
    {
        Console.Error.WriteLine(message);
    }
    FileErrorLogger.LogError(message);
    return ExitCode.DownloadError;
}
