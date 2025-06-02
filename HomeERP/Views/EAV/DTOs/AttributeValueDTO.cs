using HomeERP.Domain.EAV.Models;

namespace HomeERP.Views.EAV.DTOs
{
    public class AttributeValueDTO
    {
        public Guid AttributeId { get; set; }

        public AttributeType AttributeType { get; set; }

        public string? AttributeValue { get; set; }

        public IFormFile? File { get; set; }
    }
}
