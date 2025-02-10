using HomeERP.Models.Domain;
using Attribute = HomeERP.Models.Domain.Attribute;

namespace HomeERP.Models.DTOs.Response
{
    public class EntityResponse
    {
        public Entity Entity { get; set; }

        public List<Attribute> Attributes { get; set; }



        public EntityResponse() {}
        public EntityResponse(Entity Entity, List<Attribute> Attributes)
        {
            this.Entity = Entity;
            this.Attributes = Attributes;
        }

        public EntityResponse(EntityWideResponse entityWideResponse)
        {
            this.Entity = entityWideResponse.Entity;
            this.Attributes = entityWideResponse.Attributes;
        }
    }
}
