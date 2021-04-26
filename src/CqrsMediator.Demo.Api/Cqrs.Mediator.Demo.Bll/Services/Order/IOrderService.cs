using System.Collections.Generic;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal.Enum;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface IOrderService
    {
        public Task<List<Dal.Entities.Order>> FindOrdersAsync(OrderStatus? status);
        public Task<Dal.Entities.Order> GetOrderAsync(int orderId);
    }
}
