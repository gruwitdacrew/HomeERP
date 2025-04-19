using HomeERP.Models.Chore.Domain;

namespace HomeERP.Models.Chore.DTOs
{
    public class ChoreDTO
    {
        public string Name { get; set; }
        public int DeltaTimeInDays { get; set; }
        public WarningType WarningType { get; set; }
    }
}
