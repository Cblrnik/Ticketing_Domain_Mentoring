namespace Ticketing.Db.Models
{
    public class Order
    {
        public int Id { get; set; }

        public Customer? Customer { get; set; }

        public Payment? Payment { get; set; }

        public IEnumerable<Ticket>? Tickets { get; set; }
    }
}
