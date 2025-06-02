using HomeERP.Domain.EAV.Models;

namespace HomeERP.Views.EAV.DTOs.Request
{
    public class SearchObjectsRequest
    {
        public Guid EntityId { get; set; }
        public string NameSearchAttribute { get; set; }
        public List<SearchAttribute> SearchAttributes { get; set; } = new List<SearchAttribute>();
    }

    public class SearchAttribute
    {
        public Guid AttributeId { get; set; }

        public AttributeType AttributeType { get; set; }

        public List<string> Args { get; set; }
    }
}
