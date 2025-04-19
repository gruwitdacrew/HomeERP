using HomeERP.Controllers;

namespace HomeERP.Models.EAV.Domain
{
    public class Entity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public List<Object> Objects { get; set; }
        public List<Attribute> Attributes { get; set; }

        public Entity(string Name)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Attributes = new List<Attribute>();
        }
    }
}
