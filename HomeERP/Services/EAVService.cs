using HomeERP.Models.EAV.Domain;
using HomeERP.Models.EAV.DTOs;
using HomeERP.Models.EAV.DTOs.Request;
using HomeERP.Services.Utils.FileService;
using Logistics.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;
using Object = HomeERP.Models.EAV.Domain.Object;

namespace HomeERP.Services
{
    public class EAVService
    {
        private readonly AppDBContext _context;
        private readonly FileService _fileService;
        public EAVService(AppDBContext context, FileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public List<Entity> GetEntities()
        {
            return _context.Entities.ToList();
        }

        public void CreateEntityWithAttributes(Entity Entity, List<Attribute> Attributes, List<LinkAttribute> LinkAttributes)
        {
            _context.Entities.Add(Entity);
            _context.Attributes.AddRange(Attributes);
            _context.LinkAttributes.AddRange(LinkAttributes);
            _context.SaveChanges();
        }

        public void CreateEntity(Entity Entity)
        {
            _context.Entities.Add(Entity);
            _context.SaveChanges();
        }

        public void CreateObject(Object Object)
        {
            _context.Objects.Add(Object);
            foreach (FileAttributeValue FileAttributeValue in Object.AttributeValues.OfType<FileAttributeValue>())
            {
                if (FileAttributeValue.FileId != null && FileAttributeValue.File != null)
                {
                    FileAttributeValue.ContentType = _fileService.Put((Guid)FileAttributeValue.FileId, FileAttributeValue.File).Result;
                }
            }
            _context.SaveChanges();
        }

        public void UpdateObject(Object Object)
        {
            _context.Objects.Update(Object);
            foreach (FileAttributeValue FileAttributeValue in Object.AttributeValues.OfType<FileAttributeValue>())
            {
                if (FileAttributeValue.FileId != null && FileAttributeValue.File != null)
                {
                    FileAttributeValue.ContentType = _fileService.Put((Guid)FileAttributeValue.FileId, FileAttributeValue.File).Result;
                }
            }
            _context.SaveChanges();
        }

        public Entity GetEntity(Guid EntityId)
        {
            return _context.Entities.Where(Entity => Entity.Id == EntityId).Include(Entity => Entity.Attributes).First();
        }

        public void DeleteEntity(Guid EntityId)
        {
            _context.Entities.Remove(_context.Entities.Where(Entity => Entity.Id == EntityId).First());
            _context.SaveChanges();
        }

        public void DeleteObject(Guid ObjectId)
        {
            _context.Objects.Remove(_context.Objects.Where(Object => Object.Id == ObjectId).First());
            _context.SaveChanges();
        }

        public Entity GetObjectEntity(Guid ObjectId)
        {
            return _context.Objects.Where(Object => Object.Id == ObjectId).Include(Object => Object.Entity).First().Entity;
        }

        public Object GetObject(Guid ObjectId)
        {
            return _context.Objects.Where(Object => Object.Id == ObjectId).Include(Object => Object.Entity).Include(Object => Object.AttributeValues).First();
        }


        public List<Attribute> GetEntityAttributes(Guid EntityId)
        {
            List<Attribute> Attributes = _context.Attributes.Where(Attribute => Attribute.Entity.Id == EntityId).ToList();

            Attributes.ForEach(Attribute =>
            {
                if (Attribute.Type == AttributeType.Link)
                {
                    LinkAttribute tmp = _context.LinkAttributes.Where(LinkAttribute => LinkAttribute.Id == Attribute.Id).First();
                    tmp.EntityObjects = _context.Objects.Where(Object => Object.Entity.Id == tmp.LinkedEntityId).ToList();
                    Attribute = tmp;
                }
            });

            return Attributes;
        }

        public List<Object> GetEntityObjects(Guid EntityId)
        {
            return _context.Objects.Where(Object => Object.Entity.Id == EntityId).Include(Object => Object.AttributeValues).ToList();
        }

        public List<Object> SearchEntityObjects(SearchObjectsRequest SearchRequest)
        {
            var query = _context.Objects.Where(Object => Object.Entity.Id == SearchRequest.EntityId).AsQueryable();
            if (SearchRequest.NameSearchAttribute != null)
            {
                query = query.Where(Object => Object.Name.ToLower().StartsWith(SearchRequest.NameSearchAttribute.ToLower()));
            }

            foreach (var SearchAttribute in SearchRequest.SearchAttributes != null ? SearchRequest.SearchAttributes : new List<SearchAttribute>())
            {
                switch (SearchAttribute.AttributeType)
                {
                    case AttributeType.String:
                        {
                            if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as StringAttributeValue).Value.ToLower().StartsWith(SearchAttribute.Args[0].ToLower()));
                            }
                            break;
                        }
                    case AttributeType.Integer:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value >= int.Parse(SearchAttribute.Args[0]) && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value <= int.Parse(SearchAttribute.Args[1]));
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value >= int.Parse(SearchAttribute.Args[0]));
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value <= int.Parse(SearchAttribute.Args[1]));
                            }
                            break;
                        }
                    case AttributeType.Link:
                        {
                            if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as LinkAttributeValue).Value == Guid.Parse(SearchAttribute.Args[0]));
                            }
                            break;
                        }
                    //case AttributeType.File:
                    //    {
                    //        if (SearchAttribute.Args[0] != null)
                    //        {
                    //            query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FileAttributeValue).FileName.StartsWith(SearchAttribute.Args[0]));
                    //        }
                    //        break;
                    //    }
                    case AttributeType.Date:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value >= DateTime.Parse(SearchAttribute.Args[0]).ToUniversalTime() && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value <= DateTime.Parse(SearchAttribute.Args[1]).ToUniversalTime());
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value >= DateTime.Parse(SearchAttribute.Args[0]).ToUniversalTime());
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value <= DateTime.Parse(SearchAttribute.Args[1]).ToUniversalTime());
                            }
                            break;
                        }
                    case AttributeType.Float:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value >= float.Parse(SearchAttribute.Args[0]) && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value <= float.Parse(SearchAttribute.Args[1]));
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value >= float.Parse(SearchAttribute.Args[0]));
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value <= float.Parse(SearchAttribute.Args[1]));
                            }
                            break;
                        }
                }
            }
            return query.Include(Object => Object.AttributeValues).ToList();
        }

        public FileContentResult GetFile(Guid FileId)
        {
            return _fileService.Get(FileId).Result;
        }

        public FileAttributeValue GetFileAttributeValue(Guid FileId)
        {
            return _context.FileAttributeValues.Include(FileAttributeValue => FileAttributeValue.Attribute).ThenInclude(Attribute => Attribute.Entity).First(FileAttributeValue => FileAttributeValue.FileId == FileId);
        }

        public void DeleteFile(FileAttributeValue FileAttributeValue)
        {
            _fileService.Delete((Guid)FileAttributeValue.FileId).Wait();

            FileAttributeValue.FileId = null;
            FileAttributeValue.ContentType = null;

            _context.FileAttributeValues.Update(FileAttributeValue);
            _context.SaveChanges();
        }

        public Attribute GetAttribute(Guid AttributeId)
        {
            return _context.Attributes.Include(Attribute => Attribute.Entity).First(Attribute => Attribute.Id == AttributeId);
        }

        public void DeleteAttribute(Attribute Attribute)
        {
            _context.Attributes.Remove(Attribute);
            _context.SaveChanges();
        }

        public void AddAttribute(Entity Entity, Attribute Attribute)
        {
            List<Object> Objects = _context.Objects.Where(Object => Object.Entity == Attribute.Entity).Include(Object => Object.AttributeValues).ToList();
            List<AttributeValue> AttributeValues = new List<AttributeValue>();

            switch (Attribute.Type)
            {
                case AttributeType.String:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new StringAttributeValue(null, Object, Attribute));
                        }
                        break;
                    }
                case AttributeType.Integer:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new IntegerAttributeValue(null, Object, Attribute));
                        }
                        break;
                    }
                case AttributeType.Date:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new DateAttributeValue(null, Object, Attribute));
                        }
                        break;
                    }
                case AttributeType.File:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new FileAttributeValue(null, null, Object, Attribute));
                        }
                        break;
                    }
                case AttributeType.Link:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new LinkAttributeValue(null, Object, Attribute));
                        }
                        break;
                    }
                case AttributeType.Float:
                    {
                        foreach (Object Object in Objects)
                        {
                            AttributeValues.Add(new FloatAttributeValue(null, Object, Attribute));
                        }
                        break;
                    }
            }
            _context.Attributes.Add(Attribute);
            _context.AddRange(AttributeValues);
            _context.SaveChanges();
        }
    }
}
