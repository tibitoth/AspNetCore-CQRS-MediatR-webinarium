using System.Collections.Generic;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal.Entities;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface ICatalogService
    {
        public Task<List<Product>> FindProductsAsync(string name, string description);
        public Task<Product> GetProductAsync(int productId);
        public Task<Product> CreateProductAsync(string name, string description, decimal unitPrice);

        internal Task<int> ChangeProductStockAsync(int productId, int stockChange);
    }
}
