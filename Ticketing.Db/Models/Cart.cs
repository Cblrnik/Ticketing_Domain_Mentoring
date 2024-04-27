namespace Ticketing.Db.Models
{
    public class Cart
    {
        public Guid Id { get; set; }

        public List<OrderDetails> OrderDetails;

        public decimal Amount { get; set; }

        public Cart()
        {
            OrderDetails = new List<OrderDetails>();
        }
    }
}
