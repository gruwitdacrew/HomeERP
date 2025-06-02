using HomeERP.Domain.Common.Contexts;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel.Args;
using System.Xml.Linq;

namespace HomeERP.Domain.Common.Repositories
{
    public class FileRepository
    {
        private readonly FileStorageContext _context;

        public FileRepository(FileStorageContext context)
        {
            _context = context;
        }

        public async Task<string> AddFileAsync(Guid fileId, IFormFile file)
        {
            var args = new PutObjectArgs()
                .WithBucket("files")
                .WithObject(fileId.ToString())
                .WithStreamData(file.OpenReadStream())
                .WithObjectSize(file.Length)
                .WithContentType(file.ContentType)
                .WithHeaders(new Dictionary<string, string> { { "Name", Uri.EscapeDataString(file.FileName) } });

            await _context._client.PutObjectAsync(args);
            return file.ContentType;
        }

        public async Task<string?> GetFileNameAsync(Guid fileId)
        {
            var metadata = await _context._client.StatObjectAsync(
                new StatObjectArgs()
                    .WithBucket("files")
                    .WithObject(fileId.ToString())
            );

            if (metadata == null) return null;
            else return Uri.UnescapeDataString(metadata.MetaData["Name"]);
        }

        public async Task<FileContentResult?> GetFileAsync(Guid fileId)
        {
            using var memoryStream = new MemoryStream();

            var metadata = await _context._client.GetObjectAsync(
                new GetObjectArgs()
                    .WithBucket("files")
                    .WithObject(fileId.ToString())
                    .WithCallbackStream(stream => stream.CopyTo(memoryStream)));

            if (metadata == null) return null;

            return new FileContentResult(memoryStream.ToArray(), metadata.ContentType)
            {
                FileDownloadName = Uri.UnescapeDataString(metadata.MetaData["Name"])
            };
        }

        public async Task DeleteFileAsync(Guid fileId)
        {
            await _context._client.RemoveObjectAsync(
                new RemoveObjectArgs()
                    .WithBucket("files")
                    .WithObject(fileId.ToString()));
        }
    }
}
