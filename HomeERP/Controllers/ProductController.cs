using HomeERP.Domain.EAV.Models;
using HomeERP.Domain.Product.Models;
using HomeERP.Logic;
using HomeERP.Views.Product.DTOs.Request;
using Microsoft.AspNetCore.Mvc;
using Object = HomeERP.Domain.EAV.Models.Object;

namespace HomeERP.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService _productService;
        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        public IActionResult Inventory()
        {
            TempData["Bundles"] = _productService.GetBundles();
            TempData["ProductsEntity"] = _productService.GetProducts();
            TempData["ShoppingList"] = _productService.GetProductCollection(new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf")) as ShoppingList;

            return View(_productService.GetInventory());
        }

        public IActionResult Bundle(Guid BundleId)
        {
            return PartialView(_productService.GetProductCollection(BundleId) as ProductBundle);
        }

        public IActionResult EditShoppingList(CreateBundleRequest createRequest)
        {
            Entity products = _productService.GetProducts();
            List<ProductInCollection> productsInCollection = createRequest.Products.GroupBy(productInCollectionDTO => productInCollectionDTO.ProductId).Select(group => new ProductInCollection(products.Objects.First(product => product.Id == group.Key), group.Sum(x => x.ProductAmount))).ToList();

            ShoppingList list = _productService.GetProductCollection(new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf")) as ShoppingList;
            list.Products = productsInCollection;

            _productService.UpdateShoppingList(list);

            return RedirectToAction("Inventory");
        }

        public IActionResult BuyProductsFromShoppingList()
        {
            Inventory inventory = _productService.GetInventory();
            ShoppingList list = _productService.GetProductCollection(new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf")) as ShoppingList;

            foreach (var productInCollection in list.Products)
            {
                inventory.Products.First(product => product.Product == productInCollection.Product).Amount += productInCollection.Amount;
            }

            _productService.UpdateProductCollection(inventory);

            list.Products = new List<ProductInCollection>();   
            _productService.UpdateProductCollection(list);

            return RedirectToAction("Inventory");
        }

        public IActionResult AddProductToShoppingList(Guid productId)
        {
            ShoppingList list = _productService.GetProductCollection(new Guid("c2703327-15ec-4b67-96f0-be16467a9dbf")) as ShoppingList;

            ProductInCollection? product = list.Products.FirstOrDefault(product => product.Product.Id == productId);
            if (product == null)
            {
                product = new ProductInCollection(_productService.GetProduct(productId), 1);
                list.Products.Add(product);
            }
            else
            {
                product.Amount += 1;
            }

            return RedirectToAction("Inventory");
        }

        public IActionResult CreateBundle(CreateBundleRequest createRequest)
        {
            Entity products = _productService.GetProducts();
            List<ProductInCollection> productsInCollection = createRequest.Products.GroupBy(productInCollectionDTO => productInCollectionDTO.ProductId).Select(group => new ProductInCollection(products.Objects.First(product => product.Id == group.Key), group.Sum(x => x.ProductAmount))).ToList();
            ProductBundle bundle = new ProductBundle(createRequest.BundleName, productsInCollection);

            _productService.CreateBundle(bundle);

            return RedirectToAction("Inventory");
        }

        public IActionResult UseBundle(Guid BundleId)
        {
            ProductBundle bundle = _productService.GetProductCollection(BundleId) as ProductBundle;
            _productService.UseBundle(bundle);

            return RedirectToAction("Inventory");
        }

        public IActionResult DeleteBundle(Guid BundleId)
        {
            ProductBundle bundle = _productService.GetProductCollection(BundleId) as ProductBundle;
            _productService.DeleteBundle(bundle);

            return RedirectToAction("Inventory");
        }

        public IActionResult AddBundleToShoppingList(Guid BundleId)
        {
            ProductBundle bundle = _productService.GetProductCollection(BundleId) as ProductBundle;
            _productService.AddBundleToShoppingList(bundle);

            return RedirectToAction("Inventory");
        }
    }
}
