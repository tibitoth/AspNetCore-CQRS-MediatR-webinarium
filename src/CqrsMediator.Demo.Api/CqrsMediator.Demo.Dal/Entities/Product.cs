namespace CqrsMediator.Demo.Dal.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
    }
}
