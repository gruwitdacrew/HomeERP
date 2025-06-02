using HomeERP.Domain.EAV.Models;
namespace HomeERP.Views.FileOverview.DTOs.Responses
{
    public class FileOverviewCollectionResponse
    {
        public List<Entity> Entities { get; set; }

        public Guid FileId { get; set; }

        public string FileType { get; set; }

        public FileOverviewCollectionResponse(List<Entity> Entities, Guid FileId, string FileType)
        {
            this.Entities = Entities;
            this.FileId = FileId;
            this.FileType = FileType;
        }
    }
}
