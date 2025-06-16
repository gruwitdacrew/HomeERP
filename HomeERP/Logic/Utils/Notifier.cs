using HomeERP.Domain.Chores.Models;
using HomeERP.Domain.Common.Contexts;
using HomeERP.Domain.Common.Repositories;
using Minio.DataModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Task = System.Threading.Tasks.Task;
using User = HomeERP.Domain.Common.Models.User;
using TaskEntity = HomeERP.Domain.Chores.Models.Task;
using Microsoft.EntityFrameworkCore;

namespace HomeERP.Logic.Utils
{
    public class Notifier : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;

        public Notifier(IServiceScopeFactory serviceScopeFactory) 
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(CheckForTasks, null, TimeSpan.Zero, TimeSpan.FromHours(1));
            return Task.CompletedTask;
        }

        private async void CheckForTasks(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var telegramBot = scope.ServiceProvider.GetRequiredService<TelegramBot>();
                var _taskRepo = scope.ServiceProvider.GetRequiredService<BaseEntityRepository<TaskEntity>>();

                var taskInGroupsByUsers = _taskRepo.Query().Where(task => task.IsDone == false && task.ExecutionMoment >= DateTime.UtcNow && task.ExecutionMoment <= DateTime.UtcNow + TimeSpan.FromHours(1)).Include(task => task.Chore).Include(task => task.Object).GroupBy(task => task.User);

                foreach (var group in taskInGroupsByUsers)
                {
                    if (group.Key?.ChatId != null)
                    {
                        string message = "Напоминание о заданиях:";

                        foreach (var choreTasks in group.GroupBy(task => task.Chore))
                        {
                            message += "\n\n"+choreTasks.Key.Name + ":\n" + string.Join("\n", choreTasks.Select(task => $"- {task.Object.Name}"));
                        }

                        await telegramBot.SendMessage((long)group.Key.ChatId, message);
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
