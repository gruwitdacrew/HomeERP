using HomeERP.Services;
using Microsoft.AspNetCore.Mvc;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;
using Object = HomeERP.Models.EAV.Domain.Object;
using HomeERP.Models.EAV.Domain;
using HomeERP.Models.EAV.DTOs.Response;
using HomeERP.Models.EAV.DTOs.Request;
using HomeERP.Models.EAV.DTOs;

namespace HomeERP.Controllers
{
    public class EAVController : Controller
    {
        private readonly EAVService _entitiesService;

        public EAVController(EAVService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        public IActionResult Explorer(Guid? EntityId)
        {
            EntityWideResponse? EntityResponse = null;
            if (EntityId != null)
            {
                Entity Entity = _entitiesService.GetEntity((Guid)EntityId);
                List<Attribute> Attributes = _entitiesService.GetEntityAttributes((Guid)EntityId);
                List<Object> Objects = _entitiesService.GetEntityObjects((Guid)EntityId);

                EntityResponse = new EntityWideResponse(Entity, Attributes, Objects);
            }
            return View("Explorer", (_entitiesService.GetEntities(), EntityResponse));
        }

        public IActionResult SearchObjects(SearchObjectsRequest SearchObjectsRequest)
        {
            Entity Entity = _entitiesService.GetEntity(SearchObjectsRequest.EntityId);
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(SearchObjectsRequest.EntityId);
            List<Object> Objects = _entitiesService.SearchEntityObjects(SearchObjectsRequest);

            TempData["NameSearchAttribute"] = SearchObjectsRequest.NameSearchAttribute;
            foreach (var SearchAttribute in SearchObjectsRequest.SearchAttributes)
            {
                TempData[SearchAttribute.AttributeId.ToString()] = SearchAttribute.Args;
            }

            return View("Explorer", (_entitiesService.GetEntities(), new EntityWideResponse(Entity, Attributes, Objects)));
        }

        [HttpGet]
        public IActionResult CreateEntity()
        {
            return View(_entitiesService.GetEntities());
        }

        [HttpPost]
        public IActionResult CreateEntity(CreateEntityRequest request)
        {
            Entity Entity = new Entity(request.EntityName);

            foreach (AttributeDTO AttributeRaw in request.RawAttributes ?? new List<AttributeDTO>())
            {
                if (AttributeRaw.AttributeType == AttributeType.Link)
                {
                    Entity.Attributes.Add(new LinkAttribute(AttributeRaw, Entity));
                }
                else Entity.Attributes.Add(new Attribute(AttributeRaw, Entity));
            }

            _entitiesService.CreateEntity(Entity);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Entity.Id });
        }

        public IActionResult DeleteEntity(Guid EntityId)
        {
            _entitiesService.DeleteEntity(EntityId);

            return RedirectToAction("Explorer", "EAV");
        }



        [HttpGet]
        public IActionResult CreateObject(Guid EntityId)
        {
            Entity Entity = _entitiesService.GetEntity(EntityId);
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(EntityId);

            return View(new EntityResponse(Entity, Attributes));
        }

        public IActionResult DeleteObject(Guid ObjectId)
        {
            Entity Entity = _entitiesService.GetObjectEntity(ObjectId);
            _entitiesService.DeleteObject(ObjectId);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Entity.Id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateObject(CreateObjectRequest request)
        {
            Object Object = new Object(request.ObjectName, _entitiesService.GetEntity(request.EntityId));
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(request.EntityId);

            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                switch (AttributeValueRaw.AttributeType)
                {
                    case AttributeType.Integer:
                        {
                            Object.AttributeValues.Add(new IntegerAttributeValue(AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.String:
                        {
                            Object.AttributeValues.Add(new StringAttributeValue(AttributeValueRaw.AttributeValue, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.Date:
                        {
                            Object.AttributeValues.Add(new DateAttributeValue(AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.Link:
                        {
                            Object.AttributeValues.Add(new LinkAttributeValue(AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.File:
                        {
                            Object.AttributeValues.Add(new FileAttributeValue(AttributeValueRaw.File != null ? Guid.NewGuid() : null, AttributeValueRaw.File, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.Float:
                        {
                            Object.AttributeValues.Add(new FloatAttributeValue(AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                }
            }

            _entitiesService.CreateObject(Object);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Object.Entity.Id });
        }

        [HttpPost]
        public IActionResult EditObject(EditObjectRequest request)
        {
            Object Object = _entitiesService.GetObject(request.ObjectId);
            Object.Name = request.ObjectName;

            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                AttributeValue AttributeValue = Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == AttributeValueRaw.AttributeId);

                if (AttributeValue is IntegerAttributeValue IntegerAttributeValue)
                {
                    IntegerAttributeValue.Value = AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null;
                }
                else if (AttributeValue is StringAttributeValue StringAttributeValue)
                {
                    StringAttributeValue.Value = AttributeValueRaw.AttributeValue != null ? AttributeValueRaw.AttributeValue : null;
                }
                else if (AttributeValue is DateAttributeValue DateAttributeValue)
                {
                    DateAttributeValue.Value = AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue) : null;
                }
                else if (AttributeValue is LinkAttributeValue LinkAttributeValue)
                {
                    LinkAttributeValue.Value = AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null;
                }
                else if (AttributeValue is FloatAttributeValue FloatAttributeValue)
                {
                    FloatAttributeValue.Value = AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null;
                }
                else if (AttributeValue is FileAttributeValue FileAttributeValue && AttributeValueRaw.File != null)
                {
                    FileAttributeValue.FileId = Guid.NewGuid();
                    FileAttributeValue.File = AttributeValueRaw.File;
                }
            }

            _entitiesService.UpdateObject(Object);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Object.Entity.Id });
        }

        [HttpGet]
        public FileContentResult GetFile(Guid FileId)
        {
            return _entitiesService.GetFile(FileId);
        }

        public IActionResult DeleteFile(Guid FileId)
        {
            FileAttributeValue FileAttributeValue = _entitiesService.GetFileAttributeValue(FileId);
            _entitiesService.DeleteFile(FileAttributeValue);
            return RedirectToAction("Explorer", "EAV", new { EntityId = FileAttributeValue.Attribute.Entity.Id });
        }

        public IActionResult DeleteAttribute(Guid AttributeId)
        {
            Attribute Attribute = _entitiesService.GetAttribute(AttributeId);
            _entitiesService.DeleteAttribute(Attribute);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Attribute.Entity.Id });
        }

        [HttpGet]
        public IActionResult AddAttribute(Guid EntityId)
        {
            TempData["Entities"] = _entitiesService.GetEntities();
            return View(EntityId);
        }

        [HttpPost]
        public IActionResult AddAttribute(Guid EntityId, AttributeDTO AttributeDTO)
        {
            Entity Entity = _entitiesService.GetEntity(EntityId);
            Attribute Attribute;

            if (AttributeDTO.AttributeType == AttributeType.Link)
            {
                Attribute = new LinkAttribute(AttributeDTO, Entity);
            }
            else Attribute = new Attribute(AttributeDTO, Entity);

            _entitiesService.AddAttribute(Entity, Attribute);

            return RedirectToAction("Explorer", "EAV", new { EntityId = EntityId });
        }
    }
}
