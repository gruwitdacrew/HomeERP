using HomeERP.Models.Chore.DTOs;
using Attribute = HomeERP.Models.EAV.Domain.Attribute;

namespace HomeERP.Models.Chore.Domain
{
    public class Chore
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int DeltaTimeInDays { get; set; }
        public WarningType WarningType { get; set; }


        public Attribute Attribute { get; set; }

        public Chore() {}

        public Chore(ChoreDTO ChoreDTO, Attribute Attribute)
        {
            Id = Guid.NewGuid();
            Name = ChoreDTO.Name;
            DeltaTimeInDays = ChoreDTO.DeltaTimeInDays;
            WarningType = ChoreDTO.WarningType;
            this.Attribute = Attribute;
        }
    }

    public enum WarningType
    {
        Before,
        After
    }
}
