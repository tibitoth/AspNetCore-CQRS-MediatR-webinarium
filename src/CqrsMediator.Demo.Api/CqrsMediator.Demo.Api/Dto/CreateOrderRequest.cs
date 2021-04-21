using System.Collections.Generic;

namespace CqrsMediator.Demo.Api.Dto
{
    public class CreateOrderRequest
    {
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }

        public List<CreateOrderItem> OrderItems { get; set; }
    }
}
