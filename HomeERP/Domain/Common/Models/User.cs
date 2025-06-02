namespace HomeERP.Domain.Common.Models
{
    public class User : BaseEntity
    {
        public string Name { get; set; }

        public User() { }

        public User(string userName)
        {
            Name = userName;
        }
    }
}
