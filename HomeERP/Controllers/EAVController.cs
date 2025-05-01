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
            if (EntityId != null) TempData["EntityId"] = EntityId;

            return View(_entitiesService.GetEntities());
        }

        public IActionResult Entity(Guid EntityId)
        {
            TempData["Entities"] = _entitiesService.GetEntities();

            return PartialView(_entitiesService.GetEntityObjects(EntityId));
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

            TempData["Entities"] = _entitiesService.GetEntities();

            return PartialView("Entity", Entity);
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


        public IActionResult DeleteObject(Guid ObjectId)
        {
            Entity Entity = _entitiesService.GetObjectEntity(ObjectId);
            _entitiesService.DeleteObject(ObjectId);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Entity.Id });
        }

        [HttpPost]
        public IActionResult CreateObject(CreateObjectRequest request)
        {
            Object Object = new Object(request.ObjectName, _entitiesService.GetEntity(request.EntityId));
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(request.EntityId);

            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                Attribute Attribute = Attributes.First(Attribute => Attribute.Id == AttributeValueRaw.AttributeId);

                switch (Attribute.Type)
                {
                    case AttributeType.Integer:
                        {
                            Object.AttributeValues.Add(new IntegerAttributeValue(AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute));
                            break;
                        }
                    case AttributeType.String:
                        {
                            Object.AttributeValues.Add(new StringAttributeValue(AttributeValueRaw.AttributeValue, Object, Attribute));
                            break;
                        }
                    case AttributeType.Date:
                        {
                            Object.AttributeValues.Add(new DateAttributeValue(AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime() : null, Object, Attribute));
                            break;
                        }
                    case AttributeType.Link:
                        {
                            Object.AttributeValues.Add(new LinkAttributeValue(AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute));
                            break;
                        }
                    case AttributeType.File:
                        {
                            Object.AttributeValues.Add(new FileAttributeValue(AttributeValueRaw.File != null ? Guid.NewGuid() : null, AttributeValueRaw.File, Object, Attribute));
                            break;
                        }
                    case AttributeType.Float:
                        {
                            Object.AttributeValues.Add(new FloatAttributeValue(AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute));
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
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(Object.Entity.Id);

            List<AttributeValue> NewAttributeValues = new List<AttributeValue>();

            Object.Name = request.ObjectName;
            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                Attribute Attribute = Attributes.First(Attribute => Attribute.Id == AttributeValueRaw.AttributeId);

                AttributeValue AttributeValueOld = Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == Attribute.Id && AttributeValue.IsCurrent);

                switch (Attribute.Type)
                {
                    case AttributeType.Integer:
                        {
                            int? NewValue = AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((IntegerAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                if ((DateTime.UtcNow - ((DateTime)AttributeValueOld.ChangeDate)).Days <= 0)
                                {
                                    ((IntegerAttributeValue)AttributeValueOld).Value = NewValue;
                                }
                                else
                                {
                                    AttributeValueOld.IsCurrent = false;
                                    NewAttributeValues.Add(new IntegerAttributeValue(NewValue, Object, Attribute));
                                }
                            }
                            break;
                        }
                    case AttributeType.String:
                        {
                            if (((StringAttributeValue)AttributeValueOld).Value != AttributeValueRaw.AttributeValue)
                            {
                                if ((DateTime.UtcNow - ((DateTime)AttributeValueOld.ChangeDate)).Days <= 0)
                                {
                                    ((StringAttributeValue)AttributeValueOld).Value = AttributeValueRaw.AttributeValue;
                                }
                                else
                                {
                                    AttributeValueOld.IsCurrent = false;
                                    NewAttributeValues.Add(new StringAttributeValue(AttributeValueRaw.AttributeValue, Object, Attribute));
                                }
                            }
                            break;
                        }
                    case AttributeType.Date:
                        {
                            
                            DateTime? NewValue = AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime() : null;
                            if (((DateAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                if ((DateTime.UtcNow - ((DateTime)AttributeValueOld.ChangeDate)).Days <= 0)
                                {
                                    ((DateAttributeValue)AttributeValueOld).Value = NewValue;
                                }
                                else
                                {
                                    AttributeValueOld.IsCurrent = false;
                                    NewAttributeValues.Add(new DateAttributeValue(NewValue, Object, Attribute));
                                }
                            }
                            break;
                        }
                    case AttributeType.Link:
                        {
                            Guid? NewValue = AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((LinkAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                if ((DateTime.UtcNow - ((DateTime)AttributeValueOld.ChangeDate)).Days <= 0)
                                {
                                    ((LinkAttributeValue)AttributeValueOld).Value = NewValue;
                                }
                                else
                                {
                                    AttributeValueOld.IsCurrent = false;
                                    NewAttributeValues.Add(new LinkAttributeValue(NewValue, Object, Attribute));
                                }
                            }
                            break;
                        }
                    case AttributeType.File:
                        {

                            ((FileAttributeValue)AttributeValueOld).File = AttributeValueRaw.File;
                            if (AttributeValueRaw.File != null) ((FileAttributeValue)AttributeValueOld).FileId = Guid.NewGuid();
                            break;
                        }
                    case AttributeType.Float:
                        {
                            float? NewValue = AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((FloatAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                if ((DateTime.UtcNow - ((DateTime)AttributeValueOld.ChangeDate)).Days <= 0)
                                {
                                    ((FloatAttributeValue)AttributeValueOld).Value = NewValue;
                                }
                                else
                                {
                                    AttributeValueOld.IsCurrent = false;
                                    NewAttributeValues.Add(new FloatAttributeValue(NewValue, Object, Attribute));
                                }
                            }
                            break;
                        }
                }
            }

            _entitiesService.UpdateObject(Object, NewAttributeValues);

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

        public IActionResult ObjectHistory(Guid ObjectId)
        {
            Object Object = _entitiesService.GetObject(ObjectId);
            _entitiesService.GetObjectHistory(Object);

            return PartialView(Object);
        }
    }
}
