namespace CqrsMediator.Demo.Dal.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }

        public decimal UnitPrice { get; set; }
        public int Amount { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
