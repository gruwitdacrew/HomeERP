using System.ComponentModel.DataAnnotations.Schema;
using HomeERP.Domain.Common.Models;
using HomeERP.Domain.EAV.Models;

namespace HomeERP.Domain.EAV.Models
{
    public abstract class AttributeValue
    {
        public Guid ObjectId { get; set; }
        public Object Object { get; set; }

        public Guid AttributeId { get; set; }
        public Attribute Attribute { get; set; }

        public DateTime? ChangeDate { get; set; }
        public User User { get; set; }
        public bool IsCurrent { get; set; }

        protected AttributeValue() { }
        protected AttributeValue(Object Object, Attribute Attribute, User user)
        {
            this.Object = Object;
            this.Attribute = Attribute;
            ChangeDate = DateTime.UtcNow;
            User = user;
            IsCurrent = true;
        }
    }

    public class IntegerAttributeValue : AttributeValue
    {
        public int? Value { get; set; }

        public IntegerAttributeValue() { }

        public IntegerAttributeValue(int? Value, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.Value = Value;
        }
    }

    public class StringAttributeValue : AttributeValue
    {
        public string? Value { get; set; }

        public StringAttributeValue() { }
        public StringAttributeValue(string? Value, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.Value = Value;
        }
    }

    public class DateAttributeValue : AttributeValue
    {
        public DateTime? Value { get; set; }

        public DateAttributeValue() { }

        public DateAttributeValue(DateTime? Value, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.Value = Value;
        }
    }

    public class LinkAttributeValue : AttributeValue
    {
        public Guid? Value { get; set; }

        public LinkAttributeValue() { }

        public LinkAttributeValue(Guid? Value, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.Value = Value;
        }
    }

    public class FileAttributeValue : AttributeValue
    {
        public Guid? FileId { get; set; }
        public string? ContentType { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        public FileAttributeValue() { }

        public FileAttributeValue(Guid? FileId, IFormFile? File, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.FileId = FileId;
            this.File = File;
        }
    }

    public class FloatAttributeValue : AttributeValue
    {

        public float? Value { get; set; }

        public FloatAttributeValue() { }

        public FloatAttributeValue(float? Value, Object Object, Attribute Attribute, User user) : base(Object, Attribute, user)
        {
            this.Value = Value;
        }
    }
}
