using HomeERP.Models.EAV.Domain;
using HomeERP.Services.Utils.FileService;
using Logistics.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Object = HomeERP.Models.EAV.Domain.Object;

namespace HomeERP.Services
{
    public class FileOverviewService
    {
        private readonly AppDBContext _context;
        private readonly FileService _fileService;
        public FileOverviewService(AppDBContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
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
                Result.Add(await _fileService.Get(FileAttributeValue));
            }
            return Result;
        }
    }
}
