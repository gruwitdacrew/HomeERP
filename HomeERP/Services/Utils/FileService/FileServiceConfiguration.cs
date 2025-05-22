namespace HomeERP.Services.Utils.FileService
{
    public class FileServiceConfiguration
    {
        public string Endpoint { get; set; } = Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
        public string AccessKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        public string SecretKey { get; set; } = Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
    }
}
