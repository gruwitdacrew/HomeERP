using HomeERP.Domain.Common.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Object = HomeERP.Domain.EAV.Models.Object;
namespace HomeERP.Domain.Product.Models
{
    public class ProductInCollection
    {
        public Object Product { get; set; }
        public ProductCollection ProductCollection { get; set; }
        public int Amount { get; set; }

        public ProductInCollection() {}

        public ProductInCollection(Object product, int amount)
        {
            Product = product;
            Amount = amount;
        }
    }
}
