using HomeERP.Controllers;
using static HomeERP.Controllers.EntityController;

namespace HomeERP.Models.Domain
{
    public abstract class AttributeValue
    {
        public Guid ObjectId { get; set; }
        public Object Object { get; set; }

        public Guid AttributeId { get; set; }
        public Attribute Attribute { get; set; }

        protected AttributeValue() {}
        protected AttributeValue(AttributeValueDTO AttributeValueRaw, Object Object, Attribute Attribute)
        {
            this.Object = Object;
            this.Attribute = Attribute;
        }
    }

    public class IntegerAttributeValue : AttributeValue
    {
        public int Value { get; set; }

        public IntegerAttributeValue() {}

        public IntegerAttributeValue(AttributeValueDTO AttributeValueRaw, Object Object, Attribute Attribute) : base(AttributeValueRaw, Object, Attribute)
        {
            Value = int.Parse(AttributeValueRaw.AttributeValue);
        }
    }

    public class StringAttributeValue : AttributeValue
    {
        public string Value { get; set; }

        public StringAttributeValue() {}
        public StringAttributeValue(AttributeValueDTO AttributeValueRaw, Object Object, Attribute Attribute) : base(AttributeValueRaw, Object, Attribute)
        {
            Value = AttributeValueRaw.AttributeValue;
        }
    }

    public class DateAttributeValue : AttributeValue
    {
        public DateTime Value { get; set; }

        public DateAttributeValue() {}

        public DateAttributeValue(AttributeValueDTO AttributeValueRaw, Object Object, Attribute Attribute) : base(AttributeValueRaw, Object, Attribute)
        {
            Value = DateTime.Parse(AttributeValueRaw.AttributeValue).ToUniversalTime();
        }
    }

    public class LinkAttributeValue : AttributeValue
    {
        public Guid Value { get; set; }

        public LinkAttributeValue() {}

        public LinkAttributeValue(AttributeValueDTO AttributeValueRaw, Object Object, Attribute Attribute) : base(AttributeValueRaw, Object, Attribute)
        {
            Value = Guid.Parse(AttributeValueRaw.AttributeValue);
        }
    }
}
