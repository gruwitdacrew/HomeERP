using HomeERP.Models.EAV.Domain;
using HomeERP.Models.FileOverview.DTOs.Responses;
using HomeERP.Services;
using HomeERP.Services.Utils.FileService;
using Microsoft.AspNetCore.Mvc;

namespace HomeERP.Controllers
{
    public class FileOverviewController : Controller
    {
        private readonly FileOverviewService _fileOverviewService;
        private readonly FileService _fileService;

        public FileOverviewController(FileOverviewService fileOverviewService, FileService fileService)
        {
            _fileOverviewService = fileOverviewService;
            _fileService = fileService;
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
            return await _fileService.Get(FileId);
        }
    }
}
