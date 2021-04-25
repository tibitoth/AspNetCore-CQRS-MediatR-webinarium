using System;
using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Bll.Order.Events;
using CqrsMediator.Demo.Bll.Services;
using CqrsMediator.Demo.Dal;

using MediatR;

namespace CqrsMediator.Demo.Bll.Catalog.EventHandlers
{
    public class OrderCreatedEventHandler : INotificationHandler<OrderCreatedEvent>
    {
        private readonly ICatalogService _catalogService;
        private readonly AppDbContext _dbContext;

        public OrderCreatedEventHandler(ICatalogService catalogService, AppDbContext dbContext)
        {
            _catalogService = catalogService;
            _dbContext = dbContext;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var item in notification.Order.OrederItems)
            {
                await ChangeProductStockAsync(item.ProductId, -item.Amount);
            }
        }

        private async Task<int> ChangeProductStockAsync(int productId, int stockChange)
        {
            var p = _catalogService.GetProduct(productId);

            var newStock = p.Stock + stockChange;
            if (newStock < 0)
            {
                throw new InvalidOperationException("Not enough stock.");
            }

            p.Stock = newStock;
            await _dbContext.SaveChangesAsync();

            return p.Stock;
        }
    }
}
