using System.Collections.Generic;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal.Entities;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface ICatalogService
    {
        public Task<Product> GetProductAsync(int productId);
    }
}
