namespace PhysioBook.Application.Common.Interfaces;

public interface IDocumentStorageService
{
    Task<string> UploadAsync(string bucketName, string key, Stream stream, string contentType, CancellationToken cancellationToken = default);
    Task<Stream> DownloadAsync(string bucketName, string key, CancellationToken cancellationToken = default);
    Task DeleteAsync(string bucketName, string key, CancellationToken cancellationToken = default);
    Task<string> GetPresignedUrlAsync(string bucketName, string key, int expirySeconds = 3600, CancellationToken cancellationToken = default);
}
