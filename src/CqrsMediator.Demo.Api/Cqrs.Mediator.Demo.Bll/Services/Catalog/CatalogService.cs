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

        public async Task<Product> GetProductAsync(int productId)
        {
            return await _dbContext.Products.FindAsync(productId);
        }
    }
}
