namespace DownloadImagemAtestado.Cli;

internal static class CommandLineParser
{
    private static readonly string[] RequiredKeys = ["numemp", "tipcol", "numcad", "local", "link"];

    public static ParseResult Parse(string[] args)
    {
        bool quiet = args.Any(a => a is "-q" or "--quiet");
        string[] remainingArgs = args.Where(a => a is not ("-q" or "--quiet")).ToArray();

        if (remainingArgs.Length == 0 || remainingArgs.Any(a => a is "-h" or "--help" or "/?"))
        {
            return ParseResult.Help(quiet);
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < remainingArgs.Length; i++)
        {
            string arg = remainingArgs[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                return ParseResult.Error($"Argumento invalido: '{arg}'. Use --nome valor.", quiet);
            }

            string key = arg[2..];
            string value;

            int equalsIndex = key.IndexOf('=');
            if (equalsIndex >= 0)
            {
                value = key[(equalsIndex + 1)..];
                key = key[..equalsIndex];
            }
            else
            {
                if (i + 1 >= remainingArgs.Length)
                {
                    return ParseResult.Error($"O parametro '--{key}' requer um valor.", quiet);
                }
                value = remainingArgs[++i];
            }

            values[key] = value;
        }

        string[] missing = RequiredKeys.Where(k => !values.ContainsKey(k)).ToArray();
        if (missing.Length > 0)
        {
            return ParseResult.Error($"Parametros obrigatorios ausentes: {string.Join(", ", missing.Select(m => "--" + m))}", quiet);
        }

        if (!Uri.TryCreate(values["link"], UriKind.Absolute, out Uri? link) ||
            (link.Scheme != Uri.UriSchemeHttp && link.Scheme != Uri.UriSchemeHttps))
        {
            return ParseResult.Error($"O parametro '--link' precisa ser uma URL http(s) valida: '{values["link"]}'", quiet);
        }

        var options = new CommandLineOptions(
            NumEmp: values["numemp"],
            TipCol: values["tipcol"],
            NumCad: values["numcad"],
            Local: values["local"],
            Link: link,
            NomDoc: values.GetValueOrDefault("nomdoc"));

        return ParseResult.Success(options, quiet);
    }
}
