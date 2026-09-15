namespace DownloadImagemAtestado.Services;

internal sealed class AttachmentDownloader : IDisposable
{
    private const int MaxAttempts = 3;

    private readonly HttpClient _httpClient;

    public AttachmentDownloader(TimeSpan? timeout = null)
    {
        _httpClient = new HttpClient { Timeout = timeout ?? TimeSpan.FromSeconds(100) };
    }

    public async Task<string> DownloadAsync(Uri link, string destinationDirectory, string? fileName = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            fileName = AttachmentFileNameResolver.Resolve(link);
        }

        string destinationFile = Path.Combine(destinationDirectory, fileName);

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            try
            {
                FileErrorLogger.LogInfo($"Tentativa {attempt}/{MaxAttempts} de download ({link}).");

                using HttpResponseMessage response = await _httpClient.GetAsync(link, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using (FileStream fileStream = File.Create(destinationFile))
                {
                    await response.Content.CopyToAsync(fileStream, cancellationToken);
                }

                FileErrorLogger.LogInfo($"Tentativa {attempt}/{MaxAttempts}: download concluido com sucesso ({destinationFile}).");
                return destinationFile;
            }
            catch (Exception ex)
            {
                FileErrorLogger.LogError($"Tentativa {attempt}/{MaxAttempts} falhou ({link}): {ex.Message}");

                if (attempt == MaxAttempts)
                {
                    throw;
                }
            }
        }

        throw new InvalidOperationException("Falha inesperada no laco de tentativas de download.");
    }

    public void Dispose() => _httpClient.Dispose();
}
