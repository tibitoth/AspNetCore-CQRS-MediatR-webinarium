using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal;
using CqrsMediator.Demo.Dal.Entities;

using MediatR;

namespace CqrsMediator.Demo.Bll.Catalog.Queries
{
    public static class FindProduct
    {
        public class Query : IRequest<List<Product>>
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class Hander : IRequestHandler<Query, List<Product>>
        {
            private readonly AppDbContext _dbContext;

            public Hander(AppDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public Task<List<Product>> Handle(Query request, CancellationToken cancellationToken)
            {
                return Task.FromResult(_dbContext.Products
                    .Where(p => request.Name == null || p.Name.Contains(request.Name))
                    .Where(p => request.Description == null || p.Description.Contains(request.Description))
                    .ToList());
            }
        }
    }
}
