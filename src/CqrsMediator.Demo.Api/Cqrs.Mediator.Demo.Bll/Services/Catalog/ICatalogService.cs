using System.Collections.Generic;

using CqrsMediator.Demo.Dal.Entities;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface ICatalogService
    {
        public Product GetProduct(int productId);
        internal int ChangeProductStock(int productId, int stockChange);
    }
}
