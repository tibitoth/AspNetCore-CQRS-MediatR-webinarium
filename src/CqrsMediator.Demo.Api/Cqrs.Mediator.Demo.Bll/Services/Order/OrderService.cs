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

        public async Task<List<Dal.Entities.Order>> FindOrdersAsync(OrderStatus? status)
        {
            return await _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .Where(o => status == null || o.Status == status)
                .ToListAsync();
        }

        public async Task<Dal.Entities.Order> GetOrderAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.OrederItems)
                    .ThenInclude(o => o.Product)
                .SingleOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}
