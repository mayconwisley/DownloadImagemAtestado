namespace DownloadImagemAtestado.Services;

/// <summary>
/// Deriva o nome do arquivo a partir do link, quando o nome nao e informado explicitamente.
/// A Senior gera o nome do anexo como "{hash}-{sufixo}.{extensao}" (tudo depois de "/attachments/" na URL);
/// aqui mantemos apenas o hash inicial, que e o identificador estavel do anexo.
/// </summary>
internal static class AttachmentFileNameResolver
{
    private const string AttachmentsMarker = "/attachments/";
    private const string DefaultFileName = "atestado.download";

    public static string Resolve(Uri link)
    {
        string url = link.ToString();
        int markerIndex = url.IndexOf(AttachmentsMarker, StringComparison.Ordinal);

        string rawFileName = markerIndex >= 0
            ? url[(markerIndex + AttachmentsMarker.Length)..]
            : Path.GetFileName(link.LocalPath);

        rawFileName = StripQueryAndFragment(rawFileName);

        return string.IsNullOrWhiteSpace(rawFileName)
            ? DefaultFileName
            : ExtractHashFileName(Path.GetFileName(rawFileName));
    }

    private static string StripQueryAndFragment(string value)
    {
        int cutIndex = value.IndexOfAny(['?', '#']);
        return cutIndex >= 0 ? value[..cutIndex] : value;
    }

    private static string ExtractHashFileName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        int separatorIndex = nameWithoutExtension.IndexOf('-');
        string hash = separatorIndex >= 0 ? nameWithoutExtension[..separatorIndex] : nameWithoutExtension;

        return hash + extension;
    }
}
