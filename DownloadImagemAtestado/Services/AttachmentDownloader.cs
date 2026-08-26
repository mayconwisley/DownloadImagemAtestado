namespace DownloadImagemAtestado.Services;

internal sealed class AttachmentDownloader : IDisposable
{
    private readonly HttpClient _httpClient;

    public AttachmentDownloader(TimeSpan? timeout = null)
    {
        _httpClient = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(100) };
    }

    public async Task<string> DownloadAsync(Uri link, string destinationDirectory, string? fileName = null, CancellationToken cancellationToken = default)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(link, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = AttachmentFileNameResolver.Resolve(link);
        }

        string destinationFile = Path.Combine(destinationDirectory, fileName);

        await using FileStream fileStream = File.Create(destinationFile);
        await response.Content.CopyToAsync(fileStream, cancellationToken);

        return destinationFile;
    }

    public void Dispose() => _httpClient.Dispose();
}
