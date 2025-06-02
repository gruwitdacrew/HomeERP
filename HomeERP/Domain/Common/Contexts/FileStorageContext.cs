using Minio;
using Minio.DataModel.Args;

namespace HomeERP.Domain.Common.Contexts
{
    public class FileStorageContext : IDisposable
    {
        public readonly IMinioClient _client;

        public FileStorageContext(IConfiguration configuration)
        {
            FileStorageSettings settings = new FileStorageSettings();
            if (settings.Endpoint == null) configuration.Bind("FileSettings", settings);

            _client = new MinioClient()
                .WithEndpoint(settings.Endpoint, 9000)
                .WithCredentials(settings.AccessKey, settings.SecretKey)
                .Build();

            InitializeBucketAsync().Wait();
        }

        private async Task InitializeBucketAsync()
        {
            if (!_client.BucketExistsAsync(new BucketExistsArgs().WithBucket("files")).Result)
            {
                await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket("files"));
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }

    public class FileStorageSettings
    {
        public string Endpoint { get; set; } = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
        public string AccessKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        public string SecretKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
    }
}
