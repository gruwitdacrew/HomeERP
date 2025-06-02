using HomeERP.Views.EAV.DTOs;

namespace HomeERP.Views.EAV.DTOs.Request
{
    public class EditObjectRequest
    {
        public Guid ObjectId { get; set; }

        public string ObjectName { get; set; }

        public List<AttributeValueDTO> RawAttributeValues { get; set; }
    }
}
