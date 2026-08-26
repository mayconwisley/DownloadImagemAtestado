namespace DownloadImagemAtestado.Cli;

internal static class CommandLineParser
{
    private static readonly string[] RequiredKeys = ["numemp", "tipcol", "numcad", "local", "link"];

    public static ParseResult Parse(string[] args)
    {
        if (args.Length == 0 || args.Any(a => a is "-h" or "--help" or "/?"))
        {
            return ParseResult.Help();
        }

        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];
            if (!arg.StartsWith("--", StringComparison.Ordinal))
            {
                return ParseResult.Error($"Argumento invalido: '{arg}'. Use --nome valor.");
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
                if (i + 1 >= args.Length)
                {
                    return ParseResult.Error($"O parametro '--{key}' requer um valor.");
                }
                value = args[++i];
            }

            values[key] = value;
        }

        string[] missing = RequiredKeys.Where(k => !values.ContainsKey(k)).ToArray();
        if (missing.Length > 0)
        {
            return ParseResult.Error($"Parametros obrigatorios ausentes: {string.Join(", ", missing.Select(m => "--" + m))}");
        }

        if (!Uri.TryCreate(values["link"], UriKind.Absolute, out Uri? link) ||
            (link.Scheme != Uri.UriSchemeHttp && link.Scheme != Uri.UriSchemeHttps))
        {
            return ParseResult.Error($"O parametro '--link' precisa ser uma URL http(s) valida: '{values["link"]}'");
        }

        var options = new CommandLineOptions(
            NumEmp: values["numemp"],
            TipCol: values["tipcol"],
            NumCad: values["numcad"],
            Local: values["local"],
            Link: link,
            NomDoc: values.GetValueOrDefault("nomdoc"));

        return ParseResult.Success(options);
    }
}
