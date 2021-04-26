using System.Collections.Generic;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface IOrderService
    {
        public Task<List<Order>> FindOrdersAsync(OrderStatus? status);
        public Task<Order> GetOrderAsync(int orderId);
        public Task<Order> CreateOrderAsync(string customerName, string customerAddress, Dictionary<int, int> productAmounts);
    }
}
