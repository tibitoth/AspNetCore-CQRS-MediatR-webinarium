using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;

using Microsoft.EntityFrameworkCore;

namespace CqrsMediator.Demo.Bll.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly AppDbContext _dbContext;

        public CatalogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product> CreateProductAsync(string name, string description, decimal unitPrice)
        {
            var p = new Product
            {
                Name = name,
                Description = description,
                UnitPrice = unitPrice,
                Stock = 10,
            };

            _dbContext.Products.Add(p);

            await _dbContext.SaveChangesAsync();

            return p;
        }

        public async Task<List<Product>> FindProductsAsync(string name, string description)
        {
            return await _dbContext.Products
                .Where(p => name == null || p.Name.Contains(name))
                .Where(p => description == null || p.Description.Contains(description))
                .ToListAsync();
        }

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }

        async Task<int> ICatalogService.ChangeProductStockAsync(int productId, int stockChange)
        {
            var p = await GetProductAsync(productId);
            p.Stock += stockChange;

            _dbContext.SaveChanges();

            return p.Stock;
        }
    }
}
