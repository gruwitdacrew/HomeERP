using HomeERP.Domain.Common.Models;

namespace HomeERP.Domain.EAV.Models
{
    public class Object : BaseEntity
    {
        public string Name { get; set; }

        public Entity Entity { get; set; }

        public List<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();

        public Object() {}

        public Object(string Name, Entity Entity)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Entity = Entity;
        }
    }
}
