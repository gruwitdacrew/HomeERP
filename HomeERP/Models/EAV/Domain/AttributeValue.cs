using System.ComponentModel.DataAnnotations.Schema;

namespace HomeERP.Models.EAV.Domain
{
    public abstract class AttributeValue
    {
        public Guid ObjectId { get; set; }
        public Object Object { get; set; }

        public Guid AttributeId { get; set; }
        public Attribute Attribute { get; set; }

        protected AttributeValue() { }
        protected AttributeValue(Object Object, Attribute Attribute)
        {
            this.Object = Object;
            this.Attribute = Attribute;
        }
    }

    public class IntegerAttributeValue : AttributeValue
    {
        public int? Value { get; set; }

        public IntegerAttributeValue() { }

        public IntegerAttributeValue(int? Value, Object Object, Attribute Attribute) : base(Object, Attribute)
        {
            this.Value = Value;
        }
    }

    public class StringAttributeValue : AttributeValue
    {
        public string? Value { get; set; }

        public StringAttributeValue() { }
        public StringAttributeValue(string? Value, Object Object, Attribute Attribute) : base(Object, Attribute)
        {
            this.Value = Value;
        }
    }

    public class DateAttributeValue : AttributeValue
    {
        public DateTime? Value { get; set; }

        public DateAttributeValue() { }

        public DateAttributeValue(DateTime? Value, Object Object, Attribute Attribute) : base(Object, Attribute)
        {
            this.Value = Value;
        }
    }

    public class LinkAttributeValue : AttributeValue
    {
        public Guid? Value { get; set; }

        public LinkAttributeValue() { }

        public LinkAttributeValue(Guid? Value, Object Object, Attribute Attribute) : base(Object, Attribute)
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

        public FileAttributeValue(Guid? FileId, IFormFile? File, Object Object, Attribute Attribute) : base(Object, Attribute)
        {
            this.FileId = FileId;
            this.File = File;
        }
    }

    public class FloatAttributeValue : AttributeValue
    {

        public float? Value { get; set; }

        public FloatAttributeValue() { }

        public FloatAttributeValue(float? Value, Object Object, Attribute Attribute) : base(Object, Attribute)
        {
            this.Value = Value;
        }
    }
}
