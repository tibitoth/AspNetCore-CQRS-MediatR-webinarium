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

        public Order CreateOrder(string customerName, string customerAddress, Dictionary<int, int> productAmounts)
        {
            var order = new Order
            {
                Name = customerName,
                Address = customerAddress,
                OrderTime = DateTimeOffset.UtcNow,
                Status = OrderStatus.Active,
                OrederItems = productAmounts.Select(kvp => new OrderItem
                {
                    ProductId = kvp.Key,
                    Amount = kvp.Value,
                }).ToList(),
            };

            _dbContext.Add(order);

            _dbContext.SaveChanges();

            foreach (var item in order.OrederItems)
            {
                _catalogService.ChangeProductStock(item.ProductId, -item.Amount);
            }

            return order;
        }

        public List<Order> FindOrders(OrderStatus? status)
        {
            return _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .Where(o => status == null || o.Status == status)
                .ToList();
        }

        public Order GetOrder(int orderId)
        {
            return _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .SingleOrDefault(o => o.OrderId == orderId);
        }
    }
}
