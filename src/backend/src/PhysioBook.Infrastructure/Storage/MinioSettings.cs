namespace PhysioBook.Infrastructure.Storage;

public sealed class MinioSettings
{
    public const string SectionName = "Minio";

    public string Endpoint { get; set; } = "localhost:9000";
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public bool UseSSL { get; set; } = false;
    public string BucketName { get; set; } = "physiobook-documents";
}
