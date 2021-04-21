using System.Collections.Generic;

using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface IOrderService
    {
        public List<Order> FindOrders(OrderStatus? status);
        public Order GetOrder(int orderId);
        public Order CreateOrder(string customerName, string customerAddress, Dictionary<int, int> productAmounts);
    }
}
