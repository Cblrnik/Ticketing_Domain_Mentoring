namespace Ticketing.Db.Models
{
    public class TicketPriceLevel : IIdentity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public decimal Price { get; set; }
    }
}
