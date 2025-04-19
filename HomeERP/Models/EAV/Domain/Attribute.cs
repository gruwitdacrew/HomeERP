
using System.ComponentModel.DataAnnotations.Schema;
using HomeERP.Controllers;
using HomeERP.Models.EAV.DTOs;
using static HomeERP.Controllers.EAVController;

namespace HomeERP.Models.EAV.Domain
{
    public class Attribute
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Entity Entity { get; set; }

        public AttributeType Type { get; set; }

        public Attribute() { }

        public Attribute(AttributeDTO AttributeRaw, Entity Entity)
        {
            Id = Guid.NewGuid();
            Name = AttributeRaw.AttributeName;
            this.Entity = Entity;
            Type = AttributeRaw.AttributeType;
        }
    }

    public class LinkAttribute : Attribute
    {
        public Guid LinkedEntityId { get; set; }

        [NotMapped]
        public List<Object> EntityObjects { get; set; }

        public LinkAttribute() { }

        public LinkAttribute(AttributeDTO AttributeRaw, Entity Entity) : base(AttributeRaw, Entity)
        {
            LinkedEntityId = new Guid(AttributeRaw.Args[0]);
        }
    }

    public enum AttributeType
    {
        Integer,
        String,
        Date,
        Link,
        File,
        Float
    }
}
