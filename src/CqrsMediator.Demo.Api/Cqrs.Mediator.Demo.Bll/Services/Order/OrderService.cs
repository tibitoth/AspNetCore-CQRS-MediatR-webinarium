using System;
using System.Collections.Generic;
using System.Linq;

using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

using Microsoft.EntityFrameworkCore;

namespace CqrsMediator.Demo.Bll.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _dbContext;
        private readonly ICatalogService _catalogService;

        public OrderService(AppDbContext dbContext, ICatalogService catalogService)
        {
            _dbContext = dbContext;
            _catalogService = catalogService;
        }

        public List<Dal.Entities.Order> FindOrders(OrderStatus? status)
        {
            return _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .Where(o => status == null || o.Status == status)
                .ToList();
        }

        public Dal.Entities.Order GetOrder(int orderId)
        {
            return _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .SingleOrDefault(o => o.OrderId == orderId);
        }
    }
}
