﻿namespace CqrsMediator.Demo.Api.Dto
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public decimal UnitPrice { get; set; }
        public int Amount { get; set; }

        public string ProductId { get; set; }
        public string ProductName { get; set; }
    }
}
