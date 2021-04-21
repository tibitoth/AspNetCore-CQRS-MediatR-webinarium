using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Bll.Services;
using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

using MediatR;

namespace CqrsMediator.Demo.Bll.Order.Commands
{
    public static class CreateOrder
    {
        public class Command : IRequest<Dal.Entities.Order>
        {
            public string CustomerName { get; set; }
            public string CustomerAddress { get; set; }

            public List<CreateOrderItem> OrderItems { get; set; }

            public class CreateOrderItem
            {
                public int ProductId { get; set; }
                public int Amount { get; set; }
            }
        }

        public class Handler : IRequestHandler<Command, Dal.Entities.Order>
        {
            private readonly AppDbContext _dbContext;
            private readonly ICatalogService _catalogService;

            public Handler(AppDbContext dbContext, ICatalogService catalogService)
            {
                _dbContext = dbContext;
                _catalogService = catalogService;
            }

            public async Task<Dal.Entities.Order> Handle(Command request, CancellationToken cancellationToken)
            {
                var order = new Dal.Entities.Order
                {
                    Name = request.CustomerName,
                    Address = request.CustomerAddress,
                    OrderTime = DateTimeOffset.UtcNow,
                    Status = OrderStatus.Active,
                    OrederItems = request.OrderItems.Select(oi => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Amount = oi.Amount,
                    }).ToList(),
                };

                _dbContext.Add(order);

                await _dbContext.SaveChangesAsync();

                foreach (var item in order.OrederItems)
                {
                    _catalogService.ChangeProductStock(item.ProductId, -item.Amount);
                }

                return order;
            }
        }
    }
}
