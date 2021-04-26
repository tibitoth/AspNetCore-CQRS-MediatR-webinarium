using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<Order> CreateOrderAsync(string customerName, string customerAddress, Dictionary<int, int> productAmounts)
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

            await _dbContext.SaveChangesAsync();

            foreach (var item in order.OrederItems)
            {
                await _catalogService.ChangeProductStockAsync(item.ProductId, -item.Amount);
            }

            return order;
        }

        public async Task<List<Order>> FindOrdersAsync(OrderStatus? status)
        {
            return await _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .Where(o => status == null || o.Status == status)
                .ToListAsync();
        }

        public async Task<Order> GetOrderAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .SingleOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
