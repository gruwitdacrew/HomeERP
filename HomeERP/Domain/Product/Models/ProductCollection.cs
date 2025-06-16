using HomeERP.Domain.Common.Models;

namespace HomeERP.Domain.Product.Models
{
    public abstract class ProductCollection : BaseEntity
    {
        public string Name { get; set; }
        public List<ProductInCollection> Products { get; set; } = new List<ProductInCollection>();

        protected ProductCollection() {}

        protected ProductCollection(string name)
        {
            Name = name;   
        }
    }

    public class Inventory : ProductCollection
    {
        public Inventory() : base()
        {
            Name = "Инвентарь";
        }
    }

    public class ShoppingList : ProductCollection
    {
        public ShoppingList() : base()
        {
            Name = "Список покупок";
        }
    }
    public class ProductBundle : ProductCollection
    {
        public ProductBundle() : base() { }
        public ProductBundle(string name, List<ProductInCollection> products) : base(name)
        {
            Products = products;
        }
    }
}
