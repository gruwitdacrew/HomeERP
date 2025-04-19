using HomeERP.Models.EAV.Domain;

namespace HomeERP.Models.EAV.DTOs
{
    public class AttributeDTO
    {
        public string AttributeName { get; set; }

        public AttributeType AttributeType { get; set; }

        public List<string>? Args { get; set; }
    }
}
