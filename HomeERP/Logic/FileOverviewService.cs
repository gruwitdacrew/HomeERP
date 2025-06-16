using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Repositories;
using HomeERP.Domain.EAV.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Object = HomeERP.Domain.EAV.Models.Object;

namespace HomeERP.Logic
{
    public class FileOverviewService
    {
        private readonly AppDBContext _context;
        private readonly FileRepository _fileRepo;
        public FileOverviewService(AppDBContext context, FileRepository fileRepo)
        {
            _context = context;
            _fileRepo = fileRepo;
        }

        public List<Entity> GetEntities()
        {
            return _context.Entities.Where(Entity => Entity.Objects.Where(Object => Object.AttributeValues.OfType<FileAttributeValue>().Where(FileAttributeValue => FileAttributeValue.FileId != null).Count() > 0).Count() > 0).Include(Entity => Entity.Objects.Where(Object => Object.AttributeValues.Any(av => av is FileAttributeValue && ((FileAttributeValue)av).FileId != null))).ThenInclude(Object => Object.AttributeValues.Where(av => av is FileAttributeValue && ((FileAttributeValue)av).FileId != null)).ThenInclude(AttributeValue => AttributeValue.Attribute).ToList();
        }

        public List<FileAttributeValue> GetFileAttributeValues(Guid ObjectId)
        {
            return _context.Objects.Where(Object => Object.Id == ObjectId).Include(Object => Object.AttributeValues.Where(av => av is FileAttributeValue && ((FileAttributeValue)av).FileId != null)).ThenInclude(AttributeValue => AttributeValue.Attribute).First().AttributeValues.OfType<FileAttributeValue>().ToList();
        }

        public async Task<List<FileContentResult>> GetFilesById(List<Guid> FileAttributeValueDTOs)
        {
            List<FileContentResult> Result = new List<FileContentResult>();
            foreach (var FileAttributeValue in FileAttributeValueDTOs)
            {
                Result.Add(await _fileRepo.GetFileAsync(FileAttributeValue));
            }
            return Result;
        }

        public async Task<IActionResult> DownloadFile(Guid FileId)
        {
            return await _fileRepo.GetFileAsync(FileId);
        }
    }
}
