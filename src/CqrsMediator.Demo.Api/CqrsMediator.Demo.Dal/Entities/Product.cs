using System.Collections.Generic;

namespace CqrsMediator.Demo.Dal.Entities
{
    public class Product
    {
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Stock { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
