using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;

using MediatR;

namespace CqrsMediator.Demo.Bll.Catalog.Commands
{
    public static class CreateProduct
    {
        public class Command : IRequest<Product>
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal UnitPrice { get; set; }
        }

        public class Handler : IRequestHandler<Command, Product>
        {
            private readonly AppDbContext _dbContext;

            public Handler(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Product> Handle(Command request, CancellationToken cancellationToken)
            {
                var p = new Product
                {
                    Name = request.Name,
                    Description = request.Description,
                    UnitPrice = request.UnitPrice,
                    Stock = 10,
                };

                _dbContext.Products.Add(p);

                await _dbContext.SaveChangesAsync();

                return p;
            }
        }
    }
}
