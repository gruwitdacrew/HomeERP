using HomeERP.Controllers;

namespace HomeERP.Models.Domain
{
    public class Entity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Entity(string Name)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
        }
    }
}
