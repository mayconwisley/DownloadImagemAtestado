namespace DownloadImagemAtestado.Cli;

internal sealed record ParseResult
{
    public required ParseStatus Status { get; init; }
    public CommandLineOptions? Options { get; init; }
    public string? ErrorMessage { get; init; }

    public static ParseResult Success(CommandLineOptions options) =>
        new() { Status = ParseStatus.Success, Options = options };

    public static ParseResult Help() =>
        new() { Status = ParseStatus.HelpRequested };

    public static ParseResult Error(string message) =>
        new() { Status = ParseStatus.Error, ErrorMessage = message };
}
