using Microsoft.AspNetCore.Mvc;
using Minio;
using Minio.DataModel.Args;

namespace HomeERP.Services.Utils.FileService
{
    public class FileService
    {
        private IMinioClient _client;

        private FileServiceConfiguration fileServiceConfiguration = new FileServiceConfiguration();

        public FileService(IConfiguration configuration)
        {
            configuration.Bind("FileSettings", fileServiceConfiguration);

            _client = new MinioClient()
                .WithEndpoint(fileServiceConfiguration.Endpoint, 9000)
                .WithCredentials(fileServiceConfiguration.AccessKey, fileServiceConfiguration.SecretKey)
            .Build();

            if (!_client.BucketExistsAsync(new BucketExistsArgs().WithBucket("files")).Result)
            {
                _client.MakeBucketAsync(new MakeBucketArgs().WithBucket("files"));
            }
        }

        public async Task<string> Put(Guid FileId, IFormFile file)
        {
            PutObjectArgs poa = new PutObjectArgs()
                .WithBucket("files")
                .WithObject(FileId.ToString()) 
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
                .WithHeaders(new Dictionary<string, string>() { { "Name", Uri.EscapeDataString(file.FileName) } });

            var response = await _client.PutObjectAsync(poa);
  
            return file.ContentType;
        }

        public async Task<FileContentResult> Get(Guid FileId)
        {
            using (var memoryStream = new MemoryStream())
            {
                GetObjectArgs goa = new GetObjectArgs()
                    .WithBucket("files")
                    .WithObject(FileId.ToString())
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(memoryStream);
                    });

                var metadata = await _client.GetObjectAsync(goa);

                return new FileContentResult(memoryStream.ToArray(), metadata.ContentType)
                {
                    FileDownloadName = Uri.UnescapeDataString(metadata.MetaData["Name"])
                };
            }
        }

        public async Task Delete(Guid fileId)
        {
            RemoveObjectArgs roa = new RemoveObjectArgs()
                .WithBucket("files")
                .WithObject(fileId.ToString());

            await _client.RemoveObjectAsync(roa);
        }
    }
}
