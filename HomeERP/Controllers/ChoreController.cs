using HomeERP.Models.Chore.DTOs;
using HomeERP.Models.Chore.Domain;
using HomeERP.Services;
using Microsoft.AspNetCore.Mvc;


namespace HomeERP.Controllers
{
    public class ChoreController : Controller
    {
        private readonly ChoreService _choreService;
        public ChoreController(ChoreService choreService)
        {
            _choreService = choreService;
        }
        public IActionResult ChoreExplorer(Guid? ChoreId)
        {
            TempData["Entities"] = _choreService.GetEntities();
            if (ChoreId != null) TempData["ChoreId"] = ChoreId;

            return View(_choreService.GetChores());
        }

        public IActionResult ChoreTasks(Guid ChoreId)
        {
            Chore Chore = _choreService.GetChoreTasks(ChoreId);
            return PartialView(Chore);
        }

        public IActionResult CreateChore(Guid AttributeId, ChoreDTO ChoreDTO)
        {
            Chore Chore = new Chore(ChoreDTO, _choreService.GetDateAttribute(AttributeId));
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
            _choreService.DoChore(Chore, Request.Tasks.Where(Task => Task.IsDone == "on").Select(Task => Task.ObjectId).ToList(), Request.NewDate);

            return RedirectToAction("ChoreExplorer", "Chore", new { ChoreId = Request.ChoreId });
        }
    }
}
