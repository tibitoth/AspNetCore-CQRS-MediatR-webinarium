using System.Collections.Generic;

using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

namespace CqrsMediator.Demo.Bll.Services
{
    public interface IOrderService
    {
        public List<Dal.Entities.Order> FindOrders(OrderStatus? status);
        public Dal.Entities.Order GetOrder(int orderId);
    }
}
