namespace DownloadImagemAtestado.Cli;

internal sealed record ParseResult
{
    public required ParseStatus Status { get; init; }
    public CommandLineOptions? Options { get; init; }
    public string? ErrorMessage { get; init; }
    public bool Quiet { get; init; }

    public static ParseResult Success(CommandLineOptions options, bool quiet) =>
        new() { Status = ParseStatus.Success, Options = options, Quiet = quiet };

    public static ParseResult Help(bool quiet) =>
        new() { Status = ParseStatus.HelpRequested, Quiet = quiet };

    public static ParseResult Error(string message, bool quiet) =>
        new() { Status = ParseStatus.Error, ErrorMessage = message, Quiet = quiet };
}
