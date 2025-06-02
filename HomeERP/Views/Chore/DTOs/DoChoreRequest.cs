namespace HomeERP.Views.Chores.DTOs
{
    public class DoChoreRequest
    {
        public Guid ChoreId { get; set; }
        public List<Guid> TaskIds { get; set; }
        public DateTime? NewDate { get; set; }
        public TimeSpan? TrackedTime { get; set; }
    }
}
