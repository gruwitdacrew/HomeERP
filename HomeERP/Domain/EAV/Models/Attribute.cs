using System.ComponentModel.DataAnnotations.Schema;
using HomeERP.Domain.Common.Models;
using HomeERP.Views.EAV.DTOs;

namespace HomeERP.Domain.EAV.Models
{
    public class Attribute : BaseEntity
    {
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
        public virtual Entity LinkedEntity { get; set; }

        public LinkAttribute() { }

        public LinkAttribute(AttributeDTO AttributeRaw, Entity Entity, Entity LinkedEntity) : base(AttributeRaw, Entity)
        {
            this.LinkedEntity = LinkedEntity;
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
