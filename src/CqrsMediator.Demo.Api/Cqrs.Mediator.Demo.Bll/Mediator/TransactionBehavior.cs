using System.Threading;
using System.Threading.Tasks;

using CqrsMediator.Demo.Dal;

using MediatR;

namespace CqrsMediator.Demo.Bll.Mediator
{
    public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IBaseRequest
    {
        private readonly AppDbContext _dbContext;

        public TransactionBehavior(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            if (!(request is ICommand<TResponse>))
            {
                return await next();
            }

            using var tran = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await next();
                await tran.CommitAsync();
                return result;
            }
            catch (System.Exception)
            {
                await tran.RollbackAsync();
                throw;
            }
        }
    }
}
