namespace HomeERP.Models.EAV.Domain
{
    public class Object
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Entity Entity { get; set; }

        public List<AttributeValue> AttributeValues { get; set; }

        public Object()
        {

        }

        public Object(string Name, Entity Entity)
        {
            Id = Guid.NewGuid();
            this.Name = Name;
            this.Entity = Entity;
            this.AttributeValues = new List<AttributeValue>();
        }
    }
}
