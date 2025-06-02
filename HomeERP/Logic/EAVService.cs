using HomeERP.Domain.Common;
using HomeERP.Domain.Common.Models;
using HomeERP.Domain.Common.Repositories;
using HomeERP.Domain.EAV.Models;
using HomeERP.Views.EAV.DTOs.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Attribute = HomeERP.Domain.EAV.Models.Attribute;
using Object = HomeERP.Domain.EAV.Models.Object;

namespace HomeERP.Services
{
    public class EAVService
    {
        private readonly BaseEntityRepository<Entity> _entityRepo;
        private readonly BaseEntityRepository<Object> _objectRepo;
        private readonly BaseEntityRepository<Attribute> _attributeRepo;
        private readonly GenericRepository<FileAttributeValue> _fileAttributeValueRepo;
        private readonly FileRepository _fileRepo;
        private readonly BaseEntityRepository<User> _userRepo;
        public EAVService(BaseEntityRepository<Entity> entityRepo, BaseEntityRepository<Object> objectRepo, BaseEntityRepository<Attribute> attributeRepo, GenericRepository<FileAttributeValue> fileAttributeValueRepo, FileRepository fileRepo, BaseEntityRepository<User> userRepo)
        {
            _entityRepo = entityRepo;
            _objectRepo = objectRepo;
            _attributeRepo = attributeRepo;
            _fileAttributeValueRepo = fileAttributeValueRepo;
            _fileRepo = fileRepo;
            _userRepo = userRepo;
        }

        public List<Entity> GetEntities()
        {
            return _entityRepo.Query().ToList();
        }

        public void CreateEntity(Entity entity)
        {
            _entityRepo.Add(entity);
        }

        public Entity GetEntity(Guid entityId)
        {
            Entity entity = _entityRepo.GetBy(entityId);
            _attributeRepo.Query().Where(attribute => attribute.Entity == entity).Include(Attribute => (Attribute as LinkAttribute).LinkedEntity.Objects).Load();

            return entity;
        }

        public void GetEntityObjects(Entity entity)
        {
            _objectRepo.Query().Where(Object => Object.Entity == entity)
                .Include(Object => Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent))
                .AsEnumerable()
                .Select(Object =>
                {
                    Object.AttributeValues = Object.AttributeValues.OrderBy(AttributeValue => Array.IndexOf(Object.Entity.Attributes.ToArray(), AttributeValue.Attribute)).ToList();
                    return Object;
                }).ToList();
        }

        public void DeleteEntity(Entity entity)
        {
            _entityRepo.Delete(entity);
        }



        public async Task CreateObject(Object Object)
        {
            foreach (FileAttributeValue fileAttributeValue in Object.AttributeValues.OfType<FileAttributeValue>())
            {
                if (fileAttributeValue.FileId != null && fileAttributeValue.File != null)
                {
                    fileAttributeValue.ContentType = await _fileRepo.AddFileAsync((Guid)fileAttributeValue.FileId, fileAttributeValue.File);
                }
            }
            _objectRepo.Add(Object);
        }

        public Object GetObjectCurrent(Guid objectId)
        {
            Object Object = _objectRepo.Query().Where(Object => Object.Id == objectId)
                .Include(Object => Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent))
                .Include(Object => Object.Entity).ThenInclude(Entity => Entity.Attributes).ThenInclude(Attribute => (Attribute as LinkAttribute).LinkedEntity.Objects)
                .FirstOrDefault();

            _attributeRepo.Query().Where(attribute => attribute.Entity == Object.Entity).Load();
            _entityRepo.Query().Where(linkedEntity => Object.Entity.Attributes.OfType<LinkAttribute>().Select(linkAttribute => linkAttribute.LinkedEntity).Contains(linkedEntity)).Include(entity => entity.Objects).Load();

            return Object;
        }

        public Object GetObjectWithHistory(Guid objectId)
        {
            Object Object = _objectRepo.Query().Where(Object => Object.Id == objectId)
                .Include(Object => Object.AttributeValues).ThenInclude(AttributeValue => AttributeValue.User)
                .Include(Object => Object.Entity).ThenInclude(Entity => Entity.Attributes).ThenInclude(Attribute => (Attribute as LinkAttribute).LinkedEntity.Objects)
                .FirstOrDefault();

            return Object;
        }

        public async Task UpdateObject(Object Object)
        {
            foreach (FileAttributeValue fileAttributeValue in Object.AttributeValues.OfType<FileAttributeValue>())
            {
                if (fileAttributeValue.FileId != null && fileAttributeValue.File != null)
                {
                    fileAttributeValue.ContentType = await _fileRepo.AddFileAsync((Guid)fileAttributeValue.FileId, fileAttributeValue.File);
                }
            }
            _objectRepo.Update(Object);
        }

        public void DeleteObject(Object Object)
        {
            _objectRepo.Delete(Object);
        }

        public void SearchEntityObjects(SearchObjectsRequest searchRequest, Entity entity)
        {
            var query = _objectRepo.Query().Where(Object => Object.Entity.Id == searchRequest.EntityId).AsQueryable();
            if (searchRequest.NameSearchAttribute != null)
            {
                query = query.Where(Object => Object.Name.ToLower().StartsWith(searchRequest.NameSearchAttribute.ToLower()));
            }

            foreach (var SearchAttribute in searchRequest.SearchAttributes ?? new List<SearchAttribute>())
            {
                switch (SearchAttribute.AttributeType)
                {
                    case AttributeType.String:
                        {
                            if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as StringAttributeValue).Value.ToLower().StartsWith(SearchAttribute.Args[0].ToLower()));
                            }
                            break;
                        }
                    case AttributeType.Integer:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value >= int.Parse(SearchAttribute.Args[0]) && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value <= int.Parse(SearchAttribute.Args[1]));
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value >= int.Parse(SearchAttribute.Args[0]));
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as IntegerAttributeValue).Value <= int.Parse(SearchAttribute.Args[1]));
                            }
                            break;
                        }
                    case AttributeType.Link:
                        {
                            if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as LinkAttributeValue).Value == Guid.Parse(SearchAttribute.Args[0]));
                            }
                            break;
                        }
                    case AttributeType.Date:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value >= DateTime.Parse(SearchAttribute.Args[0]).ToUniversalTime() && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value <= DateTime.Parse(SearchAttribute.Args[1]).ToUniversalTime());
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value >= DateTime.Parse(SearchAttribute.Args[0]).ToUniversalTime());
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as DateAttributeValue).Value <= DateTime.Parse(SearchAttribute.Args[1]).ToUniversalTime());
                            }
                            break;
                        }
                    case AttributeType.Float:
                        {
                            if (SearchAttribute.Args[0] != null && SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value >= float.Parse(SearchAttribute.Args[0]) && (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value <= float.Parse(SearchAttribute.Args[1]));
                            }
                            else if (SearchAttribute.Args[0] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value >= float.Parse(SearchAttribute.Args[0]));
                            }
                            else if (SearchAttribute.Args[1] != null)
                            {
                                query = query.Where(Object => (Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent).First(AttributeValue => AttributeValue.AttributeId == SearchAttribute.AttributeId) as FloatAttributeValue).Value <= float.Parse(SearchAttribute.Args[1]));
                            }
                            break;
                        }
                }
            }
            query.Include(Object => Object.AttributeValues.Where(AttributeValue => AttributeValue.IsCurrent)).AsEnumerable().Select(Object =>
            {
                Object.AttributeValues = Object.AttributeValues.OrderBy(AttributeValue => Array.IndexOf(Object.Entity.Attributes.ToArray(), AttributeValue.Attribute)).ToList();
                return Object;
            }).ToList();
        }



        public async Task<FileContentResult> GetFile(Guid fileId)
        {
            return await _fileRepo.GetFileAsync(fileId);
        }

        public FileAttributeValue GetFileAttributeValue(Guid fileId)
        {
            return _fileAttributeValueRepo.Query().Include(attributeValue => attributeValue.Attribute).ThenInclude(attribute => attribute.Entity).First(fileAttributeValue => fileAttributeValue.FileId == fileId);
        }

        public async Task DeleteFile(FileAttributeValue fileAttributeValue)
        {
            await _fileRepo.DeleteFileAsync((Guid)fileAttributeValue.FileId);

            fileAttributeValue.FileId = null;
            fileAttributeValue.ContentType = null;

            _fileAttributeValueRepo.Update(fileAttributeValue);
        }

        public Attribute GetAttribute(Guid attributeId)
        {
            Attribute attribute = _attributeRepo.GetBy(attributeId);
            _entityRepo.Query().Where(entity => entity == attribute.Entity).Load();

            return attribute;
        }

        public void DeleteAttribute(Attribute attribute)
        {
            _attributeRepo.Delete(attribute);
        }

        public void AddAttribute(Entity Entity, Attribute Attribute)
        {
            List<Object> Objects = _objectRepo.Query().ToList();
            User user = GetCurrentUser();

            switch (Attribute.Type)
            {
                case AttributeType.String:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new StringAttributeValue(null, Object, Attribute, user));
                        }
                        break;
                    }
                case AttributeType.Integer:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new IntegerAttributeValue(null, Object, Attribute, user));
                        }
                        break;
                    }
                case AttributeType.Date:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new DateAttributeValue(null, Object, Attribute, user));
                        }
                        break;
                    }
                case AttributeType.File:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new FileAttributeValue(null, null, Object, Attribute, user));
                        }
                        break;
                    }
                case AttributeType.Link:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new LinkAttributeValue(null, Object, Attribute, user));
                        }
                        break;
                    }
                case AttributeType.Float:
                    {
                        foreach (Object Object in Objects)
                        {
                            Object.AttributeValues.Add(new FloatAttributeValue(null, Object, Attribute, user));
                        }
                        break;
                    }
            }

            _attributeRepo.Add(Attribute);
            _objectRepo.UpdateRange(Objects);
        }

        public void AddUser(User user)
        {
            _userRepo.Add(user);
        }

        public void DeleteUser(Guid userId)
        {
            _userRepo.Delete(_userRepo.GetBy(userId));
        }

        public User GetCurrentUser()
        {
            return _userRepo.GetBy(Storage.SessionUserId);
        }

        public User GetUser(Guid userId)
        {
            return _userRepo.GetBy(userId);
        }

        public List<User> GetUsers()
        {
            return _userRepo.Query().ToList();
        }


    }
}
