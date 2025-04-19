namespace HomeERP.Models.EAV.DTOs.Request
{
    public class EditObjectRequest
    {
        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public List<AttributeValueDTO> RawAttributeValues { get; set; }
    }
}
