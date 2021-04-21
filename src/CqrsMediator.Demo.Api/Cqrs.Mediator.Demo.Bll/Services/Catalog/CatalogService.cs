using System.Collections.Generic;
using System.Linq;

using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;

namespace CqrsMediator.Demo.Bll.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly AppDbContext _dbContext;

        public CatalogService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Product CreateProduct(string name, string description, decimal unitPrice)
        {
            var p = new Product
            {
                Name = name,
                Description = description,
                UnitPrice = unitPrice,
                Stock = 10,
            };

            _dbContext.Products.Add(p);

            _dbContext.SaveChanges();

            return p;
        }

        public Product GetProduct(int productId)
        {
            return _dbContext.Products.Find(productId);
        }

        int ICatalogService.ChangeProductStock(int productId, int stockChange)
        {
            var p = GetProduct(productId);
            p.Stock += stockChange;

            _dbContext.SaveChanges();

            return p.Stock;
        }
    }
}
