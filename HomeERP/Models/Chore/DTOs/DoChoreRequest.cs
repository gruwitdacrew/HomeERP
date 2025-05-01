namespace HomeERP.Models.Chore.DTOs
{
    public class DoChoreRequest
    {
        public Guid ChoreId { get; set; }
        public List<ChoreTask> Tasks { get; set; }
        public DateTime NewDate { get; set; }
    }

    public class ChoreTask
    {
        public Guid ObjectId { get; set; }
        public string IsDone { get; set; }
    }
}
