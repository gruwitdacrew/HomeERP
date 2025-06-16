namespace HomeERP.Views.Product.DTOs.Request
{
    public class CreateBundleRequest
    {
        public string BundleName { get; set; }
        public List<ProductInCollectionDTO> Products { get; set; }
    }

    public class ProductInCollectionDTO
    {
        public Guid ProductId { get; set; }
        public int ProductAmount { get; set; }
    }
}
