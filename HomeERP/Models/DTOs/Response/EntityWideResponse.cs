using HomeERP.Models.Domain;
using Attribute = HomeERP.Models.Domain.Attribute;
using Object = HomeERP.Models.Domain.Object;

namespace HomeERP.Models.DTOs.Response
{
    public class EntityWideResponse
    {
        public Entity Entity { get; set; }

        public List<Attribute> Attributes { get; set; }

        public List<Object> Objects { get; set; }

        public Dictionary<Object, Dictionary<Attribute, AttributeValue>> AttributeValues { get; set; }

        public EntityWideResponse(Entity Entity, List<Attribute> Attributes, List<Object> Objects, Dictionary<Object, Dictionary<Attribute, AttributeValue>> AttributeValues)
        {
            this.Entity = Entity;
            this.Attributes = Attributes;
            this.Objects = Objects;
            this.AttributeValues = AttributeValues;
        }

        public EntityWideResponse()
        {
            
        }
    }
}
