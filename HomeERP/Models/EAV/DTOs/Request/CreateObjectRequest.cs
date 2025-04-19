namespace HomeERP.Models.EAV.DTOs.Request
{
    public class CreateObjectRequest
    {
        public Guid EntityId { get; set; }

        public string ObjectName { get; set; }

        public List<AttributeValueDTO> RawAttributeValues { get; set; }
    }
}
