using HomeERP.Domain.Common.Models;
using HomeERP.Views.Chores.DTOs;
using Object = HomeERP.Domain.EAV.Models.Object;

namespace HomeERP.Domain.Chores.Models
{
    public abstract class Chore : BaseEntity
    {
        public string Name { get; set; }
        public ChoreType Type { get; set; }
        public List<Task> Tasks { get; set; } = new List<Task>();

        protected Chore() {}

        protected Chore(ChoreDTO choreDTO, List<Object> objects)
        {
            Name = choreDTO.Name;
            Tasks = objects.Select(Object => new Task(this, Object)).ToList();
        }
    }

    public class PlanChore : Chore
    {
        public PlanChore() : base() {}

        public PlanChore(ChoreDTO choreDTO, List<Object> objects) : base(choreDTO, objects) { }
    }

    public class RepetitiveChore : Chore
    {
        public TimeSpan DeltaTime { get; set; }
        public RepetitiveChore() : base() { }

        public RepetitiveChore(ChoreDTO choreDTO, List<Object> objects) : base(choreDTO, objects)
        {
            DeltaTime = new TimeSpan(choreDTO.Days, choreDTO.Hours, 0, 0);
        }
    }

    public enum ChoreType
    {
        Repetitive,
        Plan
    }
}
