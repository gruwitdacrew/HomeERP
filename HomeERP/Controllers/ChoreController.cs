using HomeERP.Services;
using Microsoft.AspNetCore.Mvc;
using HomeERP.Views.Chores.DTOs;
using HomeERP.Domain.Chores.Models;
using Object = HomeERP.Domain.EAV.Models.Object;
using HomeERP.Domain.Common.Models;
using Task = HomeERP.Domain.Chores.Models.Task;


namespace HomeERP.Controllers
{
    public class ChoreController : Controller
    {
        private readonly ChoreService _choreService;
        private readonly EAVService _entityService;
        public ChoreController(ChoreService choreService, EAVService entityService)
        {
            _choreService = choreService;
            _entityService = entityService;
        }
        public IActionResult ChoreExplorer(Guid? choreId)
        {
            TempData["Entities"] = _choreService.GetEntities();
            if (choreId != null) TempData["ChoreId"] = choreId;

            return View(_choreService.GetChores());
        }

        public IActionResult ChoreTasks(Guid choreId)
        {
            ViewData["Users"] = _entityService.GetUsers();
            Chore Chore = _choreService.GetChoreTasks(choreId, _entityService.GetCurrentUser());
            return PartialView(Chore);
        }

        public IActionResult CreateChore(ChoreDTO choreDTO)
        {
            Chore Chore;
            List<Object> objects = _choreService.GetObjects(choreDTO.ObjectIds);

            if (choreDTO.ChoreType == ChoreType.Repetitive)
            {
                Chore = new RepetitiveChore(choreDTO, objects);
            }
            else
            {
                Chore = new PlanChore(choreDTO, objects);
            }
            _choreService.CreateChore(Chore);

            return RedirectToAction("ChoreExplorer", "Chore", new { ChoreId = Chore.Id });
        }

        public IActionResult DeleteChore(Guid ChoreId)
        {
            Chore Chore = _choreService.GetChore(ChoreId);
            _choreService.DeleteChore(Chore);

            return RedirectToAction("ChoreExplorer", "Chore");
        }

        public IActionResult DoChore(DoChoreRequest Request)
        {
            Chore Chore = _choreService.GetChore(Request.ChoreId);
            User user = _entityService.GetCurrentUser();

            _choreService.DoChore(Chore, Request.TaskIds, Request.NewDate, Request.TrackedTime, user);

            return RedirectToAction("ChoreExplorer", "Chore", new { ChoreId = Request.ChoreId });
        }

        public IActionResult AssignUserToTask(Guid taskId, Guid userId)
        {
            Task task = _choreService.GetTask(taskId);
            User? user = _entityService.GetUser(userId);
            _choreService.AssignUserToTask(task, user);

            return RedirectToAction("ChoreExplorer", "Chore", new { ChoreId = task.Chore.Id });
        }

        public IActionResult ChoreJournal(Guid choreId)
        {
            Chore chore = _choreService.GetChoreJournal(choreId);

            return PartialView(chore);
        }
    }
}
