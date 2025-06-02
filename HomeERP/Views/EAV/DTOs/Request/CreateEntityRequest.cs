using HomeERP.Views.EAV.DTOs;

namespace HomeERP.Views.EAV.DTOs.Request
{
    public class CreateEntityRequest
    {
        public string EntityName { get; set; }

        public List<AttributeDTO> RawAttributes { get; set; }
    }
}
