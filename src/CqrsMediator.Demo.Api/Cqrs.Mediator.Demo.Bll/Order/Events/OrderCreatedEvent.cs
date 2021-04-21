using MediatR;

namespace CqrsMediator.Demo.Bll.Order.Events
{
    public class OrderCreatedEvent : INotification
    {
        public Dal.Entities.Order Order { get; set; }
    }
}
