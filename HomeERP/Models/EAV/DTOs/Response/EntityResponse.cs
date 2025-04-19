using HomeERP.Models.EAV.Domain;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;

namespace HomeERP.Models.EAV.DTOs.Response
{
    public class EntityResponse
    {
        public Entity Entity { get; set; }

        public List<Attribute> Attributes { get; set; }



        public EntityResponse() { }
        public EntityResponse(Entity Entity, List<Attribute> Attributes)
        {
            this.Entity = Entity;
            this.Attributes = Attributes;
        }

        public EntityResponse(EntityWideResponse EntityWideResponse)
        {
            Entity = EntityWideResponse.Entity;
            Attributes = EntityWideResponse.Attributes;
        }
    }
}
