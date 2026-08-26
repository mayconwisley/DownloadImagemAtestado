namespace DownloadImagemAtestado.Cli;

internal sealed record CommandLineOptions(
    string NumEmp,
    string TipCol,
    string NumCad,
    string Local,
    Uri Link,
    string? NomDoc);
