using HomeERP.Controllers;
using HomeERP.Domain.Common.Models;

namespace HomeERP.Domain.EAV.Models
{
    public class Entity : BaseEntity
    {
        public string Name { get; set; }

        public List<Object> Objects { get; set; } = new List<Object>();
        public List<Attribute> Attributes { get; set; } = new List<Attribute>();

        public Entity(string Name)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
        }
    }
}
