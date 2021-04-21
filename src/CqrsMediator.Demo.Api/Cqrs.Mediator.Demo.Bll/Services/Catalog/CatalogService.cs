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

        public Product GetProduct(int productId)
        {
            return _dbContext.Products.Find(productId);
        }        
    }
}
