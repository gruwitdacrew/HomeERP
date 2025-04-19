using HomeERP.Models.Chore.DTOs;
using HomeERP.Models.EAV.Domain;
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
        public IActionResult GetChores()
        {
            return PartialView(_choreService.GetChores());
        }

        public IActionResult CreateChore(Guid AttributeId, ChoreDTO ChoreDTO)
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateChore()
        {
            return PartialView(_choreService.GetEntities());
        }
    }
}
