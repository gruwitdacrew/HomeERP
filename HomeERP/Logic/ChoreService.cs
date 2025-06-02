using HomeERP.Domain.Chores.Models;
using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Models;
using HomeERP.Domain.Common.Repositories;
using HomeERP.Domain.EAV.Models;
using Microsoft.EntityFrameworkCore;
using Object = HomeERP.Domain.EAV.Models.Object;
using Task = HomeERP.Domain.Chores.Models.Task;

namespace HomeERP.Services
{
    public class ChoreService
    {
        private readonly BaseEntityRepository<Object> _objectRepo;
        private readonly BaseEntityRepository<Entity> _entitiesRepo;
        private readonly BaseEntityRepository<Chore> _choreRepo;
        private readonly BaseEntityRepository<Task> _taskRepo;
        public ChoreService(BaseEntityRepository<Object> objectRepo, BaseEntityRepository<Entity> entityRepo, BaseEntityRepository<Chore> choreRepo, BaseEntityRepository<Task> taskRepo)
        {
            _objectRepo = objectRepo;
            _entitiesRepo = entityRepo;
            _choreRepo = choreRepo;
            _taskRepo = taskRepo;
        }

        public List<Entity> GetEntities()
        {
            List<Entity> Entities = _entitiesRepo.Query().Include(entity => entity.Objects).ToList();
            return Entities;
        }

        public List<Chore> GetChores()
        {
            return _choreRepo.Query().ToList();
        }

        public List<Object> GetObjects(List<Guid> objectIds)
        {
            return _objectRepo.Query().Where(Object => objectIds.Contains(Object.Id)).ToList();
        }

        public Chore GetChoreTasks(Guid ChoreId, User user)
        {
            Chore Chore = _choreRepo.GetBy(ChoreId);

            _taskRepo.Query().Where(task => task.Chore == Chore && task.IsDone == false && (task.User == user || task.User == null)).Include(task => task.Object).Include(task => task.User).Load();

            return Chore;
        }

        public void CreateChore(Chore Chore)
        {
            _choreRepo.Add(Chore);
        }

        public Chore GetChore(Guid ChoreId)
        {
            return _choreRepo.GetBy(ChoreId);
        }

        public Chore GetChoreJournal(Guid ChoreId)
        {
            Chore chore = _choreRepo.GetBy(ChoreId);

            _taskRepo.Query().Where(task => task.Chore.Id == ChoreId && task.IsDone).Include(task => task.Object).Include(task => task.User).OrderBy(task => task.ExecutionMoment).Load();

            return chore;
        }

        public void DoChore(Chore Chore, List<Guid> TaskIds, DateTime? NewDate, TimeSpan? trackedTime, User user)
        {
            List<Task> tasks = _taskRepo.Query().Where(task => TaskIds.Contains(task.Id)).Include(task => task.Object).ToList();
            foreach (Task task in tasks)
            {
                task.ExecutionMoment = DateTime.UtcNow;
                task.TrackedTime = trackedTime;
                task.IsDone = true;
                task.User = user;
            }

            DateTime NewExecutionTime = Chore is RepetitiveChore repetitiveChore ? DateTime.UtcNow.Add(repetitiveChore.DeltaTime) : ((DateTime)NewDate).ToUniversalTime();

            List<Task> newTasks = tasks.Select(task => new Task(task.Chore, task.Object, task.User, NewExecutionTime)).ToList();
            _taskRepo.UpdateRange(tasks);
            _taskRepo.AddRange(newTasks);
        }

        public void DeleteChore(Chore Chore)
        {
            _choreRepo.Delete(Chore);
        }

        public Task GetTask(Guid taskId)
        {
            return _taskRepo.Query().Where(task => task.Id == taskId).Include(task => task.Chore).Include(task => task.User).FirstOrDefault();
        }

        public void AssignUserToTask(Task task, User? user)
        {
            task.User = user;
            _taskRepo.Update(task);
        }
    }
}
