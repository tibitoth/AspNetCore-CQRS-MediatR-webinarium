namespace CqrsMediator.Demo.Api.Dto
{
    public class Product
    {
        public int ProductId { get; set; }
        public int Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }
    }
}
