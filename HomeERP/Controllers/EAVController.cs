using HomeERP.Logic;
using Microsoft.AspNetCore.Mvc;
using HomeERP.Views.EAV.DTOs;
using HomeERP.Views.EAV.DTOs.Request;
using Attribute = HomeERP.Domain.EAV.Models.Attribute;
using Object = HomeERP.Domain.EAV.Models.Object;
using HomeERP.Domain.EAV.Models;
using HomeERP.Domain.Common.Models;
using HomeERP.Domain.Common;
using Microsoft.AspNetCore.Http.Extensions;

namespace HomeERP.Controllers
{
    public class EAVController : Controller
    {
        private readonly EAVService _entitiesService;
        private readonly ProductService _productService;

        public EAVController(EAVService entitiesService, ProductService productService)
        {
            _entitiesService = entitiesService;
            _productService = productService;
        }

        public IActionResult Explorer(Guid? EntityId)
        {
            if (EntityId != null) TempData["EntityId"] = EntityId;

            return View(_entitiesService.GetEntities());
        }

        public IActionResult Entity(Guid EntityId)
        {
            TempData["Entities"] = _entitiesService.GetEntities();

            Entity entity = _entitiesService.GetEntity(EntityId);
            _entitiesService.GetEntityObjects(entity);

            return PartialView(entity);
        }

        public IActionResult SearchObjects(SearchObjectsRequest SearchObjectsRequest)
        {
            Entity entity = _entitiesService.GetEntity(SearchObjectsRequest.EntityId);
            _entitiesService.SearchEntityObjects(SearchObjectsRequest, entity);

            TempData["NameSearchAttribute"] = SearchObjectsRequest.NameSearchAttribute;
            foreach (var SearchAttribute in SearchObjectsRequest.SearchAttributes)
            {
                TempData[SearchAttribute.AttributeId.ToString()] = SearchAttribute.Args;
            }

            TempData["Entities"] = _entitiesService.GetEntities();

            return PartialView("Entity", entity);
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
                    Entity.Attributes.Add(new LinkAttribute(AttributeRaw, Entity, _entitiesService.GetEntity(new Guid(AttributeRaw.Args[0]))));
                }
                else Entity.Attributes.Add(new Attribute(AttributeRaw, Entity));
            }

            _entitiesService.CreateEntity(Entity);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Entity.Id });
        }

        public IActionResult DeleteEntity(Guid entityId)
        {
            Entity entity = _entitiesService.GetEntity(entityId);
            _entitiesService.DeleteEntity(entity);

            return RedirectToAction("Explorer", "EAV");
        }


        public IActionResult DeleteObject(Guid objectId)
        {
            Object Object = _entitiesService.GetObjectCurrent(objectId);
            _entitiesService.DeleteObject(Object);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Object.Entity.Id });
        }

        [HttpPost]
        public async Task<IActionResult> CreateObject(CreateObjectRequest request)
        {
            Entity entity = _entitiesService.GetEntity(request.EntityId);
            Object Object = new Object(request.ObjectName, entity);
            User user = _entitiesService.GetCurrentUser();

            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                Attribute Attribute = Object.Entity.Attributes.First(Attribute => Attribute.Id == AttributeValueRaw.AttributeId);

                switch (Attribute.Type)
                {
                    case AttributeType.Integer:
                        {
                            Object.AttributeValues.Add(new IntegerAttributeValue(AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute, user));
                            break;
                        }
                    case AttributeType.String:
                        {
                            Object.AttributeValues.Add(new StringAttributeValue(AttributeValueRaw.AttributeValue, Object, Attribute, user));
                            break;
                        }
                    case AttributeType.Date:
                        {
                            Object.AttributeValues.Add(new DateAttributeValue(AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime() : null, Object, Attribute, user));
                            break;
                        }
                    case AttributeType.Link:
                        {
                            Object.AttributeValues.Add(new LinkAttributeValue(AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute, user));
                            break;
                        }
                    case AttributeType.File:
                        {
                            Object.AttributeValues.Add(new FileAttributeValue(AttributeValueRaw.File != null ? Guid.NewGuid() : null, AttributeValueRaw.File, Object, Attribute, user));
                            break;
                        }
                    case AttributeType.Float:
                        {
                            Object.AttributeValues.Add(new FloatAttributeValue(AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null, Object, Attribute, user));
                            break;
                        }
                }
            }

            await _entitiesService.CreateObject(Object);

            if (entity.Id == new Guid("e2603327-15ec-4b67-96f0-be16467a9dbf")) _productService.AddProductToInventory(Object);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Object.Entity.Id });
        }

        [HttpPost]
        public async Task<IActionResult> EditObject(EditObjectRequest request)
        {
            Object Object = _entitiesService.GetObjectCurrent(request.ObjectId);
            User user = _entitiesService.GetCurrentUser();

            List<AttributeValue> NewAttributeValues = new List<AttributeValue>();

            Object.Name = request.ObjectName;
            foreach (AttributeValueDTO AttributeValueRaw in request.RawAttributeValues ?? new List<AttributeValueDTO>())
            {
                Attribute Attribute = Object.Entity.Attributes.First(Attribute => Attribute.Id == AttributeValueRaw.AttributeId);
                AttributeValue AttributeValueOld = Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == Attribute.Id && AttributeValue.IsCurrent);

                switch (Attribute.Type)
                {
                    case AttributeType.Integer:
                        {
                            int? NewValue = AttributeValueRaw.AttributeValue != null ? int.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((IntegerAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                AttributeValueOld.IsCurrent = false;
                                Object.AttributeValues.Add(new IntegerAttributeValue(NewValue, Object, Attribute, user));
                            }
                            break;
                        }
                    case AttributeType.String:
                        {
                            if (((StringAttributeValue)AttributeValueOld).Value != AttributeValueRaw.AttributeValue)
                            {
                                AttributeValueOld.IsCurrent = false;
                                Object.AttributeValues.Add(new StringAttributeValue(AttributeValueRaw.AttributeValue, Object, Attribute, user));
                            }
                            break;
                        }
                    case AttributeType.Date:
                        {
                            
                            DateTime? NewValue = AttributeValueRaw.AttributeValue != null ? DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime() : null;
                            if (((DateAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                AttributeValueOld.IsCurrent = false;
                                Object.AttributeValues.Add(new DateAttributeValue(NewValue, Object, Attribute, user));
                            }
                            break;
                        }
                    case AttributeType.Link:
                        {
                            Guid? NewValue = AttributeValueRaw.AttributeValue != null ? Guid.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((LinkAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                AttributeValueOld.IsCurrent = false;
                                Object.AttributeValues.Add(new LinkAttributeValue(NewValue, Object, Attribute, user));
                            }
                            break;
                        }
                    case AttributeType.File:
                        {

                            ((FileAttributeValue)AttributeValueOld).File = AttributeValueRaw.File;
                            if (((FileAttributeValue)AttributeValueOld).FileId != null)
                            {
                                await _entitiesService.DeleteFile((FileAttributeValue)AttributeValueOld);
                            }
                            if (AttributeValueRaw.File != null)
                            {
                                ((FileAttributeValue)AttributeValueOld).FileId = Guid.NewGuid();
                            }
                            break;
                        }
                    case AttributeType.Float:
                        {
                            float? NewValue = AttributeValueRaw.AttributeValue != null ? float.Parse(AttributeValueRaw.AttributeValue) : null;
                            if (((FloatAttributeValue)AttributeValueOld).Value != NewValue)
                            {
                                AttributeValueOld.IsCurrent = false;
                                Object.AttributeValues.Add(new FloatAttributeValue(NewValue, Object, Attribute, user));
                            }
                            break;
                        }
                }
            }

            await _entitiesService.UpdateObject(Object);

            return RedirectToAction("Explorer", "EAV", new { EntityId = Object.Entity.Id });
        }

        [HttpGet]
        public async Task<FileContentResult> GetFile(Guid FileId)
        {
            return await _entitiesService.GetFile(FileId);
        }

        public async Task<IActionResult> DeleteFile(Guid FileId)
        {
            FileAttributeValue FileAttributeValue = _entitiesService.GetFileAttributeValue(FileId);
            await _entitiesService.DeleteFile(FileAttributeValue);

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
                Attribute = new LinkAttribute(AttributeDTO, Entity, _entitiesService.GetEntity(new Guid(AttributeDTO.Args[0])));
            }
            else Attribute = new Attribute(AttributeDTO, Entity);

            _entitiesService.AddAttribute(Entity, Attribute);

            return RedirectToAction("Explorer", "EAV", new { EntityId = EntityId });
        }

        public IActionResult ObjectHistory(Guid objectId)
        {
            Object Object = _entitiesService.GetObjectWithHistory(objectId);

            return PartialView(Object);
        }

        [HttpPost]
        public IActionResult AddUser(string userName)
        {
            User user = new User(userName);
            _entitiesService.AddUser(user);

            return RedirectToAction("GetUsers", "EAV");
        }

        public IActionResult DeleteUser(Guid userId, string returnUrl)
        {
            _entitiesService.DeleteUser(userId);

            Storage.SessionUserId = _entitiesService.GetUsers().First().Id;

            return Redirect(returnUrl);
        }

        public IActionResult SelectUser(Guid userId, string returnUrl)
        {
            Storage.SessionUserId = userId;

            return Redirect(returnUrl);
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            return PartialView("UserList", _entitiesService.GetUsers());
        }
    }
}
