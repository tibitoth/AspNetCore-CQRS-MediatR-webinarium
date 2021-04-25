using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Bll.Mediator;
using CqrsMediator.Demo.Bll.Order.Events;
using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;
using CqrsMediator.Demo.Dal.Enum;

using MediatR;

namespace CqrsMediator.Demo.Bll.Order.Commands
{
    public static class CreateOrder
    {
        public class Command : ICommand<Dal.Entities.Order>
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
            private readonly IMediator _mediator;

            public Handler(AppDbContext dbContext, IMediator mediator)
            {
                _dbContext = dbContext;
                _mediator = mediator;
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

                await _mediator.Publish(new OrderCreatedEvent() { Order = order });

                return order;
            }
        }
    }
}
