using Object = HomeERP.Domain.EAV.Models.Object;
using Attribute = HomeERP.Domain.EAV.Models.Attribute;
using HomeERP.Domain.Common.Models;

namespace HomeERP.Domain.Chores.Models
{
    public class Task : BaseEntity
    {
        public Chore Chore { get; set; }

        public Object Object { get; set; }
        public User? User { get; set; }

        public DateTime? ExecutionMoment { get; set; }
        public TimeSpan? TrackedTime { get; set; }
        public bool IsDone { get; set; }

        public Task() {}

        public Task(Chore chore, Object Object)
        {
            Chore = chore;
            this.Object = Object;
            IsDone = false;
        }

        public Task(Chore chore, Object Object, User user, DateTime executionTime) : this(chore, Object)
        {
            User = user;
            ExecutionMoment = executionTime;
        }
    }
}
