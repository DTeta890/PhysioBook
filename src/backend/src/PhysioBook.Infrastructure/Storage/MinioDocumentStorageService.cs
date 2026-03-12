using Minio;
using Minio.DataModel.Args;
using PhysioBook.Application.Common.Interfaces;

namespace PhysioBook.Infrastructure.Storage;

public sealed class MinioDocumentStorageService : IDocumentStorageService
{
    private readonly IMinioClient _minioClient;

    public MinioDocumentStorageService(IMinioClient minioClient)
    {
        _minioClient = minioClient;
    }

    public async Task<string> UploadAsync(
        string bucketName,
        string key,
        Stream stream,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        await EnsureBucketExistsAsync(bucketName, cancellationToken);

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

        return key;
    }

    public async Task<Stream> DownloadAsync(
        string bucketName,
        string key,
        CancellationToken cancellationToken = default)
    {
        var memoryStream = new MemoryStream();

        var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key)
            .WithCallbackStream(async (stream, ct) =>
            {
                await stream.CopyToAsync(memoryStream, ct);
                memoryStream.Position = 0;
            });

        await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

        return memoryStream;
    }

    public async Task DeleteAsync(
        string bucketName,
        string key,
        CancellationToken cancellationToken = default)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);
    }

    public async Task<string> GetPresignedUrlAsync(
        string bucketName,
        string key,
        int expirySeconds = 3600,
        CancellationToken cancellationToken = default)
    {
        var presignedGetObjectArgs = new PresignedGetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(key)
            .WithExpiry(expirySeconds);

        return await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);
        var exists = await _minioClient.BucketExistsAsync(bucketExistsArgs, cancellationToken);

        if (!exists)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);
            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);
        }
    }
}
