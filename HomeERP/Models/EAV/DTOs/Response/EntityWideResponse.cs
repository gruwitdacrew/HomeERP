using HomeERP.Models.EAV.Domain;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;
using Object = HomeERP.Models.EAV.Domain.Object;

namespace HomeERP.Models.EAV.DTOs.Response
{
    public class EntityWideResponse
    {
        public Entity Entity { get; set; }

        public List<Attribute> Attributes { get; set; }

        public List<Object> Objects { get; set; }

        public EntityWideResponse(Entity Entity, List<Attribute> Attributes, List<Object> Objects)
        {
            this.Entity = Entity;
            this.Attributes = Attributes;
            this.Objects = Objects;
        }
    }
}
