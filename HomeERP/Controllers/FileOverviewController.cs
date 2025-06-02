using HomeERP.Domain.Common.Repositories;
using HomeERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace HomeERP.Controllers
{
    public class FileOverviewController : Controller
    {
        private readonly FileOverviewService _fileOverviewService;

        public FileOverviewController(FileOverviewService fileOverviewService)
        {
            _fileOverviewService = fileOverviewService;
        }
        public async Task<IActionResult> Overview(Guid FileId)
        {
            return View(_fileOverviewService.GetEntities());
        }

        public IActionResult GetObjectFiles(Guid ObjectId)
        {
            return PartialView("ObjectFiles", _fileOverviewService.GetFileAttributeValues(ObjectId));
        }

        public async Task<IActionResult> DownloadFile(Guid FileId)
        {
            return await _fileOverviewService.DownloadFile(FileId);
        }
    }
}
