using HomeERP.Models.Chore.Domain;
using HomeERP.Models.EAV.Domain;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;
using Object = HomeERP.Models.EAV.Domain.Object;

namespace HomeERP.Services
{
    public class ChoreService
    {
        private readonly AppDBContext _context;
        public ChoreService(AppDBContext context)
        {
            _context = context;
        }

        public List<Entity> GetEntities()
        {
            List<Entity> Entities = _context.Entities.Where(Entity => Entity.Attributes.Any(Attribute => Attribute.Type == AttributeType.Date)).ToList();
            foreach (Entity Entity in Entities)
            {
                _context.Attributes.Where(Attribute => Attribute.Entity == Entity && Attribute.Type == AttributeType.Date).ToList();
            }
            return Entities;
        }

        public List<Chore> GetChores()
        {
            return _context.Chores.ToList();
        }

        public Attribute GetDateAttribute(Guid AttributeId)
        {
            return _context.Attributes.First(Attribute => Attribute.Id == AttributeId && Attribute.Type == AttributeType.Date);
        }

        public Chore GetChoreTasks(Guid ChoreId)
        {
            Chore Chore = _context.Chores.Include(Chore => Chore.Attribute).ThenInclude(Attribute => Attribute.Entity).First(Chore => Chore.Id == ChoreId);

            List<Object> Objects = _context.Objects.Where(Object => Object.Entity == Chore.Attribute.Entity && Object.AttributeValues.First(AttributeValue =>
                                                    AttributeValue.AttributeId == Chore.Attribute.Id &&
                                                    ((DateAttributeValue)AttributeValue).Value.HasValue) != null).OrderBy(Object => (Object.AttributeValues.First(AttributeValue => AttributeValue.AttributeId == Chore.Attribute.Id) as DateAttributeValue).Value).ToList();

            _context.DateAttributeValues.Where(AttributeValue => AttributeValue.Attribute == Chore.Attribute && Objects.Contains(AttributeValue.Object)).ToList();

            return Chore;
        }

        public void CreateChore(Chore Chore)
        {
            _context.Chores.Add(Chore);
            _context.SaveChanges();
        }

        public Chore GetChore(Guid ChoreId)
        {
            return _context.Chores.Include(Chore => Chore.Attribute).First(Chore => Chore.Id == ChoreId);
        }

        public void DoChore(Chore Chore, List<Guid> ObjectIds, DateTime NewDate)
        {
            List<DateAttributeValue> DateAttributeValues = _context.DateAttributeValues.Where(AttributeValue => ObjectIds.Contains(AttributeValue.Object.Id) && AttributeValue.Attribute == Chore.Attribute).ToList();

            foreach (DateAttributeValue DateAttributeValue in DateAttributeValues)
            {
                DateAttributeValue.Value = NewDate.ToUniversalTime();
            }

            _context.DateAttributeValues.UpdateRange(DateAttributeValues);
            _context.SaveChanges();
        }

        public void DeleteChore(Chore Chore)
        {
            _context.Chores.Remove(Chore);
            _context.SaveChanges();
        }
    }
}
