namespace DownloadImagemAtestado.Services;

internal static class DestinationFolderBuilder
{
    public static string CreateFolder(string local, string numemp, string tipcol, string numcad)
    {
        string folderName = $"{Sanitize(numemp)}-{Sanitize(tipcol)}-{Sanitize(numcad)}";
        string destinationDirectory = Path.Combine(local, folderName);
        Directory.CreateDirectory(destinationDirectory);
        return destinationDirectory;
    }

    private static string Sanitize(string value)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        return new string(value.Trim().Select(c => invalidChars.Contains(c) ? '_' : c).ToArray());
    }
}
