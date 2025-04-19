namespace HomeERP.Models.EAV.DTOs.Request
{
    public class CreateEntityRequest
    {
        public string EntityName { get; set; }

        public List<AttributeDTO> RawAttributes { get; set; }
    }
}
