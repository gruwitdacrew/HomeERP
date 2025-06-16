using HomeERP.Domain.Common.Repositories;
using HomeERP.Domain.EAV.Models;
using HomeERP.Domain.Product.Models;
using Microsoft.EntityFrameworkCore;
using Object = HomeERP.Domain.EAV.Models.Object;

namespace HomeERP.Logic
{
    public class ProductService
    {
        private readonly BaseEntityRepository<ProductCollection> _productCollectionRepository;
        private readonly GenericRepository<ProductInCollection> _productInCollectionRepository;
        private readonly BaseEntityRepository<Entity> _entityRepository;
        private readonly BaseEntityRepository<Object> _objectRepository;
        public ProductService(BaseEntityRepository<ProductCollection> productRepository, GenericRepository<ProductInCollection> productInCollectionRepository, BaseEntityRepository<Entity> entityRepository, BaseEntityRepository<Object> objectRepository)
        {
            _productCollectionRepository = productRepository;
            _productInCollectionRepository = productInCollectionRepository;
            _entityRepository = entityRepository;
            _objectRepository = objectRepository;
        }

        public List<ProductBundle> GetBundles()
        {
            return _productCollectionRepository.Query().OfType<ProductBundle>().ToList();
        }
        public Entity GetProducts()
        {
            return _entityRepository.Query().Where(entity => entity.Id == new Guid("e2603327-15ec-4b67-96f0-be16467a9dbf")).Include(entity => entity.Objects).First();
        }

        public Inventory GetInventory()
        {
            Inventory inventory = _productCollectionRepository.Query().OfType<Inventory>().First();

            _productInCollectionRepository.Query().Where(productInCollection => productInCollection.ProductCollection == inventory).Include(productInCollection => productInCollection.Product).Load();

            return inventory;
        }

        public ProductCollection GetProductCollection(Guid collectionId)
        {
            ProductCollection productCollection = _productCollectionRepository.GetBy(collectionId);

            _productInCollectionRepository.Query().Where(productInCollection => productInCollection.ProductCollection == productCollection).Include(productCollection => productCollection.Product).Load();

            return productCollection;
        }

        public Object GetProduct(Guid productId)
        {
            return _objectRepository.GetBy(productId);
        }

        public void AddProductToInventory(Object product)
        {
            Inventory inventory = _productCollectionRepository.Query().OfType<Inventory>().First();
            inventory.Products.Add(new ProductInCollection(product, 0));

            _productCollectionRepository.Update(inventory);
        }

        public void CreateBundle(ProductBundle bundle)
        {
            _productCollectionRepository.Add(bundle);
        }

        public void UseBundle(ProductBundle bundle)
        {
            Inventory inventory = _productCollectionRepository.Query().OfType<Inventory>().First();
            _productInCollectionRepository.Query().Where(productInCollection => productInCollection.ProductCollection == inventory).Include(productInCollection => productInCollection.Product).Load();

            foreach (var bundleProduct in bundle.Products)
            {
                inventory.Products.First(product => product.Product.Id == bundleProduct.Product.Id).Amount -= bundleProduct.Amount;
            }

            _productCollectionRepository.Update(inventory);
        }

        public void DeleteBundle(ProductBundle bundle)
        {
            _productCollectionRepository.Delete(bundle);
        }

        public void AddBundleToShoppingList(ProductBundle bundle)
        {
            ShoppingList shoppingList = _productCollectionRepository.Query().OfType<ShoppingList>().First();
            _productInCollectionRepository.Query().Where(productInCollection => productInCollection.ProductCollection == shoppingList).Include(productInCollection => productInCollection.Product).Load();

            foreach (var bundleProduct in bundle.Products)
            {
                ProductInCollection? product = shoppingList.Products.FirstOrDefault(product => product.Product.Id == bundleProduct.Product.Id);
                if (product == null)
                {
                    product = new ProductInCollection(bundleProduct.Product, bundleProduct.Amount);
                    shoppingList.Products.Add(product);
                }
                else
                {
                    product.Amount += bundleProduct.Amount;
                }    
            }

            _productCollectionRepository.Update(shoppingList);
        }

        public void UpdateShoppingList(ShoppingList list)
        {
            _productCollectionRepository.Update(list);
        }

        public void UpdateProductCollection(ProductCollection collection)
        {
            _productCollectionRepository.Update(collection);
        }
    }
}
