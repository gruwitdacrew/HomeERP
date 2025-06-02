using HomeERP.Domain.Chores.Models;

namespace HomeERP.Views.Chores.DTOs
{
    public class ChoreDTO
    {
        public string Name { get; set; }
        public ChoreType ChoreType { get; set; }
        public int Days { get; set; }
        public int Hours { get; set; }
        public List<Guid> ObjectIds { get; set; }
    }
}
