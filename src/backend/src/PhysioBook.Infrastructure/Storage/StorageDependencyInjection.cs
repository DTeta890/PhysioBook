using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PhysioBook.Application.Common.Interfaces;
using PhysioBook.Infrastructure.Persistence.Repositories;

namespace PhysioBook.Infrastructure.Storage;

public static class StorageDependencyInjection
{
    public static IServiceCollection AddDocumentStorage(this IServiceCollection services, IConfiguration configuration)
    {
        var minioSettings = new MinioSettings();
        configuration.GetSection(MinioSettings.SectionName).Bind(minioSettings);
        services.Configure<MinioSettings>(configuration.GetSection(MinioSettings.SectionName));

        services.AddSingleton<IMinioClient>(_ =>
            new MinioClient()
                .WithEndpoint(minioSettings.Endpoint)
                .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
                .WithSSL(minioSettings.UseSSL)
                .Build());

        services.AddScoped<IDocumentStorageService, MinioDocumentStorageService>();
        services.AddScoped<IPatientDocumentRepository, PatientDocumentRepository>();

        return services;
    }
}
