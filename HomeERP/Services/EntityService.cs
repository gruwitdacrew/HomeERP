using HomeERP.Models.Domain;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;
using Attribute = HomeERP.Models.Domain.Attribute;
using Object = HomeERP.Models.Domain.Object;

namespace HomeERP.Services
{
    public class EntityService
    {
        private readonly AppDBContext _context;
        public EntityService(AppDBContext context)
        {
            _context = context;
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
        public void CreateObjectWithAttributeValues(Object Object, List<AttributeValue> AttributeValues)
        {
            _context.Objects.Add(Object);
            foreach (AttributeValue AttributeValue in AttributeValues)
            {
                if (AttributeValue is IntegerAttributeValue IAV)
                {
                    _context.IntegerAttributeValues.Add(IAV);
                }
                else if (AttributeValue is StringAttributeValue SAV)
                {
                    _context.StringAttributeValues.Add(SAV);
                }
                else if (AttributeValue is DateAttributeValue DAV)
                {
                    _context.DateAttributeValues.Add(DAV);
                }
                else if (AttributeValue is LinkAttributeValue LAV)
                {
                    _context.LinkAttributeValues.Add(LAV);
                }
            }
            _context.SaveChanges();
        }

        public void EditObjectWithAttributeValues(Object Object, List<AttributeValue> AttributeValues)
        {
            _context.Objects.Update(Object);
            foreach (AttributeValue AttributeValue in AttributeValues)
            {
                if (AttributeValue is IntegerAttributeValue IAV)
                {
                    _context.IntegerAttributeValues.Update(IAV);
                }
                else if (AttributeValue is StringAttributeValue SAV)
                {
                    _context.StringAttributeValues.Update(SAV);
                }
                else if (AttributeValue is DateAttributeValue DAV)
                {
                    _context.DateAttributeValues.Update(DAV);
                }
                else if (AttributeValue is LinkAttributeValue LAV)
                {
                    _context.LinkAttributeValues.Update(LAV);
                }
            }
            _context.SaveChanges();
        }

        public Entity GetEntity(Guid EntityId)
        {
            return _context.Entities.Where(Entity => Entity.Id == EntityId).First();
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
            return _context.Objects.Where(Object => Object.Id == ObjectId).Include(Object => Object.Entity).First();
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
            return _context.Objects.Where(Object => Object.Entity.Id == EntityId).ToList();
        }

        public List<AttributeValue> GetObjectAttributeValues(Guid ObjectId)
        {
            List<AttributeValue> AttributeValues = new List<AttributeValue>();

            Object Object = _context.Objects.Where(Object => Object.Id == ObjectId).First();
            List<Attribute> EntityAttributes = _context.Attributes.Where(Attribute => Attribute.Entity == Object.Entity).ToList();

            foreach (Attribute Attribute in EntityAttributes)
            {
                AttributeValue? tmp;
                switch (Attribute.Type)
                {
                    case AttributeType.Integer:
                        {
                            tmp = _context.IntegerAttributeValues.Where(AttributeValue => AttributeValue.Object == Object && AttributeValue.Attribute == Attribute).First();
                            break;
                        }
                    case AttributeType.String:
                        {
                            tmp = _context.StringAttributeValues.Where(AttributeValue => AttributeValue.Object == Object && AttributeValue.Attribute == Attribute).First();
                            break;
                        }
                    case AttributeType.Date:
                        {
                            tmp = _context.DateAttributeValues.Where(AttributeValue => AttributeValue.Object == Object && AttributeValue.Attribute == Attribute).First();
                            break;
                        }
                    default:
                        {
                            tmp = _context.LinkAttributeValues.Where(AttributeValue => AttributeValue.Object == Object && AttributeValue.Attribute == Attribute).FirstOrDefault();
                            break;
                        }
                }
                AttributeValues.Add(tmp);
            }

            return AttributeValues;
        }
    }
}
