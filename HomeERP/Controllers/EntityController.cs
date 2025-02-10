using System.Diagnostics;
using HomeERP.Models;
using HomeERP.Services;
using Microsoft.AspNetCore.Mvc;
using HomeERP.Models.Domain;
using HomeERP.Models.DTOs.Response;
using Attribute = HomeERP.Models.Domain.Attribute;
using Object = HomeERP.Models.Domain.Object;

namespace HomeERP.Controllers
{
    public class EntityController : Controller
    {
        private readonly EntityService _entitiesService;

        public EntityController(EntityService entitiesService)
        {
            _entitiesService = entitiesService;
        }

        public IActionResult Explorer(Guid? CurrentEntityId)
        {
            if (CurrentEntityId == null)
            {
                return View(new EntityCollectionResponse(null, _entitiesService.GetEntities()));
            }
            else
            {
                Entity Entity = _entitiesService.GetEntity((Guid)CurrentEntityId);
                List<Attribute> Attributes = _entitiesService.GetEntityAttributes((Guid)CurrentEntityId);
                List<Object> Objects = _entitiesService.GetEntityObjects((Guid)CurrentEntityId);

                Dictionary<Object, Dictionary<Attribute, AttributeValue>> AttributeValues = new Dictionary<Object, Dictionary<Attribute, AttributeValue>>();
                foreach (Object Object in Objects)
                {
                    Dictionary<Attribute, AttributeValue> ObjectAttributeValues = new Dictionary<Attribute, AttributeValue>();
                    foreach (AttributeValue AttributeValue in _entitiesService.GetObjectAttributeValues(Object.Id))
                    {
                        ObjectAttributeValues.Add(AttributeValue.Attribute, AttributeValue);
                    }
                    AttributeValues.Add(Object, ObjectAttributeValues);
                }
                return View(new EntityCollectionResponse(new EntityWideResponse(Entity, Attributes, Objects, AttributeValues), _entitiesService.GetEntities()));
            }
        }

        [HttpGet]
        public IActionResult Create()
        {

            return View(_entitiesService.GetEntities());
        }

        [HttpPost]
        public IActionResult Create(CreateEntityRequest request)
        {
            Entity Entity = new Entity(request.EntityName);
            List<Attribute> Attributes = new List<Attribute>();
            List<LinkAttribute> LinkAttributes = new List<LinkAttribute>();
            foreach (AttributeDTO AttributeRaw in request.RawAttributes ?? new List<AttributeDTO>())
            {
                if (AttributeRaw.AttributeType == AttributeType.Link)
                {
                    LinkAttributes.Add(new LinkAttribute(AttributeRaw, Entity));
                }
                else Attributes.Add(new Attribute(AttributeRaw, Entity));
            }

            _entitiesService.CreateEntityWithAttributes(Entity, Attributes, LinkAttributes);

            return RedirectToAction("Explorer", "Entity", new { CurrentEntityId = Entity.Id });
        }

        public IActionResult DeleteEntity(Guid EntityId)
        {
            _entitiesService.DeleteEntity(EntityId);

            return RedirectToAction("Explorer", "Entity");
        }

        public IActionResult DeleteObject(Guid ObjectId)
        {
            Entity Entity = _entitiesService.GetObjectEntity(ObjectId);
            _entitiesService.DeleteObject(ObjectId);

            return RedirectToAction("Explorer", "Entity", new { CurrentEntityId = Entity.Id });
        }

        [HttpGet]
        public IActionResult CreateObject(Guid EntityId)
        {
            Entity Entity = _entitiesService.GetEntity(EntityId);
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(EntityId);

            return View(new EntityResponse(Entity, Attributes));
        }

        [HttpPost]
        public IActionResult CreateObject(CreateObjectRequest request)
        {
            Object Object = new Object(request.ObjectName, _entitiesService.GetEntity(request.EntityId));
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(request.EntityId);
            List<AttributeValue> AttributeValues = new List<AttributeValue>();
            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                switch (AttributeValueRaw.AttributeType)
                {
                    case AttributeType.Integer:
                        {
                            AttributeValues.Add(new IntegerAttributeValue(AttributeValueRaw, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.String:
                        {
                            AttributeValues.Add(new StringAttributeValue(AttributeValueRaw, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.Date:
                        {
                            AttributeValues.Add(new DateAttributeValue(AttributeValueRaw, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                    case AttributeType.Link:
                        {
                            AttributeValues.Add(new LinkAttributeValue(AttributeValueRaw, Object, Attributes.Where(Attribute => Attribute.Id == AttributeValueRaw.AttributeId).First()));
                            break;
                        }
                }
            }

            _entitiesService.CreateObjectWithAttributeValues(Object, AttributeValues);

            return RedirectToAction("Explorer", "Entity", new { CurrentEntityId = Object.Entity.Id });
        }

        [HttpPost]
        public IActionResult EditObject(EditObjectRequest request)
        {
            Object Object = _entitiesService.GetObject(request.ObjectId);
            Object.Name = request.ObjectName;
            List<Attribute> Attributes = _entitiesService.GetEntityAttributes(Object.Entity.Id);
            List<AttributeValue> AttributeValues = _entitiesService.GetObjectAttributeValues(Object.Id);
            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                switch (AttributeValueRaw.AttributeType)
                {
                    case AttributeType.Integer:
                        {
                            (AttributeValues.Where(AttributeValue => AttributeValue.AttributeId == AttributeValueRaw.AttributeId).First() as IntegerAttributeValue).Value = int.Parse(AttributeValueRaw.AttributeValue);
                            break;
                        }
                    case AttributeType.String:
                        {
                            (AttributeValues.Where(AttributeValue => AttributeValue.AttributeId == AttributeValueRaw.AttributeId).First() as StringAttributeValue).Value = AttributeValueRaw.AttributeValue;
                            break;
                        }
                    case AttributeType.Date:
                        {
                            (AttributeValues.Where(AttributeValue => AttributeValue.AttributeId == AttributeValueRaw.AttributeId).First() as DateAttributeValue).Value = DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime();
                            break;
                        }
                    case AttributeType.Link:
                        {
                            (AttributeValues.Where(AttributeValue => AttributeValue.AttributeId == AttributeValueRaw.AttributeId).First() as LinkAttributeValue).Value = Guid.Parse(AttributeValueRaw.AttributeValue);
                            break;
                        }
                }
            }

            _entitiesService.EditObjectWithAttributeValues(Object, AttributeValues);

            return RedirectToAction("Explorer", "Entity", new { CurrentEntityId = Object.Entity.Id });
        }

        public class CreateEntityRequest
        {
            public string EntityName { get; set; }

            public List<AttributeDTO> RawAttributes { get; set; }
        }

        public class AttributeDTO
        {
            public string AttributeName { get; set; }

            public AttributeType AttributeType { get; set; }

            public List<string>? Args { get; set; }
        }

        public class CreateObjectRequest
        {
            public Guid EntityId { get; set; }

            public string ObjectName { get; set; }

            public List<AttributeValueDTO> RawAttributeValues { get; set; }
        }

        public class EditObjectRequest
        {
            public Guid ObjectId { get; set; }

            public string ObjectName { get; set; }

            public List<AttributeValueDTO> RawAttributeValues { get; set; }
        }

        public class AttributeValueDTO
        {
            public Guid AttributeId { get; set; }

            public string AttributeValue { get; set; }

            public AttributeType AttributeType { get; set; }
        }
    }
}
