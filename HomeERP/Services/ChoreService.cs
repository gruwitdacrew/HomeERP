using HomeERP.Models.Chore.Domain;
using HomeERP.Models.EAV.Domain;
using Logistics.Data;
using Microsoft.EntityFrameworkCore;

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
            List<Entity> Entities = _context.Entities.ToList();
            foreach (Entity Entity in Entities)
            {
                _context.Attributes.Where(Attribute => Attribute.Entity == Entity && Attribute.Type == AttributeType.Date).ToList();
            }
            return Entities;
        }

        public List<Chore> GetChores()
        {
            List<Chore> Chores = _context.Chores.Include(Chore => Chore.Attribute).ThenInclude(Attribute => Attribute.Entity).ToList();
            foreach (Chore Chore in Chores)
            {
                _context.Objects.Where(Object =>
                    Object.Entity == Chore.Attribute.Entity &&
                    Object.AttributeValues.First(AttributeValue => 
                                                    AttributeValue.AttributeId == Chore.Attribute.Id && 
                                                    (AttributeValue as DateAttributeValue).Value.HasValue && 
                                                    (Chore.WarningType == WarningType.After && ((DateTime)(AttributeValue as DateAttributeValue).Value).AddDays(Chore.DeltaTimeInDays) >= DateTime.UtcNow || Chore.WarningType == WarningType.Before && DateTime.UtcNow.AddDays(-Chore.DeltaTimeInDays) >= (DateTime)(AttributeValue as DateAttributeValue).Value)
                                                ) != null).ToList();
            }
            return Chores;
        }

        public void CreateChore(Chore Chore)
        {
            _context.Chores.Add(Chore);
            _context.SaveChanges();
        }
    }
}
