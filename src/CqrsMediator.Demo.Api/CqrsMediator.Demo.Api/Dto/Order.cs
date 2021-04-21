using System;
using System.Collections.Generic;

using CqrsMediator.Demo.Dal.Enum;

namespace CqrsMediator.Demo.Api.Dto
{
    public class Order
    {
        public int OrderId { get; set; }

        public DateTimeOffset OrderTime { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> OrederItems { get; set; }
    }
}
